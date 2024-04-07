using DataIntegrationProvider.Application.Application.Common.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace DataIntegrationProvider.Infrastructure.Services
{
    public class FreeRestApi : IFreeRestApi
    {
        string authHeader;
        private readonly IConfiguration _configuration;
        private readonly ILogger<FreeRestApi> _logger;
        public FreeRestApi(IConfiguration configuration, ILogger<FreeRestApi> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }


    
        public async Task<string> Get(string urlPath, CancellationToken cancellationToken = default)
        {
            var client = new RestClient();
            string fullPath = $"{urlPath}";
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            var request = new RestRequest(fullPath);
            request.Method = Method.Get;
            //request.AddHeader("token", "eyJhbGciOiJIUzUxMiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIzZjRlMmJiNS1kNDVkLTQ4MDItYzMwZC0wOGRhNzQ2Y2E0OTgiLCJleHAiOjE2OTI2OTM2MDB9.l8fuwCicq1-tCsaZekgmFDcLXvs_EcCM2TcyMB7OPvmSt7hgWhPptmemEH_RPVxSZv25k8FJRTvd1dAE1rOglA");
            request.AddHeader("Accept", "*/*");
            request.AddHeader("Content-Type", "application/json; charset=utf-8");
            var response = await client.ExecuteAsync(request);
            if (response.IsSuccessful)
            {
                if (string.IsNullOrEmpty(response.Content))
                {
                    _logger.LogWarning($"In Get FreeRestApi Path:{fullPath}  Response Is Successfull But Response.Data Is Null");
                }
                return response.Content;
            }
            return null;
        }
    }



}
