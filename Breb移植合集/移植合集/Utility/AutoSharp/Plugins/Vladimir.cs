using System; using EloBuddy; using EloBuddy.SDK.Menu; using EloBuddy.SDK; using EloBuddy.SDK.Menu.Values;
using AutoSharp.Utils;
using LeagueSharp;
using LeagueSharp.Common;

namespace AutoSharp.Plugins
{
    public class Vladimir : PluginBase
    {
        public Vladimir()
        {

            Q = new LeagueSharp.Common.Spell(SpellSlot.Q, 600);
            E = new LeagueSharp.Common.Spell(SpellSlot.E, 610);
            R = new LeagueSharp.Common.Spell(SpellSlot.R, 700);
            R.SetSkillshot(0.25f, 175, 700, false, SkillshotType.SkillshotCircle);

        }


        public override void OnUpdate(EventArgs args)
        {
            if (ComboMode)
            {

                if (E.IsReady() && Heroes.Player.LSDistance(Target) < E.Range)
                {
                    E.Cast(Target);
                }
                if (Q.IsReady() && Heroes.Player.LSDistance(Target) < Q.Range)
                {
                    Q.Cast(Target);
                }
                if (R.IsReady() && Heroes.Player.LSDistance(Target) < R.Range)
                {
                    if (R.IsKillable(Target))
                    {
                        R.Cast(Target);
                    }
                    else 
                    { 
                        R.CastIfWillHit(Target, 2); 
                    }

                }
                if (W.IsReady() && Heroes.Player.LSDistance(Target) < W.Range && Player.HealthPercent <=20 )
                {
                    W.Cast();
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


