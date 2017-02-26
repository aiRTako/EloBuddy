namespace MoonEzreal
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
                mainMenu.AddGroupLabel("My GitHub: https://github.com/NightMoon032/EloBuddy");
                mainMenu.AddGroupLabel("If you have Feedback pls post to my topic");
                mainMenu.AddGroupLabel("---------------------");
                mainMenu.AddGroupLabel("Credit: NightMoon");
            }

            comboMenu = mainMenu.AddSubMenu("Combo", "Combo");
            {
                comboMenu.AddLine("Q");
                comboMenu.AddBool("ComboQ", "Use Q");
                comboMenu.AddLine("W");
                comboMenu.AddBool("ComboW", "Use W");
                comboMenu.AddLine("E");
                comboMenu.AddBool("ComboE", "Use E");
                comboMenu.AddBool("ComboESafe", "Use E| Safe Check");
                comboMenu.AddBool("ComboEWall", "Use E| Dont Dash to Wall");
                comboMenu.AddLine("R");
                comboMenu.AddBool("ComboR", "Use R");
            }

            harassMenu = mainMenu.AddSubMenu("Harass", "Harass");
            {
                harassMenu.AddLine("Q");
                harassMenu.AddBool("HarassQ", "Use Q");
                harassMenu.AddLine("W");
                harassMenu.AddBool("HarassW", "Use W");
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
                clearMenu.AddBool("LaneClearQFast", "Use Q| Fast Clear Mode, if you disable this Option just only lasthit Q");
                clearMenu.AddLine("LaneClear W");
                clearMenu.AddBool("LaneClearW", "Use W");
                clearMenu.AddLine("LaneClearMana");
                clearMenu.AddSlider("LaneClearMP", "When Player Mana Percent >= x%, Enabled LaneClear Spell");

                clearMenu.AddSeparator();

                clearMenu.AddText("JungleClear Settings");
                clearMenu.AddLine("JungleClear Q");
                clearMenu.AddBool("JungleClearQ", "Use Q");
                clearMenu.AddSlider("JungleClearMP", "When Player Mana Percent >= x%, Enabled JungleClear Spell");

                clearMenu.AddSeparator();

                clearMenu.AddLine("LastHit Q");
                clearMenu.AddBool("LastHitQ", "Use Q");
                clearMenu.AddSlider("LastHitMP", "When Player Mana Percent >= x%, Enabled LastHit Spell");

                ManaManager.AddSpellFarm(clearMenu);
            }

            killStealMenu = mainMenu.AddSubMenu("KillSteal", "KillSteal");
            {
                killStealMenu.AddText("Q");
                killStealMenu.AddBool("KillStealQ", "Use Q");
                killStealMenu.AddText("W");
                killStealMenu.AddBool("KillStealW", "Use W");
                killStealMenu.AddText("R");
                killStealMenu.AddBool("KillStealR", "Use R");
                killStealMenu.AddText("KillSteal R Target");
                if (EntityManager.Heroes.Enemies.Any())
                {
                    foreach(var target in EntityManager.Heroes.Enemies)
                    {
                        killStealMenu.AddBool("KillStealR" + target.ChampionName.ToLower(), target.ChampionName);
                    }
                }  
            }

            miscMenu = mainMenu.AddSubMenu("Misc", "Misc");
            {
                miscMenu.AddText("R");
                miscMenu.AddSlider("MinRRange", "Min R Cast Range Settings(Global): ", 800, 0, 1500);
                miscMenu.AddSlider("MaxRRange", "Max R Cast Range Settings(Global): ", 3000, 1500, 1500);
                miscMenu.AddKey("SemiR", "Semi Cast R Key", KeyBind.BindTypes.PressToggle, 'T');

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
                drawMenu.AddBool("DrawW", "Draw W Range", false);
                drawMenu.AddBool("DrawE", "Draw E Range", false);
                drawMenu.AddBool("DrawR", "Draw R Range", false);
                ManaManager.AddDrawFarm(drawMenu);
                DamageIndicator.AddToMenu(drawMenu);
            }
        }


        internal static bool ComboQ => comboMenu.GetBool("ComboQ");
        internal static bool ComboW => comboMenu.GetBool("ComboW");
        internal static bool ComboE => comboMenu.GetBool("ComboE");
        internal static bool ComboESafe => comboMenu.GetBool("ComboESafe");
        internal static bool ComboEWall => comboMenu.GetBool("ComboEWall"); 
        internal static bool ComboR => comboMenu.GetBool("ComboR");

        internal static bool HarassQ => harassMenu.GetBool("HarassQ");
        internal static bool HarassW => harassMenu.GetBool("HarassW");
        internal static int HarassMP => harassMenu.GetSlider("HarassMP");

        internal static bool LaneClearQ => clearMenu.GetBool("LaneClearQ");
        internal static bool LaneClearQFast => clearMenu.GetBool("LaneClearQFast");
        internal static bool LaneClearW => clearMenu.GetBool("LaneClearW");
        internal static int LaneClearMP => clearMenu.GetSlider("LaneClearMP");

        internal static bool JungleClearQ => clearMenu.GetBool("JungleClearQ");
        internal static int JungleClearMP => clearMenu.GetSlider("JungleClearMP");

        internal static bool LastHitQ => clearMenu.GetBool("LastHitQ");
        internal static int LastHitMP => clearMenu.GetSlider("LastHitMP");

        internal static bool KillStealQ => killStealMenu.GetBool("KillStealQ");
        internal static bool KillStealW => killStealMenu.GetBool("KillStealW");
        internal static bool KillStealR => killStealMenu.GetBool("KillStealR");

        internal static bool SemiR => miscMenu.GetKey("SemiR");
        internal static int MinRRange => miscMenu.GetSlider("MinRRange");
        internal static int MaxRRange => miscMenu.GetSlider("MaxRRange");

        internal static bool EnabledAnti => miscMenu.GetBool("EnabledAnti");
        internal static int AntiGapCloserHp => miscMenu.GetSlider("AntiGapCloserHp");

        internal static bool EnabledAntiMelee => miscMenu.GetBool("EnabledAntiMelee");
        internal static int AntiMeleeHp => miscMenu.GetSlider("AntiMeleeHp");

        internal static bool DrawQ => drawMenu.GetBool("DrawQ");
        internal static bool DrawW => drawMenu.GetBool("DrawW");
        internal static bool DrawE => drawMenu.GetBool("DrawE");
        internal static bool DrawR => drawMenu.GetBool("DrawR");
    }
}
