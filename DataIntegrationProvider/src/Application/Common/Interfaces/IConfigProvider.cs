using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataIntegrationProvider.Application.Application.Common.Configurations;
using DataIntegrationProvider.Domain.ConfigEntities;
using DataIntegrationProvider.Domain.Enums;

namespace DataIntegrationProvider.Application.Application.Common.Interfaces
{
    public interface IConfigProvider
    {
        //void UpdateConfig();
        //List<DataConfig> DataConfigs { get; }
        ServiceInfo GetLastSerivceInfoDetail(long SerivceInfoDetailId);
        string ParseUrl(ServiceInfo serviceInfo, out long? dataParentRef);
        string GetResponseKeyFromResponse(string response, string keyParameter);
        Task UpdateResponseKey(long serivceInfoDetailId, string responseKey);
        Task UpdateUrlPath(long serivceInfoDetailId, string responseKey);
        Task UpdateRelatedInfo(long ResponseDataId, string filterParameter,bool dataNotExists);
        Task<string> GetApiKey(ServiceInfoCategoryId serviceInfoCategoryId);
        Task UpdateApiKey(ServiceInfoCategoryId serviceInfoCategoryId, string apiKey);


    }
}
