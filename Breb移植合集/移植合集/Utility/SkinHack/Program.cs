using System;
using System.Collections.Generic;
using EloBuddy;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using LeagueSharp.Common;

namespace SkinsSharp
{
    class Program
    {
        private static Menu menu;
        private static Dictionary<String, int> ChampSkins = new Dictionary<String, int>();
        private static Dictionary<int, bool> Enabled = new Dictionary<int, bool>();
        private static Dictionary<AIHeroClient, bool> WasDead = new Dictionary<AIHeroClient, bool>();

        private static List<AIHeroClient> HeroList = new List<AIHeroClient>();

        public static bool getCheckBoxItem(string item)
        {
            return menu[item].Cast<CheckBox>().CurrentValue;
        }

        public static int getSliderItem(string item)
        {
            return menu[item].Cast<Slider>().CurrentValue;
        }

        public static bool getKeyBindItem(string item)
        {
            return menu[item].Cast<KeyBind>().CurrentValue;
        }

        public static int getBoxItem(string item)
        {
            return menu[item].Cast<ComboBox>().CurrentValue;
        }

        private static bool OnlyDefault
        {
            get { return getCheckBoxItem("onlydefault"); }
        }

        public static void GameLoad()
        {

            menu = MainMenu.AddMenu("换肤#", "Skinswitcher");

            menu.Add("onlydefault", new CheckBox("持续保持换肤"));

            try
            {
                HeroList.Add(ObjectManager.Player);

                WasDead.Add(ObjectManager.Player, false);

                Enabled.Add(ObjectManager.Player.NetworkId, false);

                var currenthero = ObjectManager.Player;

                menu.Add("skin." + ObjectManager.Player.ChampionName, new ComboBox("换肤", 0, "Skin 0", "Skin 1", "Skin 2", "Skin 3", "Skin 4", "Skin 5", "Skin 6", "Skin 7", "Skin 8", "Skin 9", "Skin 10", "Skin 11", "Skin 12", "Skin 13", "Skin 14", "Skin 15"));

                ChampSkins.Add(ObjectManager.Player.Name, getBoxItem("skin." + ObjectManager.Player.ChampionName));

                if (OnlyDefault && ObjectManager.Player.CharData.BaseSkinName == ObjectManager.Player.BaseSkinName)
                {
                    Enabled[ObjectManager.Player.NetworkId] = true;
                    ObjectManager.Player.SetSkin(ObjectManager.Player.ChampionName, ChampSkins[ObjectManager.Player.Name]);
                }
                else if (!OnlyDefault)
                {
                    Enabled[ObjectManager.Player.NetworkId] = true;
                    ObjectManager.Player.SetSkin(ObjectManager.Player.ChampionName, ChampSkins[ObjectManager.Player.Name]);
                }

                menu["skin." + ObjectManager.Player.ChampionName].Cast<ComboBox>().OnValueChange += delegate (ValueBase<int> sender, ValueBase<int>.ValueChangeArgs args)
                {
                    Enabled[ObjectManager.Player.NetworkId] = true;
                    var skinid = args.NewValue;
                    if (ObjectManager.Player.ChampionName == "Ezreal" && skinid == 5)
                    {
                        skinid += 1;
                    }
                    ChampSkins[currenthero.Name] = skinid;
                    currenthero.SetSkin(currenthero.ChampionName, ChampSkins[currenthero.Name]);
                };
            }
            catch (Exception e)
            {
                Console.Write(e + " " + e.StackTrace);
            }

            Game.OnUpdate += RenewSkins;
        }


        static void RenewSkins(EventArgs args)
        {
            if (ObjectManager.Player.IsDead && !WasDead[ObjectManager.Player])
            {
                WasDead[ObjectManager.Player] = true;
                return;
            }

            if (!ObjectManager.Player.IsDead && WasDead[ObjectManager.Player] && Enabled[ObjectManager.Player.NetworkId])
            {
                ObjectManager.Player.SetSkin(ObjectManager.Player.ChampionName, ChampSkins[ObjectManager.Player.Name]);
                WasDead[ObjectManager.Player] = false;
            }
        }
    }
}