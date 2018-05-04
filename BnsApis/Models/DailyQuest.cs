using BnsApis.Converters;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BnsApis.Models
{
    public class DailyQuest
    {
        [JsonProperty("quest")]
        public string Quest { get; set; }

        [JsonProperty("location")]
        public string Location { get; set; }

        [JsonProperty("gold")]
        [JsonConverter(typeof(GoldJsonConverter))]
        public Gold GoldReward { get; set; }

        [JsonProperty("xp")]
        public string XP { get; set; }
    }
}
