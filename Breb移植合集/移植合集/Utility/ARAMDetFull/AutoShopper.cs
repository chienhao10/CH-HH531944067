using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Permissions;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using EloBuddy;

namespace ARAMDetFull
{
    public enum ItemCondition
    {
        TAKE_PRIMARY = 0,
        ENEMY_AP = 1,
        ENEMY_MR = 2,
        ENEMY_RANGED = 3,
        ENEMY_LOSING = 4,
    }

    

    public class AutoShopper
    {
        private static readonly List<FullItem> itemList = new List<FullItem>();

        public static AIHeroClient player = ObjectManager.Player;

        public static int testGold = 1375;

        public static Build curBuild;

        private static bool gotStartingItems = false;

        private static List<InvItem> inv = new List<InvItem>();
        private static List<InvItem> inv2 = new List<InvItem>();

        private static List<int> canBuyOnfull = new List<int>();

        private static bool finished = false;
    }

    public class Build
    {
        public List<ItemId> startingItems;
        public List<ConditionalItem> coreItems;
    }

    public class BuyItem
    {
        public FullItem item;
        public int price;
    }


    public class InvItem
    {
        private bool got = false;
        public int id;

        public bool used()
        {
            return got;
        }

        public void setUsed()
        {
            got = true;
        }
    }

    public class ConditionalItem
    {

        private FullItem selected;

        private FullItem primary;
        private FullItem secondary;
        private ItemCondition condition;
        
        public ConditionalItem(ItemId pri, ItemId sec = ItemId.Unknown, ItemCondition cond = ItemCondition.TAKE_PRIMARY)
        {
        }
    }
}
