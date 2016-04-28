using System.Collections.Generic;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using LeagueSharp.Common;
using SharpDX;

namespace PortAIO.Utility.BrianSharp
{
    internal class Helper
    {
        #region Enums

        public enum SmiteType
        {
            Grey,

            Purple,

            Red,

            Blue,

            None
        }

        #endregion

        private static AIHeroClient myHero
        {
            get { return Player.Instance; }
        }

        public static SmiteType CurrentSmiteType
        {
            get
            {
                if (myHero.GetSpellSlot("s5_summonersmitequick").IsReady())
                {
                    return SmiteType.Grey;
                }
                if (myHero.GetSpellSlot("itemsmiteaoe").IsReady())
                {
                    return SmiteType.Purple;
                }
                if (myHero.GetSpellSlot("s5_summonersmiteduel").IsReady())
                {
                    return SmiteType.Red;
                }
                return myHero.GetSpellSlot("s5_summonersmiteplayerganker").IsReady() ? SmiteType.Blue : SmiteType.None;
            }
        }

        public static bool IsPet(Obj_AI_Minion obj)
        {
            var pets = new[]
            {
                "annietibbers", "elisespiderling", "heimertyellow", "heimertblue", "leblanc", "malzaharvoidling",
                "shacobox", "shaco", "yorickspectralghoul", "yorickdecayedghoul", "yorickravenousghoul",
                "zyrathornplant", "zyragraspingplant"
            };
            return pets.Contains(obj.CharData.BaseSkinName.ToLower());
        }

        public static List<Obj_AI_Minion> GetMinions(Vector3 from, float range, MinionTypes type = MinionTypes.All,
            MinionTeam team = MinionTeam.Enemy, MinionOrderTypes order = MinionOrderTypes.Health)
        {
            var result = from minion in ObjectManager.Get<Obj_AI_Minion>()
                where minion.IsValidTarget(range, false, @from)
                let minionTeam = minion.Team
                where
                    (team == MinionTeam.Neutral && minionTeam == GameObjectTeam.Neutral)
                    || (team == MinionTeam.Ally
                        && minionTeam
                        == (myHero.Team == GameObjectTeam.Chaos ? GameObjectTeam.Chaos : GameObjectTeam.Order))
                    || (team == MinionTeam.Enemy
                        && minionTeam
                        == (myHero.Team == GameObjectTeam.Chaos ? GameObjectTeam.Order : GameObjectTeam.Chaos))
                    || (team == MinionTeam.NotAlly && minionTeam != myHero.Team)
                    || (team == MinionTeam.NotAllyForEnemy
                        && (minionTeam == myHero.Team || minionTeam == GameObjectTeam.Neutral))
                    || team == MinionTeam.All
                where
                    (minion.IsMelee() && type == MinionTypes.Melee)
                    || (!minion.IsMelee() && type == MinionTypes.Ranged) || type == MinionTypes.All
                where MinionManager.IsMinion(minion) || minionTeam == GameObjectTeam.Neutral || IsPet(minion)
                select minion;
            switch (order)
            {
                case MinionOrderTypes.Health:
                    result = result.OrderBy(i => i.Health);
                    break;
                case MinionOrderTypes.MaxHealth:
                    result = result.OrderBy(i => i.MaxHealth).Reverse();
                    break;
            }
            return result.ToList();
        }

        public static List<Obj_AI_Minion> GetMinions(float range, MinionTypes type = MinionTypes.All,
            MinionTeam team = MinionTeam.Enemy, MinionOrderTypes order = MinionOrderTypes.Health)
        {
            return GetMinions(myHero.ServerPosition, range, type, team, order);
        }
    }
}