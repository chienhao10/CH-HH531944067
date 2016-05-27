namespace KappAzir.Modes
{
    using System.Linq;

    using EloBuddy;
    using EloBuddy.SDK;
    using EloBuddy.SDK.Enumerations;

    using Utility;

    internal class Harass
    {
        public static void Execute()
        {
            var menu = Menus.HarassMenu;
            var Qmana = Player.Instance.ManaPercent > menu.slider("Qmana");
            var Wmana = Player.Instance.ManaPercent > menu.slider("Wmana");
            var Emana = Player.Instance.ManaPercent > menu.slider("Emana");
            var QS = Orbwalker.ValidAzirSoldiers.Count(s => s.IsAlly) >= menu.slider("QS");
            var Q = QS && Qmana && menu.checkbox("Q") && Azir.Q.IsReady();
            var W = Wmana && menu.checkbox("W") && Azir.W.IsReady();
            var E = Emana && menu.checkbox("E") && Azir.E.IsReady();
            var Wsave = menu.checkbox("Wsave") && Azir.W.Handle.Ammo > 1;
            var Wlimit = menu.slider("WS") >= Orbwalker.ValidAzirSoldiers.Count(s => s.IsAlly);
            var target = TargetSelector.GetTarget(Azir.Q.Range + 25, DamageType.Magical);

            if (target == null || !target.IsKillable())
            {
                return;
            }

            if (W && Wsave && Wlimit)
            {
                if (target.IsValidTarget(Azir.W.Range))
                {
                    var pred = Azir.W.GetPrediction(target);
                    Azir.W.Cast(pred.CastPosition);
                }
                if (menu.checkbox("Q") && !target.IsValidTarget(Azir.W.Range) && Azir.Q.IsReady()
                    && Player.Instance.Mana > Azir.Q.Mana() + Azir.W.Mana() && target.IsValidTarget(Azir.Q.Range - 25) && menu.checkbox("WQ"))
                {
                    var p = Player.Instance.Position.Extend(target.Position, Azir.W.Range);
                    Azir.W.Cast(p.To3D());
                }
            }

            if (Orbwalker.AzirSoldiers.Count(s => s.IsAlly) == 0)
            {
                return;
            }

            if (Q)
            {
                if (Azir.Q.GetPrediction(target).HitChance >= HitChance.High || target.IsCC())
                {
                    Azir.Q.Cast(target);
                }
            }

            if (E && target.Ehit())
            {
                if ((target.CountEnemeis(750) >= menu.slider("Esafe")) || (menu.slider("EHP") >= Player.Instance.HealthPercent)
                    || (!menu.checkbox("Edive") && target.IsUnderHisturret()))
                {
                    return;
                }

                Azir.E.Cast(target);
            }
        }
    }
}