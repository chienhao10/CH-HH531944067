using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using SharpDX;
using Settings = Twitch.Config.ModesMenu.Flee;
using SettingsPrediction = Twitch.Config.PredictionMenu;
using SettingsMana = Twitch.Config.ManaManagerMenu;

namespace Twitch.Modes
{
    public sealed class Flee : ModeBase
    {
        public override bool ShouldBeExecuted()
        {
            return Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Flee);
        }

        public override void Execute()
        {
            if (Settings.UseQ && Q.IsReady() && PlayerMana >= SettingsMana.MinQMana)
            {
                Q.Cast();
            }
            if (Settings.UseW && W.IsReady() && !QActive && PlayerMana >= SettingsMana.MinQMana)
            {
                var enemy =
                    EntityManager.Heroes.Enemies.Where(
                        e => e.IsValidTarget(W.Range) && W.GetPrediction(e).HitChance >= SettingsPrediction.MinWHCFlee)
                        .OrderBy(e => e.Distance(_Player))
                        .FirstOrDefault();
                if (enemy != null)
                {
                    W.Cast(W.GetPrediction(enemy).CastPosition);
                    Debug.WriteChat("Casting W in Flee, Target: {0}, Distance: {1}", enemy.ChampionName,
                        "" + Player.Instance.Distance(enemy));
                }
            }
            
        }
    }
}
