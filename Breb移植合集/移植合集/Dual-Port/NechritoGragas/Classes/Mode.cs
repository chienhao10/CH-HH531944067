using LeagueSharp.Common;
using LeagueSharp;
using SharpDX;
using System;
using EloBuddy.SDK;
using EloBuddy;

namespace Nechrito_Gragas
{
    class Mode
    {
        private static AIHeroClient Player => ObjectManager.Player;
        public static void ComboLogic()
        {
            var Target = TargetSelector.SelectedTarget;
            if (Target != null && Target.LSIsValidTarget() && !Target.IsZombie && (Program.Player.LSDistance(Target.Position) <= 900) && MenuConfig.ComboR)
            {
                if (Spells._q.IsReady() && Spells._r.IsReady() && !Target.LSIsDashing())
                {
                    var pos = Spells._r.GetPrediction(Target).CastPosition + 110;
                    {
                        Spells._q.CastIfHitchanceEquals(Target, HitChance.VeryHigh);
                        Spells._r.Cast(pos);
                    }
                }
                var target = TargetSelector.GetTarget(700f, DamageType.Magical);
                if (target != null && target.IsValidTarget() && !target.IsZombie)
                {
                    if (Spells._q.IsReady() && !Spells._r.IsReady())
                    {
                        var pos = Spells._q.GetPrediction(target).CastPosition;
                        Spells._q.Cast(target);
                    }
                    else if (Spells._r.IsReady() && !MenuConfig.OnlyR)
                    {
                        var pos = Spells._r.GetPrediction(target).CastPosition + 100;
                        if (Target.IsFacing(Program.Player))
                        {
                            Spells._r.Cast(pos + 30);
                        }
                        if (!Target.IsFacing(Program.Player))
                        {
                            Spells._r.Cast(pos + 35);
                        }
                    }
                    // Smite
                    else if (Spells.Smite != SpellSlot.Unknown && Spells._r.IsReady()
                          && Player.Spellbook.CanUseSpell(Spells.Smite) == SpellState.Ready && !Target.IsZombie)
                        Player.Spellbook.CastSpell(Spells.Smite, Target);
                    // E
                    if (Spells._e.IsReady())
                    {
                        Spells._e.CastIfHitchanceEquals(target, HitChance.High);
                    }
                    // W
                    if (Spells._w.IsReady() && !Spells._e.IsReady())
                        Spells._w.Cast();
                }
            }
        }

        public static void JungleLogic()
        {
            var mobs = MinionManager.GetMinions(400 + Program.Player.AttackRange, MinionTypes.All, MinionTeam.Neutral,
           MinionOrderTypes.MaxHealth);
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear) || Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear))
            {
                if (mobs.Count == 0 || mobs == null)
                    return;

                if (Spells._w.IsReady())
                    Spells._w.Cast();
                if (Spells._e.IsReady())
                    Spells._e.Cast(mobs[0]);
                if (Spells._q.IsReady())
                    Spells._q.Cast(mobs[0]);
            }
        }
        public static void HarassLogic()
        {
            var target = TargetSelector.GetTarget(Spells._r.Range - 50, DamageType.Magical);
            if (target != null && target.IsValidTarget() && !target.IsZombie)
            {
                if (Spells._e.IsReady())
                    Spells._e.Cast(target);
                if (Spells._q.IsReady())
                {
                    var pos = Spells._q.GetPrediction(target).CastPosition;
                    Spells._q.Cast(pos);
                }
            }
        }
    }
}
