using System; using EloBuddy; using EloBuddy.SDK.Menu; using EloBuddy.SDK; using EloBuddy.SDK.Menu.Values;
using System.Linq;
using AutoSharp.Utils;
using LeagueSharp;
using LeagueSharp.Common;


namespace AutoSharp.Plugins
{
    public class Akali : PluginBase
    {
        public Akali()
        {
            Q = new LeagueSharp.Common.Spell(SpellSlot.Q, 600);

            W = new LeagueSharp.Common.Spell(SpellSlot.W, 700);

            E = new LeagueSharp.Common.Spell(SpellSlot.E, 325);

            R = new LeagueSharp.Common.Spell(SpellSlot.R, 800);
        }

        public override void OnUpdate(EventArgs args)
        {
            KS();
            if (ComboMode)
            {
                if (Q.IsReady() && Heroes.Player.LSDistance(Target) < Q.Range)
                {
                    Q.Cast(Target);
                }
                if (W.IsReady() && (Player.HealthPercent < 20 || (!Q.IsReady() && !E.IsReady() && !R.IsReady())))
                {
                    W.Cast(Player.Position);
                }
                if (E.IsReady() && Heroes.Player.LSDistance(Target) < E.Range)
                {
                    E.Cast(Target);
                }
                if (R.CastCheck(Target, "ComboRKS"))
                {
                    R.Cast(Target);
                }
            }
        }

        // ReSharper disable once InconsistentNaming
        public void KS()
        {
            foreach (
                var target in
                    ObjectManager.Get<AIHeroClient>()
                        .Where(x => Player.LSDistance(x) < 900 && x.LSIsValidTarget() && x.IsEnemy && !x.IsDead))
            {
                // ReSharper disable once ConditionIsAlwaysTrueOrFalse
                if (target != null)
                {
                    //R
                    if (Player.LSDistance(target.ServerPosition) <= R.Range &&
                        (Player.LSGetSpellDamage(target, SpellSlot.R)) > target.Health + 50)
                    {
                        if (R.CastCheck(Target, "ComboRKS"))
                        {
                            R.Cast(target);
                            return;
                        }
                    }
                }
            }
        }

        public override void ComboMenu(Menu config)
        {
            config.AddBool("ComboQ", "Use Q", true);
            config.AddBool("ComboW", "Use W", true);
            config.AddBool("ComboE", "Use E", true);
            config.AddBool("ComboRKS", "Use R", true);
        }
    }
}