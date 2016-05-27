using EloBuddy;

namespace AutoBuddy
{
    enum Lane
    {
        Unknown,
        Top,
        Mid,
        Bot,
        HQ,
        Spawn
    }
    class ChampLane
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
