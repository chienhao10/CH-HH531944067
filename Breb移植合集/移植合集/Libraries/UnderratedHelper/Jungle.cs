using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using SharpDX;
using Spell = LeagueSharp.Common.Spell;
using LeagueSharp.Common;

namespace UnderratedAIO.Helpers
{
    public class Jungle
    {
        public static AIHeroClient player = ObjectManager.Player;

        private static readonly string[] jungleMonsters =
        {
            "TT_Spiderboss", "SRU_Blue", "SRU_Red", "SRU_Dragon",
            "SRU_Baron"
        };

        public static readonly string[] bosses = {"TT_Spiderboss", "SRU_Dragon", "SRU_Baron"};
        public static SpellSlot smiteSlot = SpellSlot.Unknown;
        public static Spell smite;

        public static Obj_AI_Minion GetNearest(Vector3 pos, float range = 1500f)
        {
            return
                ObjectManager.Get<Obj_AI_Minion>()
                    .FirstOrDefault(
                        minion =>
                            minion.IsValidTarget() && minion.IsValid && minion.LSDistance(pos) < range &&
                            jungleMonsters.Any(name => minion.Name.StartsWith(name)) && !minion.Name.Contains("Mini") &&
                            !minion.Name.Contains("Spawn"));
        }
    }
}