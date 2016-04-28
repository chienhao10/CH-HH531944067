using EloBuddy;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;

namespace SCommon.Orbwalking
{
    public class ConfigMenu
    {
        private static Menu m_Menu;

        public ConfigMenu(Menu menuToAttach)
        {
            m_Menu = MainMenu.AddMenu("Orbwalking", "Orbwalking.Root");
            m_Menu.Add("Orbwalking.Root.iExtraWindup", new Slider("Extra Windup Time"));
            m_Menu.Add("Orbwalking.Root.iMovementDelay", new Slider("Movement Delay", 0, 0, 1000));
            m_Menu.Add("Orbwalking.Root.iHoldPosition", new Slider("Hold Area Radius", 0, 0, 250));

            m_Menu.Add("Orbwalking.Root.blLastHit", new KeyBind("Last Hit", false, KeyBind.BindTypes.HoldActive, 'X'));
            m_Menu.Add("Orbwalking.Root.blHarass", new KeyBind("Harass", false, KeyBind.BindTypes.HoldActive, 'C'));
            m_Menu.Add("Orbwalking.Root.blLaneClear",
                new KeyBind("Lane Clear", false, KeyBind.BindTypes.HoldActive, 'V'));
            m_Menu.Add("Orbwalking.Root.blCombo", new KeyBind("Combo", false, KeyBind.BindTypes.HoldActive, 32));

            m_Menu.AddGroupLabel("Misc");
            m_Menu.Add("Orbwalking.Misc.blAttackStructures", new CheckBox("Attack Structures"));
            m_Menu.Add("Orbwalking.Misc.blFocusNormalWhileTurret", new CheckBox("Focus mins. not focused by turret"));
            m_Menu.Add("Orbwalking.Misc.blSupportMode", new CheckBox("Support Mode", false));
            m_Menu.Add("Orbwalking.Misc.blDontAttackChampWhileLaneClear",
                new CheckBox("Dont attack champions while Lane Clear", false));
            m_Menu.Add("Orbwalking.Misc.blDisableAA", new CheckBox("Disable AutoAttack", false));
            m_Menu.Add("Orbwalking.Misc.blDontMoveMouseOver", new CheckBox("Mouse over hero to stop move", false));
            m_Menu.Add("Orbwalking.Misc.blMagnetMelee", new CheckBox("Magnet Target (Only Melee)"));
            m_Menu.Add("Orbwalking.Misc.iStickRange", new Slider("Stick Range", 390, 0, 600));
            m_Menu.Add("Orbwalking.Misc.blDontMoveInRange", new CheckBox("Dont move if enemy in AA range", false));
            m_Menu.Add("Orbwalking.Misc.blLegitMode", new CheckBox("Legit Mode", false));
            m_Menu.Add("Orbwalking.Misc.iLegitPercent", new Slider("Make Me Legit %", 20));

            m_Menu.AddGroupLabel("Drawings");
            m_Menu.Add("Orbwalking.Drawings.SelfAACircle", new CheckBox("Self AA Circle"));
            m_Menu.Add("Orbwalking.Drawings.EnemyAACircle", new CheckBox("Enemy AA Circle", false));
            m_Menu.Add("Orbwalking.Drawings.LastHitMinion", new CheckBox("Last Hitable Minion", false));
            m_Menu.Add("Orbwalking.Drawings.HoldZone", new CheckBox("Hold Zone", false));
            m_Menu.Add("Orbwalking.Drawings.iLineWidth", new Slider("Line Width", 2, 1, 6));
        }

        /// <summary>
        ///     Gets or sets combo key is pressed
        /// </summary>
        public bool Combo
        {
            get { return getKeyBindItem("Orbwalking.Root.blCombo"); }
        }

        /// <summary>
        ///     Gets harass key is pressed
        /// </summary>
        public bool Harass
        {
            get { return getKeyBindItem("Orbwalking.Root.blHarass"); }
        }

        /// <summary>
        ///     Gets lane clear key is pressed
        /// </summary>
        public bool LaneClear
        {
            get { return getKeyBindItem("Orbwalking.Root.blLaneClear"); }
        }

        /// <summary>
        ///     Gets last hit key is pressed
        /// </summary>
        public bool LastHit
        {
            get { return getKeyBindItem("Orbwalking.Root.blLastHit"); }
        }

        /// <summary>
        ///     Gets or sets extra windup time value
        /// </summary>
        public int ExtraWindup
        {
            get { return getSliderItem("Orbwalking.Root.iExtraWindup"); }
        }

        /// <summary>
        ///     Gets or sets movement delay value
        /// </summary>
        public int MovementDelay
        {
            get { return getSliderItem("Orbwalking.Root.iMovementDelay"); }
        }

        /// <summary>
        ///     Gets or sets hold area radius value
        /// </summary>
        public int HoldAreaRadius
        {
            get { return getSliderItem("Orbwalking.Root.iHoldPosition"); }
        }

        /// <summary>
        ///     Gets or sets attack structures value
        /// </summary>
        public bool AttackStructures
        {
            get { return getCheckBoxItem("Orbwalking.Misc.blAttackStructures"); }
        }

        /// <summary>
        ///     Gets or sets focus normal while turret value
        /// </summary>
        public bool FocusNormalWhileTurret
        {
            get { return getCheckBoxItem("Orbwalking.Misc.blFocusNormalWhileTurret"); }
        }

        /// <summary>
        ///     Gets or sets support mode value
        /// </summary>
        public bool SupportMode
        {
            get { return getCheckBoxItem("Orbwalking.Misc.blSupportMode"); }
        }

        /// <summary>
        ///     Gets or sets Dont attack champions while laneclear mode value
        /// </summary>
        public bool DontAttackChampWhileLaneClear
        {
            get { return getCheckBoxItem("Orbwalking.Misc.blDontAttackChampWhileLaneClear"); }
        }

        /// <summary>
        ///     Gets or sets disable aa value
        /// </summary>
        public bool DisableAA
        {
            get { return getCheckBoxItem("Orbwalking.Misc.blDisableAA"); }
        }

        /// <summary>
        ///     Gets or sets Dont move over value
        /// </summary>
        public bool DontMoveMouseOver
        {
            get { return getCheckBoxItem("Orbwalking.Misc.blDontMoveMouseOver"); }
        }

        /// <summary>
        ///     Gets or set magnet melee value
        /// </summary>
        public bool MagnetMelee
        {
            get { return getCheckBoxItem("Orbwalking.Misc.blMagnetMelee"); }
        }

        /// <summary>
        ///     Gets or sets stick range value
        /// </summary>
        public int StickRange
        {
            get { return getSliderItem("Orbwalking.Misc.iStickRange"); }
        }

        /// <summary>
        ///     Gets or sets dont move in aa range value
        /// </summary>
        public bool DontMoveInRange
        {
            get { return getCheckBoxItem("Orbwalking.Misc.blDontMoveInRange"); }
        }

        /// <summary>
        ///     Gets or set legit percent value
        /// </summary>
        public int LegitPercent
        {
            get { return getSliderItem("Orbwalking.Misc.iLegitPercent"); }
        }

        /// <summary>
        ///     Gets Self aa circle value
        /// </summary>
        public bool SelfAACircle
        {
            get { return getCheckBoxItem("Orbwalking.Drawings.SelfAACircle"); }
        }

        /// <summary>
        ///     Gets enemy aa circle value
        /// </summary>
        public bool EnemyAACircle
        {
            get { return getCheckBoxItem("Orbwalking.Drawings.EnemyAACircle"); }
        }

        /// <summary>
        ///     Gets last hit minion value
        /// </summary>
        public bool LastHitMinion
        {
            get { return getCheckBoxItem("Orbwalking.Drawings.LastHitMinion"); }
        }

        /// <summary>
        ///     Gets hold zone value
        /// </summary>
        public bool HoldZone
        {
            get { return getCheckBoxItem("Orbwalking.Drawings.HoldZone"); }
        }

        /// <summary>
        ///     Gets line width value
        /// </summary>
        public int LineWidth
        {
            get { return getSliderItem("Orbwalking.Drawings.iLineWidth"); }
        }

        public static bool getCheckBoxItem(string item)
        {
            return m_Menu[item].Cast<CheckBox>().CurrentValue;
        }

        public static int getSliderItem(string item)
        {
            return m_Menu[item].Cast<Slider>().CurrentValue;
        }

        public static bool getKeyBindItem(string item)
        {
            return m_Menu[item].Cast<KeyBind>().CurrentValue;
        }
    }
}