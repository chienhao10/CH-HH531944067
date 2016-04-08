using EloBuddy.SDK;
using SettingsMana = Tristana.Config.ManaManagerMenu;
using SettingsPrediction = Tristana.Config.PredictionMenu;

namespace Tristana.Modes
{
    public sealed class LastHit : ModeBase
    {

        public override bool ShouldBeExecuted()
        {
            return false;
            //return Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LastHit);
        }

        public override void Execute()
        {
           
        }
    }
}
