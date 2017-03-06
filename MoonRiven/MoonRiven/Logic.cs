namespace MoonRiven
{
    using myCommon;

    using System;

    using EloBuddy;
    using EloBuddy.SDK;
    using EloBuddy.SDK.Menu;

    internal class Logic
    {
        //Spell
        internal static Spell.Skillshot Q { get; set; }
        internal static Spell.Active W { get; set; }
        internal static Spell.Skillshot E { get; set; }
        internal static Spell.Active R { get; set; }
        internal static Spell.Skillshot R1 { get; set; }
        internal static SpellSlot Flash { get; set; } = SpellSlot.Unknown;
        internal static SpellSlot Ignite { get; set; } = SpellSlot.Unknown;

        //Menu
        internal static Menu mainMenu { get; set; }
        internal static Menu miscMenu { get; set; }
        internal static Menu comboMenu { get; set; }
        internal static Menu burstMenu { get; set; }
        internal static Menu harassMenu { get; set; }
        internal static Menu clearMenu { get; set; }
        internal static Menu fleeMenu { get; set; }
        internal static Menu killStealMenu { get; set; }
        internal static Menu qMenu { get; set; }
        internal static Menu wMenu { get; set; }
        internal static Menu eMenu { get; set; }
        internal static Menu rMenu { get; set; }
        internal static Menu drawMenu { get; set; }

        //Orbwalker
        internal static bool isComboMode => Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo);
        internal static bool isHarassMode => Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass);
        internal static bool isLaneClearMode => Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear);
        internal static bool isJungleClearMode => Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear);
        internal static bool isFleeMode => Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Flee);
        internal static bool isNoneMode => Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.None);

        //Status
        internal static int qStack { get; set; } = 0;
        internal static int lastQTime { get; set; } = 0;
        internal static int lastCastTime { get; set; } = 0;
        internal static bool isRActive => Player.GetSpell(SpellSlot.R).Name == "RivenIzunaBlade";

        //Tick Count
        internal static int TickCount => Environment.TickCount & int.MaxValue;
        internal static int GameTimeTickCount => (int)(Game.Time * 1000f);

        //myTarget
        internal static AIHeroClient myTarget { get; set; } = null;

        //Logic
        internal static bool UseItem()
        {
            if (Item.HasItem(3077) && Item.CanUseItem(3077))
            {
                return Item.UseItem(3077);
            }

            if (Item.HasItem(3074) && Item.CanUseItem(3074))
            {
                return Item.UseItem(3074);
            }

            if (Item.HasItem(3053) && Item.CanUseItem(3053))
            {
                return Item.UseItem(3053);
            }

            return false;
        }

        internal static bool HaveShield(Obj_AI_Base target)
        {
            if (target.HasBuff("SivirE"))
            {
                return true;
            }

            if (target.HasBuff("BlackShield"))
            {
                return true;
            }

            if (target.HasBuff("NocturneShit"))
            {
                return true;
            }

            return false;
        }

        internal static bool CastQ(Obj_AI_Base target)
        {
            if (target == null || target.IsDead || !target.IsValidTarget() || Q.Level == 0 || !Q.IsReady())
            {
                return false;
            }

            switch (MenuInit.QMode)
            {
                case 0:
                    return Player.Instance.Spellbook.CastSpell(SpellSlot.Q, target.Position);
                case 1:
                    return Player.Instance.Spellbook.CastSpell(SpellSlot.Q, Game.CursorPos);
                default:
                    return false;
            }
        }

        internal static bool R1Logic(AIHeroClient target)
        {
            if (!target.IsValidTarget(500) || R.Name != "RivenFengShuiEngine" || !MenuInit.ComboR1)
            {
                return false;
            }

            return R.Cast();
        }

        internal static bool R2Logic(AIHeroClient target)
        {
            if (!target.IsValidTarget() || R.Name == "RivenFengShuiEngine" || MenuInit.ComboR2 == 3)
            {
                return false;
            }

            switch (MenuInit.ComboR2)
            {
                case 0:
                    if (target.HealthPercent < 20 ||
                        (target.Health > DamageCalculate.GetRDamage(target) + Player.Instance.GetAutoAttackDamage(target) * 2 &&
                         target.HealthPercent < 40) ||
                        (target.Health <= DamageCalculate.GetRDamage(target)) ||
                        (target.Health <= DamageCalculate.GetComboDamage(target)))
                    {
                        return R1.Cast(target.Position);
                    }
                    break;
                case 1:
                    if (DamageCalculate.GetRDamage(target) > target.Health && target.DistanceToPlayer() < 600)
                    {
                        return R1.Cast(target.Position);
                    }
                    break;
                case 2:
                    if (target.DistanceToPlayer() < 600)
                    {
                        return R1.Cast(target.Position);
                    }
                    break;
            }

            return false;
        }
    }
}
