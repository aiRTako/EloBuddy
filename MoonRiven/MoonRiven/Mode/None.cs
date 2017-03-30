namespace MoonRiven_2.Mode
{
    using EloBuddy;
    using EloBuddy.SDK;
    using myCommon;
    using System;

    internal class None : Logic
    {
        internal static void Init()
        {
            if (TargetSelector.SelectedTarget != null && TargetSelector.SelectedTarget.IsValidRange())
            {
                myTarget = TargetSelector.SelectedTarget;
            }
            else if (Orbwalker.GetTarget() != null  && Orbwalker.GetTarget().Type == GameObjectType.AIHeroClient && 
                Orbwalker.GetTarget().IsValidRange())
            {
                myTarget = (AIHeroClient)Orbwalker.GetTarget();
            }
            else
            {
                myTarget = null;
            }

            if (W.Level > 0)
            {
                W.Range = (uint)(Player.HasBuff("RivenFengShuiEngine") ? 330 : 260);
            }

            if (MenuInit.KeepQ && Player.HasBuff("RivenTriCleave"))
            {
                if (Player.GetBuff("RivenTriCleave").EndTime - Game.Time < 0.3)
                {
                    Player.Instance.Spellbook.CastSpell(SpellSlot.Q, Game.CursorPos);
                }
            }

            if (qStack != 0 && Environment.TickCount - lastQTime > 3800)
            {
                qStack = 0;
            }
        }
    }
}
