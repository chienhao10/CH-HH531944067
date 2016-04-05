namespace KappaUtility.Summoners
{
    using System.Linq;

    using EloBuddy;
    using EloBuddy.SDK;
    using EloBuddy.SDK.Menu.Values;

    internal class Smite
    {
        public static void Smiteopepi()
        {
            if (Spells.Smite != null
                && (Spells.SummMenu[Player.Instance.ChampionName + "EnableactiveSmite"].Cast<KeyBind>().CurrentValue
                    || Spells.SummMenu[Player.Instance.ChampionName + "EnableSmite"].Cast<KeyBind>().CurrentValue))
            {
                var smitemob = Spells.SummMenu["smitemob"].Cast<CheckBox>().CurrentValue && Spells.Smite.IsReady();
                var smitecombo = Spells.SummMenu["smitecombo"].Cast<CheckBox>().CurrentValue && Spells.Smite.IsReady();
                var smiteks = Spells.SummMenu["smiteks"].Cast<CheckBox>().CurrentValue && Spells.Smite.IsReady();

                foreach (var mob in
                    ObjectManager.Get<Obj_AI_Minion>()
                        .Where(
                            jmob =>
                            !jmob.HasBuffOfType(BuffType.Invulnerability) && jmob.IsHPBarRendered && jmob.IsMonster
                            && jmob.IsVisible && !jmob.IsDead && !jmob.IsZombie
                            && ((Spells.SummMenu["drake"].Cast<CheckBox>().CurrentValue
                                 && jmob.BaseSkinName == "SRU_Dragon")
                                || (Spells.SummMenu["baron"].Cast<CheckBox>().CurrentValue
                                    && jmob.BaseSkinName == "SRU_Baron")
                                || (Spells.SummMenu["gromp"].Cast<CheckBox>().CurrentValue
                                    && jmob.BaseSkinName == "SRU_Gromp")
                                || (Spells.SummMenu["krug"].Cast<CheckBox>().CurrentValue
                                    && jmob.BaseSkinName == "SRU_Krug")
                                || (Spells.SummMenu["razorbeak"].Cast<CheckBox>().CurrentValue
                                    && jmob.BaseSkinName == "SRU_Razorbeak")
                                || (Spells.SummMenu["crab"].Cast<CheckBox>().CurrentValue
                                    && jmob.BaseSkinName == "Sru_Crab")
                                || (Spells.SummMenu["murkwolf"].Cast<CheckBox>().CurrentValue
                                    && jmob.BaseSkinName == "SRU_Murkwolf")
                                || (Spells.SummMenu["blue"].Cast<CheckBox>().CurrentValue
                                    && jmob.BaseSkinName == "SRU_Blue")
                                || (Spells.SummMenu["red"].Cast<CheckBox>().CurrentValue
                                    && jmob.BaseSkinName == "SRU_Red")))
                        .Where(mob => smitemob)
                        .Where(
                            mob =>
                            Player.Instance.GetSummonerSpellDamage(mob, DamageLibrary.SummonerSpells.Smite)
                            >= mob.Health))
                {
                    Spells.Smite.Cast(mob);
                }

                foreach (var target in
                    ObjectManager.Get<AIHeroClient>()
                        .Where(
                            hero =>
                            hero != null && hero.IsHPBarRendered && !hero.HasBuffOfType(BuffType.Invulnerability)
                            && hero.IsValid && hero.IsVisible && hero.IsEnemy && !hero.IsDead && !hero.IsZombie
                            && !Spells.SummMenu["DontSmite" + hero.BaseSkinName].Cast<CheckBox>().CurrentValue)
                        .Where(target => target.IsValidTarget(Spells.Smite.Range)))
                {
                    if (smitecombo)
                    {
                        Spells.Smite.Cast(target);
                    }

                    if (smiteks
                        && Player.Instance.GetSummonerSpellDamage(target, DamageLibrary.SummonerSpells.Smite)
                        > target.TotalShieldHealth())
                    {
                        Spells.Smite.Cast(target);
                    }
                }
            }
        }
    }
}