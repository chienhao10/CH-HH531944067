
namespace FioraProject.Evade
{
    using System;
    using System.Linq;
    using LeagueSharp;
    using LeagueSharp.Common;
    using SharpDX;
    using EloBuddy.SDK.Menu.Values;
    using EloBuddy.SDK.Menu;
    internal static class Config
    {
        #region Constants

        public const int DiagonalEvadePointsCount = 7;

        public const int DiagonalEvadePointsStep = 20;

        public const int EvadingFirstTimeOffset = 250;

        public const int EvadingSecondTimeOffset = 80;

        public const int ExtraEvadeDistance = 15;

        public const int GridSize = 10;

        public const int SkillShotsExtraRadius = 9;

        public const int SkillShotsExtraRange = 20;

        public static Menu evadeMenu;

        #endregion

        #region Public Methods and Operators

        public static void CreateMenu()
        {
            evadeMenu = Program.Menu.AddSubMenu("Evade Skillshot", "Evade");
            evadeMenu.AddGroupLabel("Credit: Evade#");

            evadeMenu.AddGroupLabel("Use Spells to Dodge");
            evadeMenu.AddSeparator();
            foreach (var spell in EvadeSpellDatabase.Spells)
            {
                evadeMenu.AddGroupLabel(string.Format("{0} ({1})", spell.Name, spell.Slot));
                evadeMenu.Add(spell.Slot + "Tower", new CheckBox("Under Tower", false));
                evadeMenu.Add(spell.Slot + "Delay", new Slider("Extra Delay", 100, 0, 150));
                evadeMenu.Add(spell.Slot + "DangerLevel", new Slider("If Danger Level >=", 1, 1, 5));
                evadeMenu.Add(spell.Slot + "Enabled", new CheckBox("Enabled"));
                evadeMenu.AddSeparator();
            }
            evadeMenu.AddSeparator();

            evadeMenu.AddGroupLabel("Dodge :");
            evadeMenu.AddSeparator();
            foreach (var spell in SpellDatabase.Spells.Where(i => HeroManager.Enemies.Any(a => string.Equals(a.ChampionName, i.ChampionName, StringComparison.InvariantCultureIgnoreCase))))
            {
                evadeMenu.AddGroupLabel(string.Format("{0} ({1})", spell.SpellName, spell.Slot));
                evadeMenu.Add(spell.SpellName + "DangerLevel", new Slider("Danger Level", spell.DangerValue, 1, 5));
                evadeMenu.Add(spell.SpellName + "IsDangerous", new CheckBox("Is Dangerous", spell.IsDangerous));
                evadeMenu.Add(spell.SpellName + "DisableFoW", new CheckBox("Disable FoW Dodging", false));
                evadeMenu.Add(spell.SpellName + "Draw", new CheckBox("Draw", false));
                evadeMenu.Add(spell.SpellName + "Enabled", new CheckBox("Enabled", false));
                evadeMenu.AddSeparator();
            }
            evadeMenu.AddSeparator();

            evadeMenu.AddGroupLabel("Settings :");
            evadeMenu.Add("DrawStatus", new CheckBox("Draw Evade Status"));
            evadeMenu.Add("Enabled", new KeyBind("Enabled", false, KeyBind.BindTypes.PressToggle, 'K'));
            evadeMenu.Add("OnlyDangerous", new KeyBind("Dodge Only Dangerous", false, KeyBind.BindTypes.HoldActive, 32));
        }

        #endregion
    }
}