using System; using EloBuddy; using EloBuddy.SDK.Menu; using EloBuddy.SDK; using EloBuddy.SDK.Menu.Values;
using System.Linq;
using AutoSharp.Utils;
using LeagueSharp;
using LeagueSharp.Common;

namespace AutoSharp.Plugins
{
    public class Tristana : PluginBase
    {
        public Tristana()
        {
            Q = new LeagueSharp.Common.Spell(SpellSlot.Q, 703);
            W = new LeagueSharp.Common.Spell(SpellSlot.W, 900);
            E = new LeagueSharp.Common.Spell(SpellSlot.E, 703);
            R = new LeagueSharp.Common.Spell(SpellSlot.R, 703);

            W.SetSkillshot(500, 270, 1500, false, SkillshotType.SkillshotCone);
        }

        public override void OnUpdate(EventArgs args)
        {
            if (ComboMode)
            {
                Ks();
                if (Q.IsReady() && Heroes.Player.LSDistance(Target) < Q.Range && Orbwalking.InAutoAttackRange(Target))
                {
                    Q.Cast();
                }
                if (E.IsReady() && Heroes.Player.LSDistance(Target) < E.Range)
                {
                    E.Cast(Target);
                }
            }
        }

        public void Ks()
        {
            foreach (
                var target in
                    ObjectManager.Get<AIHeroClient>()
                        .Where(x => Player.LSDistance(x) < R.Range && x.LSIsValidTarget() && x.IsEnemy && !x.IsDead))
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

                    if (W.IsReady() && Heroes.Player.LSDistance(Target) < W.Range && W.IsKillable(target))
                    {
                        W.Cast(Target);
                        return;
                    }
                }
            }
        }

        public override void OnPossibleToInterrupt(AIHeroClient unit, Interrupter2.InterruptableTargetEventArgs spell)
        {
            if (spell.DangerLevel < Interrupter2.DangerLevel.High || unit.IsAlly)
            {
                return;
            }

            if (R.CastCheck(unit, "Interrupt.R"))
            {
                R.Cast(unit);
            }
        }

        public override void ComboMenu(Menu config)
        {
            config.AddBool("ComboQ", "Use Q", true);
            config.AddBool("ComboW", "Use W", true);
            config.AddBool("ComboE", "Use E", true);
            config.AddBool("ComboRKS", "Use R KS", true);
        }

        public override void InterruptMenu(Menu config)
        {
            config.AddBool("Interrupt.R", "Use R to Interrupt Spells", true);
        }
    }
}