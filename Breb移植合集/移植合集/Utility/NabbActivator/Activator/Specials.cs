using System.Linq;
using EloBuddy;
using LeagueSharp.SDK;

namespace NabbActivator
{
    /// <summary>
    ///     The activator class.
    /// </summary>
    internal partial class Activator
    {
        /// <summary>
        ///     Called on do-cast.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The args.</param>
        public static void Specials(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!Vars.getCheckBoxItem(Vars.TypesMenu, "defensives"))
            {
                return;
            }

            if (sender != null &&
                args.Target != null)
            {
                /// <summary>
                ///     The Ohmwrecker logic.
                /// </summary>
                if (Items.CanUseItem(3056) &&
                    sender.LSIsValidTarget(750f))
                {
                    if (args.Target.IsAlly &&
                        sender is Obj_AI_Turret &&
                        args.Target is AIHeroClient)
                    {
                        Items.UseItem(3056, sender);
                    }
                }
            }
        }
    }
}