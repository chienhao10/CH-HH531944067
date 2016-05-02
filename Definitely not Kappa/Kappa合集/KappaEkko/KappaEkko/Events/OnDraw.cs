namespace KappaEkko.Events
{
    using System;

    using EloBuddy;
    using EloBuddy.SDK;
    using EloBuddy.SDK.Menu.Values;
    using EloBuddy.SDK.Rendering;

    using SharpDX;

    internal class OnDraw
    {
        public static void Draw(EventArgs args)
        {
            var hpPos = ObjectManager.Player.HPBarPosition;
            if (Menu.DrawMenu.Get<CheckBox>("Q").CurrentValue && Spells.Q.IsLearned)
            {
                Circle.Draw(Color.Purple, Spells.Q.Range, ObjectManager.Player.Position);
            }

            if (Menu.DrawMenu.Get<CheckBox>("W").CurrentValue && Spells.W.IsLearned)
            {
                Circle.Draw(Color.Purple, Spells.W.Range, ObjectManager.Player.Position);
            }

            if (Menu.DrawMenu.Get<CheckBox>("E").CurrentValue && Spells.E.IsLearned)
            {
                Circle.Draw(Color.Purple, Spells.E.Range, ObjectManager.Player.Position);
            }

            if (Menu.DrawMenu.Get<CheckBox>("R").CurrentValue && Spells.R.IsLearned && Spells.EkkoREmitter != null)
            {
                Circle.Draw(Color.Purple, Spells.R.Range, Spells.EkkoREmitter.Position);
                Drawing.DrawText(
                    hpPos.X + 140f,
                    hpPos.Y + 5,
                    System.Drawing.Color.White,
                    "R Will Hit " + Spells.EkkoREmitter.Position.CountEnemiesInRange(Spells.R.Range),
                    10);
            }

            if (Menu.DrawMenu.Get<CheckBox>("debug").CurrentValue)
            {
                var Wtarget = TargetSelector.GetTarget(Spells.W.Range, DamageType.Magical);
                if (Menu.ComboMenu.Get<CheckBox>("Wpred").CurrentValue && Wtarget != null)
                {
                    var pred = Spells.W.GetPrediction(Wtarget);
                    Circle.Draw(Color.Purple, Spells.W.Radius, pred.CastPosition);
                    Drawing.DrawText(
                        hpPos.X + 140f,
                        hpPos.Y - 10,
                        System.Drawing.Color.White,
                        "W Will Hit " + pred.CastPosition.CountEnemiesInRange(Spells.W.Radius),
                        10);
                }

                if (Menu.ComboMenu.Get<CheckBox>("W").CurrentValue && !Menu.ComboMenu.Get<CheckBox>("Wpred").CurrentValue && Wtarget != null
                    && Spells.W.IsReady())
                {
                    Circle.Draw(Color.Purple, Spells.W.Radius, Wtarget.Position);
                    Drawing.DrawText(
                        hpPos.X + 140f,
                        hpPos.Y - 15,
                        System.Drawing.Color.White,
                        "W Will Hit " + Wtarget.Position.CountEnemiesInRange(Spells.W.Radius),
                        10);
                }
            }
        }
    }
}