using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using LeagueSharp.Common;

namespace YasuoPro
{
    internal static class ItemManager
    {
        private static readonly AIHeroClient Player = ObjectManager.Player;

        internal class Item
        {
            internal int eneinrangecount;
            internal Items.Item item;
            internal int minioncount;
            internal ItemCastType type;

            public Item(int itemid, float range, ItemCastType type, int eneinrange = 1)
            {
                this.type = type;
                eneinrangecount = eneinrange;
                item = new Items.Item(itemid, range);
            }


            internal bool Cast(AIHeroClient target, bool farmcast = false)
            {
                if (!item.IsReady())
                {
                    return false;
                }

                if (!farmcast)
                {
                    if ((type == ItemCastType.SelfCast || type == ItemCastType.RangeCast) &&
                        Player.CountEnemiesInRange(item.Range) >= eneinrangecount)
                    {
                        item.Cast();
                        return true;
                    }

                    if (type == ItemCastType.TargettedCast && target.IsInRange(item.Range))
                    {
                        item.Cast(target);
                        return true;
                    }
                }

                else if (farmcast)
                {
                    if ((type == ItemCastType.SelfCast || type == ItemCastType.RangeCast) &&
                        ObjectManager.Get<Obj_AI_Minion>().Count(x => x.IsValidTarget(item.Range)) >= minioncount)
                    {
                        item.Cast();
                        return true;
                    }
                }


                return false;
            }
        }

        internal enum ItemCastType
        {
            SelfCast,
            TargettedCast,
            RangeCast,
            SkillshotCast
        }
    }
}