namespace MoonRiven_2.Mode
{
    using myCommon;

    using EloBuddy;
    using EloBuddy.SDK;

    using System.Linq;

    internal class LaneClear : Logic
    {
        internal static void InitTick()
        {
            if (TickCount - lastCastTime < 1200)
                return;

            var minions =
                EntityManager.MinionsAndMonsters.GetLaneMinions(EntityManager.UnitTeam.Enemy, Player.Instance.Position,
                    400).ToList();

            if (minions.Any())
            {
                if (MenuInit.LaneClearItem && minions.Count >= 3)
                {
                    UseItem();
                }

                if (MenuInit.LaneClearQ && MenuInit.LaneClearQSmart && Q.IsReady() && minions.Count >= 3)
                {
                    var minion = minions.FirstOrDefault();

                    if (minion != null)
                        Player.Instance.Spellbook.CastSpell(SpellSlot.Q, minion.Position);
                }

                if (MenuInit.LaneClearW && W.IsReady())
                {
                    if (minions.Count(x => W.IsInRange(x)) >= MenuInit.LaneClearWCount)
                    {
                        Player.Instance.Spellbook.CastSpell(SpellSlot.W);
                    }
                }
            }
        }

        internal static void InitAfterAttack(AttackableUnit target)
        {
            if (MenuInit.LaneClearQT && Q.IsReady() &&
                (target.Type == GameObjectType.obj_AI_Turret || target.Type == GameObjectType.obj_Turret ||
                 target.Type == GameObjectType.obj_Barracks ||
                 target.Type == GameObjectType.obj_BarracksDampener ||
                 target.Type == GameObjectType.obj_HQ) &&
                !EntityManager.Heroes.Enemies.Exists(x => x.IsValidRange(800) && x.DistanceToPlayer() <= 800))
            {
                Player.Instance.Spellbook.CastSpell(SpellSlot.Q, target.Position);
            }
            else if (target.Type == GameObjectType.obj_AI_Minion && target.Team != GameObjectTeam.Neutral)
            {
                var minions =
                    EntityManager.MinionsAndMonsters.GetLaneMinions(EntityManager.UnitTeam.Enemy, Player.Instance.Position,
                        400).ToList();

                if (minions.Any())
                {
                    if (MenuInit.LaneClearQ && Q.IsReady() && minions.Count >= 2)
                    {
                        var minion = minions.FirstOrDefault();

                        if (minion != null)
                            Player.Instance.Spellbook.CastSpell(SpellSlot.Q, minion.Position);
                    }
                }
            }
        }
    }
}
