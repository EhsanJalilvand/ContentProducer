using SharedDomain.ObjectValues;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedDomainService.Interfaces
{
    public interface ICrawlServerHandler
    {
        Task<bool> StartService( Action<CrawlRequestCommand, IMessageHandler> message);
    }
  
}
