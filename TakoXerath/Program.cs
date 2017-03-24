namespace TakoXerath_Loader
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
                                                 @"\AppData\Roaming\EloBuddy\Addons\Libraries\TakoXerath.dll";

        private const string dllAddress = "https://raw.githubusercontent.com/aiRTako/MyAddonDB/master/Xerath/TakoXerath.dll";
        private const string dllVersion = "https://raw.githubusercontent.com/aiRTako/MyAddonDB/master/Xerath/TakoXerath.txt";

        private static void Main(string[] Args)
        {
            Loading.OnLoadingComplete += eventArgs =>
            {
                try
                {
                    if (!File.Exists(dllPath))
                    {
                        Chat.Print("Tako Xerath: Now Download the Addon, please waiting...", Color.Orange);
                        DownloadAddon();
                    }
                    else
                    {
                        var GitVersion = DownloadVersion();

                        var myAddon = Assembly.LoadFrom(dllPath);
                        var myVersion = myAddon.GetName().Version.ToString();

                        if (GitVersion != myVersion)
                        {
                            Chat.Print("Tako Xerath: Have a new Update for this Addon, now Download the new Version", Color.Orange);
                            DownloadAddon();
                            return;
                        }

                        var myType = myAddon.GetType("h");
                        var main = myType.GetMethod("a", BindingFlags.NonPublic | BindingFlags.Static);

                        if (main != null)
                        {
                            main.Invoke(null, null);
                            Chat.Print("Tako Xerath: Load Successful, Enjoy the Time", Color.Orange);
                        }
                        else
                        {
                            Chat.Print("Tako Xerath: The Addon Inject Error", Color.Orange);
                        }
                    }
                }
                catch
                {
                    Chat.Print("Tako Xerath: Please Check you Internet, Search the Addon Error", Color.Orange);
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

            Chat.Print("Tako Xerath: Download Successful... Please F5 Reload the Addon!", Color.Orange);
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