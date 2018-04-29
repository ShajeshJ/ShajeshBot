using BnsApis.Models;
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
    public class MarketplaceApi
    {
        private string _mpUrl;

        public MarketplaceApi()
        {
            _mpUrl = ConfigurationManager.AppSettings["BNS_BASE_URL"] + ConfigurationManager.AppSettings["MP_RESOURCE"];
        }

        public async Task<MarketplaceListing> GetMarketplaceListing(int id)
        {
            var url = _mpUrl + $"/{id}";

            var client = new HttpClient();

            var response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var jsonBody = await response.Content.ReadAsStringAsync();

            var body = JsonConvert.DeserializeObject<MarketplaceListing[]>(jsonBody);
            var listing = body.FirstOrDefault();

            return listing;
        }
    }
}
