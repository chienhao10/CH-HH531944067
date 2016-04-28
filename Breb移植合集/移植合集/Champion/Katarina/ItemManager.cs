using LeagueSharp.Common;
using LeagueSharp.Common.Data;

namespace Staberina
{
    internal static class ItemManager
    {
        public static Items.Item LudensEcho
        {
            get { return ItemData.Ludens_Echo.GetItem(); }
        }


        public static bool IsValidAndReady(this Items.Item item)
        {
            return item != null && item.IsReady();
        }
    }
}