namespace SkyLv_Taric
{
    using System;

    using LeagueSharp;
    using LeagueSharp.Common;
    using EloBuddy;
    using EloBuddy.SDK;
    using EloBuddy.SDK.Menu;
    using EloBuddy.SDK.Menu.Values;
    internal class Draw
    {
        #region #GET
        private static AIHeroClient Player
        {
            get
            {
                return SkyLv_Taric.Player;
            }
        }

        private static LeagueSharp.Common.Spell Q
        {
            get
            {
                return SkyLv_Taric.Q;
            }
        }

        private static LeagueSharp.Common.Spell W
        {
            get
            {
                return SkyLv_Taric.W;
            }
        }

        private static LeagueSharp.Common.Spell E
        {
            get
            {
                return SkyLv_Taric.E;
            }
        }

        private static LeagueSharp.Common.Spell R
        {
            get
            {
                return SkyLv_Taric.R;
            }
        }
        #endregion

        static Draw()
        {
            Drawing.OnDraw += Drawing_OnDraw;
        }

        public static bool getCheckBoxItem(Menu m, string item)
        {
            return m[item].Cast<CheckBox>().CurrentValue;
        }

        public static int getSliderItem(Menu m, string item)
        {
            return m[item].Cast<Slider>().CurrentValue;
        }

        public static bool getKeyBindItem(Menu m, string item)
        {
            return m[item].Cast<KeyBind>().CurrentValue;
        }

        public static int getBoxItem(Menu m, string item)
        {
            return m[item].Cast<ComboBox>().CurrentValue;
        }

        public static void Drawing_OnDraw(EventArgs args)
        {
            foreach (var spell in SkyLv_Taric.SpellList)
            {
                var menuItem = getCheckBoxItem(SkyLv_Taric.Draw, spell.Slot + "Range");

                var c = System.Drawing.Color.Orange;

                if (spell.Slot == SpellSlot.Q)
                {
                    c = System.Drawing.Color.Orange;
                }

                if (spell.Slot == SpellSlot.W)
                {
                    c = System.Drawing.Color.Green;
                }

                if (spell.Slot == SpellSlot.E)
                {
                    c = System.Drawing.Color.Blue;
                }

                if (spell.Slot == SpellSlot.R)
                {
                    c = System.Drawing.Color.Gold;
                }

                if (menuItem && (spell.Slot != SpellSlot.R || R.Level > 0))
                    Render.Circle.DrawCircle(Player.Position, spell.Range, c, getSliderItem(SkyLv_Taric.Draw, "SpellDraw.Radius"));
            }

            if (getCheckBoxItem(SkyLv_Taric.Draw, "DrawOrbwalkTarget"))
            {
                var orbT = Orbwalker.LastTarget;
                if (orbT.IsValidTarget())
                    Render.Circle.DrawCircle(orbT.Position, 100, System.Drawing.Color.Pink, getSliderItem(SkyLv_Taric.Draw, "OrbwalkDraw.Radius"));
            }
        }
    }
}
