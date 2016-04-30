using System;
using System.Linq;
using LeagueSharp.Common;
using TreeLib.Extensions;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;

namespace TreeLib.SpellData
{
    internal static class Config
    {
        #region Public Methods and Operators

        public static void CreateMenu()
        {
            Menu = MainMenu.AddMenu("Evade Skillshot", "Evade");
            foreach (var spell in SpellDatabase.Spells.Where(i => HeroManager.Enemies.Any(a => string.Equals(a.ChampionName, i.ChampionName, StringComparison.InvariantCultureIgnoreCase))))
            {
                Menu.AddGroupLabel(string.Format("{0} ({1})", spell.SpellName, spell.Slot));
                Menu.Add("DangerLevel", new Slider("Danger Level", spell.DangerValue, 1, 5));
                Menu.Add("IsDangerous", new CheckBox("Is Dangerous", spell.IsDangerous));
                Menu.Add("DisableFoW", new CheckBox("Disable FoW Dodging", false));
                Menu.Add("Draw", new CheckBox("Draw", false));
                Menu.Add("Enabled", new CheckBox("Enabled", !spell.DisabledByDefault));
                Menu.AddSeparator();
            }
            Menu.AddSeparator();
            Menu.Add("DrawStatus", new CheckBox("Draw Evade Status"));
            Menu.Add("EnabledA", new KeyBind("Enabled", false, KeyBind.BindTypes.PressToggle, 'K'));
            Menu.Add("OnlyDangerous", new KeyBind("Dodge Only Dangerous", false, KeyBind.BindTypes.HoldActive, 32));
        }

        #endregion

        #region Constants

        public const int DiagonalEvadePointsCount = 7;

        public const int DiagonalEvadePointsStep = 20;

        public const int EvadingFirstTimeOffset = 250;

        public const int EvadingSecondTimeOffset = 80;

        public const int ExtraEvadeDistance = 15;

        public const int GridSize = 10;

        public const int SkillShotsExtraRadius = 9;

        public const int SkillShotsExtraRange = 20;

        public static Menu Menu;

        #endregion
    }
}