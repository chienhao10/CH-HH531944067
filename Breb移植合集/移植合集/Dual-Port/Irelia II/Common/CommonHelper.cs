using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using SharpDX.Direct3D9;
using Font = SharpDX.Direct3D9.Font;
using EloBuddy;

namespace Irelia.Common
{

    public static class CommonHelper
    {
        public static Font TextStatus, Text, TextLittle;
        public static string Tab => "       ";

        public static void Init()
        {
            TextStatus = new Font(Drawing.Direct3DDevice,
                new FontDescription
                {
                    FaceName = "Segoe UI",
                    Height = 17,
                    OutputPrecision = FontPrecision.Default,
                    Quality = FontQuality.ClearTypeNatural,
                    Weight = FontWeight.Bold
                });

            Text = new Font(Drawing.Direct3DDevice,
                new FontDescription
                {
                    FaceName = "Segoe UI",
                    Height = 19,
                    OutputPrecision = FontPrecision.Default,
                    Quality = FontQuality.ClearTypeNatural,
                });

            TextLittle = new Font(Drawing.Direct3DDevice,
                new FontDescription
                {
                    FaceName = "Segoe UI",
                    Height = 15,
                    OutputPrecision = FontPrecision.Default,
                    Quality = FontQuality.ClearTypeNatural,
                });
        }

        public static string FormatTime(double time)
        {
            TimeSpan t = TimeSpan.FromSeconds(time);
            if (t.Minutes > 0)
            {
                return $"{t.Minutes:D1}:{t.Seconds:D2}";
            }
            return $"{t.Seconds:D}";
        }

        public static Vector3 CenterOfVectors(Vector3[] vectors)
        {
            var sum = Vector3.Zero;
            if (vectors == null || vectors.Length == 0)
                return sum;

            sum = vectors.Aggregate(sum, (current, vec) => current + vec);
            return sum / vectors.Length;
        }



        public static void DrawText(Font vFont, string vText, float vPosX, float vPosY, ColorBGRA vColor)
        {
            vFont.DrawText(null, vText, (int) vPosX, (int) vPosY, vColor);
        }

        public static bool ShouldCastSpell(Obj_AI_Base t)
        {
            return !t.LSIsValidTarget(Orbwalking.GetRealAutoAttackRange(t) + 65) || !ObjectManager.Player.HasSheenBuff();
        }
    }

    public static class Colors
    {
        public static SharpDX.Color SubMenu => SharpDX.Color.GreenYellow;
        public static SharpDX.Color ColorMana => SharpDX.Color.Aquamarine;
        public static SharpDX.Color ColorItems => SharpDX.Color.Cornsilk;
        public static SharpDX.Color ColorWarning => SharpDX.Color.IndianRed;
        public static SharpDX.Color ColorPermaShow => SharpDX.Color.Aqua;

        public static SharpDX.Color MenuColor(this Spell spell)
        {
            switch (spell.Slot)
            {
                case SpellSlot.Q:
                {
                    return SharpDX.Color.LightSalmon;
                }

                case SpellSlot.W:
                {
                    return SharpDX.Color.DarkSeaGreen;
                }

                case SpellSlot.E:
                {
                    return SharpDX.Color.Aqua;
                }

                case SpellSlot.R:
                {
                    return SharpDX.Color.Yellow;
                }
            }

            return SharpDX.Color.Wheat;
        }

        public static void DrawRange(this Spell spell, System.Drawing.Color color, bool draw = true,
            bool checkCoolDown = false)
        {
            if (!draw)
            {
                return;
            }

            if (checkCoolDown)
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, spell.Range,
                    spell.IsReady() ? color : System.Drawing.Color.Gray,
                    spell.IsReady() ? 5 : 1);
            }
            else
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, spell.Range, color, 1);
            }
        }
    }
}