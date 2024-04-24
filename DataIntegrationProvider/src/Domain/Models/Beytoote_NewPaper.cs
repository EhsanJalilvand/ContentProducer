using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnigmaDataProvider.Domain.Models
{
    public class Beytoote_NewPaper : ModelBase
    {
        public Beytoote_NewPaper()
        {
            Urls = new List<Tuple<string, string>>();
        }
        public string Title { get; set; }
        public List<Tuple<string,string>> Urls { get; set; }
    }
}
