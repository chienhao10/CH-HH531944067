using System.Drawing;
using System.Linq;
using EloBuddy;

namespace Slutty_ryze
{
    internal class GlobalManager
    {
        #region Variable Declaration


        public delegate float DamageToUnitDelegate(AIHeroClient hero);

        #endregion

        #region Public Properties

        #region Auto Properties

        public static bool EnableFillDamage { get; set; }
        public static bool EnableDrawingDamage { get; set; }
        public static Color DamageFillColor { get; set; }

        #endregion

        public static bool CheckTarget(Obj_AI_Base minion)
        {
            return minion.IsMinion || minion.MaxHealth > 3 || minion.Armor > 0 || minion.IsTargetable;
        }

        public static AIHeroClient GetHero = ObjectManager.Player;

        public static bool CheckMinion(Obj_AI_Base minion)
        {
            return minion != null && minion.IsMinion && minion.MaxHealth > 3 && minion.IsTargetable;
        }

        public static int GetPassiveBuff
        {
            get
            {
                var data = GetHero.Buffs.FirstOrDefault(b => b.DisplayName == "RyzePassiveStack");
                return data == null ? 0 : data.Count;
            }
        }

        #endregion
    }
}