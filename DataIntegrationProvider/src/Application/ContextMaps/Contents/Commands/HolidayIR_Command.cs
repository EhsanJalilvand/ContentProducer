using AutoMapper;
using DataIntegrationProvider.Domain.Enums;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataIntegrationProvider.Application.Application.Common.Abstractions;
using DataIntegrationProvider.Application.Application.Common.Interfaces;
using DataIntegrationProvider.Domain.ConfigEntities;
using System.Xml;
using System.Net;
using System.IO;
using System.Xml.Linq;
using System.Threading;
using System.Xml.Serialization;
using Microsoft.EntityFrameworkCore;
using TSE.SiteAPI.Application.Common.CustomAttributes;
using TSE.SiteAPI.Application.Common.Interfaces;
using Marten;
using TSE.SiteAPI.Application.Common.HttpProvider;
using EnigmaDataProvider.Domain.Models;
using System.Globalization;

namespace DataIntegrationProvider.Application.Application.ContextMaps.Contents.Commands
{
    public class HolidayIR_Command : RecieverCommandAbstraction<HolidayIR>
    {
        private readonly IHolidayIRApi _holidayIRApi;
        public HolidayIR_Command(IDocumentSession _documentSession, ILogger<RecieverCommandAbstraction<HolidayIR>> logger, ITSETMCSoapProvider iTSETMCSoapProvider, IHolidayIRApi holidayIRApi) : base(_documentSession, logger)
        {
            _holidayIRApi = holidayIRApi;
        }

        public override ServiceCategoryId ServiceCategoryId => ServiceCategoryId.HolidayIR;
        public override LanguageId LanguageId => LanguageId.Fa;

        public override CategoryId CategoryId => CategoryId.Calendar;

        public override SubCategoryId SubCategoryId => SubCategoryId.Holiday;
        protected async override Task<HolidayIR> GetData(PlanningInfo detail)
        {
            string key = "HolidayIR";
            var config = DocumentSession.Query<Config>().Where(w => w.Key == key).FirstOrDefault();
            PersianCalendar persianCalendar = new PersianCalendar();
            int year = 0, month = 0, day = 0;
            DateTime dtime = DateTime.Now;
            if (config == null)
            {
                dtime = DateTime.Now.AddMonths(-1);
                year = persianCalendar.GetYear(dtime);
                month = persianCalendar.GetMonth(dtime);
                day = persianCalendar.GetDayOfMonth(dtime);

                config = new Config() { Key = key, Value = dtime.ToString("yyyyMMdd") };
            }
            else
            {
                dtime = DateTime.ParseExact(config.Value, "yyyyMMdd", null);
                dtime = dtime.AddDays(1);

                year = persianCalendar.GetYear(dtime);
                month = persianCalendar.GetMonth(dtime);
                day = persianCalendar.GetDayOfMonth(dtime);

                config.Value = dtime.ToString("yyyyMMdd");

            }
            DocumentSession.Store(config);

            var result = await _holidayIRApi.GetResponse(year, month, day);
            return result;
        }

        protected override void Dispose()
        {

        }
    }


}
