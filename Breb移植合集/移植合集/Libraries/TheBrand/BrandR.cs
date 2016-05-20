using System.Collections.Generic;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using LeagueSharp.Common;
using PortAIO.Champion.Brand;
using TheBrand.Commons.ComboSystem;

namespace TheBrand
{
    internal class BrandR : Skill // 550 bounce range
    {
        private readonly float[] _maxDamage = {450, 750, 1050};
        private BrandE _brandE;
        private BrandQ _brandQ;
        private Skill[] _otherSkills;


        public BrandR(SpellSlot slot, float range = 3.402823E+38f, DamageType damageType = DamageType.Physical)
            : base(slot, range, damageType)
        {
            OnlyUpdateIfTargetValid = true;
        }

        private float MaxDamage
        {
            get { return Level > 0 ? _maxDamage[Level - 1] + ObjectManager.Player.TotalMagicalDamage*1.5f : 0; }
        } //TODO: check if TotalMagicDamage is really AP

        public override void Initialize(ComboProvider combo)
        {
            var skills = combo.GetSkills().ToList();
            skills.Remove(this);
            _otherSkills = skills.ToArray();
            _brandE = combo.GetSkill<BrandE>();
            _brandQ = combo.GetSkill<BrandQ>();
            base.Initialize(combo);
        }

        public override void Execute(AIHeroClient target)
        {
            if (target == null)
                target = TargetSelector.GetTarget(1200, DamageType.Magical);
            if (target == null) return;

            var dmgPerBounce = ObjectManager.Player.GetSpellDamage(target, Slot) + BrandCombo.GetPassiveDamage(target);
            if (dmgPerBounce > target.Health && target.LSDistance(ObjectManager.Player) > 750)
            {
                TryBridgeUlt(target);
            }

            if (dmgPerBounce > target.Health && !Provider.ShouldBeDead(target) &&
                ObjectManager.Player.GetAutoAttackDamage(target, true) < target.Health &&
                (_otherSkills.All(
                    skill => skill.Instance.State != SpellState.Ready && skill.Instance.State != SpellState.Surpressed
                    /*&& !skill.IsSafeCasting(1)*/) ||
                 ObjectManager.Player.GetSpellDamage(target, SpellSlot.Q) + dmgPerBounce +
                 ObjectManager.Player.GetAutoAttackDamage(target) > target.Health && !target.HasBuff("brandablaze") &&
                 target.LSDistance(ObjectManager.Player) < 750))
            {
                if (ObjectManager.Player.HealthPercent - target.HealthPercent < Program.getRMenuSL("healthDifference") ||
                    !Program.getRMenuCB("DontRwith") ||
                    Program.getRMenuCB("Ignorewhenfleeing") &&
                    target.LSDistance(ObjectManager.Player) > ObjectManager.Player.AttackRange)
                {
                    Cast(target);
                }
                else if (ObjectManager.Player.HealthPercent < target.HealthPercent)
                {
                    Cast(target);
                }
            }


            // if (target.LSDistance(ObjectManager.Player) > 750) return;
            var inBounce = new bool[HeroManager.Enemies.Count];
            var canBounce = BounceCheck(target, inBounce);
            if (canBounce)
            {
                var inBounceEnemies =
                    HeroManager.Enemies.Where(enemy => inBounce[HeroManager.Enemies.IndexOf(enemy)]).ToArray();
                var distance = target.LSDistance(ObjectManager.Player);

                var bounceCount = inBounce.Count(item => item);
                if (bounceCount <= 1) return;
                //Console.WriteLine("bounce r " + bounceCount);

                if (inBounceEnemies.Any(enemy => dmgPerBounce > enemy.Health && MaxDamage > enemy.Health) &&
                    (bounceCount == 2 || Program.getRMenuCB("RiskyR")))
                {
                    TryUlt(target, inBounceEnemies, distance);
                }
                else if (bounceCount == 2 && dmgPerBounce*3 > target.Health && MaxDamage > target.Health &&
                         distance < 750 && Program.getRMenuCB("RiskyR"))
                {
                    Cast(target);
                }
                else if (dmgPerBounce*2 > target.Health && MaxDamage > target.Health)
                {
                    TryUlt(target, inBounceEnemies, distance);
                }
                else if (Program.getRMenuCB("Ultnonkillable") && Program.getRMenuSL("whenminXtargets") <= bounceCount)
                {
                    TryUlt(target, inBounceEnemies, distance, false);
                }
            }
        }

        private void TryUlt(AIHeroClient target, IEnumerable<AIHeroClient> alternate, float distance,
            bool bridgeUlt = true)
        {
            if (distance > 750)
            {
                var alternateTarget = alternate.FirstOrDefault(enemy => enemy.LSDistance(ObjectManager.Player) < 750);
                if (alternateTarget == null && bridgeUlt)
                {
                    TryBridgeUlt(target);
                }
                else
                {
                    Cast(alternateTarget);
                }
            }
            else
                Cast(target);
        }

        private void TryBridgeUlt(AIHeroClient target)
        {
            if (!Program.getRMenuCB("BridgeR")) return;

            #region bridge ult

            if (target.LSDistance(ObjectManager.Player) > 750 &&
                (_brandE.Instance.State == SpellState.Ready || _brandQ.Instance.State == SpellState.Ready))
            {
                var bridgeSpellSlot = _brandE.Instance.State == SpellState.Ready ? _brandE.Slot : _brandQ.Slot;
                var bridgeSpellRange = bridgeSpellSlot == SpellSlot.E ? 650 : 1000;
                Obj_AI_Base bridgeUnit = null;
                var bridgeUnitDistance = 0f;
                var minions = MinionManager.GetMinions(650);
                if (minions != null && minions.Count > 0)
                {
                    var unit = GetMinimumDistanceUnit(target, minions, bridgeSpellSlot);
                    if (unit != null && unit.LSDistance(target) < 500)
                    {
                        bridgeUnit = unit;
                        bridgeUnitDistance = unit.LSDistance(ObjectManager.Player);
                    }
                }
                if (bridgeUnit == null)
                {
                    var unit = GetMinimumDistanceUnit(target,
                        HeroManager.Enemies.Where(enemy => enemy.LSDistance(ObjectManager.Player) < bridgeSpellRange),
                        bridgeSpellSlot);
                    if (unit != null && unit.LSDistance(target) < 500)
                    {
                        bridgeUnit = unit;
                        bridgeUnitDistance = unit.LSDistance(ObjectManager.Player);
                    }
                }


                if (bridgeUnit != null)
                {
                    if (bridgeSpellSlot == SpellSlot.E && bridgeUnitDistance < 650)
                    {
                        _brandE.Cast(bridgeUnit);
                        QueueCast(() =>
                        {
                            if (bridgeUnit.HasBuff("brandablaze"))
                                Cast(bridgeUnit);
                        });
                    }
                    else
                    {
                        var prediction = _brandQ.GetPrediction(bridgeUnit);
                        if (prediction.CollisionObjects.Count == 0)
                        {
                            _brandQ.QueueCast(prediction.CastPosition);
                            QueueCast(() =>
                            {
                                if (bridgeUnit.HasBuff("brandablaze"))
                                    Cast(bridgeUnit);
                            });
                        }
                        else
                        {
                            var collidingObj = prediction.CollisionObjects.First();
                            if (collidingObj.LSDistance(target) < 500)
                            {
                                _brandQ.Cast(prediction.CastPosition);
                                QueueCast(() =>
                                {
                                    if (bridgeUnit.HasBuff("brandablaze"))
                                        Cast(bridgeUnit);
                                });
                            }
                        }
                    }
                }
            }

            #endregion
        }


        private bool BounceCheck(AIHeroClient target, bool[] inBounce)
        {
            for (var i = 0; i < HeroManager.Enemies.Count; i++)
            {
                if (!inBounce[i] && HeroManager.Enemies[i].LSDistance(target) < 500 &&
                    HeroManager.Enemies[i].IsValidTarget())
                {
                    if (!HeroManager.Enemies[i].HasBuff("brandablaze"))
                    {
                        var minions = MinionManager.GetMinions(target.Position, 500);
                        if (minions.Any(minion => !minion.HasBuff("brandablaze")))
                            return false;
                    }
                    inBounce[i] = true;
                    var ret = BounceCheck(HeroManager.Enemies[i], inBounce);
                    if (!ret)
                        return false;
                }
            }
            return true;
        }

        private Obj_AI_Base GetMinimumDistanceUnit(AIHeroClient target, IEnumerable<Obj_AI_Base> units, SpellSlot bridge)
        {
            var minDistance = float.MaxValue;
            Obj_AI_Base minUnit = null;
            foreach (var objAiBase in units)
            {
                var distance = objAiBase.LSDistance(target);
                if (
                    !(distance < minDistance &&
                      ObjectManager.Player.GetSpellDamage(objAiBase, bridge) < objAiBase.Health)) continue;
                minDistance = distance;
                minUnit = objAiBase;
            }
            return minUnit;
        }


        public override int GetPriority()
        {
            return 5;
        }
    }
}