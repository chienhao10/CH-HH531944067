namespace SkyLv_Taric
{
    using EloBuddy;
    using EloBuddy.SDK.Menu;
    using EloBuddy.SDK.Menu.Values;
    using LeagueSharp;
    using LeagueSharp.Common;


    internal class AntiGapCLoser
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

        private static Spell E
        {
            get
            {
                return SkyLv_Taric.E;
            }
        }
        #endregion

        static AntiGapCLoser()
        {
            AntiGapcloser.OnEnemyGapcloser += AntiGapcloser_OnEnemyGapcloser;
        }

        public static void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            var MinimumHpEGapCloser = getSliderItem(SkyLv_Taric.Combo, "Taric.MinimumHpEGapCloser");
            var MinimumEnemyEGapCloser = getSliderItem(SkyLv_Taric.Combo, "Taric.MinimumEnemyEGapCloser");
            var UseAutoEGapCloser = getCheckBoxItem(SkyLv_Taric.Combo, "Taric.UseAutoEGapCloser");
            var PacketCast = getCheckBoxItem(SkyLv_Taric.Misc, "Taric.UsePacketCast");

            if (Player.LSIsRecalling()) return;

            if (UseAutoEGapCloser && gapcloser.End.LSDistance(Player.ServerPosition) < E.Range && Player.HealthPercent <= MinimumHpEGapCloser && CustomLib.enemyChampionInPlayerRange(800) >= MinimumEnemyEGapCloser)
            {
                E.Cast(gapcloser.End, PacketCast);
            }
        }
    }
}
