using EloBuddy;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Events;
using KappaLeBlanc;
namespace Modes
{
    class AntiGapcloser : Helper
    {
        public static void Execute(AIHeroClient sender, Gapcloser.GapcloserEventArgs e)
        {
            if (sender.IsAlly || sender.IsDead || sender.IsMe) return;

            if (CastCheckbox(LBMenu.AntiGapcloserM, "E"))
            {
                if (Lib.E.IsReady())
                {
                    var epred = Lib.E.GetPrediction(sender);
                    if (epred.HitChance >= HitChance.Medium)
                    {
                        Lib.E.Cast(epred.CastPosition);
                    }
                }
            }
            if (CastCheckbox(LBMenu.AntiGapcloserM, "RE"))
            {
                if (Lib.R.Name == "LeblancSoulShackleM")
                {
                    if (Lib.R.IsReady())
                    {
                        var epred = Lib.E.GetPrediction(sender);
                        if (epred.HitChance >= HitChance.Medium)
                        {
                            Lib.R.Cast(epred.CastPosition);
                        }
                    }
                }
            }
        }
    }
}