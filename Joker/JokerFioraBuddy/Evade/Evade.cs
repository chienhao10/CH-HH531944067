using System;
using System.Collections.Generic;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using SharpDX;

namespace JokerFioraBuddy.Evade
{
    public class Evade
    {
        public static List<Skillshot> DetectedSkillshots = new List<Skillshot>();

        public static Vector2 PlayerPosition
        {
            get { return Player.Instance.ServerPosition.To2D(); }
        }

        private static void OnDetectSkillshot(Skillshot skillshot)
        {
            var alreadyAdded = false;
            foreach (var item in DetectedSkillshots)
            {
                if (item.SpellData.SpellName == skillshot.SpellData.SpellName &&
                    item.Unit.NetworkId == skillshot.Unit.NetworkId &&
                    skillshot.Direction.AngleBetween(item.Direction) < 5 &&
                    (skillshot.Start.Distance(item.Start) < 100 || skillshot.SpellData.FromObjects.Length == 0))
                {
                    alreadyAdded = true;
                }
            }
            if (skillshot.Unit.Team == ObjectManager.Player.Team)
            {
                return;
            }
            if (skillshot.Start.Distance(PlayerPosition) >
                (skillshot.SpellData.Range + skillshot.SpellData.Radius + 1000) * 1.5)
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
                        var end = skillshot.Start +
                                  skillshot.SpellData.Range *
                                  originalDirection.Rotated(skillshot.SpellData.MultipleAngle * i);
                        var skillshotToAdd = new Skillshot(
                            skillshot.DetectionType, skillshot.SpellData, skillshot.StartTick, skillshot.Start, end,
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
                    var newDirection = -(skillshot.End - skillshot.Start).Normalized();
                    var end = skillshot.Start + newDirection * skillshot.Start.Distance(skillshot.End);
                    var skillshotToAdd = new Skillshot(
                        skillshot.DetectionType, skillshot.SpellData, skillshot.StartTick, skillshot.Start, end,
                        skillshot.Unit);
                    DetectedSkillshots.Add(skillshotToAdd);
                    return;
                }
                if (skillshot.SpellData.Centered)
                {
                    var start = skillshot.Start - skillshot.Direction * skillshot.SpellData.Range;
                    var end = skillshot.Start + skillshot.Direction * skillshot.SpellData.Range;
                    var skillshotToAdd = new Skillshot(
                        skillshot.DetectionType, skillshot.SpellData, skillshot.StartTick, start, end, skillshot.Unit);
                    DetectedSkillshots.Add(skillshotToAdd);
                    return;
                }
                if (skillshot.SpellData.SpellName == "SyndraE" || skillshot.SpellData.SpellName == "syndrae5")
                {
                    const int Angle = 60;
                    var edge1 =
                        (skillshot.End - skillshot.Unit.ServerPosition.To2D()).Rotated(
                            -Angle / 2f * (float)Math.PI / 180);
                    var edge2 = edge1.Rotated(Angle * (float)Math.PI / 180);
                    foreach (var skillshotToAdd in
                        from minion in
                            ObjectManager.Get<Obj_AI_Minion>()
                                .Where(x => x.IsEnemy)
                                .Where(i => i.Name == "Seed" && i.Distance(skillshot.Unit) < 800)
                        let v = minion.ServerPosition.To2D() - skillshot.Unit.ServerPosition.To2D()
                        where edge1.CrossProduct(v) > 0 && v.CrossProduct(edge2) > 0
                        let start = minion.ServerPosition.To2D()
                        let end =
                            skillshot.Unit.ServerPosition.To2D()
                                .Extend(
                                    minion.ServerPosition.To2D(), skillshot.Unit.Distance(minion) > 200 ? 1300 : 1000)
                        select
                            new Skillshot(
                                skillshot.DetectionType, skillshot.SpellData, skillshot.StartTick, start, end,
                                skillshot.Unit))
                    {
                        DetectedSkillshots.Add(skillshotToAdd);
                    }
                    return;
                }
                if (skillshot.SpellData.SpellName == "AlZaharCalloftheVoid")
                {
                    var start = skillshot.End - skillshot.Direction.Perpendicular() * 400;
                    var end = skillshot.End + skillshot.Direction.Perpendicular() * 400;
                    var skillshotToAdd = new Skillshot(
                        skillshot.DetectionType, skillshot.SpellData, skillshot.StartTick, start, end, skillshot.Unit);
                    DetectedSkillshots.Add(skillshotToAdd);
                    return;
                }
                if (skillshot.SpellData.SpellName == "DianaArc")
                {
                    var skillshotToAdd = new Skillshot(
                        skillshot.DetectionType, SpellDatabase.GetByName("DianaArcArc"), skillshot.StartTick,
                        skillshot.Start, skillshot.End, skillshot.Unit);

                    DetectedSkillshots.Add(skillshotToAdd);
                }
                if (skillshot.SpellData.SpellName == "ZiggsQ")
                {
                    var d1 = skillshot.Start.Distance(skillshot.End);
                    var d2 = d1 * 0.4f;
                    var d3 = d2 * 0.69f;
                    var bounce1SpellData = SpellDatabase.GetByName("ZiggsQBounce1");
                    var bounce2SpellData = SpellDatabase.GetByName("ZiggsQBounce2");
                    var bounce1Pos = skillshot.End + skillshot.Direction * d2;
                    var bounce2Pos = bounce1Pos + skillshot.Direction * d3;
                    bounce1SpellData.Delay =
                        (int)(skillshot.SpellData.Delay + d1 / skillshot.SpellData.MissileSpeed + 500);
                    bounce2SpellData.Delay =
                        (int)(bounce1SpellData.Delay + d2 / bounce1SpellData.MissileSpeed + 500);
                    var bounce1 = new Skillshot(
                        skillshot.DetectionType, bounce1SpellData, skillshot.StartTick, skillshot.End, bounce1Pos,
                        skillshot.Unit);
                    var bounce2 = new Skillshot(
                        skillshot.DetectionType, bounce2SpellData, skillshot.StartTick, bounce1Pos, bounce2Pos,
                        skillshot.Unit);
                    DetectedSkillshots.Add(bounce1);
                    DetectedSkillshots.Add(bounce2);
                }
                if (skillshot.SpellData.SpellName == "ZiggsR")
                {
                    skillshot.SpellData.Delay =
                        (int)(1500 + 1500 * skillshot.End.Distance(skillshot.Start) / skillshot.SpellData.Range);
                }
                if (skillshot.SpellData.SpellName == "JarvanIVDragonStrike")
                {
                    var endPos = Vector2.Zero;
                    foreach (var s in DetectedSkillshots)
                    {
                        if (s.Unit.NetworkId == skillshot.Unit.NetworkId && s.SpellData.Slot == SpellSlot.E)
                        {
                            var extendedE = new Skillshot(
                                skillshot.DetectionType, skillshot.SpellData, skillshot.StartTick, skillshot.Start,
                                skillshot.End + skillshot.Direction * 100, skillshot.Unit);
                            if (!extendedE.IsSafe(s.End))
                            {
                                endPos = s.End;
                            }
                            break;
                        }
                    }
                    foreach (var m in ObjectManager.Get<Obj_AI_Minion>())
                    {
                        if (m.BaseSkinName == "jarvanivstandard" && m.Team == skillshot.Unit.Team)
                        {
                            var extendedE = new Skillshot(
                                skillshot.DetectionType, skillshot.SpellData, skillshot.StartTick, skillshot.Start,
                                skillshot.End + skillshot.Direction * 100, skillshot.Unit);
                            if (!extendedE.IsSafe(m.Position.To2D()))
                            {
                                endPos = m.Position.To2D();
                            }
                            break;
                        }
                    }
                    if (endPos.IsValid())
                    {
                        skillshot = new Skillshot(
                            DetectionType.ProcessSpell, SpellDatabase.GetByName("JarvanIVEQ"), Environment.TickCount,
                            skillshot.Start, endPos, skillshot.Unit);
                        skillshot.End = endPos + 200 * (endPos - skillshot.Start).Normalized();
                        skillshot.Direction = (skillshot.End - skillshot.Start).Normalized();
                    }
                }
            }
            if (skillshot.SpellData.SpellName == "OriannasQ")
            {
                var skillshotToAdd = new Skillshot(
                    skillshot.DetectionType, SpellDatabase.GetByName("OriannaQend"), skillshot.StartTick,
                    skillshot.Start, skillshot.End, skillshot.Unit);
                DetectedSkillshots.Add(skillshotToAdd);
            }
            if (skillshot.SpellData.DisableFowDetection && skillshot.DetectionType == DetectionType.RecvPacket)
            {
                return;
            }
            DetectedSkillshots.Add(skillshot);
        }

        static Evade()
        {
            Collision.Initialize();
            Game.OnUpdate += args =>
                {
                    DetectedSkillshots.RemoveAll(i => !i.IsActive());

                    foreach (var skillshot in DetectedSkillshots)
                    {
                        skillshot.Game_OnGameUpdate();
                    }
                };

            SkillshotDetector.OnDetectSkillshot += OnDetectSkillshot;
        }

        public static void Initialize()
        { }

        public static List<Skillshot> GetSkillshotsAboutToHit(Obj_AI_Base unit, int time)
        {
            time += 150;
            return DetectedSkillshots.Where(i => i.IsAboutToHit(time, unit)).ToList();
        }
    }
}
