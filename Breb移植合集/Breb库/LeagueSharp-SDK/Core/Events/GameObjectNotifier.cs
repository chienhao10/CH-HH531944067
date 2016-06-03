namespace LeagueSharp.SDK
{
    using EloBuddy;
    using System;

    /// <summary>
    ///     Raises events when a type of <see cref="GameObject"/> is created.
    /// <example>
    /// <code>
    /// GameObjectNotifier&lt;Obj_AI_Minion&gt;.OnCreate += (sender, minion) => Game.PrintChat(minion.Name); 
    /// </code>
    /// </example>
    /// </summary>
    /// <typeparam name="T">The type of <see cref="GameObject" /></typeparam>
    public class GameObjectNotifier<T>
        where T : GameObject
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes static members of the <see cref="GameObjectNotifier{T}" /> class.
        /// </summary>
        static GameObjectNotifier()
        {
            GameObject.OnCreate += GameObjectOnCreate;
            GameObject.OnDelete += GameObjectOnDelete;
        }

        #endregion

        #region Public Events

        /// <summary>
        ///     Occurs when a <see cref="GameObject" /> of the type <typeparamref name="T" /> is created.
        /// </summary>
        public static event EventHandler<T> OnCreate;

        /// <summary>
        ///     Occurs when a <see cref="GameObject" /> of the type <typeparamref name="T" /> is deleted.
        /// </summary>
        public static event EventHandler<T> OnDelete;

        #endregion

        #region Methods

        private static void GameObjectOnCreate(GameObject sender, EventArgs args)
        {
            var obj = sender as T;
            if (obj != null)
            {
                OnCreate?.Invoke(null, obj);
            }
        }

        private static void GameObjectOnDelete(GameObject sender, EventArgs args)
        {
            var obj = sender as T;
            if (obj != null)
            {
                OnDelete?.Invoke(null, obj);
            }
        }

        #endregion
    }
}