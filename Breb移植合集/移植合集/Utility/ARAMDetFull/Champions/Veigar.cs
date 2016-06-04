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
    class Veigar : Champion
    {

        public Veigar()
        {
            ARAMSimulator.champBuild = new Build
            {
                coreItems = new List<ConditionalItem>
                        {
                            new ConditionalItem(ItemId.Void_Staff),
                            new ConditionalItem(ItemId.Sorcerers_Shoes),
                            new ConditionalItem(ItemId.Rabadons_Deathcap),
                            new ConditionalItem(ItemId.Ludens_Echo),
                            new ConditionalItem(ItemId.Zhonyas_Hourglass),
                            new ConditionalItem(ItemId.Banshees_Veil),
                        },
                startingItems = new List<ItemId>
                        {
                            ItemId.Blasting_Wand,ItemId.Boots_of_Speed
                        }
            };
        }

        public override void useQ(Obj_AI_Base target)
        {
            if (!Q.IsReady() || target == null)
                return;
            Q.Cast(target);
        }

        public override void useW(Obj_AI_Base target)
        {
            if (!W.IsReady() || target == null)
                return;
            W.Cast(target);
        }

        public override void useE(Obj_AI_Base target)
        {
            if (!E.IsReady() || target == null)
                return;
            Vector3? cagePos = GetCageCastPosition(target);
            if (cagePos != null)
                E.Cast((Vector3)cagePos);

        }


        public override void useR(Obj_AI_Base target)
        {
            if (!R.IsReady() || target == null)
                return;
            if (target.Health > 50 && target.Health < R.GetDamage(target) + 100)
                R.CastOnUnit(target);
        }

        public override void useSpells()
        {
            if (W.IsReady())
            {
                var target =
                    ObjectManager.Get<AIHeroClient>()
                        .FirstOrDefault(h => h.LSIsValidTarget(W.Range) && h.GetStunDuration() >= W.Delay - 0.5f);
                if (target != null)
                    W.Cast(target);
            }

            var tar = ARAMTargetSelector.getBestTarget(Q.Range);
            if (tar != null) useQ(tar);
            else LastHitQ(true);
            tar = ARAMTargetSelector.getBestTarget(W.Range);
            if (tar != null) useW(tar);
            tar = ARAMTargetSelector.getBestTarget(E.Range + 300);
            if (tar != null) useE(tar);
            tar = ARAMTargetSelector.getBestTarget(R.Range);
            if (tar != null) useR(tar);
        }

        private static float _widthSqr;
        private static float _radius;
        private static float _radiusSqr;

        public override void setUpSpells()
        {
            //Create the spells
            Q = new LeagueSharp.Common.Spell(SpellSlot.Q, 950);
            W = new LeagueSharp.Common.Spell(SpellSlot.W, 900);
            E = new LeagueSharp.Common.Spell(SpellSlot.E, 650);
            R = new LeagueSharp.Common.Spell(SpellSlot.R, 650);

            // Finetune spells
            //Q.SetTargetted(0.25f, 1500);
            Q.SetSkillshot(0.25f, 70f, 2000f, false, SkillshotType.SkillshotLine);
            W.SetSkillshot(1.25f, 125, float.MaxValue, false, SkillshotType.SkillshotCircle);
            E.Width = 700;
            _widthSqr = E.Width * E.Width;
            _radius = E.Width / 2;
            _radiusSqr = _radius * _radius;
            R.SetTargetted(0.25f, 1400);
        }


        public Vector3? GetCageCastPosition(Obj_AI_Base target)
        {
            // Get target position after 0.2 seconds
            var prediction = LeagueSharp.Common.Prediction.GetPrediction(target, 0.2f);

            // Validate single cast position
            if (prediction.Hitchance < HitChance.High)
                return null;

            // Check if there are other targets around that could be stunned
            var nearTargets = ObjectManager.Get<AIHeroClient>().Where(
                h =>
                    h.NetworkId != target.NetworkId &&
                    h.LSIsValidTarget(E.Range + _radius) &&
                    h.LSDistance(target, true) < _widthSqr);

            foreach (var target2 in nearTargets)
            {
                // Get target2 position after 0.2 seconds
                var prediction2 = LeagueSharp.Common.Prediction.GetPrediction(target2, 0.5f);

                // Validate second cast position
                if (prediction2.Hitchance < HitChance.High ||
                    prediction.UnitPosition.LSDistance(prediction2.UnitPosition, true) > _widthSqr)
                    continue;

                // Calculate middle point and perpendicular
                var distanceSqr = prediction.UnitPosition.LSDistance(prediction2.UnitPosition, true);
                var distance = Math.Sqrt(distanceSqr);
                var middlePoint = (prediction.UnitPosition + prediction2.UnitPosition) / 2;
                var perpendicular = (prediction.UnitPosition - prediction2.UnitPosition).Normalized().LSTo2D().LSPerpendicular();

                // Calculate cast poistion
                var length = (float)Math.Sqrt(_radiusSqr - distanceSqr);
                var castPosition = middlePoint.LSTo2D() + perpendicular * length;

                // Validate cast position
                if (castPosition.LSDistance(player.Position.LSTo2D(), true) > _radiusSqr)
                    castPosition = middlePoint.LSTo2D() - perpendicular * length;
                // Validate again, if failed continue for loop
                if (castPosition.LSDistance(player.Position.LSTo2D(), true) > _radiusSqr)
                    continue;

                // Found valid second cast position
                return castPosition.To3D();
            }

            // Returning single cast position
            return prediction.UnitPosition.LSExtend(player.Position, _radius);
        }

        public void farmQ()
        {
            if (player.ManaPercent < 35 || !Q.IsReady())
                return;

            var mins = MinionManager.GetMinions(Q.Range).Where(min => min.Health < Q.GetDamage(min)).ToList();
            var positions = MinionManager.GetMinionsPredictedPositions(mins, Q.Delay, Q.Width, Q.Speed, Q.From, Q.Range, true, SkillshotType.SkillshotLine);


            var minsQ = MinionManager.GetBestLineFarmLocation(positions, Q.Width, Q.Range);
            if (minsQ.MinionsHit != 0)
                Q.Cast(minsQ.Position);

        }

        private void LastHitQ(bool auto = false)
        {
            if (!Q.IsReady())
            {
                return;
            }
            var minions =
                MinionManager.GetMinions(Q.Range, MinionTypes.All, MinionTeam.NotAlly)
                    .Where(
                        m =>
                            m.LSDistance(player) < Q.Range);
            var objAiBases = minions as Obj_AI_Base[] ?? minions.ToArray();
            if (objAiBases.Any())
            {
                foreach (var minion in objAiBases)
                {
                    float minPHP = HealthPrediction.GetHealthPrediction(minion,
                        (int)(player.LSDistance(minion) / Q.Speed));
                    if (minPHP <= 0 || minPHP > Q.GetDamage(minion))
                        continue;
                    var collision = Q.GetCollision(
                        player.Position.LSTo2D(), new List<Vector2>() { player.Position.LSExtend(minion.Position, Q.Range).LSTo2D() }, 70f);
                    if (collision.Count <= 2 || collision[0].NetworkId == minion.NetworkId || collision[1].NetworkId == minion.NetworkId)
                    {
                        if (collision.Count == 1)
                        {
                            Q.Cast(minion);
                        }
                        else
                        {
                            var other = collision.FirstOrDefault(c => c.NetworkId != minion.NetworkId);
                            if (other != null &&
                                (player.LSGetAutoAttackDamage(other) * 2 > other.Health - Q.GetDamage(other)) &&
                                HealthPrediction.GetHealthPrediction(minion, 1500) > 0 &&
                                Q.GetDamage(other) < other.Health)
                            {
                                if (Orbwalker.CanAutoAttack)
                                {
                                    Player.IssueOrder(GameObjectOrder.AutoAttack, other);
                                }
                            }
                            else
                            {
                                Q.Cast(minion);
                            }
                        }
                    }
                }
            }
        }
    }
}
