using System;
using System.Collections.Generic;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using LeagueSharp.Common;
using SharpDX;
using UnderratedAIO.Helpers;
using Color = System.Drawing.Color;
using Damage = LeagueSharp.Common.Damage;
using Environment = System.Environment;
using Prediction = LeagueSharp.Common.Prediction;
using Spell = LeagueSharp.Common.Spell;
using Utility = LeagueSharp.Common.Utility;

namespace UnderratedAIO.Champions
{
    internal class Rumble
    {
        public static Menu config;
        public static Spell Q, W, E, R;
        public static readonly AIHeroClient player = ObjectManager.Player;
        public static bool justE;
        public static IncomingDamage IncDamages;

        public static Menu menuC, menuD, menuH, menuLC, menuM;
        public float lastE;

        public Rumble()
        {
            IncDamages = new IncomingDamage();
            InitRumble();
            InitMenu();
            Drawing.OnDraw += Game_OnDraw;
            Game.OnUpdate += Game_OnGameUpdate;
            Obj_AI_Base.OnProcessSpellCast += Game_ProcessSpell;
            CustomEvents.Unit.OnDash += Unit_OnDash;
            AntiGapcloser.OnEnemyGapcloser += OnEnemyGapcloser;
        }

        private static bool Enhanced
        {
            get { return player.Mana >= 50 && player.Mana < 100; }
        }


        private static bool Silenced
        {
            get { return player.HasBuff("rumbleoverheat"); }
        }

        private static bool ActiveQ
        {
            get { return player.HasBuff("RumbleFlameThrower"); }
        }

        private static bool ActiveE
        {
            get { return player.HasBuff("RumbleGrenade"); }
        }

        private void OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (getCheckBoxItem(menuM, "usewgc") && gapcloser.End.LSDistance(player.Position) < 200)
            {
                W.Cast();
            }
        }

        private void Unit_OnDash(Obj_AI_Base sender, Dash.DashItem args)
        {
            if (sender.IsEnemy && getCheckBoxItem(menuM, "useegc") && sender is AIHeroClient &&
                args.EndPos.LSDistance(player.Position) < E.Range && E.CanCast(sender))
            {
                Utility.DelayAction.Add(args.Duration, () => { E.Cast(args.EndPos); });
            }
        }

        private void InitRumble()
        {
            Q = new Spell(SpellSlot.Q, 500);
            W = new Spell(SpellSlot.W);
            E = new Spell(SpellSlot.E, 950);
            E.SetSkillshot(0.25f, 70, 1200, true, SkillshotType.SkillshotLine);
            R = new Spell(SpellSlot.R, 1700);
            R.SetSkillshot(0.4f, 130, 2500, false, SkillshotType.SkillshotLine);
        }

        private void Game_OnGameUpdate(EventArgs args)
        {
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                Combo();
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass))
            {
                Harass();
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear) ||
                Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear))
            {
                Clear();
            }

            var data = IncDamages.GetAllyData(player.NetworkId);
            if (data != null && W.IsReady() && getCheckBoxItem(menuM, "usew") &&
                (preventSilence(W) || (!getCheckBoxItem(menuM, "blockW") && !preventSilence(W))) &&
                (data.DamageTaken > getShield()*getSliderItem(menuM, "shieldPercent")/100 ||
                 getSliderItem(menuM, "Aggro") <= data.DamageCount))
            {
                W.Cast();
            }
            if (getKeyBindItem(menuC, "castR"))
            {
                var target = TargetSelector.GetTarget(1700, DamageType.Magical);
                if (target != null)
                {
                    HandleR(target, true);
                }
            }
        }


        private void Harass()
        {
            var target = TargetSelector.GetTarget(1300, DamageType.Magical);
            if (target == null || target.IsInvulnerable)
            {
                return;
            }
            if (Qhit(target.Position) && getCheckBoxItem(menuH, "useqH") && preventSilence(Q))
            {
                Q.Cast(target.Position);
            }
            if (E.CanCast(target) && getCheckBoxItem(menuH, "useeH") && preventSilence(E) &&
                (!ActiveE ||
                 Environment.TickCount - lastE > getSliderItem(menuH, "HeDelay") ||
                 getEdamage(target) > target.Health))
            {
                E.CastIfHitchanceEquals(target, HitChance.VeryHigh, getCheckBoxItem(config, "packets"));
            }
        }

        private static float getEdamage(AIHeroClient target)
        {
            if (!E.IsReady())
            {
                return 0;
            }
            var num = ActiveE ? 1 : 2;
            var dmg = player.LSGetSpellDamage(target, SpellSlot.E)*num;
            return (float) (Enhanced ? dmg*1.5f : dmg);
        }

        private static float getQdamage(AIHeroClient target)
        {
            if (!Q.IsReady() && !ActiveQ)
            {
                return 0;
            }
            var dmg = player.LSGetSpellDamage(target, SpellSlot.Q);
            return (float) (Enhanced ? dmg*1.5f : dmg);
        }

        private double getShield()
        {
            return new double[] {50, 80, 110, 140, 170}[W.Level - 1] + 0.4f*player.TotalMagicalDamage;
        }

        private static float getRdamage()
        {
            var dmg = new double[] {130, 185, 240}[R.Level - 1] + 0.3f*player.TotalMagicalDamage;
            return (float) dmg;
        }

        private bool Qhit(Vector3 target)
        {
            return Q.IsReady() && CombatHelper.IsFacing(player, target, 80) &&
                   target.LSDistance(player.Position) < Q.Range;
        }

        private void Clear()
        {
            if (Q.IsReady() && getCheckBoxItem(menuLC, "useqLC") && preventSilence(Q))
            {
                var minons = MinionManager.GetMinions(player.Position, Q.Range, MinionTypes.All, MinionTeam.NotAlly);
                if (minons.Count(m => Qhit(m.Position)) >= getSliderItem(menuLC, "qMinHit"))
                {
                    Q.Cast(Game.CursorPos);
                }
            }
        }

        private bool preventSilence(Spell spell)
        {
            if (spell.Slot == SpellSlot.E && ActiveE)
            {
                return true;
            }
            return 20 + player.Mana < 100;
        }

        private void Combo()
        {
            var target = TargetSelector.GetTarget(1700, DamageType.Magical);
            if (target == null || target.IsInvulnerable || target.MagicImmune)
            {
                return;
            }
            var edmg = getEdamage(target);
            var qdmg = getQdamage(target);
            var ignitedmg = (float) player.GetSummonerSpellDamage(target, Damage.SummonerSpell.Ignite);
            var hasIgnite = player.Spellbook.CanUseSpell(player.GetSpellSlot("SummonerDot")) == SpellState.Ready;
            if (getCheckBoxItem(menuC, "useIgnite") &&
                ignitedmg > HealthPrediction.GetHealthPrediction(target, 700) && hasIgnite &&
                !CombatHelper.CheckCriticalBuffs(target) &&
                (!ActiveQ ||
                 !(CombatHelper.IsFacing(player, target.Position, 30) && target.LSDistance(player) < Q.Range)))
            {
                player.Spellbook.CastSpell(player.GetSpellSlot("SummonerDot"), target);
            }
            if (Q.CanCast(target) && getCheckBoxItem(menuC, "useq") && Qhit(target.Position) &&
                (preventSilence(Q) ||
                 (target.Health < PassiveDmg(target)*2 || qdmg > target.Health) &&
                 target.LSDistance(player) < Orbwalking.GetRealAutoAttackRange(target)))
            {
                Q.Cast(target.Position);
            }
            if (getCheckBoxItem(menuC, "usee") && E.CanCast(target) &&
                (((preventSilence(E) ||
                   (target.Health < PassiveDmg(target)*2 &&
                    target.LSDistance(player) < Orbwalking.GetRealAutoAttackRange(target))) &&
                  (!ActiveE ||
                   Environment.TickCount - lastE > getSliderItem(menuC, "eDelay"))) ||
                 edmg > target.Health))
            {
                E.CastIfHitchanceEquals(target, HitChance.High, getCheckBoxItem(config, "packets"));
            }
            if (W.IsReady() && getCheckBoxItem(menuC, "wSpeed") && ActiveQ && preventSilence(W) &&
                target.LSDistance(player) < Q.Range &&
                Prediction.GetPrediction(target, 0.2f).UnitPosition.LSDistance(player.Position) > Q.Range)
            {
                W.Cast();
            }
            var canR = ComboDamage(target) > target.Health && qdmg < target.Health && target.LSDistance(player) < Q.Range &&
                       !Silenced;
            if (R.IsReady() &&
                ((target.Health <
                  getRdamage()*(target.CountAlliesInRange(600) > 0 && target.HealthPercent > 15 ? 5 : 3) &&
                  target.LSDistance(player) > Q.Range) ||
                 (target.LSDistance(player) < Q.Range && target.Health < getRdamage()*3 + edmg &&
                  target.Health > qdmg) ||
                 player.CountEnemiesInRange(R.Range) >= getSliderItem(menuC, "Rmin")))
            {
                HandleR(target, canR);
            }
        }

        private void HandleR(Obj_AI_Base target, bool manual = false)
        {
            var targE = R.GetPrediction(target);
            if ((getCheckBoxItem(menuC, "user") && player.CountEnemiesInRange(R.Range + 175) <= 1) || manual)
            {
                if (target.IsMoving)
                {
                    var pos = targE.CastPosition;
                    if (pos.IsValid() && pos.LSDistance(player.Position) < R.Range + 1000 &&
                        targE.Hitchance >= HitChance.VeryHigh)
                    {
                        R.Cast(target.Position.LSExtend(pos, -target.MoveSpeed), pos);
                    }
                }
                else
                {
                    R.Cast(target.Position.LSExtend(player.Position, 500), target.Position);
                }
            }
            else if (targE.Hitchance >= HitChance.VeryHigh)
            {
                var pred = getBestRVector3(target, targE);
                if (pred != Vector3.Zero &&
                    CombatHelper.GetCollisionCount(
                        target, target.Position.LSExtend(pred, 1000), R.Width, new[] {CollisionableObjects.Heroes}) >=
                    getSliderItem(menuC, "Rmin"))
                {
                    R.Cast(target.Position.LSExtend(pred, -target.MoveSpeed), pred);
                }
            }
        }

        private Vector3 getBestRVector3(Obj_AI_Base target, PredictionOutput targE)
        {
            var otherHeroes =
                HeroManager.Enemies.Where(
                    e => e.IsValidTarget() && e.NetworkId != target.NetworkId && player.LSDistance(e) < 1000)
                    .Select(e => R.GetPrediction(e))
                    .Where(o => o.Hitchance > HitChance.High && o.CastPosition.LSDistance(targE.UnitPosition) < 1000);
            if (otherHeroes.Any())
            {
                var best =
                    otherHeroes.OrderByDescending(
                        hero =>
                            CombatHelper.GetCollisionCount(
                                target, target.Position.LSExtend(hero.CastPosition, 1000), R.Width,
                                new[] {CollisionableObjects.Heroes})).FirstOrDefault();
                if (best != null)
                {
                    return best.CastPosition;
                }
            }
            return Vector3.Zero;
        }

        private void Game_OnDraw(EventArgs args)
        {
            DrawHelper.DrawCircle(getCheckBoxItem(menuD, "drawqq"), Q.Range, Color.FromArgb(180, 100, 146, 166));
            DrawHelper.DrawCircle(getCheckBoxItem(menuD, "drawee"), E.Range, Color.FromArgb(180, 100, 146, 166));
        }

        private static float ComboDamage(AIHeroClient hero)
        {
            double damage = 0;
            if (Q.IsReady())
            {
                damage += getQdamage(hero);
            }
            if (E.IsReady())
            {
                damage += getEdamage(hero);
            }
            if (R.IsReady())
            {
                damage += getRdamage()*4;
            }
            //damage += ItemHandler.GetItemsDamage(target);
            var ignitedmg = player.GetSummonerSpellDamage(hero, Damage.SummonerSpell.Ignite);
            if (player.Spellbook.CanUseSpell(player.GetSpellSlot("summonerdot")) == SpellState.Ready &&
                hero.Health < damage + ignitedmg)
            {
                damage += ignitedmg;
            }
            return (float) damage;
        }


        private static float PassiveDmg(Obj_AI_Base hero)
        {
            return
                (float)
                    (player.CalcDamage(hero, DamageType.Magical, 20 + 5*player.Level + player.TotalMagicalDamage*0.3f) +
                     player.GetAutoAttackDamage(hero));
        }

        private void Game_ProcessSpell(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.IsMe)
            {
                if (args.SData.Name == "RumbleGrenade")
                {
                    var dist = player.LSDistance(args.End);
                    justE = true;
                    Utility.DelayAction.Add(
                        (int) ((dist > E.Range ? E.Range : dist)/E.Speed*1000), () => justE = false);
                    lastE = Environment.TickCount;
                }
            }
            if (getCheckBoxItem(menuM, "usew") && sender is AIHeroClient && sender.IsEnemy &&
                player.LSDistance(sender) < Q.Range && IncDamages.GetAllyData(player.NetworkId).AnyCC)
            {
                W.Cast();
            }
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

        private void InitMenu()
        {
            config = MainMenu.AddMenu("Rumble ", "Rumble");

            // Draw settings
            menuD = config.AddSubMenu("Drawings ", "dsettings");
            menuD.Add("drawqq", new CheckBox("Draw Q range"));
                //.SetValue(new Circle(false, Color.FromArgb(180, 100, 146, 166)));
            menuD.Add("drawee", new CheckBox("Draw E range"));
                //.SetValue(new Circle(false, Color.FromArgb(180, 100, 146, 166)));

            // Combo Settings 
            menuC = config.AddSubMenu("Combo ", "csettings");
            menuC.Add("useq", new CheckBox("Use Q"));
            menuC.Add("usee", new CheckBox("Use E"));
            menuC.Add("eDelay", new Slider("   Delay between E", 2000, 0, 2990));
            menuC.Add("wSpeed", new CheckBox("Use W to speed up"));
            menuC.Add("user", new CheckBox("Use R 1v1"));
            menuC.Add("Rmin", new Slider("Use R teamfigh", 2, 1, 5));
            menuC.Add("castR", new KeyBind("R manual cast", false, KeyBind.BindTypes.HoldActive, 'T'));
            menuC.Add("useIgnite", new CheckBox("Use Ignite"));

            // Harass Settings
            menuH = config.AddSubMenu("Harass ", "Hsettings");
            menuH.Add("useqH", new CheckBox("Use Q"));
            menuH.Add("useeH", new CheckBox("Use E"));
            menuH.Add("HeDelay", new Slider("   Delay between E", 1500, 0, 2990));

            // LaneClear Settings
            menuLC = config.AddSubMenu("LaneClear ", "Lcsettings");
            menuLC.Add("useqLC", new CheckBox("Use Q"));
            menuLC.Add("qMinHit", new Slider("   Min hit", 3, 1, 6));

            // MISC
            menuM = config.AddSubMenu("Misc ", "Msettings");
            menuM.Add("usewgc", new CheckBox("Use W gapclosers", false));
            menuM.Add("useegc", new CheckBox("Use E gapclosers"));
            menuM.Add("usew", new CheckBox("Use W to shield"));
            menuM.Add("shieldPercent", new Slider("   Shield %", 50));
            menuM.Add("Aggro", new Slider("   Aggro", 3, 0, 10));
            menuM.Add("blockW", new CheckBox("   Don't silence me pls"));

            config.Add("packets", new CheckBox("Use Packets", false));
        }
    }
}