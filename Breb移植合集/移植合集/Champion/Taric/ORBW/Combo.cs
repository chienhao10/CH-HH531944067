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
    internal class Combo
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


        static Combo()
        {            
            Game.OnUpdate += Game_OnUpdate;
        }

        private static void Game_OnUpdate(EventArgs args)
        {
            ComboLogic();
        }

        public static void ComboLogic()
        {
            var PacketCast = getCheckBoxItem(SkyLv_Taric.Misc, "Taric.UsePacketCast");
            var useQ = getCheckBoxItem(SkyLv_Taric.Combo, "Taric.UseQCombo");
            var useW = getCheckBoxItem(SkyLv_Taric.Combo, "Taric.UseWCombo");
            var useE = getCheckBoxItem(SkyLv_Taric.Combo, "Taric.UseECombo");

            var MinimumStackSelfQCombo = getSliderItem(SkyLv_Taric.Combo, "Taric.MinimumStackSelfQCombo");
            var MinimumHpPercentSelfQCombo = getSliderItem(SkyLv_Taric.Combo, "Taric.MinimumHpPercentSelfQCombo");

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
               var target = TargetSelector.GetTarget(E.Range, DamageType.Physical);

                if (target.IsValidTarget(E.Range))
                {
                    if (useE && E.IsReady() && (!CustomLib.HavePassiveAA() || !getCheckBoxItem(SkyLv_Taric.Combo, "Taric.UseTaricAAPassiveCombo") || Player.LSDistance(target) > Orbwalking.GetRealAutoAttackRange(Player)))
                    {
                        if (Player.LSDistance(target) < E.Range)
                        {
                            E.CastIfHitchanceEquals(target, HitChance.VeryHigh, PacketCast);
                            return;
                        }
                    }

                    if (target.IsValidTarget(Orbwalking.GetRealAutoAttackRange(Player)))
                    {
                        if (!getCheckBoxItem(SkyLv_Taric.Combo, "Taric.UseWIncomingDamageCombo") && useW && W.IsReady() && (!CustomLib.HavePassiveAA() || !getCheckBoxItem(SkyLv_Taric.Combo, "Taric.UseTaricAAPassiveCombo")) && (!E.IsReady() || !useE))
                        {
                            W.Cast(Player, PacketCast);
                            return;
                        }

                        if (useQ && Q.IsReady() && Q.Instance.Ammo >= MinimumStackSelfQCombo && Player.HealthPercent <= MinimumHpPercentSelfQCombo && (!CustomLib.HavePassiveAA() || !getCheckBoxItem(SkyLv_Taric.Combo, "Taric.UseTaricAAPassiveCombo")) && (!E.IsReady() || !useE) && (Player.HealthPercent < 100 || (!W.IsReady() || !useW)))
                        {
                            Q.Cast(Player, PacketCast);
                            return;
                        }
                    }
                }

                #region Ally E
                if (getCheckBoxItem(SkyLv_Taric.Combo, "Taric.UseEFromAlly") && E.IsReady() && Player.Mana >= E.ManaCost)
                {
                    foreach (var AllyHeroE in ObjectManager.Get<AIHeroClient>().Where(a => !a.IsMe && !a.IsDead && a.Team == ObjectManager.Player.Team && Player.LSDistance(a) < 1600 && (a.HasBuff("TaricWAllyBuff") || a.HasBuff("TaricW"))))
                    {
                        var Allytarget = ObjectManager.Get<AIHeroClient>().Where(t => !t.IsDead && t.Team != ObjectManager.Player.Team && AllyHeroE.LSDistance(t) < E.Range).FirstOrDefault();

                        if (Allytarget.IsValidTarget())
                        {
                            if (getCheckBoxItem(SkyLv_Taric.Combo, AllyHeroE.NetworkId + "AllyCCEComboFromAlly") && (AllyHeroE.IsCharmed || AllyHeroE.IsStunned || AllyHeroE.IsRooted || !AllyHeroE.CanAttack))
                            {
                                E.Cast(Allytarget.ServerPosition, PacketCast);
                                return;
                            }

                            if (getCheckBoxItem(SkyLv_Taric.Combo, AllyHeroE.NetworkId + "TargetCCEComboFromAlly") && (Allytarget.IsCharmed || Allytarget.IsStunned || Allytarget.IsRooted || !Allytarget.CanAttack))
                            {
                                E.Cast(Allytarget.ServerPosition, PacketCast);
                                return;
                            }

                            if (getCheckBoxItem(SkyLv_Taric.Combo, AllyHeroE.NetworkId + "AlwaysComboFromAlly"))
                            {
                                E.Cast(Allytarget.ServerPosition, PacketCast);
                                return;
                            }
                        }
                    }
                }
                #endregion

                #region UseQAlly
                if (getCheckBoxItem(SkyLv_Taric.Combo, "Taric.UseQAlly") && getBoxItem(SkyLv_Taric.Combo, "Taric.UseQAllyMode") == 0)
                {
                    if (Q.IsReady() && Player.Mana >= Q.ManaCost)
                    {
                        foreach (var AllyHeroQ in HeroManager.Allies.Where(x => !x.IsMe && !x.IsDead && Player.LSDistance(x) < Q.Range &&
                        Q.Instance.Ammo >= getSliderItem(SkyLv_Taric.Combo, x.NetworkId + "MinimumStacksQAlly") &&
                        x.HealthPercent <= getSliderItem(SkyLv_Taric.Combo, x.NetworkId + "MinimumHpQAlly")))
                        {
                            if (AllyHeroQ.IsValidTarget())
                            {
                                Q.Cast(PacketCast);
                                return;
                            }
                        }
                    }
                }
                #endregion

                #region UseWAlly
                if (getCheckBoxItem(SkyLv_Taric.Combo, "Taric.UseWAlly") && getBoxItem(SkyLv_Taric.Combo, "Taric.UseWAllyMode") == 0)
                {
                    if (W.IsReady() && Player.Mana >= W.ManaCost)
                    {
                        var AllyHeroW = HeroManager.Allies.Where(x => !x.IsMe && !x.IsDead && Player.LSDistance(x) <= W.Range &&
                        !getCheckBoxItem(SkyLv_Taric.Combo, x.NetworkId + "IncomingDamageWAlly") &&
                        x.HealthPercent <= getSliderItem(SkyLv_Taric.Combo, x.NetworkId + "MinimumHpWAlly")).MinOrDefault(t => t.HealthPercent);

                        if (AllyHeroW.IsValidTarget())
                        {
                            W.Cast(AllyHeroW, PacketCast);
                            return;
                        }
                    }
                }
                #endregion

            }
        }
    }
}
