using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common; using EloBuddy;

namespace ARAMDetFull.Champions
{
    class Urgot : Champion
    {
        private Spell Q2;

        public Urgot()
        {
            ARAMSimulator.champBuild = new Build
            {
                coreItems = new List<ConditionalItem>
                        {
                            new ConditionalItem(ItemId.The_Black_Cleaver),
                            new ConditionalItem(ItemId.Mercurys_Treads),
                            new ConditionalItem(ItemId.Last_Whisper),
                            new ConditionalItem(ItemId.The_Bloodthirster),
                            new ConditionalItem(ItemId.Frozen_Heart),
                            new ConditionalItem(ItemId.Banshees_Veil),
                        },
                startingItems = new List<ItemId>
                        {
                            ItemId.Phage
                        }
            };
        }

        private void SmartQ()
        {
            if (!Q.IsReady())
            {
                return;
            }

            foreach (var obj in
                ObjectManager.Get<AIHeroClient>()
                    .Where(obj => obj.LSIsValidTarget(Q2.Range) && obj.HasBuff("urgotcorrosivedebuff")))
            {
                W.Cast();
                Q2.Cast(obj.ServerPosition);
            }
        }

        public override void useQ(Obj_AI_Base target)
        {
            if(target == null)
                return;
            if (Q.IsReady()  &&
                target.LSIsValidTarget(target.HasBuff("urgotcorrosivedebuff") ? Q2.Range : Q.Range))
            {
                Q.Cast(target.ServerPosition);
            }
        }

        public override void useW(Obj_AI_Base target)
        {
            if (!W.IsReady() || target == null)
                return;
            if (target.LSIsValidTarget(300))
                W.Cast(target);
        }

        public override void useE(Obj_AI_Base target)
        {
            if (!E.IsReady() || target == null)
            {
                return;
            }

            var hitchance = (HitChance.Medium);

            if (target.LSIsValidTarget(E.Range))
            {
                E.CastIfHitchanceEquals(target, hitchance);
            }
            else
            {
                var tar = ARAMTargetSelector.getBestTarget(E.Range);
                if(tar != null)
                    E.CastIfHitchanceEquals(tar, HitChance.High);
            }
        }

        public override void useR(Obj_AI_Base target)
        {
            if (!R.IsReady() || target == null)
                return;
            if (safeGap(target))
                R.CastOnUnit(target);
        }

        public override void useSpells()
        {
            SmartQ();
            var tar = ARAMTargetSelector.getBestTarget(Q.Range);
            if (tar != null) useQ(tar);
            tar = ARAMTargetSelector.getBestTarget(450);
            useW(tar);
            tar = ARAMTargetSelector.getBestTarget(E.Range+100);
            if (tar != null) useE(tar);
            var target = ARAMTargetSelector.getBestTarget(R.Range);
            if (target != null) useR(target);
           
        }

        public override void setUpSpells()
        {
            Q = new Spell(SpellSlot.Q, 1000);
            Q2 = new Spell(SpellSlot.Q, 1200);
            W = new Spell(SpellSlot.W);
            E = new Spell(SpellSlot.E, 900);
            R = new Spell(SpellSlot.R, 500);

            Q.SetSkillshot(0.10f, 100f, 1600f, true, SkillshotType.SkillshotLine);
            Q2.SetSkillshot(0.10f, 100f, 1600f, false, SkillshotType.SkillshotLine);
            E.SetSkillshot(0.283f, 0f, 1750f, false, SkillshotType.SkillshotCircle);
        }
    }
}
