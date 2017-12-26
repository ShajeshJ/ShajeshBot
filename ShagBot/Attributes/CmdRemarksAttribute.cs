using Discord.Commands;
using ShagBot.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ShagBot.Attributes
{
    public class CmdRemarksAttribute : RemarksAttribute
    {
        public CmdRemarksAttribute(string text)
            :base(text)
        {

        }

        public CmdRemarksAttribute(string resourceName, Type resourceType)
            : base(resourceType.GetProperty(resourceName,
                BindingFlags.Static
                | BindingFlags.NonPublic
                | BindingFlags.Public).GetValue(null).ToString().Unescape())
        {

        }
    }
}
