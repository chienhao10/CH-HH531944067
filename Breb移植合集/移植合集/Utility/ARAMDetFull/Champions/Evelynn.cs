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
    class EvelynnA : Champion
    {

        public EvelynnA()
        {
            Spellbook.OnCastSpell += Spellbook_OnCastSpell;

            ARAMSimulator.champBuild = new Build
            {
                coreItems = new List<ConditionalItem>
                        {
                            new ConditionalItem(ItemId.Rylais_Crystal_Scepter),
                            new ConditionalItem(ItemId.Sorcerers_Shoes),
                            new ConditionalItem(ItemId.Liandrys_Torment),
                            new ConditionalItem(ItemId.Zhonyas_Hourglass),
                            new ConditionalItem(ItemId.Abyssal_Scepter),
                            new ConditionalItem(ItemId.Banshees_Veil),
                        },
                startingItems = new List<ItemId>
                        {
                            ItemId.Needlessly_Large_Rod
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

        }


        public override void useR(Obj_AI_Base target)
        {
        }

        public override void useSpells()
        {
            var target = ARAMTargetSelector.getBestTarget(Q.Range);

            if (target != null)
            {
                Q.Cast();

                if (ObjectManager.Player.HasBuffOfType(BuffType.Slow))
                    W.Cast();

                if (E.IsReady())
                    E.CastOnUnit(target);

                if (R.IsReady() && GetComboDamage(target) > target.Health)
                    R.Cast(target, false, true);
            }

        }

        private void Spellbook_OnCastSpell(Spellbook sender, SpellbookCastSpellEventArgs args)
        {
            if (sender.Owner.IsMe && args.Slot == SpellSlot.R)
            {
                if (ObjectManager.Get<AIHeroClient>()
                .Count(
                    hero =>
                        hero.LSIsValidTarget() &&
                        hero.LSDistance(new Vector2(args.EndPosition.X, args.EndPosition.Y)) <= R.Range) == 0)
                    args.Process = false;
            }
        }


        public override void setUpSpells()
        {
            //Create the spells
            Q = new Spell(SpellSlot.Q, 500f);
            W = new Spell(SpellSlot.W, Q.Range);
            E = new Spell(SpellSlot.E, 225f + 2 * 65f);
            R = new Spell(SpellSlot.R, 650f);

            R.SetSkillshot(0.25f, 350f, float.MaxValue, false, SkillshotType.SkillshotCircle);
        }

        public override void farm()
        {
            if(player.ManaPercent <45)
                return;
            var minions = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, Q.Range);

            foreach (var minion in minions.Where(minion => minion.LSIsValidTarget(Q.Range)))
            {
                if (Q.IsReady())
                    Q.Cast();

                if (E.IsReady())
                    E.CastOnUnit(minion);
            }
        }

        private float GetComboDamage(Obj_AI_Base target)
        {
            float comboDamage = 0;

            if ((ObjectManager.Player.Spellbook.GetSpell(SpellSlot.Q).Level) > 0)
                comboDamage += Q.GetDamage(target) * 3;
            if (E.IsReady())
                comboDamage += E.GetDamage(target);
            if (R.IsReady())
                comboDamage += R.GetDamage(target);

            return comboDamage;
        }
    }
}
