namespace KappaKindred.Events
{
    using System;

    using EloBuddy;
    using EloBuddy.SDK.Menu.Values;
    using EloBuddy.SDK.Rendering;

    using SharpDX;

    internal class OnDraw
    {
        public static void Draw(EventArgs args)
        {
            if (Menu.DrawMenu.Get<CheckBox>("Q").CurrentValue && Spells.Q.IsLearned)
            {
                if (Spells.Q.IsReady())
                {
                    Circle.Draw(Color.OrangeRed, Spells.Q.Range, Player.Instance.Position);
                }

                if (!Spells.Q.IsReady())
                {
                    Circle.Draw(Color.DarkRed, Spells.Q.Range, ObjectManager.Player.Position);
                }
            }

            if (Menu.DrawMenu.Get<CheckBox>("W").CurrentValue && Spells.W.IsLearned)
            {
                if (Spells.W.IsReady())
                {
                    Circle.Draw(Color.OrangeRed, Spells.W.Range, ObjectManager.Player.Position);
                }

                if (!Spells.W.IsReady())
                {
                    Circle.Draw(Color.DarkRed, Spells.W.Range, ObjectManager.Player.Position);
                }
            }

            if (Menu.DrawMenu.Get<CheckBox>("E").CurrentValue && Spells.E.IsLearned)
            {
                if (Spells.E.IsReady())
                {
                    Circle.Draw(Color.OrangeRed, Spells.E.Range, ObjectManager.Player.Position);
                }

                if (!Spells.E.IsReady())
                {
                    Circle.Draw(Color.DarkRed, Spells.E.Range, ObjectManager.Player.Position);
                }
            }

            if (Menu.DrawMenu.Get<CheckBox>("R").CurrentValue && Spells.R.IsLearned)
            {
                if (Spells.R.IsReady())
                {
                    Circle.Draw(Color.OrangeRed, Spells.R.Range, ObjectManager.Player.Position);
                }

                if (!Spells.R.IsReady())
                {
                    Circle.Draw(Color.DarkRed, Spells.R.Range, ObjectManager.Player.Position);
                }
            }
        }
    }
}