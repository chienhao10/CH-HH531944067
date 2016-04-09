using EloBuddy;
using EloBuddy.SDK;
using System;

namespace AkaYasuo
{
    class DamageManager
    {
        public static double _GetQDmg(Obj_AI_Base target)
        {
            var dmgItem = 0d;
            if (Program.Sheen.IsOwned() && (Program.Sheen.IsReady() || Player.HasBuff("Sheen")))
            {
                dmgItem = Variables._Player.BaseAttackDamage;
            }
            if (Program.Trinity.IsOwned() && (Program.Trinity.IsReady() || Player.HasBuff("Sheen")))
            {
                dmgItem = Variables._Player.BaseAttackDamage * 2;
            }
            var k = 1d;
            var reduction = 0d;
            var dmg = Variables._Player.TotalAttackDamage * (Variables._Player.Crit >= 0.85f ? (Item.HasItem(3031) ? 1.875 : 1.5) : 1)
                      + dmgItem;
            if (Item.HasItem(3153))
            {
                var dmgBotrk = Math.Max(0.08 * target.Health, 10);
                if (target.IsValid<Obj_AI_Minion>())
                {
                    dmgBotrk = Math.Min(dmgBotrk, 60);
                }
                dmg += dmgBotrk;
            }
            if (target.IsValid<AIHeroClient>())
            {
                var hero = (AIHeroClient)target;
                if (Item.HasItem(3047, hero))
                {
                    k *= 0.9d;
                }
                if (hero.ChampionName == "Fizz")
                {
                    reduction += hero.Level > 15
                                     ? 14
                                     : (hero.Level > 12
                                            ? 12
                                            : (hero.Level > 9 ? 10 : (hero.Level > 6 ? 8 : (hero.Level > 3 ? 6 : 4))));
                }
                var mastery = hero.Masteries.MinOrDefault(m => m.Page == MasteryPage.Defense && m.Id == 65);
                if (mastery != null && mastery.Points > 0)
                {
                    reduction += 1 * mastery.Points;
                }
            }
            return Variables._Player.CalculateDamageOnUnit(target, DamageType.Physical, 20 * Program.Q.Level + (float)(dmg - reduction) * (float)k)
                   + (Variables._Player.GetBuffCount("ItemStatikShankCharge") == 100
                          ? Variables._Player.CalculateDamageOnUnit(
                              target,
                              DamageType.Magical,
                              100 * (float)(Variables._Player.Crit >= 0.85f ? (Item.HasItem(3031) ? 2.25 : 1.8) : 1))
                          : 0);
        }

        public static double _GetEDmg(Obj_AI_Base target)
        {
            return Variables._Player.CalculateDamageOnUnit(
                target,
                DamageType.Magical,
                (float)((50 + 20 * Program.E.Level) * (1 + Math.Max(0, Variables._Player.GetBuffCount("YasuoDashScalar") * 0.25))
                + 0.6 * Variables._Player.FlatMagicDamageMod));
        }
    }
}
