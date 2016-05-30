using System;
using System.Linq;
using AutoBuddy.Utilities.AutoShop;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using SharpDX;
using Color = System.Drawing.Color;

namespace AutoBuddy.MainLogics
{
    internal class Survi
    {
        private readonly LogicSelector current;
        private bool active;
        public float dangerValue;
        private int hits;
        private int hits2;
        private LogicSelector.MainLogics returnTo;
        private float spierdalanko;

        public Survi(LogicSelector currentLogic)
        {
            current = currentLogic;
            Game.OnTick += Game_OnTick;
            Obj_AI_Base.OnSpellCast += Obj_AI_Base_OnSpellCast;
            DecHits();
            if (MainMenu.GetMenu("AB").Get<CheckBox>("debuginfo").CurrentValue)
                Drawing.OnDraw += Drawing_OnDraw;
        }

        private void Obj_AI_Base_OnSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender == null || args.Target == null) return;
            if (!args.Target.IsMe) return;
            if (sender.IsAlly) return;
            if (sender.Type == GameObjectType.obj_AI_Turret)
                SetSpierdalanko((1100 - AutoWalker.p.Distance(sender)) / AutoWalker.p.MoveSpeed);
            else if (sender.Type == GameObjectType.obj_AI_Minion)
            {
                hits++;
                hits2++;
            }
            else if (sender.Type == GameObjectType.AIHeroClient) hits += 2;
        }


        private void SetSpierdalanko(float sec)
        {
            spierdalanko = Game.Time + sec;
            if (active || (current.current == LogicSelector.MainLogics.CombatLogic && AutoWalker.p.HealthPercent() > 13))
                return;
            LogicSelector.MainLogics returnT = current.SetLogic(LogicSelector.MainLogics.SurviLogic);
            if (returnT != LogicSelector.MainLogics.SurviLogic) returnTo = returnT;
        }

        private void SetSpierdalankoUnc(float sec)
        {
            spierdalanko = Game.Time + sec;
            if (active) return;
            LogicSelector.MainLogics returnT = current.SetLogic(LogicSelector.MainLogics.SurviLogic);
            if (returnT != LogicSelector.MainLogics.SurviLogic) returnTo = returnT;
        }

        public void Activate()
        {
            if (active) return;
            active = true;
        }

        private void Drawing_OnDraw(EventArgs args)
        {
            Drawing.DrawText(250, 10, Color.Gold,
                "Survi, active: " + active + "  hits: " + hits + "  dangervalue: " + dangerValue);
        }

        public void Deactivate()
        {
            active = false;
        }

        private void Game_OnTick(EventArgs args)
        {
           
            if (hits * 20 > AutoWalker.p.HealthPercent() || (hits2 >= 5 && AutoWalker.p.Level < 8 && AutoWalker.p.HealthPercent < 50 && !EntityManager.Heroes.Enemies.Any(en => en.IsVisible() && en.HealthPercent < 10 && en.Distance(AutoWalker.p) < current.myChamp.OptimalMaxComboDistance)))
            {
                SetSpierdalanko(.5f);
            }
            dangerValue = current.localAwareness.LocalDomination(AutoWalker.p);
            if (dangerValue > -2000 || AutoWalker.p.Distance(AutoWalker.EnemyLazer) < 1500)
            {
                SetSpierdalankoUnc(.5f);
                current.saveMylife = true;
            }
            if (!active)
            {
                return;
            }                       
            if (Shop.CanShop == false)
            {
                int hppotval = Program.hpvaluePot;
                if (ObjectManager.Player.HealthPercent() < hppotval)
                {
                    AutoWalker.UseHPot();
                }
            }
            if (Game.Time > spierdalanko)
            {
                current.saveMylife = false;
                current.SetLogic(returnTo);
            }
            Vector3 enemyTurret = AutoWalker.p.GetNearestTurret().Position;
            
            Vector3 closestSafePoint;
            if (AutoWalker.p.Distance(enemyTurret) > 1200)
            {
                closestSafePoint = AutoWalker.p.GetNearestTurret(false).Position;
                if (closestSafePoint.Distance(AutoWalker.p) > 2000)
                {
                    AIHeroClient ally = EntityManager.Heroes.Allies.Where(
                        a =>
                            a.Distance(AutoWalker.p) < 1500 &&
                            current.localAwareness.LocalDomination(a.Position) < -40000)
                        .OrderBy(al => al.Distance(AutoWalker.p))
                        .FirstOrDefault();
                    if (ally != null)
                        closestSafePoint = ally.Position;
                }
                if (closestSafePoint.Distance(AutoWalker.p) > 150)
                {
                    AIHeroClient ene =
                        EntityManager.Heroes.Enemies
                            .FirstOrDefault(en => en.Health > 0 && en.Distance(closestSafePoint) < 300);
                    if (ene != null)
                    {
                        closestSafePoint = AutoWalker.MyNexus.Position;
                    }
                }
                AutoWalker.SetMode(AutoWalker.p.Distance(closestSafePoint) < 200
                    ? Orbwalker.ActiveModes.Combo
                    : Orbwalker.ActiveModes.Flee);
                AutoWalker.WalkTo(closestSafePoint.Extend(AutoWalker.MyNexus, 200).To3DWorld());
            }
            else
            {
                AutoWalker.WalkTo(AutoWalker.p.Position.Away(enemyTurret, 1200));
                AutoWalker.SetMode(Orbwalker.ActiveModes.Flee);
            }


            if (AutoWalker.p.HealthPercent < 15 && AutoWalker.HasIgnite == true && AutoWalker.Ignite.IsReady())
            {
                AIHeroClient i = EntityManager.Heroes.Enemies.FirstOrDefault(en => en.Health < 50 + 20 * AutoWalker.p.Level && en.Distance(AutoWalker.p) < 600);
                if (i != null) AutoWalker.UseIgnite(i);
            }

            if (AutoWalker.p.HealthPercent < 10)
            {
                if (AutoWalker.p.HealthPercent < 7)
                {
                    AutoWalker.UseHeal();
                    AutoWalker.UseBarrier();
                    //AutoWalker.UseSeraphs();
                }
                
            }
            
            if (EntityManager.Heroes.Enemies.Any(en => en.IsVisible() && en.Distance(AutoWalker.p) < 600))
            {
                //if (AutoWalker.p.HealthPercent < 30)
                //    AutoWalker.UseSeraphs();
                if (AutoWalker.p.HealthPercent < 25)
                    AutoWalker.UseBarrier();
                if (AutoWalker.p.HealthPercent < 18)
                    AutoWalker.UseHeal();
            }

            if (AutoWalker.HasGhost == true && AutoWalker.Ghost.IsReady() && dangerValue > 20000)
                AutoWalker.UseGhost();
            if (dangerValue > 10000)
            {
                //if (AutoWalker.p.HealthPercent < 45)
                //    AutoWalker.UseSeraphs();
                if (AutoWalker.p.HealthPercent < 30)
                    AutoWalker.UseBarrier();
                if (AutoWalker.p.HealthPercent < 25)
                    AutoWalker.UseHeal();
            }
            
            current.myChamp.Survi();
        }

        private void DecHits()
        {
            if (hits > 3)
                hits = 3;
            if (hits > 0)
                hits--;
            hits2--;
            Core.DelayAction(DecHits, 600);
        }
    }
}