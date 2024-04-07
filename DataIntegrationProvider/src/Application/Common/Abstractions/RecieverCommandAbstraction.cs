using DataIntegrationProvider.Domain.Enums;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DataIntegrationProvider.Application.Application.Common.Configurations;
using DataIntegrationProvider.Application.Application.Common.Interfaces;
using DataIntegrationProvider.Application.Common.Interfaces;
using DataIntegrationProvider.Domain.ConfigEntities;
using Quartz;
using Share.Domain.Extensions;

namespace DataIntegrationProvider.Application.Application.Common.Abstractions
{
    public abstract class RecieverCommandAbstraction<T> : IRecieverService where T : class
    {
        private readonly IConfigProvider _configProvider;
        private readonly ServiceConfig _serviceConfig;
        private readonly ILogger<RecieverCommandAbstraction<T>> _logger;
        private readonly IConfigDbContext _configDbContext;
        private string _serviceInfoLog;
        private static bool _flag = false;
        public RecieverCommandAbstraction(IConfigProvider configProvider, ILogger<RecieverCommandAbstraction<T>> logger, IOptionsMonitor<ServiceConfig> serviceConfig, IConfigDbContext configDbContext)
        {
            _configProvider = configProvider;
            _logger = logger;
            if (serviceConfig != null)
                _serviceConfig = serviceConfig.CurrentValue;
            _configDbContext = configDbContext;
        }
        protected IConfigProvider ConfigProvider { get { return _configProvider; } }
        protected string ServiceInfoLog { get { return _serviceInfoLog; } }
        protected IConfigDbContext ConfigDbContext { get { return _configDbContext; } }
        public abstract ServiceInfoCategoryId ServiceInfoCategoryId { get; }
        protected abstract Task<List<T>> GetData(ServiceInfo serviceInfo, string urlPath);
        protected abstract Task<CodalResponseData> SaveData(List<T> response, ServiceInfo detail, long? dataParentRef);
        protected abstract Task<bool> DeleteData(List<T> response, ServiceInfo detail, long? dataParentRef);
        protected abstract void Dispose();
        public async Task Run(ServiceInfo detail, Action<long> ProcessStartedCallback, Action<long> ProcessEndedCallback)
        {
            if (_flag)
                return;
            _flag = true;
            List<T> allData = null;
            var dtime = DateTime.Now;
            try
            {
                await Task.Delay(200);
                detail = _configDbContext.ServiceInfoS.FirstOrDefault(p => p.ServiceInfoId == detail.ServiceInfoId);

                if (ProcessStartedCallback != null)
                    ProcessStartedCallback(detail.ServiceInfoId);
                _serviceInfoLog = $"ServiceInfoCategory: {ServiceInfoCategoryId.GetDisplayName()} ServiceInfoTypeName: {detail.ServiceInfoTypeName} InstanceName: {_serviceConfig.InstanceName} SerivceInfoDetailId :{detail.ServiceInfoId}";
                _logger.LogInformation($"Start Call Api For Recieve Data {ServiceInfoLog}");


                if (detail == null)
                {
                    _logger.LogWarning($"SerivceInfoDetail For Id:{detail.ServiceInfoId}  Is Null");
                    return;
                }

                _logger.LogInformation($"SetUrl KeyParameter:{detail.UrlPath}  ResponseKey: {detail.ResponseKey} Data {ServiceInfoLog}");
                long? dataParentRef = null;
                var url = _configProvider.ParseUrl(detail, out dataParentRef);
                if (string.IsNullOrEmpty(url))
                {
                    _logger.LogWarning($"Exists Not Value For Related Key In Url :{url}  Data {ServiceInfoLog}");
                    return;
                }
                _logger.LogInformation($"Start GetData With url :{url}  Data {ServiceInfoLog}");
                try
                {
                    allData = await GetData(detail, url);
                    if (allData == null || !allData.Any())
                    {
                        _logger.LogWarning($"Null Data Response in Calling Data {ServiceInfoLog}");
                        if (dataParentRef.HasValue)
                            await _configProvider.UpdateRelatedInfo(dataParentRef.Value, detail.FilterParameter, true);
                        return;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogCritical(ex, $"Error Raised In Get Data {ServiceInfoLog}");
                    return;
                }



                try
                {
                    if (detail.CanDelete)
                    {
                        _logger.LogWarning($"Start Delete OldData For Data {ServiceInfoLog}");
                        await DeleteData(allData, detail, dataParentRef);
                    }
                    _logger.LogInformation($"Start Save Response {allData.Count} Count For Data {ServiceInfoLog}");
                    await SaveData(allData, detail, dataParentRef);

                    _logger.LogInformation($" {allData.Count} Count Saved Successfully For Data {ServiceInfoLog}");
                }
                catch (Exception ex)
                {
                    _logger.LogCritical(ex, $"Exception Raised in Saving Data {ServiceInfoLog}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, $"Exception Raised in Calling Data {ServiceInfoLog}");
            }
            finally
            {
                var dtime2 = DateTime.Now;
                _logger.LogInformation($"ProceeId {detail.ServiceInfoId} Run In {dtime2.Subtract(dtime).TotalSeconds} Sec");
                await Task.Delay(detail.Interval * 1000);
                if (ProcessEndedCallback != null)
                    ProcessEndedCallback(detail.ServiceInfoId);
                if (allData != null)
                    allData.Clear();
                allData = null;
                _configDbContext.Dispose();
                Dispose();
                detail = null;
                _flag = false;
            }
        }

        public async Task Execute(IJobExecutionContext context)
        {
            var serviceInfo = context.JobDetail.JobDataMap.Get("serviceInfo");
            if (serviceInfo == null)
                return;
            var info = (ServiceInfo)serviceInfo;

            await Run(info, null, null);
        }

    }
}
