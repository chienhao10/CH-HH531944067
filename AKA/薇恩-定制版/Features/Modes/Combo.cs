using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EloBuddy;
using EloBuddy.SDK;
using Auto_Carry_Vayne.Manager;
using Auto_Carry_Vayne.Logic;

namespace Auto_Carry_Vayne.Features.Modes
{
    class Combo
    {
        public static void Load()
        {
            var target = TargetSelector.GetTarget((int)Variables._Player.GetAutoAttackRange(),
    DamageType.Physical);
            if (target == null) return;
            UseQ();
            UseE();
            UseR();
        }

        public static void UseQ()
        {
            var target = TargetSelector.GetTarget((int)Variables._Player.GetAutoAttackRange(), DamageType.Physical);
            if (target == null) return;
            if (Variables.AfterAttack && Manager.MenuManager.UseQ && Manager.SpellManager.Q.IsReady())
            {
                #region check for 2 w stacks
                if (Manager.MenuManager.UseQStacks && target.GetBuffCount("vaynesilvereddebuff") != 2)
                {
                    return;
                }
                #endregion
                var TumblePos = NewTumble.AkaQPosition();
                Player.CastSpell(SpellSlot.Q, TumblePos);
            }
        }

        public static void UseE()
        {
            if (Variables.AfterAttack && Manager.MenuManager.UseE && Manager.SpellManager.E.IsReady())
            {
                Condemn.Execute();
            }
        }

        public static void UseR()
        {
            if (Manager.MenuManager.UseR && Manager.SpellManager.R.IsReady())
            {
                if (Variables._Player.CountEnemiesInRange(1000) >= Manager.MenuManager.UseRSlider)
                {
                    Manager.SpellManager.R.Cast();
                }
            }
        }
    }
}

