using System.Linq;
using hJhin.Extensions;
using EloBuddy;
using LeagueSharp.SDK;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK.Menu;

namespace hJhin.Modes
{
    static class Ultimate
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

        public static void Execute()
        {
            if (getCheckBoxItem(Config.ultiMenu, "combo.r"))
            {
                if (ObjectManager.Player.IsActive(Spells.R))
                {
                    if (getCheckBoxItem(Config.ultiMenu, "auto.shoot.bullets"))
                    {
                        var enemies = GameObjects.EnemyHeroes.Where(
                                                    x =>
                                                        x.LSIsValidTarget(Spells.R.Range) &&
                                                        getCheckBoxItem(Config.ultiMenu, "combo.r." + x.NetworkId)
                                                        && Spells.R.GetPrediction(x).Hitchance >= Provider.HikiChance())
                                                    .MinOrDefault(x => x.Health);

                        var pred = Spells.R.GetPrediction(enemies);
                        if (enemies != null && pred.Hitchance >= Provider.HikiChance())
                        {
                            Spells.R.Cast(pred.CastPosition);
                            return;
                        }
                    }
                }
                else
                {
                    if (Spells.R.IsReady() && getKeyBindItem(Config.SemiManualUlt, "semi.manual.ult"))
                    {
                        var enemies = GameObjects.EnemyHeroes.Where(
                                                    x =>
                                                        x.LSIsValidTarget(Spells.R.Range) &&
                                                        getCheckBoxItem(Config.ultiMenu, "combo.r." + x.NetworkId)
                                                        && Spells.R.GetPrediction(x).Hitchance >= Provider.HikiChance())
                                                    .MinOrDefault(x => x.Health);

                        var pred = Spells.R.GetPrediction(enemies);
                        if (enemies != null && pred.Hitchance >= Provider.HikiChance())
                        {
                            Spells.R.Cast(pred.CastPosition);
                            return;
                        }
                    }
                }
            }
        }
    }
}
