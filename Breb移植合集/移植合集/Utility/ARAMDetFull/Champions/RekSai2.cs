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
    class RekSai2 : Champion
    {

        public Spell Q1, W1, E1, Q2, W2, E2;
        public List<Spell> BurrowedSpells = new List<Spell>();
        public List<Spell> UnburrowedSpells = new List<Spell>();

        public RekSai2()
        {
            ARAMSimulator.champBuild = new Build
            {
                coreItems = new List<ConditionalItem>
                {
                    new ConditionalItem(ItemId.The_Black_Cleaver),
                    new ConditionalItem(ItemId.Mercurys_Treads,ItemId.Ninja_Tabi,ItemCondition.ENEMY_AP),
                    new ConditionalItem(ItemId.The_Bloodthirster),
                    new ConditionalItem(ItemId.Spirit_Visage, ItemId.Randuins_Omen, ItemCondition.ENEMY_AP),
                    new ConditionalItem(ItemId.Ravenous_Hydra_Melee_Only),
                    new ConditionalItem(ItemId.Banshees_Veil, ItemId.Randuins_Omen, ItemCondition.ENEMY_AP),
                },
                startingItems = new List<ItemId>
                {
                    ItemId.Phage
                }
            };


            LXOrbwalker.AfterAttack += afterAttack;
            Obj_AI_Base.OnProcessSpellCast += OnProcessSpell;
        }

        private void OnProcessSpell(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!sender.IsMe)
                return;

            if (args.SData.Name.ToLowerInvariant() == "reksaiq")
                LXOrbwalker.ResetAutoAttackTimer();

            if (args.SData.Name.ToLowerInvariant().Contains("reksaiqatt")
                || args.SData.Name.ToLowerInvariant() == "reksaie")
                LeagueSharp.Common.Utility.DelayAction.Add(500, LXOrbwalker.ResetAutoAttackTimer);


        }

        private void afterAttack(AttackableUnit unit, AttackableUnit target)
        {
            if (unit.IsMe && !player.IsBurrowed()
                && E1.IsReady()
                && player.ManaPercent > 99f)
            {
                E1.CastOnUnit((Obj_AI_Base)target);
            }
        }

        public override void useQ(Obj_AI_Base target)
        {
            if (!Q1.IsReady() || target == null || player.IsBurrowed())
                return;
            Q1.Cast();
        }

        public void useQ2(Obj_AI_Base target)
        {
            if (!Q2.IsReady() || target == null || !player.IsBurrowed())
                return;
            Q2.Cast(target);
        }

        public override void useW(Obj_AI_Base target)
        {
            if (!W.IsReady() || target == null)
                return;
            //if (!Q.IsReady(4500) && player.Mana > 200)
            //      W.Cast();
        }

        public override void useE(Obj_AI_Base target)
        {
            //if (!E.IsReady() || target == null || !player.IsBurrowed() || !safeGap(target))
            //    return;
           // E2.Cast(target);
        }

        public void useE2(Obj_AI_Base target)
        {
            if (!E2.IsReady() || target == null || !player.IsBurrowed() || !safeGap(player.Position.LSExtend(target.Position,E2.Range).LSTo2D()))
                return;
            E2.Cast(target);
        }

        public override void useR(Obj_AI_Base target)
        {
            
        }

        public override void useSpells()
        {
            if (player.IsChannelingImportantSpell())
                return;

            var tar = ARAMTargetSelector.getBestTarget(Q1.Range);
            if (tar != null) useQ(tar);
            tar = ARAMTargetSelector.getBestTarget(Q2.Range);
            if (tar != null) useQ2(tar);
            manageW();
            tar = ARAMTargetSelector.getBestTarget(E2.Range);
            if (tar != null) useE2(tar);


        }


        public override void setUpSpells()
        {
            Q1 = new Spell(SpellSlot.Q, 300);
            UnburrowedSpells.Add(Q1);
            W1 = new Spell(SpellSlot.W, 250);
            UnburrowedSpells.Add(W1);
            E1 = new Spell(SpellSlot.E, 250);
            UnburrowedSpells.Add(E1);
            Q2 = new Spell(SpellSlot.Q, 1500, DamageType.Magical);
            Q2.SetSkillshot(0.125f, 60, 4000, true, SkillshotType.SkillshotLine);
            BurrowedSpells.Add(Q2);
            W2 = new Spell(SpellSlot.W, 250);
            BurrowedSpells.Add(W2);
            E2 = new Spell(SpellSlot.E, 850);
            E2.SetSkillshot(0, 60, 1600, false, SkillshotType.SkillshotLine);
            BurrowedSpells.Add(E2);

        }


        public override void farm()
        {
            var minions = ObjectManager.Get<Obj_AI_Minion>().Where(x => x.IsEnemy && x.LSDistance(player.Position) <= 250f);
            if (minions.Count() >= 2
                && !player.IsBurrowed()
                && Q1.IsReady())
            {
                Q1.Cast();
            }
        }

        public void Combo()
        {
            var target = ARAMTargetSelector.getBestTarget(1400f);
            manageW();
            if (!player.IsBurrowed()
                && target.LSIsValidTarget(Q1.Range)
                && Q1.IsReady())
            {
                Q1.Cast();
            }
            if (player.IsBurrowed()
                && target.LSIsValidTarget(Q2.Range)
                && Q2.IsReady())
            {
                    Q2.CastIfHitchanceEquals(target, HitChance.High);
            }
            if (player.IsBurrowed()
                && target.LSIsValidTarget(E2.Range)
                && E2.IsReady())
            {
                    E2.CastIfHitchanceEquals(target, HitChance.High);
            }

        }

        public void manageW()
        {
            var target = ARAMTargetSelector.getBestTarget(1400f);
            if (target == null) return;
            if (!player.IsBurrowed())
            {
                if ( W1.IsReady()
                        && player.HealthPercent < 9
                        && player.Mana > 0)
                {
                    W1.Cast();
                }

                    if (W1.IsReady()
                        && !player.QActive())
                    {
                        W1.Cast();
                    }

                    if (!E1.IsInRange(target)
                        && player.LSDistance(target.Position) < E2.Range
                        && W1.IsReady())
                    {
                        W1.Cast();
                   }
                
            }
            else if (player.IsBurrowed())
            {
                    if ( W2.IsReady()
                        && player.IsUnder(target))
                    {
                        W2.Cast();
                    }
            }
        }
    }
}
