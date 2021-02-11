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
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using System;
using System.Threading.Tasks;

namespace Deemixrr
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }



        public void ConfigureServices(IServiceCollection services)
        {
            var connectionString = Configuration.GetConnectionString("DefaultConnection");

            services.AddSingleton(Configuration.GetSection("Hangfire").Get<HangFireConfiguration>());
            services.AddSingleton(Configuration.GetSection("JobConfiguration").Get<JobConfiguration>());
            services.AddSingleton(Configuration.GetSection("DelayConfiguration").Get<DelayConfiguration>());

            var loginConfiguration = new LoginConfiguration();
            Configuration.GetSection("LoginConfiguration").Bind(loginConfiguration);
            services.AddSingleton(loginConfiguration);

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
                options.SignIn.RequireConfirmedAccount = false;

                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequiredLength = 1;
                options.Password.RequiredUniqueChars = 1;
            }).AddEntityFrameworkStores<AppDbContext>();

            services.AddHangfire(x =>
            {
                x.UseInMemoryStorage().WithJobExpirationTimeout(TimeSpan.FromDays(3));
                x.UseConsole();
            });


            services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto | ForwardedHeaders.XForwardedHost;
            });


            //Services
            services.AddSingleton<IDeezerApiService, DeezerApiService>();
            services.AddSingleton<IDeemixService, DeemixService>();
            services.AddScoped<IDataRepository, DataRepository>();
            services.AddScoped<IConfigurationService, ConfigurationService>();

            services.AddAutoMapper(typeof(Startup));

            services.AddDatabaseDeveloperPageExceptionFilter();
            services.AddControllersWithViews();
        }

        public async void Configure(IApplicationBuilder app, IWebHostEnvironment env, IServiceProvider serviceProvider, HangFireConfiguration hangFireConfiguration, JobConfiguration jobConfiguration)
        {
            InitializeDatabase(app);
            InitializeHangfire(app, serviceProvider, hangFireConfiguration, jobConfiguration);
            await InitializeLogin(app);

            app.UseForwardedHeaders();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseMigrationsEndPoint();
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

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
            });
        }


        private void InitializeDatabase(IApplicationBuilder app)
        {
            using var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope();

            serviceScope.ServiceProvider.GetRequiredService<AppDbContext>().Database.Migrate();
        }

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

        private async Task InitializeLogin(IApplicationBuilder app)
        {
            using var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope();

            var loginConfig = serviceScope.ServiceProvider.GetRequiredService<LoginConfiguration>();

            if (!string.IsNullOrEmpty(loginConfig.Username) && !string.IsNullOrEmpty(loginConfig.Password))
            {
                var userManager = serviceScope.ServiceProvider.GetRequiredService<UserManager<User>>();
                var appDbContext = serviceScope.ServiceProvider.GetRequiredService<AppDbContext>();

                foreach (var identityUser in await appDbContext.Users.ToListAsync())
                {
                    appDbContext.Users.Remove(identityUser);
                }
                await appDbContext.SaveChangesAsync();

                await userManager.CreateAsync(new User { UserName = loginConfig.Username, EmailConfirmed = true }, loginConfig.Password);
            }
        }
    }
}
