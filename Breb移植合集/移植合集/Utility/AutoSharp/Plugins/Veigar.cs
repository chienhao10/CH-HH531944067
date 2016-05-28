using System; using EloBuddy; using EloBuddy.SDK.Menu; using EloBuddy.SDK; using EloBuddy.SDK.Menu.Values;
using System.Linq;
using AutoSharp.Utils;
using LeagueSharp;
using LeagueSharp.Common;

namespace AutoSharp.Plugins
{
    public class Veigar : PluginBase
    {
        public Veigar()
        {
            Q = new LeagueSharp.Common.Spell(SpellSlot.Q, 650);
            W = new LeagueSharp.Common.Spell(SpellSlot.W, 900);
            E = new LeagueSharp.Common.Spell(SpellSlot.E, 1005);
            R = new LeagueSharp.Common.Spell(SpellSlot.R, 650);
            W.SetSkillshot(1.25f, 230f, float.MaxValue, false, SkillshotType.SkillshotCircle);
            E.SetSkillshot(.2f, 330f, float.MaxValue, false, SkillshotType.SkillshotCircle);
        }

        public override void OnUpdate(EventArgs args)
        {
            KS();
            if (ComboMode)
            {
                if (E.IsReady() && Heroes.Player.LSDistance(Target) < E.Range)
                {
                    E.Cast(Target, true);
                }
                if (Q.IsReady() && Heroes.Player.LSDistance(Target) < Q.Range)
                {
                    Q.Cast(Target, true);
                }
                if (W.IsReady() && Heroes.Player.LSDistance(Target) < W.Range)
                {
                    W.Cast(Target.Position, true);
                }
            }

            if (HarassMode)
            {
                if (Q.CastCheck(Target, "HarassQ"))
                {
                    Q.Cast(Target, true);
                }
                if (W.CastCheck(Target, "HarassW"))
                {
                    W.Cast(Target, true);
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

        public override void OnPossibleToInterrupt(AIHeroClient unit, Interrupter2.InterruptableTargetEventArgs spell)
        {
            if (spell.DangerLevel < Interrupter2.DangerLevel.High || unit.IsAlly)
            {
                return;
            }

            if (E.CastCheck(unit, "Interrupt.E"))
            {
                E.Cast(unit, true);
            }
        }

        public override void ComboMenu(Menu config)
        {
            config.AddBool("ComboQ", "Use Q", true);
            config.AddBool("ComboW", "Use W", true);
            config.AddBool("ComboE", "Use E", true);
            config.AddBool("ComboRKS", "Use R KS", true);
        }

        public override void HarassMenu(Menu config)
        {
            config.AddBool("HarassQ", "Use Q", true);
            config.AddBool("HarassW", "Use W", true);
        }

        public override void InterruptMenu(Menu config)
        {
            config.AddBool("Interrupt.E", "Use E to Interrupt Spells", true);
        }
    }
}