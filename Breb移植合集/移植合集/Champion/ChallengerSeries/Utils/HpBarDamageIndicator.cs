using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.SDK;
using SharpDX;
using Color = System.Drawing.Color;
using EloBuddy;

namespace Challenger_Series.Utils
{

    public static class HpBarDamageIndicator
    {
        public delegate float DamageToUnitDelegate(AIHeroClient hero);

        public static Color Color = Color.Lime;
        public static bool Enabled = true;
        private static DamageToUnitDelegate _damageToUnit;


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

        public static void Drawing_OnDraw(EventArgs args)
        {
            if (!Enabled || _damageToUnit == null)
            {
                return;
            }

            foreach (var unit in GameObjects.EnemyHeroes.Where(h => h.IsValid && h.IsHPBarRendered && h.Distance(ObjectManager.Player.ServerPosition) < 1300))
            {
                var damage = _damageToUnit(unit);

                var damagePercentage = ((unit.Health - damage) > 0 ? (unit.Health - damage) : 0) / unit.MaxHealth;
                var currentHealthPercentage = unit.HealthPercent;
                var startPoint = new Vector2(
                    (int)(unit.HPBarPosition.X + 10 + (damagePercentage * 100)),
                    (int)(unit.HPBarPosition.Y + 25) - 5);
                var endPoint =
                    new Vector2(
                        (int)(unit.HPBarPosition.X + 10 + (currentHealthPercentage)),
                        (int)(unit.HPBarPosition.Y + 25) - 5);
                Drawing.DrawLine(startPoint, endPoint, 9, Color.LimeGreen);
                Drawing.DrawText(unit.HPBarPosition.X + 160, unit.HPBarPosition.Y + 20, Color.LimeGreen, Math.Floor(damage / unit.Health * 100) + "%");
            }
        }
    }
}
