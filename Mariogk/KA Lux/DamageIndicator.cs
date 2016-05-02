using System;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using SharpDX;
using SharpDX.Direct3D9;

using Settings = KA_Lux.Config.Modes.Draw;

namespace KA_Lux
{
    public static class DamageIndicator
    {
        private const int BarWidth = 106;
        private const float LineThickness = 9.8f;
        private const float YOffSet = 14;

        public delegate float DamageToUnitDelegate(AIHeroClient hero);

        private static DamageToUnitDelegate DamageToUnit { get; set; }

        private static Font _Font, _Font2;

        public static void Initialize(DamageToUnitDelegate damageToUnit)
        {
            DamageToUnit = damageToUnit;
            Drawing.OnEndScene += OnEndScene;

            _Font = new Font(
                Drawing.Direct3DDevice,
                new FontDescription
                {
                    FaceName = "Segoi UI",
                    Height = 18,
                    Weight = FontWeight.Bold,
                    OutputPrecision = FontPrecision.Default,
                    Quality = FontQuality.ClearType,


                });

            _Font2 = new Font(
                Drawing.Direct3DDevice,
                new FontDescription
                {
                    FaceName = "Segoi UI",
                    Height = 12,
                    Weight = FontWeight.Bold,
                    OutputPrecision = FontPrecision.Default,
                    Quality = FontQuality.ClearType,

                });
        }

        private static void OnEndScene(EventArgs args)
        {
            if (Settings.DrawHealth || Settings.DrawPercent)
            {
                foreach (var unit in EntityManager.Heroes.Enemies.Where(u => u.IsValidTarget() && u.IsHPBarRendered))
                {
                    var damage = DamageToUnit(unit);

                    if (damage <= 0)
                    {
                        continue;
                    }

                    if (Settings.DrawHealth)
                    {
                        var damagePercentage = ((unit.TotalShieldHealth() - damage) > 0 ? (unit.TotalShieldHealth() - damage) : 0) /
                                               (unit.MaxHealth + unit.AllShield + unit.AttackShield + unit.MagicShield);
                        var currentHealthPercentage = unit.TotalShieldHealth() / (unit.MaxHealth + unit.AllShield + unit.AttackShield + unit.MagicShield);

                        var startPoint = new Vector2((int)(unit.HPBarPosition.X + damagePercentage * BarWidth), (int)unit.HPBarPosition.Y - 5 + YOffSet);
                        var endPoint = new Vector2((int)(unit.HPBarPosition.X + currentHealthPercentage * BarWidth) + 1, (int)unit.HPBarPosition.Y - 5 + YOffSet);

                        var colorH = System.Drawing.Color.FromArgb(Settings.HealthColor.A - 120, Settings.HealthColor.R,
                            Settings.HealthColor.G, Settings.HealthColor.B);

                        Drawing.DrawLine(startPoint, endPoint, LineThickness, colorH);
                    }

                    if (Settings.DrawPercent)
                    {
                        var color = new Color(Settings.HealthColor.R, Settings.HealthColor.G, Settings.HealthColor.B, Settings.HealthColor.A - 5);
                        _Font.DrawText(null, string.Concat(Math.Ceiling(damage / unit.TotalShieldHealth() * 100), "%"), (int)unit.HPBarPosition[0] + 102, (int)unit.HPBarPosition[1] + 29, color);
                        if (Settings.DrawStatistics)
                        {
                            _Font2.DrawText(null, "-" + Math.Round(SpellDamage.GetTotalDamage(unit)) + " / " + Math.Round((unit.Health - SpellDamage.GetTotalDamage(unit))), (int)unit.HPBarPosition[0] + 82, (int)unit.HPBarPosition[1] - 9, color);
                        }
                    }
                }
            }
        }
    }
}