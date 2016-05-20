using AutoBuddy.MainLogics;
using EloBuddy;

namespace AutoBuddy.MyChampLogic
{
    internal interface IChampLogic
    {
        int[] skillSequence { get; }
        float MaxDistanceForAA { get; }
        float OptimalMaxComboDistance { get; }
        float HarassDistance { get; }
        LogicSelector Logic { set; }
        string ShopSequence { get; }
        void Harass(AIHeroClient target);
        void Survi();
        void Combo(AIHeroClient target);
    }
}