namespace MoonEzreal
{
    using System;

    using EloBuddy;
    using EloBuddy.SDK.Events;

    internal class Program
    {
        private static void Main(string[] Args)
        {
            Loading.OnLoadingComplete += OnLoadingComplete;
        }

        private static void OnLoadingComplete(EventArgs Args)
        {
            if (Player.Instance.Hero != Champion.Ezreal)
            {
                return;
            }

            Ezreal.Init();
        }
    }
}
