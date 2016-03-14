using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Events;

using Settings = KA_Ezreal.Config.Modes.Misc;
using LastConfig = KA_Ezreal.Config.Modes.LastHit;

namespace KA_Ezreal
{
    internal static class EventsManager
    {
        public static bool CanQCancel;
        public static bool CanWCancel;

        public static void Initialize()
        {
            Gapcloser.OnGapcloser += Gapcloser_OnGapcloser;
            Orbwalker.OnPostAttack += Orbwalker_OnPostAttack;
            Orbwalker.OnUnkillableMinion += Orbwalker_OnUnkillableMinion;
        }

        private static void Orbwalker_OnUnkillableMinion(Obj_AI_Base target, Orbwalker.UnkillableMinionArgs args)
        {
            var minion = target as Obj_AI_Minion;
            if (minion != null && minion.IsValidTarget(SpellManager.Q.Range) && LastConfig.UseQ &&
                Player.Instance.ManaPercent >= LastConfig.LastMana &&
                Prediction.Health.GetPrediction(minion, SpellManager.Q.CastDelay) > 10 &&
                Prediction.Health.GetPrediction(minion, SpellManager.Q.CastDelay) <
                SpellDamage.GetRealDamage(SpellSlot.Q, minion))
            {
                SpellManager.Q.Cast(minion);
            }
        }

        private static void Orbwalker_OnPostAttack(AttackableUnit target, System.EventArgs args)
        {
            CanQCancel = SpellManager.Q.IsReady();
            if (!CanQCancel)
            {
                CanWCancel = SpellManager.W.IsReady();
            }
        }

        private static void Gapcloser_OnGapcloser(AIHeroClient sender, Gapcloser.GapcloserEventArgs e)
        {
            if (!Settings.GapE && !SpellManager.E.IsReady() && sender.IsAlly) return;

            if (sender.IsEnemy && sender.IsVisible && sender.IsInRange(Player.Instance, SpellManager.E.Range))
            {
                SpellManager.E.Cast(Player.Instance.Position.Shorten(sender.Position, SpellManager.E.Range));
            }
        }
    }
}
