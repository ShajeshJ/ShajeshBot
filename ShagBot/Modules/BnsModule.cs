using BnsApis;
using BnsApis.SheetApis;
using Discord;
using Discord.Commands;
using Google.Apis.Sheets.v4;
using ShagBot.Attributes;
using System;
using System.Configuration;
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

        SheetsService _ssService;

        public BnsModule(SheetsService sheetService)
        {
            _ssService = sheetService;
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
            
            embed.Author = new EmbedAuthorBuilder()
            {
                Name = $"{parsedDay.ToString()}'s Dailies",
                IconUrl = ConfigurationManager.AppSettings["DAILY_QUEST_ICON"]
            };

            foreach (var daily in dailies)
            {
                var field = new EmbedFieldBuilder();
                field.Name = daily.Quest;

                if (detailed)
                {
                    field.Value = $"{daily.Location}\r\n{daily.GoldReward.GoldPart} {GoldIcon} {daily.GoldReward.SilverPart} {SilverIcon}\r\n{daily.XP} XP";
                }
                else
                {
                    field.Value = daily.Location;
                }

                embed.AddField(field);
            }

            embed.Footer = new EmbedFooterBuilder()
            {
                Text = "Data retrieved from the Unofficial BnS API. Documentation: https://slate.silveress.ie/docs_bns"
            };

            await ReplyAsync("", embed: embed.Build());
        }

        [Command("bns mp")]
        [Alias("mp")]
        [RequireBotContext(CmdChannelType.BnsChannel)]
        [CmdSummary(nameof(Resource.BnsMpSummary), typeof(Resource))]
        [CmdRemarks(nameof(Resource.BnsMpRemarks), typeof(Resource))]
        public async Task GetMarketplaceValue([Remainder]string item)
        {
            //Search for the item through personal sheets to find corresponding id
            var itemSearchApi = new ItemSearchApi(_ssService);
            var id = await itemSearchApi.GetItemId(item);

            if (id == 0)
            {
                await ReplyAsync($"Could not find item with search key '{item}'");
                return;
            }

            //Use item id to retrieve item details (in particular, image for the item)
            var itemApi = new ItemApi();
            var itemDetails = await itemApi.GetItemDetails(id);

            if (itemDetails == null)
            {
                await ReplyAsync($"Could not find item with search key '{item}'");
                return;
            }

            //Use item id to retrieve current marketplace value of the item
            var mpApi = new MarketplaceApi();
            var listings = await mpApi.GetMarketplaceListing(id);

            var embed = new EmbedBuilder();

            embed.ThumbnailUrl = itemDetails.ImgUrl;
            embed.Title = itemDetails.Name;

            var maxListing = Math.Min(10, listings?.Listings.Length ?? 0);

            var formattedListings = "";

            for (int i = 0; i < maxListing; i++)
            {
                var listing = listings.Listings[i];

                formattedListings += $"{listing.PricePerItem.GoldPart} {GoldIcon} {listing.PricePerItem.SilverPart} {SilverIcon} x {listing.Count}";
                formattedListings += $"   |   {listing.Price.Total/10000.0} g Total\r\n";
            }

            formattedListings = string.IsNullOrEmpty(formattedListings) ? "<No listings available>" : formattedListings;

            embed.AddField(new EmbedFieldBuilder()
            {
                Name = $"Lowest marketplace listings for {itemDetails.Name}.",
                Value = formattedListings
            });

            embed.Footer = new EmbedFooterBuilder()
            {
                Text = "Data retrieved from the Unofficial BnS API. Documentation: https://slate.silveress.ie/docs_bns"
            };
            
            if (listings != null)
            {
                embed.Timestamp = new DateTimeOffset(listings.DateRetrieved);
            }

            await ReplyAsync("", embed: embed.Build());
        }
    }
}
