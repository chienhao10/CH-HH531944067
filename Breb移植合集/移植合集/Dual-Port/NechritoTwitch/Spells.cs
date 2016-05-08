using EloBuddy;
using LeagueSharp.Common;

namespace Nechrito_Twitch
{
    class Spells
    {
        private static AIHeroClient Player = ObjectManager.Player;
        public static Spell  _w, _e, _r, _recall;

        public static void Initialise()
        {
            _w = new Spell(SpellSlot.W, 950);
           _w.SetSkillshot(0.25f, 120f, 1400f, false, SkillshotType.SkillshotCircle);
            _e = new Spell(SpellSlot.E, 1200);
            _r = new Spell(SpellSlot.R, 900);
            _recall = new Spell(SpellSlot.Recall);
        }
    }
}
