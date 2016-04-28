using EloBuddy;
using EloBuddy.SDK;
using ExorAIO.Utilities;
using LeagueSharp.Common;

namespace ExorAIO.Champions.Olaf
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
            if (!args.Target.IsValid<AIHeroClient>() ||
                Bools.IsSpellShielded((AIHeroClient) args.Target))
            {
                return;
            }

            /// <summary>
            ///     The E Weaving Logic.
            /// </summary>
            if (Variables.E.IsReady() &&
                ((AIHeroClient) args.Target).IsValidTarget(Variables.E.Range) &&
                Variables.getCheckBoxItem(Variables.EMenu, "espell.combo"))
            {
                Variables.E.CastOnUnit((AIHeroClient) args.Target);
            }
        }
    }
}