using EloBuddy;
using ExorSDK.Utilities;
using LeagueSharp;
using LeagueSharp.SDK.Core.Utils;

namespace ExorSDK.Champions.Caitlyn
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
            if (!(args.Target is AIHeroClient) || Invulnerable.Check(args.Target as AIHeroClient)) {}
        }
    }
}