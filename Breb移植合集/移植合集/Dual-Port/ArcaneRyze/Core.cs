#region

using System.Linq;
using EloBuddy;
using LeagueSharp.SDK;
using LeagueSharp.SDK.Core;
using Spell = LeagueSharp.SDK.Spell;
using EloBuddy.SDK;
#endregion

namespace Arcane_Ryze
{
    internal class Core
    {
        public static int PassiveStack // Thanks Hoes
        {
            get
            {
                var data = Player.Buffs.FirstOrDefault(b => b.DisplayName == "RyzePassiveStack");
                return data == null ? 0 : data.Count;
            }
        }
        public static bool RyzeR => Player.Buffs.Any(x => x.Name.ToLower().Contains("RyzeR"));
        public static AIHeroClient Target => TargetSelector.GetTarget(Player.AttackRange, DamageType.Magical);
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
                Q = new Spell(SpellSlot.Q, 865);
                W = new Spell(SpellSlot.W, 585);
                E = new Spell(SpellSlot.E, 585);
                R = new Spell(SpellSlot.R);

                if(RyzeR)
                {
                    Q.SetSkillshot(0.25f, 50f, 1700f, false, SkillshotType.SkillshotLine);
                }
                else
                {
                    Q.SetSkillshot(0.25f, 50f, 1700f, true, SkillshotType.SkillshotLine);
                }
                

                Ignite = Player.GetSpellSlot("SummonerDot");
            }
        }
    }
}
