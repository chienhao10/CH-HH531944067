using System;
using LeagueSharp;
using EloBuddy;

namespace SCommon.Orbwalking
{
    public static class Events
    {
        /// <summary>
        /// Delegate for Before Attack Event
        /// </summary>
        /// <param name="args">The <see cref="BeforeAttackArgs"/> instance that contains event args<param>
        public delegate void dBeforeAttackEvent(BeforeAttackArgs args);

        /// <summary>
        /// Delegate for After Attack Event
        /// </summary>
        /// <param name="args">The <see cref="AfterAttackArgs"/> instance that contains event args</param>
        public delegate void dAfterAttackEvent(AfterAttackArgs args);

        /// <summary>
        /// Delegate for On Attack Event
        /// </summary>
        /// <param name="args">The <see cref="OnAttackArgs"/> instance that contains event args</param>
        public delegate void dOnAttackEvent(OnAttackArgs args);

        /// <summary>
        /// The event fired before aa
        /// </summary>
        public static event dBeforeAttackEvent BeforeAttack;

        /// <summary>
        /// The event fired after aa
        /// </summary>
        public static event dAfterAttackEvent AfterAttack;

        /// <summary>
        /// The event fired aa windup started
        /// </summary>
        public static event dOnAttackEvent OnAttack;

        /// <summary>
        /// Invokes before attack event
        /// </summary>
        /// <param name="instance">Orbwalker instance</param>
        /// <param name="target">Target</param>
        /// <returns></returns>
        public static BeforeAttackArgs FireBeforeAttack(Orbwalker instance, AttackableUnit target)
        {
            BeforeAttackArgs args = new BeforeAttackArgs();
            args.Instance = instance;
            args.Target = target;

            if (BeforeAttack != null)
                BeforeAttack(args);

            return args;
        }

        /// <summary>
        /// Invokes after attack event
        /// </summary>
        /// <param name="instance">Orbwalker instance</param>
        /// <param name="target">Target</param>
        /// <returns></returns>
        public static AfterAttackArgs FireAfterAttack(Orbwalker instance, AttackableUnit target)
        {
            if (instance != null)
                instance.ForcedTarget = null;

            AfterAttackArgs args = new AfterAttackArgs();
            args.Instance = instance;
            args.Target = target;

            if (AfterAttack != null)
                AfterAttack(args);

            if (args.ResetAATimer)
                instance.ResetAATimer();

            return args;
        }

        /// <summary>
        /// Invokes on attack event
        /// </summary>
        /// <param name="instance">Orbwalker instance</param>
        /// <param name="target">Target</param>
        /// <returns></returns>
        public static OnAttackArgs FireOnAttack(Orbwalker instance, AttackableUnit target)
        {
            OnAttackArgs args = new OnAttackArgs();
            args.Instance = instance;
            args.Target = target;

            if (OnAttack != null)
                OnAttack(args);

            if (args.Cancel)
                EloBuddy.Player.IssueOrder(GameObjectOrder.Stop, ObjectManager.Player);

            return args;
        }
    }

    /// <summary>
    /// Before Attack Event Args
    /// </summary>
    public class BeforeAttackArgs : EventArgs
    {
        /// <summary>
        /// Orbwalking Instance
        /// </summary>
        public Orbwalker Instance;

        /// <summary>
        /// Target to attack in next aa
        /// </summary>
        public AttackableUnit Target;

        /// <summary>
        /// Process next aa if true
        /// </summary>
        public bool Process = true;
    }

    /// <summary>
    /// After Attack Event Args
    /// </summary>
    public class AfterAttackArgs : EventArgs
    {
        /// <summary>
        /// Orbwalking Instance
        /// </summary>
        public Orbwalker Instance;

        /// <summary>
        /// The target which attacked last
        /// </summary>
        public AttackableUnit Target;

        /// <summary>
        /// Call reset aa timer in next update
        /// </summary>
        public bool ResetAATimer = false;
    }

    /// <summary>
    /// On Attack Event Args
    /// </summary>
    public class OnAttackArgs : EventArgs
    {
        /// <summary>
        /// Orbwalking Instance
        /// </summary>
        public Orbwalker Instance;

        /// <summary>
        /// The target which currently attacking
        /// </summary>
        public AttackableUnit Target;

        /// <summary>
        /// Cancel winduping aa
        /// </summary>
        public bool Cancel = false;
    }
}