using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnigmaDataProvider.Domain.Models
{
    public class ModelBase
    {
        public long ID { get; set; }
        public DateTime CreateTime { get; set; }
        public string ServiceCategoryName { get; set; }
        public string Language { get; set; }
        public string Category { get; set; }
        public string[] Tags { get; set; }
    }
}
