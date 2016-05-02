namespace KappAzir.Modes
{
    using System.Linq;

    using EloBuddy;
    using EloBuddy.SDK;

    using Mario_s_Lib;

    using SharpDX;
    using static Menus;
    using static SpellsManager;

    internal class KillSteal : ModeManager
    {
        public static void Execute()
        {
            if (kstarget(Q.Range) == null)
            {
                return;
            }
            if (W.IsReady() && KillStealMenu.GetCheckBoxValue("wUse"))
            {
                if (Q.IsReady() && Q.Handle.SData.Mana + W.Handle.SData.Mana < Azir.Mana
                    && Azir.GetSpellDamage(kstarget(Q.Range), SpellSlot.Q) >= kstarget(Q.Range).TotalShieldHealth())
                {
                    var p = Azir.Distance(kstarget(Q.Range), true) > W.RangeSquared
                                ? Azir.Position.To2D().Extend(kstarget(Q.Range).Position.To2D(), W.Range)
                                : kstarget(Q.Range).Position.To2D();
                    W.Cast((Vector3)p);
                }
            }

            if (Azir.GetSpellDamage(kstarget(Q.Range), SpellSlot.Q) >= kstarget(Q.Range).TotalShieldHealth())
            {
                if (Orbwalker.AzirSoldiers.Any(s => s.IsAlly))
                {
                    Q.TryToCast(Q.GetPrediction(kstarget(Q.Range)).CastPosition, KillStealMenu);
                }
            }

            if (Azir.GetSpellDamage(kstarget(E.Range), SpellSlot.E) >= kstarget(E.Range).TotalShieldHealth())
            {
                if (Ehit(kstarget(E.Range)))
                {
                    E.TryToCast(kstarget(E.Range).Position, KillStealMenu);
                }
            }
        }

        private static AIHeroClient kstarget(uint range)
        {
            return EntityManager.Heroes.Enemies.FirstOrDefault(x => x.IsValidTarget(range) && x != null);
        }
    }
}