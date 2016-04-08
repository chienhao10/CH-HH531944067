using System;
using System.Collections.Generic;
using AdEvade.Config;
using AdEvade.Helpers;
using AdEvade.Utils;
using EloBuddy;
using EloBuddy.SDK;
using SharpDX;
using Spell = AdEvade.Data.Spells.Spell;

namespace AdEvade.Data
{
    public class PositionInfo
    {
        private static AIHeroClient MyHero { get { return ObjectManager.Player; } }

        public int PosDangerLevel = 0;
        public int PosDangerCount = 0;
        public bool IsDangerousPos = false;
        public float DistanceToMouse = 0;
        public List<int> DodgeableSpells = new List<int>();
        public List<int> UndodgeableSpells = new List<int>();
        public List<int> SpellList = new List<int>();
        public Vector2 Position;
        public float Timestamp;
        public float EndTime = 0;
        public bool HasExtraDistance = false;
        public float ClosestDistance = float.MaxValue;
        public float IntersectionTime = float.MaxValue;
        public bool RejectPosition = false;
        public float PosDistToChamps = float.MaxValue;
        public bool HasComfortZone = true;
        public Obj_AI_Base Target = null;
        public bool RecalculatedPath = false;
        public float Speed = 0;

        public PositionInfo(
            Vector2 position,
            int posDangerLevel,
            int posDangerCount,
            bool isDangerousPos,
            float distanceToMouse,
            List<int> dodgeableSpells,
            List<int> undodgeableSpells)
        {
            Position = position;
            PosDangerLevel = posDangerLevel;
            PosDangerCount = posDangerCount;
            IsDangerousPos = isDangerousPos;
            DistanceToMouse = distanceToMouse;
            DodgeableSpells = dodgeableSpells;
            UndodgeableSpells = undodgeableSpells;
            Timestamp = EvadeUtils.TickCount;
        }

        public PositionInfo(
            Vector2 position,
            bool isDangerousPos,
            float distanceToMouse)
        {
            Position = position;
            IsDangerousPos = isDangerousPos;
            DistanceToMouse = distanceToMouse;
            Timestamp = EvadeUtils.TickCount;
        }

        public static PositionInfo SetAllDodgeable()
        {
            return SetAllDodgeable(MyHero.Position.To2D());
        }

        public static PositionInfo SetAllDodgeable(Vector2 position)
        {
            List<int> dodgeableSpells = new List<int>();
            List<int> undodgeableSpells = new List<int>();

            foreach (KeyValuePair<int, Spell> entry in SpellDetector.Spells)
            {
                Spell spell = entry.Value;
                dodgeableSpells.Add(entry.Key);
            }

            return new PositionInfo(
                position,
                0,
                0,
                true,
                0,
                dodgeableSpells,
                undodgeableSpells);
        }

        public static PositionInfo SetAllUndodgeable()
        {
            ConsoleDebug.WriteLineColor("Setting all Undodgeable", ConsoleColor.Red);
            List <int> dodgeableSpells = new List<int>();
            List<int> undodgeableSpells = new List<int>();

            var posDangerLevel = 0;
            var posDangerCount = 0;

            foreach (KeyValuePair<int, Spell> entry in SpellDetector.Spells)
            {
                Spell spell = entry.Value;
                undodgeableSpells.Add(entry.Key);

                var spellDangerLevel = spell.Dangerlevel;

                posDangerLevel = Math.Max(posDangerLevel, (int) spellDangerLevel);
                posDangerCount += (int)spellDangerLevel;
            }

            return new PositionInfo(
                MyHero.Position.To2D(),
                posDangerLevel,
                posDangerCount,
                true,
                0,
                dodgeableSpells,
                undodgeableSpells);
        }
    }

    public static class PositionInfoExtensions
    {
        public static AIHeroClient MyHero { get { return ObjectManager.Player; } }

        public static int GetHighestSpellId(this PositionInfo posInfo)
        {
            if (posInfo == null)
                return 0;

            int highest = 0;

            foreach (var spellId in posInfo.UndodgeableSpells)
            {
                highest = Math.Max(highest, spellId);
            }

            foreach (var spellId in posInfo.DodgeableSpells)
            {
                highest = Math.Max(highest, spellId);
            }

            return highest;
        }

        public static bool IsSamePosInfo(this PositionInfo posInfo1, PositionInfo posInfo2)
        {
            return new HashSet<int>(posInfo1.SpellList).SetEquals(posInfo2.SpellList);
        }

        public static bool IsBetterMovePos(this PositionInfo newPosInfo)
        {
            PositionInfo posInfo = null;
            var path = MyHero.Path;
            if (path.Length > 0)
            {
                var movePos = path[path.Length - 1].To2D();
                posInfo = EvadeHelper.CanHeroWalkToPos(movePos, GameData.HeroInfo.MoveSpeed, 0, 0, false);
            }
            else
            {
                posInfo = EvadeHelper.CanHeroWalkToPos(GameData.HeroInfo.ServerPos2D, GameData.HeroInfo.MoveSpeed, 0, 0, false);
            }

            if (posInfo.PosDangerCount < newPosInfo.PosDangerCount)
            {
                return false;
            }

            return true;
        }

        public static PositionInfo CompareLastMovePos(this PositionInfo newPosInfo)
        {
            PositionInfo posInfo = null;
            var path = MyHero.Path;
            if (path.Length > 0)
            {
                var movePos = path[path.Length - 1].To2D();
                posInfo = EvadeHelper.CanHeroWalkToPos(movePos, GameData.HeroInfo.MoveSpeed, 0, 0, false);
            }
            else
            {
                posInfo = EvadeHelper.CanHeroWalkToPos(GameData.HeroInfo.ServerPos2D, GameData.HeroInfo.MoveSpeed, 0, 0, false);
            }

            if (posInfo.PosDangerCount < newPosInfo.PosDangerCount)
            {
                return posInfo;
            }

            return newPosInfo;
        }
    }
}