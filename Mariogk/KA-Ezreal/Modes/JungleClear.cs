using System.Linq;
using EloBuddy;
using EloBuddy.SDK;

using Settings = KA_Ezreal.Config.Modes.LaneClear;

namespace KA_Ezreal.Modes
{
    public sealed class JungleClear : ModeBase
    {
        public override bool ShouldBeExecuted()
        {
            return Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear);
        }

        public override void Execute()
        {
            var jungleMonsters =
                EntityManager.MinionsAndMonsters.GetJungleMonsters()
                    .OrderByDescending(m => m.MaxHealth)
                    .FirstOrDefault(m => m.IsValidTarget(Q.Range));
            if (jungleMonsters == null) return;
            if (Settings.UseQjungle && Q.IsReady() && Settings.JungleMana <= Player.Instance.ManaPercent)
            {
                Q.Cast(jungleMonsters);
            }
        }
    }
}
