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
    class Thresh : Champion
    {
        private const int QFollowTime = 3000;
        private Obj_AI_Base QTarget;
        private int QTick;

        public Thresh()
        {
            ARAMSimulator.champBuild = new Build
            {
                coreItems = new List<ConditionalItem>
                        {
                            new ConditionalItem(ItemId.Sunfire_Cape),
                            new ConditionalItem(ItemId.Mercurys_Treads,ItemId.Ninja_Tabi,ItemCondition.ENEMY_AP),
                            new ConditionalItem(ItemId.Spirit_Visage),
                            new ConditionalItem(ItemId.Iceborn_Gauntlet),
                            new ConditionalItem(ItemId.Randuins_Omen),
                            new ConditionalItem(ItemId.Banshees_Veil),
                        },
                startingItems = new List<ItemId>
                        {
                            ItemId.Giants_Belt
                        }
            };
        }

        private bool FollowQ
        {
            get { return LXOrbwalker.now <= QTick + QFollowTime; }
        }

        private bool FollowQBlock
        {
            get { return LXOrbwalker.now - QTick >= QFollowTime; }
        }


        public override void useQ(Obj_AI_Base target)
        {
            if (!Q.IsReady() || target == null)
                return;
            if (FollowQBlock)
            {
                if (Q.Cast(target) == Spell.CastStates.SuccessfullyCasted)
                {
                    QTick = LXOrbwalker.now;
                    QTarget = target;
                }
            }
            if (FollowQ && !Sector.inTowerRange(target.Position.LSTo2D()))
            {
                Q.Cast();
            }
        }

        public override void useW(Obj_AI_Base target)
        {
            if (!W.IsReady())
            EngageFriendLatern();
        }

        public override void useE(Obj_AI_Base target)
        {
            if (!E.IsReady() || target == null)
                return;
            if (AllyBelowHp(25, E.Range) != null)
            {
                E.Cast(target.Position);
            }
            else
            {
                E.Cast(ReversePosition(ObjectManager.Player.Position, target.Position));
            }
        }



        public override void useR(Obj_AI_Base target)
        {
            if (target == null)
                return;
            if (target.LSIsValidTarget(R.Range) && R.IsReady() && EnemyInRange(2,400))
            {
                R.Cast();
            }
        }

        public override void useSpells()
        {
            var tar = ARAMTargetSelector.getBestTarget(Q.Range);
            useQ(tar);
            tar = ARAMTargetSelector.getBestTarget(W.Range);
            useW(tar);
            tar = ARAMTargetSelector.getBestTarget(E.Range);
            useE(tar);
            tar = ARAMTargetSelector.getBestTarget(R.Range);
            useR(tar);
        }



        public override void setUpSpells()
        {
            Q = new Spell(SpellSlot.Q, 1025);
            W = new Spell(SpellSlot.W, 950);
            E = new Spell(SpellSlot.E, 400);
            R = new Spell(SpellSlot.R, 400);

            Q.SetSkillshot(0.5f, 70f, 1900, true, SkillshotType.SkillshotCircle);
        }

        public static bool EnemyInRange(int numOfEnemy, float range)
        {
            return LeagueSharp.Common.Utility.LSCountEnemiesInRange(ObjectManager.Player, (int)range) >= numOfEnemy;
        }

        public static Vector3 ReversePosition(Vector3 positionMe, Vector3 positionEnemy)
        {
            var x = positionMe.X - positionEnemy.X;
            var y = positionMe.Y - positionEnemy.Y;
            return new Vector3(positionMe.X + x, positionMe.Y + y, positionMe.Z);
        }

        public static AIHeroClient AllyBelowHp(int percentHp, float range)
        {
            foreach (var ally in ObjectManager.Get<AIHeroClient>())
            {
                if (ally.IsMe)
                {
                    if (((ObjectManager.Player.Health / ObjectManager.Player.MaxHealth) * 100) < percentHp)
                    {
                        return ally;
                    }
                }
                else if (ally.IsAlly)
                {
                    if (Vector3.Distance(ObjectManager.Player.Position, ally.Position) < range &&
                        ((ally.Health / ally.MaxHealth) * 100) < percentHp)
                    {
                        return ally;
                    }
                }
            }

            return null;
        }

        public void OnPossibleToInterrupt(Obj_AI_Base unit, InterruptableSpell spell)
        {
            if (spell.DangerLevel < InterruptableDangerLevel.High || unit.IsAlly)
            {
                return;
            }
            if (E.IsReady() && E.IsInRange(unit))
                E.Cast(unit.Position);
        }


        private void EngageFriendLatern()
        {
            if (!W.IsReady())
            {
                return;
            }


            foreach (var friend in
                ObjectManager.Get<AIHeroClient>()
                    .Where(
                        hero =>
                            hero.IsAlly && hero.LSDistance(player) <= W.Range + 200  && hero.Health / hero.MaxHealth * 100 >= 10 &&
                            hero.LSCountEnemiesInRange(350) >= 1))
            {
                 W.Cast(friend.Position, true);
                 return;
            }
        }
    }
}
