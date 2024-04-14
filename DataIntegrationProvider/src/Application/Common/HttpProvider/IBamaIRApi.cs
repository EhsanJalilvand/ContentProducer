using EnigmaDataProvider.Domain.Models;
using Refit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TSE.SiteAPI.Application.Common.HttpProvider
{
    public interface IBamaIRApi
    {
        [Get("/cad/api/Price/hierarchy?pageIndex={index}&searchQuery=&pageSize=5")]
        Task<BamaIR_Response> GetResponse(int index, CancellationToken cancellationToken = default);
    }
}
