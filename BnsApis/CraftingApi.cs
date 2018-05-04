using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace BnsApis
{
    public class CraftingApi
    {
        private string _craftUrl;

        public CraftingApi()
        {
            _craftUrl = ConfigurationManager.AppSettings["BNS_BASE_URL"] + ConfigurationManager.AppSettings["CR_RESOURCE"];
        }

        public async Task GetCraftingCost(int id)
        {
            var url = _craftUrl + $"?id={id}&active=TRUE";

            var client = new HttpClient();

            var response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var jsonBody = await response.Content.ReadAsStringAsync();

            //var body = JsonConvert.DeserializeObject<MarketplaceListing[]>(jsonBody);
            //var listing = body.FirstOrDefault();

            //return listing;
        }
    }
}
