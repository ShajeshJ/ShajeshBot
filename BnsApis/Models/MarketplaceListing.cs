using BnsApis.Converters;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BnsApis.Models
{
    public class MarketplaceListing
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("listings")]
        public Listing[] Listings { get; set; }

        [JsonProperty("ISO")]
        public DateTime DateRetrieved { get; set; }
    }

    public class Listing
    {
        [JsonProperty("price")]
        [JsonConverter(typeof(GoldJsonConverter))]
        public Gold Price { get; set; }

        [JsonProperty("count")]
        public int Count { get; set; }

        [JsonProperty("each")]
        [JsonConverter(typeof(GoldJsonConverter))]
        public Gold PricePerItem { get; set; }
    }
}
