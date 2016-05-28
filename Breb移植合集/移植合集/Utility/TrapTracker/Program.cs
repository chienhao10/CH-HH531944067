using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EloBuddy;
using EloBuddy.SDK;
using LeagueSharp.Common;
using SharpDX;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;

namespace AntiTrap
{
    class Program
    {
        public static Menu Config;
        private static AIHeroClient Player { get { return ObjectManager.Player; } }


        public static void Game_OnGameLoad()
        {
            Config = MainMenu.AddMenu("陷阱计时器", "AntiTrap");
            Config.Add("Jinxe", new CheckBox("金科丝 E"));
            Config.Add("caitw", new CheckBox("女警 W"));
            Config.Add("teemor", new CheckBox("提莫 R"));
            Config.Add("Draw", new CheckBox("先显示"));

            Drawing.OnDraw += Drawing_OnDraw;
        }

        public static bool getCheckBoxItem(string item)
        {
            return Config[item].Cast<CheckBox>().CurrentValue;
        }

        private static void Drawing_OnDraw(EventArgs args)
        {
            if ((int)(Game.Time * 10) % 2 == 0)
                return;

            foreach (var obj in ObjectManager.Get<Obj_GeneralParticleEmitter>().Where(obj => obj.IsValid))
            {

                var distance = obj.Position.LSDistance(ObjectManager.Player.Position);
                if (distance > 1500)
                    continue;

                var name = obj.Name.ToLower();

                if (name.Contains("yordleTrap_idle_red.troy".ToLower()) && getCheckBoxItem("caitw"))
                {
                    if (distance < 200)
                        TryDodge(obj.Position, 200);
                    if (getCheckBoxItem("Draw"))
                        Render.Circle.DrawCircle(obj.Position, 100, System.Drawing.Color.OrangeRed, 1);
                }
            }

            foreach (var obj in ObjectManager.Get<Obj_AI_Minion>().Where(obj => obj.IsValid && obj.IsEnemy))
            {
                var distance = obj.Position.LSDistance(ObjectManager.Player.Position);
                if (distance > 1500)
                    continue;
                if (obj.Name == "k" && getCheckBoxItem("Jinxe"))
                {
                    if (distance < 260)
                        TryDodge(obj.Position, 260);
                    if (getCheckBoxItem("Draw"))
                        Render.Circle.DrawCircle(obj.Position, 100, System.Drawing.Color.OrangeRed, 1);
                }
                if (obj.Name == "Noxious Trap")
                {
                    if (distance < 240)
                        TryDodge(obj.Position, 240);
                    if (getCheckBoxItem("Draw"))
                        Render.Circle.DrawCircle(obj.Position, 100, System.Drawing.Color.OrangeRed, 1);
                }
            }
        }

        private static void TryDodge(Vector3 position, float range)
        {
            var points = CirclePoints(15, 100, Player.Position);
            var bestPoint = points.Where(x => x.LSDistance(position) > range).OrderBy(y => y.LSDistance(Game.CursorPos)).FirstOrDefault();

            if (bestPoint != null)
                EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, bestPoint);

        }

        public static List<Vector3> CirclePoints(float CircleLineSegmentN, float radius, Vector3 position)
        {
            List<Vector3> points = new List<Vector3>();
            for (var i = 1; i <= CircleLineSegmentN; i++)
            {
                var angle = i * 2 * Math.PI / CircleLineSegmentN;
                var point = new Vector3(position.X + radius * (float)Math.Cos(angle), position.Y + radius * (float)Math.Sin(angle), position.Z);
                points.Add(point);
            }
            return points;
        }
    }
}
