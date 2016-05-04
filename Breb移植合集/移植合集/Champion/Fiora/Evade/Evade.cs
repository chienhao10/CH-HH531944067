namespace FioraProject.Evade
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    using SharpDX;

    using Color = System.Drawing.Color;
    using EloBuddy;
    internal class Evade
    {
        #region Static Fields

        public static List<Skillshot> DetectedSkillshots = new List<Skillshot>();

        public static int LastWardJumpAttempt = 0;

        #endregion

        #region Delegates

        public delegate void EvadingH();

        public delegate void TryEvadingH(List<Skillshot> skillshot, Vector2 to);

        #endregion

        #region Public Events

        public static event EvadingH Evading;

        public static event TryEvadingH TryEvading;

        #endregion

        #region Public Properties

        public static Vector2 PlayerPosition
        {
            get
            {
                return ObjectManager.Player.ServerPosition.LSTo2D();
            }
        }

        #endregion

        #region Public Methods and Operators

        public static void Init()
        {
            Config.CreateMenu();
            Collision.Init();
            Game.OnUpdate += args =>
                {
                    DetectedSkillshots.RemoveAll(i => !i.IsActive);
                    foreach (var skillshot in DetectedSkillshots)
                    {
                        skillshot.OnUpdate();
                    }
                    if (!Program.getKeyBindItem(Config.evadeMenu, "Enabled"))
                    {
                        return;
                    }
                    if (ObjectManager.Player.IsDead)
                    {
                        return;
                    }
                    if (ObjectManager.Player.IsCastingInterruptableSpell(true))
                    {
                        return;
                    }
                    if (ObjectManager.Player.ChampionName == "Sion" && ObjectManager.Player.HasBuff("SionR"))
                    {
                        return;
                    }
                    if (ObjectManager.Player.HasBuffOfType(BuffType.SpellShield)
                        || ObjectManager.Player.HasBuffOfType(BuffType.SpellImmunity))
                    {
                        return;
                    }
                    if (Evading != null)
                    {
                        Evading();
                    }
                    var currentPath = ObjectManager.Player.GetWaypoints();
                    var safeResult = IsSafePoint(PlayerPosition);
                    var safePath = IsSafePath(currentPath, 100);
                    if (!safePath.IsSafe && !safeResult.IsSafe && TryEvading != null)
                    {
                        TryEvading(safeResult.SkillshotList, Game.CursorPos.LSTo2D());
                    }
                };
            SkillshotDetector.OnDetectSkillshot += OnDetectSkillshot;
            SkillshotDetector.OnDeleteMissile += (skillshot, missile) =>
                {
                    if (skillshot.SpellData.SpellName == "VelkozQ")
                    {
                        var spellData = SpellDatabase.GetByName("VelkozQSplit");
                        var direction = skillshot.Direction.LSPerpendicular();
                        if (DetectedSkillshots.Count(i => i.SpellData.SpellName == "VelkozQSplit") == 0)
                        {
                            for (var i = -1; i <= 1; i = i + 2)
                            {
                                var skillshotToAdd = new Skillshot(
                                    DetectionType.ProcessSpell,
                                    spellData,
                                    Utils.GameTimeTickCount,
                                    missile.Position.LSTo2D(),
                                    missile.Position.LSTo2D() + i * direction * spellData.Range,
                                    skillshot.Unit);
                                DetectedSkillshots.Add(skillshotToAdd);
                            }
                        }
                    }
                };
            Drawing.OnDraw += args =>
                {
                    if (Program.getCheckBoxItem(Config.evadeMenu, "DrawStatus"))
                    {
                        var active = Program.getKeyBindItem(Config.evadeMenu, "Enabled");
                        var text = string.Format("Evade Skillshot: {0}", active ? (Program.getKeyBindItem(Config.evadeMenu, "OnlyDangerous") ? "Dangerous" : "On") : "Off");
                        var pos = Drawing.WorldToScreen(ObjectManager.Player.Position);
                        Drawing.DrawText(pos.X - (float)50 / 2, pos.Y + 40, active ? (Program.getKeyBindItem(Config.evadeMenu, "OnlyDangerous") ? Color.Yellow : Color.White) : Color.Gray, text);
                    }
                    foreach (var skillshot in DetectedSkillshots)
                    {
                        skillshot.Draw(skillshot.Enable && Program.getKeyBindItem(Config.evadeMenu, "Enabled") ? Color.White : Color.Red, Color.LimeGreen);
                    }
                };
        }

        public static bool IsAboutToHit(Obj_AI_Base unit, int time)
        {
            return SkillshotAboutToHit(unit, time).Count > 0;
        }

        public static SafePathResult IsSafePath(List<Vector2> path, int timeOffset, int speed = -1, int delay = 0)
        {
            var isSafe = true;
            var intersections = new List<FoundIntersection>();
            var intersection = new FoundIntersection();
            foreach (var sResult in
                from skillshot in DetectedSkillshots
                where skillshot.Enable
                select skillshot.IsSafePath(path, timeOffset, speed, delay))
            {
                isSafe = isSafe && sResult.IsSafe;
                if (sResult.Intersection.Valid)
                {
                    intersections.Add(sResult.Intersection);
                }
            }
            if (isSafe)
            {
                return new SafePathResult(true, intersection);
            }
            var intersetion = intersections.MinOrDefault(i => i.Distance);
            return new SafePathResult(false, intersetion.Valid ? intersetion : intersection);
        }

        public static IsSafeResult IsSafePoint(Vector2 point)
        {
            var result = new IsSafeResult { SkillshotList = new List<Skillshot>() };
            foreach (var skillshot in DetectedSkillshots.Where(i => i.Enable && i.IsDanger(point)))
            {
                result.SkillshotList.Add(skillshot);
            }
            result.IsSafe = result.SkillshotList.Count == 0;
            return result;
        }

        public static bool IsSafeToBlink(Vector2 point, int timeOffset, int delay)
        {
            return DetectedSkillshots.Where(i => i.Enable).All(i => i.IsSafeToBlink(point, timeOffset, delay));
        }

        public static List<Skillshot> SkillshotAboutToHit(Obj_AI_Base unit, int time)
        {
            time += 150;
            return DetectedSkillshots.Where(i => i.Enable && i.IsAboutToHit(time, unit)).ToList();
        }

        #endregion

        #region Methods

        private static void OnDetectSkillshot(Skillshot skillshot)
        {
            var alreadyAdded = false;
            foreach (var item in DetectedSkillshots)
            {
                if (item.SpellData.SpellName == skillshot.SpellData.SpellName
                    && item.Unit.NetworkId == skillshot.Unit.NetworkId
                    && skillshot.Direction.LSAngleBetween(item.Direction) < 5
                    && (skillshot.Start.LSDistance(item.Start) < 100 || skillshot.SpellData.FromObjects.Length == 0))
                {
                    alreadyAdded = true;
                }
            }
            if (skillshot.Unit.Team == ObjectManager.Player.Team)
            {
                return;
            }
            if (skillshot.Start.LSDistance(PlayerPosition)
                > (skillshot.SpellData.Range + skillshot.SpellData.Radius + 1000) * 1.5)
            {
                return;
            }
            if (alreadyAdded && !skillshot.SpellData.DontCheckForDuplicates)
            {
                return;
            }
            if (skillshot.DetectionType == DetectionType.ProcessSpell)
            {
                if (skillshot.SpellData.MultipleNumber != -1)
                {
                    var originalDirection = skillshot.Direction;
                    for (var i = -(skillshot.SpellData.MultipleNumber - 1) / 2;
                         i <= (skillshot.SpellData.MultipleNumber - 1) / 2;
                         i++)
                    {
                        var end = skillshot.Start
                                  + skillshot.SpellData.Range
                                  * originalDirection.LSRotated(skillshot.SpellData.MultipleAngle * i);
                        var skillshotToAdd = new Skillshot(
                            skillshot.DetectionType,
                            skillshot.SpellData,
                            skillshot.StartTick,
                            skillshot.Start,
                            end,
                            skillshot.Unit);
                        DetectedSkillshots.Add(skillshotToAdd);
                    }
                    return;
                }
                if (skillshot.SpellData.SpellName == "UFSlash")
                {
                    skillshot.SpellData.MissileSpeed = 1600 + (int)skillshot.Unit.MoveSpeed;
                }
                if (skillshot.SpellData.SpellName == "SionR")
                {
                    skillshot.SpellData.MissileSpeed = (int)skillshot.Unit.MoveSpeed;
                }
                if (skillshot.SpellData.Invert)
                {
                    var newDirection = -(skillshot.End - skillshot.Start).LSNormalized();
                    var end = skillshot.Start + newDirection * skillshot.Start.LSDistance(skillshot.End);
                    var skillshotToAdd = new Skillshot(
                        skillshot.DetectionType,
                        skillshot.SpellData,
                        skillshot.StartTick,
                        skillshot.Start,
                        end,
                        skillshot.Unit);
                    DetectedSkillshots.Add(skillshotToAdd);
                    return;
                }
                if (skillshot.SpellData.Centered)
                {
                    var start = skillshot.Start - skillshot.Direction * skillshot.SpellData.Range;
                    var end = skillshot.Start + skillshot.Direction * skillshot.SpellData.Range;
                    var skillshotToAdd = new Skillshot(
                        skillshot.DetectionType,
                        skillshot.SpellData,
                        skillshot.StartTick,
                        start,
                        end,
                        skillshot.Unit);
                    DetectedSkillshots.Add(skillshotToAdd);
                    return;
                }
                if (skillshot.SpellData.SpellName == "SyndraE" || skillshot.SpellData.SpellName == "syndrae5")
                {
                    const int Angle = 60;
                    var edge1 =
                        (skillshot.End - skillshot.Unit.ServerPosition.LSTo2D()).LSRotated(
                            -Angle / 2f * (float)Math.PI / 180);
                    var edge2 = edge1.LSRotated(Angle * (float)Math.PI / 180);
                    foreach (var skillshotToAdd in
                        from minion in
                            ObjectManager.Get<Obj_AI_Minion>().Where(x=>x.IsEnemy).Where(i => i.Name == "Seed" && i.LSDistance(skillshot.Unit) < 800)
                        let v = minion.ServerPosition.LSTo2D() - skillshot.Unit.ServerPosition.LSTo2D()
                        where edge1.LSCrossProduct(v) > 0 && v.LSCrossProduct(edge2) > 0
                        let start = minion.ServerPosition.LSTo2D()
                        let end =
                            skillshot.Unit.ServerPosition.LSTo2D()
                            .LSExtend(
                                minion.ServerPosition.LSTo2D(),
                                skillshot.Unit.LSDistance(minion) > 200 ? 1300 : 1000)
                        select
                            new Skillshot(
                            skillshot.DetectionType,
                            skillshot.SpellData,
                            skillshot.StartTick,
                            start,
                            end,
                            skillshot.Unit))
                    {
                        DetectedSkillshots.Add(skillshotToAdd);
                    }
                    return;
                }
                if (skillshot.SpellData.SpellName == "AlZaharCalloftheVoid")
                {
                    var start = skillshot.End - skillshot.Direction.LSPerpendicular() * 400;
                    var end = skillshot.End + skillshot.Direction.LSPerpendicular() * 400;
                    var skillshotToAdd = new Skillshot(
                        skillshot.DetectionType,
                        skillshot.SpellData,
                        skillshot.StartTick,
                        start,
                        end,
                        skillshot.Unit);
                    DetectedSkillshots.Add(skillshotToAdd);
                    return;
                }
                if (skillshot.SpellData.SpellName == "DianaArc")
                {
                    var skillshotToAdd = new Skillshot(
                        skillshot.DetectionType,
                        SpellDatabase.GetByName("DianaArcArc"),
                        skillshot.StartTick,
                        skillshot.Start,
                        skillshot.End,
                        skillshot.Unit);

                    DetectedSkillshots.Add(skillshotToAdd);
                }
                if (skillshot.SpellData.SpellName == "ZiggsQ")
                {
                    var d1 = skillshot.Start.LSDistance(skillshot.End);
                    var d2 = d1 * 0.4f;
                    var d3 = d2 * 0.69f;
                    var bounce1SpellData = SpellDatabase.GetByName("ZiggsQBounce1");
                    var bounce2SpellData = SpellDatabase.GetByName("ZiggsQBounce2");
                    var bounce1Pos = skillshot.End + skillshot.Direction * d2;
                    var bounce2Pos = bounce1Pos + skillshot.Direction * d3;
                    bounce1SpellData.Delay =
                        (int)(skillshot.SpellData.Delay + d1 * 1000f / skillshot.SpellData.MissileSpeed + 500);
                    bounce2SpellData.Delay =
                        (int)(bounce1SpellData.Delay + d2 * 1000f / bounce1SpellData.MissileSpeed + 500);
                    var bounce1 = new Skillshot(
                        skillshot.DetectionType,
                        bounce1SpellData,
                        skillshot.StartTick,
                        skillshot.End,
                        bounce1Pos,
                        skillshot.Unit);
                    var bounce2 = new Skillshot(
                        skillshot.DetectionType,
                        bounce2SpellData,
                        skillshot.StartTick,
                        bounce1Pos,
                        bounce2Pos,
                        skillshot.Unit);
                    DetectedSkillshots.Add(bounce1);
                    DetectedSkillshots.Add(bounce2);
                }
                if (skillshot.SpellData.SpellName == "ZiggsR")
                {
                    skillshot.SpellData.Delay =
                        (int)(1500 + 1500 * skillshot.End.LSDistance(skillshot.Start) / skillshot.SpellData.Range);
                }
                if (skillshot.SpellData.SpellName == "JarvanIVDragonStrike")
                {
                    var endPos = Vector2.Zero;
                    foreach (var s in DetectedSkillshots)
                    {
                        if (s.Unit.NetworkId == skillshot.Unit.NetworkId && s.SpellData.Slot == SpellSlot.E)
                        {
                            var extendedE = new Skillshot(
                                skillshot.DetectionType,
                                skillshot.SpellData,
                                skillshot.StartTick,
                                skillshot.Start,
                                skillshot.End + skillshot.Direction * 100,
                                skillshot.Unit);
                            if (!extendedE.IsSafe(s.End))
                            {
                                endPos = s.End;
                            }
                            break;
                        }
                    }
                    foreach (var m in ObjectManager.Get<Obj_AI_Minion>())
                    {
                        if (m.CharData.BaseSkinName == "jarvanivstandard" && m.Team == skillshot.Unit.Team)
                        {
                            var extendedE = new Skillshot(
                                skillshot.DetectionType,
                                skillshot.SpellData,
                                skillshot.StartTick,
                                skillshot.Start,
                                skillshot.End + skillshot.Direction * 100,
                                skillshot.Unit);
                            if (!extendedE.IsSafe(m.Position.LSTo2D()))
                            {
                                endPos = m.Position.LSTo2D();
                            }
                            break;
                        }
                    }
                    if (endPos.IsValid())
                    {
                        skillshot = new Skillshot(
                            DetectionType.ProcessSpell,
                            SpellDatabase.GetByName("JarvanIVEQ"),
                            Utils.GameTimeTickCount,
                            skillshot.Start,
                            endPos,
                            skillshot.Unit);
                        skillshot.End = endPos + 200 * (endPos - skillshot.Start).LSNormalized();
                        skillshot.Direction = (skillshot.End - skillshot.Start).LSNormalized();
                    }
                }
            }
            if (skillshot.SpellData.SpellName == "OriannasQ")
            {
                var skillshotToAdd = new Skillshot(
                    skillshot.DetectionType,
                    SpellDatabase.GetByName("OriannaQend"),
                    skillshot.StartTick,
                    skillshot.Start,
                    skillshot.End,
                    skillshot.Unit);
                DetectedSkillshots.Add(skillshotToAdd);
            }
            if (skillshot.SpellData.DisableFowDetection && skillshot.DetectionType == DetectionType.RecvPacket)
            {
                return;
            }
            DetectedSkillshots.Add(skillshot);
        }

        #endregion

        public struct IsSafeResult
        {
            #region Fields

            public bool IsSafe;

            public List<Skillshot> SkillshotList;

            #endregion
        }
    }
}