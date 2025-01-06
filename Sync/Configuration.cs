using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sync
{
    public interface IConfiguration
    {
        string GetValue(string key);
    }

    internal class Configuration : IConfiguration
    {
        public Configuration(string apiKey) 
        {
            Values = new Dictionary<string, string>()
            {
                { "VirtuousApiBaseUrl", "https://api.virtuoussoftware.com" },
                { "VirtuousApiKey", apiKey }
            };
        }

        private Dictionary<string, string> Values { get; set; }

        public string GetValue(string key)
        {
            return Values[key];
        }
    }
}
