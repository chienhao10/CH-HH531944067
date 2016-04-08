using System;
using System.Collections.Generic;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK.Rendering;
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
                lastEstimatedPosition = Vector3.Zero;
                SnipeChance = HitChance.Impossible;
                Teleport.OnTeleport -= SnipePredictionOnTeleport;
                Drawing.OnDraw -= OnDraw;
                Game.OnUpdate -= MoveCamera;
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

        private Vector3 lastEstimatedPosition;
        private void OnDraw(EventArgs args)
        {
            Vector3 lastPositionOnPath = target.Position;

            new Circle(new ColorBGRA(new Vector4(1,0,0,1)), 50, filled:true).Draw(lastPositionOnPath);
            new Circle(new ColorBGRA(new Vector4(0, 1, 0, 1)), 50, filled: true).Draw(lastEstimatedPosition);

            var lastPositionOnPathWTC = Drawing.WorldToScreen(lastPositionOnPath);
            var lastEstimatedPositionWTC = Drawing.WorldToScreen(lastEstimatedPosition);
            var myPosWTC = Drawing.WorldToScreen(ObjectManager.Player.Position);

            Drawing.DrawLine(lastPositionOnPathWTC, lastEstimatedPositionWTC, 1, System.Drawing.Color.Green);
            Drawing.DrawLine(myPosWTC, lastPositionOnPathWTC, 1, System.Drawing.Color.Red);
        }

        private void SnipePredictionOnTeleport(Obj_AI_Base sender, Teleport.TeleportEventArgs args)
        {
            if (sender != target) return;

            float timeElapsed_ms = Core.GameTickCount - invisibleStartTime;

            /*new try of target to recall*/
            //if (Core.GameTickCount - lastAbortTick <= 1000)
                //timeElapsed = lastAbortTick - invisibleStartTime;

            if (DoesCollide().Any())
                SnipeChance = HitChance.Collision;

            if (args.Status == TeleportStatus.Start)
            {
                float maxWalkDist = target.Position.Distance(lastRealPath.Last());
                float moveSpeed = target.MoveSpeed;

                float normalTime_ms = maxWalkDist/moveSpeed*1000;

                /*target hasn't reached end point*/
                if (timeElapsed_ms <= normalTime_ms)
                {
                    SnipeChance = HitChance.High;
                }
                else if (timeElapsed_ms > normalTime_ms) /*target reached endPoint and is nearby*/
                {
                    float extraTimeElapsed = timeElapsed_ms - normalTime_ms;
                    float targetSafeZoneTime = ultBoundingRadius/moveSpeed*1000;

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

                float realDist = moveSpeed * (timeElapsed_ms/1000);
                CastPosition = GetCastPosition(realDist);
                lastEstimatedPosition = CastPosition;
            }

            if (args.Status == TeleportStatus.Abort)
            {
                SnipeChance = HitChance.Impossible;
                CancelProcess();
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

            if (currentHitChanceInt >= minHitChance)
            {
                if (Listing.snipeMenu.Get<CheckBox>("snipeDraw").CurrentValue)
                    Drawing.OnDraw += OnDraw;
                CheckUltCast(args.Start + args.Duration);
            }
            else
                CancelProcess();
                
        }

        /// <summary>
        /// Searches for the best path point having a dist which is euqal to the walked dist
        /// </summary>
        /// <param name="walkedDist"></param>
        /// <returns></returns>
        private Vector3 GetCastPosition(float walkedDist)
        {
            float accuracy = Listing.snipeMenu.Get<Slider>("snipeAccuracy").CurrentValue / 100;

            var pathDirVec = lastRealPath.Last() - lastRealPath.First();

            Vector3 bestPathDirVec = Vector3.Zero;
            float smallestDeltaDistToWalkDist = float.MaxValue;

            for (float i = 1 - accuracy; i > 0; i -= accuracy)
            {
                var shortPathDirVec = pathDirVec*i;
                if (Math.Abs(shortPathDirVec.Length() - walkedDist) < smallestDeltaDistToWalkDist)
                {
                    smallestDeltaDistToWalkDist = Math.Abs(shortPathDirVec.Length() - walkedDist);
                    bestPathDirVec = shortPathDirVec;
                }
            }

            return lastRealPath.First() + bestPathDirVec;
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
                var castDelay =
                    Listing.spellDataList.First(x => x.championName == ObjectManager.Player.ChampionName).Delay;

                if (Listing.snipeMenu.Get<CheckBox>("snipeCinemaMode").CurrentValue)
                    Core.DelayAction(() =>
                    {
                        Game.OnUpdate += MoveCamera;
                    }, (int)castDelay);

                Core.DelayAction(() =>
                {
                    CancelProcess();
                    Camera.CameraX = ObjectManager.Player.Position.X;
                    Camera.CameraY = ObjectManager.Player.Position.Y;
                }, (int)(castDelay + travelTime) + 500);
            }
            else
                CancelProcess();
        }

        string LastUltMissileName { get; set; }

        private void MoveCamera(EventArgs args)
        {
            var ultMissile = ObjectManager.Get<MissileClient>()
                .First(x => x.IsAlly && x.IsValidMissile() && x.SpellCaster is AIHeroClient &&
                            ((AIHeroClient)x.SpellCaster).IsMe);
            Vector2 camPos = new Vector2(Camera.CameraX, Camera.CameraY);

            LastUltMissileName = ultMissile.Name;

            bool camAtProjectile = camPos.Distance(ultMissile.Position) <= 150;
            if (EntityManager.Heroes.Enemies.Any(x => x.Distance(ObjectManager.Player) <= 1000 && x.IsValid) && 
                camAtProjectile)
            {
                Camera.CameraX = ObjectManager.Player.Position.X;
                Camera.CameraY = ObjectManager.Player.Position.Y;
                return;
            }

            

            Camera.CameraX = ultMissile.Position.X;
            Camera.CameraY = ultMissile.Position.Y;
        }
    }
}
