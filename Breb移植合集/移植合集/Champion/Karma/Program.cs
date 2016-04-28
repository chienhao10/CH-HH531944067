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

namespace Karma
{
    internal class Program
    {
        private const string ChampionName = "Karma";

        private static readonly List<Spell> SpellList = new List<Spell>();

        private static Spell _q;
        private static Spell _w;
        private static Spell _e;
        private static Spell _r;

        private static Menu _config, comboMenu, harassMenu, miscMenu, drawMenu;

        private static bool MantraIsActive
        {
            get { return ObjectManager.Player.HasBuff("KarmaMantra"); }
        }

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

        public static void Game_OnGameLoad()
        {
            if (ObjectManager.Player.ChampionName != ChampionName)
            {
                return;
            }

            _q = new Spell(SpellSlot.Q, 950f);
            _w = new Spell(SpellSlot.W, 700f);
            _e = new Spell(SpellSlot.E, 800f);
            _r = new Spell(SpellSlot.R);

            _q.SetSkillshot(0.25f, 60f, 1700f, true, SkillshotType.SkillshotLine);
            _w.SetTargetted(0.25f, 2200f);
            _e.SetTargetted(0.25f, float.MaxValue);

            SpellList.Add(_q);
            SpellList.Add(_w);
            SpellList.Add(_e);
            SpellList.Add(_r);

            _config = MainMenu.AddMenu(ChampionName, ChampionName);

            comboMenu = _config.AddSubMenu("Combo", "Combo");
            comboMenu.Add("UseQCombo", new CheckBox("Use Q"));
            comboMenu.Add("UseWCombo", new CheckBox("Use W"));
            comboMenu.Add("UseRCombo", new CheckBox("Use R"));

            harassMenu = _config.AddSubMenu("Harass", "Harass");
            harassMenu.Add("UseQHarass", new CheckBox("Use Q"));
            harassMenu.Add("UseWHarass", new CheckBox("Use W", false));
            harassMenu.Add("UseRHarass", new CheckBox("Use R"));

            miscMenu = _config.AddSubMenu("Misc", "Misc");
            miscMenu.Add("UseEDefense", new CheckBox("Use E For Defense"));

            drawMenu = _config.AddSubMenu("Drawings", "Drawings");
            drawMenu.Add("QRange", new CheckBox("Q Range"));
            drawMenu.Add("WRange", new CheckBox("W Range"));
            drawMenu.Add("WRootRange", new CheckBox("W Root Range"));
            drawMenu.Add("ERange", new CheckBox("E Range"));

            AntiGapcloser.OnEnemyGapcloser += AntiGapcloser_OnEnemyGapcloser;
            Drawing.OnDraw += Drawing_OnDraw;
            Game.OnUpdate += Game_OnGameUpdate;
            Interrupter2.OnInterruptableTarget += Interrupter2_OnInterruptableTarget;
        }

        private static void Interrupter2_OnInterruptableTarget(AIHeroClient sender,
            Interrupter2.InterruptableTargetEventArgs args)
        {
            if (sender.IsValidTarget(1000f) && args.DangerLevel == Interrupter2.DangerLevel.High && _e.IsReady())
            {
                _r.Cast();

                if (!_r.IsReady())
                {
                    _e.Cast(ObjectManager.Player);
                }
            }
        }

        private static void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (gapcloser.Sender.IsValidTarget(300f))
            {
                _e.Cast(ObjectManager.Player);
                _q.Cast(gapcloser.Sender);
            }
        }

        private static void Drawing_OnDraw(EventArgs args)
        {
            var menuItem = getCheckBoxItem(drawMenu, "WRootRange");
            if (menuItem)
            {
                foreach (
                    var enemy in
                        ObjectManager.Get<AIHeroClient>().Where(h => h.IsValidTarget() && h.HasBuff("KarmaSpiritBind")))
                {
                    var distance = 1 - Math.Min(Math.Max(850 - ObjectManager.Player.Distance(enemy), 0), 450)/450;
                    Render.Circle.DrawCircle(ObjectManager.Player.Position, 850,
                        Color.FromArgb((int) (50*distance), Color.MintCream), -420, true);
                    Render.Circle.DrawCircle(ObjectManager.Player.Position, 850,
                        Color.FromArgb((int) (255*distance), Color.MintCream), 10);
                    break;
                }
            }

            foreach (var spell in SpellList)
            {
                menuItem = getCheckBoxItem(drawMenu, spell.Slot + "Range");
                if (menuItem)
                {
                    Render.Circle.DrawCircle(ObjectManager.Player.Position, spell.Range,
                        Color.FromArgb(255, 255, 255, 255));
                }
            }
        }

        private static void Game_OnGameUpdate(EventArgs args)
        {
            _q.Width = MantraIsActive ? 80f : 60f; // Mantra increases the q line width
            _q.Range = MantraIsActive ? 1250f : 1050f;
            if (getCheckBoxItem(miscMenu, "UseEDefense"))
            {
                foreach (
                    var hero in
                        ObjectManager.Get<AIHeroClient>()
                            .Where(
                                hero =>
                                    hero.IsValidTarget(_e.Range) && hero.IsAlly &&
                                    ObjectManager.Get<AIHeroClient>()
                                        .Count(h => h.IsValidTarget() && h.Distance(hero) < 400) > 1))
                {
                    _e.Cast(hero);
                }
            }

            if (!Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo) &&
                !Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass))
            {
                return;
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass) &&
                ObjectManager.Player.Mana -
                ObjectManager.Player.Spellbook.Spells.First(s => s.Slot == SpellSlot.W).SData.Mana -
                ObjectManager.Player.Spellbook.Spells.First(s => s.Slot == SpellSlot.E).SData.Mana < 0)
            {
                return;
            }


            var qTarget = TargetSelector.GetTarget(_q.Range, DamageType.Magical);
            var wTarget = TargetSelector.GetTarget(_w.Range, DamageType.Magical);

            var qActive =
                getCheckBoxItem(
                    Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo) ? comboMenu : harassMenu,
                    "UseQ" + (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo) ? "Combo" : "Harass"));
            var wActive =
                getCheckBoxItem(
                    Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo) ? comboMenu : harassMenu,
                    "UseW" + (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo) ? "Combo" : "Harass"));
            var rActive =
                getCheckBoxItem(
                    Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo) ? comboMenu : harassMenu,
                    "UseR" + (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo) ? "Combo" : "Harass"));

            if (wActive && wTarget != null && _w.IsReady())
            {
                if (ObjectManager.Player.Health/ObjectManager.Player.MaxHealth/
                    (qTarget.Health/qTarget.MaxHealth) < 1)
                {
                    if (rActive)
                    {
                        _r.Cast();
                    }

                    if (!rActive || !_r.IsReady())
                    {
                        _w.Cast(wTarget);
                    }
                }
            }

            if (qActive && qTarget != null && _q.IsReady())
            {
                if (rActive)
                {
                    _r.Cast();
                }

                if (!rActive || !_r.IsReady())
                {
                    var qPrediction = _q.GetPrediction(qTarget);
                    if (qPrediction.Hitchance >= HitChance.High)
                    {
                        _q.Cast(qPrediction.CastPosition);
                    }
                    else if (qPrediction.Hitchance == HitChance.Collision)
                    {
                        var minionsHit = qPrediction.CollisionObjects;
                        var closest =
                            minionsHit.Where(m => m.NetworkId != ObjectManager.Player.NetworkId)
                                .OrderBy(m => m.Distance(ObjectManager.Player))
                                .FirstOrDefault();

                        if (closest != null && closest.Distance(qPrediction.UnitPosition) < 200)
                        {
                            _q.Cast(qPrediction.CastPosition);
                        }
                    }
                }
            }

            if (wActive && wTarget != null)
            {
                _w.Cast(wTarget);
            }
        }
    }
}