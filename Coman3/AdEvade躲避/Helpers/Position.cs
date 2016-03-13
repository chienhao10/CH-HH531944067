using System;
using System.Collections.Generic;
using AdEvade.Config;
using AdEvade.Data;
using AdEvade.Data.Spells;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu.Values;
using SharpDX;
using Spell = AdEvade.Data.Spells.Spell;

namespace AdEvade.Helpers
{
    public static class Position
    {
        private static AIHeroClient MyHero { get { return ObjectManager.Player; } }

        public static int CheckPosDangerLevel(this Vector2 pos, float extraBuffer)
        {
            var dangerlevel = 0;
            foreach (KeyValuePair<int, Spell> entry in SpellDetector.Spells)
            {
                Spell spell = entry.Value;

                if (pos.InSkillShot(spell, GameData.HeroInfo.BoundingRadius + extraBuffer))
                {
                    dangerlevel += (int) spell.Dangerlevel;
                }
            }
            return dangerlevel;
        }

        public static bool InSkillShot(this Vector2 position, Spell spell, float radius, bool predictCollision = true)
        {
            if (spell.SpellType == SpellType.Line)
            {
                Vector2 spellPos = spell.CurrentSpellPosition;
                Vector2 spellEndPos = predictCollision ? spell.GetSpellEndPosition() : spell.EndPos;

                //spellPos = spellPos - spell.direction * radius; //leave some space at back of spell
                //spellEndPos = spellEndPos + spell.direction * radius; //leave some space at the front of spell

                /*if (spell.info.projectileSpeed == float.MaxValue
                    && Evade.GetTickCount - spell.startTime > spell.info.spellDelay)
                {
                    return false;
                }*/

                var projection = position.ProjectOn(spellPos, spellEndPos);

                /*if (projection.SegmentPoint.Distance(spellEndPos) < 100) //Check Skillshot endpoints
                {
                    //unfinished
                }*/

                return projection.IsOnSegment && projection.SegmentPoint.Distance(position) <= spell.Radius + radius;
            }
            else if (spell.SpellType == SpellType.Circular)
            {
                if (spell.Info.SpellName == "VeigarEventHorizon")
                {
                    return position.Distance(spell.EndPos) <= spell.Radius + radius - GameData.HeroInfo.BoundingRadius
                        && position.Distance(spell.EndPos) >= spell.Radius + radius - GameData.HeroInfo.BoundingRadius - 125;
                }

                return position.Distance(spell.EndPos) <= spell.Radius + radius - GameData.HeroInfo.BoundingRadius;
            }
            else if (spell.SpellType == SpellType.Arc)
            {
                if (position.IsLeftOfLineSegment(spell.StartPos, spell.EndPos))
                {
                    return false;
                }

                var spellRange = spell.StartPos.Distance(spell.EndPos);
                var midPoint = spell.StartPos + spell.Direction * (spellRange/2);

                return position.Distance(midPoint) <= spell.Radius + radius - GameData.HeroInfo.BoundingRadius;
            }
            else if (spell.SpellType == SpellType.Cone)
            {

            }
            return false;
        }

        public static bool IsLeftOfLineSegment(this Vector2 pos, Vector2 start, Vector2 end)
        {
            return ((end.X - start.X) * (pos.Y - start.Y) - (end.Y - start.Y) * (pos.X - start.X)) > 0;
        }

        public static float GetDistanceToTurrets(this Vector2 pos)
        {
            float minDist = float.MaxValue;

            foreach (var entry in GameData.Turrets)
            {
                var turret = entry.Value;
                if (turret == null || !turret.IsValid || turret.IsDead)
                {
                    Core.DelayAction(() => GameData.Turrets.Remove(entry.Key), 1);
                    continue;
                }

                if (turret.IsAlly)
                {
                    continue;
                }

                var distToTurret = pos.Distance(turret.Position.To2D());

                minDist = Math.Min(minDist, distToTurret);
            }

            return minDist;
        }

        public static float GetDistanceToChampions(this Vector2 pos)
        {
            float minDist = float.MaxValue;

            foreach (var hero in EntityManager.Heroes.Enemies)
            {
                if (hero != null && hero.IsValid && !hero.IsDead && hero.IsVisible)
                {
                    var heroPos = hero.ServerPosition.To2D();
                    var dist = heroPos.Distance(pos);

                    minDist = Math.Min(minDist, dist);
                }
            }

            return minDist;
        }

        public static bool HasExtraAvoidDistance(this Vector2 pos, float extraBuffer)
        {
            foreach (KeyValuePair<int, Spell> entry in SpellDetector.Spells)
            {
                Spell spell = entry.Value;

                if (spell.SpellType == SpellType.Line)
                {
                    if (pos.InSkillShot(spell, GameData.HeroInfo.BoundingRadius + extraBuffer))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public static float GetPositionValue(this Vector2 pos)
        {
            float posValue = pos.Distance(Game.CursorPos.To2D());

            if (Config.Properties.GetBool(ConfigValue.PreventDodgingNearEnemy))
            {
                var minComfortDistance = ConfigValue.MinimumComfortZone.GetInt();
                var distanceToChampions = pos.GetDistanceToChampions();

                if (minComfortDistance > distanceToChampions)
                {
                    posValue += 2 * (minComfortDistance - distanceToChampions);
                }
            }

            if (Config.Properties.GetBool(ConfigValue.PreventDodgingUnderTower))
            {
                var turretRange = 875 + GameData.HeroInfo.BoundingRadius;
                var distanceToTurrets = pos.GetDistanceToTurrets();

                if (turretRange > distanceToTurrets)
                {
                    posValue += 5 * (turretRange - distanceToTurrets);
                }
            }

            return posValue;
        }

        public static bool CheckDangerousPos(this Vector2 pos, float extraBuffer, bool checkOnlyDangerous = false)
        {
            foreach (KeyValuePair<int, Spell> entry in SpellDetector.Spells)
            {
                Spell spell = entry.Value;

                if (checkOnlyDangerous && (int) spell.Dangerlevel < (int)SpellDangerLevel.High)
                {
                    continue;
                }

                if (pos.InSkillShot(spell, GameData.HeroInfo.BoundingRadius + extraBuffer))
                {
                    return true;
                }
            }
            return false;
        }

        public static List<Vector2> GetSurroundingPositions(int maxPosToCheck = 150, int posRadius = 25)
        {
            List<Vector2> positions = new List<Vector2>();

            int posChecked = 0;
            int radiusIndex = 0;

            Vector2 heroPoint = GameData.HeroInfo.ServerPos2D;
            Vector2 lastMovePos = Game.CursorPos.To2D();

            List<PositionInfo> posTable = new List<PositionInfo>();

            while (posChecked < maxPosToCheck)
            {
                radiusIndex++;

                int curRadius = radiusIndex * (2 * posRadius);
                int curCircleChecks = (int)Math.Ceiling((2 * Math.PI * (double)curRadius) / (2 * (double)posRadius));

                for (int i = 1; i < curCircleChecks; i++)
                {
                    posChecked++;
                    var cRadians = (2 * Math.PI / (curCircleChecks - 1)) * i; //check decimals
                    var pos = new Vector2((float)Math.Floor(heroPoint.X + curRadius * Math.Cos(cRadians)),
                                          (float)Math.Floor(heroPoint.Y + curRadius * Math.Sin(cRadians)));

                    positions.Add(pos);
                }
            }

            return positions;
        }
    }
}
