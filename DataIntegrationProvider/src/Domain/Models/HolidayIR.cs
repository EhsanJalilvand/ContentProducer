using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnigmaDataProvider.Domain.Models
{
    public partial class HolidayIR: ModelBase
    {
        [JsonProperty("is_holiday")]
        public bool is_holiday { get; set; }

        [JsonProperty("events")]
        public Event[] Events { get; set; }
    }
    public partial class Event
    {
        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("additional_description")]
        public string AdditionalDescription { get; set; }

        [JsonProperty("is_holiday")]
        public bool is_holiday { get; set; }

        [JsonProperty("is_religious")]
        public bool is_religious { get; set; }
    }



}
