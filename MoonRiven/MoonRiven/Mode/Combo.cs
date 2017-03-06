namespace MoonRiven.Mode
{
    using myCommon;

    using EloBuddy;
    using EloBuddy.SDK;
    using EloBuddy.SDK.Events;

    using System;

    internal class Combo : Logic
    {
        internal static void InitTick()
        {
            var target = TargetSelector.GetTarget(800f, DamageType.Physical);

            if (target != null && target.IsValidTarget(800f))
            {
                if (MenuInit.ComboDot && Ignite != SpellSlot.Unknown &&
                    Player.Instance.Spellbook.GetSpell(Ignite).IsReady &&
                    target.IsValidTarget(600) &&
                    ((target.Health < DamageCalculate.GetComboDamage(target) && target.IsValidTarget(400)) ||
                     target.Health < DamageCalculate.GetIgniteDmage(target)) &&
                    Player.Instance.Spellbook.CastSpell(Ignite, target))
                {
                    return;
                }

                if (MenuInit.ComboYoumuu && Item.HasItem(3142) && Item.CanUseItem(3142) &&
                    target.IsValidTarget(580) && Item.UseItem(3142))
                {
                    return;
                }

                if (MenuInit.ComboR1 && R.IsReady() && !isRActive &&
                    target.Health <= DamageCalculate.GetComboDamage(target)*1.3 && target.IsValidTarget(600f) &&
                    R1Logic(target))
                {
                    return;
                }

                if (MenuInit.ComboR2 != 3 && R.IsReady() && isRActive && R2Logic(target))
                {
                    return;
                }

                if (MenuInit.ComboQGap && Q.IsReady() && Environment.TickCount - lastQTime > 1200 &&
                    !Player.Instance.IsDashing() && target.IsValidTarget(480) &&
                    target.DistanceToPlayer() >
                    Player.Instance.GetAutoAttackRange() + Player.Instance.BoundingRadius + 50)
                {
                    CastQ(target);
                    return;
                }

                if (MenuInit.ComboEGap && E.IsReady() && target.IsValidTarget(600) &&
                    target.DistanceToPlayer() >
                    Player.Instance.GetAutoAttackRange() + Player.Instance.BoundingRadius + 50)
                {
                    Player.Instance.Spellbook.CastSpell(SpellSlot.E, target.Position);
                    return;
                }

                if (MenuInit.ComboWLogic && W.IsReady() && target.IsValidTarget(W.Range))
                {
                    if (qStack == 0 && Player.Instance.Spellbook.CastSpell(SpellSlot.W))
                    {
                        return;
                    }

                    if (Q.IsReady() && qStack > 1 && Player.Instance.Spellbook.CastSpell(SpellSlot.W))
                    {
                        return;
                    }

                    if (Player.Instance.HasBuff("RivenFeint") && Player.Instance.Spellbook.CastSpell(SpellSlot.W))
                    {
                        return;
                    }

                    if (!target.IsFacing(Player.Instance))
                    {
                        Player.Instance.Spellbook.CastSpell(SpellSlot.W);
                    }
                }
            }
        }

        internal static void InitAfterAttack(AttackableUnit tar)
        {
            AIHeroClient target = null;

            if (myTarget.IsValidTarget())
            {
                target = myTarget;
            }
            else if (tar is AIHeroClient)
            {
                target = (AIHeroClient)tar;
            }

            if (target != null && target.IsValidTarget(400))
            {
                if (MenuInit.ComboItem)
                {
                    UseItem();
                }

                if (Q.IsReady() && target.IsValidTarget(400))
                {
                    CastQ(target);
                    return;
                }

                if (MenuInit.ComboR2 != 3 && R.IsReady() && isRActive && qStack == 2 && Q.IsReady() && R2Logic(target))
                {
                    return;
                }

                if (MenuInit.ComboW && W.IsReady() && target.IsValidTarget(W.Range) && !HaveShield(target))
                {
                    Player.Instance.Spellbook.CastSpell(SpellSlot.W);
                    return;
                }

                if (MenuInit.ComboE && !Q.IsReady() && !W.IsReady() && E.IsReady() && target.IsValidTarget(400))
                {
                    Player.Instance.Spellbook.CastSpell(SpellSlot.E, target.Position);
                    return;
                }

                if (MenuInit.ComboR1 && R.IsReady() && !isRActive)
                {
                    R1Logic(target);
                }
            }
        }

        internal static void InitSpellCast(GameObjectProcessSpellCastEventArgs Args)
        {
            var target = myTarget.IsValidTarget() ? myTarget : TargetSelector.GetTarget(600f, DamageType.Physical);

            if (Args.SData == null)
            {
                return;
            }

            if (target != null && target.IsValidTarget(600f))
            {
                switch (Args.SData.Name)
                {
                    case "ItemTiamatCleave":
                        if (MenuInit.ComboW && W.IsReady() && target.IsValidTarget(W.Range))
                        {
                            Player.Instance.Spellbook.CastSpell(SpellSlot.W);
                        }
                        else if (Q.IsReady() && target.IsValidTarget(400))
                        {
                            CastQ(target);
                        }
                        break;
                    case "RivenMartyr":
                        if (Q.IsReady() && target.IsValidTarget(400))
                        {
                            CastQ(target);
                        }
                        else if (MenuInit.ComboR1 && R.IsReady() && !isRActive)
                        {
                            R1Logic(target);
                        }
                        break;
                    case "RivenFeint":
                        if (MenuInit.ComboR1 && R.IsReady() && !isRActive && target.IsValidTarget(500f))
                        {
                            R1Logic(target);
                        }
                        break;
                    case "RivenFengShuiEngine":
                        if (MenuInit.ComboW && W.IsReady() && target.IsValidTarget(W.Range))
                        {
                            Player.Instance.Spellbook.CastSpell(SpellSlot.W);
                        }
                        break;
                    default:
                        if (Args.SData.Name == "RivenIzunaBlade" && qStack == 2)
                        {
                            if (Q.IsReady() && target.IsValidTarget(400))
                            {
                                Player.Instance.Spellbook.CastSpell(SpellSlot.Q, target.Position);
                            }
                        }
                        break;
                }
            }
        }
    }
}
