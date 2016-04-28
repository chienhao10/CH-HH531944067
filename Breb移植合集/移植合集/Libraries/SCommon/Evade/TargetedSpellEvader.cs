using System;
using System.Linq;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using LeagueSharp.Common;
using SCommon.Database;

namespace SCommon.Evade
{
    public class TargetedSpellEvader
    {
        private static Menu m_Menu;
        private Action<DetectedTargetedSpellArgs> m_fnEvade;

        /// <summary>
        ///     Initializes TargetedSpellEvader class
        /// </summary>
        /// <param name="fn">The evade function.</param>
        /// <param name="menuToAttach">The menu to attach.</param>
        public TargetedSpellEvader(Action<DetectedTargetedSpellArgs> fn, Menu menuToAttach)
        {
            TargetedSpellDetector.OnDetected += TargetedSpellDetector_OnDetected;
            RegisterEvadeFunction(fn);

            m_Menu = MainMenu.AddMenu("Targeted Spell Evader", "SCommon.TargetedSpellEvader.Root");
            foreach (var enemy in HeroManager.Enemies)
            {
                foreach (var spell in SpellDatabase.TargetedSpells.Where(p => p.ChampionName == enemy.ChampionName))
                {
                    m_Menu.Add(string.Format("SCommon.TargetedSpellEvader.Spell.{0}", spell.SpellName),
                        new CheckBox(string.Format("{0} ({1})", spell.ChampionName, spell.Slot) +
                                     (spell.IsDangerous ? " (Dangerous)" : "")));
                }
            }
            m_Menu.Add("SCommon.TargetedSpellEvader.DisableInCombo", new CheckBox("Disable In Combo Mode", false));
            m_Menu.Add("SCommon.TargetedSpellEvader.OnlyDangerous", new CheckBox("Only Dangerous", false));
            m_Menu.Add("SCommon.TargetedSpellEvader.Enabled", new CheckBox("Enabled"));
        }

        /// <summary>
        ///     Gets TargetedSpellEvader is enabled
        /// </summary>
        public bool IsEnabled
        {
            get { return getCheckBoxItem("SCommon.TargetedSpellEvader.Enabled"); }
        }

        /// <summary>
        ///     Gets TargetedSpellEvader is disabled while combo mode
        /// </summary>
        public bool DisableInComboMode
        {
            get { return getCheckBoxItem("SCommon.TargetedSpellEvader.DisableInCombo"); }
        }

        /// <summary>
        ///     Gets TargetedSpellEvader is enabled for only dangerous spells
        /// </summary>
        public bool OnlyDangerous
        {
            get { return getCheckBoxItem("SCommon.TargetedSpellEvader.OnlyDangerous"); }
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

        /// <summary>
        ///     Registers evade function
        /// </summary>
        /// <param name="fn">The function.</param>
        public void RegisterEvadeFunction(Action<DetectedTargetedSpellArgs> fn)
        {
            m_fnEvade = fn;
        }

        /// <summary>
        ///     Unregisters the evade function
        /// </summary>
        public void UnregisterEvadeFunction()
        {
            m_fnEvade = null;
        }

        /// <summary>
        ///     Event callback which fired when targeted spell is detected
        /// </summary>
        /// <param name="args">The args.</param>
        private void TargetedSpellDetector_OnDetected(DetectedTargetedSpellArgs args)
        {
            if (IsEnabled)
            {
                if (OnlyDangerous && !args.SpellData.IsDangerous)
                    return;

                if (m_fnEvade != null &&
                    getCheckBoxItem("SCommon.TargetedSpellEvader.Spell." + args.SpellData.SpellName))
                    m_fnEvade(args);
            }
        }
    }
}