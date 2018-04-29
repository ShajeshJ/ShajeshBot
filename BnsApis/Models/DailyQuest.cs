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
        public int Gold { get; set; }

        #region Gold Division Properties

        public int GoldPortion
        {
            get
            {
                return Gold / 10000;
            }
        }
        public int SilverPortion
        {
            get
            {
                return (Gold / 100) % 100;
            }
        }
        public int CopperPortion
        {
            get
            {
                return Gold % 100;
            }
        }

        #endregion

        [JsonProperty("xp")]
        public string XP { get; set; }
    }
}
