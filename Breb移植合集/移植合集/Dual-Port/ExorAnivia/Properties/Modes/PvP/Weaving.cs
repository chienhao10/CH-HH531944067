using EloBuddy;
using LeagueSharp.Common;

namespace ExorAIO.Champions.Anivia
{
    using System;
    using ExorAIO.Utilities;

    /// <summary>
    ///     The logics class.
    /// </summary>
    partial class Logics
    {
        /// <summary>
        ///     Called on do-cast.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The args.</param>
        public static void Weaving(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!args.Target.IsValid<AIHeroClient>() ||
                Bools.IsSpellShielded((AIHeroClient)args.Target))
            {
                return;
            }

        }
    }
}
