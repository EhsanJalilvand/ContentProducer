using DataIntegrationProvider.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataIntegrationProvider.Application.Application.Common.Configurations;
using DataIntegrationProvider.Domain.ConfigEntities;
using Quartz;

namespace DataIntegrationProvider.Application.Application.Common.Interfaces
{
    public interface IRecieverService: IJob
    {
        ServiceInfoCategoryId ServiceInfoCategoryId { get;  }
        Task Run(ServiceInfo dataConfigItem, Action<long> ProcessStartedCallback, Action<long> processEndedCallback);
    }
}
