using SharedDomain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedDomain.ObjectValues
{
    public class CrawlResponseStatus
    {
        public CrawlResponseStatusType CrawlResponseStatusType { get; set; }
        public string Message { get; set; }
        public string Content { get; set; }
        public string Address { get; set; }
        public bool EvaluateScriptStatus { get; set; }
        public object EvaluateScriptResult { get; set; }
    }
}
