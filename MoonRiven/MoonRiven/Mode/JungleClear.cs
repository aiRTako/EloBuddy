namespace MoonRiven_2.Mode
{
    using myCommon;

    using EloBuddy;
    using EloBuddy.SDK;

    using System.Linq;

    internal class JungleClear : Logic
    {
        internal static void InitTick()
        {
            if (!MenuInit.JungleClearE || !E.IsReady())
                return;

            var mobs =
                EntityManager.MinionsAndMonsters.GetJungleMonsters(Player.Instance.Position,
                    E.Range + Player.Instance.GetAutoAttackRange()).ToList();

            if (mobs.Any())
            {
                if (!Q.IsReady() && qStack == 0 && !W.IsReady())
                {
                    Player.Instance.Spellbook.CastSpell(SpellSlot.E, Game.CursorPos);
                }

                if (!mobs.Any(x => x.DistanceToPlayer() <= E.Range))
                {
                    var mob = mobs.FirstOrDefault();

                    if (mob != null)
                    {
                        Player.Instance.Spellbook.CastSpell(SpellSlot.E, mob.Position);
                    }
                }
            }
        }

        internal static void InitAfterAttack(AttackableUnit tar)
        {
            var mobs =
                EntityManager.MinionsAndMonsters.GetJungleMonsters(Player.Instance.Position,
                    E.Range + Player.Instance.GetAutoAttackRange()).ToList();

            if (!mobs.Any())
                return;

            var target = tar as Obj_AI_Minion;

            if (target != null && target.IsValidRange(400) && target.Health > Player.Instance.GetAutoAttackDamage(target, true) &&
                !target.Name.Contains("Plant"))
            {
                if (MenuInit.JungleClearItem)
                {
                    UseItem();
                }

                if (MenuInit.JungleClearQ && Q.IsReady() && target.IsValidRange(400) && CastQ(target))
                {
                    return;
                }

                if (MenuInit.JungleClearW && W.IsReady() && target.IsValidRange(W.Range) && 
                    Player.Instance.Spellbook.CastSpell(SpellSlot.W))
                {
                    return;
                }

                if (MenuInit.JungleClearE && E.IsReady() && target.IsValidRange(400))
                {
                    Player.Instance.Spellbook.CastSpell(SpellSlot.E, Game.CursorPos);
                }
            }
        }
    }
}
