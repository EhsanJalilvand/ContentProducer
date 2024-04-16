using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System
{
    public static class JsonValidator
    {
        public static T IsValidJson<T>(this string strInput) where T : class
        {
            if (string.IsNullOrWhiteSpace(strInput)) return null;

            strInput = strInput.Trim();
            if ((strInput.StartsWith("{") && strInput.EndsWith("}")) || //For object
                (strInput.StartsWith("[") && strInput.EndsWith("]"))) //For array
            {
                try
                {
                    var aa = typeof(T);
                    var obj = System.Text.Json.JsonSerializer.Deserialize<T>(strInput);
                    return obj;
                }
                catch // not valid
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }
    }
}
