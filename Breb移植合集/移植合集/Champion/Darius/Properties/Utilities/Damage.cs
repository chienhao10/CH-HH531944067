using EloBuddy;
using ExorAIO.Utilities;

namespace ExorAIO.Champions.Darius
{
    /// <summary>
    ///     The killsteal class.
    /// </summary>
    internal class Damage
    {
        public static float GetRDamage(AIHeroClient target)
        {
            return (float) (Variables.R.GetDamage(target)*(1 + 0.20*target.GetBuffCount("dariushemo")));
        }
    }
}