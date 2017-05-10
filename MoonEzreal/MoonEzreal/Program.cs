namespace MoonEzreal_Loader
{
    using EloBuddy;
    using EloBuddy.SDK.Events;

    using System;
    using System.Drawing;
    using System.IO;
    using System.Reflection;

    internal class Program
    {
        private static readonly string dllPath = @"C:\Users\" + Environment.UserName + @"\AppData\Roaming\EloBuddy\Addons\Libraries\MoonEzreal.dll";

        private static void Main(string[] Args)
        {
            Loading.OnLoadingComplete += eventArgs =>
            {
                if (File.Exists(dllPath))
                {
                    File.Delete(dllPath);
                }

                var bydll = Properties.Resources.MoonEzreal;
                using (var fs = new FileStream(dllPath, FileMode.Create))
                {
                    fs.Write(bydll, 0, bydll.Length);
                }

                var dllpath = Assembly.LoadFrom(dllPath);
                var main = dllpath.GetType("h").GetMethod("a", BindingFlags.NonPublic | BindingFlags.Static);

                main.Invoke(null, null);
                Chat.Print("Moon Ezreal: Load Successful, Enjoy the Time", Color.Orange);
            };
        }
    }
}
