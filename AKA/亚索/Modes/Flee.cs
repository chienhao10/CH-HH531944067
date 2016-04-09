using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK.Events;

namespace AkaYasuo.Modes
{
    partial class Flee
    {
        public static void Flee2()
        {
            if (MenuManager.FleeMenu["EscQ"].Cast<CheckBox>().CurrentValue && Program.Q.IsReady() && !Variables.HaveQ3 && Variables._Player.IsDashing())
            {
                if (Variables.QCirTarget != null)
                {
                    Variables.CastQCir(Variables.QCirTarget);
                }
                var minionObj = EntityManager.MinionsAndMonsters.GetLaneMinions(EntityManager.UnitTeam.Enemy,
                    Variables._Player.GetDashInfo().EndPos,
                    Variables.QCirWidth);
                if (minionObj.Any() && Variables._Player.Distance(Variables._Player.GetDashInfo().EndPos) < 150
                    )
                {
                    Variables.CastQCir(minionObj.MinOrDefault(i => i.Distance(Variables._Player)));
                }
            }
            var obj = Variables.GetNearObj();
            if (obj == null || !Program.E.IsReady())
            {
                return;
            }
            Program.E.Cast(obj);
        }
    }
}
