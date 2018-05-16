using ShajeshBot.Extensions;
using ShajeshBot.Utilities;
using System.Linq;

namespace ShajeshBot
{
    public class GuildContext
    {
        public static readonly ulong AdminRoleId = CustomConfigManager.AppSettings["Admin_Role"].ToUInt64();
        public static readonly ulong GuildId = CustomConfigManager.AppSettings["Guild"].ToUInt64();
        public static readonly ulong BnsChannelId = CustomConfigManager.AppSettings["Bns_Channel"].ToUInt64();
        public static readonly ulong CmdChannelId = CustomConfigManager.AppSettings["Bot_Channel"].ToUInt64();
        public static readonly ulong[] CmdRoleIds = CustomConfigManager.AppSettings["Bot_AllowedRoles"]
                                                .Split('|').Select(x => x.Trim(' ').ToUInt64()).ToArray();
    }
}
