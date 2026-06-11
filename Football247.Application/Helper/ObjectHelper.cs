using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Football247.Application.Helper
{
    public static class ObjectHelper
    {
        public static IDictionary<string, string>? GetDictionary(this object? obj)
        {
            //JObject jObject = JsonConvert.DeserializeObject<JObject>(obj.Serialize());
            var json = JsonConvert.SerializeObject(obj);
            JObject jObject = JsonConvert.DeserializeObject<JObject>(json);

            if (jObject == null)
            {
                return null;
            }

            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            foreach (KeyValuePair<string, JToken> item in jObject)
            {
                dictionary.Add(item.Key, item.Value?.ToString() ?? string.Empty);
            }

            return dictionary;
        }
    }
}
