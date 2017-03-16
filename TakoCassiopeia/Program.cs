namespace TakoCassiopeia_Loader
{
    using EloBuddy;
    using EloBuddy.SDK.Events;

    using System;
    using System.Drawing;
    using System.IO;
    using System.Net;
    using System.Reflection;

    internal class Program
    {
        private static readonly string dllPath = @"C:\Users\" + Environment.UserName +
                                                 @"\AppData\Roaming\EloBuddy\Addons\Libraries\TakoCassiopeia.dll";

        private const string dllAddress = "https://raw.githubusercontent.com/aiRTako/MyAddonDB/master/Cassiopeia/TakoCassiopeia.dll";
        private const string dllVersion = "https://raw.githubusercontent.com/aiRTako/MyAddonDB/master/Cassiopeia/TakoCassiopeia.txt";

        private static void Main(string[] Args)
        {
            Loading.OnLoadingComplete += eventArgs =>
            {
                try
                {
                    if (!File.Exists(dllPath))
                    {
                        Chat.Print("Tako Cassiopeia: Now Download the Addon, please waiting...", Color.Orange);
                        DownloadAddon();
                    }
                    else
                    {
                        var GitVersion = DownloadVersion();

                        var myAddon = Assembly.LoadFrom(dllPath);
                        var myVersion = myAddon.GetName().Version.ToString();

                        if (GitVersion != myVersion)
                        {
                            Chat.Print("Tako Cassiopeia: Have a new Update for this Addon, now Download the new Version", Color.Orange);
                            DownloadAddon();
                            return;
                        }

                        var myType = myAddon.GetType("\uFDD8");
                        var main = myType.GetMethod("\uFDD0", BindingFlags.NonPublic | BindingFlags.Static);
                        main.Invoke(null, null);
                        Chat.Print("Tako Cassiopeia: Load Successful, Enjoy the Time", Color.Orange);
                    }
                }
                catch
                {
                    Chat.Print("Tako Cassiopeia: Please Check you Internet, Search the Addon Error", Color.Orange);
                }
            };
        }

        private static void DownloadAddon()
        {
            if (File.Exists(dllPath))
            {
                File.Delete(dllPath);
            }

            using (var web = new WebClient())
            {
                web.DownloadFile(dllAddress, dllPath);
            }

            Chat.Print("Tako Cassiopeia: Download Successful... Please F5 Reload the Addon!", Color.Orange);
        }

        private static string DownloadVersion()
        {
            using (var web = new WebClient())
            {
                var version = web.DownloadString(dllVersion);

                return version;
            }
        }
    }
}