using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using KappaLeBlanc;
using System;
namespace Modes
{
    class Combo : Helper
    {
        public static void Execute()
        {
            var target = TargetSelector.GetTarget(Lib.W.Range * 2, DamageType.Magical);
            if (target != null)
            {
                var ComboM = LBMenu.ComboM;
                var WReady = !Program.RavenForm && Lib.W.IsReady();

                if (CastCheckbox(ComboM, "Q"))
                {
                    if (Lib.Q.IsReady())
                    {
                        if (WReady || Lib.E.IsReady() || Lib.R.IsReady() || target.HasBuff("LeblancSoulShackle") || Lib.QlasTick > Environment.TickCount || myHero.Level == 1 ||
                            target.HasBuff("LeblancChaosOrbM") || (Lib.W.GetCooldown() > 0 && Lib.W.GetCooldown() < 4) || (Lib.E.GetCooldown() > 0 && Lib.E.GetCooldown() < 4))
                        {
                            if (target.IsValidTarget(Lib.Q.Range))
                            {
                                Lib.Q.Cast(target);
                            }
                        }
                    }
                }
                if (CastCheckbox(ComboM, "W"))
                {
                    if (CastCheckbox(ComboM, "extW") && myHero.Distance(target) > Lib.Q.Range)
                    {
                        if (WReady)
                        {
                            var wpos = myHero.Position.Extend(target, Lib.W.Range).To3D();
                            if (Lib.Q.IsReady() && CastCheckbox(ComboM, "Q"))
                            {
                                if (Lib.Q.Range + Lib.W.Range > myHero.Distance(target))
                                {
                                    Lib.CastW(wpos);
                                }
                            }
                            else if (Lib.E.IsReady() && CastCheckbox(ComboM, "E"))
                            {
                                if (Lib.E.Range + Lib.W.Range > myHero.Distance(target))
                                {
                                    Lib.CastW(wpos);
                                }
                            }
                        }
                    }
                    else
                    {
                        if (WReady)
                        {
                            if (Lib.W.IsInRange(target))
                            {
                                if (target.HasBuff("LeblancChaosOrb") || target.HasBuff("LeblancSoulShackle") || myHero.Level == 1 || target.HasBuff("LeblancChaosOrbM") || target.HasBuff("LeblancSoulShackleM") || Lib.QlasTick > Environment.TickCount)
                                {
                                    Lib.CastW(target);
                                }
                            }
                        }
                    }
                }
                if (CastCheckbox(ComboM, "E"))
                {
                    if (Lib.E.IsReady() && (!WReady || myHero.Level == 1))
                    {
                        var epred = Lib.E.GetPrediction(target);

                        if (epred.HitChance >= HitChance.Medium)
                        {
                            Lib.E.Cast(epred.CastPosition);
                        }
                    }
                }
                if (CastCheckbox(ComboM, "R"))
                {
                    if (Lib.R.IsReady())
                    {
                        if (CastCheckbox(ComboM, "RQ"))
                        {
                            if (Lib.R.Name == "LeblancChaosOrbM") // Q
                            {
                                if (WReady || Lib.E.IsReady() || Lib.Q.IsReady() || target.HasBuff("LeblancSoulShackle") || Lib.QlasTick > Environment.TickCount || target.HasBuff("LeblancChaosOrb") || (Lib.W.GetCooldown() > 0 && Lib.W.GetCooldown() <= 4) || (Lib.E.GetCooldown() > 0 && Lib.E.GetCooldown() <= 4)
                                    )
                                {
                                    Lib.CastR(target);
                                    //Lib.R.Cast(target);
                                }
                            }
                        }
                        if (CastCheckbox(ComboM, "RW"))
                        {
                            if (Lib.R.Name == "LeblancSlideM") // W
                            {
                                if (target.CountEnemiesInRange(Lib.W.Width) > 1)
                                {
                                    //Lib.CastR(target);
                                    Lib.R.Cast(target);
                                }
                                else if (!Lib.Q.IsReady() && !Lib.E.IsReady())
                                {
                                    //Lib.CastR(target);
                                    Lib.R.Cast(target);
                                }
                            }
                        }
                        if (CastCheckbox(ComboM, "RE"))
                        {
                            if (Lib.R.Name == "LeblancSoulShackleM") // E
                            {
                                if (Lib.Q.IsReady() || Lib.E.IsReady())
                                {
                                    //Lib.CastR(target);
                                    var epred = Lib.E.GetPrediction(target);
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
        }
    }
}
