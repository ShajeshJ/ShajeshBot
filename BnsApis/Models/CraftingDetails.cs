using BnsApis.Converters;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BnsApis.Models
{
    public class CraftingDetails
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("createdBy")]
        public string CraftingSource { get; set; }

        [JsonProperty("item")]
        public string ItemName { get; set; }

        [JsonProperty("output")]
        public int Quantity { get; set; }

        [JsonProperty("cost")]
        [JsonConverter(typeof(GoldJsonConverter))]
        public Gold Cost { get; set; }

        [JsonProperty("hours")]
        public int CraftDuration { get; set; }

        [JsonProperty("ingredients")]
        public CraftingIngredient[] Ingredients { get; set; }

        [JsonProperty("attemptCost")]
        public CraftingIngredient[] AttemptIngredients { get; set; }
    }

    public class CraftingIngredient
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        
        [JsonProperty("quantity")]
        public int Quantity { get; set; }
    }
}
