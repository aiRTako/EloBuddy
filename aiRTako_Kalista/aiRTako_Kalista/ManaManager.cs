namespace aiRTako_Kalista
{
    using EloBuddy;
    using EloBuddy.SDK.Menu;
    using EloBuddy.SDK.Menu.Values;

    using Color = System.Drawing.Color;

    internal static class ManaManager
    {
        internal static bool SpellFarm;
        internal static bool SpellHarass;

        internal static void AddSpellFarm(Menu mainMenu)
        {
            mainMenu.AddSeparator();
            mainMenu.AddGroupLabel("Farm Settings");
            mainMenu.Add("SpellFarm", new CheckBox("Use Spell Farm(Mouse Score Control)"));
            var key = mainMenu.Add("SpellHarass",
                new KeyBind("Use Spell Harass Enemy(in Farm Mode)", true, KeyBind.BindTypes.PressToggle, 'H'));

            SpellFarm = mainMenu["SpellFarm"].Cast<CheckBox>().CurrentValue;
            SpellHarass = mainMenu["SpellHarass"].Cast<KeyBind>().CurrentValue;

            key.OnValueChange += delegate (ValueBase<bool> sender, ValueBase<bool>.ValueChangeArgs Args)
            {
                SpellHarass = Args.NewValue;
            };

            Game.OnWndProc += delegate (WndEventArgs Args)
            {
                if (Args.Msg == 0x20a)
                {
                    mainMenu["SpellFarm"].Cast<CheckBox>().CurrentValue = !SpellFarm;
                    SpellFarm = mainMenu["SpellFarm"].Cast<CheckBox>().CurrentValue;
                }
            };
        }

        internal static void AddDrawFarm(Menu mainMenu)
        {
            mainMenu.AddSeparator();
            mainMenu.AddLabel("Draw Farm Logic");
            mainMenu.Add("DrawFarm", new CheckBox("Draw Spell Farm Status"));
            mainMenu.Add("DrawHarass", new CheckBox("Draw Spell Harass Status"));

            Drawing.OnDraw += delegate 
            {
                if (!Player.Instance.IsDead && !MenuGUI.IsChatOpen)
                {
                    if (mainMenu["DrawFarm"].Cast<CheckBox>().CurrentValue)
                    {
                        var MePos = Drawing.WorldToScreen(Player.Instance.Position);

                        Drawing.DrawText(MePos[0] - 57, MePos[1] + 48, Color.FromArgb(242, 120, 34),
                            "Spell Farm:" + (SpellFarm ? "On" : "Off"));
                    }

                    if (mainMenu["DrawHarass"].Cast<CheckBox>().CurrentValue)
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
