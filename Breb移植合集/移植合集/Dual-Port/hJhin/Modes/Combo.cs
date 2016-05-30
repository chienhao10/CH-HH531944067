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
    class Combo
    {
        public static bool getCheckBoxItem(Menu m, string item)
        {
            return m[item].Cast<CheckBox>().CurrentValue;
        }

        public static int getSliderItem(Menu m, string item)
        {
            return m[item].Cast<Slider>().CurrentValue;
        }

        private static readonly int MinRange = getSliderItem(Config.comboMenu, "combo.w.min");
        private static readonly int MaxRange = getSliderItem(Config.comboMenu, "combo.w.max");
        public static void ExecuteQ()
        {
            foreach (var enemy in GameObjects.EnemyHeroes.Where(x=> x.LSIsValidTarget(Spells.Q.Range)))
            {
                Spells.Q.Cast(enemy);
            }
        }

        public static void ExecuteW()
        {
            if (getCheckBoxItem(Config.comboMenu, "combo.w.mark"))
            {
                foreach (var enemy in GameObjects.EnemyHeroes.Where(x => x.LSIsValidTarget(Spells.W.Range) &&
                    (x.IsStunnable() || x.IsEnemyImmobile())))
                {
                    Spells.W.Cast(enemy);
                }
            }

            else
            {
                foreach (var enemy in GameObjects.EnemyHeroes.Where(x => x.IsValid && x.Distance(ObjectManager.Player) < MaxRange
                    && x.Distance(ObjectManager.Player) > MinRange && Spells.W.GetPrediction(x).Hitchance
                    >= Provider.HikiChance() && x.LSIsValidTarget(Spells.W.Range)))
                {
                    Spells.W.Cast(enemy);
                }
            }
        }
           
        public static void ExecuteE()
        {
            foreach (var enemy in GameObjects.EnemyHeroes.Where(x => x.LSIsValidTarget(Spells.E.Range) && x.IsEnemyImmobile()))
            {
                var pred = Spells.E.GetPrediction(enemy);
                if (pred.Hitchance >= Provider.HikiChance())
                {
                    Spells.E.Cast(pred.CastPosition);
                }
            }
        }

        public static void Execute()
        {
            if (Spells.Q.IsReady() && getCheckBoxItem(Config.comboMenu, "combo.q"))
            {
                ExecuteQ();
            }
            if (Spells.W.IsReady() && getCheckBoxItem(Config.comboMenu, "combo.w"))
            {
                ExecuteW();
            }
            if (Spells.E.IsReady() && getCheckBoxItem(Config.comboMenu, "combo.e"))
            {
                ExecuteE();
            }
        }
    }
}
