using System;
using EloBuddy;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK.Rendering;
using Color = SharpDX.Color;

namespace LelBlanc
{
    internal class OnDraw
    {
        public static void DrawRange(EventArgs args)
        {
            if (Config.DrawingMenu["drawQ"].Cast<CheckBox>().CurrentValue)
            {
                Circle.Draw(!Program.Q.IsReady() ? Color.Red : Color.Green, Program.Q.Range, Player.Instance.Position);
            }
            if (Config.DrawingMenu["drawW"].Cast<CheckBox>().CurrentValue)
            {
                Circle.Draw(!Program.W.IsReady() ? Color.Red : Color.Green, Program.W.Range, Player.Instance.Position);
            }
            if (Config.DrawingMenu["drawE"].Cast<CheckBox>().CurrentValue)
            {
                Circle.Draw(!Program.E.IsReady() ? Color.Red : Color.Green, Program.E.Range, Player.Instance.Position);
            }
        }
    }
}