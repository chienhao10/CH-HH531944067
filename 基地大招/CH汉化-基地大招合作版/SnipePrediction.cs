using System;
using System.Collections.Generic;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu.Values;
using SharpDX;

namespace HumanziedBaseUlt
{
    class SnipePrediction
    {
        public readonly AIHeroClient target;
        private readonly int invisibleStartTime;
        private readonly Vector3[] lastRealPath;

        private readonly float ultBoundingRadius;

        private Vector3 CastPosition;
        private HitChance SnipeChance;

        private int lastAbortTick;

        private static float PathLengthTo(Vector3[] path, Vector3 to)
        {
            var distance = 0f;
            for (var i = 0; i < path.Length - 1; i++)
            {
                distance += path[i].Distance(path[i + 1]);

                if (path[i + 1] == to)
                    break;
            }
            return distance;
        }
        private IEnumerable<Obj_AI_Base> DoesCollide()
        {
            if (ObjectManager.Player.ChampionName == "Ezreal")
                return new List<Obj_AI_Base>();

            var heroEntry = Listing.spellDataList.First(x => x.championName == ObjectManager.Player.ChampionName);
            Vector3 destPos = lastRealPath.LastOrDefault();

            return (from unit in EntityManager.Heroes.Enemies.Where(h => ObjectManager.Player.Distance(h) < 2000)
                    let pred =
                        Prediction.Position.PredictLinearMissile(unit, 2000, (int)heroEntry.Width, (int)heroEntry.Delay,
                            heroEntry.Speed, -1)
                    let endpos = ObjectManager.Player.ServerPosition.Extend(destPos, 2000)
                    let projectOn = pred.UnitPosition.To2D().ProjectOn(ObjectManager.Player.ServerPosition.To2D(), endpos)
                    where projectOn.SegmentPoint.Distance(endpos) < (int)heroEntry.Width + unit.BoundingRadius
                    select unit).Cast<Obj_AI_Base>().ToList();
        }

        public void CancelProcess()
        {
            try
            {
                SnipeChance = HitChance.Impossible;
                Teleport.OnTeleport -= SnipePredictionOnTeleport;
            }
            catch
            {
                // ignored
            }
        }

        public SnipePrediction(Events.InvisibleEventArgs targetArgs)
        {
            SnipeChance = HitChance.Impossible;
            target = targetArgs.sender;
            invisibleStartTime = targetArgs.StartTime;
            lastRealPath = targetArgs.LastRealPath;

            // ReSharper disable once PossibleNullReferenceException
            ultBoundingRadius = Listing.spellDataList.FirstOrDefault(x => x.championName == ObjectManager.Player.ChampionName).Width;

            Teleport.OnTeleport += SnipePredictionOnTeleport;
        }

        private void SnipePredictionOnTeleport(Obj_AI_Base sender, Teleport.TeleportEventArgs args)
        {
            if (sender != target) return;

            float timeElapsed = Core.GameTickCount - invisibleStartTime;

            /*new try of target to recall*/
            //if (Core.GameTickCount - lastAbortTick <= 1000)
                //timeElapsed = lastAbortTick - invisibleStartTime;

            if (DoesCollide().Any())
                SnipeChance = HitChance.Collision;

            if (args.Status == TeleportStatus.Start)
            { 
                float walkDist = target.Distance(lastRealPath.Last());
                float moveSpeed = target.MoveSpeed;

                float normalTime = walkDist/moveSpeed;

                /*target hasn't reached end point*/
                if (timeElapsed/1000 <= normalTime)
                {
                    SnipeChance = HitChance.High;
                }
                else if (timeElapsed/1000 > normalTime) /*target reached endPoint and is nearby*/
                {
                    float extraTimeElapsed = timeElapsed / 1000 - normalTime;
                    float targetSafeZoneTime = ultBoundingRadius/moveSpeed;

                    if (extraTimeElapsed < targetSafeZoneTime)
                    {
                        /*target has reached end point but is still in danger zone*/
                        SnipeChance = HitChance.Medium;
                    }
                    else
                    {
                        /*target too far away*/
                        SnipeChance = HitChance.Low;
                    }
                }

                float realDist = moveSpeed * timeElapsed / 1000;
                CastPosition = lastRealPath.OrderBy(x => Math.Abs(PathLengthTo(lastRealPath, x) - realDist)).First();
            }

            if (args.Status == TeleportStatus.Abort)
            {
                SnipeChance = HitChance.Impossible;
                lastAbortTick = Core.GameTickCount;
            }

            int minHitChance = Listing.snipeMenu["minSnipeHitChance"].Cast<Slider>().CurrentValue;
            int currentHitChanceInt = 0;

            if ((int) SnipeChance <= 2)
                currentHitChanceInt = 0;
            else if (SnipeChance == HitChance.Low)
                currentHitChanceInt = 1;
            else if (SnipeChance == HitChance.Medium)
                currentHitChanceInt = 2;
            else if (SnipeChance == HitChance.High)
                currentHitChanceInt = 3;

            if (CastPosition != null && currentHitChanceInt >= minHitChance)
                CheckUltCast(args.Start + args.Duration);
        }
        /// <summary>
        /// HitChance (collision etc..) OK
        /// </summary>
        /// <param name="recallEnd"></param>
        private void CheckUltCast(int recallEnd)
        {
            float regedHealthRecallFinished = Algorithm.SimulateHealthRegen(target, invisibleStartTime, recallEnd);
            float totalEnemyHp = target.Health + regedHealthRecallFinished;

            float timeLeft = recallEnd - Core.GameTickCount;
            float travelTime = Algorithm.GetUltTravelTime(ObjectManager.Player, CastPosition);

            bool enoughDmg = Damage.GetAioDmg(target, timeLeft, CastPosition) > totalEnemyHp;
            bool intime = travelTime <= timeLeft;

            if (intime && enoughDmg)
            {
                Player.CastSpell(SpellSlot.R, CastPosition);
                Teleport.OnTeleport -= SnipePredictionOnTeleport;
            }
        }
    }
}
