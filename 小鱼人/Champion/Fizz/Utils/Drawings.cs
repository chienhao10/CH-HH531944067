#region

using System;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK.Rendering;
using SharpDX;
using Color = System.Drawing.Color;

#endregion

namespace SimplisticTemplate.Champion.Fizz.Utils
{
    internal static class Drawings
    {
        private static readonly Text KillableText = new Text("",
            new System.Drawing.Font(System.Drawing.FontFamily.GenericSansSerif, 9, System.Drawing.FontStyle.Bold));

        private static AIHeroClient Me
        {
            get { return ObjectManager.Player; }
        }

        public static void OnDraw(EventArgs args)
        {
            var disable = GameMenu.DrawMenu["disable"].Cast<CheckBox>().CurrentValue;
            var drawQ = GameMenu.DrawMenu["drawQ"].Cast<CheckBox>().CurrentValue;
            var drawW = GameMenu.DrawMenu["drawW"].Cast<CheckBox>().CurrentValue;
            var drawE = GameMenu.DrawMenu["drawE"].Cast<CheckBox>().CurrentValue;
            var drawR = GameMenu.DrawMenu["drawR"].Cast<CheckBox>().CurrentValue;
            var drawRPred = GameMenu.DrawMenu["drawRPred"].Cast<CheckBox>().CurrentValue;
            if (disable) return;

            if (drawQ && Fizz.Q.IsReady())
            {
                new Circle {Color = Color.White, Radius = Fizz.Q.Range, BorderWidth = 2f}.Draw(Me.Position);
            }

            if (drawW && Fizz.W.IsReady())
            {
                new Circle {Color = Color.White, Radius = Fizz.W.Range, BorderWidth = 2f}.Draw(Me.Position);
            }

            if (drawE && Fizz.E.IsReady())
            {
                new Circle {Color = Color.White, Radius = Fizz.E.Range, BorderWidth = 2f}.Draw(Me.Position);
            }

            if (drawR && Fizz.R.IsReady())
            {
                new Circle {Color = Color.Crimson, Radius = Fizz.R.Range, BorderWidth = 2f}.Draw(Me.Position);
            }
            var target = TargetSelector.GetTarget(Fizz.R.Range, DamageType.Magical);
            if (drawRPred && Fizz.R.IsReady() && target.IsValidTarget())
            {
                new Circle {Color = Color.Crimson, Radius = Fizz.R.Width, BorderWidth = 2f}.Draw(
                    Fizz.R.GetPrediction(target).CastPosition);
            }
        }

        public static void OnDamageDraw(EventArgs args)
        {
            var disable = GameMenu.DrawMenu["disable"].Cast<CheckBox>().CurrentValue;
            var drawDamage = GameMenu.DrawMenu["drawDamage"].Cast<CheckBox>().CurrentValue;
            if (disable) return;

            if (drawDamage)
            {
                foreach (var ai in EntityManager.Heroes.Enemies)
                {
                    if (ai.IsValidTarget())
                    {
                        var drawn = 0;
                        if (Modes.Combo.ComboDamage(ai) >= ai.Health && drawn == 0)
                        {
                            KillableText.Position = Drawing.WorldToScreen(ai.Position) - new Vector2(40, -40);
                            KillableText.Color = Color.Firebrick;
                            KillableText.TextValue = "100% Killable";
                            KillableText.Draw();
                            drawn = 1;
                        }

                        if (Modes.Combo.ComboDamage(ai) + 300 >= ai.Health && drawn == 0)
                        {
                            KillableText.Position = Drawing.WorldToScreen(ai.Position) - new Vector2(40, -40);
                            KillableText.Color = Color.AntiqueWhite;
                            KillableText.TextValue = "50% Killable - HP Left: " +
                                                     (Math.Abs((int) ai.Health - (int) Modes.Combo.ComboDamage(ai)));
                            KillableText.Draw();
                            drawn = 1;
                        }

                        if (Modes.Combo.ComboDamage(ai) < ai.Health && drawn == 0)
                        {
                            KillableText.Position = Drawing.WorldToScreen(ai.Position) - new Vector2(40, -40);
                            KillableText.Color = Color.ForestGreen;
                            KillableText.TextValue = "Not Killable - HP Left: " +
                                                     (Math.Abs((int) ai.Health - (int) Modes.Combo.ComboDamage(ai)));
                            KillableText.Draw();
                        }
                    }
                }
            }
        }
    }
}