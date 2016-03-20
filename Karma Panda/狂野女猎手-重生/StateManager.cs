using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu.Values;
// ReSharper disable InconsistentNaming

namespace NidaleeBuddyEvolution
{
    internal class StateManager
    {
        /// <summary>
        /// Does Combo
        /// </summary>
        public static void Combo()
        {
            var onUpdate = NidaleeMenu.MiscMenu["useQC_OnUpdate"].Cast<CheckBox>().CurrentValue;

            // Sliders
            var predQH = NidaleeMenu.ComboMenu["predQH"].Cast<Slider>().CurrentValue;
            var predWH = NidaleeMenu.ComboMenu["predWH"].Cast<Slider>().CurrentValue;
            var predWC = NidaleeMenu.ComboMenu["predWC"].Cast<Slider>().CurrentValue;
            var predEC = NidaleeMenu.ComboMenu["predEC"].Cast<Slider>().CurrentValue;

            if (!Essentials.CatForm())
            {
                var qTarget = TargetSelector.GetTarget(Program.QHuman.Range, DamageType.Magical);

                if (qTarget != null && Essentials.ShouldUseSpell(qTarget, SpellSlot.Q, NidaleeMenu.ComboMenu))
                {
                    var pred = Program.QHuman.GetPrediction(qTarget);

                    if (pred != null && pred.HitChancePercent >= predQH)
                    {
                        Program.QHuman.Cast(pred.CastPosition);
                    }
                }

                var wTarget = TargetSelector.GetTarget(Program.WHuman.Range, DamageType.Magical);

                if (wTarget != null && Essentials.ShouldUseSpell(wTarget, SpellSlot.W, NidaleeMenu.ComboMenu))
                {
                    var pred = Program.WHuman.GetPrediction(wTarget);

                    if (pred != null && pred.HitChancePercent >= predWH)
                    {
                        Program.WHuman.Cast(pred.CastPosition);
                    }
                }

                #region R Logic

                /* Changing to Cat Form Logic */

                if (!Essentials.ShouldUseSpell(null, SpellSlot.R, NidaleeMenu.HarassMenu))
                {
                    return;
                }

                if (Essentials.LastHuntedTarget != null && Program.WExtended.IsLearned &&
                    Program.WExtended.IsInRange(Essentials.LastHuntedTarget) &&
                    Essentials.IsReady(Essentials.SpellTimer["Takedown"]) &&
                    Essentials.IsReady(Essentials.SpellTimer["Pounce"]) &&
                    Essentials.IsReady(Essentials.SpellTimer["ExPounce"]) &&
                    Essentials.IsReady(Essentials.SpellTimer["Swipe"]))
                {
                    Program.R.Cast();
                    Core.DelayAction(() =>
                    {
                        if (Essentials.ShouldUseSpell(Essentials.LastHuntedTarget, SpellSlot.W, NidaleeMenu.ComboMenu))
                        {
                            Program.WExtended.Cast(Essentials.LastHuntedTarget.ServerPosition);
                            Essentials.LastHuntedTarget = null;
                        }
                    }, Program.R.CastDelay);
                }

                var wETarget = TargetSelector.GetTarget(Program.WExtended.Range, DamageType.Magical);

                if (wETarget != null && Program.WExtended.IsLearned && Essentials.IsHunted(wETarget) &&
                    (Essentials.IsReady(Essentials.SpellTimer["Takedown"]) &&
                     Essentials.IsReady(Essentials.SpellTimer["ExPounce"]) &&
                     Essentials.IsReady(Essentials.SpellTimer["Swipe"])))
                {
                    Program.R.Cast();
                }

                var wCTarget = TargetSelector.GetTarget(Program.WCat.Range, DamageType.Magical);

                if (wCTarget != null && Program.WCat.IsLearned &&
                    (Essentials.IsReady(Essentials.SpellTimer["Takedown"]) &&
                     Essentials.IsReady(Essentials.SpellTimer["Pounce"]) &&
                     Essentials.IsReady(Essentials.SpellTimer["Swipe"])))
                {
                    Program.R.Cast();
                    Core.DelayAction(() =>
                    {
                        if (Essentials.ShouldUseSpell(wCTarget, SpellSlot.W, NidaleeMenu.ComboMenu))
                        {
                            Program.WCat.Cast(wCTarget.ServerPosition);
                        }
                    }, Program.R.CastDelay);
                }

                #endregion
            }
            else
            {
                var qTarget = TargetSelector.GetTarget(Program.QCat.Range, DamageType.Mixed);

                if (qTarget != null && onUpdate &&
                    Essentials.ShouldUseSpell(qTarget, SpellSlot.Q, NidaleeMenu.ComboMenu))
                {
                    Program.QCat.Cast(qTarget);
                }

                var wTarget = TargetSelector.GetTarget(Program.WExtended.Range, DamageType.Magical);

                if (wTarget != null && Essentials.ShouldUseSpell(wTarget, SpellSlot.W, NidaleeMenu.ComboMenu))
                {
                    if (Essentials.IsHunted(wTarget))
                    {
                        var pred = Program.WExtended.GetPrediction(wTarget);

                        if (pred != null && pred.HitChancePercent >= predWC)
                        {
                            Program.WExtended.Cast(pred.CastPosition);
                        }
                    }
                    else if (!Essentials.IsHunted(wTarget))
                    {
                        var pred = Program.WCat.GetPrediction(wTarget);

                        if (pred != null && pred.HitChancePercent >= predWC)
                        {
                            Program.WCat.Cast(pred.CastPosition);
                        }
                    }
                }

                var eTarget = TargetSelector.GetTarget(Program.ECat.Range, DamageType.Magical);

                if (eTarget != null && Essentials.ShouldUseSpell(eTarget, SpellSlot.E, NidaleeMenu.ComboMenu))
                {
                    var pred = Program.ECat.GetPrediction(eTarget);

                    if (pred != null && pred.HitChancePercent >= predEC)
                    {
                        Program.ECat.Cast(pred.CastPosition);
                    }
                }

                #region R Logic

                /* Changing to Human Form Logic */

                if (!Essentials.ShouldUseSpell(null, SpellSlot.R, NidaleeMenu.ComboMenu) ||
                    !Essentials.IsReady(Essentials.SpellTimer["Swipe"])
                    || !Essentials.IsReady(Essentials.SpellTimer["Pounce"]) ||
                    !Essentials.IsReady(Essentials.SpellTimer["ExPounce"]) ||
                    !Essentials.IsReady(Essentials.SpellTimer["Takedown"]))
                {
                    return;
                }

                var qHTarget = TargetSelector.GetTarget(Program.QHuman.Range, DamageType.Magical);

                if (qHTarget != null && (Essentials.IsReady(Essentials.SpellTimer["Javelin"])))
                {
                    Program.R.Cast();
                    Core.DelayAction(() =>
                    {
                        if (!Essentials.ShouldUseSpell(qHTarget, SpellSlot.Q, NidaleeMenu.ComboMenu))
                        {
                            return;
                        }

                        var pred = Program.QHuman.GetPrediction(qHTarget);

                        if (pred != null && pred.HitChancePercent >= predQH)
                        {
                            Program.QHuman.Cast(pred.CastPosition);
                        }
                    }, Program.R.CastDelay);
                }

                var eHTarget =
                    EntityManager.Heroes.Allies.FirstOrDefault(
                        t =>
                            Program.EHuman.IsInRange(t) &&
                            t.Health <= NidaleeMenu.MiscMenu["autoHealPercent"].Cast<Slider>().CurrentValue);

                if (eHTarget != null && NidaleeMenu.MiscMenu["autoHeal"].Cast<CheckBox>().CurrentValue &&
                    NidaleeMenu.MiscMenu["autoHeal_" + eHTarget.BaseSkinName].Cast<CheckBox>().CurrentValue &&
                    (Essentials.IsReady(Essentials.SpellTimer["Primalsurge"])))
                {
                    Program.R.Cast();
                    Core.DelayAction(() =>
                    {
                        if (Essentials.ShouldUseSpell(eHTarget, SpellSlot.E, NidaleeMenu.ComboMenu))
                        {
                            Program.EHuman.Cast(eHTarget);
                        }
                    }, Program.R.CastDelay);
                }

                #endregion
            }
        }

        /// <summary>
        /// Does Harass
        /// </summary>
        public static void Harass()
        {
            if (Essentials.CatForm() && Essentials.IsReady(Essentials.SpellTimer["Javelin"]) &&
                Essentials.ShouldUseSpell(null, SpellSlot.R, NidaleeMenu.HarassMenu))
            {
                Program.R.Cast();
            }

            var qTarget = TargetSelector.GetTarget(Program.QHuman.Range, DamageType.Magical);

            if (!Essentials.CatForm() && Essentials.ShouldUseSpell(qTarget, SpellSlot.Q, NidaleeMenu.HarassMenu))
            {
                var pred = Program.QHuman.GetPrediction(qTarget);

                if (pred != null &&
                    pred.HitChancePercent >= NidaleeMenu.HarassMenu["predQH"].Cast<Slider>().CurrentValue)
                {
                    Program.QHuman.Cast(pred.CastPosition);
                }
            }
        }

        /// <summary>
        /// Does Lane Clear
        /// </summary>
        public static void LaneClear()
        {
            var useQC = NidaleeMenu.LaneClearMenu["useQC"].Cast<CheckBox>().CurrentValue;
            var useWC = NidaleeMenu.LaneClearMenu["useWC"].Cast<CheckBox>().CurrentValue;
            var useEC = NidaleeMenu.LaneClearMenu["useEC"].Cast<CheckBox>().CurrentValue;
            var useR = NidaleeMenu.LaneClearMenu["useR"].Cast<CheckBox>().CurrentValue;
            var predWC = NidaleeMenu.LaneClearMenu["predWC"].Cast<Slider>().CurrentValue;
            var predEC = NidaleeMenu.LaneClearMenu["predEC"].Cast<Slider>().CurrentValue;
            var onUpdate = NidaleeMenu.MiscMenu["useQC_OnUpdate"].Cast<CheckBox>().CurrentValue;

            if (Essentials.CatForm())
            {
                if (useQC && onUpdate)
                {
                    var minion = EntityManager.MinionsAndMonsters.GetLaneMinions(EntityManager.UnitTeam.Enemy,
                        Player.Instance.ServerPosition, Program.QCat.Range).FirstOrDefault(t => t.IsValidTarget());

                    if (minion != null &&
                        minion.Health <= Essentials.DamageLibrary.CalculateDamage(minion, true, false, false, false))
                    {
                        Program.QCat.Cast(minion);
                    }
                }

                if (useWC)
                {
                    var turret =
                        EntityManager.Turrets.Enemies.FirstOrDefault(
                            t => t.IsValidTarget() && t.IsInAutoAttackRange(Player.Instance));

                    if (turret == null)
                    {
                        var minion = EntityManager.MinionsAndMonsters.GetLaneMinions(EntityManager.UnitTeam.Enemy,
                            Player.Instance.ServerPosition, Program.WExtended.Range)
                            .Where(t => t.IsValidTarget() && !Essentials.IsHunted(t));

                        var minionH = EntityManager.MinionsAndMonsters.GetLaneMinions(EntityManager.UnitTeam.Enemy,
                            Player.Instance.ServerPosition, Program.WExtended.Range)
                            .Where(t => t.IsValidTarget() && Essentials.IsHunted(t));

                        var pred = EntityManager.MinionsAndMonsters.GetCircularFarmLocation(minion, Program.WCat.Width,
                            (int) Program.WCat.Range);

                        var predH = EntityManager.MinionsAndMonsters.GetCircularFarmLocation(minionH,
                            Program.WExtended.Width,
                            (int) Program.WExtended.Range);

                        if (pred.HitNumber >= predWC)
                        {
                            Program.WCat.Cast(pred.CastPosition);
                        }

                        if (predH.HitNumber >= predWC)
                        {
                            Program.WExtended.Cast(predH.CastPosition);
                        }
                    }
                }

                if (useEC)
                {
                    var minion = EntityManager.MinionsAndMonsters.GetLaneMinions(EntityManager.UnitTeam.Enemy,
                        Player.Instance.ServerPosition, Program.ECat.Range).Where(t => t.IsValidTarget());

                    var ePrediction = EntityManager.MinionsAndMonsters.GetCircularFarmLocation(minion,
                        Program.ECat.Range,
                        Program.ECat.ConeAngleDegrees, Program.ECat.CastDelay, Program.ECat.Speed);

                    if (ePrediction.HitNumber >= predEC)
                    {
                        Program.ECat.Cast(ePrediction.CastPosition);
                    }
                }

                if (useR && Program.R.IsReady() && !Essentials.IsReady(Essentials.SpellTimer["Takedown"])
                    && !Essentials.IsReady(Essentials.SpellTimer["Pounce"])
                    && !Essentials.IsReady(Essentials.SpellTimer["Swipe"]))
                {
                    Program.R.Cast();
                }
            }
            else if (!Essentials.CatForm())
            {
                var minion =
                    EntityManager.MinionsAndMonsters.EnemyMinions.FirstOrDefault(
                        t => t.IsValidTarget(Essentials.CatRange));

                if (minion == null)
                {
                    return;
                }

                if (useR && Program.R.IsReady()
                    && Essentials.IsReady(Essentials.SpellTimer["Takedown"])
                    && Essentials.IsReady(Essentials.SpellTimer["Pounce"])
                    && Essentials.IsReady(Essentials.SpellTimer["Swipe"]))
                {
                    Program.R.Cast();
                }
            }
        }

        /// <summary>
        /// Does Jungle Clear
        /// </summary>
        public static void JungleClear()
        {
            var useQH = NidaleeMenu.JungleClearMenu["useQH"].Cast<CheckBox>().CurrentValue;
            var useQC = NidaleeMenu.JungleClearMenu["useQC"].Cast<CheckBox>().CurrentValue;
            var useWC = NidaleeMenu.JungleClearMenu["useWC"].Cast<CheckBox>().CurrentValue;
            var useEC = NidaleeMenu.JungleClearMenu["useEC"].Cast<CheckBox>().CurrentValue;
            var useR = NidaleeMenu.JungleClearMenu["useR"].Cast<CheckBox>().CurrentValue;
            var predQH = NidaleeMenu.JungleClearMenu["predQH"].Cast<Slider>().CurrentValue;
            var predWC = NidaleeMenu.JungleClearMenu["predWC"].Cast<Slider>().CurrentValue;
            var predEC = NidaleeMenu.JungleClearMenu["predEC"].Cast<Slider>().CurrentValue;
            var onUpdate = NidaleeMenu.MiscMenu["useQC_OnUpdate"].Cast<CheckBox>().CurrentValue;

            if (!Essentials.CatForm())
            {
                if (useQH)
                {
                    var jungleMob =
                        EntityManager.MinionsAndMonsters.GetJungleMonsters(Player.Instance.ServerPosition,
                            Program.QHuman.Range)
                            .Where(t => t.IsValidTarget() && Essentials.JungleMobsList.Contains(t.BaseSkinName))
                            .OrderByDescending(t => t.MaxHealth)
                            .FirstOrDefault();

                    if (jungleMob != null && Program.QHuman.IsReady())
                    {
                        var pred = Program.QHuman.GetPrediction(jungleMob);

                        if (pred != null && pred.HitChancePercent >= predQH)
                        {
                            Program.QHuman.Cast(pred.CastPosition);
                        }
                    }

                }

                var m = EntityManager.MinionsAndMonsters.GetJungleMonsters(Player.Instance.ServerPosition,
                    Essentials.CatRange).FirstOrDefault();

                if (useR && m != null && Program.R.IsReady() && !Essentials.IsReady(Essentials.SpellTimer["Javelin"])
                    && Essentials.IsReady(Essentials.SpellTimer["Takedown"])
                    && Essentials.IsReady(Essentials.SpellTimer["Pounce"])
                    && Essentials.IsReady(Essentials.SpellTimer["Swipe"]))
                {
                    Program.R.Cast();
                }

                var huntedMob = EntityManager.MinionsAndMonsters.GetJungleMonsters(Player.Instance.ServerPosition,
                    Program.WExtended.Range)
                    .FirstOrDefault(t => Essentials.IsHunted(t) && t.IsValidTarget(Program.WExtended.Range));

                if (useR && Program.R.IsReady() && huntedMob != null &&
                    Essentials.IsReady(Essentials.SpellTimer["ExPounce"]))
                {
                    Program.R.Cast();
                    Core.DelayAction(() =>
                    {
                        if (useWC)
                        {
                            var pred = Program.WExtended.GetPrediction(huntedMob);

                            if (pred != null && pred.HitChancePercent >= predWC)
                            {
                                Program.WExtended.Cast(pred.CastPosition);
                            }
                        }
                    }, Program.R.CastDelay);
                }
            }
            else if (Essentials.CatForm())
            {
                if (useQC && onUpdate)
                {
                    var qTarget = EntityManager.MinionsAndMonsters.GetJungleMonsters(Player.Instance.ServerPosition,
                        Program.QCat.Range).OrderByDescending(t => t.MaxHealth).FirstOrDefault(t => t.IsValidTarget());

                    if (qTarget != null)
                    {
                        Program.QCat.Cast(qTarget);
                    }
                }

                if (useWC)
                {
                    if (Essentials.LastHuntedTarget != null)
                    {
                        var pred = Program.WExtended.GetPrediction(Essentials.LastHuntedTarget);

                        if (pred != null && pred.HitChancePercent >= predWC)
                        {
                            Program.WExtended.Cast(pred.CastPosition);
                        }
                    }

                    var huntedMob = EntityManager.MinionsAndMonsters.GetJungleMonsters(Player.Instance.ServerPosition,
                        Program.WExtended.Range)
                        .FirstOrDefault(t => Essentials.IsHunted(t) && t.IsValidTarget(Program.WExtended.Range));

                    if (huntedMob != null)
                    {
                        var pred = Program.WExtended.GetPrediction(huntedMob);

                        if (pred != null && pred.HitChancePercent >= predWC)
                        {
                            Program.WExtended.Cast(pred.CastPosition);
                        }
                    }

                    var wTarget =
                        EntityManager.MinionsAndMonsters.GetJungleMonsters(Player.Instance.ServerPosition,
                            Program.WCat.Range)
                            .OrderByDescending(t => t.MaxHealth)
                            .FirstOrDefault(t => t.IsValidTarget());

                    if (wTarget != null)
                    {
                        var pred = Program.WCat.GetPrediction(wTarget);

                        if (pred != null && pred.HitChancePercent >= predWC)
                        {
                            Program.WCat.Cast(pred.CastPosition);
                        }
                    }
                }

                if (useEC)
                {
                    var eTarget =
                        EntityManager.MinionsAndMonsters.Monsters.Where(t => t.IsValidTarget(Program.ECat.Range))
                            .ToArray();

                    var ePrediction = EntityManager.MinionsAndMonsters.GetCircularFarmLocation(eTarget,
                        Program.ECat.Range,
                        Program.ECat.ConeAngleDegrees, Program.ECat.CastDelay, Program.ECat.Speed);

                    if (ePrediction.HitNumber >= predEC)
                    {
                        Program.ECat.Cast(ePrediction.CastPosition);
                    }
                }

                var qHTarget =
                    EntityManager.MinionsAndMonsters.GetJungleMonsters(Player.Instance.ServerPosition,
                        Program.QHuman.Range).FirstOrDefault();

                if (qHTarget == null)
                {
                    return;
                }

                if (useR && Program.R.IsReady() && Essentials.IsReady(Essentials.SpellTimer["Javelin"]))
                {
                    Program.R.Cast();
                }
            }
        }
    }
}