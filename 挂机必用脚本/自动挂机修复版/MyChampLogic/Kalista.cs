using AutoBuddy.MainLogics;
using EloBuddy;
using EloBuddy.SDK;
using System;
using System.Drawing;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;

namespace AutoBuddy.MyChampLogic
{
    internal class Kalista : IChampLogic
    {

        public float MaxDistanceForAA { get { return int.MaxValue; } }
        public float OptimalMaxComboDistance { get { return AutoWalker.p.AttackRange; } }
        public float HarassDistance { get { return AutoWalker.p.AttackRange; } }

        public Spell.Active Q;
        public Spell.Skillshot W, E, R;

        public Kalista()
        {
            skillSequence = new[] {2, 1, 3, 2, 2, 4, 2, 1, 2, 1, 4, 1, 1, 3, 3, 4, 3, 3};
            ShopSequence =
                "3340:Buy,2003:StartHpPot,1055:Buy,1053:Buy,3144:Buy,1043:Buy,3153:Buy,3006:Buy,3086:Buy,1043:Buy,3085:Buy,1038:Buy,2003:StopHpPot,1053:Buy,3072:Buy,3086:Buy,3046:Buy,1055:Sell,3140:Buy,3139:Buy";
        }

        public int[] skillSequence { get; private set; }
        public LogicSelector Logic { get; set; }


        public string ShopSequence { get; private set; }

        public void Harass(AIHeroClient target)
        {
        }

        public void Survi()
        {
        }

        public void Combo(AIHeroClient target)
        {
        }
    }
}