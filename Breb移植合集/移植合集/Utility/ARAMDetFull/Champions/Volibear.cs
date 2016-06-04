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
    class Volibear : Champion
    {

        public Volibear()
        {
            ARAMSimulator.champBuild = new Build
            {
                coreItems = new List<ConditionalItem>
                        {
                            new ConditionalItem(ItemId.Banshees_Veil,ItemId.Sunfire_Cape,ItemCondition.ENEMY_AP),
                            new ConditionalItem(ItemId.Mercurys_Treads),
                            new ConditionalItem(ItemId.Frozen_Mallet),
                            new ConditionalItem(ItemId.Spirit_Visage,ItemId.Randuins_Omen,ItemCondition.ENEMY_AP),
                            new ConditionalItem(ItemId.Blade_of_the_Ruined_King),
                            new ConditionalItem(ItemId.Maw_of_Malmortius,ItemId.Thornmail,ItemCondition.ENEMY_AP),
                        },
                startingItems = new List<ItemId>
                        {
                            ItemId.Giants_Belt,ItemId.Boots_of_Speed
                        }
            };
            LXOrbwalker.BeforeAttack += beforeAttack;
            Obj_AI_Base.OnProcessSpellCast += OnProcessSpell;
            Console.WriteLine("Volibear in");
        }


        private void OnProcessSpell(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {

        }

        private void beforeAttack(LXOrbwalker.BeforeAttackEventArgs args)
        {
            if (args.Unit.IsMe && args.Target is AIHeroClient && Q.IsReady())
            {
                Q.Cast();
                Aggresivity.addAgresiveMove(new AgresiveMove(55, 3500, true));
            }
        }

        public override void useQ(Obj_AI_Base target)
        {
            if (!Q.IsReady() || target == null)
                return;
            Q.Cast();
            Aggresivity.addAgresiveMove(new AgresiveMove(55,3500,true));
        }

        public override void useW(Obj_AI_Base target)
        {
            if (!W.IsReady() || target == null || target.HealthPercent>65)
                return;
            
            W.CastOnUnit(target);
        }

        public override void useE(Obj_AI_Base target)
        {
            if (!E.IsReady() || target == null)
                return;
            E.Cast();
        }

        public override void useR(Obj_AI_Base target)
        {
            if (target == null || !R.IsReady())
                return;
            if (player.LSCountEnemiesInRange(450) > 1)
            {
                R.Cast();
                Aggresivity.addAgresiveMove(new AgresiveMove(65, 5500, true));
            }
        }

        public override void useSpells()
        {
            if (player.IsChannelingImportantSpell())
                return;


            var tar = ARAMTargetSelector.getBestTarget(Q.Range);
            if (tar != null) useQ(tar);
            tar = ARAMTargetSelector.getBestTarget(E.Range);
            if (tar != null) useE(tar);
            tar = ARAMTargetSelector.getBestTarget(W.Range);
            if (tar != null) useW(tar);
            tar = ARAMTargetSelector.getBestTarget(R.Range);
            if (tar != null) useR(tar);
        }


        public override void setUpSpells()
        {
            Q = new Spell(SpellSlot.Q, 520);
            W = new Spell(SpellSlot.W, 400);
            E = new Spell(SpellSlot.E, 400);
            R = new Spell(SpellSlot.R, 240);

            //  R.SetSkillshot(0.2f, 320, float.MaxValue, false, SkillshotType.SkillshotCircle);

        }


        public override void farm()
        {
            if (player.ManaPercent < 65)
                return;

            var AllMinions = MinionManager.GetMinions(player.ServerPosition, Q.Range, MinionTypes.All, MinionTeam.Enemy, MinionOrderTypes.Health);
            foreach (var minion in AllMinions)
            {
                if (E.IsReady() && E.GetDamage(minion) > minion.Health)
                {
                    E.Cast(minion);
                }
            }
        }
    }
}
