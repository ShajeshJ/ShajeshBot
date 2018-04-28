using BnsApis;
using Discord;
using Discord.Commands;
using ShagBot.Attributes;
using System;
using System.Linq;
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

        private string GoldIcon
        {
            get
            {
                return Context.Guild.Emotes.FirstOrDefault(e => e.Name == "bns_gold").ToString() ?? "gold";
            }
        }

        private string SilverIcon
        {
            get
            {
                return Context.Guild.Emotes.FirstOrDefault(e => e.Name == "bns_silver").ToString() ?? "silver";
            }
        }

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

            var questApi = new QuestApi();
            var dailies = await questApi.GetDailyChallenge(parsedDay);

            var embed = new EmbedBuilder();

            embed.Title = $"{parsedDay.ToString()}'s Dailies";

            foreach (var daily in dailies)
            {
                var field = new EmbedFieldBuilder();
                field.Name = daily.Quest;

                if (detailed)
                {
                    field.Value = $"{daily.Location}\r\n{daily.GoldPortion} {GoldIcon} {daily.SilverPortion} {SilverIcon}\r\n{daily.XP} XP";
                }
                else
                {
                    field.Value = daily.Location;
                }

                embed.AddField(field);
            }

            await ReplyAsync("", embed: embed.Build());
        }

        [Command("bns mp")]
        [Alias("mp")]
        [RequireBotContext(CmdChannelType.BnsChannel)]
        public async Task GetMarketplaceValue(string item)
        {

        }
    }
}
