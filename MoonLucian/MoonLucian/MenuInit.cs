namespace MoonLucian
{
    using myCommon;

    using System.Linq;

    using EloBuddy;
    using EloBuddy.SDK;
    using EloBuddy.SDK.Menu;
    using EloBuddy.SDK.Menu.Values;

    internal class MenuInit : Logic
    {
        internal static void Init()
        {
            mainMenu = MainMenu.AddMenu("Moon" + Player.Instance.ChampionName, "Moon" + Player.Instance.ChampionName);
            {
                mainMenu.AddGroupLabel("pls setting the Orbwalker");
                mainMenu.AddGroupLabel("Orbwalker -> Advanced -> Update event listening -> Enabled On Update(more fast)");
                mainMenu.AddGroupLabel("--------------------");
                mainMenu.AddGroupLabel("My GitHub: https://github.com/NightMoon032/MoonSeries");
                mainMenu.AddGroupLabel("If you have Feedback pls post to my topic");
                mainMenu.AddGroupLabel("---------------------");
                mainMenu.AddGroupLabel("Credit: NightMoon");
            }

            comboMenu = mainMenu.AddSubMenu("Combo", "Combo");
            {
                comboMenu.AddLine("Q");
                comboMenu.AddBool("ComboQ", "Use Q");
                comboMenu.AddBool("ComboQExtend", "Use Q Extend");
                comboMenu.AddLine("W");
                comboMenu.AddBool("ComboW", "Use W");
                comboMenu.AddBool("ComboWFast", "Use W Fast Cast to Reset the Passive");
                comboMenu.AddLine("E");
                comboMenu.AddBool("ComboEDash", "Use E Dash to target");
                comboMenu.AddBool("ComboEReset", "Use E Reset Auto Attack");
                comboMenu.AddBool("ComboESafe", "Use E| Safe Check");
                comboMenu.AddBool("ComboEWall", "Use E| Dont Dash to Wall");
                comboMenu.AddLine("R");
                comboMenu.AddBool("ComboR", "Use R");
            }

            harassMenu = mainMenu.AddSubMenu("Harass", "Harass");
            {
                harassMenu.AddLine("Q");
                harassMenu.AddBool("HarassQ", "Use Q");
                harassMenu.AddBool("HarassQExtend", "Use Q Extend");
                harassMenu.AddLine("W");
                harassMenu.AddBool("HarassW", "Use W", false);
                harassMenu.AddLine("Mana");
                harassMenu.AddSlider("HarassMP", "When Player Mana Percent >= x%, Enabled Spell Harass", 60);
                harassMenu.AddLine("Harass Target");
                if (EntityManager.Heroes.Enemies.Any())
                {
                    foreach (var target in EntityManager.Heroes.Enemies)
                    {
                        harassMenu.AddBool("Harasstarget" + target.ChampionName.ToLower(), target.ChampionName);
                    }
                }
            }

            clearMenu = mainMenu.AddSubMenu("Clear", "Clear");
            {
                clearMenu.AddText("LaneClear Settings");
                clearMenu.AddLine("LaneClear Q");
                clearMenu.AddBool("LaneClearQ", "Use Q");
                clearMenu.AddLine("LaneClear W");
                clearMenu.AddBool("LaneClearW", "Use W", false);
                clearMenu.AddLine("LaneClearMana");
                clearMenu.AddSlider("LaneClearMP", "When Player Mana Percent >= x%, Enabled LaneClear Spell", 60);

                clearMenu.AddSeparator();

                clearMenu.AddText("PushTurret Settings");
                clearMenu.AddLine("TurretClear W");
                clearMenu.AddBool("TurretClearW", "Use W");
                clearMenu.AddLine("TurretClear E");
                clearMenu.AddBool("TurretClearE", "Use E");
                clearMenu.AddLine("TurretClearMana");
                clearMenu.AddSlider("TurretClearMP", "When Player Mana Percent >= x%, Enabled TurretClear Spell", 60);

                clearMenu.AddSeparator();

                clearMenu.AddText("JungleClear Settings");
                clearMenu.AddLine("JungleClear Q");
                clearMenu.AddBool("JungleClearQ", "Use Q");
                clearMenu.AddLine("JungleClear W");
                clearMenu.AddBool("JungleClearW", "Use W");
                clearMenu.AddLine("JungleClear E");
                clearMenu.AddBool("JungleClearE", "Use E");
                clearMenu.AddSlider("JungleClearMP", "When Player Mana Percent >= x%, Enabled JungleClear Spell", 30);

                ManaManager.AddSpellFarm(clearMenu);
            }

            killStealMenu = mainMenu.AddSubMenu("KillSteal", "KillSteal");
            {
                killStealMenu.AddText("Q");
                killStealMenu.AddBool("KillStealQ", "Use Q");
                killStealMenu.AddText("W");
                killStealMenu.AddBool("KillStealW", "Use W");
            }

            miscMenu = mainMenu.AddSubMenu("Misc", "Misc");
            {
                miscMenu.AddText("R");
                miscMenu.AddKey("SemiR", "Semi Cast R Key", KeyBind.BindTypes.HoldActive, 'T');

                miscMenu.AddSeparator();

                miscMenu.AddText("Anti Gapcloser");
                miscMenu.AddBool("EnabledAnti", "Enabled");
                miscMenu.AddSlider("AntiGapCloserHp", "When Player HealthPercent <= x%, Enabled Anti Gapcloser Settings", 30);
                miscMenu.AddText("Anti Target");
                if (EntityManager.Heroes.Enemies.Any())
                {
                    foreach (var target in EntityManager.Heroes.Enemies)
                    {
                        miscMenu.AddBool("AntiGapCloserE" + target.ChampionName.ToLower(), target.ChampionName, target.IsMelee);
                    }
                }

                miscMenu.AddSeparator();

                miscMenu.AddText("Anti Melee");
                miscMenu.AddBool("EnabledAntiMelee", "Enabled");
                miscMenu.AddSlider("AntiMeleeHp", "When Player HealthPercent <= x%, Enabled Anti Melee Settings", 30);
            }

            drawMenu = mainMenu.AddSubMenu("Drawings", "Drawings");
            {
                drawMenu.AddText("Spell Range"); 
                drawMenu.AddBool("DrawQ", "Draw Q Range", false);
                drawMenu.AddBool("DrawQExtend", "Draw QExtend Range", false);
                drawMenu.AddBool("DrawW", "Draw W Range", false);
                drawMenu.AddBool("DrawE", "Draw E Range", false);
                drawMenu.AddBool("DrawR", "Draw R Range", false);
                ManaManager.AddDrawFarm(drawMenu);
                DamageIndicator.AddToMenu(drawMenu);
            }
        }

        
        internal static bool ComboQ => comboMenu.GetBool("ComboQ");
        internal static bool ComboQExtend => comboMenu.GetBool("ComboQExtend"); 
        internal static bool ComboW => comboMenu.GetBool("ComboW");
        internal static bool ComboWFast => comboMenu.GetBool("ComboWFast"); 
        internal static bool ComboEDash => comboMenu.GetBool("ComboEDash");
        internal static bool ComboEReset => comboMenu.GetBool("ComboEReset");
        internal static bool ComboESafe => comboMenu.GetBool("ComboESafe");
        internal static bool ComboEWall => comboMenu.GetBool("ComboEWall"); 
        internal static bool ComboR => comboMenu.GetBool("ComboR");
        
        internal static bool HarassQ => harassMenu.GetBool("HarassQ");
        internal static bool HarassQExtend => harassMenu.GetBool("HarassQExtend");
        internal static bool HarassW => harassMenu.GetBool("HarassW");
        internal static int HarassMP => harassMenu.GetSlider("HarassMP");

        internal static bool LaneClearQ => clearMenu.GetBool("LaneClearQ");
        internal static bool LaneClearW => clearMenu.GetBool("LaneClearW");
        internal static int LaneClearMP => clearMenu.GetSlider("LaneClearMP");

        internal static bool TurretClearW => clearMenu.GetBool("TurretClearW");
        internal static bool TurretClearE => clearMenu.GetBool("TurretClearE");
        internal static int TurretClearMP => clearMenu.GetSlider("TurretClearMP");

        internal static bool JungleClearQ => clearMenu.GetBool("JungleClearQ");
        internal static bool JungleClearW => clearMenu.GetBool("JungleClearW");
        internal static bool JungleClearE => clearMenu.GetBool("JungleClearE");
        internal static int JungleClearMP => clearMenu.GetSlider("JungleClearMP");


        internal static bool KillStealQ => killStealMenu.GetBool("KillStealQ");
        internal static bool KillStealW => killStealMenu.GetBool("KillStealW");

        internal static bool SemiR => miscMenu.GetKey("SemiR");

        internal static bool EnabledAnti => miscMenu.GetBool("EnabledAnti");
        internal static int AntiGapCloserHp => miscMenu.GetSlider("AntiGapCloserHp");

        internal static bool EnabledAntiMelee => miscMenu.GetBool("EnabledAntiMelee");
        internal static int AntiMeleeHp => miscMenu.GetSlider("AntiMeleeHp");

        internal static bool DrawQ => drawMenu.GetBool("DrawQ");
        internal static bool DrawQExtend => drawMenu.GetBool("DrawQExtend");
        internal static bool DrawW => drawMenu.GetBool("DrawW");
        internal static bool DrawE => drawMenu.GetBool("DrawE");
        internal static bool DrawR => drawMenu.GetBool("DrawR");
    }
}
