using Deemixrr.Configuration;
using Deemixrr.Data;
using Deemixrr.Jobs.RecurringJobs;
using Deemixrr.Repositories;
using Deemixrr.Services;

using Hangfire;
using Hangfire.Console;
using Hangfire.Dashboard.Dark;

using HangfireBasicAuthenticationFilter;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using System;

namespace Deemixrr
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }



        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();
            //ReverseProxy Fix https://docs.microsoft.com/de-de/aspnet/core/host-and-deploy/proxy-load-balancer?view=aspnetcore-3.1
            services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto | ForwardedHeaders.XForwardedHost;
            });

            var connectionString = Configuration.GetConnectionString("DefaultConnection");

            // Config
            var hangfireConfiguration = new HangFireConfiguration();
            Configuration.Bind("Hangfire", hangfireConfiguration);
            services.AddSingleton(hangfireConfiguration);

            var deezerApiConfiguration = new DeezerApiConfiguration();
            Configuration.Bind("DeezerApi", deezerApiConfiguration);
            services.AddSingleton(deezerApiConfiguration);

            var jobConfiguration = new JobConfiguration();
            Configuration.Bind("JobConfiguration", jobConfiguration);
            services.AddSingleton(jobConfiguration);

            var delayConfiguration = new DelayConfiguration();
            Configuration.Bind("DelayConfiguration", delayConfiguration);
            services.AddSingleton(delayConfiguration);

            //Hangfire
            services.AddHangfire(x =>
            {
                x.UseInMemoryStorage().WithJobExpirationTimeout(TimeSpan.FromDays(3));
                x.UseConsole();
            });

            services.AddDbContext<AppDbContext>(options =>
            {
                if (string.IsNullOrWhiteSpace(connectionString))
                {
                    options.UseSqlite("Data Source=deemixrr.db", x => x.MigrationsAssembly("Deemixrr.Data.Migrations.Sqlite"));
                }
                else
                {
                    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString), x => x.MigrationsAssembly("Deemixrr.Data.Migrations.Mysql"));
                }
            });

            services.AddDefaultIdentity<User>(options =>
            {
                options.User.AllowedUserNameCharacters = null;
            }).AddEntityFrameworkStores<AppDbContext>();

            //Services
            services.AddSingleton<IDeezerApiService, DeezerApiService>();
            services.AddSingleton<IDeemixService, DeemixService>();
            services.AddScoped<IDataRepository, DataRepository>();
            services.AddScoped<IConfigurationService, ConfigurationService>();

            services.AddAutoMapper(typeof(Startup));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IServiceProvider serviceProvider, HangFireConfiguration hangFireConfiguration, JobConfiguration jobConfiguration)
        {
            InitializeDatabase(app);

            //ReverseProxy Fix https://docs.microsoft.com/de-de/aspnet/core/host-and-deploy/proxy-load-balancer?view=aspnetcore-3.1
            app.UseForwardedHeaders();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            InitializeHangfire(app, serviceProvider, hangFireConfiguration, jobConfiguration);

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();

                endpoints.MapHangfireDashboard(new DashboardOptions
                {
                    Authorization = new[]
                    {
                        new HangfireCustomBasicAuthenticationFilter
                        {
                            User = hangFireConfiguration.Username ?? "Admin",
                            Pass = hangFireConfiguration.Password ?? "SuperSecurePWD!123"
                        }
                    }
                });
            });
        }

        private void InitializeDatabase(IApplicationBuilder app)
        {
            using var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope();

            serviceScope.ServiceProvider.GetRequiredService<AppDbContext>().Database.Migrate();
        }

        //Hangfire Initialization
        private void InitializeHangfire(IApplicationBuilder applicationBuilder, IServiceProvider serviceProvider, HangFireConfiguration hangFireConfiguration, JobConfiguration jobConfiguration)
        {
            GlobalConfiguration.Configuration.UseActivator(new HangfireActivator(serviceProvider));
            GlobalConfiguration.Configuration.UseDarkDashboard();

            applicationBuilder.UseHangfireServer(new BackgroundJobServerOptions
            {
                WorkerCount = hangFireConfiguration.Workers == 0 ? 1 : hangFireConfiguration.Workers
            });

            applicationBuilder.UseHangfireDashboard(hangFireConfiguration.DashboardPath ?? "/jobs", new DashboardOptions
            {
                Authorization = new[] { new HangfireCustomBasicAuthenticationFilter
                {
                    User = hangFireConfiguration.Username ?? "Admin",
                    Pass = hangFireConfiguration.Password ?? "SuperSecurePWD!123"
                } }
            });

            RecurringJob.AddOrUpdate<ScrapeGenreRecurringJob>(x => x.Execute(null), Cron.Daily);
            RecurringJob.AddOrUpdate<SizeCalculatorRecurringJob>(x => x.Execute(null), jobConfiguration.SizeCalculatorRecurringJob);
            RecurringJob.AddOrUpdate<GetUpdatesRecurringJob>(x => x.Execute(null), jobConfiguration.GetUpdatesRecurringJob);
        }

    }
}