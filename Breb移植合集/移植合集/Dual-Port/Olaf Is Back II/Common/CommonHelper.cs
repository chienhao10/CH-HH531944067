using System;
using System.Collections.Generic;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using SharpDX.Direct3D9;
using Font = SharpDX.Direct3D9.Font;
using EloBuddy;

namespace OlafxQx.Common
{

    public static class CommonHelper
    {

        public static List<BuffDatabase> BuffDb = new List<BuffDatabase>();

        public class BuffDatabase
        {
            public string BuffName;

            public BuffDatabase() { }

            public BuffDatabase(string buffName)
            {
                BuffName = buffName;
            }
        }

   
        public static Font Text, TextLittle;
        public static string Tab => "       ";

        public static void Init()
        {
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

            BuffDb.Add(new BuffDatabase
            {
                BuffName = "JudicatorIntervention"
            });

            BuffDb.Add(new BuffDatabase
            {
                BuffName = "Undying Rage"
            });

            Drawing.OnPreReset += DrawingOnOnPreReset;
            Drawing.OnPostReset += DrawingOnOnPostReset;
            AppDomain.CurrentDomain.DomainUnload += CurrentDomainOnDomainUnload;
            AppDomain.CurrentDomain.ProcessExit += CurrentDomainOnDomainUnload;
        }

        private static void CurrentDomainOnDomainUnload(object sender, EventArgs eventArgs)
        {
            Text.Dispose();
            TextLittle.Dispose();
        }

        private static void DrawingOnOnPostReset(EventArgs args)
        {
            Text.OnResetDevice();
            TextLittle.OnResetDevice();
        }

        private static void DrawingOnOnPreReset(EventArgs args)
        {
            Text.OnLostDevice();
            TextLittle.OnLostDevice();
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

        public static void DrawText(Font vFont, string vText, float vPosX, float vPosY, ColorBGRA vColor)
        {
            vFont.DrawText(null, vText, (int) vPosX, (int) vPosY, vColor);
        }

        public static bool ShouldCastSpell(Obj_AI_Base t)
        {
            return !t.LSIsValidTarget(Orbwalking.GetRealAutoAttackRange(t) + 65) || !ObjectManager.Player.HasSheenBuff();
        }


        public static bool HaveImmortalBuff(this Obj_AI_Base obj)
        {
            return (from b in obj.Buffs join b1 in BuffDb on b.DisplayName equals b1.BuffName select new { b, b1 }).Distinct().Any();
        }


        public static bool HaveOlafSlowBuff(this Obj_AI_Base t)
        {
            return t.Buffs.Any(buff => buff.Name == "olafslow"); 
        }
        public static bool HasSheenBuff(this Obj_AI_Base obj)
        {
            return obj.Buffs.Any(buff => buff.Name.ToLower() == "sheen");
        }

        public static bool OlafHaveFrenziedStrikes
        {
            get { return ObjectManager.Player.Buffs.Any(buff => buff.DisplayName == "OlafFrenziedStrikes"); }
        }

        public static bool OlafHaveRagnarok
        {
            get { return ObjectManager.Player.Buffs.Any(buff => buff.DisplayName == "OlafRagnarok"); }
        }

        public static bool OlafHasAttackSpeedBuff
        {
            get { return ObjectManager.Player.Buffs.Any(buff => buff.DisplayName == "OlafFrenziedStrikes") || ObjectManager.Player.Buffs.Any(buff => buff.DisplayName == "SpectralFury"); }
        }

        public static bool HasPassive(this Obj_AI_Base obj)
        {
            return obj.PassiveCooldownEndTime - (Game.Time - 15.5) <= 0;
        }

        public static bool HasBuffInst(this Obj_AI_Base obj, string buffName)
        {
            return obj.Buffs.Any(buff => buff.DisplayName == buffName);
        }

        public static bool HasBlueBuff(this Obj_AI_Base obj)
        {
            return obj.Buffs.Any(buff => buff.DisplayName == "CrestoftheAncientGolem");
        }

        public static bool HasRedBuff(this Obj_AI_Base obj)
        {
            return obj.Buffs.Any(buff => buff.DisplayName == "BlessingoftheLizardElder");
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