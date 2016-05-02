namespace KappaEkko.Events
{
    using System;

    using EloBuddy;
    using EloBuddy.SDK;
    using EloBuddy.SDK.Menu.Values;

    using Modes;

    internal class OnUpdate
    {
        public static AIHeroClient Player = ObjectManager.Player;

        public static void Update(EventArgs args)
        {
            Player = ObjectManager.Player;
            var lanemana = Menu.ManaMenu["lanemana"].Cast<Slider>().CurrentValue;
            var harassmana = Menu.ManaMenu["harassmana"].Cast<Slider>().CurrentValue;
            if (Player.IsDead || MenuGUI.IsChatOpen || Player.IsRecalling())
            {
                return;
            }

            var flags = Orbwalker.ActiveModesFlags;
            if (flags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                Combo.Start();
            }

            if (flags.HasFlag(Orbwalker.ActiveModes.Harass) && EloBuddy.Player.Instance.ManaPercent >= harassmana)
            {
                Harass.Start();
            }

            if (flags.HasFlag(Orbwalker.ActiveModes.LaneClear) && EloBuddy.Player.Instance.ManaPercent >= lanemana)
            {
                Clear.Start();
            }

            if (flags.HasFlag(Orbwalker.ActiveModes.JungleClear))
            {
                Jungle.Start();
            }

            PermaActive.Active();
        }
    }
}