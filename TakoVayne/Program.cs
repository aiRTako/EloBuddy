namespace TakoVayne_Loader
{
    using EloBuddy;
    using EloBuddy.SDK.Events;

    using System;
    using System.Drawing;
    using System.IO;
    using System.Reflection;

    internal class Program
    {
        private static readonly string getAppDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        private static readonly string dllPath = Path.Combine(getAppDataPath, @"EloBuddy\Addons\Libraries\TakoVayne.dll");

        private static void Main(string[] Args)
        {
            Loading.OnLoadingComplete += eventArgs =>
            {
                if (File.Exists(dllPath))
                {
                    File.Delete(dllPath);
                }

                var bydll = Properties.Resources.TakoVayne;
                using (var fs = new FileStream(dllPath, FileMode.Create))
                {
                    fs.Write(bydll, 0, bydll.Length);
                }

                var a = Assembly.LoadFrom(dllPath);
                var myType = a.GetType("TakoVayne.Loader");
                var main = myType.GetMethod("Main", BindingFlags.Public | BindingFlags.Static);

                main.Invoke(null, null);
            };
        }
    }
}
