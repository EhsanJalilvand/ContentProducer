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
using SharedDomainService.Interfaces;
using SharedDomain.ObjectValues;
using SharedDomain.Enums;
using HtmlAgilityPack;
namespace DataIntegrationProvider.Application.Application.ContextMaps.Contents.Commands
{
    [Plan(11, 0, 17, 40, 10, true, false,true)]
    public class Beytoote_NewsPaper_Command : RecieverCommandAbstraction<Beytoote_NewPaper>
    {
        private readonly ICrawlClientHandler _crawlClientHandler;
        public Beytoote_NewsPaper_Command(IDocumentSession _documentSession, ILogger<RecieverCommandAbstraction<Beytoote_NewPaper>> logger, ITSETMCSoapProvider iTSETMCSoapProvider, ICrawlClientHandler crawlClientHandler) : base(_documentSession, logger)
        {
            _crawlClientHandler = crawlClientHandler;
        }

        public override ServiceCategoryId ServiceCategoryId => ServiceCategoryId.Beytoote;
        public override LanguageId LanguageId => LanguageId.Fa;

        public override CategoryId CategoryId => CategoryId.News;

        public override string[] Tags => new string[] { TagNames.NewsPaper };

        protected async override Task<Beytoote_NewPaper> GetData(PlanningInfo detail)
        {
            Beytoote_NewPaper beytoote_NewPaper = new Beytoote_NewPaper();
            string baseAddress = "https://www.beytoote.com/news/newspaper/";
            string address = string.Empty;
            await _crawlClientHandler.SendCommand(new CrawlRequestCommand() { CrawlRequestCommandType = CrawlRequestCommandType.GetAddress }, async (a) =>
            {
                address = a.Address;
                if (string.IsNullOrEmpty(a.Address) || a.Address != baseAddress)
                {
                    _crawlClientHandler.SendCommand(new CrawlRequestCommand() { CrawlRequestCommandType = CrawlRequestCommandType.LoadUrlAsync, Script = baseAddress }, null).GetAwaiter().GetResult();
                    string currentAddress = string.Empty;
                    _crawlClientHandler.SendCommand(new CrawlRequestCommand() { CrawlRequestCommandType = CrawlRequestCommandType.GetAddress }, async (ad) =>
                    {
                        currentAddress = ad.Address;
                    }).GetAwaiter().GetResult();

                    if (currentAddress != baseAddress)
                    {
                        _crawlClientHandler.SendCommand(new CrawlRequestCommand() { CrawlRequestCommandType = CrawlRequestCommandType.WaitForSelector, Script = ".domain-suggest", Interval = 60 }, null).GetAwaiter().GetResult();
                    }
                }

            });


            await _crawlClientHandler.SendCommand(new CrawlRequestCommand() { CrawlRequestCommandType = CrawlRequestCommandType.GetContent }, (a) =>
              {
                  var content = a.Content;
                  var htmlDoc = new HtmlDocument();
                  htmlDoc.LoadHtml(content);

                  var mainSection = htmlDoc.GetElementbyId("innertop");
                  var div1 = mainSection.SelectSingleNode("//div");
                  var div2 = div1.SelectSingleNode("//div[@class='allmode_topbox']");
                  var div3 = div2.SelectSingleNode("//div[@class='allmode_img']");
                  var aTag = div3.FirstChild;

                  string hrefValue = aTag.GetAttributeValue("href", string.Empty);

                  var todayUrl = $"https://www.beytoote.com{hrefValue}";
                  string address = string.Empty;
                  _crawlClientHandler.SendCommand(new CrawlRequestCommand() { CrawlRequestCommandType = CrawlRequestCommandType.GetAddress }, async (a) =>
                  {
                      address = a.Address;
                      if (string.IsNullOrEmpty(a.Address) || a.Address != todayUrl)
                      {
                          _crawlClientHandler.SendCommand(new CrawlRequestCommand() { CrawlRequestCommandType = CrawlRequestCommandType.LoadUrlAsync, Script = todayUrl }, null).GetAwaiter().GetResult();
                          string currentAddress = string.Empty;
                          _crawlClientHandler.SendCommand(new CrawlRequestCommand() { CrawlRequestCommandType = CrawlRequestCommandType.GetAddress }, async (ad) =>
                          {
                              currentAddress = ad.Address;
                          }).GetAwaiter().GetResult();

                          if (currentAddress != todayUrl)
                          {
                              _crawlClientHandler.SendCommand(new CrawlRequestCommand() { CrawlRequestCommandType = CrawlRequestCommandType.WaitForSelector, Script = ".domain-suggest", Interval = 60 }, null).GetAwaiter().GetResult();
                          }
                      }

                  }).GetAwaiter().GetResult();


                  //_crawlClientHandler.SendCommand(new CrawlRequestCommand() { CrawlRequestCommandType = CrawlRequestCommandType.LoadUrlAsync, Script = todayUrl }, (a) => { }).GetAwaiter().GetResult();

                  _crawlClientHandler.SendCommand(new CrawlRequestCommand() { CrawlRequestCommandType = CrawlRequestCommandType.GetContent }, (a) =>
                  {
                     
                      var htmlDoc = new HtmlDocument();
                      htmlDoc.LoadHtml(a.Content);

                      var mainSection = htmlDoc.DocumentNode.SelectSingleNode("//article");

                      var h1 = mainSection.SelectSingleNode("//h1/a");
                      beytoote_NewPaper.Title = h1.InnerHtml;

                      var imgarticles = mainSection.SelectNodes("//div[@class='imgarticle']");
                      foreach (var item in imgarticles)
                      {
                          var img = item.FirstChild.GetAttributes("src").First().Value;
                          beytoote_NewPaper.Urls.Add(new Tuple<string, string>($"https://www.beytoote.com{img}",string.Empty));
                      }

                  });
              });


            return beytoote_NewPaper;
        }

        protected override void Dispose()
        {

        }
    }


}
