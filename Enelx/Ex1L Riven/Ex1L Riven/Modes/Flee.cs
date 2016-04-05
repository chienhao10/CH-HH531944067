using EloBuddy;
using EloBuddy.SDK;
using Ex1L_Riven.Base;

namespace Ex1L_Riven.Modes
{
    internal class Flee
    {
        public static bool ShouldBeExecuted()
        {
            return ModeController.Activemode(Orbwalker.ActiveModes.Flee);
        }

        public static void Execute()
        {
            if (Spells.Q.IsReady())
            {
                Player.CastSpell(SpellSlot.Q, Game.CursorPos);
            }

            if (Spells.E.IsReady())
            {
                Player.CastSpell(SpellSlot.E, Game.CursorPos);
            }
        }
    }
}