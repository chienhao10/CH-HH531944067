namespace Lulu
{
    using System;
    using System.Linq;

    using EloBuddy;
    using EloBuddy.SDK.Menu.Values;
    using EloBuddy.SDK.Rendering;

    using SharpDX;

    public static class PixManager
    {
        public static bool DrawPix { get; set; }

        private static Obj_AI_Base _pix = null;

        public const string PixObjectName = "RobotBuddy";

        public static Obj_AI_Base Pix
        {
            get
            {
                if (_pix != null && _pix.IsValid)
                {
                    return _pix;
                }

                return null;
            }
        }

        static PixManager()
        {
            Game.OnUpdate += Game_OnUpdate;
            Drawing.OnDraw += Drawing_OnEndScene;

            _pix = ObjectManager.Get<Obj_AI_Base>().FirstOrDefault(o => o.IsAlly && o.Name == PixObjectName);
            if (Pix == null)
            {
                return;
            }
        }

        private static void Game_OnUpdate(EventArgs args)
        {
            if (Pix == null)
            {
                _pix = ObjectManager.Get<Obj_AI_Base>().FirstOrDefault(o => o.IsAlly && o.Name == PixObjectName);
            }
        }

        private static void Drawing_OnEndScene(EventArgs args)
        {
            if (!Program.menuIni.Get<CheckBox>("Drawings").CurrentValue)
            {
                return;
            }

            if (Program.DrawMenu["PixQ"].Cast<CheckBox>().CurrentValue && DrawPix)
            {
                if (Pix != null)
                {
                    Circle.Draw(Color.MediumPurple, Program.Q.Range, Pix.Position);
                    Drawing.DrawCircle(Pix.Position + new Vector3(0, 0, 15), 150, System.Drawing.Color.Purple);
                }
            }

            if (Program.DrawMenu["PixP"].Cast<CheckBox>().CurrentValue && DrawPix && Pix != null)
            {
                Drawing.DrawCircle(Pix.Position + new Vector3(0, 0, 15), 150, System.Drawing.Color.Purple);
            }
        }
    }
}