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
    class Karthus : Champion
    {

        private const float SpellQWidth = 160f;
        private const float SpellWWidth = 160f;

        private bool _comboE;

        internal class EnemyInfo
        {
            public AIHeroClient Player;
            public EnemyInfo(AIHeroClient player)
            {
                Player = player;
            }
        }

        public Karthus()
        {
            ARAMSimulator.champBuild = new Build
            {
                coreItems = new List<ConditionalItem>
                        {
                            new ConditionalItem(ItemId.Rod_of_Ages),
                            new ConditionalItem(ItemId.Sorcerers_Shoes),
                            new ConditionalItem(ItemId.Rylais_Crystal_Scepter),
                            new ConditionalItem(ItemId.Rabadons_Deathcap),
                            new ConditionalItem(ItemId.Liandrys_Torment),
                            new ConditionalItem(ItemId.Void_Staff),
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
            CastQ(target);
        }

        public override void useW(Obj_AI_Base target)
        {
            if (!W.IsReady() || target == null)
                return;
            CastW(ARAMTargetSelector.getBestTarget(W.Range),35);

        }

        public override void useE(Obj_AI_Base target)
        {
            if (!E.IsReady())
                return;

            if (target != null)
            {
                var enoughMana =player.Mana<300;

                if (ObjectManager.Player.Spellbook.GetSpell(SpellSlot.E).ToggleState == 1)
                {
                    if (ObjectManager.Player.LSDistance(target.ServerPosition) <= E.Range && enoughMana)
                    {
                        _comboE = true;
                        E.Cast();
                    }
                }
                else if (!enoughMana)
                    RegulateEState(true);
            }
            else
                RegulateEState();
        }

        public override void useR(Obj_AI_Base target)
        {
            if (target == null)
                return;
            if (IsInPassiveForm() && R.IsReady())
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
            Q = new Spell(SpellSlot.Q, 875);
            W = new Spell(SpellSlot.W, 1000);
            E = new Spell(SpellSlot.E, 505);
            R = new Spell(SpellSlot.R, 20000f);

            Q.SetSkillshot(1f, 160, float.MaxValue, false, SkillshotType.SkillshotCircle);
            W.SetSkillshot(.5f, 70, float.MaxValue, false, SkillshotType.SkillshotCircle);
            E.SetSkillshot(1f, 505, float.MaxValue, false, SkillshotType.SkillshotCircle);
            R.SetSkillshot(3f, float.MaxValue, float.MaxValue, false, SkillshotType.SkillshotCircle);
        }

        void CastQ(Obj_AI_Base target, int minManaPercent = 0)
        {
            if (!Q.IsReady() )
                return;
            if (target == null)
                return;
            Q.Width = GetDynamicQWidth(target);
            Q.Cast(target);
        }

        void CastQ(Vector2 pos, int minManaPercent = 0)
        {
            if (!Q.IsReady())
                return;
             Q.Cast(pos);
        }

        void CastW(Obj_AI_Base target, int minManaPercent = 0)
        {
            if (!W.IsReady())
                return;
            if (target == null)
                return;
            W.Width = GetDynamicWWidth(target);
            W.Cast(target);
        }

        public float GetTargetHealth(EnemyInfo playerInfo, int additionalTime)
        {
            if (playerInfo.Player.IsVisible)
                return playerInfo.Player.Health;

            var predictedhealth = playerInfo.Player.Health + playerInfo.Player.HPRegenRate * ((LXOrbwalker.now - + additionalTime) / 1000f);

            return predictedhealth > playerInfo.Player.MaxHealth ? playerInfo.Player.MaxHealth : predictedhealth;
        }

        void RegulateEState(bool ignoreTargetChecks = false)
        {
            if (!E.IsReady() || IsInPassiveForm() ||
                ObjectManager.Player.Spellbook.GetSpell(SpellSlot.E).ToggleState != 2)
                return;
            var target = ARAMTargetSelector.getBestTarget(E.Range);
            var minions = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, E.Range, MinionTypes.All, MinionTeam.NotAlly);

            if (!ignoreTargetChecks && (target != null || (!_comboE && minions.Count != 0)))
                return;
            E.CastOnUnit(ObjectManager.Player);
            _comboE = false;
        }

        float GetDynamicWWidth(Obj_AI_Base target)
        {
            return Math.Max(70, (1f - (ObjectManager.Player.LSDistance(target) / W.Range)) * SpellWWidth);
        }

        float GetDynamicQWidth(Obj_AI_Base target)
        {
            return Math.Max(30, (1f - (ObjectManager.Player.LSDistance(target) / Q.Range)) * SpellQWidth);
        }

        static bool IsInPassiveForm()
        {
            return ObjectManager.Player.IsZombie; //!ObjectManager.Player.IsHPBarRendered;
        }
    }
}
