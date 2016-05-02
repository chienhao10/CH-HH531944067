namespace KappaKindred.Events
{
    using System;
    using System.Linq;

    using EloBuddy;
    using EloBuddy.SDK;
    using EloBuddy.SDK.Menu.Values;

    using Modes;

    internal class OnUpdate
    {
        public static void Update(EventArgs args)
        {
            var lanemana = Menu.ManaMenu["lanemana"].Cast<Slider>().CurrentValue;
            var harassmana = Menu.ManaMenu["harassmana"].Cast<Slider>().CurrentValue;
            var target = TargetSelector.GetTarget(Spells.Q.Range, DamageType.Physical);
            if (Player.Instance.IsDead || MenuGUI.IsChatOpen || Player.Instance.IsRecalling())
            {
                return;
            }

            if (target != null && target.HasBuff("kindredrnodeathbuff") && target.HealthPercent <= 15
                && Menu.ComboMenu["Pspells"].Cast<CheckBox>().CurrentValue)
            {
                return;
            }

            var flags = Orbwalker.ActiveModesFlags;
            if (flags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                Combo.Start();
            }

            if (flags.HasFlag(Orbwalker.ActiveModes.LaneClear) && Player.Instance.ManaPercent >= lanemana)
            {
                Clear.Start();
            }

            if (flags.HasFlag(Orbwalker.ActiveModes.JungleClear))
            {
                Jungle.Start();
            }

            if (flags.HasFlag(Orbwalker.ActiveModes.Harass) && Player.Instance.ManaPercent >= harassmana)
            {
                Harass.Start();
            }

            if (flags.HasFlag(Orbwalker.ActiveModes.Flee))
            {
                Flee.Start();
            }

            if (Menu.ComboMenu["Emark"].Cast<CheckBox>().CurrentValue)
            {
                var Etarget =
                    ObjectManager.Get<AIHeroClient>()
                        .Where(enemy => !enemy.IsDead && enemy.IsValidTarget(Player.Instance.GetAutoAttackRange()))
                        .FirstOrDefault(enemy => enemy.Buffs.Any(buff => buff.Name == "KindredERefresher" && buff.Count > 0));
                if (Etarget != null)
                {
                    TargetSelector.GetPriority(Etarget);
                }
            }

            if (Menu.ComboMenu["Pmark"].Cast<CheckBox>().CurrentValue)
            {
                var Ptarget =
                    ObjectManager.Get<AIHeroClient>()
                        .Where(enemy => !enemy.IsDead && enemy.IsValidTarget(Player.Instance.GetAutoAttackRange()))
                        .FirstOrDefault(enemy => enemy.Buffs.Any(buff => buff.Name == "KindredHitTracker"));

                if (Ptarget != null)
                {
                    TargetSelector.GetPriority(Ptarget);
                }
            }
        }
    }
}