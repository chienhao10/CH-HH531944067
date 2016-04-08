using SettingsMana = Twitch.Config.ManaManagerMenu;
using SettingsPrediction = Twitch.Config.PredictionMenu;

namespace Twitch.Modes
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
