using EloBuddy;
using AddonTemplate.Logic;
using Aka_s_Vayne_reworked.Logic;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu.Values;
using SharpDX;

namespace Aka_s_Vayne_reworked.Modes
{
    internal class Combo
    {

        public static void Load()
        {
            var target = TargetSelector.GetTarget((int)Variables._Player.GetAutoAttackRange(),
    DamageType.Physical);

            UseQ();
            UseE();
            UseR();
            UseTrinket(target);
            if (MenuManager.ComboMenu["AAReset"].Cast<CheckBox>().CurrentValue) Events._game.Customorbwalker();
        }

        public static void UseQ()
        {
            var target = TargetSelector.GetTarget((int)Variables._Player.GetAutoAttackRange(), DamageType.Physical);

            if (Functions.Modes.Combo.AfterAttack && MenuManager.ComboMenu["UseQwhen"].Cast<ComboBox>().CurrentValue == 0)
            {
                if (target == null) return;
                QLogic.OnCastTumble(target, Game.CursorPos);

            }

            if (Functions.Modes.Combo.BeforeAttack && MenuManager.ComboMenu["UseQwhen"].Cast<ComboBox>().CurrentValue == 1)
            {
                if (target == null) return;
                QLogic.PreCastTumble(target);
            }

            if (MenuManager.ComboMenu["UseQwhen"].Cast<ComboBox>().CurrentValue == 2)
            {
                return;
            }
        }

        public static void UseE()
        {
            if (Functions.Modes.Combo.AfterAttack && MenuManager.ComboMenu["comboUseE"].Cast<CheckBox>().CurrentValue)
            {
                NewELogic.Execute();
            }
        }

        public static void UseR()
        {
            if (MenuManager.ComboMenu["comboUseR"].Cast<CheckBox>().CurrentValue && Program.R.IsReady())
            {
                Functions.Modes.Combo.ComboUltimateLogic();
            }
        }

        public static void UseTrinket(Obj_AI_Base target)
        {
            if (target == null)
            {
                return;
            }
            if (Variables._Player.Spellbook.GetSpell(SpellSlot.Trinket).IsReady &&
                Variables._Player.Spellbook.GetSpell(SpellSlot.Trinket).SData.Name.ToLower().Contains("totem"))
            {
                Core.DelayAction(delegate
                {
                    if (MenuManager.CondemnMenu["trinket"].Cast<CheckBox>().CurrentValue)
                    {
                        var pos = Mechanics.GetFirstNonWallPos(Variables._Player.Position.To2D(), target.Position.To2D());
                        if (NavMesh.GetCollisionFlags(pos).HasFlag(CollisionFlags.Grass))
                        {
                            Program.totem.Cast(pos.To3D());
                        }
                    }
                }, 200);
            }

        }
    }
}

