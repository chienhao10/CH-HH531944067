using EloBuddy;

namespace NabbActivator
{
    /// <summary>
    ///     The managers class.
    /// </summary>
    internal class Managers
    {
        /// <summary>
        ///     Sets the minimum necessary health percent to use a health potion.
        /// </summary>
        public static int MinHealthPercent => Vars.getSliderItem(Vars.SliderMenu, "health");

        /// <summary>
        ///     Sets the minimum necessary mana percent to use a mana potion.
        /// </summary>
        public static int MinManaPercent => Vars.getSliderItem(Vars.SliderMenu, "mana");
    }
}