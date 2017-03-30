namespace MoonRiven_2.Mode
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
                        x => x.IsValidRange(R.Range) && DamageCalculate.GetRDamage(x) > x.Health && !x.IsUnKillable()))
                {
                    if (target.IsValidRange(R.Range - 100) && !target.IsValidRange(500))
                    {
                        R1.Cast(target.Position);
                    }
                }
            }
        }
    }
}
