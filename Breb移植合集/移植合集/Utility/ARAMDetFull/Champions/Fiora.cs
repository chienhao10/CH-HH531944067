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
    class Fiora : Champion
    {

        public Fiora()
        {
            LXOrbwalker.AfterAttack += ExecuteAfterAttack;

            ARAMSimulator.champBuild = new Build
            {
                coreItems = new List<ConditionalItem>
                {
                    new ConditionalItem(ItemId.Ravenous_Hydra_Melee_Only),
                    new ConditionalItem(ItemId.Mercurys_Treads, ItemId.Ninja_Tabi, ItemCondition.ENEMY_AP),
                    new ConditionalItem(ItemId.The_Black_Cleaver),
                    new ConditionalItem(ItemId.Last_Whisper),
                    new ConditionalItem(ItemId.Guardian_Angel),
                    new ConditionalItem(ItemId.Banshees_Veil),
                },
                startingItems = new List<ItemId>
                {
                    ItemId.Pickaxe,ItemId.Boots_of_Speed
                }
            };

        }

        public override void useQ(Obj_AI_Base target)
        {
            if (Q.CanCast(target))
            {
                Q.Cast();
            }
        }

        public override void useW(Obj_AI_Base target)
        {

        }

        public override void useE(Obj_AI_Base target)
        {
            if (E.CanCast(target) && (Q.IsReady() || R.IsReady()))
            {
                if ((MapControl.balanceAroundPoint(player.Position.LSTo2D(), 700) >= -1 || (MapControl.fightIsOn() != null && MapControl.fightIsOn().NetworkId == target.NetworkId)))

                    E.Cast(target.ServerPosition);
            }
        }

        public override void useR(Obj_AI_Base target)
        {
            if (R.CanCast(target) && !Q.IsKillable(target))
            {
                CastR(target);
            }


        }

        public override void setUpSpells()
        {
            //Initialize our Spells
            Q = new Spell(SpellSlot.Q, 425);
            W = new Spell(SpellSlot.W);
            E = new Spell(SpellSlot.E, 540);
            R = new Spell(SpellSlot.R, 460);
        }

        public override void useSpells()
        {
            var tar = ARAMTargetSelector.getBestTarget(E.Range);
            if (tar != null) useQ(tar);
            if (tar != null) useE(tar);
            if (tar != null) useR(tar);
        }

        //Afterattack
        public void ExecuteAfterAttack(AttackableUnit unit, AttackableUnit target)
        {
            if (!unit.IsMe || !(target is AIHeroClient))
                return;
            W.Cast();
        }

        private void CastR(Obj_AI_Base target)
        {
            if (!R.IsReady())
                return;

            foreach (var hero in ObjectManager.Get<AIHeroClient>().Where(hero => hero.LSIsValidTarget(R.Range)))
            {
                if (player.LSGetSpellDamage(target, SpellSlot.R) - 50 > hero.Health)
                {
                    R.Cast(target);
                }

                else if (player.LSGetSpellDamage(target, SpellSlot.R) - 50 < hero.Health)
                {
                    foreach (var buff in hero.Buffs.Where(buff => buff.Name == "dariushemo"))
                    {
                        if (player.LSGetSpellDamage(target, SpellSlot.R, 1) * (1 + buff.Count / 5) - 50 > target.Health)
                        {
                            R.CastOnUnit(target, true);
                        }
                    }
                }
            }
        }

    }
}
