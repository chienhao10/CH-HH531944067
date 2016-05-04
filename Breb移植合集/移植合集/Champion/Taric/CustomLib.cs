namespace SkyLv_Taric
{
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;
    using EloBuddy;
    public static class CustomLib
    {
        #region #GET
        private static AIHeroClient Player
        {
            get
            {
                return SkyLv_Taric.Player;
            }
        }

        private static Spell Q
        {
            get
            {
                return SkyLv_Taric.Q;
            }
        }
        #endregion


        public static float enemyChampionInPlayerRange(float Range)
        {
            return ObjectManager.Get<AIHeroClient>().Where(target => !target.IsMe && target.Team != ObjectManager.Player.Team && target.LSDistance(Player) <= Range).Count();
        }

        public static float EnemyMinionInPlayerRange(float Range)
        {
            return ObjectManager.Get<Obj_AI_Minion>().Where(m => m.Team != ObjectManager.Player.Team && m.LSDistance(Player) <= Range && !m.IsDead).Count();
        }

        public static bool HavePassiveAA()
        {
            if (Player.GetBuffCount("TaricPassiveAttack") == -1)
            {
                return false;
            }
            else return true;
        }
    }
}
