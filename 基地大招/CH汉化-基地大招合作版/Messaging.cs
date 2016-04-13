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
            OwnDelayInfo,
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
        /// <value name="taretName">Champion name</value>
        /// <value name="type"></value>
        /// <value name="param">time parameter</value>
        public static void ProcessInfo(string targetName, MessagingType type, float param)
        {
            bool spam = CheckSpam(type);
            if (spam)
                return;
            SetLastSendTime(type);

            SendMessage(param, targetName, type);
        }

        private static void SendMessage(float value, string targetName, MessagingType type)
        {
            switch (type)
            {
                case MessagingType.OwnDelayInfo:
                    Chat.Print("<font color=\"#0cf006\">" + targetName + "=> My ult cast delay: " + value + " ms</font>");
                break;
                case MessagingType.DelayTooSmall:
                    string msg2 = "<font color=\"#D01616\">" + "Regeneration delay too small: " + value + "</font>";
                    Chat.Print(msg2);
                    //AllyMessaging.SendMessageToAllies("Regeneration delay too small: " + value);
                break;
                case MessagingType.NotEnoughTime:
                    Chat.Print("<font color=\"#D01616\">" + "Not enough time for me. Target: " + targetName + "</font>");
                break;
                case MessagingType.NotEnougDamage:
                    string msg4 = "<font color=\"#D01616\">" + "Not enough damage at all: " + value + "</font>";
                    Chat.Print(msg4);
                    //AllyMessaging.SendMessageToAllies("Not enough damage at all: " + value);
                break;
            }
        }

        private static bool CheckSpam(MessagingType type)
        {
            switch (type)
            {
                case MessagingType.OwnDelayInfo:
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
                case MessagingType.OwnDelayInfo:
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
            get { return Listing.MiscMenu.Get<CheckBox>("allyMessaging").CurrentValue; }
        }

        static Vector3 enemySpawn
        {
            get { return ObjectManager.Get<Obj_SpawnPoint>().First(x => x.IsEnemy).Position; }
        }

        public static void SendBaseUltInfoToAllies(float timeLeft, float regInBaseDelay)
        {
            if (!Enabled)
                return;

            int premadesInvolvedCount = 0;

            foreach (var ally in EntityManager.Heroes.Allies.Where(
                x => x.IsValid && x.Health > 0 && !x.ChampionName.ToLower().Contains("karthus") && !x.IsMe))
            {
                bool isGlobalChamp = Listing.UltSpellDataList.Any(x => x.Key == ally.ChampionName);
                if (!isGlobalChamp)
                    continue;

                string menuid = ally.ChampionName + "/Premade";
                bool isPremade = Listing.allyconfig.Get<CheckBox>(menuid).CurrentValue;
                if (!isPremade)
                    continue;

                var spell = ally.Spellbook.GetSpell(SpellSlot.R);
                var cooldown = spell.CooldownExpires - Game.Time;

                float travelTime = Algorithm.GetUltTravelTime(ally, enemySpawn);
                bool intime = travelTime <= timeLeft + regInBaseDelay;
                //bool collision = Algorithm.GetCollision(ally.ChampionName, enemySpawn).Any();

                float delay = timeLeft + regInBaseDelay - travelTime;
                bool canr = cooldown <= delay/1000 && ally.Mana >= 100 && ally.Level >= 6;

                bool messageSpam = CheckMessageSpam(ally);
                bool lastMoment = timeLeft + regInBaseDelay - travelTime <= 300;

                if (canr && intime && (!messageSpam || lastMoment))
                {
                    OnMessageSent(ally);
                    string roundedDelay = (delay / 1000).ToString("0.0");

                    string msg = !lastMoment
                        ? ally.Name + " CountDownnnnnnnnnnnnnnnnnnn: " + roundedDelay
                        : ally.Name + " GOOO!";
                    Chat.Say("/w " + msg);
                    premadesInvolvedCount++;
                }
            }

            #region karthus with premades
            var karthusAlly =
                EntityManager.Heroes.Allies.FirstOrDefault(x => x.IsValid && x.ChampionName.ToLower().Contains("karthus"));

            if (karthusAlly != null && premadesInvolvedCount > 0)
            {
                string karthusMenuid = karthusAlly.ChampionName + "/Premade";
                bool isKarthusPremade = Listing.allyconfig.Get<CheckBox>(karthusMenuid).CurrentValue;

                if (isKarthusPremade && premadesInvolvedCount > 0)
                {
                    var spell = karthusAlly.Spellbook.GetSpell(SpellSlot.R);
                    var cooldown = spell.CooldownExpires - Game.Time;

                    bool intimeKarthus = timeLeft + regInBaseDelay >= 4000;
                    float delay = timeLeft + regInBaseDelay - 4000;

                    bool canr = cooldown <= delay/1000 && karthusAlly.Mana >= 100 && karthusAlly.Level >= 6;

                    bool messageSpam = CheckMessageSpam(karthusAlly);
                    bool lastMoment = timeLeft + regInBaseDelay - 4000 <= 300;

                    if (canr && intimeKarthus && (!messageSpam || lastMoment))
                    {
                        OnMessageSent(karthusAlly);
                        string roundedDelay = (delay / 1000).ToString("0.0");

                        string msg = !lastMoment
                        ? karthusAlly.Name + " CountDownnnnnnnnnnnnnnnnnnn: " + roundedDelay
                        : karthusAlly.Name + " GOOO!";
                        Chat.Say("/w " + msg);
                    }
                }
            }
            #endregion  karthus with premades
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
                return Environment.TickCount - lastMessageSendTicksToAllies[ally.ChampionName] < 500;

            return false;
        }

        public static void SendMessageToAllies(string msg)
        {
            if (!Enabled)
                return;

            foreach (var ally in EntityManager.Heroes.Allies)
            {
                bool isGlobalUltChamp =
                    Listing.UltSpellDataList.Any(
                        x => x.Key == ally.ChampionName && x.Key != ObjectManager.Player.ChampionName);
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
