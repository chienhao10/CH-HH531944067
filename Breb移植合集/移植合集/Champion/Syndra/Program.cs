using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using LeagueSharp.Common;
using SebbyLib;
using HealthPrediction = SebbyLib.HealthPrediction;
using Prediction = SebbyLib.Prediction.Prediction;
using PredictionInput = SebbyLib.Prediction.PredictionInput;
using Spell = LeagueSharp.Common.Spell;
using Utility = LeagueSharp.Common.Utility;

namespace OneKeyToWin_AIO_Sebby.Champions
{
    internal class Syndra
    {
        private static readonly Menu Config = Program.Config;
        private static Spell E, Q, R, W, EQ, Eany;
        private static float QMANA, WMANA, EMANA, RMANA;
        private static readonly List<Obj_AI_Minion> BallsList = new List<Obj_AI_Minion>();
        private static bool EQcastNow;

        public static Menu draw, q, w, e, r, harass, farm;

        public static AIHeroClient Player
        {
            get { return ObjectManager.Player; }
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

        public static void LoadOKTW()
        {
            Q = new Spell(SpellSlot.Q, 790);
            W = new Spell(SpellSlot.W, 950);
            E = new Spell(SpellSlot.E, 700);
            EQ = new Spell(SpellSlot.Q, Q.Range + 400);
            Eany = new Spell(SpellSlot.Q, Q.Range + 400);
            R = new Spell(SpellSlot.R, 675);

            Q.SetSkillshot(0.6f, 125f, float.MaxValue, false, SkillshotType.SkillshotCircle);
            W.SetSkillshot(0.25f, 140f, 1600f, false, SkillshotType.SkillshotCircle);
            E.SetSkillshot(0.25f, 100, 2500f, false, SkillshotType.SkillshotLine);
            EQ.SetSkillshot(0.6f, 100f, 2500f, false, SkillshotType.SkillshotLine);
            Eany.SetSkillshot(0.30f, 50f, 2500f, false, SkillshotType.SkillshotLine);

            draw = Config.AddSubMenu("Draw");
            draw.Add("qRange", new CheckBox("Q range", false));
            draw.Add("wRange", new CheckBox("W range", false));
            draw.Add("eRange", new CheckBox("E range", false));
            draw.Add("rRange", new CheckBox("R range", false));
            draw.Add("onlyRdy", new CheckBox("Draw when skill rdy"));

            q = Config.AddSubMenu("Q Config");
            q.Add("autoQ", new CheckBox("Auto Q"));
            q.Add("harrasQ", new CheckBox("Harass Q"));
            q.Add("QHarassMana", new Slider("Harass Mana", 30));

            w = Config.AddSubMenu("W Config");
            w.Add("autoW", new CheckBox("Auto W"));
            w.Add("harrasW", new CheckBox("Harass W"));

            e = Config.AddSubMenu("E Config");
            e.Add("autoE", new CheckBox("Auto Q + E combo, ks"));
            e.Add("harrasE", new CheckBox("Harass Q + E", false));
            e.Add("EInterrupter", new CheckBox("Auto Q + E Interrupter"));
            e.Add("useQE", new KeyBind("Semi-manual Q + E near mouse key", false, KeyBind.BindTypes.PressToggle, 'T'));
                //32 == space
            foreach (var enemy in ObjectManager.Get<AIHeroClient>().Where(enemy => enemy.IsEnemy))
                e.Add("Egapcloser" + enemy.NetworkId, new CheckBox("Q + E Gap : " + enemy.ChampionName));
            foreach (var enemy in ObjectManager.Get<AIHeroClient>().Where(enemy => enemy.IsEnemy))
                e.Add("Eon" + enemy.NetworkId, new CheckBox("Q + E :" + enemy.ChampionName));

            r = Config.AddSubMenu("R Config");
            r.Add("autoR", new CheckBox("Auto R KS"));
            r.Add("Rcombo", new CheckBox("Extra combo dmg calculation"));
            foreach (var enemy in ObjectManager.Get<AIHeroClient>().Where(enemy => enemy.IsEnemy))
                r.Add("Rmode" + enemy.NetworkId,
                    new ComboBox("Use on : " + enemy.ChampionName, 0, "KS", "Always", "Never"));

            harass = Config.AddSubMenu("Harass");
            foreach (var enemy in ObjectManager.Get<AIHeroClient>().Where(enemy => enemy.IsEnemy))
                harass.Add("harras" + enemy.NetworkId, new CheckBox(enemy.ChampionName));

            farm = Config.AddSubMenu("Farm");
            farm.Add("farmQout", new CheckBox("Last hit Q minion out range AA"));
            farm.Add("farmQ", new CheckBox("Lane clear Q"));
            farm.Add("farmW", new CheckBox("Lane clear W"));
            farm.Add("Mana", new Slider("LaneClear Mana", 80));
            farm.Add("LCminions", new Slider(" LaneClear minimum minions", 2, 0, 10));
            farm.Add("jungleQ", new CheckBox("Jungle clear Q"));
            farm.Add("jungleW", new CheckBox("Jungle clear W"));

            Game.OnUpdate += Game_OnGameUpdate;
            GameObject.OnCreate += Obj_AI_Base_OnCreate;
            Drawing.OnDraw += Drawing_OnDraw;
            Obj_AI_Base.OnProcessSpellCast += Obj_AI_Base_OnProcessSpellCast;
            Interrupter2.OnInterruptableTarget += Interrupter2_OnInterruptableTarget;
            AntiGapcloser.OnEnemyGapcloser += AntiGapcloser_OnEnemyGapcloser;
        }

        private static void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.IsMe && args.Slot == SpellSlot.Q && EQcastNow && E.IsReady())
            {
                var customeDelay = Q.Delay - (E.Delay + Player.LSDistance(args.End)/E.Speed);
                Utility.DelayAction.Add((int) (customeDelay*1000), () => E.Cast(args.End));
            }
        }

        private static void Interrupter2_OnInterruptableTarget(AIHeroClient sender,
            Interrupter2.InterruptableTargetEventArgs args)
        {
            if (E.IsReady() && getCheckBoxItem(e, "EInterrupter"))
            {
                if (sender.IsValidTarget(E.Range))
                {
                    E.Cast(sender.Position);
                }
                else if (Q.IsReady() && sender.IsValidTarget(EQ.Range))
                {
                    TryBallE(sender);
                }
            }
        }

        private static void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (E.IsReady() && getCheckBoxItem(e, "Egapcloser" + gapcloser.Sender.NetworkId))
            {
                if (Q.IsReady())
                {
                    EQcastNow = true;
                    Q.Cast(gapcloser.End);
                }
                else if (gapcloser.Sender.IsValidTarget(E.Range))
                {
                    E.Cast(gapcloser.Sender);
                }
            }
        }

        private static void Obj_AI_Base_OnCreate(GameObject sender, EventArgs args)
        {
            if (sender.IsAlly && sender.Type == GameObjectType.obj_AI_Minion && sender.Name == "Seed")
            {
                var ball = sender as Obj_AI_Minion;
                BallsList.Add(ball);
            }
        }

        private static void Game_OnGameUpdate(EventArgs args)
        {
            if (!E.IsReady())
                EQcastNow = false;

            if (Program.LagFree(1))
            {
                SetMana();
                BallCleaner();
                Jungle();
            }

            if (Program.LagFree(1) && E.IsReady() && getCheckBoxItem(e, "autoE"))
                LogicE();

            if (Program.LagFree(2) && Q.IsReady() && getCheckBoxItem(q, "autoQ"))
                LogicQ();

            if (Program.LagFree(3) && W.IsReady() && getCheckBoxItem(w, "autoW"))
                LogicW();

            if (Program.LagFree(4) && R.IsReady() && getCheckBoxItem(r, "autoR"))
                LogicR();
        }

        private static void TryBallE(AIHeroClient t)
        {
            if (Q.IsReady())
            {
                CastQE(t);
            }
            if (!EQcastNow)
            {
                var ePred = Eany.GetPrediction(t);
                if (ePred.Hitchance >= HitChance.VeryHigh)
                {
                    var playerToCP = Player.LSDistance(ePred.CastPosition);
                    foreach (var ball in BallsList.Where(ball => Player.LSDistance(ball.Position) < E.Range))
                    {
                        var ballFinalPos = Player.ServerPosition.Extend(ball.Position, playerToCP);
                        if (ballFinalPos.LSDistance(ePred.CastPosition) < 50)
                            E.Cast(ball.Position);
                    }
                }
            }
        }

        private static void LogicE()
        {
            if (getKeyBindItem(e, "useQE"))
            {
                var mouseTarget = Program.Enemies.Where(enemy =>
                    enemy.IsValidTarget(Eany.Range)).OrderBy(enemy => enemy.LSDistance(Game.CursorPos)).FirstOrDefault();

                if (mouseTarget != null)
                {
                    TryBallE(mouseTarget);
                    return;
                }
            }

            var t = TargetSelector.GetTarget(Eany.Range, DamageType.Magical);
            if (t.IsValidTarget())
            {
                if (OktwCommon.GetKsDamage(t, E) + Q.GetDamage(t) > t.Health)
                    TryBallE(t);
                if (Program.Combo && Player.Mana > RMANA + EMANA + QMANA && getCheckBoxItem(e, "Eon" + t.NetworkId))
                    TryBallE(t);
                if (Program.Farm && Player.Mana > RMANA + EMANA + QMANA + WMANA && getCheckBoxItem(e, "harrasE") &&
                    getCheckBoxItem(harass, "harras" + t.NetworkId))
                    TryBallE(t);
            }
        }

        private static void LogicR()
        {
            R.Range = R.Level == 3 ? 750 : 675;

            var Rcombo = getCheckBoxItem(r, "Rcombo");

            foreach (
                var enemy in Program.Enemies.Where(enemy => enemy.IsValidTarget(R.Range) && OktwCommon.ValidUlt(enemy)))
            {
                var Rmode = getBoxItem(r, "Rmode" + enemy.NetworkId);

                if (Rmode == 2)
                    continue;
                if (Rmode == 1)
                    R.Cast(enemy);

                var comboDMG = OktwCommon.GetKsDamage(enemy, R);
                comboDMG += R.GetDamage(enemy, 1)*(R.Instance.Ammo - 3);
                comboDMG += OktwCommon.GetEchoLudenDamage(enemy);

                if (Rcombo)
                {
                    if (Q.IsReady() && enemy.IsValidTarget(600))
                        comboDMG += Q.GetDamage(enemy);

                    if (E.IsReady())
                        comboDMG += E.GetDamage(enemy);

                    if (W.IsReady())
                        comboDMG += W.GetDamage(enemy);
                }

                if (enemy.Health < comboDMG)
                {
                    R.Cast(enemy);
                }
            }
        }

        private static void LogicW()
        {
            if (W.Instance.ToggleState == 1)
            {
                var t = TargetSelector.GetTarget(W.Range - 150, DamageType.Magical);
                if (t.IsValidTarget())
                {
                    if (Program.Combo && Player.Mana > RMANA + QMANA + WMANA)
                        CatchW(t);
                    else if (Program.Farm && getCheckBoxItem(w, "harrasW") &&
                             getCheckBoxItem(harass, "harras" + t.NetworkId)
                             && Player.ManaPercent > getSliderItem(q, "QHarassMana") && OktwCommon.CanHarras())
                    {
                        CatchW(t);
                    }
                    else if (OktwCommon.GetKsDamage(t, W) > t.Health)
                        CatchW(t);
                    else if (Player.Mana > RMANA + WMANA)
                    {
                        foreach (
                            var enemy in
                                Program.Enemies.Where(
                                    enemy => enemy.IsValidTarget(W.Range) && !OktwCommon.CanMove(enemy)))
                            CatchW(t);
                    }
                }
                else if (Program.LaneClear && !Q.IsReady() && Player.ManaPercent > getSliderItem(farm, "Mana") &&
                         getCheckBoxItem(farm, "farmW"))
                {
                    var allMinions = Cache.GetMinions(Player.ServerPosition, W.Range);
                    var farmPos = W.GetCircularFarmLocation(allMinions, W.Width);

                    if (farmPos.MinionsHit >= getSliderItem(farm, "LCminions"))
                        CatchW(allMinions.FirstOrDefault());
                }
            }
            else
            {
                var t = TargetSelector.GetTarget(W.Range, DamageType.Magical);
                if (t.IsValidTarget())
                {
                    Program.CastSpell(W, t);
                }
                else if (Program.LaneClear && getCheckBoxItem(farm, "farmW"))
                {
                    var allMinions = Cache.GetMinions(Player.ServerPosition, W.Range);
                    var farmPos = W.GetCircularFarmLocation(allMinions, W.Width);

                    if (farmPos.MinionsHit > 1)
                        W.Cast(farmPos.Position);
                }
            }
        }

        private static void LogicQ()
        {
            var t = TargetSelector.GetTarget(Q.Range, DamageType.Magical);
            if (t.IsValidTarget())
            {
                if (Program.Combo && Player.Mana > RMANA + QMANA + EMANA && !E.IsReady())
                    Program.CastSpell(Q, t);
                else if (Program.Farm && getCheckBoxItem(q, "harrasQ") &&
                         getCheckBoxItem(harass, "harras" + t.NetworkId) &&
                         Player.ManaPercent > getSliderItem(q, "QHarassMana") && OktwCommon.CanHarras())
                    Program.CastSpell(Q, t);
                else if (OktwCommon.GetKsDamage(t, Q) > t.Health)
                    Program.CastSpell(Q, t);
                else if (Player.Mana > RMANA + QMANA)
                {
                    foreach (
                        var enemy in
                            Program.Enemies.Where(
                                enemy => enemy.IsValidTarget(Q.Range) && !OktwCommon.CanMove(enemy)))
                        Program.CastSpell(Q, t);
                }
            }

            if (Orbwalker.IsAutoAttacking)
                return;

            if (!Program.None && !Program.Combo && Player.ManaPercent > getSliderItem(farm, "Mana"))
            {
                var allMinions = Cache.GetMinions(Player.ServerPosition, Q.Range);

                if (getCheckBoxItem(farm, "farmQout"))
                {
                    foreach (
                        var minion in
                            allMinions.Where(
                                minion =>
                                    minion.IsValidTarget(Q.Range) &&
                                    (!Player.IsInAutoAttackRange(minion) ||
                                     (!minion.UnderTurret(true) && minion.UnderTurret()))))
                    {
                        var hpPred = HealthPrediction.GetHealthPrediction(minion, 600);
                        if (hpPred < Q.GetDamage(minion) && hpPred > minion.Health - hpPred*2)
                        {
                            Q.Cast(minion);
                            return;
                        }
                    }
                }
                if (Program.LaneClear && getCheckBoxItem(farm, "farmQ"))
                {
                    var farmPos = Q.GetCircularFarmLocation(allMinions, Q.Width);
                    if (farmPos.MinionsHit >= getSliderItem(farm, "LCminions"))
                        Q.Cast(farmPos.Position);
                }
            }
        }

        private static void Jungle()
        {
            if (Program.LaneClear && Player.Mana > RMANA + QMANA)
            {
                var mobs = Cache.GetMinions(Player.ServerPosition, Q.Range, MinionTeam.Neutral);
                if (mobs.Count > 0)
                {
                    var mob = mobs[0];
                    if (Q.IsReady() && getCheckBoxItem(farm, "jungleQ"))
                    {
                        Q.Cast(mob.ServerPosition);
                    }
                    else if (W.IsReady() && getCheckBoxItem(farm, "jungleW") &&
                             Utils.TickCount - Q.LastCastAttemptT > 900)
                    {
                        W.Cast(mob.ServerPosition);
                    }
                }
            }
        }

        private static void CastQE(Obj_AI_Base target)
        {
            var CoreType2 = SebbyLib.Prediction.SkillshotType.SkillshotLine;

            var predInput2 = new PredictionInput
            {
                Aoe = false,
                Collision = EQ.Collision,
                Speed = EQ.Speed,
                Delay = EQ.Delay,
                Range = EQ.Range,
                From = Player.ServerPosition,
                Radius = EQ.Width,
                Unit = target,
                Type = CoreType2
            };

            var poutput2 = Prediction.GetPrediction(predInput2);

            if (OktwCommon.CollisionYasuo(Player.ServerPosition, poutput2.CastPosition))
                return;

            var castQpos = poutput2.CastPosition;

            if (Player.LSDistance(castQpos) > Q.Range)
                castQpos = Player.Position.LSExtend(castQpos, Q.Range);

            if (Program.getSliderItem("HitChance") == 0)
            {
                if (poutput2.Hitchance >= SebbyLib.Prediction.HitChance.VeryHigh)
                {
                    EQcastNow = true;
                    Q.Cast(castQpos);
                }
            }
            else if (Program.getSliderItem("HitChance") == 1)
            {
                if (poutput2.Hitchance >= SebbyLib.Prediction.HitChance.High)
                {
                    EQcastNow = true;
                    Q.Cast(castQpos);
                }
            }
            else if (Program.getSliderItem("HitChance") == 2)
            {
                if (poutput2.Hitchance >= SebbyLib.Prediction.HitChance.Medium)
                {
                    EQcastNow = true;
                    Q.Cast(castQpos);
                }
            }
        }

        private static void Drawing_OnDraw(EventArgs args)
        {
            if (getCheckBoxItem(draw, "qRange"))
            {
                if (getCheckBoxItem(draw, "onlyRdy"))
                {
                    if (Q.IsReady())
                        Utility.DrawCircle(ObjectManager.Player.Position, Q.Range, Color.Cyan, 1, 1);
                }
                else
                    Utility.DrawCircle(ObjectManager.Player.Position, Q.Range, Color.Cyan, 1, 1);
            }
            if (getCheckBoxItem(draw, "wRange"))
            {
                if (getCheckBoxItem(draw, "onlyRdy"))
                {
                    if (W.IsReady())
                        Utility.DrawCircle(ObjectManager.Player.Position, W.Range, Color.Orange, 1, 1);
                }
                else
                    Utility.DrawCircle(ObjectManager.Player.Position, W.Range, Color.Orange, 1, 1);
            }
            if (getCheckBoxItem(draw, "eRange"))
            {
                if (getCheckBoxItem(draw, "onlyRdy"))
                {
                    if (E.IsReady())
                        Utility.DrawCircle(ObjectManager.Player.Position, EQ.Range, Color.Yellow, 1, 1);
                }
                else
                    Utility.DrawCircle(ObjectManager.Player.Position, EQ.Range, Color.Yellow, 1, 1);
            }
            if (getCheckBoxItem(draw, "rRange"))
            {
                if (getCheckBoxItem(draw, "onlyRdy"))
                {
                    if (R.IsReady())
                        Utility.DrawCircle(ObjectManager.Player.Position, R.Range, Color.Gray, 1, 1);
                }
                else
                    Utility.DrawCircle(ObjectManager.Player.Position, R.Range, Color.Gray, 1, 1);
            }
        }

        private static void SetMana()
        {
            if ((Program.getCheckBoxItem("manaDisable") && Program.Combo) || Player.HealthPercent < 20)
            {
                QMANA = 0;
                WMANA = 0;
                EMANA = 0;
                RMANA = 0;
                return;
            }

            QMANA = Q.Instance.SData.Mana;
            WMANA = W.Instance.SData.Mana;
            EMANA = E.Instance.SData.Mana;

            if (!R.IsReady())
                RMANA = QMANA - Player.PARRegenRate*Q.Instance.Cooldown;
            else
                RMANA = R.Instance.SData.Mana;
        }

        private static void BallCleaner()
        {
            if (BallsList.Count > 0)
            {
                BallsList.RemoveAll(ball => !ball.IsValid || ball.Mana == 19);
            }
        }

        private static void CatchW(Obj_AI_Base t, bool onlyMinin = false)
        {
            if (Utils.TickCount - W.LastCastAttemptT < 150)
                return;

            var catchRange = 925;
            Obj_AI_Base obj = null;
            if (BallsList.Count > 0 && !onlyMinin)
            {
                obj = BallsList.Find(ball => ball.LSDistance(Player) < catchRange);
            }
            if (obj == null)
            {
                obj =
                    MinionManager.GetMinions(Player.ServerPosition, catchRange, MinionTypes.All, MinionTeam.NotAlly,
                        MinionOrderTypes.MaxHealth).FirstOrDefault();
            }

            if (obj != null)
            {
                foreach (
                    var minion in
                        MinionManager.GetMinions(Player.ServerPosition, catchRange, MinionTypes.All, MinionTeam.NotAlly,
                            MinionOrderTypes.MaxHealth))
                {
                    if (t.LSDistance(minion) < t.LSDistance(obj))
                        obj = minion;
                }

                W.Cast(obj.Position);
            }
        }
    }
}