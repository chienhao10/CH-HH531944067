using EloBuddy;

namespace ExorAIO.Champions.Tryndamere
{
    /// <summary>
    ///     The methods class.
    /// </summary>
    internal class Methods
    {
        /// <summary>
        ///     Sets the methods.
        /// </summary>
        public static void Initialize()
        {
            Game.OnUpdate += Tryndamere.OnUpdate;
        }
    }
}