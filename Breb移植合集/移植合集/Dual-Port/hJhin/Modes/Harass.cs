using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using hJhin.Extensions;
using EloBuddy;
using LeagueSharp.SDK;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK.Menu;

namespace hJhin.Modes
{
    class Harass
    {

        public static bool getCheckBoxItem(Menu m, string item)
        {
            return m[item].Cast<CheckBox>().CurrentValue;
        }

        public static int getSliderItem(Menu m, string item)
        {
            return m[item].Cast<Slider>().CurrentValue;
        }


        private static void ExecuteQ()
        {
            foreach (var enemy in GameObjects.EnemyHeroes.Where(x => x.LSIsValidTarget(Spells.Q.Range)))
            {
                Spells.Q.CastOnUnit(enemy);
            }
        }

        private static void ExecuteW()
        {
            foreach (var enemy in GameObjects.EnemyHeroes.Where(x => x.LSIsValidTarget(Spells.W.Range)))
            {
                var pred = Spells.W.GetPrediction(enemy);
                if (pred.Hitchance >= Provider.HikiChance())
                {
                    Spells.W.Cast(pred.CastPosition);
                }
            }
        }

        public static void Execute()
        {
            if (ObjectManager.Player.ManaPercent < getSliderItem(Config.harassMenu, "harass.mana"))
            {
                return;
            }

            if (Spells.Q.IsReady() && getCheckBoxItem(Config.harassMenu, "harass.q"))
            {
                ExecuteQ();
            }

            if (Spells.W.IsReady() && getCheckBoxItem(Config.harassMenu, "harass.w"))
            {
                ExecuteW();
            }
        }

    }
}
