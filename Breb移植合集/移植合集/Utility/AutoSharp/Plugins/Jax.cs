using System; using EloBuddy; using EloBuddy.SDK.Menu; using EloBuddy.SDK; using EloBuddy.SDK.Menu.Values;
using AutoSharp.Utils;
using LeagueSharp;
using LeagueSharp.Common;

namespace AutoSharp.Plugins
{
    public class Jax : PluginBase
    {
        public Jax()
        {
            //spelldata from Mechanics-StackOverflow Galio
            Q = new LeagueSharp.Common.Spell(SpellSlot.Q, 680f);
            W = new LeagueSharp.Common.Spell(SpellSlot.W);
            E = new LeagueSharp.Common.Spell(SpellSlot.E, 190f);
            R = new LeagueSharp.Common.Spell(SpellSlot.R);

            Q.SetTargetted(0.50f, 75f);
        }

        public override void OnAfterAttack(AttackableUnit target, EventArgs args)
        {
            var t = target as AIHeroClient;
            if (t != null)
            {
                if (W.IsReady())
                {
                    W.Cast();
                }
            }
        }

        public override void OnUpdate(EventArgs args)
        {
            if (ComboMode)
            {
                if (Q.IsReady() && Heroes.Player.LSDistance(Target) < Q.Range)
                {
                    if (E.IsReady())
                    {
                        E.Cast();
                    }
                    if (R.IsReady())
                    {
                        R.Cast();
                    }
                    Q.Cast(Target);
                }
                if (E.IsReady() && Target.LSIsValidTarget(E.Range))
                {
                    E.Cast();
                }
            }
        }

        public override void ComboMenu(Menu config)
        {
            config.AddBool("ComboQ", "Use Q", true);
            config.AddBool("ComboW", "Use W", true);
            config.AddBool("ComboE", "Use E", true);
            config.AddBool("ComboR", "Use R", true);
        }
    }
}