using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common; using EloBuddy;

namespace ARAMDetFull.Champions
{
    class Ashe : Champion
    {

        public Ashe()
        {

            Interrupter.OnPossibleToInterrupt += Game_OnPossibleToInterrupt;
            Obj_AI_Base.OnProcessSpellCast += Game_OnProcessSpell;

            ARAMSimulator.champBuild = new Build
            {
                coreItems = new List<ConditionalItem>
                        {
                            new ConditionalItem(ItemId.Infinity_Edge),
                            new ConditionalItem(ItemId.Berserkers_Greaves),
                            new ConditionalItem(ItemId.Phantom_Dancer),
                            new ConditionalItem(ItemId.The_Bloodthirster),
                            new ConditionalItem(ItemId.Last_Whisper),
                            new ConditionalItem(ItemId.Banshees_Veil),
                        },
                startingItems = new List<ItemId>
                        {
                            ItemId.Boots_of_Speed,ItemId.Pickaxe
                        }
            };
        }

        private void AfterAttack(Obj_AI_Base unit, Obj_AI_Base target)
        {
            if (target is AIHeroClient && Q.IsReady())
            {
                Q.Cast();
                Player.IssueOrder(GameObjectOrder.AutoAttack, target);
            }
        }

        public override void useQ(Obj_AI_Base target)
        {

        }

        public override void useW(Obj_AI_Base target)
        {

        }

        public override void useE(Obj_AI_Base target)
        {
            if (!E.IsReady() || target == null)
                return;
            E.Cast(target);
        }


        public override void useR(Obj_AI_Base target)
        {
            if (!R.IsReady() || target == null)
                return;
            if (target.HealthPercent < 35)
                R.Cast(target);
        }

        public override void useSpells()
        {
            var tar = ARAMTargetSelector.getBestTarget(E.Range);
            if (tar != null) useE(tar);
            tar = ARAMTargetSelector.getBestTarget(W.Range);
            if (tar != null) useW(tar);

            if (R.IsReady())
            {
                foreach (var enem in ObjectManager.Get<AIHeroClient>()
                    .Where(ene => ene.IsEnemy && ene.LSDistance(player, true) < R.Range * R.Range).Where(enem => enem.HealthPercent < 35))
                {
                    R.Cast(enem);
                    return;
                }
            }


            if (W.IsReady())
            {
                foreach (var enem in ObjectManager.Get<AIHeroClient>().Where(ene => ene.LSDistance(player, true) < W.Range * W.Range))
                {
                    if (enem.GetEnemiesInRange(330).Count > 1)
                    {
                        W.CastOnUnit(enem);
                        return;
                    }
                }

            }

        }

        public override void setUpSpells()
        {
            Q = new Spell(SpellSlot.Q);
            W = new Spell(SpellSlot.W, 1200);
            E = new Spell(SpellSlot.E);
            R = new Spell(SpellSlot.R, 20000);

            W.SetSkillshot(250f, (float)(24.32f * Math.PI / 180), 902f, true, SkillshotType.SkillshotCone);
            E.SetSkillshot(377f, 299f, 1400f, false, SkillshotType.SkillshotLine);
            R.SetSkillshot(250f, 130f, 1600f, false, SkillshotType.SkillshotLine);
        }

        public void Game_OnPossibleToInterrupt(Obj_AI_Base unit, InterruptableSpell spell)
        {
            if (spell.DangerLevel == InterruptableDangerLevel.High && R.IsReady() && unit.LSIsValidTarget(1500))
            {
                R.Cast(unit);
            }
        }

        private float GetComboDamage(AIHeroClient t)
        {
            var fComboDamage = 0f;

            if (W.IsReady())
                fComboDamage += (float)ObjectManager.Player.LSGetSpellDamage(t, SpellSlot.W);

            if (R.IsReady())
                fComboDamage += (float)ObjectManager.Player.LSGetSpellDamage(t, SpellSlot.R);

            if (ObjectManager.Player.GetSpellSlot("summonerdot") != SpellSlot.Unknown &&
                ObjectManager.Player.Spellbook.CanUseSpell(ObjectManager.Player.GetSpellSlot("summonerdot")) ==
                SpellState.Ready && ObjectManager.Player.LSDistance(t) < 550)
                fComboDamage += (float)ObjectManager.Player.GetSummonerSpellDamage(t, Damage.SummonerSpell.Ignite);


            return fComboDamage;
        }

        public void Game_OnProcessSpell(Obj_AI_Base unit, GameObjectProcessSpellCastEventArgs spell)
        {
            if ( unit.Team == ObjectManager.Player.Team)
                return;

            if (spell.SData.Name.ToLower() == "summonerflash")
                E.Cast(spell.End);
        }

        private bool AsheQCastReady
        {
            get { return ObjectManager.Player.HasBuff("AsheQCastReady"); }
        }

        public bool IsQActive
        {
            get { return ObjectManager.Player.HasBuff("FrostShot"); }
        }
    }
}
