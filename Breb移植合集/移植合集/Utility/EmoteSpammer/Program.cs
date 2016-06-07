#region
using System;
using System.Collections.Generic;
using System.Linq;

using System.Text;
using System.Threading;
using SharpDX;
using LeagueSharp;
using LeagueSharp.Common;
using EloBuddy.SDK.Menu;
using EloBuddy;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK;
#endregion

namespace EmoteSpammer
{
    internal class Program
    {
        private static Menu Config;
        public static int tick;

        public static void Game_OnGameLoad()
        {
            Config = MainMenu.AddMenu("表情发送器", "EmoteSpammer");
            Config.Add("EmotePress", new KeyBind("一键发送表情", false, KeyBind.BindTypes.HoldActive, 32));
            Config.Add("EmoteToggable", new KeyBind("开关按键", false, KeyBind.BindTypes.PressToggle, 'H'));
            Config.Add("Type", new ComboBox("发送哪一个表情（动作）?", 0, "笑", "嘲讽", "笑话", "跳舞"));
            Config.Add("delay", new Slider("Delay", 0, 0, 1000));

            Game.OnUpdate += OnUpdate;
        }

        private static void OnUpdate(EventArgs args)
        {
            if (ObjectManager.Player.HasBuff("Recall")) return;
            {
                if (Core.GameTickCount - tick >= Config["delay"].Cast<Slider>().CurrentValue)
                {
                    if (Config["EmotePress"].Cast<KeyBind>().CurrentValue)
                    {
                        SPAM();
                    }
                    if (Config["EmoteToggable"].Cast<KeyBind>().CurrentValue)
                    {
                        SPAM();
                    }
                }
            }
        }

        private static void SPAM()
        {
            if (Config["Type"].Cast<ComboBox>().CurrentValue == 0)
            {
                tick = Core.GameTickCount;
                Player.DoEmote(Emote.Laugh);
            }
            if (Config["Type"].Cast<ComboBox>().CurrentValue == 1)
            {
                tick = Core.GameTickCount;
                Player.DoEmote(Emote.Taunt);
            }
            if (Config["Type"].Cast<ComboBox>().CurrentValue == 2)
            {
                tick = Core.GameTickCount;
                Player.DoEmote(Emote.Joke);
            }
            if (Config["Type"].Cast<ComboBox>().CurrentValue == 3)
            {
                tick = Core.GameTickCount;
                Player.DoEmote(Emote.Dance);
            }
            Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos, false);
        }
    }
}
