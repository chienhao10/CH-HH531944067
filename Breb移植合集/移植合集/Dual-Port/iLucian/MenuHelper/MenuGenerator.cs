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

        public static Menu comboOptions, harassOptions, laneclearOptions, miscOptions, jungleclearOptions;

        public static void Generate()
        {
            Variables.Menu = MainMenu.AddMenu("I卢锡安", "com.ilucian");

            comboOptions = Variables.Menu.AddSubMenu(":: I卢锡安 - 连招", "com.ilucian.combo");
            comboOptions.Add("com.ilucian.combo.q", new CheckBox("使用 Q", true));
            comboOptions.Add("com.ilucian.combo.qExtended", new CheckBox("使用 延长Q", true));
            comboOptions.Add("com.ilucian.combo.w", new CheckBox("使用 W", true));
            comboOptions.Add("com.ilucian.combo.e", new CheckBox("使用 E", true));
            comboOptions.Add("com.ilucian.combo.startE", new CheckBox("连招 E 起手", true));
            comboOptions.Add("com.ilucian.combo.eRange", new Slider("E 冲刺距离", 65, 50, 475));
            comboOptions.Add("com.ilucian.combo.eMode", new ComboBox("E 模式", 5, "风筝", "边上", "鼠标", "敌人", "快速模式", "智能 E", "URF（阿福快打）"));
            comboOptions.Add("com.ilucian.combo.forceR", new KeyBind("半自动R按键", false, KeyBind.BindTypes.HoldActive, 'T'));

            harassOptions = Variables.Menu.AddSubMenu(":: I卢锡安 - 骚扰", "com.ilucian.harass");
            harassOptions.AddGroupLabel("自动骚扰 : ");
            harassOptions.Add("com.ilucian.harass.auto.autoharass", new KeyBind("开关按键", false, KeyBind.BindTypes.PressToggle, 'Z'));
            harassOptions.Add("com.ilucian.harass.auto.q", new CheckBox("使用 Q", true));
            harassOptions.Add("com.ilucian.harass.auto.qExtended", new CheckBox("使用延长 Q", true));
            harassOptions.AddSeparator();
            harassOptions.Add("com.ilucian.harass.whitelist", new CheckBox("延长 Q 白名单", true));            
                foreach (var hero in HeroManager.Enemies)
                {
                  harassOptions.Add("com.ilucian.harass.whitelist." + hero.NetworkId, new CheckBox("不Q: " + hero.ChampionName));
                }
            
            harassOptions.AddGroupLabel("骚扰 : ");
            harassOptions.Add("com.ilucian.harass.q", new CheckBox("使用 Q", true));
            harassOptions.Add("com.ilucian.harass.qExtended", new CheckBox("使用延长 Q", true));
            harassOptions.Add("com.ilucian.harass.w", new CheckBox("使用 W", true));
            harassOptions.Add("com.ilucian.harass.minMana", new Slider("最低蓝量%使用", 80, 10, 100));

            laneclearOptions = Variables.Menu.AddSubMenu(":: I卢锡安 - 清线", "com.ilucian.laneclear");
            laneclearOptions.Add("com.ilucian.laneclear.q", new CheckBox("使用 Q", true));
            laneclearOptions.Add("com.ilucian.laneclear.qMinions", new Slider("对 X 小兵使用Q", 3, 1, 10));

            jungleclearOptions = Variables.Menu.AddSubMenu(":: iLucian - 清野", "com.ilucian.jungleclear");
            jungleclearOptions.Add("com.ilucian.jungleclear.q", new CheckBox("使用 Q", true));
            jungleclearOptions.Add("com.ilucian.jungleclear.w", new CheckBox("使用 W", true));
            jungleclearOptions.Add("com.ilucian.jungleclear.e", new CheckBox("使用 E", true));

            miscOptions = Variables.Menu.AddSubMenu(":: I卢锡安 - 杂项", "com.ilucian.misc");
            miscOptions.Add("com.ilucian.misc.antiVayne", new CheckBox("防薇恩定墙", true));
            miscOptions.Add("com.ilucian.misc.usePrediction", new CheckBox("使用 W 预判", true));
            miscOptions.Add("com.ilucian.misc.gapcloser", new CheckBox("使用 E 接近/防突进", true));
            miscOptions.Add("com.ilucian.misc.eqKs", new CheckBox("EQ - 抢头", true));
            miscOptions.Add("com.ilucian.misc.useChampions", new CheckBox("对敌方使用EQ", true));
            miscOptions.Add("com.ilucian.misc.extendChamps", new CheckBox("对敌方使用延长Q", true));
            miscOptions.Add("com.ilucian.misc.drawQ", new CheckBox("显示 延长Q 范围", true));
        }
    }
}