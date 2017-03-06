namespace MoonRiven.Mode
{
    using EloBuddy;
    using EloBuddy.SDK;

    using System;

    internal class Burst : Logic
    {
        internal static void InitTick()
        {
            var target = TargetSelector.SelectedTarget;

            if (target == null || !target.IsValidTarget())
                return;

            if (MenuInit.BurstMode == 0)
            {
                ShyBurstTick(target);
            }
            else
            {
                EQFlashBurstTick(target);
            }
        }

        private static void ShyBurstTick(AIHeroClient target)
        {
            if (MenuInit.BurstDot && Ignite != SpellSlot.Unknown && Player.Instance.Spellbook.GetSpell(Ignite).IsReady)
            {
                Player.Instance.Spellbook.CastSpell(Ignite, target);
            }

            if (E.IsReady() && R.IsReady() && W.IsReady() && !isRActive)
            {
                if (target.IsValidTarget(E.Range + Player.Instance.BoundingRadius - 30))
                {
                    Player.Instance.Spellbook.CastSpell(SpellSlot.E, target.Position);
                    Core.DelayAction(() => R.Cast(), 10);
                    Core.DelayAction(() => Player.Instance.Spellbook.CastSpell(SpellSlot.W), 60);
                    Core.DelayAction(() => Player.Instance.Spellbook.CastSpell(SpellSlot.Q), 150);
                    return;
                }

                if (MenuInit.BurstFlash && Flash != SpellSlot.Unknown && Player.Instance.Spellbook.GetSpell(Flash).IsReady)
                {
                    if (target.IsValidTarget(E.Range + Player.Instance.BoundingRadius + 425 - 50))
                    {
                        Player.Instance.Spellbook.CastSpell(SpellSlot.E, target.Position);
                        Core.DelayAction(() => R.Cast(), 10);
                        Core.DelayAction(() => Player.Instance.Spellbook.CastSpell(SpellSlot.W), 60);
                        Core.DelayAction(() => Player.Instance.Spellbook.CastSpell(Flash, target.Position), 61);
                        Core.DelayAction(() => Player.Instance.Spellbook.CastSpell(SpellSlot.Q), 150);
                    }
                }
            }
            else
            {
                if (W.IsReady() && target.IsValidTarget(W.Range))
                {
                    Player.Instance.Spellbook.CastSpell(SpellSlot.W);
                }
            }
        }

        private static void EQFlashBurstTick(AIHeroClient target)
        {
            if (MenuInit.BurstDot && Ignite != SpellSlot.Unknown && Player.Instance.Spellbook.GetSpell(Ignite).IsReady)
            {
                Player.Instance.Spellbook.CastSpell(Ignite, target);
            }

            if (MenuInit.BurstFlash && Flash != SpellSlot.Unknown && Player.Instance.Spellbook.GetSpell(Flash).IsReady)
            {
                if (target.IsValidTarget(E.Range + 425 + Q.Range - 150) && qStack > 0 && E.IsReady() && R.IsReady() &&
                    !isRActive && W.IsReady())
                {
                    Player.Instance.Spellbook.CastSpell(SpellSlot.E, target.Position);
                    Core.DelayAction(() => R.Cast(), 10);
                    Core.DelayAction(() => Player.Instance.Spellbook.CastSpell(Flash, target.Position), 50);
                    Core.DelayAction(() => Player.Instance.Spellbook.CastSpell(SpellSlot.Q, target.Position), 61);
                    Core.DelayAction(() => UseItem(), 62);
                    Core.DelayAction(() => Player.Instance.Spellbook.CastSpell(SpellSlot.W), 70);
                    Core.DelayAction(() => R1.Cast(target.Position), 71);
                    return;
                }

                if (qStack < 2 && Environment.TickCount - lastQTime >= 850)
                {
                    Player.Instance.Spellbook.CastSpell(SpellSlot.Q, Game.CursorPos);
                }
            }
            else
            {
                if (target.IsValidTarget(E.Range + Q.Range - 150) && qStack == 2 && E.IsReady() && R.IsReady() && !isRActive && W.IsReady())
                {
                    Player.Instance.Spellbook.CastSpell(SpellSlot.E, target.Position);
                    Core.DelayAction(() => R.Cast(), 10);
                    Core.DelayAction(() => Player.Instance.Spellbook.CastSpell(SpellSlot.Q, target.Position), 50);
                    Core.DelayAction(() => UseItem(), 61);
                    Core.DelayAction(() => Player.Instance.Spellbook.CastSpell(SpellSlot.W), 62);
                    Core.DelayAction(() => R1.Cast(target.Position), 70);
                    return;
                }

                if (target.IsValidTarget(E.Range + Q.Range + Q.Range + Q.Range))
                {
                    if (qStack < 2 && Environment.TickCount - lastQTime >= 850)
                    {
                        Player.Instance.Spellbook.CastSpell(SpellSlot.Q, Game.CursorPos);
                    }
                }
            }
        }

        internal static void InitAfterAttack(AttackableUnit tar)
        {
            var target = TargetSelector.SelectedTarget;

            if (target == null || !target.IsValidTarget())
                return;

            if (MenuInit.BurstMode == 0)
            {
                ShyBurst(target);
            }
            else
            {
                EQFlashBurst(target);
            }
        }

        private static void ShyBurst(AIHeroClient target)
        {
            UseItem();

            if (R.IsReady() && isRActive)
            {
                R1.Cast(target.Position);
                return;
            }

            if (Q.IsReady() && CastQ(target))
            {
                return;
            }

            if (W.IsReady() && target.IsValidTarget(W.Range) && Player.Instance.Spellbook.CastSpell(SpellSlot.W))
            {
                return;
            }

            if (E.IsReady())
            {
                Player.Instance.Spellbook.CastSpell(SpellSlot.E, target.Position);
            }
        }

        private static void EQFlashBurst(AIHeroClient target)
        {
            UseItem();

            if (R.IsReady() && isRActive)
            {
                R1.Cast(target.Position);
                return;
            }

            if (Q.IsReady() && CastQ(target))
            {
                return;
            }

            if (W.IsReady() && target.IsValidTarget(W.Range) && Player.Instance.Spellbook.CastSpell(SpellSlot.W))
            {
                return;
            }

            if (E.IsReady())
            {
                Player.Instance.Spellbook.CastSpell(SpellSlot.E, target.Position);
            }
        }

        internal static void InitSpellCast(GameObjectProcessSpellCastEventArgs Args)
        {
            if (Args.SData == null)
            {
                return;
            }

            var target = TargetSelector.SelectedTarget;

            if (target != null && target.IsValidTarget())
            {
                if (MenuInit.BurstMode == 0)
                {
                    ShyBurst(target, Args);
                }
                else
                {
                    EQFlashBurst(target, Args);
                }
            }
        }

        private static void ShyBurst(AIHeroClient target, GameObjectProcessSpellCastEventArgs Args)
        {
            switch (Args.SData.Name)
            {
                case "ItemTiamatCleave":
                    if (W.IsReady() && target.IsValidTarget(W.Range))
                    {
                        Player.Instance.Spellbook.CastSpell(SpellSlot.W);
                    }
                    else if (Q.IsReady() && target.IsValidTarget(400))
                    {
                        CastQ(target);
                    }
                    break;
                case "RivenMartyr":
                    if (MenuInit.ComboR2 == 0 && R.IsReady() && isRActive)
                    {
                        R1.Cast(target.Position);
                    }
                    else if (Q.IsReady() && target.IsValidTarget(400))
                    {
                        CastQ(target);
                    }
                    break;
                case "RivenFeint":
                    if (Item.HasItem(3142) && Item.CanUseItem(3142))
                    {
                        Item.UseItem(3142);
                    }

                    if (R.IsReady() && !isRActive)
                    {
                        R.Cast();
                    }
                    break;
                case "RivenIzunaBlade":
                    if (Q.IsReady() && target.IsValidTarget(400))
                    {
                        Player.Instance.Spellbook.CastSpell(SpellSlot.Q, target.Position);
                    }
                    break;
            }
        }

        private static void EQFlashBurst(AIHeroClient target, GameObjectProcessSpellCastEventArgs Args)
        {
            switch (Args.SData.Name)
            {
                case "ItemTiamatCleave":
                    if (W.IsReady() && target.IsValidTarget(W.Range))
                    {
                        Player.Instance.Spellbook.CastSpell(SpellSlot.W);
                    }
                    else if (Q.IsReady() && target.IsValidTarget(400))
                    {
                        CastQ(target);
                    }
                    break;
                case "RivenMartyr":
                    if (MenuInit.ComboR2 == 0 && R.IsReady() && isRActive)
                    {
                        R1.Cast(target.Position);
                    }
                    else if (Q.IsReady() && target.IsValidTarget(400))
                    {
                        CastQ(target);
                    }
                    break;
                case "RivenIzunaBlade":
                    if (Q.IsReady() && target.IsValidTarget(400))
                    {
                        Player.Instance.Spellbook.CastSpell(SpellSlot.Q, target.Position);
                    }
                    break;
            }
        }
    }
}
