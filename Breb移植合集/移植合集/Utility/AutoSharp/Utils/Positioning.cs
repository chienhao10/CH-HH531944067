using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Policy;
using ClipperLib;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using AutoSharp.Auto.SummonersRift;
using Path = System.Collections.Generic.List<ClipperLib.IntPoint>;
using Paths = System.Collections.Generic.List<System.Collections.Generic.List<ClipperLib.IntPoint>>;
using GamePath = System.Collections.Generic.List<SharpDX.Vector2>;
using EloBuddy;
using EloBuddy.SDK;
// ReSharper disable InconsistentNaming

namespace AutoSharp.Utils
{
    internal static class Positioning
    {
        internal static List<Vector3> ValidPossibleMoves;
        internal static Vector3 RandomlyChosenMove;
        internal static int LastUpdate;

        internal static void OnUpdate(EventArgs args)
        {
            if (Environment.TickCount - LastUpdate < 250) return;

            if (Heroes.Player.UnderTurret(true))
            {
                var turret =
                    Turrets.EnemyTurrets.FirstOrDefault(t => t.LSDistance(Heroes.Player.ServerPosition) <= 800);
                if (turret.CountNearbyAllyMinions(800) < 3) { Orbwalker.MoveTo(HeadQuarters.AllyHQ.RandomizePosition()); }
            }

            LastUpdate = Environment.TickCount;

            ValidPossibleMoves = new List<Vector3>();

            if (Heroes.Player.IsMelee || Heroes.Player.AttackRange < 450)
            {
                UseAutoSharpARAMPositioning();
            }
            else
            {
                UseAIMARAMPositioning();
            }
        }

        internal static void UseAutoSharpARAMPositioning()
        {
            var farthestAlly =
                Heroes.AllyHeroes.Where(a => !a.IsMe && !a.IsDead && a.HealthPercent > 10 && !a.InFountain()).OrderByDescending(h => h.LSDistance(HeadQuarters.AllyHQ)).FirstOrDefault();

            if (farthestAlly == null || farthestAlly.InFountain())
            {
                var minion =
                    Minions.AllyMinions.OrderByDescending(t => t.LSDistance(HeadQuarters.AllyHQ)).FirstOrDefault();
                if (minion != null)
                {
                    RandomlyChosenMove = minion.Position.RandomizePosition();
                }
                else
                {
                    var turret = Wizard.GetFarthestAllyTurret();
                    if (turret != null)
                    {
                        RandomlyChosenMove = turret.Position;
                    }
                }
                return;
            }

            if (Heroes.Player.IsMelee) RandomlyChosenMove = farthestAlly.ServerPosition.Randomize(-150, 150);

            ValidPossibleMoves.Add(farthestAlly.Position.RandomizePosition());
            //initialize the vectorlist with a position known to exist,
            //so it doesn't follow the mouse anymore

            var team = Heroes.AllyHeroes.Where(h => !h.IsMe && !h.IsDead && h.LSDistance(farthestAlly) < 500 && h.Position.LSCountEnemiesInRange(550) <= h.Position.CountAlliesInRange(550)).ToList();

            var teamPoly = team.Select(hero => new Geometry.Circle(hero.Position.LSTo2D(), 200).ToPolygon()).ToList();

            foreach (var hp in teamPoly)
            {
                foreach (var point in hp.Points)
                {
                    var v3 = point.To3D();
                    if (!point.LSIsWall() &&
                        Heroes.EnemyHeroes.Count(e => e.LSDistance(v3) < 300) <
                        Heroes.AllyHeroes.Count(e => e.LSDistance(v3) <= 300))
                    {
                        ValidPossibleMoves.Add(v3);
                    }
                }
            }
            if (Heroes.Player.LSCountEnemiesInRange(700) != 0)
            {
                var hero = Heroes.EnemyHeroes.OrderBy(h => h.LSDistance(Heroes.Player)).FirstOrDefault();
                if (hero != null)
                {
                    var PRADAPos = hero.GetPRADAPos();
                    RandomlyChosenMove = PRADAPos != Vector3.Zero
                        ? PRADAPos
                        : ValidPossibleMoves.OrderBy(v => v.LSDistance(HeadQuarters.AllyHQ.Position)).FirstOrDefault();
                    return;
                }
            }
            RandomlyChosenMove =
                ValidPossibleMoves.OrderBy(v => v.LSDistance(HeadQuarters.AllyHQ.Position)).FirstOrDefault();
        }

        internal static void UseAIMARAMPositioning()
        {
            var random = new Random();
            var allyZonePathList = AllyZone().OrderBy(p => random.Next()).FirstOrDefault();
            var allyZoneVectorList = new List<Vector2>();

            //create vectors from points and remove walls
            foreach (var point in allyZonePathList)
            {
                var v2 = new Vector2(point.X, point.Y);
                if (!v2.LSIsWall())
                {
                    allyZoneVectorList.Add(v2);
                }
            }
            var pointClosestToEnemyHQ =
                allyZoneVectorList.OrderBy(p => p.LSDistance(HeadQuarters.EnemyHQ.Position)).FirstOrDefault();
            var minNum = 150;
            var maxNum = (int)Heroes.Player.AttackRange - 100;
            if (Heroes.Player.Team == GameObjectTeam.Order)
            {
                pointClosestToEnemyHQ = GetAllyPosList().OrderByDescending(b => b.LSDistance(HeadQuarters.AllyHQ.Position)).FirstOrDefault();
                pointClosestToEnemyHQ.X = pointClosestToEnemyHQ.X - random.Next(minNum, maxNum);
                pointClosestToEnemyHQ.Y = pointClosestToEnemyHQ.Y - random.Next(minNum, maxNum);
            }
            if (Heroes.Player.Team == GameObjectTeam.Chaos)
            {
                pointClosestToEnemyHQ = GetAllyPosList().OrderByDescending(q => q.LSDistance(HeadQuarters.AllyHQ.Position)).FirstOrDefault();
                pointClosestToEnemyHQ.X = pointClosestToEnemyHQ.X + random.Next(minNum, maxNum);
                pointClosestToEnemyHQ.Y = pointClosestToEnemyHQ.Y + random.Next(minNum, maxNum);

            }
            RandomlyChosenMove = pointClosestToEnemyHQ.To3D();
        }

        #region Broscience from AIM
        /// <summary>
        /// Returns a list of points in the Ally Zone
        /// </summary>
        internal static Paths AllyZone()
        {
            var teamPolygons = new List<Geometry.Polygon>();
            foreach (var hero in Heroes.AllyHeroes.Where(h => !h.IsMe && !h.IsDead && h.HealthPercent > 10 && !(h.InFountain() || h.InShop())))
            {
                teamPolygons.Add(GetChampionRangeCircle(hero).ToPolygon());
            }
            var teamPaths = Geometry.ClipPolygons(teamPolygons);
            var newTeamPaths = teamPaths;
            foreach (var pathList in teamPaths)
            {
                Path wall = new Path();
                foreach (var path in pathList)
                {
                    if (LeagueSharp.Common.Utility.LSIsWall(new Vector2(path.X, path.Y)))
                    {
                        wall.Add(path);
                    }
                }
                newTeamPaths.Remove(wall);
            }
            return newTeamPaths;
        }

        /// <summary>
        /// Returns a list of points in the Enemy Zone
        /// </summary>
        internal static Paths EnemyZone()
        {
            var teamPolygons = new List<Geometry.Polygon>();
            foreach (var hero in Heroes.EnemyHeroes.Where(h => !h.IsDead && h.IsVisible))
            {
                teamPolygons.Add(GetChampionRangeCircle(hero).ToPolygon());
            }
            var teamPaths = Geometry.ClipPolygons(teamPolygons);
            var newTeamPaths = teamPaths;
            foreach (var pathList in teamPaths)
            {
                Path wall = new Path();
                foreach (var path in pathList)
                {
                    if (LeagueSharp.Common.Utility.LSIsWall(new Vector2(path.X, path.Y)))
                    {
                        wall.Add(path);
                    }
                }
                newTeamPaths.Remove(wall);
            }
            return newTeamPaths;
        }

        /// <summary>
        /// Returns a circle with center at hero position and radius of the highest impact range a hero has.
        /// </summary>
        /// <param name="hero">The target hero.</param>
        internal static Geometry.Circle GetChampionRangeCircle(AIHeroClient hero)
        {
            var heroSpells = new List<SpellData>
            {
                SpellData.GetSpellData(hero.GetSpell(SpellSlot.Q).Name),
                SpellData.GetSpellData(hero.GetSpell(SpellSlot.W).Name),
                SpellData.GetSpellData(hero.GetSpell(SpellSlot.E).Name),
            };
            var spellsOrderedByRange = heroSpells.OrderBy(s => s.CastRange);
            if (spellsOrderedByRange.FirstOrDefault() != null)
            {
                var highestSpellRange = spellsOrderedByRange.FirstOrDefault().CastRange;
                return new Geometry.Circle(hero.ServerPosition.LSTo2D(), highestSpellRange > hero.AttackRange ? highestSpellRange : hero.AttackRange);
            }
            return new Geometry.Circle(hero.ServerPosition.LSTo2D(), hero.AttackRange);
        }

        /// <summary>
        /// Returns a polygon that contains each position of a team champion
        /// </summary>
        /// <param name="allyTeam">returns the polygon for ally team if true, enemy if false</param>
        /// <returns></returns>
        internal static Geometry.Polygon GetTeamPolygon(bool allyTeam = true)
        {
            var poly = new Geometry.Polygon();
            foreach (var v2 in allyTeam ? GetAllyPosList() : GetEnemyPosList())
            {
                poly.Add(v2);
            }
            poly.ToClipperPath();
            return poly;
        }

        /// <summary>
        /// Returns a clipper path list of all ally champions
        /// </summary>
        public static Paths GetAllyPaths()
        {
            var allyPaths = new Paths(GetAllyPosList().Count);
            for (int i = 0; i < GetAllyPosList().Count; i++)
            {
                var randomizedAllyPos = GetAllyPosList().ToArray()[i].Randomize(-150, 150);
                allyPaths[i].Add(new IntPoint(randomizedAllyPos.X, randomizedAllyPos.Y));
            }
            return allyPaths;
        }

        /// <summary>
        /// returns a clipper paths list of all enemy champion positions
        /// </summary>
        public static Paths GetEnemyPaths()
        {
            var enemyPaths = new Paths(GetEnemyPosList().Count);
            for (int i = 0; i < GetEnemyPosList().Count; i++)
            {
                var enemyPos = GetEnemyPosList().ToArray()[i];
                enemyPaths[i].Add(new IntPoint(enemyPos.X, enemyPos.Y));
            }
            return enemyPaths;
        }

        /// <summary>
        /// returns a list of all ally positions
        /// </summary>
        public static List<Vector2> GetAllyPosList()
        {
            var allies = Heroes.AllyHeroes.Where(h => !h.IsMe && !h.IsDead && !h.InFountain()).ToList();
            return allies.Select(ally => ally.ServerPosition.LSTo2D()).ToList();
        }

        /// <summary>
        /// returns a list of all enemy positions
        /// </summary>
        public static List<Vector2> GetEnemyPosList()
        {
            var enemies = Heroes.EnemyHeroes.Where(h => !h.IsDead && h.IsVisible).ToList();
            return enemies.Select(enemy => enemy.ServerPosition.LSTo2D()).ToList();
        }
        #endregion
    }
}