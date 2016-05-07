namespace SkyLv_Taric
{
    using System;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;
    using EloBuddy;
    using EloBuddy.SDK.Menu;
    using EloBuddy.SDK.Menu.Values;
    internal class OnUpdateFeatures
    {
        #region #GET
        private static AIHeroClient Player
        {
            get
            {
                return SkyLv_Taric.Player;
            }
        }

        private static Spell Q
        {
            get
            {
                return SkyLv_Taric.Q;
            }
        }

        private static Spell W
        {
            get
            {
                return SkyLv_Taric.W;
            }
        }

        private static Spell R
        {
            get
            {
                return SkyLv_Taric.R;
            }
        }
        #endregion

        static OnUpdateFeatures()
        {
            Game.OnUpdate += Game_OnUpdate;
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

        private static void Game_OnUpdate(EventArgs args)
        {
            if (getCheckBoxItem(SkyLv_Taric.Combo, "Taric.UseAutoR"))
            {
                AutoR();
            }

            if (getCheckBoxItem(SkyLv_Taric.Combo, "Taric.UseAutoW"))
            {
                AutoW();
            }

            if (getCheckBoxItem(SkyLv_Taric.Combo, "Taric.UseAutoQ"))
            {
                AutoQ();
            }

            if (getCheckBoxItem(SkyLv_Taric.Combo, "Taric.UseQAlly"))
            {
                AutoQAlly();
            }

            if (getCheckBoxItem(SkyLv_Taric.Combo, "Taric.UseWAlly"))
            {
                AutoWAlly();
            }
        }

        public static void AutoR()
        {
            var PacketCast = getCheckBoxItem(SkyLv_Taric.Misc, "Taric.UsePacketCast");
            var MinimumHpSafeAutoR = getSliderItem(SkyLv_Taric.Combo, "Taric.MinimumHpSafeAutoR");
            var MinimumEnemySafeAutoR = getSliderItem(SkyLv_Taric.Combo, "Taric.MinimumEnemySafeAutoR");

            if (Player.LSIsRecalling()) return;

            if (R.IsReady() && Player.Mana >= R.ManaCost && CustomLib.enemyChampionInPlayerRange(800) >= MinimumEnemySafeAutoR && Player.HealthPercent <= MinimumHpSafeAutoR)
            {
                R.Cast(Player, PacketCast);
            }
        }

        public static void AutoQ()
        {
            var PacketCast = getCheckBoxItem(SkyLv_Taric.Misc, "Taric.UsePacketCast");
            var MinimumHpSafeAutoQ = getSliderItem(SkyLv_Taric.Combo, "Taric.MinimumHpSafeAutoQ");
            var MinimumEnemySafeAutoQ = getSliderItem(SkyLv_Taric.Combo, "Taric.MinimumEnemySafeAutoQ");
            var MinimumStackSafeAutoQ = getSliderItem(SkyLv_Taric.Combo, "Taric.MinimumStackSafeAutoQ");

            if (Player.LSIsRecalling()) return;

            if (Q.IsReady() && Player.Mana >= Q.ManaCost && Q.Instance.Ammo >= MinimumStackSafeAutoQ && CustomLib.enemyChampionInPlayerRange(800) >= MinimumEnemySafeAutoQ && Player.HealthPercent <= MinimumHpSafeAutoQ)
            {
                Q.Cast(Player, PacketCast);
            }
        }

        public static void AutoW()
        {
            var PacketCast = getCheckBoxItem(SkyLv_Taric.Misc, "Taric.UsePacketCast");
            var MinimumHpSafeAutoW = getSliderItem(SkyLv_Taric.Combo, "Taric.MinimumHpSafeAutoW");
            var MinimumEnemySafeAutoW = getSliderItem(SkyLv_Taric.Combo, "Taric.MinimumEnemySafeAutoW");

            if (Player.LSIsRecalling()) return;

            if (W.IsReady() && Player.Mana >= W.ManaCost && CustomLib.enemyChampionInPlayerRange(800) >= MinimumEnemySafeAutoW && Player.HealthPercent <= MinimumHpSafeAutoW)
            {
                W.Cast(Player, PacketCast);
            }
        }

        public static void AutoQAlly()
        {
            var PacketCast = getCheckBoxItem(SkyLv_Taric.Misc, "Taric.UsePacketCast");

            if (Player.LSIsRecalling()) return;

            if (getBoxItem(SkyLv_Taric.Combo, "Taric.UseQAllyMode") == 1 && Q.IsReady() && Player.Mana >= Q.ManaCost)
            {
                foreach (var AllyHeroQ in HeroManager.Allies.Where(x => !x.IsMe && !x.IsDead && Player.LSDistance(x) < Q.Range && 
                Q.Instance.Ammo >= getSliderItem(SkyLv_Taric.Combo, x.NetworkId + "MinimumStacksQAlly") &&
                x.HealthPercent <= getSliderItem(SkyLv_Taric.Combo, x.NetworkId + "MinimumHpQAlly")))
                {
                    if (AllyHeroQ.LSIsValidTarget())
                    {
                        Q.Cast(PacketCast);
                        return;
                    }
                }
            }
        }

        public static void AutoWAlly()
        {
            var PacketCast = getCheckBoxItem(SkyLv_Taric.Misc, "Taric.UsePacketCast");

            if (Player.LSIsRecalling()) return;

            if (getBoxItem(SkyLv_Taric.Combo, "Taric.UseWAllyMode") == 1 && W.IsReady() && Player.Mana >= W.ManaCost)
            {
                var AllyHeroW = HeroManager.Allies.Where(x => !x.IsMe && !x.IsDead && Player.LSDistance(x) <= W.Range && !getCheckBoxItem(SkyLv_Taric.Combo, x.NetworkId + "IncomingDamageWAlly") && x.HealthPercent <= getSliderItem(SkyLv_Taric.Combo, x.NetworkId + "MinimumHpWAlly")).MinOrDefault(t => t.HealthPercent);

                if (AllyHeroW.LSIsValidTarget())
                {
                    W.Cast(AllyHeroW, PacketCast);
                    return;
                }
            }
        }

    }
}
