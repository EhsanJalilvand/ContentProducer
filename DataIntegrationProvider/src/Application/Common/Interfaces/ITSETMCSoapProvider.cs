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
        List<T> GetData<T>(ServiceInfo serviceInfo, string url,params int[] flow) where T:class;
    }
}
