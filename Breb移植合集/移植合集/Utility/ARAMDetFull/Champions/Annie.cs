using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common; using EloBuddy;

namespace ARAMDetFull.Champions
{
    class Annie : Champion
    {
        public Obj_AI_Base Tibbers { get; private set; }

        private bool haveStun
        {
            get
            {
                var buffs = player.Buffs.Where(buff => (buff.Name.ToLower() == "pyromania" || buff.Name.ToLower() == "pyromania_particle"));
                var buffInstances = buffs as BuffInstance[] ?? buffs.ToArray();
                if (buffInstances.Any())
                {
                    var buff = buffInstances.First();
                    if (buff.Name.ToLower() == "pyromania_particle")
                        return true;
                    else
                        return false;
                }
                return false;
            }
        }

        public Annie()
        {
            GameObject.OnCreate += NewTibbers;
            GameObject.OnDelete += DeleteTibbers;

            ARAMSimulator.champBuild = new Build
            {
                coreItems = new List<ConditionalItem>
                        {
                            new ConditionalItem(ItemId.Rabadons_Deathcap),
                            new ConditionalItem(ItemId.Sorcerers_Shoes),
                            new ConditionalItem(ItemId.Rylais_Crystal_Scepter),
                            new ConditionalItem(ItemId.Void_Staff),
                            new ConditionalItem(ItemId.Ludens_Echo),
                            new ConditionalItem(ItemId.Zhonyas_Hourglass),
                        },
                startingItems = new List<ItemId>
                        {
                            ItemId.Boots_of_Speed,ItemId.Blasting_Wand
                        }
            };
        }

        private void NewTibbers(GameObject sender, EventArgs args)
        {
            if (IsTibbers(sender))
            {
                Tibbers = (Obj_AI_Base)sender;
            }
        }

        private void DeleteTibbers(GameObject sender, EventArgs args)
        {
            if (IsTibbers(sender))
            {
                Tibbers = null;
            }
        }

        private static bool IsTibbers(GameObject sender)
        {
            return ((sender != null) && (sender.IsValid) && (sender.Name.ToLowerInvariant().Equals("tibbers"))
                    && (sender.IsAlly));
        }

        public override void useQ(Obj_AI_Base target)
        {
            if (!Q.IsReady() || target == null)
                return;
            Q.CastOnUnit(target);
        }

        public override void useW(Obj_AI_Base target)
        {
            if (!W.IsReady())
                return;
            W.Cast(target);
        }

        public override void useE(Obj_AI_Base target)
        {
            if (!E.IsReady() || target == null || haveStun)
                return;
            E.Cast();
        }


        public override void useR(Obj_AI_Base target)
        {
            if (!R.IsReady() || target == null)
                return;
            if ((R.CastIfWillHit(target, 2) && haveStun) || (R.CastIfWillHit(target, 3)) || R.IsKillable(target))
                R.Cast(target);
        }

        public override void useSpells()
        {
            var tar = ARAMTargetSelector.getBestTarget(W.Range);
            if (tar != null) useW(tar);
            tar = ARAMTargetSelector.getBestTarget(Q.Range);
            if (tar != null) useQ(tar);
            tar = ARAMTargetSelector.getBestTarget(E.Range);
            if (tar != null) useE(tar);
            tar = ARAMTargetSelector.getBestTarget(R.Range);
            if (tar != null) useR(tar);
            if (Tibbers != null && tar != null)
            {
                tar = ARAMTargetSelector.getBestTarget(1500);
                if (Tibbers.LSDistance(tar.Position) > 200)
                {
                    EloBuddy.Player.IssueOrder(GameObjectOrder.MovePet, tar);
                }
                else
                {
                    EloBuddy.Player.IssueOrder(GameObjectOrder.AutoAttackPet, tar);
                }
            }

        }

        public override void setUpSpells()
        {
            //Create the spells
            Q = new Spell(SpellSlot.Q, 625f);
            W = new Spell(SpellSlot.W, 600f);
            E = new Spell(SpellSlot.E,1200f);
            R = new Spell(SpellSlot.R, 625f);

            Q.SetTargetted(0.25f, 1400f);
            W.SetSkillshot(0.50f, 250f, 3000, false, SkillshotType.SkillshotCircle);
            R.SetSkillshot(0.20f, 250f, float.MaxValue, false, SkillshotType.SkillshotCircle);
        }


        public override void farm()
        {
            if(player.ManaPercent < 55 || !Q.IsReady())
                return;
            
            foreach (var minion in MinionManager.GetMinions(Q.Range))
            {
                if (minion.Health > ObjectManager.Player.LSGetAutoAttackDamage(minion) && minion.Health < Q.GetDamage(minion))
                {
                    Q.Cast(minion);
                    return;
                }
            }
        }
    }
}
