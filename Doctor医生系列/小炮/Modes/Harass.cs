using System;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using Settings = Tristana.Config.ModesMenu.Harass;
using SettingsMana = Tristana.Config.ManaManagerMenu;
using SettingsPrediction = Tristana.Config.PredictionMenu;

namespace Tristana.Modes
{
    public sealed class Harass : ModeBase
    {
        public override bool ShouldBeExecuted()
        {
            return false; // Doing it all in Events
            //return Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass);
        }

        public override void Execute()
        {
        }
    }
}
