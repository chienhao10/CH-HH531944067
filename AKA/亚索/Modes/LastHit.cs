
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu.Values;

namespace AkaYasuo.Modes
{
    partial class LastHit
    {
        public static void Lasthitmode()
        {
            if (MenuManager.LastHitMenu["Q"].Cast<CheckBox>().CurrentValue && Program.Q.IsReady() && !Variables._Player.IsDashing()
    && (!Variables.HaveQ3 || MenuManager.LastHitMenu["Q3"].Cast<CheckBox>().CurrentValue))
            {
                var obj =
                    EntityManager.MinionsAndMonsters.GetLaneMinions(EntityManager.UnitTeam.Enemy, Variables._Player.ServerPosition,
                        !Variables.HaveQ3 ? Variables.QRange : Variables.Q2Range).OrderByDescending(m => m.Health).FirstOrDefault(i => DamageManager._GetEDmg(i) >= i.Health);
                if (obj != null)
                {
                   (!Variables.HaveQ3 ? Program.Q : Program.Q2).Cast(obj);
                }
            }
            if (MenuManager.LastHitMenu["E"].Cast<CheckBox>().CurrentValue && Program.E.IsReady())
            {
                var obj =
                    EntityManager.MinionsAndMonsters.GetLaneMinions(EntityManager.UnitTeam.Enemy, Variables._Player.ServerPosition, Program.E.Range).OrderByDescending(m => m.Health)
                        .Where(
                            i =>
                            Variables.CanCastE(i)
                            && (!Variables._Player.IsInAutoAttackRange(i) || i.Health > Variables._Player.GetAutoAttackDamage(i, true)))
                        .FirstOrDefault(i => DamageManager._GetEDmg(i) >= i.Health);
                if (obj != null)
                {
                    Program.E.Cast(obj);
                }
            }
        }
    }
}
