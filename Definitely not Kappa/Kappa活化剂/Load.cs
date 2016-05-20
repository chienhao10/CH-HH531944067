namespace KappaUtility
{
    using System;

    using EloBuddy;
    using EloBuddy.SDK;
    using EloBuddy.SDK.Events;
    using EloBuddy.SDK.Menu;

    using Common;

    using EloBuddy.SDK.Menu.Values;

    using Items;

    using Misc;

    using Summoners;

    using Trackers;

    internal class Load
    {
        protected static bool loadedreveal = false;

        protected static bool loadedtrack = false;

        public static Menu UtliMenu;

        public static void Execute()
        {
            UtliMenu = MainMenu.AddMenu("CH汉化活化剂", "KappaUtility");
            UtliMenu.AddLabel("CH汉化 Kappa活化剂，有同样功能的脚本最好不要一起开");
            UtliMenu.AddGroupLabel("全局设置 [需要F5才生效]");
            UtliMenu.Add("AutoLvlUp", new CheckBox("开启自动加点"));
            UtliMenu.Add("AutoQSS", new CheckBox("自动解控"));
            UtliMenu.Add("AutoTear", new CheckBox("开启女神叠加"));
            UtliMenu.Add("AutoReveal", new CheckBox("开启自动插眼"));
            UtliMenu.Add("GanksDetector", new CheckBox("开启Gank提示"));
            UtliMenu.Add("Tracker", new CheckBox("开启记录器"));
            UtliMenu.Add("SkinHax", new CheckBox("开启换肤"));
            UtliMenu.Add("Spells", new CheckBox("开启召唤师技能"));
            UtliMenu.Add("Potions", new CheckBox("开启吃药"));
            UtliMenu.Add("Offensive", new CheckBox("开启进攻物品"));
            UtliMenu.Add("Defensive", new CheckBox("开启防守物品"));
            if (UtliMenu["AutoLvlUp"].Cast<CheckBox>().CurrentValue)
            {
                AutoLvlUp.OnLoad();
            }
            if (UtliMenu["AutoQSS"].Cast<CheckBox>().CurrentValue)
            {
                AutoQSS.OnLoad();
            }
            if (UtliMenu["AutoTear"].Cast<CheckBox>().CurrentValue)
            {
                AutoTear.OnLoad();
            }
            if (UtliMenu["AutoReveal"].Cast<CheckBox>().CurrentValue)
            {
                AutoReveal.OnLoad();
                loadedreveal = true;
            }
            if (UtliMenu["GanksDetector"].Cast<CheckBox>().CurrentValue)
            {
                GanksDetector.OnLoad();
            }
            if (UtliMenu["Tracker"].Cast<CheckBox>().CurrentValue)
            {
                Tracker.OnLoad();
                Surrender.OnLoad();
                loadedtrack = true;
            }
            if (UtliMenu["SkinHax"].Cast<CheckBox>().CurrentValue)
            {
                SkinHax.OnLoad();
            }
            if (UtliMenu["Spells"].Cast<CheckBox>().CurrentValue)
            {
                Spells.OnLoad();
                Flash.FOnLoad();
            }
            if (UtliMenu["Potions"].Cast<CheckBox>().CurrentValue)
            {
                Potions.OnLoad();
            }
            if (UtliMenu["Offensive"].Cast<CheckBox>().CurrentValue)
            {
                Offensive.OnLoad();
            }
            if (UtliMenu["Defensive"].Cast<CheckBox>().CurrentValue)
            {
                Defensive.OnLoad();
            }

            Game.OnTick += GameOnTick;
            Drawing.OnEndScene += OnEndScene;
            Drawing.OnDraw += DrawingOnDraw;
        }

        private static void DrawingOnDraw(EventArgs args)
        {
            try
            {
                Spells.Drawings();
                GanksDetector.OnDraw();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private static void OnEndScene(EventArgs args)
        {
            try
            {
                if (loadedreveal)
                {
                    AutoReveal.Drawings();
                }
                if (loadedtrack)
                {
                    Traps.Draw();
                    Tracker.HPtrack();
                    Tracker.track();
                }
                GanksDetector.OnEndScene();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private static void GameOnTick(EventArgs args)
        {
            try
            {
                var flags = Orbwalker.ActiveModesFlags;
                if (flags.HasFlag(Orbwalker.ActiveModes.Combo))
                {
                    Offensive.Items();
                    Defensive.Items();
                }

                if (loadedreveal)
                {
                    AutoReveal.Reveal();
                }

                AutoLvlUp.Levelup();
                AutoTear.OnUpdate();
                GanksDetector.OnUpdate();
                Smite.Smiteopepi();
                Spells.Cast();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}