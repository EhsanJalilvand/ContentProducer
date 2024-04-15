using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnigmaDataProvider.Domain.Models
{
    public class _ContentBase:ModelBase
    {
        public string SourceUrl { get; set; }
        public string Content { get; set; }

    }
}
