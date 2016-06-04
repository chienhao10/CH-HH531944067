using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common; using EloBuddy;

namespace ARAMDetFull.Champions
{
    class Gragas : Champion
    {
        private GameObject bomb = null;

        public Gragas()
        {
            GameObject.OnCreate += OnCreateObject;
            GameObject.OnDelete += GameObject_OnDelete;

            ARAMSimulator.champBuild = new Build
            {
                coreItems = new List<ConditionalItem>
                        {
                            new ConditionalItem(ItemId.Rod_of_Ages),
                            new ConditionalItem(ItemId.Mercurys_Treads),
                            new ConditionalItem(ItemId.Athenes_Unholy_Grail),
                            new ConditionalItem(ItemId.Lich_Bane),
                            new ConditionalItem(ItemId.Rabadons_Deathcap),
                            new ConditionalItem(ItemId.Banshees_Veil),
                        },
                startingItems = new List<ItemId>
                        {
                            ItemId.Catalyst_the_Protector
                        }
            };
        }

        private void OnCreateObject(GameObject sender, EventArgs args)
        {
            if (sender.IsAlly && sender.Name == "Gragas_BaseQ_Ally.troy")
            {
                bomb = sender;
            }
        }

        void GameObject_OnDelete(GameObject sender, EventArgs args)
        {
            if (sender.IsAlly && sender.Name == "Gragas_BaseQ_Ally.troy")
            {
                bomb = null;
            }
        }

        public override void useQ(Obj_AI_Base target)
        {
            if (!Q.IsReady() || target == null || bomb != null)
                return;
            Q.Cast(target);
        }

        public override void useW(Obj_AI_Base target)
        {
            if (!W.IsReady() || target == null || Q.IsReady())
                return;
            W.Cast();
        }

        public override void useE(Obj_AI_Base target)
        {
            if (!E.IsReady() || target == null)
                return;
            if (safeGap(target))
                E.Cast(target);
        }


        public override void useR(Obj_AI_Base target)
        {
            if (!R.IsReady() || target == null)
                return;
            if (target.LSIsValidTarget(R.Range+200))
            {
                R.CastIfWillHit(target, 2);
            }
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

            if (bomb != null && Q.IsReady() && LeagueSharp.Common.Utility.LSCountEnemiesInRange(bomb.Position, (250))>0)
            {
                Q.Cast();
            }
        }

        public override void setUpSpells()
        {
            Q = new Spell(SpellSlot.Q, 775);
            W = new Spell(SpellSlot.W, 400);
            E = new Spell(SpellSlot.E, 600);
            R = new Spell(SpellSlot.R, 1050);
            Q.SetSkillshot(0.3f, 110f, 1000f, false, SkillshotType.SkillshotCircle);
            E.SetSkillshot(0.3f, 50, 1000, true, SkillshotType.SkillshotLine);
            R.SetSkillshot(0.3f, 700, 1000, false, SkillshotType.SkillshotCircle);
        }
    }
}
