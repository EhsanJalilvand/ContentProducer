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
    public class TSETMCRestApi : ITSETMCRestApi
    {
        string authHeader;
        private readonly IConfiguration _configuration;
        private readonly ILogger<TSETMCRestApi> _logger;
        public TSETMCRestApi(IConfiguration configuration, ILogger<TSETMCRestApi> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }


    
        public async Task<string> Get(string urlPath, CancellationToken cancellationToken = default)
        {
            var client = new RestClient();
            string fullPath = $"{urlPath}";
            //ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            var request = new RestRequest(fullPath);
            request.Method = Method.Get;
            request.AddHeader("User-Agent", "Mozilla/4.0");
            request.AddHeader("Accept", "*/*");
            request.AddHeader("Content-Type", "application/json; charset=utf-8");
            var response = await client.ExecuteAsync(request);
            if (response.IsSuccessful)
            {
                if (string.IsNullOrEmpty(response.Content))
                {
                    _logger.LogWarning($"In Get TSETMCRestApi Path:{fullPath}  Response Is Successfull But Response.Data Is Null");
                }
                return response.Content;
            }
            return null;
        }
    }



}
