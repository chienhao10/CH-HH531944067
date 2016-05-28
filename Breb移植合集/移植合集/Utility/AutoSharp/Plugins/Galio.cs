using System; using EloBuddy; using EloBuddy.SDK.Menu; using EloBuddy.SDK; using EloBuddy.SDK.Menu.Values;
using AutoSharp.Utils;
using LeagueSharp;
using LeagueSharp.Common;

namespace AutoSharp.Plugins
{
    public class Galio : PluginBase
    {
        public Galio()
        {
            //spelldata from Mechanics-StackOverflow Galio
            Q = new LeagueSharp.Common.Spell(SpellSlot.Q, 940);
            W = new LeagueSharp.Common.Spell(SpellSlot.W, 800);
            E = new LeagueSharp.Common.Spell(SpellSlot.E, 1180);
            R = new LeagueSharp.Common.Spell(SpellSlot.R, 570); //Decreased range on purpose

            Q.SetSkillshot(0.25f, 150, 1250, false, SkillshotType.SkillshotCircle);
            E.SetSkillshot(0.25f, 90, 1250, false, SkillshotType.SkillshotLine);
        }

        public override void OnUpdate(EventArgs args)
        {
            if (ComboMode)
            {
                if (Q.IsReady() && Heroes.Player.LSDistance(Target) < Q.Range)
                {
                    Q.CastIfHitchanceEquals(Target, HitChance.Medium, UsePackets);
                }
                if (E.IsReady() && Heroes.Player.LSDistance(Target) < E.Range)
                {
                    E.CastIfHitchanceEquals(Target, HitChance.Medium);
                }
                if (R.IsReady())
                {
                    R.CastIfWillHit(Target, 2);
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