using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShagBot.Models
{
    public class PendingChannelModel
    {
        public ulong? CategoryId { get; set; }
        public ulong RequestUserId { get; set; }
        public string ChannelName { get; set; }
    }
}
