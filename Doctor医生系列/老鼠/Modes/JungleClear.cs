using System;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using Settings = Twitch.Config.ModesMenu.JungleClear;
using SettingsPrediction = Twitch.Config.PredictionMenu;
using SettingsMana = Twitch.Config.ManaManagerMenu;

namespace Twitch.Modes
{
    public sealed class JungleClear : ModeBase
    {
        public override bool ShouldBeExecuted()
        {
           
            return Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear);
        }

        public override void Execute()
        {
            if (Settings.UseE && E.IsReady() && PlayerMana >= SettingsMana.MinEMana)
            {
                var monstersCount =
                    EntityManager.MinionsAndMonsters.GetJungleMonsters(_PlayerPos, E.Range)
                        .Count(m => m.IsValidTarget() && EStacks(m) >= Settings.MinEStacks);
                if (monstersCount >= Settings.MinETargets)
                {
                    E.Cast();
                    Debug.WriteChat("Casting E in JungleClear, Targets: {0}", "" + monstersCount);
                }
            }
            if (Settings.UseW && W.IsReady() && PlayerMana >= SettingsMana.MinWMana)
            {
                var monsters =
                    EntityManager.MinionsAndMonsters.GetJungleMonsters(_PlayerPos, W.Range)
                        .Where(m => m.IsValidTarget());
                var position = EntityManager.MinionsAndMonsters.GetCircularFarmLocation(monsters, W.Width, (int) W.Range);
                if (position.HitNumber >= Settings.MinWTargets)
                {
                    W.Cast(position.CastPosition);
                    Debug.WriteChat("Casting W in JungleClear, Targets: {0}", "" + position.HitNumber);
                }
            }
        }
    }
}
