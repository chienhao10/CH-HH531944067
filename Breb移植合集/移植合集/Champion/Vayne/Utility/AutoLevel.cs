using EloBuddy;
using EloBuddy.SDK;
using LS = LeagueSharp.Common;
using SV = SoloVayne.Skills.Tumble;

namespace Vayne1
{
    //github.com/dakotasblack
    internal class AutoLevel
    {
        public static int QOff = 0, WOff = 0, EOff = 0, ROff = 0;

        public static int[] Level = {0, 0, 0, 0};

        public static int[] AbilitySequence;

        public static AIHeroClient myHero
        {
            get { return Player.Instance; }
        }

        public static void _AutoSpell()
        {
            AbilitySequence = new[] {1, 2, 3, 2, 2, 4, 1, 1, 1, 1, 4, 2, 2, 3, 3, 4, 3, 3};
            var qL = Program.Q.Level + QOff;
            var wL = Program.W.Level + WOff;
            var eL = Program.E.Level + EOff;
            var rL = Program.R.Level + ROff;

            Level = new[] {0, 0, 0, 0};

            for (var i = 1; i <= ObjectManager.Player.Level; i++)
            {
                switch (AbilitySequence[i - 1])
                {
                    case 1:
                        Level[0] += 1;
                        break;
                    case 2:
                        Level[1] += 1;
                        break;
                    case 3:
                        Level[2] += 1;
                        break;
                    case 4:
                        Level[3] += 1;
                        break;
                }
            }

            if (qL < Level[0])
            {
                LevelUp(SpellSlot.Q);
            }

            if (wL < Level[1])
            {
                LevelUp(SpellSlot.W);
            }

            if (eL < Level[2])
            {
                LevelUp(SpellSlot.E);
            }

            if (rL < Level[3])
            {
                LevelUp(SpellSlot.R);
            }
        }

        public static void LevelUp(SpellSlot slot)
        {
            Core.DelayAction(() => { myHero.Spellbook.LevelSpell(slot); }, 150);
        }
    }
}