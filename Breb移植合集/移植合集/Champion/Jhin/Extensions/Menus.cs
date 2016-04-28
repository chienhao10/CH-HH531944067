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
            Config = MainMenu.AddMenu(":: Jhin - The Virtuoso", ":: Jhin - The Virtuoso");

            qMenu = Config.AddSubMenu(":: Q", ":: Q");
            qMenu.Add("q.combo", new CheckBox("Use (Q)"));

            wMenu = Config.AddSubMenu(":: W", ":: W");
            wMenu.Add("w.combo", new CheckBox("Use (W)"));
            wMenu.Add("w.combo.min.distance", new Slider("Min. Distance", 400, 1, 2500));
            wMenu.Add("w.combo.max.distance", new Slider("Max. Distance", 1000, 1, 2500));
            wMenu.Add("w.passive.combo", new CheckBox("Use (W) If Enemy Is Marked", false));
            wMenu.Add("w.hit.chance",
                new ComboBox("(W) Hit Chance", 2, "Low", "Medium", "High", "Very High", "Only Immobile"));
                //.SetValue(new StringList(Provider.HitchanceNameArray, 2)));

            eMenu = Config.AddSubMenu(":: E", ":: E");
            eMenu.Add("e.combo", new CheckBox("Use (E)"));
            eMenu.Add("e.combo.teleport", new CheckBox("Auto (E) Teleport"));
            eMenu.Add("e.hit.chance",
                new ComboBox("(E) Hit Chance", 2, "Low", "Medium", "High", "Very High", "Only Immobile"));
                //.SetValue(new StringList(Provider.HitchanceNameArray, 2)));

            harassMenu = Config.AddSubMenu(":: Harass Settings", ":: Harass Settings");
            harassMenu.AddGroupLabel(":: W");
            harassMenu.Add("w.harass", new CheckBox("Use (W)"));
            harassMenu.Add("harass.mana", new Slider("Min. Mana Percentage", 50, 1, 99));

            clearMenu = Config.AddSubMenu(":: Clear Settings", ":: Clear Settings");
            clearMenu.AddGroupLabel(":: Wave Clear");
            clearMenu.Add("q.clear", new CheckBox("Use (Q)"));
            clearMenu.Add("w.clear", new CheckBox("Use (W)"));
            clearMenu.Add("w.hit.x.minion", new Slider("Min. Minion", 4, 1, 5));
            clearMenu.AddSeparator();
            clearMenu.AddGroupLabel(":: Jungle Clear");
            clearMenu.Add("q.jungle", new CheckBox("Use (Q)"));
            clearMenu.Add("w.jungle", new CheckBox("Use (W)"));
            clearMenu.AddSeparator();
            clearMenu.Add("clear.mana", new Slider("LaneClear Min. Mana Percentage", 50, 1, 99));
            clearMenu.Add("jungle.mana", new Slider("Jungle Min. Mana Percentage", 50, 1, 99));

            ksMenu = Config.AddSubMenu(":: Kill Steal", ":: Kill Steal");
            ksMenu.Add("q.ks", new CheckBox("Use (Q)"));
            ksMenu.Add("w.ks", new CheckBox("Use (W)"));

            miscMenu = Config.AddSubMenu(":: Miscellaneous", ":: Miscellaneous");
            miscMenu.Add("auto.e.immobile", new CheckBox("Auto Cast (E) Immobile Target"));
            miscMenu.AddSeparator();
            miscMenu.AddGroupLabel(":: Ultimate Settings");
            miscMenu.AddLabel(":: R - Whitelist");
            foreach (var enemy in HeroManager.Enemies.Where(enemy => enemy.IsValid))
            {
                miscMenu.Add("r.combo." + enemy.ChampionName, new CheckBox("(R): " + enemy.ChampionName));
            }
            miscMenu.Add("r.combo", new CheckBox("Use (R)"));
            miscMenu.Add("auto.shoot.bullets", new CheckBox("If Jhin Casting (R) Auto Cast Bullets"));
            miscMenu.Add("r.hit.chance",
                new ComboBox("(R) Hit Chance", 1, "Low", "Medium", "High", "Very High", "Only Immobile"));
                //.SetValue(new StringList(Provider.HitchanceNameArray, 1)));
            miscMenu.Add("semi.manual.ult", new KeyBind("Semi-Manual (R)!", false, KeyBind.BindTypes.HoldActive, 'A'));

            drawMenu = Config.AddSubMenu(":: Drawings", ":: Drawings");
            drawMenu.AddGroupLabel(":: Damage Draw");
            drawMenu.Add("aa.indicator", new CheckBox("(AA) Indicator"));
            drawMenu.Add("sniper.text", new CheckBox("Sniper Text"));
            drawMenu.AddSeparator();
            drawMenu.Add("q.draw", new CheckBox("(Q) Range"));
            drawMenu.Add("w.draw", new CheckBox("(W) Range"));
            drawMenu.Add("e.draw", new CheckBox("(E) Range"));
            drawMenu.Add("r.draw", new CheckBox("(R) Range"));
        }
    }
}