#region

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using LeagueSharp.Common;
using Spell = LeagueSharp.Common.Spell;

#endregion

namespace Evelynn
{
    internal class Program
    {
        public const string ChampionName = "Evelynn";

        //Spells
        public static List<Spell> SpellList = new List<Spell>();

        public static Spell Q;
        public static Spell W;
        public static Spell E;
        public static Spell R;

        //Menu
        public static Menu Config, comboMenu, laneClearMenu, jungleClearMenu, drawMenu;

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

        public static void Game_OnGameLoad()
        {
            if (ObjectManager.Player.BaseSkinName != ChampionName) return;

            //Create the spells
            Q = new Spell(SpellSlot.Q, 500f);
            W = new Spell(SpellSlot.W, Q.Range);
            E = new Spell(SpellSlot.E, 225f + 2*65f);
            R = new Spell(SpellSlot.R, 650f);

            R.SetSkillshot(0.25f, 350f, float.MaxValue, false, SkillshotType.SkillshotCircle);

            SpellList.Add(Q);
            SpellList.Add(W);
            SpellList.Add(E);
            SpellList.Add(R);

            //Create the menu
            Config = MainMenu.AddMenu(ChampionName, ChampionName);

            comboMenu = Config.AddSubMenu("Combo", "Combo");
            comboMenu.Add("UseQCombo", new CheckBox("Use Q"));
            comboMenu.Add("UseWCombo", new CheckBox("Use W"));
            comboMenu.Add("UseECombo", new CheckBox("Use E"));
            comboMenu.Add("UseRCombo", new CheckBox("Use R"));

            laneClearMenu = Config.AddSubMenu("LaneClear", "LaneClear");
            laneClearMenu.Add("UseQLaneClear", new CheckBox("Use Q"));
            laneClearMenu.Add("UseELaneClear", new CheckBox("Use E"));

            jungleClearMenu = Config.AddSubMenu("JungleFarm", "JungleFarm");
            jungleClearMenu.Add("UseQJFarm", new CheckBox("Use Q"));
            jungleClearMenu.Add("UseEJFarm", new CheckBox("Use E"));

            drawMenu = Config.AddSubMenu("Drawings", "Drawings");
            drawMenu.Add("QRange", new CheckBox("Use Q"));
            drawMenu.Add("ERange", new CheckBox("Use E"));
            drawMenu.Add("RRange", new CheckBox("Use R"));

            Drawing.OnDraw += Drawing_OnDraw;
            Game.OnUpdate += Game_OnGameUpdate;
            Spellbook.OnCastSpell += Spellbook_OnCastSpell;
        }

        private static void Drawing_OnDraw(EventArgs args)
        {
            foreach (var spell in SpellList)
            {
                if (spell.Slot == SpellSlot.W)
                {
                    return;
                }
                var menuItem = getCheckBoxItem(drawMenu, spell.Slot + "Range");
                if (menuItem)
                    Render.Circle.DrawCircle(ObjectManager.Player.Position, spell.Range, Color.Black);
            }
        }

        private static void Game_OnGameUpdate(EventArgs args)
        {
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                Combo();
                return;
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear) ||
                Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear))
            {
                LaneClear();
                JungleFarm();
            }
        }

        private static void Spellbook_OnCastSpell(Spellbook sender, SpellbookCastSpellEventArgs args)
        {
            if (sender.Owner.IsMe && args.Slot == SpellSlot.R)
            {
                if (ObjectManager.Get<AIHeroClient>()
                    .Count(
                        hero =>
                            hero.IsValidTarget() &&
                            hero.Distance(args.StartPosition.To2D()) <= R.Range) == 0)
                    args.Process = false;
            }
        }

        private static void Combo()
        {
            var target = TargetSelector.GetTarget(Q.Range, DamageType.Magical);

            if (target != null)
            {
                if (getCheckBoxItem(comboMenu, "UseQCombo"))
                    Q.Cast();

                if (getCheckBoxItem(comboMenu, "UseWCombo") && W.IsReady() &&
                    ObjectManager.Player.HasBuffOfType(BuffType.Slow))
                    W.Cast();

                if (getCheckBoxItem(comboMenu, "UseECombo") && E.IsReady())
                    E.CastOnUnit(target);

                if (getCheckBoxItem(comboMenu, "UseRCombo") && R.IsReady() && GetComboDamage(target) > target.Health)
                    R.Cast(target, false, true);
            }
        }

        private static void JungleFarm()
        {
            var mobs = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, Q.Range,
                MinionTypes.All,
                MinionTeam.Neutral, MinionOrderTypes.MaxHealth);

            if (mobs.Count > 0)
            {
                if (getCheckBoxItem(jungleClearMenu, "UseQJFarm") && Q.IsReady())
                    Q.Cast();

                if (getCheckBoxItem(jungleClearMenu, "UseEJFarm") && E.IsReady())
                    E.CastOnUnit(mobs[0]);
            }
        }

        private static void LaneClear()
        {
            var minions = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, Q.Range);

            foreach (var minion in minions.FindAll(minion => minion.IsValidTarget(Q.Range)))
            {
                if (getCheckBoxItem(laneClearMenu, "UseQLaneClear") && Q.IsReady())
                    Q.Cast();

                if (getCheckBoxItem(laneClearMenu, "UseELaneClear") && E.IsReady())
                    E.CastOnUnit(minion);
            }
        }

        private static float GetComboDamage(Obj_AI_Base target)
        {
            float comboDamage = 0;

            if (ObjectManager.Player.Spellbook.GetSpell(SpellSlot.Q).Level > 0)
                comboDamage += Q.GetDamage(target)*3;
            if (E.IsReady())
                comboDamage += E.GetDamage(target);
            if (R.IsReady())
                comboDamage += R.GetDamage(target);

            return comboDamage;
        }
    }
}