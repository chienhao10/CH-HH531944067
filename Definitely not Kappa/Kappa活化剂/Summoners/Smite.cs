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

                foreach (var mob in
                    ObjectManager.Get<Obj_AI_Minion>()
                        .Where(
                            jmob =>
                            !jmob.HasBuffOfType(BuffType.Invulnerability) && jmob.IsHPBarRendered && jmob.IsMonster && jmob.IsVisible && !jmob.IsDead
                            && !jmob.IsZombie && jmob.IsKillable()
                            && ((SummMenu["drake"].Cast<CheckBox>().CurrentValue && jmob.BaseSkinName == "SRU_Dragon")
                                || (SummMenu["baron"].Cast<CheckBox>().CurrentValue && jmob.BaseSkinName == "SRU_Baron")
                                || (SummMenu["gromp"].Cast<CheckBox>().CurrentValue && jmob.BaseSkinName == "SRU_Gromp")
                                || (SummMenu["krug"].Cast<CheckBox>().CurrentValue && jmob.BaseSkinName == "SRU_Krug")
                                || (SummMenu["razorbeak"].Cast<CheckBox>().CurrentValue && jmob.BaseSkinName == "SRU_Razorbeak")
                                || (SummMenu["crab"].Cast<CheckBox>().CurrentValue && jmob.BaseSkinName == "Sru_Crab")
                                || (SummMenu["murkwolf"].Cast<CheckBox>().CurrentValue && jmob.BaseSkinName == "SRU_Murkwolf")
                                || (SummMenu["blue"].Cast<CheckBox>().CurrentValue && jmob.BaseSkinName == "SRU_Blue")
                                || (SummMenu["red"].Cast<CheckBox>().CurrentValue && jmob.BaseSkinName == "SRU_Red")))
                        .Where(mob => smitemob)
                        .Where(mob => Player.Instance.GetSummonerSpellDamage(mob, DamageLibrary.SummonerSpells.Smite) >= mob.Health))
                {
                    Smite.Cast(mob);
                }

                foreach (var target in
                    ObjectManager.Get<AIHeroClient>()
                        .Where(
                            hero =>
                            hero != null && hero.IsHPBarRendered && !hero.HasBuffOfType(BuffType.Invulnerability) && hero.IsValid && hero.IsVisible
                            && hero.IsEnemy && !hero.IsDead && !hero.IsZombie
                            && !SummMenu["DontSmite" + hero.BaseSkinName].Cast<CheckBox>().CurrentValue)
                        .Where(target => target.IsValidTarget(Smite.Range)))
                {
                    if (smitecombo)
                    {
                        Smite.Cast(target);
                    }

                    if (smiteks && target.IsKillable()
                        && Player.Instance.GetSummonerSpellDamage(target, DamageLibrary.SummonerSpells.Smite) > target.TotalShieldHealth())
                    {
                        Smite.Cast(target);
                    }
                }
            }
        }
    }
}