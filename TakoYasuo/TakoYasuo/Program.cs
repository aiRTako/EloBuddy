namespace TakoYasuo
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
                                                 @"\AppData\Roaming\EloBuddy\Addons\Libraries\TakoYasuo.dll";

        private const string dllAddress = "https://raw.githubusercontent.com/aiRTako/MyAddonDB/master/Yasuo/TakoYasuo.dll";
        private const string dllVersion = "https://raw.githubusercontent.com/aiRTako/MyAddonDB/master/Yasuo/TakoYasuo.txt";

        private static void Main(string[] Args)
        {
            Loading.OnLoadingComplete += eventArgs =>
            {
                try
                {
                    if (!File.Exists(dllPath))
                    {
                        Chat.Print("TakoYasuo: 鐜板湪姝ｅ湪涓嬭浇鑴氭湰鏁版嵁, Waiting.....", Color.Orange);
                        Chat.Print("TakoYasuo: Now Download the Addon, Waiting.....", Color.Orange);
                        DownloadAddon();
                    }
                    else
                    {
                        var GitVersion = DownloadVersion();
                        const string myVersion = "1.0.0.1";

                        if (myVersion != GitVersion)
                        {
                            Chat.Print("TakoYasuo: 妫€鏌ュ埌鏇存柊, " + "寮€濮嬫洿鏂扮増鏈腑!", Color.Red);
                            Chat.Print("TakoYasuo: Your Version is Outdate! Now Download the New Version!", Color.Red);
                            DownloadAddon();
                            return;
                        }

                        var myAddon = Assembly.LoadFrom(dllPath);
                        var myType = myAddon.GetType("l", false);
                        var main = myType.GetMethod("a", BindingFlags.NonPublic | BindingFlags.Static);

                        if (main != null)
                        {
                            main.Invoke(null, null);
                        }
                        else
                        {
                            Chat.Print("TakoYasuo: Error! Please Clean Your Appdata, and Reload the Addon", Color.Red);
                        }
                    }
                }
                catch
                {
                    Chat.Print("TakoYasuo: 缃戠粶杩炴帴澶辫触! 璇锋鏌ヤ綘鐨勭綉缁滈棶棰樺啀鎸変竴娆5", Color.Red);
                    Chat.Print("TakoYasuo: Please Check Your Internet and Clean your Appdata", Color.Red);
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

            Chat.Print("TakoYasuo: 鏇存柊鎴愬姛! 璇锋寜涓€娆5閲嶆柊鍔犺浇", Color.Orange);
            Chat.Print("TakoYasuo: Now Download Successful! Please Press F5 Reload the Addon", Color.Orange);
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
