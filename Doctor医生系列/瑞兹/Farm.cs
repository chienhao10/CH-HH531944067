using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK.Rendering;
using SharpDX;
namespace Ryze
{
    public class Farm
    {
        public static void LastHit()
        {
            foreach (Obj_AI_Base minion in EntityManager.MinionsAndMonsters.GetLaneMinions(EntityManager.UnitTeam.Enemy, Program.myHero.ServerPosition, Program.Q.Range, true))
            {
                if (Program.ManaPercent > Program.SubMenu["LastHit"]["LastHitMana"].Cast<Slider>().CurrentValue)
                {
                    if (Program.SubMenu["LastHit"]["Q"].Cast<CheckBox>().CurrentValue && Program.Q.IsReady() && minion.IsValidTarget())
                    {
                        var linepred = Prediction.Position.PredictLinearMissile(minion, 900, 90, 250, 1500, 1);
                        var QPred = Program.Q.GetPrediction(minion);
                        var collision = QPred.GetCollisionObjects<Obj_AI_Minion>().Where(m => m.IsMinion && !m.IsDead);
                        if (collision.Count() < 1 && (Program.myHero.GetSpellDamage(minion, SpellSlot.Q) >= minion.Health))
                        {
                            Program.Q.Cast(linepred.CastPosition);
                        }
                    }
                    if (Program.SubMenu["LastHit"]["W"].Cast<CheckBox>().CurrentValue && Program.W.IsReady() && minion.IsValidTarget(Program.W.Range))
                    {
                        if (Program.myHero.GetSpellDamage(minion, SpellSlot.W) >= minion.Health)
                        {
                            Program.W.Cast(minion);
                        }
                    }
                    if (Program.SubMenu["LastHit"]["E"].Cast<CheckBox>().CurrentValue && Program.E.IsReady() && minion.IsValidTarget(Program.E.Range))
                    {
                        if (Program.myHero.GetSpellDamage(minion, SpellSlot.E) >= minion.Health)
                        {
                            Program.E.Cast(minion);
                        }
                    }
                }
            }
        }

        public static void LaneClear()
        {
            foreach (var minion in EntityManager.MinionsAndMonsters.GetLaneMinions(EntityManager.UnitTeam.Enemy, Program.myHero.Position, Program.Q.Range))
            {
                if (Program.SubMenu["LaneClear"]["Q"].Cast<CheckBox>().CurrentValue && Program.Q.IsReady() && minion.IsValidTarget())
                {
                    if ((Program.myHero.GetSpellDamage(minion, SpellSlot.Q) >= minion.Health))
                    {
                        Program.Q.Cast(minion);
                    }
                    else if (Program.Q.IsReady())
                    {
                        Program.Q.Cast(minion);
                    }
                }
                if (Program.SubMenu["LaneClear"]["W"].Cast<CheckBox>().CurrentValue && Program.W.IsReady() && minion.IsValidTarget(Program.W.Range))
                {
                    if (Program.myHero.GetSpellDamage(minion, SpellSlot.W) >= minion.Health)
                    {
                        Program.W.Cast(minion);
                    }
                    else if (Program.W.IsReady())
                    {
                        Program.W.Cast(minion);
                    }
                }
                if (Program.SubMenu["LaneClear"]["E"].Cast<CheckBox>().CurrentValue && Program.E.IsReady() && minion.IsValidTarget(Program.E.Range))
                {
                    if (Program.myHero.GetSpellDamage(minion, SpellSlot.E) >= minion.Health)
                    {
                        Program.E.Cast(minion);
                    }
                    else if (Program.E.IsReady())
                    {
                        Program.E.Cast(minion);
                    }
                }
                if (Program.SubMenu["LaneClear"]["R"].Cast<CheckBox>().CurrentValue && Program.R.IsReady() && Program.myHero.Distance(minion) <= Program.W.Range)
                {
                    Program.R.Cast();
                }
            }
        }

        public static void JungleClear()
        {
            Obj_AI_Base jungle = EntityManager.MinionsAndMonsters.GetJungleMonsters(Program.myHero.Position, Program.Q.Range, true).FirstOrDefault();
            {
                var useQ = Program.SubMenu["JungleClear"]["Q"].Cast<CheckBox>().CurrentValue;
                var useW = Program.SubMenu["JungleClear"]["W"].Cast<CheckBox>().CurrentValue;
                var useE = Program.SubMenu["JungleClear"]["E"].Cast<CheckBox>().CurrentValue;
                var useR = Program.SubMenu["JungleClear"]["R"].Cast<CheckBox>().CurrentValue;

                if (jungle != null && jungle.IsValidTarget())
                {
                    if (useW && Program.W.IsReady())
                        Program.W.Cast(jungle);
                    else if (!Program.W.IsReady() && useQ && Program.Q.IsReady())
                        Program.Q.Cast(jungle);
                    else if (!Program.Q.IsReady() && useE && Program.E.IsReady())
                        Program.E.Cast(jungle);
                    else if (!Program.E.IsReady() && useR && Program.R.IsReady() && Program.myHero.GetBuffCount("ryzepassivestack") == 4 || Program.myHero.GetBuffCount("ryzepassivestack") == 2 || Program.myHero.GetBuffCount("ryzepassivestack") == 3)
                        Program.R.Cast();
                    else if (!Program.R.IsReady() && useW && Program.W.IsReady())
                        Program.W.Cast(jungle);
                    else if (!Program.W.IsReady() && useQ && Program.Q.IsReady())
                        Program.Q.Cast(jungle);
                    else if (!Program.Q.IsReady() && useE && Program.E.IsReady())
                        Program.E.Cast(jungle);
                    else if (!Program.E.IsReady() && useQ && Program.Q.IsReady())
                        Program.Q.Cast(jungle);
                    else if (!Program.Q.IsReady() && useW && Program.W.IsReady())
                        Program.W.Cast(jungle);
                    else if (!Program.W.IsReady() && useQ && Program.Q.IsReady())
                        Program.Q.Cast(jungle);
                    else if (!Program.Q.IsReady() && useE && Program.E.IsReady())
                        Program.E.Cast(jungle);
                    else if (!Program.E.IsReady() && useQ && Program.Q.IsReady())
                        Program.Q.Cast(jungle);
                    else if (!Program.Q.IsReady() && useW && Program.W.IsReady())
                        Program.W.Cast(jungle);
                    else if (!Program.W.IsReady() && useQ && Program.Q.IsReady())
                        Program.Q.Cast(jungle);
                    else if (!Program.Q.IsReady() && useE && Program.E.IsReady())
                        Program.E.Cast(jungle);
                    else if (!Program.E.IsReady() && useQ && Program.Q.IsReady())
                        Program.Q.Cast(jungle);
                    else if (!Program.Q.IsReady() && useW && Program.W.IsReady())
                        Program.W.Cast(jungle);
                    else if (!Program.W.IsReady() && useQ && Program.Q.IsReady())
                        Program.Q.Cast(jungle);
                    else if (!Program.Q.IsReady() && useE && Program.E.IsReady())
                        Program.E.Cast(jungle);
                    else if (!Program.E.IsReady() && useQ && Program.Q.IsReady())
                        Program.Q.Cast(jungle);
                    else if (!Program.Q.IsReady() && useW && Program.W.IsReady())
                        Program.W.Cast(jungle);
                    else if (!Program.W.IsReady() && useQ && Program.Q.IsReady())
                        Program.Q.Cast(jungle);
                    else if (!Program.Q.IsReady() && useE && Program.E.IsReady())
                        Program.E.Cast(jungle);
                    else if (!Program.E.IsReady() && useQ && Program.Q.IsReady())
                        Program.Q.Cast(jungle);
                    else if (!Program.Q.IsReady() && useW && Program.W.IsReady())
                        Program.W.Cast(jungle);
                    else if (!Program.W.IsReady() && useQ && Program.Q.IsReady())
                        Program.Q.Cast(jungle);
                    else if (!Program.Q.IsReady() && useE && Program.E.IsReady())
                        Program.E.Cast(jungle);
                    else if (!Program.E.IsReady() && useQ && Program.Q.IsReady())
                        Program.Q.Cast(jungle);
                    else if (!Program.Q.IsReady() && useW && Program.W.IsReady())
                        Program.W.Cast(jungle);
                }
            }
        }
    }
}
