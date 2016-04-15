namespace Khappa_Zix.Misc
{
    using System;

    using EloBuddy.SDK.Menu.Values;
    using EloBuddy.SDK.Rendering;

    using Load;

    using SharpDX;

    internal class Drawings
    {
        internal static void Drawing_OnDraw(EventArgs args)
        {
            if (menu.Draw["Q"].Cast<CheckBox>().CurrentValue)
            {
                Circle.Draw(Load.Q.IsReady() ? Color.OrangeRed : Color.DarkRed, Load.Q.Range, Load.player.Position);
            }

            if (menu.Draw["W"].Cast<CheckBox>().CurrentValue)
            {
                Circle.Draw(Load.W.IsReady() ? Color.OrangeRed : Color.DarkRed, Load.W.Range, Load.player.Position);
            }

            if (menu.Draw["E"].Cast<CheckBox>().CurrentValue)
            {
                Circle.Draw(Load.E.IsReady() ? Color.OrangeRed : Color.DarkRed, Load.E.Range, Load.player.Position);
            }
        }
    }
}