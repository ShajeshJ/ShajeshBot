using Discord.Commands;
using ShajeshBot.Extensions;
using System;
using System.Reflection;

namespace ShajeshBot.Attributes
{
    public class CmdSummaryAttribute : SummaryAttribute
    {
        public CmdSummaryAttribute(string text)
            :base(text)
        {

        }

        public CmdSummaryAttribute(string resourceName, Type resourceType)
            : base(resourceType.GetProperty(resourceName, 
                BindingFlags.Static
                | BindingFlags.NonPublic
                | BindingFlags.Public).GetValue(null).ToString().Unescape())
        {

        }
    }
}
