using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using Irelia.Common;
using SharpDX;
using Collision = LeagueSharp.Common.Collision;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using EloBuddy;
using EloBuddy.SDK;

namespace Irelia.Modes
{
    internal static class ModeFlee
    {
        public static Menu MenuLocal { get; private set; }

        public static void Init(Menu ParentMenu)
        {
            MenuLocal = ParentMenu.AddSubMenu("Flee", "Flee");
            {
                MenuLocal.Add("Flee.UseQ", new ComboBox("Q:", 1 , "Off", "On"));//.SetValue(new StringList(new[] { "Off", "On" }, 1)).SetFontStyle(FontStyle.Regular, Champion.PlayerSpells.Q.MenuColor()));
                MenuLocal.Add("Flee.Youmuu", new ComboBox("Item Youmuu:", 1, "Off", "On"));//.SetValue(new StringList(new[] { "Off", "On" }, 1)).SetFontStyle(FontStyle.Regular, Champion.PlayerSpells.Q.MenuColor()));
                MenuLocal.Add("Flee.DrawMouse", new ComboBox("Draw Mouse Position:", 1, "Off", "On"));//.SetValue(new StringList(new[] { "Off", "On" }, 1)).SetFontStyle(FontStyle.Regular, Champion.PlayerSpells.Q.MenuColor()));
            }

            Game.OnUpdate += OnUpdate;
            Drawing.OnDraw += delegate(EventArgs args)
            {
                if (!Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Flee))
                {
                    return;
                }

                if (getBoxItem(MenuLocal, "Flee.DrawMouse") == 1)
                {
                    Render.Circle.DrawCircle(Game.CursorPos, 150f, System.Drawing.Color.Red);
                }
            };
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

        private static void OnUpdate(EventArgs args)
        {
            if (!Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Flee))
            {
                return;
            }

            EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
            Orbwalker.DisableAttacking = (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo));
            

            var t = TargetSelector.GetTarget(Champion.PlayerSpells.Q.Range, DamageType.Physical);
            if (t.LSIsValidTarget())
            {
                if (getBoxItem(MenuLocal, "Flee.UseQ") == 1 && Champion.PlayerSpells.Q.IsReady())
                {
                    Champion.PlayerSpells.CastQCombo(t);
                }

                if (getBoxItem(MenuLocal, "Flee.Youmuu") == 1 && Common.CommonItems.Youmuu.IsReady())
                {
                    Common.CommonItems.Youmuu.Cast();
                }
            }
        }
    }
}
