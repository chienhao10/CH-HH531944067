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
    class Ahri : Champion
    {

        //settings
        private static bool _rOn = false;
        private static int _rTimer;
        private static int _rTimeLeft = 0;

        public Ahri()
        {
            //Interrupter.OnPossibleToInterrupt += InterrupterOnOnPossibleToInterrupt;
            ARAMSimulator.champBuild = new Build
            {
                coreItems = new List<ConditionalItem>
                        {
                            new ConditionalItem(ItemId.Sorcerers_Shoes),
                            new ConditionalItem(ItemId.Athenes_Unholy_Grail),
                            new ConditionalItem(ItemId.Rabadons_Deathcap),
                            new ConditionalItem(ItemId.Zhonyas_Hourglass),
                            new ConditionalItem(ItemId.Ludens_Echo),
                            new ConditionalItem(ItemId.Void_Staff),
                        },
                startingItems = new List<ItemId>
                        {
                            ItemId.Boots_of_Speed,ItemId.Chalice_of_Harmony
                        }
            };
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

        public override void setUpSpells()
        {
            Q = new Spell(SpellSlot.Q, 900);
            W = new Spell(SpellSlot.W, 650);
            E = new Spell(SpellSlot.E, 875);
            R = new Spell(SpellSlot.R, 850);

            Q.SetSkillshot(0.25f, 100, 1600, false, SkillshotType.SkillshotLine);
            E.SetSkillshot(0.25f, 60, 1200, true, SkillshotType.SkillshotLine);
        }

        public override void useSpells()
        {
            var tar = ARAMTargetSelector.getBestTarget(Q.Range+800);
            if (tar != null)
                Combo();
            else
                Farm();
        }



        private float GetComboDamage(Obj_AI_Base enemy)
        {
            if (enemy == null)
                return 0;

            double damage = 0d;

            if (Q.IsReady())
            {
                damage += player.LSGetSpellDamage(enemy, SpellSlot.Q);
                damage += player.LSGetSpellDamage(enemy, SpellSlot.Q, 1);
            }


            if (W.IsReady())
                damage += player.LSGetSpellDamage(enemy, SpellSlot.W);

            if (_rOn)
                damage += player.LSGetSpellDamage(enemy, SpellSlot.R) * RCount();
            else if (R.IsReady())
                damage += player.LSGetSpellDamage(enemy, SpellSlot.R) * 3;


            if (E.IsReady())
                damage += player.LSGetSpellDamage(enemy, SpellSlot.E);

            return (float)damage;
        }

        private void Combo()
        {
            UseSpells(true,true,true,true, "Combo");
        }

        private void Harass()
        {
            UseSpells(true,true,true, false, "Harass");
        }

        private void UseSpells(bool useQ, bool useW, bool useE, bool useR, string source)
        {
            var range = Q.Range;
            AIHeroClient eTarget = ARAMTargetSelector.getBestTarget(range);



            AIHeroClient rETarget = ARAMTargetSelector.getBestTarget(E.Range);

            var dmg = GetComboDamage(eTarget);

            if (eTarget == null)
                return;

            //end items-------

            //E
            if (useE && E.IsReady() && player.LSDistance(eTarget) < E.Range)
            {
                    E.Cast(eTarget);
                    if (Q.IsReady())
                    {
                        Q.Cast(eTarget);
                    }
                    return;
            }

            //W
            if (useW && W.IsReady() && player.LSDistance(eTarget) <= W.Range - 100 &&
                ShouldW(eTarget, source))
            {
                W.Cast();
            }

            if (source == "Harass")
            {
                if (useQ && Q.IsReady() && player.LSDistance(eTarget) <= Q.Range &&
                    ShouldQ(eTarget, source) && player.LSDistance(eTarget) > 600)
                {
                    Q.Cast(eTarget, true);
                    return;
                }
            }
            else if (useQ && Q.IsReady() && player.LSDistance(eTarget) <= Q.Range &&
                     ShouldQ(eTarget, source))
            {
                    Q.Cast(eTarget, true);
                    return;
            }

            //R
            if (useR && R.IsReady() && player.LSDistance(eTarget) < R.Range)
            {
                if (E.IsReady())
                {
                    if (CheckReq(rETarget))
                        E.Cast(rETarget);
                }
                if (ShouldR(eTarget) && R.IsReady())
                {
                    R.Cast(player.Position.LSExtend(ARAMSimulator.fromNex.Position, 250));
                    _rTimer = LXOrbwalker.now - 250;
                }
                if (_rTimeLeft > 9500 && _rOn && R.IsReady())
                {
                    R.Cast(player.Position.LSExtend(ARAMSimulator.fromNex.Position,250));
                    _rTimer = LXOrbwalker.now - 250;
                }
            }
        }

        private void CheckKs()
        {
            foreach (AIHeroClient target in ObjectManager.Get<AIHeroClient>().Where(x => x.LSIsValidTarget(1300) && x.IsEnemy && !x.IsDead).OrderByDescending(GetComboDamage))
            {
                if (target != null)
                {
                    if (player.LSDistance(target.ServerPosition) <= W.Range &&
                        (player.LSGetSpellDamage(target, SpellSlot.Q) + player.LSGetSpellDamage(target, SpellSlot.Q, 1) +
                         player.LSGetSpellDamage(target, SpellSlot.W)) > target.Health && Q.IsReady() && Q.IsReady())
                    {
                        Q.Cast(target);
                        return;
                    }

                    if (player.LSDistance(target.ServerPosition) <= Q.Range &&
                        (player.LSGetSpellDamage(target, SpellSlot.Q) + player.LSGetSpellDamage(target, SpellSlot.Q, 1)) >
                        target.Health && Q.IsReady())
                    {
                        Q.Cast(target);
                        return;
                    }

                    if (player.LSDistance(target.ServerPosition) <= E.Range &&
                        (player.LSGetSpellDamage(target, SpellSlot.E)) > target.Health & E.IsReady())
                    {
                        E.Cast(target);
                        return;
                    }

                    if (player.LSDistance(target.ServerPosition) <= W.Range &&
                        (player.LSGetSpellDamage(target, SpellSlot.W)) > target.Health && W.IsReady())
                    {
                        W.Cast();
                        return;
                    }

                    Vector3 dashVector = player.Position +
                                         Vector3.Normalize(target.ServerPosition - player.Position) * 425;
                    if (player.LSDistance(target.ServerPosition) <= R.Range &&
                        (player.LSGetSpellDamage(target, SpellSlot.R)) > target.Health && R.IsReady() && _rOn &&
                        target.LSDistance(dashVector) < 425 && R.IsReady())
                    {
                        R.Cast(dashVector);
                    }
                }
            }
        }

        private bool ShouldQ(AIHeroClient target, string source)
        {
            if (source == "Combo")
            {
                if ((player.LSGetSpellDamage(target, SpellSlot.Q) + player.LSGetSpellDamage(target, SpellSlot.Q, 1)) >
                    target.Health)
                    return true;

                if (_rOn)
                    return true;


                if (target.HasBuffOfType(BuffType.Charm) || target.HasBuffOfType(BuffType.Taunt))
                    return true;
            }

            if (source == "Harass")
            {
                if ((player.LSGetSpellDamage(target, SpellSlot.Q) + player.LSGetSpellDamage(target, SpellSlot.Q, 1)) >
                    target.Health)
                    return true;

                if (_rOn)
                    return true;


                if (target.HasBuffOfType(BuffType.Charm) || target.HasBuffOfType(BuffType.Taunt))
                    return true;
            }

            return false;
        }

        private bool ShouldW(AIHeroClient target, string source)
        {
            if (source == "Combo")
            {
                if (player.LSGetSpellDamage(target, SpellSlot.W) > target.Health)
                    return true;

                if (_rOn)
                    return true;


                if (target.HasBuffOfType(BuffType.Charm) || target.HasBuffOfType(BuffType.Taunt))
                    return true;
            }
            if (source == "Harass")
            {
                if (player.LSGetSpellDamage(target, SpellSlot.W) > target.Health)
                    return true;

                if (_rOn)
                    return true;


                if (target.HasBuffOfType(BuffType.Charm) || target.HasBuffOfType(BuffType.Taunt))
                    return true;
            }

            return false;
        }

        private bool ShouldR(AIHeroClient target)
        {


            if (GetComboDamage(target) > target.Health && !_rOn)
            {
                if (target.HasBuffOfType(BuffType.Charm) || target.HasBuffOfType(BuffType.Taunt))
                    return true;
            }

            if (target.HasBuffOfType(BuffType.Charm) && _rOn || target.HasBuffOfType(BuffType.Taunt))
                return true;


            if (player.LSGetSpellDamage(target, SpellSlot.R) * 2 > target.Health)
                return true;

            if (_rOn && _rTimeLeft > 9500)
                return true;

            return false;
        }

        private bool CheckReq(AIHeroClient target)
        {

            if (GetComboDamage(target) > target.Health && !_rOn && target.LSCountEnemiesInRange(500)<3)
            {
                if (E.IsReady())
                {
                    //Chat.Print("added delay: " + addedDelay);

                        return true;
                }
            }

            return false;
        }

        private bool IsRActive()
        {
            return player.HasBuff("AhriTumble");
        }

        private int RCount()
        {
            var buff = player.Buffs.FirstOrDefault(x => x.Name == "AhriTumble");
            if (buff != null)
                return buff.Count;
            return 0;
        }

        private void Farm()
        {
            if (player.ManaPercent < 60) return;

            List<Obj_AI_Base> allMinionsQ = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, Q.Range,
                MinionTypes.All, MinionTeam.NotAlly);
            List<Obj_AI_Base> allMinionsW = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, W.Range,
                MinionTypes.All, MinionTeam.NotAlly);


            if (Q.IsReady())
            {
                MinionManager.FarmLocation qPos = Q.GetLineFarmLocation(allMinionsQ);
                if (qPos.MinionsHit >= 3)
                {
                    Q.Cast(qPos.Position);
                }
            }

            if (allMinionsW.Count > 0 && W.IsReady())
                W.Cast();
        }
    }
}
