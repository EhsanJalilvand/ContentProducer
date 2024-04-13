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
    public interface IHolidayIRApi
    {
        [Get("/jalali/{year}/{month}/{day}")]
        Task<HolidayIR> GetResponse(int year,int month,int day, CancellationToken cancellationToken = default);
    }
}
