using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common; using EloBuddy;
using SharpDX;

namespace ARAMDetFull.Champions
{
    class Brad : Champion
    {
        public Spell stunQ;

        public Brad()
        {
            ARAMSimulator.champBuild = new Build
            {
                coreItems = new List<ConditionalItem>
                {
                    new ConditionalItem(ItemId.Athenes_Unholy_Grail),
                    new ConditionalItem(ItemId.Ionian_Boots_of_Lucidity),
                    new ConditionalItem(ItemId.Lich_Bane),
                    new ConditionalItem(ItemId.Rabadons_Deathcap),
                    new ConditionalItem(ItemId.Void_Staff),
                    new ConditionalItem(ItemId.Banshees_Veil),
                },
                startingItems = new List<ItemId>
                {
                    ItemId.Boots_of_Speed,ItemId.Chalice_of_Harmony
                }
            };
        }

        public override void useQ(Obj_AI_Base target)
        {
            if (!Q.IsReady() || target == null)
                return;
            if (target.LSIsValidTarget())
            {
                if (!castStunQ(target))
                {
                    Q.Cast(target);
                }
            }
        }

        public override void useW(Obj_AI_Base target)
        {
            if (!W.IsReady())
                return;
            if (player.Mana > 270)
                W.Cast(player.Position.LSExtend(ARAMSimulator.fromNex.Position, 645));
        }

        public override void useE(Obj_AI_Base target)
        {
           
        }


        public override void useR(Obj_AI_Base target)
        {
            if (!R.IsReady() || target == null)
                return;
            if (GetEnemys(target) >= 3)
            {
                R.Cast(target, false, true);
            }
        }

        public override void useSpells()
        {
            var tar = ARAMTargetSelector.getBestTarget(Q.Range);
            if (tar != null) useQ(tar);
            tar = ARAMTargetSelector.getBestTarget(W.Range);
            useW(tar);
            tar = ARAMTargetSelector.getBestTarget(E.Range);
            if (tar != null) useE(tar);
            tar = ARAMTargetSelector.getBestTarget(R.Range);
            if (tar != null) useR(tar);
        }

        public override void setUpSpells()
        {
            //Create the spells

            Q = new Spell(SpellSlot.Q, 900f);
            W = new Spell(SpellSlot.W, 900f);
            E = new Spell(SpellSlot.E, 1000f);
            R = new Spell(SpellSlot.R, 2500f);
            stunQ = new Spell(SpellSlot.Q, Q.Range);

            Q.SetSkillshot(0.25f, 60, 1200, false, SkillshotType.SkillshotLine);
            R.SetSkillshot(0.5f, 325, 1800, false, SkillshotType.SkillshotCircle);
            stunQ.SetSkillshot(Q.Delay, Q.Width, Q.Speed, true, SkillshotType.SkillshotLine);
        }



        private int GetEnemys(Obj_AI_Base target)
        {
            int Enemys = 0;
            foreach (AIHeroClient enemys in ObjectManager.Get<AIHeroClient>())
            {
                var pred = R.GetPrediction(enemys, true);
                if (pred.Hitchance >= HitChance.High && !enemys.IsMe && enemys.IsEnemy && Vector3.Distance(player.Position, pred.UnitPosition) <= R.Range)
                {
                    Enemys = Enemys + 1;
                }
            }
            return Enemys;
        }
        private bool castStunQ(Obj_AI_Base target)
        {
            var prediction = stunQ.GetPrediction(target);

            var direction = (player.ServerPosition - prediction.UnitPosition).LSNormalized();
            var endOfQ = (Q.Range) * direction;

            var checkPoint = prediction.UnitPosition.LSExtend(player.ServerPosition, -Q.Range / 4);

            if ((prediction.UnitPosition.GetFirstWallPoint(checkPoint).HasValue) || (prediction.CollisionObjects.Count == 1))
            {
                Q.Cast(prediction.UnitPosition);
                return true;
            }
            return false;
        }

    }

    public static class VectorHelper
    {
        public static Vector3? GetFirstWallPoint(this Vector3 from, Vector3 to, float step = 25)
        {
            var direction = (to - from).LSNormalized();

            for (float d = 0; d < from.LSDistance(to); d = d + step)
            {
                var testPoint = from + d * direction;
                if (NavMesh.GetCollisionFlags(testPoint.X, testPoint.Y).HasFlag(CollisionFlags.Wall) ||
                    NavMesh.GetCollisionFlags(testPoint.X, testPoint.Y).HasFlag(CollisionFlags.Building))
                {
                    return from + d * direction;
                }
            }

            return null;
        }

        public static Vector2? GetFirstWallPoint(this Vector2 from, Vector2 to, float step = 25)
        {
            var direction = (to - from).LSNormalized();

            for (float d = 0; d < from.LSDistance(to); d = d + step)
            {
                var testPoint = from + d * direction;
                if (NavMesh.GetCollisionFlags(testPoint.X, testPoint.Y).HasFlag(CollisionFlags.Wall) ||
                    NavMesh.GetCollisionFlags(testPoint.X, testPoint.Y).HasFlag(CollisionFlags.Building))
                {
                    return from + d * direction;
                }
            }

            return null;
        }
    }
}
