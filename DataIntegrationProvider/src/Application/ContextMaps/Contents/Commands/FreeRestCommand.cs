using AutoMapper;
using DataIntegrationProvider.Application.Common.Interfaces;
using DataIntegrationProvider.Domain.Enums;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataIntegrationProvider.Application.Application.Common.Abstractions;
using DataIntegrationProvider.Application.Application.Common.Configurations;
using DataIntegrationProvider.Application.Application.Common.Interfaces;
using DataIntegrationProvider.Domain.ConfigEntities;
using Newtonsoft.Json;

namespace DataIntegrationProvider.Application.Application.ContextMaps.Contents.Commands
{
    public class FreeRestCommand : RecieverCommandAbstraction<string>
    {
        private readonly IFreeRestApi _freeRestApi;
        private readonly ILogger _logger;
        public FreeRestCommand(IConfigProvider configProvider, ILogger<RecieverCommandAbstraction<string>> logger, IOptionsMonitor<ServiceConfig> serviceConfig, IFreeRestApi freeRestApi, IConfigDbContext configDbContext) : base(configProvider, logger, serviceConfig, configDbContext)
        {
            _freeRestApi = freeRestApi;
            _logger = logger;
        }

        public override ServiceInfoCategoryId ServiceInfoCategoryId => ServiceInfoCategoryId.FreeRest;

        protected override Task<bool> DeleteData(List<string> response, ServiceInfo detail, long? dataParentRef)
        {
            throw new NotImplementedException();
        }

        protected override void Dispose()
        {
            return;
        }

        protected override async Task<List<string>> GetData(ServiceInfo serviceInfo, string urlPath)
        {
            var allData = await _freeRestApi.Get(urlPath);


            if (string.IsNullOrEmpty(allData) || allData.Trim() == "[]")
            {
                return null;
            }
            allData = allData.Trim();


            if (!string.IsNullOrEmpty(serviceInfo.ResponseData))
            {
                var dicTemp = JsonConvert.DeserializeObject<Dictionary<string, object>>(allData);
                if (!dicTemp.ContainsKey(serviceInfo.ResponseData))
                {
                    _logger.LogWarning($"{serviceInfo.ResponseData} Not Exists In Response Data  In {ServiceInfoLog}");
                    return null;
                }
                allData = dicTemp[serviceInfo.ResponseData].ToString();
            }


            List<object> responseList = new List<object>();
            if (allData.StartsWith("[") && allData.EndsWith("]"))
                responseList = JsonConvert.DeserializeObject<List<object>>(allData);
            else
                responseList.Add(allData);


            return responseList.Cast<string>().ToList();
        }

        protected async override Task<CodalResponseData> SaveData( List<string> responseList, ServiceInfo serviceInfo, long? dataParentRef)
        {
            string lastData = string.Empty;

            foreach (var response in responseList)
            {
                lastData = response.ToString();

                var result = StoreData( response.ToString(), serviceInfo, dataParentRef);
                if (result == null)
                {
                    continue;
                }

                _logger.LogInformation($"Data Successfully Saved  In {ServiceInfoLog}");
                _logger.LogInformation($"Start Update ResponseKey {ServiceInfoLog}");
                await ConfigProvider.UpdateResponseKey(serviceInfo.ServiceInfoId, result.ResponseKey);
                if (dataParentRef.HasValue)
                    await ConfigProvider.UpdateRelatedInfo(dataParentRef.Value, serviceInfo.FilterParameter, false);
            }


            _logger.LogInformation($"Start Update UpdateUrlPath {ServiceInfoLog}");
            if (!string.IsNullOrEmpty(lastData))
                await ConfigProvider.UpdateUrlPath(serviceInfo.ServiceInfoId, lastData);

            return null;
        }


        private CodalResponseData StoreData( string response, ServiceInfo detail, long? dataParentRef)
        {
            throw new NotImplementedException();
            //var hashBody = _securityService.Hash(response);
            //if (dataConfigItem.CanDelete)
            //{
            //    var allOldData = ConfigDbContext.CodalResponseDatas.Where(w => w.SerivceInfoId == dataConfigItem.ServiceInfoId && w.BodyHash == hashBody && w.IsDeleted == false).ToList();
            //    foreach (var item in allOldData)
            //    {
            //        item.IsDeleted = true;
            //    }
            //}
            //var key = ConfigProvider.GetResponseKeyFromResponse(response, dataConfigItem.KeyParameter);
            //if (ConfigDbContext.CodalResponseDatas.Any(a => !string.IsNullOrEmpty(a.ResponseKey) && a.ResponseKey == key && a.SerivceInfoId == dataConfigItem.ServiceInfoId))
            //    return null;
            //var responseData = new CodalResponseData();
            //responseData.InsertTime = DateTimeHelper.GetCurrentDateTime();
            //responseData.IsDeleted = false;
            //responseData.ResponseKey = "";
            //responseData.SerivceInfoId = dataConfigItem.ServiceInfoId;
            //responseData.Body = response;
            //responseData.BodyHash = hashBody;
            //responseData.ResponseKey = key;

            //if (detail.ParentRef.HasValue)
            //    responseData.ParentRef = dataParentRef;

            //ConfigDbContext.CodalResponseDatas.Add(responseData);

            //ConfigDbContext.SaveChangesAsync(CancellationToken.None).GetAwaiter().GetResult();
            //return responseData;
        }



    }
}
