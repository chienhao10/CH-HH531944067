using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;

using Settings = KA_Ezreal.Config.Modes.Misc;
using Configs = KA_Ezreal.Config.Modes.Harass;

namespace KA_Ezreal.Modes
{
    public sealed class PermaActive : ModeBase
    {
        public override bool ShouldBeExecuted()
        {
            return true;
        }

        private static readonly Item Tear = new Item(ItemId.Tear_of_the_Goddess);
        private static readonly Item Manamune = new Item(ItemId.Manamune);
        private static readonly Item Archengel = new Item(ItemId.Archangels_Staff);

        public override void Execute()
        {
            var targetManamune = TargetSelector.GetTarget(Q.Range, DamageType.Physical);
            if (targetManamune == null || targetManamune.IsZombie) return;
            if (Manamune.IsReady() && Manamune.IsOwned() && Player.Instance.ManaPercent >= 55)
            {
                Manamune.Cast();
            }
            else
            {
                Manamune.Cast();
            }

            if (Settings.CCedR && R.IsReady())
            {
                var target = TargetSelector.GetTarget(R.Range, DamageType.Physical);
                if (target != null)
                {
                    if (target.HasBuffOfType(BuffType.Stun) || target.HasBuffOfType(BuffType.Snare) ||
                        target.HasBuffOfType(BuffType.Knockup) &&
                        target.IsInRange(Player.Instance, Settings.maxR) &&
                        !target.IsInRange(Player.Instance, Settings.minR))
                    {
                        R.Cast(target.Position);
                    }
                }
            }

            if (Settings.KSR && R.IsReady())
            {
                var targetR = TargetSelector.GetTarget(R.Range, DamageType.Physical);
                if (targetR != null)
                {
                    if (Prediction.Health.GetPrediction(targetR, R.CastDelay) > 10 &&
                        Prediction.Health.GetPrediction(targetR, R.CastDelay) <
                        SpellDamage.GetRealDamage(SpellSlot.R, targetR) &&
                        targetR.IsInRange(Player.Instance, Settings.maxR) &&
                        !targetR.IsInRange(Player.Instance, Settings.minR))
                    {
                        var pred = R.GetPrediction(targetR);
                        if (pred.HitChancePercent >= 90)
                        {
                            R.Cast(pred.CastPosition);
                        }
                    }
                }
            }

            if (Configs.UseQAuto && Q.IsReady() && Configs.ManaAutoHarass <= Player.Instance.ManaPercent)
            {
                var target = TargetSelector.GetTarget(Q.Range, DamageType.Physical);
                if (target == null || target.IsZombie) return;

                if (target.IsValidTarget(Q.Range))
                {
                    var pred = Q.GetPrediction(target);
                    if (pred.HitChancePercent >= 75)
                    {
                        Q.Cast(pred.CastPosition);
                    }
                }
            }

            if (Configs.UseWAuto && W.IsReady() && Configs.ManaAutoHarass <= Player.Instance.ManaPercent)
            {
                var target = TargetSelector.GetTarget(W.Range, DamageType.Physical);
                if (target == null || target.IsZombie) return;

                if (target.IsValidTarget(W.Range))
                {
                    var pred = W.GetPrediction(target);
                    if (pred.HitChancePercent >= 90)
                    {
                        W.Cast(pred.CastPosition);
                    }
                }
            }
            //Tear AutoStackin
            if (Settings.AutoTearStack && Q.IsReady() &&
                (Player.Instance.ManaPercent >= Settings.MinManaToAutoStack || Player.Instance.IsInShopRange()) &&
                (Tear.IsOwned() || Archengel.IsOwned() || Manamune.IsOwned()))
            {
                var target = EntityManager.Heroes.Enemies.FirstOrDefault(e => e.IsValidTarget(Q.Range));

                var laneMinion =
                    EntityManager.MinionsAndMonsters.GetLaneMinions()
                        .OrderByDescending(m => m.Health)
                        .FirstOrDefault(
                            m =>
                                m.IsValidTarget(Q.Range) &&
                                Prediction.Health.GetPrediction(m, Q.CastDelay) <=
                                SpellDamage.GetRealDamage(SpellSlot.Q, m) &&
                                Prediction.Health.GetPrediction(m, Q.CastDelay) > 10);

                var pos = Player.Instance.Position.Extend(Game.CursorPos, Q.Range).To3D();

                if (target != null)
                {
                    var pred = Q.GetPrediction(target);
                    if (pred.HitChance >= HitChance.High)
                    {
                        Q.Cast(pred.CastPosition);
                    }
                    ;
                }
                else if (laneMinion != null)
                {
                    var pred = Q.GetPrediction(laneMinion);
                    if (pred.HitChance >= HitChance.High)
                    {
                        Q.Cast(pred.CastPosition);
                    }
                }
                else
                {
                    Q.Cast(pos);
                }
            }
            //JungleSteal
            if (R.IsReady() && Settings.JungleSteal)
            {
                var targetR = TargetSelector.GetTarget(R.Range, DamageType.Magical);
                if (targetR != null)
                {
                    if (Settings.JungleStealBlue)
                    {
                        var blue =
                            EntityManager.MinionsAndMonsters.GetJungleMonsters()
                                .FirstOrDefault(
                                    m =>
                                        Prediction.Health.GetPrediction(m, R.CastDelay + (int)(1000 * Player.Instance.Distance(m) / R.Speed)) <= SpellDamage.GetRealDamage(SpellSlot.R, m) &&
                                        m.IsValidTarget(R.Range) &&
                                        m.BaseSkinName == "SRU_Blue" && m.IsInRange(targetR, 1500) &&
                                        Prediction.Health.GetPrediction(m, R.CastDelay + (int)(1000 * Player.Instance.Distance(m) / R.Speed)) > m.CountEnemiesInRange(1000) * 70);
                        if (blue != null)
                        {
                            
                            var pred = R.GetPrediction(blue);
                            if (pred.HitChance >= HitChance.High)
                            {
                                R.Cast(pred.CastPosition);
                            }
                        }
                    }

                    if (Settings.JungleStealRed)
                    {
                        var red =
                            EntityManager.MinionsAndMonsters.GetJungleMonsters()
                                .FirstOrDefault(
                                    m =>
                                        Prediction.Health.GetPrediction(m, R.CastDelay + (int)(1000 * Player.Instance.Distance(m) / R.Speed)) <
                                        SpellDamage.GetRealDamage(SpellSlot.R, m) &&
                                        m.IsValidTarget(R.Range) &&
                                        m.BaseSkinName == "SRU_Red" && m.IsInRange(targetR, 1500) &&
                                        Prediction.Health.GetPrediction(m, R.CastDelay + (int)(1000 * Player.Instance.Distance(m) / R.Speed)) > m.CountEnemiesInRange(1000) * 70);
                        if (red != null)
                        {
                            var pred = R.GetPrediction(red);
                            if (pred.HitChance >= HitChance.High)
                            {
                                R.Cast(pred.CastPosition);
                            }
                        }
                    }

                    if (Settings.JungleStealDrag)
                    {
                        var drag =
                            EntityManager.MinionsAndMonsters.GetJungleMonsters()
                                .FirstOrDefault(
                                    m =>
                                        Prediction.Health.GetPrediction(m, R.CastDelay + (int)(1000 * Player.Instance.Distance(m) / R.Speed)) <
                                        SpellDamage.GetRealDamage(SpellSlot.R, m) &&
                                        m.IsValidTarget(R.Range) &&
                                        m.BaseSkinName == "SRU_Dragon" && m.IsInRange(targetR, 1500) &&
                                        Prediction.Health.GetPrediction(m, R.CastDelay + (int)(1000 * Player.Instance.Distance(m) / R.Speed)) > m.CountEnemiesInRange(1000) * 70);

                        if (drag != null)
                        {
                            var pred = R.GetPrediction(drag);
                            if (pred.HitChance >= HitChance.High)
                            {
                                R.Cast(pred.CastPosition);
                            }
                        }
                    }

                    if (Settings.JungleStealBaron)
                    {
                        var baron =
                            EntityManager.MinionsAndMonsters.GetJungleMonsters()
                                .FirstOrDefault(
                                    m =>
                                        Prediction.Health.GetPrediction(m, R.CastDelay + (int)(1000 * Player.Instance.Distance(m) / R.Speed)) <
                                        SpellDamage.GetRealDamage(SpellSlot.R, m) &&
                                        m.IsValidTarget(R.Range) &&
                                        m.BaseSkinName == "SRU_Baron" && m.IsInRange(targetR, 1500) &&
                                        Prediction.Health.GetPrediction(m, R.CastDelay + (int)(1000 * Player.Instance.Distance(m) / R.Speed)) > m.CountEnemiesInRange(1000) * 70);

                        if (baron != null)
                        {
                            var pred = R.GetPrediction(baron);
                            if (pred.HitChance >= HitChance.High)
                            {
                                R.Cast(pred.CastPosition);
                            }
                        }
                    }
                }
            }
        }
    }
}
