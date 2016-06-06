using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using KappaLeBlanc;
namespace Modes
{
    class Harass : Helper
    {
        public static void Execute()
        {
            var target = TargetSelector.GetTarget(Lib.W.Range * 2, DamageType.Magical);
            if (target == null) return;
            var _Q = CastCheckbox(LBMenu.HSM, "Q") && Lib.Q.IsReady() && CastSlider(LBMenu.HSM, "QMana") < myHero.ManaPercent;
            var _W = CastCheckbox(LBMenu.HSM, "W") && !Program.RavenForm && Lib.W.IsReady() && CastSlider(LBMenu.HSM, "WMana") < myHero.ManaPercent;
            var _E = CastCheckbox(LBMenu.HSM, "E") && Lib.E.IsReady() && CastSlider(LBMenu.HSM, "EMana") < myHero.ManaPercent;
            var extW = CastCheckbox(LBMenu.HSM, "extW");
            var wpos = myHero.Position.Extend(target, Lib.W.Range).To3D();


            if (CastCheckbox(LBMenu.HSM, "AutoW"))
            {
                if (Program.RavenForm && !_Q && !_E)
                {
                    myHero.Spellbook.CastSpell(SpellSlot.W);
                }
            }
            if (_Q)
            {
                if (Lib.Q.IsInRange(target))
                {
                    Lib.Q.Cast(target);
                }
                else if (extW && myHero.IsInRange(target, Lib.Q.Range + Lib.W.Range))
                {
                    Lib.CastW(wpos);
                }
            }
            else if (_W)
            {
                var wpred = Lib.W.GetPrediction(target);
                Lib.W.Cast(wpred.CastPosition);
            }
            else if (_E)
            {
                var epred = Lib.E.GetPrediction(target);
                if (epred.HitChance >= HitChance.High)
                {
                    Lib.E.Cast(epred.CastPosition);
                }
            }
        }
    }
}