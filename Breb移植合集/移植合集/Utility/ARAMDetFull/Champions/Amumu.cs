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
    class Amumu : Champion
    {

        public Amumu()
        {
            ARAMSimulator.champBuild = new Build
            {
                coreItems = new List<ConditionalItem>
                {
                    new ConditionalItem(ItemId.Rylais_Crystal_Scepter),
                    new ConditionalItem(ItemId.Mercurys_Treads, ItemId.Ninja_Tabi, ItemCondition.ENEMY_AP),
                    new ConditionalItem(ItemId.Liandrys_Torment),
                    new ConditionalItem(ItemId.Spirit_Visage, ItemId.Randuins_Omen, ItemCondition.ENEMY_AP),
                    new ConditionalItem(ItemId.Abyssal_Scepter),
                    new ConditionalItem(ItemId.Banshees_Veil),
                },
                startingItems = new List<ItemId>
                {
                    ItemId.Giants_Belt
                }
            };
        }

        public override void useQ(Obj_AI_Base target)
        {
            if (Q.CanCast(target))
            {
                CastQ(target);
            }
        }

        public override void useW(Obj_AI_Base target)
        {
            if (!W.IsReady())
                return;
        }

        public override void useE(Obj_AI_Base target)
        {
            if (E.CanCast(target) )
            {
                CastE(target);
            }
        }

        public override void useR(Obj_AI_Base target)
        {
            if (R.CanCast(target))
            {
                AutoUlt();
            }


        }

        public override void setUpSpells()
        {
            Q = new Spell(SpellSlot.Q, 1080);
            W = new Spell(SpellSlot.W, 300);
            E = new Spell(SpellSlot.E, 350);
            R = new Spell(SpellSlot.R, 450);

            Q.SetSkillshot(.25f, 90, 2000, true, SkillshotType.SkillshotLine);  //check delay
            W.SetSkillshot(0f, W.Range, float.MaxValue, false, SkillshotType.SkillshotCircle); //correct
            E.SetSkillshot(.5f, E.Range, float.MaxValue, false, SkillshotType.SkillshotCircle); //check delay
            R.SetSkillshot(.25f, R.Range, float.MaxValue, false, SkillshotType.SkillshotCircle); //check delay

        }

        public override void useSpells()
        {
            var tar = ARAMTargetSelector.getBestTarget(E.Range);
            if (tar != null) useQ(tar);
            if (tar != null) useW(tar);
            if (tar != null) useE(tar);
            if (tar != null) useR(tar);
        }

        void AutoUlt()
        {
            var comboR = 2;

            if (comboR > 0 && R.IsReady())
            {
                int enemiesHit = 0;
                int killableHits = 0;

                foreach (AIHeroClient enemy in ObjectManager.Get<AIHeroClient>().Where(he => he.IsEnemy && he.LSIsValidTarget(R.Range)))
                {
                    var prediction = Prediction.GetPrediction(enemy, R.Delay);

                    if (prediction != null && prediction.UnitPosition.LSDistance(ObjectManager.Player.ServerPosition) <= R.Range)
                    {
                        enemiesHit++;

                        if (ObjectManager.Player.LSGetSpellDamage(enemy, SpellSlot.W) >= enemy.Health)
                            killableHits++;
                    }
                }

                if (enemiesHit >= comboR || (killableHits >= 1 && ObjectManager.Player.Health / ObjectManager.Player.MaxHealth <= 0.1))
                    CastR();
            }
        }

        void CastE(Obj_AI_Base target)
        {
            if (!E.IsReady() || target == null || !target.LSIsValidTarget())
                return;

            if (E.GetPrediction(target).UnitPosition.LSDistance(ObjectManager.Player.ServerPosition) <= E.Range)
                E.CastOnUnit(ObjectManager.Player);
        }


        void CastQ(Obj_AI_Base target, HitChance hitChance = HitChance.High)
        {
            if (!Q.IsReady())
                return;
            if (target == null || !target.LSIsValidTarget())
                return;

            Q.CastIfHitchanceEquals(target, hitChance);
        }

        void CastR()
        {
            if (!R.IsReady())
                return;
            R.Cast();
        }
    }
}
