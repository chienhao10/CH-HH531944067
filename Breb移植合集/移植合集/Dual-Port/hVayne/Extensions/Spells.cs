using EloBuddy;
using LeagueSharp.SDK;
using Spell = LeagueSharp.SDK.Spell;

namespace hVayne.Extensions
{
    class Spells
    {
        public static Spell Q, W, E, R;
        public static void ExecuteSpells()
        {
            Q = new Spell(SpellSlot.Q,300f);
            W = new Spell(SpellSlot.W);
            E = new Spell(SpellSlot.E,550f);
            E.SetTargetted(0.25F, 1600f);
            R = new Spell(SpellSlot.R);
        }
    }
}
