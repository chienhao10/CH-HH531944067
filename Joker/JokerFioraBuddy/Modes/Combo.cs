using EloBuddy;
using EloBuddy.SDK;
using SharpDX;
using System;
using Settings = JokerFioraBuddy.Config.Modes.Combo;

namespace JokerFioraBuddy.Modes
{
    public sealed class Combo : ModeBase
    {
        public override bool ShouldBeExecuted()
        {
            return Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo);
        }

        public override void Execute()
        {

            var target = TargetSelector2.GetTarget(Q.Range, DamageType.Physical);
   

            if (target != null && target.IsValidTarget(R.Range))
            {
                PassiveManager.castAutoAttack(target);

                if (Settings.UseYomuus)
                    ItemManager.UseYomu();

                if (Settings.UseQ && Q.IsReady() && target.IsValidTarget(Q.Range) && !target.IsZombie)
                    SpellManager.castQ();

                if (Settings.UseCutlassBOTRK)
                    ItemManager.UseCastables();
                
                if (Settings.UseTiamatHydra)
                    ItemManager.useHydra(target);

                if (Settings.UseE && E.IsReady() && target.IsValidTarget(E.Range) && !target.IsZombie)
                    E.Cast();

                if (Settings.UseR && R.IsReady() && target.IsValidTarget(R.Range) && !target.IsZombie && Config.Modes.Combo.useRonTarget(target.ChampionName) && Player.Instance.HealthPercent <= Config.Modes.Combo.rSliderValue())
                    SpellManager.castR();
                
            }
        }
    }
}