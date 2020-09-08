using System;

using Deemixrr.Extensions;

using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace Deemixrr
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); })
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    var cfg = config.SetBasePath(Environment.CurrentDirectory)
                        .AddJsonFile("appsettings.Development.json", true, true)
                        .AddEnvironmentVariables()
                        .Build();

                    config.AddDbConfiguration(x => SqlServerDbContextOptionsExtensions.UseSqlServer(x, cfg.GetConnectionString("DefaultConnection")));
                });
    }
}