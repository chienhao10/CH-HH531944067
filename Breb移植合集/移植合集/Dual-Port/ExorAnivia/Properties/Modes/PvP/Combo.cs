using System;
using System.Linq;
using ExorSDK.Utilities;
using LeagueSharp;
using LeagueSharp.SDK;
using LeagueSharp.SDK.Core.Utils;
using EloBuddy;

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
        public static void Combo(EventArgs args)
        {
            if (Bools.HasSheenBuff() ||
                !Targets.Target.LSIsValidTarget() ||
                Invulnerable.Check(Targets.Target))
            {
                return;
            }

            /// <summary>
            ///     The Wall Logic.
            /// </summary>
            if (Vars.W.IsReady() &&
                GameObjects.Player.ManaPercent > 30 &&
                Targets.Target.LSIsValidTarget(Vars.W.Range) &&
                Vars.getCheckBoxItem(Vars.WMenu, "combo") &&
                Vars.getCheckBoxItem(Vars.WhiteListMenu, Targets.Target.ChampionName.ToLower()))
            {
                if (GameObjects.Player.Distance(
                        GameObjects.Player.ServerPosition.LSExtend(
                            Targets.Target.ServerPosition,
                            GameObjects.Player.Distance(Targets.Target) + Targets.Target.BoundingRadius + 20f)) < Vars.W.Range)
                {
                    Vars.W.Cast(
                        GameObjects.Player.ServerPosition.LSExtend(
                            Targets.Target.ServerPosition,
							GameObjects.Player.Distance(Targets.Target) + Targets.Target.BoundingRadius + 20f));
                }
            }

            /// <summary>
            ///     The E Combo Logic.
            /// </summary>
            if (Vars.E.IsReady() &&
				Vars.getCheckBoxItem(Vars.EMenu, "combo"))
			{
                foreach (var target in GameObjects.EnemyHeroes.Where(
					t =>
						t.HasBuff("chilled") &&
						t.LSIsValidTarget(Vars.E.Range)))
				{
					Vars.E.CastOnUnit(target);
				}
			}

            /// <summary>
            ///     The R Combo Logic.
            /// </summary>
            if (Vars.R.IsReady() &&
                Targets.Target.LSIsValidTarget(Vars.R.Range) &&
                GameObjects.Player.Spellbook.GetSpell(SpellSlot.R).ToggleState == 1 &&
                Vars.getCheckBoxItem(Vars.RMenu, "combo"))
            {
                Vars.R.Cast(Targets.Target.ServerPosition);
            }

            /// <summary>
            ///     The Q Combo Logic.
            /// </summary>
            if (Vars.Q.IsReady() &&
                Targets.Target.LSIsValidTarget(Vars.Q.Range) &&
                GameObjects.Player.Spellbook.GetSpell(SpellSlot.Q).ToggleState == 1 &&
                Vars.getCheckBoxItem(Vars.QMenu, "combo"))
            {
                Vars.Q.Cast(Vars.Q.GetPrediction(Targets.Target).UnitPosition);
            }
        }
    }
}