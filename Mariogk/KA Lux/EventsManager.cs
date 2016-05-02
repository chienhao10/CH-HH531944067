using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Events;
using KA_Lux;
using Settings= KA_Lux.Config.Modes.Misc;

namespace KA_Lux
{
    internal static class EventsManager
    {
        public static void Initialize()
        {
            Gapcloser.OnGapcloser += Gapcloser_OnGapcloser;
            Interrupter.OnInterruptableSpell += Interrupter_OnInterruptableSpell;
        }

        private static void Gapcloser_OnGapcloser(AIHeroClient sender, Gapcloser.GapcloserEventArgs e)
        {
            if (!sender.IsEnemy || !Settings.AntiGapCloser) return;

            if (sender.IsValidTarget(SpellManager.Q.Range) && Player.Instance.ManaPercent >= Settings.MiscMana)
            {
                SpellManager.Q.Cast(sender);
            }
        }

        private static void Interrupter_OnInterruptableSpell(Obj_AI_Base sender, Interrupter.InterruptableSpellEventArgs e)
        {
            if (!sender.IsEnemy || !Settings.InterruptSpell || e.DangerLevel != DangerLevel.High) return;

            if (sender.IsValidTarget(SpellManager.Q.Range) && Player.Instance.ManaPercent >= Settings.MiscMana)
            {
                SpellManager.Q.Cast(sender);
            }
        }
    }
}
