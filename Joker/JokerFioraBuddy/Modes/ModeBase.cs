using EloBuddy.SDK;

namespace JokerFioraBuddy.Modes
{
    public abstract class ModeBase
    {
        protected Spell.Skillshot Q
        {
            get { return SpellManager.Q; }
        }

        protected Spell.Skillshot W
        {
            get { return SpellManager.W; }
        }

        protected Spell.Active E
        {
            get { return SpellManager.E; }
        }

        protected Spell.Targeted R
        {
            get { return SpellManager.R; }
        }

        protected Spell.Targeted IG
        {
            get { return SpellManager.IG; }
        }

        public abstract bool ShouldBeExecuted();

        public abstract void Execute();
    }
}
