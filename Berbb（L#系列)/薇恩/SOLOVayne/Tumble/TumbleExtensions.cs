using System;
using System.Collections.Generic;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using SharpDX;
using Color = System.Drawing.Color;
using LeagueSharp.Common;

namespace SoloVayne.Skills.Tumble
{
    static class TumbleExtensions
    {
        /// <summary>
        /// Determines whether the position is safe.
        /// </summary>
        /// <param name="position">The position.</param>
        /// <returns></returns>
        public static bool IsSafe(this Vector3 position)
        {
            return position.IsSafeEx() && position.IsNotIntoEnemies() && HeroManager.Enemies.All(m => m.LSDistance(position) > 350f) && (!position.UnderTurret(true) || (ObjectManager.Player.UnderTurret(true) && position.UnderTurret(true) && ObjectManager.Player.HealthPercent > 10));
        }

        /// <summary>
        /// Determines whether the position is Safe using the allies/enemies logic
        /// </summary>
        /// <param name="Position">The position.</param>
        /// <returns></returns>
        public static bool IsSafeEx(this Vector3 Position)
        {
            if (Position.UnderTurret(true) && !ObjectManager.Player.UnderTurret())
            {
                return false;
            }
            var range = 1000f;
            var lowHealthAllies =
                HeroManager.Allies.Where(a => a.IsValidTarget(range, false) && a.HealthPercent < 10 && !a.IsMe);
            var lowHealthEnemies =
                HeroManager.Allies.Where(a => a.IsValidTarget(range) && a.HealthPercent < 10);
            var enemies = ObjectManager.Player.CountEnemiesInRange(range);
            var allies = ObjectManager.Player.CountAlliesInRange(range);
            var enemyTurrets = GameObjects.EnemyTurrets.Where(m => m.IsValidTarget(975f));
            var allyTurrets = GameObjects.AllyTurrets.Where(m => m.IsValidTarget(975f, false));

            return (allies - lowHealthAllies.Count() + allyTurrets.Count() * 2 + 1 >= enemies - lowHealthEnemies.Count() + (!ObjectManager.Player.UnderTurret(true) ? enemyTurrets.Count() * 2 : 0));
        }

        /// <summary>
        /// Determines whether the position is not into enemies.
        /// </summary>
        /// <param name="position">The position.</param>
        /// <returns></returns>
        public static bool IsNotIntoEnemies(this Vector3 position)
        {
            if (!Vayne.Program.smartq && !Vayne.Program.noqenemies)
            {
                return true;
            }

            var enemyPoints = TumbleHelper.GetEnemyPoints();
            if (enemyPoints.ToList().Contains(position.LSTo2D()) && !enemyPoints.Contains(ObjectManager.Player.ServerPosition.LSTo2D()))
            {
                return false;
            }

            var closeEnemies = HeroManager.Enemies.FindAll(en => en.IsValidTarget(1500f) && !(en.LSDistance(ObjectManager.Player.ServerPosition) < en.AttackRange + 65f));
            if (closeEnemies.All(enemy => position.LSCountEnemiesInRange(enemy.AttackRange > 350 ? enemy.AttackRange : 400) == 0))
            {
                return true;
            }

            return false;
        }
    }
}
