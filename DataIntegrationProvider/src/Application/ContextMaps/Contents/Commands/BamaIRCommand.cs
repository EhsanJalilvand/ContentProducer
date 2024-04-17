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
using HtmlAgilityPack;
using EnigmaDataProvider.Domain.Constans;
using EnigmaDataProvider.Domain.CustomAttributes;

namespace DataIntegrationProvider.Application.Application.ContextMaps.Contents.Commands
{
    [Plan(11, 0, 17, 40, 10, true, false)]
    public class BamaIRCommand : RecieverCommandAbstraction<BamaIR>
    {
        private readonly IBamaIRApi _bamaIRApi;
        public BamaIRCommand(IDocumentSession _documentSession, ILogger<RecieverCommandAbstraction<BamaIR>> logger, ITSETMCSoapProvider iTSETMCSoapProvider, IBamaIRApi bamaIRApi) : base(_documentSession, logger)
        {
            _bamaIRApi = bamaIRApi;
        }

        public override ServiceCategoryId ServiceCategoryId => ServiceCategoryId.BamaIR;
        public override LanguageId LanguageId => LanguageId.Fa;

        public override CategoryId CategoryId => CategoryId.Market;

        public override string[] Tags => new string[] {TagNames.Car};

  

        protected async override Task<BamaIR> GetData(PlanningInfo detail)
        {
            BamaIR responseTemp = null;
            List<BamaIR_Group> Datums = new List<BamaIR_Group>();
            int i = 0;
            do
            {
                responseTemp = await _bamaIRApi.GetResponse(i);
                if(responseTemp != null)
                foreach (var item in responseTemp.Data)
                {
                        Datums.Add(item);
                }
                i++;
            } while (responseTemp!=null && responseTemp.Data.Any());

            if (!Datums.Any())
                return null;

            BamaIR bamaIR_Response = new BamaIR() { Data=Datums.ToArray()};
            return bamaIR_Response;
        }


        protected override void Dispose()
        {

        }
    }


}
