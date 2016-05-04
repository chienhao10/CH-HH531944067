using System.Xml.Serialization;

namespace FioraProject.Evade
{
    using System.Collections.Generic;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    using SharpDX;
    using EloBuddy;
    public static class Evader
    {
        #region Public Methods and Operators

        public static List<Vector2> GetEvadePoints(
            int speed = -1,
            int delay = 0,
            bool isBlink = false,
            bool onlyGood = false)
        {
            speed = speed == -1 ? (int)ObjectManager.Player.MoveSpeed : speed;
            var goodCandidates = new List<Vector2>();
            var badCandidates = new List<Vector2>();
            var polygonList = new List<Geometry.Polygon>();
            var takeClosestPath = false;
            foreach (var skillshot in Evade.DetectedSkillshots.Where(i => i.Enable))
            {
                if (skillshot.SpellData.TakeClosestPath && skillshot.IsDanger(Evade.PlayerPosition))
                {
                    takeClosestPath = true;
                }
                polygonList.Add(skillshot.EvadePolygon);
            }
            var dangerPolygons = Geometry.ClipPolygons(polygonList).ToPolygons();
            var myPosition = Evade.PlayerPosition;
            foreach (var poly in dangerPolygons)
            {
                for (var i = 0; i <= poly.Points.Count - 1; i++)
                {
                    var sideStart = poly.Points[i];
                    var sideEnd = poly.Points[(i == poly.Points.Count - 1) ? 0 : i + 1];
                    var originalCandidate = myPosition.LSProjectOn(sideStart, sideEnd).SegmentPoint;
                    var distanceToEvadePoint = Vector2.DistanceSquared(originalCandidate, myPosition);
                    if (distanceToEvadePoint < 600 * 600)
                    {
                        var sideDistance = Vector2.DistanceSquared(sideEnd, sideStart);
                        var direction = (sideEnd - sideStart).LSNormalized();
                        var s = (distanceToEvadePoint < 200 * 200 && sideDistance > 90 * 90)
                                    ? Config.DiagonalEvadePointsCount
                                    : 0;
                        for (var j = -s; j <= s; j++)
                        {
                            var candidate = originalCandidate + j * Config.DiagonalEvadePointsStep * direction;
                            var pathToPoint = ObjectManager.Player.GetPath(candidate.To3D()).ToList().LSTo2D();
                            if (!isBlink)
                            {
                                if (Evade.IsSafePath(pathToPoint, Config.EvadingFirstTimeOffset, speed, delay).IsSafe)
                                {
                                    goodCandidates.Add(candidate);
                                }
                                if (Evade.IsSafePath(pathToPoint, Config.EvadingSecondTimeOffset, speed, delay).IsSafe
                                    && j == 0)
                                {
                                    badCandidates.Add(candidate);
                                }
                            }
                            else
                            {
                                if (Evade.IsSafeToBlink(
                                    pathToPoint[pathToPoint.Count - 1],
                                    Config.EvadingFirstTimeOffset,
                                    delay))
                                {
                                    goodCandidates.Add(candidate);
                                }
                                if (Evade.IsSafeToBlink(
                                    pathToPoint[pathToPoint.Count - 1],
                                    Config.EvadingSecondTimeOffset,
                                    delay))
                                {
                                    badCandidates.Add(candidate);
                                }
                            }
                        }
                    }
                }
            }
            if (takeClosestPath)
            {
                if (goodCandidates.Count > 0)
                {
                    goodCandidates = new List<Vector2>
                                         { goodCandidates.MinOrDefault(i => ObjectManager.Player.LSDistance(i,true)) };
                }
                if (badCandidates.Count > 0)
                {
                    badCandidates = new List<Vector2>
                                        { badCandidates.MinOrDefault(i => ObjectManager.Player.LSDistance(i,true)) };
                }
            }
            return goodCandidates.Count > 0 ? goodCandidates : (onlyGood ? new List<Vector2>() : badCandidates);
        }

        public static List<Obj_AI_Base> GetEvadeTargets(
            SpellValidTargets[] validTargets,
            int speed,
            int delay,
            float range,
            bool isBlink = false,
            bool onlyGood = false,
            bool dontCheckForSafety = false)
        {
            var badTargets = new List<Obj_AI_Base>();
            var goodTargets = new List<Obj_AI_Base>();
            var allTargets = new List<Obj_AI_Base>();
            foreach (var targetType in validTargets)
            {
                switch (targetType)
                {
                    case SpellValidTargets.AllyChampions:
                        allTargets.AddRange(HeroManager.Allies.Where(i => i.LSIsValidTarget(range, false) && !i.IsMe));
                        break;
                    case SpellValidTargets.AllyMinions:
                        allTargets.AddRange(
                            ObjectManager.Get<Obj_AI_Minion>().Where(x => x.IsAlly).Where(
                                i =>
                                    i.LSIsValidTarget(range, false, ObjectManager.Player.Position) &&
                                    MinionManager.IsMinion(i)));
                        break;
                    case SpellValidTargets.AllyWards:
                        allTargets.AddRange(
                            ObjectManager.Get<Obj_AI_Minion>()
                                .Where(x => x.IsAlly && MinionManager.IsWard(x))
                                .Where(i => i.LSIsValidTarget(range, false)));
                        break;
                    case SpellValidTargets.EnemyChampions:
                        allTargets.AddRange(HeroManager.Enemies.Where(i => i.LSIsValidTarget(range)));
                        break;
                    case SpellValidTargets.EnemyMinions:
                        allTargets.AddRange(
                            ObjectManager.Get<Obj_AI_Minion>().Where(x => x.IsEnemy).Where(
                                i =>
                                    i.LSIsValidTarget(range, true, ObjectManager.Player.Position) &&
                                    MinionManager.IsMinion(i)));
                        allTargets.AddRange(
                            ObjectManager.Get<Obj_AI_Minion>()
                                .Where(x => x.Team == GameObjectTeam.Neutral)
                                .Where(i => i.LSIsValidTarget(range, true, ObjectManager.Player.Position)));
                        break;
                    case SpellValidTargets.EnemyWards:
                        allTargets.AddRange(
                            ObjectManager.Get<Obj_AI_Minion>()
                                .Where(x => x.IsEnemy && MinionManager.IsWard(x))
                                .Where(i => i.LSIsValidTarget(range)));
                        break;

                }
            }
            foreach (var target in
                allTargets.Where(i => dontCheckForSafety || Evade.IsSafePoint(i.ServerPosition.LSTo2D()).IsSafe))
            {
                if (isBlink)
                {
                    if (Utils.GameTimeTickCount - Evade.LastWardJumpAttempt < 250
                        || Evade.IsSafeToBlink(target.ServerPosition.LSTo2D(), Config.EvadingFirstTimeOffset, delay))
                    {
                        goodTargets.Add(target);
                    }
                    if (Utils.GameTimeTickCount - Evade.LastWardJumpAttempt < 250
                        || Evade.IsSafeToBlink(target.ServerPosition.LSTo2D(), Config.EvadingSecondTimeOffset, delay))
                    {
                        badTargets.Add(target);
                    }
                }
                else
                {
                    var pathToTarget = new List<Vector2> { Evade.PlayerPosition, target.ServerPosition.LSTo2D() };
                    if (Utils.GameTimeTickCount - Evade.LastWardJumpAttempt < 250
                        || Evade.IsSafePath(pathToTarget, Config.EvadingFirstTimeOffset, speed, delay).IsSafe)
                    {
                        goodTargets.Add(target);
                    }
                    if (Utils.GameTimeTickCount - Evade.LastWardJumpAttempt < 250
                        || Evade.IsSafePath(pathToTarget, Config.EvadingSecondTimeOffset, speed, delay).IsSafe)
                    {
                        badTargets.Add(target);
                    }
                }
            }
            return goodTargets.Count > 0 ? goodTargets : (onlyGood ? new List<Obj_AI_Base>() : badTargets);
        }

        #endregion
    }
}