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
    class Jungle
    {
        public static bool getCheckBoxItem(Menu m, string item)
        {
            return m[item].Cast<CheckBox>().CurrentValue;
        }

        public static int getSliderItem(Menu m, string item)
        {
            return m[item].Cast<Slider>().CurrentValue;
        }


        public static void Execute()
        {
            if (ObjectManager.Player.ManaPercent < getSliderItem(Config.jungleMenu, "jungle.mana"))
            {
                return;
            }

            if (Spells.Q.IsReady() && getCheckBoxItem(Config.jungleMenu, "jungle.q"))
            {
                foreach (var minion in GameObjects.JungleLarge.Where(x=> x.LSIsValidTarget(Spells.Q.Range)))
                {
                    Spells.Q.CastOnUnit(minion);
                }
            }

            if (Spells.W.IsReady() && getCheckBoxItem(Config.jungleMenu, "jungle.w"))
            {
                foreach (var minion in GameObjects.JungleLarge.Where(x => x.LSIsValidTarget(Spells.W.Range)))
                {
                    Spells.W.Cast(minion);
                }
            }
        }
    }
}
