using BnsApis.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace BnsApis
{
    public class CraftingApi
    {
        private string _craftUrl;

        private string[] _craftSourceBlackList = { "process", "unsealing", "upgrade" };

        public CraftingApi()
        {
            _craftUrl = ConfigurationManager.AppSettings["BNS_BASE_URL"] + ConfigurationManager.AppSettings["CR_RESOURCE"];
        }

        public async Task<CraftingDetails[]> GetCraftingCost(string fullName)
        {
            var url = _craftUrl + $"?item={WebUtility.UrlEncode(fullName)}&active=TRUE";

            var client = new HttpClient();

            var response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var jsonBody = await response.Content.ReadAsStringAsync();

            var body = JsonConvert.DeserializeObject<CraftingDetails[]>(jsonBody);
            var craftList = body.Where(x => !_craftSourceBlackList.Contains(x.CraftingSource)).ToArray();

            return craftList;
        }
    }
}
