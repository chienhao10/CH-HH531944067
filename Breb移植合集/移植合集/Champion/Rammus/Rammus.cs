using System;
using System.Drawing;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using LeagueSharp.Common;
using PortAIO.Utility.BrianSharp;
using Spell = LeagueSharp.Common.Spell;

namespace BrianSharp.Plugin
{
    internal class Rammus : Helper
    {
        public static Spell Q, W, E, R;

        public static Menu config, comboMenu, clearMenu, fleeMenu, antiGapMenu, interruptMenu, drawMenu;

        public Rammus()
        {
            Q = new Spell(SpellSlot.Q, 200, DamageType.Magical);
            W = new Spell(SpellSlot.W);
            E = new Spell(SpellSlot.E, 325);
            R = new Spell(SpellSlot.R, 375, DamageType.Magical);

            config = MainMenu.AddMenu("Rammus", "Rammus");

            comboMenu = config.AddSubMenu("Combo", "Combo");
            comboMenu.Add("Q", new CheckBox("Use Q"));
            comboMenu.Add("W", new CheckBox("Use W"));
            comboMenu.Add("E", new CheckBox("Use E"));
            comboMenu.Add("EW", new CheckBox("-> Only Have W"));
            comboMenu.Add("R", new CheckBox("Use R"));
            comboMenu.Add("RMode", new ComboBox("-> Mode", 0, "Always", "# Enemy"));
            comboMenu.Add("RCountA", new Slider("--> If Enemy >=", 2, 1, 5));

            clearMenu = config.AddSubMenu("Clear", "Clear");
            clearMenu.Add("Q", new CheckBox("Use Q"));
            clearMenu.Add("W", new CheckBox("Use W"));
            clearMenu.Add("E", new CheckBox("Use E"));
            clearMenu.Add("EHpA", new Slider("-> If Hp >=", 50));
            clearMenu.Add("EW", new CheckBox("-> Only Have W"));

            fleeMenu = config.AddSubMenu("Flee", "Flee");
            fleeMenu.Add("Q", new CheckBox("Use Q"));

            antiGapMenu = config.AddSubMenu("Anti Gap Closer", "AntiGap");
            antiGapMenu.Add("Q", new CheckBox("Use Q"));
            foreach (
                var spell in
                    AntiGapcloser.Spells.Where(i => HeroManager.Enemies.Any(a => i.ChampionName == a.ChampionName)))
            {
                antiGapMenu.Add(spell.ChampionName + "_" + spell.Slot,
                    new CheckBox("-> Skill " + spell.Slot + " Of " + spell.ChampionName));
            }

            interruptMenu = config.AddSubMenu("Interrupt", "Interrupt");
            interruptMenu.Add("E", new CheckBox("Use E"));
            foreach (
                var spell in
                    Interrupter.Spells.Where(i => HeroManager.Enemies.Any(a => i.ChampionName == a.ChampionName)))
            {
                interruptMenu.Add(spell.ChampionName + "_" + spell.Slot,
                    new CheckBox("-> Skill " + spell.Slot + " Of " + spell.ChampionName));
            }

            drawMenu = config.AddSubMenu("Draw", "Draw");
            drawMenu.Add("E", new CheckBox("E Range", false));
            drawMenu.Add("R", new CheckBox("R Range", false));

            Game.OnUpdate += OnUpdate;
            Drawing.OnDraw += OnDraw;
            AntiGapcloser.OnEnemyGapcloser += OnEnemyGapcloser;
            Interrupter.OnPossibleToInterrupt += OnPossibleToInterrupt;
        }

        public static AIHeroClient Player
        {
            get { return ObjectManager.Player; }
        }

        private static bool HaveQ
        {
            get { return Player.HasBuff("PowerBall"); }
        }

        private static bool HaveW
        {
            get { return Player.HasBuff("DefensiveBallCurl"); }
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

        private static void OnUpdate(EventArgs args)
        {
            if (Player.IsDead || MenuGUI.IsChatOpen || Player.IsRecalling())
            {
                return;
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                Fight();
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear) ||
                Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear))
            {
                Clear();
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Flee))
            {
                if (getCheckBoxItem(fleeMenu, "Q") && !HaveQ && Q.Cast())
                {
                }
            }
        }

        private static void OnDraw(EventArgs args)
        {
            if (Player.IsDead)
            {
                return;
            }
            if (getCheckBoxItem(drawMenu, "E") && E.Level > 0)
            {
                Render.Circle.DrawCircle(Player.Position, E.Range, E.IsReady() ? Color.Green : Color.Red);
            }
            if (getCheckBoxItem(drawMenu, "R") && R.Level > 0)
            {
                Render.Circle.DrawCircle(Player.Position, R.Range, R.IsReady() ? Color.Green : Color.Red);
            }
        }

        private static void OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (Player.IsDead || !getCheckBoxItem(antiGapMenu, "Q") ||
                !getCheckBoxItem(antiGapMenu, gapcloser.Sender.ChampionName + "_" + gapcloser.Slot) || !Q.IsReady())
            {
                return;
            }
            if (!HaveQ)
            {
                Q.Cast();
            }
            EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, gapcloser.Sender.ServerPosition);
        }

        private static void OnPossibleToInterrupt(AIHeroClient unit, InterruptableSpell spell)
        {
            if (Player.IsDead || !getCheckBoxItem(interruptMenu, "E") ||
                !getCheckBoxItem(interruptMenu, unit.ChampionName + "_" + spell.Slot) || !E.CanCast(unit) || HaveQ)
            {
                return;
            }
            E.CastOnUnit(unit);
        }

        private static void Fight()
        {
            if (getCheckBoxItem(comboMenu, "R") && R.IsReady())
            {
                switch (getBoxItem(comboMenu, "RMode"))
                {
                    case 0:
                        if (R.GetTarget() != null && R.Cast())
                        {
                            return;
                        }
                        break;
                    case 1:
                        if (Player.CountEnemiesInRange(R.Range) >= getSliderItem(comboMenu, "RCountA") &&
                            R.Cast())
                        {
                            return;
                        }
                        break;
                }
            }
            if (HaveQ)
            {
                return;
            }
            if (getCheckBoxItem(comboMenu, "Q") && Q.IsReady() && Q.GetTarget(600) != null &&
                ((getCheckBoxItem(comboMenu, "E") && E.IsReady() && E.GetTarget() == null) || !HaveW) && Q.Cast())
            {
                return;
            }
            if (getCheckBoxItem(comboMenu, "E") && (!getCheckBoxItem(comboMenu, "EW") || HaveW) &&
                E.CastOnBestTarget().IsCasted())
            {
                return;
            }
            if (getCheckBoxItem(comboMenu, "W") && Q.GetTarget(100) != null)
            {
                W.Cast();
            }
        }

        private static void Clear()
        {
            var minionObj = GetMinions(600, MinionTypes.All, MinionTeam.NotAlly, MinionOrderTypes.MaxHealth);
            if (!minionObj.Any() || HaveQ)
            {
                return;
            }
            if (getCheckBoxItem(clearMenu, "Q") && Q.IsReady() && !HaveW &&
                (minionObj.Count(i => Q.IsInRange(i)) > 2 || minionObj.Any(i => i.MaxHealth >= 1200 && Q.IsInRange(i)) ||
                 !minionObj.Any(i => Player.IsInAutoAttackRange(i))) && Q.Cast())
            {
                return;
            }
            if (getCheckBoxItem(clearMenu, "E") && E.IsReady() &&
                Player.HealthPercent >= getSliderItem(clearMenu, "EHpA") &&
                (!getCheckBoxItem(clearMenu, "EW") || HaveW))
            {
                var obj = minionObj.FirstOrDefault(i => E.IsInRange(i) && i.Team == GameObjectTeam.Neutral);
                if (obj != null && E.CastOnUnit(obj))
                {
                    return;
                }
            }
            if (getCheckBoxItem(clearMenu, "W") && W.IsReady() &&
                (minionObj.Count(i => Player.IsInAutoAttackRange(i)) > 2 ||
                 minionObj.Any(i => i.MaxHealth >= 1200 && Player.IsInAutoAttackRange(i))))
            {
                W.Cast();
            }
        }
    }
}