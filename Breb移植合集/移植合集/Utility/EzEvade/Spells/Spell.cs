using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu.Values;
using SharpDX;
using LeagueSharp.Common;

namespace ezEvade
{
    public class Spell
    {
        public float startTime;
        public float endTime;
        public Vector2 startPos;
        public Vector2 endPos;
        public Vector2 direction;
        public float height;
        public int heroID;
        public int projectileID;
        public SpellData info;
        public int spellID;
        public GameObject spellObject = null;
        public SpellType spellType;

        public Vector2 currentSpellPosition = Vector2.Zero;
        public Vector2 currentNegativePosition = Vector2.Zero;
        public Vector2 predictedEndPos = Vector2.Zero;

        public float radius = 0;
        public int dangerlevel = 1;

        public float evadeTime = float.MinValue;
        public float spellHitTime = float.MinValue;

        public Spell()
        {

        }
    }

    public static class SpellExtensions
    {
        public static float GetSpellRadius(this Spell spell)
        {
            var radius = ObjectCache.menuCache.cache[spell.info.spellName + "SpellRadius"].Cast<Slider>().CurrentValue;
            var extraRadius = ObjectCache.menuCache.cache["ExtraSpellRadius"].Cast<Slider>().CurrentValue;

            if (spell.info.hasEndExplosion && spell.spellType == SpellType.Circular)
            {
                return spell.info.secondaryRadius + extraRadius;
            }

            if (spell.spellType == SpellType.Arc)
            {
                var spellRange = spell.startPos.LSDistance(spell.endPos);
                var arcRadius = spell.info.radius * (1 + spellRange/100) + extraRadius;
                                
                return arcRadius;
            }

            return (float)(radius + extraRadius);
        }

        public static int GetSpellDangerLevel(this Spell spell)
        {
            var dangerStr = ObjectCache.menuCache.cache[spell.info.spellName + "DangerLevel"].Cast<Slider>().DisplayName;

            var dangerlevel = 1;

            switch (dangerStr)
            {
                case "Low":
                    dangerlevel = 1;
                    break;
                case "High":
                    dangerlevel = 3;
                    break;
                case "Extreme":
                    dangerlevel = 4;
                    break;
                default:
                    dangerlevel = 2;
                    break;
            }

            return dangerlevel;
        }

        public static string GetSpellDangerString(this Spell spell)
        {
            switch (spell.GetSpellDangerLevel())
            {
                case 1:
                    return "Low";
                case 3:
                    return "High";
                case 4:
                    return "Extreme";
                default:
                    return "Normal";
            }
        }

        public static bool hasProjectile(this Spell spell)
        {
            return spell.info.projectileSpeed > 0 && spell.info.projectileSpeed != float.MaxValue;
        }

        public static Vector2 GetSpellProjection(this Spell spell, Vector2 pos, bool predictPos = false)
        {
            if (spell.spellType == SpellType.Line
                || spell.spellType == SpellType.Arc)
            {
                if (predictPos)
                {
                    var spellPos = spell.currentSpellPosition;
                    var spellEndPos = spell.GetSpellEndPosition();

                    return pos.LSProjectOn(spellPos, spellEndPos).SegmentPoint;
                }
                else
                {
                    return pos.LSProjectOn(spell.startPos, spell.endPos).SegmentPoint;
                }
            }
            else if (spell.spellType == SpellType.Circular)
            {
                return spell.endPos;
            }

            return Vector2.Zero;
        }

        public static Obj_AI_Base CheckSpellCollision(this Spell spell)
        {
            if (spell.info.collisionObjects.Count() < 1)
            {
                return null;
            }

            List<Obj_AI_Base> collisionCandidates = new List<Obj_AI_Base>();
            var spellPos = spell.currentSpellPosition;
            var distanceToHero = spellPos.LSDistance(ObjectCache.myHeroCache.serverPos2D);

            if (spell.info.collisionObjects.Contains(CollisionObjectType.EnemyChampions))
            {
                collisionCandidates.AddRange(EntityManager.Heroes.Allies.Where(h => !h.IsMe && h.LSIsValidTarget(distanceToHero)).Cast<Obj_AI_Base>());
            }

            if (spell.info.collisionObjects.Contains(CollisionObjectType.EnemyMinions))
            {
                collisionCandidates.AddRange(ObjectManager.Get<Obj_AI_Minion>().Where(h => h.Team == Evade.myHero.Team && h.LSIsValidTarget()).Where(minion => minion.CharData.BaseSkinName.ToLower() != "teemomushroom" && minion.CharData.BaseSkinName.ToLower() != "shacobox").Cast<Obj_AI_Base>());
            }

            var sortedCandidates = collisionCandidates.OrderBy(h => h.LSDistance(spellPos));

            return sortedCandidates.FirstOrDefault(candidate => candidate.ServerPosition.LSTo2D().InSkillShot(spell, candidate.BoundingRadius, false));
        }

        public static float GetSpellHitTime(this Spell spell, Vector2 pos)
        {

            if (spell.spellType == SpellType.Line)
            {
                if (spell.info.projectileSpeed == float.MaxValue)
                {
                    return Math.Max(0, spell.endTime - EvadeUtils.TickCount - ObjectCache.gamePing);
                }

                var spellPos = spell.GetCurrentSpellPosition(true, ObjectCache.gamePing);
                return 1000 * spellPos.LSDistance(pos) / spell.info.projectileSpeed;
            }
            else if (spell.spellType == SpellType.Circular)
            {
                return Math.Max(0, spell.endTime - EvadeUtils.TickCount - ObjectCache.gamePing);
            }

            return float.MaxValue;
        }

        public static bool CanHeroEvade(this Spell spell, Obj_AI_Base hero, out float rEvadeTime, out float rSpellHitTime)
        {
            var heroPos = hero.ServerPosition.LSTo2D();
            float evadeTime = 0;
            float spellHitTime = 0;

            if (spell.spellType == SpellType.Line)
            {
                var projection = heroPos.LSProjectOn(spell.startPos, spell.endPos).SegmentPoint;
                evadeTime = 1000 * (spell.radius - heroPos.LSDistance(projection) + hero.BoundingRadius) / hero.MoveSpeed;
                spellHitTime = spell.GetSpellHitTime(projection);
            }
            else if (spell.spellType == SpellType.Circular)
            {
                evadeTime = 1000 * (spell.radius - heroPos.LSDistance(spell.endPos)) / hero.MoveSpeed;
                spellHitTime = spell.GetSpellHitTime(heroPos);
            }

            rEvadeTime = evadeTime;
            rSpellHitTime = spellHitTime;

            return spellHitTime > evadeTime;
        }

        public static BoundingBox GetLinearSpellBoundingBox(this Spell spell)
        {
            var myBoundingRadius = ObjectCache.myHeroCache.boundingRadius;
            var spellDir = spell.direction;
            var pSpellDir = spell.direction.LSPerpendicular();
            var spellRadius = spell.radius;
            var spellPos = spell.currentSpellPosition - spellDir * myBoundingRadius; //leave some space at back of spell
            var endPos = spell.GetSpellEndPosition() + spellDir * myBoundingRadius; //leave some space at the front of spell

            var startRightPos = spellPos + pSpellDir * (spellRadius + myBoundingRadius);
            var endLeftPos = endPos - pSpellDir * (spellRadius + myBoundingRadius);


            return new BoundingBox(new Vector3(endLeftPos.X, endLeftPos.Y, -1), new Vector3(startRightPos.X, startRightPos.Y, 1));
        }

        public static Vector2 GetSpellEndPosition(this Spell spell)
        {
            return spell.predictedEndPos == Vector2.Zero ? spell.endPos : spell.predictedEndPos;
        }

        public static void UpdateSpellInfo(this Spell spell)
        {
            spell.currentSpellPosition = spell.GetCurrentSpellPosition();
            spell.currentNegativePosition = spell.GetCurrentSpellPosition(true);

            spell.dangerlevel = spell.GetSpellDangerLevel();

            if (spell.info.name == "TaricE")
            {
                var taric = HeroManager.Enemies.FirstOrDefault(x => x.ChampionName == "Taric");
                if (taric != null)
                {
                    spell.currentSpellPosition = taric.ServerPosition.LSTo2D();
                    spell.endPos = taric.ServerPosition.LSTo2D() + spell.direction * spell.info.range;
                }
            }

            if (spell.info.name == "TaliyahQ")
            {
                var taliyah = HeroManager.Enemies.FirstOrDefault(x => x.ChampionName == "Taliyah");
                if (taliyah != null)
                {
                    spell.currentSpellPosition = taliyah.ServerPosition.LSTo2D();
                    spell.endPos = taliyah.ServerPosition.LSTo2D() + spell.direction * spell.info.range;
                }
            }

            if (spell.info.name == "TaricE2")
            {
                var partner = HeroManager.Enemies.FirstOrDefault(x => x.HasBuff("taricwleashactive") && x.ChampionName != "Taric");
                if (partner != null)
                {
                    spell.currentSpellPosition = partner.ServerPosition.LSTo2D();
                    spell.endPos = partner.ServerPosition.LSTo2D() + spell.direction * spell.info.range;
                }
            }
        }

        public static Vector2 GetCurrentSpellPosition(this Spell spell, bool allowNegative = false, float delay = 0, 
            float extraDistance = 0)
        {
            Vector2 spellPos = spell.startPos;

            if (spell.spellType == SpellType.Line
                || spell.spellType == SpellType.Arc)
            {
                float spellTime = EvadeUtils.TickCount - spell.startTime - spell.info.spellDelay;

                if (spell.info.projectileSpeed == float.MaxValue)
                    return spell.startPos;

                if (spellTime >= 0 || allowNegative)
                {
                    spellPos = spell.startPos + spell.direction * spell.info.projectileSpeed * (spellTime / 1000);
                }
            }
            else if (spell.spellType == SpellType.Circular)
            {
                spellPos = spell.endPos;
            }

            if (spell.spellObject != null && spell.spellObject.IsValid && spell.spellObject.IsVisible &&
                spell.spellObject.Position.LSTo2D().LSDistance(ObjectCache.myHeroCache.serverPos2D) < spell.info.range + 1000)
            {
                spellPos = spell.spellObject.Position.LSTo2D();
            }

            if (delay > 0 && spell.info.projectileSpeed != float.MaxValue
                          && spell.spellType == SpellType.Line)
            {
                spellPos = spellPos + spell.direction * spell.info.projectileSpeed * (delay / 1000);
            }

            if (extraDistance > 0 && spell.info.projectileSpeed != float.MaxValue
                          && spell.spellType == SpellType.Line)
            {
                spellPos = spellPos + spell.direction * extraDistance;
            }

            return spellPos;
        }

        public static bool LineIntersectLinearSpell(this Spell spell, Vector2 a, Vector2 b)
        {
            var myBoundingRadius = ObjectManager.Player.BoundingRadius;
            var spellDir = spell.direction;
            var pSpellDir = spell.direction.LSPerpendicular();
            var spellRadius = spell.radius;
            var spellPos = spell.currentSpellPosition;// -spellDir * myBoundingRadius; //leave some space at back of spell
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
            var spellDir = spell.direction;
            var pSpellDir = spell.direction.LSPerpendicular();
            var spellRadius = spell.radius;
            var spellPos = spell.currentSpellPosition - spellDir * myBoundingRadius; //leave some space at back of spell
            var endPos = spell.GetSpellEndPosition() + spellDir * myBoundingRadius; //leave some space at the front of spell

            var startRightPos = spellPos + pSpellDir * (spellRadius + myBoundingRadius);
            var startLeftPos = spellPos - pSpellDir * (spellRadius + myBoundingRadius);
            var endRightPos = endPos + pSpellDir * (spellRadius + myBoundingRadius);
            var endLeftPos = endPos - pSpellDir * (spellRadius + myBoundingRadius);

            List<LeagueSharp.Common.Geometry.IntersectionResult> intersects = new List<LeagueSharp.Common.Geometry.IntersectionResult>();
            Vector2 heroPos = ObjectManager.Player.ServerPosition.LSTo2D();

            intersects.Add(a.LSIntersection(b, startRightPos, startLeftPos));
            intersects.Add(a.LSIntersection(b, endRightPos, endLeftPos));
            intersects.Add(a.LSIntersection(b, startRightPos, endRightPos));
            intersects.Add(a.LSIntersection(b, startLeftPos, endLeftPos));

            var sortedIntersects = intersects.Where(i => i.Intersects).OrderBy(i => i.Point.LSDistance(heroPos)); //Get first intersection

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
