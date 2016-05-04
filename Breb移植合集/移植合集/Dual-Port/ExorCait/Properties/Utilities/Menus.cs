using LeagueSharp.Common;

namespace ExorAIO.Champions.Caitlyn
{
    using System.Drawing;
    using ExorAIO.Utilities;
    using Color = SharpDX.Color;
    using EloBuddy.SDK.Menu.Values;
    using EloBuddy.SDK.Menu;    /// <summary>
                                ///     The menu class.
                                /// </summary>
    class Menus
    {
        /// <summary>
        ///     Initializes the menus.
        /// </summary>
        /// 

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

        public static void Initialize()
        {
            Variables.QMenu = Variables.Menu.AddSubMenu("使用 Q 至:", "qmenu");
            Variables.QMenu.Add("qspell.auto", new CheckBox("逻辑"));
            Variables.QMenu.Add("qspell.ks", new CheckBox("前提"));
            Variables.QMenu.Add("qspell.farm", new CheckBox("清线"));
            Variables.QMenu.Add("qspell.mana", new Slider("清线: 蓝 >= x%", 50));

            Variables.WMenu = Variables.Menu.AddSubMenu("使用 W 至:", "wmenu");
            Variables.WMenu.Add("wspell.auto", new CheckBox("逻辑"));
            Variables.WMenu.Add("wspell.gp", new CheckBox("防突进"));

            Variables.EMenu = Variables.Menu.AddSubMenu("使用 E 至:", "emenu");
            Variables.EMenu.Add("espell.combo", new CheckBox("连招"));
            Variables.EMenu.Add("espell.gp", new CheckBox("防突进"));

            Variables.RMenu = Variables.Menu.AddSubMenu("使用 R 至:", "rmenu");
            Variables.RMenu.Add("rspell.ks", new CheckBox("抢头"));

            Variables.DrawingsMenu = Variables.Menu.AddSubMenu("线圈", "drawingsmenu");
            Variables.DrawingsMenu.Add("drawings.q", new CheckBox("Q 范围", false));
            Variables.DrawingsMenu.Add("drawings.w", new CheckBox("W 范围", false));
            Variables.DrawingsMenu.Add("drawings.e", new CheckBox("E 范围", false));
            Variables.DrawingsMenu.Add("drawings.r", new CheckBox("R 范围", false));
        }
    }
}
