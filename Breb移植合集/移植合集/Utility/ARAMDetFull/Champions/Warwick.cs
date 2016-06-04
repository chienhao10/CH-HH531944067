using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common; using EloBuddy;

namespace ARAMDetFull.Champions
{
    class WarwickA : Champion
    {


        public WarwickA()
        {
            LXOrbwalker.AfterAttack += aAtack;

            ARAMSimulator.champBuild = new Build
            {
                coreItems = new List<ConditionalItem>
                        {
                            new ConditionalItem(ItemId.Sunfire_Cape),
                            new ConditionalItem(ItemId.Mercurys_Treads,ItemId.Ninja_Tabi,ItemCondition.ENEMY_AP),
                            new ConditionalItem(ItemId.Trinity_Force),
                            new ConditionalItem(ItemId.Wits_End),
                            new ConditionalItem(ItemId.Randuins_Omen),
                            new ConditionalItem(ItemId.Banshees_Veil),
                        },
                startingItems = new List<ItemId>
                        {
                            ItemId.Giants_Belt
                        }
            };
        }

        private void aAtack(Obj_AI_Base unit, Obj_AI_Base target)
        {
            if (!W.IsReady())
            {
                return;
            }
            if (target != null && target is AIHeroClient && unit.IsMe)
                W.Cast();
        }


        public override void useQ(Obj_AI_Base target)
        {
            if (Q.IsReady() && target != null)
                Q.CastOnUnit(target);
        }

        public override void useW(Obj_AI_Base target)
        {
        }

        public override void useE(Obj_AI_Base target)
        {
        }


        public override void useR(Obj_AI_Base target)
        {
            if (!R.IsReady() || target == null || target.MagicImmune)
                return;
            if ((!Sector.inTowerRange(target.Position.LSTo2D()) &&
                 MapControl.balanceAroundPoint(target.Position.LSTo2D(), 700) >= -1) ||
                (MapControl.fightIsOn() != null && MapControl.fightIsOn().NetworkId == target.NetworkId))
            {
                R.Cast(target);
                return;
            }

            if (target.HealthPercent < 30)
                R.Cast(target);
        }

        public override void useSpells()
        {
            var tar = ARAMTargetSelector.getBestTarget(Q.Range);
            if (tar != null) useQ(tar);
            tar = ARAMTargetSelector.getBestTarget(R.Range);
            if (tar != null) useR(tar);

        }

        public override void farm()
        {
            base.farm();
        }

        public override void setUpSpells()
        {
            //Create the spells
            Q = new Spell(SpellSlot.Q, 400);
            W = new Spell(SpellSlot.W, 1250);
            R = new Spell(SpellSlot.R, 700);
        }


    }
}
