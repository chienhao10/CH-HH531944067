using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using LeagueSharp.Common;
using UnderratedAIO.Helpers;
using Damage = LeagueSharp.Common.Damage;
using Prediction = LeagueSharp.Common.Prediction;
using Spell = LeagueSharp.Common.Spell;
using Utility = LeagueSharp.Common.Utility;

namespace UnderratedAIO.Champions
{
    internal class Poppy
    {
        public static Menu config;
        public static readonly AIHeroClient player = ObjectManager.Player;
        public static Spell Q, W, E, R;
        public static double[] eSecond = new double[5] {75, 125, 175, 225, 275};
        public static List<string> NotDash = new List<string> {"Udyr", "Malphite"};

        public static Menu menuD, menuC, menuH, menuLC, menuM;

        public Poppy()
        {
            Initpoppy();
            InitMenu();
            Game.OnUpdate += Game_OnGameUpdate;
            Drawing.OnDraw += Game_OnDraw;
            AntiGapcloser.OnEnemyGapcloser += OnEnemyGapcloser;
            Interrupter2.OnInterruptableTarget += OnInterruptableTarget;
        }

        private void OnInterruptableTarget(AIHeroClient sender, Interrupter2.InterruptableTargetEventArgs args)
        {
            if (getCheckBoxItem(menuM, "useEint") && E.IsReady() && E.CanCast(sender))
            {
                E.CastOnUnit(sender, getCheckBoxItem(config, "packets"));
            }
        }


        private static void Game_OnGameUpdate(EventArgs args)
        {
            var targetf = TargetSelector.GetTarget(1000, DamageType.Magical);
            if (getKeyBindItem(menuC, "useeflashforced"))
            {
                if (targetf == null)
                {
                    Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
                }
                else
                {
                    var bestpos = CombatHelper.bestVectorToPoppyFlash2(targetf);
                    var hasFlash = player.Spellbook.CanUseSpell(player.GetSpellSlot("SummonerFlash")) ==
                                   SpellState.Ready;
                    if (E.IsReady() && hasFlash && !CheckWalls(player, targetf) && bestpos.IsValid())
                    {
                        player.Spellbook.CastSpell(player.GetSpellSlot("SummonerFlash"), bestpos);
                        Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
                    }
                    else if (!hasFlash)
                    {
                        Combo();
                        Orbwalker.OrbwalkTo(Game.CursorPos);
                        //Orbwalking.Orbwalk(targetf, Game.CursorPos, 90, 90);
                    }
                }
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                Combo();
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass))
            {
                Harass();
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear) ||
                Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear))
            {
                Clear();
            }

            if (!player.IsDead)
            {
                foreach (var dashingEnemy in
                    HeroManager.Enemies.Where(
                        e =>
                            e.IsValidTarget() && e.Distance(player) < 1600 &&
                            getSliderItem(menuM, "useAutoW" + e.BaseSkinName) > 0)
                        .OrderByDescending(e => getSliderItem(menuM, "useAutoW" + e.BaseSkinName))
                        .ThenBy(e => e.Distance(player)))
                {
                    var nextpos = Prediction.GetPrediction(dashingEnemy, 0.1f).UnitPosition;
                    if (dashingEnemy.IsDashing() && !dashingEnemy.HasBuffOfType(BuffType.SpellShield) &&
                        !dashingEnemy.HasBuff("poppyepushenemy") && dashingEnemy.Distance(player) <= W.Range &&
                        (nextpos.Distance(player.Position) > W.Range || (player.Distance(dashingEnemy) < W.Range - 100)) &&
                        dashingEnemy.IsTargetable && !NotDash.Contains(dashingEnemy.ChampionName))
                    {
                        W.Cast();
                    }
                    if (
                        CombatHelper.DashDatas.Any(
                            d => d.ChampionName == dashingEnemy.ChampionName && d.IsReady(dashingEnemy)))
                    {
                        break;
                    }
                }
            }
        }


        private static void Combo()
        {
            var target = TargetSelector.GetTarget(1000, DamageType.Magical);
            if (target == null)
            {
                return;
            }
            var cmbdmg = ComboDamage(target);
            var hasFlash = player.Spellbook.CanUseSpell(player.GetSpellSlot("SummonerFlash")) == SpellState.Ready;
            if (getCheckBoxItem(menuC, "usee") && E.IsReady())
            {
                if (getCheckBoxItem(menuC, "useewall"))
                {
                    var bestpos = CombatHelper.bestVectorToPoppyFlash2(target);
                    var damage =
                        (float)
                            (ComboDamage(target) +
                             player.CalcDamage(target, DamageType.Magical,
                                 eSecond[E.Level - 1] + 0.8f*player.FlatMagicDamageMod) +
                             player.GetAutoAttackDamage(target)*4);
                    var damageno = ComboDamage(target) + player.GetAutoAttackDamage(target)*4;
                    if (getCheckBoxItem(menuC, "useeflash") && hasFlash && !CheckWalls(player, target) &&
                        damage > target.Health && target.Health > damageno &&
                        CombatHelper.bestVectorToPoppyFlash(target).IsValid())
                    {
                        player.Spellbook.CastSpell(player.GetSpellSlot("SummonerFlash"), bestpos);
                        Utility.DelayAction.Add(
                            100, () => E.CastOnUnit(target, getCheckBoxItem(config, "packets")));
                    }
                    if (E.CanCast(target) &&
                        (CheckWalls(player, target) ||
                         target.Health < E.GetDamage(target) + player.GetAutoAttackDamage(target, true)))
                    {
                        E.CastOnUnit(target, getCheckBoxItem(config, "packets"));
                    }
                    if (E.CanCast(target) && Q.IsReady() && Q.Instance.SData.Mana + E.Instance.SData.Mana > player.Mana &&
                        target.Health <
                        E.GetDamage(target) + Q.GetDamage(target) + player.GetAutoAttackDamage(target, true))
                    {
                        E.CastOnUnit(target, getCheckBoxItem(config, "packets"));
                    }
                }
                else
                {
                    if (E.CanCast(target))
                    {
                        E.CastOnUnit(target, getCheckBoxItem(config, "packets"));
                    }
                }
            }
            if (getCheckBoxItem(menuC, "useq") && Q.IsReady() && Q.CanCast(target) && target.Distance(player) < Q.Range &&
                (player.Distance(target) > Orbwalking.GetRealAutoAttackRange(target) || !Orbwalker.CanAutoAttack))
            {
                Q.CastIfHitchanceEquals(target, HitChance.High, getCheckBoxItem(config, "packets"));
            }

            var hasIgnite = player.Spellbook.CanUseSpell(player.GetSpellSlot("SummonerDot")) == SpellState.Ready &&
                            getCheckBoxItem(menuC, "useIgnite");
            var ignitedmg = hasIgnite ? (float) player.GetSummonerSpellDamage(target, Damage.SummonerSpell.Ignite) : 0f;
            if (ignitedmg > target.Health && hasIgnite && !E.CanCast(target) && !Q.CanCast(target) &&
                (player.Distance(target) > Q.Range || player.HealthPercent < 30))
            {
                player.Spellbook.CastSpell(player.GetSpellSlot("SummonerDot"), target);
            }
            if (getCheckBoxItem(menuC, "userindanger") && R.IsReady() && player.CountEnemiesInRange(800) >= 2 &&
                player.CountEnemiesInRange(800) > player.CountAlliesInRange(1500) + 1 && player.HealthPercent < 60 ||
                (player.Health < target.Health && player.HealthPercent < 40 &&
                 player.CountAlliesInRange(1000) + 1 < player.CountEnemiesInRange(1000)))
            {
                var targ =
                    HeroManager.Enemies.Where(
                        e =>
                            e.IsValidTarget() && R.CanCast(e) &&
                            (player.HealthPercent < 60 || e.CountEnemiesInRange(300) > 2) &&
                            HeroManager.Enemies.Count(h => h.Distance(e) < 400 && e.HealthPercent < 35) == 0 &&
                            R.GetPrediction(e).CastPosition.Distance(player.Position) < R.ChargedMaxRange)
                        .OrderByDescending(e => R.GetPrediction(e).CastPosition.CountEnemiesInRange(400))
                        .ThenByDescending(e => e.Distance(target))
                        .FirstOrDefault();
                if (R.Range > 1300 && targ == null)
                {
                    targ =
                        HeroManager.Enemies.Where(
                            e =>
                                e.IsValidTarget() && R.CanCast(e) &&
                                R.GetPrediction(e).CastPosition.Distance(player.Position) < R.ChargedMaxRange)
                            .OrderByDescending(e => R.GetPrediction(e).CastPosition.CountEnemiesInRange(400))
                            .ThenByDescending(e => e.Distance(target))
                            .FirstOrDefault();
                }
                if (!R.IsCharging && targ != null)
                {
                    R.StartCharging();
                }
                if (R.IsCharging && targ != null && R.CanCast(targ) && R.Range > 1000 && R.Range > targ.Distance(player))
                {
                    R.CastIfHitchanceEquals(targ, HitChance.Medium, getCheckBoxItem(config, "packets"));
                }
                if (R.IsCharging && targ != null && R.Range < 1000)
                {
                    return;
                }
            }
            if (getCheckBoxItem(menuC, "user") && R.IsReady() && player.Distance(target) < 1400 &&
                !target.UnderTurret(true))
            {
                var cond = (Rdmg(target) < target.Health && ignitedmg + Rdmg(target) > target.Health &&
                            player.Distance(target) < 600) ||
                           (target.Distance(player) > E.Range && Rdmg(target) > target.Health &&
                            target.Distance(player) < 1100);
                if (!R.IsCharging && cond && !Q.IsReady() && player.HealthPercent < 40)
                {
                    R.StartCharging();
                    if (hasIgnite && cmbdmg > target.Health && cmbdmg - Rdmg(target) < target.Health)
                    {
                        if (!target.HasBuff("summonerdot"))
                        {
                            player.Spellbook.CastSpell(player.GetSpellSlot("SummonerDot"), target);
                        }
                    }
                }
                if (R.IsCharging && R.CanCast(target) && R.Range > target.Distance(player) && cond)
                {
                    R.CastIfHitchanceEquals(target, HitChance.High, getCheckBoxItem(config, "packets"));
                }
            }
        }

        private static void Clear()
        {
            var mob = Jungle.GetNearest(player.Position);
            var perc = getSliderItem(menuLC, "minmana")/100f;
            if (player.Mana < player.MaxMana*perc)
            {
                return;
            }
            if (getCheckBoxItem(menuLC, "useeLC") && E.CanCast(mob) && CheckWalls(player, mob))
            {
                E.CastOnUnit(mob, getCheckBoxItem(config, "packets"));
            }
            var bestPositionQ =
                Q.GetLineFarmLocation(MinionManager.GetMinions(Q.Range, MinionTypes.All, MinionTeam.NotAlly));
            if (bestPositionQ.MinionsHit >= getSliderItem(menuLC, "qMinHit") && getCheckBoxItem(menuLC, "useqLC"))
            {
                Q.Cast(bestPositionQ.Position, getCheckBoxItem(config, "packets"));
            }
        }

        private static void Harass()
        {
            var target = TargetSelector.GetTarget(Q.Range, DamageType.Magical);
            var perc = getSliderItem(menuH, "minmanaH")/100f;
            if (player.Mana < player.MaxMana*perc || target == null)
            {
                return;
            }
            if (getCheckBoxItem(menuH, "useqH") && Q.IsReady() && Q.CanCast(target) &&
                target.Distance(player) < Q.Range &&
                (player.Distance(target) > Orbwalking.GetRealAutoAttackRange(target) || !Orbwalker.CanAutoAttack))
            {
                Q.CastIfHitchanceEquals(target, HitChance.High, getCheckBoxItem(config, "packets"));
            }
        }

        private static void Game_OnDraw(EventArgs args)
        {
            DrawHelper.DrawCircle(getCheckBoxItem(menuD, "drawqq"), Q.Range, Color.DarkCyan);
            DrawHelper.DrawCircle(getCheckBoxItem(menuD, "drawww"), W.Range, Color.DarkCyan);
            DrawHelper.DrawCircle(getCheckBoxItem(menuD, "drawee"), E.Range, Color.DarkCyan);
            DrawHelper.DrawCircle(getCheckBoxItem(menuD, "drawrr"), R.Range, Color.DarkCyan);
        }

        private static void OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (getCheckBoxItem(menuM, "useEgap") && E.IsReady() && E.CanCast(gapcloser.Sender) &&
                CheckWalls(player, gapcloser.Sender))
            {
                E.CastOnUnit(gapcloser.Sender, getCheckBoxItem(config, "packets"));
            }
        }

        public static bool CheckWalls(Obj_AI_Base player, Obj_AI_Base enemy)
        {
            var distance = player.Position.Distance(enemy.Position);
            for (var i = 1; i < 6; i++)
            {
                if (player.Position.Extend(enemy.Position, distance + 60*i).IsWall())
                {
                    return true;
                }
            }
            return false;
        }

        public static float ComboDamage(AIHeroClient hero)
        {
            double damage = 0;
            if (Q.IsReady())
            {
                damage += player.LSGetSpellDamage(hero, SpellSlot.Q);
            }
            if (E.IsReady())
            {
                damage += (float) player.LSGetSpellDamage(hero, SpellSlot.E);
            }
            if (player.Spellbook.CanUseSpell(player.GetSpellSlot("summonerdot")) == SpellState.Ready &&
                player.Distance(hero) < 500)
            {
                damage += (float) player.GetSummonerSpellDamage(hero, Damage.SummonerSpell.Ignite);
            }
            if (R.IsReady() || R.IsCharging)
            {
                damage += (float) Rdmg(hero);
            }
            return (float) damage;
        }

        public static double Rdmg(Obj_AI_Base target)
        {
            return player.CalcDamage(target, DamageType.Physical,
                new double[] {200, 300, 400}[R.Level - 1] +
                0.9f*(player.BaseAttackDamage + player.FlatPhysicalDamageMod));
        }

        private static void Initpoppy()
        {
            Q = new Spell(SpellSlot.Q, 400);
            Q.SetSkillshot(0.55f, 90f, float.MaxValue, false, SkillshotType.SkillshotLine);
            W = new Spell(SpellSlot.W, 400);
            E = new Spell(SpellSlot.E, 500);
            R = new Spell(SpellSlot.R);
            R.SetSkillshot(0.5f, 90f, 1400, true, SkillshotType.SkillshotLine);
            R.SetCharged("PoppyR", "PoppyR", 425, 1400, 1.0f);
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

        private static void InitMenu()
        {
            config = MainMenu.AddMenu("Poppy", "Poppy");

            // Draw settings
            menuD = config.AddSubMenu("Drawings ", "dsettings");
            menuD.Add("drawqq", new CheckBox("Draw Q range")); //.SetValue(new Circle(false, Color.DarkCyan));
            menuD.Add("drawww", new CheckBox("Draw W range")); //.SetValue(new Circle(false, Color.DarkCyan));
            menuD.Add("drawee", new CheckBox("Draw E range")); //.SetValue(new Circle(false, Color.DarkCyan));
            menuD.Add("drawrr", new CheckBox("Draw R range")); //.SetValue(new Circle(false, Color.DarkCyan));

            // Combo Settings
            menuC = config.AddSubMenu("Combo ", "csettings");
            menuC.Add("useq", new CheckBox("Use Q"));
            menuC.Add("usew", new CheckBox("Use W"));
            menuC.Add("usee", new CheckBox("Use E"));
            menuC.Add("useewall", new CheckBox("Use E only near walls"));
            menuC.Add("useeflash", new CheckBox("Use flash to positioning", false));
            menuC.Add("useeflashforced",
                new KeyBind("Forced flash+E if possible", false, KeyBind.BindTypes.HoldActive, 'T'));
            menuC.Add("user", new CheckBox("Use R to maximize dmg"));
            menuC.Add("userindanger", new CheckBox("Use R in teamfight"));
            menuC.Add("useIgnite", new CheckBox("Use Ignite"));

            // Harass Settings
            menuH = config.AddSubMenu("Harass ", "Hsettings");
            menuH.Add("useqH", new CheckBox("Use Q"));
            menuH.Add("minmanaH", new Slider("Keep X% mana", 1, 1));

            // LaneClear Settings
            menuLC = config.AddSubMenu("Clear ", "Lcsettings");
            menuLC.Add("useqLC", new CheckBox("Use Q"));
            menuLC.Add("qMinHit", new Slider("   Q min hit", 3, 1, 6));
            menuLC.Add("useeLC", new CheckBox("Use E"));
            menuLC.Add("minmana", new Slider("Keep X% mana", 50, 1));

            // Misc Settings
            menuM = config.AddSubMenu("Misc", "Msettings");
            menuM.Add("useEint", new CheckBox("Use E interrupt"));
            menuM.Add("useEgap", new CheckBox("Use E on gapcloser near walls"));
            menuM.AddSeparator();
            menuM.AddGroupLabel("0 = Disabled");
            foreach (var hero in ObjectManager.Get<AIHeroClient>().Where(hero => hero.IsEnemy))
            {
                menuM.Add("useAutoW" + hero.BaseSkinName,
                    new Slider("Auto W : " + hero.BaseSkinName,
                        CombatHelper.DashDatas.Any(d => d.ChampionName == hero.ChampionName) ? 5 : 0, 0, 5));
            }

            config.Add("packets", new CheckBox("Use Packets", false));
        }
    }
}