using EloBuddy;

namespace ExorAIO.Utilities
{
    /// <summary>
    ///     The Mana manager class.
    /// </summary>
    internal class ManaManager
    {
        /// <summary>
        ///     The minimum mana needed to cast the Q Spell.
        /// </summary>
        public static int NeededQMana
        {
            get
            {
                return Variables.QMenu["qspell.mana"] != null
                    ? (int) (Variables.Q.ManaCost/ObjectManager.Player.MaxMana*100) +
                      Variables.getSliderItem(Variables.QMenu, "qspell.mana")
                    : 0;
                //Variables.QMenu["qspell.mana"]
            }
        }

        /// <summary>
        ///     The minimum mana needed to cast the W Spell.
        /// </summary>
        public static int NeededWMana
        {
            get
            {
                return Variables.WMenu["wspell.mana"] != null
                    ? (int) (Variables.W.ManaCost/ObjectManager.Player.MaxMana*100) +
                      Variables.getSliderItem(Variables.WMenu, "wspell.mana")
                    : 0;
            }
        }

        /// <summary>
        ///     The minimum mana needed to cast the E Spell.
        /// </summary>
        public static int NeededEMana
        {
            get
            {
                return Variables.EMenu["espell.mana"] != null
                    ? (int) (Variables.E.ManaCost/ObjectManager.Player.MaxMana*100) +
                      Variables.getSliderItem(Variables.EMenu, "espell.mana")
                    : 0;
            }
        }

        /// <summary>
        ///     The minimum mana needed to cast the R Spell.
        /// </summary>
        public static int NeededRMana
        {
            get
            {
                return Variables.RMenu["rspell.mana"] != null
                    ? (int) (Variables.R.ManaCost/ObjectManager.Player.MaxMana*100) +
                      Variables.getSliderItem(Variables.RMenu, "rspell.mana")
                    : 0;
            }
        }

        /// <summary>
        ///     The minimum mana needed to stack the Tear Item.
        /// </summary>
        public static int NeededTearMana
        {
            get
            {
                return Variables.MiscMenu["misc.tearmana"] != null
                    ? (int) (Variables.Q.ManaCost/ObjectManager.Player.MaxMana*100) +
                      Variables.getSliderItem(Variables.MiscMenu, "misc.tearmana")
                    : 0;
            }
        }
    }
}