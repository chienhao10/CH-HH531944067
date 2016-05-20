using System;
using LeagueSharp.Common;
using EloBuddy.SDK;

namespace NechritoRiven
{
    class Burst
    {
        private static void Game_OnUpdate(EventArgs args)
        {
            BurstLogic();
        }
        public static void BurstLogic()
        {
            var target = TargetSelector.SelectedTarget;
            if (target != null && target.IsValidTarget() && !target.IsZombie)
            {
                // Flash
                if (target.Health < Dmg.Totaldame(target) || MenuConfig.AlwaysF)
                {
                    if ((Program.Player.LSDistance(target.Position) <= 700) && (Program.Player.LSDistance(target.Position) >= 600) && Program.Player.Spellbook.GetSpell(Spells.Flash).IsReady() && Spells._r.IsReady() && Spells._e.IsReady() && Spells._w.IsReady())
                    {
                        Logic.CastYoumoo();
                        if (Spells._e.IsReady() && Spells._r.IsReady() && Spells._r.Instance.Name == Program.IsFirstR)
                        {
                            Spells._e.Cast(target.Position);
                            Spells._r.Cast();
                        }
                        if (Program.Player.Spellbook.GetSpell(Spells.Flash).IsReady())
                        {
                            Utility.DelayAction.Add(10, () => Program.Player.Spellbook.CastSpell(Spells.Flash, target.Position));
                        }
                        if (Spells._w.IsReady())
                        {
                            Spells._w.Cast(target);
                            Utility.DelayAction.Add(35, Logic.CastHydra);
                        }
                        if (Spells._r.IsReady() && Spells._r.Instance.Name == Program.IsSecondR)
                        {
                            Spells._r.Cast(target.ServerPosition);
                        }
                        if (Spells._q.IsReady())
                        { Utility.DelayAction.Add(150, () => Logic.ForceCastQ(target)); }

                    }
                }
                // Burst
                if ((Program.Player.LSDistance(target.Position) <= Spells._e.Range + Program.Player.AttackRange + Program.Player.BoundingRadius - 20))
                {
                    if (Spells._e.IsReady())
                    { Spells._e.Cast(target.Position); }
                    Logic.CastYoumoo();
                    if (Spells._r.IsReady())
                    { Logic.ForceR(); }
                    Utility.DelayAction.Add(160, Logic.ForceW);
                    Utility.DelayAction.Add(70, Logic.ForceItem);
                    Utility.DelayAction.Add(70, Logic.CastHydra);
                    if (Spells._r.IsReady() && Spells._r.Instance.Name == Program.IsSecondR)
                    { Spells._r.Cast(target.ServerPosition); }
                    if (Spells._q.IsReady())
                    { Utility.DelayAction.Add(150, () => Logic.ForceCastQ(target)); }
                }
            }
        }
    }
}