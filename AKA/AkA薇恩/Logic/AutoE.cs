
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu.Values;

namespace Aka_s_Vayne_reworked.Logic
{
    class AutoE
    {
        public static void OnExecute()
        {
            if (!MenuManager.CondemnMenu["UseEauto"].Cast<CheckBox>().CurrentValue)
            {
                return;
            }

            var CondemnTarget = NewELogic.GetCondemnTarget(Variables._Player.ServerPosition);
            if (CondemnTarget.IsValidTarget() && Program.E.IsReady())
            {
                Program.E.Cast(CondemnTarget);
            }
        }
    }
}
