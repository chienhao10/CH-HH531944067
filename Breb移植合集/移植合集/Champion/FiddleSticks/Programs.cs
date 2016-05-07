using System;
using System.Drawing;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using Feedlesticks.Core;
using LeagueSharp.Common;

namespace Feedlesticks
{
    internal class Program
    {
        /// <summary>
        ///     Fiddle (easy)
        /// </summary>
        public static readonly AIHeroClient FiddleStick = ObjectManager.Player;

        /// <summary>
        ///     OnLoad section
        /// </summary>
        /// <param name="args"></param>
        public static void Game_OnGameLoad()
        {
            Menus.Config = MainMenu.AddMenu("Feedlestick", "Feedlestick");

            {
                Spells.Init();
                Menus.Init();
            }

            Spellbook.OnCastSpell += Spellbook_OnCastSpell;
            Obj_AI_Base.OnProcessSpellCast += OnProcessSpellCast;
            Game.OnUpdate += Game_OnGameUpdate;
            Drawing.OnDraw += OnDraw;
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

        /// <summary>
        ///     Drawing Stuff
        /// </summary>
        /// <param name="args"></param>
        private static void OnDraw(EventArgs args)
        {
            if (FiddleStick.IsDead)
            {
                return;
            }
            if (Spells.Q.IsReady() && getCheckBoxItem(Menus.drawMenu, "q.draw"))
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, Spells.Q.Range, Color.White);
            }
            if (Spells.W.IsReady() && getCheckBoxItem(Menus.drawMenu, "w.draw"))
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, Spells.W.Range, Color.DarkSeaGreen);
            }
            if (Spells.E.IsReady() && getCheckBoxItem(Menus.drawMenu, "e.draw"))
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, Spells.E.Range, Color.Gold);
            }
            if (Spells.R.IsReady() && getCheckBoxItem(Menus.drawMenu, "r.draw"))
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, Spells.R.Range, Color.DodgerBlue);
            }
        }

        public static bool IsWActive
        {
            get { return ObjectManager.Player.HasBuff("fiddlebuff"); }
        }

        /// <summary>
        ///     Process spell cast. thats need for last w game time
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private static void OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            Helper.OnProcessSpellCast(sender, args);
        }

        /// <summary>
        ///     W lock
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private static void Spellbook_OnCastSpell(Spellbook sender, SpellbookCastSpellEventArgs args)
        {
            Helper.Spellbook_OnCastSpell(sender, args);
        }

        /// <summary>
        ///     Combo stuff and immobile stuff
        /// </summary>
        /// <param name="args"></param>
        private static void Game_OnGameUpdate(EventArgs args)
        {
            if (FiddleStick.IsDead)
            {
                return;
            }

            if (IsWActive)
            {
                Orbwalker.DisableAttacking = true;
                Orbwalker.DisableMovement = true;
            }
            else
            {
                Orbwalker.DisableAttacking = false;
                Orbwalker.DisableMovement = false;
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo) && !IsWActive)
            {
                Combo();
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass) && !IsWActive)
            {
                Harass();
            }

            if ((Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear) ||
                Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear)) && !IsWActive)
            {
                Jungle();
                WaveClear();
            }

            if (getCheckBoxItem(Menus.qMenu, "auto.q.immobile"))
            {
                foreach (
                    var enemy in
                        HeroManager.Enemies.Where(
                            x =>
                                x.IsValidTarget(Spells.Q.Range) &&
                                getCheckBoxItem(Menus.qMenu, "q.enemy." + x.NetworkId) && Helper.IsEnemyImmobile(x)))
                {
                    Spells.Q.Cast(enemy);
                }
            }
            if (getCheckBoxItem(Menus.qMenu, "auto.q.channeling"))
            {
                foreach (
                    var enemy in
                        HeroManager.Enemies.Where(
                            x =>
                                x.IsValidTarget(Spells.Q.Range) &&
                                getCheckBoxItem(Menus.qMenu, "q.enemy." + x.NetworkId) &&
                                x.IsChannelingImportantSpell()))
                {
                    Spells.Q.Cast(enemy);
                }
            }
            if (getCheckBoxItem(Menus.eMenu, "auto.e.enemy.immobile"))
            {
                foreach (
                    var enemy in
                        HeroManager.Enemies.Where(
                            x =>
                                x.IsValidTarget(Spells.E.Range) &&
                                getCheckBoxItem(Menus.eMenu, "e.enemy." + x.NetworkId) && Helper.IsEnemyImmobile(x)))
                {
                    Spells.E.Cast(enemy);
                }
            }
            if (getCheckBoxItem(Menus.eMenu, "auto.e.enemy.channeling"))
            {
                foreach (
                    var enemy in
                        HeroManager.Enemies.Where(
                            x =>
                                x.IsValidTarget(Spells.E.Range) &&
                                getCheckBoxItem(Menus.eMenu, "e.enemy." + x.NetworkId) &&
                                x.IsChannelingImportantSpell()))
                {
                    Spells.E.Cast(enemy);
                }
            }
        }

        private static void Harass()
        {
            if (ObjectManager.Player.ManaPercent < getSliderItem(Menus.harassMenu, "harass.mana"))
            {
                return;
            }

            if (Spells.Q.IsReady() && getCheckBoxItem(Menus.harassMenu, "q.harass"))
            {
                foreach (
                    var enemy in
                        HeroManager.Enemies.Where(o => o.IsValidTarget(Spells.Q.Range) && !o.IsDead && !o.IsZombie))
                {
                    if (getCheckBoxItem(Menus.qMenu, "q.enemy." + enemy.NetworkId))
                    {
                        Spells.Q.CastOnUnit(enemy);
                    }
                }
            }
            if (Spells.E.IsReady() && getCheckBoxItem(Menus.harassMenu, "e.harass"))
            {
                foreach (
                    var enemy in
                        HeroManager.Enemies.Where(o => o.IsValidTarget(Spells.E.Range) && !o.IsDead && !o.IsZombie))
                {
                    if (getCheckBoxItem(Menus.eMenu, "e.enemy." + enemy.NetworkId))
                    {
                        Spells.E.CastOnUnit(enemy);
                    }
                }
            }
        }

        private static void Jungle()
        {
            if (ObjectManager.Player.ManaPercent < getSliderItem(Menus.jungleMenu, "jungle.mana"))
            {
                return;
            }

            var mob = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, Spells.Q.Range, MinionTypes.All,
                MinionTeam.Neutral, MinionOrderTypes.MaxHealth);
            if (mob.Count > 0)
            {
                if (Spells.Q.IsReady() && getCheckBoxItem(Menus.jungleMenu, "q.jungle"))
                {
                    Spells.Q.CastOnUnit(mob[0]);
                }
                if (Spells.W.IsReady() && getCheckBoxItem(Menus.jungleMenu, "w.jungle"))
                {
                    Spells.W.CastOnUnit(mob[0]);
                }
                if (Spells.E.IsReady() && getCheckBoxItem(Menus.jungleMenu, "e.jungle"))
                {
                    Spells.E.CastOnUnit(mob[0]);
                }
            }
        }

        /// <summary>
        ///     Combo
        /// </summary>
        private static void Combo()
        {
            if (Spells.Q.IsReady() && getCheckBoxItem(Menus.comboMenu, "q.combo"))
            {
                foreach (
                    var enemy in
                        HeroManager.Enemies.Where(o => o.IsValidTarget(Spells.Q.Range) && !o.IsDead && !o.IsZombie))
                {
                    if (getCheckBoxItem(Menus.qMenu, "q.enemy." + enemy.NetworkId))
                    {
                        Spells.Q.CastOnUnit(enemy);
                    }
                }
            }
            if (Spells.W.IsReady() && getCheckBoxItem(Menus.comboMenu, "w.combo"))
            {
                foreach (
                    var enemy in
                        HeroManager.Enemies.Where(o => o.IsValidTarget(Spells.W.Range) && !o.IsDead && !o.IsZombie))
                {
                    if (getCheckBoxItem(Menus.wMenu, "w.enemy." + enemy.NetworkId))
                    {
                        Spells.W.CastOnUnit(enemy);
                    }
                }
            }
            if (Spells.E.IsReady() && getCheckBoxItem(Menus.comboMenu, "e.combo"))
            {
                foreach (var enemy in HeroManager.Enemies.Where(o => o.IsValidTarget(Spells.E.Range) && !o.IsDead && !o.IsZombie))
                {
                    if (getCheckBoxItem(Menus.eMenu, "e.enemy." + enemy.NetworkId) && enemy.CountEnemiesInRange(Spells.E.Range) >= getSliderItem(Menus.eMenu, "e.enemy.count"))
                    {
                        Spells.E.CastOnUnit(enemy);
                    }
                }
            }
        }

        /// <summary>
        ///     WaveClear
        /// </summary>
        private static void WaveClear()
        {
            if (ObjectManager.Player.ManaPercent < getSliderItem(Menus.clearMenu, "clear.mana"))
            {
                return;
            }

            var min = MinionManager.GetMinions(ObjectManager.Player.Position, Spells.Q.Range, MinionTypes.All,
                MinionTeam.Enemy, MinionOrderTypes.MaxHealth);
            if (Spells.E.IsReady() && getCheckBoxItem(Menus.clearMenu, "e.clear"))
            {
                if (min.Count > getSliderItem(Menus.clearMenu, "e.minion.hit.count"))
                {
                    Spells.E.CastOnUnit(min[0]);
                }
            }
            if (Spells.W.IsReady() && getCheckBoxItem(Menus.clearMenu, "w.clear"))
            {
                Spells.W.CastOnUnit(min[0]);
            }
        }
    }
}