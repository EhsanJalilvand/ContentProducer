using SharedDomain.ObjectValues;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedDomainService.Interfaces
{
    public interface ICrawlClientHandler
    {
        Task<bool> SendCommand( CrawlRequestCommand command, Action<CrawlResponseStatus> response);
    }
}
