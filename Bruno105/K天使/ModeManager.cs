using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK.Rendering;
using System.Linq;


namespace KKayle
{
   internal class ModeManager : Program
    {


        public static void Combo()
        {
            var Q = Program.Q;
            var W = Program.W;
            var E = Program.E;
            var R = Program.R;
            var alvo = TargetSelector.GetTarget(W.Range, DamageType.Magical);
            if (!alvo.IsValid()) return;

            if (Q.IsReady() && Q.IsInRange(alvo))
            {
                Q.Cast(alvo);
            }
            if (W.IsReady() && W.IsInRange(alvo) && Program.ComboMenu["ComboW"].Cast<CheckBox>().CurrentValue)
            {
                W.Cast(Player.Instance);
            }
            if (E.IsReady() && E.IsInRange(alvo))
            {
                E.Cast();
            }
        }

        public static void Harass()
        {
            var Q = Program.Q;
            var W = Program.W;
            var E = Program.E;
            var R = Program.R;
            var alvo = TargetSelector.GetTarget(W.Range, DamageType.Magical);
            if (!alvo.IsValid()) return;
            if (!(Player.Instance.ManaPercent > Program.HarassMenu["ManaH"].Cast<Slider>().CurrentValue))
            {
                return;
            }
            if (Q.IsReady() && Q.IsInRange(alvo) && Program.HarassMenu["HarassQ"].Cast<CheckBox>().CurrentValue)
            {
                Q.Cast(alvo);
            }
            if (W.IsReady() && W.IsInRange(alvo) && Program.HarassMenu["HarassW"].Cast<CheckBox>().CurrentValue)
            {
                W.Cast(Player.Instance);
            }
            if (E.IsReady() && (E.IsInRange(alvo)) && Program.HarassMenu["HarassE"].Cast<CheckBox>().CurrentValue)
            {
                E.Cast();
            }
        }

        public static void LaneClear()
        {
            var Q = Program.Q;
            var W = Program.W;
            var E = Program.E;
            var R = Program.R;
            var minion = EntityManager.MinionsAndMonsters.EnemyMinions.FirstOrDefault(m => m.IsValidTarget(Q.Range));
            var qminions = EntityManager.MinionsAndMonsters.EnemyMinions.FirstOrDefault(m => m.IsValidTarget(Program.Q.Range));
            var Cminion = EntityManager.MinionsAndMonsters.EnemyMinions.Where(t => t.IsInRange(Player.Instance.Position, Q.Range) && !t.IsDead && t.IsValid && !t.IsInvulnerable).Count();
            if (minion == null) return;
            if (!(Player.Instance.ManaPercent > Program.FarmMenu["ManaF"].Cast<Slider>().CurrentValue))
            {
                return;
            }
            if (Q.IsReady() && Program.FarmMenu["FarmQ"].Cast<CheckBox>().CurrentValue && Q.IsInRange(qminions) && minion.IsValidTarget(Q.Range) && qminions.Health < DamageLib.QCalc(qminions))
            {
                    Q.Cast(qminions);

                               }
            if (E.IsReady() && Program.FarmMenu["FarmE"].Cast<CheckBox>().CurrentValue /*&& (Cminion >= Program.FarmMenu["MinionE"].Cast<Slider>().CurrentValue)*/)
            {
                E.Cast();
            }

        }


        public static void JungleClear()
        {
            var Q = Program.Q;
            var W = Program.W;
            var E = Program.E;
            var R = Program.R;
            var jungleMonsters = EntityManager.MinionsAndMonsters.GetJungleMonsters().OrderByDescending(j => j.Health).FirstOrDefault(j => j.IsValidTarget(Program.Q.Range));
            var Cminion = EntityManager.MinionsAndMonsters.EnemyMinions.Where(t => t.IsInRange(Player.Instance.Position, Q.Range) && !t.IsDead && t.IsValid && !t.IsInvulnerable).Count();
            if (jungleMonsters == null) return;
            if (!(Player.Instance.ManaPercent > Program.FarmMenu["ManaF"].Cast<Slider>().CurrentValue))
            {
                return;
            }
            if (Q.IsReady() && Program.FarmMenu["FarmQ"].Cast<CheckBox>().CurrentValue && Q.IsInRange(jungleMonsters) && jungleMonsters.IsValidTarget(Q.Range))
            {
                Q.Cast(jungleMonsters);

                if (E.IsReady() && Program.FarmMenu["FarmE"].Cast<CheckBox>().CurrentValue && jungleMonsters.IsValidTarget(Q.Range))
                {
                    E.Cast();
                }

            }
        }

        public static void LastHit()
        {

            var Q = Program.Q;
            var E = Program.E;
            var qminions = EntityManager.MinionsAndMonsters.EnemyMinions.FirstOrDefault(m => m.IsValidTarget(Program.Q.Range));
            var eminions = EntityManager.MinionsAndMonsters.EnemyMinions.FirstOrDefault(m => m.IsValidTarget(Program.Q.Range) && (DamageLib.ECalc(m) > m.Health + 50));
            if (qminions == null) return;

            if (Q.IsReady() && Program.Q.IsInRange(qminions) && Program.FarmMenu["LastQ"].Cast<CheckBox>().CurrentValue && qminions.Health < DamageLib.QCalc(qminions))

                Q.Cast(qminions);
          //  if (E.IsReady() && Program.Q.IsInRange(eminions) && Program.FarmMenu["LastE"].Cast<CheckBox>().CurrentValue && (eminions.Health + 150) < DamageLib.ECalc(eminions))

            //    E.Cast();
        }
        public static void Flee()
        {

            var Q = Program.Q;
            var W = Program.W;
            var alvo = TargetSelector.GetTarget(Q.Range, DamageType.Magical);
            if (Q.IsReady() && Q.IsInRange(alvo))
            {
                Q.Cast(alvo);

            }
            if (W.IsReady())
            {
                W.Cast(Player.Instance);

            }
      
        }
             public static void AutoHeal()
             {


            var Q = Program.Q;
            var W = Program.W;
            var E = Program.E;
            var R = Program.R;

            if (!W.IsReady())
            {
                return;
            }
          if(Program.HealMenu["AutoW"].Cast<CheckBox>().CurrentValue && !(Program.PlayerInstance.IsRecalling())){
                var lowestHealthAlly = EntityManager.Heroes.Allies.Where(a => W.IsInRange(a) && !a.IsMe).OrderBy(a => a.Health).FirstOrDefault();

                if (Program.HealthPercent() <= Program.HealMenu["HealSelf"].Cast<Slider>().CurrentValue)
                {
                    W.Cast(Program.PlayerInstance);
                }

                else if (lowestHealthAlly != null)
                {
                    if (!(lowestHealthAlly.Health <= Program.HealMenu["HealAlly"].Cast<Slider>().CurrentValue))
                    {
                        return;
                    }
                    if (Program.HealMenu["autoHeal_" + lowestHealthAlly.BaseSkinName].Cast<CheckBox>().CurrentValue)
                    {
                        W.Cast(lowestHealthAlly);
                    }
                }
          }
          }
            public static void AutoUlt()
             {
                
                 var Q = Program.Q;
                 var W = Program.W;
                 var E = Program.E;
                 var R = Program.R;
                 var alvo = TargetSelector.GetTarget(3000, DamageType.Magical);
                 if(alvo == null) return;
                 if (!alvo.IsValid()) return;
                 if (!R.IsReady() || Player.Instance.IsRecalling())
                 {
                     return;
                 }
                
               //  if (Program.OnDamage == false) return;
                 if(_Player.Distance(alvo) >= 3000) return;
                 var lowestHealthAllies = EntityManager.Heroes.Allies.Where(a => R.IsInRange(a) && !a.IsMe).OrderBy(a => a.Health).FirstOrDefault();

                 if (Player.Instance.HealthPercent <= Program.UltMenu["UltSelf"].Cast<Slider>().CurrentValue)
                 {
                     R.Cast(Player.Instance);
                 }

                 if (lowestHealthAllies == null)
                 {
                     return;
                 }
                 

                 if (!(lowestHealthAllies.Health <= Program.UltMenu["UltAlly"].Cast<Slider>().CurrentValue))
                 {
                     return;
                 }
                 if (Program.UltMenu["autoUlt_" + lowestHealthAllies.BaseSkinName].Cast<CheckBox>().CurrentValue)
                 {
                     R.Cast(lowestHealthAllies);
                 }
             }



     }
}





        

