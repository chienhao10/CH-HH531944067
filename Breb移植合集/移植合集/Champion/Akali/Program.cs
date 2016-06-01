using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using LeagueSharp.Common;
using Damage = LeagueSharp.Common.Damage;
using Spell = LeagueSharp.Common.Spell;

namespace PortAIO.Champion.Akali
{
    internal class Program
    {
        public const string ChampionName = "Akali";

        public static Spell Q;
        public static Spell W;
        public static Spell E;
        public static Spell R;
        public static List<Spell> SpellList = new List<Spell>();

        public static SpellSlot IgniteSlot;
        public static Items.Item Hex;
        public static Items.Item Cutlass;

        public static Menu Menu, ComboMenu, HarassMenu, FarmMenu, DrawingMenu, MiscMenu, JungleMenu;

        private static AIHeroClient myHero
        {
            get { return Player.Instance; }
        }

        private static AIHeroClient enemyHaveMota
        {
            get
            {
                return
                    (from enemy in
                        ObjectManager.Get<AIHeroClient>().Where(enemy => enemy.IsEnemy && enemy.LSIsValidTarget(R.Range))
                        from buff in enemy.Buffs
                        where buff.DisplayName == "AkaliMota"
                        select enemy).FirstOrDefault();
            }
        }

        public static void Main()
        {
            Game_OnGameLoad();
        }

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

        private static void Game_OnGameLoad()
        {
            if (myHero.BaseSkinName != ChampionName)
                return;

            Q = new Spell(SpellSlot.Q, 600f);
            W = new Spell(SpellSlot.W, 700f);
            E = new Spell(SpellSlot.E, 290f);
            R = new Spell(SpellSlot.R, 800f);

            SpellList.Add(Q);
            SpellList.Add(W);
            SpellList.Add(E);
            SpellList.Add(R);

            IgniteSlot = myHero.GetSpellSlot("SummonerDot");
            Hex = new Items.Item(3146, 700);
            Cutlass = new Items.Item(3144, 450);

            Menu = MainMenu.AddMenu("xQx | " + ChampionName, ChampionName);

            ComboMenu = Menu.AddSubMenu("连招", "Combo");
            ComboMenu.AddGroupLabel("按下连招键！让脚本带你上王者!");

            HarassMenu = Menu.AddSubMenu("骚扰", "Harass");
            HarassMenu.Add("UseQHarass", new CheckBox("使用 Q"));
            HarassMenu.Add("UseEHarass", new CheckBox("使用 E"));
            HarassMenu.Add("HarassEnergy", new Slider("骚扰最低能量: ", 50));
            HarassMenu.Add("HarassUseQT", new KeyBind("使用 Q (开关)!", false, KeyBind.BindTypes.PressToggle, 'J'));

            FarmMenu = Menu.AddSubMenu("骚扰", "Farm");
            FarmMenu.Add("UseQFarm", new CheckBox("使用 Q"));
            FarmMenu.Add("UseEFarm", new CheckBox("使用 E"));

            JungleMenu = Menu.AddSubMenu("清野", "JungleFarm");
            JungleMenu.Add("UseQJFarm", new CheckBox("使用 Q"));
            JungleMenu.Add("UseEJFarm", new CheckBox("使用 E"));

            MiscMenu = Menu.AddSubMenu("杂项", "Misc");
            MiscMenu.Add("KillstealR", new CheckBox("抢头 R", false));

            DrawingMenu = Menu.AddSubMenu("线圈", "Drawings");
            DrawingMenu.Add("QRange", new CheckBox("Q 范围"));
            DrawingMenu.Add("RRange", new CheckBox("R 范围"));
            
            Drawing.OnDraw += Drawing_OnDraw;
            Game.OnUpdate += Game_OnUpdate;
        }

        private static float GetComboDamage(Obj_AI_Base vTarget)
        {
            if (vTarget == null)
            {
                return 0.0f;
            }

            var fComboDamage = 0d;

            if (Q.IsReady())
                fComboDamage += myHero.LSGetSpellDamage(vTarget, SpellSlot.Q) +
                                myHero.LSGetSpellDamage(vTarget, SpellSlot.Q, 1);
            if (E.IsReady())
                fComboDamage += myHero.LSGetSpellDamage(vTarget, SpellSlot.E);

            if (R.IsReady())
                fComboDamage += myHero.LSGetSpellDamage(vTarget, SpellSlot.R)*R.Instance.Ammo;

            if (Items.CanUseItem(3146))
                fComboDamage += myHero.GetItemDamage(vTarget, Damage.DamageItems.Hexgun);

            if (IgniteSlot != SpellSlot.Unknown && myHero.Spellbook.CanUseSpell(IgniteSlot) == SpellState.Ready)
                fComboDamage += myHero.GetSummonerSpellDamage(vTarget, Damage.SummonerSpell.Ignite);

            return (float) fComboDamage;
        }

        private static void Game_OnUpdate(EventArgs args)
        {
            if (W.IsReady() && myHero.LSCountEnemiesInRange(W.Range/2 + 100) >= 2)
            {
                W.Cast(myHero.Position);
            }

            if (myHero.HasBuff("zedulttargetmark"))
            {
                if (W.IsReady())
                {
                    W.Cast(myHero.Position);
                }
            }

            Orbwalker.DisableAttacking = false;

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                Combo();
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass) ||
                getKeyBindItem(HarassMenu, "HarassUseQT"))
            {
                var vEnergy = getSliderItem(HarassMenu, "HarassEnergy");
                if (myHero.ManaPercent >= vEnergy)
                    Harass();
            }

            var lc = Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear);
            if (lc)
            {
                Farm(lc);
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear))
            {
                JungleFarm();
            }

            if (getCheckBoxItem(MiscMenu, "KillstealR"))
            {
                Killsteal();
            }
        }

        private static void Combo()
        {
            var t = TargetSelector.GetTarget(R.IsReady() ? R.Range : Q.Range, DamageType.Magical);

            if (t == null)
            {
                return;
            }

            if (GetComboDamage(t) > t.Health && IgniteSlot != SpellSlot.Unknown &&
                myHero.Spellbook.CanUseSpell(IgniteSlot) == SpellState.Ready)
            {
                myHero.Spellbook.CastSpell(IgniteSlot, t);
            }

            if (Q.IsReady() && t.LSIsValidTarget(Q.Range))
            {
                Q.CastOnUnit(t);
            }

            if (Hex.IsReady() && t.LSIsValidTarget(Hex.Range))
            {
                Hex.Cast(t);
            }

            if (Cutlass.IsReady() && t.LSIsValidTarget(Cutlass.Range))
            {
                Cutlass.Cast(t);
            }

            var motaEnemy = enemyHaveMota;
            if (motaEnemy != null && motaEnemy.LSIsValidTarget(Orbwalking.GetRealAutoAttackRange(t)))
                return;

            if (E.IsReady() && t.LSIsValidTarget(E.Range))
            {
                E.Cast();
            }

            if (R.IsReady() && t.LSIsValidTarget(R.Range))
            {
                R.CastOnUnit(t);
            }
        }

        private static void Harass()
        {
            var useQ = getCheckBoxItem(HarassMenu, "UseQHarass") && Q.IsReady();
            var useE = getCheckBoxItem(HarassMenu, "UseEHarass") && E.IsReady();
            var t = TargetSelector.GetTarget(Q.Range, DamageType.Magical);

            if (useQ && t.LSIsValidTarget(Q.Range))
            {
                Q.CastOnUnit(t);
            }

            if (useE && t.LSIsValidTarget(E.Range))
            {
                E.Cast();
            }
        }

        private static void Farm(bool laneClear)
        {
            var allMinions = MinionManager.GetMinions(myHero.ServerPosition, Q.Range);
            var useQ = getCheckBoxItem(FarmMenu, "UseQFarm");
            var useE = getCheckBoxItem(FarmMenu, "UseEFarm");

            if (useQ && Q.IsReady())
            {
                foreach (var minion in allMinions)
                {
                    if (minion.LSIsValidTarget() &&
                        HealthPrediction.GetHealthPrediction(minion, (int) (myHero.LSDistance(minion)*1000/1400)) <
                        0.75*myHero.LSGetSpellDamage(minion, SpellSlot.Q))
                    {
                        Q.CastOnUnit(minion);
                        return;
                    }
                }
            }

            if (useE && E.IsReady())
            {
                if (
                    allMinions.Any(
                        minion =>
                            minion.LSIsValidTarget(E.Range) &&
                            minion.Health < 0.75*myHero.LSGetSpellDamage(minion, SpellSlot.E) &&
                            minion.LSIsValidTarget(E.Range)))
                {
                    E.Cast();
                    return;
                }
            }

            if (laneClear)
            {
                foreach (var minion in allMinions)
                {
                    if (useQ)
                        Q.CastOnUnit(minion);

                    if (useE && minion.LSIsValidTarget(E.Range))
                        E.Cast();
                }
            }
        }

        private static void JungleFarm()
        {
            var mobs = MinionManager.GetMinions(
                myHero.ServerPosition, Q.Range, MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth);
            if (mobs.Count > 0)
            {
                var mob = mobs[0];

                if (Q.IsReady())
                    Q.CastOnUnit(mob);

                if (E.IsReady() && E.IsInRange(mob))
                    E.Cast();
            }
        }

        private static void Killsteal()
        {
            var useR = getCheckBoxItem(MiscMenu, "KillstealR") && R.IsReady();
            if (useR)
            {
                foreach (var hero in ObjectManager.Get<AIHeroClient>().Where(hero => hero.LSIsValidTarget(R.Range)))
                {
                    if (hero.LSDistance(ObjectManager.Player) <= R.Range &&
                        myHero.LSGetSpellDamage(hero, SpellSlot.R) >= hero.Health)
                        R.CastOnUnit(hero, true);
                }
            }
        }

        private static void Drawing_OnDraw(EventArgs args)
        {
            if (myHero.IsDead)
                return;

            var qcheck = getCheckBoxItem(DrawingMenu, "QRange");
            var rcheck = getCheckBoxItem(DrawingMenu, "RRange");
            if (qcheck)
            {
                Render.Circle.DrawCircle(myHero.Position, Q.Range, Color.FromArgb(255, 255, 255, 255), 1);
            }
            if (rcheck)
            {
                Render.Circle.DrawCircle(myHero.Position, R.Range, Color.FromArgb(255, 255, 255, 255), 1);
            }
        }
    }
}