namespace MoonEzreal_Loader
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
                                                 @"\AppData\Roaming\EloBuddy\Addons\Libraries\MoonEzreal.dll";

        private const string dllAddress = "https://raw.githubusercontent.com/aiRTako/MyAddonDB/master/Ezreal/MoonEzreal.dll";
        private const string dllVersion = "https://raw.githubusercontent.com/aiRTako/MyAddonDB/master/Ezreal/MoonEzreal.txt";

        private static void Main(string[] Args)
        {
            Loading.OnLoadingComplete += eventArgs =>
            {
                if (!File.Exists(dllPath))
                {
                    Chat.Print("Moon Ezreal: Now Download the Addon, please waiting...", Color.Orange);
                    DownloadAddon();
                }
                else
                {
                    var GitVersion = DownloadVersion();

                    var myAddon = Assembly.LoadFrom(dllPath);
                    var myVersion = myAddon.GetName().Version.ToString();

                    if (GitVersion != myVersion)
                    {
                        Chat.Print("Moon Ezreal: Have a new Update for this Addon, now Download the new Version", Color.Orange);
                        DownloadAddon();
                        return;
                    }

                    var myType = myAddon.GetType("i");
                    var main = myType.GetMethod("a", BindingFlags.NonPublic | BindingFlags.Static);
                    main.Invoke(null, null);
                    Chat.Print("Moon Ezreal: Load Successful, Enjoy the Time", Color.Orange);
                }
            };
        }

        private static void DownloadAddon()
        {
            using (var web = new WebClient())
            {
                web.DownloadFile(dllAddress, dllPath);
            }

            Chat.Print("Moon Ezreal: Download Successful... Please F5 Reload the Addon!", Color.Orange);
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
