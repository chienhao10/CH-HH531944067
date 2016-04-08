using System;
using System.Collections.Generic;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu.Values;
using SharpDX;

namespace HumanziedBaseUlt
{
    public static class Messaging
    {
        public enum MessagingType
        {
            DelayInfo,
            DelayTooSmall,
            NotEnoughTime,
            NotEnougDamage,
        }

        private static int last_DelayInfo = 0;
        private static int last_DelayTooSmall = 0;
        private static int last_NotEnoughTime = 0;
        private static int last_NotEnoughDamage = 0;

        /// <summary>
        /// Prints ult castDelay in chat
        /// </summary>
        /// <param name="args">Champion name</param>
        /// <param name="type"></param>
        /// <param name="param">time parameter</param>
        public static void ProcessInfo(string args, MessagingType type, float param)
        {
            bool spam = CheckSpam(type);
            if (spam)
                return;

            SendMessage(param, args, type);

            SetLastSendTime(type);
        }

        private static void SendMessage(float param, string args, MessagingType type)
        {
            switch (type)
            {
                case MessagingType.DelayInfo:
                    Chat.Print("<font color=\"#0cf006\">" + args + "My ult cast delay: " + param + " ms</font>");
                break;
                case MessagingType.DelayTooSmall:
                    string msg2 = "<font color=\"#D01616\">" + "Regeneration delay too small: " + param + "</font>";
                    Chat.Print(msg2);
                    AllyMessaging.SendMessageToAllies("Regeneration delay too small: " + param);
                break;
                case MessagingType.NotEnoughTime:
                    Chat.Print("<font color=\"#D01616\">" + "Not enough time for me: " + param + "</font>");
                break;
                case MessagingType.NotEnougDamage:
                    string msg4 = "<font color=\"#D01616\">" + "Not enough damage at all: " + param + "</font>";
                    Chat.Print(msg4);
                    AllyMessaging.SendMessageToAllies("Not enough damage at all: " + param);
                break;
            }
        }

        private static bool CheckSpam(MessagingType type)
        {
            switch (type)
            {
                case MessagingType.DelayInfo:
                    return Environment.TickCount - last_DelayInfo < 15000;
                case MessagingType.DelayTooSmall:
                    return Environment.TickCount - last_DelayTooSmall < 15000;
                case MessagingType.NotEnoughTime:
                    return Environment.TickCount - last_NotEnoughTime < 15000;
                case MessagingType.NotEnougDamage:
                    return Environment.TickCount - last_NotEnoughDamage < 15000;
            }

            return false;
        }

        private static void SetLastSendTime(MessagingType type)
        {
            switch (type)
            {
                case MessagingType.DelayInfo:
                    last_DelayInfo = Environment.TickCount;
                    break;
                case MessagingType.DelayTooSmall:
                    last_DelayTooSmall = Environment.TickCount;
                    break;
                case MessagingType.NotEnoughTime:
                    last_NotEnoughTime = Environment.TickCount;
                    break;
                case MessagingType.NotEnougDamage:
                    last_NotEnoughDamage = Environment.TickCount;
                    break;
            }
        }
    }

    public static class AllyMessaging
    {
        private static bool Enabled {
            get { return Listing.config.Get<CheckBox>("allyMessaging").CurrentValue; }
        }

        static Vector3 enemySpawn
        {
            get { return ObjectManager.Get<Obj_SpawnPoint>().First(x => x.IsEnemy).Position; }
        }

        public static void SendBaseUltInfoToAllies(float timeLeft, float regInBaseDelay)
        {
            if (!Enabled)
                return;

            timeLeft = timeLeft + regInBaseDelay;

            foreach (var ally in EntityManager.Heroes.Allies.Where(x => x.IsValid))
            {
                bool isGlobalUltChamp =
                    Listing.spellDataList.Any(
                        x => x.championName == ally.ChampionName && x.championName != ObjectManager.Player.ChampionName);
                if (isGlobalUltChamp)
                {
                    string menuid = ally.ChampionName + "/Premade";
                    if (Listing.allyconfig.Get<CheckBox>(menuid).CurrentValue)
                    {
                        float travelTime = Algorithm.GetUltTravelTime(ally, enemySpawn);
                        bool canr = ally.Spellbook.GetSpell(SpellSlot.R).IsReady && ally.Mana >= 100;
                        bool intime = travelTime <= timeLeft;
                        float delay = timeLeft - travelTime;
                        bool collision = Algorithm.GetCollision(ally.ChampionName, enemySpawn).Any();
                        bool messageSpam = CheckMessageSpam(ally);

                        if (canr && intime && !collision && !messageSpam)
                        {
                            OnMessageSent(ally);
                            Chat.Say("/w " + ally.Name + "BaseUlt CountDown:" + delay);
                        }
                    }
                }
            }
        }

        static readonly Dictionary<string, int> lastMessageSendTicksToAllies = new Dictionary<string, int>(4);
        private static void OnMessageSent(AIHeroClient ally)
        {
            if (lastMessageSendTicksToAllies.ContainsKey(ally.ChampionName))
                lastMessageSendTicksToAllies[ally.ChampionName] = Environment.TickCount;
            else
                lastMessageSendTicksToAllies.Add(ally.ChampionName, Environment.TickCount);
        }

        static bool CheckMessageSpam(AIHeroClient ally)
        {
            bool infoExists = lastMessageSendTicksToAllies.ContainsKey(ally.ChampionName);
            if (infoExists)
                return Environment.TickCount - lastMessageSendTicksToAllies[ally.ChampionName] < 1000;

            return false;
        }

        public static void SendMessageToAllies(string msg)
        {
            if (!Enabled)
                return;

            foreach (var ally in EntityManager.Heroes.Allies)
            {
                bool isGlobalUltChamp =
                    Listing.spellDataList.Any(
                        x => x.championName == ally.ChampionName && x.championName != ObjectManager.Player.ChampionName);
                if (isGlobalUltChamp)
                {
                    string menuid = ally.ChampionName + "/Premade";
                    if (Listing.allyconfig.Get<CheckBox>(menuid).CurrentValue)
                    {
                        Chat.Say("/w " + ally.Name + " " + msg);
                    }
                }
            }
        }
    }
}
