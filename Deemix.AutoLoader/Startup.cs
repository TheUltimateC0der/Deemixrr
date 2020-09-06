
using System;

using AutoMapper;

using Deemix.AutoLoader.Configuration;
using Deemix.AutoLoader.Data;
using Deemix.AutoLoader.Jobs.RecurringJobs;
using Deemix.AutoLoader.Repositories;
using Deemix.AutoLoader.Services;

using Hangfire;

using HangfireBasicAuthenticationFilter;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Deemix.AutoLoader
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

            var connectionString = Configuration.GetConnectionString("DefaultConnection");

            // Config
            var hangfireConfiguration = new HangFireConfiguration();
            Configuration.Bind("Hangfire", hangfireConfiguration);
            services.AddSingleton(hangfireConfiguration);

            var deezerApiConfiguration = new DeezerApiConfiguration();
            Configuration.Bind("DeezerApi", deezerApiConfiguration);
            services.AddSingleton(deezerApiConfiguration);

            //Hangfire
            services.AddHangfire(x =>
                x.UseSqlServerStorage(connectionString)
            );

            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(connectionString)
            );
            services.AddDefaultIdentity<User>(options =>
            {
                options.User.AllowedUserNameCharacters = null;
            }).AddEntityFrameworkStores<AppDbContext>();

            //Services
            services.AddSingleton<IDeezerApiService, DeezerApiService>();
            services.AddSingleton<IDeemixService, DeemixService>();
            services.AddScoped<IDataRepository, DataRepository>();

            services.AddAutoMapper(typeof(Startup));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IServiceProvider serviceProvider, HangFireConfiguration hangFireConfiguration)
        {
            //ReverseProxy Fix
            app.UseForwardedHeaders();
            app.Use((context, next) =>
            {
                context.Request.Scheme = "https";
                return next();
            });

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            InitializeHangfire(app, serviceProvider, hangFireConfiguration);

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





        //Hangfire Initialization
        private void InitializeHangfire(IApplicationBuilder applicationBuilder, IServiceProvider serviceProvider, HangFireConfiguration hangFireConfiguration)
        {
            GlobalConfiguration.Configuration
                .UseActivator(new HangfireActivator(serviceProvider));

            applicationBuilder.UseHangfireServer(new BackgroundJobServerOptions
            {
                WorkerCount = hangFireConfiguration.Workers
            });

            applicationBuilder.UseHangfireDashboard(hangFireConfiguration.DashboardPath ?? "/jobs", new DashboardOptions
            {
                Authorization = new[] { new HangfireCustomBasicAuthenticationFilter
                {
                    User = hangFireConfiguration.Username ?? "Admin",
                    Pass = hangFireConfiguration.Password ?? "SuperSecurePWD!123"
                } }
            });

            RecurringJob.AddOrUpdate<FavoriteArtistsRecurringJob>(x => x.Execute(), Cron.Never);
            RecurringJob.AddOrUpdate<ScrapeGenreRecurringJob>(x => x.Execute(), Cron.Never);
            //RecurringJob.AddOrUpdate<GetShowCertificationsRecurringJob>(x => x.Execute(), Cron.Daily);

            //Starting all jobs here for initial db fill
            //foreach (var recurringJob in JobStorage.Current.GetConnection().GetRecurringJobs())
            //{
            //    RecurringJob.Trigger(recurringJob.Id);
            //}
        }

    }
}
