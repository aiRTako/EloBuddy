namespace MoonRiven
{
    using EloBuddy;
    using EloBuddy.SDK.Events;

    using System;

    internal class Program
    {
        private static void Main(string[] Args)
        {
            Loading.OnLoadingComplete += OnLoadingComplete;
        }

        private static void OnLoadingComplete(EventArgs Args)
        {
            if (Player.Instance.Hero != Champion.Riven)
            {
                return;
            }

            Riven.Init();
        }
    }
}
