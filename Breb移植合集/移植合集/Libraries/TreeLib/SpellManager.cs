using ClipperLib;
using Color = System.Drawing.Color;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK;
using EloBuddy;
using Font = SharpDX.Direct3D9.Font;
using LeagueSharp.Common.Data;
using LeagueSharp.Common;
using SharpDX.Direct3D9;
using SharpDX;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using System.Security.AccessControl;
using System;
using System.Speech.Synthesis;

namespace TreeLib.Managers
{
    public static class SpellManager
    {
        public static LeagueSharp.Common.Spell Ignite;
        public static LeagueSharp.Common.Spell Smite;
        internal static Menu Menu;

        internal static void Initialize()
        {
            CustomEvents.Game.OnGameLoad += Game_OnGameLoad;
        }

        private static void Game_OnGameLoad(EventArgs args)
        {
            Menu = MainMenu.AddMenu("Summoners", "Summoners");
            Menu.Add("SmiteManagerEnabled", new CheckBox("Load Smite Manager"));
            Menu.Add("IgniteManagerEnabled", new CheckBox("Load Ignite Manager"));

            var smite = ObjectManager.Player.Spellbook.Spells.FirstOrDefault(h => h.Name.ToLower().Contains("smite"));

            if (smite != null && !smite.Slot.Equals(SpellSlot.Unknown))
            {
                Smite = new LeagueSharp.Common.Spell(smite.Slot, 500);
                Smite.SetTargetted(.333f, 20);

                if (Menu["SmiteManagerEnabled"].Cast<CheckBox>().CurrentValue)
                {
                    SmiteManager.Initialize();
                }
            }

            var igniteSlot = ObjectManager.Player.GetSpellSlot("summonerdot");

            if (!igniteSlot.Equals(SpellSlot.Unknown))
            {
                Ignite = new LeagueSharp.Common.Spell(igniteSlot, 600);
                Ignite.SetTargetted(.172f, 20);

                if (Menu["IgniteManagerEnabled"].Cast<CheckBox>().CurrentValue)
                {
                    IgniteManager.Initialize();
                }
            }
        }
    }
}