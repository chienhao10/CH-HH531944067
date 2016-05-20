using LeagueSharp;
using LeagueSharp.Common;

namespace ExorAIO.Champions.Caitlyn
{
    using System;
    using System.Linq;
    using ExorAIO.Utilities;
    using EloBuddy;
    using EloBuddy.SDK;    /// <summary>
                           ///     The champion class.
                           /// </summary>
    class Caitlyn
    {
        /// <summary>
        ///     Loads Caitlyn.
        /// </summary>
        public static void OnLoad()
        {
            /// <summary>
            ///     Initializes the menus.
            /// </summary>
            Menus.Initialize();

            /// <summary>
            ///     Initializes the methods.
            /// </summary>
            Methods.Initialize();

            /// <summary>
            ///     Initializes the drawings.
            /// </summary>
            Drawings.Initialize();
        }

        /// <summary>
        ///     Fired when the game is updated.
        /// </summary>
        /// <param name="args">The <see cref="EventArgs" /> instance containing the event data.</param>
        public static void OnUpdate(EventArgs args)
        {
            if (ObjectManager.Player.IsDead)
            {
                return;
            }

            /// <summary>
            ///     Updates the spells.
            /// </summary>
            Spells.Initialize();

            /// <summary>
            ///     Initializes the Automatic actions.
            /// </summary>
            Logics.Automatic(args);

            /// <summary>
            ///     Initializes the Killsteal events.
            /// </summary>
            Logics.Killsteal(args);

            if (Orbwalker.IsAutoAttacking)
            {
                return;
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                Logics.Combo(args);
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass))
            {
                Logics.Harass(args);
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear) || Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear))
            {
                Logics.Clear(args);
            }
        }

        /// <summary>
        ///     Fired on animation start.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="GameObjectPlayAnimationEventArgs" /> instance containing the event data.</param>
        public static void OnPlayAnimation(Obj_AI_Base sender, GameObjectPlayAnimationEventArgs args)
        {
            if (sender.IsMe &&
                args.Animation.Equals("Spell3"))
            {
                if (Variables.Q.IsReady() && Targets.Target.IsValidTarget(Variables.Q.Range) && Menus.getCheckBoxItem(Variables.QMenu, "qspell.auto"))
                {
                    Variables.Q.Cast(Variables.Q.GetPrediction(Targets.Target).UnitPosition);
                }
            }
        }

        /// <summary>
        ///     Fired on an incoming gapcloser.
        /// </summary>
        /// <param name="gapcloser">The gapcloser.</param>
        public static void OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            /// <summary>
            ///     The Anti-GapCloser W Logic.
            /// </summary>
            if (Variables.W.IsReady() &&
                ObjectManager.Player.LSDistance(gapcloser.End) < Variables.W.Range &&
                Menus.getCheckBoxItem(Variables.WMenu, "wspell.gp"))
            {
                if (!ObjectManager.Get<Obj_AI_Minion>().Any(
                    m =>
                        m.LSDistance(gapcloser.End) < 100f &&
                        m.CharData.BaseSkinName.Contains("Cupcake")))
                {
                    Variables.W.Cast(gapcloser.End);
                }
            }

            /// <summary>
            ///     The Anti-GapCloser E Logic.
            /// </summary>
            if (Variables.E.IsReady() &&
                ObjectManager.Player.LSDistance(gapcloser.End) < 300f &&
                Menus.getCheckBoxItem(Variables.EMenu, "espell.gp"))
            {
                Variables.E.Cast(gapcloser.Sender.Position);
            }
        }
    }
}
