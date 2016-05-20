using EloBuddy.SDK;
using EloBuddy;

namespace Auto_Carry_Vayne.Features.Module.Misc
{
    class Gapcloser : IModule
    {
        public void OnLoad()
        {
            EloBuddy.SDK.Events.Gapcloser.OnGapcloser += Gapcloser_OnGapCloser;
        }

        private static void Gapcloser_OnGapCloser(AIHeroClient sender, EloBuddy.SDK.Events.Gapcloser.GapcloserEventArgs e)
        {
            if (sender == null || sender.IsAlly) return;

            if ((e.End.Distance(Variables._Player) <= 70) && Manager.MenuManager.GapcloseE)
            {
                Manager.SpellManager.E.Cast(sender);
            }

            if ((e.End.Distance(Variables._Player) <= 70) && Manager.MenuManager.GapcloseQ)
            {
                var QPos = e.End.Extend(Variables._Player.Position, Manager.SpellManager.Q.Range);
                Player.CastSpell(SpellSlot.Q, QPos.To3D());
            }
        }

        public ModuleType GetModuleType()
        {
            return ModuleType.OnUpdate;
        }

        public bool ShouldGetExecuted()
        {
            return Manager.MenuManager.RNoAA;
        }

        public void OnExecute() { }
    }
}
