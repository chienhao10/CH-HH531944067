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
            {Spells.Q2, new Spell(SpellSlot.Q, 1200)},
            {Spells.W, new Spell(SpellSlot.W, 1000)},
            {Spells.E, new Spell(SpellSlot.E, 475)},
            {Spells.R, new Spell(SpellSlot.R, 3000f)}
        };

        public static Menu Menu { get; set; }

        public static bool HasPassive()
        {
            return ObjectManager.Player.HasBuff("LucianPassiveBuff");
        }

        internal enum Spells
        {
            Q,
            Q2,
            W,
            E,
            R
        }
    }
}