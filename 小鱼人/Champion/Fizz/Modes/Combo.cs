#region

using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Menu.Values;
using SimplisticTemplate.Champion.Fizz.Utils;

#endregion

namespace SimplisticTemplate.Champion.Fizz.Modes
{
    internal static class Combo
    {
        private static AIHeroClient Me
        {
            get { return ObjectManager.Player; }
        }

        public static void Execute()
        {
            var target = TargetSelector.GetTarget(Fizz.R.Range, DamageType.Magical);
            if (!target.IsValidTarget()) return;

            var useRGap = GameMenu.ComboMenu["useRGap"].Cast<CheckBox>().CurrentValue;
            var useEGap = GameMenu.ComboMenu["useEGap"].Cast<CheckBox>().CurrentValue;
            var useQ = GameMenu.ComboMenu["useQ"].Cast<CheckBox>().CurrentValue;
            var useW = GameMenu.ComboMenu["useW"].Cast<CheckBox>().CurrentValue;
            var useE = GameMenu.ComboMenu["useE"].Cast<CheckBox>().CurrentValue;
            var useR = GameMenu.ComboMenu["useR"].Cast<CheckBox>().CurrentValue;
            var useWMode = GameMenu.MiscMenu["useWMode"].Cast<Slider>().CurrentValue;

            if (useRGap && Fizz.Q.IsReady() && Fizz.W.IsReady() && Fizz.E.IsReady() && Fizz.R.IsReady() &&
                Me.Distance(target) < (Fizz.Q.Range - 50) + (Fizz.E.Range + 350) && ComboDamage(target) > target.Health &&
                Fizz.R.GetPrediction(target).HitChance >= HitChance.High)
            {
                CastR(target, HitChance.High);
                Fizz.E.Cast(Me.ServerPosition.Extend(target.ServerPosition, Fizz.E.Range - 1).To3D());
                Fizz.E.Cast(Me.ServerPosition.Extend(target.ServerPosition, Fizz.E.Range - 1).To3D());
                Fizz.Q.Cast(target);
                Fizz.W.Cast();
                return;
            }

            if (useR && Fizz.R.IsReady() && Fizz.R.GetPrediction(target).HitChance > HitChance.High)
            {
                if (ComboDamage(target) - 30 > target.Health)
                {
                    CastR(target, HitChance.High);
                }

                if (Me.GetSpellDamage(target, SpellSlot.R) - 30 > target.Health)
                {
                    CastR(target, HitChance.High);
                    return;
                }
            }

            if (Fizz.E.IsReady() && useE && useQ && Me.Distance(target) <= (Fizz.E.Range + 350) &&
                Me.Mana >= Me.Spellbook.GetSpell(SpellSlot.Q).SData.Mana + Me.Spellbook.GetSpell(SpellSlot.E).SData.Mana &&
                Fizz.Q.IsReady(2) && Me.GetSpellDamage(target, SpellSlot.Q) + 50 > target.Health && useEGap)
            {
                Fizz.E.Cast(Me.ServerPosition.Extend(target.ServerPosition, Fizz.E.Range - 1).To3D());
                Fizz.E.Cast(Me.ServerPosition.Extend(target.ServerPosition, Fizz.E.Range - 1).To3D());
                Fizz.Q.Cast(target);
                Fizz.W.Cast();
                return;
            }

            if (Fizz.E.IsReady() && useE && useQ && Me.Distance(target) <= Fizz.E.Range &&
                Me.Mana >= Me.Spellbook.GetSpell(SpellSlot.Q).SData.Mana + Me.Spellbook.GetSpell(SpellSlot.E).SData.Mana &&
                Fizz.Q.IsReady(2))
            {
                Fizz.E.Cast(target);
            }

            if (useE && Fizz.E.IsReady())
            {
                Fizz.E.Cast(target);
            }

            if (useW && useWMode == 0 && Fizz.W.IsReady())
            {
                Fizz.W.Cast();
            }

            if (Fizz.Q.IsReady() && useQ)
            {
                Fizz.Q.Cast(target);
            }

            if (useW && useWMode == 0 && Fizz.Q.IsReady(1) && Fizz.W.IsReady() &&
                Me.Mana >= Me.Spellbook.GetSpell(SpellSlot.W).SData.Mana + Me.Spellbook.GetSpell(SpellSlot.Q).SData.Mana)
            {
                Fizz.Q.Cast();
                Fizz.W.Cast();
                Fizz.E.Cast(target);
                if (Me.GetSpellDamage(target, SpellSlot.R) > target.Health)
                {
                    CastR(target, HitChance.High);
                    return;
                }
            }


            if (useW && useWMode == 1 && Fizz.W.IsReady() && Me.IsInAutoAttackRange(target))
            {
                Fizz.W.Cast();
            }
        }

        // ReSharper disable once InconsistentNaming
        public static void QRCombo()
        {
            Orbwalker.OrbwalkTo(Game.CursorPos);
            var target = TargetSelector.GetTarget(Fizz.R.Range, DamageType.Magical);
            if (!target.IsValidTarget() && !Fizz.R.IsReady(2) && !Fizz.Q.IsReady(1))
            {
                Execute();
                return;
            }

            var useQ = GameMenu.ComboMenu["useQ"].Cast<CheckBox>().CurrentValue;
            var useR = GameMenu.ComboMenu["useR"].Cast<CheckBox>().CurrentValue;

            if (useQ && useR && Fizz.Q.IsReady() && Fizz.Q.IsInRange(target) && Fizz.R.IsReady() &&
                target.IsValidTarget() &&
                Me.Mana > Me.Spellbook.GetSpell(SpellSlot.R).SData.Mana + Me.Spellbook.GetSpell(SpellSlot.Q).SData.Mana &&
                Fizz.R.GetPrediction(target).HitChance > HitChance.Medium)
            {
                Fizz.Q.Cast(target);
                CastR(target, HitChance.Medium);
                Execute();
            }
        }

        private static void CastR(Obj_AI_Base target, HitChance h)
        {
            if (Fizz.R.GetPrediction(target).HitChance >= h)
            {
                var pred = Fizz.R.GetPrediction(target).CastPosition;
                Fizz.R.Cast(pred);
            }
        }

        public static float ComboDamage(Obj_AI_Base target)
        {
            var damage = 0d;

            if (Fizz.Q.IsReady(3))
            {
                damage += Me.GetSpellDamage(target, SpellSlot.Q);
            }

            if (Fizz.W.IsReady(5))
            {
                damage += Me.GetSpellDamage(target, SpellSlot.W);
            }

            if (Fizz.E.IsReady(2))
            {
                damage += Me.GetSpellDamage(target, SpellSlot.E);
            }

            if (Fizz.R.IsReady(5))
            {
                damage += Me.GetSpellDamage(target, SpellSlot.R);
            }

            damage += Me.GetAutoAttackDamage(target)*3;
            return (float) damage;
        }
    }
}