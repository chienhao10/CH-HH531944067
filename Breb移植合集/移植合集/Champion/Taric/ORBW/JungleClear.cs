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
    internal class JungleClear
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



        static JungleClear()
        {
            Game.OnUpdate += Game_OnUpdate;
        }

        private static void Game_OnUpdate(EventArgs args)
        {
            JungleClearLogic();
        }

        public static void JungleClearLogic()
        {
            var PacketCast = getCheckBoxItem(SkyLv_Taric.Misc, "Taric.UsePacketCast");
            var useQ = getCheckBoxItem(SkyLv_Taric.JungleClear, "Taric.UseQJungleClear");
            var useW = getCheckBoxItem(SkyLv_Taric.JungleClear, "Taric.UseWJungleClear");
            var useE = getCheckBoxItem(SkyLv_Taric.JungleClear, "Taric.UseEJungleClear");

            var MiniManaQ = getSliderItem(SkyLv_Taric.JungleClear, "Taric.QMiniManaJungleClear");
            var MiniManaW = getSliderItem(SkyLv_Taric.JungleClear, "Taric.WMiniManaJungleClear");
            var MiniManaE = getSliderItem(SkyLv_Taric.JungleClear, "Taric.EMiniManaJungleClear");

            var MinionN = MinionManager.GetMinions(E.Range, MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth).FirstOrDefault();

            if (MinionN.IsValidTarget() && (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear) || Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear)))
            {
                if (getCheckBoxItem(SkyLv_Taric.Misc, "Taric.SafeJungleClear") && Player.CountEnemiesInRange(1500) > 0) return;

                if (useE && E.IsReady() && Player.ManaPercent > MiniManaE)
                {
                    if (getCheckBoxItem(SkyLv_Taric.Misc, "Taric.SpellOnlyBigMonster"))
                    {
                        foreach (var target in ObjectManager.Get<Obj_AI_Base>().Where(target => SkyLv_Taric.Monsters.Contains(target.BaseSkinName) && !target.IsDead))
                        {
                            if (target.IsValidTarget() && (!CustomLib.HavePassiveAA() || !getCheckBoxItem(SkyLv_Taric.Misc, "Taric.UseTaricAAPassiveJungleClear")))
                                E.CastIfHitchanceEquals(target, HitChance.VeryHigh, PacketCast);
                        }
                    }
                    else if(!getCheckBoxItem(SkyLv_Taric.Misc, "Taric.SpellOnlyBigMonster") && MinionN.IsValidTarget() && (!CustomLib.HavePassiveAA() || !getCheckBoxItem(SkyLv_Taric.Misc, "Taric.UseTaricAAPassiveJungleClear")))
                        E.CastIfHitchanceEquals(MinionN, HitChance.VeryHigh, PacketCast);
                }

                if (useW && W.IsReady() && Player.ManaPercent > MiniManaW && (!E.IsReady() || !useE))
                {
                    if (getCheckBoxItem(SkyLv_Taric.Misc, "Taric.SpellOnlyBigMonster"))
                    {
                        foreach (var target in ObjectManager.Get<Obj_AI_Base>().Where(target => SkyLv_Taric.Monsters.Contains(target.BaseSkinName) && !target.IsDead))
                        {
                            if (target.IsValidTarget() && (!CustomLib.HavePassiveAA() || !getCheckBoxItem(SkyLv_Taric.Misc, "Taric.UseTaricAAPassiveJungleClear")))
                                W.Cast(Player, PacketCast);
                        }
                    }
                    else if (!getCheckBoxItem(SkyLv_Taric.Misc, "Taric.SpellOnlyBigMonster") && MinionN.IsValidTarget() && (!CustomLib.HavePassiveAA() || !getCheckBoxItem(SkyLv_Taric.Misc, "Taric.UseTaricAAPassiveJungleClear")))
                        W.Cast(Player, PacketCast);
                }

                if (useQ && Q.IsReady() && Player.ManaPercent > MiniManaQ && (!E.IsReady() || !useE) && (!W.IsReady() || !useW))
                {
                    if (getCheckBoxItem(SkyLv_Taric.Misc, "Taric.SpellOnlyBigMonster"))
                    {
                        foreach (var target in ObjectManager.Get<Obj_AI_Base>().Where(target => SkyLv_Taric.Monsters.Contains(target.BaseSkinName) && !target.IsDead))
                        {
                            if (target.IsValidTarget() && (!CustomLib.HavePassiveAA() || !getCheckBoxItem(SkyLv_Taric.Misc, "Taric.UseTaricAAPassiveJungleClear")))
                                Q.Cast(Player, PacketCast);
                        }
                    }
                    else if (!getCheckBoxItem(SkyLv_Taric.Misc, "Taric.SpellOnlyBigMonster") && MinionN.IsValidTarget() && (!CustomLib.HavePassiveAA() || !getCheckBoxItem(SkyLv_Taric.Misc, "Taric.UseTaricAAPassiveJungleClear")))
                        Q.Cast(Player, PacketCast);
                }
            }
        }
    }
}
