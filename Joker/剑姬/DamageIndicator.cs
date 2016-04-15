using System;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK.Rendering;
using System.Drawing;
using SharpDX;

using Settings = JokerFioraBuddy.Config.Drawings;
namespace JokerFioraBuddy
{
    public static class DamageIndicator
    {
        public delegate float DamageToUnitDelegate(AIHeroClient hero);

        private const int xOffset = 5;
        private const int yOffset = 20;
        private const int Width = 103;
        private const int Height = 8;

        public static DamageToUnitDelegate DamageToUnit { get; set; }
        public static Text TextKillable { get; private set; }
        public static Text TextMinion { get; private set; }

        static DamageIndicator()
        {
            TextKillable = new Text("", new Font(FontFamily.GenericSansSerif, 11, FontStyle.Bold)) { Color = System.Drawing.Color.Red };
            TextMinion = new Text("", new Font(FontFamily.GenericSansSerif, 11, FontStyle.Bold)) { Color = System.Drawing.Color.LimeGreen };

            Drawing.OnDraw += Drawing_OnDraw;
        }

        static void Drawing_OnDraw(EventArgs args)
        {
            if (Settings.ShowKillable)
            {
                foreach (var unit in ObjectManager.Get<AIHeroClient>().Where(h => h.IsValid && h.IsHPBarRendered && h.IsEnemy))
                {

                    var barPos = unit.HPBarPosition;
                    var damage = DamageToUnit(unit);
                    var percentHealthAfterDamage = Math.Max(0, unit.Health - damage) / unit.MaxHealth;
                    var yPos = barPos.Y + yOffset;
                    var xPosDamage = barPos.X + xOffset + Width * percentHealthAfterDamage;
                    var xPosCurrentHp = barPos.X + xOffset + Width * unit.Health / unit.MaxHealth;

                    if (damage > unit.Health)
                    {
                        TextKillable.Position = new Vector2((int)barPos.X - 15, (int)barPos.Y + yOffset + 20);
                        TextKillable.TextValue = "连招可击杀!";
                        TextKillable.Color = System.Drawing.Color.LimeGreen;
                    }
                    else
                    {
                        TextKillable.Position = new Vector2((int)barPos.X, (int)barPos.Y + yOffset + 20);
                        TextKillable.TextValue = "不可击杀!";
                        TextKillable.Color = System.Drawing.Color.Red;
                    }

                    TextKillable.Draw();
                }
            }
        }

        public static void Initialize()
        {

        }
    }
}
