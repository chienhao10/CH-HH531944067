using System;
using System.Collections.Generic;
using System.Linq;
using EloBuddy;

namespace Karthus
{

    internal class EnemyInfo
    {
        public AIHeroClient Player;
        public int LastSeen;
        //public int LastPinged;

        public EnemyInfo(AIHeroClient player)
        {
            Player = player;
        }
    }

    internal class Helper
    {

        public static int GameTimeTickCount
        {
            get
            {
                return (int)(Game.Time * 1000);
            }
        }
        public static int TickCount
        {
            get
            {
                return Environment.TickCount & int.MaxValue;
            }
        }
        public static IEnumerable<AIHeroClient> EnemyTeam;
        public static IEnumerable<AIHeroClient> OwnTeam;
        public static List<EnemyInfo> EnemyInfo = new List<EnemyInfo>();

        public Helper()
        {
            var champions = ObjectManager.Get<AIHeroClient>().ToList();

            OwnTeam = champions.Where(x => x.IsAlly);
            EnemyTeam = champions.Where(x => x.IsEnemy);

            EnemyInfo = EnemyTeam.Select(x => new EnemyInfo(x)).ToList();

            Game.OnUpdate += Game_OnUpdate;
        }

        void Game_OnUpdate(EventArgs args)
        {
            var time = TickCount;

            foreach (EnemyInfo enemyInfo in EnemyInfo.Where(x => x.Player.IsVisible))
            {
                enemyInfo.LastSeen = time;
            }
        }

        public static EnemyInfo GetPlayerInfo(AIHeroClient enemy)
        {
            return EnemyInfo.Find(x => x.Player.NetworkId == enemy.NetworkId);
        }

        public static float GetTargetHealth(EnemyInfo playerInfo, int additionalTime)
        {
            if (playerInfo.Player.IsVisible)
            {
                return playerInfo.Player.Health;
            }

            var predictedhealth = playerInfo.Player.Health + playerInfo.Player.HPRegenRate * ((TickCount - playerInfo.LastSeen + additionalTime) / 1000f);

            return predictedhealth > playerInfo.Player.MaxHealth ? playerInfo.Player.MaxHealth : predictedhealth;
        }
    }
}