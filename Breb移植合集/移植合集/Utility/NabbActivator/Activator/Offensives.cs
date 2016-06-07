using System;
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
        ///     Called when the game updates itself.
        /// </summary>
        /// <param name="args">The <see cref="EventArgs" /> instance containing the event data.</param>
        public static void Offensives(EventArgs args)
        {
            if (!Targets.Target.LSIsValidTarget() ||
                !Vars.getCheckBoxItem(Vars.TypesMenu, "offensives") ||
                !Vars.getKeyBindItem(Vars.KeysMenu, "combo"))
            {
                return;
            }

            /// <summary>
            ///     The Bilgewater Cutlass Logic.
            /// </summary>
            if (Items.CanUseItem(3144) &&
                Targets.Target.LSIsValidTarget(550f))
            {
                Items.UseItem(3144, Targets.Target);
            }

            /// <summary>
            ///     The Blade of the Ruined King Logic.
            /// </summary>
            if (Items.CanUseItem(3153) &&
                Targets.Target.LSIsValidTarget(550f) &&
                GameObjects.Player.HealthPercent <= 90)
            {
                Items.UseItem(3153, Targets.Target);
            }

            /// <summary>
            ///     The Entropy Logic.
            /// </summary>     
            if (Items.CanUseItem(3184) &&
                GameObjects.Player.Spellbook.IsAutoAttacking)
            {
                Items.UseItem(3184);
            }

            /// <summary>
            ///     The Frost Queen's Claim Logic.
            /// </summary>
            if (Items.CanUseItem(3092))
            {
                if (GameObjects.EnemyHeroes.Count(
                    t =>
                        t.LSIsValidTarget(4000f) &&
                        t.CountEnemyHeroesInRange(1500f) <=
                            GameObjects.Player.CountAllyHeroesInRange(1500f) + t.CountAllyHeroesInRange(1500f) - 1) >= 1)
                {
                    Items.UseItem(3092);
                }
            }

            /// <summary>
            ///     The Hextech Gunblade Logic.
            /// </summary>
            if (Items.CanUseItem(3146) &&
                Targets.Target.LSIsValidTarget(700f))
            {
                Items.UseItem(3146, Targets.Target);
            }

            /// <summary>
            ///     The Youmuu's Ghostblade Logic.
            /// </summary>
            if (Items.CanUseItem(3142))
            {
                if (GameObjects.Player.Spellbook.IsAutoAttacking ||
                    GameObjects.Player.IsCastingInterruptableSpell())
                {
                    Items.UseItem(3142);
                }
            }

            /// <summary>
            ///     The Hextech GLP-800 Logic.
            /// </summary>
            if (Items.CanUseItem(3030) &&
                Targets.Target.LSIsValidTarget(800f))
            {
                Items.UseItem(3030, Movement.GetPrediction(Targets.Target, 0.5f).UnitPosition);
            }
            
            /// <summary>
            ///     The Hextech Protobelt Logic.
            /// </summary>
            if (Items.CanUseItem(3152) &&
                Targets.Target.LSIsValidTarget(
                    GameObjects.Player.Distance(GameObjects.Player.ServerPosition.LSExtend(Game.CursorPos, 850f))))
            {
                Items.UseItem(3152, GameObjects.Player.ServerPosition.LSExtend(Game.CursorPos, 850f));
            }
        }
    }
}