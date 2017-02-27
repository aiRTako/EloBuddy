namespace MoonEzreal.myCommon
{
    using EloBuddy;
    using EloBuddy.SDK;
    using EloBuddy.SDK.Menu;
    using EloBuddy.SDK.Menu.Values;

    using Color = System.Drawing.Color;

    internal static class ManaManager
    {
        private static Menu FarmMenu;
        private static Menu drawMenu;

        internal static bool SpellFarm;
        internal static bool SpellHarass;

        internal static bool HasEnoughMana(int manaPercent)
        {
            return Player.Instance.ManaPercent >= manaPercent;// && !Player.Instance.IsUnderEnemyturret();
        }

        internal static void AddSpellFarm(Menu mainMenu)
        {
            FarmMenu = mainMenu;

            mainMenu.AddSeparator();
            mainMenu.AddText("Moon Farm Logic");
            mainMenu.AddBool("SpellFarm", "Use Spell Farm(Mouse Score Control)");
            var key = mainMenu.Add("SpellHarass", new KeyBind("Use Spell Harass Enemy(in Farm Mode)", true, KeyBind.BindTypes.PressToggle, 'H'));

            SpellFarm = mainMenu.GetBool("SpellFarm");
            SpellHarass = mainMenu.GetKey("SpellHarass");

            key.OnValueChange += delegate (ValueBase<bool> sender, ValueBase<bool>.ValueChangeArgs Args)
            {
                SpellHarass = Args.NewValue;
            };

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
            drawMenu = mainMenu;

            mainMenu.AddSeparator();
            mainMenu.AddText("Draw Moon Farm Logic");
            mainMenu.AddBool("DrawFarm", "Draw Spell Farm Status");
            mainMenu.AddBool("DrawHarass", "Draw Spell Harass Status");

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

                    if (drawMenu.GetBool("DrawHarass"))
                    {
                        var MePos = Drawing.WorldToScreen(Player.Instance.Position);

                        Drawing.DrawText(MePos[0] - 57, MePos[1] + 68, Color.FromArgb(242, 120, 34),
                            "Spell Haras:" + (SpellHarass ? "On" : "Off"));
                    }
                }
            };
        }
    }
}
