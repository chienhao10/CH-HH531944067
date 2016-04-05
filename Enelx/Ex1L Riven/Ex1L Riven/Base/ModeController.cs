using System;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu.Values;
using Ex1L_Riven.Modes;

namespace Ex1L_Riven.Base
{
    internal class ModeController
    {
        public static int Mode = 1;

        static ModeController()
        {
            Game.OnTick += OnTick;
        }

        public static void ModeSwitch(ValueBase<bool> sender, ValueBase<bool>.ValueChangeArgs args)
        {
            if (args.NewValue)
            {
                switch (Mode)
                {
                    case 1:
                        Mode = 2;
                        break;
                    case 2:
                        Mode = 1;
                        break;
                }
            }
        }

        private static void OnTick(EventArgs args)
        {
            PermaActive.Execute();

            if (Activemode(Orbwalker.ActiveModes.Combo))
            {
                switch (Mode)
                {
                    case 1:
                        Combo.Execute();
                        break;
                    case 2:
                        Burst.Execute();
                        break;
                }
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