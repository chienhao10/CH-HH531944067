using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DZLib.Modules;
using LeagueSharp;
using LeagueSharp.Common;
using EloBuddy.SDK.Menu;
using EloBuddy;

namespace iLucian
{
    internal class Variables
    {
        public static readonly Dictionary<Spells, Spell> Spell = new Dictionary<Spells, Spell>
        {
            {Spells.Q, new Spell(SpellSlot.Q, 675)},
            {Spells.Q2, new Spell(SpellSlot.Q, 900)},
            {Spells.W, new Spell(SpellSlot.W, 900)},
            {Spells.E, new Spell(SpellSlot.E, 475)},
            {Spells.R, new Spell(SpellSlot.R, 3000f)},
            {Spells.Q3, new Spell(SpellSlot.Q)}


        };


        public static Menu Menu { get; set; }
        public static int LastECast = 0;

        public static bool HasPassive()
        {
            return ObjectManager.Player.HasBuff("LucianPassiveBuff");
        }

        internal enum Spells
        {
            Q,
            Q2,
            Q3,
            W,
            E,
            R
        }
    }
}