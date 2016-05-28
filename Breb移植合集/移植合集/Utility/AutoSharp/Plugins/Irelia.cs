using System; using EloBuddy; using EloBuddy.SDK.Menu; using EloBuddy.SDK; using EloBuddy.SDK.Menu.Values;
using AutoSharp.Utils;
using LeagueSharp;
using LeagueSharp.Common;

namespace AutoSharp.Plugins
{
    public class Irelia : PluginBase
    {
        public Irelia()
        {
            Q = new LeagueSharp.Common.Spell(SpellSlot.Q, 650);
            W = new LeagueSharp.Common.Spell(SpellSlot.W, Orbwalking.GetRealAutoAttackRange(ObjectManager.Player));
            E = new LeagueSharp.Common.Spell(SpellSlot.E, 425);
            R = new LeagueSharp.Common.Spell(SpellSlot.R, 1000);
        }

        public override void OnUpdate(EventArgs args)
        {
            if (ComboMode)
            {
                Combo(Target);
            }
        }

        private void Combo(AIHeroClient target)
        {
            if (target == null || !target.IsValid)
            {
                return;
            }

            if (Q.IsReady() && Player.LSDistance(target) < Q.Range)
            {
                if (W.IsReady())
                {
                    W.Cast();
                }
                Q.Cast(target);
            }

            if (E.IsReady())
            {
                E.Cast(target);
            }
            if (R.IsReady() && Player.LSDistance(target) < Q.Range)
            {
                R.Cast(Target.Position);
            }
        }

        public override void ComboMenu(Menu config)
        {
            config.AddBool("ComboQ", "Use Q", true);
            config.AddBool("ComboW", "Use W", true);
            config.AddBool("ComboE", "Use E", true);
            config.AddBool("ComboR", "Use R1", true);
        }
    }
}