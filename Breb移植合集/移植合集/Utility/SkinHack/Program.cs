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

            menu = MainMenu.AddMenu("Skins#", "Skinswitcher");

            menu.Add("forall", new CheckBox("Enable for all (reload required)", false));
            menu.Add("onlydefault", new CheckBox("Keep loaded skins"));

            try
            {
                foreach (var hero in HeroManager.AllHeroes)
                {
                    if (!getCheckBoxItem("forall") && hero.Name != ObjectManager.Player.Name)
                    {
                        continue;
                    }

                    HeroList.Add(hero);

                    WasDead.Add(hero, false);

                    Enabled.Add(hero.NetworkId, false);

                    var currenthero = hero;

                    menu.Add("skin." + hero.ChampionName, new ComboBox("Change Skin", 0, "Skin 0", "Skin 1", "Skin 2", "Skin 3", "Skin 4", "Skin 5", "Skin 6", "Skin 7", "Skin 8", "Skin 9", "Skin 10", "Skin 11", "Skin 12", "Skin 13", "Skin 14", "Skin 15"));

                    ChampSkins.Add(hero.Name, getBoxItem("skin." + hero.ChampionName));

                    if (OnlyDefault && hero.CharData.BaseSkinName == hero.BaseSkinName)
                    {
                        Enabled[hero.NetworkId] = true;
                        hero.SetSkin(hero.ChampionName, ChampSkins[hero.Name]);
                    }
                    else if (!OnlyDefault)
                    {
                        Enabled[hero.NetworkId] = true;
                        hero.SetSkin(hero.ChampionName, ChampSkins[hero.Name]);
                    }

                    menu["skin." + hero.ChampionName].Cast<ComboBox>().OnValueChange += delegate(ValueBase<int> sender, ValueBase<int>.ValueChangeArgs args)
                    {
                        Enabled[hero.NetworkId] = true;
                        var skinid = args.NewValue;
                        if (hero.ChampionName == "Ezreal" && skinid == 5)
                        {
                            skinid += 1;
                        }
                        ChampSkins[currenthero.Name] = skinid;
                        currenthero.SetSkin(currenthero.ChampionName, ChampSkins[currenthero.Name]);
                    };
                }
            }
            catch (Exception e)
            {
                Console.Write(e + " " + e.StackTrace);
            }

            Game.OnUpdate += RenewSkins;
        }


        static void RenewSkins(EventArgs args)
        {
            foreach (var hero in HeroList)
            {
                if (!getCheckBoxItem("forall") && !hero.IsMe)
                {
                    continue;
                }
                if (hero.IsDead && !WasDead[hero])
                {
                    WasDead[hero] = true;
                    continue;
                }
                if (!hero.IsDead && WasDead[hero] && Enabled[hero.NetworkId])
                {
                    hero.SetSkin(hero.ChampionName, ChampSkins[hero.Name]);
                    WasDead[hero] = false;
                }
            }
        }


        static void FloatPropertyChange(GameObject sender, GameObjectFloatPropertyChangeEventArgs args)
        {
            try
            {
                if (!(sender is AIHeroClient) || args.Property != "mHP" || sender.Name != ObjectManager.Player.Name && !getCheckBoxItem("forall"))
                {
                    return;
                }

                var hero = (AIHeroClient)sender;

                if (args.Value.Equals(args.Value) && args.Value.Equals(hero.MaxHealth) && !hero.IsDead)
                {
                    hero.SetSkin(hero.ChampionName, ChampSkins[hero.Name]);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}
