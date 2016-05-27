using System;
using System.Collections.Generic;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using SharpDX;
using Color = System.Drawing.Color;
using LeagueSharp.Common;
using LeagueSharp.Common.Data;

namespace NabbTracker
{
    using Font = SharpDX.Direct3D9.Font;
    using Color = System.Drawing.Color;    

    /// <summary>
    /// The Variables class.
    /// </summary>
    class Variables
    {

        public static int TickCount = (int)(Game.Time * 1000);

        public static bool getCheckBoxItem(Menu m, string item)
        {
            return m[item].Cast<CheckBox>().CurrentValue;
        }

        public static int getSliderItem(Menu m, string item)
        {
            return m[item].Cast<Slider>().CurrentValue;
        }

        public static bool getKeyBindItem(Menu m, string item)
        {
            return m[item].Cast<KeyBind>().CurrentValue;
        }

        /// <summary>
        ///     The Exp Healthbars X coordinate.
        /// </summary>
        public static int ExpX { internal get; set; }

        /// <summary>
        ///     The Exp Healthbars Y coordinate.
        /// </summary>
        public static int ExpY { internal get; set; }

        public static readonly List<string> SpecialChampions = new List<string> { "Annie", "Jhin" };

        /// <summary>
        ///     The Exp Healthbars X coordinate adjustment.
        /// </summary>
        public static int ExpXAdjustment(AIHeroClient target)
        {
            if (SpecialChampions.Contains(target.ChampionName))
            {
                return 77;
            }

            return 85;
        }

        /// <summary>
        ///     The Spells Healthbars Y coordinate adjustment.
        /// </summary>
        public static int ExpYAdjustment(AIHeroClient target)
        {
            if (SpecialChampions.Contains(target.ChampionName))
            {
                return -38;
            }

            return target.IsMe
                ? -30
                : -22;
        }

        /// <summary>
        /// The Menu.
        /// </summary>
        public static Menu Menu { get; set; }

        /// <summary>
        /// The Menu name.
        /// </summary>        
        public static string MainMenuName = "nabbtracker";

        /// <summary>
        /// The Menu Codename.
        /// </summary>
        public static string MainMenuCodeName = "NabbTracker";

        /// <summary>
        /// The Text fcnt.
        /// </summary>
        public static Font DisplayTextFont = new Font(Drawing.Direct3DDevice, new System.Drawing.Font("Tahoma", 8));

        /// <summary>
        /// Gets the SummonerSpell name.
        /// </summary>
        public static string GetSummonerSpellName;

        /// <summary>
        /// Gets the spellslots.
        /// </summary>
        public static SpellSlot[] SpellSlots = 
        {
            SpellSlot.Q, SpellSlot.W, SpellSlot.E, SpellSlot.R
        };

        /// <summary>
        ///     Gets the summoner spellslots.
        /// </summary>
        public static SpellSlot[] SummonerSpellSlots = 
        {
            SpellSlot.Summoner1, SpellSlot.Summoner2
        };


        /// <summary>
        ///     The Spells Healthbars X coordinate.
        /// </summary>
        public static int SpellX { get; set; }

        /// <summary>
        ///     The Spells Healthbars Y coordinate.
        /// </summary>
        public static int SpellY { get; set; }

        /// <summary>
        ///     The SummonerSpells Healthbar X coordinate.
        /// </summary>        
        public static int SummonerSpellX { get; set; }
        
        /// <summary>
        ///     The SummonerSpells Healthbar Y coordinate.
        /// </summary>        
        public static int SummonerSpellY { get; set; }

        /// <summary>
        ///     The SpellLevel X coordinate.
        /// </summary>        
        public static int SpellLevelX { get; set; }
        
        /// <summary>
        ///     The Healthbars Y coordinate.
        /// </summary>        
        public static int SpellLevelY { get; set; }
    }
}
