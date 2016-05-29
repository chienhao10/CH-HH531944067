using System;
using System.Linq;
using ExorSDK.Utilities;
using LeagueSharp;
using LeagueSharp.SDK;
using EloBuddy;
using LeagueSharp.SDK.Core.Utils;
using EloBuddy.SDK;

namespace ExorSDK.Champions.Anivia
{
    /// <summary>
    ///     The logics class.
    /// </summary>
    internal partial class Logics
    {
        /// <summary>
        ///     Called when the game updates itself.
        /// </summary>
        /// <param name="args">The <see cref="EventArgs" /> instance containing the event data.</param>
        public static void Automatic(EventArgs args)
        {
            if (GameObjects.Player.LSIsRecalling())
            {
                return;
            }

            /// <summary>
            ///     The R Stacking Manager.
            /// </summary>
            if (GameObjects.Player.InFountain() &&
                Bools.HasTear(GameObjects.Player) &&
                GameObjects.Player.Spellbook.GetSpell(SpellSlot.R).ToggleState == 1 &&
                Vars.getCheckBoxItem(Vars.MiscMenu, "tear"))
            {
                Vars.R.Cast(Game.CursorPos);
            }

            /// <summary>
            ///     The Automatic Q Logic.
            /// </summary>
            if (Vars.Q.IsReady() &&
                GameObjects.Player.Spellbook.GetSpell(SpellSlot.Q).ToggleState == 1 &&
                Vars.getCheckBoxItem(Vars.QMenu, "logical"))
            {
                foreach (var target in GameObjects.EnemyHeroes.Where(
                    t =>
                        Bools.IsImmobile(t) &&
                        !Invulnerable.Check(t) &&
                        t.LSIsValidTarget(Vars.Q.Range)))
                {
                    Vars.Q.Cast(Vars.Q.GetPrediction(target).UnitPosition);
                }
            }

            /// <summary>
            ///     The Automatic W Logic.
            /// </summary>
            if (Vars.W.IsReady() &&
                Vars.getCheckBoxItem(Vars.WMenu, "logical"))
            {
                foreach (var target in GameObjects.EnemyHeroes.Where(
                    t =>
                        Bools.IsImmobile(t) &&
                        !Invulnerable.Check(t) &&
                        t.LSIsValidTarget(Vars.W.Range)))
                {
                    Vars.W.Cast(
                        GameObjects.Player.ServerPosition.LSExtend(
                            target.ServerPosition, GameObjects.Player.Distance(target)+20f));
                }
            }

            /// <summary>
            ///     The Q Missile Manager.
            /// </summary>
            if (Vars.Q.IsReady() &&
                Anivia.QMissile != null &&
                GameObjects.Player.Spellbook.GetSpell(SpellSlot.Q).ToggleState != 1)
            {

                if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear) || Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear))
                {
                    /// <summary>
                    ///     The Q Clear Logic.
                    /// </summary>
                    if (Anivia.QMissile.Position.CountEnemyHeroesInRange(Vars.Q.Width * 2 - 5f) > 0)
                    {
                        Vars.Q.Cast();
                    }

                    if (Vars.getSliderItem(Vars.QMenu, "clear") == 101)
                    {
                        return;
                    }

                    if (Targets.QMinions.Count() >= 2)
                    {
                        Vars.Q.Cast();
                    }
                }
                else
                {
                    if (!Vars.getCheckBoxItem(Vars.QMenu, "combo"))
                    {
                        return;
                    }

                    if (Anivia.QMissile.Position.CountEnemyHeroesInRange(Vars.Q.Width * 2 - 5f) > 0)
                    {
                        Vars.Q.Cast();
                    }
                }
            }

            /// <summary>
            ///     The R Missile Manager.
            /// </summary>
            if (Vars.R.IsReady() &&
                Anivia.RMissile != null &&
                !GameObjects.Player.InFountain() &&
                GameObjects.Player.Spellbook.GetSpell(SpellSlot.R).ToggleState != 1)
            {

                if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear) || Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear))
                {
                    /// <summary>
                    ///     The R Clear Logic.
                    /// </summary>
                    if (Vars.getSliderItem(Vars.RMenu, "clear") == 101)
                    {
                        return;
                    }

                    if (!Targets.RMinions.Any() ||
                        GameObjects.Player.ManaPercent <
                            ManaManager.GetNeededMana(Vars.R.Slot, Vars.getSliderItem(Vars.RMenu, "clear")))
                    {
                        Vars.R.Cast();
                    }
                }
                else
                {
                    /// <summary>
                    ///     The Default R Logic.
                    /// </summary>
                    if (!Vars.getCheckBoxItem(Vars.RMenu, "combo"))
                    {
                        return;
                    }

                    if (Anivia.RMissile.Position.CountEnemyHeroesInRange(Vars.R.Width + 250f) < 1)
                    {
                        Vars.R.Cast();
                    }
                }
            }
        }
    }
}