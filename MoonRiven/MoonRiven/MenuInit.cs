namespace MoonRiven
{
    using myCommon;

    using EloBuddy;
    using EloBuddy.SDK.Menu;
    using EloBuddy.SDK.Menu.Values;

    internal class MenuInit : Logic
    {
        internal static void Init()
        {
            mainMenu = MainMenu.AddMenu("Moon" + Player.Instance.ChampionName, "Moon" + Player.Instance.ChampionName);
            {
                mainMenu.AddGroupLabel("Please Set the Orbwalker");
                mainMenu.AddGroupLabel("Orbwalker -> Advanced -> Update Event Listening -> Enabled On Update(more fast)");
                mainMenu.AddGroupLabel("--------------------");
                mainMenu.AddGroupLabel("One Things You Need to Know");
                mainMenu.AddGroupLabel("Combo/Burst/Harass Mode");
                mainMenu.AddGroupLabel("Dont Forgot Use Left Click to Select Target");
                mainMenu.AddGroupLabel("It will make the Addon More Better");
                mainMenu.AddGroupLabel("---------------------");
                mainMenu.AddGroupLabel("My GitHub: https://github.com/NightMoon032/MoonSeries");
                mainMenu.AddGroupLabel("If you have Feedback pls post to my topic");
                mainMenu.AddGroupLabel("----------------------");
                mainMenu.AddGroupLabel("Credit: NightMoon");
            }

            comboMenu = mainMenu.AddSubMenu("Combo", "Combo");
            {
                comboMenu.AddLine("Q");
                comboMenu.AddBool("ComboQGap", "Use Q Gapcloser");
                comboMenu.AddLine("W");
                comboMenu.AddBool("ComboW", "Use W");
                comboMenu.AddBool("ComboWLogic", "Use W Cancel Spell Animation");
                comboMenu.AddLine("E");
                comboMenu.AddBool("ComboE", "Use E");
                comboMenu.AddBool("ComboEGap", "Use E Gapcloser");
                comboMenu.AddLine("R");
                comboMenu.AddKey("ComboR1", "Use R1: ", KeyBind.BindTypes.PressToggle, 'G', true);
                comboMenu.AddList("ComboR2", "Use R2 Mode: ", new[] { "my Logic", "Only KillSteal", "First Cast", "Off" });
                comboMenu.AddLine("Others");
                comboMenu.AddBool("ComboItem", "Use Item");
                comboMenu.AddBool("ComboYoumuu", "Use Youmuu");
                comboMenu.AddBool("ComboDot", "Use Ignite");
                comboMenu.AddLine("Burst");
            }

            burstMenu = mainMenu.AddSubMenu("Burst", "Burst");
            {
                burstMenu.AddBool("BurstFlash", "Use Flash");
                burstMenu.AddBool("BurstDot", "Use Ignite");
                burstMenu.AddList("BurstMode", "Burst Mode: ", new[] { "Shy Mode", "EQ Mode" });
                burstMenu.AddKey("BurstEnabledKey", "Enabled Burst Key: ", KeyBind.BindTypes.PressToggle, 'T');
                burstMenu.AddSeparator();
                burstMenu.AddLabel("How to burst");
                burstMenu.AddLabel("1.you need to enbaled the Key");
                burstMenu.AddLabel("2.Select the Target (or not, but this will be force target to burst)");
                burstMenu.AddLabel("3.just press the Combo Key");
            }

            harassMenu = mainMenu.AddSubMenu("Harass", "Harass");
            {
                harassMenu.AddLine("Q");
                harassMenu.AddBool("HarassQ", "Use Q");
                harassMenu.AddLine("W");
                harassMenu.AddBool("HarassW", "Use W");
                harassMenu.AddLine("E");
                harassMenu.AddBool("HarassE", "Use E");
                harassMenu.AddLine("Mode");
                harassMenu.AddList("HarassMode", "Harass Mode: ", new[] { "Smart", "Normal" });
            }

            clearMenu = mainMenu.AddSubMenu("Clear", "Clear");
            {
                clearMenu.AddText("LaneClear Settings");
                clearMenu.AddLine("LaneClear Q");
                clearMenu.AddBool("LaneClearQ", "Use Q");
                clearMenu.AddBool("LaneClearQSmart", "Use Q Smart Farm");
                clearMenu.AddBool("LaneClearQT", "Use Q Reset Attack Turret");
                clearMenu.AddLine("LaneClear W");
                clearMenu.AddBool("LaneClearW", "Use W", false);
                clearMenu.AddSlider("LaneClearWCount", "Use W| Min hit Count >= x", 3, 1, 10);
                clearMenu.AddLine("LaneClear Items");
                clearMenu.AddBool("LaneClearItem", "Use Items");

                clearMenu.AddSeparator();

                clearMenu.AddText("JungleClear Settings");
                clearMenu.AddLine("JungleClear Q");
                clearMenu.AddBool("JungleClearQ", "Use Q");
                clearMenu.AddLine("JungleClear W");
                clearMenu.AddBool("JungleClearW", "Use W");
                clearMenu.AddLine("JungleClear E");
                clearMenu.AddBool("JungleClearE", "Use E");
                clearMenu.AddLine("JungleClear Items");
                clearMenu.AddBool("JungleClearItem", "Use Items");

                ManaManager.AddSpellFarm(clearMenu);
            }

            fleeMenu = mainMenu.AddSubMenu("Flee", "Flee");
            {
                fleeMenu.AddText("Q");
                fleeMenu.AddBool("FleeQ", "Use Q");
                fleeMenu.AddText("W");
                fleeMenu.AddBool("FleeW", "Use W");
                fleeMenu.AddText("E");
                fleeMenu.AddBool("FleeE", "Use E");
            }

            killStealMenu = mainMenu.AddSubMenu("KillSteal", "KillSteal");
            {
                killStealMenu.AddText("R");
                killStealMenu.AddBool("KillStealR", "Use R");
            }

            miscMenu = mainMenu.AddSubMenu("Misc", "Misc");
            {
                miscMenu.AddText("Q");
                miscMenu.AddBool("KeepQ", "Keep Q alive");
                miscMenu.AddList("QMode", "Q Mode: ", new[] { "To target", "To mouse" });

                miscMenu.AddSeparator();
                miscMenu.AddText("W");
                miscMenu.AddBool("AntiGapcloserW", "Anti Gapcloser");
                miscMenu.AddBool("InterruptW", "Interrupt Danger Spell");

                //miscMenu.AddSeparator();
                //miscMenu.AddText("E");
                //miscMenu.AddBool("DodgeE", "Dodge some Spell");

                miscMenu.AddSeparator();
                miscMenu.AddText("Animation");
                miscMenu.AddBool("manualCancel", "Semi Cancel Animation");
                miscMenu.AddBool("manualCancelPing", "Cancel Animation Calculate Ping?");
            }

            eMenu = mainMenu.AddSubMenu("Evade", "Evade");
            {  
                //TODO: 
                //1.use E Evade Logic
                //2.use E Dodge the unit spell(need evade?)
                //3.evade Logic need create the new Menu(not in the miscMenu)
                eMenu.AddLabel("TODO");
            }

            drawMenu = mainMenu.AddSubMenu("Drawings", "Drawings");
            {
                drawMenu.AddText("Spell Range"); 
                drawMenu.AddBool("DrawW", "Draw W Range", false);
                drawMenu.AddBool("DrawE", "Draw E Range", false);
                drawMenu.AddBool("DrawR1", "Draw R1 Range", false);
                drawMenu.AddBool("DrawR", "Draw R Status");
                drawMenu.AddBool("DrawBurst", "Draw R Status");
                ManaManager.AddDrawFarm(drawMenu);
                DamageIndicator.AddToMenu(drawMenu);
            }
        }

        internal static bool ComboQGap => comboMenu.GetBool("ComboQGap");
        internal static bool ComboW => comboMenu.GetBool("ComboW"); 
        internal static bool ComboWLogic => comboMenu.GetBool("ComboWLogic");
        internal static bool ComboE => comboMenu.GetBool("ComboE"); 
        internal static bool ComboEGap => comboMenu.GetBool("ComboEGap");
        internal static bool ComboR1 => comboMenu.GetKey("ComboR1");
        internal static int ComboR2 => comboMenu.GetList("ComboR2");
        internal static bool ComboItem => comboMenu.GetBool("ComboItem"); 
        internal static bool ComboYoumuu => comboMenu.GetBool("ComboYoumuu");
        internal static bool ComboDot => comboMenu.GetBool("ComboDot");

        internal static bool BurstFlash => burstMenu.GetBool("BurstFlash");
        internal static bool BurstDot => burstMenu.GetBool("BurstDot");
        internal static int BurstMode => burstMenu.GetList("BurstMode");
        internal static bool BurstEnabledKey => burstMenu.GetKey("BurstEnabledKey");
 
        internal static bool HarassQ => harassMenu.GetBool("HarassQ");
        internal static bool HarassW => harassMenu.GetBool("HarassW");
        internal static bool HarassE => harassMenu.GetBool("HarassE");
        internal static int HarassMode => harassMenu.GetList("HarassMode");

        internal static bool LaneClearQ => clearMenu.GetBool("LaneClearQ");
        internal static bool LaneClearQSmart => clearMenu.GetBool("LaneClearQSmart");
        internal static bool LaneClearQT => clearMenu.GetBool("LaneClearQT");
        internal static bool LaneClearW => clearMenu.GetBool("LaneClearW");
        internal static int LaneClearWCount => clearMenu.GetSlider("LaneClearWCount");
        internal static bool LaneClearItem => clearMenu.GetBool("LaneClearItem");

        internal static bool JungleClearQ => clearMenu.GetBool("JungleClearQ");
        internal static bool JungleClearW => clearMenu.GetBool("JungleClearW");
        internal static bool JungleClearE => clearMenu.GetBool("JungleClearE");
        internal static bool JungleClearItem => clearMenu.GetBool("JungleClearItem");

        internal static bool FleeQ => fleeMenu.GetBool("FleeQ");
        internal static bool FleeW => fleeMenu.GetBool("FleeW");
        internal static bool FleeE => fleeMenu.GetBool("FleeE");

        internal static bool KillStealR => killStealMenu.GetBool("KillStealR");

        internal static bool KeepQ => miscMenu.GetBool("KeepQ");
        internal static int QMode => miscMenu.GetList("QMode");

        internal static bool AntiGapcloserW => miscMenu.GetBool("AntiGapcloserW");
        internal static bool InterruptW => miscMenu.GetBool("InterruptW");

        //internal static bool DodgeE => miscMenu.GetBool("DodgeE");

        internal static bool manualCancel => miscMenu.GetBool("manualCancel");
        internal static bool manualCancelPing => miscMenu.GetBool("manualCancelPing");

        internal static bool DrawW => drawMenu.GetBool("DrawW");
        internal static bool DrawE => drawMenu.GetBool("DrawE");
        internal static bool DrawR1 => drawMenu.GetBool("DrawR1");
        internal static bool DrawR => drawMenu.GetBool("DrawR");
        internal static bool DrawBurst => drawMenu.GetBool("DrawBurst");
    }
}
