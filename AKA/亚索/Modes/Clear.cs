using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu.Values;
using SharpDX;
using EloBuddy.SDK.Events;

namespace AkaYasuo.Modes
{
    partial class Clear
    {
        public static void NewLane()
        {

            if (MenuManager.LaneClearMenu["Q"].Cast<CheckBox>().CurrentValue && Program.Q.IsReady() && (!Variables.HaveQ3 || MenuManager.LaneClearMenu["Q3"].Cast<CheckBox>().CurrentValue))
            {
                if (Variables._Player.IsDashing())
                {
                    var minionObj = (EntityManager.MinionsAndMonsters.GetLaneMinions(EntityManager.UnitTeam.Enemy,
                        Variables._Player.GetDashInfo().EndPos,
                        Variables.QCirWidth));

                    var monsterObj = (EntityManager.MinionsAndMonsters.GetJungleMonsters(Variables._Player.GetDashInfo().EndPos, Variables.QCirWidth));

                    if ((minionObj.Any(i => DamageManager._GetQDmg(i) >= i.Health || i.Team == GameObjectTeam.Neutral)
                         || minionObj.Count() > 1) && Variables._Player.Distance(Variables._Player.GetDashInfo().EndPos) < 150
                        )
                    {
                        Variables.CastQCir(minionObj.MinOrDefault(i => i.Distance(Variables._Player)));
                    }

                    if ((monsterObj.Any(i => DamageManager._GetQDmg(i) >= i.Health || i.Team == GameObjectTeam.Neutral)
                          || monsterObj.Count() > 1) && Variables._Player.Distance(Variables._Player.GetDashInfo().EndPos) < 150
                        )
                    {
                        Variables.CastQCir(monsterObj.MinOrDefault(i => i.Distance(Variables._Player)));
                    }

                }
                else
                {
                    var minionObj = EntityManager.MinionsAndMonsters.GetLaneMinions(EntityManager.UnitTeam.Enemy, Variables._Player.ServerPosition,
                        !Variables.HaveQ3 ? Variables.QRange : Variables.Q2Range).OrderByDescending(m => m.Health);

                    var monsterObj = EntityManager.MinionsAndMonsters.GetJungleMonsters(Variables._Player.ServerPosition,
    !Variables.HaveQ3 ? Variables.QRange : Variables.Q2Range).OrderByDescending(m => m.Health);

                    if (minionObj.Any())
                    {
                        if (!Variables.HaveQ3)
                        {
                            var obj = minionObj.FirstOrDefault(i => DamageManager._GetQDmg(i) >= i.Health);

                            if (obj != null)
                            {
                                Program.Q.Cast(obj.ServerPosition);
                            }
                        }
                        if (Variables.HaveQ3)
                        {
                            var obj = minionObj.FirstOrDefault(i => DamageManager._GetQDmg(i) >= i.Health);

                            if (obj != null)
                            {
                                Program.Q2.Cast(obj.ServerPosition);
                            }
                        }
                    }

                    if (monsterObj.Any())
                    {
                        if (!Variables.HaveQ3)
                        {
                            var obj = monsterObj.FirstOrDefault();

                            if (obj != null)
                            {
                                Program.Q.Cast(obj.ServerPosition);
                            }
                        }
                        if (Variables.HaveQ3)
                        {
                            var obj = monsterObj.FirstOrDefault();

                            if (obj != null)
                            {
                                Program.Q2.Cast(obj.ServerPosition);
                            }
                        }
                    }
                }
            }

            if (MenuManager.LaneClearMenu["E"].Cast<CheckBox>().CurrentValue && Program.E.IsReady())
            {
                var minionObj =
                    EntityManager.MinionsAndMonsters.GetLaneMinions(EntityManager.UnitTeam.Enemy, Variables._Player.ServerPosition, Program.E.Range).OrderByDescending(m => m.Health)
                        .Where(i => Variables.CanCastE(i) && !(Variables.PosAfterE(i).IsUnderTurret()))
                        .ToList();

                var monsterObj =
    EntityManager.MinionsAndMonsters.GetJungleMonsters(Variables._Player.ServerPosition, Program.E.Range).OrderByDescending(m => m.Health)
        .Where(i => Variables.CanCastE(i) && !(Variables.PosAfterE(i).IsUnderTurret()))
        .ToList();

                if (minionObj.Any())
                {
                    var obj = minionObj.FirstOrDefault(i => DamageManager._GetEDmg(i) >= i.Health);
                    if (obj != null)
                    {
                        Program.E.Cast(obj);
                    }
                }

                if (monsterObj.Any())
                {
                    var obj = monsterObj.FirstOrDefault(i => DamageManager._GetEDmg(i) >= i.Health);
                    if (obj != null)
                    {
                        Program.E.Cast(obj);
                    }
                }
            }

            if (MenuManager.LaneClearMenu["Items"].Cast<CheckBox>().CurrentValue)
            {
                foreach (Obj_AI_Base minion in EntityManager.MinionsAndMonsters.GetLaneMinions(EntityManager.UnitTeam.Enemy, Variables._Player.Position, Program.Q.Range, true).OrderByDescending(m => m.Health))
                {
                    Items.UseItems(minion);
                }

                foreach (Obj_AI_Base minion in EntityManager.MinionsAndMonsters.GetJungleMonsters(Variables._Player.Position, Program.Q.Range, true).OrderByDescending(m => m.Health))
                {
                    Items.UseItems(minion);
                }
            } 
        }
    }
}
