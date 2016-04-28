namespace Valvrave_Sharp.Evade
{
    #region

    using System;
    using System.Linq;
    using EloBuddy;
    using LeagueSharp.SDK;
    using EloBuddy.SDK.Menu.Values;
    using EloBuddy.SDK.Menu;
    using Valvrave_Sharp.Core;
    using EloBuddy.SDK;
    #endregion

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

        #endregion

        public static Menu evadeMenu;

        #region Public Methods and Operators

        public static void CreateMenu(Menu mainMenu)
        {
            evadeMenu = mainMenu.AddSubMenu("Evade", "Evade Skillshot");

            evadeMenu.AddGroupLabel("Settings ::::::::::::::::::::::");
            evadeMenu.Add("DisableFoW", new CheckBox("Disable All FoW Dodging", false));
            evadeMenu.Add("Enabled", new KeyBind("Enabled", false, KeyBind.BindTypes.PressToggle, 'K'));
            evadeMenu.Add("OnlyDangerous", new KeyBind("Dodge Only Dangerous", false, KeyBind.BindTypes.HoldActive, 32));
            evadeMenu.AddSeparator();

            evadeMenu.AddGroupLabel("My Spells :::::::::::::::::::::");
            evadeMenu.AddSeparator();
            {
                foreach (var spell in EvadeSpellDatabase.Spells)
                {
                    evadeMenu.AddGroupLabel(spell.Name);
                    if (spell.UnderTower)
                    {
                        evadeMenu.Add(spell.Name + "Tower", new CheckBox("Under Tower", false));
                    }
                    if (spell.ExtraDelay)
                    {
                        evadeMenu.Add(spell.Name + "Delay", new Slider("Extra Delay", 100, 0, 150));
                    }
                    evadeMenu.Add(spell.Name + "DangerLevel", new Slider("If Danger Level >=", spell.DangerLevel, 1, 5));
                    if (spell.IsTargetted && spell.ValidTargets.Contains(SpellValidTargets.AllyWards))
                    {
                        evadeMenu.Add(spell.Name + "WardJump", new CheckBox("Ward Jump"));
                    }
                    evadeMenu.Add(spell.Name + "Enabled", new CheckBox("Enabled"));
                    evadeMenu.AddSeparator();
                }
            }
            evadeMenu.AddSeparator();

            evadeMenu.AddGroupLabel("Evade Spells ::::::::::::::::::::");
            foreach (var spell in SpellDatabase.Spells.Where(i => EntityManager.Heroes.Enemies.Any(a => string.Equals(a.ChampionName, i.ChampionName, StringComparison.InvariantCultureIgnoreCase))))
            {
                evadeMenu.AddGroupLabel($"{spell.SpellName} ({spell.Slot})");
                evadeMenu.Add(spell.SpellName + "DangerLevel", new Slider("Danger Level", spell.DangerValue, 1, 5));
                evadeMenu.Add(spell.SpellName + "IsDangerous", new CheckBox("Is Dangerous", spell.IsDangerous));
                if (!spell.DisableFowDetection)
                {
                    evadeMenu.Add(spell.SpellName + "DisableFoW", new CheckBox("Disable FoW Dodging", false));
                }
                evadeMenu.Add(spell.SpellName + "Draw", new CheckBox("Draw"));
                evadeMenu.Add(spell.SpellName + "Enabled", new CheckBox("Enabled", !spell.DisabledByDefault));
            }
            evadeMenu.AddSeparator();

            evadeMenu.AddGroupLabel("Draw");
            evadeMenu.Add("Skillshot", new CheckBox("Skillshot"));
            evadeMenu.Add("Status", new CheckBox("Evade Status"));
            evadeMenu.AddSeparator();

        }

        #endregion
    }
}