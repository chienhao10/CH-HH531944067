
using EloBuddy;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK;

namespace AkaYasuo.Events
{
    class _interrupter
    {
        public static void Interrupt(Obj_AI_Base sender,
            Interrupter.InterruptableSpellEventArgs e)
        {
            if (e != null && Program.Q2.IsReady() && Variables.HaveQ3 && sender.IsValidTarget(Program.Q2.Range) && MenuManager.MiscMenu["InterruptQ"].Cast<CheckBox>().CurrentValue)
            {
                Program.Q2.Cast(sender.ServerPosition);
            }
        }
    }
}
