using System;
using System.Drawing;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using LeagueSharp.Common;
using vSupport_Series.Core.Plugins;
using Spell = LeagueSharp.Common.Spell;

namespace vSupport_Series.Champions
{
    public class Sona : Helper
    {
        public static Menu Config;
        public static Spell Q, W, E, R;

        public static AIHeroClient Player = ObjectManager.Player;

        public static Menu comboMenu, healManager, heal, rSettings, harass, misc, drawing;

        public Sona()
        {
            SonaOnLoad();
        }

        public static void SonaOnLoad()
        {
            Q = new Spell(SpellSlot.Q, 850f);
            W = new Spell(SpellSlot.W, 1000f);
            E = new Spell(SpellSlot.E, 350f);
            R = new Spell(SpellSlot.R, 1000f);

            R.SetSkillshot(0.5f, 125, 3000f, false, SkillshotType.SkillshotLine);

            Config = MainMenu.AddMenu("vSupport Series: " + ObjectManager.Player.ChampionName, "vSupport Series");

            comboMenu = Config.AddSubMenu("Combo Settings", "Combo Settings");
            comboMenu.Add("sona.q.combo", new CheckBox("Use Q"));
            comboMenu.Add("sona.r.combo", new CheckBox("Use R"));

            healManager = Config.AddSubMenu("(W) Heal Settings", "(W) Heal Settings");
            healManager.Add("sona.heal.disable", new CheckBox("Disable healing?", false));
            healManager.Add("sona.heal.limit", new Slider("Min. sona HP Percent for Heal", 40, 1));

            heal = Config.AddSubMenu("(W) Prio", "PRIOASD");
            foreach (var ally in ObjectManager.Get<AIHeroClient>().Where(o => o.IsAlly && o.IsValid && !o.IsMe))
            {
                if (LowPriority.Contains(ally.ChampionName))
                {
                    heal.Add("sona.heal" + ally.NetworkId, new CheckBox("Heal: " + ally.ChampionName));
                    heal.Add("sona.heal.percent" + ally.NetworkId, new Slider("Min. " + ally.ChampionName + " HP Percent", 15, 1, 99));
                }
                if (MediumPriority.Contains(ally.ChampionName))
                {
                    heal.Add("sona.heal" + ally.NetworkId, new CheckBox("Heal: " + ally.ChampionName));
                    heal.Add("sona.heal.percent" + ally.NetworkId, new Slider("Min. new CheckBox(" + ally.ChampionName + " HP Percent", 20, 1, 99));
                }
                if (HighChamps.Contains(ally.ChampionName))
                {
                    heal.Add("sona.heal" + ally.NetworkId, new CheckBox("Heal: " + ally.ChampionName));
                    heal.Add("sona.heal.percent" + ally.NetworkId, new Slider("Min. " + ally.ChampionName + " HP Percent", 30, 1, 99));
                }
            }

            rSettings = Config.AddSubMenu("(R) Ult Settings", "(R) Ult Settings");
            rSettings.Add("sona.r.killsteal", new CheckBox("Killsteal using R", false));

            harass = Config.AddSubMenu("Harass Settings", "Harass Settings");
            harass.Add("sona.q.harass", new CheckBox("Use Q"));
            harass.Add("sona.harass.mana", new Slider("Min. Mana Percent", 50, 1));

            misc = Config.AddSubMenu("Miscellaneous", "Miscellaneous");
            misc.Add("sona.anti", new CheckBox("Gapcloser (Q)"));
            misc.Add("sona.inter", new CheckBox("Interrupt (R)"));

            drawing = Config.AddSubMenu("Draw Settings", "Draw Settings");
            drawing.Add("sona.q.draw", new CheckBox("Q Range")); //.SetValue(new Circle(true, Color.Chartreuse)));
            drawing.Add("sona.w.draw", new CheckBox("W Range")); //.SetValue(new Circle(true, Color.Yellow)));
            drawing.Add("sona.e.draw", new CheckBox("E Range")); //.SetValue(new Circle(true, Color.White)));
            drawing.Add("sona.r.draw", new CheckBox("R Range")); //.SetValue(new Circle(true, Color.SandyBrown)));

            Config.Add("sona.hitchance", new ComboBox("Skillshot Hit Chance", 2, HitchanceNameArray));

            Game.OnUpdate += SonaOnUpdate;
            Drawing.OnDraw += SonaOnDraw;
            AntiGapcloser.OnEnemyGapcloser += SonaOnEnemyGapcloser;
            Interrupter2.OnInterruptableTarget += SonaOnInterruptableTarget;
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

        private static void SonaOnInterruptableTarget(AIHeroClient sender, Interrupter2.InterruptableTargetEventArgs args)
        {
            if (sender.IsMe || sender.IsAlly)
            {
                return;
            }
            if (R.IsReady() && sender.IsValidTarget(R.Range) && getCheckBoxItem(misc, "sona.inter"))
            {
                R.CastIfHitchanceEquals(sender, HitChance.High);
            }
        }

        private static void SonaOnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (gapcloser.Sender.IsMe || gapcloser.Sender.IsAlly)
            {
                return;
            }
            if (R.IsReady() && getCheckBoxItem(misc, "sona.anti") &&
                R.GetPrediction(gapcloser.Sender).Hitchance > HitChance.High
                && gapcloser.Sender.IsValidTarget(1000))
            {
                R.Cast(gapcloser.Sender);
            }
        }

        private static void SonaOnUpdate(EventArgs args)
        {
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                Combo();
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                Harass();
            }

            if (!getCheckBoxItem(healManager, "sona.heal.disable") && W.IsReady())
            {
                WManager();
            }

            if (getCheckBoxItem(rSettings, "sona.r.killsteal") && R.IsReady())
            {
                Killsteal();
            }
        }

        private static void WManager()
        {
            if (ObjectManager.Player.IsDead && ObjectManager.Player.IsZombie)
            {
                return;
            }

            if (ObjectManager.Player.IsRecalling() || ObjectManager.Player.InFountain())
            {
                return;
            }

            foreach (
                var shield in
                    ObjectManager.Get<AIHeroClient>()
                        .Where(
                            x => x.IsAlly && !x.IsMe && !x.IsDead && x.LSDistance(ObjectManager.Player.Position) < W.Range)
                )
            {
                if (shield == null)
                {
                    return;
                }

                if (shield != null && getCheckBoxItem(heal, "sona.heal" + shield.NetworkId) &&
                    shield.HealthPercent < getSliderItem(heal, "sona.heal.percent" + shield.NetworkId) &&
                    ObjectManager.Player.HealthPercent > getSliderItem(heal, "sona.heal.limit"))
                {
                    W.Cast(shield);
                }
            }
        }

        private static void Killsteal()
        {
            if (ObjectManager.Player.IsDead && ObjectManager.Player.IsZombie)
            {
                return;
            }

            if (getCheckBoxItem(rSettings, "sona.r.killsteal") && R.IsReady())
            {
                foreach (
                    var target in HeroManager.Enemies.Where(x => x.IsValidTarget(R.Range) && !x.IsDead && !x.IsZombie))
                {
                    if (R.GetDamage(target) > target.Health && R.IsInRange(target))
                    {
                        R.CastIfHitchanceEquals(target, SpellHitChance(Config, "sona.hitchance"));
                    }
                }
            }
        }

        private static void Combo()
        {
            if (getCheckBoxItem(comboMenu, "sona.q.combo") && Q.IsReady())
            {
                foreach (
                    var enemy in HeroManager.Enemies.Where(x => x.IsValidTarget(Q.Range) && !x.IsDead && !x.IsZombie))
                {
                    Q.Cast(enemy);
                }
            }

            if (getCheckBoxItem(comboMenu, "sona.r.combo") && R.IsReady())
            {
                foreach (
                    var enemy in HeroManager.Enemies.Where(x => x.IsValidTarget(R.Range) && !x.IsDead && !x.IsZombie))
                {
                    if (R.IsInRange(enemy))
                    {
                        R.CastIfHitchanceEquals(enemy, SpellHitChance(Config, "sona.hitchance"));
                    }
                }
            }
        }

        private static void Harass()
        {
            if (ObjectManager.Player.ManaPercent < getSliderItem(harass, "sona.harass.mana"))
            {
                return;
            }

            if (getCheckBoxItem(harass, "sona.q.harass") && Q.IsReady())
            {
                foreach (var enemy in HeroManager.Enemies.Where(x => x.IsValidTarget(Q.Range) && !x.IsDead
                                                                     && !x.IsZombie &&
                                                                     !x.HasBuffOfType(BuffType.SpellShield) &&
                                                                     !x.HasBuffOfType(BuffType.SpellImmunity)))
                {
                    Q.Cast(enemy);
                }
            }
        }

        private static void SonaOnDraw(EventArgs args)
        {
            if (Q.IsReady() && getCheckBoxItem(drawing, "sona.q.draw"))
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, Q.Range, Color.Chartreuse);
            }

            if (W.IsReady() && getCheckBoxItem(drawing, "sona.w.draw"))
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, W.Range, Color.Yellow);
            }

            if (E.IsReady() && getCheckBoxItem(drawing, "sona.e.draw"))
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, E.Range, Color.White);
            }

            if (R.IsReady() && getCheckBoxItem(drawing, "sona.r.draw"))
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, R.Range, Color.SandyBrown);
            }
        }
    }
}