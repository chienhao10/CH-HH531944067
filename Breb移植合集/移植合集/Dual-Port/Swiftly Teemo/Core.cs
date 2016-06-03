#region

using EloBuddy;
using LeagueSharp.SDK;
using EloBuddy.SDK;
using Spell = LeagueSharp.SDK.Spell;

#endregion

namespace Swiftly_Teemo
{
    internal class Core
    {
        public static AIHeroClient Target => TargetSelector.GetTarget(Spells.Q.Range, DamageType.Physical);
        public static AIHeroClient Player => ObjectManager.Player;
        public class Spells
        {
            public static SpellSlot Ignite;
            public static Spell Q { get; set; }
            public static Spell W { get; set; }
            public static Spell E { get; set; }
            public static Spell R { get; set; }
            public static void Load()
            {
                Q = new Spell(SpellSlot.Q, 680);
                W = new Spell(SpellSlot.W);
                E = new Spell(SpellSlot.E);
                R = new Spell(SpellSlot.R, 300);

                Q.SetTargetted(0.5f, 1500f);
                R.SetSkillshot(0.5f, 120f, 1000f, false, SkillshotType.SkillshotCircle);

                Ignite = Player.GetSpellSlot("SummonerDot");
            }
        }
    }
}
