using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using Leblanc.Common;
using SharpDX;
using Collision = LeagueSharp.Common.Collision;
using EloBuddy.SDK.Menu;
using EloBuddy;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK;

namespace Leblanc.Modes
{
    internal static class ModeFlee
    {
        public static Menu MenuLocal { get; private set; }

        public static void Init(Menu ParentMenu)
        {
            MenuLocal = ParentMenu.AddSubMenu("Flee", "Flee");
            {
                MenuLocal.Add("Flee.UseW", new CheckBox("W:"));
                MenuLocal.Add("Flee.UseR", new CheckBox("R:"));
                MenuLocal.Add("Flee.DrawMouse", new CheckBox("Show Mouse Cursor Position:"));
            }

            Game.OnUpdate += OnUpdate;
            Drawing.OnDraw += delegate(EventArgs args)
            {
                if (!Modes.ModeDraw.MenuLocal["Draw.Enable"].Cast<CheckBox>().CurrentValue)
                {
                    return;
                }

                if (!Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Flee))
                {
                    return;
                }

                if (MenuLocal["Flee.DrawMouse"].Cast<CheckBox>().CurrentValue)
                {
                    Render.Circle.DrawCircle(Game.CursorPos, 150f, System.Drawing.Color.Red);
                }
            };
        }

        private static void OnUpdate(EventArgs args)
        {
            if (!Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Flee))
            {
                return;
            }

            EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);

            if (MenuLocal["Flee.UseW"].Cast<CheckBox>().CurrentValue)
            {
                Champion.PlayerSpells.CastW(Game.CursorPos);
            }
            if (MenuLocal["Flee.UseR"].Cast<CheckBox>().CurrentValue)
            {
                Champion.PlayerSpells.CastW2(Game.CursorPos);
            }
        }
    }
}
