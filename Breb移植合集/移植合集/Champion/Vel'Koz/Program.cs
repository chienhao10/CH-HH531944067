using System;
using System.Collections.Generic;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using LeagueSharp.Common;
using SebbyLib;
using SharpDX;
using Color = System.Drawing.Color;
using Spell = LeagueSharp.Common.Spell;
using Utility = LeagueSharp.Common.Utility;

namespace OneKeyToWin_AIO_Sebby.Champions
{
    internal class Velkoz
    {
        private static readonly Menu Config = Program.Config;
        private static Spell E, Q, R, W, QSplit, QDummy;
        private static float QMANA, WMANA, EMANA, RMANA;
        private static MissileClient QMissile;
        private static List<Vector3> pointList;

        public static Menu draw, qMenu, wMenu, eMenu, rMenu, harassMenu, farmMenu;

        public static AIHeroClient Player
        {
            get { return ObjectManager.Player; }
        }

        public static void LoadOKTW()
        {
            Q = new Spell(SpellSlot.Q, 1180);
            QSplit = new Spell(SpellSlot.Q, 1000);
            QDummy = new Spell(SpellSlot.Q, (float) Math.Sqrt(Math.Pow(Q.Range, 2) + Math.Pow(QSplit.Range, 2)));
            W = new Spell(SpellSlot.W, 1000);
            E = new Spell(SpellSlot.E, 800);
            R = new Spell(SpellSlot.R, 1500);

            Q.SetSkillshot(0.25f, 70f, 1300f, true, SkillshotType.SkillshotLine);
            QSplit.SetSkillshot(0.1f, 70f, 2100f, true, SkillshotType.SkillshotLine);
            QDummy.SetSkillshot(0.5f, 55f, 1200, false, SkillshotType.SkillshotLine);
            W.SetSkillshot(0.25f, 85f, 1700f, false, SkillshotType.SkillshotLine);
            E.SetSkillshot(1f, 180f, float.MaxValue, false, SkillshotType.SkillshotCircle);
            R.SetSkillshot(0.1f, 80f, float.MaxValue, false, SkillshotType.SkillshotLine);

            draw = Config.AddSubMenu("Drawings");
            draw.Add("qRange", new CheckBox("Q range", false));
            draw.Add("wRange", new CheckBox("W range", false));
            draw.Add("eRange", new CheckBox("E range", false));
            draw.Add("rRange", new CheckBox("R range", false));
            draw.Add("onlyRdy", new CheckBox("Draw when skill rdy"));

            qMenu = Config.AddSubMenu("Q Config");
            qMenu.Add("autoQ", new CheckBox("Auto Q"));
            qMenu.Add("harrasQ", new CheckBox("Harass Q"));
            qMenu.Add("QHarassMana", new Slider("Harass Mana", 30));

            wMenu = Config.AddSubMenu("W Config");
            wMenu.Add("autoW", new CheckBox("Auto W"));
            wMenu.Add("harrasW", new CheckBox("Harass W"));

            eMenu = Config.AddSubMenu("E Config");
            eMenu.Add("autoE", new CheckBox("Auto E"));
            eMenu.Add("harrasE", new CheckBox("Harass E", false));
            eMenu.Add("EInterrupter", new CheckBox("Auto E Interrupter"));
            eMenu.AddGroupLabel("E Gap Closer Settings : ");
            eMenu.Add("EmodeGC",
                new ComboBox("Gap Closer position mode", 0, "Dash end position", "Player position", "Prediction"));
            foreach (var enemy in ObjectManager.Get<AIHeroClient>().Where(enemy => enemy.IsEnemy))
                eMenu.Add("EGCchampion" + enemy.NetworkId, new CheckBox("On : " + enemy.ChampionName));

            rMenu = Config.AddSubMenu("R Config");
            rMenu.Add("autoR", new CheckBox("Auto R KS"));

            harassMenu = Config.AddSubMenu("Harass");
            foreach (var enemy in ObjectManager.Get<AIHeroClient>().Where(enemy => enemy.IsEnemy))
                harassMenu.Add("harras" + enemy.NetworkId, new CheckBox("Harass : " + enemy.ChampionName));

            farmMenu = Config.AddSubMenu("Farm");
            farmMenu.Add("farmE", new CheckBox("Lane clear E"));
            farmMenu.Add("farmW", new CheckBox("Lane clear W"));
            farmMenu.Add("Mana", new Slider("LaneClear Mana", 80));
            farmMenu.Add("LCminions", new Slider("LaneClear minimum minions", 2, 0, 10));
            farmMenu.Add("jungleQ", new CheckBox("Jungle clear Q"));
            farmMenu.Add("jungleW", new CheckBox("Jungle clear W"));
            farmMenu.Add("jungleE", new CheckBox("Jungle clear E"));

            Game.OnUpdate += Game_OnGameUpdate;
            GameObject.OnCreate += Obj_AI_Base_OnCreate;
            Drawing.OnDraw += Drawing_OnDraw;
            Interrupter2.OnInterruptableTarget += Interrupter2_OnInterruptableTarget;
            AntiGapcloser.OnEnemyGapcloser += AntiGapcloser_OnEnemyGapcloser;
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

        private static void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            var t = gapcloser.Sender;
            if (E.IsReady() && t.IsValidTarget(E.Range) && getCheckBoxItem(eMenu, "EGCchampion" + t.NetworkId))
            {
                if (getBoxItem(eMenu, "EmodeGC") == 0)
                    E.Cast(gapcloser.End);
                else if (getBoxItem(eMenu, "EmodeGC") == 1)
                    E.Cast(Player.Position);
                else
                    E.Cast(t.ServerPosition);
            }
        }

        private static void Interrupter2_OnInterruptableTarget(AIHeroClient sender,
            Interrupter2.InterruptableTargetEventArgs args)
        {
            if (E.IsReady() && sender.IsValidTarget(E.Range) && getCheckBoxItem(eMenu, "EInterrupter"))
            {
                E.Cast(sender);
            }
        }

        private static void Obj_AI_Base_OnCreate(GameObject sender, EventArgs args)
        {
            if (sender.IsValid<MissileClient>() && sender.IsAlly)
            {
                var missile = (MissileClient) sender;
                if (missile.SData.Name != null && missile.SData.Name == "VelkozQMissile")
                    QMissile = missile;
            }
        }

        private static void Game_OnGameUpdate(EventArgs args)
        {
            if (Player.IsChannelingImportantSpell())
            {
                if (getCheckBoxItem(rMenu, "autoR"))
                {
                    var t = TargetSelector.GetTarget(R.Range + 150, DamageType.Magical);
                    if (t.IsValidTarget() && OktwCommon.ValidUlt(t))
                    {
                        Player.Spellbook.UpdateChargeableSpell(SpellSlot.R, R.GetPrediction(t, true).CastPosition, false, false);
                    }
                }
                Orbwalker.DisableAttacking = true;
                Orbwalker.DisableMovement = true;
            }
            else
            {
                if (R.IsReady() && getCheckBoxItem(rMenu, "autoR"))
                LogicR();
                Orbwalker.DisableAttacking = false;
                Orbwalker.DisableMovement = false;
            }

            if (!Player.IsChannelingImportantSpell())
            {
                Orbwalker.DisableAttacking = false;
                Orbwalker.DisableMovement = false;
            }

            if (Program.LagFree(4))
            {
                //R.Cast(Game.CursorPos);
            }

            if (Program.LagFree(0))
            {
                SetMana();
                Jungle();
            }

            if (Q.IsReady() && getCheckBoxItem(qMenu, "autoQ"))
                LogicQ();

            if (Program.LagFree(3) && E.IsReady() && getCheckBoxItem(eMenu, "autoE"))
                LogicE();

            if (Program.LagFree(4))
            {
                if (W.IsReady() && getCheckBoxItem(wMenu, "autoW"))
                    LogicW();
            }
        }

        private static void LogicR()
        {
            var t = TargetSelector.GetTarget(R.Range, DamageType.Magical);
            if (t.IsValidTarget() && Player.CountEnemiesInRange(400) == 0 && !Player.UnderTurret(true))
            {
                //900 - 100%
                //1500 - 10 %
                var rDmg = OktwCommon.GetKsDamage(t, R);
                var distance = Player.LSDistance(t);

                if (distance > 900 && OktwCommon.CanMove(t))
                {
                    var adjust = (R.Range - distance)/600;
                    rDmg = rDmg*adjust;
                }

                if (rDmg > t.Health && OktwCommon.ValidUlt(t))
                {
                    R.Cast(t);
                }
            }
        }

        private static void LogicE()
        {
            var t = TargetSelector.GetTarget(E.Range, DamageType.Magical);
            if (t.IsValidTarget())
            {
                if (Program.Combo && Player.Mana > RMANA + EMANA)
                    Program.CastSpell(E, t);
                else if (Program.Farm && OktwCommon.CanHarras() && getCheckBoxItem(eMenu, "harrasE") &&
                         getCheckBoxItem(harassMenu, "harras" + t.NetworkId) &&
                         Player.Mana > RMANA + EMANA + WMANA + EMANA)
                    Program.CastSpell(E, t);
                else
                {
                    var eDmg = OktwCommon.GetKsDamage(t, E);
                    var qDmg = Q.GetDamage(t);
                    if (qDmg + eDmg > t.Health)
                    {
                        if (eDmg > t.Health)
                            Program.CastSpell(E, t);
                        else if (Player.Mana > QMANA + EMANA)
                            Program.CastSpell(E, t);
                        return;
                    }
                }
                if (!Program.None && Player.Mana > RMANA + EMANA)
                {
                    foreach (
                        var enemy in
                            Program.Enemies.Where(enemy => enemy.IsValidTarget(E.Range) && !OktwCommon.CanMove(enemy)))
                        E.Cast(enemy);
                }
            }
            else if (Program.LaneClear && Player.ManaPercent > getSliderItem(farmMenu, "Mana") &&
                     getCheckBoxItem(farmMenu, "farmE") && Player.Mana > RMANA + EMANA)
            {
                var minionList = Cache.GetMinions(Player.ServerPosition, E.Range);
                var farmPosition = E.GetCircularFarmLocation(minionList, E.Width);

                if (farmPosition.MinionsHit >= getSliderItem(farmMenu, "LCminions"))
                    E.Cast(farmPosition.Position);
            }
        }

        private static void LogicW()
        {
            var t = TargetSelector.GetTarget(W.Range, DamageType.Magical);
            if (t.IsValidTarget())
            {
                if (Program.Combo && Player.Mana > RMANA + WMANA)
                    Program.CastSpell(W, t);
                else if (Program.Farm && getCheckBoxItem(wMenu, "harrasW") &&
                         getCheckBoxItem(harassMenu, "harras" + t.NetworkId)
                         && Player.Mana > RMANA + WMANA + EMANA + QMANA + WMANA && OktwCommon.CanHarras())
                    Program.CastSpell(W, t);
                else
                {
                    var wDmg = OktwCommon.GetKsDamage(t, W);
                    var qDmg = Q.GetDamage(t);
                    if (wDmg > t.Health)
                        Program.CastSpell(W, t);
                    else if (qDmg + wDmg > t.Health && Player.Mana > QMANA + WMANA)
                        Program.CastSpell(W, t);
                }
                if (!Program.None && Player.Mana > RMANA + WMANA)
                {
                    foreach (
                        var enemy in
                            Program.Enemies.Where(enemy => enemy.IsValidTarget(W.Range) && !OktwCommon.CanMove(enemy)))
                        W.Cast(enemy, true);
                }
            }
            else if (Program.LaneClear && Player.ManaPercent > getSliderItem(farmMenu, "Mana") &&
                     getCheckBoxItem(farmMenu, "farmW") && Player.Mana > RMANA + WMANA)
            {
                var minionList = Cache.GetMinions(Player.ServerPosition, W.Range);
                var farmPosition = W.GetLineFarmLocation(minionList, W.Width);

                if (farmPosition.MinionsHit >= getSliderItem(farmMenu, "LCminions"))
                    W.Cast(farmPosition.Position);
            }
        }

        private static void LogicQ()
        {
            var t = TargetSelector.GetTarget(QDummy.Range, DamageType.Magical);
            if (t.IsValidTarget())
            {
                if (Q.Instance.Name == "VelkozQ" && Utils.TickCount - Q.LastCastAttemptT > 150)
                {
                    if (Program.LagFree(1) || Program.LagFree(2))
                    {
                        QSplit.Collision = true;
                        if (Program.Combo && Player.Mana > RMANA + QMANA)
                            CastQ(t);
                        else if (Program.Farm && OktwCommon.CanHarras() && getCheckBoxItem(qMenu, "harrasQ") &&
                                 getCheckBoxItem(harassMenu, "harras" + t.NetworkId) &&
                                 Player.ManaPercent > getSliderItem(qMenu, "QHarassMana"))
                            CastQ(t);
                        else
                        {
                            var qDmg = OktwCommon.GetKsDamage(t, Q);
                            var wDmg = W.GetDamage(t);
                            if (qDmg > t.Health)
                                CastQ(t);
                            else if (qDmg + wDmg > t.Health && Player.Mana > QMANA + WMANA)
                                CastQ(t);
                        }
                        if (!Program.None && Player.Mana > RMANA + QMANA)
                        {
                            foreach (
                                var enemy in
                                    Program.Enemies.Where(
                                        enemy => enemy.IsValidTarget(QDummy.Range) && !OktwCommon.CanMove(enemy)))
                                CastQ(t);
                        }
                    }
                }
                else
                {
                    DetonateQ(t);
                }
            }
        }

        private static void CastQ(Obj_AI_Base t)
        {
            var Qpred = Q.GetPrediction(t);
            if (Qpred.Hitchance >= HitChance.High)
            {
                Program.CastSpell(Q, t);
            }
            else
            {
                var pred = QDummy.GetPrediction(t);
                if (pred.Hitchance >= HitChance.High)
                {
                    if (Program.LagFree(1))
                        pointList = AimQ(t.ServerPosition);
                    if (Program.LagFree(2))
                        BestAim(t.ServerPosition);
                }
            }
        }

        private static void DetonateQ(Obj_AI_Base t)
        {
            if (QMissile != null && QMissile.IsValid)
            {
                QSplit.Collision = false;
                var realPosition = QMissile.StartPosition.LSExtend(QMissile.EndPosition,
                    QMissile.StartPosition.LSDistance(QMissile.Position) + Game.Ping/2 + 60);
                //Q.Cast();

                QSplit.UpdateSourcePosition(realPosition, realPosition);

                var start = QMissile.StartPosition.To2D();
                var end = realPosition.To2D();
                var radius = QSplit.Range;

                var dir = (end - start).Normalized();
                var pDir = dir.Perpendicular();

                var rightEndPos = end + pDir*radius;
                var leftEndPos = end - pDir*radius;

                var rEndPos = new Vector3(rightEndPos.X, rightEndPos.Y, ObjectManager.Player.Position.Z);
                var lEndPos = new Vector3(leftEndPos.X, leftEndPos.Y, ObjectManager.Player.Position.Z);

                if (QSplit.WillHit(t, rEndPos) || QSplit.WillHit(t, lEndPos))
                    Q.Cast();
            }
        }

        private static List<Vector3> AimQ(Vector3 finalPos)
        {
            var CircleLineSegmentN = 36;
            var radius = 500;
            var position = Player.Position;

            var points = new List<Vector3>();
            for (var i = 1; i <= CircleLineSegmentN; i++)
            {
                var angle = i*2*Math.PI/CircleLineSegmentN;
                var point = new Vector3(position.X + radius*(float) Math.Cos(angle),
                    position.Y + radius*(float) Math.Sin(angle), position.Z);
                if (point.Distance(Player.Position.Extend(finalPos, radius)) < 430)
                {
                    points.Add(point);
                    //Utility.DrawCircle(point, 20, System.Drawing.Color.Aqua, 1, 1);
                }
            }

            var point2 = points.OrderBy(x => x.LSDistance(finalPos));
            points = point2.ToList();
            points.RemoveAt(0);
            points.RemoveAt(1);
            return points;
        }

        private static void BestAim(Vector3 predictionPos)
        {
            var start = Player.Position.To2D();
            var c1 = predictionPos.LSDistance(Player.Position);
            var playerPos2d = Player.Position.To2D();

            foreach (var point in pointList)
            {
                for (var j = 400; j <= 1100; j = j + 50)
                {
                    var posExtend = Player.Position.LSExtend(point, j);

                    var a1 = Player.LSDistance(posExtend);
                    var b1 = (float) Math.Sqrt(c1*c1 - a1*a1);

                    if (b1 > QSplit.Range)
                        continue;

                    var pointA = Player.Position.LSExtend(point, a1);

                    var end = pointA.To2D();
                    var dir = (end - start).Normalized();
                    var pDir = dir.Perpendicular();

                    var rightEndPos = end + pDir*b1;
                    var leftEndPos = end - pDir*b1;

                    var rEndPos = new Vector3(rightEndPos.X, rightEndPos.Y, ObjectManager.Player.Position.Z);
                    var lEndPos = new Vector3(leftEndPos.X, leftEndPos.Y, ObjectManager.Player.Position.Z);

                    if (lEndPos.LSDistance(predictionPos) < QSplit.Width)
                    {
                        var collision = Q.GetCollision(playerPos2d, new List<Vector2> {posExtend.To2D()});
                        if (collision.Count > 0)
                            break;

                        var collisionS = QSplit.GetCollision(pointA.To2D(), new List<Vector2> {lEndPos.To2D()});
                        if (collisionS.Count > 0)
                            break;

                        Q.Cast(pointA);
                        return;
                    }
                    if (rEndPos.LSDistance(predictionPos) < QSplit.Width)
                    {
                        var collision = Q.GetCollision(playerPos2d, new List<Vector2> {posExtend.To2D()});
                        if (collision.Count > 0)
                            break;

                        var collisionR = QSplit.GetCollision(pointA.To2D(), new List<Vector2> {rEndPos.To2D()});
                        if (collisionR.Count > 0)
                            break;

                        Q.Cast(pointA);
                        return;
                    }
                }
            }
        }

        private static void Jungle()
        {
            if (Program.LaneClear && Player.Mana > RMANA + QMANA)
            {
                var mobs = Cache.GetMinions(Player.ServerPosition, E.Range, MinionTeam.Neutral);
                if (mobs.Count > 0)
                {
                    var mob = mobs[0];
                    if (W.IsReady() && getCheckBoxItem(farmMenu, "jungleW"))
                    {
                        W.Cast(mob.ServerPosition);
                    }
                    else if (Q.IsReady() && getCheckBoxItem(farmMenu, "jungleQ"))
                    {
                        Q.Cast(mob.ServerPosition);
                    }

                    else if (E.IsReady() && getCheckBoxItem(farmMenu, "jungleE"))
                    {
                        E.Cast(mob.ServerPosition);
                    }
                }
            }
        }

        private static void Drawing_OnDraw(EventArgs args)
        {
            //Utility.DrawCircle(Game.CursorPos, E.Width, System.Drawing.Color.Cyan, 1, 1);
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
                        Utility.DrawCircle(ObjectManager.Player.Position, E.Range, Color.Yellow, 1, 1);
                }
                else
                    Utility.DrawCircle(ObjectManager.Player.Position, E.Range, Color.Yellow, 1, 1);
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
    }
}