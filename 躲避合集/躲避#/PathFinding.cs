using System.Collections.Generic;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using SharpDX;

namespace EvadePlus
{
    public class PathFinding
    {
        public EvadePlus Evade;

        public PathFinding(EvadePlus evade)
        {
            Evade = evade;
        }

        public Vector2[] CreatePathOld(Vector2 start, Vector2 end)
        {
            const int extraWidth = 30;
            var walkPolygons = Geometry.ClipPolygons(Evade.Skillshots.Select(c => c.ToPolygon(extraWidth))).ToPolygons();

            //if (walkPolygons.Any(pol => pol.IsInside(start)))
            //{
            //    var polPoints =
            //        Geometry.ClipPolygons(
            //            Evade.SkillshotDetector.DetectedSkillshots.Where(c => c.IsValid)
            //                .Select(c => c.ToPolygon(extraWidth))
            //                .ToList())
            //            .ToPolygons()
            //            .Where(pol => pol.IsInside(start))
            //            .SelectMany(pol => pol.Points)
            //            .ToList();
            //    polPoints.Sort((p1, p2) => p1.Distance(start, true).CompareTo(p2.Distance(start, true)));
            //    start = polPoints.First().Extend(start, -150);
            //}

            //if (walkPolygons.Any(pol => pol.IsInside(end)))
            //{
            //    var polPoints =
            //        Geometry.ClipPolygons(
            //            Evade.SkillshotDetector.DetectedSkillshots.Where(c => c.IsValid)
            //                .Select(c => c.ToPolygon(extraWidth))
            //                .ToList())
            //            .ToPolygons()
            //            .Where(pol => pol.IsInside(end))
            //            .SelectMany(pol => pol.Points)
            //            .ToList();
            //    polPoints.Sort((p1, p2) => p1.Distance(end, true).CompareTo(p2.Distance(end, true)));
            //    end = polPoints.First().Extend(end, -extraWidth);
            //}

            var ritoPath = Player.Instance.GetPath(end.To3DWorld()).ToArray().ToVector2().ToList(); //start.To3DWorld(), 
            var pathPoints = new List<Vector2>();
            var polygonDictionary = new Dictionary<Vector2, Geometry.Polygon>();

            for (var i = 0; i < ritoPath.Count - 1; i++)
            {
                var lineStart = ritoPath[i];
                var lineEnd = ritoPath[i + 1];

                foreach (var pol in walkPolygons)
                {
                    var intersectionPoints = pol.GetIntersectionPointsWithLineSegment(lineStart, lineEnd);
                    foreach (var p in intersectionPoints)
                    {
                        if (!polygonDictionary.ContainsKey(p))
                        {
                            polygonDictionary.Add(p, pol);
                            pathPoints.Add(p);
                        }
                    }
                }
            }
            ritoPath.RemoveAll(p => walkPolygons.Any(pol => pol.IsInside(p)));
            pathPoints.AddRange(ritoPath);
            pathPoints.SortPath(Player.Instance.ServerPosition.To2D());

            var path = new List<Vector2>();

            while (pathPoints.Count > 0)
            {
                if (pathPoints.Count == 1)
                {
                    path.Add(pathPoints[0]);
                    break;
                }

                var current = pathPoints[0];
                var next = pathPoints[1];

                Geometry.Polygon pol1;
                Geometry.Polygon pol2;

                if (polygonDictionary.TryGetValue(current, out pol1) && polygonDictionary.TryGetValue(next, out pol2) &&
                    pol1.Equals(pol2))
                {
                    var detailedPolygon = pol1.ToDetailedPolygon();
                    detailedPolygon.Points.Sort(
                        (p1, p2) => p1.Distance(current, true).CompareTo(p2.Distance(current, true)));
                    current = detailedPolygon.Points.First();

                    detailedPolygon.Points.Sort((p1, p2) => p1.Distance(next, true).CompareTo(p2.Distance(next, true)));
                    next = detailedPolygon.Points.First();

                    detailedPolygon = pol1.ToDetailedPolygon();
                    var index = detailedPolygon.Points.FindIndex(p => p == current);
                    var linkedList = new LinkedList<Vector2>(detailedPolygon.Points, index);

                    var nextPath = new List<Vector2>();
                    var previousPath = new List<Vector2>();
                    var nextLength = 0F;
                    var previousLength = 0F;
                    var nextWall = false;
                    var previousWall = false;

                    while (true)
                    {
                        var c = linkedList.Next();

                        if (c.IsWall())
                        {
                            nextWall = true;
                            break;
                        }

                        nextPath.Add(c);

                        if (nextPath.Count > 1)
                            nextLength += nextPath[nextPath.Count - 2].Distance(c, true);

                        if (c == next)
                            break;
                    }

                    linkedList.Index = index;
                    while (true)
                    {
                        var c = linkedList.Previous();

                        if (c.IsWall())
                        {
                            previousWall = true;
                            break;
                        }

                        previousPath.Add(c);

                        if (previousPath.Count > 1)
                            previousLength += previousPath[previousPath.Count - 2].Distance(c, true);

                        if (c == next)
                            break;
                    }

                    var shortest = nextWall && previousWall
                        ? (nextLength > previousLength ? nextPath : previousPath)
                        : (nextWall || previousWall
                            ? (nextWall ? previousPath : nextPath)
                            : nextLength < previousLength ? nextPath : previousPath);
                    path.AddRange(shortest);

                    if (previousWall && nextWall)
                        break;
                }
                else
                {
                    path.Add(current);
                    path.Add(next);
                }

                pathPoints.RemoveRange(0, 2);
            }

            return path.ToArray();
        }

        public Vector2[] CreatePath(Vector2 start, Vector2 end)
        {
            var path = Player.Instance.GetPath(end.To3DWorld(), true);
            var polygons = Evade.ClippedPolygons.OrderByDescending(p => p.CenterOfPolygon().Distance(Player.Instance, true)).Reverse().ToArray();

            for (var i = 0; i < path.Length - 1; i++)
            {
                var lineStart = path[i];
                var lineEnd = path[i + 1];

                foreach (var pol in polygons)
                {
                    var points = pol.GetIntersectionPointsWithLineSegment(lineStart.To2D(), lineEnd.To2D());

                    if (points.Length > 0)
                    {
                        var endPoint =
                            points.OrderByDescending(p => p.Distance(Player.Instance, true))
                                .Last()
                                .Extend(Player.Instance.ServerPosition, Player.Instance.HitBoxRadius());
                        return new[] { Player.Instance.ServerPosition.To2D(), endPoint };
                    }
                }
            }

            return path.ToVector2();
        }

        public Vector2[] CleanPath(Vector2[] path)
        {
            if (path == null || path.Length == 0)
            {
                return path;
            }

            var newPath = new List<Vector2>();
            var current = path[0];

            for (var i = 1; i < path.Length; i++)
            {
                if (Evade.ClippedPolygons.Any(p => p.IsIntersectingWithLineSegment(current, path[i])))
                {
                    newPath.Add(path[i]);
                    current = path[i];
                }
            }

            newPath.Add(current);
            newPath.Add(path.Last());

            return newPath.ToArray();
        }

        public Vector2[] GetPath(Vector2 start, Vector2 end)
        {
            return /*CleanPath*/(CreatePath(start, end));
        }
    }
}