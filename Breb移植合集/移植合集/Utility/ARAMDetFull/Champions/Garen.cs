using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common; using EloBuddy;

namespace ARAMDetFull.Champions
{
    class Garen : Champion
    {

        public Garen()
        {
            LXOrbwalker.AfterAttack += AfterAttack;

            ARAMSimulator.champBuild = new Build
            {
                coreItems = new List<ConditionalItem>
                {
                    new ConditionalItem(ItemId.The_Black_Cleaver),
                    new ConditionalItem(ItemId.Mercurys_Treads,ItemId.Ninja_Tabi,ItemCondition.ENEMY_AP),
                    new ConditionalItem(ItemId.Sunfire_Cape, ItemId.Banshees_Veil, ItemCondition.ENEMY_AP),
                    new ConditionalItem(ItemId.Frozen_Mallet),
                    new ConditionalItem(ItemId.Spirit_Visage, ItemId.Randuins_Omen, ItemCondition.ENEMY_AP),
                    new ConditionalItem(ItemId.Last_Whisper),
                },
                startingItems = new List<ItemId>
                {
                    ItemId.Phage
                }
            };
        }

         private static bool GarenE
        {
            get
            { return player.Buffs.Any(buff => buff.Name == "GarenE"); }
        }

        private static bool GarenQ
        {
            get
            { return player.Buffs.Any(buff => buff.Name == "GarenQ"); }
        }

        private void AfterAttack(Obj_AI_Base unit, Obj_AI_Base target)
        {
            if (unit.IsMe && target is AIHeroClient && Q.IsReady() && !GarenE)
            {
                Q.Cast();
                Player.IssueOrder(GameObjectOrder.AutoAttack, target);
            }
        }

        public override void useQ(Obj_AI_Base target)
        {
            if (Q.IsReady() && !GarenE && player.LSCountEnemiesInRange(400) > 1)
                Q.Cast();
        }

        public override void useW(Obj_AI_Base target)
        {
            if (W.IsReady())
                W.Cast();
        }

        public override void useE(Obj_AI_Base target)
        {
            if (!E.IsReady() || target == null || GarenE)
                return;
            E.Cast();
        }


        public override void useR(Obj_AI_Base target)
        {
            if (!R.IsReady() || target == null)
                return;
            if (R.IsKillable(target))
                R.CastOnUnit(target);
        }

        public override void useSpells()
        {
            var tar = ARAMTargetSelector.getBestTarget(Q.Range);
            if (tar != null) useQ(tar);
            tar = ARAMTargetSelector.getBestTarget(E.Range);
            if (tar != null) useE(tar);
            tar = ARAMTargetSelector.getBestTarget(W.Range);
            if (tar != null) useW(tar);

            if (R.IsReady())
            {
                foreach (var enem in MapControl.enemy_champions.Where(ene => ene.hero.LSDistance(player)<R.Range))
                {
                    if (R.IsKillable(enem.hero))
                    {
                        R.CastOnUnit(enem.hero);
                        return;
                    }
                }

            }

        }

        public override void setUpSpells()
        {
            //Create the spells
            Q = new Spell(SpellSlot.Q, player.AttackRange);
            W = new Spell(SpellSlot.W,350);
            E = new Spell(SpellSlot.E, 330);
            R = new Spell(SpellSlot.R, 400);
        }
    }
}
