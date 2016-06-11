using System;
using System.Drawing;
using LeagueSharp;
using LeagueSharp.Common;
using Color = SharpDX.Color;

namespace Marksman.Common
{
    using EloBuddy;
    using EloBuddy.SDK.Menu;
    using EloBuddy.SDK.Menu.Values;
    using System.Linq;
    using System.Security.AccessControl;

    public static class CommonEmote
    {
        public static Menu LocalMenu;

        public static int[] SpellLevels;

        public static void Init(Menu nParentMenu)
        {
            LocalMenu = nParentMenu;
            LocalMenu.AddGroupLabel("Emote");
            LocalMenu.Add("Emote.Kill", new ComboBox("Kill:", 0, "Off", "Master Badge", "Laugh", "Taunt", "Joke", "Dance"));
            LocalMenu.Add("Emote.Assist", new ComboBox("Assist:", 0, "Off", "Master Badge", "Laugh", "Taunt", "Joke", "Dance"));
            LocalMenu.Add("Emote.Victory", new ComboBox("Victory:", 0, "Off", "Master Badge", "Laugh", "Taunt", "Joke", "Dance"));
            LocalMenu.Add("Emote.Enable", new CheckBox("Enable:"));

            Game.OnNotify += GameOnOnNotify;

        }

        static void ExecuteEmote(int nEmote)
        {
            switch (nEmote)
            {
                case 1:
                    Chat.Say("/masterybadge");
                    break;
                case 2:
                    Player.DoEmote(Emote.Laugh);
                    break;
                case 3:
                    Player.DoEmote(Emote.Taunt);
                    break;
                case 4:
                    Player.DoEmote(Emote.Joke);
                    break;
                case 5:
                    Player.DoEmote(Emote.Dance);
                    break;
            }
        }

        private static void GameOnOnNotify(GameNotifyEventArgs args)
        {
            if (!LocalMenu["Emote.Enable"].Cast<CheckBox>().CurrentValue)
            {
                return;
            }

            var nEmoteKill = LocalMenu["Emote.Kill"].Cast<ComboBox>().CurrentValue;
            if (nEmoteKill != 0 && args.EventId == GameEventId.OnChampionKill)
            {
                ExecuteEmote(nEmoteKill);
            }

            nEmoteKill = LocalMenu["Emote.Assist"].Cast<ComboBox>().CurrentValue;
            if (nEmoteKill != 0 && args.EventId == GameEventId.OnDeathAssist)
            {
                ExecuteEmote(nEmoteKill);
            }

            nEmoteKill = LocalMenu["Emote.Victory"].Cast<ComboBox>().CurrentValue;
            if (nEmoteKill != 0 && args.EventId == GameEventId.OnVictoryPointThreshold1)
            {
                ExecuteEmote(nEmoteKill);
            }
        }
    }
}
