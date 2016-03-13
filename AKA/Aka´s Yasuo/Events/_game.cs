using System;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK.Events;

namespace AkaYasuo.Events
{
    class _game
    {
        public static void AutoQ()
        {
            if (!MenuManager.HarassMenu["AutoQ"].Cast<KeyBind>().CurrentValue || Variables._Player.IsDashing()
                || (Variables.HaveQ3 && !MenuManager.HarassMenu["AutoQ3"].Cast<CheckBox>().CurrentValue)    
                || (Variables._Player.IsUnderEnemyturret() && !MenuManager.HarassMenu["QTower"].Cast<CheckBox>().CurrentValue))
            {
                return;
            }
            var target = TargetSelector.GetTarget(!Variables.HaveQ3 ? Variables.QRange : Variables.Q2Range, DamageType.Physical);
            if (target == null)
            {
                return;
            }
            (!Variables.HaveQ3 ? Program.Q : Program.Q2).Cast(target);
        }

        public static void StackQ()
        {
            if (!MenuManager.MiscMenu["StackQ"].Cast<CheckBox>().CurrentValue || !Program.Q.IsReady() || Variables._Player.IsDashing() || Variables.HaveQ3)
            {
                return;
            }

            var target = TargetSelector.GetTarget(Program.Q.Range, DamageType.Physical);
            if (target != null && (!Variables._Player.IsUnderEnemyturret() || !target.ServerPosition.IsUnderTurret()))
            {
                Program.Q.Cast(target.ServerPosition);
            }
            else
            {
                var minionObj = EntityManager.MinionsAndMonsters.GetLaneMinions(EntityManager.UnitTeam.Enemy, Variables._Player.ServerPosition, Variables.QRange);
                if (minionObj.Any())
                {
                    var obj = minionObj.FirstOrDefault(i => DamageManager._GetQDmg(i) >= i.Health)
          ?? minionObj.MinOrDefault(i => i.Distance(Variables._Player));
                    if (obj != null)
                    {
                        Program.Q.Cast(obj);
                    }
                }
            }
        }

        public static void LevelUpSpells()
        {
            if (!MenuManager.MiscMenu["autolvl"].Cast<CheckBox>().CurrentValue) return;

            var qL = Variables._Player.Spellbook.GetSpell(SpellSlot.Q).Level + Variables.QOff;
            var wL = Variables._Player.Spellbook.GetSpell(SpellSlot.W).Level + Variables.WOff;
            var eL = Variables._Player.Spellbook.GetSpell(SpellSlot.E).Level + Variables.EOff;
            var rL = Variables._Player.Spellbook.GetSpell(SpellSlot.R).Level + Variables.ROff;
            if (qL + wL + eL + rL >= Variables._Player.Level) return;
            int[] level = { 0, 0, 0, 0 };
            for (var i = 0; i < Variables._Player.Level; i++)
            {
                level[Variables.abilitySequence[i] - 1] = level[Variables.abilitySequence[i] - 1] + 1;
            }
            if (qL < level[0]) Variables._Player.Spellbook.LevelSpell(SpellSlot.Q);
            if (wL < level[1]) Variables._Player.Spellbook.LevelSpell(SpellSlot.W);
            if (eL < level[2]) Variables._Player.Spellbook.LevelSpell(SpellSlot.E);
            if (rL < level[3]) Variables._Player.Spellbook.LevelSpell(SpellSlot.R);
        }
    }
}
