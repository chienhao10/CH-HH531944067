namespace Valvrave_Sharp.Evade
{
    #region

    using System.Collections.Generic;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.SDK;
    using LeagueSharp.SDK.Core.Utils;

    using SharpDX;

    using Valvrave_Sharp.Core;

    #endregion
    using EloBuddy;
    using EloBuddy.SDK.Menu.Values;
    using EloBuddy.SDK.Menu;
    public static class Evader
    {
        #region Public Methods and Operators

        public static List<Vector2> GetEvadePoints(
            int speed = -1,
            int delay = 0,
            bool isBlink = false,
            bool onlyGood = false)
        {
            speed = speed == -1 ? (int)Program.Player.MoveSpeed : speed;
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
                    var sideEnd = poly.Points[i == poly.Points.Count - 1 ? 0 : i + 1];
                    var originalCandidate = myPosition.LSProjectOn(sideStart, sideEnd).SegmentPoint;
                    var distanceToEvadePoint = originalCandidate.LSDistanceSquared(myPosition);
                    if (!(distanceToEvadePoint < 600 * 600))
                    {
                        continue;
                    }
                    var sideDistance = sideEnd.LSDistanceSquared(sideStart);
                    var direction = (sideEnd - sideStart).LSNormalized();
                    var s = distanceToEvadePoint < 200 * 200 && sideDistance > 90 * 90
                                ? Config.DiagonalEvadePointsCount
                                : 0;
                    for (var j = -s; j <= s; j++)
                    {
                        var candidate = originalCandidate + j * Config.DiagonalEvadePointsStep * direction;
                        var pathToPoint = Program.Player.GetPath(candidate.ToVector3()).ToList().ToVector2();
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
            if (takeClosestPath)
            {
                if (goodCandidates.Count > 0)
                {
                    goodCandidates = new List<Vector2>
                                         { goodCandidates.MinOrDefault(i => Program.Player.DistanceSquared(i)) };
                }
                if (badCandidates.Count > 0)
                {
                    badCandidates = new List<Vector2>
                                        { badCandidates.MinOrDefault(i => Program.Player.DistanceSquared(i)) };
                }
            }
            return goodCandidates.Count > 0 ? goodCandidates : (onlyGood ? new List<Vector2>() : badCandidates);
        }

        #endregion

        #region Methods

        public static bool getCheckBoxItem(string item)
        {
            return Config.evadeMenu[item].Cast<CheckBox>().CurrentValue;
        }

        public static int getSliderItem(string item)
        {
            return Config.evadeMenu[item].Cast<Slider>().CurrentValue;
        }

        public static bool getKeyBindItem(string item)
        {
            return Config.evadeMenu[item].Cast<KeyBind>().CurrentValue;
        }

        public static int getBoxItem(string item)
        {
            return Config.evadeMenu[item].Cast<ComboBox>().CurrentValue;
        }

        internal static List<Obj_AI_Base> GetEvadeTargets(
            this EvadeSpellData spellData,
            bool onlyGood = false,
            bool dontCheckForSafety = false)
        {
            var badTargets = new List<Obj_AI_Base>();
            var goodTargets = new List<Obj_AI_Base>();
            var allTargets = new List<Obj_AI_Base>();
            foreach (var targetType in spellData.ValidTargets)
            {
                switch (targetType)
                {
                    case SpellValidTargets.AllyChampions:
                        allTargets.AddRange(
                            GameObjects.AllyHeroes.Where(i => i.LSIsValidTarget(spellData.Range, false) && !i.IsMe));
                        break;
                    case SpellValidTargets.AllyMinions:
                        allTargets.AddRange(
                            GameObjects.AllyMinions.Where(
                                i => i.LSIsValidTarget(spellData.Range, false) && (i.IsMinion() || i.IsPet())));
                        break;
                    case SpellValidTargets.AllyWards:
                        allTargets.AddRange(
                            GameObjects.AllyWards.Where(i => i.LSIsValidTarget(spellData.Range, false) && i.IsWard()));
                        break;
                    case SpellValidTargets.EnemyChampions:
                        allTargets.AddRange(GameObjects.EnemyHeroes.Where(i => i.LSIsValidTarget(spellData.Range)));
                        break;
                    case SpellValidTargets.EnemyMinions:
                        allTargets.AddRange(
                            GameObjects.EnemyMinions.Where(
                                i => i.LSIsValidTarget(spellData.Range) && (i.IsMinion() || i.IsPet(false))));
                        allTargets.AddRange(GameObjects.Jungle.Where(i => i.LSIsValidTarget(spellData.Range)));
                        break;
                    case SpellValidTargets.EnemyWards:
                        allTargets.AddRange(GameObjects.EnemyWards.Where(i => i.LSIsValidTarget(spellData.Range)));
                        break;
                }
            }
            var underTower = getCheckBoxItem(spellData.Name + "Tower");
            foreach (var target in allTargets)
            {
                if (spellData.CheckBuffName != "" && target.HasBuff(spellData.CheckBuffName))
                {
                    continue;
                }
                var pos = spellData.FixedRange
                              ? Evade.PlayerPosition.Extend(target.ServerPosition, spellData.Range)
                              : target.ServerPosition.ToVector2();
                if (!dontCheckForSafety && !Evade.IsSafePoint(pos).IsSafe)
                {
                    continue;
                }
                if (spellData.UnderTower && pos.IsUnderEnemyTurret() && !underTower)
                {
                    continue;
                }
                if (spellData.IsBlink)
                {
                    if (Evade.IsSafeToBlink(pos, Config.EvadingFirstTimeOffset, spellData.Delay))
                    {
                        goodTargets.Add(target);
                    }
                    if (Evade.IsSafeToBlink(pos, Config.EvadingSecondTimeOffset, spellData.Delay))
                    {
                        badTargets.Add(target);
                    }
                }
                else if (spellData.IsDash)
                {
                    var pathToTarget = new List<Vector2> { Evade.PlayerPosition, pos };
                    if (Variables.TickCount - Evade.LastWardJumpAttempt < 250
                        || Evade.IsSafePath(
                            pathToTarget,
                            Config.EvadingFirstTimeOffset,
                            spellData.Speed,
                            spellData.Delay).IsSafe)
                    {
                        goodTargets.Add(target);
                    }
                    if (Variables.TickCount - Evade.LastWardJumpAttempt < 250
                        || Evade.IsSafePath(
                            pathToTarget,
                            Config.EvadingSecondTimeOffset,
                            spellData.Speed,
                            spellData.Delay).IsSafe)
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