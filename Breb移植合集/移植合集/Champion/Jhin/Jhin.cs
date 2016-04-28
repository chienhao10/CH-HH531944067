using System;
using System.Drawing;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using Jhin___The_Virtuoso.Extensions;
using Jhin___The_Virtuoso.Modes;
using LeagueSharp.Common;

namespace Jhin___The_Virtuoso
{
    public class Jhin
    {
        /// <summary>
        ///     Jhin On Load Event
        /// </summary>
        public static void JhinOnLoad()
        {
            Spells.Initialize();
            Menus.Initialize();

            Game.OnUpdate += JhinOnUpdate;
            Drawing.OnDraw += JhinOnDraw;
        }

        /// <summary>
        ///     Jhin's On Update Event
        /// </summary>
        /// <param name="args">args</param>
        private static void JhinOnUpdate(EventArgs args)
        {
            #region Orbwalker & Modes 

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                Combo.ExecuteCombo();
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear) ||
                Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear))
            {
                Jungle.ExecuteJungle();
                Clear.ExecuteClear();
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass))
            {
                Mixed.ExecuteHarass();
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.None))
            {
                None.ImmobileExecute();
                None.KillSteal();
                None.TeleportE();
                Ultimate.ComboUltimate();
            }

            #endregion

            #region Check Ultimate

            if (ObjectManager.Player.IsActive(Spells.R))
            {
                Orbwalker.DisableMovement = true;
                Orbwalker.DisableAttacking = true;
            }
            else
            {
                Orbwalker.DisableMovement = false;
                Orbwalker.DisableAttacking = false;
            }

            #endregion
        }

        private static void JhinOnDraw(EventArgs args)
        {
            if (Menus.getCheckBoxItem(Menus.drawMenu, "q.draw") && Spells.Q.IsReady())
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, Spells.Q.Range, Color.White);
            }
            if (Menus.getCheckBoxItem(Menus.drawMenu, "w.draw") && Spells.W.IsReady())
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, Spells.W.Range, Color.Gold);
            }
            if (Menus.getCheckBoxItem(Menus.drawMenu, "e.draw") && Spells.E.IsReady())
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, Spells.E.Range, Color.DodgerBlue);
            }
            if (Menus.getCheckBoxItem(Menus.drawMenu, "r.draw") && Spells.R.IsReady())
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, Spells.R.Range, Color.GreenYellow);
            }
            if (Menus.getCheckBoxItem(Menus.drawMenu, "aa.indicator"))
            {
                foreach (
                    var enemy in
                        HeroManager.Enemies.Where(
                            x => x.IsValidTarget(1500) && x.IsValid && x.IsVisible && !x.IsDead && !x.IsZombie))
                {
                    Drawing.DrawText(enemy.HPBarPosition.X, enemy.HPBarPosition.Y, Color.Gold,
                        string.Format("{0} Basic Attack = Kill", Provider.BasicAttackIndicator(enemy)));
                }
            }
            if (Menus.getCheckBoxItem(Menus.drawMenu, "sniper.text") && ObjectManager.Player.IsActive(Spells.R))
            {
                foreach (var enemy in HeroManager.Enemies.Where(x => x.IsValidTarget(Spells.R.Range)))
                {
                    var damage = Spells.R.GetDamage(enemy)*4;
                    if (enemy.Health <= damage)
                    {
                        Render.Circle.DrawCircle(enemy.Position, 100, Color.Gold, 10);
                    }
                }
            }
        }
    }
}