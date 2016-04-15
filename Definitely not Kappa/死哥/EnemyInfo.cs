namespace Karthus
{
    using EloBuddy;

    internal class EnemyInfo
    {
        public AIHeroClient Player;

        public int LastSeen;

        //public int LastPinged;

        public EnemyInfo(AIHeroClient player)
        {
            this.Player = player;
        }
    }
}