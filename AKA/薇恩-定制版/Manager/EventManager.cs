using System;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Events;
using System.Linq;
using Auto_Carry_Vayne.Features.Module;

namespace Auto_Carry_Vayne.Manager
{
    class EventManager
    {
        public static void Load()
        {
            Obj_AI_Base.OnSpellCast += Obj_AI_Base_OnSpellCast;
            Obj_AI_Base.OnBasicAttack += Obj_AI_Base_OnBasicAttack;
            Spellbook.OnStopCast += Spellbook_OnStopCast;
            Game.OnUpdate += Game_OnUpdate;
            Logic.Mechanics.LoadFlash();
            Traps.Load();

            foreach (var module in Variables.moduleList)
            {
                module.OnLoad();
            }
        }

        private static void Game_OnUpdate(EventArgs args)
        {
            Logic.Mechanics.FlashE();
            Logic.Mechanics.Insec();

            foreach (var module in Variables.moduleList.Where(module => module.GetModuleType() == ModuleType.OnUpdate
    && module.ShouldGetExecuted()))
            {
                module.OnExecute();
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass)) Features.Modes.Harass.HarassCombo();
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear)) Features.Modes.JungleClear.Load();
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Flee)) Features.Modes.Flee.Load();
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo)) Features.Modes.Combo.Load();
        }

        private static void Obj_AI_Base_OnSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            Features.Modes.LaneClear.SpellCast(sender, args);
        }

        private static void Obj_AI_Base_OnBasicAttack(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.IsMe)
            {
                Variables.lastaa = Game.Time * 1000;
            }
        }

        private static void Spellbook_OnStopCast(Obj_AI_Base sender, SpellbookStopCastEventArgs args)
        {
            if (sender.IsMe && (Game.Time * 1000) - Variables.lastaa < ObjectManager.Player.AttackCastDelay * 1000 + 50f && !args.ForceStop)
            {
                Variables.lastaa = 0f;
            }
        }
    }
}
