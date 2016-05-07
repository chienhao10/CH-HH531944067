namespace SkyLv_Taric
{
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;
    using EloBuddy;
    using EloBuddy.SDK;
    using EloBuddy.SDK.Menu;
    using EloBuddy.SDK.Menu.Values;
    internal class CastOnDash
    {
        #region #GET

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

        private static AIHeroClient Player
        {
            get
            {
                return SkyLv_Taric.Player;
            }
        }

        private static LeagueSharp.Common.Spell E
        {
            get
            {
                return SkyLv_Taric.E;
            }
        }
        #endregion

        static CastOnDash()
        {
            CustomEvents.Unit.OnDash += Unit_OnDash;
        }

        #region On Dash
        static void Unit_OnDash(Obj_AI_Base sender, Dash.DashItem args)
        {
            
            var target = TargetSelector.GetTarget(E.Range * 2, DamageType.Magical);
            var PacketCast = getCheckBoxItem(SkyLv_Taric.Misc, "Taric.UsePacketCast");

            var td = sender as AIHeroClient;

            if (!td.IsEnemy || td == null || Player.IsRecalling())
            {
                return;
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                if (sender.NetworkId == target.NetworkId)
                {
                    if (getCheckBoxItem(SkyLv_Taric.Combo, "Taric.EOnDashendPosition") && E.IsReady() && Player.Distance(args.EndPos) < E.Range)
                    {
                        var delay = (int)(args.EndTick - Game.Time - E.Delay - 0.1f);
                        if (delay > 0)
                        {
                            LeagueSharp.Common.Utility.DelayAction.Add(delay * 1000, () => E.Cast(args.EndPos, PacketCast));
                        }
                        else
                        {
                            E.Cast(args.EndPos, PacketCast);
                        }
                    }
                }

                if (getCheckBoxItem(SkyLv_Taric.Combo, "Taric.UseEFromAlly") && E.IsReady() && Player.Mana >= E.ManaCost)
                {
                    foreach (var AllyHero in ObjectManager.Get<AIHeroClient>().Where(a => !a.IsMe && a.IsDead && a.Team == ObjectManager.Player.Team && Player.Distance(a) < 1600 && (a.HasBuff("TaricWAllyBuff") || a.HasBuff("TaricW"))))
                    {
                        var Allytarget = ObjectManager.Get<AIHeroClient>().Where(t => !t.IsDead && t.Team != ObjectManager.Player.Team && AllyHero.Distance(args.EndPos) < E.Range).FirstOrDefault();

                        if (sender.NetworkId == Allytarget.NetworkId)
                        {
                            if (getCheckBoxItem(SkyLv_Taric.Combo, AllyHero.NetworkId + "TargetDashEPEComboFromAlly"))
                            {
                                var delay = (int)(args.EndTick - Game.Time - E.Delay - 0.1f);
                                if (delay > 0)
                                {
                                    LeagueSharp.Common.Utility.DelayAction.Add(delay * 1000, () => E.Cast(args.EndPos, PacketCast));
                                }
                                else
                                {
                                    E.Cast(args.EndPos, PacketCast);
                                }
                            }
                        }
                    }
                }
            }
        }
        #endregion
    }
}
