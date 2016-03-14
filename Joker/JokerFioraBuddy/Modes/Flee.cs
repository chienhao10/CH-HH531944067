using EloBuddy;
using EloBuddy.SDK;
using SharpDX;
using System;

using Settings = JokerFioraBuddy.Config.Modes.Flee;

namespace JokerFioraBuddy.Modes
{
    public sealed class Flee : ModeBase
    {
        public override bool ShouldBeExecuted()
        {
            return Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Flee);
        }

        public override void Execute()
        {
            if (Settings.UseQ && Q.IsReady())
                Q.Cast(Game.CursorPos);
        }
    }
}
