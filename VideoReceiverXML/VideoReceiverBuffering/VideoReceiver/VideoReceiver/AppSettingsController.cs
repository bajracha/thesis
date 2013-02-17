using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace VideoReceiver
{
    class AppSettingsController
    {
        public static string GetAppSetting(string key, string defaultValue)
        {
            return (ConfigurationSettings.AppSettings[key] != null) ? ConfigurationSettings.AppSettings[key] : defaultValue; 
        }

        public static int GetAppSetting(string key, int defaultValue)
        {
            int value = defaultValue;

            if (ConfigurationSettings.AppSettings[key] != null)
            {
                try
                {
                    value = Convert.ToInt32(ConfigurationSettings.AppSettings[key]);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error converting the value in the config file.\n" + e.Message + "\nUsing the Default Value: " + defaultValue + " for " + key);
                }
            }

            return value;
        }
    }
}
