using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using KappaLeBlanc;
namespace Modes
{
    class Flee : Helper
    {
        public static void Execute()
        {
            var target = TargetSelector.GetTarget(Lib.E.Range, DamageType.Magical);
            if (target != null)
            {
                if (CastCheckbox(LBMenu.FLM, "E"))
                {
                    var epred = Lib.E.GetPrediction(target);
                    if (epred.HitChance >= HitChance.Medium)
                    {
                        Lib.E.Cast(epred.CastPosition);
                    }
                }
            }
            if (CastCheckbox(LBMenu.FLM, "W"))
            {
                var wpos = myHero.Position.Extend(Game.CursorPos, Lib.W.Range).To3D();
                if (Lib.W.IsReady())
                {
                    Lib.CastW(wpos);
                }
            }
            if (CastCheckbox(LBMenu.FLM, "R"))
            {
                if (Lib.R.IsReady())
                {
                    var wpos = myHero.Position.Extend(Game.CursorPos, Lib.W.Range).To3D();
                    if (Lib.R.Name == "LeblancSlideM")
                    {
                        Lib.CastR(wpos);
                    }
                }
            }
        }
    }
}
