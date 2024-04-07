
using DataIntegrationProvider.Application.Application.Common.Interfaces;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DataProvider.WebUI
{

    public class CachingConfigBackgroundServices : IHostedService
    {

        private readonly ILogger<CachingConfigBackgroundServices> logger;
        private readonly IServiceProvider _serviceProvider;

        public CachingConfigBackgroundServices(ILogger<CachingConfigBackgroundServices> logger, IServiceProvider serviceProvider)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _serviceProvider = serviceProvider;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            //logger.LogDebug($"Executing service [{nameof(CachingConfigBackgroundServices)}]");
            //while (true)
            //{
            //    try
            //    {
            //        using (var scope = _serviceProvider.CreateScope())
            //        {
            //            var configProvider = scope.ServiceProvider.GetService<IConfigProvider>();
            //            configProvider.UpdateConfig();
            //        }
            //    }
            //    catch (Exception ex)
            //    {
            //        logger.LogError(ex, ex.Message);
            //    }
            //    finally
            //    {
            //        await Task.Delay(TimeSpan.FromSeconds(30));
            //    }
            //}
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            logger.LogDebug($"Stopping service [{nameof(CachingConfigBackgroundServices)}]");
        }
    }
}