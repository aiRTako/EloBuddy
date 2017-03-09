namespace aiRTako_Kalista.DamageLib
{
    using EloBuddy;
    using EloBuddy.SDK;

    internal class DamageCalculate
    {
        internal static float GetRealEDamage(AIHeroClient target)
        {
            if (target == null || target.IsDead || target.IsZombie)
            {
                return 0;
            }

            if (!target.HasBuff("kalistaexpungemarker"))
                return 0;

            if (IsUnKillable(target))
                return 0;

            var damage = 0d;

            damage += GetEDamage(target);

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

            var qDMG = new double[] {10, 70, 130, 190, 250}[Player.Instance.Spellbook.GetSpell(SpellSlot.Q).Level - 1] +
                       Player.Instance.BaseAttackDamage + Player.Instance.FlatPhysicalDamageMod;

            return Player.Instance.CalculateDamageOnUnit(target, DamageType.Magical, (float)qDMG);
        }

        internal static float GetEDamage(Obj_AI_Base target)
        {
            if (Player.Instance.Spellbook.GetSpell(SpellSlot.E).Level == 0 ||
                !Player.Instance.Spellbook.GetSpell(SpellSlot.E).IsReady)
            {
                return 0f;
            }

            if (!target.HasBuff("kalistaexpungemarker"))
                return 0f;

            var count = target.GetBuffCount("kalistaexpungemarker");

            if (count > 0)
            {
                var eDMG = new double[] {20, 30, 40, 50, 60}[Player.Instance.Spellbook.GetSpell(SpellSlot.E).Level - 1] +
                           0.6*(Player.Instance.BaseAttackDamage + Player.Instance.FlatPhysicalDamageMod) +
                           (count - 1)*
                           (new double[] {10, 14, 19, 25, 32}[Player.Instance.Spellbook.GetSpell(SpellSlot.E).Level - 1] +
                            new[] {0.2, 0.225, 0.25, 0.275, 0.3}[Player.Instance.Spellbook.GetSpell(SpellSlot.E).Level - 1] *
                            (Player.Instance.BaseAttackDamage + Player.Instance.FlatPhysicalDamageMod));

                return Player.Instance.CalculateDamageOnUnit(target, DamageType.Magical, (float)eDMG);

                //var standalone = new double[] {20, 30, 40, 50, 60}[
                //                     Player.Instance.Spellbook.GetSpell(SpellSlot.E).Level - 1]
                //                 + 0.6*(Player.Instance.BaseAttackDamage + Player.Instance.FlatPhysicalDamageMod);
                //var eDMG = 
                //           (count - 1)*
                //           (new[] {0.25, 0.30, 0.35, 0.40, 0.45}[
                //                Player.Instance.Spellbook.GetSpell(SpellSlot.E).Level - 1]*standalone + standalone);

                //return Player.Instance.CalculateDamageOnUnit(target, DamageType.Physical, (float) eDMG);
            }

            return 0f;
        }

        internal static bool IsUnKillable(AIHeroClient target)
        {
            if (target == null || target.IsDead || target.Health <= 0)
            {
                return true;
            }

            if (target.HasBuff("KindredRNoDeathBuff"))
            {
                return true;
            }

            if (target.HasBuff("UndyingRage") && target.GetBuff("UndyingRage").EndTime - Game.Time > 0.3 && target.Health <= target.MaxHealth * 0.10f)
            {
                return true;
            }

            if (target.HasBuff("JudicatorIntervention"))
            {
                return true;
            }

            if (target.HasBuff("ChronoShift") && target.GetBuff("ChronoShift").EndTime - Game.Time > 0.3 && target.Health <= target.MaxHealth * 0.10f)
            {
                return true;
            }

            if (target.HasBuff("VladimirSanguinePool"))
            {
                return true;
            }

            if (target.HasBuff("ShroudofDarkness"))
            {
                return true;
            }

            if (target.HasBuff("SivirShield"))
            {
                return true;
            }

            if (target.HasBuff("itemmagekillerveil"))
            {
                return true;
            }

            return target.HasBuff("FioraW");
        }

    }
}