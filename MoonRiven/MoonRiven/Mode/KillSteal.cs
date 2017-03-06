namespace MoonRiven.Mode
{
    using myCommon;

    using EloBuddy.SDK;

    using System.Linq;

    internal class KillSteal : Logic
    {
        internal static void InitTick()
        {
            if (MenuInit.KillStealR && isRActive && R1.IsReady())
            {
                foreach (
                    var target in
                    EntityManager.Heroes.Enemies.Where(
                        x => x.IsValidTarget(R.Range) && DamageCalculate.GetRDamage(x) > x.Health && !x.IsUnKillable()))
                {
                    if (target.IsValidTarget(R.Range - 100) && !target.IsValidTarget(500))
                    {
                        R1.Cast(target.Position);
                    }
                }
            }
        }
    }
}
