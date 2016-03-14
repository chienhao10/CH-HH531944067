using System;
using System.Collections.Generic;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK.Rendering;
using SharpDX;
using Settings = SivirDamage.Config.Misc;
using SivirDamage;
namespace SivirDamage
{
    public static class Program
    {
        // Change this line to the champion you want to make the addon for,
        // watch out for the case being correct!
        public const string ChampName = "Sivir";

        public static void OnLoadingComplete(EventArgs args)
        {
            // Verify the champion we made this addon for
            if (Player.Instance.ChampionName != ChampName)
            {
                // Champion is not the one we made this addon for,
                // therefore we return
                return;
            }

            // Initialize the classes that we need
            Config.Initialize();
            SpellManager.Initialize();
            ModeManager.Initialize();
            SaveMePls.Initialize();
            if (Settings.autolevelskills)
            {
                Player.Instance.Spellbook.LevelSpell(SpellSlot.Q);
            }

            // Listen to events we need
            Drawing.OnDraw += OnDraw;
            Player.OnLevelUp += SivirDamage.Modes.PermaActive.autoLevelSkills;

            Orbwalker.OnAttack += Modes.Combo.PostAttack;
            
        }

        private static void OnDraw(EventArgs args)
        {
            // Draw range circles of our spells
            if (Settings._drawQ.CurrentValue)
                Circle.Draw(Color.Aqua, SpellManager.Q.Range, Player.Instance.Position);

        }
    }
}
