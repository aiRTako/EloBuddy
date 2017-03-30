namespace MoonRiven_2.Mode
{
    using EloBuddy;
    using EloBuddy.SDK;
    using EloBuddy.SDK.Events;
    using myCommon;
    using System.Linq;

    internal class Flee : Logic
    {
        internal static void InitTick()
        {
            if (MenuInit.FleeW && W.IsReady() && EntityManager.Heroes.Enemies.Any(x => x.IsValidRange(W.Range) && !HaveShield(x)))
            {
                Player.Instance.Spellbook.CastSpell(SpellSlot.W);
            }

            if (MenuInit.FleeQ && Q.IsReady() && !Player.Instance.IsDashing())
            {
                Player.Instance.Spellbook.CastSpell(SpellSlot.Q, Player.Instance.Position.Extend(Game.CursorPos, 350f).To3D());
            }

            if (MenuInit.FleeE && E.IsReady() && ((!Q.IsReady() && qStack == 0) || qStack == 2))
            {
                Player.Instance.Spellbook.CastSpell(SpellSlot.E, Player.Instance.Position.Extend(Game.CursorPos, E.Range).To3D());
            }
        }
    }
}
