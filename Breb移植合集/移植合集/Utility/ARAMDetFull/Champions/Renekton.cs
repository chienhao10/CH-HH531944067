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
    class Renekton : Champion
    {


        public Renekton()
        {
            LXOrbwalker.BeforeAttack += Orbwalking_BeforeAttack;

            ARAMSimulator.champBuild = new Build
            {
                coreItems = new List<ConditionalItem>
                {
                    new ConditionalItem(ItemId.Ravenous_Hydra_Melee_Only),
                    new ConditionalItem(ItemId.Mercurys_Treads, ItemId.Ninja_Tabi, ItemCondition.ENEMY_AP),
                    new ConditionalItem(ItemId.Spirit_Visage, ItemId.Randuins_Omen, ItemCondition.ENEMY_AP),
                    new ConditionalItem(ItemId.Last_Whisper),
                    new ConditionalItem(ItemId.The_Black_Cleaver),
                    new ConditionalItem(ItemId.Banshees_Veil),
                },
                startingItems = new List<ItemId>
                {
                    ItemId.Pickaxe,ItemId.Boots_of_Speed
                }
            };
        }

        private void Orbwalking_BeforeAttack(LXOrbwalker.BeforeAttackEventArgs args)
        {
            Obj_AI_Base target = args.Target;
            if(target is AIHeroClient)
                if (W.IsReady() && args.Target == target)
                    W.Cast();
        }


        public override void useQ(Obj_AI_Base target)
        {
            if (!Q.IsReady())
                return;
                Q.Cast();
        }

        public override void useW(Obj_AI_Base target)
        {
            if (!W.IsReady())
                return;
            //W.Cast();
        }


        public override void useE(Obj_AI_Base target)
        {
            if (!E.IsReady() || W.IsReady())
                return;
            if (!Sector.inTowerRange(target.Position.LSTo2D()) &&
                (MapControl.balanceAroundPoint(target.Position.LSTo2D(), 700) >= -1 ||
                 (MapControl.fightIsOn() != null && MapControl.fightIsOn().NetworkId == target.NetworkId)))
            E.Cast(target);
        }

        public override void useR(Obj_AI_Base target)
        {
            if (!R.IsReady())
                return;
            if(player.HealthPercent<50)
            R.Cast();
        }

        public override void setUpSpells()
        {
            Q = new Spell(SpellSlot.Q, 255);
            W = new Spell(SpellSlot.W,200);
            E = new Spell(SpellSlot.E, 450);
            R = new Spell(SpellSlot.R,350);
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
