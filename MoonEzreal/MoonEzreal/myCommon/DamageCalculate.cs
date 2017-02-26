namespace MoonEzreal.myCommon
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

            var attackDMG = Player.Instance.GetAutoAttackDamage(target);
            var qDMG = new double[] { 34, 55, 75, 95, 115 }[Player.Instance.Spellbook.GetSpell(SpellSlot.Q).Level - 1] +
                0.4 * Player.Instance.TotalMagicalDamage + 1.1 * (Player.Instance.BaseAttackDamage + Player.Instance.FlatPhysicalDamageMod);

            return Damage.CalculateDamageOnUnit(Player.Instance, target, DamageType.Physical, (float)qDMG);
        }

        internal static float GetWDamage(AIHeroClient target)
        {
            if (Player.Instance.Spellbook.GetSpell(SpellSlot.W).Level == 0 ||
                !Player.Instance.Spellbook.GetSpell(SpellSlot.W).IsReady)
            {
                return 0f;
            }

            var wDMG = new double[] { 270, 115, 160, 205, 250 }[Player.Instance.Spellbook.GetSpell(SpellSlot.W).Level - 1] +
                0.8 * Player.Instance.TotalMagicalDamage;

            return Damage.CalculateDamageOnUnit(Player.Instance, target, DamageType.Magical, (float)wDMG);
        }

        internal static float GetEDamage(AIHeroClient target)
        {
            if (Player.Instance.Spellbook.GetSpell(SpellSlot.E).Level == 0 ||
                !Player.Instance.Spellbook.GetSpell(SpellSlot.E).IsReady)
            {
                return 0f;
            }

            var eDMG = new double[] { 75, 125, 175, 225, 275 }[Player.Instance.Spellbook.GetSpell(SpellSlot.E).Level - 1] +
                0.75 * Player.Instance.TotalMagicalDamage + 0.5 * Player.Instance.FlatPhysicalDamageMod;

            return Damage.CalculateDamageOnUnit(Player.Instance, target, DamageType.Magical, (float)eDMG);
        }

        internal static float GetRDamage(AIHeroClient target, int CollsionCount = 0)
        {
            if (Player.Instance.Spellbook.GetSpell(SpellSlot.R).Level == 0 ||
                !Player.Instance.Spellbook.GetSpell(SpellSlot.R).IsReady)
            {
                return 0f;
            }

            var rDMG = new double[] { 350, 500, 650 }[Player.Instance.Spellbook.GetSpell(SpellSlot.R).Level - 1] + 
                0.9 * Player.Instance.TotalMagicalDamage + 1 * Player.Instance.FlatPhysicalDamageMod;
            var realDMG = CollsionCount == 0 ? (float)rDMG : CollsionCount >= 7 ? (float)rDMG * 0.3 : (float)rDMG * (1 - (CollsionCount / 10));
            return Damage.CalculateDamageOnUnit(Player.Instance, target, DamageType.Magical, (float)realDMG);
        }

        internal static float GetIgniteDmage(Obj_AI_Base target)
        {
            return 50 + 20 * ObjectManager.Player.Level - target.HPRegenRate / 5 * 3;
        }
    }
}