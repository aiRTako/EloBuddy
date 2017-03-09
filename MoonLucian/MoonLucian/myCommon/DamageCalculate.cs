namespace MoonLucian.myCommon
{
    using EloBuddy;
    using EloBuddy.SDK;

    internal class DamageCalculate
    {
        internal static float GetComboDamage(AIHeroClient target)
        {
            if (target == null || target.IsDead || target.IsZombie)
            {
                return 0;
            }

            var damage = 0d;

            if (Player.Instance.BaseAttackDamage + Player.Instance.FlatPhysicalDamageMod > Player.Instance.TotalMagicalDamage)
            {
                damage += Player.Instance.GetAutoAttackDamage(target);
            }

            if (Player.Instance.GetSpellSlotFromName("SummonerDot") != SpellSlot.Unknown && 
                Player.Instance.Spellbook.GetSpell(Player.Instance.GetSpellSlotFromName("SummonerDot")).IsReady)
            {
                damage += GetIgniteDmage(target);
            }

            damage += GetQDamage(target);
            damage += GetWDamage(target);
            damage += GetEDamage(target);
            damage += GetRDamage(target);

            if (ObjectManager.Player.HasBuff("SummonerExhaust"))
            {
                damage = damage * 0.6f;
            }

            if (target.CharData.BaseSkinName == "Moredkaiser")
            {
                damage -= target.Mana;
            }

            if (target.HasBuff("GarenW"))
            {
                damage = damage * 0.7f;
            }

            if (target.HasBuff("ferocioushowl"))
            {
                damage = damage * 0.7f;
            }

            if (target.HasBuff("BlitzcrankManaBarrierCD") && target.HasBuff("ManaBarrier"))
            {
                damage -= target.Mana / 2f;
            }

            return (float)damage;

        }

        internal static float GetQDamage(Obj_AI_Base target)
        {
            if (Player.Instance.Spellbook.GetSpell(SpellSlot.Q).Level == 0 || 
                !Player.Instance.Spellbook.GetSpell(SpellSlot.Q).IsReady)
            {
                return 0f;
            }

            var qDMG =
                new double[] {80, 115, 150, 185, 220}[Player.Instance.Spellbook.GetSpell(SpellSlot.Q).Level - 1] +
                new double[] {60, 70, 80, 90, 100}[Player.Instance.Spellbook.GetSpell(SpellSlot.Q).Level - 1]/100*
                Player.Instance.FlatPhysicalDamageMod;

            return Player.Instance.CalculateDamageOnUnit(target, DamageType.Physical, (float)qDMG);
        }

        internal static float GetWDamage(AIHeroClient target)
        {
            if (Player.Instance.Spellbook.GetSpell(SpellSlot.W).Level == 0 ||
                !Player.Instance.Spellbook.GetSpell(SpellSlot.W).IsReady)
            {
                return 0f;
            }

            var wDMG = new double[] { 60, 100, 140, 180, 220 }[Player.Instance.Spellbook.GetSpell(SpellSlot.W).Level - 1] +
                0.9 * Player.Instance.TotalMagicalDamage;

            return Player.Instance.CalculateDamageOnUnit(target, DamageType.Magical, (float)wDMG);
        }

        internal static float GetEDamage(AIHeroClient target)
        {
            if (Player.Instance.Spellbook.GetSpell(SpellSlot.E).Level == 0 ||
                !Player.Instance.Spellbook.GetSpell(SpellSlot.E).IsReady)
            {
                return 0f;
            }

            return (float)GetPassiveDMG(target);
        }

        internal static float GetRDamage(AIHeroClient target)
        {
            if (Player.Instance.Spellbook.GetSpell(SpellSlot.R).Level == 0 ||
                !Player.Instance.Spellbook.GetSpell(SpellSlot.R).IsReady)
            {
                return 0f;
            }

            var rDMG = new double[] { 20, 35, 50 }[Player.Instance.Spellbook.GetSpell(SpellSlot.R).Level - 1] + 
                0.1 * Player.Instance.TotalMagicalDamage + 0.2 * Player.Instance.FlatPhysicalDamageMod;

            return Player.Instance.CalculateDamageOnUnit(target, DamageType.Magical, (float)rDMG);
        }

        internal static float GetIgniteDmage(Obj_AI_Base target)
        {
            return 50 + 20 * Player.Instance.Level - target.HPRegenRate / 5 * 3;
        }

        internal static double GetPassiveDMG(Obj_AI_Base target)
        {
            if (Player.Instance.Level >= 13)
            {
                return Player.Instance.GetAutoAttackDamage(target) + 0.6 * Player.Instance.GetAutoAttackDamage(target);
            }
            else if (Player.Instance.Level >= 7)
            {
                return Player.Instance.GetAutoAttackDamage(target) + 0.5 * Player.Instance.GetAutoAttackDamage(target);
            }
            else
                return Player.Instance.GetAutoAttackDamage(target) + 0.4 * Player.Instance.GetAutoAttackDamage(target);
        }
    }
}