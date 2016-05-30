using LeagueSharp.SDK;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK.Menu;

namespace hJhin.Extensions
{
    class Config
    {
        private static Menu Menu;

        public static Menu comboMenu, harassMenu, itemMenu, clearMenu, miscMenu, ultiMenu, jungleMenu, SemiManualUlt, hitchanceMenu;

        public static void ExecuteMenu()
        {

            Menu = MainMenu.AddMenu("hJhin", "hJhin");

            comboMenu = Menu.AddSubMenu("Combo Settings", "combo.settings");
            comboMenu.AddLabel("(Q) Settings");
            comboMenu.Add("combo.q", new CheckBox("Use (Q)"));
            comboMenu.AddLabel("(W) Settings");
            comboMenu.Add("combo.w", new CheckBox("Use (W)"));
            comboMenu.Add("combo.w.min", new Slider("Min. Distance", 400, 1, 2500));
            comboMenu.Add("combo.w.max", new Slider("Max. Distance", 1000, 1, 2500));
            comboMenu.Add("combo.w.mark", new CheckBox("Use (W) If Enemy is Marked"));
            comboMenu.AddLabel("(E) Settings");
            comboMenu.Add("combo.e", new CheckBox("Use (E)"));

            harassMenu = Menu.AddSubMenu("Harass Settings", "harass.settings");
            harassMenu.AddLabel("(Q) Settings");
            harassMenu.Add("harass.q", new CheckBox("Use (Q)"));
            harassMenu.AddLabel("(W) Settings");
            harassMenu.Add("harass.w", new CheckBox("Use (W)"));
            harassMenu.AddLabel("Mana Settings");
            harassMenu.Add("harass.mana", new Slider("Min. Mana", 50, 1, 99));

            clearMenu = Menu.AddSubMenu("WaveClear Settings", "laneclear.settings");
            clearMenu.AddLabel("(Q) Settings");
            clearMenu.Add("lane.q", new CheckBox("Use (Q)"));
            clearMenu.AddLabel("(W) Settings");
            clearMenu.Add("lane.w", new CheckBox("Use (W)"));
            clearMenu.Add("lane.w.min.count", new Slider("Min. Minion", 4, 1, 5));
            clearMenu.AddLabel("Mana Settings");
            clearMenu.Add("lane.mana", new Slider("Min. Mana", 50, 1, 99));

            jungleMenu = Menu.AddSubMenu("Jungle Settings", "jungle.settings");
            jungleMenu.AddLabel("(Q) Settings");
            jungleMenu.Add("jungle.q", new CheckBox("Use (Q)"));
            jungleMenu.AddLabel("(W) Settings");
            jungleMenu.Add("jungle.w", new CheckBox("Use (W)"));
            jungleMenu.AddLabel("Mana Settings");
            jungleMenu.Add("jungle.mana", new Slider("Min. Mana", 50, 1, 99));

            miscMenu = Menu.AddSubMenu("Miscellaneous", "misc.settings");
            miscMenu.Add("auto.e.immobile", new CheckBox("Auto Cast (E) Immobile Target"));
            miscMenu.AddLabel("Scrying Orb Settings");
            miscMenu.Add("auto.orb.buy", new CheckBox("Auto Orb. Buy"));
            miscMenu.Add("orb.level", new Slider("Orb. Level", 9, 9, 18));

            ultiMenu = Menu.AddSubMenu("Ultimate Settings", "ultimate.settings");
            ultiMenu.AddLabel("Ultimate Whitelist");

            foreach (var enemy in GameObjects.EnemyHeroes)
            {
                ultiMenu.Add("combo.r." + enemy.NetworkId, new CheckBox("(R): " + enemy.ChampionName));
            }

            ultiMenu.AddLabel("Ultimate Settings");
            ultiMenu.Add("combo.r", new CheckBox("Use (R)"));
            ultiMenu.Add("auto.shoot.bullets", new CheckBox("If Jhin Casting (R) Auto Cast Bullets"));


            itemMenu = Menu.AddSubMenu("Activator Settings", "activator.settings");
            itemMenu.Add("use.qss", new CheckBox("Use QSS"));
            itemMenu.AddLabel("Quicksilver Sash Debuffs");
            itemMenu.Add("use.charm", new CheckBox("Charm"));
            itemMenu.Add("use.snare", new CheckBox("Snare"));
            itemMenu.Add("use.polymorph", new CheckBox("Polymorph"));
            itemMenu.Add("use.stun", new CheckBox("Stun"));
            itemMenu.Add("use.suppression", new CheckBox("Suppression"));
            itemMenu.Add("use.taunt", new CheckBox("Taunt"));
            itemMenu.AddLabel("BOTRK Settings");
            itemMenu.Add("use.botrk", new CheckBox("Use BOTRK"));
            itemMenu.Add("botrk.vayne.hp", new Slider("Min. Vayne HP", 20, 1, 99));
            itemMenu.Add("botrk.enemy.hp", new Slider("Min. Enemy", 20, 1, 99));
            itemMenu.AddLabel("Youmuu Settings");
            itemMenu.Add("use.youmuu", new CheckBox("Use Youmuu"));

            SemiManualUlt = Menu.AddSubMenu("hJhin Keys", "genel.seperator1");
            SemiManualUlt.Add("semi.manual.ult", new KeyBind("Semi-Manual (R)!", false, KeyBind.BindTypes.HoldActive, 'T'));

            hitchanceMenu = Menu.AddSubMenu("hJhin HitChance", "hit.seperator1");
            hitchanceMenu.Add("hitchance", new ComboBox("HitChance ?", 0, "Medium", "High", "Very High"));

            
        }
    }
}