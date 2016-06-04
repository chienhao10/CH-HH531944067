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
    class Zilean : Champion
    {
        public Zilean()
        {
            ARAMSimulator.champBuild = new Build
            {
                coreItems = new List<ConditionalItem>
                    {
                        new ConditionalItem(ItemId.Athenes_Unholy_Grail),
                        new ConditionalItem(ItemId.Sorcerers_Shoes),
                        new ConditionalItem(ItemId.Rabadons_Deathcap),
                        new ConditionalItem(ItemId.Ludens_Echo),
                        new ConditionalItem(ItemId.Zhonyas_Hourglass),
                        new ConditionalItem(ItemId.Void_Staff),
                    },
                startingItems = new List<ItemId>
                    {
                        ItemId.Needlessly_Large_Rod
                    }
            };
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
            if (!Q.IsReady(3500) && player.Mana>150)
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
            if (target == null)
                return;
        }

        public override void useSpells()
        {
            UltAlly();
            var tar = ARAMTargetSelector.getBestTarget(Q.Range);
            if (tar != null) useQ(tar);
            tar = ARAMTargetSelector.getBestTarget(W.Range);
            useW(tar);
            tar = ARAMTargetSelector.getBestTarget(E.Range);
            if (tar != null) useE(tar);

        }


        public override void setUpSpells()
        {
            Q = new Spell(SpellSlot.Q, 900);
            W = new Spell(SpellSlot.W, 700);
            E = new Spell(SpellSlot.E, 550);
            R = new Spell(SpellSlot.R, 900);
            Q.SetSkillshot(0.30f, 110f, 2000f, false, SkillshotType.SkillshotCircle);
        }

       

        private void UltAlly()
        {
            if (!R.IsReady())
                return;
            var allyMinHP = 30;

            foreach (var hero in HeroManager.Allies)
            {

                if (player.HasBuff("Recall") || player.InFountain()) return;
                if ((hero.HealthPercent <= allyMinHP) && R.IsReady() &&
                    hero.LSCountEnemiesInRange(700) > 0 &&
                    (hero.LSDistance(player.ServerPosition) <= R.Range))
                {
                    R.Cast(hero);
                }
            }
        }
    }
}
