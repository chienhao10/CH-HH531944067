using EloBuddy;

namespace AutoBuddy.Utilities
{
    internal enum Lane
    {
        Unknown,
        Top,
        Bot,
        Jungle,
        HQ,
        Spawn
    }

    internal class ChampLane
    {
        public readonly AIHeroClient champ;
        public readonly Lane lane;

        public ChampLane(AIHeroClient champ, Lane lane)
        {
            this.champ = champ;
            this.lane = lane;
        }
    }
}