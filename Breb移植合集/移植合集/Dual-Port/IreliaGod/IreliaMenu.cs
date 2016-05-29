using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using LeagueSharp.Common;


namespace IreliaGod
{
    class IreliaMenu
    {
        public static Menu Config = MainMenu.AddMenu("Irelia God", "IreliaGod");

        public static Menu comboMenu, targetSelectorMenu, harassMenu, laneclearMenu, drawingsMenu, miscMenu, fleeMenu;

        public static void Initialize()
        {

            targetSelectorMenu = Config.AddSubMenu("Target Selector", "Target Selector");
            {
                targetSelectorMenu.Add("force.target", new CheckBox("Force focus selected target", false));
                targetSelectorMenu.Add("force.target.range", new Slider("if within:", 1500, 0, 2500));
            }

            comboMenu = Config.AddSubMenu("Combo", "Combo settings");
            {
                comboMenu.Add("combo.q", new CheckBox("Use Q on enemy"));
                comboMenu.Add("combo.q.minrange", new Slider("Minimum range to Q enemy", 450, 0, 650));
                comboMenu.Add("combo.q.undertower", new Slider("Q enemy under tower only if their health % under", 40));
                comboMenu.Add("combo.q.lastsecond", new CheckBox("Q to target always before W buff ends (range doesnt matter)"));
                comboMenu.Add("combo.q.gc", new CheckBox("Q to gapclose (killable minions)"));
                comboMenu.AddSeparator(15);
                comboMenu.Add("combo.w", new CheckBox("Use W"));
                comboMenu.AddSeparator(15);
                comboMenu.Add("combo.e", new CheckBox("Use E"));
                comboMenu.Add("combo.e.logic", new CheckBox("E : advanced logic"));
                comboMenu.AddSeparator(15);
                comboMenu.Add("combo.r", new CheckBox("Use R"));
                comboMenu.Add("combo.r.weave", new CheckBox("R : sheen synergy"));
                comboMenu.Add("combo.r.selfactivated", new CheckBox("R : only if self activated", false));
                comboMenu.AddSeparator(15);
                comboMenu.Add("combo.items", new CheckBox("Use items"));
                comboMenu.Add("combo.ignite", new CheckBox("Use ignite if combo killable"));
            }

            harassMenu = Config.AddSubMenu("Harass", "Harass settings");
            {
                harassMenu.Add("harass.q", new CheckBox("Use Q on enemy"));
                harassMenu.Add("harass.q.minrange", new Slider("Q : Minimum range to Q enemy", 450, 0, 650));
                harassMenu.Add("harass.q.undertower", new Slider("Q enemy under tower only if their health % under", 40));
                harassMenu.Add("harass.q.lastsecond", new CheckBox("Use Q to target always before W buff ends (range doesnt matter)"));
                harassMenu.AddSeparator(15);
                harassMenu.Add("harass.q.gc", new CheckBox("Use Q to gapclose (killable minions)"));
                harassMenu.Add("harass.w", new CheckBox("Use W"));
                harassMenu.AddSeparator(15);
                harassMenu.Add("harass.e", new CheckBox("Use E"));
                harassMenu.Add("harass.e.logic", new CheckBox("E : advanced logic"));
                harassMenu.AddSeparator(15);
                harassMenu.Add("harass.r", new CheckBox("Use R"));
                harassMenu.Add("harass.r.weave", new CheckBox("R : sheen synergy"));
                harassMenu.AddSeparator(15);
                harassMenu.Add("harass.mana", new Slider("Mana manager (%)", 40, 1));
            }

            laneclearMenu = Config.AddSubMenu("Laneclear", "Laneclear settings");
            {
                laneclearMenu.Add("laneclear.r", new CheckBox("Use R"));
                laneclearMenu.Add("laneclear.r.minimum", new Slider("   minimum minions", 2, 1, 6));
                laneclearMenu.Add("laneclear.mana", new Slider("Mana manager (%)", 40, 1));
                laneclearMenu.Add("useqfarm", new ComboBox("Q Farm Mode: ", 0, "ONLY-UNKILLABLE", "ALWAYS", "NEVER"));
            }

            drawingsMenu = Config.AddSubMenu("Drawings", "Drawings settings");
            {
                drawingsMenu.Add("drawings.q", new CheckBox("Draw Q"));
                drawingsMenu.Add("drawings.e", new CheckBox("Draw E"));
                drawingsMenu.Add("drawings.r", new CheckBox("Draw R"));
            }

            miscMenu = Config.AddSubMenu("Misc", "Misc. settings"); // R to heal
            {
                miscMenu.Add("misc.ks.q", new CheckBox("Killsteal Q"));
                miscMenu.Add("misc.ks.e", new CheckBox("Killsteal E"));
                miscMenu.Add("misc.ks.r", new CheckBox("Killsteal R"));
                miscMenu.Add("misc.age", new CheckBox("Anti-Gapclose E"));
                miscMenu.Add("misc.interrupt", new CheckBox("Stun interruptable spells"));
                miscMenu.Add("misc.stunundertower", new CheckBox("Stun enemy with tower aggro"));
            }

            fleeMenu = Config.AddSubMenu("Flee", "Flee settings");
            {
                fleeMenu.Add("flee.q", new CheckBox("Use Q"));
                fleeMenu.Add("flee.e", new CheckBox("Use E"));
                fleeMenu.Add("flee.r", new CheckBox("Use R"));
            }
        }
    }
}
