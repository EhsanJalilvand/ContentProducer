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

namespace DataIntegrationProvider.Application.Application.ContextMaps.Contents.Commands
{
    public class TSETMC_ClientTypeCommand : RecieverCommandAbstraction<PlanningInfo>
    {
        public TSETMC_ClientTypeCommand(IDocumentSession _documentSession, ILogger<RecieverCommandAbstraction<PlanningInfo>> logger, ITSETMCSoapProvider iTSETMCSoapProvider) : base(_documentSession, logger)
        {
        }

        public override PlanningInfoId PlanningInfoId => PlanningInfoId.TGJU_Summary;

        protected async override Task<List<PlanningInfo>> GetData(PlanningInfo serviceInfo)
        {
            return new List<PlanningInfo>() { new PlanningInfo() };
        }

        protected override async Task<bool> SaveData(List<PlanningInfo> response, PlanningInfo detail)
        {
      
            return true;
        }


        protected override async Task<bool> DeleteData(List<PlanningInfo> response, PlanningInfo detail)
        {
           
            return true;
        }
        protected override void Dispose()
        {
           
        }
    }


}
