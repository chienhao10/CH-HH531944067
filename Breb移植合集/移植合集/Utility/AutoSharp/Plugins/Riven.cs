using System; using EloBuddy; using EloBuddy.SDK.Menu; using EloBuddy.SDK; using EloBuddy.SDK.Menu.Values;
using AutoSharp.Utils;
using LeagueSharp;
using LeagueSharp.Common;

namespace AutoSharp.Plugins
{
    public class Riven : PluginBase
    {
        public bool RActive;

        public Riven()
        {
            E = new LeagueSharp.Common.Spell(SpellSlot.E, 390f);
            Q = new LeagueSharp.Common.Spell(SpellSlot.Q, 250f);
            W = new LeagueSharp.Common.Spell(SpellSlot.W, 150f);
            R = new LeagueSharp.Common.Spell(SpellSlot.R, 900f);
        }

        public override void OnAfterAttack(AttackableUnit target, EventArgs args)
        {
            var t = target as AIHeroClient;
            if (t != null)
            {
                if (Q.IsReady())
                {
                    Q.Cast();
                    if (R.IsReady() && !RActive)
                    {
                        R.Cast();
                        RActive = true;
                    }
                    EloBuddy.Player.IssueOrder(GameObjectOrder.AttackTo, t);
                }
            }
        }

        public override void OnUpdate(EventArgs args)
        {
            if (ComboMode)
            {
                if (W.IsReady() && Heroes.Player.LSDistance(Target) < W.Range)
                {
                    W.Cast(Target);
                }
                if (RActive && Player.LSDistance(Target) < R.Range && R.IsKillable(Target))
                {
                    R.Cast(Target);
                    RActive = false;
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