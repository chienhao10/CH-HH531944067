using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DZLib.Modules;
using iDZEzreal.Modules;
using LeagueSharp;
using LeagueSharp.Common;
using EloBuddy;
using EloBuddy.SDK.Menu;

namespace iDZEzreal
{
    internal class Variables
    {
        public static readonly Dictionary<SpellSlot, Spell> Spells = new Dictionary<SpellSlot, Spell>
        {   
            {SpellSlot.Q, new Spell(SpellSlot.Q, 1250)},
            {SpellSlot.W, new Spell(SpellSlot.W, 1000)},
            {SpellSlot.E, new Spell(SpellSlot.E, 475)},
            {SpellSlot.R, new Spell(SpellSlot.R, 3000f)}
        };

        public static Menu Menu { get; set; }
        public static Menu comboMenu { get; set; }
        public static Menu mixedMenu { get; set; }
        public static Menu farmMenu { get; set; }
        public static Menu miscMenu { get; set; }
        public static Menu moduleMenu { get; set; }
        public static Menu drawingsMenu { get; set; }

        public static readonly List<IModule> Modules = new List<IModule>
        {
            new AutoHarassModule(), new QKSModule(), new SemiRModule()
        }; 
    }
}