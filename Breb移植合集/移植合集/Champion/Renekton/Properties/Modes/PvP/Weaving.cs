using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using ExorAIO.Utilities;
using LeagueSharp.Common;

namespace ExorAIO.Champions.Renekton
{
    /// <summary>
    ///     The logics class.
    /// </summary>
    internal partial class Logics
    {
        /// <summary>
        ///     Called on do-cast.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The args.</param>
        public static void Weaving(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            /// <summary>
            ///     The W JungleClear Logic.
            /// </summary>
            if (Variables.W.IsReady() &&
                Targets.JungleMinions.Any() &&
                Variables.getCheckBoxItem(Variables.WMenu, "wspell.jgc"))
            {
                Variables.W.Cast();
            }

            if (!args.Target.IsValid<AIHeroClient>() ||
                Bools.IsSpellShielded((AIHeroClient) args.Target))
            {
                return;
            }

            /// <summary>
            ///     The W Weaving Logic.
            /// </summary>
            if (Variables.W.IsReady() &&
                ((AIHeroClient) args.Target).IsValidTarget(Variables.W.Range) &&
                Variables.getCheckBoxItem(Variables.WMenu, "wspell.combo"))
            {
                Variables.W.Cast();
            }
        }
    }
}