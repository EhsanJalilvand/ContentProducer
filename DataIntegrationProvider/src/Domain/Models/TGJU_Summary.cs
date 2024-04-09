using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnigmaDataProvider.Domain.Models
{
    public class TGJU_Summary
    {
        public long ID { get; set; }
        [JsonProperty("current")]
        public Dictionary<string, TGJU_SummaryItem> Current { get; set; }
    }
    public partial class TGJU_SummaryItem
    {

        [JsonProperty("p")]
        public string P { get; set; }

        [JsonProperty("h")]
        public string H { get; set; }

        [JsonProperty("l")]
        public string L { get; set; }

        [JsonProperty("d")]
        public string D { get; set; }

        [JsonProperty("dp")]
        public double Dp { get; set; }

        [JsonProperty("dt")]
        public string Dt { get; set; }

        [JsonProperty("t")]
        public string T { get; set; }

        [JsonProperty("ts")]
        public string Ts { get; set; }
    }




}
