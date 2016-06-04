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
    class Zyra : Champion
    {

        public Zyra()
        {
            ARAMSimulator.champBuild = new Build
            {
                coreItems = new List<ConditionalItem>
                        {
                            new ConditionalItem(ItemId.Rabadons_Deathcap),
                            new ConditionalItem(ItemId.Sorcerers_Shoes),
                            new ConditionalItem(ItemId.Morellonomicon),
                            new ConditionalItem(ItemId.Void_Staff),
                            new ConditionalItem(ItemId.Liandrys_Torment),
                            new ConditionalItem(ItemId.Banshees_Veil),
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
            if (Q.Cast(target) == Spell.CastStates.SuccessfullyCasted)
            {
                 CastW(Q.GetPrediction(target).CastPosition);
            }
        }

        public override void useW(Obj_AI_Base target)
        {
           
        }

        public override void useE(Obj_AI_Base target)
        {
            if (!E.IsReady() || target == null)
                return;
            if (E.Cast(target) == Spell.CastStates.SuccessfullyCasted)
            {
                  CastW(Q.GetPrediction(target).CastPosition);
            }
        }

        public override void useR(Obj_AI_Base target)
        {
            if (target == null)
                return;
            if (target.LSIsValidTarget(R.Range) && R.IsReady())
            {
                R.CastIfWillHit(target, 2);
            }
        }

        public override void useSpells()
        {
            var tar = ARAMTargetSelector.getBestTarget(Passive.Range);
            if (ZyraisZombie())
            {
                if(tar != null)
                    CastPassive(tar);
                return;
            }

            tar = ARAMTargetSelector.getBestTarget(Q.Range);
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
            Q = new Spell(SpellSlot.Q, 800);
            W = new Spell(SpellSlot.W, 825);
            E = new Spell(SpellSlot.E, 1100);
            R = new Spell(SpellSlot.R, 700);
            Passive = new Spell(SpellSlot.Q, 1470);

            Q.SetSkillshot(0.8f, 60f, float.MaxValue, false, SkillshotType.SkillshotCircle);
            E.SetSkillshot(0.5f, 70f, 1400f, false, SkillshotType.SkillshotLine);
            R.SetSkillshot(0.5f, 500f, float.MaxValue, false, SkillshotType.SkillshotCircle);
            Passive.SetSkillshot(0.5f, 70f, 1400f, false, SkillshotType.SkillshotLine);
        }

        private bool ZyraisZombie()
        {
            return player.Spellbook.GetSpell(SpellSlot.Q).Name == player.Spellbook.GetSpell(SpellSlot.E).Name ||
                   player.Spellbook.GetSpell(SpellSlot.W).Name == player.Spellbook.GetSpell(SpellSlot.R).Name;
        }

        private void CastPassive(Obj_AI_Base target)
        {
            if (!Passive.IsReady())
            {
                return;
            }
            if (!target.LSIsValidTarget(E.Range))
            {
                return;
            }
            Passive.CastIfHitchanceEquals(target, HitChance.High);
        }


        private Spell Passive { get; set; }

        private int WCount
        {
            get { return W.Instance.Level > 0 ? W.Instance.Ammo : 0; }
        }

        private void CastW(Vector3 v)
        {
            if (!W.IsReady())
            {
                return;
            }

            if (WCount == 1)
            {
                LeagueSharp.Common.Utility.DelayAction.Add(50, () => W.Cast(new Vector2(v.X - 5, v.Y - 5)));
            }

            if (WCount == 2)
            {
                LeagueSharp.Common.Utility.DelayAction.Add(50, () => W.Cast(new Vector2(v.X - 5, v.Y - 5)));
                LeagueSharp.Common.Utility.DelayAction.Add(180, () => W.Cast(new Vector2(v.X - 5, v.Y - 5)));
            }
        }
    }
}
