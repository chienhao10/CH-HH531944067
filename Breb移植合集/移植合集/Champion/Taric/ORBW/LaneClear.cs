namespace SkyLv_Taric
{
    using System;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;
    using EloBuddy;
    using EloBuddy.SDK;
    using EloBuddy.SDK.Menu.Values;
    using EloBuddy.SDK.Menu;
    internal class LaneClear
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



        static LaneClear()
        {
            Game.OnUpdate += Game_OnUpdate;
        }

        private static void Game_OnUpdate(EventArgs args)
        {
            LaneClearLogic();
        }

        public static void LaneClearLogic()
        {
            var PacketCast = getCheckBoxItem(SkyLv_Taric.Misc, "Taric.UsePacketCast");

            var UseQLaneClear = getCheckBoxItem(SkyLv_Taric.LaneClear, "Taric.UseQLaneClear");
            var UseWLaneClear = getCheckBoxItem(SkyLv_Taric.LaneClear, "Taric.UseWLaneClear");
            var UseELaneClear = getCheckBoxItem(SkyLv_Taric.LaneClear, "Taric.UseELaneClear");

            var QMiniManaLaneClear = getSliderItem(SkyLv_Taric.LaneClear, "Taric.QMiniManaLaneClear");
            var WMiniManaLaneClear = getSliderItem(SkyLv_Taric.LaneClear, "Taric.WMiniManaLaneClear");
            var EMiniManaLaneClear = getSliderItem(SkyLv_Taric.LaneClear, "Taric.EMiniManaLaneClear");

            var QMiniMinimionAroundLaneClear = getSliderItem(SkyLv_Taric.LaneClear, "Taric.QMiniMinimionAroundLaneClear");
            var WMiniMinimionAroundLaneClear = getSliderItem(SkyLv_Taric.LaneClear, "Taric.WMiniMinimionAroundLaneClear");
            var EMiniHitLaneClear = getSliderItem(SkyLv_Taric.LaneClear, "Taric.EMiniHitLaneClear");

            var Minion = MinionManager.GetMinions(E.Range, MinionTypes.All, MinionTeam.Enemy).FirstOrDefault();

            if (Minion.IsValidTarget() && (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear) || Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear)))
            {
                if (getCheckBoxItem(SkyLv_Taric.LaneClear, "Taric.SafeLaneClear") && Player.CountEnemiesInRange(1500) > 0) return;

                if (UseELaneClear && Player.ManaPercent > EMiniManaLaneClear && E.IsReady() && (!CustomLib.HavePassiveAA() || !getCheckBoxItem(SkyLv_Taric.LaneClear, "Taric.UseTaricAAPassiveLaneClear")))
                {
                    var allMinionsE = MinionManager.GetMinions(Player.Position, E.Range, MinionTypes.All, MinionTeam.Enemy);

                    if (allMinionsE.Any())
                    {
                        var farmAll = Q.GetLineFarmLocation(allMinionsE, 150f);
                        if (farmAll.MinionsHit >= EMiniHitLaneClear)
                        {
                            E.Cast(farmAll.Position, PacketCast);
                            return;
                        }
                    }
                }

                if (UseWLaneClear && CustomLib.EnemyMinionInPlayerRange(E.Range) >= WMiniMinimionAroundLaneClear && Player.ManaPercent > WMiniManaLaneClear && W.IsReady() && (!CustomLib.HavePassiveAA() || !getCheckBoxItem(SkyLv_Taric.LaneClear, "Taric.UseTaricAAPassiveLaneClear")) && (!E.IsReady() || !UseELaneClear))
                {
                    W.Cast(Player, PacketCast);
                    return;
                }

                if (UseQLaneClear && CustomLib.EnemyMinionInPlayerRange(E.Range) >= QMiniMinimionAroundLaneClear && Player.ManaPercent > QMiniManaLaneClear && Q.IsReady() && (!CustomLib.HavePassiveAA() || !getCheckBoxItem(SkyLv_Taric.LaneClear, "Taric.UseTaricAAPassiveLaneClear")) && (!E.IsReady() || !UseELaneClear) && (Player.HealthPercent < 100 || (!W.IsReady() || !UseWLaneClear)))
                {
                    Q.Cast(Player, PacketCast);
                    return;
                }
            }
        }
    }
}
