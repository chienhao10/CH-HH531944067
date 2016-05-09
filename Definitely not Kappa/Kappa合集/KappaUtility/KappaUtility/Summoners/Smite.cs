namespace KappaUtility.Summoners
{
    using System;
    using System.Linq;

    using EloBuddy;
    using EloBuddy.SDK;
    using EloBuddy.SDK.Menu.Values;

    using Common;

    internal class Smite : Spells
    {
        public static void Smiteopepi()
        {
            if (Smite != null
                && (SummMenu[Player.Instance.ChampionName + "EnableactiveSmite"].Cast<KeyBind>().CurrentValue
                    || SummMenu[Player.Instance.ChampionName + "EnableSmite"].Cast<KeyBind>().CurrentValue))
            {
                var smitemob = SummMenu["smitemob"].Cast<CheckBox>().CurrentValue && Smite.IsReady();
                var smitecombo = SummMenu["smitecombo"].Cast<CheckBox>().CurrentValue && Smite.IsReady()
                                 && Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo);
                var smiteks = SummMenu["smiteks"].Cast<CheckBox>().CurrentValue && Smite.IsReady();

                if (smitemob)
                {
                    foreach (var jmob in EntityManager.MinionsAndMonsters.GetJungleMonsters().Where(e => Junglemobs.Contains(e.BaseSkinName)))
                    {
                        if (jmob != null && jmob.IsValidTarget(Smite.Range) && SummMenu[jmob.BaseSkinName].Cast<CheckBox>().CurrentValue
                            && jmob.IsKillable())
                        {
                            var predhealth = Player.Instance.GetSummonerSpellDamage(jmob, DamageLibrary.SummonerSpells.Smite)
                                             >= Prediction.Health.GetPrediction(jmob, Smite.CastDelay * 1000);
                            var health = Player.Instance.GetSummonerSpellDamage(jmob, DamageLibrary.SummonerSpells.Smite) >= jmob.Health;
                            if (predhealth || health)
                            {
                                Smite.Cast(jmob);
                            }
                        }
                    }
                }

                if (Player.Spells.FirstOrDefault(o => o.SData.Name.ToLower().Contains("summonersmiteduel")) == null)
                {
                    if (smiteks)
                    {
                        foreach (var enemy in EntityManager.Heroes.Enemies.Where(e => e.IsKillable()))
                        {
                            if (enemy != null && enemy.IsValidTarget(Smite.Range))
                            {
                                Console.WriteLine(Helpers.SmiteDamage(enemy));
                                var predh = Helpers.SmiteDamage(enemy) >= Prediction.Health.GetPrediction(enemy, Smite.CastDelay * 1000);
                                var hks = Helpers.SmiteDamage(enemy) >= enemy.TotalShieldHealth();
                                if (predh || hks)
                                {
                                    Smite.Cast(enemy);
                                }
                            }
                        }
                    }

                    if (smitecombo)
                    {
                        var target = TargetSelector.GetTarget(Smite.Range, DamageType.True);
                        if (target != null && target.IsValidTarget(Smite.Range) && target.IsKillable())
                        {
                            Smite.Cast(target);
                        }
                    }
                }
            }
        }

        public static void Orbwalker_OnPostAttack(AttackableUnit target, EventArgs args)
        {
            var smitecombo = SummMenu["smitecombo"].Cast<CheckBox>().CurrentValue && Smite.IsReady()
                             && Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo);
            if (smitecombo && Player.Spells.FirstOrDefault(o => o.SData.Name.ToLower().Contains("summonersmiteduel")) != null)
            {
                var client = target as AIHeroClient;
                if (client != null)
                {
                    Smite.Cast(client);
                }
            }
        }
    }
}