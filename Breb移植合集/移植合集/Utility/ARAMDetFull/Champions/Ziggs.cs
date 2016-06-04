using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using EloBuddy;
using SharpDX;
using EloBuddy.SDK;

namespace ARAMDetFull.Champions
{
    class ZiggsA : Champion
    {
        public static LeagueSharp.Common.Spell Q1;
        public static LeagueSharp.Common.Spell Q2;
        public static LeagueSharp.Common.Spell Q3;

        private int UseSecondWT = 0;

        public ZiggsA()
        {
            ARAMSimulator.champBuild = new Build
            {
                coreItems = new List<ConditionalItem>
                        {
                            new ConditionalItem(ItemId.Athenes_Unholy_Grail),
                            new ConditionalItem(ItemId.Sorcerers_Shoes),
                            new ConditionalItem(ItemId.Ludens_Echo),
                            new ConditionalItem(ItemId.Rabadons_Deathcap),
                            new ConditionalItem(ItemId.Void_Staff),
                            new ConditionalItem(ItemId.Banshees_Veil),
                        },
                startingItems = new List<ItemId>
                        {
                            ItemId.Chalice_of_Harmony,ItemId.Boots_of_Speed
                        }
            };
        }

        public override void useQ(Obj_AI_Base target)
        {
            if (!Q1.IsReady() || target == null)
                return;
            CastQ(target);
        }

        public override void useW(Obj_AI_Base target)
        {
            if (!W.IsReady())
                return;
            PredictionOutput po = E.GetPrediction(target);
            var dist = po.UnitPosition.LSDistance(player.Position);
            if (dist < 900)
            {
                var pos = player.Position.LSExtend(po.UnitPosition, dist + 90);
                W.Cast(pos);
            }

        }

        public override void useE(Obj_AI_Base target)
        {
            if (!E.IsReady() || target == null)
                return;

            //var distToTar = target.LSDistance(player);

            E.Cast(target, false, true);
        }


        public override void useR(Obj_AI_Base target)
        {
            if (!R.IsReady() || target == null)
                return;
            if ((ObjectManager.Player.LSGetSpellDamage(target, SpellSlot.Q) +
                         ObjectManager.Player.LSGetSpellDamage(target, SpellSlot.W) +
                         ObjectManager.Player.LSGetSpellDamage(target, SpellSlot.E) +
                         ObjectManager.Player.LSGetSpellDamage(target, SpellSlot.R) > target.Health) &&
                        ObjectManager.Player.LSDistance(target) <= Q2.Range)
            {
                R.Delay = 2000 + 1500 * target.LSDistance(ObjectManager.Player) / 5300;
                R.Cast(target, true, true);
            }
        }

        public override void useSpells()
        {
            var tar = ARAMTargetSelector.getBestTarget(W.Range);
            if (tar != null) useW(tar);
            tar = ARAMTargetSelector.getBestTarget(Q2.Range - 100);
            if (tar != null) useQ(tar);
            
            tar = ARAMTargetSelector.getBestTarget(E.Range);
            if (tar != null) useE(tar);
            var target = ARAMTargetSelector.getBestTarget(R.Range);
            if (target != null) useR(target);


            if (Utils.TickCount - UseSecondWT < 500 &&
                ObjectManager.Player.Spellbook.GetSpell(SpellSlot.W).Name == "ziggswtoggle")
            {
                W.Cast((SharpDX.Vector2) ObjectManager.Player.ServerPosition, true);
            }

            //R aoe in teamfights
            if ( R.IsReady())
            {
                var alliesarround = 0;
                var n = 0;
                foreach (var ally in ObjectManager.Get<AIHeroClient>())
                {
                    if (ally.IsAlly && !ally.IsMe && ally.LSIsValidTarget(float.MaxValue, false) &&
                        ally.LSDistance(target) < 700)
                    {
                        alliesarround++;
                        if (Utils.TickCount - ally.LastCastedSpellT() < 1500)
                        {
                            n++;
                        }
                    }
                }

                if (n < Math.Max(alliesarround / 2 - 1, 1))
                {
                    return;
                }

                switch (alliesarround)
                {
                    case 2:
                        R.CastIfWillHit(target, 2);
                        break;
                    case 3:
                        R.CastIfWillHit(target, 3);
                        break;
                    case 4:
                        R.CastIfWillHit(target, 4);
                        break;
                }
            }

            foreach (var pos in from enemy in ObjectManager.Get<AIHeroClient>()
                                where
                                    enemy.LSIsValidTarget() &&
                                    enemy.LSDistance(ObjectManager.Player) <=
                                    enemy.BoundingRadius + enemy.AttackRange + ObjectManager.Player.BoundingRadius &&
                                    enemy.IsMelee()
                                let direction =
                                    (enemy.ServerPosition.LSTo2D() - ObjectManager.Player.ServerPosition.LSTo2D()).Normalized()
                                let pos = ObjectManager.Player.ServerPosition.LSTo2D()
                                select pos + Math.Min(200, Math.Max(50, enemy.LSDistance(ObjectManager.Player) / 2)) * direction)
            {
                W.Cast(pos.To3D(), true);
                UseSecondWT = Utils.TickCount;
            }


            target = ARAMTargetSelector.getBestTarget(Q3.Range);

            if(target == null)
                Farm(true);

        }

        public override void setUpSpells()
        {
            Q1 = new LeagueSharp.Common.Spell(SpellSlot.Q, 850f);
            Q2 = new LeagueSharp.Common.Spell(SpellSlot.Q, 1125f);
            Q3 = new LeagueSharp.Common.Spell(SpellSlot.Q, 1400f);

            W = new LeagueSharp.Common.Spell(SpellSlot.W, 1000f);
            E = new LeagueSharp.Common.Spell(SpellSlot.E, 900f);
            R = new LeagueSharp.Common.Spell(SpellSlot.R, 5300f);

            Q1.SetSkillshot(0.3f, 130f, 1700f, false, SkillshotType.SkillshotCircle);
            Q2.SetSkillshot(0.25f + Q1.Delay, 130f, 1700f, false, SkillshotType.SkillshotCircle);
            Q3.SetSkillshot(0.3f + Q2.Delay, 130f, 1700f, false, SkillshotType.SkillshotCircle);

            W.SetSkillshot(0.25f, 275f, 1750f, false, SkillshotType.SkillshotCircle);
            E.SetSkillshot(0.5f, 70f, 1750f, false, SkillshotType.SkillshotCircle);
            R.SetSkillshot(1f, 500f, float.MaxValue, false, SkillshotType.SkillshotCircle);
        }

        private void CastQ(Obj_AI_Base target)
        {
            PredictionOutput prediction;

            if (ObjectManager.Player.LSDistance(target) < Q1.Range)
            {
                var oldrange = Q1.Range;
                Q1.Range = Q2.Range;
                prediction = Q1.GetPrediction(target, true);
                Q1.Range = oldrange;
            }
            else if (ObjectManager.Player.LSDistance(target) < Q2.Range)
            {
                var oldrange = Q2.Range;
                Q2.Range = Q3.Range;
                prediction = Q2.GetPrediction(target, true);
                Q2.Range = oldrange;
            }
            else if (ObjectManager.Player.LSDistance(target) < Q3.Range)
            {
                prediction = Q3.GetPrediction(target, true);
            }
            else
            {
                return;
            }

            if (prediction.Hitchance >= HitChance.High)
            {
                if (ObjectManager.Player.ServerPosition.LSDistance(prediction.CastPosition) <= Q1.Range + Q1.Width)
                {
                    Vector3 p;
                    if (ObjectManager.Player.ServerPosition.LSDistance(prediction.CastPosition) > 300)
                    {
                        p = prediction.CastPosition -
                            100 *
                            (prediction.CastPosition.LSTo2D() - ObjectManager.Player.ServerPosition.LSTo2D()).Normalized()
                                .To3D();
                    }
                    else
                    {
                        p = prediction.CastPosition;
                    }

                    Q1.Cast(p);
                }
                else if (ObjectManager.Player.ServerPosition.LSDistance(prediction.CastPosition) <=
                         ((Q1.Range + Q2.Range) / 2))
                {
                    var p = ObjectManager.Player.ServerPosition.LSTo2D()
                        .LSExtend(prediction.CastPosition.LSTo2D(), Q1.Range - 100);

                    if (!CheckQCollision(target, prediction.UnitPosition, p.To3D()))
                    {
                        Q1.Cast(p.To3D());
                    }
                }
                else
                {
                    var p = ObjectManager.Player.ServerPosition.LSTo2D() +
                            Q1.Range *
                            (prediction.CastPosition.LSTo2D() - ObjectManager.Player.ServerPosition.LSTo2D()).Normalized
                                ();

                    if (!CheckQCollision(target, prediction.UnitPosition, p.To3D()))
                    {
                        Q1.Cast(p.To3D());
                    }
                }
            }
        }

        private void Farm(bool laneClear)
        {
            if (!Orbwalker.CanMove)
            {
                return;
            }
            if (60 >
                ObjectManager.Player.Mana / ObjectManager.Player.MaxMana * 100)
            {
                return;
            }

            var rangedMinions = MinionManager.GetMinions(
                ObjectManager.Player.ServerPosition, Q2.Range, MinionTypes.Ranged);
            var allMinions = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, Q2.Range);

            var useQi = 2;
            var useEi = 0;
            var useQ = (laneClear && (useQi == 1 || useQi == 2)) || (!laneClear && (useQi == 0 || useQi == 2));
            var useE = (laneClear && (useEi == 1 || useEi == 2)) || (!laneClear && (useEi == 0 || useEi == 2));

            if (laneClear)
            {
                if (Q1.IsReady() && useQ)
                {
                    var rangedLocation = Q2.GetCircularFarmLocation(rangedMinions);
                    var location = Q2.GetCircularFarmLocation(allMinions);

                    var bLocation = (location.MinionsHit > rangedLocation.MinionsHit + 1) ? location : rangedLocation;

                    if (bLocation.MinionsHit > 0)
                    {
                        Q2.Cast(bLocation.Position.To3D());
                    }
                }

                if (E.IsReady() && useE)
                {
                    var rangedLocation = E.GetCircularFarmLocation(rangedMinions, E.Width * 2);
                    var location = E.GetCircularFarmLocation(allMinions, E.Width * 2);

                    var bLocation = (location.MinionsHit > rangedLocation.MinionsHit + 1) ? location : rangedLocation;

                    if (bLocation.MinionsHit > 2)
                    {
                        E.Cast(bLocation.Position.To3D());
                    }
                }
            }
            else
            {
                if (useQ && Q1.IsReady())
                {
                    foreach (var minion in allMinions)
                    {
                        if (!Orbwalking.InAutoAttackRange(minion))
                        {
                            var Qdamage = ObjectManager.Player.LSGetSpellDamage(minion, SpellSlot.Q) * 0.75;

                            if (Qdamage > Q1.GetHealthPrediction(minion))
                            {
                                Q2.Cast(minion);
                            }
                        }
                    }
                }

                if (E.IsReady() && useE)
                {
                    var rangedLocation = E.GetCircularFarmLocation(rangedMinions, E.Width * 2);
                    var location = E.GetCircularFarmLocation(allMinions, E.Width * 2);

                    var bLocation = (location.MinionsHit > rangedLocation.MinionsHit + 1) ? location : rangedLocation;

                    if (bLocation.MinionsHit > 2)
                    {
                        E.Cast(bLocation.Position.To3D());
                    }
                }
            }
        }

        private bool CheckQCollision(Obj_AI_Base target, Vector3 targetPosition, Vector3 castPosition)
        {
            var direction = (castPosition.LSTo2D() - ObjectManager.Player.ServerPosition.LSTo2D()).Normalized();
            var firstBouncePosition = castPosition.LSTo2D();
            var secondBouncePosition = firstBouncePosition +
                                       direction * 0.4f *
                                       ObjectManager.Player.ServerPosition.LSTo2D().LSDistance(firstBouncePosition);
            var thirdBouncePosition = secondBouncePosition +
                                      direction * 0.6f * firstBouncePosition.LSDistance(secondBouncePosition);

            //TODO: Check for wall collision.

            if (thirdBouncePosition.LSDistance(targetPosition.LSTo2D()) < Q1.Width + target.BoundingRadius)
            {
                //Check the second one.
                foreach (var minion in ObjectManager.Get<Obj_AI_Minion>())
                {
                    if (minion.LSIsValidTarget(3000))
                    {
                        var predictedPos = Q2.GetPrediction(minion);
                        if (predictedPos.UnitPosition.LSTo2D().LSDistance(secondBouncePosition) <
                            Q2.Width + minion.BoundingRadius)
                        {
                            return true;
                        }
                    }
                }
            }

            if (secondBouncePosition.LSDistance(targetPosition.LSTo2D()) < Q1.Width + target.BoundingRadius ||
                thirdBouncePosition.LSDistance(targetPosition.LSTo2D()) < Q1.Width + target.BoundingRadius)
            {
                //Check the first one
                foreach (var minion in ObjectManager.Get<Obj_AI_Minion>())
                {
                    if (minion.LSIsValidTarget(3000))
                    {
                        var predictedPos = Q1.GetPrediction(minion);
                        if (predictedPos.UnitPosition.LSTo2D().LSDistance(firstBouncePosition) <
                            Q1.Width + minion.BoundingRadius)
                        {
                            return true;
                        }
                    }
                }

                return false;
            }

            return true;
        }
    }
}
