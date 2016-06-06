using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using KappaLeBlanc;
using System.Linq;

namespace Modes
{
    class Killsteal : Helper
    {
        public static void Execute()
        {
            if (CastSlider(LBMenu.Misc, "AutoW") > myHero.HealthPercent)
            {
                if (Program.RavenForm)
                {
                    myHero.Spellbook.CastSpell(SpellSlot.W);
                }
            }
            var target = TargetSelector.GetTarget(Lib.W.Range * 2 + Lib.Q.Range, DamageType.Magical);
            if (target == null || !target.IsValidTarget()) return;

            var RReady = Lib.R.Name.Equals("LeblancChaosOrbM") || Lib.R.Name.Equals("LeblancSoulShackleM") || Lib.R.Name.Equals("LeblancSlideM");
            var WReady = !!Program.RavenForm && Lib.W.IsReady();
            var ksm = LBMenu.KSM;
            var wpos = myHero.Position.Extend(target, Lib.W.Range).To3D();


            var QDmg = Lib.Q.GetDamage(target);
            var WDmg = Lib.W.GetDamage(target);
            var EDmg = Lib.E.GetDamage(target);
            var RDmg = Lib.R.GetDamage(target);

            if (Lib.R.IsReady() && CastCheckbox(ksm, "R") && Lib.R.IsInRange(target))
            {
                if (QDmg < target.Health || !Lib.Q.IsReady())
                {
                    if (RDmg > target.Health)
                    {
                        Lib.CastR(target);
                    }
                }
            }
            if (QDmg + WDmg + EDmg > target.Health)
            {
                var epred = Lib.E.GetPrediction(target);
                if (epred.HitChance >= HitChance.Medium)
                {
                    if (Lib.Q.IsInRange(target) && CastCheckbox(ksm, "Q"))
                    {
                        Lib.Q.Cast(target);
                    }
                    if (Lib.W.IsInRange(target) && CastCheckbox(ksm, "W"))
                    {
                        Lib.CastW(target);
                    }
                    if (Lib.E.IsInRange(target) && CastCheckbox(ksm, "E"))
                    {
                        Lib.E.Cast(epred.CastPosition);
                    }
                }
            }
            if (Lib.Q.IsReady() && CastCheckbox(ksm, "Q") && QDmg > target.Health)
            {
                if (myHero.IsInRange(target, Lib.Q.Range + Lib.W.Range))
                {
                    if (Lib.Q.IsInRange(target))
                    {
                        Lib.Q.Cast(target);
                    }
                    else if (WReady)
                    {
                        if (Lib.W.Range + Lib.Q.Range > myHero.Distance(target) && CastCheckbox(ksm, "W") && CastCheckbox(ksm, "extW"))
                            Lib.CastW(wpos);
                    }
                    else if (RReady)
                    {
                        if (Lib.W.Range + Lib.Q.Range > myHero.Distance(target) && CastCheckbox(ksm, "R") && CastCheckbox(ksm, "extW"))
                            Lib.CastR(wpos);
                    }
                }
                else if (myHero.IsInRange(target, Lib.Q.Range + Lib.W.Range * 2) && Lib.Q.IsReady() && WReady && RReady)
                {
                    if (Lib.Q.IsInRange(target))
                    {
                        Lib.Q.Cast(target);
                    }
                    else if (CastCheckbox(ksm, "W") && CastCheckbox(ksm, "wr"))
                    {
                        Lib.CastW(wpos);
                        Core.DelayAction(() =>
                        Lib.CastR(wpos), (int) myHero.Distance(wpos) / Lib.W.Speed + Game.Ping / 2);
                    }
                }
            }

            else if (Lib.E.IsReady() && CastCheckbox(ksm, "E") && EDmg > target.Health)
            {
                var epred = Lib.E.GetPrediction(target);

                if (myHero.IsInRange(target, Lib.E.Range + Lib.W.Range))
                {
                    if (Lib.E.IsInRange(target) && epred.HitChance >= HitChance.High)
                    {
                        Lib.E.Cast(epred.CastPosition);
                    }
                    else if (WReady)
                    {
                        if (Lib.W.Range + Lib.E.Range > myHero.Distance(target) && CastCheckbox(ksm, "W") && CastCheckbox(ksm, "extW"))
                            Lib.CastW(wpos);
                    }
                    else if (RReady)
                    {
                        if (Lib.W.Range + Lib.E.Range > myHero.Distance(target) && CastCheckbox(ksm, "R") && CastCheckbox(ksm, "extW"))
                            Lib.CastR(wpos);
                    }
                }
                else if (myHero.IsInRange(target, Lib.E.Range + Lib.W.Range * 2) && Lib.E.IsReady() && WReady && RReady)
                {
                    if (Lib.E.IsInRange(target) && epred.HitChance >= HitChance.High)
                    {
                        Lib.E.Cast(epred.CastPosition);
                    }
                    else if (CastCheckbox(ksm, "W") && CastCheckbox(ksm, "wr"))
                    {
                        Lib.CastW(wpos);
                        Core.DelayAction(() =>
                        Lib.CastR(wpos), (int)myHero.Distance(wpos) / Lib.W.Speed + Game.Ping / 2);
                    }
                }
            }
            else if (WReady && CastCheckbox(ksm, "W") && WDmg > target.Health)
            {
                if (Lib.W.IsInRange(target))
                {
                    Lib.CastW(target);
                }
                else if (CastCheckbox(ksm, "wr"))
                {
                    if (myHero.IsInRange(target, Lib.W.Range * 2))
                    {
                        Lib.CastR(wpos);
                    }
                }
            }
        }
    }
}

