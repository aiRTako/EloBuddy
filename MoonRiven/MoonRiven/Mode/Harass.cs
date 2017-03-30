namespace MoonRiven_2.Mode
{
    using myCommon;

    using EloBuddy;
    using EloBuddy.SDK;

    using System;

    internal class Harass : Logic
    {
        internal static void InitTick()
        {
            var target = TargetSelector.GetTarget(E.Range + Player.Instance.BoundingRadius, DamageType.Physical);

            if (target.IsValidRange())
            {
                if (MenuInit.HarassMode == 0)
                {
                    if (E.IsReady() && MenuInit.HarassE && qStack == 2)
                    {
                        var pos = Player.Instance.Position + (Player.Instance.Position - target.Position).Normalized() * E.Range;
                        Player.Instance.Spellbook.CastSpell(SpellSlot.E,
                            Player.Instance.Position.Extend(pos, E.Range).To3D());
                    }

                    if (Q.IsReady() && MenuInit.HarassQ && qStack == 2)
                    {
                        var pos = Player.Instance.Position + (Player.Instance.Position - target.Position).Normalized() * E.Range;

                        Core.DelayAction(() => Player.Instance.Spellbook.CastSpell(SpellSlot.Q,
                            Player.Instance.Position.Extend(pos, Q.Range).To3D()), 100);
                    }

                    if (W.IsReady() && MenuInit.HarassW && target.IsValidRange(W.Range) && qStack == 1)
                    {
                        Player.Instance.Spellbook.CastSpell(SpellSlot.W);
                    }

                    if (Q.IsReady() && MenuInit.HarassQ)
                    {
                        if (qStack == 0)
                        {
                            CastQ(target);
                            Orbwalker.ForcedTarget = target;
                        }

                        if (qStack == 1 && Environment.TickCount - lastQTime > 600)
                        {
                            CastQ(target);
                            Orbwalker.ForcedTarget = target;
                        }
                    }
                }
                else
                {
                    if (E.IsReady() && MenuInit.HarassE &&
                        target.DistanceToPlayer() <= E.Range + (Q.IsReady() ? Q.Range : Player.Instance.AttackRange))
                    {
                        Player.Instance.Spellbook.CastSpell(SpellSlot.E, target.Position);
                    }

                    if (Q.IsReady() && MenuInit.HarassQ && target.IsValidRange(Q.Range) && qStack == 0 &&
                        Environment.TickCount - lastQTime > 500)
                    {
                        CastQ(target);
                        Orbwalker.ForcedTarget = target;
                    }

                    if (W.IsReady() && MenuInit.HarassW && target.IsValidRange(W.Range) && (!Q.IsReady() || qStack == 1))
                    {
                        Player.Instance.Spellbook.CastSpell(SpellSlot.W);
                    }
                }
            }
        }

        internal static void InitAfterAttack(AttackableUnit tar)
        {
            AIHeroClient target = null;

            if (myTarget.IsValidRange())
            {
                target = myTarget;
            }
            else if (tar is AIHeroClient)
            {
                target = (AIHeroClient)tar;
            }

            if (target == null || !target.IsValidRange())
                return;

            if (MenuInit.HarassQ && Q.IsReady())
            {
                if (MenuInit.HarassMode == 0)
                {
                    if (qStack == 1)
                    {
                        CastQ(target);
                    }
                }
                else
                {
                    CastQ(target);
                }
            }
        }
    }
}
