using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common; using EloBuddy;

namespace ARAMDetFull.Champions
{
    class Cassiopeia : Champion
    {

        public Cassiopeia()
        {
            ARAMSimulator.champBuild = new Build
            {
                coreItems = new List<ConditionalItem>
                {
                    new ConditionalItem(ItemId.Rod_of_Ages),
                    new ConditionalItem(ItemId.Sorcerers_Shoes),
                    new ConditionalItem(ItemId.Rabadons_Deathcap),
                    new ConditionalItem(ItemId.Rylais_Crystal_Scepter),
                    new ConditionalItem(ItemId.Liandrys_Torment),
                    new ConditionalItem(ItemId.Zhonyas_Hourglass),
                },
                startingItems = new List<ItemId>
                {
                    ItemId.Catalyst_the_Protector
                }
            };
        }

        public override void useQ(Obj_AI_Base target)
        {
            if (!Q.IsReady() || target == null)
                return;
            if (target.LSIsValidTarget(Q.Range+100))
                Q.Cast(target);
        }

        public override void useW(Obj_AI_Base target)
        {
            if (!W.IsReady() || target == null)
                return;
            if (target.LSIsValidTarget(W.Range + 100))
                W.Cast(target);
        }

        public override void useE(Obj_AI_Base target)
        {
            if (!E.IsReady() || target == null)
                return;
            if ((target.HasBuffOfType(BuffType.Poison)))
            {
                E.CastOnUnit(target);
            }
        }

        public override void useR(Obj_AI_Base target)
        {
            if (target == null)
                return;
            if (target.LSIsValidTarget(R.Range) && R.IsReady())
            {
                R.Cast(target);
            }
        }

        public override void useSpells()
        {
            var tar = ARAMTargetSelector.getBestTarget(Q.Range);
            useQ(tar);
             tar = ARAMTargetSelector.getBestTarget(W.Range);
             useW(tar);
             tar = ARAMTargetSelector.getBestTarget(E.Range);
             useE(tar);
             tar = ARAMTargetSelector.getBestTarget(R.Range);
             useR(tar);
        }

        public override void setUpSpells()
        {
            Q = new Spell(SpellSlot.Q, 850);
            Q.SetSkillshot(0.6f, 120, float.MaxValue, false, SkillshotType.SkillshotCircle);

            W = new Spell(SpellSlot.W, 850);
            W.SetSkillshot(0.5f, 150, 2500, false, SkillshotType.SkillshotCircle);

            E = new Spell(SpellSlot.E, 700);

            R = new Spell(SpellSlot.R, 700);
            R.SetSkillshot(0.6f, 500, float.MaxValue, false, SkillshotType.SkillshotCone);
        }
    }
}
