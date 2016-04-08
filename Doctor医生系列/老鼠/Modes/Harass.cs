using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using Settings = Twitch.Config.ModesMenu.Harass;
using SettingsPrediction = Twitch.Config.PredictionMenu;
using SettingsMana = Twitch.Config.ManaManagerMenu;

namespace Twitch.Modes
{
    public sealed class Harass : ModeBase
    {
        public override bool ShouldBeExecuted()
        {
            return Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass);
        }

        public override void Execute()
        {
            if (Settings.UseE && E.IsReady() && PlayerMana >= SettingsMana.MinEMana)
            {
                var enemy =
                    EntityManager.Heroes.Enemies.FirstOrDefault(e => e.IsValidTarget(E.Range) && EStacks(e) >= Settings.MinEStacks);
                if (enemy != null)
                {
                    E.Cast();
                    Debug.WriteChat("Casting E in Combo, Target: {0}, Distance: {1}", enemy.ChampionName,
                        "" + Player.Instance.Distance(enemy));
                }
            }
            if (Settings.UseW && W.IsReady() && PlayerMana >= SettingsMana.MinWMana)
            {
                var enemy = TargetSelector.GetTarget(W.Range, DamageType.True);
                if (enemy != null && enemy.IsValidTarget(W.Range))
                {
                    var pred = W.GetPrediction(enemy);
                    if (pred.HitChance >= SettingsPrediction.MinWHCHarass)
                    {
                        W.Cast(pred.CastPosition);
                        Debug.WriteChat("Casting W in Combo, Target: {0}, Distance: {1}", enemy.ChampionName,
                            "" + Player.Instance.Distance(enemy));
                    }
                }
            }
        }
    }
}
