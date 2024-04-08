using DataIntegrationProvider.Domain.ConfigEntities;
using Microsoft.Extensions.Logging;
using Share.Domain.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using TSE.SiteAPI.Application.Common.CustomAttributes;
using TSE.SiteAPI.Application.Common.Interfaces;

namespace DataIntegrationProvider.Infrastructure.Services
{
    public class TSETMCSoapProvider : ITSETMCSoapProvider
    {
        private readonly ILogger<TSETMCSoapProvider> _logger;
        public TSETMCSoapProvider(ILogger<TSETMCSoapProvider> logger)
        {
            _logger = logger;
        }
        private readonly string _uri = "http://service.tsetmc.com/WebService/TsePublicV2.asmx";
        public List<TModel> GetData<TModel>(PlanningInfo serviceInfo, string url, params int[] flow) where TModel : class
        {
            int tryCount = 3;
            List<TModel> dataModels = new List<TModel>();
            var _serviceInfoLog = $"ServiceInfoCategory: {serviceInfo.PlanningInfoId.GetDisplayName()} ServiceInfoTypeName: {serviceInfo.PlanName}  SerivceInfoDetailId :{serviceInfo.PlanningInfoId}";
            try
            {
                if (flow == null || !flow.Any())
                    flow = new int[] { -1 };

                var tag = typeof(TModel).GetCustomAttributes(typeof(SoapBodyTagNameAttribute), true).FirstOrDefault() as SoapBodyTagNameAttribute;
                if (tag == null)
                {
                    _logger.LogWarning($"SoapBodyTagNameAttribute For {typeof(TModel).Name} Not Set .Service Info  {_serviceInfoLog}");
                    return default(List<TModel>);
                }
                foreach (int i in flow)
                {
                    XmlDocument soapEnvelopeXml = CreateSoapEnvelope(tag.TagName, i);
                    HttpWebRequest webRequest = CreateWebRequest(_uri, url);
                    InsertSoapEnvelopeIntoWebRequest(soapEnvelopeXml, webRequest);
                    _logger.LogInformation($"Begin Get Response For {typeof(TModel).Name} Not Set .Service Info  {_serviceInfoLog}");
                    IAsyncResult asyncResult = webRequest.BeginGetResponse(null, null);
                    asyncResult.AsyncWaitHandle.WaitOne();
                    string soapResult;
                    using (WebResponse webResponse = webRequest.EndGetResponse(asyncResult))
                    {
                        using (StreamReader rd = new StreamReader(webResponse.GetResponseStream()))
                        {
                           // _logger.LogInformation($"Start Read Stream For {typeof(TModel).Name} Not Set .Service Info  {_serviceInfoLog}");
                            soapResult = rd.ReadToEndAsync().GetAwaiter().GetResult();
                        }
                    }

                    var root = typeof(TModel).GetCustomAttributes(typeof(XmlRootAttribute), true).FirstOrDefault() as XmlRootAttribute;

                    if (root == null)
                    {
                        _logger.LogWarning($"XmlRootAttribute For {typeof(TModel).Name} Not Set .Service Info  {_serviceInfoLog}");
                        return default(List<TModel>);
                    }
                    soapEnvelopeXml.RemoveAll();

                    var items = XDocument.Parse(soapResult)
                        .Descendants(root.ElementName)
                        .Select(e => Deserialize<TModel>(e.ToString()))
                        .ToList();
                    dataModels.AddRange(items);
                }

               // _logger.LogInformation($"{dataModels.Count} {typeof(TModel).Name} Readed .Service Info  {_serviceInfoLog}");
            }
            catch (Exception ex)
            {
                tryCount--;
                if (tryCount <= 0)
                    return default(List<TModel>);
                Task.Delay(20000).GetAwaiter().GetResult();
                _logger.LogError($"Error Raised For {typeof(TModel).Name} \n.Error: {ex.Message} \n  .Service Info  {_serviceInfoLog}");
                _logger.LogWarning($"Remain {tryCount} TryCount For {typeof(TModel).Name}.Service Info  {_serviceInfoLog}");
                return GetData<TModel>(serviceInfo, url, flow);

            }
            return dataModels;

        }
        private HttpWebRequest CreateWebRequest(string url, string action)
        {
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(url);
            webRequest.Headers.Add("SOAPAction", action);
            webRequest.ContentType = "text/xml; charset=utf-8";
            webRequest.Accept = "text/xml";
            webRequest.Method = "POST";
            return webRequest;
        }
        private XmlDocument CreateSoapEnvelope(string tagName, int i)
        {
            XmlDocument soapEnvelopeDocument = new XmlDocument();

            string flow = i < 0 ? "" : $"<Flow>{i}</Flow>";

            soapEnvelopeDocument.LoadXml(@$"<?xml version='1.0' encoding='utf - 8'?>
                                            <soap:Envelope xmlns:xsi='http://www.w3.org/2001/XMLSchema-instance' xmlns:xsd='http://www.w3.org/2001/XMLSchema' xmlns:soap='http://schemas.xmlsoap.org/soap/envelope/'>
                                              <soap:Body>
                                                <{tagName} xmlns='http://tsetmc.com/'>
                                                  <UserName>irbourse.com</UserName>
                                                  <Password>irbourse</Password>
                                                  {flow}
                                                </{tagName}>
                                              </soap:Body>
                                            </soap:Envelope>");

            return soapEnvelopeDocument;
        }
        private void InsertSoapEnvelopeIntoWebRequest(XmlDocument soapEnvelopeXml, HttpWebRequest webRequest)
        {
            // Specify UTF-8 encoding
            Encoding utf8Encoding = new UTF8Encoding(false);

            // Open the request stream using XmlWriter with UTF-8 encoding
            using (Stream stream = webRequest.GetRequestStream())
            using (XmlWriter xmlWriter = XmlWriter.Create(stream, new XmlWriterSettings { Encoding = utf8Encoding }))
            {
                // Save the XmlDocument to the XmlWriter
                soapEnvelopeXml.Save(xmlWriter);
            }
        }
        private T Deserialize<T>(string xml)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            using (StringReader reader = new StringReader(xml))
            {
                return (T)serializer.Deserialize(reader);
            }
        }

    }
}
