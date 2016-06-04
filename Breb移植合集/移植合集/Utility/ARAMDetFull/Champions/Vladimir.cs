using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common; using EloBuddy;

namespace ARAMDetFull.Champions
{
    class Vladimir : Champion
    {
        
        public Vladimir()
        {
            ARAMSimulator.champBuild = new Build
            {
                coreItems = new List<ConditionalItem>
                        {
                            new ConditionalItem(ItemId.Will_of_the_Ancients),
                            new ConditionalItem(ItemId.Ionian_Boots_of_Lucidity),
                            new ConditionalItem(ItemId.Spirit_Visage),
                            new ConditionalItem(ItemId.Rabadons_Deathcap),
                            new ConditionalItem(ItemId.Rylais_Crystal_Scepter),
                            new ConditionalItem(ItemId.Liandrys_Torment),
                        },
                startingItems = new List<ItemId>
                        {
                            ItemId.Hextech_Revolver
                        }
            };
        }

        public override void useQ(Obj_AI_Base target)
        {
            if (!Q.IsReady() || target == null)
                return;
            //if (!Sector.inTowerRange(target.Position.LSTo2D()) && (MapControl.balanceAroundPoint(target.Position.LSTo2D(), 700) >= -1 || (MapControl.fightIsOn() != null && MapControl.fightIsOn().NetworkId == target.NetworkId)))
                Q.CastOnUnit(target);
        }

        public override void useW(Obj_AI_Base target)
        {
            if (!W.IsReady())
                return;
            if (player.LSCountEnemiesInRange(400) > 1 || (player.HealthPercent < 25 && player.LSCountEnemiesInRange(700)>0))
                W.Cast();
        }

        public override void useE(Obj_AI_Base target)
        {
            if (!E.IsReady() || target == null)
                return;
            E.Cast();
        }


        public override void useR(Obj_AI_Base target)
        {
            if (!R.IsReady() || target == null)
                return;
            if (target.LSIsValidTarget(R.Range))
            {
                R.CastIfWillHit(target, 2);
            }
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

        public override void setUpSpells()
        {
            //Create the spells
            Q = new Spell(SpellSlot.Q, 650);
            W = new Spell(SpellSlot.W, 700);
            E = new Spell(SpellSlot.E, 610); //1000?
            R = new Spell(SpellSlot.R, 625); //300?

            R.SetSkillshot(0.25f, 175, 700, false, SkillshotType.SkillshotCircle);
        }

        public override void farm()
        {
            var mins = MinionManager.GetMinions(W.Range - 50);
            if (Q.IsReady() && mins.Count > 0)
            {
                Q.Cast(mins.FirstOrDefault());
            }
            if (E.IsReady() && player.HealthPercent>30 && mins.Count>0)
            {
                E.Cast();
            }
        }
    }
}
