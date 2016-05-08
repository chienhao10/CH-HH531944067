using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp.Common;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;

namespace iLucian.MenuHelper
{
    class MenuGenerator
    {

        public static Menu comboOptions, harassOptions, laneclearOptions, miscOptions;

        public static void Generate()
        {
            Variables.Menu = MainMenu.AddMenu("I卢锡安", "com.ilucian");

            comboOptions = Variables.Menu.AddSubMenu(":: I卢锡安 - 连招", "com.ilucian.combo");
            comboOptions.Add("com.ilucian.combo.q", new CheckBox("使用 Q", true));
            comboOptions.Add("com.ilucian.combo.qExtended", new CheckBox("使用 延长Q", true));
            comboOptions.Add("com.ilucian.combo.w", new CheckBox("使用 W", true));
            comboOptions.Add("com.ilucian.combo.e", new CheckBox("使用 E", true));
            comboOptions.Add("com.ilucian.combo.eMode", new ComboBox("E 模式", 0, "风筝", "边上", "鼠标", "敌人"));

            harassOptions = Variables.Menu.AddSubMenu(":: I卢锡安 - 骚扰", "com.ilucian.harass");
            harassOptions.Add("com.ilucian.harass.q", new CheckBox("使用 Q", true));
            harassOptions.Add("com.ilucian.harass.qExtended", new CheckBox("使用 延长Q", true));
            harassOptions.Add("com.ilucian.harass.w", new CheckBox("使用 W", true));

            laneclearOptions = Variables.Menu.AddSubMenu(":: I卢锡安 - 清线", "com.ilucian.laneclear");
            laneclearOptions.Add("com.ilucian.laneclear.q", new CheckBox("使用 Q", true));
            laneclearOptions.Add("com.ilucian.laneclear.qMinions", new Slider("对 X 小兵使用Q", 3, 1, 10));

            miscOptions = Variables.Menu.AddSubMenu(":: I卢锡安 - 杂项", "com.ilucian.misc");
            miscOptions.Add("com.ilucian.misc.usePrediction", new CheckBox("使用 W 预判", true));
            miscOptions.Add("com.ilucian.misc.gapcloser", new CheckBox("使用 E 接近/防突进", true));
            miscOptions.Add("com.ilucian.misc.eqKs", new CheckBox("EQ - 抢头", true));
        }
    }
}