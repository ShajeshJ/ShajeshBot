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
    public class QuestApi
    {
        private string _questUrl;

        public QuestApi()
        {
            _questUrl = ConfigurationManager.AppSettings["BNS_BASE_URL"] + ConfigurationManager.AppSettings["QUEST_RESOURCE"];
        }

        public async Task<List<DailyQuest>> GetDailyChallenge(DayOfWeek day)
        {
            var url = _questUrl + $"?daily_challenge={day.ToString()}";

            var client = new HttpClient();

            var response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var jsonBody = await response.Content.ReadAsStringAsync();

            var body = JsonConvert.DeserializeObject<DailyQuest[]>(jsonBody);
            var dailies = body.OrderByDescending(d => d.GoldReward).ToList();

            return dailies;
        }
    }
}
