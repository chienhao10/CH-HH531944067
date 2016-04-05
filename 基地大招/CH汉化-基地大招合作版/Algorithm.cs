using System;
using System.Collections.Generic;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu.Values;
using SharpDX;

namespace HumanziedBaseUlt
{
    static class Algorithm
    {
        static float recusrviveDelayDeviation = 50;
        private static float lastEnemyReg = 0;

        public static float SimulateHealthRegen(AIHeroClient enemy, int StartTime, int EndTime)
        {    
            float regen = 0;

            int start = (int)Math.Round((double)(StartTime / 1000));
            int end = (int)Math.Round((double)(EndTime / 1000));

            bool hasbuff = Listing.Regeneration.HasPotionActive(enemy);
            BuffInstance regenBuff = hasbuff ? Listing.Regeneration.GetPotionBuff(enemy) : null;
            float buffEndTime = hasbuff ? regenBuff.EndTime : 0;

            //if (hasbuff)
                //Chat.Print("startTime " + start + "//buffendTime: " + buffEndTime + "//GeneralEndTime: " + end);

            for (int i = start; i <= end; ++i)
            {
                regen += 
                    i >= buffEndTime || !hasbuff
                    ? 
                    Listing.Regeneration.GetNormalRegenRate(enemy)
                    : 
                    Listing.Regeneration.GetPotionRegenRate(regenBuff);
            }

            return regen;
        }

        /// <summary>
        /// per second
        /// </summary>
        /// <param name="enemy"></param>
        /// <returns></returns>
        private static float GetFountainReg(AIHeroClient enemy)
        {
            float regSpeedDefault = Listing.config.Get<Slider>("fountainReg").CurrentValue / 10;
            float regSpeedMin20 = Listing.config.Get<Slider>("fountainRegMin20").CurrentValue / 10;

            float normalHpReged = enemy.MaxHealth/100*regSpeedDefault + enemy.MaxHealth / 100*2.8f;
            float min20HpReged = enemy.MaxHealth/100* regSpeedMin20;

            float fountainReg = Listing.config.Get<CheckBox>("min20").CurrentValue ? min20HpReged :
                normalHpReged;

            return fountainReg;
        }

        public static float SimulateRealDelayTime(AIHeroClient enemy, int recallEndTick, float aioDmg)
        {
            float fountainReg = GetFountainReg(enemy);
            Events.InvisibleEventArgs invisEntry = Listing.invisEnemiesList.First(x => x.sender.Equals(enemy));

            float regedRecallEnd = SimulateHealthRegen(enemy, invisEntry.StartTime, recallEndTick);
            float hpRecallEnd = enemy.Health + regedRecallEnd;

            // totalEnemyHp + fountainReg * seconds = myDmg
            float normalDelay = ((aioDmg - hpRecallEnd) / fountainReg) * 1000;


            //Theory: ENEMY REGS NORMAL HP DURING DELAY TIME TO SHOOT IN FOUNTAIN => FALSE

            /*float arriveTime0 = recallEndTick + normalDelay;

            int start = recallEndTick;
            int end = (int)arriveTime0;

            float additional_STD_RegDuringDelay = SimulateHealthRegen(enemy, start, end);

            // totalEnemyHp + fountainReg * seconds + normalRegAfterRecallFinished * seconds = myDmg <=> time
            float realDelayTime = ((aioDmg - hpRecallEnd) / (fountainReg + additional_STD_RegDuringDelay)) * 1000;*/

            lastEnemyReg = regedRecallEnd + fountainReg * (normalDelay / 1000);

            return normalDelay;
        }

        public static float GetLastEstimatedEnemyReg()
        {
            return lastEnemyReg;
        }

        public static IEnumerable<Obj_AI_Base> GetCollision(string sourceName, Vector3? dest = null)
        {
            if (sourceName == "Ezreal")
                return new List<Obj_AI_Base>();

            var heroEntry = Listing.spellDataList.First(x => x.championName == sourceName);
            Vector3 enemyBaseVec = dest ?? ObjectManager.Get<Obj_SpawnPoint>().First(x => x.IsEnemy).Position;

            return (from unit in EntityManager.Heroes.Enemies.Where(h => ObjectManager.Player.Distance(h) < 2000)
                    let pred =
                        Prediction.Position.PredictLinearMissile(unit, 2000, (int)heroEntry.Width, (int)heroEntry.Delay,
                            heroEntry.Speed, -1)
                    let endpos = ObjectManager.Player.ServerPosition.Extend(enemyBaseVec, 2000)
                    let projectOn = pred.UnitPosition.To2D().ProjectOn(ObjectManager.Player.ServerPosition.To2D(), endpos)
                    where projectOn.SegmentPoint.Distance(endpos) < (int)heroEntry.Width + unit.BoundingRadius
                    select unit).Cast<Obj_AI_Base>().ToList();
        }

        public static float GetUltTravelTime(AIHeroClient source, Vector3? dest = null)
        {
            try
            {
                var targetpos = dest.HasValue ? dest.Value : ObjectManager.Get<Obj_SpawnPoint>().First(x => x.IsEnemy).Position;
                float speed = Listing.spellDataList.First(x => x.championName == source.ChampionName).Speed;
                float delay = Listing.spellDataList.First(x => x.championName == source.ChampionName).Delay;


                float distance = source.ServerPosition.Distance(targetpos);

                float missilespeed = speed;

                if (source.ChampionName.ToLower().Contains("jinx") && distance > 1350)
                {
                    const float accelerationrate = 0.3f; //= (1500f - 1350f) / (2200 - speed), 1 unit = 0.3units/second

                    var acceldifference = distance - 1350f;

                    if (acceldifference > 150f) //it only accelerates 150 units
                        acceldifference = 150f;

                    var difference = distance - 1500f;

                    missilespeed = (1350f * speed + acceldifference * (speed + accelerationrate * acceldifference) +
                        difference * 2200f) / distance;
                }

                return (distance / missilespeed + delay) * 1000;
            }
            catch
            {
                return int.MaxValue;
            }
        }
    }
}
