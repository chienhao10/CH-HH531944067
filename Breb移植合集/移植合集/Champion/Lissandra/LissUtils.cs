using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using LeagueSharp.Common;
using SharpDX;

namespace SephLissandra
{
    internal class LissUtils
    {
        private static readonly AIHeroClient Player = Lissandra.Player;

        public static HitChance GetHitChance(string search)
        {
            var hitchance = Lissandra.getBoxItem(Lissandra.miscMenu, search);
            switch (hitchance)
            {
                case 0:
                    return HitChance.Low;
                case 1:
                    return HitChance.Medium;
                case 2:
                    return HitChance.High;
            }
            return HitChance.Medium;
        }

        public static bool isHealthy()
        {
            return Player.HealthPercent > 25;
        }

        public static bool PointUnderEnemyTurret(Vector2 Point)
        {
            var EnemyTurrets =
                ObjectManager.Get<Obj_AI_Turret>()
                    .Where(t => t.IsEnemy && Vector2.Distance(t.Position.To2D(), Point) < 900f);
            return EnemyTurrets.Any();
        }

        public static bool PointUnderAllyTurret(Vector3 Point)
        {
            var AllyTurrets =
                ObjectManager.Get<Obj_AI_Turret>().Where(t => t.IsAlly && Vector3.Distance(t.Position, Point) < 900f);
            return AllyTurrets.Any();
        }

        public static bool CanSecondE()
        {
            return Player.HasBuff("LissandraE");
        }

        public static bool AutoSecondE()
        {
            return Lissandra.getCheckBoxItem(Lissandra.comboMenu, "Combo.UseE2");
        }
    }
}