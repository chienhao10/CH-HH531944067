namespace KappAzir.Modes
{
    using System.Linq;

    using EloBuddy;
    using EloBuddy.SDK;
    using EloBuddy.SDK.Enumerations;

    using Mario_s_Lib;

    using SharpDX;
    using static Menus;
    using static SpellsManager;

    /// <summary>
    /// This mode will run when the key of the orbwalker is pressed
    /// </summary>
    internal class Harass : ModeManager
    {
        /// <summary>
        /// Put in here what you want to do when the mode is running
        /// </summary>
        public static void Execute()
        {
            var target = TargetSelector.GetTarget(1250, DamageType.Magical);
            if (target == null)
            {
                return;
            }

            if (W.IsReady() && HarassMenu.GetCheckBoxValue("wUse"))
            {
                if (W.Handle.Ammo == 1 && HarassMenu.GetCheckBoxValue("wSave"))
                {
                    return;
                }

                if (Orbwalker.AzirSoldiers.Any(x => !x.IsInRange(target, Orbwalker.AzirSoldierAutoAttackRange)))
                {
                    W.Cast(W.GetPrediction(target).CastPosition);
                }

                if (Orbwalker.AzirSoldiers.Count == 0)
                {
                    W.Cast(W.GetPrediction(target).CastPosition);
                }

                if (target.IsValidTarget(Q.Range - 25) && Q.IsReady() && Q.Handle.SData.Mana + W.Handle.SData.Mana < Azir.Mana)
                {
                    var p = Azir.Distance(target, true) > W.RangeSquared
                                ? Azir.Position.To2D().Extend(target.Position.To2D(), W.Range)
                                : target.Position.To2D();
                    W.Cast((Vector3)p);
                }
            }

            if (Q.IsReady() && HarassMenu.GetCheckBoxValue("qUse"))
            {
                if (Orbwalker.AzirSoldiers.Any(x => x.IsInRange(target, Orbwalker.AzirSoldierAutoAttackRange)))
                {
                    return;
                }

                Q.RangeCheckSource = Azir.ServerPosition;
                Q.SourcePosition = Orbwalker.AzirSoldiers.FirstOrDefault(s => s.IsAlly)?.ServerPosition;

                var pred = Q.GetPrediction(target);
                if (pred.HitChance >= hitchance)
                {
                    Q.Cast(pred.CastPosition);
                }
                if (target.GetDamage(SpellSlot.Q) >= target.TotalShieldHealth())
                {
                    Q.Cast(target.ServerPosition);
                }
            }

            if (Ehit(target) && E.IsReady() && HarassMenu.GetCheckBoxValue("eUse"))
            {
                if (target.IsUnderHisturret() && !HarassMenu.GetCheckBoxValue("eDive")
                    || (target.CountEnemiesInRange(750) >= HarassMenu.GetSliderValue("eSave")))
                {
                    return;
                }
                E.TryToCast(target, HarassMenu);
            }
        }
    }
}