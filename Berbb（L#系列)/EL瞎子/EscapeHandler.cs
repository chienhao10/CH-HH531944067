namespace LeeSin
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using EloBuddy;
    using EloBuddy.SDK;
    using EloBuddy.SDK.Enumerations;
    using EloBuddy.SDK.Events;
    using EloBuddy.SDK.Menu;
    using EloBuddy.SDK.Menu.Values;
    using SharpDX;
    using Color = System.Drawing.Color;

    internal class JumpHandler
    {
        #region Static Fields

        public static bool InitQ; 

        private static readonly List<Vector3> JunglePos = new List<Vector3>()
                                                              {
                                                                  new Vector3(6271.479f, 12181.25f, 56.47668f),
                                                                  new Vector3(6971.269f, 10839.12f, 55.2f),
                                                                  new Vector3(8006.336f, 9517.511f, 52.31763f),
                                                                  new Vector3(10995.34f, 8408.401f, 61.61731f),
                                                                  new Vector3(10895.08f, 7045.215f, 51.72278f),
                                                                  new Vector3(12665.45f, 6466.962f, 51.70544f),
                                                                  new Vector3(4966.042f, 10475.51f, 71.24048f),
                                                                  new Vector3(39000.529f, 7901.832f, 51.84973f),
                                                                  new Vector3(2106.111f, 8388.643f, 51.77686f),
                                                                  new Vector3(3753.737f, 6454.71f, 52.46301f),
                                                                  new Vector3(6776.247f, 5542.872f, 55.27625f),
                                                                  new Vector3(7811.688f, 4152.602f, 53.79456f),
                                                                  new Vector3(8528.921f, 2822.875f, 50.92188f),
                                                                  new Vector3(9850.102f, 4432.272f, 71.24072f),
                                                                  new Vector3(3926f, 7918f, 51.74162f)
                                                              };

        private static bool active;

        private static Geometry.Polygon rect;

        #endregion

        #region Public Properties

        public static Obj_AI_Base BuffedEnemy
        {
            get
            {
                return ObjectManager.Get<Obj_AI_Base>().FirstOrDefault(unit => unit.IsEnemy && unit.HasQBuff());
            }
        }

        #endregion

        #region Properties

        private static AIHeroClient Player
        {
            get
            {
                return ObjectManager.Player;
            }
        }

        #endregion

        #region Public Methods and Operators

        public static void Load()
        {
            Drawing.OnDraw += args => Draw();
            Game.OnUpdate += args => Tick();
        }

        #endregion

        #region Methods

        private static void Draw()
        {
            /*
            if (!InitMenu.Menu.Item("escapeMode").GetValue<bool>() || !InitMenu.Menu.Item("ElLeeSin.Draw.Escape").GetValue<bool>())
            {
                return;
            }

            if (active && Program.Q.IsReady() && InitMenu.Menu.Item("ElLeeSin.Draw.Q.Width").GetValue<bool>())
            {
                rect.Draw(Color.White);
            }
            foreach (var pos in JunglePos)
            {
                if (rect != null)
                {
                    if (pos.Distance(Player.Position) < 2000)
                    {
                        Drawing.DrawCircle(pos, 100, (rect.IsOutside(pos.To2D()) ? Color.White : Color.DeepSkyBlue));
                    }
                }
                else
                {
                    if (pos.Distance(Player.Position) < 2000)
                    {
                        Drawing.DrawCircle(pos, 100, Color.White);
                    }
                }
            }
             */
        }

        private static void Escape()
        {

            Program.Orbwalk(Game.CursorPos);

            if (BuffedEnemy.IsValidTarget() && BuffedEnemy.IsValidTarget())
            {
                InitQ = false;
                return;
            }
            if (InitQ)
            {
                foreach (var point in JunglePos)
                {
                    if (Player.Distance(point) < 100 || Program.LastQ2 + 2000 < Environment.TickCount)
                    {
                        InitQ = false;
                    }
                }
            }

            rect = new Geometry.Polygon.Rectangle(
                Player.Position.To2D(),
                Player.Position.To2D().Extend(Game.CursorPos.To2D(), 1050),
                100);

            if (Program.QState && Program.Q.IsReady())
            {
                foreach (var pos in JunglePos)
                {
                    if (rect.IsOutside(pos.To2D()))
                    {
                        continue;
                    }
                    InitQ = true;
                    Program.Q.Cast(pos);
                    return;
                }
            }
            else if (Program.Q.IsReady() && !Program.QState)
            {
                Program.Q.Cast();
                InitQ = true;
            }
        }

        private static void Tick()
        {
            /*
            if (InitMenu.Menu.Item("ElLeeSin.Escape").GetValue<KeyBind>().Active && InitMenu.Menu.Item("escapeMode").GetValue<bool>())
            {
                Escape();
                active = true;
            }
            else
            {
                active = false;
            }
             */
        }

        #endregion
    }
}