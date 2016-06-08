using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using OlafxQx.Common;
using SharpDX;
using Collision = LeagueSharp.Common.Collision;
using EloBuddy.SDK.Menu;
using EloBuddy;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK;

namespace OlafxQx.Modes
{
    internal static class ModeHarass
    {
        public static Menu MenuLocal { get; private set; }

        private static LeagueSharp.Common.Spell Q => Champion.PlayerSpells.Q;
        private static LeagueSharp.Common.Spell E => Champion.PlayerSpells.E;

        public static void Init()
        {
            MenuLocal = ModeConfig.MenuConfig.AddSubMenu("Harass", "Harass");
            {
                MenuLocal.Add("Harass.Q", new ComboBox("Q:", 1, "Off", "On: Small", "On: Large"));
                var qRange = (int) Q.Range;
                MenuLocal.Add("Harass.Q.SmallRange", new Slider("Q Small Range:", qRange / 2, qRange / 2, qRange));
                MenuLocal.Add("Harass.E", new ComboBox("E:", 0 , "Off", "On"));
            }

            Game.OnUpdate += OnUpdate;
        }

        private static void OnUpdate(EventArgs args)
        {
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                return;
            }

            if (ObjectManager.Player.ManaPercent < CommonManaManager.HarassMinManaPercent)
            {
                return;
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass))
            {
                ExecuteHarass();
            }

            if (ModeConfig.MenuKeys["Key.HarassToggle"].Cast<KeyBind>().CurrentValue)
            {
                ExecuteToggle();
            }
        }

        private static void ExecuteToggle()
        {
            ExecuteHarass();
        }

        private static void ExecuteHarass()
        {
            var useQ = MenuLocal["Harass.Q"].Cast<ComboBox>().CurrentValue;
            if (useQ == 0)
            {
                return;
            }

            var t = TargetSelector.GetTarget(useQ == 2 ? Q.Range : MenuLocal["Harass.Q.SmallRange"].Cast<Slider>().CurrentValue, DamageType.Physical);
            if (!t.IsValidTarget())
            {
                return;
            }

            if (t.UnderTurret(true))
            {
                return;
            }

            Champion.PlayerSpells.CastQ(t, useQ == 2 ? Q.Range : MenuLocal["Harass.Q.SmallRange"].Cast<Slider>().CurrentValue);

            if (MenuLocal["Harass.E"].Cast<ComboBox>().CurrentValue == 1)
            {
                Champion.PlayerSpells.CastE(t);
            }
        }
    }
}
