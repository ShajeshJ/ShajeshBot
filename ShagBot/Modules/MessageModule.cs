using Discord.Commands;
using ImageMagick;
using ShagBot.Attributes;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShagBot.Modules
{
    public class MessageModule : ModuleBase<SocketCommandContext>
    {
        [Command("spoiler")]
        [RequireBotContext(CmdChannelType.GuildChannel)]
        [CmdSummary(nameof(Resource.SpoilerSummary), typeof(Resource))]
        [CmdRemarks(nameof(Resource.SpoilerRemarks), typeof(Resource))]
        public async Task CreateSpoilerMsg([Remainder]string message)
        {
            await Context.Message.DeleteAsync(); //Always want to delete spoiler messages, regardless of if they are valid

            if (message.Length > 300)
            {
                await ReplyAsync("Spoiler messages cannot be greater than 300 characters in length");
                return;
            }

            //Handle potential special escape characters (does not support new line currently)
            message = message.Replace("\r", "");
            message = message.Replace("\n", " ");

            var font = new Font(FontFamily.GenericSansSerif, 14);
            var brush = new SolidBrush(Color.FromArgb(153, 170, 181));
            var boundingBox = new RectangleF(0, 0, 500, 250);

            var hideImage = new Bitmap(500, 250);
            var showImage = new Bitmap(500, 250);

            var hideDrawing = Graphics.FromImage(hideImage);
            var showDrawing = Graphics.FromImage(showImage);

            hideDrawing.Clear(Color.FromArgb(44, 47, 51));
            showDrawing.Clear(Color.FromArgb(44, 47, 51));

            hideDrawing.DrawString("[Hover to Show Spoilers]", font, brush, boundingBox);
            showDrawing.DrawString(message, font, brush, boundingBox);

            hideDrawing.Save();
            showDrawing.Save();

            font.Dispose();
            brush.Dispose();
            hideDrawing.Dispose();
            showDrawing.Dispose();
            
            int spoilerIdx = 0;

            using (var collection = new MagickImageCollection())
            {
                collection.Add(new MagickImage(hideImage));
                collection.Add(new MagickImage(showImage));

                collection[0].AnimationDelay = -1;
                collection[1].AnimationDelay = 3_600_000;

                while (File.Exists($"spoiler{spoilerIdx}.gif"))
                {
                    spoilerIdx++;
                }
                
                collection.Write($"spoiler{spoilerIdx}.gif");
            }

            await Context.Channel.SendFileAsync($"spoiler{spoilerIdx}.gif", $"[Spoiler] {Context.User.Mention} said:");

            File.Delete($"spoiler{spoilerIdx}.gif");
        }
    }
}
