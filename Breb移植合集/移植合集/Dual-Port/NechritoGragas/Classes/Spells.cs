
using EloBuddy;
using LeagueSharp;
using LeagueSharp.Common;

namespace Nechrito_Gragas
{
    class Spells
    {
        private static AIHeroClient Player = ObjectManager.Player;
        public static SpellSlot Ignite, Smite;
        public static Spell _q, _w, _e, _r;
        public static void Initialise()
        {
            _q = new Spell(SpellSlot.Q, 775);
            _q.SetSkillshot(0.3f, 110f, 1000f, false, SkillshotType.SkillshotCircle);
            _w = new Spell(SpellSlot.W, 0);
            _e = new Spell(SpellSlot.E, 600);
            _e.SetSkillshot(0.15f, 50, 900, true, SkillshotType.SkillshotLine);
            _r = new Spell(SpellSlot.R, 1050);
            _r.SetSkillshot(0.3f, 700, 1000, false, SkillshotType.SkillshotCircle);
            Ignite = Player.GetSpellSlot("SummonerDot");
        }
    }
}
