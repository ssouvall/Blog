using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RockwellBlog.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RockwellBlog
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            //Comment the CreateHostBuilder out and add the lines below in the method
            //CreateHostBuilder(args).Build().Run();
            var host = CreateHostBuilder(args).Build();
            
            //Add the dataService and call the ManageDataAsync() method
            var serviceProvider = host.Services.CreateScope().ServiceProvider;
            var dataService = serviceProvider.GetRequiredService<DataService>();
            await dataService.ManageDataAsync();

            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
