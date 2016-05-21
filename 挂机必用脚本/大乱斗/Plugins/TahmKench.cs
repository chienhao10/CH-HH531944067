using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;

namespace AutoSharp.Plugins
{
    public class TahmKench : PluginBase
    {
        public TahmKench()
        {
            Q = new Spell(SpellSlot.Q, 800);
            Q.SetSkillshot(.1f, 75, 2000, true, SkillshotType.SkillshotLine);
            W = new Spell(SpellSlot.W, 250);
            E = new Spell(SpellSlot.E);
            R = new Spell(SpellSlot.R, 4000);
        }

        public override void OnUpdate(EventArgs args)
        {
            if (Q.IsReady())
            {
                var target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);
                Q.CastIfHitchanceEquals(target, HitChance.VeryHigh);
            }
            if (ObjectManager.Player.HealthPercent < 10 && E.IsReady())
            {
                E.Cast();
            }
        }
    }
}
