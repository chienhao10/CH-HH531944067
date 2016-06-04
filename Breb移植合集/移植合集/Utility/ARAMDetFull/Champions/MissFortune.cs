using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using EloBuddy;
using SharpDX;
using EloBuddy.SDK;

namespace ARAMDetFull.Champions
{
    class MissFortune : Champion
    {
        private LeagueSharp.Common.Spell Q1;
        private float RCastTime = 0;

        public MissFortune()
        {
            LXOrbwalker.AfterAttack += afterAttack;

            ARAMSimulator.champBuild = new Build
            {
                coreItems = new List<ConditionalItem>
                        {
                            new ConditionalItem(ItemId.Infinity_Edge),
                            new ConditionalItem(ItemId.Berserkers_Greaves),
                            new ConditionalItem(ItemId.Phantom_Dancer),
                            new ConditionalItem(ItemId.Youmuus_Ghostblade),
                            new ConditionalItem(ItemId.Last_Whisper),
                            new ConditionalItem(ItemId.The_Bloodthirster),
                        },
                startingItems = new List<ItemId>
                        {
                            ItemId.Pickaxe,ItemId.Boots_of_Speed
                        }
            };
        }

        private void afterAttack(AttackableUnit unit, AttackableUnit target)
        {
            if (!unit.IsMe)
                return;

            if (player.IsChannelingImportantSpell() || Game.Time - RCastTime < 0.2)
            {
                Orbwalker.DisableAttacking = true;
                Orbwalker.DisableMovement = true;
                return;
            }

            Orbwalker.DisableAttacking = false;
            Orbwalker.DisableMovement = false;

            if (!(target is AIHeroClient))
                return;
            var t = target as AIHeroClient;

            if (Q.IsReady() && t.LSIsValidTarget(Q.Range))
            {
                Q.Cast(t);
            }
            if (W.IsReady())
            {
                W.Cast();
            }
        }

        public override void useQ(Obj_AI_Base target)
        {
            if (!Q.IsReady() || target == null)
                return;
            Q.Cast(target);
        }

        public override void useW(Obj_AI_Base target)
        {
            if (!W.IsReady() || target == null)
                return;
            if (!Q.IsReady(4500) && player.Mana > 200)
                W.Cast();
        }

        public override void useE(Obj_AI_Base target)
        {
            if (!E.IsReady() || target == null)
                return;
            E.Cast(target);
        }

        public override void useR(Obj_AI_Base target)
        {
            if (target == null)
                return;
        }

        public override void useSpells()
        {
            if (player.IsChannelingImportantSpell())
                return;

            var tar = ARAMTargetSelector.getBestTarget(Q.Range);
            if (tar != null) useQ(tar);
            tar = ARAMTargetSelector.getBestTarget(W.Range);
            useW(tar);
            tar = ARAMTargetSelector.getBestTarget(E.Range);
            if (tar != null) useE(tar);
            LogicR();
        }


        public override void setUpSpells()
        {
            Q = new LeagueSharp.Common.Spell(SpellSlot.Q, 655f);
            Q1 = new LeagueSharp.Common.Spell(SpellSlot.Q, 1100f);
            W = new LeagueSharp.Common.Spell(SpellSlot.W,550);
            E = new LeagueSharp.Common.Spell(SpellSlot.E, 800f);
            R = new LeagueSharp.Common.Spell(SpellSlot.R, 1350f);

            Q1.SetSkillshot(0.25f, 50f, 2000f, true, SkillshotType.SkillshotLine);
            Q.SetTargetted(0.25f, 1400f);
            E.SetSkillshot(0.5f, 200f, float.MaxValue, false, SkillshotType.SkillshotCircle);
            R.SetSkillshot(0.25f, 200f, 2000f, false, SkillshotType.SkillshotCircle);

        }

        private void LogicR()
        {
            var t = ARAMTargetSelector.getBestTarget(R.Range);

            if (t.LSIsValidTarget(R.Range))
            {
                var rDmg = R.GetDamage(t) + (W.GetDamage(t) * 10);

                if (player.LSCountEnemiesInRange(700) == 0 && t.CountAlliesInRange(400) == 0)
                {
                    var tDis = player.LSDistance(t.ServerPosition);
                    if (rDmg * 7 > t.Health && tDis < 800)
                    {
                        R.Cast(t, true, true);
                        RCastTime = Game.Time;
                    }
                    else if (rDmg * 6 > t.Health && tDis < 900)
                    {
                        R.Cast(t, true, true);
                        RCastTime = Game.Time;
                    }
                    else if (rDmg * 5 > t.Health && tDis < 1000)
                    {
                        R.Cast(t, true, true);
                        RCastTime = Game.Time;
                    }
                    else if (rDmg * 4 > t.Health && tDis < 1100)
                    {
                        R.Cast(t, true, true);
                        RCastTime = Game.Time;
                    }
                    else if (rDmg * 3 > t.Health && tDis < 1200)
                    {
                        R.Cast(t, true, true);
                        RCastTime = Game.Time;
                    }
                    else if (rDmg > t.Health && tDis < 1300)
                    {
                        R.Cast(t, true, true);
                        RCastTime = Game.Time;
                    }
                    return;
                }
                else if (rDmg * 8 > t.Health && t.LSCountEnemiesInRange(300) > 2 && player.LSCountEnemiesInRange(700) == 0)
                {
                    R.Cast(t, true, true);
                    RCastTime = Game.Time;
                    return;
                }
                else if (rDmg * 8 > t.Health  && player.LSCountEnemiesInRange(600) == 0)
                {
                    R.Cast(t, true, true);
                    RCastTime = Game.Time;
                    return;
                }
            }

        }
    }
}
