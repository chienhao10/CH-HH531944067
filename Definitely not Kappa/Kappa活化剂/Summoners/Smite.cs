namespace KappaUtility.Summoners
{
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

                foreach (var jmob in EntityManager.MinionsAndMonsters.GetJungleMonsters())
                {
                    if (jmob != null && Junglemobs.Contains(jmob.BaseSkinName) && smitemob)
                    {
                        if (jmob.IsHPBarRendered && jmob.IsKillable() && SummMenu[jmob.BaseSkinName].Cast<CheckBox>().CurrentValue)
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

                foreach (var target in
                    EntityManager.Heroes.Enemies.Where(
                        hero =>
                        hero != null && hero.IsHPBarRendered && !hero.HasBuffOfType(BuffType.Invulnerability) && hero.IsValid && hero.IsVisible
                        && hero.IsEnemy && !hero.IsDead && !hero.IsZombie && !SummMenu["DontSmite" + hero.BaseSkinName].Cast<CheckBox>().CurrentValue)
                        .Where(target => target.IsValidTarget(Smite.Range)))
                {
                    if (smitecombo)
                    {
                        Smite.Cast(target);
                    }

                    if (smiteks && target.IsKillable()
                        && Player.Instance.GetSummonerSpellDamage(target, DamageLibrary.SummonerSpells.Smite) >= target.TotalShieldHealth())
                    {
                        Smite.Cast(target);
                    }
                }
            }
        }
    }
}