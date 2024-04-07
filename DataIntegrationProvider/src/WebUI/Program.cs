
using DataIntegrationProvider.Application;
using DataIntegrationProvider.Application.Application.Common.Interfaces;
using DataIntegrationProvider.Application.Application.ContextMaps.Contents.Commands;
using DataIntegrationProvider.Domain.Enums;
using DataIntegrationProvider.Infrastructure;
using DataIntegrationProvider.Infrastructure.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Quartz;
using Share.Infrastructure.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataIntegrationProvider.WebUI
{
    public class Program
    {
        public async static Task Main(string[] args)
        {
            TseLog.Addlog();
            var host = CreateHostBuilder(args).Build();

            var scope = host.Services.CreateScope();
            var services = scope.ServiceProvider;



            var configDbContext = services.GetRequiredService<ConfigDbContext>();
            configDbContext.Database.Migrate();

            var codalDbContext = services.GetRequiredService<CodalDbContext>();
            codalDbContext.Database.Migrate();

            await host.RunAsync();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
   


            return Host.CreateDefaultBuilder(args)
                //.UseWindowsService()
                .ConfigureServices(async services => {

                    var builder = new ConfigurationBuilder().AddJsonFile("appsettings.json");
                    var Configuration = builder.Build();
                    services.AddOptions();
                    services.AddInfrastructure(Configuration);
                    services.AddJobs(Configuration);
                }).UseTselog();
        }
    }
}
