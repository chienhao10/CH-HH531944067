using System;
//using System.Linq;
using EloBuddy;
using LeagueSharp.SDK;

namespace NabbActivator
{
    /// <summary>
    ///     The main class.
    /// </summary>
    internal class Index
    {
        /// <summary>
        ///     Loads the Activator.
        /// </summary>
        public static void OnLoad()
        {
            /// <summary>
            ///     Initialize the menus.
            /// </summary>
            Menus.Initialize();

            /// <summary>
            ///     Initialize the spells.
            /// </summary>
            ISpells.Initialize();

            /// <summary>
            ///     Initializes the methods.
            /// </summary>
            Methods.Initialize();

            /// <summary>
            ///     Initializes the resetters.
            /// </summary>
            Resetters.Initialize();

            /// <summary>
            ///     Initializes the drawings.
            /// </summary>
            Drawings.Initialize();

            /// <summary>
            ///     Initializes the healthbars.
            /// </summary>
            Healthbars.Initialize();
        }

        /// <summary>
        ///     Called when the game updates itself.
        /// </summary>
        /// <param name="args">The <see cref="EventArgs" /> instance containing the event data.</param>
        public static void OnUpdate(EventArgs args)
        {
            /*
            foreach (var enemy in GameObjects.EnemyHeroes.Where(
                t =>
                    t.CharData.BaseSkinName.Equals("Galio")))
            {
                foreach (var buff in enemy.Buffs.Where(b => b.Name != "galiorunicskin" && b.Name != "RegenerationPotion"))
                {
                    Console.WriteLine($"{enemy.CharData.BaseSkinName}: {buff.Name}");
                }
            }


            if (Targets.Target != null)
            {
                foreach (var buff in Targets.Target.Buffs.Where(b => b.Caster.IsMe))
                {
                    Console.WriteLine($"{Targets.Target.ChampionName}: {buff.Name}");
                }
            }

            foreach (var buff in GameObjects.Player.Buffs.Where(b => b.Caster.IsMe && b.Name != "isninja"))
            {
                Console.WriteLine($"{GameObjects.Player.ChampionName}: {buff.Name}");
            }
            */

            if (ObjectManager.Player.IsDead)
            {
                return;
            }

            /// <summary>
            ///     Loads the spells logics.
            /// </summary>
            Activator.Spells(args);

            /// <summary>
            ///     Loads the cleansers logics.
            /// </summary>
            Activator.Cleansers(args);

            /// <summary>
            ///     Loads the offensives logics.
            /// </summary>
            Activator.Offensives(args);

            /// <summary>
            ///     Loads the defensives logics.
            /// </summary>
            Activator.Defensives(args);

            /// <summary>
            ///     Loads the consumables logics.
            /// </summary>
            Activator.Consumables(args);
        }

        /// <summary>
        ///     Called on do-cast.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The args.</param>
        public static void OnDoCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            /// <summary>
            ///     Loads the special items logics.
            /// </summary>
            Activator.Specials(sender, args);

            /// <summary>
            ///     Loads the resetter-items logics.
            /// </summary>
            Activator.Resetters(sender, args);
        }

        /// <summary>
        ///     Called on do-cast.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The args.</param>
        public static void OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            /*
            if (sender.IsMe)
            {
                Console.WriteLine(args.SData.Name);
            }
            */
        }
    }
}