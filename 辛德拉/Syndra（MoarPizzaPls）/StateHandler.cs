using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu.Values;
using System.Collections;
using SharpDX;
using EloBuddy.SDK.Enumerations;

namespace Syndra
{
    class StateHandler
    {
        public static AIHeroClient _Player { get { return ObjectManager.Player; } }
        public static bool debug;
        public static Vector3 qe;
        public static bool qeb = false;
        public static List<AIHeroClient> antiOverkill = new List<AIHeroClient>();
        private static AIHeroClient qTarget;
        private static AIHeroClient wTarget;
        private static AIHeroClient qeTarget;
        private static AIHeroClient rTarget;
        private static float comboDamage;
        public static float GetDynamicRange()
        {
            if (Program.Q.IsReady())
            {
                return Program.Q.Range;
            }
            return _Player.GetAutoAttackRange();
        }

        public static void LastHit()
        {
            if (!Player.Instance.CanCast)
            {
                if(debug) Chat.Print("Can't cast.");
                return;
            }
           // var rangedMinionsQ = EntityManager.GetLaneMinions(EntityManager.UnitTeam.Enemy, Player.Instance.ServerPosition.To2D(), Program.Q.Range + Program.Q.Width + 30).Where(a => a.IsRanged);
            var allMinionsQ = EntityManager.MinionsAndMonsters.GetLaneMinions(EntityManager.UnitTeam.Enemy, Player.Instance.ServerPosition, Program.Q.Range + Program.Q.Width + 30);
           // var rangedMinionsW = EntityManager.GetLaneMinions(EntityManager.UnitTeam.Enemy, Player.Instance.ServerPosition.To2D(), Program.W.Range + Program.W.Width + 30).Where(a => a.IsRanged);
           // var allMinionsW = EntityManager.GetLaneMinions(EntityManager.UnitTeam.Enemy, Player.Instance.ServerPosition.To2D(), Program.Q.Range + Program.Q.Width + 30);

            var useQ = Config.Modes.Farm.UseQ;
            if (useQ && Program.Q.IsReady())
            {
                foreach (Obj_AI_Minion minion in allMinionsQ)
                {
                    if ((!Player.Instance.IsInAutoAttackRange(minion) || (!Orbwalker.CanAutoAttack && Orbwalker.LastTarget.NetworkId != minion.NetworkId)) && (minion.Health < 0.8 * QDamage(minion)))
                    {
                        Program.Q.Cast(minion);
                        break;
                    }
                }
            }
        }

        public static void WaveClear()
        {
        }

        public static float QDamage(Obj_AI_Base target)
        {

            if (Program.Q.Level < 5)
                return _Player.CalculateDamageOnUnit(target, DamageType.Magical,
                    (float) Program.qDamage[Program.Q.Level-1] + 0.6f*_Player.FlatMagicDamageMod);
            else
                return _Player.CalculateDamageOnUnit(target, DamageType.Magical,
                    (float)Program.qDamage[Program.Q.Level - 1] + 0.69f * _Player.FlatMagicDamageMod);
        }

        public static float WDamage(Obj_AI_Base target)
        {
            return _Player.CalculateDamageOnUnit(target, DamageType.Magical,
                (float) Program.wDamage[Program.W.Level-1] + 0.7f*_Player.FlatMagicDamageMod);
        }

        public static float EDamage(Obj_AI_Base target)
        {
            return _Player.CalculateDamageOnUnit(target, DamageType.Magical,
                (float) Program.eDamage[Program.E.Level-1] + 0.4f*_Player.FlatMagicDamageMod);
        }

        public static float RDamage(Obj_AI_Base target)
        {
            int ammo = ObjectManager.Player.Spellbook.GetSpell(SpellSlot.R).Ammo;
            return _Player.CalculateDamageOnUnit(target, DamageType.Magical,
                (float) Program.rDamage[Program.R.Level-1]*ammo + 0.2f*_Player.FlatMagicDamageMod*ammo);
        }


        private static void UseSpells(bool useQ, bool useW, bool useE, bool useR, bool useQE, bool useIgnite, bool isAAHarass, AIHeroClient forcedEnemy = null)
        {
            //Chat.Print(qeb);

            debug = Config.Misc.Menu["debug"].Cast<CheckBox>().CurrentValue;
            if (instaE())
                return;
           
            if (forcedEnemy != null && debug)
                Chat.Print("Forcing " + forcedEnemy.Name);
            qTarget = !Program.Q.IsReady() ? null : ( forcedEnemy!=null ? forcedEnemy : TargetSelector.GetTarget(Program.Q.Range + Program.Q.Width, DamageType.Magical));
            wTarget = !Program.W.IsReady() ? null : ( forcedEnemy!=null ? forcedEnemy : TargetSelector.GetTarget(Program.W.Range, DamageType.Magical));
            rTarget = ( forcedEnemy!=null ? forcedEnemy : TargetSelector.GetTarget(Program.R.Range, DamageType.Magical));
            qeTarget = !Program.E.IsReady() ? null : ( forcedEnemy!=null ? forcedEnemy : TargetSelector.GetTarget(Program.EQ.Range, DamageType.Magical));

            comboDamage = rTarget != null ? GetComboDamage(rTarget) : 0;

            //QE
            if (qeTarget != null && Program.Q.IsReady() && Program.E.IsReady() && useQE && (Program.tickCount - Program.QECombo > 15) && (Program.tickCount - Program.LastWGrabAttempt > 15))
            {
                UseQE(qeTarget);
                return;
            }

            //E
            if (qeTarget != null && Program.tickCount - Program.LastWGrabAttempt > 30 && Program.E.IsReady() && useE && (Program.tickCount - Program.QECombo > 15))
            {
                foreach (var enemy in ObjectManager.Get<AIHeroClient>().Where(a => a.IsEnemy == true))
                {
                    if (enemy.IsValidTarget(Program.EQ.Range))
                    {
                        if(UseE(enemy))
                            return;
                    }
                }
            }
            
            // if(debug) Chat.Print(qTarget.Name);
            //Q
            if (qTarget != null && useQ && Program.Q.IsReady() && (Program.tickCount - Program.LastQCastAttempt > 15))
            {

                Program.LastQCastAttempt = Program.tickCount;

                if (isAAHarass)
                {
                    Program.Q.Cast(qTarget.ServerPosition);
                    if(debug) Chat.Print("Q AA harass");
                }
                //Chat.Print(_Player.Distance(Prediction.Position.PredictUnitPosition(qTarget, 600)));
                else if (Program.Q.Range + Program.Q.Radius > _Player.Distance(Prediction.Position.PredictUnitPosition(qTarget, Program.Q.CastDelay))
                    && _Player.Distance(Prediction.Position.PredictUnitPosition(qTarget, Program.Q.CastDelay)) > Program.Q.Range)
                {
                    Program.Q.Cast(_Player.ServerPosition.To2D().Extend(Prediction.Position.PredictUnitPosition(qTarget, Program.Q.CastDelay), Program.Q.Range).To3D());
                    if (debug) Chat.Print("Over max range Q ");
                    return;
                }
                else if (_Player.Distance(Prediction.Position.PredictUnitPosition(qTarget, Program.Q.CastDelay)) <= Program.Q.Range)
                {
                    Program.Q.Cast(Prediction.Position.PredictUnitPosition(qTarget, Program.Q.CastDelay).To3D());
                    if (debug) Chat.Print("Under max range Q");
                    return;
                }
                else
                    if (debug) Chat.Print("Target Q OOR");
                
            }
            //W
            if (useW && wTarget != null)
            {
                SpellDataInst data = ObjectManager.Player.Spellbook.GetSpell(SpellSlot.W);
                if (data.ToggleState == 1 && Program.W.IsReady() && (Program.tickCount - Program.LastWGrabAttempt > 30) && ((Program.tickCount - Program.QECombo > 30)))
                {
                    if (debug) Chat.Print("Grabbing shit");
                    var gObjectPos = GetGrabableObjectPos(false);
                    Program.W.Cast(gObjectPos);
                    Program.LastWGrabAttempt = Program.tickCount;
                    return;
                }
                else if (wTarget != null && data.ToggleState == 2 && Program.W.IsReady() && (Program.tickCount - Program.LastWCastAttempt > 30) /*&&
                    ((Program.tickCount - Program.QECombo > 30) || (!Program.E.IsReady()))*/ && OrbManager.Get_Current_Orb()!=null)
                {
                    if (debug)
                        Chat.Print("IN W CAST");
                    Program.W.SourcePosition = OrbManager.Get_Current_Orb().ServerPosition;
                    if(debug) Chat.Print("Plain W");

                    if (isAAHarass)
                    {
                        Program.W.Cast(wTarget.ServerPosition);
                        if(debug) Chat.Print("W AA harass");
                    }
                    //Chat.Print(_Player.Distance(Prediction.Position.PredictUnitPosition(qTarget, 600)));
                    else if (Program.W.Range + Program.W.Radius > _Player.Distance(Prediction.Position.PredictUnitPosition(wTarget, Program.W.CastDelay))
                        && _Player.Distance(Prediction.Position.PredictUnitPosition(wTarget, Program.W.CastDelay)) > Program.W.Range)
                    {
                        Program.W.Cast(_Player.ServerPosition.To2D().Extend(Prediction.Position.PredictUnitPosition(wTarget, Program.W.CastDelay), Program.W.Range).To3D());
                        if (debug) Chat.Print("Over max range W ");
                        return;
                    }
                    else if (_Player.Distance(Prediction.Position.PredictUnitPosition(wTarget, Program.W.CastDelay)) <= Program.W.Range)
                    {
                        Program.W.Cast(Prediction.Position.PredictUnitPosition(wTarget, Program.W.CastDelay).To3D());
                        if (debug) Chat.Print("Under max range W");
                        return;
                    }
                    else
                        if (debug) Chat.Print("Target W OOR");

                    Program.LastWCastAttempt = Program.tickCount;
                    return;
                }
            }


            //R
            if (rTarget != null && (Program.tickCount - Program.LastRCastAttempt > 15))
            {
                useR = (!Config.Misc.Menu["DontUlt" + rTarget.BaseSkinName].Cast<CheckBox>().CurrentValue && useR);
                //useR = !antiOverkill.Contains(rTarget) && useR;
                //if(debug) Chat.Print("R1 PRE");
                if (useR && (comboDamage > rTarget.Health)/* && (comboDamage - RDamage(rTarget) < rTarget.Health)*/ && Program.R.IsReady() && !rTarget.IsInvulnerable && !rTarget.IsZombie)
                {
                    if(debug) Chat.Print("R1");
                    Program.R.Cast(rTarget);
                    Program.LastRCastAttempt = Program.tickCount;
                    return;
                }
              /*  else if (useR && (comboDamage > rTarget.Health) && (comboDamage - RDamage(rTarget) > rTarget.Health) && Program.R.IsReady() && !rTarget.IsInvulnerable && !rTarget.IsZombie)
                {
                    antiOverkill.Add(rTarget);
                }*/
            }



            //Ignite
            if (Program.gotIgnite && useIgnite && useR && rTarget != null && Program.ignite != SpellSlot.Unknown &&
                _Player.Spellbook.CanUseSpell(Program.ignite) == SpellState.Ready && !rTarget.IsInvulnerable && !rTarget.IsZombie && EntityManager.Heroes.Allies.Count(m => m.Distance(rTarget) < 400 && !m.IsMe) <=1)
            {
                if (comboDamage < rTarget.Health && comboDamage + ObjectManager.Player.GetSummonerSpellDamage(rTarget, DamageLibrary.SummonerSpells.Ignite) > rTarget.Health)
                {
                    _Player.Spellbook.CastSpell(Program.ignite, rTarget);
                    Program.R.Cast(rTarget);
                    return;
                }
            }
            
            //WE
           /* if (wTarget == null && qeTarget != null && Program.E.IsReady() && useE && OrbManager.Grab_Shit(true) != null)
            {
                if(debug) Chat.Print("Use WE");
               // Program.EQ.CastDelay = Program.E.CastDelay + (int)Program.Q.Range / Program.W.Speed;
                Program.EQ.SourcePosition = _Player.ServerPosition.To2D().Extend(qeTarget.ServerPosition.To2D(), Program.Q.Range-200).To3D();
                var prediction = Program.EQ.GetPrediction(qeTarget);
                if (prediction.HitChance >= HitChance.High)
                {
                    Program.W.Cast(_Player.ServerPosition.To2D().Extend(prediction.CastPosition.To2D(), Program.Q.Range - 200).To3D());
                    Program.WECombo = Program.tickCount;
                    return;
                }
            }*/
        }

        private static bool instaE()
        {
            if (qeb && Program.E.IsReady() && Vector3.Distance(qe, _Player.ServerPosition) < Program.Q.Range && !Program.Q.IsReady() && Program.tickCount - Program.QECombo < 30) //note to myself: yes, Q range. E has extended range for ballz
            {
                Program.E.Cast(qe);
                Program.QECombo = Program.tickCount;
                if (debug) Chat.Print("insta E");
                return true;
            }
            else if (qeb && (Program.Q.IsReady() || !Program.E.IsReady()))
            {
                if (debug) Chat.Print("E down or Q still up.");
                qeb = false;
            }
            else if (qeb)
            {
                if (debug) Chat.Print("No Q to E for a long time");
                qeb = false;
            }
            return false;
        }

        private static Vector3 GetGrabableObjectPos(bool onlyOrbs)
        {
            if (!onlyOrbs)
                foreach (var minion in ObjectManager.Get<Obj_AI_Minion>().Where(minion => minion.IsValidTarget(Program.W.Range)))
                    return minion.ServerPosition;

            return OrbManager.Grab_Shit().ServerPosition;
        }
        
        public static float GetComboDamage(Obj_AI_Base enemy)
        {
            var damage = 0f;

            if (Program.Q.IsReady())
                damage += QDamage(enemy);

            if (Program.W.IsReady())
                damage += WDamage(enemy);

            if (Program.E.IsReady())
                damage += EDamage(enemy);

            //if (Program.ignite!= SpellSlot.Unknown && _Player.Spellbook.CanUseSpell(Program.ignite) == SpellState.Ready)
              //  damage += ObjectManager.Player.GetSummonerSpellDamage(enemy, DamageLibrary.SummonerSpells.Ignite);
            
            if (!Config.Misc.Menu["DontUlt" + enemy.BaseSkinName].Cast<CheckBox>().CurrentValue && Program.R.IsReady())
                damage += RDamage(enemy);

            //if(debug) Chat.Print(damage + " damage");
            return (float)damage;
        }

        public static float GetComboDamageConsiderRange(AIHeroClient enemy)
        {
            var damage = 0f;

            if (Program.Q.IsReady() && Program.Q.Range + Program.Q.Radius > _Player.Distance(Prediction.Position.PredictUnitPosition(enemy, Program.Q.CastDelay)))
                damage += QDamage(enemy);

            if (Program.W.IsReady() && Program.W.Range + Program.W.Radius > _Player.Distance(Prediction.Position.PredictUnitPosition(enemy, Program.W.CastDelay)))
                damage += WDamage(enemy);

            if (Program.E.IsReady() && Program.E.Range + Program.E.Radius > _Player.Distance(Prediction.Position.PredictUnitPosition(enemy, Program.E.CastDelay)))
                damage += EDamage(enemy);

            //if (Program.ignite!= SpellSlot.Unknown && _Player.Spellbook.CanUseSpell(Program.ignite) == SpellState.Ready)
            //  damage += ObjectManager.Player.GetSummonerSpellDamage(enemy, DamageLibrary.SummonerSpells.Ignite);
            SpellDataInst data = ObjectManager.Player.Spellbook.GetSpell(SpellSlot.R);
            if (!Config.Misc.Menu["DontUlt" + enemy.BaseSkinName].Cast<CheckBox>().CurrentValue 
                && Program.R.IsReady() && Program.R.Range > _Player.Distance(enemy))
                damage += RDamage(enemy);

            //if(debug) Chat.Print(damage + " damage");
            return (float)damage;
        }

        public static void UseQE(Obj_AI_Base enemy, bool forcedEnemy = false)
        {
            if(debug) Chat.Print("in qe");
            //Program.EQ.CastDelay = (int)Program.E.CastDelay;// +(int)(_Player.Distance(enemy) / Program.EQ.Speed * 1000); //Program.Q.CastDelay + (int)/*Program.Q.Range */ _Player.Distance(enemy)/ Program.EQ.Speed * 100;
            Program.EQ.SourcePosition= _Player.ServerPosition;//.To2D().Extend(enemy.ServerPosition.To2D(), Program.Q.Range - 200).To3D();

            var prediction = Program.EQ.GetPrediction(enemy);

            if (debug)
            {
                Chat.Print(enemy.Name + " EQ PREDICTION RESULT: " + prediction.HitChance);
                //Chat.Print("EQ delay = " + Program.EQ.CastDelay);
            }


            if (forcedEnemy)
                if (Program.Q.Range + Program.Q.Radius < _Player.Distance(prediction.CastPosition))
                    return;

            //var startPoint = prediction.CastPosition.To2D();//.Extend(_Player.ServerPosition.To2D(), 100); //_Player.ServerPosition.To2D();//
            var endPoint = _Player.ServerPosition.To2D()
                .Extend(prediction.CastPosition.To2D(), Program.EQ.Range /*Vector3.Distance(_Player.Position, orb) > 200 ? 1300 : 1000*/);

            if ((prediction.HitChance == HitChance.High || prediction.HitChance == HitChance.Collision || prediction.HitChance == HitChance.Medium) &&
                prediction.UnitPosition.To2D().Distance(_Player.ServerPosition.To2D(), endPoint, false) <
                        Program.EQ.Width + enemy.BoundingRadius)
            {

                if(debug) Chat.Print("Use QE");
                if (prediction.CastPosition.Distance(_Player) < Program.Q.Range)
                    Program.Q.Cast(prediction.CastPosition);
                else
                    Program.Q.Cast(_Player.ServerPosition.To2D().Extend(prediction.CastPosition.To2D(), (float)(Program.Q.Range - 150)).To3D());
                //Program.E.Cast(_Player.ServerPosition.To2D().Extend(prediction.CastPosition.To2D(), (float)(Program.E.Range - 200)).To3D());
                qe = _Player.ServerPosition.To2D().Extend(prediction.CastPosition.To2D(), (float)(Program.Q.Range-150)).To3D();
                qeb = true;
                Program.QECombo = Program.tickCount;
            }
        }

        private static bool UseE(Obj_AI_Base enemy)
        {
            foreach (var orb in OrbManager.GetAllOrbs())
                if (Vector3.Distance(_Player.ServerPosition, orb.ServerPosition) < Program.Q.Range)
                {
                    var startPoint = _Player.ServerPosition.To2D();// orb.ServerPosition.To2D();//.Extend(_Player.ServerPosition.To2D(), 100); //_Player.ServerPosition.To2D();//
                    var endPoint = _Player.ServerPosition.To2D()
                        .Extend(orb.ServerPosition.To2D(), Program.EQ.Range /*Vector3.Distance(_Player.Position, orb) > 200 ? 1300 : 1000*/);
                    //Program.EQ.CastDelay = (int)Program.E.CastDelay;// +(int)(_Player.Distance(enemy) / Program.EQ.Speed * 1000);//(int)Vector3.Distance(_Player.Position, orb) / Program.E.Speed;
                    Program.EQ.SourcePosition = _Player.ServerPosition;//orb;
                    var enemyPred = Program.EQ.GetPrediction(enemy);

                    if (debug)
                    {
                        Chat.Print(enemy.Name + " E PREDICTION RESULT: " + enemyPred.HitChance);
                        Chat.Print("EQ delay = " + Program.EQ.CastDelay);
                    }

                    if ((enemyPred.HitChance == HitChance.High || enemyPred.HitChance == HitChance.Collision || enemyPred.HitChance == HitChance.Medium) &&
                        enemyPred.UnitPosition.To2D().Distance(startPoint, endPoint, false) <
                        Program.EQ.Width + enemy.BoundingRadius)
                    {
                        //preventing reverse E,hopefully
                        double deltaY = enemyPred.UnitPosition.Y - _Player.Position.Y;
                        double deltaX = enemyPred.UnitPosition.X - _Player.Position.X;
                        double angleInDegreesEnemy = Math.Atan2(deltaY, deltaX) * 180 / Math.PI;
                        deltaY = orb.ServerPosition.Y - _Player.Position.Y;
                        deltaX = orb.ServerPosition.X - _Player.Position.X;
                        double angleInDegreesOrb = Math.Atan2(deltaY, deltaX) * 180 / Math.PI;
                        angleInDegreesEnemy += 360;
                        angleInDegreesOrb += 360;
                        angleInDegreesEnemy %= 360;
                        angleInDegreesOrb %= 360;
                        if (Math.Abs(angleInDegreesEnemy - angleInDegreesOrb) > 45)
                        {
                            if (debug)
                                Chat.Print("Prevented a weird angle E: " + Math.Abs(angleInDegreesEnemy - angleInDegreesOrb));
                                continue;
                        }

                        Program.QECombo = Program.tickCount;
                        if(debug) Chat.Print("Use E " + enemy.Name + " " + Program.EQ.Range);
                        Program.E.Cast(orb.ServerPosition);
                        qe = orb.ServerPosition;
                        qeb = true;
                        return true;
                    }
                }
            return false;
        }

        public static void Combo(AIHeroClient forcedTarged = null)
        {
            UseSpells(true, true, true, true, true, Config.Modes.Combo.UseIgnite, false, forcedTarged);

            //UseSpells(false,false, true, false, true, Config.Modes.Combo.UseIgnite, false);
        }

        public static void Harass(bool auto)
        {
            UseSpells(Config.Modes.Harass.UseQ, Config.Modes.Harass.UseW, Config.Modes.Harass.UseE, false, Config.Modes.Harass.UseE, false, auto);
        }

        public static void FinishW()
        {

            var wTarget = !Program.W.IsReady() ? null : TargetSelector.GetTarget(Program.W.Range, DamageType.Magical);

            if (wTarget != null)
            {
                //if(debug) Chat.Print(wTarget.Name);
                SpellDataInst data = ObjectManager.Player.Spellbook.GetSpell(SpellSlot.W);
                if (wTarget != null && data.ToggleState == 2 && Program.W.IsReady() && (Program.tickCount - Program.LastWCastAttempt > 15) /*&&
                    ((Program.tickCount - Program.QECombo > 30) || (!Program.E.IsReady()))*/)
                {
                    if (OrbManager.Get_Current_Orb()!=null)
                    {
                        //Program.W.SourcePosition = OrbManager.WObject(false).ServerPosition;
                        Program.W.Cast(wTarget);
                        if (debug) Chat.Print("Finish W");
                        Program.LastWCastAttempt = Program.tickCount;
                    }
                }
            }
        }

        internal static void CheckForKS()
        {
           /* var qTarget = !Program.Q.IsReady() ? null : TargetSelector.GetTarget(Program.Q.Range + Program.Q.Width, DamageType.Magical);
            var wTarget = !Program.W.IsReady() ? null : TargetSelector.GetTarget(Program.W.Range, DamageType.Magical);
            var rTarget = TargetSelector.GetTarget(Program.R.Range, DamageType.Magical);
            var qeTarget = !Program.E.IsReady() ? null : TargetSelector.GetTarget(Program.EQ.Range, DamageType.Magical);*/

            var inRange = EntityManager.Heroes.Enemies.Where<AIHeroClient>(a => a.Distance(_Player) < Program.EQ.Range && !a.IsInvulnerable && !a.IsZombie && !a.IsDead && a.IsValidTarget());

            foreach(AIHeroClient enemy in inRange)
            {
                try
                {
                    if (GetComboDamageConsiderRange(enemy) >= enemy.Health)
                    {
                        if (debug) Chat.Print("Killable " + enemy.Name);
                        Combo(enemy);
                    };
                }
                catch (Exception e)
                {
                    if (debug)
                        Console.WriteLine(e.StackTrace);
                }
            }

        }
    }
}
