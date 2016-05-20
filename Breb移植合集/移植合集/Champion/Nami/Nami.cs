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

            Config = MainMenu.AddMenu("V辅助合集: " + Player.ChampionName, "V辅助合集");

            comboMenu = Config.AddSubMenu("连招", "Combo Settings");
            comboMenu.Add("nami.q.combo", new CheckBox("使用 Q"));
            comboMenu.Add("nami.w.combo", new CheckBox("使用 W"));
            comboMenu.Add("nami.e.combo", new CheckBox("使用 E"));
            comboMenu.Add("nami.r.combo", new CheckBox("使用 R"));
            comboMenu.AddGroupLabel("R 设置");
            comboMenu.Add("nami.min.enemy.count", new Slider("最少敌人数量", 3, 1, 5));

            healMenu = Config.AddSubMenu("治疗设置", "Heal Settings");
            healMenu.Add("nami.heal.disable", new CheckBox("屏蔽治疗?", false));
            healMenu.Add("nami.heal.mana", new Slider("最低蓝量使用治疗", 20, 1, 99));
            healMenu.Add("nami.heal.self", new CheckBox("治疗自己?"));
            healMenu.Add("nami.heal.self.percent", new Slider("最低血量治疗自己", 30, 1, 99));
            healMenu.AddSeparator();
            healMenu.AddGroupLabel("治疗白名单 : ");
            foreach (var ally in ObjectManager.Get<AIHeroClient>().Where(o => o.IsAlly && !o.IsMe))
            {
                healMenu.Add("heal." + ally.NetworkId, new CheckBox(string.Format("治疗: {0}", ally.CharData.BaseSkinName)));
                healMenu.Add("heal.percent." + ally.NetworkId, new Slider(string.Format("治疗: {0} 生命% ", ally.CharData.BaseSkinName), 30, 1, 99));
            }

            wsettings = Config.AddSubMenu("(W) 设置", "(W) Settings");
            wsettings.AddGroupLabel("(W) 白名单");
            foreach (var enemy in ObjectManager.Get<AIHeroClient>().Where(o => o.IsEnemy))
            {
                wsettings.Add("wwhite." + enemy.NetworkId, new CheckBox(string.Format("(W): {0}", enemy.CharData.BaseSkinName)));
            }

            esettings = Config.AddSubMenu("(E) 设置", "(E) Settings");
            esettings.Add("e.mana", new Slider("最低蓝量使用 (E)", 20, 1, 99));
            esettings.AddGroupLabel("(E) 白名单");
            foreach (var ally in ObjectManager.Get<AIHeroClient>().Where(o => o.IsAlly && !o.IsMe))
            {
                esettings.Add("ewhite." + ally.NetworkId, new CheckBox(string.Format("(E): {0}", ally.CharData.BaseSkinName)));
            }

            harass = Config.AddSubMenu("骚扰设置", "Harass Settings");
            harass.Add("nami.q.harass", new CheckBox("使用 Q"));
            harass.Add("nami.w.harass", new CheckBox("使用 W"));
            harass.Add("nami.harass.mana", new Slider("最低蓝量 %", 50, 1, 99));

            drawing = Config.AddSubMenu("线圈竖直", "Draw Settings");
            drawing.Add("nami.q.draw", new CheckBox("Q 范围")); //.SetValue(new Circle(true, Color.Chartreuse)));
            drawing.Add("nami.w.draw", new CheckBox("W 范围")); //.SetValue(new Circle(true, Color.Yellow)));
            drawing.Add("nami.e.draw", new CheckBox("E 范围")); //.SetValue(new Circle(true, Color.White)));
            drawing.Add("nami.r.draw", new CheckBox("R 范围")); //.SetValue(new Circle(true, Color.SandyBrown)));

            miscMenu = Config.AddSubMenu("杂项", "misc Settings");
            miscMenu.Add("nami.q.hitchance", new ComboBox("技能命中率", 2, HitchanceNameArray));

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
                                    x.IsAlly && !x.IsMe && x.LSDistance(Player.Position) < E.Range && !x.IsDead &&
                                    !x.IsZombie))
                {
                    if (getCheckBoxItem(esettings, "ewhite." + ally.NetworkId))
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
                    if (getCheckBoxItem(wsettings, "wwhite." + enemy.NetworkId))
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
                    if (getCheckBoxItem(wsettings, "wwhite." + enemy.NetworkId))
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
                    if (getCheckBoxItem(healMenu, "heal." + ally.NetworkId) &&
                        ally.HealthPercent < getSliderItem(healMenu, "heal.percent." + ally.NetworkId))
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
