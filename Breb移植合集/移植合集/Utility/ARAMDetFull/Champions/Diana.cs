using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common; using EloBuddy;

namespace ARAMDetFull.Champions
{
    class Diana : Champion
    {

        public Diana()
        {
            ARAMSimulator.champBuild = new Build
            {
                coreItems = new List<ConditionalItem>
                        {
                            new ConditionalItem(ItemId.Nashors_Tooth),
                            new ConditionalItem(ItemId.Sorcerers_Shoes),
                            new ConditionalItem(ItemId.Zhonyas_Hourglass),
                            new ConditionalItem(ItemId.Abyssal_Scepter),
                            new ConditionalItem(ItemId.Lich_Bane),
                            new ConditionalItem(ItemId.Rabadons_Deathcap),
                        },
                startingItems = new List<ItemId>
                        {
                            ItemId.Stinger
                        }
            };
        }

        public override void useQ(Obj_AI_Base target)
        {
            if (!Q.IsReady() || target == null)
                return;
            Console.WriteLine("Cast QQ");
            Q.Cast(target);
        }

        public override void useW(Obj_AI_Base target)
        {
            if (!W.IsReady())
                return;
            W.Cast();
        }

        public override void useE(Obj_AI_Base target)
        {
            if (!E.IsReady() || target == null)
                return;
            E.Cast();
        }


        public override void useR(Obj_AI_Base target)
        {
            if (!R.IsReady() || target == null)
                return;
            if ((target.HasBuff("dianamoonlight") && safeGap(target))|| target.HealthPercent<28)
                R.Cast(target);
        }

        public override void useSpells()
        {
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
            //Create the spells
            Q = new Spell(SpellSlot.Q, 900);
            W = new Spell(SpellSlot.W, 240);
            E = new Spell(SpellSlot.E, 450);
            R = new Spell(SpellSlot.R, 825);

            Q.SetSkillshot(0.35f, 180f, 1800f, false, SkillshotType.SkillshotCircle);
        }


        public override void farm()
        {
            if (player.ManaPercent < 55)
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
