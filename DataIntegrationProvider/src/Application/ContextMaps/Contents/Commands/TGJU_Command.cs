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
using EnigmaDataProvider.Domain.Constans;
using EnigmaDataProvider.Domain.CustomAttributes;

namespace DataIntegrationProvider.Application.Application.ContextMaps.Contents.Commands
{
    [Plan(11, 0, 17, 0, 5, true, false,false)]
    public class TGJU_Command : RecieverCommandAbstraction<TGJU>
    {
        private readonly ITgjuApi _tgjuApi;
        public TGJU_Command(IDocumentSession _documentSession, ILogger<RecieverCommandAbstraction<TGJU>> logger, ITSETMCSoapProvider iTSETMCSoapProvider, ITgjuApi tgjuApi) : base(_documentSession, logger)
        {
            _tgjuApi = tgjuApi;
        }

        public override ServiceCategoryId ServiceCategoryId => ServiceCategoryId.TGJU;
        public override LanguageId LanguageId => LanguageId.Fa;

        public override CategoryId CategoryId => CategoryId.Market;

        public override string[] Tags => new string[] { TagNames.Summary };

        protected async override Task<TGJU> GetData(PlanningInfo detail)
        {
            var result = await _tgjuApi.GetSummary();
            return result;
        }

        protected override void Dispose()
        {

        }
    }


}
