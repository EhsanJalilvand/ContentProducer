using DataIntegrationProvider.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataIntegrationProvider.Domain.ConfigEntities
{
    public class ServiceInfo
    {
        public ServiceInfo()
        {
            Childs = new Collection<ServiceInfo>();
        }
        public long ServiceInfoId { get; set; }
        public ServiceInfoCategoryId ServiceInfoCategoryId { get; set; }
        public string ServiceInfoTypeName { get; set; }
        public bool IsActive { get; set; }
        public bool CanDelete { get; set; }
        public bool RunInHoliday { get; set; }
        public string InstanceName { get; set; }


        public string UrlPath { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan? StopTime { get; set; }
        public int Interval { get; set; }
        public string ResponseData { get; set; }
        public string KeyParameter { get; set; }
        public string ResponseKey { get; set; }
        public string FilterParameter { get; set; }

        public long? ParentRef { get; set; }
        public ServiceInfo Parent { get; set; }
        public ServiceInfoCategory ServiceInfoCategory { get; set; }
        public ICollection<ServiceInfo> Childs { get; set; }
    }
}
