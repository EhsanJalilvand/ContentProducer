using DataIntegrationProvider.Domain.ConfigEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TSE.SiteAPI.Application.Common.Interfaces
{
    public interface ITSETMCSoapProvider
    {
        List<T> GetData<T>(PlanningInfo planningInfo, string url,params int[] flow) where T:class;
    }
}
