using EloBuddy;

namespace ExorAIO.Champions.Renekton
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
            Game.OnUpdate += Renekton.OnUpdate;
            Obj_AI_Base.OnSpellCast += Renekton.OnDoCast;
        }
    }
}