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

namespace DataIntegrationProvider.Application.Application.ContextMaps.Contents.Commands
{
    public class TGJU_SummaryCommand : RecieverCommandAbstraction<TGJU_Summary>
    {
        private readonly ITgjuApi _tgjuApi;
        public TGJU_SummaryCommand(IDocumentSession _documentSession, ILogger<RecieverCommandAbstraction<TGJU_Summary>> logger, ITSETMCSoapProvider iTSETMCSoapProvider, ITgjuApi tgjuApi) : base(_documentSession, logger)
        {
            _tgjuApi = tgjuApi;
        }

        public override PlanningInfoId PlanningInfoId => PlanningInfoId.TGJU_Summary;

        protected async override Task<TGJU_Summary> GetData(PlanningInfo detail)
        {
            var result = await _tgjuApi.GetSummary();
            return result;
        }

        protected override async Task<bool> SaveData(TGJU_Summary response, PlanningInfo detail)
        {
            DocumentSession.Store(response);
            await DocumentSession.SaveChangesAsync();
            return true;
        }


        protected override async Task<bool> DeleteData(TGJU_Summary response, PlanningInfo detail)
        {

            DocumentSession.HardDeleteWhere<TGJU_Summary>(x => x.ID>0);
            await DocumentSession.SaveChangesAsync();
            return true;
        }
        protected override void Dispose()
        {

        }
    }


}
