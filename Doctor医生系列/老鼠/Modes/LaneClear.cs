using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using Settings = Twitch.Config.ModesMenu.LaneClear;
using SettingsPrediction = Twitch.Config.PredictionMenu;
using SettingsMana = Twitch.Config.ManaManagerMenu;

namespace Twitch.Modes
{
    public sealed class LaneClear : ModeBase
    {
        public override bool ShouldBeExecuted()
        {
            return Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear);
        }

        public override void Execute()
        {
            if (Settings.UseE && E.IsReady() && PlayerMana >= SettingsMana.MinEMana)
            {
                var minionCount =
                    EntityManager.MinionsAndMonsters.GetLaneMinions(EntityManager.UnitTeam.Enemy, _PlayerPos, E.Range)
                        .Count(m => m.IsValidTarget() && EStacks(m) >= 1);
                if (minionCount >= Settings.MinETargets)
                {
                    E.Cast();
                    Debug.WriteChat("Casting E in LaneClear, Targets: {0}", "" + minionCount);
                }
            }
            if (Settings.UseW && W.IsReady() && PlayerMana >= SettingsMana.MinWMana)
            {
                var minions =
                    EntityManager.MinionsAndMonsters.GetLaneMinions(EntityManager.UnitTeam.Enemy, _PlayerPos, W.Range)
                        .Where(m => m.IsValidTarget());
                var position = EntityManager.MinionsAndMonsters.GetCircularFarmLocation(minions, W.Width, (int)W.Range);
                if (position.HitNumber >= Settings.MinWTargets)
                {
                    W.Cast(position.CastPosition);
                    Debug.WriteChat("Casting W in LaneClear, Targets: {0}", "" + position.HitNumber);
                }
            }
        }
    }
}
