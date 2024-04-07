using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataIntegrationProvider.Domain.ConfigEntities
{
    public class CodalResponseData
    {
        public CodalResponseData()
        {
            Childs=new List<CodalResponseData>();
        }
        public long CodalResponseDataId { get; set; }
        public long SerivceInfoId { get; set; }
        public DateTime InsertTime { get; set; }
        public bool IsDeleted { get; set; }
        public string Body { get; set; }
        public string BodyHash { get; set; }
        public string ResponseKey { get; set; }
        public string RelatedInfo { get; set; }
        public long? ParentRef { get; set; }
        public CodalResponseData Parent { get; set; }
        public virtual ICollection<CodalResponseData> Childs { get; set; }
    }
}
