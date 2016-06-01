using System;
using System.Collections.Generic;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using EloBuddy;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK;

namespace Slutty_ryze
{
    internal class LaneOptions
    {
        #region Public Functions

        public static bool getCheckBoxItem(Menu m, string item)
        {
            return m[item].Cast<CheckBox>().CurrentValue;
        }

        public static int getSliderItem(Menu m, string item)
        {
            return m[item].Cast<Slider>().CurrentValue;
        }

        public static bool getKeyBindItem(Menu m, string item)
        {
            return m[item].Cast<KeyBind>().CurrentValue;
        }

        public static int getBoxItem(Menu m, string item)
        {
            return m[item].Cast<ComboBox>().CurrentValue;
        }

        private static bool QSpell  { get { return getCheckBoxItem(MenuManager.combo1Menu, "useQ"); } }
        private static bool ESpell { get { return getCheckBoxItem(MenuManager.combo1Menu, "useE"); } }
        private static bool WSpell { get { return getCheckBoxItem(MenuManager.combo1Menu, "useW"); } }
        private static bool RSpell { get { return getCheckBoxItem(MenuManager.combo1Menu, "useR"); } }
        private static bool RwwSpell { get { return getCheckBoxItem(MenuManager.combo1Menu, "useRww"); } }
        private static readonly Random Seeder = new Random();

        //struct MinionHealthPerSecond
        //{
        //    public float LastHp;
        //    public float DamagePerSecond;
        //}

        //private MinionHealthPerSecond[] calcMinionHealth(Obj_AI_Base[] minionsBase)
        //{
        //    MinionHealthPerSecond[] minionsStruct = new MinionHealthPerSecond[minionsBase.Length];
        //    const int checkDelay = 2;
        //    for (int i = 0; checkDelay > i; i++)
        //    {
        //        var startTime = Utils.TickCount;
        //        var endTime = startTime + 1;
        //        if (Utils.TickCount < endTime)

        //        for (int index = 0; index < minionsBase.Length; index++)
        //        {
        //            if (minionsBase[index].IsDead)
        //                    continue;

        //             var cMinionHP = minionsBase[index].Health;

        //             if (Math.Abs(minionsStruct[index].LastHp) > 1)
        //                minionsStruct[index].DamagePerSecond = (minionsStruct[index].LastHp - minionsBase[index].Health/checkDelay);

        //            minionsStruct[index].LastHp = minionsBase[index].Health;
        //        }
        //    }

        //    return minionsStruct;
        //}

        public static void LaneClear()
        {
            if (GlobalManager.GetPassiveBuff == 4
                && !GlobalManager.GetHero.HasBuff("RyzeR")
                && getCheckBoxItem(MenuManager.laneMenu, "passiveproc"))
                return;

            var qlchSpell = getCheckBoxItem(MenuManager.laneMenu, "useQlc");
            var elchSpell = getCheckBoxItem(MenuManager.laneMenu, "useElc");
            var wlchSpell = getCheckBoxItem(MenuManager.laneMenu, "useWlc");

            var q2LSpell = getCheckBoxItem(MenuManager.laneMenu, "useQ2L");
            var e2LSpell = getCheckBoxItem(MenuManager.laneMenu, "useE2L");
            var w2LSpell = getCheckBoxItem(MenuManager.laneMenu, "useW2L");

            var rSpell = getCheckBoxItem(MenuManager.laneMenu, "useRl");
            var rSlider = getSliderItem(MenuManager.laneMenu, "rMin");
            var minMana = getSliderItem(MenuManager.laneMenu, "useEPL");

            var minionCount = MinionManager.GetMinions(GlobalManager.GetHero.Position, Champion.Q.Range, MinionTypes.All,
                MinionTeam.NotAlly);
            if (GlobalManager.GetHero.ManaPercent <= minMana)
                return;
            
            foreach (var minion in minionCount)
            {
                if (!GlobalManager.CheckMinion(minion)) continue;

                var minionHp = minion.Health;// Reduce Calls and add in randomization buffer.
                //if (GlobalManager.Config.Item("doHuman"))
                //    minionHp = minion.Health * (1 + (Seeder.Next(GlobalManager.Config.Item("minCreepHPOffset").GetValue<Slider>().Value, GlobalManager.Config.Item("maxCreepHPOffset").GetValue<Slider>().Value) / 100.0f));//Randomioze Minion Hp from min to max hp less than damage
                if (minion.IsDead) return;

                SpellSequence(minion, "useQ2L", "useE2L", "useW2L", "useRl");

                if (qlchSpell
                    && Champion.Q.IsReady()
                    && minion.LSIsValidTarget(Champion.Q.Range)
                    && minionHp <= Champion.Q.GetDamage(minion) && GlobalManager.CheckMinion(minion))
                    Champion.Q.Cast(minion);

                else if (wlchSpell
                         && Champion.W.IsReady()
                         && minion.LSIsValidTarget(Champion.W.Range)
                         && minionHp <= Champion.W.GetDamage(minion) && GlobalManager.CheckMinion(minion))
                    Champion.W.CastOnUnit(minion);

                else if (elchSpell
                         && Champion.E.IsReady()
                         && minion.LSIsValidTarget(Champion.E.Range)
                         && minionHp <= Champion.E.GetDamage(minion) && GlobalManager.CheckMinion(minion))
                    Champion.E.CastOnUnit(minion);

                if (rSpell
                    && Champion.R.IsReady()
                    && minion.LSIsValidTarget(Champion.Q.Range)
                    && minionCount.Count > rSlider && GlobalManager.CheckMinion(minion))
                    Champion.R.Cast();


            }
        }

        //get assembly version
        public static void JungleClear()
        {
            //Convert to use new system later
            var mSlider = getSliderItem(MenuManager.jungleMenu, "useJM");

          //  if (GlobalManager.GetHero.ManaPercent < mSlider)
            //    return;


            var jungle = MinionManager.GetMinions(Champion.Q.Range, MinionTypes.All, MinionTeam.Neutral,
                MinionOrderTypes.MaxHealth);
            foreach (var jung in jungle)
            {
                SpellSequence(jung, "useQj", "useWj", "useEj", "useRj");
            }

        }


        public static void LastHit()
        {
            #region Old
            var qlchSpell = getCheckBoxItem(MenuManager.lastMenu, "useQl2h");
            var elchSpell = getCheckBoxItem(MenuManager.lastMenu, "useEl2h");
            var wlchSpell = getCheckBoxItem(MenuManager.lastMenu, "useWl2h");

            var minionCount = MinionManager.GetMinions(GlobalManager.GetHero.Position, Champion.Q.Range, MinionTypes.All,
                MinionTeam.NotAlly);

            foreach (var minion in minionCount)
            {
                var minionHp = minion.Health;// Reduce Calls and add in randomization buffer.
                //if (GlobalManager.Config.Item("doHuman"))
                //    minionHp = minion.Health * (1 + (Seeder.Next(GlobalManager.Config.Item("minCreepHPOffset").GetValue<Slider>().Value, GlobalManager.Config.Item("maxCreepHPOffset").GetValue<Slider>().Value) / 100.0f));//Randomioze Minion Hp from min to max hp less than damage

                if (qlchSpell
                    && Champion.Q.IsReady()
                    && minion.LSIsValidTarget(Champion.Q.Range - 20)
                    && minionHp < Champion.Q.GetDamage(minion))
                    Champion.Q.Cast(minion);

                if (wlchSpell
                    && Champion.W.IsReady()
                    && minion.LSIsValidTarget(Champion.W.Range - 10)
                    && minionHp < Champion.W.GetDamage(minion))
                    Champion.W.CastOnUnit(minion);

                if (elchSpell
                    && Champion.E.IsReady()
                    && minion.LSIsValidTarget(Champion.E.Range - 10)
                    && minionHp < Champion.E.GetDamage(minion))
                    Champion.E.CastOnUnit(minion);
            }
            #endregion
            #region New
            //New verion
            //var bSpells = new bool[3];
            //bSpells[0] = GlobalManager.Config.Item("useQl2h");
            //bSpells[1] = GlobalManager.Config.Item("useEl2h");
            //bSpells[2] = GlobalManager.Config.Item("useWl2h");


            //var minionCount = MinionManager.GetMinions(GlobalManager.GetHero.Position, Champion.Q.Range, MinionTypes.All,
            //    MinionTeam.NotAlly);

            //var minionHpOffset = 1.0f;
            //if (GlobalManager.Config.Item("doHuman"))
            //    minionHpOffset = (1 + (Seeder.Next(GlobalManager.Config.Item("minCreepHPOffset").GetValue<Slider>().Value, GlobalManager.Config.Item("maxCreepHPOffset").GetValue<Slider>().Value) / 100.0f));//Randomioze Minion Hp from min to max hp less than damage

            //foreach (var minion in minionCount)
            //{
            //    StartComboSequence(minion, bSpells, new[] { 'Q', 'W', 'E' }, minionHpOffset);
            //}
            #endregion
        }

        public static void Mixed()
        {
            #region Old
            var qSpell = getCheckBoxItem(MenuManager.mixedMenu, "UseQM");
            var qlSpell = getCheckBoxItem(MenuManager.mixedMenu, "UseQMl");
            var eSpell = getCheckBoxItem(MenuManager.mixedMenu, "UseEM");
            var wSpell = getCheckBoxItem(MenuManager.mixedMenu, "UseWM");
            var minMana = getSliderItem(MenuManager.laneMenu, "useEPL");

            if (GlobalManager.GetHero.ManaPercent < getSliderItem(MenuManager.mixedMenu, "mMin"))
                return;


            var target = TargetSelector.GetTarget(900, DamageType.Magical);
            if (qSpell
                && Champion.Q.IsReady()
                && target.LSIsValidTarget(Champion.Q.Range))
                Champion.Q.Cast(target);

            if (wSpell
                && Champion.W.IsReady()
                && target.LSIsValidTarget(Champion.W.Range))
                Champion.W.CastOnUnit(target);

            if (eSpell
                && Champion.E.IsReady()
                && target.LSIsValidTarget(Champion.E.Range))
                Champion.E.CastOnUnit(target);

            var minionCount = MinionManager.GetMinions(GlobalManager.GetHero.Position, Champion.Q.Range, MinionTypes.All,
                MinionTeam.NotAlly);
            {
                if (GlobalManager.GetHero.ManaPercent <= minMana)
                    return;

                foreach (
                    var minion in
                        minionCount.Where(
                            minion =>
                                qlSpell && Champion.Q.IsReady() && minion.Health < Champion.Q.GetDamage(minion) &&
                                GlobalManager.CheckTarget(minion)))
                {
                    Champion.Q.Cast(minion);
                }
            }
#endregion


        }

        public static void CastQ(Obj_AI_Base target, bool menu = true)
        {
            if (menu && !QSpell) return;
            if (target.LSIsValidTarget(Champion.Q.Range)
                && Champion.Q.IsReady())
                Champion.Q.Cast(target);
        }

        public static void CastQn(Obj_AI_Base target, bool menu = true)
        {
            if (menu && !QSpell) return;
            if (target.LSIsValidTarget(Champion.Qn.Range)
                && Champion.Qn.IsReady())
                Champion.Qn.Cast(target);
        }

        public static void CastW(Obj_AI_Base target, bool menu = true)
        {
            if (menu && !WSpell) return;
            if (target.LSIsValidTarget(Champion.W.Range)
                && Champion.W.IsReady())
                Champion.W.Cast(target);
        }

        public static void CastE(Obj_AI_Base target, bool menu = true)
        {
            if (menu && !ESpell) return;
            if (target.LSIsValidTarget(Champion.E.Range)
                && Champion.E.IsReady())
                Champion.E.Cast(target);
        }

        public static void CastR(Obj_AI_Base target, bool menu = true)
        {
            if (menu && !RSpell) return;
            if (!Champion.R.IsReady())
                return;
            if (target.LSIsValidTarget(Champion.W.Range)
                && target.Health > (Champion.Q.GetDamage(target) + Champion.E.GetDamage(target)))
            {
                if (target.HasBuff("RyzeW"))
                    Champion.R.Cast();
            }
        }

        public static void SpellSequence(Obj_AI_Base target, string Q, string W, string E, string R)
        {
            var menu = MenuManager.jungleMenu;

            var qSpell = false;
            var wSpell = false;
            var eSpell = false;
            var rSpell = false;

            if (Q.Contains("2"))
            {
                menu = MenuManager.laneMenu;
                qSpell = getCheckBoxItem(MenuManager.laneMenu, "useQ2L");
                wSpell = getCheckBoxItem(MenuManager.laneMenu, "useW2L");
                eSpell = getCheckBoxItem(MenuManager.laneMenu, "useE2L");
                rSpell = getCheckBoxItem(MenuManager.laneMenu, "useRl");
            }
            else
            {
                menu = MenuManager.jungleMenu;
                qSpell = getCheckBoxItem(MenuManager.jungleMenu, "useQj");
                wSpell = getCheckBoxItem(MenuManager.jungleMenu, "useWj");
                eSpell = getCheckBoxItem(MenuManager.jungleMenu, "useEj");
                rSpell = getCheckBoxItem(MenuManager.jungleMenu, "useRj");
            }

            switch (getBoxItem(MenuManager.combo1Menu, "combooptions"))
            {
                case 0:
                    if (target.LSIsValidTarget(Champion.Q.Range))
                    {
                        if (GlobalManager.GetPassiveBuff <= 1 && !GlobalManager.GetHero.HasBuff("ryzepassivecharged"))
                        {
                            if (qSpell)
                                CastQ(target, false);
                            if (eSpell)
                                CastE(target, false);
                            if (wSpell)
                                CastW(target, false);
                            if (rSpell)
                                CastR(target, false);
                        }

                        if (GlobalManager.GetPassiveBuff == 2)
                        {
                            if (qSpell)
                                CastQn(target, false);
                            if (wSpell)
                                CastW(target, false);
                            if (eSpell)
                                CastE(target, false);
                            if (rSpell)
                                CastR(target, false);

                        }


                        if (GlobalManager.GetPassiveBuff == 3)
                        {
                            if (qSpell)
                                CastQn(target, false);
                            if (eSpell)
                                CastE(target, false);
                            if (wSpell)
                                CastW(target, false);
                            if (rSpell)
                                CastR(target, false);
                        }

                        if (GlobalManager.GetPassiveBuff == 4)
                        {
                            if (wSpell)
                                CastW(target, false);
                            if (qSpell)
                                CastQn(target, false);
                            if (eSpell)
                                CastE(target, false);
                            if (rSpell)
                                CastR(target, false);
                        }

                        if (GlobalManager.GetHero.HasBuff("ryzepassivecharged"))
                        {
                            if (wSpell)
                                CastW(target, false);
                            if (qSpell)
                                CastQn(target, false);
                            if (eSpell)
                                CastE(target, false);
                            if (rSpell)
                                CastR(target, false);
                        }

                        if (Champion.R.IsReady() &&
                            (GlobalManager.GetPassiveBuff == 4 || GlobalManager.GetHero.HasBuff("ryzepassivecharged")) &&
                            rSpell)
                        {
                            if (!Champion.Q.IsReady() && !Champion.W.IsReady() && !Champion.E.IsReady())
                            {
                                Champion.R.Cast();
                            }
                        }
                    }
                    break;

                case 1:
                    if (target.LSIsValidTarget(Champion.Q.Range))
                    {
                        if (Champion.R.IsReady() || !Champion.R.IsReady())
                        {
                            if (GlobalManager.GetPassiveBuff <= 1 &&
                                !GlobalManager.GetHero.HasBuff("ryzepassivecharged"))
                            {
                                if (rSpell)
                                    CastR(target, false);
                                if (eSpell)
                                    CastE(target, false);
                                if (qSpell)
                                    CastQ(target, false);
                                if (wSpell)
                                    CastW(target, false);
                            }

                            if (GlobalManager.GetPassiveBuff == 2)
                            {
                                if (rSpell)
                                    CastR(target, false);
                                if (eSpell)
                                    CastE(target, false);
                                if (wSpell)
                                    CastW(target, false);
                                if (qSpell)
                                    CastQn(target, false);
                            }


                            if (GlobalManager.GetPassiveBuff == 3)
                            {
                                if (wSpell)
                                    CastW(target, false);
                                if (rSpell)
                                    CastR(target, false);
                                if (qSpell)
                                    CastQn(target, false);
                                if (eSpell)
                                    CastE(target, false);
                            }

                            if (GlobalManager.GetPassiveBuff == 4)
                            {
                                if (eSpell)
                                    CastE(target, false);
                                if (rSpell)
                                    CastR(target, false);
                                if (wSpell)
                                    CastW(target, false);
                                if (qSpell)
                                    CastQn(target, false);
                            }

                            if (GlobalManager.GetHero.HasBuff("ryzepassivecharged"))
                            {
                                if (rSpell)
                                    CastR(target, false);
                                if (wSpell)
                                    CastW(target, false);
                                if (qSpell)
                                    CastQn(target, false);
                                if (eSpell)
                                    CastE(target, false);
                            }
                        }

                        //if (!Champion.R.IsReady())
                        //{
                        //    if (GlobalManager.GetPassiveBuff <= 1 &&
                        //        !GlobalManager.GetHero.HasBuff("ryzepassivecharged"))
                        //    {
                        //        if (rSpell)
                        //            CastR(target, false);
                        //        if (eSpell)
                        //            CastE(target, false);
                        //        if (qSpell)
                        //            CastQ(target, false);
                        //        if (wSpell)
                        //            CastW(target, false);
                        //    }

                        //    if (GlobalManager.GetPassiveBuff == 2)
                        //    {
                        //        if (rSpell)
                        //            CastR(target, false);
                        //        if (eSpell)
                        //            CastE(target, false);
                        //        if (wSpell)
                        //            CastW(target, false);
                        //        if (qSpell)
                        //            CastQn(target, false);
                        //    }


                        //    if (GlobalManager.GetPassiveBuff == 3)
                        //    {
                        //        if (wSpell)
                        //            CastW(target, false);
                        //        if (rSpell)
                        //            CastR(target, false);
                        //        if (qSpell)
                        //            CastQn(target, false);
                        //        if (eSpell)
                        //            CastE(target, false);
                        //    }

                        //    if (GlobalManager.GetPassiveBuff == 4)
                        //    {
                        //        if (eSpell)
                        //            CastE(target, false);
                        //        if (rSpell)
                        //            CastR(target, false);
                        //        if (wSpell)
                        //            CastW(target, false);
                        //        if (qSpell)
                        //            CastQn(target, false);
                        //    }

                        //    if (GlobalManager.GetHero.HasBuff("ryzepassivecharged"))
                        //    {
                        //        if (rSpell)
                        //            CastR(target, false);
                        //        if (wSpell)
                        //            CastW(target, false);
                        //        if (qSpell)
                        //            CastQn(target, false);
                        //        if (eSpell)
                        //            CastE(target, false);
                        //    }
                        //}


                        if (Champion.R.IsReady() &&
                            (GlobalManager.GetPassiveBuff == 4 ||
                             GlobalManager.GetHero.HasBuff("ryzepassivecharged")) &&
                            rSpell)
                        {
                            if (!Champion.Q.IsReady() && !Champion.W.IsReady() && !Champion.E.IsReady())
                            {
                                Champion.R.Cast();
                            }
                        }
                    }
                    break;
            }
        }

        public static
            void ImprovedCombo()
        {
            Champion.SetIgniteSlot(GlobalManager.GetHero.GetSpellSlot("summonerdot"));
            var target = TargetSelector.GetTarget(Champion.W.Range, DamageType.Magical);

            if (!target.LSIsValidTarget(Champion.Q.Range) || !GlobalManager.CheckTarget(target)) return;

            if (target.LSIsValidTarget(Champion.W.Range) &&
                (target.Health < Champion.IgniteDamage(target) + Champion.W.GetDamage(target)))
                GlobalManager.GetHero.Spellbook.CastSpell(Champion.GetIgniteSlot(), target);

            var bSpells = new bool[5];
            var qSpell = getCheckBoxItem(MenuManager.combo1Menu, "useQ");
            var eSpell = getCheckBoxItem(MenuManager.combo1Menu, "useE");
            var wSpell = getCheckBoxItem(MenuManager.combo1Menu, "useW");
            var rSpell = getCheckBoxItem(MenuManager.combo1Menu, "useR");
            var rwwSpell = getCheckBoxItem(MenuManager.combo1Menu, "useRww");

            if (target.LSIsValidTarget(Champion.Q.Range))
            {
                if (GlobalManager.GetPassiveBuff <= 1 && !GlobalManager.GetHero.HasBuff("ryzepassivecharged"))
                {
                    CastQ(target);
                    CastE(target);
                    CastW(target);
                    CastR(target);
                }

                if (GlobalManager.GetPassiveBuff == 2)
                {
                    CastR(target);
                    CastQn(target);
                    CastW(target);
                    CastQn(target);
                    CastE(target);
                    CastQn(target);
                    CastW(target);
                    CastQn(target);
                    CastE(target);
                }


                if (GlobalManager.GetPassiveBuff == 3 && Champion.R.IsReady())
                {
                    CastR(target);
                    CastW(target);
                    CastQn(target);
                    CastE(target);
                    CastQn(target);
                    CastW(target);
                    CastQn(target);
                    CastE(target);
                }

                if (GlobalManager.GetPassiveBuff == 3 && !Champion.R.IsReady())
                {
                    CastQn(target);
                    CastW(target);
                    CastQn(target);
                    CastE(target);
                    CastW(target);
                    CastQn(target);
                    CastE(target);
                }

                if (GlobalManager.GetPassiveBuff == 4)
                {
                    CastW(target);
                    CastQn(target);
                    CastE(target);
                    CastR(target);
                }

                if (GlobalManager.GetHero.HasBuff("ryzepassivecharged"))
                {
                    CastW(target);
                    CastQn(target);
                    CastE(target);
                    CastR(target);
                }
            }
           else
           {
               if (wSpell 
                   && Champion.W.IsReady()
                 && target.LSIsValidTarget(Champion.W.Range))
                  Champion.W.CastOnUnit(target);

                if (qSpell
                 && Champion.Qn.IsReady()
                 && target.LSIsValidTarget(Champion.Qn.Range))
                  Champion.Qn.Cast(target);

              if (eSpell
                  && Champion.E.IsReady()
                    && target.LSIsValidTarget(Champion.E.Range))                  Champion.E.CastOnUnit(target);            }
            if (Champion.R.IsReady() && (GlobalManager.GetPassiveBuff == 4 || GlobalManager.GetHero.HasBuff("ryzepassivecharged")) && rSpell)
            {
                if (!Champion.Q.IsReady() && !Champion.W.IsReady() && !Champion.E.IsReady())
                {
                    Champion.R.Cast();
                }
            }
        }

        private static void StartComboSequence(Obj_AI_Base target, IReadOnlyList<bool> bSpells, IEnumerable<char> seq, float hpOffset = 1)
        {
            foreach (var com in seq)
            {
                var isMinion = target.IsMinion;
                switch (com)
                {
                    case 'Q':
                        if (!bSpells[0]) continue;

                        if (target.LSIsValidTarget(Champion.Q.Range) && Champion.Q.IsReady() && !target.IsInvulnerable)
                        {
                            if (isMinion &&
                                !(target.Health * hpOffset < Champion.Q.GetDamage(target) &&
                                  GlobalManager.CheckMinion(target))) continue;
                            if (GlobalManager.GetPassiveBuff > 2 ||
                                GlobalManager.GetHero.HasBuff("RyzePassiveStack") && Champion.Q.IsReady())
                                Champion.Qn.Cast(target);
                            else
                                Champion.Q.Cast(target);
                        }

                        continue;

                    case 'W':
                        if (!bSpells[1]) continue;

                        if (target.LSIsValidTarget(Champion.W.Range) && bSpells[1] && Champion.W.IsReady() && !target.IsInvulnerable)
                            if (isMinion && !(target.Health * hpOffset < Champion.W.GetDamage(target) && GlobalManager.CheckMinion(target))) continue;
                        Champion.W.Cast(target);

                        continue;

                    case 'E':
                        if (!bSpells[2]) continue;

                        if (target.LSIsValidTarget(Champion.E.Range) && bSpells[2] && Champion.E.IsReady() && !target.IsInvulnerable)
                            if (isMinion && !(target.Health * hpOffset < Champion.E.GetDamage(target) && GlobalManager.CheckMinion(target))) continue;
                        Champion.E.Cast(target);


                        continue;

                    case 'R':
                        if (!bSpells[3]) continue;
                        if (!target.LSIsValidTarget(Champion.W.Range) || !(target.Health > (Champion.Q.GetDamage(target) + Champion.E.GetDamage(target))) || target.IsInvulnerable)
                            continue;
                        if (!Champion.R.IsReady()) continue;
                        if (bSpells[4] && target.HasBuff("RyzeW") || !bSpells[4])
                            Champion.R.Cast();
                        continue;
                }

            }

            if (!Champion.R.IsReady() || GlobalManager.GetPassiveBuff != 4 || !bSpells[4]) return;
            if (Champion.Q.IsReady() || Champion.W.IsReady() || Champion.E.IsReady()) return;

            Champion.R.Cast();
        }
    }
}
#endregion
