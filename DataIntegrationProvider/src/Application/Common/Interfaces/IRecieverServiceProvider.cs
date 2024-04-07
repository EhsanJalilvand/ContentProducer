using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataIntegrationProvider.Application.Application.Common.Interfaces
{
    public interface IRecieverServiceProvider
    {
        void RegisterAllCommand();
        void Start();
    }
}
