namespace MoonLucian
{
    using EloBuddy;
    using EloBuddy.SDK;
    using EloBuddy.SDK.Menu;

    using System;
    using System.Linq;

    internal class Logic
    {
        //Spell
        internal static Spell.Targeted Q { get; set; }
        internal static Spell.Skillshot QExtend { get; set; }
        internal static Spell.Skillshot W { get; set; }
        internal static Spell.Skillshot E { get; set; }
        internal static Spell.Skillshot R { get; set; }
        internal static Spell.Skillshot R1 { get; set; }

        //Item
        internal static Item Youmuus { get; set; }

        //Menu
        internal static Menu mainMenu { get; set; }
        internal static Menu modeMenu { get; set; }
        internal static Menu miscMenu { get; set; }
        internal static Menu comboMenu { get; set; }
        internal static Menu harassMenu { get; set; }
        internal static Menu clearMenu { get; set; }
        internal static Menu killStealMenu { get; set; }
        internal static Menu qMenu { get; set; }
        internal static Menu wMenu { get; set; }
        internal static Menu eMenu { get; set; }
        internal static Menu rMenu { get; set; }
        internal static Menu drawMenu { get; set; }

        //Orbwalker
        internal static bool isComboMode => Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo);
        internal static bool isHarassMode => Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass);
        internal static bool isFarmMode => Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear) ||
                                           Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear) || 
                                           Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LastHit);
        internal static bool isLaneClearMode => Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear);
        internal static bool isJungleClearMode => Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear);
        internal static bool isLastHitMode => Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LastHit);

        //Mana
        internal static float QMana { get; set; }
        internal static float WMana { get; set; }
        internal static float EMana { get; set; }
        internal static float RMana { get; set; }
        internal static float PlayerMana => Player.Instance.Mana;
        internal static float PlayerManaPercent => Player.Instance.ManaPercent;

        //Status
        internal static int lastCastTime { get; set; }
        internal static bool havePassive { get; set; }
        internal static bool havePassiveBuff => Player.Instance.Buffs.Any(x => x.Name.ToLower() == "lucianpassivebuff");

        //Tick Count
        internal static int TickCount
        {
            get
            {
                return Environment.TickCount & int.MaxValue;
            }
        }
    }
}
