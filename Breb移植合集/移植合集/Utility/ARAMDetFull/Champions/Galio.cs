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
    class Galio : Champion
    {

        public Galio()
        {
            ARAMSimulator.champBuild = new Build
            {
                coreItems = new List<ConditionalItem>
                    {
                        new ConditionalItem(ItemId.Mercurys_Treads),
                        new ConditionalItem(ItemId.Athenes_Unholy_Grail),
                        new ConditionalItem(ItemId.Abyssal_Scepter),
                        new ConditionalItem(ItemId.Spirit_Visage),
                        new ConditionalItem(ItemId.Rabadons_Deathcap),
                        new ConditionalItem(ItemId.Void_Staff),
                    },
                startingItems = new List<ItemId>
                    {
                        ItemId.Chalice_of_Harmony,ItemId.Boots_of_Speed
                    }
            };

            LXOrbwalker.AfterAttack += afterAttack;
            Obj_AI_Base.OnProcessSpellCast += OnProcessSpell;
            Console.WriteLine("Galio in");
        }

        private void OnProcessSpell(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!W.IsReady() || sender.IsAlly || !(sender is AIHeroClient) || !(args.Target is AIHeroClient) || args.Target.IsEnemy)
                return;
            if (W.IsInRange(args.Target))
                W.CastOnUnit((AIHeroClient)args.Target);

            //26386025
        }

        private void afterAttack(AttackableUnit unit, AttackableUnit target)
        {
            
        }

        public override void useQ(Obj_AI_Base target)
        {
            if (!Q.IsReady() || target == null)
                return;
            Q.Cast(target);
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
            if (!E.IsReady() || target == null)
                return;
            E.Cast(target);
        }

        public override void useR(Obj_AI_Base target)
        {
            if (target == null || !R.IsReady())
                return;
            if (player.LSCountEnemiesInRange(450) > 1)
            {
                if (W.IsReady())
                    W.Cast();
                R.Cast();
            }
        }

        public override void useSpells()
        {
            if(player.IsChannelingImportantSpell())
                return;
            

            var tar = ARAMTargetSelector.getBestTarget(Q.Range);
            if (tar != null) useQ(tar);
            tar = ARAMTargetSelector.getBestTarget(E.Range);
            if (tar != null) useE(tar);
            tar = ARAMTargetSelector.getBestTarget(R.Range);
            if (tar != null) useR(tar);
        }


        public override void setUpSpells()
        {
            Q = new Spell(SpellSlot.Q, 940f);
            W = new Spell(SpellSlot.W, 800f);
            E = new Spell(SpellSlot.E, 1180f);
            R = new Spell(SpellSlot.R, 450f);

            Q.SetSkillshot(0.5f, 120, 1300, false, SkillshotType.SkillshotCircle);
            E.SetSkillshot(0.5f, 140, 1200, false, SkillshotType.SkillshotLine);
          //  R.SetSkillshot(0.2f, 320, float.MaxValue, false, SkillshotType.SkillshotCircle);

        }


        public override void farm()
        {
            if (player.ManaPercent < 65)
                return;

            var AllMinions = MinionManager.GetMinions(player.ServerPosition, Q.Range, MinionTypes.All, MinionTeam.Enemy, MinionOrderTypes.Health);
            foreach (var minion in AllMinions)
            {
                if (Q.IsReady() && Q.GetDamage(minion)>minion.Health)
                {
                    Q.Cast(minion);
                    return;
                }
                if (E.IsReady() && E.GetDamage(minion) > minion.Health)
                {
                    E.Cast(minion);
                }
            } 
        }
    }
}
