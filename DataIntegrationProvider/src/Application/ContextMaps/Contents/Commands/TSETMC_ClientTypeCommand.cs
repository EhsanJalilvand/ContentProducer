//using AutoMapper;
//using DataIntegrationProvider.Application.Common.Interfaces;
//using DataIntegrationProvider.Domain.Enums;
//using Microsoft.Extensions.Logging;
//using Microsoft.Extensions.Options;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using DataIntegrationProvider.Application.Application.Common.Abstractions;
//using DataIntegrationProvider.Application.Application.Common.Configurations;
//using DataIntegrationProvider.Application.Application.Common.Interfaces;
//using DataIntegrationProvider.Domain.ConfigEntities;
//using System.Xml;
//using System.Net;
//using System.IO;
//using System.Xml.Linq;
//using System.Threading;
//using System.Xml.Serialization;
//using Microsoft.EntityFrameworkCore;
//using TSE.SiteAPI.Application.Common.CustomAttributes;
//using TSE.SiteAPI.Application.Common.Interfaces;
//using DataIntegrationProvider.WebUI.Entities;

//namespace DataIntegrationProvider.Application.Application.ContextMaps.Contents.Commands
//{
//    public class TSETMC_ClientTypeCommand : RecieverCommandAbstraction<ClientType>
//    {
//        private readonly ITSETMCSoapProvider _iTSETMCSoapProvider;
//        private readonly ITSETMCDBContext _siteDBContext;
//        private readonly IConfigDbContext _configDbContext;
//        public TSETMC_ClientTypeCommand(IConfigProvider configProvider, ILogger<RecieverCommandAbstraction<ClientType>> logger, IOptionsMonitor<ServiceConfig> serviceConfig, IConfigDbContext dataDbContext, ITSETMCDBContext siteDBContext, IConfigDbContext configDbContext, ITSETMCSoapProvider iTSETMCSoapProvider) : base(configProvider, logger, serviceConfig, dataDbContext)
//        {
//            _siteDBContext = siteDBContext;
//            _configDbContext = configDbContext;
//            _iTSETMCSoapProvider = iTSETMCSoapProvider;
//        }

//        public override ServiceInfoCategoryId ServiceInfoCategoryId => ServiceInfoCategoryId.TSETMC_ClientType;

//        protected async override Task<List<ClientType>> GetData(ServiceInfo serviceInfo, string urlPath)
//        {
//            var counter = serviceInfo.ResponseKey.ToLong() ?? 0;
//            if (counter == 0)
//                counter = _siteDBContext.ClientTypes.AsNoTracking().Max(m => m.ClientType_counter) ?? 0;
//            counter++;

//            var ClientTypes = _iTSETMCSoapProvider.GetData<ClientTypeDTO>(serviceInfo, urlPath);
//            var result = ClientTypes.Select(s => new ClientType()
//            {
//                Sell_CountI = s.Sell_CountI,
//                Sell_CountN = s.Sell_CountN,
//                Sell_I_Volume = s.Sell_I_Volume,
//                Sell_N_Volume = s.Sell_N_Volume,
//                Buy_CountI = s.Buy_CountI,
//                Buy_CountN = s.Buy_CountN,
//                Buy_I_Volume = s.Buy_I_Volume,
//                Buy_N_Volume = s.Buy_N_Volume,
//                ClientType_counter = counter,
//                InsCode = s.InsCode,
//            }).Where(w => w.InsCode.HasValue).Distinct().ToList();

//            ClientTypes.Clear();
//            return result;
//        }

//        protected override async Task<CodalResponseData> SaveData(List<ClientType> response, ServiceInfo detail, long? dataParentRef)
//        {
//            var si = _configDbContext.ServiceInfoS.FirstOrDefault(p => p.ServiceInfoId == detail.ServiceInfoId);
//            si.ResponseKey = response.First().ClientType_counter.ToString();

//            await _siteDBContext.BulkInsert(response);
//            _siteDBContext.UpdateCounter("clientType", si.ResponseKey);
//            await _configDbContext.SaveChangesAsync(CancellationToken.None);
//            return null;
//        }


//        protected override async Task<bool> DeleteData(List<ClientType> response, ServiceInfo detail, long? dataParentRef)
//        {
//            _siteDBContext.Database.ExecuteSqlRaw($"TRUNCATE TABLE [{nameof(ClientType)}]");
//            return true;
//        }
//        protected override void Dispose()
//        {
//            _siteDBContext.Dispose();
//        }
//    }

//    [SoapBodyTagName(TagName = "ClientType")]
//    [Serializable]
//    [XmlRoot("Data")] // Update with the actual root element name
//    public class ClientTypeDTO
//    {

//        [XmlElement("InsCode")]
//        public long? InsCode { get; set; }

//        [XmlElement("Buy_CountI")]
//        public int? Buy_CountI { get; set; }

//        [XmlElement("Buy_CountN")]
//        public int? Buy_CountN { get; set; }

//        [XmlElement("Buy_I_Volume")]
//        public decimal? Buy_I_Volume { get; set; }

//        [XmlElement("Buy_N_Volume")]
//        public decimal? Buy_N_Volume { get; set; }

//        [XmlElement("Sell_CountI")]
//        public int? Sell_CountI { get; set; }

//        [XmlElement("Sell_CountN")]
//        public int? Sell_CountN { get; set; }

//        [XmlElement("Sell_I_Volume")]
//        public decimal? Sell_I_Volume { get; set; }

//        [XmlElement("Sell_N_Volume")]
//        public decimal? Sell_N_Volume { get; set; }

//        [XmlElement("ClientType_counter")]
//        public long? ClientType_counter { get; set; }
//    }
//}
