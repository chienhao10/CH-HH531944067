using EloBuddy;
using EloBuddy.SDK;
using KappaLeBlanc;
using System.Linq;
namespace Modes
{
    class Laneclear : Helper
    {
        public static void Execute()
        {
            var _Q = CastCheckbox(LBMenu.LCM, "Q") && Lib.Q.IsReady() && CastSlider(LBMenu.LCM, "QMana") < myHero.ManaPercent;
            var _W = CastCheckbox(LBMenu.LCM, "W") && Lib.W.IsReady() && CastSlider(LBMenu.LCM, "WMana") < myHero.ManaPercent;
            var _R = CastCheckbox(LBMenu.LCM, "R") && Lib.R.IsReady() && Lib.R.Name == "LeblancSlideM";

            if (_W)
            {
                var minions = EntityManager.MinionsAndMonsters.GetLaneMinions(EntityManager.UnitTeam.Enemy, myHero.ServerPosition, Lib.W.Range);
                if (minions != null)
                {
                    var Wminions = EntityManager.MinionsAndMonsters.GetCircularFarmLocation(minions, Lib.W.Width, (int)Lib.W.Range);
                    if (CastSlider(LBMenu.LCM, "WMin") <= Wminions.HitNumber)
                    {
                        Lib.CastW(Wminions.CastPosition);
                    }
                }
            }
            if (_R)
            {
                var minions = EntityManager.MinionsAndMonsters.GetLaneMinions(EntityManager.UnitTeam.Enemy, myHero.ServerPosition, Lib.W.Range);
                if (minions != null)
                {
                    var Rminions = EntityManager.MinionsAndMonsters.GetCircularFarmLocation(minions, Lib.W.Width, (int)Lib.W.Range);
                    if (CastSlider(LBMenu.LCM, "RMin") <= Rminions.HitNumber)
                    {
                        Lib.CastR(Rminions.CastPosition);
                    }
                }
            }
            if (_Q)
            {
                var Qminion = EntityManager.MinionsAndMonsters.EnemyMinions.FirstOrDefault(minion => minion.Health < myHero.GetSpellDamage(minion, SpellSlot.Q)
                && myHero.Distance(minion) <= Lib.Q.Range
                && minion.IsEnemy);
                if (Qminion != null)
                {
                    Lib.Q.Cast(Qminion);
                }
            }
            if (CastCheckbox(LBMenu.LCM, "W2"))
            {
                if (Program.RavenForm)
                {
                    myHero.Spellbook.CastSpell(SpellSlot.W);
                }
            }
        }
    }
}