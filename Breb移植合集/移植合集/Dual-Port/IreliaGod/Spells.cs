using EloBuddy;
using LeagueSharp;
using LeagueSharp.Common;

namespace IreliaGod
{
    class Spells
    {
        public static Spell Q { get; private set; }
        public static Spell W { get; private set; }
        public static Spell E { get; private set; }
        public static Spell R { get; private set; }
        public static SpellSlot Ignite { get; private set; }
        public static Items.Item Youmuu { get; private set; }
        public static Items.Item Cutlass { get; private set; }
        public static Items.Item Blade { get; private set; }
        public static Items.Item Tiamat { get; private set; }
        public static Items.Item Hydra { get; private set; }


        public static void Initialize()
        {
            Q = new Spell(SpellSlot.Q, 650);
            W = new Spell(SpellSlot.W);
            E = new Spell(SpellSlot.E, 350);
            R = new Spell(SpellSlot.R, 1000);

            Q.SetTargetted(0f, 2200);
            R.SetSkillshot(0.5f, 120, 1600, false, SkillshotType.SkillshotLine);

            Ignite = ObjectManager.Player.GetSpellSlot("summonerdot");
            Youmuu = new Items.Item(3142);
            Cutlass = new Items.Item(3144, 450f);
            Blade = new Items.Item(3153, 450f);
            Tiamat = new Items.Item(3077, 400f);
            Hydra = new Items.Item(3074, 400f);
        }
    }
}
