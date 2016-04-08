using System;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using Settings = Tristana.Config.ModesMenu.LaneClear;
using SettingsMana = Tristana.Config.ManaManagerMenu;
using SettingsPrediction = Tristana.Config.PredictionMenu;

namespace Tristana.Modes
{
    public sealed class LaneClear : ModeBase
    {
        public override bool ShouldBeExecuted()
        {
            return Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear);
        }

        public override void Execute()
        {
            if (Settings.UseW & W.IsReady() && PlayerMana >= SettingsMana.MinWMana)
            {
                var minions = EntityManager.MinionsAndMonsters.GetLaneMinions(EntityManager.UnitTeam.Enemy, _PlayerPos, W.Range + W.Width);
                var farmLoc = EntityManager.MinionsAndMonsters.GetCircularFarmLocation(minions, W.Width, (int)W.Range);
                if (farmLoc.HitNumber >= Settings.MinWTargets)
                {
                    W.Cast(farmLoc.CastPosition);
                    Debug.WriteChat("Casting W in LaneClear on {0} targets.", farmLoc.HitNumber.ToString());
                }
            }
        }
    }
}
