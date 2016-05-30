using System;
using System.Linq;
using AutoBuddy.Humanizers;
using AutoBuddy.Utilities;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK.Rendering;
using SharpDX;
using Color = System.Drawing.Color;

namespace AutoBuddy.MainLogics
{
    internal class Push
    {
        private readonly LogicSelector currentLogic;
        private bool active;
        private Obj_AI_Minion[] currentWave;
        private int CurrentWaveNum;
        private Lane lane;

        private float randomAngle;
        private float randomExtend;
        private Vector3 randomVector;
        private bool wholeWave;

        private float lastRand;
        private Vector3 rand;
        private ColorBGRA color;
        private ColorBGRA colorGreen;
        private ColorBGRA colorRed;
        private float lastAtk;


        public Push(LogicSelector current)
        {
            color=new ColorBGRA(255, 210, 105, 255);
            colorRed = new ColorBGRA(139, 0, 0, 255);
            colorGreen = new ColorBGRA(0, 100, 0, 255);
            SetRandVector();
            randomVector = new Vector3();
            currentWave = new Obj_AI_Minion[0];
            currentLogic = current;
            Core.DelayAction(SetWaveNumber, 500);
            SetCurrentWave();
            SetOffset();
            if (MainMenu.GetMenu("AB").Get<CheckBox>("debuginfo").CurrentValue)
                Drawing.OnDraw += Drawing_OnDraw;
        }

        public Obj_AI_Base myTurret { get; private set; }
        public Obj_AI_Base enemyTurret { get; private set; }

        private void SetRandVector()
        {
            randomVector.X = RandGen.r.NextFloat(0, 300);
            randomVector.Y = RandGen.r.NextFloat(0, 300);
            Core.DelayAction(SetRandVector, 1000);
        }

        public void Reset(Obj_AI_Base myTower, Obj_AI_Base enemyTower, Lane ln)
        {
            Vector3 pingPos = AutoWalker.p.Distance(AutoWalker.MyNexus) - 100 > myTower.Distance(AutoWalker.MyNexus)
                ? enemyTower.Position
                : myTower.Position;
            Core.DelayAction(() => SafeFunctions.Ping(PingCategory.OnMyWay, pingPos.Randomized()), RandGen.r.Next(3000));
            lane = ln;
            currentWave = new Obj_AI_Minion[0];
            myTurret = myTower;
            enemyTurret = enemyTower;
            randomExtend = 0;
            currentLogic.SetLogic(LogicSelector.MainLogics.PushLogic);
        }

        public void Activate()
        {
            AutoWalker.SetMode(Orbwalker.ActiveModes.LaneClear);
            currentLogic.current = LogicSelector.MainLogics.PushLogic;
            if (active) return;

            Game.OnTick += Game_OnTick;
            active = true;
        }

        public void Deactivate()
        {
            active = false;
            Game.OnTick -= Game_OnTick;
        }

        private void Game_OnTick(EventArgs args)
        {
            if (Shop.CanShop == false)
            {
                int hppotval = Program.hpvaluePot;
                if (ObjectManager.Player.HealthPercent() < hppotval)
                {
                    AutoWalker.UseHPot();
                }
            }
            if (!active||myTurret==null) return;
            if (!AutoWalker.p.IsDead() && (myTurret.Health <= 0 || enemyTurret.Health <= 0))
            {
                currentLogic.loadLogic.SetLane();
            }
            if (currentWave.Length == 0)
                UnderMyTurret();
            else if (AutoWalker.p.Distance(enemyTurret) < 950 + AutoWalker.p.BoundingRadius)
                UnderEnemyTurret();
            else
                Between();
        }


        private void Drawing_OnDraw(EventArgs args)
        {
            Drawing.DrawText(250, 40, Color.Gold,
                "Push, active: " + active + "  wave num: " + CurrentWaveNum + " minions left: " + currentWave.Length);
            Circle.Draw(color, 100, currentWave.Length <= 0 ? AutoWalker.p.Position : AvgPos(currentWave));

            if (myTurret != null)
                Circle.Draw(colorGreen, 200, myTurret.Position);
            
            if (enemyTurret != null)
                Circle.Draw(colorRed, 200, enemyTurret.Position);
        }

        private void UnderEnemyTurret()
        {
            if (
                ObjectManager.Get<Obj_AI_Minion>()
                    .Count(min => min.IsAlly && min.HealthPercent() > 30 && min.Distance(enemyTurret) < 850) < 2 || (EntityManager.Heroes.Enemies.Any(en => en.IsVisible && en.HasBuffOfType(BuffType.Damage)&&AutoWalker.p.HealthPercent-en.HealthPercent<65 && en.Distance(enemyTurret) < 800 && AutoWalker.p.Distance(enemyTurret) < AutoWalker.p.BoundingRadius+850)))
            {
                AutoWalker.SetMode(Orbwalker.ActiveModes.LaneClear);
                AutoWalker.WalkTo(AutoWalker.p.Position.Away(enemyTurret.Position, 1200));
                return;
            }
            if (AutoWalker.p.Distance(enemyTurret) <
                AutoWalker.p.AttackRange + enemyTurret.BoundingRadius + Orbwalker.HoldRadius &&
                AutoWalker.p.Distance(enemyTurret) > AutoWalker.p.AttackRange)
            {
                AutoWalker.SetMode(Orbwalker.ActiveModes.None);
                if (Game.Time > lastAtk)
                {
                    lastAtk = Game.Time + RandGen.r.NextFloat(.2f, .4f);
                    Player.IssueOrder(GameObjectOrder.AttackUnit, enemyTurret);
                }
                
            }
            else
            {
                AutoWalker.SetMode(Orbwalker.ActiveModes.LastHit);
                AutoWalker.WalkTo(
                    enemyTurret.Position.Extend(AutoWalker.p, AutoWalker.p.AttackRange + enemyTurret.BoundingRadius)
                        .To3DWorld());
            }
        }

        private void Between()
        {
            AutoWalker.SetMode(Orbwalker.ActiveModes.LaneClear);
            Vector3 p = AvgPos(currentWave);
            if (p.Distance(AutoWalker.MyNexus) > myTurret.Distance(AutoWalker.MyNexus))
            {
                AIHeroClient ally =
                    EntityManager.Heroes.Allies.Where(
                        al => !al.IsMe &&
                              AutoWalker.p.Distance(al) < 1500 &&
                              al.Distance(enemyTurret) < p.Distance(enemyTurret) + 100 &&
                              currentLogic.localAwareness.LocalDomination(al) < -10000)
                        .OrderBy(l => l.Distance(AutoWalker.p))
                        .FirstOrDefault();
                if (ally != null &&
                    Math.Abs(p.Distance(AutoWalker.EneMyNexus) - AutoWalker.p.Distance(AutoWalker.EneMyNexus)) < 200)
                    p = ally.Position.Extend(myTurret, 160).To3DWorld() + randomVector;
                p =
                    p.Extend(p.Extend(
                        AutoWalker.p.Distance(myTurret) < AutoWalker.p.Distance(enemyTurret)
                            ? myTurret
                            : enemyTurret,
                        400).To3D().RotatedAround(p, 1.57f), randomExtend).To3DWorld();
                AutoWalker.WalkTo(p);
            }
            else
                UnderMyTurret();
        }

        private void UnderMyTurret()
        {
            if (AutoWalker.p.Gold <= 100 ||
                !EntityManager.MinionsAndMonsters.EnemyMinions.Any(en => en.Distance(myTurret) < 1000))
            {
                if (Game.Time > lastRand)
                {
                    lastRand = Game.Time+RandGen.r.NextFloat(5, 10);
                    rand = new Vector3(RandGen.r.NextFloat(-400, 400), RandGen.r.NextFloat(-400, 400), 0);
                    while ((myTurret.Position.Extend(AutoWalker.MyNexus, 200).To3D() + rand).Distance(myTurret)<250)
                    {
                        rand = new Vector3(RandGen.r.NextFloat(-400, 400), RandGen.r.NextFloat(-400, 400), 0);
                    }
                    
                }

                AutoWalker.WalkTo(myTurret.Position.Extend(AutoWalker.MyNexus, 200).To3D()+rand);
                return;
            }

            Vector3 p = new Vector3();
            AIHeroClient ally =
                EntityManager.Heroes.Allies.Where(
                    al => !al.IsMe &&
                          AutoWalker.p.Distance(al) < 1200 && al.Distance(enemyTurret) < p.Distance(enemyTurret) + 150 &&
                          currentLogic.localAwareness.LocalDomination(al) < -15000)
                    .OrderBy(l => l.Distance(AutoWalker.p))
                    .FirstOrDefault();
            if (AutoWalker.p.Gold > 100 && ally != null)
            {
                p = ally.Position.Extend(myTurret, 160).To3DWorld() + randomVector;
                AutoWalker.SetMode(AutoWalker.p.Distance(enemyTurret) < 900
                    ? Orbwalker.ActiveModes.LastHit
                    : Orbwalker.ActiveModes.LaneClear);
                AutoWalker.WalkTo(p);
            }
            else
            {
                AutoWalker.WalkTo(myTurret.Position.Extend(AutoWalker.p.Position, 350 + randomExtend/2)
                    .To3D()
                    .RotatedAround(myTurret.Position, randomAngle));
            }
        }

        private Vector3 AvgPos(Obj_AI_Minion[] objects)
        {
            double x = 0, y = 0;
            foreach (Obj_AI_Minion obj in objects)
            {
                x += obj.Position.X;
                y += obj.Position.Y;
            }
            return new Vector2((float) (x/objects.Count()), (float) (y/objects.Count())).To3DWorld();
        }

        private void SetOffset()
        {
            if (!active)
            {
                Core.DelayAction(SetOffset, 500);
                return;
            }
            float newEx = randomExtend;
            while (Math.Abs(newEx - randomExtend) < 190)
            {
                newEx = RandGen.r.NextFloat(-400, 400);
            }
            randomAngle = RandGen.r.NextFloat(0, 6.28f);
            randomExtend = newEx;
            Core.DelayAction(SetOffset, RandGen.r.Next(800, 1600));
        }

        private void SetWaveNumber()
        {
            Core.DelayAction(SetWaveNumber, 500);
            Obj_AI_Minion closest =
                ObjectManager.Get<Obj_AI_Minion>()
                    .Where(
                        min => min.IsAlly && min.Name.Length > 13 && min.GetLane() == lane && min.HealthPercent() > 80)
                    .OrderBy(min => min.Distance(enemyTurret))
                    .FirstOrDefault();
            if (closest != null)
            {
                CurrentWaveNum = closest.GetWave();
            }
        }

        private void SetCurrentWave()
        {
            if (CurrentWaveNum == 0)
            {
                Core.DelayAction(SetCurrentWave, 1000);
                return;
            }
            currentWave =
                currentWave.Where(
                    min => min.Health > 1)
                    .ToArray();
            if (currentWave.Length > 1)
            {
                Core.DelayAction(SetCurrentWave, 1000);
                return;
            }

            Obj_AI_Minion[] newMinions =
                ObjectManager.Get<Obj_AI_Minion>()
                    .Where(min => min.IsAlly && min.GetLane() == lane && min.GetWave() == CurrentWaveNum)
                    .ToArray();
            if (!wholeWave && newMinions.Length < 7)
            {
                wholeWave = true;

                Core.DelayAction(SetCurrentWave,
                    newMinions.Any(min => min.Distance(AutoWalker.MyNexus) < 800) ? 3000 : 300);
            }
            else
            {
                wholeWave = false;
                currentWave = newMinions;
                Core.DelayAction(SetCurrentWave, 1000);
            }
        }
    }
}