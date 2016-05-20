namespace SkyLv_Taric
{
    using System;

    using LeagueSharp;
    using LeagueSharp.Common;
    using EloBuddy;
    using EloBuddy.SDK;
    using EloBuddy.SDK.Menu.Values;
    using EloBuddy.SDK.Menu;
    internal class Harass
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

        private static LeagueSharp.Common.Spell Q
        {
            get
            {
                return SkyLv_Taric.Q;
            }
        }
        private static LeagueSharp.Common.Spell W
        {
            get
            {
                return SkyLv_Taric.W;
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



        static Harass()
        {
            Game.OnUpdate += Game_OnUpdate;
        }

        private static void Game_OnUpdate(EventArgs args)
        {
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass) || getKeyBindItem(SkyLv_Taric.Harass, "Taric.HarassActiveT"))
            {
                HarassLogic();
            }
        }

        public static void HarassLogic()
        {
            var PacketCast = getCheckBoxItem(SkyLv_Taric.Misc, "Taric.UsePacketCast");
            var UseQHarass = getCheckBoxItem(SkyLv_Taric.Harass, "Taric.UseQHarass");
            var UseWHarass = getCheckBoxItem(SkyLv_Taric.Harass, "Taric.UseWHarass");
            var UseEHarass = getCheckBoxItem(SkyLv_Taric.Harass, "Taric.UseEHarass");

            var QMiniManaHarass = getSliderItem(SkyLv_Taric.Harass, "Taric.QMiniManaHarass");
            var WMiniManaHarass = getSliderItem(SkyLv_Taric.Harass, "Taric.WMiniManaHarass");
            var EMiniManaHarass = getSliderItem(SkyLv_Taric.Harass, "Taric.EMiniManaHarass");

            var target = TargetSelector.GetTarget(E.Range, DamageType.Physical);

            if (target.IsValidTarget())
            {
                if (UseEHarass && Player.ManaPercent > EMiniManaHarass && E.IsReady() && (!CustomLib.HavePassiveAA() || !getCheckBoxItem(SkyLv_Taric.Harass, "Taric.UseTaricAAPassiveHarass")))
                {
                    if (Player.LSDistance(target) < E.Range)
                    {
                        E.CastIfHitchanceEquals(target, HitChance.VeryHigh, PacketCast);
                        return;
                    }
                }

                if (UseWHarass && Player.ManaPercent > WMiniManaHarass && W.IsReady() && (!CustomLib.HavePassiveAA() || !getCheckBoxItem(SkyLv_Taric.Harass, "Taric.UseTaricAAPassiveHarass")) && (!E.IsReady() || !UseEHarass))
                {
                    W.Cast(Player, PacketCast);
                    return;
                }

                if (UseQHarass && Player.ManaPercent > QMiniManaHarass && Q.IsReady() && (!CustomLib.HavePassiveAA() || !getCheckBoxItem(SkyLv_Taric.Harass, "Taric.UseTaricAAPassiveHarass")) && (!E.IsReady() || !UseEHarass) && (Player.HealthPercent < 100 || (!W.IsReady() || !UseWHarass)))
                {
                    Q.Cast(Player, PacketCast);
                    return;
                }
            }
        }
    }
}
