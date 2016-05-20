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
                && (SummMenu["EnableactiveSmite"].Cast<KeyBind>().CurrentValue
                    || SummMenu["EnableSmite"].Cast<KeyBind>().CurrentValue))
            {
                var smitemob = SummMenu["smitemob"].Cast<CheckBox>().CurrentValue && Smite.IsReady();
                var smitecombo = SummMenu["smitecombo"].Cast<CheckBox>().CurrentValue && Smite.IsReady()
                                 && Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo);
                var smiteks = SummMenu["smiteks"].Cast<CheckBox>().CurrentValue && Smite.IsReady();
                if (smitemob)
                {
                    if (Smite.Handle.Ammo == 1 && SummMenu["smitesavej"].Cast<CheckBox>().CurrentValue)
                    {
                        return;
                    }
                    var jmobs =
                        EntityManager.MinionsAndMonsters.GetJungleMonsters()
                            .Where(
                                j =>
                                (SRJunglemobs.Contains(j.BaseSkinName) || TTJunglemobs.Contains(j.BaseSkinName)) && j.IsKillable()
                                && j.IsValidTarget(Smite.Range));
                    foreach (var jmob in jmobs)
                    {
                        if (jmob != null && SummMenu[jmob.BaseSkinName].Cast<CheckBox>().CurrentValue)
                        {
                            var predh = Helpers.SmiteDamage(jmob) >= Prediction.Health.GetPrediction(jmob, Smite.CastDelay * 1000);
                            var hks = Helpers.SmiteDamage(jmob) >= jmob.TotalShieldHealth();
                            if (predh || hks)
                            {
                                Smite.Cast(jmob);
                            }
                        }
                    }
                }

                if (Player.Spells.FirstOrDefault(o => o.SData.Name.ToLower().Contains("summonersmiteduel")) == null)
                {
                    if (Smite.Handle.Ammo == 1 && SummMenu["smitesaveh"].Cast<CheckBox>().CurrentValue)
                    {
                        return;
                    }
                    if (smiteks)
                    {
                        foreach (var enemy in EntityManager.Heroes.Enemies.Where(e => e.IsKillable()))
                        {
                            if (enemy != null && enemy.IsValidTarget(Smite.Range))
                            {
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
            if (Smite.Handle.Ammo == 1 && SummMenu["smitesaveh"].Cast<CheckBox>().CurrentValue)
            {
                return;
            }
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