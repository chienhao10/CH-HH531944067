namespace Karthus
{
    using EloBuddy;

    internal class Ti
    {
        public int timeCheck;

        public Ti(AIHeroClient player)
        {
            this.Player = player;
        }

        public AIHeroClient Player { get; set; }
    }
}