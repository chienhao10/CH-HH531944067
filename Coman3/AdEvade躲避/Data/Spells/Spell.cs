using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AdEvade.Config;
using AdEvade.Helpers;
using AdEvade.Utils;
using EloBuddy;
using EloBuddy.SDK;
using SharpDX;

namespace AdEvade.Data.Spells
{
    public class Spell
    {
        public float StartTime;
        public float EndTime;
        public Vector2 StartPos;
        public Vector2 EndPos;
        public Vector2 Direction;
        public float Height;
        public int HeroId;
        public int ProjectileId;
        public SpellData Info;
        public int SpellId;
        public GameObject SpellObject = null;
        public SpellType SpellType;

        public Vector2 CurrentSpellPosition = Vector2.Zero;
        public Vector2 CurrentNegativePosition = Vector2.Zero;
        public Vector2 PredictedEndPos = Vector2.Zero;

        public float Radius = 0;
        public SpellDangerLevel Dangerlevel = SpellDangerLevel.Low;

        public float EvadeTime = float.MinValue;
        public float SpellHitTime = float.MinValue;
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("Name: " + Info.SpellName + " : " + Info.CharName + " : " + Info.MissileName);
            sb.Append(" Danger Level: " + Dangerlevel);
            sb.Append(" Raduis: " + Info.Radius + "   Range: " + Info.Range + "   Is Fixed Range: " + Info.FixedRange);
            return sb.ToString();
        }
    }
    public static class SpellExtensions
    {
        public static float GetSpellRadius(this Spell spell)
        {
            var radius = Config.Properties.GetSpell(spell.Info.SpellName).Radius;
            var extraRadius = ConfigValue.ExtraSpellRadius.GetInt() ;
            if (spell.Info.HasEndExplosion && spell.SpellType == SpellType.Circular)
            {
                return spell.Info.SecondaryRadius + extraRadius;
            }

            if (spell.SpellType == SpellType.Arc)
            {
                var spellRange = spell.StartPos.Distance(spell.EndPos);
                var arcRadius = spell.Info.Radius * (1 + spellRange / 100) + extraRadius;

                return arcRadius;
            }

            return radius + extraRadius;
        }

        public static SpellDangerLevel GetSpellDangerLevel(this Spell spell)
        {
            return Config.Properties.GetSpell(spell.Info.SpellName).DangerLevel;
        }

        public static string GetSpellDangerString(this Spell spell)
        {
            return Enum.GetName(typeof(SpellDangerLevel), spell.GetSpellDangerLevel());
        }

        public static bool HasProjectile(this Spell spell)
        {
            // ReSharper disable once CompareOfFloatsByEqualityOperator
            return spell.Info.ProjectileSpeed > 0 && spell.Info.ProjectileSpeed != float.MaxValue;
        }

        public static Vector2 GetSpellProjection(this Spell spell, Vector2 pos, bool predictPos = false)
        {
            if (spell.SpellType == SpellType.Line
                || spell.SpellType == SpellType.Arc)
            {
                if (predictPos)
                {
                    var spellPos = spell.CurrentSpellPosition;
                    var spellEndPos = spell.GetSpellEndPosition();

                    return pos.ProjectOn(spellPos, spellEndPos).SegmentPoint;
                }
                return pos.ProjectOn(spell.StartPos, spell.EndPos).SegmentPoint;
            }

            if (spell.SpellType == SpellType.Circular)
            {
                return spell.EndPos;
            }

            return Vector2.Zero;
        }

        public static Obj_AI_Base CheckSpellCollision(this Spell spell)
        {
            if (spell.Info.CollisionObjects.Count() < 1)
            {
                return null;
            }

            List<Obj_AI_Base> collisionCandidates = new List<Obj_AI_Base>();
            var spellPos = spell.CurrentSpellPosition;
            var distanceToHero = spellPos.Distance(GameData.HeroInfo.ServerPos2D);

            if (spell.Info.CollisionObjects.Contains(CollisionObjectType.EnemyChampions))
            {
                foreach (var hero in EntityManager.Heroes.Allies
                    .Where(h => !h.IsMe && h.IsValidTarget(distanceToHero)))
                {
                    collisionCandidates.Add(hero);
                }
            }

            if (spell.Info.CollisionObjects.Contains(CollisionObjectType.EnemyMinions))
            {
                foreach (var minion in ObjectManager.Get<Obj_AI_Minion>()
                    .Where(h => h.Team == GameData.MyHero.Team && h.IsValidTarget()))
                {
                    if (!minion.IsMinion)
                    {
                        continue;
                    }

                    collisionCandidates.Add(minion);
                }
            }

            var sortedCandidates = collisionCandidates.OrderBy(h => h.Distance(spellPos));

            foreach (var candidate in sortedCandidates)
            {
                if (candidate.ServerPosition.To2D().InSkillShot(spell, candidate.BoundingRadius, false))
                {
                    return candidate;
                }
            }

            return null;
        }

        public static float GetSpellHitTime(this Spell spell, Vector2 pos)
        {

            if (spell.SpellType == SpellType.Line)
            {
                if (spell.Info.ProjectileSpeed == float.MaxValue)
                {
                    return Math.Max(0, spell.EndTime - EvadeUtils.TickCount - Game.Ping);
                }

                var spellPos = spell.GetCurrentSpellPosition(true, Game.Ping);
                return 1000 * spellPos.Distance(pos) / spell.Info.ProjectileSpeed;
            }
            else if (spell.SpellType == SpellType.Circular)
            {
                return Math.Max(0, spell.EndTime - EvadeUtils.TickCount - Game.Ping);
            }

            return float.MaxValue;
        }

        public static bool CanHeroEvade(this Spell spell, Obj_AI_Base hero, out float rEvadeTime, out float rSpellHitTime)
        {
            var heroPos = hero.ServerPosition.To2D();
            float evadeTime = 0;
            float spellHitTime = 0;

            if (spell.SpellType == SpellType.Line)
            {
                var projection = heroPos.ProjectOn(spell.StartPos, spell.EndPos).SegmentPoint;
                evadeTime = 1000 * (spell.Radius - heroPos.Distance(projection) + hero.BoundingRadius) / hero.MoveSpeed;
                spellHitTime = spell.GetSpellHitTime(projection);
            }
            else if (spell.SpellType == SpellType.Circular)
            {
                evadeTime = 1000 * (spell.Radius - heroPos.Distance(spell.EndPos)) / hero.MoveSpeed;
                spellHitTime = spell.GetSpellHitTime(heroPos);
            }

            rEvadeTime = evadeTime;
            rSpellHitTime = spellHitTime;

            return spellHitTime > evadeTime;
        }

        public static BoundingBox GetLinearSpellBoundingBox(this Spell spell)
        {
            var myBoundingRadius = GameData.HeroInfo.BoundingRadius;
            var spellDir = spell.Direction;
            var pSpellDir = spell.Direction.Perpendicular();
            var spellRadius = spell.Radius;
            var spellPos = spell.CurrentSpellPosition - spellDir * myBoundingRadius; //leave some space at back of spell
            var endPos = spell.GetSpellEndPosition() + spellDir * myBoundingRadius; //leave some space at the front of spell

            var startRightPos = spellPos + pSpellDir * (spellRadius + myBoundingRadius);
            var endLeftPos = endPos - pSpellDir * (spellRadius + myBoundingRadius);


            return new BoundingBox(new Vector3(endLeftPos.X, endLeftPos.Y, -1), new Vector3(startRightPos.X, startRightPos.Y, 1));
        }

        public static Vector2 GetSpellEndPosition(this Spell spell)
        {
            return spell.PredictedEndPos == Vector2.Zero ? spell.EndPos : spell.PredictedEndPos;
        }

        public static void UpdateSpellInfo(this Spell spell)
        {
            spell.CurrentSpellPosition = spell.GetCurrentSpellPosition();
            spell.CurrentNegativePosition = spell.GetCurrentSpellPosition(true, 0);

            spell.Dangerlevel = spell.GetSpellDangerLevel();
            //spell.radius = spell.GetSpellRadius();
        }

        public static Vector2 GetCurrentSpellPosition(this Spell spell, bool allowNegative = false, float delay = 0,
            float extraDistance = 0)
        {
            Vector2 spellPos = spell.StartPos;

            if (spell.SpellType == SpellType.Line
                || spell.SpellType == SpellType.Arc)
            {
                float spellTime = EvadeUtils.TickCount - spell.StartTime - spell.Info.SpellDelay;

                if (spell.Info.ProjectileSpeed == float.MaxValue)
                    return spell.StartPos;

                if (spellTime >= 0 || allowNegative)
                {
                    spellPos = spell.StartPos + spell.Direction * spell.Info.ProjectileSpeed * (spellTime / 1000);
                }
            }
            else if (spell.SpellType == SpellType.Circular)
            {
                spellPos = spell.EndPos;
            }

            if (spell.SpellObject != null && spell.SpellObject.IsValid && spell.SpellObject.IsVisible &&
                spell.SpellObject.Position.To2D().Distance(GameData.HeroInfo.ServerPos2D) < spell.Info.Range + 1000)
            {
                spellPos = spell.SpellObject.Position.To2D();
            }

            if (delay > 0 && spell.Info.ProjectileSpeed != float.MaxValue
                          && spell.SpellType == SpellType.Line)
            {
                spellPos = spellPos + spell.Direction * spell.Info.ProjectileSpeed * (delay / 1000);
            }

            if (extraDistance > 0 && spell.Info.ProjectileSpeed != float.MaxValue
                          && spell.SpellType == SpellType.Line)
            {
                spellPos = spellPos + spell.Direction * extraDistance;
            }

            return spellPos;
        }

        public static bool LineIntersectLinearSpell(this Spell spell, Vector2 a, Vector2 b)
        {
            var myBoundingRadius = ObjectManager.Player.BoundingRadius;
            var spellDir = spell.Direction;
            var pSpellDir = spell.Direction.Perpendicular();
            var spellRadius = spell.Radius;
            var spellPos = spell.CurrentSpellPosition;// -spellDir * myBoundingRadius; //leave some space at back of spell
            var endPos = spell.GetSpellEndPosition();// +spellDir * myBoundingRadius; //leave some space at the front of spell

            var startRightPos = spellPos + pSpellDir * (spellRadius + myBoundingRadius);
            var startLeftPos = spellPos - pSpellDir * (spellRadius + myBoundingRadius);
            var endRightPos = endPos + pSpellDir * (spellRadius + myBoundingRadius);
            var endLeftPos = endPos - pSpellDir * (spellRadius + myBoundingRadius);

            bool int1 = MathUtils.CheckLineIntersection(a, b, startRightPos, startLeftPos);
            bool int2 = MathUtils.CheckLineIntersection(a, b, endRightPos, endLeftPos);
            bool int3 = MathUtils.CheckLineIntersection(a, b, startRightPos, endRightPos);
            bool int4 = MathUtils.CheckLineIntersection(a, b, startLeftPos, endLeftPos);

            if (int1 || int2 || int3 || int4)
            {
                return true;
            }

            return false;
        }

        public static bool LineIntersectLinearSpellEx(this Spell spell, Vector2 a, Vector2 b, out Vector2 intersection) //edited
        {
            var myBoundingRadius = ObjectManager.Player.BoundingRadius;
            var spellDir = spell.Direction;
            var pSpellDir = spell.Direction.Perpendicular();
            var spellRadius = spell.Radius;
            var spellPos = spell.CurrentSpellPosition - spellDir * myBoundingRadius; //leave some space at back of spell
            var endPos = spell.GetSpellEndPosition() + spellDir * myBoundingRadius; //leave some space at the front of spell

            var startRightPos = spellPos + pSpellDir * (spellRadius + myBoundingRadius);
            var startLeftPos = spellPos - pSpellDir * (spellRadius + myBoundingRadius);
            var endRightPos = endPos + pSpellDir * (spellRadius + myBoundingRadius);
            var endLeftPos = endPos - pSpellDir * (spellRadius + myBoundingRadius);

            List<Geometry.IntersectionResult> intersects = new List<Geometry.IntersectionResult>();
            Vector2 heroPos = ObjectManager.Player.ServerPosition.To2D();

            intersects.Add(a.Intersection(b, startRightPos, startLeftPos));
            intersects.Add(a.Intersection(b, endRightPos, endLeftPos));
            intersects.Add(a.Intersection(b, startRightPos, endRightPos));
            intersects.Add(a.Intersection(b, startLeftPos, endLeftPos));

            var sortedIntersects = intersects.Where(i => i.Intersects).OrderBy(i => i.Point.Distance(heroPos)); //Get first intersection

            if (sortedIntersects.Count() > 0)
            {
                intersection = sortedIntersects.First().Point;
                return true;
            }

            intersection = Vector2.Zero;
            return false;
        }

    }
}