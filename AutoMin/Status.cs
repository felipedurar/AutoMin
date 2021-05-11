using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AutoMin
{
    public static class Status
    {
        public static bool SettingsChanged = false;
        //
        public static bool Running = true;
        public static int CurrentIdleSecs = 0;
        public static int TriggerSecs = 60;
        public static bool IsIdle = false;

        public static string GetSettingsFilePath()
        {
            string cPath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            return Path.Combine(cPath, "settings.dat");
        }

    }
}
