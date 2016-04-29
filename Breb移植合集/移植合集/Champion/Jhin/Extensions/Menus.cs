using System.Linq;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using LeagueSharp.Common;

namespace Jhin___The_Virtuoso.Extensions
{
    internal static class Menus
    {
        /// <summary>
        ///     Menu
        /// </summary>
        public static Menu Config, qMenu, wMenu, eMenu, clearMenu, ksMenu, miscMenu, drawMenu, harassMenu;

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

        /// <summary>
        ///     Initialize menu
        /// </summary>
        public static void Initialize()
        {
            Config = MainMenu.AddMenu(":: 烬", ":: Jhin - The Virtuoso");

            qMenu = Config.AddSubMenu(":: Q", ":: Q");
            qMenu.Add("q.combo", new CheckBox("使用 (Q)"));

            wMenu = Config.AddSubMenu(":: W", ":: W");
            wMenu.Add("w.combo", new CheckBox("使用 (W)"));
            wMenu.Add("w.combo.min.distance", new Slider("最低距离", 400, 1, 2500));
            wMenu.Add("w.combo.max.distance", new Slider("最远距离", 1000, 1, 2500));
            wMenu.Add("w.passive.combo", new CheckBox("使用 W 如果敌方有标记", false));
            wMenu.Add("w.hit.chance",
                new ComboBox("(W) 命中率", 2, "低", "中", "高", "非常规", "不可移动的"));
                //.SetValue(new StringList(Provider.HitchanceNameArray, 2)));

            eMenu = Config.AddSubMenu(":: E", ":: E");
            eMenu.Add("e.combo", new CheckBox("使用 (E)"));
            eMenu.Add("e.combo.teleport", new CheckBox("自动 (E) 传送的"));
            eMenu.Add("e.hit.chance",
                new ComboBox("(E) 命中率", 2, "低", "中", "高", "非常规", "不可移动的"));
                //.SetValue(new StringList(Provider.HitchanceNameArray, 2)));

            harassMenu = Config.AddSubMenu(":: 骚扰设置", ":: Harass Settings");
            harassMenu.AddGroupLabel(":: W");
            harassMenu.Add("w.harass", new CheckBox("使用 (W)"));
            harassMenu.Add("harass.mana", new Slider("最低蓝量%", 50, 1, 99));

            clearMenu = Config.AddSubMenu(":: 推线设置", ":: Clear Settings");
            clearMenu.AddGroupLabel(":: 清线");
            clearMenu.Add("q.clear", new CheckBox("使用 (Q)"));
            clearMenu.Add("w.clear", new CheckBox("使用 (W)"));
            clearMenu.Add("w.hit.x.minion", new Slider("Min. Minion", 4, 1, 5));
            clearMenu.AddSeparator();
            clearMenu.AddGroupLabel(":: 清野");
            clearMenu.Add("q.jungle", new CheckBox("使用 (Q)"));
            clearMenu.Add("w.jungle", new CheckBox("使用 (W)"));
            clearMenu.AddSeparator();
            clearMenu.Add("clear.mana", new Slider("清线最低蓝量使用%", 50, 1, 99));
            clearMenu.Add("jungle.mana", new Slider("清野最低蓝量使用%", 50, 1, 99));

            ksMenu = Config.AddSubMenu(":: 抢头", ":: Kill Steal");
            ksMenu.Add("q.ks", new CheckBox("使用 (Q)"));
            ksMenu.Add("w.ks", new CheckBox("使用 (W)"));

            miscMenu = Config.AddSubMenu(":: 杂项", ":: Miscellaneous");
            miscMenu.Add("auto.e.immobile", new CheckBox("自动 (E) 不可移动目标"));
            miscMenu.AddSeparator();
            miscMenu.AddGroupLabel(":: 大招设置");
            miscMenu.AddLabel(":: R - 白名单");
            foreach (var enemy in HeroManager.Enemies.Where(enemy => enemy.IsValid))
            {
                miscMenu.Add("r.combo." + enemy.ChampionName, new CheckBox("(R): " + enemy.ChampionName));
            }
            miscMenu.Add("r.combo", new CheckBox("使用 (R)"));
            miscMenu.Add("auto.shoot.bullets", new CheckBox("如果按下 (R) 将自动发射子弹"));
            miscMenu.Add("r.hit.chance",
                new ComboBox("(R) 命中率", 1, "低", "中", "高", "非常规", "不可移动的"));
                //.SetValue(new StringList(Provider.HitchanceNameArray, 1)));
            miscMenu.Add("semi.manual.ult", new KeyBind("半自动 (R)!", false, KeyBind.BindTypes.HoldActive, 'A'));

            drawMenu = Config.AddSubMenu(":: 线圈", ":: Drawings");
            drawMenu.AddGroupLabel(":: 伤害显示");
            drawMenu.Add("aa.indicator", new CheckBox("(AA) 指示器"));
            drawMenu.Add("sniper.text", new CheckBox("狙击文字"));
            drawMenu.AddSeparator();
            drawMenu.Add("q.draw", new CheckBox("(Q) 范围"));
            drawMenu.Add("w.draw", new CheckBox("(W) 范围"));
            drawMenu.Add("e.draw", new CheckBox("(E) 范围"));
            drawMenu.Add("r.draw", new CheckBox("(R) 范围"));
        }
    }
}