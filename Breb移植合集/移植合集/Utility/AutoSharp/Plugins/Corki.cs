using System; using EloBuddy; using EloBuddy.SDK.Menu; using EloBuddy.SDK; using EloBuddy.SDK.Menu.Values;
using AutoSharp.Utils;
using LeagueSharp;
using LeagueSharp.Common;

namespace AutoSharp.Plugins
{
    public class Corki : PluginBase
    {
        public Corki()
        {
            Q = new LeagueSharp.Common.Spell(SpellSlot.Q, 825f);
            E = new LeagueSharp.Common.Spell(SpellSlot.E, 600f);
            R = new LeagueSharp.Common.Spell(SpellSlot.R, 1300f);

            Q.SetSkillshot(0.3f, 120f, 1000f, false, SkillshotType.SkillshotCircle);
            E.SetSkillshot(0f, (float) (45 * Math.PI / 180), 1500, false, SkillshotType.SkillshotCone);
            R.SetSkillshot(0.2f, 40f, 2000f, true, SkillshotType.SkillshotLine);
        }

        public override void OnUpdate(EventArgs args)
        {
            if (ComboMode)
            {
                if (Q.IsReady() && Heroes.Player.LSDistance(Target) < Q.Range)
                {
                    Q.Cast(Target);
                }
                if (E.IsReady() && Heroes.Player.LSDistance(Target) < E.Range)
                {
                    E.Cast(Target);
                }

                if (R.IsReady() && Heroes.Player.LSDistance(Target) < R.Range)
                {
                    R.Cast(Target);
                }
            }
        }

        public override void ComboMenu(Menu config)
        {
            config.AddBool("ComboQ", "Use Q", true);
            config.AddBool("ComboE", "Use E", true);
            config.AddBool("ComboR", "Use R", true);
        }

        public override void HarassMenu(Menu config)
        {
            config.AddBool("HarassQ", "Use Q", true);
            config.AddBool("HarassE", "Use E", true);
            config.AddBool("HarassR", "Use R", true);
        }
    }
}