using LeagueSharp;
using LeagueSharp.Common;

namespace ExorAIO.Champions.Anivia
{
    using System;
    using System.Linq;
    using ExorAIO.Utilities;
    using EloBuddy;
    using EloBuddy.SDK;

    /// <summary>
    ///     The logics class.
    /// </summary>
    partial class Logics
    {
        /// <summary>
        ///     Called when the game updates itself.
        /// </summary>
        /// <param name="args">The <see cref="EventArgs"/> instance containing the event data.</param>
        public static void Automatic(EventArgs args)
        {
            if (ObjectManager.Player.IsRecalling())
            {
                return;
            }

            /// <summary>
            ///     The R Stacking Manager.
            /// </summary>
            if (ObjectManager.Player.InFountain() &&
                Bools.HasTear(ObjectManager.Player) &&
                ObjectManager.Player.Spellbook.GetSpell(SpellSlot.R).ToggleState == 1 &&
               Variables.getCheckBoxItem(Variables.MiscMenu, "misc.tear"))
            {
                Variables.R.Cast(Game.CursorPos);
            }

            /// <summary>
            ///     The Automatic Q Logic.
            /// </summary>
            if (Variables.Q.IsReady() &&
                ObjectManager.Player.Spellbook.GetSpell(SpellSlot.Q).ToggleState == 1 &&
                Variables.getCheckBoxItem(Variables.QMenu, "qspell.auto"))
            {
                foreach (AIHeroClient target in
                    HeroManager.Enemies.Where(
                    t =>
                        Bools.IsImmobile(t) &&
                        !Bools.IsSpellShielded(t) &&
                        t.IsValidTarget(Variables.Q.Range)))
                {
                    Variables.Q.Cast(Variables.Q.GetPrediction(target).CastPosition);
                }
            }

            /// <summary>
            ///     The Q Missile Manager.
            /// </summary>
            if (Variables.Q.IsReady() &&
                Anivia.QMissile != null &&
                ObjectManager.Player.Spellbook.GetSpell(SpellSlot.Q).ToggleState != 1)
            {
                switch (Orbwalker.ActiveModesFlags)
                {
                    /// <summary>
                    ///     The Q Clear Logic.
                    /// </summary>
                    case Orbwalker.ActiveModes.LaneClear:

                        if (Targets.QMinions.Count() >= 3)
                        {
                            Variables.Q.Cast();
                        }
                        else if (Anivia.QMissile.Position.CountEnemiesInRange(100f) > 0)
                        {
                            Variables.Q.Cast();
                        }
                        break;

                    /// <summary>
                    ///     The Default Q Logic.
                    /// </summary>
                    default:
                        if (Anivia.QMissile.Position.CountEnemiesInRange(100f) > 0)
                        {
                            Variables.Q.Cast();
                        }
                        break;
                }
            }

            /// <summary>
            ///     The R Missile Manager.
            /// </summary>
            if (Variables.R.IsReady() &&
                Anivia.RMissile != null &&
                !ObjectManager.Player.InFountain() &&
                ObjectManager.Player.Spellbook.GetSpell(SpellSlot.R).ToggleState != 1)
            {
                switch (Orbwalker.ActiveModesFlags)
                {
                    /// <summary>
                    ///     The R Clear Logic.
                    /// </summary>
                    case Orbwalker.ActiveModes.LaneClear:
                        if (Targets.RMinions.Count() < 3)
                        {
                            Variables.R.Cast();
                        }
                        break;

                    /// <summary>
                    ///     The Default R Logic.
                    /// </summary>
                    default:
                        if (Anivia.RMissile.Position.CountEnemiesInRange(Variables.R.Width) < 1)
                        {
                            Variables.R.Cast();
                        }
                        break;
                }
            }
        }
    }
}
