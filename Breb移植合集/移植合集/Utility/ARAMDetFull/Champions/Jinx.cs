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
    class Jinx : Champion
    {
        //ManaMenager
        public static int QMANA;
        public static int WMANA;
        public static int EMANA;
        public static int RMANA;
        public static bool Farm = true;
        public static double WCastTime = 0;

        public Jinx()
        {

            LXOrbwalker.BeforeAttack += BeforeAttack;
            LXOrbwalker.AfterAttack += afterAttack;

            ARAMSimulator.champBuild = new Build
            {
                coreItems = new List<ConditionalItem>
                        {
                            new ConditionalItem(ItemId.Infinity_Edge),
                            new ConditionalItem(ItemId.Berserkers_Greaves),
                            new ConditionalItem(ItemId.Phantom_Dancer),
                            new ConditionalItem(ItemId.Blade_of_the_Ruined_King),
                            new ConditionalItem(ItemId.Mercurial_Scimitar),
                            new ConditionalItem(ItemId.Last_Whisper),
                        },
                startingItems = new List<ItemId>
                        {
                            ItemId.Pickaxe,ItemId.Boots_of_Speed
                        }
            };
        }

        private void BeforeAttack(LXOrbwalker.BeforeAttackEventArgs args)
        {
            var t = ARAMTargetSelector.getBestTarget(bonusRange() + 50);
            if (t.LSIsValidTarget() && Q.IsReady() && FishBoneActive)
            {
                var distance = GetRealDistance(t);
                var powPowRange = GetRealPowPowRange(t);
                if ((distance < powPowRange) && (ObjectManager.Player.Mana < RMANA + WMANA + 20 || ObjectManager.Player.LSGetAutoAttackDamage(t) * 2 < t.Health))
                    Q.Cast();
                else if (Farm && (distance > bonusRange() || distance < powPowRange || ObjectManager.Player.Mana < RMANA + EMANA + WMANA + WMANA))
                    Q.Cast();
            }
        }

        public override void useQ(Obj_AI_Base target)
        {
            if (!Q.IsReady())
                return; 
            ManaMenager();
            if (Farm)
                if (ObjectManager.Player.Mana > RMANA + WMANA + EMANA + 10 && !FishBoneActive)
                    farmQ();
            var t = ARAMTargetSelector.getBestTarget(bonusRange() + 50);
            if (t.LSIsValidTarget())
            {
                var distance = GetRealDistance(t);
                var powPowRange = GetRealPowPowRange(t);

                if (!FishBoneActive && (distance > powPowRange))
                {
                    if ( (ObjectManager.Player.Mana > RMANA + WMANA + 20 || ObjectManager.Player.LSGetAutoAttackDamage(t) * 2 > t.Health))
                        Q.Cast();
                    else if (Farm && haras() && ObjectManager.Player.Mana > RMANA + WMANA + EMANA + WMANA && distance < bonusRange() + t.BoundingRadius)
                        Q.Cast();
                }
            }
        }

        public override void useW(Obj_AI_Base target)
        {
            if (!W.IsReady())
                return;
            W.Cast(target);
        }

        public override void useE(Obj_AI_Base target)
        {
            if (!E.IsReady())
                return;
            E.Cast(target);
        }

        public override void useR(Obj_AI_Base target)
        {
            if (!R.IsReady())
                return;
            if (target.Health < R.GetDamage(target))
                R.Cast(target);
        }

        public override void setUpSpells()
        {
            //Create the spells
            Q = new Spell(SpellSlot.Q, 1000);
            W = new Spell(SpellSlot.W, 1500f);
            E = new Spell(SpellSlot.E, 900f);
            R = new Spell(SpellSlot.R, 2500f);

            W.SetSkillshot(0.6f, 60f, 3300f, true, SkillshotType.SkillshotLine);
            E.SetSkillshot(1.1f, 1f, 1750f, false, SkillshotType.SkillshotCircle);
            R.SetSkillshot(0.7f, 140f, 1500f, false, SkillshotType.SkillshotLine);
        }

        public override void useSpells()
        {
            var tar = ARAMTargetSelector.getBestTarget(1010);
            useQ(tar);
            tar = ARAMTargetSelector.getBestTarget(E.Range);
            if (tar != null) useE(tar);
            tar = ARAMTargetSelector.getBestTarget(W.Range);
            if (tar != null) useW(tar);
            tar = ARAMTargetSelector.getBestTarget(R.Range);
            if (tar != null) useR(tar);
        }

        private void afterAttack(AttackableUnit unit, AttackableUnit target)
        {
            if (!unit.IsMe || !Q.IsReady() || !FishBoneActive) return;
            var t = ARAMTargetSelector.getBestTarget(bonusRange() + 50);
            if (t.LSIsValidTarget())
            {
                var distance = GetRealDistance(t);
                var powPowRange = GetRealPowPowRange(t);
                if ( (distance < powPowRange) && (ObjectManager.Player.Mana < RMANA + WMANA + 20 || ObjectManager.Player.LSGetAutoAttackDamage(t) * 2 < t.Health))
                    Q.Cast();
                else if (Farm && (distance > bonusRange() || distance < powPowRange || ObjectManager.Player.Mana < RMANA + EMANA + WMANA + WMANA))
                    Q.Cast();
            }
        }


        public void farmQ()
        {
            var allMinionsQ = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, bonusRange() + 30, MinionTypes.All);
            foreach (var minion in allMinionsQ)
            {
                if (!Orbwalking.InAutoAttackRange(minion) && minion.Health < ObjectManager.Player.LSGetAutoAttackDamage(minion) && GetRealPowPowRange(minion) < GetRealDistance(minion) && bonusRange() < GetRealDistance(minion))
                {
                    Q.Cast();
                    return;
                }
                else if (Orbwalking.InAutoAttackRange(minion) && CountEnemies(minion, 150) > 0)
                {
                    Q.Cast();
                    return;
                }
            }
        }

        public bool haras()
        {
            var allMinionsQ = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, bonusRange(), MinionTypes.All);
            var haras = true;
            foreach (var minion in allMinionsQ)
            {
                if (minion.Health < ObjectManager.Player.LSGetAutoAttackDamage(minion) * 1.5 && bonusRange() > GetRealDistance(minion))
                    haras = false;
            }
            if (haras)
                return true;
            else
                return false;
        }

        public override void farm()
        {
            base.farm();
            if (FishBoneActive && Q.IsReady())
                Q.Cast();
        }

        public float bonusRange()
        {
            return 620f + ObjectManager.Player.BoundingRadius + 50 + 25 * ObjectManager.Player.Spellbook.GetSpell(SpellSlot.Q).Level;
        }

        private bool FishBoneActive
        {
            get { return Math.Abs(ObjectManager.Player.AttackRange - 525f) > float.Epsilon; }
        }

        private int CountEnemies(Obj_AI_Base target, float range)
        {
            return
                ObjectManager.Get<AIHeroClient>()
                    .Count(
                        hero =>
                            hero.LSIsValidTarget() && hero.Team != ObjectManager.Player.Team &&
                            hero.ServerPosition.LSDistance(target.ServerPosition) <= range);
        }
        private int CountAlliesNearTarget(Obj_AI_Base target, float range)
        {
            return
                ObjectManager.Get<AIHeroClient>()
                    .Count(
                        hero =>
                            hero.Team == ObjectManager.Player.Team &&
                            hero.ServerPosition.LSDistance(target.ServerPosition) <= range);
        }

        private float GetRealPowPowRange(GameObject target)
        {
            return 610f + ObjectManager.Player.BoundingRadius + target.BoundingRadius;
        }

        private float GetRealDistance(GameObject target)
        {
            return ObjectManager.Player.ServerPosition.LSDistance(target.Position) + ObjectManager.Player.BoundingRadius +
                   target.BoundingRadius;
        }

        public void ManaMenager()
        {
            QMANA = 10;
            WMANA = 40 + 10 * W.Level;
            EMANA = 50;
            if (!R.IsReady())
                RMANA = WMANA - ObjectManager.Player.Level * 2;
            else
                RMANA = 100;

            if (Farm)
                RMANA = RMANA + (CountEnemies(ObjectManager.Player, 2500) * 20);

            if (ObjectManager.Player.Health < ObjectManager.Player.MaxHealth * 0.2)
            {
                QMANA = 0;
                WMANA = 0;
                EMANA = 0;
                RMANA = 0;
            }
        }

    }
}
