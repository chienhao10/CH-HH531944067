using EloBuddy;
using LeagueSharp.Common;

namespace Nechrito_Diana
{
    class Spells
    {
        public static SpellSlot Ignite, Flash;
        public static LeagueSharp.Common.Spell _q, _w, _e, _r;
        private static AIHeroClient Player = ObjectManager.Player;
        public static void Initialise()
        {
            _q = new LeagueSharp.Common.Spell(SpellSlot.Q, 825f);
             _q.SetSkillshot(0.25f, 185, 1640, false, SkillshotType.SkillshotCone);
            _w = new LeagueSharp.Common.Spell(SpellSlot.W);
            _e = new LeagueSharp.Common.Spell(SpellSlot.E, 450);
            _r = new LeagueSharp.Common.Spell(SpellSlot.R, 825);

            Ignite = Player.GetSpellSlot("SummonerDot");
            Flash = Player.GetSpellSlot("SummonerFlash");
        }
    }
}
