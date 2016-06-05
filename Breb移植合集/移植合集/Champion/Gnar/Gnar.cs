using System;
using System.Collections.Generic;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using LeagueSharp.Common;
using SharpDX;
using Color = System.Drawing.Color;
using Prediction = LeagueSharp.Common.Prediction;
using Utility = LeagueSharp.Common.Utility;

namespace Slutty_Gnar_Reworked
{
    internal class Gnar : MenuConfig
    {
        private static readonly AIHeroClient Player = ObjectManager.Player;
        public static int rcast { get; set; }
        public static int lastq { get; set; }

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

        public static void OnLoad()
        {
            #region OnLoad

            if (Player.ChampionName != "Gnar")
                return;
            CreateMenu();
            Game.OnUpdate += Game_OnUpdate;
            CustomEvents.Unit.OnDash += Unit_OnDash;
            Orbwalker.OnPostAttack += AfterAttack;
            Drawing.OnDraw += Drawing_OnDraw;
            Interrupter2.OnInterruptableTarget += GnarInterruptableSpell;
            AntiGapcloser.OnEnemyGapcloser += OnGapCloser;

            #endregion
        }

        private static void GnarInterruptableSpell(AIHeroClient sender, Interrupter2.InterruptableTargetEventArgs args)
        {
            #region Interruptable

            if (Player.IsMegaGnar())
            {
                if (GnarSpells.WMega.IsReady()
                    && GnarSpells.WMega.IsInRange(sender)
                    && getCheckBoxItem(miscMenu, "qwi"))
                    GnarSpells.WMega.Cast(sender.ServerPosition);
            }

            #endregion
        }

        private static void OnGapCloser(ActiveGapcloser gapcloser)
        {
            #region on gap closer

            if (gapcloser.Sender.IsAlly
                || gapcloser.Sender.IsMe)
                return;
            if (Player.IsMiniGnar() && getCheckBoxItem(miscMenu, "qgap"))
            {
                if (GnarSpells.QMini.IsInRange(gapcloser.Start))
                    GnarSpells.QMini.Cast(gapcloser.Sender.Position);
            }

            #endregion
        }

        private static void Drawing_OnDraw(EventArgs args)
        {
            #region Drawings

            if (!Player.Position.IsOnScreen())
                return;

            var qDraw = getCheckBoxItem(drawMenu, "qDraw");
            var eDraw = getCheckBoxItem(drawMenu, "eDraw");
            var wDraw = getCheckBoxItem(drawMenu, "wDraw");
            var draw = getCheckBoxItem(drawMenu, "Draw");

            if (Player.IsDead || !draw)
                return;

            if (Player.IsMegaGnar())
            {
                if (qDraw && GnarSpells.QMega.Level > 0)
                    Render.Circle.DrawCircle(Player.Position, GnarSpells.QMega.Range, Color.Green, 3);
                if (eDraw && GnarSpells.EMega.Level > 0)
                    Render.Circle.DrawCircle(Player.Position, GnarSpells.EMega.Range, Color.Gold, 3);
                if (wDraw && GnarSpells.WMega.Level > 0)
                    Render.Circle.DrawCircle(Player.Position, GnarSpells.WMega.Range, Color.Black, 3);
            }

            if (Player.IsMiniGnar())
            {
                if (qDraw && GnarSpells.QMini.Level > 0)
                    Render.Circle.DrawCircle(Player.Position, GnarSpells.QMini.Range, Color.Green, 3);
                if (eDraw && GnarSpells.EMini.Level > 0)
                    Render.Circle.DrawCircle(Player.Position, GnarSpells.EMini.Range, Color.LightBlue, 3);
            }

            #endregion
        }

        private static void AfterAttack(AttackableUnit target, EventArgs args)
        {
            #region After attack

            if (!Player.IsMiniGnar())
                return;

            var targets = TargetSelector.GetTarget(GnarSpells.QMini.Range, DamageType.Physical);
            if (Player.LSDistance(target) <= 450)
            {
                if (GnarSpells.QnMini.IsReady())
                    GnarSpells.QnMini.Cast(targets);
            }

            #endregion
        }

        private static void Unit_OnDash(Obj_AI_Base sender, Dash.DashItem args)
        {
            #region dash

            if (!getCheckBoxItem(miscMenu, "qwd"))
            {
                return;
            }

            var useQ = getCheckBoxItem(comboMenu, "UseQMini");
            var useQm = getCheckBoxItem(comboMenu, "UseQMega");
            var target = TargetSelector.GetTarget(GnarSpells.QMini.Range, DamageType.Magical);

            if (!sender.IsEnemy || sender == null)
            {
                return;
            }

            if (sender.NetworkId == target.NetworkId
                && Player.IsMiniGnar())
            {
                if (useQ
                    && GnarSpells.QMini.IsReady()
                    && args.EndPos.LSDistance(Player) <= GnarSpells.QMini.Range)
                {
                    var delay = (int) (args.EndTick - Game.Time - GnarSpells.QMini.Delay - 0.1f);
                    if (delay > 0)
                    {
                        Utility.DelayAction.Add(delay*1000, () => GnarSpells.QMini.Cast(args.EndPos));
                    }
                    else
                    {
                        GnarSpells.QMini.Cast(args.EndPos);
                    }
                }
                if (sender.NetworkId == target.NetworkId
                    && Player.IsMegaGnar())
                {
                    if (useQm
                        && GnarSpells.QMini.IsReady()
                        && args.EndPos.LSDistance(Player) <= GnarSpells.QMega.Range)
                    {
                        var delay = (int) (args.EndTick - Game.Time - GnarSpells.QMega.Delay - 0.1f);
                        if (delay > 0)
                        {
                            Utility.DelayAction.Add(delay*1000, () => GnarSpells.QMega.Cast(args.EndPos));
                        }
                        else
                        {
                            GnarSpells.QMega.Cast(args.EndPos);
                        }
                    }
                }
            }
        }

        #endregion

        private static void Game_OnUpdate(EventArgs args)
        {
            #region On Update

            KillSteal();

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                Combo();
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear) ||
                Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear))
            {
                LaneClear();
                JungleClear();
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass))
            {
                Mixed();
            }

            if (getKeyBindItem(miscMenu, "fleekey") || Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Flee))
                Flee();

            #region force target

            var qSpell = getCheckBoxItem(comboMenu, "focust");
            var target = HeroManager.Enemies.Find(en => en.IsValidTarget(ObjectManager.Player.AttackRange)
                                                        &&
                                                        en.Buffs.Any(buff => buff.Name == "gnarwproc" && buff.Count == 2));
            if (qSpell && target != null)
            {
                Orbwalker.ForcedTarget = target;
            }
            else
            {
                Orbwalker.ForcedTarget = null;
            }

            #endregion

            #region Auto Q

            var autoQ = getCheckBoxItem(harassMenu, "autoq");
            if (autoQ && target != null)
                GnarSpells.QMini.Cast(target);

            #endregion

            #endregion
        }

        private static void Flee()
        {
            #region flee

            EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
            if (Player.IsMiniGnar())
            {
                var minionCount = MinionManager.GetMinions(Player.Position, GnarSpells.QMini.Range, MinionTypes.All,
                    MinionTeam.All);
                foreach (var minion in minionCount)
                {
                    var minionPrediction = GnarSpells.EMini.GetPrediction(minion);

                    var k =
                        ObjectManager.Get<Obj_AI_Minion>().Where(x => Player.IsFacing(x)
                                                                      && x.IsMinion
                                                                      && x.LSDistance(Player) <= GnarSpells.EMini.Range)
                            .OrderByDescending(x => x.LSDistance(Player))
                            .First();

                    if (k == null)
                        return;
                    var edm = Player.ServerPosition.LSExtend(minionPrediction.CastPosition,
                        Player.ServerPosition.LSDistance(minionPrediction.CastPosition) + GnarSpells.EMini.Range);
                    if (!ObjectManager.Get<Obj_AI_Turret>().Any(type => type.IsMinion
                                                                        && !type.IsDead
                                                                        && type.LSDistance(edm, true) < 775*775))
                    {
                        GnarSpells.EMini.Cast(edm.Extend(Game.CursorPos,
                            Player.ServerPosition.LSDistance(minionPrediction.CastPosition) + GnarSpells.EMini.Range));
                    }
                }
            }
        }

        #endregion

        private static void KillSteal()
        {
            #region Kill Steal

            var target = TargetSelector.GetTarget(GnarSpells.QMini.Range, DamageType.Physical);

            if (target == null)
                return;

            var qSpell = getCheckBoxItem(ksMenu, "qks");
            var rSpell = getCheckBoxItem(ksMenu, "rks");
            var eqSpell = getCheckBoxItem(ksMenu, "qeks");

            if (qSpell)
            {
                if (Player.IsMiniGnar())
                {
                    if (GnarSpells.QMini.IsReady()
                        && target.IsValidTarget(GnarSpells.QMini.Range - 30)
                        && target.Health <= GnarSpells.QMini.GetDamage(target))
                        GnarSpells.QMini.Cast(target);
                }
                if (Player.IsMegaGnar())
                {
                    if (GnarSpells.QMega.IsReady()
                        && target.IsValidTarget(GnarSpells.QMega.Range - 30)
                        && target.Health <= GnarSpells.QMega.GetDamage(target))
                        GnarSpells.QMega.Cast(target);
                }
            }

            if (rSpell)
            {
                if (Player.IsMegaGnar()
                    && GnarSpells.RMega.IsReady()
                    && target.Health <= GnarSpells.RMega.GetDamage(target))
                    GnarSpells.RMega.Cast(target);
            }

            if (eqSpell
                && Player.IsMiniGnar()
                && Player.LSDistance(target) > 1400)
            {
                var prediction = GnarSpells.EMini.GetPrediction(target);
                var ed = Player.ServerPosition.Extend(prediction.CastPosition,
                    Player.ServerPosition.LSDistance(prediction.CastPosition) + GnarSpells.EMini.Range);

                if (!ObjectManager.Get<Obj_AI_Turret>().Any(type => type.Team != Player.Team
                                                                    && !type.IsDead
                                                                    && type.LSDistance(ed, true) < 775*775))
                {
                    GnarSpells.EMini.Cast(prediction.CastPosition);
                    lastq = Environment.TickCount;
                }

                var minionCount = MinionManager.GetMinions(Player.Position, GnarSpells.QMini.Range, MinionTypes.All,
                    MinionTeam.All);
                foreach (var minion in minionCount)
                {
                    var minionPrediction = GnarSpells.EMini.GetPrediction(minion);
                    var k =
                        ObjectManager.Get<Obj_AI_Minion>().Where(x => Player.IsFacing(x)
                                                                      && x.IsMinion
                                                                      && x.LSDistance(Player) <= GnarSpells.EMini.Range)
                            .OrderByDescending(x => x.LSDistance(Player))
                            .First();

                    var edm = Player.ServerPosition.Extend(minionPrediction.CastPosition,
                        Player.ServerPosition.LSDistance(minionPrediction.CastPosition) + GnarSpells.EMini.Range);
                    if (!ObjectManager.Get<Obj_AI_Turret>().Any(type => type.IsMinion != Player.IsMinion
                                                                        && !type.IsDead
                                                                        && type.LSDistance(edm, true) < 775*775
                                                                        && k.IsValid))
                    {
                        GnarSpells.EMini.Cast(k);
                        lastq = Environment.TickCount;
                    }
                }
                if (GnarSpells.QMini.IsReady()
                    && Environment.TickCount - lastq > 500)
                {
                    GnarSpells.QMini.Cast(target);
                }
            }

            #endregion
        }

        private static void JungleClear()
        {
            #region Jungle Clear

            var qSpell = getCheckBoxItem(clearMenu, "UseQj");
            var wSpell = getCheckBoxItem(clearMenu, "UseWj");

            var jungle = MinionManager.GetMinions(GnarSpells.QMini.Range, MinionTypes.All, MinionTeam.Neutral,
                MinionOrderTypes.MaxHealth);

            foreach (var jungleminion in jungle)
            {
                if (!jungleminion.IsValidTarget())
                    return;

                if (Player.IsMiniGnar())
                {
                    if (qSpell && GnarSpells.QMini.IsReady())
                        GnarSpells.QMini.Cast(jungleminion);
                }
                if (Player.IsMegaGnar())
                {
                    if (wSpell && GnarSpells.WMega.IsReady())
                        GnarSpells.WMega.Cast(jungleminion);
                }
            }

            #endregion
        }

        private static void LaneClear()
        {
            #region laneclear

            var qSpell = getCheckBoxItem(clearMenu, "UseQl");
            var wSpell = getCheckBoxItem(clearMenu, "UseWl");
            var qlSpell = getSliderItem(clearMenu, "UseQlslider");
            var wlSpell = getSliderItem(clearMenu, "UseWlslider");

            var minions = MinionManager.GetMinions(GnarSpells.QMini.Range, MinionTypes.All, MinionTeam.Enemy,
                MinionOrderTypes.MaxHealth);
            if (minions == null)
                return;

            var QFarmLocation =
                GnarSpells.QMini.GetLineFarmLocation(
                    MinionManager.GetMinionsPredictedPositions(MinionManager.GetMinions(GnarSpells.QMini.Range),
                        GnarSpells.QMini.Delay, GnarSpells.QMini.Width, GnarSpells.QMini.Speed,
                        Player.Position, GnarSpells.QMini.Range,
                        false, SkillshotType.SkillshotLine), GnarSpells.QMini.Width);

            var WFarmLocation =
                GnarSpells.WMega.GetLineFarmLocation(
                    MinionManager.GetMinionsPredictedPositions(MinionManager.GetMinions(GnarSpells.WMega.Range),
                        GnarSpells.WMega.Delay, GnarSpells.WMega.Width, GnarSpells.WMega.Speed,
                        Player.Position, GnarSpells.WMega.Range,
                        false, SkillshotType.SkillshotLine), GnarSpells.WMega.Width);

            if (Player.IsMiniGnar())
            {
                if (qSpell && GnarSpells.QMini.IsReady() && QFarmLocation.MinionsHit > qlSpell)
                    GnarSpells.QMini.Cast(QFarmLocation.Position);
            }
            if (Player.IsMegaGnar())
            {
                if (wSpell && GnarSpells.WMega.IsReady() && WFarmLocation.MinionsHit > wlSpell)
                    GnarSpells.WMega.Cast(WFarmLocation.Position);
                if (qSpell && GnarSpells.QMega.IsReady())
                    GnarSpells.QMega.Cast(minions[0]);
            }

            #endregion
        }

        private static void Mixed()
        {
            #region mixed

            var target = TargetSelector.GetTarget(GnarSpells.QMini.Range, DamageType.Physical);
            var qSpell = getCheckBoxItem(harassMenu, "qharras");
            var qsSpell = getCheckBoxItem(harassMenu, "qharras2");
            var wSpell = getCheckBoxItem(harassMenu, "wharras");

            if (target == null)
                return;

            if (Player.IsMiniGnar())
            {
                if (qSpell && target.IsValidTarget(GnarSpells.QMini.Range)
                    && Player.LSDistance(target) > 450)
                {
                    if (!qsSpell)
                        GnarSpells.QMini.Cast(target);

                    else if (target.Buffs.Any(buff => buff.Name == "gnarwproc" && buff.Count == 2))
                        GnarSpells.QMini.Cast(target);
                }
            }

            if (Player.IsMegaGnar())
            {
                if (wSpell && Environment.TickCount - rcast >= 600)
                    GnarSpells.WMega.Cast(target);

                if (GnarSpells.QMega.IsReady() && qSpell)
                    GnarSpells.QMega.Cast(target);
            }
        }

        #endregion

        /// <summary>
        ///     Combo
        /// </summary>
        private static void Combo()
        {
            #region Mini Gnar

            if (Player.IsMiniGnar())
            {
                var target = TargetSelector.GetTarget(GnarSpells.QMini.Range, DamageType.Physical);
                var qSpell = getCheckBoxItem(comboMenu, "UseQMini");
                var qsSpell = getCheckBoxItem(comboMenu, "UseQs");
                var eSpell = getCheckBoxItem(comboMenu, "eGap");
                if (target == null)
                    return;
                var qpred = GnarSpells.QMini.GetPrediction(target);
                var collision = GnarSpells.QMini.GetCollision(Player.Position.To2D(), new List<Vector2> {qpred.CastPosition.To2D()});
                var mincol = collision.Where(obj => obj != null && obj.IsValidTarget() && !obj.IsDead && obj.IsMinion);

                var aiBases = mincol as Obj_AI_Base[] ?? mincol.ToArray();
                var objAiBases = mincol as IList<Obj_AI_Base> ?? aiBases.ToList();
                var count = objAiBases.Count();

                var firstcol = objAiBases.OrderBy(m => m.LSDistance(Player.ServerPosition, true)).FirstOrDefault();

                if (qSpell && target.IsValidTarget(GnarSpells.QMini.Range) && Player.LSDistance(target) > 450)
                {
                    if (!aiBases.Any())
                    {
                        if (!qsSpell)
                            GnarSpells.QnMini.Cast(qpred.CastPosition);

                        else if (target.Buffs.Any(buff => buff.Name == "gnarwproc" && buff.Count == 2))
                            GnarSpells.QnMini.Cast(qpred.CastPosition);
                    }
                    else
                    {
                        if (objAiBases.Any(minc => count >= 1 && firstcol.LSDistance(target) <= 350))
                        {
                            if (!qsSpell)
                            {
                                GnarSpells.QMini.Cast(qpred.CastPosition);
                            }

                            else if (target.Buffs.Any(buff => buff.Name == "gnarwproc" && buff.Count == 2))
                            {
                                GnarSpells.QnMini.Cast(qpred.CastPosition);
                            }
                        }
                    }
                }

                if (eSpell && GnarSpells.EMini.IsReady() && Player.IsAboutToTransform())
                {
                    var targetA = GnarSpells.EMini.GetTarget(GnarSpells.EMini.Width / 2f);
                    if (targetA != null)
                    {
                        var prediction = GnarSpells.EMini.GetPrediction(targetA);

                        if (prediction.Hitchance >= HitChance.High)
                        {
                            var arrivalPoint = Player.ServerPosition.Extend(prediction.CastPosition, Player.ServerPosition.LSDistance(prediction.CastPosition) + GnarSpells.EMini.Range);
                            if (!ObjectManager.Get<Obj_AI_Turret>().Any(t => t.Team != Player.Team && !t.IsDead && t.LSDistance(arrivalPoint, true) < 775 * 775))
                            {
                                if (GnarSpells.EMini.Cast(prediction.CastPosition))
                                {
                                    return;
                                }
                            }
                        }
                    }
                }
            }
            else

            #endregion

            #region Mega Gnar

            {
                var target = TargetSelector.GetTarget(GnarSpells.QMega.Range, DamageType.Physical);
                var rSlider = getSliderItem(comboMenu, "useRSlider");
                var emSpell = getCheckBoxItem(comboMenu, "UseEMega");
                var qmSpell = getCheckBoxItem(comboMenu, "UseQMega");
                var wSpell = getCheckBoxItem(comboMenu, "UseWMega");
                if (target == null)
                    return;
                if (GnarSpells.RMega.IsReady() && getCheckBoxItem(comboMenu, "UseRMega"))
                {
                    if (Player.CountEnemiesInRange(420) >= rSlider)
                    {
                        var prediction = Prediction.GetPrediction(target, GnarSpells.RMega.Delay);
                        if (GnarSpells.RMega.IsInRange(prediction.UnitPosition))
                        {
                            var direction = (Player.ServerPosition - prediction.UnitPosition).Normalized();
                            var maxAngle = 180f;
                            var step = maxAngle/6f;
                            var currentAngle = 0f;
                            var currentStep = 0f;
                            while (true)
                            {
                                if (currentStep > maxAngle && currentAngle < 0)
                                    break;

                                if ((currentAngle == 0 || currentAngle < 0) && currentStep != 0)
                                {
                                    currentAngle = currentStep*(float) Math.PI/180;
                                    currentStep += step;
                                }
                                else if (currentAngle > 0)
                                    currentAngle = -currentAngle;

                                Vector3 checkPoint;
                                if (currentStep == 0)
                                {
                                    currentStep = step;
                                    checkPoint = prediction.UnitPosition + 500*direction;
                                }
                                else
                                    checkPoint = prediction.UnitPosition + 500*direction.Rotated(currentAngle);

                                if (prediction.UnitPosition.GetFirstWallPoint(checkPoint).HasValue
                                    && !target.IsStunned
                                    &&
                                    target.Health >=
                                    GnarSpells.QMega.GetDamage(target) + Player.GetAutoAttackDamage(target))
                                {
                                    GnarSpells.RMega.Cast(Player.Position +
                                                          500*(checkPoint - prediction.UnitPosition).Normalized());
                                    rcast = Environment.TickCount;
                                    break;
                                }
                            }
                        }
                    }
                }

                if (GnarSpells.EMega.IsReady() && target.LSDistance(Player) > 500 && emSpell && target.HealthPercent <= Player.HealthPercent)
                    GnarSpells.EMega.Cast(target);

                if (wSpell && Environment.TickCount - rcast >= 600)
                    GnarSpells.WMega.Cast(target);

                if (qmSpell && Environment.TickCount - rcast >= 700)
                {
                    if (target.LSDistance(Player) <= 130)
                    {
                        if (GnarSpells.QnMega.IsReady())
                            GnarSpells.QnMega.Cast(target);
                    }
                    else if (GnarSpells.QMega.IsReady())
                        GnarSpells.QMega.Cast(target);
                }
            }
            #endregion
        }
    }
}