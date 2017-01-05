
using System;
using System.Collections.Generic;
using System.IO.IsolatedStorage;

namespace BaseTool
{
    /// <summary>
    /// 基于IsolatedStorageSettings的存储结构
    /// </summary>
    public static class GetAppSettings
    {
        public static IDictionary<string, string> Appsettings
        {
            get
            {
                IDictionary<string, string> di = new Dictionary<string, string>();
                try
                {
                    var appSettings = IsolatedStorageSettings.ApplicationSettings;
                    foreach (var o in appSettings.Keys)
                    {
                        var key = o.ToString();
                        di.Add(key, appSettings[key].ToString());
                    }
                }
                catch
                {
                    // ignored
                }
                return di;
            }
            set
            {
                if (value == null) throw new ArgumentNullException("value");
                Appsettings = value;
            }
        }
    }
}
