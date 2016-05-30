using System;
using System.Linq;
using AutoBuddy.Utilities.AutoShop;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using SharpDX;

namespace AutoBuddy.MainLogics
{
    internal class Combat
    {
        private readonly LogicSelector current;
        private bool active;
        private string lastMode = " ";
        private LogicSelector.MainLogics returnTo;

        public Combat(LogicSelector currentLogic)
        {
            current = currentLogic;
            Game.OnTick += Game_OnTick;
            Drawing.OnDraw += Drawing_OnDraw;
        }

        private void Drawing_OnDraw(EventArgs args)
        {
            if (!MainMenu.GetMenu("AB").Get<CheckBox>("debuginfo").CurrentValue)
                return;

            Drawing.DrawText(250, 25, System.Drawing.Color.Gold,
                "Combat, active:  " + active + " last mode: " + lastMode);
        }


        public void Activate()
        {
            if (active) return;
            active = true;
        }

        public void Deactivate()
        {
            active = false;
        }

        private void Game_OnTick(EventArgs args)
        {
            if (current.current == LogicSelector.MainLogics.SurviLogic) return;
            AIHeroClient har = null;
            AIHeroClient victim = null;
            if (current.surviLogic.dangerValue < -15000)
                victim = EntityManager.Heroes.Enemies.Where(
                    vic => !vic.IsZombie &&
                        vic.Distance(AutoWalker.myHero) < vic.BoundingRadius + AutoWalker.myHero.AttackRange + 450 &&
                        vic.IsVisible() && vic.Health > 0 &&
                        current.localAwareness.MyStrength()/current.localAwareness.HeroStrength(vic) > 1.5)
                    .OrderBy(v => v.Health)
                    .FirstOrDefault();

            
            if (victim == null || AutoWalker.myHero.GetNearestTurret().Distance(AutoWalker.myHero) > 1100)
            {
                har =
                    EntityManager.Heroes.Enemies.Where(
                        h => !h.IsZombie &&
                            h.Distance(AutoWalker.myHero) < AutoWalker.myHero.AttackRange + h.BoundingRadius + 50 && h.IsVisible() &&
                            h.HealthPercent() > 0).OrderBy(h => h.Distance(AutoWalker.myHero)).FirstOrDefault();
            }


            if ((victim != null || har != null) && !active)
            {
                LogicSelector.MainLogics returnT = current.SetLogic(LogicSelector.MainLogics.CombatLogic);
                if (returnT != LogicSelector.MainLogics.CombatLogic) returnTo = returnT;
            }
            if (!active)
                return;
            if (victim == null && har == null)
            {
                current.SetLogic(returnTo);
                return;
            }
            if (victim != null)
            {
                current.myChamp.Combo(victim);
                Vector3 vicPos = Prediction.Position.PredictUnitPosition(victim, 500).To3D();

                Vector3 posToWalk =
                    AutoWalker.myHero.Position.Away(vicPos,
                        (victim.BoundingRadius + current.myChamp.OptimalMaxComboDistance - 30)*
                        Math.Min(current.localAwareness.HeroStrength(victim)/current.localAwareness.MyStrength()*1.6f, 1));
                        
                if (NavMesh.GetCollisionFlags(posToWalk).HasFlag(CollisionFlags.Wall))
                {
                    posToWalk =
                        vicPos.Extend(current.pushLogic.myTurret,
                            (victim.BoundingRadius + AutoWalker.myHero.AttackRange - 30)*
                            Math.Min(
                                current.localAwareness.HeroStrength(victim)/current.localAwareness.MyStrength()*2f, 1))
                            .To3DWorld();
                }

                Obj_AI_Turret nearestEnemyTurret = posToWalk.GetNearestTurret();

                if (victim.Health < 10 + 4 * AutoWalker.myHero.Level && EntityManager.Heroes.Allies.Any(al=>!al.IsDead()&&al.Distance(vicPos)<550))
                    AutoWalker.UseIgnite(victim);
                if (victim.Health + victim.HPRegenRate * 2.5f < 50 + 20 * AutoWalker.myHero.Level && vicPos.Distance(nearestEnemyTurret)<1350)
                    AutoWalker.UseIgnite(victim);
                lastMode = "combo";
                if (AutoWalker.myHero.Distance(nearestEnemyTurret) < 950 + AutoWalker.myHero.BoundingRadius)
                {

                    if (victim.Health > AutoWalker.myHero.GetAutoAttackDamage(victim) + 15 ||
                        victim.Distance(AutoWalker.myHero) > AutoWalker.myHero.AttackRange + victim.BoundingRadius - 20)
                    {


                        lastMode = "enemy under turret, ignoring";
                        current.SetLogic(returnTo);
                        return;
                    }
                    lastMode = "combo under turret";
                }
                Orbwalker.DisableAttacking = current.myChamp.MaxDistanceForAA <
                                             AutoWalker.myHero.Distance(victim) + victim.BoundingRadius+10;
                AutoWalker.SetMode(Orbwalker.ActiveModes.Combo);
                AutoWalker.WalkTo(posToWalk);


                if (AutoWalker.Ghost != null && AutoWalker.Ghost.IsReady() &&
                    AutoWalker.myHero.HealthPercent()/victim.HealthPercent() > 2 &&
                    victim.Distance(AutoWalker.myHero) > AutoWalker.myHero.AttackRange + victim.BoundingRadius + 150 &&
                    victim.Distance(victim.Position.GetNearestTurret()) > 1500)
                    AutoWalker.Ghost.Cast();

                if (ObjectManager.Player.HealthPercent() < 35)
                {
                    if (AutoWalker.myHero.HealthPercent < 25)
                        AutoWalker.UseSeraphs();
                    if (AutoWalker.myHero.HealthPercent < 20)
                        AutoWalker.UseBarrier();

                        AutoWalker.UseHPot();
                }
            }
            else
            {
                Vector3 harPos = Prediction.Position.PredictUnitPosition(har, 500).To3D();
                harPos = AutoWalker.myHero.Position.Away(harPos, current.myChamp.HarassDistance + har.BoundingRadius - 20);
                
                lastMode = "harass";
                Obj_AI_Turret tu = harPos.GetNearestTurret();
                AutoWalker.SetMode(Orbwalker.ActiveModes.Harass);
                if (harPos.Distance(tu) < 1000)
                {
                    if (harPos.Distance(tu) < 850 + AutoWalker.myHero.BoundingRadius)
                        AutoWalker.SetMode(Orbwalker.ActiveModes.Flee);
                    harPos = AutoWalker.myHero.Position.Away(tu.Position, 1090);
                    
                    lastMode = "harass under turret";

                    /*if (harPos.Distance(AutoWalker.MyNexus) > tu.Distance(AutoWalker.MyNexus))
                        harPos =
                            tu.Position.Extend(AutoWalker.MyNexus, 1050 + AutoWalker.myHero.BoundingRadius).To3DWorld();*/
                }


                current.myChamp.Harass(har);


                AutoWalker.WalkTo(harPos);
            }
        }
    }
}