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
            
            Drawing.OnDraw += Drawing_OnDraw;
        }

        private void Obj_AI_Base_OnSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender == null || args.Target == null) return;
            if (!args.Target.IsMe) return;
            if (sender.IsAlly) return;
            if (sender.Type == GameObjectType.obj_AI_Turret)
                SetSpierdalanko((1100 - AutoWalker.myHero.Distance(sender)) / AutoWalker.myHero.MoveSpeed);
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
            if (active || (current.current == LogicSelector.MainLogics.CombatLogic && AutoWalker.myHero.HealthPercent() > 13))
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
            if (!MainMenu.GetMenu("AB").Get<CheckBox>("debuginfo").CurrentValue)
                return;

            Drawing.DrawText(250, 10, Color.Gold,
                "Survi, active: " + active + "  hits: " + hits + "  dangervalue: " + dangerValue);
        }

        public void Deactivate()
        {
            active = false;
        }

        private void Game_OnTick(EventArgs args)
        {
            if (AutoWalker.myHero.HealthPercent< 15 && AutoWalker.Ignite != null && AutoWalker.Ignite.IsReady())
            {
                AIHeroClient i = EntityManager.Heroes.Enemies.FirstOrDefault(en => en.Health < 50 + 20*AutoWalker.myHero.Level&&en.Distance(AutoWalker.myHero)<600);
                if (i != null) AutoWalker.UseIgnite(i);
            }
            if (hits * 20 > AutoWalker.myHero.HealthPercent() || (hits2 >= 5 && AutoWalker.myHero.Level < 8 && AutoWalker.myHero.HealthPercent < 50 && !EntityManager.Heroes.Enemies.Any(en => en.IsVisible() && en.HealthPercent < 10 && en.Distance(AutoWalker.myHero) < current.myChamp.OptimalMaxComboDistance)))
            {
                SetSpierdalanko(.5f);
            }
            dangerValue = current.localAwareness.LocalDomination(AutoWalker.myHero);
            if (dangerValue > -2000 || AutoWalker.myHero.Distance(AutoWalker.EnemyLazer) < 1500)
            {
                SetSpierdalankoUnc(.5f);
                current.saveMylife = true;
            }
            if (!active)
            {
                return;
            }
            if (AutoWalker.myHero.HealthPercent < 43)
            {
                AutoWalker.UseHPot();
            }
            if (Game.Time > spierdalanko)
            {
                current.saveMylife = false;
                current.SetLogic(returnTo);
            }
            Vector3 enemyTurret = AutoWalker.myHero.GetNearestTurret().Position;
            
            Vector3 closestSafePoint;
            if (AutoWalker.myHero.Distance(enemyTurret) > 1200)
            {
                closestSafePoint = AutoWalker.myHero.GetNearestTurret(false).Position;
                if (closestSafePoint.Distance(AutoWalker.myHero) > 2000)
                {
                    AIHeroClient ally = EntityManager.Heroes.Allies.Where(
                        a =>
                            a.Distance(AutoWalker.myHero) < 1500 &&
                            current.localAwareness.LocalDomination(a.Position) < -40000)
                        .OrderBy(al => al.Distance(AutoWalker.myHero))
                        .FirstOrDefault();
                    if (ally != null)
                        closestSafePoint = ally.Position;
                }
                if (closestSafePoint.Distance(AutoWalker.myHero) > 150)
                {
                    AIHeroClient ene =
                        EntityManager.Heroes.Enemies
                            .FirstOrDefault(en => en.Health > 0 && en.Distance(closestSafePoint) < 300);
                    if (ene != null)
                    {
                        closestSafePoint = AutoWalker.MyNexus.Position;
                    }
                }
                AutoWalker.SetMode(AutoWalker.myHero.Distance(closestSafePoint) < 200
                    ? Orbwalker.ActiveModes.Combo
                    : Orbwalker.ActiveModes.Flee);
                AutoWalker.WalkTo(closestSafePoint.Extend(AutoWalker.MyNexus, 200).To3DWorld());
            }
            else
            {
                AutoWalker.WalkTo(AutoWalker.myHero.Position.Away(enemyTurret, 1200));
                AutoWalker.SetMode(Orbwalker.ActiveModes.Flee);
            }

                if (AutoWalker.myHero.HealthPercent < 30)
                {
                    AutoWalker.UseHeal();
                }

                if (AutoWalker.myHero.HealthPercent < 15)
                {
                    AutoWalker.UseBarrier();
                    AutoWalker.UseSeraphs();
                }

            if (EntityManager.Heroes.Enemies.Any(en => en.IsVisible() && en.Distance(AutoWalker.myHero) < 600))
            {
                if (AutoWalker.myHero.HealthPercent < 30)
                    AutoWalker.UseSeraphs();
                if (AutoWalker.myHero.HealthPercent < 15)
                    AutoWalker.UseBarrier();
                if (AutoWalker.myHero.HealthPercent < 25)
                    AutoWalker.UseHeal();
            }

            if (AutoWalker.Ghost!=null&&AutoWalker.Ghost.IsReady() && dangerValue > 20000)
                AutoWalker.UseGhost();
            if (dangerValue > 10000)
            {
                if (AutoWalker.myHero.HealthPercent < 45)
                    AutoWalker.UseSeraphs();
                if (AutoWalker.myHero.HealthPercent < 30)
                    AutoWalker.UseBarrier();
                if (AutoWalker.myHero.HealthPercent < 25)
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