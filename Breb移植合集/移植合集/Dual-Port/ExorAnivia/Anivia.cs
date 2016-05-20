using LeagueSharp;
using LeagueSharp.Common;

namespace ExorAIO.Champions.Anivia
{
    using System;
    using ExorAIO.Utilities;
    using EloBuddy;
    using EloBuddy.SDK;

    /// <summary>
    ///     The champion class.
    /// </summary>
    class Anivia
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
        public static void OnLoad()
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
        /// <param name="args">The <see cref="EventArgs"/> instance containing the event data.</param>
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
        /// <param name="args">The <see cref="EventArgs"/> instance containing the event data.</param>
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
            if (ObjectManager.Player.IsDead)
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
        ///     Called on interruptable spell.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="Interrupter2.InterruptableTargetEventArgs"/> instance containing the event data.</param>
        public static void OnInterruptableTarget(AIHeroClient sender, Interrupter2.InterruptableTargetEventArgs args)
        {
            /// <summary>
            ///     The Interrupter W Logic.
            /// </summary>
            if (Variables.W.IsReady() &&
                !Bools.IsSpellShielded(sender) &&
                sender.IsValidTarget(Variables.W.Range) &&
               Menus.getCheckBoxItem(Variables.WMenu, "wspell.ir"))
            {
                Variables.W.Cast(ObjectManager.Player.ServerPosition.Extend(Targets.Target.ServerPosition, ObjectManager.Player.Distance(Targets.Target) + 5f));
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
                gapcloser.Sender.IsValidTarget(Variables.W.Range) &&
                Menus.getCheckBoxItem(Variables.WMenu, "wspell.gp"))
            {
                if (gapcloser.SkillType.Equals(GapcloserType.Targeted) &&
                    ObjectManager.Player.Distance(gapcloser.End) < 100f)
                {
                    Variables.W.Cast(gapcloser.End.Extend(gapcloser.Sender.Position, 10f));
                }
            }
        }
    }
}
