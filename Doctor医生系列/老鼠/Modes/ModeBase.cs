using EloBuddy;
using EloBuddy.SDK;
using SharpDX;

namespace Twitch.Modes
{
    public abstract class ModeBase
    {
        protected Spell.Active Q
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
        protected Spell.Active R
        {
            get { return SpellManager.R; }
        }
        protected Spell.Targeted Ignite
        {
            get { return SpellManager.Ignite; }
        }

        protected float PlayerHealth
        {
            get { return Player.Instance.HealthPercent; }
        }

        protected float PlayerMana
        {
            get { return Player.Instance.ManaPercent; }
        }

        protected bool HasIgnite
        {
            get { return SpellManager.HasIgnite(); }
        }

        protected AIHeroClient _Player
        {
            get { return Player.Instance; }
        }

        protected Vector3 _PlayerPos
        {
            get { return Player.Instance.Position; }
        }
        protected bool RActive
        {
            get { return Player.Instance.HasBuff("TwitchFullAutomatic"); }
        }

        protected bool QActive
        {
            get { return Player.Instance.HasBuff("TwitchHideInShadows"); }
        }

        protected int EStacks(Obj_AI_Base obj)
        {
            return SpellManager.EStacks(obj);
        }

        public abstract bool ShouldBeExecuted();

        public abstract void Execute();
    }
}
