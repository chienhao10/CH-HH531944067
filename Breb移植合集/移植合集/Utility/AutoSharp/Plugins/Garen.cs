using System;
using EloBuddy;
using EloBuddy.SDK.Menu; using EloBuddy.SDK; using EloBuddy.SDK.Menu.Values;
using System.Linq;
using AutoSharp.Utils;
using LeagueSharp;
using LeagueSharp.Common;

namespace AutoSharp.Plugins
{
    public class Garen : PluginBase
    {
        public Garen()
        {
            Q = new LeagueSharp.Common.Spell(SpellSlot.Q);
            W = new LeagueSharp.Common.Spell(SpellSlot.W);
            E = new LeagueSharp.Common.Spell(SpellSlot.E, 165);
            R = new LeagueSharp.Common.Spell(SpellSlot.R, 400);
        }

        public override void OnAfterAttack(AttackableUnit target, EventArgs args)
        {
            var t = target as AIHeroClient;
            if (t != null)
            {
                if (Q.IsReady())
                {
                    Q.Cast();
                    Orbwalker.ResetAutoAttack();
                }
            }
        }

        public override void OnUpdate(EventArgs args)
        {
            KS();
            if (ComboMode)
            {
                if (Q.IsReady() && Player.LSCountEnemiesInRange(R.Range) > 0)
                {
                    Q.Cast();
                    Orbwalker.ResetAutoAttack();


                }
                if (W.IsReady() && Player.LSCountEnemiesInRange(R.Range) > 0)
                {
                    W.Cast();
                }

                if (E.IsReady() && Player.LSCountEnemiesInRange(R.Range) > 0)
                {
                    E.Cast();

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
            config.AddBool("ComboRKS", "Use R KS", true);
        }
    }
}