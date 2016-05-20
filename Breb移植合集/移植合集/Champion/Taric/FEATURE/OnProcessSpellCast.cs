namespace SkyLv_Taric
{
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;
    using EloBuddy;
    using EloBuddy.SDK;
    using EloBuddy.SDK.Menu;
    using EloBuddy.SDK.Menu.Values;
    internal class OnProcessSpellCast
    {
        #region #GET
        private static AIHeroClient Player
        {
            get
            {
                return SkyLv_Taric.Player;
            }
        }

        private static LeagueSharp.Common.Spell W
        {
            get
            {
                return SkyLv_Taric.W;
            }
        }

        public static bool getCheckBoxItem(Menu m, string item)
        {
            return m[item].Cast<CheckBox>().CurrentValue;
        }

        public static int getSliderItem(Menu m, string item)
        {
            return m[item].Cast<Slider>().CurrentValue;
        }

        public static bool getKeyBindItem(Menu m, string item)
        {
            return m[item].Cast<KeyBind>().CurrentValue;
        }

        public static int getBoxItem(Menu m, string item)
        {
            return m[item].Cast<ComboBox>().CurrentValue;
        }
        #endregion

        static OnProcessSpellCast()
        {
            Obj_AI_Base.OnProcessSpellCast += Obj_AI_Base_OnProcessSpellCast;
        }

        public static void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {

            if (Player.LSIsRecalling()) return;

            if ((sender.IsValid<AIHeroClient>() || sender.IsValid<Obj_AI_Turret>()) && sender.IsEnemy)
            {
                var PacketCast = getCheckBoxItem(SkyLv_Taric.Misc, "Taric.UsePacketCast");


                if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo) && getCheckBoxItem(SkyLv_Taric.Combo, "Taric.UseWIncomingDamageCombo") && W.IsReady() && W.ManaCost <= Player.Mana)
                {
                    if (Player.LSDistance(args.End) <= Player.BoundingRadius && sender.LSGetSpellDamage(Player, args.SData.Name.ToString()) > 0)
                    {
                        W.CastOnUnit(Player, PacketCast);
                    }
                }

                if (getCheckBoxItem(SkyLv_Taric.Combo, "Taric.UseWAlly"))
                {

                    switch (getBoxItem(SkyLv_Taric.Combo, "Taric.UseWAllyMode"))
                    {

                        case 0:
                            {
                                if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
                                {
                                    if (W.IsReady() && Player.Mana >= W.ManaCost)
                                    {
                                        foreach (var ally in HeroManager.Allies.Where(x => x.IsValidTarget(W.Range, false) && !x.IsMe && Player.LSDistance(x) <= W.Range && getCheckBoxItem(SkyLv_Taric.Combo, x.NetworkId + "IncomingDamageWAlly") && x.HealthPercent <= getSliderItem(SkyLv_Taric.Combo, x.NetworkId + "MinimumHpWAlly")))
                                        {
                                            foreach (var SendingUnit in HeroManager.Enemies.Where(x => x.NetworkId == sender.NetworkId))
                                            {
                                                if (ally.LSDistance(args.End) <= ally.BoundingRadius && SendingUnit.LSGetSpellDamage(ally, args.SData.Name.ToString()) > 0)
                                                {
                                                    W.CastOnUnit(ally, PacketCast);
                                                }
                                            }
                                        }
                                    }
                                }
                                break;
                            }

                        case 1:
                            {
                                if (W.IsReady() && Player.Mana >= W.ManaCost)
                                {
                                    foreach (var ally in HeroManager.Allies.Where(x => x.IsValidTarget(W.Range, false) && !x.IsMe && Player.LSDistance(x) <= W.Range && getCheckBoxItem(SkyLv_Taric.Combo, x.NetworkId + "IncomingDamageWAlly") && x.HealthPercent <= getSliderItem(SkyLv_Taric.Combo, x.NetworkId + "MinimumHpWAlly")))
                                    {
                                        foreach (var SendingUnit in HeroManager.Enemies.Where(x => x.NetworkId == sender.NetworkId))
                                        {
                                            if (ally.LSDistance(args.End) <= ally.BoundingRadius)
                                            {
                                                W.CastOnUnit(ally, PacketCast);
                                            }
                                        }
                                    }
                                }
                                break;
                            }
                    }
                }
            }
        }
    }
}
