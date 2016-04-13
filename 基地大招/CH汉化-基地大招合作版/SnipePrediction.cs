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

            var heroEntry = Listing.UltSpellDataList[ObjectManager.Player.ChampionName];
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
                lastEstimatedPosition = new Vector3(0,0,0);
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

            ultBoundingRadius = Listing.UltSpellDataList[ObjectManager.Player.ChampionName].Width;

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

                    
                float realDist = moveSpeed*(timeElapsed_ms/1000);
                CastPosition = GetCastPosition(realDist);
                
                lastEstimatedPosition = CastPosition;
            }

            if (args.Status == TeleportStatus.Abort)
            {
                SnipeChance = HitChance.Impossible;
                CancelProcess();
            }

            int minHitChance = Listing.snipeMenu.Get<Slider>("minSnipeHitChance").CurrentValue;
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
            var pathDirVec = lastRealPath.Last() - lastRealPath.First();

            Vector3 bestPathDirVec = new Vector3(0, 0, 0);
            float smallestDeltaDistToWalkDist = 25000f;
            
            for (float i = 1 - 0.01f; i > 0; i -= 0.01f)
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
            float travelTime = Algorithm.GetUltTravelTime(ObjectManager.Player, CastPosition);

            float regedHealthTillArrival = Algorithm.SimulateHealthRegen(target, invisibleStartTime, 
                (int)Math.Floor(Core.GameTickCount + travelTime));

            float totalEnemyHp = target.Health + regedHealthTillArrival;

            float timeLeft = recallEnd - Core.GameTickCount;

            bool enoughDmg = Damage.GetAioDmg(target, timeLeft, CastPosition, totalEnemyHp) > totalEnemyHp;
            bool intime = travelTime < timeLeft;

            if (intime && enoughDmg)
            {
                Player.CastSpell(SpellSlot.R, CastPosition);
                var castDelay =
                    Listing.UltSpellDataList[ObjectManager.Player.ChampionName].Delay;

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

        private void MoveCamera(EventArgs args)
        {
            var ultMissile = ObjectManager.Get<MissileClient>()
                .FirstOrDefault(x => x.IsAlly && x.IsValidMissile() && x.SpellCaster is AIHeroClient &&
                            ((AIHeroClient)x.SpellCaster).IsMe);

            if (ultMissile == null)
                return;

            if (EntityManager.Heroes.Enemies.Any(x => x.Distance(ObjectManager.Player) <= 1000 && x.IsValid))
            {
                Camera.CameraX = ObjectManager.Player.Position.X;
                Camera.CameraY = ObjectManager.Player.Position.Y;
                CancelProcess();
                return;
            }

            

            Camera.CameraX = ultMissile.Position.X;
            Camera.CameraY = ultMissile.Position.Y;
        }
    }
}
