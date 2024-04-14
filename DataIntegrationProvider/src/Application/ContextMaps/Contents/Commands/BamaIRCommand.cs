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
namespace DataIntegrationProvider.Application.Application.ContextMaps.Contents.Commands
{
    public class BamaIRCommand : RecieverCommandAbstraction<BamaIR_Response>
    {
        private readonly IBamaIRApi _bamaIRApi;
        public BamaIRCommand(IDocumentSession _documentSession, ILogger<RecieverCommandAbstraction<BamaIR_Response>> logger, ITSETMCSoapProvider iTSETMCSoapProvider, IBamaIRApi bamaIRApi) : base(_documentSession, logger)
        {
            _bamaIRApi = bamaIRApi;
        }

        public override PlanningInfoId PlanningInfoId => PlanningInfoId.BamaIR_Response;
        protected async override Task<BamaIR_Response> GetData(PlanningInfo detail)
        {
            BamaIR_Response responseTemp = null;
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

            BamaIR_Response bamaIR_Response = new BamaIR_Response() { Data=Datums.ToArray()};
            return bamaIR_Response;
        }

        protected override async Task<bool> SaveData(BamaIR_Response response, PlanningInfo detail)
        {
            DocumentSession.Store(response);
            await DocumentSession.SaveChangesAsync();
            return true;
        }


        protected override async Task<bool> DeleteData(BamaIR_Response response, PlanningInfo detail)
        {

            DocumentSession.HardDeleteWhere<BamaIR_Response>(x => x.ID > 0);
            await DocumentSession.SaveChangesAsync();
            return true;
        }
        protected override void Dispose()
        {

        }
    }


}
