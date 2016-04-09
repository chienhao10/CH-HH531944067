
using EloBuddy;
using EloBuddy.SDK;

namespace Auto_Carry_Vayne.Features.Modes
{
    class Flee
    {
        public static void Load()
        {
            UseE();
            UseQ();
        }

        public static void UseE()
        {
            if (Manager.MenuManager.UseEFlee)
            {
                var target = TargetSelector.GetTarget(Manager.SpellManager.E.Range, DamageType.Physical);
                if (Manager.SpellManager.E.IsReady() && target != null)
                {
                    Manager.SpellManager.E.Cast(target);
                }
            }
        }

        public static void UseQ()
        {
            if (Manager.MenuManager.UseQFlee)
            {
                if (Manager.SpellManager.Q.IsReady())
                {
                    Player.CastSpell(SpellSlot.Q, Game.CursorPos);
                }
            }
        }
    }
}
