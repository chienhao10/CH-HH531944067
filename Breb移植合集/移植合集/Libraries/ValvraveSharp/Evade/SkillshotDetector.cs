namespace Valvrave_Sharp.Evade
{
    #region

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;

    using LeagueSharp;
    using LeagueSharp.SDK;
    using LeagueSharp.SDK.Core.Utils;

    using SharpDX;

    #endregion
    using EloBuddy;
    internal static class SkillshotDetector
    {
        #region Static Fields

        private static readonly List<Obj_AI_Base> TrackObjects = new List<Obj_AI_Base>();

        #endregion

        #region Constructors and Destructors

        static SkillshotDetector()
        {
            Obj_AI_Base.OnProcessSpellCast += OnProcessSpellCast;
            GameObject.OnCreate += (sender, args) => { DelayAction.Add(0, () => MissileOnCreate(sender)); };
            GameObject.OnDelete += MissileOnDelete;
            GameObject.OnCreate += (sender, args) =>
                {
                    var spellData = SpellDatabase.GetBySourceObjectName(sender.Name);
                    if (spellData == null || Config.evadeMenu[spellData.SpellName + "Enabled"] == null)
                    {
                        return;
                    }
                    TriggerOnDetectSkillshot(
                        DetectionType.ProcessSpell,
                        spellData,
                        Variables.TickCount - Game.Ping / 2,
                        sender.Position.ToVector2(),
                        sender.Position.ToVector2(),
                        sender.Position.ToVector2(),
                        GameObjects.Heroes.MinOrDefault(i => i.IsAlly ? 1 : 0));
                };
            GameObject.OnDelete += (sender, args) =>
                {
                    if (!sender.IsValid || sender.Team == Program.Player.Team)
                    {
                        return;
                    }
                    for (var i = Evade.DetectedSkillshots.Count - 1; i >= 0; i--)
                    {
                        var skillshot = Evade.DetectedSkillshots[i];
                        if (skillshot.SpellData.ToggleParticleName != ""
                            && new Regex(skillshot.SpellData.ToggleParticleName).IsMatch(sender.Name))
                        {
                            Evade.DetectedSkillshots.RemoveAt(i);
                        }
                    }
                };
            GameObject.OnCreate += (sender, args) =>
                {
                    var shadow = sender as Obj_AI_Minion;
                    if (shadow != null && shadow.CharData.BaseSkinName == "zedshadow" && shadow.IsEnemy)
                    {
                        TrackObjects.Add(shadow);
                    }
                };
            Obj_AI_Base.OnPlayAnimation += (sender, args) =>
                {
                    if (sender.IsAlly || args.Animation != "Death" || TrackObjects.Count == 0)
                    {
                        return;
                    }
                    TrackObjects.ForEach(
                        i =>
                            {
                                if (i.Compare(sender))
                                {
                                    TrackObjects.Remove(i);
                                }
                            });
                };
            GameObject.OnDelete += (sender, args) =>
                {
                    if (sender.IsAlly || TrackObjects.Count == 0)
                    {
                        return;
                    }
                    TrackObjects.ForEach(
                        i =>
                            {
                                if (i.Compare(sender))
                                {
                                    TrackObjects.Remove(i);
                                }
                            });
                };
        }

        #endregion

        #region Delegates

        public delegate void OnDeleteSkillshotH(Skillshot skillshot, MissileClient missile);

        public delegate void OnDetectSkillshotH(Skillshot skillshot);

        #endregion

        #region Public Events

        public static event OnDeleteSkillshotH OnDeleteSkillshot;

        public static event OnDetectSkillshotH OnDetectSkillshot;

        #endregion

        #region Methods

        private static void MissileOnCreate(GameObject sender)
        {
            var missile = sender as MissileClient;
            if (missile == null || !missile.IsValid)
            {
                return;
            }
            var unit = missile.SpellCaster as AIHeroClient;
            if (unit == null || !unit.IsValid || unit.Team == Program.Player.Team)
            {
                return;
            }
            //Game.PrintChat("P: " + missile.SData.Name);
            var spellData = SpellDatabase.GetByMissileName(missile.SData.Name);
            if (spellData == null)
            {
                return;
            }
            var missilePosition = missile.Position.ToVector2();
            var unitPosition = missile.StartPosition.ToVector2();
            var endPos = missile.EndPosition.ToVector2();
            var direction = (endPos - unitPosition).LSNormalized();
            if (unitPosition.Distance(endPos) > spellData.Range || spellData.FixedRange)
            {
                endPos = unitPosition + direction * spellData.Range;
            }
            if (spellData.ExtraRange != -1)
            {
                endPos = endPos
                         + Math.Min(spellData.ExtraRange, spellData.Range - endPos.Distance(unitPosition)) * direction;
            }
            var castTime = Variables.TickCount - Game.Ping / 2 - (spellData.MissileDelayed ? 0 : spellData.Delay)
                           - (int)(1000f * missilePosition.Distance(unitPosition) / spellData.MissileSpeed);
            TriggerOnDetectSkillshot(
                DetectionType.RecvPacket,
                spellData,
                castTime,
                unitPosition,
                endPos,
                endPos,
                unit,
                missile);
        }

        private static void MissileOnDelete(GameObject sender, EventArgs args)
        {
            var missile = sender as MissileClient;
            if (missile == null || !missile.IsValid)
            {
                return;
            }
            var unit = missile.SpellCaster as AIHeroClient;
            if (unit == null || !unit.IsValid || unit.Team == Program.Player.Team)
            {
                return;
            }
            var spellName = missile.SData.Name;
            if (OnDeleteSkillshot != null)
            {
                foreach (var skillshot in
                    Evade.DetectedSkillshots.Where(
                        i =>
                        i.SpellData.MissileSpellName == spellName && i.Unit.NetworkId == unit.NetworkId
                        && (missile.EndPosition.ToVector2() - missile.StartPosition.ToVector2()).AngleBetween(
                            i.Direction) < 10 && i.SpellData.CanBeRemoved))
                {
                    OnDeleteSkillshot(skillshot, missile);
                    break;
                }
            }
            Evade.DetectedSkillshots.RemoveAll(
                i =>
                (i.SpellData.MissileSpellName == spellName || i.SpellData.ExtraMissileNames.Contains(spellName))
                && (i.Unit.NetworkId == unit.NetworkId
                    && (missile.EndPosition.ToVector2() - missile.StartPosition.ToVector2()).AngleBetween(i.Direction)
                    < 10 && i.SpellData.CanBeRemoved || i.SpellData.ForceRemove));
        }

        private static void OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            var unit = sender as AIHeroClient;
            if (unit == null || !unit.IsValid || unit.Team == Program.Player.Team)
            {
                return;
            }
            if (args.SData.Name == "dravenrdoublecast")
            {
                Evade.DetectedSkillshots.RemoveAll(
                    i => i.Unit.NetworkId == unit.NetworkId && i.SpellData.SpellName == "DravenRCast");
            }
            //Game.PrintChat("N: " + args.SData.Name);
            var spellData = SpellDatabase.GetByName(args.SData.Name);
            if (spellData == null)
            {
                return;
            }
            var startPos = new Vector2();
            if (spellData.FromObject != "")
            {
                GameObjects.EnemyMinions.Where(i => i.CharData.BaseSkinName == spellData.FromObject)
                    .ForEach(i => startPos = i.Position.ToVector2());
            }
            else
            {
                startPos = unit.ServerPosition.ToVector2();
            }
            if (spellData.FromObjects != null && spellData.FromObjects.Length > 0)
            {
                foreach (var obj in
                    TrackObjects.Where(i => spellData.FromObjects.Contains(i.CharData.BaseSkinName)))
                {
                    var start = obj.Position.ToVector2();
                    var end = start + spellData.Range * (args.End.ToVector2() - obj.Position.ToVector2()).LSNormalized();
                    TriggerOnDetectSkillshot(
                        DetectionType.ProcessSpell,
                        spellData,
                        Variables.TickCount - Game.Ping / 2,
                        start,
                        end,
                        end,
                        unit);
                }
            }
            if (!startPos.IsValid())
            {
                return;
            }
            var endPos = args.End.ToVector2();
            if (spellData.SpellName == "LucianQ" && args.Target != null
                && args.Target.NetworkId == Program.Player.NetworkId)
            {
                return;
            }
            var direction = (endPos - startPos).LSNormalized();
            if (startPos.Distance(endPos) > spellData.Range || spellData.FixedRange)
            {
                endPos = startPos + direction * spellData.Range;
            }
            if (spellData.ExtraRange != -1)
            {
                endPos = endPos
                         + Math.Min(spellData.ExtraRange, spellData.Range - endPos.Distance(startPos)) * direction;
            }
            TriggerOnDetectSkillshot(
                DetectionType.ProcessSpell,
                spellData,
                Variables.TickCount - Game.Ping / 2,
                startPos,
                endPos,
                args.End.ToVector2(),
                unit);
        }

        private static void TriggerOnDetectSkillshot(
            DetectionType detectionType,
            SpellData spellData,
            int startT,
            Vector2 start,
            Vector2 end,
            Vector2 originalEnd,
            Obj_AI_Base unit,
            MissileClient missile = null)
        {
            OnDetectSkillshot?.Invoke(new Skillshot(detectionType, spellData, startT, start, end, unit, missile) { OriginalEnd = originalEnd });
        }

        #endregion
    }
}