using System;
using System.Collections.Generic;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using Jhin.Managers;
using SharpDX;
using Color = System.Drawing.Color;

namespace Jhin.Utilities
{
    public class DamageSource
    {
        public Color Color = Color.FromArgb(170, Color.White);
        public Func<bool> Condition = () => true;
        public Func<Obj_AI_Base, float> Damage = target => 0;
        public Func<Obj_AI_Base, string> Text = target => "";
    }

    public static class DamageIndicator
    {
        private const int BarWidth = 104;
        private const int BarHeight = 9;
        private static readonly Vector2 BarOffset = new Vector2(1, 9.8f);
        private static readonly Vector2 TextOffset = new Vector2(1, -15);
        public static List<DamageSource> DamageSources = new List<DamageSource>();

        public static void Initialize()
        {
            DamageSources = new List<DamageSource>
            {
                new DamageSource
                {
                    Condition = () => true,
                    Damage =
                        target =>
                            AIO.CurrentChampion.GetBestCombo(target).Damage - SpellManager.Ignite.GetDamage(target) -
                            SpellManager.Smite.GetDamage(target),
                    Color = Color.FromArgb(170, Color.DarkOrange),
                    Text =
                        target =>
                            (AIO.CurrentChampion.GetBestCombo(target).Q ? "Q" : "") +
                            (AIO.CurrentChampion.GetBestCombo(target).W ? "W" : "") +
                            (AIO.CurrentChampion.GetBestCombo(target).E ? "E" : "") +
                            (AIO.CurrentChampion.GetBestCombo(target).R ? "R" : "")
                },
                new DamageSource
                {
                    Condition = () => SpellManager.Ignite.IsReady,
                    Damage = target => SpellManager.Ignite.GetDamage(target),
                    Color = Color.FromArgb(220, Color.DarkRed),
                    Text = target => "I"
                },
                new DamageSource
                {
                    Condition = () => SpellManager.Smite.IsReady,
                    Damage = target => SpellManager.Smite.GetDamage(target),
                    Color = Color.FromArgb(220, Color.DodgerBlue),
                    Text = target => "S"
                }
            };
        }

        public static void Draw()
        {
            foreach (var enemy in UnitManager.ValidEnemyHeroes.Where(m => m.VisibleOnScreen))
            {
                var health = enemy.TotalShieldHealth();
                var maxHealth = enemy.TotalShieldMaxHealth();
                var barPosition = new Vector2(enemy.HPBarPosition.X + enemy.HPBarXOffset,
                    enemy.HPBarPosition.Y + enemy.HPBarYOffset);
                var barPoint = barPosition + new Vector2(BarOffset.X, BarOffset.Y);
                var lastStartPoint = new Vector2(-1, -1);
                foreach (var d in DamageSources.Where(source => source.Condition()))
                {
                    if (health > 0f)
                    {
                        var damage = d.Damage(enemy);
                        var afterHealth = health - damage;
                        var damagePercent = (afterHealth > 0f ? afterHealth : 0f)/maxHealth;
                        var currentHealthPercent = health/maxHealth;
                        var startPoint = new Vector2(damagePercent*BarWidth, 0);
                        var endPoint = new Vector2(currentHealthPercent*BarWidth, 0);
                        Drawing.DrawLine(barPoint + startPoint, barPoint + endPoint, BarHeight, d.Color);
                        health -= damage;
                        lastStartPoint = startPoint;
                    }
                }
                if (lastStartPoint.X > -1 && lastStartPoint.Y > -1 && enemy.Health > 0)
                {
                    var textPoint = barPosition + new Vector2(TextOffset.X, TextOffset.Y);
                    var text = Math.Min(100 - (int) (health/enemy.TotalShieldHealth()*100), 100) + "%";
                    Drawing.DrawText(textPoint + lastStartPoint, Color.White, text, 6);
                }
                /*
                var damage = KillSteal.RDamageOnEnemies[enemy.NetworkId];

                */
            }
        }
    }
}