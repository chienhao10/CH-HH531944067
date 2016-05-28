using System; using EloBuddy; using EloBuddy.SDK.Menu; using EloBuddy.SDK; using EloBuddy.SDK.Menu.Values;
using AutoSharp.Utils;
using LeagueSharp;
using LeagueSharp.Common;

namespace AutoSharp.Plugins
{
    public class Evelynn : PluginBase
    {
        public Evelynn()
        {
            Q = new LeagueSharp.Common.Spell(SpellSlot.Q, 500f);
            W = new LeagueSharp.Common.Spell(SpellSlot.W, Q.Range);
            E = new LeagueSharp.Common.Spell(SpellSlot.E, 225f + 2 * 65f);
            R = new LeagueSharp.Common.Spell(SpellSlot.R, 650f);

            R.SetSkillshot(0.25f, 350f, float.MaxValue, false, SkillshotType.SkillshotCircle);
        }

        public override void OnUpdate(EventArgs args)
        {
            if (ComboMode)
            {
                if (Player.LSCountEnemiesInRange(Q.Range) > 0)
                {
                    Q.Cast();
                }
                if (W.IsReady() && ObjectManager.Player.HasBuffOfType(BuffType.Slow))
                {
                    W.Cast();
                }

                if (E.IsReady() && Heroes.Player.LSDistance(Target) < E.Range)
                {
                    E.Cast(Target);
                }
                if (R.IsReady())
                {
                    R.CastIfWillHit(Target, 1);
                }
				if ( Q.IsReady())
				{
					Q.Cast();
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