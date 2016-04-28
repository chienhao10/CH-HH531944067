using System;
using System.Drawing;
using System.Linq;
using EloBuddy;
using LeagueSharp.Common;

namespace ElDiana
{
    internal class DrawDamage //by xSalice
    {
        #region Delegates

        public delegate float DamageToUnitDelegate(AIHeroClient hero);

        #endregion

        #region Public Properties

        public static DamageToUnitDelegate DamageToUnit
        {
            get { return _damageToUnit; }

            set
            {
                if (_damageToUnit == null)
                {
                    Drawing.OnDraw += Drawing_OnDraw;
                }
                _damageToUnit = value;
            }
        }

        #endregion

        #region Methods

        private static void Drawing_OnDraw(EventArgs args)
        {
            if (!Enabled || _damageToUnit == null)
            {
                return;
            }

            foreach (var unit in HeroManager.Enemies.Where(h => h.IsValid && h.IsHPBarRendered))
            {
                var barPos = unit.HPBarPosition;
                var damage = _damageToUnit(unit);
                var percentHealthAfterDamage = Math.Max(0, unit.Health - damage)/unit.MaxHealth;
                var yPos = barPos.Y + YOffset;
                var xPosDamage = barPos.X + XOffset + Width*percentHealthAfterDamage;
                var xPosCurrentHp = barPos.X + XOffset + Width*unit.Health/unit.MaxHealth;

                if (damage > unit.Health)
                {
                    Text.X = (int) barPos.X + XOffset;
                    Text.Y = (int) barPos.Y + YOffset - 13;
                    Text.text = "Killable: " + (unit.Health - damage);
                    Text.OnEndScene();
                }

                Drawing.DrawLine(xPosDamage, yPos, xPosDamage, yPos + Height, 1, Color);

                if (Fill)
                {
                    var differenceInHP = xPosCurrentHp - xPosDamage;
                    var pos1 = barPos.X + 9 + 107*percentHealthAfterDamage;

                    for (var i = 0; i < differenceInHP; i++)
                    {
                        Drawing.DrawLine(pos1 + i, yPos, pos1 + i, yPos + Height, 1, FillColor);
                    }
                }
            }
        }

        #endregion

        #region Constants

        private const int Height = 8;

        private const int Width = 103;

        private const int XOffset = 10;

        private const int YOffset = 20;

        #endregion

        #region Static Fields

        public static Color Color = Color.Lime;

        public static bool Enabled = true;

        public static bool Fill = true;

        public static Color FillColor = Color.Goldenrod;

        private static readonly Render.Text Text = new Render.Text(0, 0, "", 14, SharpDX.Color.Red, "monospace");

        private static DamageToUnitDelegate _damageToUnit;

        #endregion
    }
}