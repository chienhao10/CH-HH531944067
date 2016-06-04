using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common; using EloBuddy;

namespace ARAMDetFull.Champions
{
    class Khazix : Champion
    {

        private static bool Qevolved = false, Wevolved = false, Eevolved = false;


        public Khazix()
        {
            Console.WriteLine("Kha started");
            ARAMSimulator.champBuild = new Build
            {
                coreItems = new List<ConditionalItem>
                {
                    new ConditionalItem(ItemId.Ravenous_Hydra_Melee_Only),
                    new ConditionalItem(ItemId.Mercurys_Treads),
                    new ConditionalItem(ItemId.The_Black_Cleaver),
                    new ConditionalItem(ItemId.Spirit_Visage, ItemId.Randuins_Omen, ItemCondition.ENEMY_AP),
                    new ConditionalItem(ItemId.The_Bloodthirster),
                    new ConditionalItem(ItemId.Banshees_Veil),
                },
                startingItems = new List<ItemId>
                {
                    ItemId.Vampiric_Scepter,ItemId.Boots_of_Speed
                }
            };
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
            if (!E.IsReady() || target == null)
                return;
            if (target.Health > E.GetDamage(target) && target.LSDistance(player,true)>350*350)
                return;

            if (safeGap(target))
                E.Cast(target);
        }


        public override void useR(Obj_AI_Base target)
        {
            if (!R.IsReady() || target == null)
                return;
            R.Cast();
        }

        public override void useSpells()
        {
            if (E.IsReady() && player.HealthPercent < 35 && player.GetEnemiesInRange(600).Count(ene => !ene.IsDead) > 1)
                E.Cast(ARAMSimulator.fromNex.Position);
            CheckSpells();
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
            Q = new Spell(SpellSlot.Q, 325f);
            W = new Spell(SpellSlot.W, 1000f);
            E = new Spell(SpellSlot.E, 600f);
            R = new Spell(SpellSlot.R, 300f);

            W.SetSkillshot(0.225f, 80f, 828.5f, true, SkillshotType.SkillshotLine);

            E.SetSkillshot(0.25f, 100f, 1000f, false, SkillshotType.SkillshotCircle);
        }

        private void CheckSpells()
        {

            //check for evolutions
            if (ObjectManager.Player.HasBuff("khazixqevo") && !Qevolved)
            {
                Q.Range = 375;
                Qevolved = true;
            }
            if (ObjectManager.Player.HasBuff("khazixwevo") && !Wevolved)
            {
                Wevolved = true;
                W.SetSkillshot(0.225f, 100f, 828.5f, true, SkillshotType.SkillshotLine);
            }

            if (ObjectManager.Player.HasBuff("khazixeevo") && !Eevolved)
            {
                E.Range = 1000;
                Eevolved = true;
            }
            if (player.EvolvePoints > 0)
            {
                if (!Eevolved)
                {
                    player.Spellbook.EvolveSpell(SpellSlot.E);
                    return;
                }
                if (!Qevolved)
                {
                    player.Spellbook.EvolveSpell(SpellSlot.Q);
                    return;
                }
                if (!Wevolved)
                {
                    player.Spellbook.EvolveSpell(SpellSlot.W);
                    return;
                }
            }
        }
    }
}
