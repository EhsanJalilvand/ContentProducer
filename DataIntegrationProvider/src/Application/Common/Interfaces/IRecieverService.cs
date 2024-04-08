using DataIntegrationProvider.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataIntegrationProvider.Domain.ConfigEntities;
using Quartz;

namespace DataIntegrationProvider.Application.Application.Common.Interfaces
{
    public interface IRecieverService: IJob
    {
        PlanningInfoId PlanningInfoId { get;  }
        Task Run(PlanningInfo planningInfo);
    }
}
