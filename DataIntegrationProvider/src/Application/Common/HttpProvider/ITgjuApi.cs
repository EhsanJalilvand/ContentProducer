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
    public interface ITgjuApi
    {
        [Get("/ajax.json")]
        Task<TGJU_Summary> GetSummary(CancellationToken cancellationToken = default);
    }
}
