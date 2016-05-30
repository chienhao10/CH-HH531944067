using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using hJhin.Extensions;
using EloBuddy;
using LeagueSharp.SDK;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;

namespace hJhin.Modes
{
    class Clear
    {
        private static void ExecuteQ()
        {
            var minion = GameObjects.EnemyMinions.Where(x => x.LSIsValidTarget(Spells.Q.Range)).MinOrDefault(x=> x.Health);
            if (minion != null)
            {
                Spells.Q.CastOnUnit(minion);
            }
        }

        public static bool getCheckBoxItem(Menu m, string item)
        {
            return m[item].Cast<CheckBox>().CurrentValue;
        }

        public static int getSliderItem(Menu m, string item)
        {
            return m[item].Cast<Slider>().CurrentValue;
        }


        public static void ExecuteW()
        {
            var minions =
                ObjectManager.Get<Obj_AI_Minion>()
                    .Where(x => x.IsMinion && x.IsEnemy && x.LSIsValidTarget(Spells.W.Range))
                    .ToList();

            var minionhit = Spells.W.GetLineFarmLocation(minions).MinionsHit;
            if (minionhit >= getSliderItem(Config.clearMenu, "lane.w.min.count"))
            {
                Spells.W.Cast(Spells.W.GetLineFarmLocation(minions).Position);
            }

            
        }

        public static void Execute()
        {
            if (ObjectManager.Player.ManaPercent < getSliderItem(Config.clearMenu, "lane.mana"))
            {
                return;
            }

            if (Spells.Q.IsReady() && getCheckBoxItem(Config.clearMenu, "lane.q"))
            {
                ExecuteQ();
            }

            if (Spells.W.IsReady() && getCheckBoxItem(Config.clearMenu, "lane.w"))
            {
                ExecuteW();
            }
        }
    }
}
