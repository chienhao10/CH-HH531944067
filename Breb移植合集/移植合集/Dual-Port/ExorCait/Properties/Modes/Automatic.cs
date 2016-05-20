using LeagueSharp;
using LeagueSharp.Common;

namespace ExorAIO.Champions.Caitlyn
{
    using System;
    using System.Linq;
    using ExorAIO.Utilities;
    using EloBuddy;
    using EloBuddy.SDK;    /// <summary>
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
            if (ObjectManager.Player.LSIsRecalling())
            {
                return;
            }

            /// <summary>
            ///     The Automatic Q Logic.
            /// </summary>
            if (Variables.Q.IsReady() && Menus.getCheckBoxItem(Variables.QMenu, "qspell.auto"))
            {
                foreach (var target in GameObjects.EnemyHeroes.Where(t => !Bools.IsSpellShielded(t) && t.IsValidTarget(Variables.Q.Range)))
                {
                    if (target.HasBuff("caitlynyordletrapdebuff") || target.HasBuff("caitlynyordletrapinternal"))
                    {
                        Variables.Q.Cast(target);
                    }
                }
            }

            /// <summary>
            ///     The Automatic W Logic. 
            /// </summary>
            if (Variables.W.IsReady() && Menus.getCheckBoxItem(Variables.WMenu, "wspell.auto"))
            {
                foreach (AIHeroClient target in HeroManager.Enemies.Where(t => Bools.IsImmobile(t) && !Bools.IsSpellShielded(t) && t.LSIsValidTarget(Variables.W.Range) && t.IsEnemy))
                {
                    if (!ObjectManager.Get<Obj_AI_Minion>().Any(m => m.LSDistance(target.Position) < 100f && m.CharData.BaseSkinName.Contains("Cupcake")))
                    {
                        Variables.W.Cast(target.Position);
                    }
                }
            }
        }
    }
}
