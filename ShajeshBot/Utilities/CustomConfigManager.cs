using ShajeshBot.Extensions;
using System.Collections.Generic;
using System.Configuration;

namespace ShajeshBot.Utilities
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
