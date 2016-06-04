using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common; using EloBuddy;

namespace ARAMDetFull.Champions
{
    class Soraka : Champion
    {


        public Soraka()
        {
            Interrupter2.OnInterruptableTarget += InterrupterOnOnPossibleToInterrupt;
            AntiGapcloser.OnEnemyGapcloser += AntiGapcloserOnOnEnemyGapcloser;

            ARAMSimulator.champBuild = new Build
            {
                coreItems = new List<ConditionalItem>
                        {
                            new ConditionalItem(ItemId.Athenes_Unholy_Grail),
                            new ConditionalItem(ItemId.Sorcerers_Shoes),
                            new ConditionalItem(ItemId.Rabadons_Deathcap),
                            new ConditionalItem(ItemId.Void_Staff),
                            new ConditionalItem(ItemId.Ludens_Echo),
                            new ConditionalItem(ItemId.Spirit_Visage),
                        },
                startingItems = new List<ItemId>
                        {
                            ItemId.Chalice_of_Harmony,ItemId.Boots_of_Speed
                        }
            };
        }


        private  void AntiGapcloserOnOnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            var unit = gapcloser.Sender;

            if ( unit.LSIsValidTarget(Q.Range) && Q.IsReady())
            {
                Q.Cast(unit);
            }

            if (unit.LSIsValidTarget(E.Range) && E.IsReady())
            {
                E.Cast(unit);
            }
        }

        private void InterrupterOnOnPossibleToInterrupt(AIHeroClient sender, Interrupter2.InterruptableTargetEventArgs args)
        {
            var unit = sender;
            var spell = args;


            if (!unit.LSIsValidTarget(E.Range))
            {
                return;
            }
            if (!E.IsReady())
            {
                return;
            }

            E.Cast(unit);
        }

        public override void useQ(Obj_AI_Base target)
        {
            if (Q.IsReady())
            {
                Q.Cast(target);
            }
        }

        public override void useW(Obj_AI_Base target)
        {
            
        }

        public override void useE(Obj_AI_Base target)
        {
            if (E.IsReady())
                E.Cast(target);
        }


        public override void useR(Obj_AI_Base target)
        {
        }

        public override void useSpells()
        {
            var tar = ARAMTargetSelector.getBestTarget(Q.Range);
            if (tar != null) useQ(tar);
            tar = ARAMTargetSelector.getBestTarget(E.Range);
            if (tar != null) useE(tar);

        }

        public override void farm()
        {
            base.farm();
            AutoR();
            AutoW();
        }

        public override void killSteal()
        {
            base.killSteal();
            AutoR();
            AutoW();
        }

        public override void setUpSpells()
        {
            //Create the spells
            Q = new Spell(SpellSlot.Q, 950);
            W = new Spell(SpellSlot.W, 550);
            E = new Spell(SpellSlot.E, 925);
            R = new Spell(SpellSlot.R);

            Q.SetSkillshot(0.5f, 300, 1750, false, SkillshotType.SkillshotCircle);
            E.SetSkillshot(0.5f, 70f, 1750, false, SkillshotType.SkillshotCircle);
        }

        private void AutoR()
        {
            if (!R.IsReady())
            {
                return;
            }

            foreach (var friend in
               LXOrbwalker.AllAllys.Where(x => x.IsAlly).Where(x => !x.IsDead).Where(x => !x.IsZombie))
            {
                var health = 35;

                if (friend.HealthPercent <= health)
                {
                    R.Cast();
                }
            }
        }

        private void AutoW()
        {
            if (!W.IsReady())
            {
                return;
            }
            if(player.HealthPercent<45)
                return;

            foreach (var friend in
                from friend in
                    LXOrbwalker.AllAllys
                        .Where(x => !x.IsEnemy && !x.IsMe)
                        .Where(friend => W.IsInRange(friend.ServerPosition, W.Range))
                let healthPercent = 75
                where friend.HealthPercent <= healthPercent
                select friend)
            {
                W.CastOnUnit(friend);
            }
        }
    }
}
