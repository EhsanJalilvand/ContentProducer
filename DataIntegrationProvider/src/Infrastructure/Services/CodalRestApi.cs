using DataIntegrationProvider.Application.Application.Common.Interfaces;
using DataIntegrationProvider.Domain.Enums;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using RestSharp;
using RestSharp.Authenticators;
using RestSharp.Authenticators.OAuth2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace DataIntegrationProvider.Infrastructure.Services
{
    public class CodalRestApi : ICodalRestApi
    {
        string authHeader;
        private readonly IConfiguration _configuration;
        private readonly ILogger<CodalRestApi> _logger;
        public static string CodalToken = "";
        private readonly string codal_UserName = "user.tse";
        private readonly string codal_Password = "tse@userMm123456";
        private readonly string codal_Userkey = "2274fe089d699e8fd7f884d72ea5e0e30839fd25";
        public CodalRestApi(IConfiguration configuration, ILogger<CodalRestApi> logger)
        {
            _configuration = configuration;
            _logger = logger;

        }
        private static bool flagBusy = false;
        PlanningInfoId ServiceInfoCategoryId;
        public async Task SetAPIKey(PlanningInfoId serviceInfoCategoryId)
        {
            ServiceInfoCategoryId = serviceInfoCategoryId;
            //CodalToken = await _configProvider.GetApiKey(serviceInfoCategoryId);
        }
        public async Task Login(CancellationToken cancellationToken = default)
        {
            while (flagBusy)
            {
                await Task.Delay(1000);
            }
            flagBusy = true;
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(@"https://api.rbcapi.ir");
                    client.DefaultRequestHeaders.Accept.Clear();
                    dynamic user = new JObject();
                    user.Username = codal_UserName;
                    user.Password = codal_Password;
                    user.ProjectCode = "1002";
                    var content = new StringContent(user.ToString(), Encoding.UTF8, "application/json");
                    client.DefaultRequestHeaders.Add("UserKey", codal_Userkey);
                    HttpResponseMessage responseMessage = client.PostAsync("/login", content).Result;

                    if ((int)responseMessage.StatusCode == 401)
                    {
                        await Task.Delay(1000 * 60 * 60 * 4);
                        await Login(CancellationToken.None);
                    }
                    else if ((int)responseMessage.StatusCode == 403)
                    {
                        CodalToken = String.Empty;
                    }
                    else
                    {
                        CodalToken = await responseMessage.Content.ReadAsStringAsync();
                        CodalToken=CodalToken.Substring(1,CodalToken.Length-2);
                        //await _configProvider.UpdateApiKey(ServiceInfoCategoryId, CodalToken);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                flagBusy = false;
            }

        }
        public async Task<string> Get(string urlPath, CancellationToken cancellationToken = default)
        {
            var authenticator = new JwtAuthenticator(CodalToken);
            var client = new RestClient();
            client.Authenticator = authenticator;


            string fullPath = $"{urlPath}";

            var request = new RestRequest(fullPath);

            request.Method = Method.Get;
            var response = await client.ExecuteAsync(request);
            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                await Login(cancellationToken);
                if (string.IsNullOrEmpty(CodalToken))
                    return string.Empty;
                return await Get(urlPath, cancellationToken);
            }
            if (response.IsSuccessful)
            {
                if (string.IsNullOrEmpty(response.Content))
                {
                    _logger.LogWarning($"In Get CodalRestApi Path:{fullPath}  Response Is Successfull But Response.Data Is Null");
                }
                return response.Content;
            }
            return null;
        }


    }



}
