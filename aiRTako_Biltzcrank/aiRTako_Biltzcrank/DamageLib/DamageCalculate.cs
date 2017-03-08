namespace aiRTako_Biltzcrank.DamageLib
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

            var qDMG = new double[] { 80, 135, 190, 245, 300 }[Player.Instance.Spellbook.GetSpell(SpellSlot.Q).Level - 1] + Player.Instance.TotalMagicalDamage;

            return Player.Instance.CalculateDamageOnUnit(target, DamageType.Magical, (float)qDMG);
        }

        internal static float GetWDamage(Obj_AI_Base target)
        {
            if (Player.Instance.Spellbook.GetSpell(SpellSlot.W).Level == 0 ||
                !Player.Instance.Spellbook.GetSpell(SpellSlot.W).IsReady)
            {
                return 0f;
            }

            return 0f;
        }

        internal static float GetEDamage(Obj_AI_Base target)
        {
            if (Player.Instance.Spellbook.GetSpell(SpellSlot.E).Level == 0 ||
                !Player.Instance.Spellbook.GetSpell(SpellSlot.E).IsReady)
            {
                return 0f;
            }

            return 2 * Player.Instance.GetAutoAttackDamage(target);
        }

        internal static float GetRDamage(Obj_AI_Base target)
        {
            if (Player.Instance.Spellbook.GetSpell(SpellSlot.R).Level == 0 ||
                !Player.Instance.Spellbook.GetSpell(SpellSlot.R).IsReady)
            {
                return 0f;
            }

            var rDMG = new double[] { 250, 375, 500 }[Player.Instance.Spellbook.GetSpell(SpellSlot.R).Level - 1] + Player.Instance.TotalMagicalDamage;

            return Player.Instance.CalculateDamageOnUnit(target, DamageType.Magical, (float)rDMG);
        }

        internal static float GetIgniteDmage(Obj_AI_Base target)
        {
            return 50 + 20 * Player.Instance.Level - target.HPRegenRate / 5 * 3;
        }
    }
}