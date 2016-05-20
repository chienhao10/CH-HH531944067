using System;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using LeagueSharp.Common;
using Spell = LeagueSharp.Common.Spell;
using Utility = LeagueSharp.Common.Utility;

//using DetuksSharp;

namespace MasterSharp
{
    internal class MasterYi
    {
        public static AIHeroClient player = ObjectManager.Player;

        public static SummonerItems sumItems = new SummonerItems(player);

        public static Spellbook sBook = player.Spellbook;

        public static SpellDataInst Qdata = sBook.GetSpell(SpellSlot.Q);
        public static SpellDataInst Wdata = sBook.GetSpell(SpellSlot.W);
        public static SpellDataInst Edata = sBook.GetSpell(SpellSlot.E);
        public static SpellDataInst Rdata = sBook.GetSpell(SpellSlot.R);
        public static Spell Q = new Spell(SpellSlot.Q, 600);
        public static Spell W = new Spell(SpellSlot.W, 0);
        public static Spell E = new Spell(SpellSlot.E, 0);
        public static Spell R = new Spell(SpellSlot.R, 0);


        public static SpellSlot smite = SpellSlot.Unknown;


        public static Obj_AI_Base selectedTarget = null;

        public static void setSkillShots()
        {
            setupSmite();
        }

        public static void setupSmite()
        {
            if (player.Spellbook.GetSpell(SpellSlot.Summoner1).SData.Name.ToLower().Contains("smite"))
            {
                smite = SpellSlot.Summoner1;
            }
            else if (player.Spellbook.GetSpell(SpellSlot.Summoner2).SData.Name.ToLower().Contains("smite"))
            {
                smite = SpellSlot.Summoner2;
            }
        }

        public static void slayMaderDuker(Obj_AI_Base target)
        {
            try
            {
                if (target == null)
                    return;
                if (MasterSharp.getCheckBoxItem(MasterSharp.comboMenu, "useSmite"))
                    useSmiteOnTarget(target);

                if (target.LSDistance(player) < 500)
                {
                    sumItems.cast(SummonerItems.ItemIds.Ghostblade);
                }
                if (target.LSDistance(player) < 300)
                {
                    sumItems.cast(SummonerItems.ItemIds.Hydra);
                }
                if (target.LSDistance(player) < 300)
                {
                    sumItems.cast(SummonerItems.ItemIds.Tiamat);
                }
                if (target.LSDistance(player) < 300)
                {
                    sumItems.cast(SummonerItems.ItemIds.Cutlass, target);
                }
                if (target.LSDistance(player) < 500 && player.Health/player.MaxHealth*100 < 85)
                {
                    sumItems.cast(SummonerItems.ItemIds.BotRK, target);
                }

                if (MasterSharp.getCheckBoxItem(MasterSharp.comboMenu, "useQ") &&
                    (Orbwalker.CanMove || Q.IsKillable(target)))
                    useQSmart(target);
                if (MasterSharp.getCheckBoxItem(MasterSharp.comboMenu, "useE"))
                    useESmart(target);
                if (MasterSharp.getCheckBoxItem(MasterSharp.comboMenu, "useR"))
                    useRSmart(target);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }


        public static void useQtoKill(Obj_AI_Base target)
        {
            if (Q.IsReady() && (target.Health <= Q.GetDamage(target) || iAmLow(0.20f)))
                Q.Cast(target, MasterSharp.getCheckBoxItem(MasterSharp.extraMenu, "packets"));
        }

        public static void useESmart(Obj_AI_Base target)
        {
            if (ObjectManager.Player.IsInAutoAttackRange(target) && E.IsReady() && (aaToKill(target) > 2 || iAmLow()))
                E.Cast(MasterSharp.getCheckBoxItem(MasterSharp.extraMenu, "packets"));
        }

        public static void useRSmart(Obj_AI_Base target)
        {
            if (ObjectManager.Player.IsInAutoAttackRange(target) && R.IsReady() && aaToKill(target) > 5)
                R.Cast(MasterSharp.getCheckBoxItem(MasterSharp.extraMenu, "packets"));
        }

        public static void useQSmart(Obj_AI_Base target)
        {
            try
            {
                if (!Q.IsReady() || target.Path.Count() == 0 || !target.IsMoving)
                    return;
                var nextEnemPath = target.Path[0].To2D();
                var dist = player.Position.To2D().LSDistance(target.Position.To2D());
                var distToNext = nextEnemPath.LSDistance(player.Position.To2D());
                if (distToNext <= dist)
                    return;
                var msDif = player.MoveSpeed - target.MoveSpeed;
                if (msDif <= 0 && !ObjectManager.Player.IsInAutoAttackRange(target) && Orbwalker.CanAutoAttack)
                    Q.Cast(target);

                var reachIn = dist/msDif;
                if (reachIn > 4)
                    Q.Cast(target);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static void useSmiteOnTarget(Obj_AI_Base target)
        {
            if (smite != SpellSlot.Unknown && player.Spellbook.CanUseSpell(smite) == SpellState.Ready)
            {
                if (target.LSDistance(player, true) <= 700*700 &&
                    (yiGotItemRange(3714, 3718) || yiGotItemRange(3706, 3710)))
                {
                    player.Spellbook.CastSpell(smite, target);
                }
            }
        }

        public static bool iAmLow(float lownes = .25f)
        {
            return player.Health/player.MaxHealth < lownes;
        }

        public static int aaToKill(Obj_AI_Base target)
        {
            return 1 + (int) (target.Health/player.GetAutoAttackDamage(target));
        }

        public static void evadeBuff(BuffInstance buf, TargetedSkills.TargSkill skill)
        {
            if (Q.IsReady() && jumpEnesAround() != 0 && buf.EndTime - Game.Time < skill.delay/1000)
            {
                useQonBest();
            }
            else if (W.IsReady() && (!Q.IsReady() || jumpEnesAround() != 0) && buf.EndTime - Game.Time < 0.4f)
            {
                var dontMove = 400;
                Orbwalker.DisableMovement = true;
                W.Cast();
                Utility.DelayAction.Add(dontMove, () => Orbwalker.DisableMovement = false);
            }
        }

        public static void evadeDamage(int useQ, int useW, GameObjectProcessSpellCastEventArgs psCast, int delay = 250)
        {
            if (useQ != 0 && Q.IsReady() && jumpEnesAround() != 0 &&
                MasterSharp.getCheckBoxItem(MasterSharp.evadeMenu, "smartQDogue"))
            {
                if (delay != 0)
                    Utility.DelayAction.Add(delay, useQonBest);
                else
                    useQonBest();
            }
            else if (useW != 0 && W.IsReady() && MasterSharp.getCheckBoxItem(MasterSharp.evadeMenu, "smartW"))
            {
                var dontMove = 500;
                Orbwalker.DisableMovement = true;
                W.Cast();
                Utility.DelayAction.Add(dontMove, () => Orbwalker.DisableMovement = false);
            }
        }

        public static int jumpEnesAround()
        {
            return
                ObjectManager.Get<Obj_AI_Base>()
                    .Count(ob => ob.IsEnemy && !(ob is FollowerObject) && (ob is Obj_AI_Minion || ob is AIHeroClient) &&
                                 ob.LSDistance(player) < 600 && !ob.IsDead);
        }

        public static void evadeSkillShot(Skillshot sShot)
        {
            var sd = SpellDatabase.GetByMissileName(sShot.SpellData.MissileSpellName);
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo) &&
                (MasterSharp.skillShotMustBeEvaded(sd.MenuItemName) ||
                 MasterSharp.skillShotMustBeEvadedW(sd.MenuItemName)))
            {
                var spellDamage = (float) sShot.Unit.LSGetSpellDamage(player, sd.SpellName);
                var willKill = player.Health <= spellDamage;
                if (Q.IsReady() && jumpEnesAround() != 0 && MasterSharp.skillShotMustBeEvaded(sd.MenuItemName) ||
                    willKill)
                {
                    useQonBest();
                }
                else if ((!Q.IsReady(150) || !MasterSharp.skillShotMustBeEvaded(sd.MenuItemName)) && W.IsReady() &&
                         (MasterSharp.skillShotMustBeEvadedW(sd.MenuItemName) || willKill))
                {
                    var dontMove = 500;
                    Orbwalker.DisableMovement = true;
                    W.Cast();
                    Utility.DelayAction.Add(dontMove, () => Orbwalker.DisableMovement = false);
                }
            }

            if (!Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.None) &&
                (MasterSharp.skillShotMustBeEvadedAllways(sd.MenuItemName) ||
                 MasterSharp.skillShotMustBeEvadedWAllways(sd.MenuItemName)))
            {
                var spellDamage = (float) sShot.Unit.LSGetSpellDamage(player, sd.SpellName);
                var willKill = player.Health <= spellDamage;
                if (Q.IsReady() && jumpEnesAround() != 0 &&
                    (MasterSharp.skillShotMustBeEvadedAllways(sd.MenuItemName) || willKill))
                {
                    useQonBest();
                }
                else if ((!Q.IsReady() || !MasterSharp.skillShotMustBeEvadedAllways(sd.MenuItemName)) && W.IsReady() &&
                         (MasterSharp.skillShotMustBeEvadedWAllways(sd.MenuItemName) || willKill))
                {
                    var dontMove = 500;
                    Orbwalker.DisableMovement = true;
                    W.Cast();
                    Utility.DelayAction.Add(dontMove, () => Orbwalker.DisableMovement = false);
                }
            }
        }


        public static void useQonBest()
        {
            try
            {
                if (!Q.IsReady())
                {
                    //Console.WriteLine("Fuk uo here ");
                    return;
                }
                if (selectedTarget != null)
                {
                    if (selectedTarget.LSDistance(player) < 600)
                    {
                        // Console.WriteLine("Q on targ ");
                        Q.Cast(selectedTarget, MasterSharp.getCheckBoxItem(MasterSharp.extraMenu, "packets"));
                        return;
                    }

                    var bestOther =
                        ObjectManager.Get<Obj_AI_Base>()
                            .Where(
                                ob =>
                                    ob.IsEnemy && (ob is Obj_AI_Minion || ob is AIHeroClient) &&
                                    ob.LSDistance(player) < 600 && !ob.IsDead)
                            .OrderBy(ob => ob.LSDistance(selectedTarget, true)).FirstOrDefault();
                    //Console.WriteLine("do shit? " + bestOther.Name);

                    if (bestOther != null)
                    {
                        Q.Cast(bestOther, MasterSharp.getCheckBoxItem(MasterSharp.extraMenu, "packets"));
                    }
                }
                else
                {
                    var bestOther =
                        ObjectManager.Get<Obj_AI_Base>()
                            .Where(
                                ob =>
                                    ob.IsEnemy && !(ob is FollowerObject) && (ob is Obj_AI_Minion || ob is AIHeroClient) &&
                                    ob.LSDistance(player) < 600 && !ob.IsDead)
                            .OrderBy(ob => ob.LSDistance(Game.CursorPos, true)).FirstOrDefault();
                    //Console.WriteLine("do shit? " + bestOther.Name);

                    if (bestOther != null)
                    {
                        Q.Cast(bestOther, MasterSharp.getCheckBoxItem(MasterSharp.extraMenu, "packets"));
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        public static bool yiGotItemRange(int from, int to)
        {
            return player.InventoryItems.Any(item => (int) item.Id >= @from && (int) item.Id <= to);
        }
    }
}