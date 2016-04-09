using System.Collections.Generic;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using Jhin;
using Jhin.Utilities;

namespace Jhin.Managers
{
    public static class UnitManager
    {
        public static List<AIHeroClient> ValidHeroes = new List<AIHeroClient>();
        public static List<AIHeroClient> ValidEnemyHeroes = new List<AIHeroClient>();
        public static List<AIHeroClient> ValidEnemyHeroesInRange = new List<AIHeroClient>();
        public static List<AIHeroClient> ValidAllyHeroes = new List<AIHeroClient>();
        public static List<AIHeroClient> ValidAllyHeroesInRange = new List<AIHeroClient>();

        public static void Initialize()
        {
            Game.OnTick += delegate
            {
                if (FpsBooster.CanBeExecuted(CalculationType.IsValidTarget))
                {
                }
                ValidHeroes.Clear();
                ValidHeroes.AddRange(EntityManager.Heroes.AllHeroes.Where(h => h.IsValidTarget()));
                ValidEnemyHeroes.Clear();
                ValidEnemyHeroes.AddRange(ValidHeroes.Where(h => h.IsEnemy && !h.HasUndyingBuff() && !h.HasIgnite()));
                ValidEnemyHeroesInRange.Clear();
                ValidEnemyHeroesInRange.AddRange(
                    ValidEnemyHeroes.Where(h => AIO.MyHero.IsInRange(h, AIO.CurrentChampion.Range + h.BoundingRadius)));
                ValidAllyHeroes.Clear();
                ValidAllyHeroes.AddRange(ValidHeroes.Where(h => h.IsAlly));
                ValidAllyHeroesInRange.Clear();
                ValidAllyHeroesInRange.AddRange(
                    ValidAllyHeroes.Where(h => AIO.MyHero.IsInRange(h, AIO.CurrentChampion.Range + h.BoundingRadius)));
            };
        }

        public static bool HasIgnite(this AIHeroClient hero)
        {
            return hero.TargetHaveBuff("summonerdot") &&
                   hero.TotalShieldHealth() + hero.HPRegenRate*2 <=
                   AIO.MyHero.GetSummonerSpellDamage(hero, DamageLibrary.SummonerSpells.Ignite);
        }
    }
}