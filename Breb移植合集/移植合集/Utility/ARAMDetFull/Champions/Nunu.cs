using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common; using EloBuddy;

namespace ARAMDetFull.Champions
{
    class Nunu : Champion
    {

        public Nunu()
        {
            ARAMSimulator.champBuild = new Build
            {
                coreItems = new List<ConditionalItem>
                        {
                            new ConditionalItem(ItemId.Rod_of_Ages),
                            new ConditionalItem(ItemId.Sorcerers_Shoes),
                            new ConditionalItem(ItemId.Liandrys_Torment),
                            new ConditionalItem(ItemId.Rabadons_Deathcap),
                            new ConditionalItem(ItemId.Void_Staff),
                            new ConditionalItem(ItemId.Banshees_Veil),
                        },
                startingItems = new List<ItemId>
                        {
                            ItemId.Catalyst_the_Protector
                        }
            };
        }

        public override void useQ(Obj_AI_Base target)
        {
        }

        public override void useW(Obj_AI_Base target)
        {
            if (!W.IsReady() || target == null)
                return;
            W.Cast();
        }

        public override void useE(Obj_AI_Base target)
        {
            if (!E.IsReady() || target == null)
                return;
            E.CastOnUnit(target);
        }

        public override void useR(Obj_AI_Base target)
        {
            if (target == null || !R.IsReady())
                return;
            if (player.Position.GetAliveEnemiesInRange(500)>1)
            {
                R.Cast();
            }
        }

        public override void setUpSpells()
        {
            Q = new Spell(SpellSlot.Q, 185);
            W = new Spell(SpellSlot.W, 125);
            E = new Spell(SpellSlot.E, 550);
            R = new Spell(SpellSlot.R, 650);
        }

        public override void useSpells()
        {
            if (Q.IsReady()  &&
                    player.HealthPercent < 80)
            {
                var minion = MinionManager.GetMinions(player.Position, Q.Range).FirstOrDefault();
                if (minion.LSIsValidTarget(Q.Range))
                {
                    Q.CastOnUnit(minion);
                }
            }

            var tar = ARAMTargetSelector.getBestTarget(W.Range);
            useW(tar);
            tar = ARAMTargetSelector.getBestTarget(E.Range);
            useE(tar);
            tar = ARAMTargetSelector.getBestTarget(R.Range);
            useR(tar);
        }
    }
}
