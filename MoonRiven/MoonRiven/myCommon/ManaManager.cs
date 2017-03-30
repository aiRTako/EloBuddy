namespace MoonRiven_2.myCommon
{
    using EloBuddy;
    using EloBuddy.SDK.Menu;
    using EloBuddy.SDK.Menu.Values;

    using Color = System.Drawing.Color;

    internal static class ManaManager
    {
        internal static bool SpellFarm;

        internal static bool HasEnoughMana(int manaPercent)
        {
            return Player.Instance.ManaPercent >= manaPercent; //&& !Player.Instance.IsUnderEnemyturret();
        }

        internal static void AddSpellFarm(Menu mainMenu)
        {
            mainMenu.AddSeparator();
            mainMenu.AddText("Moon Farm Logic");
            mainMenu.AddBool("SpellFarm", "Use Spell Farm(Mouse Score Control)");

            SpellFarm = mainMenu.GetBool("SpellFarm");

            Game.OnWndProc += delegate (WndEventArgs Args)
            {
                if (Args.Msg == 0x20a)
                {
                    mainMenu["SpellFarm"].Cast<CheckBox>().CurrentValue = !SpellFarm;
                    SpellFarm = mainMenu.GetBool("SpellFarm");
                }
            };
        }

        internal static void AddDrawFarm(Menu mainMenu)
        {
            mainMenu.AddSeparator();
            mainMenu.AddText("Draw Moon Farm Logic");
            mainMenu.AddBool("DrawFarm", "Draw Spell Farm Status");

            Drawing.OnDraw += delegate 
            {
                if (!Player.Instance.IsDead && !MenuGUI.IsChatOpen)
                {
                    if (mainMenu.GetBool("DrawFarm"))
                    {
                        var MePos = Drawing.WorldToScreen(Player.Instance.Position);

                        Drawing.DrawText(MePos[0] - 57, MePos[1] + 48, Color.FromArgb(242, 120, 34),
                            "Spell Farm:" + (SpellFarm ? "On" : "Off"));
                    }
                }
            };
        }
    }
}
