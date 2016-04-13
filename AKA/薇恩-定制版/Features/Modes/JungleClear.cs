using EloBuddy;
using System.Linq;
using EloBuddy.SDK;
using Auto_Carry_Vayne.Manager;

namespace Auto_Carry_Vayne.Features.Modes
{
    class JungleClear
    {
        public static void Load()
        {
            UseQ();
            UseE();
        }

        public static void UseQ()
        {
            if (Manager.MenuManager.UseQJC && Manager.SpellManager.Q.IsReady() && Variables.AfterAttack)
            {
                foreach (var Mob in EntityManager.MinionsAndMonsters.Monsters.Where(x => x.IsValid && !x.IsDead && x.Position.Distance(Variables._Player) < Variables._Player.GetAutoAttackRange(x)))
                {
                    Player.CastSpell(SpellSlot.Q, Game.CursorPos);
                }
            }
        }

        public static void UseE()
        {
            if (Manager.MenuManager.UseEJC && Manager.SpellManager.E.IsReady())
            {
                JungleCondemn();
            }
        }

        #region Jungle Condemn
        public static void JungleCondemn()
        {
            foreach (
                var jungleMobs in
                     ObjectManager.Get<Obj_AI_Minion>()
                        .Where(
                            o =>
                                o.IsValidTarget(Manager.SpellManager.E.Range) && o.Team == GameObjectTeam.Neutral && o.IsVisible &&
                                !o.IsDead))
            {
                if (jungleMobs.BaseSkinName == "SRU_Razorbeak" || jungleMobs.BaseSkinName == "SRU_Red" ||
                    jungleMobs.BaseSkinName == "SRU_Blue" || jungleMobs.BaseSkinName == "SRU_Dragon" ||
                    jungleMobs.BaseSkinName == "SRU_Krug" || jungleMobs.BaseSkinName == "SRU_Gromp" ||
                    jungleMobs.BaseSkinName == "Sru_Crab")
                {
                    var pushDistance = Manager.MenuManager.CondemnPushDistance;
                    var targetPosition = Manager.SpellManager.E2.GetPrediction(jungleMobs).UnitPosition;
                    var pushDirection = (targetPosition - Variables._Player.ServerPosition).Normalized();
                    float checkDistance = pushDistance / 40f;
                    for (int i = 0; i < 40; i++)
                    {
                        var finalPosition = targetPosition + (pushDirection * checkDistance * i);
                        var collFlags = NavMesh.GetCollisionFlags(finalPosition);
                        if (collFlags.HasFlag(CollisionFlags.Wall) || collFlags.HasFlag(CollisionFlags.Building))
                        {
                            Manager.SpellManager.E.Cast(jungleMobs);
                        }
                    }

                }
            }
            #endregion
        }
    }
}
