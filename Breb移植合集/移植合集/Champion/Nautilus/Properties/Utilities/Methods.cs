using EloBuddy;

namespace ExorAIO.Champions.Nautilus
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
            Game.OnUpdate += Nautilus.OnUpdate;
            Obj_AI_Base.OnSpellCast += Nautilus.OnDoCast;
        }
    }
}