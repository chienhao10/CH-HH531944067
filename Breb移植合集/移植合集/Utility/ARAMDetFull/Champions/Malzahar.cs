using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common; using EloBuddy;

namespace ARAMDetFull.Champions
{
    class Malzahar : Champion
    {

        public Malzahar()
        {
            AntiGapcloser.OnEnemyGapcloser += AntiGapcloserOnOnEnemyGapcloser;
            Interrupter2.OnInterruptableTarget += InterrupterOnOnPossibleToInterrupt;
            Console.WriteLine("Malzahar loaded!");

            ARAMSimulator.champBuild = new Build
            {
                coreItems = new List<ConditionalItem>
                {
                    new ConditionalItem(ItemId.Rod_of_Ages),
                    new ConditionalItem(ItemId.Sorcerers_Shoes),
                    new ConditionalItem(ItemId.Rylais_Crystal_Scepter),
                    new ConditionalItem(ItemId.Liandrys_Torment),
                    new ConditionalItem(ItemId.Abyssal_Scepter, ItemId.Zhonyas_Hourglass, ItemCondition.ENEMY_AP),
                    new ConditionalItem(ItemId.Rabadons_Deathcap),
                },
                startingItems = new List<ItemId>
                {
                    ItemId.Catalyst_the_Protector,ItemId.Boots_of_Speed
                }
            };
        }

        private void InterrupterOnOnPossibleToInterrupt(AIHeroClient unit, Interrupter2.InterruptableTargetEventArgs args)
        {
            if (!unit.LSIsValidTarget())
            {
                return;
            }

            if (args.DangerLevel != Interrupter2.DangerLevel.High)
            {
                return;
            }
            Q.Cast(unit);
        }

        private void AntiGapcloserOnOnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (!gapcloser.Sender.LSIsValidTarget())
            {
                return;
            }
            Q.Cast(gapcloser.Sender);
        }

        public override void useQ(Obj_AI_Base target)
        {
            if (!Q.IsReady() || target == null)
                return;
            Q.Cast(target);
        }

        public override void useW(Obj_AI_Base target)
        {
            if (!W.IsReady())
                return;
            if (target.LSIsValidTarget(W.Range))
            {
                if (!W.CastIfWillHit(target, 2))
                {
                    if (target.IsStunned || target.IsRooted || !target.CanMove)
                        W.Cast(target);
                }
            }
        }

        public override void useE(Obj_AI_Base target)
        {
            if (!E.IsReady() || target == null)
                return;
            E.CastOnUnit(target);
        }


        public override void useR(Obj_AI_Base target)
        {
            if (!R.IsReady() || target == null)
                return;
            R.Cast(target);
        }

        public override void useSpells()
        {
            var tar = ARAMTargetSelector.getBestTarget(Q.Range);
            if (tar != null) useQ(tar);
            else
            {
                farm();
            };
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
            Q = new Spell(SpellSlot.Q, 900);
            W = new Spell(SpellSlot.W, 880);
            E = new Spell(SpellSlot.E, 650); //1000?
            R = new Spell(SpellSlot.R, 700); //300?

            Q.SetSkillshot(0.5f, 100, float.MaxValue, false, SkillshotType.SkillshotLine);
            W.SetSkillshot(0.5f, 240, 20, false, SkillshotType.SkillshotCircle);
        }

        public override void farm()
        {
            if(player.ManaPercent<40)
                return;
            if (E.IsReady())
            {
                var mins = MinionManager.GetMinions(E.Range).Where(min => min.Health < E.GetDamage(min)).ToList();
                if (mins.Count() != 0)
                    E.CastOnUnit(mins.FirstOrDefault());
            }
        }
    }
}
