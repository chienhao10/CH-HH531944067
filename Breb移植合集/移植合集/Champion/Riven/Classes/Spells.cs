using EloBuddy;
using LeagueSharp.Common;

namespace NechritoRiven
{
    class Spells
    {
        public const string IsFirstR = "RivenFengShuiEngine";
        public const string IsSecondR = "RivenIzunaBlade";
        private static AIHeroClient Player = ObjectManager.Player;
        public static SpellSlot Ignite, Flash;
        public static LeagueSharp.Common.Spell _q, _w, _e, _r;
        public static void Initialise()
        {
            _q = new LeagueSharp.Common.Spell(SpellSlot.Q, 260f);
            _q.SetSkillshot(0.25f, 100f, 2200f, false, SkillshotType.SkillshotCircle);
            _w = new LeagueSharp.Common.Spell(SpellSlot.W, 250f);
            _e = new LeagueSharp.Common.Spell(SpellSlot.E, 270);
            _r = new LeagueSharp.Common.Spell(SpellSlot.R, 900);
            _r.SetSkillshot(0.25f, (float)(45 * 0.5), 1600, false, SkillshotType.SkillshotCone);
            Ignite = Player.GetSpellSlot("SummonerDot");
            Flash = Player.GetSpellSlot("SummonerFlash");
        }
    }
}