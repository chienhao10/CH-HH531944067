using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EloBuddy.SDK;
using EloBuddy;
using SharpDX;

namespace Auto_Carry_Vayne.Features.Module.Tumble
{
    class QKS : IModule
    {
        public void OnLoad()
        {

        }

        public ModuleType GetModuleType()
        {
            return ModuleType.OnUpdate;
        }

        public bool ShouldGetExecuted()
        {
            return Manager.SpellManager.Q.IsReady();
        }

        public void OnExecute()
        {
            var currentTarget = TargetSelector.GetTarget((int)Variables._Player.GetAutoAttackRange(),
    DamageType.Physical);

            if (!currentTarget.IsValidTarget() || currentTarget.IsZombie || currentTarget.IsInvulnerable || currentTarget.IsDead)
            {
                return;
            }

            if (currentTarget.ServerPosition.Distance(Variables._Player.ServerPosition) <=
                Variables._Player.GetAutoAttackRange())
            {
                return;
            }

            if (currentTarget.Health <
                Variables._Player.GetAutoAttackDamage(currentTarget) +
                Variables._Player.GetSpellDamage(currentTarget, SpellSlot.Q)
                && currentTarget.Health > 0)
            {
                var extendedPosition = (Vector3)Variables._Player.ServerPosition.Extend(
                    currentTarget.ServerPosition, 300f);
                //    if (extendedPosition.IsSafe())
                {
                    Player.CastSpell(SpellSlot.Q, extendedPosition);
                }
            }
        }
    }
}
