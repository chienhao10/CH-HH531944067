using System;
using System.Collections.Generic;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using Color = System.Drawing.Color;
using EloBuddy;
using EloBuddy.SDK.Menu.Values;

namespace ChewyMoonsShaco
{
    internal class Illuminati
    {
        private const float BoxSafeDistance = 100;
        public static float TriangleLegDistance = ChewyMoonShaco.getSliderItem(ChewyMoonShaco.illuminatiMenu, "BoxDistance");
        private static readonly List<Obj_AI_Minion> Boxes = new List<Obj_AI_Minion>();
        private static Vector3 _extendPoint;

        public static void Init()
        {
            // Add all existing boxes
            Boxes.AddRange(ObjectManager.Get<Obj_AI_Minion>().Where(x => x.BaseSkinName == "shacobox"));

            GameObject.OnCreate += (sender, args) =>
            {
                if (!(sender is Obj_AI_Minion))
                {
                    return;
                }

                var @object = (Obj_AI_Minion) sender;

                if (@object.BaseSkinName == "shacobox")
                {
                    Boxes.Add(@object);
                }
            };

            GameObject.OnDelete += (sender, args) =>
            {
                if (!(sender is Obj_AI_Minion))
                {
                    return;
                }

                var @object = (Obj_AI_Minion) sender;

                if (@object.BaseSkinName == "shacobox")
                {
                    Boxes.RemoveAll(x => x.NetworkId == @object.NetworkId);
                }
            };

            Drawing.OnDraw += DrawingOnOnDraw;

            _extendPoint = ObjectManager.Player.ServerPosition;

            Game.OnUpdate += GameOnOnUpdate;
        }

        private static void GameOnOnUpdate(EventArgs args)
        {
            if (!ChewyMoonShaco.getCheckBoxItem(ChewyMoonShaco.illuminatiMenu, "RepairTriangle"))
            {
                return;
            }

            foreach (var shacoBox in Boxes)
            {
                var angle = 120;

                var point = RotateAroundPoint(
                    angle, shacoBox.Position.LSTo2D(), shacoBox.Position.LSExtend(_extendPoint, TriangleLegDistance));

                if (!Boxes.Any(x => x.LSDistance(point) < BoxSafeDistance) && ChewyMoonShaco.W.IsInRange(point))
                {
                    ChewyMoonShaco.W.Cast(point);
                }
                else
                {
                    angle = -120;

                    point = RotateAroundPoint(
                        angle, shacoBox.Position.LSTo2D(), shacoBox.Position.LSExtend(_extendPoint, TriangleLegDistance));

                    if (!Boxes.Any(x => x.LSDistance(point) < BoxSafeDistance) && ChewyMoonShaco.W.IsInRange(point))
                    {
                        ChewyMoonShaco.W.Cast(point);
                    }
                    else
                    {
                        angle = 240;

                        point = RotateAroundPoint(
                            angle, shacoBox.Position.LSTo2D(), shacoBox.Position.LSExtend(_extendPoint, TriangleLegDistance));

                        if (!Boxes.Any(x => x.LSDistance(point) < BoxSafeDistance) && ChewyMoonShaco.W.IsInRange(point))
                        {
                            ChewyMoonShaco.W.Cast(point);
                        }
                    }
                }
            }
        }

        private static Vector2 RotateAroundPoint(double angleDegree, Vector2 center, Vector3 point)
        {
            var angle = angleDegree * Math.PI / 180;

            var rotatedX = Math.Cos(angle) * (point.X - center.X) - Math.Sin(angle) * (point.Y - center.Y) + center.X;
            var rotatedY = Math.Sin(angle) * (point.X - center.X) + Math.Cos(angle) * (point.Y - center.Y) + center.Y;

            return new Vector2((float) rotatedX, (float) rotatedY);
        }

        public static void PlaceInitialBox()
        {
            ChewyMoonShaco.W.Cast(ObjectManager.Player.Position.LSExtend(Game.CursorPos, ChewyMoonShaco.W.Range));
        }

        private static void DrawingOnOnDraw(EventArgs args)
        {
            foreach (var shacoBox in Boxes)
            {
                var angle = 0;

                var point = RotateAroundPoint(
                    angle, shacoBox.Position.LSTo2D(), shacoBox.Position.LSExtend(_extendPoint, TriangleLegDistance));

                if (!Boxes.Any(x => x.LSDistance(point) < BoxSafeDistance))
                {
                    Drawing.DrawCircle(point.To3D(), BoxSafeDistance, Color.Aqua);
                }

                angle = 120;

                point = RotateAroundPoint(
                    angle, shacoBox.Position.LSTo2D(), shacoBox.Position.LSExtend(_extendPoint, TriangleLegDistance));

                if (!Boxes.Any(x => x.LSDistance(point) < shacoBox.BoundingRadius))
                {
                    Drawing.DrawCircle(point.To3D(), BoxSafeDistance, Color.Aqua);
                }

                angle = 240;

                point = RotateAroundPoint(
                    angle, shacoBox.Position.LSTo2D(), shacoBox.Position.LSExtend(_extendPoint, TriangleLegDistance));

                if (!Boxes.Any(x => x.LSDistance(point) < shacoBox.BoundingRadius))
                {
                    Drawing.DrawCircle(point.To3D(), BoxSafeDistance, Color.Aqua);
                }

                angle = 360;

                point = RotateAroundPoint(
                    angle, shacoBox.Position.LSTo2D(), shacoBox.Position.LSExtend(_extendPoint, TriangleLegDistance));

                if (!Boxes.Any(x => x.LSDistance(point) < BoxSafeDistance))
                {
                    Drawing.DrawCircle(point.To3D(), BoxSafeDistance, Color.Aqua);
                }
            }
        }
    }
}