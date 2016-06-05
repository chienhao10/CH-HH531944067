using EloBuddy;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;

namespace SCommon.Orbwalking
{
    public class ConfigMenu
    {
        private static Menu m_Menu;

        public ConfigMenu()
        {
            m_Menu = MainMenu.AddMenu("走砍菜单", "Orbwalking.Root");
            m_Menu.AddGroupLabel("按键");
            m_Menu.Add("Orbwalking.Root.blLastHit", new KeyBind("尾兵", false, KeyBind.BindTypes.HoldActive, 'X'));
            m_Menu.Add("Orbwalking.Root.blHarass", new KeyBind("骚扰", false, KeyBind.BindTypes.HoldActive, 'C'));
            m_Menu.Add("Orbwalking.Root.blLaneClear",new KeyBind("清线", false, KeyBind.BindTypes.HoldActive, 'V'));
            m_Menu.Add("Orbwalking.Root.blCombo", new KeyBind("连招", false, KeyBind.BindTypes.HoldActive, 32));
            m_Menu.AddSeparator();
            m_Menu.AddGroupLabel("杂项");
            m_Menu.Add("Orbwalking.Misc.blAttackStructures", new CheckBox("攻击建筑"));
            m_Menu.Add("Orbwalking.Misc.blFocusNormalWhileTurret", new CheckBox("未被塔攻击时集火塔"));
            m_Menu.Add("Orbwalking.Misc.blSupportMode", new CheckBox("辅助模式", false));
            m_Menu.Add("Orbwalking.Misc.blDontAttackChampWhileLaneClear", new CheckBox("清线不攻击敌方英雄", false));
            m_Menu.Add("Orbwalking.Misc.blDisableAA", new CheckBox("屏蔽普攻", false));
            m_Menu.Add("Orbwalking.Misc.blDontMoveMouseOver", new CheckBox("鼠标移动在英雄身上停止移动", false));
            m_Menu.Add("Orbwalking.Misc.blMagnetMelee", new CheckBox("黏住目标 (近程英雄)"));
            m_Menu.Add("Orbwalking.Misc.iStickRange", new Slider("黏住范围", 390, 0, 600));
            m_Menu.Add("Orbwalking.Misc.blDontMoveInRange", new CheckBox("敌方在普攻范围则不移动", false));
            m_Menu.AddSeparator();
            m_Menu.Add("Orbwalking.Misc.blLegitMode", new CheckBox("安全模式（看起来不像外挂）", false));
            m_Menu.Add("Orbwalking.Misc.iLegitPercent", new Slider("安全模式%", 20));
            m_Menu.AddSeparator();
            m_Menu.AddGroupLabel("线圈");
            m_Menu.Add("Orbwalking.Drawings.SelfAACircle", new CheckBox("自身普攻范围"));
            m_Menu.Add("Orbwalking.Drawings.EnemyAACircle", new CheckBox("敌方普攻范围", false));
            m_Menu.Add("Orbwalking.Drawings.LastHitMinion", new CheckBox("可击杀的小兵", false));
            m_Menu.Add("Orbwalking.Drawings.HoldZone", new CheckBox("停止移动范围", false));
            m_Menu.Add("Orbwalking.Drawings.iLineWidth", new Slider("线宽", 2, 1, 6));
            m_Menu.AddSeparator();
            m_Menu.AddGroupLabel("额外设置");
            m_Menu.Add("Orbwalking.Root.iExtraWindup", new Slider("额外前摇时间", 0, 0, 200));
            m_Menu.Add("Orbwalking.Root.iMovementDelay", new Slider("移动延迟", 0, 0, 1000));
            m_Menu.Add("Orbwalking.Root.iHoldPosition", new Slider("停止移动半径", 0, 0, 112));
        }

        public bool LegitMode
        {
            get { return getCheckBoxItem("Orbwalking.Misc.blLegitMode"); }
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