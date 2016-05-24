using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iLucian.Utils
{
    using System.IO;

    using ClipperLib;

    using LeagueSharp;
    using LeagueSharp.Common;

    using Path = System.Collections.Generic.List<ClipperLib.IntPoint>;
    using Paths = System.Collections.Generic.List<System.Collections.Generic.List<ClipperLib.IntPoint>>;
    using GamePath = System.Collections.Generic.List<SharpDX.Vector2>;

    using SharpDX;

    using Color = System.Drawing.Color;
    using EloBuddy;
    using EloBuddy.SDK;
    class EPosition
    {
        /// <summary>
        /// Gets the Lucian E Position position using a patented logic!
        /// </summary>
        /// <returns></returns>
        public Vector3 GetEPosition()
        {
            #region The Required Variables

            var positions = DashHelper.GetRotatedEPositions();
            var enemyPositions = DashHelper.GetEnemyPoints();
            var safePositions = positions.Where(pos => !enemyPositions.Contains(pos.LSTo2D())).ToList();
            var bestPosition = ObjectManager.Player.ServerPosition.LSExtend(Game.CursorPos, 450f);
            var AverageDistanceWeight = .60f;
            var ClosestDistanceWeight = .40f;

            var bestWeightedAvg = 0f;

            var highHealthEnemiesNear =
                HeroManager.Enemies.Where(m => !m.IsMelee && m.LSIsValidTarget(1450f) && m.HealthPercent > 7).ToList();

            var alliesNear = HeroManager.Allies.Count(ally => !ally.IsMe && ally.LSIsValidTarget(1500f, false));

            var enemiesNear =
                HeroManager.Enemies.Where(m => m.LSIsValidTarget(Orbwalking.GetRealAutoAttackRange(m) + 450f + 65f))
                    .ToList();

            #endregion

            #region 1 Enemy around only

            if (ObjectManager.Player.LSCountEnemiesInRange(1500f) <= 1)
            {
                // Logic for 1 enemy near
                var backwardsPosition =
                    (ObjectManager.Player.ServerPosition.LSTo2D() - 450f * ObjectManager.Player.Direction.LSTo2D()).To3D();

                if (!backwardsPosition.UnderTurret(true))
                {
                    return backwardsPosition;
                }
            }

            #endregion

            #region Alone, 2 Enemies, 1 Killable

            if (alliesNear == 0 && enemiesNear.Count() <= 2)
            {
                if (
                    enemiesNear.Any(
                        t =>
                        t.Health + 15
                        < ObjectManager.Player.LSGetAutoAttackDamage(t) * 2 + Variables.Spell[Variables.Spells.Q].GetDamage(t)
                        && t.LSDistance(ObjectManager.Player) < Orbwalking.GetRealAutoAttackRange(t) + 80f))
                {
                    var ePosition =
                        ObjectManager.Player.ServerPosition.LSExtend(
                            highHealthEnemiesNear.OrderBy(t => t.Health).First().ServerPosition,
                            450f);

                    if (!ePosition.UnderTurret(true))
                    {
                        return ePosition;
                    }
                }
            }

            #endregion

            #region Alone, 2 Enemies, None Killable

            if (alliesNear == 0 && highHealthEnemiesNear.Count() <= 2)
            {
                // Logic for 2 enemies Near and alone

                // If there is a killable enemy among those. 
                var forwardPosition =
                    (ObjectManager.Player.ServerPosition.LSTo2D() + 450f * ObjectManager.Player.Direction.LSTo2D()).To3D();

                if (!forwardPosition.UnderTurret(true))
                {
                    return forwardPosition;
                }
            }

            #endregion

            #region Already in an enemy's attack range.

            var closeNonMeleeEnemy = DashHelper.GetClosestEnemy(ObjectManager.Player.ServerPosition.LSExtend(Game.CursorPos, 450f));

            if (closeNonMeleeEnemy != null
                && ObjectManager.Player.LSDistance(closeNonMeleeEnemy) <= closeNonMeleeEnemy.AttackRange - 85
                && !closeNonMeleeEnemy.IsMelee)
            {
                return ObjectManager.Player.ServerPosition.LSExtend(Game.CursorPos, 450).IsSafe()
                           ? ObjectManager.Player.ServerPosition.LSExtend(Game.CursorPos, 450f)
                           : Vector3.Zero;
            }

            #endregion

            #region Logic for multiple enemies / allies around.

            foreach (var position in safePositions)
            {
                var enemy = DashHelper.GetClosestEnemy(position);
                if (!enemy.LSIsValidTarget())
                {
                    continue;
                }

                var avgDist = DashHelper.GetAvgDistance(position);

                if (avgDist > -1)
                {
                    var closestDist = ObjectManager.Player.ServerPosition.LSDistance(enemy.ServerPosition);
                    var weightedAvg = closestDist * ClosestDistanceWeight + avgDist * AverageDistanceWeight;
                    if (weightedAvg > bestWeightedAvg && position.IsSafe())
                    {
                        bestWeightedAvg = weightedAvg;
                        bestPosition = position;
                    }
                }
            }

            #endregion

            var endPosition = bestPosition.IsSafe() ? bestPosition : Vector3.Zero;

            #region Couldn't find a suitable position, tumble to nearest ally logic

            if (endPosition == Vector3.Zero)
            {
                // Try to find another suitable position. This usually means we are already near too much enemies turrets so just gtfo and tumble
                // to the closest ally ordered by most health.
                var alliesClose =
                    HeroManager.Allies.Where(ally => !ally.IsMe && ally.LSIsValidTarget(1500f, false)).ToList();
                if (alliesClose.Any() && enemiesNear.Any())
                {
                    var closestMostHealth =
                        alliesClose.OrderBy(m => m.LSDistance(ObjectManager.Player))
                            .ThenByDescending(m => m.Health)
                            .FirstOrDefault();

                    if (closestMostHealth != null
                        && closestMostHealth.LSDistance(
                            enemiesNear.OrderBy(m => m.LSDistance(ObjectManager.Player)).FirstOrDefault())
                        > ObjectManager.Player.LSDistance(
                            enemiesNear.OrderBy(m => m.LSDistance(ObjectManager.Player)).FirstOrDefault()))
                    {
                        var tempPosition = ObjectManager.Player.ServerPosition.LSExtend(
                            closestMostHealth.ServerPosition,
                            450f);
                        if (tempPosition.IsSafe())
                        {
                            endPosition = tempPosition;
                        }
                    }
                }
            }

            #endregion

            #region Couldn't even tumble to ally, just go to mouse

            if (endPosition == Vector3.Zero)
            {
                var mousePosition = ObjectManager.Player.ServerPosition.LSExtend(Game.CursorPos, 450f);
                if (mousePosition.IsSafe())
                {
                    endPosition = mousePosition;
                }
            }

            #endregion

            return endPosition;
        }
    }

    class DashHelper
    {
        /// <summary>
        /// Gets the rotated e positions.
        /// </summary>
        /// <returns></returns>
        public static List<Vector3> GetRotatedEPositions()
        {
            const int CurrentStep = 30;

            // var direction = ObjectManager.Player.Direction.LSTo2D().Perpendicular();
            var direction = (Game.CursorPos - ObjectManager.Player.ServerPosition).Normalized().LSTo2D();

            var list = new List<Vector3>();
            for (var i = -105; i <= 105; i += CurrentStep)
            {
                var angleRad = LeagueSharp.Common.Geometry.DegreeToRadian(i);
                var rotatedPosition = ObjectManager.Player.Position.LSTo2D() + (450f * direction.Rotated(angleRad));
                list.Add(rotatedPosition.To3D());
            }

            return list;
        }

        /// <summary>
        /// Gets the closest enemy.
        /// </summary>
        /// <param name="from">From.</param>
        /// <returns></returns>
        public static AIHeroClient GetClosestEnemy(Vector3 from)
        {
            if (Orbwalker.LastTarget is AIHeroClient)
            {
                var owAi = Orbwalker.LastTarget as AIHeroClient;
                if (owAi.LSIsValidTarget(Orbwalking.GetRealAutoAttackRange(null) + 120f, true, from))
                {
                    return owAi;
                }
            }

            return null;
        }

        /// <summary>
        /// Determines whether the specified position is Safe using AA ranges logic.
        /// </summary>
        /// <param name="position">The position.</param>
        /// <returns></returns>
        public static bool IsSafeEx(Vector3 position)
        {
            var closeEnemies =
                HeroManager.Enemies.FindAll(
                    en =>
                    en.LSIsValidTarget(1500f)
                    && !(en.LSDistance(ObjectManager.Player.ServerPosition) < en.AttackRange + 65f))
                    .OrderBy(en => en.LSDistance(position));

            return closeEnemies.All(enemy => position.LSCountEnemiesInRange(enemy.AttackRange) <= 1);
        }

        /// <summary>
        /// Gets the average distance of a specified position to the enemies.
        /// </summary>
        /// <param name="from">From.</param>
        /// <returns></returns>
        public static float GetAvgDistance(Vector3 from)
        {
            var numberOfEnemies = from.LSCountEnemiesInRange(1200f);
            if (numberOfEnemies != 0)
            {
                var enemies =
                    HeroManager.Enemies.Where(
                        en =>
                        en.LSIsValidTarget(1200f, true, from)
                        && en.Health
                        > ObjectManager.Player.LSGetAutoAttackDamage(en) * 3
                        + Variables.Spell[Variables.Spells.W].GetDamage(en)
                        + Variables.Spell[Variables.Spells.Q].GetDamage(en)).ToList();
                var enemiesEx = HeroManager.Enemies.Where(en => en.LSIsValidTarget(1200f, true, from)).ToList();
                var LHEnemies = enemiesEx.Count() - enemies.Count();

                var totalDistance = (LHEnemies > 1 && enemiesEx.Count() > 2)
                                        ? enemiesEx.Sum(en => en.LSDistance(ObjectManager.Player.ServerPosition))
                                        : enemies.Sum(en => en.LSDistance(ObjectManager.Player.ServerPosition));

                return totalDistance / numberOfEnemies;
            }

            return -1;
        }

        /// <summary>
        /// Gets the enemy points.
        /// </summary>
        /// <param name="dynamic">if set to <c>true</c> [dynamic].</param>
        /// <returns></returns>
        public static GamePath GetEnemyPoints(bool dynamic = true)
        {
            var staticRange = 360f;
            var polygonsList =
                DashVariables.EnemiesClose.Select(
                    enemy =>
                    new SOLOGeometry.Circle(
                        enemy.ServerPosition.LSTo2D(),
                        (dynamic ? (enemy.IsMelee ? enemy.AttackRange * 1.5f : enemy.AttackRange) : staticRange)
                        + enemy.BoundingRadius + 20).ToPolygon()).ToList();
            var pathList = SOLOGeometry.ClipPolygons(polygonsList);
            var pointList =
                pathList.SelectMany(path => path, (path, point) => new Vector2(point.X, point.Y))
                    .Where(currentPoint => !currentPoint.LSIsWall())
                    .ToList();
            return pointList;
        }
    }

    class DashVariables
    {
        /// <summary>
        /// Gets the enemies close.
        /// </summary>
        /// <value>
        /// The enemies close.
        /// </value>
        public static IEnumerable<AIHeroClient> EnemiesClose
        {
            get
            {
                return
                    HeroManager.Enemies.Where(
                        m =>
                        m.LSDistance(ObjectManager.Player, true) <= Math.Pow(1000, 2) && m.LSIsValidTarget(1500, false)
                        && m.LSCountEnemiesInRange(m.IsMelee() ? m.AttackRange * 1.5f : m.AttackRange + 20 * 1.5f) > 0);
            }
        }
    }

    static class DashExtensions
    {
        /// <summary>
        /// Determines whether the position is safe.
        /// </summary>
        /// <param name="position">The position.</param>
        /// <returns></returns>
        public static bool IsSafe(this Vector3 position)
        {
            return position.IsSafeEx() && position.IsNotIntoEnemies()
                   && HeroManager.Enemies.All(m => m.LSDistance(position) > 350f)
                   && (!position.UnderTurret(true)
                       || (ObjectManager.Player.UnderTurret(true) && position.UnderTurret(true)
                           && ObjectManager.Player.HealthPercent > 10));

            // Either it is not under turret or both the player and the position are under turret already and the health percent is greater than 10.
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
                HeroManager.Allies.Where(a => a.LSIsValidTarget(range, false) && a.HealthPercent < 10 && !a.IsMe);
            var lowHealthEnemies = HeroManager.Allies.Where(a => a.LSIsValidTarget(range) && a.HealthPercent < 10);
            var enemies = ObjectManager.Player.LSCountEnemiesInRange(range);
            var allies = ObjectManager.Player.CountAlliesInRange(range);
            var enemyTurrets = GameObjects.EnemyTurrets.Where(m => m.LSIsValidTarget(975f));
            var allyTurrets = GameObjects.AllyTurrets.Where(m => m.LSIsValidTarget(975f, false));

            return allies - lowHealthAllies.Count() + allyTurrets.Count() * 2 + 1
                    >= enemies - lowHealthEnemies.Count()
                    + (!ObjectManager.Player.UnderTurret(true) ? enemyTurrets.Count() * 2 : 0);
        }

        /// <summary>
        /// Determines whether the position is not into enemies.
        /// </summary>
        /// <param name="position">The position.</param>
        /// <returns></returns>
        public static bool IsNotIntoEnemies(this Vector3 position)
        {
            var enemyPoints = DashHelper.GetEnemyPoints();
            if (enemyPoints.ToList().Contains(position.LSTo2D())
                && !enemyPoints.Contains(ObjectManager.Player.ServerPosition.LSTo2D()))
            {
                return false;
            }

            var closeEnemies =
                HeroManager.Enemies.FindAll(
                    en =>
                    en.LSIsValidTarget(1500f)
                    && !(en.LSDistance(ObjectManager.Player.ServerPosition) < en.AttackRange + 65f));
            if (!closeEnemies.All(enemy => position.LSCountEnemiesInRange(enemy.AttackRange) <= 1))
            {
                return false;
            }

            return true;
        }
    }

    public static class GameObjects
    {
        #region Static Fields

        /// <summary>
        ///     The ally heroes list.
        /// </summary>
        private static readonly List<AIHeroClient> AllyHeroesList = new List<AIHeroClient>();

        /// <summary>
        ///     The ally inhibitors list.
        /// </summary>
        private static readonly List<Obj_BarracksDampener> AllyInhibitorsList = new List<Obj_BarracksDampener>();

        /// <summary>
        ///     The ally list.
        /// </summary>
        private static readonly List<Obj_AI_Base> AllyList = new List<Obj_AI_Base>();

        /// <summary>
        ///     The ally minions list.
        /// </summary>
        private static readonly List<Obj_AI_Minion> AllyMinionsList = new List<Obj_AI_Minion>();

        /// <summary>
        ///     The ally shops list.
        /// </summary>
        private static readonly List<Obj_Shop> AllyShopsList = new List<Obj_Shop>();

        /// <summary>
        ///     The ally spawn points list.
        /// </summary>
        private static readonly List<Obj_SpawnPoint> AllySpawnPointsList = new List<Obj_SpawnPoint>();

        /// <summary>
        ///     The ally turrets list.
        /// </summary>
        private static readonly List<Obj_AI_Turret> AllyTurretsList = new List<Obj_AI_Turret>();

        /// <summary>
        ///     The ally wards list.
        /// </summary>
        private static readonly List<Obj_AI_Minion> AllyWardsList = new List<Obj_AI_Minion>();

        /// <summary>
        ///     The attackable unit list.
        /// </summary>
        private static readonly List<AttackableUnit> AttackableUnitsList = new List<AttackableUnit>();

        /// <summary>
        ///     The enemy heroes list.
        /// </summary>
        private static readonly List<AIHeroClient> EnemyHeroesList = new List<AIHeroClient>();

        /// <summary>
        ///     The enemy inhibitors list.
        /// </summary>
        private static readonly List<Obj_BarracksDampener> EnemyInhibitorsList = new List<Obj_BarracksDampener>();

        /// <summary>
        ///     The enemy list.
        /// </summary>
        private static readonly List<Obj_AI_Base> EnemyList = new List<Obj_AI_Base>();

        /// <summary>
        ///     The enemy minions list.
        /// </summary>
        private static readonly List<Obj_AI_Minion> EnemyMinionsList = new List<Obj_AI_Minion>();

        /// <summary>
        ///     The enemy shops list.
        /// </summary>
        private static readonly List<Obj_Shop> EnemyShopsList = new List<Obj_Shop>();

        /// <summary>
        ///     The enemy spawn points list.
        /// </summary>
        private static readonly List<Obj_SpawnPoint> EnemySpawnPointsList = new List<Obj_SpawnPoint>();

        /// <summary>
        ///     The enemy turrets list.
        /// </summary>
        private static readonly List<Obj_AI_Turret> EnemyTurretsList = new List<Obj_AI_Turret>();

        /// <summary>
        ///     The enemy wards list.
        /// </summary>
        private static readonly List<Obj_AI_Minion> EnemyWardsList = new List<Obj_AI_Minion>();

        /// <summary>
        ///     The game objects list.
        /// </summary>
        private static readonly List<GameObject> GameObjectsList = new List<GameObject>();

        /// <summary>
        ///     The heroes list.
        /// </summary>
        private static readonly List<AIHeroClient> HeroesList = new List<AIHeroClient>();

        /// <summary>
        ///     The inhibitors list.
        /// </summary>
        private static readonly List<Obj_BarracksDampener> InhibitorsList = new List<Obj_BarracksDampener>();

        /// <summary>
        ///     The jungle large list.
        /// </summary>
        private static readonly List<Obj_AI_Minion> JungleLargeList = new List<Obj_AI_Minion>();

        /// <summary>
        ///     The jungle legendary list.
        /// </summary>
        private static readonly List<Obj_AI_Minion> JungleLegendaryList = new List<Obj_AI_Minion>();

        /// <summary>
        ///     The jungle list.
        /// </summary>
        private static readonly List<Obj_AI_Minion> JungleList = new List<Obj_AI_Minion>();

        /// <summary>
        ///     The jungle small list.
        /// </summary>
        private static readonly List<Obj_AI_Minion> JungleSmallList = new List<Obj_AI_Minion>();

        /// <summary>
        ///     The minions list.
        /// </summary>
        private static readonly List<Obj_AI_Minion> MinionsList = new List<Obj_AI_Minion>();

        /// <summary>
        ///     The nexus list.
        /// </summary>
        private static readonly List<Obj_HQ> NexusList = new List<Obj_HQ>();

        /// <summary>
        ///     The shops list.
        /// </summary>
        private static readonly List<Obj_Shop> ShopsList = new List<Obj_Shop>();

        /// <summary>
        ///     The spawn points list.
        /// </summary>
        private static readonly List<Obj_SpawnPoint> SpawnPointsList = new List<Obj_SpawnPoint>();

        /// <summary>
        ///     The turrets list.
        /// </summary>
        private static readonly List<Obj_AI_Turret> TurretsList = new List<Obj_AI_Turret>();

        /// <summary>
        ///     The wards list.
        /// </summary>
        private static readonly List<Obj_AI_Minion> WardsList = new List<Obj_AI_Minion>();

        /// <summary>
        ///     Indicates whether the <see cref="GameObjects" /> stack was initialized and saved required instances.
        /// </summary>
        private static bool initialized;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes static members of the <see cref="GameObjects" /> class.
        /// </summary>
        static GameObjects()
        {
            Initialize();
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets the game objects.
        /// </summary>
        public static IEnumerable<GameObject> AllGameObjects => GameObjectsList;

        /// <summary>
        ///     Gets the ally.
        /// </summary>
        public static IEnumerable<Obj_AI_Base> Ally => AllyList;

        /// <summary>
        ///     Gets the ally heroes.
        /// </summary>
        public static IEnumerable<AIHeroClient> AllyHeroes => AllyHeroesList;

        /// <summary>
        ///     Gets the ally inhibitors.
        /// </summary>
        public static IEnumerable<Obj_BarracksDampener> AllyInhibitors => AllyInhibitorsList;

        /// <summary>
        ///     Gets the ally minions.
        /// </summary>
        public static IEnumerable<Obj_AI_Minion> AllyMinions => AllyMinionsList;

        /// <summary>
        ///     Gets or sets the ally nexus.
        /// </summary>
        public static Obj_HQ AllyNexus { get; set; }

        /// <summary>
        ///     Gets the ally shops.
        /// </summary>
        public static IEnumerable<Obj_Shop> AllyShops => AllyShopsList;

        /// <summary>
        ///     Gets the ally spawn points.
        /// </summary>
        public static IEnumerable<Obj_SpawnPoint> AllySpawnPoints => AllySpawnPointsList;

        /// <summary>
        ///     Gets the ally turrets.
        /// </summary>
        public static IEnumerable<Obj_AI_Turret> AllyTurrets => AllyTurretsList;

        /// <summary>
        ///     Gets the ally wards.
        /// </summary>
        public static IEnumerable<Obj_AI_Minion> AllyWards => AllyWardsList;

        /// <summary>
        ///     Gets the attackable units.
        /// </summary>
        public static IEnumerable<AttackableUnit> AttackableUnits => AttackableUnitsList;

        /// <summary>
        ///     Gets the enemy.
        /// </summary>
        public static IEnumerable<Obj_AI_Base> Enemy => EnemyList;

        /// <summary>
        ///     Gets the enemy heroes.
        /// </summary>
        public static IEnumerable<AIHeroClient> EnemyHeroes => EnemyHeroesList;

        /// <summary>
        ///     Gets the enemy inhibitors.
        /// </summary>
        public static IEnumerable<Obj_BarracksDampener> EnemyInhibitors => EnemyInhibitorsList;

        /// <summary>
        ///     Gets the enemy minions.
        /// </summary>
        public static IEnumerable<Obj_AI_Minion> EnemyMinions => EnemyMinionsList;

        /// <summary>
        ///     Gets or sets the enemy nexus.
        /// </summary>
        public static Obj_HQ EnemyNexus { get; set; }

        /// <summary>
        ///     Gets the enemy shops.
        /// </summary>
        public static IEnumerable<Obj_Shop> EnemyShops => EnemyShopsList;

        /// <summary>
        ///     Gets the enemy spawn points.
        /// </summary>
        public static IEnumerable<Obj_SpawnPoint> EnemySpawnPoints => EnemySpawnPointsList;

        /// <summary>
        ///     Gets the enemy turrets.
        /// </summary>
        public static IEnumerable<Obj_AI_Turret> EnemyTurrets => EnemyTurretsList;

        /// <summary>
        ///     Gets the enemy wards.
        /// </summary>
        public static IEnumerable<Obj_AI_Minion> EnemyWards => EnemyWardsList;

        /// <summary>
        ///     Gets the heroes.
        /// </summary>
        public static IEnumerable<AIHeroClient> Heroes => HeroesList;

        /// <summary>
        ///     Gets the inhibitors.
        /// </summary>
        public static IEnumerable<Obj_BarracksDampener> Inhibitors => InhibitorsList;

        /// <summary>
        ///     Gets the jungle.
        /// </summary>
        public static IEnumerable<Obj_AI_Minion> Jungle => JungleList;

        /// <summary>
        ///     Gets the jungle large.
        /// </summary>
        public static IEnumerable<Obj_AI_Minion> JungleLarge => JungleLargeList;

        /// <summary>
        ///     Gets the jungle legendary.
        /// </summary>
        public static IEnumerable<Obj_AI_Minion> JungleLegendary => JungleLegendaryList;

        /// <summary>
        ///     Gets the jungle small.
        /// </summary>
        public static IEnumerable<Obj_AI_Minion> JungleSmall => JungleSmallList;

        /// <summary>
        ///     Gets the minions.
        /// </summary>
        public static IEnumerable<Obj_AI_Minion> Minions => MinionsList;

        /// <summary>
        ///     Gets the nexuses.
        /// </summary>
        public static IEnumerable<Obj_HQ> Nexuses => NexusList;

        /// <summary>
        ///     Gets or sets the player.
        /// </summary>
        public static AIHeroClient Player { get; set; }

        /// <summary>
        ///     Gets the shops.
        /// </summary>
        public static IEnumerable<Obj_Shop> Shops => ShopsList;

        /// <summary>
        ///     Gets the spawn points.
        /// </summary>
        public static IEnumerable<Obj_SpawnPoint> SpawnPoints => SpawnPointsList;

        /// <summary>
        ///     Gets the turrets.
        /// </summary>
        public static IEnumerable<Obj_AI_Turret> Turrets => TurretsList;

        /// <summary>
        ///     Gets the wards.
        /// </summary>
        public static IEnumerable<Obj_AI_Minion> Wards => WardsList;

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Compares two <see cref="GameObject" /> and returns if they are identical.
        /// </summary>
        /// <param name="gameObject">The GameObject</param>
        /// <param name="object">The Compare GameObject</param>
        /// <returns>Whether the <see cref="GameObject" />s are identical.</returns>
        public static bool Compare(this GameObject gameObject, GameObject @object)
        {
            return gameObject != null && gameObject.IsValid && @object != null && @object.IsValid
                   && gameObject.NetworkId == @object.NetworkId;
        }

        /// <summary>
        ///     The get operation from the GameObjects stack.
        /// </summary>
        /// <typeparam name="T">
        ///     The requested <see cref="GameObject" /> type.
        /// </typeparam>
        /// <returns>
        ///     The List containing the requested type.
        /// </returns>
        public static IEnumerable<T> Get<T>() where T : GameObject, new()
        {
            return AllGameObjects.OfType<T>();
        }

        /// <summary>
        ///     Get get operation from the native GameObjects stack.
        /// </summary>
        /// <typeparam name="T">
        ///     The requested <see cref="GameObject" /> type.
        /// </typeparam>
        /// <returns>
        ///     The List containing the requested type.
        /// </returns>
        public static IEnumerable<T> GetNative<T>() where T : GameObject, new()
        {
            return ObjectManager.Get<T>();
        }

        #endregion

        #region Methods

        /// <summary>
        ///     The initialize method.
        /// </summary>
        internal static void Initialize()
        {
            if (initialized)
            {
                return;
            }

            initialized = true;

            CustomEvents.Game.OnGameLoad += (args) =>
            {
                Player = ObjectManager.Player;

                HeroesList.AddRange(ObjectManager.Get<AIHeroClient>());
                MinionsList.AddRange(
                    ObjectManager.Get<Obj_AI_Minion>()
                        .Where(
                            o =>
                            o.Team != GameObjectTeam.Neutral && !o.CharData.BaseSkinName.ToLower().Contains("ward")
                            && !o.CharData.BaseSkinName.ToLower().Contains("trinket")
                            && !o.CharData.BaseSkinName.Equals("gangplankbarrel")));
                TurretsList.AddRange(ObjectManager.Get<Obj_AI_Turret>());
                InhibitorsList.AddRange(ObjectManager.Get<Obj_BarracksDampener>());
                JungleList.AddRange(ObjectManager.Get<Obj_AI_Minion>().Where(o => o.Team == GameObjectTeam.Neutral));
                WardsList.AddRange(
                    ObjectManager.Get<Obj_AI_Minion>()
                        .Where(
                            o =>
                            o.CharData.BaseSkinName.ToLower().Contains("ward")
                            || o.CharData.BaseSkinName.ToLower().Contains("trinket")));
                ShopsList.AddRange(ObjectManager.Get<Obj_Shop>());
                SpawnPointsList.AddRange(ObjectManager.Get<Obj_SpawnPoint>());
                GameObjectsList.AddRange(ObjectManager.Get<GameObject>());
                NexusList.AddRange(ObjectManager.Get<Obj_HQ>());
                AttackableUnitsList.AddRange(ObjectManager.Get<AttackableUnit>());

                EnemyHeroesList.AddRange(HeroesList.Where(o => o.IsEnemy));
                EnemyMinionsList.AddRange(MinionsList.Where(o => o.IsEnemy));
                EnemyTurretsList.AddRange(TurretsList.Where(o => o.IsEnemy));
                EnemyInhibitorsList.AddRange(InhibitorsList.Where(o => o.IsEnemy));
                EnemyList.AddRange(
                    EnemyHeroesList.Cast<Obj_AI_Base>().Concat(EnemyMinionsList).Concat(EnemyTurretsList));
                EnemyNexus = NexusList.FirstOrDefault(n => n.IsEnemy);

                AllyHeroesList.AddRange(HeroesList.Where(o => o.IsAlly));
                AllyMinionsList.AddRange(MinionsList.Where(o => o.IsAlly));
                AllyTurretsList.AddRange(TurretsList.Where(o => o.IsAlly));
                AllyInhibitorsList.AddRange(InhibitorsList.Where(o => o.IsAlly));
                AllyList.AddRange(
                    AllyHeroesList.Cast<Obj_AI_Base>().Concat(AllyMinionsList).Concat(AllyTurretsList));
                AllyNexus = NexusList.FirstOrDefault(n => n.IsAlly);

                JungleSmallList.AddRange(JungleList.Where(o => o.GetJungleType() == JungleType.Small));
                JungleLargeList.AddRange(JungleList.Where(o => o.GetJungleType() == JungleType.Large));
                JungleLegendaryList.AddRange(JungleList.Where(o => o.GetJungleType() == JungleType.Legendary));

                AllyWardsList.AddRange(WardsList.Where(o => o.IsAlly));
                EnemyWardsList.AddRange(WardsList.Where(o => o.IsEnemy));

                AllyShopsList.AddRange(ShopsList.Where(o => o.IsAlly));
                EnemyShopsList.AddRange(ShopsList.Where(o => o.IsEnemy));

                AllySpawnPointsList.AddRange(SpawnPointsList.Where(o => o.IsAlly));
                EnemySpawnPointsList.AddRange(SpawnPointsList.Where(o => o.IsEnemy));

                GameObject.OnCreate += OnCreate;
                GameObject.OnDelete += OnDelete;
            };
        }

        /// <summary>
        ///     OnCreate event.
        /// </summary>
        /// <param name="sender">
        ///     The sender
        /// </param>
        /// <param name="args">
        ///     The event data
        /// </param>
        private static void OnCreate(GameObject sender, EventArgs args)
        {
            GameObjectsList.Add(sender);

            var attackableUnit = sender as AttackableUnit;
            if (attackableUnit != null)
            {
                AttackableUnitsList.Add(attackableUnit);
            }

            var hero = sender as AIHeroClient;
            if (hero != null)
            {
                HeroesList.Add(hero);
                if (hero.IsEnemy)
                {
                    EnemyHeroesList.Add(hero);
                    EnemyList.Add(hero);
                }
                else
                {
                    AllyHeroesList.Add(hero);
                    AllyList.Add(hero);
                }

                return;
            }

            var minion = sender as Obj_AI_Minion;
            if (minion != null)
            {
                if (minion.Team != GameObjectTeam.Neutral)
                {
                    if (minion.CharData.BaseSkinName.ToLower().Contains("ward")
                        || minion.CharData.BaseSkinName.ToLower().Contains("trinket"))
                    {
                        WardsList.Add(minion);
                        if (minion.IsEnemy)
                        {
                            EnemyWardsList.Add(minion);
                        }
                        else
                        {
                            AllyWardsList.Add(minion);
                        }
                    }
                    else if (!minion.CharData.BaseSkinName.Equals("gangplankbarrel"))
                    {
                        MinionsList.Add(minion);
                        if (minion.IsEnemy)
                        {
                            EnemyMinionsList.Add(minion);
                            EnemyList.Add(minion);
                        }
                        else
                        {
                            AllyMinionsList.Add(minion);
                            AllyList.Add(minion);
                        }
                    }
                }
                else
                {
                    switch (minion.GetJungleType())
                    {
                        case JungleType.Small:
                            JungleSmallList.Add(minion);
                            break;
                        case JungleType.Large:
                            JungleLargeList.Add(minion);
                            break;
                        case JungleType.Legendary:
                            JungleLegendaryList.Add(minion);
                            break;
                    }

                    JungleList.Add(minion);
                }

                return;
            }

            var turret = sender as Obj_AI_Turret;
            if (turret != null)
            {
                TurretsList.Add(turret);
                if (turret.IsEnemy)
                {
                    EnemyTurretsList.Add(turret);
                    EnemyList.Add(turret);
                }
                else
                {
                    AllyTurretsList.Add(turret);
                    AllyList.Add(turret);
                }

                return;
            }

            var shop = sender as Obj_Shop;
            if (shop != null)
            {
                ShopsList.Add(shop);
                if (shop.IsAlly)
                {
                    AllyShopsList.Add(shop);
                }
                else
                {
                    EnemyShopsList.Add(shop);
                }

                return;
            }

            var spawnPoint = sender as Obj_SpawnPoint;
            if (spawnPoint != null)
            {
                SpawnPointsList.Add(spawnPoint);
                if (spawnPoint.IsAlly)
                {
                    AllySpawnPointsList.Add(spawnPoint);
                }
                else
                {
                    EnemySpawnPointsList.Add(spawnPoint);
                }
            }

            var inhibitor = sender as Obj_BarracksDampener;
            if (inhibitor != null)
            {
                InhibitorsList.Add(inhibitor);
                if (inhibitor.IsAlly)
                {
                    AllyInhibitorsList.Add(inhibitor);
                }
                else
                {
                    EnemyInhibitorsList.Add(inhibitor);
                }
            }

            var nexus = sender as Obj_HQ;
            if (nexus != null)
            {
                NexusList.Add(nexus);
                if (nexus.IsAlly)
                {
                    AllyNexus = nexus;
                }
                else
                {
                    EnemyNexus = nexus;
                }
            }
        }

        /// <summary>
        ///     OnDelete event.
        /// </summary>
        /// <param name="sender">
        ///     The sender
        /// </param>
        /// <param name="args">
        ///     The event data
        /// </param>
        private static void OnDelete(GameObject sender, EventArgs args)
        {
            foreach (var gameObject in GameObjectsList.Where(o => o.Compare(sender)).ToList())
            {
                GameObjectsList.Remove(gameObject);
            }

            foreach (var attackableUnitObject in AttackableUnitsList.Where(a => a.Compare(sender)).ToList())
            {
                AttackableUnitsList.Remove(attackableUnitObject);
            }

            var hero = sender as AIHeroClient;
            if (hero != null)
            {
                foreach (var heroObject in HeroesList.Where(h => h.Compare(hero)).ToList())
                {
                    HeroesList.Remove(heroObject);
                    if (hero.IsEnemy)
                    {
                        EnemyHeroesList.Remove(heroObject);
                        EnemyList.Remove(heroObject);
                    }
                    else
                    {
                        AllyHeroesList.Remove(heroObject);
                        AllyList.Remove(heroObject);
                    }
                }

                return;
            }

            var minion = sender as Obj_AI_Minion;
            if (minion != null)
            {
                if (minion.Team != GameObjectTeam.Neutral)
                {
                    if (minion.CharData.BaseSkinName.ToLower().Contains("ward")
                        || minion.CharData.BaseSkinName.ToLower().Contains("trinket"))
                    {
                        foreach (var ward in WardsList.Where(w => w.Compare(minion)).ToList())
                        {
                            WardsList.Remove(ward);
                            if (minion.IsEnemy)
                            {
                                EnemyWardsList.Remove(ward);
                            }
                            else
                            {
                                AllyWardsList.Remove(ward);
                            }
                        }
                    }
                    else if (!minion.CharData.BaseSkinName.Equals("gangplankbarrel"))
                    {
                        foreach (var minionObject in MinionsList.Where(m => m.Compare(minion)).ToList())
                        {
                            MinionsList.Remove(minionObject);
                            if (minion.IsEnemy)
                            {
                                EnemyMinionsList.Remove(minionObject);
                                EnemyList.Remove(minionObject);
                            }
                            else
                            {
                                AllyMinionsList.Remove(minionObject);
                                AllyList.Remove(minionObject);
                            }
                        }
                    }
                }
                else
                {
                    foreach (var jungleObject in JungleList.Where(j => j.Compare(minion)).ToList())
                    {
                        switch (jungleObject.GetJungleType())
                        {
                            case JungleType.Small:
                                JungleSmallList.Remove(jungleObject);
                                break;
                            case JungleType.Large:
                                JungleLargeList.Remove(jungleObject);
                                break;
                            case JungleType.Legendary:
                                JungleLegendaryList.Remove(jungleObject);
                                break;
                        }

                        JungleList.Remove(jungleObject);
                    }
                }

                return;
            }

            var turret = sender as Obj_AI_Turret;
            if (turret != null)
            {
                foreach (var turretObject in TurretsList.Where(t => t.Compare(turret)).ToList())
                {
                    TurretsList.Remove(turretObject);
                    if (turret.IsEnemy)
                    {
                        EnemyTurretsList.Remove(turretObject);
                        EnemyList.Remove(turretObject);
                    }
                    else
                    {
                        AllyTurretsList.Remove(turretObject);
                        AllyList.Remove(turretObject);
                    }
                }

                return;
            }

            var shop = sender as Obj_Shop;
            if (shop != null)
            {
                foreach (var shopObject in ShopsList.Where(s => s.Compare(shop)).ToList())
                {
                    ShopsList.Remove(shopObject);
                    if (shop.IsAlly)
                    {
                        AllyShopsList.Remove(shopObject);
                    }
                    else
                    {
                        EnemyShopsList.Remove(shopObject);
                    }
                }

                return;
            }

            var spawnPoint = sender as Obj_SpawnPoint;
            if (spawnPoint != null)
            {
                foreach (var spawnPointObject in SpawnPointsList.Where(s => s.Compare(spawnPoint)).ToList())
                {
                    SpawnPointsList.Remove(spawnPointObject);
                    if (spawnPoint.IsAlly)
                    {
                        AllySpawnPointsList.Remove(spawnPointObject);
                    }
                    else
                    {
                        EnemySpawnPointsList.Remove(spawnPointObject);
                    }
                }
            }

            var inhibitor = sender as Obj_BarracksDampener;
            if (inhibitor != null)
            {
                foreach (var inhibitorObject in InhibitorsList.Where(i => i.Compare(inhibitor)).ToList())
                {
                    InhibitorsList.Remove(inhibitorObject);
                    if (inhibitor.IsAlly)
                    {
                        AllyInhibitorsList.Remove(inhibitorObject);
                    }
                    else
                    {
                        EnemyInhibitorsList.Remove(inhibitorObject);
                    }
                }
            }

            var nexus = sender as Obj_HQ;
            if (nexus != null)
            {
                foreach (var nexusObject in NexusList.Where(n => n.Compare(nexus)).ToList())
                {
                    NexusList.Remove(nexusObject);
                    if (nexusObject.IsAlly)
                    {
                        AllyNexus = null;
                    }
                    else
                    {
                        EnemyNexus = null;
                    }
                }
            }
        }

        #endregion
    }

    static class SOLOGeometry
    {
        private const int CircleLineSegmentN = 22;

        public static Vector3 SwitchYZ(this Vector3 v)
        {
            return new Vector3(v.X, v.Z, v.Y);
        }

        public static bool IsOverWall(Vector3 start, Vector3 end)
        {
            double distance = Vector3.Distance(start, end);
            for (uint i = 0; i < distance; i += 10)
            {
                var tempPosition = start.LSExtend(end, i).LSTo2D();
                if (tempPosition.LSIsWall())
                {
                    return true;
                }
            }

            return false;
        }

        // Clipper
        public static List<Polygon> ToPolygons(this Paths v)
        {
            var result = new List<Polygon>();

            foreach (var path in v)
            {
                result.Add(path.ToPolygon());
            }

            return result;
        }

        /// <summary>
        /// Returns the position on the path after t milliseconds at speed speed.
        /// </summary>
        public static Vector2 PositionAfter(this GamePath self, int t, int speed, int delay = 0)
        {
            var distance = Math.Max(0, t - delay) * speed / 1000;
            for (var i = 0; i <= self.Count - 2; i++)
            {
                var from = self[i];
                var to = self[i + 1];
                var d = (int)to.LSDistance(from);
                if (d > distance)
                {
                    return from + distance * (to - from).LSNormalized();
                }

                distance -= d;
            }

            return self[self.Count - 1];
        }

        public static Polygon ToPolygon(this Path v)
        {
            var polygon = new Polygon();
            foreach (var point in v)
            {
                polygon.Add(new Vector2(point.X, point.Y));
            }

            return polygon;
        }

        public static Paths ClipPolygons(List<Polygon> polygons)
        {
            var subj = new Paths(polygons.Count);
            var clip = new Paths(polygons.Count);

            foreach (var polygon in polygons)
            {
                subj.Add(polygon.ToClipperPath());
                clip.Add(polygon.ToClipperPath());
            }

            var solution = new Paths();
            var c = new Clipper();
            c.AddPaths(subj, PolyType.ptSubject, true);
            c.AddPaths(clip, PolyType.ptClip, true);
            c.Execute(ClipType.ctUnion, solution, PolyFillType.pftPositive, PolyFillType.pftEvenOdd);

            return solution;
        }

        public class Circle
        {
            public Vector2 Center;

            public float Radius;

            public Circle(Vector2 center, float radius)
            {
                Center = center;
                Radius = radius;
            }

            public Polygon ToPolygon(int offset = 0, float overrideWidth = -1)
            {
                var result = new Polygon();
                var outRadius = overrideWidth > 0
                                     ? overrideWidth
                                     : (offset + Radius) / (float)Math.Cos(2 * Math.PI / CircleLineSegmentN);

                for (var i = 1; i <= CircleLineSegmentN; i++)
                {
                    var angle = i * 2 * Math.PI / CircleLineSegmentN;
                    var point = new Vector2(
                        Center.X + outRadius * (float)Math.Cos(angle),
                        Center.Y + outRadius * (float)Math.Sin(angle));
                    result.Add(point);
                }

                return result;
            }
        }

        public class Polygon
        {
            public GamePath Points = new List<Vector2>();

            public void Add(Vector2 point)
            {
                Points.Add(point);
            }

            public Path ToClipperPath()
            {
                var result = new Path(Points.Count);

                foreach (var point in Points)
                {
                    result.Add(new IntPoint(point.X, point.Y));
                }

                return result;
            }

            public bool IsOutside(Vector2 point)
            {
                var p = new IntPoint(point.X, point.Y);
                return Clipper.PointInPolygon(p, ToClipperPath()) != 1;
            }

            public void Draw(Color color, int width = 1)
            {
                for (var i = 0; i <= Points.Count - 1; i++)
                {
                    var nextIndex = (Points.Count - 1 == i) ? 0 : (i + 1);
                    DrawLineInWorld(Points[i].To3D(), Points[nextIndex].To3D(), width, color);
                }
            }

            public static void DrawLineInWorld(Vector3 start, Vector3 end, int width, Color color)
            {
                var from = Drawing.WorldToScreen(start);
                var to = Drawing.WorldToScreen(end);
                Drawing.DrawLine(from[0], from[1], to[0], to[1], width, color);
            }
        }

        public class Rectangle
        {
            public Vector2 Direction;

            public Vector2 Perpendicular;

            public Vector2 REnd;

            public Vector2 RStart;

            public float Width;

            public Rectangle(Vector2 start, Vector2 end, float width)
            {
                RStart = start;
                REnd = end;
                Width = width;
                Direction = (end - start).LSNormalized();
                Perpendicular = Direction.LSPerpendicular();
            }

            public Polygon ToPolygon(int offset = 0, float overrideWidth = -1)
            {
                var result = new Polygon();

                result.Add(
                    RStart + (overrideWidth > 0 ? overrideWidth : Width + offset) * Perpendicular - offset * Direction);
                result.Add(
                    RStart - (overrideWidth > 0 ? overrideWidth : Width + offset) * Perpendicular - offset * Direction);
                result.Add(
                    REnd - (overrideWidth > 0 ? overrideWidth : Width + offset) * Perpendicular + offset * Direction);
                result.Add(
                    REnd + (overrideWidth > 0 ? overrideWidth : Width + offset) * Perpendicular + offset * Direction);

                return result;
            }
        }

        public class Ring
        {
            public Vector2 Center;

            public float Radius;

            public float RingRadius; // actually radius width.

            public Ring(Vector2 center, float radius, float ringRadius)
            {
                Center = center;
                Radius = radius;
                RingRadius = ringRadius;
            }

            public Polygon ToPolygon(int offset = 0)
            {
                var result = new Polygon();

                var outRadius = (offset + Radius + RingRadius) / (float)Math.Cos(2 * Math.PI / CircleLineSegmentN);
                var innerRadius = Radius - RingRadius - offset;

                for (var i = 0; i <= CircleLineSegmentN; i++)
                {
                    var angle = i * 2 * Math.PI / CircleLineSegmentN;
                    var point = new Vector2(
                        Center.X - outRadius * (float)Math.Cos(angle),
                        Center.Y - outRadius * (float)Math.Sin(angle));
                    result.Add(point);
                }

                for (var i = 0; i <= CircleLineSegmentN; i++)
                {
                    var angle = i * 2 * Math.PI / CircleLineSegmentN;
                    var point = new Vector2(
                        Center.X + innerRadius * (float)Math.Cos(angle),
                        Center.Y - innerRadius * (float)Math.Sin(angle));
                    result.Add(point);
                }

                return result;
            }
        }

        public class Sector
        {
            public float Angle;

            public Vector2 Center;

            public Vector2 Direction;

            public float Radius;

            public Sector(Vector2 center, Vector2 direction, float angle, float radius)
            {
                Center = center;
                Direction = direction;
                Angle = angle;
                Radius = radius;
            }

            public Polygon ToPolygon(int offset = 0)
            {
                var result = new Polygon();
                var outRadius = (Radius + offset) / (float)Math.Cos(2 * Math.PI / CircleLineSegmentN);

                result.Add(Center);
                var Side1 = Direction.LSRotated(-Angle * 0.5f);

                for (var i = 0; i <= CircleLineSegmentN; i++)
                {
                    var cDirection = Side1.LSRotated(i * Angle / CircleLineSegmentN).LSNormalized();
                    result.Add(new Vector2(Center.X + outRadius * cDirection.X, Center.Y + outRadius * cDirection.Y));
                }

                return result;
            }
        }
    }
}