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
        public int Price { get; set; }

        #region Price Gold Divisions

        public int Price_Gold
        {
            get
            {
                return Price / 10000;
            }
        }
        public int Price_Silver
        {
            get
            {
                return (Price / 100) % 100;
            }
        }
        public int Price_Copper
        {
            get
            {
                return Price % 100;
            }
        }

        #endregion

        [JsonProperty("count")]
        public int Count { get; set; }

        [JsonProperty("each")]
        public int PricePerItem { get; set; }

        #region PricePerItem Gold Divisions

        public int PricePerItem_Gold
        {
            get
            {
                return PricePerItem / 10000;
            }
        }
        public int PricePerItem_Silver
        {
            get
            {
                return (PricePerItem / 100) % 100;
            }
        }
        public int PricePerItem_Copper
        {
            get
            {
                return PricePerItem % 100;
            }
        }

        #endregion
    }
}
