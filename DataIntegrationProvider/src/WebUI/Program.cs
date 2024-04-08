
using DataIntegrationProvider.Application;
using DataIntegrationProvider.Application.Application.Common.Interfaces;
using DataIntegrationProvider.Domain.Enums;
using DataIntegrationProvider.Infrastructure;
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
using Marten;
using EnigmaDataProvider.Infrastructure.Persistence;

namespace DataIntegrationProvider.WebUI
{
    public class Program
    {
        public async static Task Main(string[] args)
        {
            TseLog.Addlog();
            var host = CreateHostBuilder(args).Build();

            //var scope = host.Services.CreateScope();
            //var services = scope.ServiceProvider;

            await host.RunAsync();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {



            return Host.CreateDefaultBuilder(args)
            //.UseWindowsService()
            .ConfigureServices(async services =>
            {

                var builder = new ConfigurationBuilder().AddJsonFile("appsettings.json");
                var Configuration = builder.Build();


                services.Configure<IEnumerable<PlanningInfoId>>(Configuration.GetSection("PlanTypes"));
                services.AddOptions();
                services.AddInfrastructure(Configuration);
                var config = services.AddMarten(options =>
                      {
                          options.Connection(Configuration.GetConnectionString("DefaultConnection"));
                          options.CreateDatabasesForTenants(c =>
                              c.ForTenant()
              .CheckAgainstPgDatabase()
              .WithOwner("postgres")
              .WithEncoding("UTF-8")
              .ConnectionLimit(-1)

              );
                      });
                services.AddJobs();
            }).UseTselog();
        }
    }
}
