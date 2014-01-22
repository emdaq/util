using System;
using System.Configuration;

namespace Emdaq.Util
{
    public static class ConfigManager
    {
        public static string GetConnString(string key)
        {
            var value = ConfigurationManager.ConnectionStrings[key];
            if (value == null)
            {
                throw new ConfigurationErrorsException("Missing connection string: " + key);
            }
            return value.ConnectionString;
        }

        public static T GetAppSetting<T>(string key, T defaultValue = default(T))
        {
            var value = ConfigurationManager.AppSettings[key];
            if (value == null)
            {
                return defaultValue;
            }
            return (T) Convert.ChangeType(value, typeof (T));
        }
    }
}