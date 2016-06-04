using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common; using EloBuddy;

namespace ARAMDetFull.Champions
{
    class Yasuo : Champion
    {

        private Spell Q2;

        public Yasuo()
        {
            ARAMSimulator.champBuild = new Build
            {
                coreItems = new List<ConditionalItem>
                {
                    new ConditionalItem(ItemId.Blade_of_the_Ruined_King),
                    new ConditionalItem(ItemId.Mercurys_Treads),
                    new ConditionalItem(ItemId.Statikk_Shiv),
                    new ConditionalItem(ItemId.Infinity_Edge),
                    new ConditionalItem(ItemId.Last_Whisper),
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
            if (Q.IsReady())
                if (HaveQ3)
                    Q2.Cast(target);
                else
                {
                    Q.Cast(target);
                }
        }

        public override void useW(Obj_AI_Base target)
        {
            if (W.IsReady())
                W.Cast(target.Position);
        }

        public override void useE(Obj_AI_Base target)
        {
            if (!E.IsReady() || target == null)
                return;
            if (safeGap(target))
                E.CastOnUnit(target);
        }


        public override void useR(Obj_AI_Base target)
        {
            if (!R.IsReady() || target == null)
                return;
            if (safeGap(target))
                 R.Cast();
        }

        public override void useSpells()
        {
            var tar = ARAMTargetSelector.getBestTarget(HaveQ3?Q2.Range:Q.Range);
            if (tar != null) useQ(tar);
            tar = ARAMTargetSelector.getBestTarget(W.Range);
            if (tar != null) useW(tar);
            tar = ARAMTargetSelector.getBestTarget(E.Range);
            if (tar != null) useE(tar);
            tar = ARAMTargetSelector.getBestTarget(R.Range);
            if (tar != null) useR(tar);

        }

        public override void farm()
        {
            base.farm();
            if (Q.IsReady())
            {
                var farmL = Q.GetLineFarmLocation(MinionManager.GetMinions(player.ServerPosition, Q.Range, MinionTypes.All, MinionTeam.Enemy, MinionOrderTypes.Health));
                if (farmL.MinionsHit > 0)
                    Q.Cast(farmL.Position);
            }
        }

        public override void setUpSpells()
        {
            //Create the spells
            Q = new Spell(SpellSlot.Q, 500);
            Q2 = new Spell(SpellSlot.Q, 1100);
            W = new Spell(SpellSlot.W, 400);
            E = new Spell(SpellSlot.E, 475, DamageType.Magical);
            R = new Spell(SpellSlot.R, 1300);
            Q.SetSkillshot(GetQDelay, 20, float.MaxValue, false, SkillshotType.SkillshotLine);
            Q2.SetSkillshot(GetQ2Delay, 90, 1500, false, SkillshotType.SkillshotLine);
        }

        private bool HaveQ3
        {
            get { return player.HasBuff("YasuoQ3W"); }
        }

        private float GetQDelay
        {
            get { return 0.4f * (1 - Math.Min((player.AttackSpeedMod - 1) * 0.58f, 0.66f)); }
        }

        private float GetQ2Delay
        {
            get { return 0.5f * (1 - Math.Min((player.AttackSpeedMod - 1) * 0.58f, 0.66f)); }
        }

    }
}
