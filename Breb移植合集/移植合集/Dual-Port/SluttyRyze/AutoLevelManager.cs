using EloBuddy;
using LeagueSharp;

namespace Slutty_ryze
{
    class AutoLevelManager
    {
        #region Structures
        struct Abilitys // So you can refeer to spell to level by slot rather than 1,2,3,4
        {
            public const int Q = 1;
            public const int W = 2;
            public const int E = 3;
            public const int R = 4;
        }
        #endregion
        #region Variable Declaration

        //  private static readonly int[] _abilitySequence = new[] { 1, 2, 3, 1, 1, 4, 1, 2, 1, 2, 4, 3, 2, 2, 3, 4, 3, 3 };
        
        private static readonly int[] AbilitySequence ={
            Abilitys.Q, Abilitys.W, Abilitys.E, Abilitys.Q, Abilitys.Q, Abilitys.R, Abilitys.Q, Abilitys.W, Abilitys.Q,
            Abilitys.W, Abilitys.R, Abilitys.E, Abilitys.W, Abilitys.W, Abilitys.E, Abilitys.R, Abilitys.E, Abilitys.E
        };

        // What used for?
        private static int QOff = 0;
        private static int WOff = 0;
        private static int EOff = 0;
        private static int ROff = 0;
        #endregion
        #region Public Functions
        public static void LevelUpSpells()
        {
            var qL = GlobalManager.GetHero.Spellbook.GetSpell(Champion.Q.Slot).Level + QOff;
            var wL = GlobalManager.GetHero.Spellbook.GetSpell(Champion.W.Slot).Level + WOff;
            var eL = GlobalManager.GetHero.Spellbook.GetSpell(Champion.E.Slot).Level + EOff;
            var rL = GlobalManager.GetHero.Spellbook.GetSpell(Champion.R.Slot).Level + ROff;

            if (qL + wL + eL + rL >= GlobalManager.GetHero.Level) return;

            int[] level = { 0, 0, 0, 0 };

            for (var i = 0; i < GlobalManager.GetHero.Level; i++)
            {
                level[AbilitySequence[i] - 1] = level[AbilitySequence[i] - 1] + 1;
            }

            if (qL < level[0]) GlobalManager.GetHero.Spellbook.LevelSpell(SpellSlot.Q);
            if (wL < level[1]) GlobalManager.GetHero.Spellbook.LevelSpell(SpellSlot.W);
            if (eL < level[2]) GlobalManager.GetHero.Spellbook.LevelSpell(SpellSlot.E);
            if (rL < level[3]) GlobalManager.GetHero.Spellbook.LevelSpell(SpellSlot.R);
        }
        #endregion
    }
}
