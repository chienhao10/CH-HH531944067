using System.Collections.Generic;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu.Values;
using HumanziedBaseUlt.DamageCalculation;
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
        public static float GetAioDmg(AIHeroClient target, float timeLeft, Vector3 dest)
        {
            float dmg = 0;
            bool elobuddyDamageMethod = Listing.MiscMenu.Get<Slider>("damageCalcMethod").CurrentValue == 0;

            foreach (var ally in EntityManager.Heroes.Allies.Where(x => x.IsValid))
            {
                if (Listing.UltSpellDataList.Any(x => x.Key == ally.ChampionName))
                {
                    string menuid = ally.ChampionName + "/Premade";
                    if (Listing.allyconfig.Get<CheckBox>(menuid).CurrentValue)
                    {
                        float travelTime = Algorithm.GetUltTravelTime(ally, dest);
                        bool canr = ally.Spellbook.GetSpell(SpellSlot.R).IsReady && ally.Mana >= 100;
                        bool intime = travelTime <= timeLeft;

                        if (canr && intime && !Algorithm.GetCollision(ally.ChampionName, dest).Any())
                        {
                            dmg += elobuddyDamageMethod ? GetBaseUltSpellDamage(target, ally) : 
                                (float)GetBaseUltSpellDamageAdvanced(target, ally);
                        }
                    }
                }
            }

            return dmg;
        }

        static Vector3 enemySpawn
        {
            get { return ObjectManager.Get<Obj_SpawnPoint>().First(x => x.IsEnemy).Position; }
        }

        public static float GetBaseUltSpellDamage(AIHeroClient target, AIHeroClient source)
        {
            var level = ObjectManager.Player.Spellbook.GetSpell(SpellSlot.R).Level - 1;
            float dmg = 0;

            if (source.ChampionName == "Jinx")
            { 
                {
                    var damage = new float[] {250, 350, 450}[level] +
                                 new float[] {25, 30, 35}[level]/100*(target.MaxHealth - target.Health) +
                                 ObjectManager.Player.FlatPhysicalDamageMod;
                    dmg = source.CalculateDamageOnUnit(target, DamageType.Physical, damage);
                }
            }
            if (source.ChampionName == "Ezreal")
            {
                {
                    var damage = new float[] {350, 500, 650}[level] + 0.9f*ObjectManager.Player.FlatMagicDamageMod +
                                 1*ObjectManager.Player.FlatPhysicalDamageMod;
                    dmg = source.CalculateDamageOnUnit(target, DamageType.Magical, damage)*0.7f;
                }
            }
            if (source.ChampionName == "Ashe")
            {
                {
                    var damage = new float[] {250, 425, 600}[level] + 1*ObjectManager.Player.FlatMagicDamageMod;
                    dmg = source.CalculateDamageOnUnit(target, DamageType.Magical, damage);
                }
            }
            if (source.ChampionName == "Draven")
            {
                {
                    var damage = new float[] {175, 275, 375}[level] + 1.1f*ObjectManager.Player.FlatPhysicalDamageMod;
                    dmg = source.CalculateDamageOnUnit(target, DamageType.Physical, damage)*0.7f;
                }
            }

            return dmg*0.7f;
        }

        public static double GetBaseUltSpellDamageAdvanced(AIHeroClient target, AIHeroClient source)
        {
            float damageMultiplicator = Listing.UltSpellDataList[source.ChampionName].DamageMultiplicator;
            int spellStage = Listing.UltSpellDataList[source.ChampionName].SpellStage;

            return source.GetSpellDamage(target, SpellSlot.R, spellStage)*damageMultiplicator;
        }
    }
}
