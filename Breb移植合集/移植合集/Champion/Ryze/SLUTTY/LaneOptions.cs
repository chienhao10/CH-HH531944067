using System;
using System.Collections.Generic;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using LeagueSharp.Common;

namespace Slutty_ryze
{
    internal class LaneOptions
    {
        #region Public Functions

        public static Menu
            _config = MenuManager._config,
            humanizerMenu = MenuManager.humanizerMenu,
            combo1Menu = MenuManager.combo1Menu,
            mixedMenu = MenuManager.mixedMenu,
            laneMenu = MenuManager.laneMenu,
            jungleMenu = MenuManager.jungleMenu,
            lastMenu = MenuManager.lastMenu,
            passiveMenu = MenuManager.passiveMenu,
            itemMenu = MenuManager.itemMenu,
            eventMenu = MenuManager.eventMenu,
            ksMenu = MenuManager.ksMenu,
            chase = MenuManager.chase;

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

        private static bool QSpell
        {
            get { return getCheckBoxItem(combo1Menu, "useQ"); }
        }

        private static bool ESpell
        {
            get { return getCheckBoxItem(combo1Menu, "useE"); }
        }

        private static bool WSpell
        {
            get { return getCheckBoxItem(combo1Menu, "useW"); }
        }

        private static bool RSpell
        {
            get { return getCheckBoxItem(combo1Menu, "useR"); }
        }

        public static void JungleClear()
        {
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

            var qlchSpell = getCheckBoxItem(lastMenu, "useQl2h");
            var elchSpell = getCheckBoxItem(lastMenu, "useEl2h");
            var wlchSpell = getCheckBoxItem(lastMenu, "useWl2h");

            var minionCount = MinionManager.GetMinions(GlobalManager.GetHero.Position, Champion.Q.Range, MinionTypes.All,
                MinionTeam.NotAlly);

            foreach (var minion in minionCount)
            {
                var minionHp = minion.Health;
                if (qlchSpell
                    && Champion.Q.IsReady()
                    && minion.IsValidTarget(Champion.Q.Range - 20)
                    && minionHp < Champion.Q.GetDamage(minion))
                    Champion.Q.Cast(minion);

                if (wlchSpell
                    && Champion.W.IsReady()
                    && minion.IsValidTarget(Champion.W.Range - 10)
                    && minionHp < Champion.W.GetDamage(minion))
                    Champion.W.CastOnUnit(minion);

                if (elchSpell
                    && Champion.E.IsReady()
                    && minion.IsValidTarget(Champion.E.Range - 10)
                    && minionHp < Champion.E.GetDamage(minion))
                    Champion.E.CastOnUnit(minion);
            }

            #endregion
        }

        public static void Mixed()
        {
            #region Old

            var qSpell = getCheckBoxItem(mixedMenu, "UseQM");
            var qlSpell = getCheckBoxItem(mixedMenu, "UseQMl");
            var eSpell = getCheckBoxItem(mixedMenu, "UseEM");
            var wSpell = getCheckBoxItem(mixedMenu, "UseWM");
            var minMana = getSliderItem(mixedMenu, "mMin");

            if (GlobalManager.GetHero.ManaPercent < minMana)
                return;


            var target = TargetSelector.GetTarget(900, DamageType.Magical);
            if (qSpell
                && Champion.Q.IsReady()
                && target.IsValidTarget(Champion.Q.Range))
                Champion.Q.Cast(target);

            if (wSpell
                && Champion.W.IsReady()
                && target.IsValidTarget(Champion.W.Range))
                Champion.W.CastOnUnit(target);

            if (eSpell
                && Champion.E.IsReady()
                && target.IsValidTarget(Champion.E.Range))
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
            if (target.IsValidTarget(Champion.Q.Range) && Champion.Q.IsReady())
                Champion.Q.Cast(target);
        }

        public static void CastQn(Obj_AI_Base target, bool menu = true)
        {
            if (menu && !QSpell) return;
            if (target.IsValidTarget(Champion.Q.Range) && Champion.Q.IsReady())
                Champion.Q.Cast(target);
        }

        public static void CastW(Obj_AI_Base target, bool menu = true)
        {
            if (menu && !WSpell) return;
            if (target.IsValidTarget(Champion.W.Range)
                && Champion.W.IsReady())
                Champion.W.Cast(target);
        }

        public static void CastE(Obj_AI_Base target, bool menu = true)
        {
            if (menu && !ESpell) return;
            if (target.IsValidTarget(Champion.E.Range)
                && Champion.E.IsReady())
                Champion.E.Cast(target);
        }

        public static void CastR(Obj_AI_Base target, bool menu = true)
        {
            if (menu && !RSpell) return;
            if (!Champion.R.IsReady())
                return;
            if (target.IsValidTarget(Champion.W.Range)
                && target.Health > Champion.Q.GetDamage(target) + Champion.E.GetDamage(target))
            {
                if (target.HasBuff("RyzeW"))
                    Champion.R.Cast();
            }
        }

        public static void SpellSequence(Obj_AI_Base target, string Q, string W, string E, string R)
        {
            //SpellSequence(jung, "useQj", "useWj", "useEj", "useRj");
            //SpellSequence(minion, "useQ2L", "useE2L", "useW2L", "useRl");

            var qSpell = false;
            var wSpell = false;
            var eSpell = false;
            var rSpell = false;

            if (Q == "useQj")
            {
                qSpell = getCheckBoxItem(jungleMenu, "useQj");
            }

            if (Q == "useQ2L")
            {
                qSpell = getCheckBoxItem(laneMenu, "useQj");
                Console.WriteLine(qSpell.ToString());
            }

            if (W == "useWj")
            {
                wSpell = getCheckBoxItem(jungleMenu, "useWj");
            }

            if (W == "useE2L")
            {
                wSpell = getCheckBoxItem(laneMenu, "useW2L");
                Console.WriteLine(wSpell.ToString());
            }

            if (E == "useEj")
            {
                eSpell = getCheckBoxItem(jungleMenu, "useEj");
            }

            if (E == "useW2L")
            {
                eSpell = getCheckBoxItem(laneMenu, "useE2L");
                Console.WriteLine(eSpell.ToString());
            }

            if (R == "useRj")
            {
                rSpell = getCheckBoxItem(jungleMenu, "useRj");
            }

            if (R == "useRl")
            {
                rSpell = getCheckBoxItem(laneMenu, "useRl");
                Console.WriteLine(rSpell.ToString());
            }

            switch (getBoxItem(combo1Menu, "combooptions"))
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

        public static void COMBO()
        {
            Champion.SetIgniteSlot(GlobalManager.GetHero.GetSpellSlot("summonerdot"));
            var target = TargetSelector.GetTarget(Champion.W.Range, DamageType.Magical);

            if (!target.IsValidTarget(Champion.Q.Range) || !GlobalManager.CheckTarget(target))
            {
                return;
            }

            if (target.IsValidTarget(Champion.W.Range) &&
                (target.Health < Champion.IgniteDamage(target) + Champion.W.GetDamage(target)))
                GlobalManager.GetHero.Spellbook.CastSpell(Champion.GetIgniteSlot(), target);

            var qSpell = getCheckBoxItem(combo1Menu, "useQ");
            var eSpell = getCheckBoxItem(combo1Menu, "useE");
            var wSpell = getCheckBoxItem(combo1Menu, "useW");
            var rSpell = getCheckBoxItem(combo1Menu, "useR");

            if (target.IsValidTarget(Champion.Q.Range))
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
                    CastQn(target);
                    CastW(target);
                    CastE(target);
                    CastR(target);
                }


                if (GlobalManager.GetPassiveBuff == 3)
                {
                    CastQn(target);
                    CastE(target);
                    CastW(target);
                    CastR(target);
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
                if (wSpell && Champion.W.IsReady() && target.IsValidTarget(Champion.W.Range))
                    Champion.W.CastOnUnit(target);

                if (qSpell && Champion.Qn.IsReady() && target.IsValidTarget(Champion.Qn.Range))
                    Champion.Qn.Cast(target);

                if (eSpell && Champion.E.IsReady() && target.IsValidTarget(Champion.E.Range))
                    Champion.E.CastOnUnit(target);
            }
            if (Champion.R.IsReady() &&
                (GlobalManager.GetPassiveBuff == 4 || GlobalManager.GetHero.HasBuff("ryzepassivecharged")) && rSpell)
            {
                if (!Champion.Q.IsReady() && !Champion.W.IsReady() && !Champion.E.IsReady())
                {
                    Champion.R.Cast();
                }
            }
        }
    }
}

#endregion