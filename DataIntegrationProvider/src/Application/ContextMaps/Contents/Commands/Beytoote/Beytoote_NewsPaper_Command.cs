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

namespace DataIntegrationProvider.Application.Application.ContextMaps.Contents.Commands
{
    public class Beytoote_NewsPaper_Command : RecieverCommandAbstraction<TGJU>
    {
        private readonly ITgjuApi _tgjuApi;
        public Beytoote_NewsPaper_Command(IDocumentSession _documentSession, ILogger<RecieverCommandAbstraction<TGJU>> logger, ITSETMCSoapProvider iTSETMCSoapProvider, ITgjuApi tgjuApi) : base(_documentSession, logger)
        {
            _tgjuApi = tgjuApi;
        }

        public override ServiceCategoryId ServiceCategoryId => ServiceCategoryId.Beytoote;
        public override LanguageId LanguageId => LanguageId.Fa;

        public override CategoryId CategoryId => CategoryId.News;

        public override string[] Tags => new string[] { TagNames.NewsPaper };

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
