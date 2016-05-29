using System;
using ExorSDK.Utilities;
using LeagueSharp;
using LeagueSharp.SDK;
using EloBuddy;
using EloBuddy.SDK;

namespace ExorSDK.Champions.Anivia
{
    /// <summary>
    ///     The champion class.
    /// </summary>
    internal class Anivia
    {
        /// <summary>
        ///     Defines the missile object for the Q.
        /// </summary>
        public static GameObject QMissile;

        /// <summary>
        ///     Defines the missile object for the R.
        /// </summary>
        public static GameObject RMissile;

        /// <summary>
        ///     Loads Anivia.
        /// </summary>
        public void OnLoad()
        {
            /// <summary>
            ///     Initializes the menus.
            /// </summary>
            Menus.Initialize();

            /// <summary>
            ///     Initializes the spells.
            /// </summary>
            Spells.Initialize();

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
        ///     Called when an object gets created by the game.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="args">The <see cref="EventArgs" /> instance containing the event data.</param>
        public static void OnCreate(GameObject obj, EventArgs args)
        {
            if (obj.IsValid)
            {
                /// <summary>
                ///     Defines the missile object for the Q.
                /// </summary>
                if (obj.Name.Equals("cryo_FlashFrost_Player_mis.troy"))
                {
                    QMissile = obj;
                }

                /// <summary>
                ///     Defines the missile object for the R.
                /// </summary>
                if (obj.Name.Contains("cryo_storm"))
                {
                    RMissile = obj;
                }
            }
        }

        /// <summary>
        ///     Called when an object gets deleted by the game.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="args">The <see cref="EventArgs" /> instance containing the event data.</param>
        public static void OnDelete(GameObject obj, EventArgs args)
        {
            if (obj.IsValid)
            {
                /// <summary>
                ///     Removes the missile object for the Q.
                /// </summary>
                if (obj.Name.Equals("cryo_FlashFrost_Player_mis.troy"))
                {
                    QMissile = null;
                }

                /// <summary>
                ///     Removes the missile object for the R.
                /// </summary>
                if (obj.Name.Contains("cryo_storm"))
                {
                    RMissile = null;
                }
            }
        }

        /// <summary>
        ///     Fired when the game is updated.
        /// </summary>
        /// <param name="args">The <see cref="EventArgs" /> instance containing the event data.</param>
        public static void OnUpdate(EventArgs args)
        {
            if (GameObjects.Player.IsDead)
            {
                return;
            }

            /// <summary>
            ///     Initializes the Automatic actions.
            /// </summary>
            Logics.Automatic(args);

            /// <summary>
            ///     Initializes the Killsteal events.
            /// </summary>
            Logics.Killsteal(args);

            if (GameObjects.Player.Spellbook.IsAutoAttacking)
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
        ///     Fired on an incoming gapcloser.
        /// </summary>
        /// <param name="sender">The object.</param>
        /// <param name="args">The <see cref="Events.GapCloserEventArgs" /> instance containing the event data.</param>
        public static void OnGapCloser(object sender, Events.GapCloserEventArgs args)
        {
            if (Vars.W.IsReady() &&
                args.IsDirectedToPlayer &&
                args.Sender.LSIsValidTarget(Vars.W.Range) &&
                Vars.getCheckBoxItem(Vars.WMenu, "gapcloser"))
            {
                Vars.W.Cast(
                    GameObjects.Player.ServerPosition.LSExtend(
                        args.Sender.ServerPosition, GameObjects.Player.BoundingRadius));
            }
        }

        /// <summary>
        ///     Fired on interruptable spell.
        /// </summary>
        /// <param name="sender">The object.</param>
        /// <param name="args">The <see cref="Events.InterruptableTargetEventArgs" /> instance containing the event data.</param>
        public static void OnInterruptableTarget(object sender, Events.InterruptableTargetEventArgs args)
        {
            if (Vars.W.IsReady() &&
                args.Sender.LSIsValidTarget(Vars.W.Range) &&
                Vars.getCheckBoxItem(Vars.WMenu, "interrupter"))
            {
                if (GameObjects.Player.Distance(
                        GameObjects.Player.ServerPosition.LSExtend(
                            args.Sender.ServerPosition,
                            GameObjects.Player.Distance(args.Sender) + 20f)) < Vars.W.Range)
                {
                    Vars.W.Cast(
                        GameObjects.Player.ServerPosition.LSExtend(
                            args.Sender.ServerPosition, GameObjects.Player.Distance(args.Sender) + 20f));
                }
            }
        }
    }
}