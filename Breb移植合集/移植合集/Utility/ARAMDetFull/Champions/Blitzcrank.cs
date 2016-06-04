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
    class Blitzcrank : Champion
    {
        public Blitzcrank()
        {
            ARAMSimulator.champBuild = new Build
            {
                coreItems = new List<ConditionalItem>
                {
                    new ConditionalItem(ItemId.Rod_of_Ages),
                    new ConditionalItem(ItemId.Ionian_Boots_of_Lucidity),
                    new ConditionalItem(ItemId.Lich_Bane),
                    new ConditionalItem(ItemId.Abyssal_Scepter, ItemId.Frozen_Heart, ItemCondition.ENEMY_AP),
                    new ConditionalItem(ItemId.Void_Staff),
                    new ConditionalItem(ItemId.Banshees_Veil),
                },
                startingItems = new List<ItemId>
                {
                    ItemId.Catalyst_the_Protector,ItemId.Boots_of_Speed
                }
            };

            Interrupter.OnPossibleToInterrupt += OnPossibleToInterrupt;
            LXOrbwalker.AfterAttack += OnAfterAttack;
        }

        public override void useW(Obj_AI_Base target)
        {
            if (!W.IsReady())
                return;
            if (player.InShop())
                W.Cast();
        }

        public override void useE(Obj_AI_Base target)
        {
            if (!E.IsReady() || W.IsReady())
                return;
            E.Cast();
        }

        public override void useR(Obj_AI_Base target)
        {
            if (!R.IsReady())
                return;
                R.Cast();


        }

        public override void setUpSpells()
        {
            Q = new Spell(SpellSlot.Q, 900);
            W = new Spell(SpellSlot.W, 200);
            E = new Spell(SpellSlot.E, 175);
            R = new Spell(SpellSlot.R, 600);

            Q.SetSkillshot(0.25f, 70f, 1800f, true, SkillshotType.SkillshotLine);
        }

        public override void useSpells()
        {
            var tar = ARAMTargetSelector.getBestTarget(Q.Range);
                if(tar != null)useQ(tar);
            tar = ARAMTargetSelector.getBestTarget(W.Range);
                if (tar != null) useW(tar);
            tar = ARAMTargetSelector.getBestTarget(R.Range);
                if (tar != null) useR(tar);
        }

        public override void useQ(Obj_AI_Base target)
        {
            if (!Q.IsReady())
            {
                return;
            }

            if (!target.LSIsValidTarget())
            {
                return;
            }

            if (target.HasBuff("BlackShield"))
            {
                return;
            }

            if (AllyInRange(1200)
                .Any(ally => ally.LSDistance(target) < ally.AttackRange + ally.BoundingRadius))
            {
                return;
            }
            Q.Cast(target);

        }

        public List<AIHeroClient> AllyInRange(float range)
        {
            return
                ObjectManager.Get<AIHeroClient>()
                    .Where(
                        h =>
                            Geometry.LSDistance(ObjectManager.Player, h.Position) < range && h.IsAlly && !h.IsMe &&
                            h.IsValid && !h.IsDead)
                    .OrderBy(h => Geometry.LSDistance(ObjectManager.Player, h.Position))
                    .ToList();
        }

        public void OnAfterAttack(AttackableUnit unit, AttackableUnit target)
        {
            if (!unit.IsMe)
            {
                return;
            }

            if (!target.IsValid<AIHeroClient>() && !target.Name.ToLower().Contains("ward"))
            {
                return;
            }

            if (!E.IsReady())
            {
                return;
            }

            if (E.Cast())
            {
                LXOrbwalker.ResetAutoAttackTimer();
                EloBuddy.Player.IssueOrder(GameObjectOrder.AttackUnit, target);
            }
        }

        public void OnPossibleToInterrupt(Obj_AI_Base unit, InterruptableSpell spell)
        {
            if (spell.DangerLevel < InterruptableDangerLevel.High || unit.IsAlly)
            {
                return;
            }

            if (E.IsReady() && E.Cast())
            {
                LXOrbwalker.ResetAutoAttackTimer();
                EloBuddy.Player.IssueOrder(GameObjectOrder.AttackUnit, unit);
            }

            if (Q.IsReady())
            {
                Q.Cast(unit);
            }

            if (R.IsReady())
            {
                R.Cast();
            }
        }
    }
}
