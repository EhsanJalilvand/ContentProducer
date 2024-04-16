using SharedDomain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedDomain.ObjectValues
{
    public class CrawlRequestCommand
    {
        public CrawlRequestCommandType CrawlRequestCommandType { get; set; }
        public string Script { get; set; }
        public int Interval { get; set; }
    }
}
