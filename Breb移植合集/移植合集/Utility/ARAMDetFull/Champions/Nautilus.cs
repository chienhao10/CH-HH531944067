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
    class Nautilus : Champion
    {
        

        public Nautilus()
        {
            Interrupter.OnPossibleToInterrupt += InterrupterOnOnPossibleToInterrupt;

            ARAMSimulator.champBuild = new Build
            {
                coreItems = new List<ConditionalItem>
                        {
                            new ConditionalItem(ItemId.Sunfire_Cape),
                            new ConditionalItem(ItemId.Mercurys_Treads,ItemId.Ninja_Tabi,ItemCondition.ENEMY_AP),
                            new ConditionalItem(ItemId.Iceborn_Gauntlet),
                            new ConditionalItem(ItemId.Trinity_Force),
                            new ConditionalItem(ItemId.Frozen_Heart),
                            new ConditionalItem(ItemId.Banshees_Veil),
                        },
                startingItems = new List<ItemId>
                        {
                            ItemId.Giants_Belt
                        }
            };
        }

        private void InterrupterOnOnPossibleToInterrupt(Obj_AI_Base unit, InterruptableSpell spell)
        {
            if (spell.DangerLevel != InterruptableDangerLevel.High)
            {
                return;
            }


            if (Q.IsReady() & Q.IsInRange(unit, Q.Range))
            {
                Q.Cast(unit);
            }


            if (R.IsReady() && R.IsInRange(unit, R.Range))
            {
                R.Cast(unit);
            }


        }

        public override void useQ(Obj_AI_Base target)
        {
            if (!Q.IsReady())
                return;
            if (safeGap(target))
            {
                var hitchance = Q.GetPrediction(target, false, 0,
                    new[]
                    {
                        CollisionableObjects.Heroes, CollisionableObjects.Minions, CollisionableObjects.Walls,
                        CollisionableObjects.YasuoWall
                    }).Hitchance;

                if (hitchance >= HitChance.Medium)
                {
                    Q.Cast(target);
                }
            }
        }

        public override void useW(Obj_AI_Base target)
        {
            if (!W.IsReady())
                return;
            W.Cast();
        }

        public override void useE(Obj_AI_Base target)
        {
            if (!E.IsReady())
                return;
                E.Cast();
        }

        public override void useR(Obj_AI_Base target)
        {
            if (!R.IsReady())
                return;
            R.Cast(target);
        }

        public override void setUpSpells()
        {
            Q = new Spell(SpellSlot.Q, 950f);
            Q.SetSkillshot(0.25f, 90f, 2000f, true, SkillshotType.SkillshotLine);
           
            W = new Spell(SpellSlot.W, 300f);
            E = new Spell(SpellSlot.E, 350f);
            R = new Spell(SpellSlot.R, 825f);
        }

        public override void useSpells()
        {
            var tar = ARAMTargetSelector.getBestTarget(Q.Range);
            if (tar != null) useQ(tar);
            tar = ARAMTargetSelector.getBestTarget(W.Range);
            if (tar != null) useW(tar);
            tar = ARAMTargetSelector.getBestTarget(E.Range);
            if (tar != null) useE(tar);
            tar = ARAMTargetSelector.getBestTarget(R.Range);
            if (tar != null) useR(tar);
        }
    }
}
