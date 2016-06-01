using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EloBuddy.SDK.Rendering;
using SharpDX;
using EloBuddy;
using EloBuddy.SDK;


namespace Auto_Carry_Vayne.Features.Utility
{
    class drawing
    {
        public static void Load()
        {
            Drawing.OnDraw += OnDraw;
        }

        public static void OnDraw(EventArgs args)
        {
            if (Manager.MenuManager.DrawE)
            {
                if (!(Manager.MenuManager.DrawOnlyRdy && !Manager.SpellManager.E.IsReady()))
                {
                    Circle.Draw(Color.Red, Manager.SpellManager.E.Range, Variables._Player.Position);
                }
            }
            if (Manager.MenuManager.DrawQ)
            {
                if (!(Manager.MenuManager.DrawOnlyRdy && !Manager.SpellManager.Q.IsReady()))
                {
                    Circle.Draw(Color.Red, Manager.SpellManager.Q.Range, Variables._Player.Position);
                }
            }
            if (Manager.MenuManager.DrawCondemn)
            {
                var t = TargetSelector.GetTarget(Manager.SpellManager.E.Range + Manager.SpellManager.Q.Range, DamageType.Physical);
                if (t.IsValidTarget())
                {
                    var color = System.Drawing.Color.Red;
                    for (var i = 1; i < 8; i++)
                    {
                        var targetBehind = t.Position +
                                           Vector3.Normalize(t.ServerPosition - ObjectManager.Player.Position) * i * 50;

                        if (!targetBehind.IsWall())
                        {
                            color = System.Drawing.Color.Aqua;
                        }
                        else
                        {
                            color = System.Drawing.Color.Red;
                        }
                    }

                    var tt = t.Position + Vector3.Normalize(t.ServerPosition - ObjectManager.Player.Position) * 8 * 50;

                    var startpos = t.Position;
                    var endpos = tt;
                    var endpos1 = tt +
                                  (startpos - endpos).To2D().Normalized().Rotated(45 * (float)Math.PI / 180).To3D() *
                                  t.BoundingRadius;
                    var endpos2 = tt +
                                  (startpos - endpos).To2D().Normalized().Rotated(-45 * (float)Math.PI / 180).To3D() *
                                  t.BoundingRadius;

                    var width = 2;

                    var x = new Geometry.Polygon.Line(startpos, endpos);
                    {
                        x.Draw(color, width);
                    }

                    var y = new Geometry.Polygon.Line(endpos, endpos1);
                    {
                        y.Draw(color, width);
                    }

                    var z = new Geometry.Polygon.Line(endpos, endpos2);
                    {
                        z.Draw(color, width);
                    }
                }
            }
            if (Manager.MenuManager.DrawTumble)
            {

                var startpos = Variables._Player.Position.Extend(Variables.EndPosition, 10);
                var endpos = startpos.Extend(Variables.EndPosition, Manager.SpellManager.Q.Range);
                var endpos1 = (Vector3)endpos +
              (startpos - endpos).Normalized().Rotated(45 * (float)Math.PI / 180).To3D() *
              Variables._Player.BoundingRadius;
                var endpos2 = (Vector3)endpos +
                              (startpos - endpos).Normalized().Rotated(-45 * (float)Math.PI / 180).To3D() *
                              Variables._Player.BoundingRadius;
                var width = 2;
                var color = System.Drawing.Color.Red;

                var x = new Geometry.Polygon.Line(startpos, endpos);
                {
                    x.Draw(color, width);
                }

                var y = new Geometry.Polygon.Line((Vector3)endpos, endpos1);
                {
                    y.Draw(color, width);
                }

                var z = new Geometry.Polygon.Line((Vector3)endpos, endpos2);
                {
                    z.Draw(color, width);
                }

                return;
            }
        }

    }
}

