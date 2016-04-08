using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using Settings = Tristana.Config.ModesMenu.Flee;
using SettingsMana = Tristana.Config.ManaManagerMenu;
using SettingsPrediction = Tristana.Config.PredictionMenu;

namespace Tristana.Modes
{
    public sealed class Flee : ModeBase
    {
        public override bool ShouldBeExecuted()
        {
            return Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Flee);
        }

        public override void Execute()
        {
            if (Settings.UseR && R.IsReady() && PlayerMana >= SettingsMana.MinRMana)
            {
                var target =
                    EntityManager.Heroes.Enemies.Where(e => e.IsValidTarget(R.Range))
                        .OrderBy(e => e.Distance(_PlayerPos))
                        .FirstOrDefault();
                if (target != null)
                {
                    if (!target.HasBuffOfType(BuffType.SpellImmunity) && !target.HasBuffOfType(BuffType.SpellShield))
                    {
                        R.Cast(target);
                        Debug.WriteChat("Casting R in Flee to knockback {0}", target.ChampionName);
                        return;
                    }
                }
            }
            if (Settings.UseW && W.IsReady() && PlayerMana >= SettingsMana.MinWMana)
            {
                var cursorPos = Game.CursorPos;
                var castPos = _PlayerPos.Distance(cursorPos) <= W.Range ? cursorPos : _PlayerPos.Extend(cursorPos, W.Range).To3D();
                W.Cast(castPos);
                Debug.WriteChat("Casting W in Flee mode.");
            }
        }
    }
}
