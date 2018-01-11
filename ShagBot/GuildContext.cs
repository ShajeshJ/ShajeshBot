using ShagBot.Extensions;
using ShagBot.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShagBot
{
    public class GuildContext
    {
        public static readonly ulong AdminRoleId = CustomConfigManager.AppSettings["Admin_Role"].ToUInt64();
        public static readonly ulong GuildId = CustomConfigManager.AppSettings["Guild"].ToUInt64();
        public static readonly ulong CmdChannel = CustomConfigManager.AppSettings["Bot_Channel"].ToUInt64();
        public static readonly ulong[] CmdRoleIds = CustomConfigManager.AppSettings["Bot_AllowedRoles"]
                                                .Split('|').Select(x => x.Trim(' ').ToUInt64()).ToArray();
    }
}
