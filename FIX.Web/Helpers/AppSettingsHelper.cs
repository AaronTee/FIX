using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace FIX.Web.Helpers
{
    public static class AppSettingsHelper
    {
        public static string GetKeyValue(string key)
        {
            try
            {
                return ConfigurationManager.AppSettings[key].ToString();
            }
            catch(Exception ex)
            {
                return string.Empty;
            }
        }
    }
}