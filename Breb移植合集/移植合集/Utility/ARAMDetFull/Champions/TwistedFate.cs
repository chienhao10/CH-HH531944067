using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common; using EloBuddy;

namespace ARAMDetFull.Champions
{
    class TwistedFateA : Champion
    {

        public TwistedFateA()
        {
            ARAMSimulator.champBuild = new Build
            {
                coreItems = new List<ConditionalItem>
                {
                    new ConditionalItem(ItemId.Lich_Bane),
                    new ConditionalItem(ItemId.Sorcerers_Shoes),
                    new ConditionalItem(ItemId.Zhonyas_Hourglass),
                    new ConditionalItem(ItemId.Rabadons_Deathcap),
                    new ConditionalItem(ItemId.Void_Staff),
                    new ConditionalItem(ItemId.Banshees_Veil),
                },
                startingItems = new List<ItemId>
                {
                    ItemId.Sheen,ItemId.Boots_of_Speed
                }
            };
        }

        public override void useQ(Obj_AI_Base target)
        {
            if (!Q.IsReady() || target == null )
                return;
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
            if (!E.IsReady() || target == null || !safeGap(target))
                return;
           // E.Cast(target.Position);
          //  Utility.DelayAction.Add(250, () => E.Cast(target.Position));
        }


        public override void useR(Obj_AI_Base target)
        {
            if (!R.IsReady() || target == null)
                return;
           // if ((target.HasBuff("dianamoonlight") && safeGap(target)) || target.HealthPercent < 28)
            //    R.Cast(target);
        }

        public override void useSpells()
        {
            var tar = ARAMTargetSelector.getBestTarget(Q.Range);
            if (tar != null) useQ(tar);
            tar = ARAMTargetSelector.getBestTarget(W.Range);
            if (tar != null) useW(tar);
        }

        public override void setUpSpells()
        {
            //Create the spells
            W = new Spell(SpellSlot.W, 550);
            E = new Spell(SpellSlot.E, 0);
            R = new Spell(SpellSlot.R, 0);

            Q = new Spell(SpellSlot.Q, 1450);
            Q.SetSkillshot(0.25f, 40f, 1000f, false, SkillshotType.SkillshotLine);
        }


        public override void farm()
        {
            if (player.ManaPercent < 55 || !Q.IsReady())
                return;

            foreach (var minion in MinionManager.GetMinions(Q.Range-100))
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
