using DataIntegrationProvider.Application.Application.Common.Configurations;
using DataIntegrationProvider.Application.Application.Common.Interfaces;
using DataIntegrationProvider.Application.Common.Interfaces;
using DataIntegrationProvider.Domain.ConfigEntities;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using DataIntegrationProvider.Domain.Enums;
using Share.Domain.Extensions;

namespace DataIntegrationProvider.Infrastructure.Services
{
    public class ConfigProvider : IConfigProvider
    {
        private readonly IConfigDbContext _configDbContext;
        private readonly ICodalDbContext _codalDbContext;
        private readonly IMemoryCache _memoryCache;

        private string _DataConfigCachKey = "DataConfig";

        public ConfigProvider(IConfigDbContext configDbContext, IMemoryCache memoryCache, ICodalDbContext codalDbContext)
        {
            _configDbContext = configDbContext;
            _memoryCache = memoryCache;
            _codalDbContext = codalDbContext;
        }

 

        public ServiceInfo GetLastSerivceInfoDetail(long SerivceInfoId)
        {
            return _configDbContext.ServiceInfoS.FirstOrDefault(p => p.ServiceInfoId == SerivceInfoId);
        }

        public string ParseUrl(ServiceInfo serviceInfo, out long? dataParentRef)
        {
            dataParentRef = null;
            var url = serviceInfo.UrlPath;
            if (string.IsNullOrEmpty(url) || url.IndexOf('{') <= 0 || url.IndexOf('}') <= 0)
            {
                return url;
            }
            while (url.Contains("{") && url.Contains("}"))
            {
                var openIndex = url.LastIndexOf("{");
                var closeIndex = url.LastIndexOf("}");

                var len = closeIndex - openIndex;
                var keyword = url.Substring(openIndex + 1, len - 1);

                var value = ParsekeyWord(keyword, serviceInfo.ResponseKey, serviceInfo.FilterParameter, serviceInfo.ParentRef, out long? dpr);
                if (!string.IsNullOrEmpty(keyword) && string.IsNullOrEmpty(value))
                    return string.Empty;
                dataParentRef = dpr;
                url = url.Replace("{" + keyword + "}", value);
            }
            return url;

        }
        public string ParsekeyWord(string keyWord, string responseKey, string filterParameter, long? configParentRef, out long? dataParentref)
        {
            dataParentref = null;
            if (keyWord.Contains("codaldate"))
            {
                return GetValueFromCodalDate(keyWord);
            }
            else if (keyWord == "date")
            {
                return DateTime.Now.ToString("yyyy-MM-dd");
            }
            else if (keyWord == "datetime")
            {
                return DateTime.Now.ToString("yyyy-MM-dd") + "T" + DateTime.Now.ToString("HH:mm:ss") + ".000Z";
            }
            else if (keyWord == "time")
            {
                return DateTime.Now.ToString("HH:mm:ss");
            }
            else if (keyWord.Contains("codalkey"))
            {
                return GetValueFromcodalKey(keyWord, filterParameter, configParentRef, ref dataParentref);
            }
            else if (keyWord.Contains("key"))
            {
                return GetKeyFromResponseKey(responseKey, keyWord);
            }
            else if (keyWord.Contains("page"))
            {
                return GetValueFromPage(keyWord);
            }

            else
                return string.Empty;

        }
        public string GetKeyFromResponseKey(string responseKey, string keyWord)
        {
            if (string.IsNullOrEmpty(responseKey))
                return string.Empty;
            if (keyWord.Contains("key+") && keyWord.Length > 4)
            {
                var digitKey = responseKey.ToDecimal();
                var inc = keyWord.Substring(keyWord.IndexOf("+") + 1, keyWord.Length - 4).ToLower().ToDecimal();
                if (!digitKey.HasValue || !inc.HasValue)
                    return string.Empty;
                return (digitKey + inc).Value.ToString();

            }
            return string.Empty;
        }
        public string GetValueFromPage(string keyWord)
        {

            if (keyWord.Contains("page:") && keyWord.Length > 5)
            {
                var pageNumber = keyWord.Substring(keyWord.IndexOf(":") + 1, keyWord.Length - 5).ToLower().ToDecimal();
                if (!pageNumber.HasValue || !pageNumber.HasValue)
                    return string.Empty;
                return (pageNumber).ToString();

            }
            return string.Empty;
        }
        public string GetValueFromCodalDate(string keyWord)
        {

            if (keyWord.Contains("codaldate:") && keyWord.Length > 10)
            {
                var date = keyWord.Substring(keyWord.IndexOf(":") + 1, keyWord.Length - 10).ToLower();

                System.String[] userDateParts = date.Trim().Split(new[] { "/" }, System.StringSplitOptions.None);
                int year = int.Parse(userDateParts[0]);
                int month = int.Parse(userDateParts[1]);
                int day = int.Parse(userDateParts[2]);
                string qdate = $"year={year}&month={month}&day={day}";

                return qdate;
            }
            return string.Empty;
        }
        public string GetValueFromcodalKey(string keyWord, string filterParameter, long? parentRef, ref long? dataParentref)
        {
            decimal? serviceInfoDetailId = parentRef;
            if (!serviceInfoDetailId.HasValue && keyWord.Contains("codalkey<") && keyWord.Length > 9)
            {
                var serviceInfoDetailId_str = keyWord.Substring(keyWord.IndexOf("<") + 1, keyWord.Length - 10).ToLower();
                serviceInfoDetailId = serviceInfoDetailId_str.ToDecimal();
                if (!serviceInfoDetailId.HasValue)
                    return string.Empty;
            }
            ///Find 1 TraceNumber
            ///
            var values = JsonConvert.DeserializeObject<Dictionary<string, string>>(filterParameter);
            var master = string.Empty;
            var file = string.Empty;
            var letterType = string.Empty;



            string otherWhere = string.Empty;
            foreach (var item in values.Keys)
            {
                if (item.ToLower() == "file" || item.ToLower() == "master")
                {
                    if (values[item] != "0")
                    {
                        otherWhere += $"AND json_value(t.relatedinfo, '$.{item}') = {values[item]} ";
                    }
                    else
                    {
                        otherWhere += $"AND (t.relatedinfo is null OR json_value(t.relatedinfo, '$.{item}') = {values[item]} OR json_value(t.relatedinfo, '$.{item}') is null) ";
                    }
                    continue;
                }
                otherWhere += $"AND json_value(t.Body, '$.{item}') = {values[item]} ";
            }
            var queryCommand = @$"select * from CodalResponseData t
where t.SerivceInfoId = {serviceInfoDetailId} {otherWhere}";
            var record = _codalDbContext.CodalResponseDatas.FromSqlRaw(queryCommand).FirstOrDefault();
            if (record == null)
                return string.Empty;

            dataParentref = record.CodalResponseDataId;
            return record.ResponseKey;
        }



        public string GetResponseKeyFromResponse(string response, string keyParameter)
        {
            if (string.IsNullOrEmpty(response) || string.IsNullOrEmpty(keyParameter))
                return string.Empty;


            var responseArray = response.Split(',');
            if (responseArray.Length == 0)
                return string.Empty;


            var trueKey = $"\"{keyParameter}\":";
            for (int i = responseArray.Length - 1; i >= 0; i--)
            {
                var value = responseArray[i];
                value = value.Replace("{", "").Replace("}", "");
                if (value.Contains(trueKey))
                {
                    return value.Replace(trueKey, "").Trim();
                }

            }

            return String.Empty;


        }
        public async Task UpdateResponseKey(long serivceInfoId, string responseKey)
        {
            var item = _configDbContext.ServiceInfoS.FirstOrDefault(p => p.ServiceInfoId == serivceInfoId);
            if (item == null)
                return;
            item.ResponseKey = responseKey;
            await _configDbContext.SaveChangesAsync(System.Threading.CancellationToken.None);
        }
        public async Task UpdateUrlPath(long serivceInfoId, string responseKey)
        {
            var serivceInfoDetail = _configDbContext.ServiceInfoS.FirstOrDefault(p => p.ServiceInfoId == serivceInfoId);
            if (serivceInfoDetail == null)
                return;
            var url = serivceInfoDetail.UrlPath;
            if (string.IsNullOrEmpty(url) || url.IndexOf('{') <= 0 || url.IndexOf('}') <= 0)
                return;
            string originalUrl = url;
            while (url.Contains("{") && url.Contains("}"))
            {
                var openIndex = url.LastIndexOf("{");
                var closeIndex = url.LastIndexOf("}");

                var len = closeIndex - openIndex;
                var keyword = url.Substring(openIndex + 1, len - 1);
                if (keyword.Contains("<") || keyword.Contains(">"))
                {
                    return;
                }
                long? dataParentRef = null;
                var value = ParsekeyWord(keyword, responseKey, serivceInfoDetail.FilterParameter, serivceInfoDetail.ParentRef, out dataParentRef);
                if (keyword.Contains("page"))
                {
                    value = ((value.ToDecimal().HasValue ? value.ToDecimal().Value : -1) + 1).ToString();
                    originalUrl = originalUrl.Replace("{" + keyword + "}", "{page:" + value + "}");
                }
                if (keyword.Contains("codaldate"))
                {
                    var st_p1 = value.Substring(value.IndexOf("=") + 1, value.IndexOf("&") - 5);
                    value = value.Replace("year=" + st_p1 + "&", "");
                    var st_p2 = value.Substring(value.IndexOf("=") + 1, value.IndexOf("&") - 6);
                    value = value.Replace("month=" + st_p2 + "&", "");
                    var st_p3 = value.Substring(value.IndexOf("=") + 1);

                    var pDate = $"{st_p1}/{st_p2}/{st_p3}";
                    var date = DateTimeHelper.ToGregorianDate("/", pDate);
                    if (!date.HasValue)
                        originalUrl = originalUrl.Replace("{" + keyword + "}", $"codaldate:{st_p1}/{st_p2}/{st_p3}");
                    else
                    {
                        var currentDateTime = DateTimeHelper.GetCurrentDateTime();
                        var fixDateTime = new DateTime(currentDateTime.Year, currentDateTime.Month, currentDateTime.Day, 0, 1, 1);
                        if (date < fixDateTime)
                            date = date.Value.AddDays(1);
                        value = DateTimeHelper.ConvertToPersianDateTime(date);
                        originalUrl = originalUrl.Replace("{" + keyword + "}", "{codaldate:" + value + "}");
                    }
                }
                url = url.Replace("{" + keyword + "}", value);
            }
            serivceInfoDetail.UrlPath = originalUrl;
            await _configDbContext.SaveChangesAsync(System.Threading.CancellationToken.None);
        }

        public async Task UpdateRelatedInfo(long ResponseDataId, string filterParameter, bool dataNotExists)
        {
            if (string.IsNullOrEmpty(filterParameter))
                return;
            var record = _codalDbContext.CodalResponseDatas.Where(w => w.CodalResponseDataId == ResponseDataId).FirstOrDefault();
            if (record == null)
                return;



            var values = JsonConvert.DeserializeObject<Dictionary<string, string>>(filterParameter);
            Dictionary<string, string> dicRelatedValue = new Dictionary<string, string>();
            if (!string.IsNullOrEmpty(record.RelatedInfo))
            {
                var oldValues = JsonConvert.DeserializeObject<Dictionary<string, string>>(record.RelatedInfo);
                if (oldValues != null)
                {
                    var remainOldValue = oldValues.Where(w => !values.Select(s => s.Key).Contains(w.Key));
                    if (remainOldValue != null)
                        foreach (var item in remainOldValue)
                        {
                            dicRelatedValue.Add(item.Key, item.Value);
                        }
                }
            }

            foreach (var item in values.Keys)
            {
                if (item.ToLower() == "file" || item.ToLower() == "master")
                {
                    if (values[item] == "0" && !dataNotExists)
                    {
                        dicRelatedValue.Add(item, "1");
                    }
                    else if (values[item] == "0" && dataNotExists)
                    {
                        dicRelatedValue.Add(item, "-1");
                    }
                    else
                        dicRelatedValue.Add(item, values[item]);
                    continue;
                }
                else
                    dicRelatedValue.Add(item, values[item]);
            }
            if (dataNotExists && dicRelatedValue.ContainsKey("file") && dicRelatedValue["file"] == "-1")
            {
                dicRelatedValue.Add("FileMessage", "Data Contain 0 Attchment");
            }
            if (dataNotExists && dicRelatedValue.ContainsKey("master") && dicRelatedValue["master"] == "-1")
            {
                dicRelatedValue.Add("MasterMessage", "Data Is Empty");
            }
            var json = JsonConvert.SerializeObject(dicRelatedValue);
            record.RelatedInfo = json;
            _configDbContext.SaveChangesAsync(System.Threading.CancellationToken.None).GetAwaiter().GetResult();
        }
        public async Task<string> GetApiKey(ServiceInfoCategoryId serviceInfoCategoryId)
        {
            throw new NotImplementedException(); 
            //var key = $"ApiKey_{serviceInfoCategoryId}";
            //if (_memoryCache.TryGetValue(key, out string apiKey))
            //{
            //    return apiKey;
            //}
            //var category = await _configDbContext.ServiceInfoCategories.FirstOrDefaultAsync(w => w.ServiceInfoCategoryId == serviceInfoCategoryId);
            //if (string.IsNullOrEmpty(category.APIKey))
            //    return "0";
            //string decryptValue = _securityService.Decrypt(key, category.APIKey);
            //UpdateCacheKey(key, decryptValue);
            //return decryptValue;

        }
        public async Task UpdateApiKey(ServiceInfoCategoryId serviceInfoCategoryId, string apiKey)
        {
            throw new NotImplementedException();
            //var key = $"ApiKey_{serviceInfoCategoryId}";
            //var category = await _configDbContext.ServiceInfoCategories.FirstOrDefaultAsync(w => w.ServiceInfoCategoryId == serviceInfoCategoryId);
            //category.APIKey = _securityService.Encrypt(key, apiKey);
            //await _configDbContext.SaveChangesAsync(System.Threading.CancellationToken.None);
            //UpdateCacheKey(key, apiKey);
        }
        private void UpdateCacheKey(string key, string value)
        {
            _memoryCache.Set(key, value, TimeSpan.FromSeconds(5));
        }
    }
}
