using DataIntegrationProvider.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataIntegrationProvider.Domain.ConfigEntities
{
    public class ServiceInfoCategory
    {
        public ServiceInfoCategory()
        {
            ServiceInfos=new List<ServiceInfo>();
        }
        public ServiceInfoCategoryId ServiceInfoCategoryId { get; set; }
        public string ServiceInfoCategoryName { get; set; }
        public string APIKey { get; set; }
        public ICollection<ServiceInfo> ServiceInfos { get; set; }
    }
}
