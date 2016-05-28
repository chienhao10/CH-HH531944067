using System;
using EloBuddy;
using EloBuddy.SDK.Menu; using EloBuddy.SDK; using EloBuddy.SDK.Menu.Values;
using AutoSharp.Utils;
using LeagueSharp;
using LeagueSharp.Common;

namespace AutoSharp.Plugins
{
    public class Caitlyn : PluginBase
    {
        public Caitlyn()
        {
            Q = new LeagueSharp.Common.Spell(SpellSlot.Q, 1240);
            W = new LeagueSharp.Common.Spell(SpellSlot.W, 820);
            E = new LeagueSharp.Common.Spell(SpellSlot.E, 800);
            R = new LeagueSharp.Common.Spell(SpellSlot.R, 2000);

            Q.SetSkillshot(0.25f, 60f, 2000f, false, SkillshotType.SkillshotLine);
            E.SetSkillshot(0.25f, 80f, 1600f, true, SkillshotType.SkillshotLine);
        }

        public override void OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (E.IsReady() && gapcloser.Sender.LSIsValidTarget(E.Range))
            {
                E.Cast(gapcloser.Sender);
            }
        }

        public override void OnUpdate(EventArgs args)
        {
            R.Range = 500 * R.Level + 1500;
            AIHeroClient t;

            if (ComboMode)
            {
                //Auto W (Stun/Snare/Taunt)
                if (W.IsReady())
                {
                    t = TargetSelector.GetTarget(W.Range, DamageType.Physical);
                    if (t.LSIsValidTarget(W.Range) &&
                        (t.HasBuffOfType(BuffType.Stun) || t.HasBuffOfType(BuffType.Snare) ||
                         t.HasBuffOfType(BuffType.Taunt) || t.HasBuff("zhonyasringshield") || t.HasBuff("Recall")))
                    {
                        W.Cast(t.Position);
                    }
                }

                //Auto Q (Stun/Snare/Taunt/Slow)
                if (Q.IsReady())
                {
                    t = TargetSelector.GetTarget(Q.Range, DamageType.Physical);
                    if (t.LSIsValidTarget(Q.Range) &&
                        (t.HasBuffOfType(BuffType.Stun) || t.HasBuffOfType(BuffType.Snare) ||
                         t.HasBuffOfType(BuffType.Taunt) || t.HasBuffOfType(BuffType.Slow)))
                    {
                        Q.Cast(t, false, true);
                    }
                }

                if (R.IsReady())
                {
                    t = TargetSelector.GetTarget(R.Range, DamageType.Physical);
                    if (t != null && t.Health <= R.GetDamage(t) && !Orbwalking.InAutoAttackRange(t))
                    {
                        R.Cast(t);
                    }
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