using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityServer.Domain.Enums
{
    public  enum AccessMode :byte
    {
        HasAccess=1,
        AccessDenied=2,
        GroupAccess=3
    }
}
