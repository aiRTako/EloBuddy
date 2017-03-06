namespace MoonRiven.myCommon
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

            if (target.IsUnKillable())
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

            if (Player.HasBuff("SummonerExhaust"))
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

            var qhan = 3 - Logic.qStack;
            var qDMG = new double[] { 30, 55, 80, 105, 130 }[Player.Instance.Spellbook.GetSpell(SpellSlot.Q).Level - 1] + 0.7 * Player.Instance.FlatPhysicalDamageMod;

            return
                (float)
                (Player.Instance.CalculateDamageOnUnit(target, DamageType.Physical, (float)qDMG) * qhan +
                 Player.Instance.GetAutoAttackDamage(target) * qhan * (1 + GetRivenPassive));
        }

        internal static float GetWDamage(AIHeroClient target)
        {
            if (ObjectManager.Player.Spellbook.GetSpell(SpellSlot.W).Level == 0 ||
                !ObjectManager.Player.Spellbook.GetSpell(SpellSlot.W).IsReady)
            {
                return 0f;
            }

            var wDMG = new double[] { 50, 80, 110, 140, 170 }[Player.Instance.Spellbook.GetSpell(SpellSlot.W).Level - 1] + Player.Instance.FlatPhysicalDamageMod;

            return Player.Instance.CalculateDamageOnUnit(target, DamageType.Physical, (float)wDMG);
        }

        internal static float GetEDamage(AIHeroClient target)
        {
            if (Player.Instance.Spellbook.GetSpell(SpellSlot.E).Level == 0 ||
                !Player.Instance.Spellbook.GetSpell(SpellSlot.E).IsReady)
            {
                return 0f;
            }

            return Player.Instance.CanMoveMent() ? Player.Instance.GetAutoAttackDamage(target) : 0;
        }

        internal static float GetRDamage(AIHeroClient target)
        {
            if (Player.Instance.Spellbook.GetSpell(SpellSlot.R).Level == 0 ||
                !Player.Instance.Spellbook.GetSpell(SpellSlot.R).IsReady)
            {
                return 0f;
            }

            var dmg = (new double[] { 80, 120, 160 }[Player.Instance.Spellbook.GetSpell(SpellSlot.R).Level - 1] +
                 0.6 * Player.Instance.FlatPhysicalDamageMod) *
                (1 + (target.MaxHealth - target.Health) /
                 target.MaxHealth > 0.75
                    ? 0.75
                    : (target.MaxHealth - target.Health) / target.MaxHealth) * 8 / 3;

            return Player.Instance.CalculateDamageOnUnit(target, DamageType.Physical, (float)dmg);
        }

        internal static float GetIgniteDmage(Obj_AI_Base target)
        {
            return 50 + 20 * Player.Instance.Level - target.HPRegenRate / 5 * 3;
        }

        internal static double GetRivenPassive
        {
            get
            {
                if (Player.Instance.Level == 18)
                {
                    return 0.5;
                }

                if (Player.Instance.Level >= 15)
                {
                    return 0.45;
                }

                if (Player.Instance.Level >= 12)
                {
                    return 0.4;
                }

                if (Player.Instance.Level >= 9)
                {
                    return 0.35;
                }

                if (Player.Instance.Level >= 6)
                {
                    return 0.3;
                }

                if (Player.Instance.Level >= 3)
                {
                    return 0.25;
                }

                return 0.2;
            }
        }

        internal static float GetRealDamage(AIHeroClient target, float DMG)
        {
            if (target == null || target.IsDead || target.IsZombie)
            {
                return 0;
            }

            if (target.IsUnKillable())
            {
                return 0;
            }

            var damage = DMG;

            if (Player.HasBuff("SummonerExhaust"))
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

            return damage;
        }
    }
}