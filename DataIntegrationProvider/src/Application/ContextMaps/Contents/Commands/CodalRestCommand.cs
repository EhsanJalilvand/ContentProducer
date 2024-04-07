//using AutoMapper;
//using DataIntegrationProvider.Application.Common.Interfaces;
//using DataIntegrationProvider.Domain.Enums;
//using Microsoft.Extensions.Logging;
//using Microsoft.Extensions.Options;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using DataIntegrationProvider.Application.Application.Common.Abstractions;
//using DataIntegrationProvider.Application.Application.Common.Configurations;
//using DataIntegrationProvider.Application.Application.Common.Interfaces;
//using Share.Application.Common.Interfaces;
//using Newtonsoft.Json;
//using DataIntegrationProvider.Domain.ConfigEntities;
//using Share.Domain.Extensions;
//using System.Threading;
//using Quartz;

//namespace DataIntegrationProvider.Application.Application.ContextMaps.Contents.Commands
//{
//    public class CodalRestCommand : RecieverCommandAbstraction<string>
//    {
//        private readonly ICodalRestApi _CodalRestApi;
//        private readonly ISecurityService _securityService;
//        private readonly ILogger<RecieverCommandAbstraction<string>> _logger;
//        private readonly ICodalDbContext _codalDbContext;
//        public CodalRestCommand(IConfigProvider configProvider, ILogger<RecieverCommandAbstraction<string>> logger, IOptionsMonitor<ServiceConfig> serviceConfig, ICodalRestApi codalRestApi, ISecurityService securityService, IConfigDbContext configDbContext, ICodalDbContext codalDbContext) : base(configProvider, logger, serviceConfig, securityService, configDbContext)
//        {
//            _CodalRestApi = codalRestApi;
//            _securityService = securityService;
//            _logger = logger;
//            _codalDbContext = codalDbContext;
//        }

//        public override ServiceInfoCategoryId ServiceInfoCategoryId => ServiceInfoCategoryId.Codal;



//        protected override async Task<bool> DeleteData(List<string> response, ServiceInfo detail, long? dataParentRef)
//        {
//            var hashBody = _securityService.Hash(response);

//            var allOldData = _codalDbContext.CodalResponseDatas.Where(w => w.SerivceInfoId == detail.ServiceInfoId && w.BodyHash == hashBody && w.IsDeleted == false).ToList();
//            foreach (var item in allOldData)
//            {
//                item.IsDeleted = true;
//            }
//            await _codalDbContext.SaveChangesAsync(CancellationToken.None);
//            return true;
//        }

//        protected override void Dispose()
//        {
//            _codalDbContext.Dispose();
//        }

//        protected override async Task<List<string>> GetData(ServiceInfo serviceInfo, string urlPath)
//        {
//            await _CodalRestApi.SetAPIKey(ServiceInfoCategoryId);
//            var allData = await _CodalRestApi.Get(urlPath);


//            if (string.IsNullOrEmpty(allData) || allData.Trim() == "[]")
//            {
//                return null;
//            }
//            allData = allData.Trim();



//            List<object> responseList = new List<object>();
//            if (allData.StartsWith("[") && allData.EndsWith("]"))
//                responseList = JsonConvert.DeserializeObject<List<object>>(allData);
//            else
//                responseList.Add(allData);


//            return responseList.Select(s => s.ToString()).ToList();
//        }

//        protected async override Task<CodalResponseData> SaveData( List<string> responseList, ServiceInfo serviceInfo, long? dataParentRef)
//        {
//            string lastData = string.Empty;

//            foreach (var response in responseList)
//            {
//                lastData = response.ToString();

//                var result = StoreData( response.ToString(), serviceInfo, dataParentRef);
//                if (result == null)
//                {
//                    continue;
//                }

//                _logger.LogInformation($"Data Successfully Saved  In {ServiceInfoLog}");
//                _logger.LogInformation($"Start Update ResponseKey {ServiceInfoLog}");
//                await ConfigProvider.UpdateResponseKey(serviceInfo.ServiceInfoId, result.ResponseKey);
//                if (dataParentRef.HasValue)
//                    await ConfigProvider.UpdateRelatedInfo(dataParentRef.Value, serviceInfo.FilterParameter, false);
//            }


//            _logger.LogInformation($"Start Update UpdateUrlPath {ServiceInfoLog}");
//            if (!string.IsNullOrEmpty(lastData))
//                await ConfigProvider.UpdateUrlPath(serviceInfo.ServiceInfoId, lastData);

//            return null;
//        }


//        private CodalResponseData StoreData( string response, ServiceInfo detail, long? dataParentRef)
//        {
//            var hashBody = _securityService.Hash(response);
//            var key = ConfigProvider.GetResponseKeyFromResponse(response, detail.KeyParameter);
//            if (_codalDbContext.CodalResponseDatas.Any(a => !string.IsNullOrEmpty(a.ResponseKey) && a.ResponseKey == key && a.SerivceInfoId == detail.ServiceInfoId))
//                return null;
//            var responseData = new CodalResponseData();
//            responseData.InsertTime = DateTimeHelper.GetCurrentDateTime();
//            responseData.IsDeleted = false;
//            responseData.ResponseKey = "";
//            responseData.SerivceInfoId = detail.ServiceInfoId;
//            responseData.Body = response;
//            responseData.BodyHash = hashBody;
//            responseData.ResponseKey = key;

//            if (detail.ParentRef.HasValue)
//                responseData.ParentRef = dataParentRef;

//            _codalDbContext.CodalResponseDatas.Add(responseData);

//            var aa = _codalDbContext.SaveChangesAsync(CancellationToken.None).GetAwaiter().GetResult();
//            return responseData;
//        }

//    }
//}
