using EloBuddy;

namespace AutoBuddy.Utilities
{
    internal class HeroInfo
    {
        public AIHeroClient hero;
        public int kills { get; private set; }
        public int kills2 { get { return hero.ChampionsKilled; } }
        public int deaths { get { return hero.Deaths; } }
        public int assists { get { return hero.Assists; } }
        public int farm { get { return hero.MinionsKilled + hero.NeutralMinionsKilled; } }
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