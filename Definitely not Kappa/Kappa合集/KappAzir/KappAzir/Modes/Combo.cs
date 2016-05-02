namespace KappAzir.Modes
{
    using System.Linq;

    using EloBuddy;
    using EloBuddy.SDK;

    using Mario_s_Lib;

    using SharpDX;
    using static Menus;
    using static SpellsManager;
    using KappAzir;

    /// <summary>
    /// This mode will run when the key of the orbwalker is pressed
    /// </summary>
    internal class Combo : ModeManager
    {
        /// <summary>
        /// Put in here what you want to do when the mode is running
        /// </summary>
        public static void Execute()
        {
            var target = TargetSelector.GetTarget(Q.Range, DamageType.Magical);

            if (target == null)
            {
                return;
            }

            if (W.IsReady() && ComboMenu.GetCheckBoxValue("wUse"))
            {
                if (W.Handle.Ammo == 1 && ComboMenu.GetCheckBoxValue("wSave"))
                {
                    return;
                }

                if (Orbwalker.AzirSoldiers.Any(x => !x.IsInRange(target, Orbwalker.AzirSoldierAutoAttackRange))
                    && ComboMenu.GetCheckBoxValue("wUseAA"))
                {
                    W.Cast(W.GetPrediction(target).CastPosition);
                }

                if (Orbwalker.AzirSoldiers.Count(s => s.IsAlly) == 0 || !ComboMenu.GetCheckBoxValue("wUseAA"))
                {
                    W.Cast(W.GetPrediction(target).CastPosition);
                }

                if (target.IsValidTarget(Q.Range - 100) && Q.IsReady() && Azir.Mana > ManaCheck(Azir) && !target.IsValidTarget(W.Range))
                {
                    var p = Azir.Distance(target, true) > W.RangeSquared
                                ? Azir.Position.To2D().Extend(target.Position.To2D(), W.Range)
                                : target.Position.To2D();
                    W.Cast((Vector3)p);
                }
            }

            var azirtower =
                ObjectManager.Get<GameObject>().FirstOrDefault(o => o != null && o.Name.ToLower().Contains("towerclicker") && Azir.Distance(o) < 500);
            if (azirtower != null && Azir.PassiveCooldownEndTime <= 0 && azirtower.CountEnemiesInRange(800) >= ComboMenu.GetSliderValue("TowerPls"))
            {
                Player.UseObject(azirtower);
            }

            if (Orbwalker.AzirSoldiers.Count(s => s.IsAlly) >= ComboMenu.GetSliderValue("Qcount") && Q.IsReady() && ComboMenu.GetCheckBoxValue("qUse"))
            {
                Q.RangeCheckSource = Azir.ServerPosition;
                Q.SourcePosition = Orbwalker.AzirSoldiers.Where(s => s.IsAlly).OrderBy(s => s.Distance(target)).FirstOrDefault()?.Position;
                if (Orbwalker.AzirSoldiers.Any(x => x.IsInRange(target, Orbwalker.AzirSoldierAutoAttackRange)) && ComboMenu.GetCheckBoxValue("qUseAA"))
                {
                    return;
                }

                if (target.CountEnemiesInRange(Q.Width) >= ComboMenu.GetSliderValue("Qaoe"))
                {
                    Q.Cast(target.Position);
                }
                
                var pred = Q.GetPrediction(target);
                if (pred.HitChance >= hitchance || target.GetDamage(SpellSlot.Q) + target.GetDamage(SpellSlot.W) >= target.TotalShieldHealth())
                {
                    Q.Cast(pred.CastPosition);
                }
            }

            if (Ehit(target) && E.IsReady() && ComboMenu.GetCheckBoxValue("eUse"))
            {
                if (target.IsUnderHisturret() && !ComboMenu.GetCheckBoxValue("eUseDive")
                    || (target.CountEnemiesInRange(800) >= ComboMenu.GetSliderValue("eSave")))
                {
                    return;
                }

                if (target.HealthPercent - Azir.HealthPercent > ComboMenu.GetSliderValue("eHealth"))
                {
                    return;
                }

                if (target.Damage() >= target.TotalShieldHealth() && ComboMenu.GetCheckBoxValue("eUsekill"))
                {
                    E.TryToCast(target, ComboMenu);
                }
                else
                {
                    if (!ComboMenu.GetCheckBoxValue("eUsekill"))
                    {
                        E.TryToCast(target, ComboMenu);
                    }
                }
            }

            if ((R.IsReady() && ComboMenu.GetCheckBoxValue("rUse") || (ComboMenu.GetKeyBindValue("Rcast"))) && target.IsValidTarget(R.Range)
                && R.GetPrediction(target).HitChance >= hitchance)
            {
                if (target.Damage() - target.GetDamage(SpellSlot.R) >= target.TotalShieldHealth() && ComboMenu.GetCheckBoxValue("rOverKill"))
                {
                    return;
                }

                var RTargets = ComboMenu.GetSliderValue("Raoe");
                if ((Azir.HealthPercent <= 20 && ComboMenu.GetCheckBoxValue("rUseSave"))
                    || (ManaCheck(Azir) < Azir.Mana && target.Damage() >= target.TotalShieldHealth() && ComboMenu.GetCheckBoxValue("rUsekill"))
                    || (Azir.CountEnemiesInRange(R.Range - 15) >= RTargets))
                {
                    if (tower != null && ComboMenu.GetCheckBoxValue("rUseTower"))
                    {
                        R.Cast(tower.ServerPosition);
                    }
                    else if (ally != null && ComboMenu.GetCheckBoxValue("rUseAlly"))
                    {
                        R.Cast(ally.ServerPosition);
                    }
                    else
                    {
                        R.Cast(target.Position);
                    }
                }
            }
        }
    }
}