using Discord;
using Discord.Commands;
using Newtonsoft.Json;
using ShagBot.Attributes;
using ShagBot.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ShagBot.Modules
{
    public class BnsModule : ModuleBase<SocketCommandContext>
    {
        private static readonly string[][] daysOfWeekAliases =
        {
            new string[] { "0", "sunday", "sun" },
            new string[] { "1", "monday", "mon" }, 
            new string[] { "2", "tuesday", "tue" }, 
            new string[] { "3", "wednesday", "wed" }, 
            new string[] { "4", "thursday", "thur", "thu" }, 
            new string[] { "5", "friday", "fri" }, 
            new string[] { "6", "saturday", "sat" }
        };

        [Command("bns dailies")]
        [Alias("dailies")]
        [RequireBotContext(CmdChannelType.BnsChannel)]
        [CmdSummary(nameof(Resource.BnsDailiesSummary), typeof(Resource))]
        [CmdRemarks(nameof(Resource.BnsDailiesRemarks), typeof(Resource))]
        public async Task GetDailyQuests(string day = null, bool detailed = false)
        {
            DayOfWeek parsedDay = 0;

            //If no day specified, then return daily quests for today
            if (day == null)
            {
                var now = DateTime.UtcNow;
                parsedDay = now.DayOfWeek;

                //Dailies reset at 12:00pm UTC. If earlier than 12:00pm UTC, then return dailies for previous day
                if (now.TimeOfDay < TimeSpan.FromHours(12))
                {
                    if (parsedDay == Enum.GetValues(typeof(DayOfWeek)).Cast<DayOfWeek>().Min())
                    {
                        parsedDay = Enum.GetValues(typeof(DayOfWeek)).Cast<DayOfWeek>().Max();
                    }
                    else
                    {
                        parsedDay = parsedDay - 1;
                    }
                }
            }
            else
            {
                bool aliasFound = false;

                for (int i = 0; i < daysOfWeekAliases.Length && !aliasFound; i++)
                {
                    for (int j = 0; j < daysOfWeekAliases[i].Length && !aliasFound; j++)
                    {
                        if (day.ToLower() == daysOfWeekAliases[i][j])
                        {
                            aliasFound = true;
                            parsedDay = (DayOfWeek)i;
                        }
                    }
                }

                if (!aliasFound)
                {
                    await ReplyAsync("Invalid day of week.");
                    return;
                }
            }

            var url = $"https://api.silveress.ie/bns/v3/dungeons/quests?daily_challenge={parsedDay.ToString()}";

            var client = new HttpClient();

            var response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var jsonBody = await response.Content.ReadAsStringAsync();

            var body = JsonConvert.DeserializeObject<DailyQuest[]>(jsonBody);
            var dailies = body.OrderByDescending(d => d.gold).ToList();

            var embed = new EmbedBuilder();

            embed.Title = $"{parsedDay.ToString()}'s Dailies";

            foreach (var daily in dailies)
            {
                var copper = daily.gold % 100;
                var silver = (daily.gold / 100) % 100;
                var gold = (daily.gold / 10000) % 100;

                var field = new EmbedFieldBuilder();
                field.Name = daily.quest;

                if (detailed)
                {
                    field.Value = $"{daily.location}\r\n{gold} gold | {silver} silver | {copper} copper\r\n{daily.xp} XP";
                }
                else
                {
                    field.Value = daily.location;
                }

                embed.AddField(field);
            }

            await ReplyAsync("", embed: embed.Build());
        }
    }

    class DailyQuest
    {
        public string quest { get; set; }
        public string location { get; set; }
        public int gold { get; set; }
        public string xp { get; set; }
    }
}
