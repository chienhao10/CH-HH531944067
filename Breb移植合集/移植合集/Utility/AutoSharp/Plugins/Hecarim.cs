using System; using EloBuddy; using EloBuddy.SDK.Menu; using EloBuddy.SDK; using EloBuddy.SDK.Menu.Values;
using AutoSharp.Utils;
using LeagueSharp;
using LeagueSharp.Common;

namespace AutoSharp.Plugins
{
    public class Hecarim : PluginBase
    {

        public Hecarim()
        {
            Q = new LeagueSharp.Common.Spell(SpellSlot.Q, 350);
            W = new LeagueSharp.Common.Spell(SpellSlot.W, 525);
            E = new LeagueSharp.Common.Spell(SpellSlot.E, 0);
            R = new LeagueSharp.Common.Spell(SpellSlot.R, 1350);

            R.SetSkillshot(0.5f, 200f, 1200f, false, SkillshotType.SkillshotLine);
        }
        public override void OnUpdate(EventArgs args)
        {

            if (ComboMode)
            {
                if (Q.IsReady() && Target.LSIsValidTarget(Q.Range))
                {
                    Q.Cast();
                }
                if (W.IsReady() && Target.LSIsValidTarget(W.Range))
                {
                    W.Cast();
                }
                if (R.IsReady() && Heroes.Player.LSDistance(Target) < R.Range)
                {
                    R.Cast(Target,UsePackets);
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
