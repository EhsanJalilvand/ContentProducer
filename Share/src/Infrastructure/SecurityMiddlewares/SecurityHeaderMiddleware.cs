using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Share.Infrastructure.SecurityMiddlewares
{
    public class SecurityHeaderMiddleware
    {
        private readonly RequestDelegate _next;
        public SecurityHeaderMiddleware(RequestDelegate next)
        {
            _next = next;
        }
        public async Task Invoke(HttpContext context)
        {
            //context.Response.OnStarting(() =>
            //{
            //        if (context.Response.Headers.ContainsKey("X-Powered-By"))
            //        {
            //            context.Response.Headers.Remove("X-Powered-By");
            //        }

            //    return Task.FromResult(0);
            //});

            context.Response.Headers.Add("x-xss-protection", new StringValues("1; mode=block"));
            context.Response.Headers.Add("X-Frame-Options", "DENY");
            context.Response.Headers.Add("X-Content-Type-Option", "nosniff");

            context.Response.Headers.Add("Cache-Control", "must-revalidate, max-age=0, s-maxage=0");
            context.Response.Headers.Add("Content-Security-Policy", "script-src 'self'; " + "style-src 'self'; " + "img-src 'self'");

     


            //if (context.Request.Path.HasValue && !context.Request.Path.Value.Contains(@"/docs/index"))
            //    context.Response.Headers.Add("Content-Security-Policy", "script-src 'self'; " + "style-src 'self'; " + "img-src 'self'");
            await _next(context);
     
        }
    }
}
