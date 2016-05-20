#region

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using LeagueSharp.Common;
using Spell = LeagueSharp.Common.Spell;

#endregion

namespace XinZhao
{
    internal class Program
    {
        public static string ChampionName = "XinZhao";

        public static Spell Q, W, E, R;

        public static List<Spell> SpellList = new List<Spell>();

        public static Menu Config;

        public static Menu comboMenu, mLane, mJungle, drawMenu, miscMenu;

        public static AIHeroClient Player
        {
            get { return ObjectManager.Player; }
        }

        private static int GetHitsR
        {
            get
            {
                {
                    return Player.CountEnemiesInRange(R.Range);
                }
            }
        }

        public static void Game_OnGameLoad()
        {
            if (Player.CharData.BaseSkinName != ChampionName)
                return;

            Q = new Spell(SpellSlot.Q);
            W = new Spell(SpellSlot.W);
            E = new Spell(SpellSlot.E, 600);
            R = new Spell(SpellSlot.R, 480);

            Config = MainMenu.AddMenu("xQx | " + ChampionName, ChampionName);

            /* [ Combo ] */
            comboMenu = Config.AddSubMenu("Combo", "Combo");
            comboMenu.Add("useECombo", new CheckBox("Use E"));
            comboMenu.Add("EMinRange", new Slider("Min. E Range", 300, 200, 500));
            comboMenu.Add("ComboUseR", new CheckBox("Use R"));
            comboMenu.Add("ComboUseRS", new Slider("Min. Enemy Count:", 2, 1, 5));

            // Lane Mode
            mLane = Config.AddSubMenu("Lane Mode", "LaneMode");
            mLane.Add("Lane.UseQ", new CheckBox("Use Q", false));
            mLane.Add("Lane.UseW", new CheckBox("Use W", false));
            mLane.Add("Lane.UseE", new CheckBox("Use E", false));
            mLane.Add("Lane.Mana", new Slider("Min. Mana Percent: ", 50));

            // Jungle 
            mJungle = Config.AddSubMenu("Jungle Mode", "JungleMode");
            mJungle.Add("Jungle.UseQ", new CheckBox("Use Q", false));
            mJungle.Add("Jungle.UseW", new CheckBox("Use W", false));
            mJungle.Add("Jungle.UseE", new CheckBox("Use E", false));
            mJungle.Add("Jungle.Mana", new Slider("Min. Mana Percent: ", 50));

            // Drawings
            drawMenu = Config.AddSubMenu("Drawings", "Drawings");
            drawMenu.Add("DrawERange", new CheckBox("E range")); //.SetValue(new Circle(false, Color.PowderBlue)));
            drawMenu.Add("DrawEMinRange", new CheckBox("E min. range")); //.SetValue(new Circle(false, Color.Aqua)));
            drawMenu.Add("DrawRRange", new CheckBox("R range")); //.SetValue(new Circle(false, Color.PowderBlue)));
            drawMenu.Add("DrawThrown", new CheckBox("Can be thrown enemy"));
                //.SetValue(new Circle(false, Color.PowderBlue)));

            // Misc
            miscMenu = Config.AddSubMenu("Misc", "Misc");
            miscMenu.Add("InterruptSpells", new CheckBox("Interrupt spells using R"));
            miscMenu.Add("BlockR", new CheckBox("Block R if it won't hit", false));

            Game.OnUpdate += Game_OnUpdate;
            Orbwalker.OnPreAttack += OrbwalkingBeforeAttack;
            Interrupter2.OnInterruptableTarget += Interrupter2_OnInterruptableTarget;
            Spellbook.OnCastSpell += Spellbook_OnCastSpell;
            Drawing.OnDraw += Drawing_OnDraw;
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

        public static int getBoxItem(Menu m, string item)
        {
            return m[item].Cast<ComboBox>().CurrentValue;
        }

        private static void OrbwalkingBeforeAttack(AttackableUnit target, Orbwalker.PreAttackArgs args)
        {
            if (!(args.Target is AIHeroClient) || !Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                return;
            }

            if (Q.IsReady())
            {
                Q.Cast();
            }

            if (W.IsReady())
            {
                W.Cast();
            }
        }

        private static void Spellbook_OnCastSpell(Spellbook sender, SpellbookCastSpellEventArgs args)
        {
            if (!getCheckBoxItem(miscMenu, "BlockR"))
            {
                return;
            }

            if (args.Slot == SpellSlot.R && GetHitsR == 0)
            {
                args.Process = false;
            }
        }

        private static void Interrupter2_OnInterruptableTarget(AIHeroClient unit,
            Interrupter2.InterruptableTargetEventArgs args)
        {
            if (!getCheckBoxItem(miscMenu, "InterruptSpells"))
            {
                return;
            }

            if (unit.IsValidTarget(R.Range) && args.DangerLevel >= Interrupter2.DangerLevel.Medium &&
                !unit.HasBuff("xenzhaointimidate"))
            {
                R.Cast();
            }
        }

        private static void Game_OnUpdate(EventArgs args)
        {
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                Combo();
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear) ||
                Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear))
            {
                var existsMana = Player.MaxMana/100*getSliderItem(mLane, "Lane.Mana");
                if (Player.Mana >= existsMana)
                {
                    LaneClear();
                    JungleFarm();
                }
            }
        }

        private static void Drawing_OnDraw(EventArgs args)
        {
            var drawERange = getCheckBoxItem(drawMenu, "DrawERange");
            if (drawERange)
            {
                Render.Circle.DrawCircle(Player.Position, E.Range, Color.PowderBlue, 1);
            }

            var drawRRange = getCheckBoxItem(drawMenu, "DrawRRange");
            if (drawRRange)
            {
                Render.Circle.DrawCircle(Player.Position, R.Range, Color.PowderBlue, 1);
            }

            var drawEMinRange = getCheckBoxItem(drawMenu, "DrawEMinRange");
            if (drawEMinRange)
            {
                var eMinRange = getSliderItem(comboMenu, "EMinRange");
                Render.Circle.DrawCircle(Player.Position, eMinRange, Color.Aqua, 1);
            }

            /* [ Draw Can Be Thrown Enemy ] */
            var drawThrownEnemy = getCheckBoxItem(drawMenu, "DrawThrown");
            if (drawThrownEnemy)
            {
                foreach (
                    var enemy in
                        from enemy in
                            ObjectManager.Get<AIHeroClient>()
                                .Where(
                                    enemy =>
                                        !enemy.IsDead && enemy.IsEnemy && Player.LSDistance(enemy) < R.Range &&
                                        R.IsReady())
                        from buff in enemy.Buffs.Where(buff => !buff.Name.Contains("xenzhaointimidate"))
                        select enemy)
                {
                    Render.Circle.DrawCircle(enemy.Position, 90f, Color.PowderBlue, 1);
                }
            }
        }

        public static void Combo()
        {
            var t = TargetSelector.GetTarget(E.Range, DamageType.Magical);

            if (!t.IsValidTarget())
            {
                return;
            }

            if (t.IsValidTarget(E.Range) && E.IsReady() && getCheckBoxItem(comboMenu, "useECombo"))
            {
                var eMinRange = getSliderItem(comboMenu, "EMinRange");
                if (ObjectManager.Player.LSDistance(t) >= eMinRange)
                {
                    E.CastOnUnit(t);
                }

                if (E.GetDamage(t) > t.Health)
                {
                    E.CastOnUnit(t);
                }
            }

            if (R.IsReady() && getCheckBoxItem(comboMenu, "ComboUseR") &&
                GetHitsR >= getSliderItem(comboMenu, "ComboUseRS"))
            {
                R.Cast();
            }
        }

        private static void LaneClear()
        {
            var useQ = getCheckBoxItem(mLane, "Lane.UseQ");
            var useW = getCheckBoxItem(mLane, "Lane.UseW");
            var useE = getCheckBoxItem(mLane, "Lane.UseE");

            var allMinions = MinionManager.GetMinions(
                Player.ServerPosition,
                E.Range,
                MinionTypes.All,
                MinionTeam.NotAlly);

            if (useQ || useW)
            {
                var minionsQ = MinionManager.GetMinions(Player.ServerPosition, 400);
                foreach (var vMinion in
                    from vMinion in minionsQ where vMinion.IsEnemy select vMinion)
                {
                    if (useQ && Q.IsReady()) Q.Cast();
                    if (useW && W.IsReady()) W.Cast();
                }
            }

            if (useE && E.IsReady())
            {
                var locE = E.GetCircularFarmLocation(allMinions);
                if (allMinions.Count == allMinions.Count(m => Player.LSDistance(m) < E.Range) && locE.MinionsHit >= 2
                    && locE.Position.IsValid()) E.Cast(locE.Position);
            }
        }

        private static void JungleFarm()
        {
            var useQ = getCheckBoxItem(mJungle, "Jungle.UseQ");
            var useW = getCheckBoxItem(mJungle, "Jungle.UseW");
            var useE = getCheckBoxItem(mJungle, "Jungle.UseE");

            var mobs = MinionManager.GetMinions(
                ObjectManager.Player.ServerPosition, E.Range, MinionTypes.All, MinionTeam.Neutral,
                MinionOrderTypes.MaxHealth);

            if (mobs.Count <= 0)
                return;

            var mob = mobs[0];
            if (useQ && Q.IsReady() && mobs.Count >= 1)
                Q.Cast();

            if (useW && W.IsReady() && mobs.Count >= 1)
                W.Cast();

            if (useE && E.IsReady() && mobs.Count >= 1)
                E.CastOnUnit(mob);
        }
    }
}