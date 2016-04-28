using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using LeagueSharp.Common;
using UnderratedAIO.Helpers;
using Damage = LeagueSharp.Common.Damage;
using Environment = UnderratedAIO.Helpers.Environment;
using Spell = LeagueSharp.Common.Spell;
using Utility = LeagueSharp.Common.Utility;

namespace UnderratedAIO.Champions
{
    internal class Chogath
    {
        public static Menu config, comboMenu, harassMenu, laneClearMenu, MiscMenu, drawMenu;
        public static readonly AIHeroClient player = ObjectManager.Player;
        public static Spell Q, W, E, R, RFlash;
        public static List<int> silence = new List<int>(new[] {1500, 1750, 2000, 2250, 2500});
        public static int knockUp = 1000;
        public static bool flashRblock;

        private static bool VorpalSpikes
        {
            get { return player.Buffs.Any(buff => buff.Name == "VorpalSpikes"); }
        }

        public static void Load()
        {
            InitChoGath();
            InitMenu();
            Drawing.OnDraw += Game_OnDraw;
            Game.OnUpdate += Game_OnGameUpdate;
            AntiGapcloser.OnEnemyGapcloser += OnEnemyGapcloser;
            Interrupter2.OnInterruptableTarget += OnPossibleToInterrupt;
        }


        private static void OnPossibleToInterrupt(AIHeroClient sender, Interrupter2.InterruptableTargetEventArgs args)
        {
            if (getCheckBoxItem(MiscMenu, "useQint"))
            {
                if (Q.CanCast(sender))
                {
                    Q.Cast(sender, getCheckBoxItem(config, "packets"));
                }
            }

            if (getCheckBoxItem(MiscMenu, "useWint"))
            {
                if (W.CanCast(sender))
                {
                    W.Cast(sender, getCheckBoxItem(config, "packets"));
                }
            }
        }

        public static void Game_OnGameUpdate(EventArgs args)
        {
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                Combo();
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass))
            {
                Harass();
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear) ||
                Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LastHit))
            {
                Clear();
            }
        }

        private static void Clear()
        {
            var minions = ObjectManager.Get<Obj_AI_Minion>().Where(m => m.IsValidTarget(400)).ToList();
            if (minions.Count > 2)
            {
                if (Items.HasItem(3077) && Items.CanUseItem(3077))
                {
                    Items.UseItem(3077);
                }
                if (Items.HasItem(3074) && Items.CanUseItem(3074))
                {
                    Items.UseItem(3074);
                }
            }

            var perc = getSliderItem(laneClearMenu, "minmana")/100f;
            if (player.Mana < player.MaxMana*perc)
            {
                return;
            }

            if (getCheckBoxItem(laneClearMenu, "usewLC") && W.IsReady() &&
                player.Spellbook.GetSpell(SpellSlot.W).SData.Mana <= player.Mana)
            {
                var minionsForW = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, W.Range, MinionTypes.All,
                    MinionTeam.NotAlly);
                var bestPositionW = W.GetLineFarmLocation(minionsForW);
                if (bestPositionW.Position.IsValid())
                {
                    if (bestPositionW.MinionsHit >= getSliderItem(laneClearMenu, "whitLC"))
                    {
                        W.Cast(bestPositionW.Position, getCheckBoxItem(config, "packets"));
                    }
                }
            }

            if (getCheckBoxItem(laneClearMenu, "useqLC") && Q.IsReady() &&
                player.Spellbook.GetSpell(SpellSlot.Q).SData.Mana <= player.Mana)
            {
                var minionsForQ = MinionManager.GetMinions(
                    ObjectManager.Player.ServerPosition, Q.Range, MinionTypes.All, MinionTeam.NotAlly);
                var bestPositionQ = Q.GetCircularFarmLocation(minionsForQ);
                if (Q.IsReady() && bestPositionQ.MinionsHit > getSliderItem(laneClearMenu, "qhitLC"))
                {
                    Q.Cast(bestPositionQ.Position, getCheckBoxItem(config, "packets"));
                }
            }
        }

        private static void Harass()
        {
            var target = TargetSelector.GetTarget(Q.Range, DamageType.Magical);
            if (getCheckBoxItem(harassMenu, "useqH"))
            {
                if (target.IsValidTarget(Q.Range) && Q.IsReady())
                {
                    Q.Cast(target, getCheckBoxItem(config, "packets"));
                }
            }
            if (getCheckBoxItem(harassMenu, "useeH"))
            {
                if (target.IsValidTarget(W.Range) && W.IsReady())
                {
                    W.Cast(target, getCheckBoxItem(config, "packets"));
                }
            }
            if (getCheckBoxItem(harassMenu, "useeH") && !VorpalSpikes && E.GetHitCount() > 0)
            {
                E.Cast();
            }
        }

        private static void Combo()
        {
            var target = TargetSelector.GetTarget(1000, DamageType.Magical);

            if (getCheckBoxItem(comboMenu, "usee") && !VorpalSpikes && E.GetHitCount() > 0 &&
                (Environment.Turret.countTurretsInRange(player) < 1 || target.Health < 150))
            {
                E.Cast();
            }

            if (target == null)
            {
                return;
            }

            if (getCheckBoxItem(comboMenu, "selected"))
            {
                target = CombatHelper.SetTarget(target, TargetSelector.SelectedTarget);
            }

            var combodmg = ComboDamage(target);

            var hasFlash = player.Spellbook.CanUseSpell(player.GetSpellSlot("SummonerFlash")) == SpellState.Ready;
            var hasIgnite = player.Spellbook.CanUseSpell(player.GetSpellSlot("SummonerDot")) == SpellState.Ready;
            var ignitedmg = (float) player.GetSummonerSpellDamage(target, Damage.SummonerSpell.Ignite);
            if (hasIgnite && ignitedmg > target.Health && !R.CanCast(target) && !W.CanCast(target) && !Q.CanCast(target))
            {
                player.Spellbook.CastSpell(player.GetSpellSlot("SummonerDot"), target);
            }

            if (hasIgnite && combodmg > target.Health && R.CanCast(target) &&
                (float) player.LSGetSpellDamage(target, SpellSlot.R) < target.Health)
            {
                player.Spellbook.CastSpell(player.GetSpellSlot("SummonerDot"), target);
            }

            flashRblock = hasIgnite;

            if (getCheckBoxItem(comboMenu, "useq") && Q.IsReady())
            {
                var qHit = getSliderItem(comboMenu, "qHit");
                var hitC = HitChance.VeryHigh;
                switch (qHit)
                {
                    case 1:
                        hitC = HitChance.Low;
                        break;
                    case 2:
                        hitC = HitChance.Medium;
                        break;
                    case 3:
                        hitC = HitChance.High;
                        break;
                    case 4:
                        hitC = HitChance.VeryHigh;
                        break;
                }
                var pred = Q.GetPrediction(target);
                if (pred.Hitchance >= hitC)
                {
                    if (target.IsMoving)
                    {
                        if (pred.CastPosition.Distance(target.ServerPosition) > 250f)
                        {
                            Q.Cast(target.Position.Extend(pred.CastPosition, 250f));
                        }
                        else
                        {
                            Q.Cast(pred.CastPosition);
                        }
                    }
                    else
                    {
                        Q.CastIfHitchanceEquals(target, hitC);
                    }
                }
            }

            if (getCheckBoxItem(comboMenu, "usew") && W.CanCast(target))
            {
                W.Cast(target, getCheckBoxItem(config, "packets"));
            }

            if (getCheckBoxItem(comboMenu, "UseFlashC") && !flashRblock && R.IsReady() && hasFlash &&
                !CombatHelper.CheckCriticalBuffs(target) && player.GetSpell(SpellSlot.R).SData.Mana <= player.Mana &&
                player.Distance(target.Position) >= 400 && player.GetSpellDamage(target, SpellSlot.R) > target.Health &&
                !Q.IsReady() && !W.IsReady() && player.Distance(target.Position) <= RFlash.Range &&
                !player.Position.Extend(target.Position, 400).IsWall())
            {
                player.Spellbook.CastSpell(player.GetSpellSlot("SummonerFlash"),
                    player.Position.Extend(target.Position, 400).To3D());
                Utility.DelayAction.Add(50, () => R.Cast(target, getCheckBoxItem(config, "packets")));
            }

            var rtarget = HeroManager.Enemies.Where(e => e.IsValidTarget() && R.CanCast(e)).OrderByDescending(TargetSelector.GetPriority).FirstOrDefault();
            if (getCheckBoxItem(comboMenu, "user") && rtarget != null &&
                player.GetSpellDamage(target, SpellSlot.R) > rtarget.Health)
            {
                R.Cast(rtarget, getCheckBoxItem(config, "packets"));
            }
        }

        private static void OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (getCheckBoxItem(MiscMenu, "useQgc"))
            {
                if (gapcloser.Sender.IsValidTarget(Q.Range) && Q.IsReady())
                {
                    Q.Cast(gapcloser.End, getCheckBoxItem(config, "packets"));
                }
            }
            if (getCheckBoxItem(MiscMenu, "useWgc"))
            {
                if (gapcloser.Sender.IsValidTarget(W.Range) && W.IsReady())
                {
                    W.Cast(gapcloser.End, getCheckBoxItem(config, "packets"));
                }
            }
        }

        private static void Game_OnDraw(EventArgs args)
        {
            DrawHelper.DrawCircle(getCheckBoxItem(drawMenu, "drawqq"), Q.Range, Color.FromArgb(180, 200, 46, 66));
            DrawHelper.DrawCircle(getCheckBoxItem(drawMenu, "drawww"), W.Range, Color.FromArgb(180, 200, 46, 66));
            DrawHelper.DrawCircle(getCheckBoxItem(drawMenu, "drawee"), E.Range, Color.FromArgb(180, 200, 46, 66));
            DrawHelper.DrawCircle(getCheckBoxItem(drawMenu, "drawrrflash"), RFlash.Range,
                Color.FromArgb(150, 250, 248, 110));
        }

        public static float ComboDamage(AIHeroClient hero)
        {
            double damage = 0;
            if (Q.IsReady())
            {
                damage += player.LSGetSpellDamage(hero, SpellSlot.Q);
            }
            if (W.IsReady())
            {
                damage += player.LSGetSpellDamage(hero, SpellSlot.W);
            }
            if (player.Spellbook.CanUseSpell(player.GetSpellSlot("summonerdot")) == SpellState.Ready)
            {
                damage += player.GetSummonerSpellDamage(hero, Damage.SummonerSpell.Ignite);
            }
            if (R.IsReady())
            {
                damage += player.LSGetSpellDamage(hero, SpellSlot.R);
            }
            return (float) damage;
        }

        private static void InitChoGath()
        {
            Q = new Spell(SpellSlot.Q, 950);
            Q.SetSkillshot(1.2f, 175f, float.MaxValue, false, SkillshotType.SkillshotCircle);
            W = new Spell(SpellSlot.W, 650);
            W.SetSkillshot(0.25f, 250f, float.MaxValue, false, SkillshotType.SkillshotCone);
            E = new Spell(SpellSlot.E, 500);
            E.SetSkillshot(E.Instance.SData.SpellCastTime, E.Instance.SData.LineWidth, E.Speed, false,
                SkillshotType.SkillshotLine);
            R = new Spell(SpellSlot.R, 175);
            RFlash = new Spell(SpellSlot.R, 555);
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

        private static void InitMenu()
        {
            config = MainMenu.AddMenu("Cho'Gath", "ChoGath");
            config.Add("packets", new CheckBox("Use Packets", false));

            // Draw settings
            drawMenu = config.AddSubMenu("Drawings ", "dsettings");
            drawMenu.Add("drawqq", new CheckBox("Draw Q range"));
            drawMenu.Add("drawww", new CheckBox("Draw W range"));
            drawMenu.Add("drawee", new CheckBox("Draw E range"));
                //.SetValue(new Circle(false, Color.FromArgb(180, 200, 46, 66)));
            drawMenu.Add("drawrrflash", new CheckBox("Draw R+flash range"));
                //.SetValue(new Circle(true, Color.FromArgb(150, 250, 248, 110)));

            // Combo Settings
            comboMenu = config.AddSubMenu("Combo ", "csettings");
            comboMenu.Add("useq", new CheckBox("Use Q"));
            comboMenu.Add("qHit", new Slider("Q hitChance (Lowest to highest) : ", 4, 1, 4));
            comboMenu.Add("usew", new CheckBox("Use W"));
            comboMenu.Add("usee", new CheckBox("Use E"));
            comboMenu.Add("user", new CheckBox("Use R"));
            comboMenu.Add("UseFlashC", new CheckBox("Use flash", false));
            comboMenu.Add("selected", new CheckBox("Focus Selected target"));
            comboMenu.Add("useIgnite", new CheckBox("Use Ignite"));

            // Harass Settings
            harassMenu = config.AddSubMenu("Harass ", "Hsettings");
            harassMenu.Add("useqH", new CheckBox("Use Q"));
            harassMenu.Add("usewH", new CheckBox("Use W"));
            harassMenu.Add("useeH", new CheckBox("Use E"));

            // LaneClear Settings
            laneClearMenu = config.AddSubMenu("LaneClear ", "Lcsettings");
            laneClearMenu.Add("useqLC", new CheckBox("Use Q"));
            laneClearMenu.Add("qhitLC", new Slider("More than x minion", 2, 1, 10));
            laneClearMenu.Add("usewLC", new CheckBox("Use W"));
            laneClearMenu.Add("whitLC", new Slider("More than x minion", 2, 1, 10));
            laneClearMenu.Add("minmana", new Slider("Keep X% mana", 1, 1));

            // Misc Settings
            MiscMenu = config.AddSubMenu("Misc ", "Msettings");
            MiscMenu.Add("useQint", new CheckBox("Use Q to interrupt"));
            MiscMenu.Add("useQgc", new CheckBox("Use Q on gapclosers"));
            MiscMenu.Add("useWint", new CheckBox("Use W to interrupt"));
            MiscMenu.Add("useWgc", new CheckBox("Use W on gapclosers"));
        }
    }
}