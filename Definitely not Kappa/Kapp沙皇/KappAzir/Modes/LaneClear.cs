namespace KappAzir.Modes
{
    using System.Linq;

    using EloBuddy;
    using EloBuddy.SDK;

    using KappAzir.Utility;

    internal class LaneClear
    {
        public static void Execute()
        {
            var minions = EntityManager.MinionsAndMonsters.EnemyMinions.OrderByDescending(m => m.MaxHealth).Where(m => m.IsKillable());
            if (minions != null)
            {
                var menu = Menus.LaneClearMenu;
                var Q = Azir.Q.IsReady() && menu.checkbox("Q") && Player.Instance.ManaPercent > menu.slider("Qmana");
                var W = Azir.W.IsReady() && menu.checkbox("W") && Player.Instance.ManaPercent > menu.slider("Wmana");
                var wsave = menu.checkbox("Wsave") && Azir.W.Handle.Ammo < 2;

                var objAiMinions = minions as Obj_AI_Minion[] ?? minions.ToArray();

                var bestQ = EntityManager.MinionsAndMonsters.GetCircularFarmLocation(
                    objAiMinions.ToArray(),
                    Orbwalker.AzirSoldierAutoAttackRange,
                    (int)Azir.Q.Range);

                var bestW = EntityManager.MinionsAndMonsters.GetCircularFarmLocation(
                    objAiMinions.ToArray(),
                    Orbwalker.AzirSoldierAutoAttackRange,
                    (int)Azir.W.Range);

                if (W && bestW.HitNumber > 0 && bestW.CastPosition != null)
                {
                    if (wsave)
                    {
                        return;
                    }

                    Azir.W.Cast(bestW.CastPosition);
                }

                if (Q && bestQ.HitNumber > 0 && bestQ.CastPosition != null)
                {
                    Azir.Q.Cast(bestQ.CastPosition);
                }
            }
        }
    }
}