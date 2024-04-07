using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TSE.SiteAPI.Application.Common.CustomAttributes
{
    public class SoapBodyTagNameAttribute: Attribute
    {
        public string TagName { get; set; }
    }
}
