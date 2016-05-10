namespace Jinx
{
    namespace DamageIndicator
    {
        using System;

        using EloBuddy;
        using EloBuddy.SDK;
        using EloBuddy.SDK.Menu.Values;
        using EloBuddy.SDK.Rendering;

        using SharpDX;

        using Color = System.Drawing.Color;

        /// <summary>
        /// Credits to Fluxy for Original Code.
        /// </summary>
        public class DamageIndicator
        {
            private const float BarLength = 109;
            private const float XOffset = 2;
            private const float YOffset = 9;
            public float CheckDistance = 1200;

            public DamageIndicator()
            {
                Drawing.OnEndScene += Drawing_OnDraw;
            }

            private static void Drawing_OnDraw(EventArgs args)
            {
                if (!Config.DrawingMenu["draw.Damage"].Cast<CheckBox>().CurrentValue) return;

                foreach (var aiHeroClient in EntityManager.Heroes.Enemies)
                {
                    if (!aiHeroClient.IsHPBarRendered) continue;

                    var pos = new Vector2(
                        aiHeroClient.HPBarPosition.X + XOffset,
                        aiHeroClient.HPBarPosition.Y + YOffset);

                    var fullbar = (BarLength)*(aiHeroClient.HealthPercent/100);

                    var drawQ = Config.DrawingMenu["draw.Q"].Cast<CheckBox>().CurrentValue;

                    var drawW = Config.DrawingMenu["draw.W"].Cast<CheckBox>().CurrentValue;

                    var drawE = Config.DrawingMenu["draw.E"].Cast<CheckBox>().CurrentValue;

                    var drawR = Config.DrawingMenu["draw.R"].Cast<CheckBox>().CurrentValue;

                    var damage = (BarLength)
                                 *((Essentials.DamageLibrary.CalculateDamage(aiHeroClient, drawQ, drawW, drawE, drawR)
                                    /aiHeroClient.MaxHealth) > 1
                                     ? 1
                                     : (Essentials.DamageLibrary.CalculateDamage(
                                         aiHeroClient,
                                         drawQ,
                                         drawW,
                                         drawE,
                                         drawR)/aiHeroClient.MaxHealth));

                    var A = Config.DrawingMenu["draw_Alpha"].Cast<Slider>().CurrentValue;
                    var R = Config.DrawingMenu["draw_Red"].Cast<Slider>().CurrentValue;
                    var G = Config.DrawingMenu["draw_Green"].Cast<Slider>().CurrentValue;
                    var B = Config.DrawingMenu["draw_Blue"].Cast<Slider>().CurrentValue;

                    Line.DrawLine(
                        Color.FromArgb(A, R, G, B),
                        9f,
                        new Vector2(pos.X, pos.Y),
                        new Vector2(pos.X + (damage > fullbar ? fullbar : damage), pos.Y));

                    Line.DrawLine(
                        Color.FromArgb(A, R, G, B),
                        3,
                        new Vector2(pos.X + (damage > fullbar ? fullbar : damage), pos.Y),
                        new Vector2(pos.X + (damage > fullbar ? fullbar : damage), pos.Y));
                }
            }
        }
    }
}