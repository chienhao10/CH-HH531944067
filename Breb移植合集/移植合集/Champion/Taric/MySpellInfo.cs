using System;
using System.Collections.Generic;
using EloBuddy;

namespace PippyTaric
{
    public static class MySpellInfo
    {
        public static readonly Dictionary<string, float> SpellTable = new Dictionary<string, float>();

        public static void Initialize()
        {
            SpellTable.Add("qRange", 750f);
            SpellTable.Add("wRange", 400f);
            SpellTable.Add("eRange", 625f);
            SpellTable.Add("rRange", 400f);
            SpellTable.Add("qDelay", 0.5f);
            SpellTable.Add("wDelay", 0.5f);
            SpellTable.Add("eDelay", 0.5f);
            SpellTable.Add("rDelay", 0.5f);
            SpellTable.Add("eSpeed", 1400f);

            if (Program.DebugMode)
            {
                foreach (var spelldata in ObjectManager.Player.Spellbook.Spells)
                {
                    Console.WriteLine("Spell Name: " + spelldata.SData.Name);
                    Console.WriteLine("Spell Range: " + spelldata.SData.CastRange);
                    Console.WriteLine("Spell Delay: " + spelldata.SData.DelayTotalTimePercent);
                    Console.WriteLine("Spell Speed: " + spelldata.SData.MissileSpeed);
                }
            }
        }
    }
}