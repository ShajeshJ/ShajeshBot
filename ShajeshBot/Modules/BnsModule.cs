﻿using BnsApis;
using BnsApis.Models;
using BnsApis.SheetApis;
using Discord;
using Discord.Commands;
using Google.Apis.Sheets.v4;
using ShajeshBot.Attributes;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;

namespace ShajeshBot.Modules
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
        [Alias("dailies", "daily")]
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
        [Alias("mp", "marketplace")]
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

        [Command("bns cr")]
        [Alias("cr", "crafting")]
        [RequireBotContext(CmdChannelType.BnsChannel)]
        [CmdSummary(nameof(Resource.BnsCrSummary), typeof(Resource))]
        [CmdRemarks(nameof(Resource.BnsCrRemarks), typeof(Resource))]
        public async Task GetCraftingCost(int amount, [Remainder]string item)
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
            var crApi = new CraftingApi();
            var craftRecipes = await crApi.GetCraftingCost(itemDetails.Name);

            var chosenRecipe = craftRecipes.Where(r => r.Quantity == amount).FirstOrDefault();

            if (chosenRecipe == null)
            {
                if (craftRecipes.Length == 0)
                {
                    await ReplyAsync($"No crafting recipes available for '{itemDetails.Name}'");
                }
                else
                {
                    var amountList = craftRecipes.Aggregate("", (accum, cur) => accum + cur.Quantity + ", ", accum => accum.TrimEnd(',', ' '));
                    await ReplyAsync($"Must choose one of the following craft amounts for '{itemDetails.Name}': {amountList}");
                }
                return;
            }

            var altRecipes = craftRecipes.Where(r => r.Quantity != chosenRecipe.Quantity);

            var mpApi = new MarketplaceApi();
            var tasks = new List<Task>();

            var mainItemListing = await mpApi.GetMarketplaceListing(itemDetails.Id);

            if ((mainItemListing?.Listings.Length ?? 0) == 0)
            {
                await ReplyAsync($"No maketplace listings available for '{itemDetails.Name}'");
                return;
            }

            var outputPrice = mainItemListing.Listings.First().PricePerItem * chosenRecipe.Quantity;

            if (chosenRecipe.Ingredients != null)
            {
                //Currently the drive only supports searching for items in a serial manner
                //Can avoid this slow linear search if/when the unofficial api returns ids along with item names
                for (int i = 0; i < chosenRecipe.Ingredients.Length; i++)
                {
                    var idx = i;
                    chosenRecipe.Ingredients[idx].Id = await itemSearchApi.GetItemId(chosenRecipe.Ingredients[idx].Name);

                    tasks.Add(Task.Run(async () =>
                    {
                        chosenRecipe.Ingredients[idx].Cost = new Gold(0);

                        //Some items may not be sellable items, or may not be listed on the marketplace; we ignore these
                        if (chosenRecipe.Ingredients[idx].Id != 0)
                        {
                            var listing = await mpApi.GetMarketplaceListing(chosenRecipe.Ingredients[idx].Id);

                            if ((listing?.Listings.Length ?? 0) != 0)
                            {
                                chosenRecipe.Ingredients[idx].Cost = listing.Listings.First().PricePerItem * chosenRecipe.Ingredients[idx].Quantity;
                            }
                        }
                    }));
                }
            }

            if (chosenRecipe.AttemptIngredients != null)
            {
                //Currently the drive only supports searching for items in a serial manner
                //Can avoid this slow linear search if/when the unofficial api returns ids along with item names
                for (int i = 0; i < chosenRecipe.AttemptIngredients.Length; i++)
                {
                    var idx = i;
                    chosenRecipe.AttemptIngredients[idx].Id = await itemSearchApi.GetItemId(chosenRecipe.AttemptIngredients[idx].Name);

                    tasks.Add(Task.Run(async () =>
                    {
                        chosenRecipe.AttemptIngredients[idx].Cost = new Gold(0);

                        //Some items may not be sellable items, or may not be listed on the marketplace; we ignore these
                        if (chosenRecipe.AttemptIngredients[idx].Id != 0)
                        {
                            var listing = await mpApi.GetMarketplaceListing(chosenRecipe.AttemptIngredients[idx].Id);

                            if ((listing?.Listings.Length ?? 0) != 0)
                            {
                                chosenRecipe.AttemptIngredients[idx].Cost = listing.Listings.First().PricePerItem 
                                                                            * chosenRecipe.AttemptIngredients[idx].Quantity;
                            }
                        }
                    }));
                }
            }

            await Task.WhenAll(tasks);

            var craftCost = chosenRecipe.Cost;

            var individualCostStr = $"{chosenRecipe.Cost.GoldPart} {GoldIcon} {chosenRecipe.Cost.SilverPart} {SilverIcon} - Crafting Price\r\n";

            if (chosenRecipe.Ingredients != null)
            {
                foreach (var ing in chosenRecipe.Ingredients)
                {
                    craftCost += ing.Cost;

                    if (ing.Cost == 0)
                    {
                        individualCostStr += $"No marketplace value available - {ing.Name} x {ing.Quantity}\r\n";
                    }
                    else
                    {
                        individualCostStr += $"{ing.Cost.GoldPart} {GoldIcon} {ing.Cost.SilverPart} {SilverIcon} - {ing.Name} x {ing.Quantity}\r\n";
                    }
                }
            }

            if (chosenRecipe.AttemptIngredients != null)
            {
                foreach (var atmpt in chosenRecipe.AttemptIngredients)
                {
                    craftCost += atmpt.Cost;

                    if (atmpt.Cost == 0)
                    {
                        individualCostStr += $"No marketplace value available - {atmpt.Name} x {atmpt.Quantity}\r\n";
                    }
                    else
                    {
                        individualCostStr += $"{atmpt.Cost.GoldPart} {GoldIcon} {atmpt.Cost.SilverPart} {SilverIcon} - {atmpt.Name} x {atmpt.Quantity}\r\n";
                    }
                }
            }

            var netCost = outputPrice - craftCost;

            var embed = new EmbedBuilder();

            embed.ThumbnailUrl = itemDetails.ImgUrl;
            embed.Title = itemDetails.Name;

            if (chosenRecipe.SuccessRate > 0.0 && chosenRecipe.SuccessRate < 1.0)
            {
                embed.Title += " (chance to fail)";
            }

            embed.Description = $"Net Earnings: {netCost.GoldPart} {GoldIcon} {netCost.SilverPart} {SilverIcon}";

            var altCraftAmtStr = altRecipes.Aggregate("", (accum, cur) => accum + cur.Quantity + ", ", accum => accum.TrimEnd(',', ' '));

            var craftSpecsStr = $"Source: {chosenRecipe.CraftingSource}";
            craftSpecsStr += chosenRecipe.CraftDuration > 0 ? $"\r\nDuration: {chosenRecipe.CraftDuration} hrs" : "";
            craftSpecsStr += string.IsNullOrEmpty(altCraftAmtStr) ? "" 
                                : $"\r\nAmount: {chosenRecipe.Quantity} (alternate craft amounts: {altCraftAmtStr})";

            embed.AddField("Crafting Information", craftSpecsStr, false);

            embed.AddField("Market Price", $"{outputPrice.GoldPart} {GoldIcon} {outputPrice.SilverPart} {SilverIcon}", true);
            embed.AddField("Crafting Cost", $"{craftCost.GoldPart} {GoldIcon} {craftCost.SilverPart} {SilverIcon}", true);

            embed.AddField("Individual Item Costs", individualCostStr, false);

            embed.Footer = new EmbedFooterBuilder()
            {
                Text = "Data retrieved from the Unofficial BnS API. Documentation: https://slate.silveress.ie/docs_bns"
            };

            await ReplyAsync("", embed: embed.Build());
        }
    }
}
