using System.Collections.Generic;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu.Values;
using SharpDX;

namespace EvadePlus
{
    public static class EvadeSpellManager
    {
        public static bool ProcessFlash(EvadePlus evade)
        {
            var dangerValue = evade.GetDangerValue();
            var flashDangerValue = EvadeMenu.SpellMenu["flash"].Cast<Slider>().CurrentValue;

            if (flashDangerValue > 0 && flashDangerValue <= dangerValue)
            {
                var castPos = GetBlinkCastPos(evade, Player.Instance.ServerPosition.To2D(), 425);
                var slot = GetFlashSpellSlot();

                if (!castPos.IsZero && slot != SpellSlot.Unknown && Player.CanUseSpell(slot) == SpellState.Ready)
                {
                    //Player.IssueOrder(GameObjectOrder.Stop, Player.Instance.Position, true);
                    Player.CastSpell(slot, castPos.To3DWorld());
                    return true;
                }
            }

            return false;
        }

        public static SpellSlot GetFlashSpellSlot()
        {
            if (Player.GetSpell(SpellSlot.Summoner1).Name == "summonerflash")
                return SpellSlot.Summoner1;
            if (Player.GetSpell(SpellSlot.Summoner2).Name == "summonerflash")
                return SpellSlot.Summoner2;
            return SpellSlot.Unknown;
        }

        public static Vector2 GetBlinkCastPos(EvadePlus evade, Vector2 center, float maxRange)
        {
            var polygons = evade.ClippedPolygons.Where(p => p.IsInside(center)).ToArray();
            var segments = new List<Vector2[]>();

            foreach (var pol in polygons)
            {
                for (var i = 0; i < pol.Points.Count; i++)
                {
                    var start = pol.Points[i];
                    var end = i == pol.Points.Count - 1 ? pol.Points[0] : pol.Points[i + 1];

                    var intersections =
                        Utils.GetLineCircleIntersectionPoints(center, maxRange, start, end)
                            .Where(p => p.IsInLineSegment(start, end))
                            .ToList();

                    if (intersections.Count == 0)
                    {
                        if (start.Distance(center, true) < maxRange.Pow() &&
                            end.Distance(center, true) < maxRange.Pow())
                        {
                            intersections = new[] {start, end}.ToList();
                        }
                        else
                        {
                            continue;
                        }
                    }
                    else if (intersections.Count == 1)
                    {
                        intersections.Add(center.Distance(start, true) > center.Distance(end, true)
                            ? end
                            : start);
                    }

                    segments.Add(intersections.ToArray());
                }
            }

            if (!segments.Any())
            {
                return Vector2.Zero;
            }

            const int maxdist = 2000;
            const int division = 30;
            var points = new List<Vector2>();

            foreach (var segment in segments)
            {
                var dist = segment[0].Distance(segment[1]);
                if (dist > maxdist)
                {
                    segment[0] = segment[0].Extend(segment[1], dist/2 - maxdist/2);
                    segment[1] = segment[1].Extend(segment[1], dist/2 - maxdist/2);
                    dist = maxdist;
                }

                var step = maxdist/division;
                var count = dist/step;

                for (var i = 0; i < count; i++)
                {
                    var point = segment[0].Extend(segment[1], i*step);
                    if (!point.IsWall())
                    {
                        points.Add(point);
                    }
                }
            }

            if (!points.Any())
            {
                return Vector2.Zero;
            }

            var evadePoint =
                points.OrderByDescending(p => p.Distance(evade.LastIssueOrderPos) + p.Distance(center)).Last();
            return evadePoint;
        }
    }
}