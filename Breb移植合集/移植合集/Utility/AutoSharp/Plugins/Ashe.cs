using System;
using EloBuddy;
using EloBuddy.SDK.Menu; using EloBuddy.SDK; using EloBuddy.SDK.Menu.Values;
using AutoSharp.Utils;
using LeagueSharp;
using LeagueSharp.Common;

namespace AutoSharp.Plugins
{
    public class Ashe : PluginBase
    {
        public Ashe()
        {
            Q = new LeagueSharp.Common.Spell(SpellSlot.Q);
            W = new LeagueSharp.Common.Spell(SpellSlot.W, 1200);
            E = new LeagueSharp.Common.Spell(SpellSlot.E, 2500);
            R = new LeagueSharp.Common.Spell(SpellSlot.R, 20000);


            W.SetSkillshot(250f, (float) (24.32f * Math.PI / 180), 902f, true, SkillshotType.SkillshotCone);
            E.SetSkillshot(377f, 299f, 1400f, false, SkillshotType.SkillshotLine);
            R.SetSkillshot(250f, 130f, 1600f, false, SkillshotType.SkillshotLine);
        }

        public bool IsQActive
        {
            get { return ObjectManager.Player.HasBuff("FrostShot"); }
        }

        public override void OnUpdate(EventArgs args)
        {
            var targetR = TargetSelector.GetTarget(10000, DamageType.Magical);
            if (ComboMode)
            {
                if (Q.IsReady() && Heroes.Player.LSDistance(Target) < Q.Range)
                {
                    if (GetQStacks() > 4)
                    {
                        Q.Cast();
                    }
                }

                if (W.IsReady() && Heroes.Player.LSDistance(Target) < W.Range)
                {
                    W.Cast(Target);
                }

                if (R.CastCheck(targetR, "ComboR") )
                {
                    R.Cast(targetR);
                }
            }

            if (HarassMode)
            {
                if (W.CastCheck(Target, "HarassW"))
                {
                    W.Cast(Target);
                }
            }
        }

        private int GetQStacks()
        {
            foreach (var buff in ObjectManager.Player.Buffs)
            {
                if (buff.Name == "asheqcastready")
                    return buff.Count;
                if (buff.Name == "AsheQ")
                    return buff.Count;
            }
            return 0;
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
            config.AddBool("ComboR", "Use R", true);
        }

        public override void HarassMenu(Menu config)
        {
            config.AddBool("HarassW", "Use W", true);
        }

        public override void InterruptMenu(Menu config)
        {
            config.AddBool("Interrupt.R", "Use R to Interrupt Spells", true);
        }
    }
}