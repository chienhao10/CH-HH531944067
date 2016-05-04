using System;
using System.Collections.Generic;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using SharpDX.Direct3D9;
using TreeLib.Extensions;
using EloBuddy;
using EloBuddy.SDK.Menu;
using Color = System.Drawing.Color;
using EloBuddy.SDK.Menu.Values;

namespace TreeLib.Core
{
    public static class ManaBarIndicator
    {
        private const float Width = 104;

        private static readonly Device DxDevice = Drawing.Direct3DDevice;

        private static readonly Line DxLine = new Line(DxDevice) { Width = 4 };

        private static Vector2 Offset
        {
            get { return new Vector2(34, 9); }
        }

        private static Vector2 StartPosition
        {
            get
            {
                var hpPos = ObjectManager.Player.HPBarPosition + Offset;
                return new Vector2(hpPos.X, hpPos.Y + 8);
            }
        }

        private static ColorBGRA DrawColor
        {
            get { return Color.Black.ToSharpDXColor(); }
        }

        public static void Initialize(Menu menu, Dictionary<SpellSlot, int[]> manaDictionary)
        {
            menu.Add("ManaBarEnabled", new CheckBox("Draw Mana Indicator"));//, Color.Black);

            Drawing.OnPreReset += DrawingOnOnPreReset;
            Drawing.OnPostReset += DrawingOnOnPostReset;
            AppDomain.CurrentDomain.DomainUnload += CurrentDomainOnDomainUnload;
            AppDomain.CurrentDomain.ProcessExit += CurrentDomainOnDomainUnload;

            Drawing.OnEndScene += eventArgs =>
            {
                if (ObjectManager.Player.IsDead || !menu["ManaBarEnabled"].Cast<CheckBox>().CurrentValue)
                {
                    return;
                }

                var spell = ObjectManager.Player.Spellbook;
                var totalMana = manaDictionary.Sum(kvp => kvp.Value[spell.GetSpell(kvp.Key).Level]);

                DrawManaPercent(
                    totalMana, totalMana > ObjectManager.Player.Mana ? new ColorBGRA(255, 0, 0, 255) : DrawColor);
            };
        }

        private static void CurrentDomainOnDomainUnload(object sender, EventArgs eventArgs)
        {
            DxLine.Dispose();
        }

        private static void DrawingOnOnPostReset(EventArgs args)
        {
            DxLine.OnResetDevice();
        }

        private static void DrawingOnOnPreReset(EventArgs args)
        {
            DxLine.OnLostDevice();
        }

        private static Vector2 GetHpPosAfterDmg(float mana)
        {
            var w = Width / ObjectManager.Player.MaxMana * mana;
            return new Vector2(StartPosition.X + w, StartPosition.Y);
        }

        public static void DrawManaPercent(float dmg, ColorBGRA color)
        {
            var pos = GetHpPosAfterDmg(dmg);
            FillManaBar(pos, color);
        }

        private static void FillManaBar(Vector2 pos, ColorBGRA color)
        {
            DxLine.Begin();
            DxLine.Draw(
                new[] { new Vector2((int) pos.X, (int) pos.Y + 4f), new Vector2((int) pos.X + 2, (int) pos.Y + 4f) },
                color);
            DxLine.End();
        }
    }
}