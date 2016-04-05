using System;
using EloBuddy;
using EloBuddy.SDK;
using Ex1L_Riven.Modes;

namespace Ex1L_Riven.Base
{
    internal class ModeController
    {
        static ModeController()
        {
            Game.OnTick += OnTick;
        }

        private static void OnTick(EventArgs args)
        {
            PermaActive.Execute();

            if (Activemode(Orbwalker.ActiveModes.Combo))
            {
                Combo.Execute();
            }

            if (Activemode(Orbwalker.ActiveModes.JungleClear))
            {
                Jungle.Execute();
            }

            if (Activemode(Orbwalker.ActiveModes.LaneClear))
            {
                Lane.Execute();
            }

            if (Activemode(Orbwalker.ActiveModes.Flee))
            {
                Flee.Execute();
            }
        }

        public static bool Activemode(Orbwalker.ActiveModes id)
        {
            return Orbwalker.ActiveModesFlags.HasFlag(id);
        }

        public static void Initialize()
        {
        }
    }
}