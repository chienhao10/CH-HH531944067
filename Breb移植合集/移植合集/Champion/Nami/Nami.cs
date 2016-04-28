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
    public class Nami : Helper
    {
        public static Menu Config;
        public static Spell Q, W, E, R;
        public static Menu comboMenu, healMenu, wsettings, esettings, harass, drawing, miscMenu;

        public static AIHeroClient Player = ObjectManager.Player;

        public Nami()
        {
            NamiOnLoad();
        }

        private static void NamiOnLoad()
        {
            Q = new Spell(SpellSlot.Q, 875);
            W = new Spell(SpellSlot.W, 725);
            E = new Spell(SpellSlot.E, 800);
            R = new Spell(SpellSlot.R, 2750);

            Q.SetSkillshot(1f, 125f, float.MaxValue, false, SkillshotType.SkillshotCircle);
            R.SetSkillshot(0.5f, 260f, 850f, false, SkillshotType.SkillshotLine);

            Config = MainMenu.AddMenu("vSupport Series: " + Player.ChampionName, "vSupport Series");

            comboMenu = Config.AddSubMenu("Combo Settings", "Combo Settings");
            comboMenu.Add("nami.q.combo", new CheckBox("Use Q"));
            comboMenu.Add("nami.w.combo", new CheckBox("Use W"));
            comboMenu.Add("nami.e.combo", new CheckBox("Use E"));
            comboMenu.Add("nami.r.combo", new CheckBox("Use R"));
            comboMenu.AddGroupLabel("R Settings");
            comboMenu.Add("nami.min.enemy.count", new Slider("Min. Enemy Count", 3, 1, 5));

            healMenu = Config.AddSubMenu("Heal Settings", "Heal Settings");
            healMenu.Add("nami.heal.disable", new CheckBox("Disable Heal?", false));
            healMenu.Add("nami.heal.mana", new Slider("Min. Mana to use Heal", 20, 1, 99));
            healMenu.Add("nami.heal.self", new CheckBox("Heal self?"));
            healMenu.Add("nami.heal.self.percent", new Slider("Min. HP to heal self", 30, 1, 99));
            healMenu.AddSeparator();
            healMenu.AddGroupLabel("Heal Whitelist : ");
            foreach (var ally in ObjectManager.Get<AIHeroClient>().Where(o => o.IsAlly && !o.IsMe))
            {
                healMenu.Add("heal." + ally.ChampionName,
                    new CheckBox(string.Format("Heal: {0}", ally.CharData.BaseSkinName)));
                healMenu.Add("heal.percent." + ally.ChampionName,
                    new Slider(string.Format("Heal: {0} HP Percent ", ally.CharData.BaseSkinName), 30, 1, 99));
            }

            wsettings = Config.AddSubMenu("(W) Settings", "(W) Settings");
            wsettings.AddGroupLabel("(W) Whitelist");
            foreach (var enemy in ObjectManager.Get<AIHeroClient>().Where(o => o.IsEnemy))
            {
                wsettings.Add("wwhite." + enemy.CharData.BaseSkinName,
                    new CheckBox(string.Format("(W): {0}", enemy.CharData.BaseSkinName)));
            }

            esettings = Config.AddSubMenu("(E) Settings", "(E) Settings");
            esettings.Add("e.mana", new Slider("Min. Mana to use (E)", 20, 1, 99));
            esettings.AddGroupLabel("(E) Whitelist");
            foreach (var ally in ObjectManager.Get<AIHeroClient>().Where(o => o.IsAlly && !o.IsMe))
            {
                esettings.Add("ewhite." + ally.CharData.BaseSkinName,
                    new CheckBox(string.Format("(E): {0}", ally.CharData.BaseSkinName)));
            }

            harass = Config.AddSubMenu("Harass Settings", "Harass Settings");
            harass.Add("nami.q.harass", new CheckBox("Use Q"));
            harass.Add("nami.w.harass", new CheckBox("Use W"));
            harass.Add("nami.harass.mana", new Slider("Min. Mana Percent", 50, 1, 99));

            drawing = Config.AddSubMenu("Draw Settings", "Draw Settings");
            drawing.Add("nami.q.draw", new CheckBox("Q Range")); //.SetValue(new Circle(true, Color.Chartreuse)));
            drawing.Add("nami.w.draw", new CheckBox("W Range")); //.SetValue(new Circle(true, Color.Yellow)));
            drawing.Add("nami.e.draw", new CheckBox("E Range")); //.SetValue(new Circle(true, Color.White)));
            drawing.Add("nami.r.draw", new CheckBox("R Range")); //.SetValue(new Circle(true, Color.SandyBrown)));

            miscMenu = Config.AddSubMenu("Misc Settings", "misc Settings");
            miscMenu.Add("nami.q.hitchance", new ComboBox("Skillshot Hit Chance", 2, HitchanceNameArray));

            Orbwalker.OnPostAttack += NamiAfterAttack;
            Game.OnUpdate += NamiOnUpdate;
            Drawing.OnDraw += NamiOnDraw;
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

        private static void NamiAfterAttack(AttackableUnit unit, EventArgs args)
        {
            if (getSliderItem(esettings, "e.mana") <= Player.ManaPercent)
            {
                return;
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo) &&
                getCheckBoxItem(comboMenu, "nami.e.combo") && E.IsReady())
            {
                foreach (
                    var ally in
                        ObjectManager.Get<AIHeroClient>()
                            .Where(
                                x =>
                                    x.IsAlly && !x.IsMe && x.Distance(Player.Position) < E.Range && !x.IsDead &&
                                    !x.IsZombie))
                {
                    if (getCheckBoxItem(esettings, "ewhite." + ally.CharData.BaseSkinName))
                    {
                        E.Cast(ally);
                    }
                }
            }
        }

        private static void NamiOnUpdate(EventArgs args)
        {
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                Combo();
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass))
            {
                Harass();
            }

            HealManager();
        }

        private static void Combo()
        {
            if (getCheckBoxItem(comboMenu, "nami.q.combo") && Q.IsReady())
            {
                foreach (
                    var enemy in
                        HeroManager.Enemies.Where(
                            x =>
                                x.IsValidTarget(Q.Range) && !x.IsDead && !x.IsZombie &&
                                !x.HasBuffOfType(BuffType.SpellShield) && !x.HasBuffOfType(BuffType.SpellImmunity)))
                {
                    Q.CastIfHitchanceEquals(enemy, SpellHitChance(miscMenu, "nami.q.hitchance"));
                }
            }
            if (getCheckBoxItem(comboMenu, "nami.w.combo") && W.IsReady())
            {
                foreach (
                    var enemy in HeroManager.Enemies.Where(x => x.IsValidTarget(W.Range) && !x.IsDead && !x.IsZombie))
                {
                    if (getCheckBoxItem(wsettings, "wwhite." + enemy.CharData.BaseSkinName))
                    {
                        W.Cast(enemy);
                    }
                }
            }
            if (getCheckBoxItem(comboMenu, "nami.r.combo") && R.IsReady())
            {
                foreach (var enemy in HeroManager.Enemies.Where(x => x.IsValidTarget(R.Range)))
                {
                    if (Player.CountEnemiesInRange(R.Range) >= getSliderItem(comboMenu, "nami.min.enemy.count"))
                    {
                        R.CastIfWillHit(enemy, getSliderItem(comboMenu, "nami.min.enemy.count"));
                    }
                }
            }
        }

        private static void Harass()
        {
            if (Player.ManaPercent < getSliderItem(harass, "nami.harass.mana"))
            {
                return;
            }

            if (getCheckBoxItem(harass, "nami.q.harass") && Q.IsReady())
            {
                foreach (
                    var enemy in
                        HeroManager.Enemies.Where(
                            x =>
                                x.IsValidTarget(Q.Range) && !x.IsDead && !x.IsZombie &&
                                !x.HasBuffOfType(BuffType.SpellShield) && !x.HasBuffOfType(BuffType.SpellImmunity)))
                {
                    Q.CastIfHitchanceEquals(enemy, SpellHitChance(miscMenu, "nami.q.hitchance"));
                }
            }
            if (getCheckBoxItem(harass, "nami.w.harass") && W.IsReady())
            {
                foreach (
                    var enemy in HeroManager.Enemies.Where(x => x.IsValidTarget(W.Range) && !x.IsDead && !x.IsZombie))
                {
                    if (getCheckBoxItem(wsettings, "wwhite." + enemy.CharData.BaseSkinName))
                    {
                        W.Cast(enemy);
                    }
                }
            }
        }

        private static void HealManager()
        {
            if (getCheckBoxItem(healMenu, "nami.heal.disable"))
            {
                return;
            }

            if (Player.ManaPercent <= getSliderItem(healMenu, "nami.heal.mana"))
            {
                return;
            }

            if (Player.IsRecalling() || Player.InFountain())
            {
                return;
            }

            if (getCheckBoxItem(healMenu, "nami.heal.self") &&
                Player.HealthPercent <= getSliderItem(healMenu, "nami.heal.self.percent"))
            {
                W.Cast(Player);
            }

            if (Player.CountAlliesInRange(W.Range) == 0)
            {
                return;
            }

            if (W.IsReady() && !Player.IsDead && !Player.IsZombie)
            {
                foreach (
                    var ally in HeroManager.Allies.Where(x => W.IsInRange(x) && !x.IsDead && !x.IsZombie && !x.IsMe))
                {
                    if (getCheckBoxItem(healMenu, "heal." + ally.ChampionName) &&
                        ally.HealthPercent < getSliderItem(healMenu, "heal.percent." + ally.ChampionName))
                    {
                        W.Cast(ally);
                    }
                }
            }
        }

        private static void NamiOnDraw(EventArgs args)
        {
            if (Q.IsReady() && getCheckBoxItem(drawing, "nami.q.draw"))
            {
                Render.Circle.DrawCircle(Player.Position, Q.Range, Color.Chartreuse);
            }
            if (W.IsReady() && getCheckBoxItem(drawing, "nami.w.draw"))
            {
                Render.Circle.DrawCircle(Player.Position, W.Range, Color.Yellow);
            }
            if (E.IsReady() && getCheckBoxItem(drawing, "nami.e.draw"))
            {
                Render.Circle.DrawCircle(Player.Position, E.Range, Color.White);
            }
            if (R.IsReady() && getCheckBoxItem(drawing, "nami.r.draw"))
            {
                Render.Circle.DrawCircle(Player.Position, R.Range, Color.SandyBrown);
            }
        }
    }
}