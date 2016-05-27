using System.Linq;
using SharpDX;
using System.Drawing;
using EloBuddy;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK.Menu;

namespace NabbTracker
{
    /// <summary>
    ///     The drawings class.
    /// </summary>
    internal class ExpTracker
    {

        public static bool getCheckBoxItem(Menu m, string item)
        {
            return m[item].Cast<CheckBox>().CurrentValue;
        }

        public static int getSliderItem(Menu m, string item)
        {
            return m[item].Cast<Slider>().CurrentValue;
        }

        public static bool getKeyBindItem(Menu m, string item)
        {
            return m[item].Cast<KeyBind>().CurrentValue;
        }

        public static int getBoxItem(Menu m, string item)
        {
            return m[item].Cast<ComboBox>().CurrentValue;
        }

        /// <summary>
        ///     Loads the range drawings.
        /// </summary>
        public static void Initialize()
        {
            Drawing.OnDraw += delegate
            {
                foreach (var unit in GameObjects.Heroes.Where(
                    e =>
                        e.IsHPBarRendered &&
                        (e.IsMe && getCheckBoxItem(Variables.Menu, "me") || e.IsEnemy && getCheckBoxItem(Variables.Menu, "enemiesEXP") || e.IsAlly && !e.IsMe && getCheckBoxItem(Variables.Menu, "alliesEXP"))))
                {

                    if (unit.Level >= 18)
                    {
                        return;
                    }

                    var actualExp = unit.Experience.XP;
                    var neededExp = 180 + (100 * unit.Level);

                    Variables.ExpX = (int)unit.HPBarPosition.X + Variables.ExpXAdjustment(unit);
                    Variables.ExpY = (int)unit.HPBarPosition.Y + Variables.ExpYAdjustment(unit);

                    if (unit.Level > 1)
                    {
                        actualExp -= (280 + (80 + 100 * unit.Level)) / 2 * (unit.Level - 1);
                    }

                    var expPercent = (int)(actualExp / neededExp * 100);

                    Drawing.DrawLine(
                        Variables.ExpX - 76,
                        Variables.ExpY - 5,
                        Variables.ExpX + 56,
                        Variables.ExpY - 5,
                        7,
                        System.Drawing.Color.Purple
                    );

                    if (expPercent > 0)
                    {
                        Drawing.DrawLine(Variables.ExpX - 76, Variables.ExpY - 5, Variables.ExpX - 76 + (float)(1.32 * expPercent), Variables.ExpY - 5, 7, System.Drawing.Color.Red);
                    }

                    Variables.DisplayTextFont.DrawText(
                        null,
                        expPercent > 0
                            ? "XP : " + expPercent.ToString() + "%"
                            : "XP : 0%",
                        Variables.ExpX - 21,
                        Variables.ExpY - 12,
                        SharpDX.Color.Yellow
                    );
                }
            };
        }
    }
}