using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SharedDomain.Configs;
using SharedDomainService.Interfaces;
using SharedInfrastructure.Services;
using System.Net;
using System.Net.Sockets;
using System.Text;
namespace BrowserForm
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {

            //Initialize Services
            var serviceCollection = new ServiceCollection()
                      .AddLogging();

    

            serviceCollection.AddSingleton<ICrawlServerHandler, CrawlServerHandler>();
            serviceCollection.AddSingleton<ICrawlClientHandler, CrawlClientHandler>();
            serviceCollection.AddTransient<SmartCrawlerForm>();


            var builder = new ConfigurationBuilder().AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            IConfiguration config = builder.Build();

            serviceCollection.Configure<SocketInfo>(config.GetSection("SocketInfo"));


            var serviceProvider= serviceCollection.BuildServiceProvider();




            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            var f= serviceProvider.GetService<SmartCrawlerForm>();
            Application.Run(f);


        }

    }

}