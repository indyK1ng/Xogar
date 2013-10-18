using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XogarLib
{
    public class Configuration
    {
        public static void SetSteamPath(string installPath)
        {
            Properties.Settings.Default.SteamInstallDirectory = installPath;
            Properties.Settings.Default.Save();
        }
    }
}
