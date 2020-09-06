
using Microsoft.AspNetCore.Hosting;

[assembly: HostingStartup(typeof(Deemix.AutoLoader.Areas.Identity.IdentityHostingStartup))]
namespace Deemix.AutoLoader.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) =>
            {
                //services.AddDbContext<AppDbContext>(options =>
                //    options.UseSqlServer(
                //        context.Configuration.GetConnectionString("AppDbContextConnection")));

                //services.AddDefaultIdentity<User>(options => options.SignIn.RequireConfirmedAccount = true)
                //    .AddEntityFrameworkStores<AppDbContext>();
            });
        }
    }
}