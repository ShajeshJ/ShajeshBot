using ShagBot.Extensions;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShagBot.Utilities
{
    public class CustomConfigManager
    {
        public static AppSettings AppSettings = new AppSettings();
    }

    public class AppSettings
    {
        public string this[string key]
        {
            get
            {
                var val = ConfigurationManager.AppSettings[key];

                if (val.IsNullOrWhitespace())
                {
                    throw new KeyNotFoundException($"The key '{key}' was not set properly.");
                }

                return val;
            }
        }
    }
}
