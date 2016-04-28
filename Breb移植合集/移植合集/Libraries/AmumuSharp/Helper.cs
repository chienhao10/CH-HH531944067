using System;
using System.Collections.Generic;
using System.Linq;
using EloBuddy;
using LeagueSharp.Common;

namespace PortAIO.Utility.AmumuSharp
{
    internal class EnemyInfo
    {
        public int LastSeen;
        public AIHeroClient Player;
        //public int LastPinged;

        public EnemyInfo(AIHeroClient player)
        {
            Player = player;
        }
    }

    internal class Helper
    {
        public List<EnemyInfo> EnemyInfo = new List<EnemyInfo>();
        public IEnumerable<AIHeroClient> EnemyTeam;
        public IEnumerable<AIHeroClient> OwnTeam;

        public Helper()
        {
            var champions = ObjectManager.Get<AIHeroClient>().ToList();

            OwnTeam = champions.Where(x => x.IsAlly);
            EnemyTeam = champions.Where(x => x.IsEnemy);

            EnemyInfo = EnemyTeam.Select(x => new EnemyInfo(x)).ToList();

            Game.OnUpdate += Game_OnUpdate;
        }

        private void Game_OnUpdate(EventArgs args)
        {
            var time = Utils.TickCount;

            foreach (var enemyInfo in EnemyInfo.Where(x => x.Player.IsVisible))
                enemyInfo.LastSeen = time;
        }

        public EnemyInfo GetPlayerInfo(AIHeroClient enemy)
        {
            return EnemyInfo.Find(x => x.Player.NetworkId == enemy.NetworkId);
        }

        public float GetTargetHealth(EnemyInfo playerInfo, int additionalTime)
        {
            if (playerInfo.Player.IsVisible)
                return playerInfo.Player.Health;

            var predictedhealth = playerInfo.Player.Health +
                                  playerInfo.Player.HPRegenRate*
                                  ((Utils.TickCount - playerInfo.LastSeen + additionalTime)/1000f);

            return predictedhealth > playerInfo.Player.MaxHealth ? playerInfo.Player.MaxHealth : predictedhealth;
        }
    }
}