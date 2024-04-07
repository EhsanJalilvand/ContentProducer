using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace DataIntegrationProvider.Application.Application.Common.Interfaces
{
    public interface IFreeRestApi
    {
        Task<string> Get(string urlPath,CancellationToken cancellationToken = default);

    }
}
