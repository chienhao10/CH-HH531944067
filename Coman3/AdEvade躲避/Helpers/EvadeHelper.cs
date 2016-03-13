using System;
using System.Collections.Generic;
using System.Linq;
using AdEvade.Config;
using AdEvade.Data;
using AdEvade.Data.EvadeSpells;
using AdEvade.Data.Spells;
using AdEvade.Utils;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu.Values;
using SharpDX;
using Spell = AdEvade.Data.Spells.Spell;

namespace AdEvade.Helpers
{
    static class EvadeHelper
    {
        private static AIHeroClient MyHero { get { return ObjectManager.Player; } }

        public static bool PlayerInSkillShot(Spell spell)
        {
            return GameData.HeroInfo.ServerPos2D.InSkillShot(spell, GameData.HeroInfo.BoundingRadius);
        }

        public static PositionInfo InitPositionInfo(Vector2 pos, float extraDelayBuffer, float extraEvadeDistance, Vector2 lastMovePos, Spell lowestEvadeTimeSpell) //clean this shit up
        {
            if (!GameData.HeroInfo.IsMoving &&
                 GameData.HeroInfo.ServerPos2D.Distance(pos) <= 75)
            {
                pos = GameData.HeroInfo.ServerPos2D;
            }

            var extraDist = ConfigValue.ExtraCpaDistance.GetInt();

            var posInfo = CanHeroWalkToPos(pos, GameData.HeroInfo.MoveSpeed, extraDelayBuffer + Game.Ping, extraDist);
            posInfo.IsDangerousPos = pos.CheckDangerousPos(6);
            posInfo.HasExtraDistance = extraEvadeDistance > 0 ? pos.CheckDangerousPos(extraEvadeDistance) : false;// ? 1 : 0;            
            posInfo.ClosestDistance = posInfo.DistanceToMouse; //GetMovementBlockPositionValue(pos, lastMovePos);
            posInfo.IntersectionTime = GetMinCpaDistance(pos);
            //GetIntersectDistance(lowestEvadeTimeSpell, GameData.HeroInfo.serverPos2D, pos);               
            //GetClosestDistanceApproach(lowestEvadeTimeSpell, pos, GameData.HeroInfo.moveSpeed, Game.Ping, GameData.HeroInfo.serverPos2D, 0);
            posInfo.DistanceToMouse = pos.GetPositionValue();
            posInfo.PosDistToChamps = pos.GetDistanceToChampions();
            posInfo.Speed = GameData.HeroInfo.MoveSpeed;

            if (ConfigValue.RejectMinDistance.GetInt() > 0
                && ConfigValue.RejectMinDistance.GetInt() >
                posInfo.ClosestDistance) //reject closestdistance
            {
                posInfo.RejectPosition = true;
            }

            if (ConfigValue.MinimumComfortZone.GetInt() >
                posInfo.PosDistToChamps)
            {
                posInfo.HasComfortZone = false;
            }

            return posInfo;
        }

        public static IOrderedEnumerable<PositionInfo> GetBestPositionTest()
        {
            int posChecked = 0;
            int maxPosToCheck = 50;
            int posRadius = 50;
            int radiusIndex = 0;

            Vector2 heroPoint = GameData.HeroInfo.ServerPos2D;
            Vector2 lastMovePos = Game.CursorPos.To2D();

            var extraDelayBuffer = Config.Properties.GetInt(ConfigValue.ExtraPingBuffer);
            var extraEvadeDistance = ConfigValue.ExtraEvadeDistance.GetInt();

            if (ConfigValue.HighPrecision.GetBool())
            {
                maxPosToCheck = 150;
                posRadius = 25;
            }

            List<PositionInfo> posTable = new List<PositionInfo>();

            List<Vector2> fastestPositions = GetFastestPositions();

            Spell lowestEvadeTimeSpell;
            var lowestEvadeTime = SpellDetector.GetLowestEvadeTime(out lowestEvadeTimeSpell);

            foreach (var pos in fastestPositions) //add the fastest positions into list of candidates
            {
                posTable.Add(InitPositionInfo(pos, extraDelayBuffer, extraEvadeDistance, lastMovePos, lowestEvadeTimeSpell));
            }

            while (posChecked < maxPosToCheck)
            {
                radiusIndex++;

                int curRadius = radiusIndex * (2 * posRadius);
                int curCircleChecks = (int)Math.Ceiling((2 * Math.PI * (double)curRadius) / (2 * (double)posRadius));

                for (int i = 1; i < curCircleChecks; i++)
                {
                    posChecked++;
                    var cRadians = (2 * Math.PI / (curCircleChecks - 1)) * i; //check decimals
                    var pos = new Vector2((float)Math.Floor(heroPoint.X + curRadius * Math.Cos(cRadians)), (float)Math.Floor(heroPoint.Y + curRadius * Math.Sin(cRadians)));


                    posTable.Add(InitPositionInfo(pos, extraDelayBuffer, extraEvadeDistance, lastMovePos, lowestEvadeTimeSpell));


                    //if (pos.IsWall())
                    //{
                    //    //Render.Circle.DrawCircle(new Vector3(pos.X, pos.Y, myHero.Position.Z), (float)25, Color.White, 3);
                    //}
                    /*
                    if (posDangerLevel > 0)
                    {
                        Render.Circle.DrawCircle(new Vector3(pos.X, pos.Y, myHero.Position.Z), (float) posRadius, Color.White, 3);
                    }*/


                    //var path = MyHero.GetPath(pos.To3D());

                    //Render.Circle.DrawCircle(path[path.Length - 1], (float)posRadius, Color.White, 3);
                    //Render.Circle.DrawCircle(new Vector3(pos.X, pos.Y, myHero.Position.Z), (float)posRadius, Color.White, 3);

                    //var posOnScreen = Drawing.WorldToScreen(path[path.Length - 1]);
                    //Drawing.DrawText(posOnScreen.X, posOnScreen.Y, Color.Aqua, "" + path.Length);
                }
            }

            var sortedPosTable = posTable.OrderBy(p => p.IsDangerousPos).ThenBy(p => p.PosDangerLevel).ThenBy(p => p.PosDangerCount).ThenBy(p => p.DistanceToMouse);

            return sortedPosTable;
        }

        public static PositionInfo GetBestPosition()
        {
            int posChecked = 0;
            int maxPosToCheck = 50;
            int posRadius = 50;
            int radiusIndex = 0;

            bool fastEvadeMode = false;

            var extraDelayBuffer = Config.Properties.GetInt(ConfigValue.ExtraPingBuffer);
            var extraEvadeDistance = ConfigValue.ExtraEvadeDistance.GetInt();

            SpellDetector.UpdateSpells();
            CalculateEvadeTime();

            if (ConfigValue.CalculateWindupDelay.GetBool())
            {
                var extraWindupDelay = AdEvade.LastWindupTime - EvadeUtils.TickCount;
                if (extraWindupDelay > 0)
                {
                    extraDelayBuffer += (int)extraWindupDelay;
                }
            }

            extraDelayBuffer += (int)(AdEvade.AvgCalculationTime);

            if (ConfigValue.HighPrecision.GetBool())
            {
                maxPosToCheck = 150;
                posRadius = 25;
            }

            Vector2 heroPoint = GameData.HeroInfo.ServerPos2D;
            Vector2 lastMovePos = Game.CursorPos.To2D();

            List<PositionInfo> posTable = new List<PositionInfo>();

            Spell lowestEvadeTimeSpell;
            var lowestEvadeTime = SpellDetector.GetLowestEvadeTime(out lowestEvadeTimeSpell);

            List<Vector2> fastestPositions = GetFastestPositions();

            foreach (var pos in fastestPositions) //add the fastest positions into list of candidates
            {
                posTable.Add(InitPositionInfo(pos, extraDelayBuffer, extraEvadeDistance, lastMovePos, lowestEvadeTimeSpell));
            }


            /*if (SpellDetector.spells.Count() == 1)
            {
                var sortedFastestTable =
                posTable.OrderBy(p => p.posDangerLevel);

                if (sortedFastestTable.First() != null && sortedFastestTable.First().posDangerLevel > 0)
                {
                    //use fastest
                }
            }*/

            while (posChecked < maxPosToCheck)
            {
                radiusIndex++;

                int curRadius = radiusIndex * (2 * posRadius);
                int curCircleChecks = (int)Math.Ceiling((2 * Math.PI * (double)curRadius) / (2 * (double)posRadius));

                for (int i = 1; i < curCircleChecks; i++)
                {
                    posChecked++;
                    var cRadians = (2 * Math.PI / (curCircleChecks - 1)) * i; //check decimals
                    var pos = new Vector2((float)Math.Floor(heroPoint.X + curRadius * Math.Cos(cRadians)), (float)Math.Floor(heroPoint.Y + curRadius * Math.Sin(cRadians)));


                    posTable.Add(InitPositionInfo(pos, extraDelayBuffer, extraEvadeDistance, lastMovePos, lowestEvadeTimeSpell));
                }
            }

            IOrderedEnumerable<PositionInfo> sortedPosTable;

            if ((EvadeMode) ConfigValue.EvadeMode.GetInt() == EvadeMode.Fast)
            {
                sortedPosTable =
                posTable.OrderBy(p => p.IsDangerousPos)
                        .ThenByDescending(p => p.IntersectionTime)
                        .ThenBy(p => p.PosDangerLevel)
                        .ThenBy(p => p.PosDangerCount);

                fastEvadeMode = true;

            }
            else if (ConfigValue.FastEvadeActivationTime.GetInt() > 0
               && ConfigValue.FastEvadeActivationTime.GetInt() + Game.Ping + extraDelayBuffer > lowestEvadeTime)
            {
                sortedPosTable =
                posTable.OrderBy(p => p.IsDangerousPos)
                        .ThenByDescending(p => p.IntersectionTime)
                        .ThenBy(p => p.PosDangerLevel)
                        .ThenBy(p => p.PosDangerCount);

                fastEvadeMode = true;
                //ConsoleDebug.WriteLine("fast evade: " + lowestEvadeTime);
            }
            else
            {
                sortedPosTable =
                posTable.OrderBy(p => p.RejectPosition)
                        .ThenBy(p => p.PosDangerLevel)
                        .ThenBy(p => p.PosDangerCount)
                        //.ThenBy(p => p.hasExtraDistance)
                        .ThenBy(p => p.DistanceToMouse);

                if (sortedPosTable.First().PosDangerCount != 0) //if can't dodge smoothly, dodge fast
                {
                    var sortedPosTableFastest =
                    posTable.OrderBy(p => p.IsDangerousPos)
                        .ThenByDescending(p => p.IntersectionTime)
                        .ThenBy(p => p.PosDangerLevel)
                        .ThenBy(p => p.PosDangerCount);

                    if (sortedPosTableFastest.First().PosDangerCount == 0)
                    {
                        sortedPosTable = sortedPosTableFastest;
                        fastEvadeMode = true;
                    }
                }
            }

            //Drawing.OnDraw += delegate(EventArgs args)
            //{
            //    foreach (var p in sortedPosTable)
            //    {
            //        Drawing.DrawCircle(p.position.To3DWorld(), 120, Color.Blue);
            //    }
            //};


            sortedPosTable.OrderByDescending(p => p.Position.Distance(MyHero));

            foreach (var posInfo in sortedPosTable)
            {
                if (CheckPathCollision(MyHero, posInfo.Position) == true)
                {
                    if (fastEvadeMode)
                    {
                        posInfo.Position = GetExtendedSafePosition(GameData.HeroInfo.ServerPos2D, posInfo.Position, extraEvadeDistance);
                        return CanHeroWalkToPos(posInfo.Position, GameData.HeroInfo.MoveSpeed, Game.Ping, 0);
                    }

                    if (PositionInfoStillValid(posInfo))
                    {

                        if (posInfo.Position.CheckDangerousPos(extraEvadeDistance)) //extra evade distance, no multiple skillshots
                        {
                            posInfo.Position = GetExtendedSafePosition(GameData.HeroInfo.ServerPos2D, posInfo.Position, extraEvadeDistance);
                        }

                        //posInfo.position = GetExtendedSafePosition(GameData.HeroInfo.serverPos2D, posInfo.position, extraEvadeDistance);
                        return posInfo;
                    }
                }
            }

            return PositionInfo.SetAllUndodgeable();
        }

        public static PositionInfo GetBestPositionMovementBlock(Vector2 movePos)
        {
            int posChecked = 0;
            int maxPosToCheck = 50;
            int posRadius = 50;
            int radiusIndex = 0;

            var extraEvadeDistance = ConfigValue.ExtraSpellRadius.GetInt();

            Vector2 heroPoint = GameData.HeroInfo.ServerPos2D;
            Vector2 lastMovePos = movePos;//Game.CursorPos.To2D(); //movePos

            List<PositionInfo> posTable = new List<PositionInfo>();

            var extraDist = ConfigValue.ExtraCpaDistance.GetInt();
            var extraDelayBuffer = Config.Properties.GetInt(ConfigValue.ExtraPingBuffer);

            while (posChecked < maxPosToCheck)
            {
                radiusIndex++;

                int curRadius = radiusIndex * (2 * posRadius);
                int curCircleChecks = (int)Math.Ceiling((2 * Math.PI * (double)curRadius) / (2 * (double)posRadius));

                for (int i = 1; i < curCircleChecks; i++)
                {
                    posChecked++;
                    var cRadians = (2 * Math.PI / (curCircleChecks - 1)) * i; //check decimals
                    var pos = new Vector2((float)Math.Floor(heroPoint.X + curRadius * Math.Cos(cRadians)), (float)Math.Floor(heroPoint.Y + curRadius * Math.Sin(cRadians)));

                    //if (pos.Distance(myHero.Position.To2D()) < 100)
                    //    dist = 0;

                    var posInfo = CanHeroWalkToPos(pos, GameData.HeroInfo.MoveSpeed, extraDelayBuffer + Game.Ping, extraDist);
                    posInfo.IsDangerousPos = pos.CheckDangerousPos(6) || CheckMovePath(pos);
                    posInfo.DistanceToMouse = pos.GetPositionValue();
                    posInfo.HasExtraDistance = extraEvadeDistance > 0 ? pos.HasExtraAvoidDistance(extraEvadeDistance) : false;

                    posTable.Add(posInfo);
                }
            }

            var sortedPosTable =
                posTable.OrderBy(p => p.IsDangerousPos)
                        .ThenBy(p => p.PosDangerLevel)
                        .ThenBy(p => p.HasExtraDistance)
                        .ThenBy(p => p.DistanceToMouse);
            //.ThenBy(p => p.intersectionTime);

            foreach (var posInfo in sortedPosTable)
            {
                if (CheckPathCollision(MyHero, posInfo.Position) == true)
                    return posInfo;
            }
            return null;
        }

        public static PositionInfo GetBestPositionBlink()
        {
            int posChecked = 0;
            int maxPosToCheck = 100;
            int posRadius = 50;
            int radiusIndex = 0;

            var extraEvadeDistance = ConfigValue.ExtraSpellRadius.GetInt();

            Vector2 heroPoint = GameData.HeroInfo.ServerPos2DPing;
            Vector2 lastMovePos = Game.CursorPos.To2D();

            int minComfortZone = ConfigValue.MinimumComfortZone.GetInt();

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
                    var pos = new Vector2((float)Math.Floor(heroPoint.X + curRadius * Math.Cos(cRadians)), (float)Math.Floor(heroPoint.Y + curRadius * Math.Sin(cRadians)));

                    bool isDangerousPos = pos.CheckDangerousPos(6);
                    var dist = pos.GetPositionValue();

                    var posInfo = new PositionInfo(pos, isDangerousPos, dist);
                    posInfo.HasExtraDistance = extraEvadeDistance > 0 ? pos.CheckDangerousPos(extraEvadeDistance) : false;

                    posInfo.PosDistToChamps = pos.GetDistanceToChampions();

                    if (minComfortZone < posInfo.PosDistToChamps)
                    {
                        posTable.Add(posInfo);
                    }
                }
            }

            var sortedPosTable =
                posTable.OrderBy(p => p.IsDangerousPos)
                        .ThenBy(p => p.HasExtraDistance)
                        .ThenBy(p => p.DistanceToMouse);

            foreach (var posInfo in sortedPosTable)
            {
                if (CheckPointCollision(MyHero, posInfo.Position) == false)
                    return posInfo;
            }

            return null;
        }

        public static Vector3 GetSafeNavPos(this Vector3 pos, AIHeroClient heroFrom)
        {
            var flags = NavMesh.GetCollisionFlags(pos);
            if (flags == CollisionFlags.Building || flags == CollisionFlags.Wall)
            {
                return heroFrom.Position.Extend(pos, pos.Distance(heroFrom) - 20).To3DWorld().GetSafeNavPos(heroFrom);
            }
            return pos;
        }
        public static PositionInfo GetBestPositionDash(EvadeSpellData spell)
        {
            int posChecked = 0;
            int maxPosToCheck = 100;
            int posRadius = 50;
            int radiusIndex = 0;

            var extraDelayBuffer = Config.Properties.GetInt(ConfigValue.ExtraPingBuffer);
            var extraEvadeDistance = 100;// Evade.Menu.SubMenu("MiscSettings").SubMenu("ExtraBuffers").Item("ExtraEvadeDistance");
            var extraDist = ConfigValue.ExtraCpaDistance.GetInt();

            Vector2 heroPoint = GameData.HeroInfo.ServerPos2DPing;
            Vector2 lastMovePos = Game.CursorPos.To2D();

            List<PositionInfo> posTable = new List<PositionInfo>();
            List<int> spellList = SpellDetector.GetSpellList();

            int minDistance = 50; //Math.Min(spell.range, minDistance)
            int maxDistance = int.MaxValue;

            if (spell.FixedRange)
            {
                minDistance = maxDistance = (int)spell.Range;
            }

            while (posChecked < maxPosToCheck)
            {
                radiusIndex++;

                int curRadius = radiusIndex * (2 * posRadius) + (minDistance - 2 * posRadius);
                int curCircleChecks = (int)Math.Ceiling((2 * Math.PI * (double)curRadius) / (2 * (double)posRadius));

                for (int i = 1; i < curCircleChecks; i++)
                {
                    posChecked++;
                    var cRadians = (2 * Math.PI / (curCircleChecks - 1)) * i; //check decimals
                    var pos = new Vector2((float)Math.Floor(heroPoint.X + curRadius * Math.Cos(cRadians)), (float)Math.Floor(heroPoint.Y + curRadius * Math.Sin(cRadians)));

                    var posInfo = CanHeroWalkToPos(pos, spell.Speed, extraDelayBuffer + Game.Ping, extraDist);
                    posInfo.IsDangerousPos = pos.CheckDangerousPos(6);
                    posInfo.HasExtraDistance = extraEvadeDistance > 0 ? pos.CheckDangerousPos(extraEvadeDistance) : false;// ? 1 : 0;                    
                    posInfo.DistanceToMouse = pos.GetPositionValue();
                    posInfo.SpellList = spellList;

                    posInfo.PosDistToChamps = pos.GetDistanceToChampions();

                    posTable.Add(posInfo);
                }

                if (curRadius >= maxDistance)
                    break;
            }

            var sortedPosTable =
                posTable.OrderBy(p => p.IsDangerousPos)
                        .ThenBy(p => p.PosDangerLevel)
                        .ThenBy(p => p.PosDangerCount)
                        .ThenBy(p => p.HasExtraDistance)
                        .ThenBy(p => p.DistanceToMouse);

            foreach (var posInfo in sortedPosTable)
            {
                if (CheckPathCollision(MyHero, posInfo.Position) == true)
                {
                    if (PositionInfoStillValid(posInfo, spell.Speed))
                    {
                        return posInfo;
                    }
                }
            }

            return null;
        }

        public static PositionInfo GetBestPositionTargetedDash(EvadeSpellData spell)
        {
            /*if (spell.spellDelay > 0)
            {
                if (CheckWindupTime(spell.spellDelay))
                {
                    return null;
                }
            }*/

            var extraDelayBuffer = Config.Properties.GetInt(ConfigValue.ExtraPingBuffer);
            var extraDist = ConfigValue.ExtraCpaDistance.GetInt();

            Vector2 heroPoint = GameData.HeroInfo.ServerPos2DPing;
            Vector2 lastMovePos = Game.CursorPos.To2D();

            List<PositionInfo> posTable = new List<PositionInfo>();
            List<int> spellList = SpellDetector.GetSpellList();

            //int minDistance = 50; //Math.Min(spell.range, minDistance)
            //int maxDistance = int.MaxValue;

            //if (spell.FixedRange)
            //{
            //    minDistance = maxDistance = (int)spell.Range;
            //}

            List<Obj_AI_Base> collisionCandidates = new List<Obj_AI_Base>();

            if (spell.SpellTargets.Contains(SpellTargets.Targetables))
            {
                foreach (var obj in ObjectManager.Get<Obj_AI_Base>()
                    .Where(h => !h.IsMe && h.IsValidTarget(spell.Range)))
                {
                    if (obj.GetType() == typeof(Obj_AI_Turret) && ((Obj_AI_Turret)obj).IsValid())
                    {
                        collisionCandidates.Add(obj);
                    }
                }
            }
            else
            {
                List<AIHeroClient> heroList = new List<AIHeroClient>();

                if (spell.SpellTargets.Contains(SpellTargets.EnemyChampions)
                    && spell.SpellTargets.Contains(SpellTargets.AllyChampions))
                {
                    heroList = EntityManager.Heroes.AllHeroes;
                }
                else if (spell.SpellTargets.Contains(SpellTargets.EnemyChampions))
                {
                    heroList = EntityManager.Heroes.Enemies;
                }
                else if (spell.SpellTargets.Contains(SpellTargets.AllyChampions))
                {
                    heroList = EntityManager.Heroes.Allies;
                }


                foreach (var hero in heroList.Where(h => !h.IsMe && h.IsValidTarget(spell.Range)))
                {
                    collisionCandidates.Add(hero);
                }

                List<Obj_AI_Minion> minionList = new List<Obj_AI_Minion>();

                if (spell.SpellTargets.Contains(SpellTargets.EnemyMinions)
                    && spell.SpellTargets.Contains(SpellTargets.AllyMinions))
                {
                    minionList = EntityManager.MinionsAndMonsters.GetLaneMinions(EntityManager.UnitTeam.Both, Player.Instance.ServerPosition, spell.Range).ToList();
                }
                else if (spell.SpellTargets.Contains(SpellTargets.EnemyMinions))
                {
                    minionList = EntityManager.MinionsAndMonsters.GetLaneMinions(EntityManager.UnitTeam.Enemy, Player.Instance.ServerPosition, spell.Range).ToList();
                }
                else if (spell.SpellTargets.Contains(SpellTargets.AllyMinions))
                {
                    minionList = EntityManager.MinionsAndMonsters.GetLaneMinions(EntityManager.UnitTeam.Ally, Player.Instance.ServerPosition, spell.Range).ToList();
                }

                foreach (var minion in minionList.Where(h => h.IsValidTarget(spell.Range)))
                {
                    collisionCandidates.Add(minion);
                }
            }

            foreach (var candidate in collisionCandidates)
            {
                var pos = candidate.ServerPosition.To2D();

                PositionInfo posInfo;

                if (spell.SpellName == "YasuoDashWrapper")
                {
                    bool hasDashBuff = false;

                    foreach (var buff in candidate.Buffs)
                    {
                        if (buff.Name == "YasuoDashWrapper")
                        {
                            hasDashBuff = true;
                            break;
                        }
                    }

                    if (hasDashBuff)
                        continue;
                }

                if (spell.BehindTarget)
                {
                    var dir = (pos - heroPoint).Normalized();
                    pos = pos + dir * (candidate.BoundingRadius + GameData.HeroInfo.BoundingRadius);
                }

                if (spell.InfrontTarget)
                {
                    var dir = (pos - heroPoint).Normalized();
                    pos = pos - dir * (candidate.BoundingRadius + GameData.HeroInfo.BoundingRadius);
                }

                if (spell.FixedRange)
                {
                    var dir = (pos - heroPoint).Normalized();
                    pos = heroPoint + dir * spell.Range;
                }

                if (spell.EvadeType == EvadeType.Dash)
                {
                    posInfo = CanHeroWalkToPos(pos, spell.Speed, extraDelayBuffer + Game.Ping, extraDist);
                    posInfo.IsDangerousPos = pos.CheckDangerousPos(6);
                    posInfo.DistanceToMouse = pos.GetPositionValue();
                    posInfo.SpellList = spellList;
                }
                else
                {
                    bool isDangerousPos = pos.CheckDangerousPos(6);
                    var dist = pos.GetPositionValue();

                    posInfo = new PositionInfo(pos, isDangerousPos, dist);
                }

                posInfo.Target = candidate;
                posTable.Add(posInfo);
            }

            if (spell.EvadeType == EvadeType.Dash)
            {
                var sortedPosTable =
                posTable.OrderBy(p => p.IsDangerousPos)
                        .ThenBy(p => p.PosDangerLevel)
                        .ThenBy(p => p.PosDangerCount)
                        .ThenBy(p => p.DistanceToMouse);

                var first = sortedPosTable.FirstOrDefault();
                if (first != null && AdEvade.LastPosInfo != null && first.IsDangerousPos == false
                    && AdEvade.LastPosInfo.PosDangerLevel > first.PosDangerLevel)
                {
                    return first;
                }
            }
            else
            {
                var sortedPosTable =
                posTable.OrderBy(p => p.IsDangerousPos)
                        //.ThenByDescending(p => p.hasComfortZone)
                        //.ThenBy(p => p.hasExtraDistance)
                        .ThenBy(p => p.DistanceToMouse);

                var first = sortedPosTable.FirstOrDefault();

                return first;
            }

            return null;

        }

        public static bool CheckWindupTime(float windupTime)
        {
            foreach (KeyValuePair<int, Spell> entry in SpellDetector.Spells)
            {
                Spell spell = entry.Value;

                var hitTime = spell.GetSpellHitTime(GameData.HeroInfo.ServerPos2D);
                if (hitTime < windupTime)
                {
                    return true;
                }
            }

            return false;
        }

        public static float GetMovementBlockPositionValue(Vector2 pos, Vector2 movePos)
        {
            float value = 0;// pos.Distance(movePos);

            foreach (KeyValuePair<int, Spell> entry in SpellDetector.Spells)
            {
                Spell spell = entry.Value;
                var spellPos = spell.GetCurrentSpellPosition(true, Game.Ping);
                var extraDist = 100 + spell.Radius;

                value -= Math.Max(0, -(10 * ((float)0.8 * extraDist) / pos.Distance(spell.GetSpellProjection(pos))) + extraDist);
            }

            return value;
        }

        public static bool PositionInfoStillValid(PositionInfo posInfo, float moveSpeed = 0)
        {
            return true; //too buggy
        }

        public static List<Vector2> GetExtendedPositions(Vector2 from, Vector2 to, float extendDistance)
        {
            Vector2 direction = (to - from).Normalized();
            List<Vector2> positions = new List<Vector2>();
            float sectorDistance = 50;

            for (float i = sectorDistance; i < extendDistance; i += sectorDistance)
            {
                Vector2 pos = to + direction * i;

                positions.Add(pos);
            }

            return positions;
        }

        public static Vector2 GetExtendedSafePosition(Vector2 from, Vector2 to, float extendDistance)
        {
            Vector2 direction = (to - from).Normalized();
            Vector2 lastPosition = to;
            float sectorDistance = 50;

            for (float i = sectorDistance; i <= extendDistance; i += sectorDistance)
            {
                Vector2 pos = to + direction * i;

                if (pos.CheckDangerousPos(6)
                    || CheckPathCollision(MyHero, pos))
                {
                    return lastPosition;
                }

                lastPosition = pos;
            }

            return lastPosition;
        }

        public static void CalculateEvadeTime()
        {
            foreach (KeyValuePair<int, Spell> entry in SpellDetector.Spells)
            {
                Spell spell = entry.Value;
                float evadeTime, spellHitTime;
                spell.CanHeroEvade(MyHero, out evadeTime, out spellHitTime);

                spell.SpellHitTime = spellHitTime;
                spell.EvadeTime = evadeTime;
            }
        }

        public static Vector2 GetFastestPosition(Spell spell)
        {
            var heroPos = GameData.HeroInfo.ServerPos2D;

            if (spell.SpellType == SpellType.Line)
            {
                var projection = heroPos.ProjectOn(spell.StartPos, spell.EndPos).SegmentPoint;
                return projection.Extend(heroPos, spell.Radius + GameData.HeroInfo.BoundingRadius + 10);
            }
            else if (spell.SpellType == SpellType.Circular)
            {
                return spell.EndPos.Extend(heroPos, spell.Radius + 10);
            }

            return Vector2.Zero;
        }

        public static List<Vector2> GetFastestPositions()
        {
            List<Vector2> positions = new List<Vector2>();

            foreach (KeyValuePair<int, Spell> entry in SpellDetector.Spells)
            {
                Spell spell = entry.Value;
                var pos = GetFastestPosition(spell);


                if (pos != Vector2.Zero)
                {
                    positions.Add(pos);
                }

            }

            return positions;
        }

        public static float CompareFastestPosition(Spell spell, Vector2 start, Vector2 movePos)
        {
            var fastestPos = GetFastestPosition(spell);
            var moveDir = (movePos - start).Normalized();
            var fastestDir = (GetFastestPosition(spell) - start).Normalized();

            return moveDir.AngleBetween(fastestDir); // * (180 / ((float)Math.PI));
        }

        public static float GetMinCpaDistance(Vector2 movePos)
        {
            float minDist = float.MaxValue;
            var heroPoint = GameData.HeroInfo.ServerPos2D;

            foreach (Spell spell in SpellDetector.Spells.Values)
            {
                minDist = Math.Min(minDist, GetClosestDistanceApproach(spell, movePos, GameData.HeroInfo.MoveSpeed, Game.Ping, GameData.HeroInfo.ServerPos2DPing, 0));
            }

            return minDist;
        }

        public static float GetCombinedIntersectionDistance(Vector2 movePos)
        {
            var heroPoint = GameData.HeroInfo.ServerPos2D;
            float sumIntersectDist = 0;

            foreach (Spell spell in SpellDetector.Spells.Values)
            {
                var intersectDist = GetIntersectDistance(spell, heroPoint, movePos);
                sumIntersectDist += intersectDist * ((int)spell.Dangerlevel + 1);
                //TODO: Check
            }

            return sumIntersectDist;
        }

        public static float GetIntersectDistance(Spell spell, Vector2 start, Vector2 end)
        {
            if (spell == null)
                return float.MaxValue;

            Vector3 start3D = new Vector3(start.X, start.Y, 0);
            Vector2 walkDir = (end - start);
            Vector3 walkDir3D = new Vector3(walkDir.X, walkDir.Y, 0);

            var heroPath = new Ray(start3D, walkDir3D);

            if (spell.SpellType == SpellType.Line)
            {
                Vector2 intersection;
                bool hasIntersection = spell.LineIntersectLinearSpellEx(start, end, out intersection);
                if (hasIntersection)
                {
                    return start.Distance(intersection);
                }
            }
            else if (spell.SpellType == SpellType.Circular)
            {
                if (end.InSkillShot(spell, GameData.HeroInfo.BoundingRadius) == false)
                {
                    Vector2 intersection1, intersection2;
                    MathUtils.FindLineCircleIntersections(spell.EndPos, spell.Radius, start, end, out intersection1, out intersection2);

                    if (intersection1.X != float.NaN && MathUtils.IsPointOnLineSegment(intersection1, start, end))
                    {
                        return start.Distance(intersection1);
                    }
                    else if (intersection2.X != float.NaN && MathUtils.IsPointOnLineSegment(intersection2, start, end))
                    {
                        return start.Distance(intersection2);
                    }
                }
            }

            return float.MaxValue;
        }

        public static PositionInfo CanHeroWalkToPos(Vector2 pos, float speed, float delay, float extraDist, bool useServerPosition = true)
        {
            int posDangerLevel = 0;
            int posDangerCount = 0;
            float closestDistance = float.MaxValue;
            List<int> dodgeableSpells = new List<int>();
            List<int> undodgeableSpells = new List<int>();

            Vector2 heroPos = GameData.HeroInfo.ServerPos2D;


            if (useServerPosition == false)
            {
                heroPos = MyHero.Position.To2D();
            }

            foreach (KeyValuePair<int, Spell> entry in SpellDetector.Spells)
            {
                Spell spell = entry.Value;

                closestDistance = Math.Min(closestDistance, GetClosestDistanceApproach(spell, pos, speed, delay, heroPos, extraDist));
                //GetIntersectTime(spell, GameData.HeroInfo.serverPos2D, pos);
                //Math.Min(closestDistance, GetClosestDistanceApproach(spell, pos, GameData.HeroInfo.moveSpeed, delay, GameData.HeroInfo.serverPos2D));

                if (pos.InSkillShot(spell, GameData.HeroInfo.BoundingRadius + 6)
                    || PredictSpellCollision(spell, pos, speed, delay, heroPos, extraDist, useServerPosition)
                    || (spell.Info.SpellType != SpellType.Line && pos.IsNearEnemy(ConfigValue.MinimumComfortZone.GetInt())))
                {
                    posDangerLevel = Math.Max(posDangerLevel, (int)spell.Dangerlevel);
                    posDangerCount += (int)spell.Dangerlevel;
                    undodgeableSpells.Add(spell.SpellId);
                }
                else
                {
                    dodgeableSpells.Add(spell.SpellId);
                }
            }

            return new PositionInfo(
                pos,
                posDangerLevel,
                posDangerCount,
                posDangerCount > 0,
                closestDistance,
                dodgeableSpells,
                undodgeableSpells);
        }

        public static float GetClosestDistanceApproach(Spell spell, Vector2 pos, float speed, float delay, Vector2 heroPos, float extraDist)
        {
            var walkDir = (pos - heroPos).Normalized();

            if (spell.SpellType == SpellType.Line && spell.Info.ProjectileSpeed != float.MaxValue)
            {
                var spellPos = spell.GetCurrentSpellPosition(true, delay);
                var spellStartPos = spell.CurrentSpellPosition;
                var spellEndPos = spell.GetSpellEndPosition();
                var extendedPos = pos.ExtendDir(walkDir, GameData.HeroInfo.BoundingRadius + speed * delay / 1000);

                Vector2 cHeroPos;
                Vector2 cSpellPos;

                var cpa2 = MathUtils.GetCollisionDistanceEx(
                    heroPos, walkDir * speed, GameData.HeroInfo.BoundingRadius,
                    spellPos, spell.Direction * spell.Info.ProjectileSpeed, spell.Radius + extraDist,
                    out cHeroPos, out cSpellPos);

                var cHeroPosProjection = cHeroPos.ProjectOn(heroPos, extendedPos);
                var cSpellPosProjection = cSpellPos.ProjectOn(spellPos, spellEndPos);

                if (cSpellPosProjection.IsOnSegment && cHeroPosProjection.IsOnSegment && cpa2 != float.MaxValue)
                {
                    return 0;
                }

                var cpa = MathUtilsCpa.CPAPointsEx(heroPos, walkDir * speed, spellPos, spell.Direction * spell.Info.ProjectileSpeed, pos, spellEndPos, out cHeroPos, out cSpellPos);

                cHeroPosProjection = cHeroPos.ProjectOn(heroPos, extendedPos);
                cSpellPosProjection = cSpellPos.ProjectOn(spellPos, spellEndPos);

                var checkDist = GameData.HeroInfo.BoundingRadius + spell.Radius + extraDist;

                if (cSpellPosProjection.IsOnSegment && cHeroPosProjection.IsOnSegment)
                {
                    return Math.Max(0, cpa - checkDist);
                }
                else
                {
                    return checkDist;
                }


                //return MathUtils.ClosestTimeOfApproach(heroPos, walkDir * speed, spellPos, spell.direction * spell.info.projectileSpeed);
            }
            else if (spell.SpellType == SpellType.Line && spell.Info.ProjectileSpeed == float.MaxValue)
            {
                var spellHitTime = Math.Max(0, spell.EndTime - EvadeUtils.TickCount - delay);  //extraDelay
                var walkRange = heroPos.Distance(pos);
                var predictedRange = speed * (spellHitTime / 1000);
                var tHeroPos = heroPos + walkDir * Math.Min(predictedRange, walkRange); //Hero predicted pos

                var projection = tHeroPos.ProjectOn(spell.StartPos, spell.EndPos);

                return Math.Max(0, tHeroPos.Distance(projection.SegmentPoint)
                    - (spell.Radius + GameData.HeroInfo.BoundingRadius + extraDist)); //+ dodgeBuffer
            }
            else if (spell.SpellType == SpellType.Circular)
            {
                var spellHitTime = Math.Max(0, spell.EndTime - EvadeUtils.TickCount - delay);  //extraDelay
                var walkRange = heroPos.Distance(pos);
                var predictedRange = speed * (spellHitTime / 1000);
                var tHeroPos = heroPos + walkDir * Math.Min(predictedRange, walkRange); //Hero predicted pos

                if (spell.Info.SpellName == "VeigarEventHorizon")
                {
                    var wallRadius = 65;
                    var midRadius = spell.Radius - wallRadius;

                    if (spellHitTime == 0)
                    {
                        return 0;
                    }

                    if (tHeroPos.Distance(spell.EndPos) >= spell.Radius)
                    {
                        return Math.Max(0, tHeroPos.Distance(spell.EndPos) - midRadius - wallRadius);
                    }
                    else
                    {
                        return Math.Max(0, midRadius - tHeroPos.Distance(spell.EndPos) - wallRadius);
                    }
                }

                var closestDist = Math.Max(0, tHeroPos.Distance(spell.EndPos) - (spell.Radius + extraDist));
                if (spell.Info.ExtraEndTime > 0 && closestDist != 0)
                {
                    var remainingTime = Math.Max(0, spell.EndTime + spell.Info.ExtraEndTime - EvadeUtils.TickCount - delay);
                    var predictedRange2 = speed * (remainingTime / 1000);
                    var tHeroPos2 = heroPos + walkDir * Math.Min(predictedRange2, walkRange);

                    if (CheckMoveToDirection(tHeroPos, tHeroPos2))
                    {
                        return 0;
                    }
                }
                else
                {
                    return closestDist;
                }
            }
            else if (spell.SpellType == SpellType.Arc)
            {
                var spellPos = spell.GetCurrentSpellPosition(true, delay);
                var spellEndPos = spell.GetSpellEndPosition();

                var pDir = spell.Direction.Perpendicular();
                spellPos = spellPos - pDir * spell.Radius / 2;
                spellEndPos = spellEndPos - pDir * spell.Radius / 2;

                var extendedPos = pos.ExtendDir(walkDir, GameData.HeroInfo.BoundingRadius);

                Vector2 cHeroPos;
                Vector2 cSpellPos;

                var cpa = MathUtilsCpa.CPAPointsEx(heroPos, walkDir * speed, spellPos, spell.Direction * spell.Info.ProjectileSpeed, pos, spellEndPos, out cHeroPos, out cSpellPos);

                var cHeroPosProjection = cHeroPos.ProjectOn(heroPos, extendedPos);
                var cSpellPosProjection = cSpellPos.ProjectOn(spellPos, spellEndPos);

                var checkDist = spell.Radius + extraDist;

                if (cHeroPos.InSkillShot(spell, GameData.HeroInfo.BoundingRadius))
                {
                    if (cSpellPosProjection.IsOnSegment && cHeroPosProjection.IsOnSegment)
                    {
                        return Math.Max(0, cpa - checkDist);
                    }
                    else
                    {
                        return checkDist;
                    }
                }
            }

            return 1;
        }

        public static bool PredictSpellCollision(Spell spell, Vector2 pos, float speed, float delay, Vector2 heroPos, float extraDist, bool useServerPosition = true)
        {
            extraDist = extraDist + 10;

            if (useServerPosition == false)
            {
                return GetClosestDistanceApproach(spell, pos, speed, 0,
                    GameData.HeroInfo.ServerPos2D, 0) == 0;
            }

            return
                    GetClosestDistanceApproach(spell, pos, speed, delay, //Game.Ping + Extra Buffer
                    GameData.HeroInfo.ServerPos2DPing, extraDist) == 0
                 || GetClosestDistanceApproach(spell, pos, speed, Game.Ping, //Game.Ping
                    GameData.HeroInfo.ServerPos2DPing, extraDist) == 0;
        }

        public static Vector2 GetRealHeroPos(float delay = 0)
        {
            var path = MyHero.Path;
            if (path.Length < 1)
            {
                return GameData.HeroInfo.ServerPos2D;
            }

            //if (!myHero.IsMoving)
            //    return myHero.Position.To2D();

            var serverPos = GameData.HeroInfo.ServerPos2D;
            var heroPos = MyHero.Position.To2D();

            var walkDir = (path[0].To2D() - heroPos).Normalized();
            var realPos = heroPos + walkDir * GameData.HeroInfo.MoveSpeed * (delay / 1000);

            return realPos;
        }

        public static bool CheckPathCollision(Obj_AI_Base unit, Vector2 movePos)
        {
            //var path = unit.Path;
            var path = unit.GetPath(GameData.HeroInfo.ServerPos2D.To3DWorld(), movePos.To3DWorld());

            if (path.Length > 0)
            {
                if (movePos.Distance(path[path.Length - 1].To2D()) > 5 || path.Length > 2)
                {
                    return true;
                }
            }

            return false;
        }

        public static bool CheckPointCollision(Obj_AI_Base unit, Vector2 movePos)
        {
            //var path = unit.Path;
            var path = unit.GetPath(movePos.To3D());

            if (path.Length > 0)
            {
                if (movePos.Distance(path[path.Length - 1].To2D()) > 5)
                {
                    return true;
                }
            }

            return false;
        }

        public static bool CheckMovePath(Vector2 movePos, float extraDelay = 0)
        {
            /*if (EvadeSpell.lastSpellEvadeCommand.evadeSpellData != null)
            {
                var evadeSpell = EvadeSpell.lastSpellEvadeCommand.evadeSpellData;
                float evadeTime = Game.Ping;

                if (EvadeSpell.lastSpellEvadeCommand.evadeSpellData.evadeType == EvadeType.Dash)
                    evadeTime += evadeSpell.spellDelay + GameData.HeroInfo.serverPos2D.Distance(movePos) / (evadeSpell.speed / 1000);
                else if (EvadeSpell.lastSpellEvadeCommand.evadeSpellData.evadeType == EvadeType.Blink)
                    evadeTime += evadeSpell.spellDelay;

                if (Evade.GetTickCount - EvadeSpell.lastSpellEvadeCommand.timestamp < evadeTime)
                {

                    ConsoleDebug.WriteLine("in" + CheckMoveToDirection(EvadeSpell.lastSpellEvadeCommand.targetPosition, movePos));
                    return CheckMoveToDirection(EvadeSpell.lastSpellEvadeCommand.targetPosition, movePos);
                }
            }*/


            var path = MyHero.GetPath(movePos.To3D());
            //var path = MyHero.Path;
            Vector2 lastPoint = Vector2.Zero;

            foreach (Vector3 point in path)
            {
                var point2D = point.To2D();
                if (lastPoint != Vector2.Zero && CheckMoveToDirection(lastPoint, point2D, extraDelay))
                {
                    return true;
                }

                if (lastPoint != Vector2.Zero)
                {
                    lastPoint = point2D;
                }
                else
                {
                    lastPoint = MyHero.ServerPosition.To2D();
                }
            }

            return false;
        }

        public static bool CheckMoveToDirection(Vector2 from, Vector2 movePos, float extraDelay = 0)
        {
            var dir = (movePos - from).Normalized();
            //movePos = movePos.ExtendDir(dir, GameData.HeroInfo.boundingRadius);

            foreach (KeyValuePair<int, Spell> entry in SpellDetector.Spells)
            {
                Spell spell = entry.Value;

                if (!from.InSkillShot(spell, GameData.HeroInfo.BoundingRadius))
                {
                    Vector2 spellPos = spell.CurrentSpellPosition;

                    if (spell.SpellType == SpellType.Line)
                    {
                        if (spell.LineIntersectLinearSpell(from, movePos))
                            return true;

                    }
                    else if (spell.SpellType == SpellType.Circular)
                    {
                        if (spell.Info.SpellName == "VeigarEventHorizon")
                        {
                            var cpa2 = MathUtilsCpa.CPAPointsEx(from, dir * GameData.HeroInfo.MoveSpeed, spell.EndPos, new Vector2(0, 0), movePos, spell.EndPos);

                            if (from.Distance(spell.EndPos) < spell.Radius &&
                                !(from.Distance(spell.EndPos) < spell.Radius - 135 &&
                                movePos.Distance(spell.EndPos) < spell.Radius - 135))
                            {
                                return true;
                            }
                            else if (from.Distance(spell.EndPos) > spell.Radius && cpa2 < spell.Radius + 10)
                            {
                                return true;
                            }
                        }
                        else
                        {
                            Vector2 cHeroPos;
                            Vector2 cSpellPos;

                            var cpa2 = MathUtils.GetCollisionDistanceEx(
                                from, dir * GameData.HeroInfo.MoveSpeed, 1,
                                spell.EndPos, new Vector2(0, 0), spell.Radius,
                                out cHeroPos, out cSpellPos);

                            var cHeroPosProjection = cHeroPos.ProjectOn(from, movePos);

                            if (cHeroPosProjection.IsOnSegment && cpa2 != float.MaxValue)
                            {
                                return true;
                            }

                            /*var cpa = MathUtilsCPA.CPAPointsEx(from, dir * GameData.HeroInfo.moveSpeed, spell.endPos, new Vector2(0, 0), movePos, spell.endPos);

                            if (cpa < spell.radius + 10)
                            {
                                return true;
                            }*/
                        }
                    }
                    else if (spell.SpellType == SpellType.Arc)
                    {
                        if (from.IsLeftOfLineSegment(spell.StartPos, spell.EndPos))
                        {
                            return MathUtils.CheckLineIntersection(from, movePos, spell.StartPos, spell.EndPos);
                        }

                        var spellRange = spell.StartPos.Distance(spell.EndPos);
                        var midPoint = spell.StartPos + spell.Direction * (spellRange / 2);

                        var cpa = MathUtilsCpa.CPAPointsEx(from, dir * GameData.HeroInfo.MoveSpeed, midPoint, new Vector2(0, 0), movePos, midPoint);

                        if (cpa < spell.Radius + 10)
                        {
                            return true;
                        }
                    }
                    else if (spell.SpellType == SpellType.Cone)
                    {

                    }
                }
            }

            return false;
        }

    }
}