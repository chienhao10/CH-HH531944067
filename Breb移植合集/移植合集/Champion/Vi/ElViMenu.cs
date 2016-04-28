using System;
using System.Drawing;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;

namespace ElVi
{
    public class ElViMenu
    {
        public static Menu
            _menu,
            cMenu,
            hMenu,
            rMenu,
            clearMenu,
            miscMenu;

        public static void Initialize()
        {
            _menu = MainMenu.AddMenu("ElVi", "menu");

            cMenu = _menu.AddSubMenu("Combo", "Combo");
            cMenu.Add("ElVi.Combo.Q", new CheckBox("Use Q"));
            cMenu.Add("ElVi.Combo.E", new CheckBox("Use E"));
            cMenu.Add("ElVi.Combo.R", new CheckBox("Use R"));
            cMenu.Add("ElVi.Combo.I", new CheckBox("Use Ignite"));
            cMenu.Add("ElVi.Combo.Flash", new KeyBind("Flash Q", false, KeyBind.BindTypes.HoldActive, 'T'));

            hMenu = _menu.AddSubMenu("Harass", "Harass");
            hMenu.Add("ElVi.Harass.Q", new CheckBox("Use Q"));
            hMenu.Add("ElVi.Harass.E", new CheckBox("Use E"));

            rMenu = _menu.AddSubMenu("R", "R");
            foreach (var enemy in ObjectManager.Get<AIHeroClient>().Where(champ => champ.IsEnemy))
            {
                rMenu.Add("ElVi.Settings.R" + enemy.BaseSkinName,
                    new CheckBox(string.Format("Ult: {0}", enemy.BaseSkinName)));
            }

            clearMenu = _menu.AddSubMenu("Clear", "Clear");
            clearMenu.Add("ElVi.LaneClear.Q", new CheckBox("Use Q LC"));
            clearMenu.Add("ElVi.LaneClear.E", new CheckBox("Use E LC"));
            clearMenu.Add("ElVi.JungleClear.Q", new CheckBox("Use Q JG"));
            clearMenu.Add("ElVi.JungleClear.E", new CheckBox("Use E JG"));
            clearMenu.Add("ElVi.Clear.Player.Mana", new Slider("Minimum Mana for clear", 55));

            miscMenu = _menu.AddSubMenu("Misc", "Misc");
            miscMenu.Add("ElVi.Draw.off", new CheckBox("Turn drawings off", false));
            miscMenu.Add("ElVi.Draw.Q", new CheckBox("Draw Q"));
            miscMenu.Add("ElVi.Draw.E", new CheckBox("Draw E"));
            miscMenu.Add("ElVi.Draw.R", new CheckBox("Draw R"));
            miscMenu.Add("ElDiana.DrawComboDamage", new CheckBox("Draw combo damage"));
            miscMenu.Add("ElVi.misc.AntiGapCloser", new CheckBox("Use Antigapcloser"));
            miscMenu.Add("ElVi.misc.Interrupter", new CheckBox("Use Interrupter"));

            DrawDamage.DamageToUnit = Vi.GetComboDamage;
            DrawDamage.Enabled = miscMenu["ElDiana.DrawComboDamage"].Cast<CheckBox>().CurrentValue;
            DrawDamage.Fill = true;
            DrawDamage.FillColor = Color.FromArgb(204, 204, 0, 0);

            miscMenu["ElDiana.DrawComboDamage"].Cast<CheckBox>().OnValueChange +=
                delegate(ValueBase<bool> sender, ValueBase<bool>.ValueChangeArgs args)
                {
                    DrawDamage.Enabled = args.NewValue;
                };

            Console.WriteLine("Menu Loaded");
        }
    }
}