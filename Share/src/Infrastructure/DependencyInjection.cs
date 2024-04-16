using Ganss.Xss;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Refit;
using Serilog;
using Serilog.Events;
using Share.Infrastructure.SecurityMiddlewares;
using SharedDomainService.Interfaces;
using SharedInfrastructure.Services;
using StackExchange.Redis;
using StackExchange.Redis.Extensions.Core.Configuration;
using StackExchange.Redis.Extensions.MsgPack;
using System;

namespace Share.Infrastructure
{
    public static class DependencyInjection
    {

        public static IServiceCollection AddHtmlSanitizer(this IServiceCollection services)
        {
            var htmlSanitizer = new HtmlSanitizer();
            var styleFormatter = new SemicolonStyleFormatter();
            htmlSanitizer.StyleFormatter = styleFormatter;
            services.AddSingleton<IHtmlSanitizer>(_ => htmlSanitizer);


            return services;
        }
        public static IServiceCollection AddSharedInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {

            services.AddSingleton<ICrawlServerHandler, CrawlServerHandler>();
            services.AddSingleton<ICrawlClientHandler, CrawlClientHandler>();
            return services;
        }
        public static IHostBuilder AddLogger(this IHostBuilder app)
        {

            app.UseSerilog((ctx, lc) => lc
       //.WriteTo.File("./Logs/log.txt", rollingInterval: RollingInterval.Day)
       .WriteTo.Console()
       .MinimumLevel.Information()
       .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
       );
            return app;
        }





    }
   
}