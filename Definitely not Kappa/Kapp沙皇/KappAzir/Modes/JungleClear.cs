namespace KappAzir.Modes
{
    using System.Linq;

    using EloBuddy;
    using EloBuddy.SDK;

    using KappAzir.Utility;

    internal class JungleClear
    {
        public static void Execute()
        {
            var mobs = EntityManager.MinionsAndMonsters.GetJungleMonsters().OrderByDescending(m => m.MaxHealth).Where(m => m.IsKillable());

            foreach (var mob in mobs)
            {
                if (mob != null)
                {
                    var menu = Menus.JungleClearMenu;
                    var Q = mob.IsValidTarget(Azir.Q.Range) && Azir.Q.IsReady() && menu.checkbox("Q")
                            && Player.Instance.ManaPercent > menu.slider("Qmana");
                    var W = mob.IsValidTarget(Azir.W.Range) && Azir.W.IsReady() && menu.checkbox("W")
                            && Player.Instance.ManaPercent > menu.slider("Wmana");
                    var wsave = menu.checkbox("Wsave") && Azir.W.Handle.Ammo < 2;

                    if (W)
                    {
                        if (wsave)
                        {
                            return;
                        }

                        Azir.W.Cast(mob.ServerPosition);
                    }

                    if (Q && Orbwalker.AzirSoldiers.Count(s => s.IsAlly) > 0)
                    {
                        Azir.Q.Cast(mob);
                    }
                }
            }
        }
    }
}