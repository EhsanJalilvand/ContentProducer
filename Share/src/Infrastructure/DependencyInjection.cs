using Ganss.Xss;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Refit;
using Share.Infrastructure.SecurityMiddlewares;

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






    }
   
}