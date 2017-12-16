using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShagBot.Models
{
    public class PendingEmojiModel
    {
        public string Url { get; set; }
        public ulong RequestUserId { get; set; }
        public string Shortcut { get; set; }
    }
}
