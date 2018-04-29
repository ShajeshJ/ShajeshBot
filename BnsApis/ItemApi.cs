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
    public class ItemApi
    {
        private string _itemUrl;

        public ItemApi()
        {
            _itemUrl = ConfigurationManager.AppSettings["BNS_BASE_URL"] + ConfigurationManager.AppSettings["ITEM_RESOURCE"];
        }

        public async Task<ItemDetails> GetItemDetails(int id)
        {
            var url = _itemUrl + $"/{id}";

            var client = new HttpClient();

            var response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var jsonBody = await response.Content.ReadAsStringAsync();

            var body = JsonConvert.DeserializeObject<ItemDetails[]>(jsonBody);
            var item = body.FirstOrDefault();

            return item;
        }
    }
}
