using System.Collections.Generic;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu.Values;
using SharpDX;

namespace HumanziedBaseUlt
{
    static class Damage
    {
        /// <summary>
        /// list of premades, dmg
        /// </summary>
        /// <param name="target"></param>
        /// <param name="timeLeft"></param>
        /// <param name="dest">Where the destination point is to check collision</param>
        /// <returns></returns>
        public static float GetAioDmg(AIHeroClient target, float timeLeft, Vector3? dest = null)
        {
            float dmg = 0;

            foreach (var ally in EntityManager.Heroes.Allies.Where(x => x.IsValid))
            {
                if (Listing.spellDataList.Any(x => x.championName == ally.ChampionName))
                {
                    string menuid = ally.ChampionName + "/Premade";
                    if (Listing.allyconfig.Get<CheckBox>(menuid).CurrentValue)
                    {
                        float travelTime = Algorithm.GetUltTravelTime(ally, dest);
                        bool canr = ally.Spellbook.GetSpell(SpellSlot.R).IsReady && ally.Mana >= 100;
                        bool intime = travelTime <= timeLeft;

                        if (canr && intime && !Algorithm.GetCollision(ally.ChampionName, dest).Any())
                        {
                            dmg += GetBaseUltSpellDamage(target, ally);
                        }
                    }
                }
            }

            return dmg;
        }

        /// <summary>
        /// Returns ally if one is enough ordered by delay time
        /// </summary>
        /// <param name="target"></param>
        /// <param name="timeLeft"></param>
        /// <param name="estimatedHp"></param>
        /// <param name="fountainReg"></param>
        /// <returns></returns>
        public static AIHeroClient ArePremadesNeeded(AIHeroClient target, float timeLeft, float estimatedHp, float fountainReg)
        {
            Dictionary<AIHeroClient, float> killingAllies = new Dictionary<AIHeroClient, float>();

            foreach (var ally in EntityManager.Heroes.Allies)
            {
                if (Listing.spellDataList.Any(x => x.championName == ally.ChampionName))
                {
                    string menuid = ally.ChampionName + "/Premade";
                    if (Listing.allyconfig.Get<CheckBox>(menuid).CurrentValue)
                    {
                        float travelTime = Algorithm.GetUltTravelTime(ally);
                        bool canr = ally.Spellbook.GetSpell(SpellSlot.R).IsReady && ally.Mana >= 100;
                        float dmg = GetBaseUltSpellDamage(target, ally);
                        bool intime = travelTime <= timeLeft;

                        if (canr && intime && !Algorithm.GetCollision(ally.ChampionName).Any() && dmg > estimatedHp)
                        {
                            float waitRegMSeconds = ((dmg - estimatedHp)/fountainReg)*1000;
                            killingAllies.Add(ally, waitRegMSeconds);
                        }
                    }
                }
            }

            if (killingAllies.Any()) //anyone can kill alone ?
                return killingAllies.OrderByDescending(x => x.Value).First().Key;

            return null;
        }

        public static float GetBaseUltSpellDamage(AIHeroClient target, AIHeroClient source)
        {
            var level = ObjectManager.Player.Spellbook.GetSpell(SpellSlot.R).Level - 1;

            if (source.ChampionName == "Jinx")
            { 
                {
                    var damage = new float[] {250, 350, 450}[level] +
                                 new float[] {25, 30, 35}[level]/100*(target.MaxHealth - target.Health) +
                                 ObjectManager.Player.FlatPhysicalDamageMod;
                    return source.CalculateDamageOnUnit(target, DamageType.Physical, damage);
                }
            }
            if (source.ChampionName == "Ezreal")
            {
                {
                    var damage = new float[] {350, 500, 650}[level] + 0.9f*ObjectManager.Player.FlatMagicDamageMod +
                                 1*ObjectManager.Player.FlatPhysicalDamageMod;
                    return source.CalculateDamageOnUnit(target, DamageType.Magical, damage)*0.7f;
                }
            }
            if (source.ChampionName == "Ashe")
            {
                {
                    var damage = new float[] {250, 425, 600}[level] + 1*ObjectManager.Player.FlatMagicDamageMod;
                    return source.CalculateDamageOnUnit(target, DamageType.Magical, damage);
                }
            }
            if (source.ChampionName == "Draven")
            {
                {
                    var damage = new float[] {175, 275, 375}[level] + 1.1f*ObjectManager.Player.FlatPhysicalDamageMod;
                    return source.CalculateDamageOnUnit(target, DamageType.Physical, damage)*0.7f;
                }
            }

            return 0;
        }
    }
}
