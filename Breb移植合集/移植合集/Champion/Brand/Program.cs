using System;
using System.Drawing;
using EloBuddy;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using LeagueSharp.Common;
using PortAIO.Properties;
using TheBrand;

namespace PortAIO.Champion.Brand
{
    internal class Program
    {
        private static BrandCombo _comboProvider;
        private static Menu _mainMenu, rOptions, miscMenu, drawingMenu, laneclearMenu;

        public static bool getMiscMenuCB(string item)
        {
            return miscMenu[item].Cast<CheckBox>().CurrentValue;
        }

        public static int getMiscMenuSL(string item)
        {
            return miscMenu[item].Cast<Slider>().CurrentValue;
        }

        public static bool getDrawMenuCB(string item)
        {
            return drawingMenu[item].Cast<CheckBox>().CurrentValue;
        }

        public static int getLaneMenuSL(string item)
        {
            return laneclearMenu[item].Cast<Slider>().CurrentValue;
        }

        public static bool getRMenuCB(string item)
        {
            return rOptions[item].Cast<CheckBox>().CurrentValue;
        }

        public static int getRMenuSL(string item)
        {
            return rOptions[item].Cast<Slider>().CurrentValue;
        }

        public static void Load()
        {
            try
            {
                if (ObjectManager.Player.ChampionName != "Brand")
                    return;

                _mainMenu = MainMenu.AddMenu("火男", "Brand");
                var comboMenu = _mainMenu.AddSubMenu("连招");
                var harassMenu = _mainMenu.AddSubMenu("骚扰");
                laneclearMenu = _mainMenu.AddSubMenu("清线");
                miscMenu = _mainMenu.AddSubMenu("杂项");
                drawingMenu = _mainMenu.AddSubMenu("线圈");

                _comboProvider = new BrandCombo(1050, new BrandQ(SpellSlot.Q), new BrandW(SpellSlot.W),
                    new BrandE(SpellSlot.E), new BrandR(SpellSlot.R));
                _comboProvider.CreateBasicMenu(comboMenu, harassMenu, null);
                _comboProvider.CreateLaneclearMenu(laneclearMenu, true, SpellSlot.Q, SpellSlot.R);

                #region Advanced Shit

                rOptions = _mainMenu.AddSubMenu("大招设置");
                rOptions.Add("BridgeR", new CheckBox("桥接 R", false));
                rOptions.Add("RiskyR", new CheckBox("风险 R"));
                rOptions.Add("Ultnonkillable", new CheckBox("R可击杀目标"));
                rOptions.Add("whenminXtargets", new Slider("^ 最少 X 目标数", 3, 1, 5));
                rOptions.Add("DontRwith", new CheckBox("不使用R当"));
                rOptions.Add("healthDifference", new Slider("生命百分比差为 X", 60, 1));
                rOptions.Add("Ignorewhenfleeing", new CheckBox("逃跑时无视"));

                laneclearMenu.Add("MinWtargets", new Slider("最低 W 命中数", 3, 1, 10));

                miscMenu.Add("eMinion", new CheckBox("对有火的小兵使用 E"));
                miscMenu.Add("aoeW", new CheckBox("尝试大范围 W"));
                miscMenu.Add("eFarmAssist", new CheckBox("E 尾兵助手"));
                miscMenu.Add("eKS", new CheckBox("E 抢头"));
                miscMenu.Add("KSCombo", new CheckBox("只在连招中抢头", false));
                miscMenu.Add("manaH", new Slider("骚扰蓝量", 50, 1));
                miscMenu.Add("manaLC", new Slider("清线蓝量", 80, 1));

                drawingMenu.Add("WPred", new CheckBox("显示 W 预判"));
                drawingMenu.Add("QRange", new CheckBox("Q 范围"));
                drawingMenu.Add("WRange", new CheckBox("W 范围"));
                drawingMenu.Add("ERange", new CheckBox("E 范围"));
                drawingMenu.Add("RRange", new CheckBox("R 范围"));

                #endregion

                _comboProvider.Initialize();

                Game.OnUpdate += Tick;
                Drawing.OnDraw += Draw;
            }
            catch (Exception ex)
            {
                Console.WriteLine(Resources.Program_Load_Error_initialitzing_TheBrand__ + ex);
            }
        }


        private static void Draw(EventArgs args)
        {
            if (ObjectManager.Player.IsDead) return;

            var q = getDrawMenuCB("QRange");
            var w = getDrawMenuCB("WRange");
            var e = getDrawMenuCB("ERange");
            var r = getDrawMenuCB("RRange");

            if (q)
                Render.Circle.DrawCircle(ObjectManager.Player.Position, 1050, Color.OrangeRed);
            if (w)
                Render.Circle.DrawCircle(ObjectManager.Player.Position, 900, Color.Red);
            if (e)
                Render.Circle.DrawCircle(ObjectManager.Player.Position, 650, Color.Goldenrod);
            if (r)
                Render.Circle.DrawCircle(ObjectManager.Player.Position, 750, Color.DarkViolet);
        }

        private static void Tick(EventArgs args)
        {
            _comboProvider.Update();
        }
    }
}