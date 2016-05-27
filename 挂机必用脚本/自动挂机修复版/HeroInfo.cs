using EloBuddy;

namespace AutoBuddy
{
    internal class HeroInfo
    {
        public AIHeroClient hero;
        public int kills { get; private set; }

        public HeroInfo(AIHeroClient h)
        {
            hero = h;
            Game.OnNotify += Game_OnNotify;
        }

        private void Game_OnNotify(GameNotifyEventArgs args)
        {
            if (args.EventId == GameEventId.OnChampionKill && args.NetworkId == hero.NetworkId)
                kills++;
        }
    }
}
