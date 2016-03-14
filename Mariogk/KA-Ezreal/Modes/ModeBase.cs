using EloBuddy.SDK;

namespace KA_Ezreal.Modes
{
    public abstract class ModeBase
    {
        protected static Spell.Skillshot Q
        {
            get { return SpellManager.Q; }
        }
        protected Spell.Skillshot W
        {
            get { return SpellManager.W; }
        }
        protected static Spell.Skillshot E
        {
            get { return SpellManager.E; }
        }
        protected static Spell.Skillshot R
        {
            get { return SpellManager.R; }
        }

        public abstract bool ShouldBeExecuted();

        public abstract void Execute();
    }
}
