using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnigmaDataProvider.Domain.Models
{

    public partial class BamaIR:ModelBase
    {
        public BamaIR_Group[] Data { get; set; }
    }

    public partial class BamaIR_Group
    {
        public string Brand { get; set; }
        public string brand_fa { get; set; }
        public long items_count { get; set; }
        public BamaIR_Group_Item[] Items { get; set; }
    }

    public partial class BamaIR_Group_Item
    {
        public long Id { get; set; }
        public string Brand { get; set; }
        public string brand_fa { get; set; }
        public string Model { get; set; }
        public string model_fa { get; set; }
        public string Trim { get; set; }
        public string trim_fa { get; set; }
        public long model_year { get; set; }
        public string Class { get; set; }
        public long Price { get; set; }
        public double price_diff { get; set; }
        public string price_date { get; set; }
        public string price_provider { get; set; }
    }

    public partial class Metadata
    {
        public string LastUpdate { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
    }


}
