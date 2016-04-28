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

                _mainMenu = MainMenu.AddMenu("The Brand", "Brand");
                var comboMenu = _mainMenu.AddSubMenu("Combo");
                var harassMenu = _mainMenu.AddSubMenu("Harass");
                laneclearMenu = _mainMenu.AddSubMenu("Lane Clear");
                miscMenu = _mainMenu.AddSubMenu("Misc");
                drawingMenu = _mainMenu.AddSubMenu("Draw");

                _comboProvider = new BrandCombo(1050, new BrandQ(SpellSlot.Q), new BrandW(SpellSlot.W),
                    new BrandE(SpellSlot.E), new BrandR(SpellSlot.R));
                _comboProvider.CreateBasicMenu(comboMenu, harassMenu, null);
                _comboProvider.CreateLaneclearMenu(laneclearMenu, true, SpellSlot.Q, SpellSlot.R);

                #region Advanced Shit

                rOptions = _mainMenu.AddSubMenu("Ult Options");
                rOptions.Add("BridgeR", new CheckBox("Bridge R", false));
                rOptions.Add("RiskyR", new CheckBox("Risky R"));
                rOptions.Add("Ultnonkillable", new CheckBox("Ult non killable"));
                rOptions.Add("whenminXtargets", new Slider("^ When min X targets", 3, 1, 5));
                rOptions.Add("DontRwith", new CheckBox("Don't R with"));
                rOptions.Add("healthDifference", new Slider("% Health difference", 60, 1));
                rOptions.Add("Ignorewhenfleeing", new CheckBox("Ignore when fleeing"));

                laneclearMenu.Add("MinWtargets", new Slider("Min W targets", 3, 1, 10));

                miscMenu.Add("eMinion", new CheckBox("E on fire-minion"));
                miscMenu.Add("aoeW", new CheckBox("Try AOE with W"));
                miscMenu.Add("eFarmAssist", new CheckBox("E farm assist"));
                miscMenu.Add("eKS", new CheckBox("E Killsteal"));
                miscMenu.Add("KSCombo", new CheckBox("Only KS in Combo", false));
                miscMenu.Add("manaH", new Slider("Mana Manager Harass", 50, 1));
                miscMenu.Add("manaLC", new Slider("Mana Manager Lane Clear", 80, 1));

                drawingMenu.Add("WPred", new CheckBox("Draw W Prediction"));
                drawingMenu.Add("QRange", new CheckBox("Q Range"));
                drawingMenu.Add("WRange", new CheckBox("W Range"));
                drawingMenu.Add("ERange", new CheckBox("E Range"));
                drawingMenu.Add("RRange", new CheckBox("R Range"));

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