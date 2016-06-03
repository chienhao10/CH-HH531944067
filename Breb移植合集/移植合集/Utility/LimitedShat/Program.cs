using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Color = System.Drawing.Color;
using Font = SharpDX.Direct3D9.Font;
using System.Timers;
using LeagueSharp;
using LeagueSharp.Common;

using System.Threading;
using SharpDX;
using EloBuddy;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK.Menu;

namespace LimitedShat
{

    internal class Program
    {
        #region DeclarationVar
        static Menu _main;
        static Vector2 _screenPos;

        static List<string> _allowed = new List<string> { "/msg", "/r", "/w", "/surrender", "/nosurrender", "/help", "/dance", "/d", "/taunt", "/t", "/joke", "/j", "/laugh", "/l", "/ff" };
        private static int _Count = 3;

        #endregion

        public static void Game_OnGameLoad()
        {
            _main = MainMenu.AddMenu("LimitedShat", "LimitedShat");
            _main.Add("drawing", new CheckBox("Drawing"));
            var posX = _main.Add("positionx", new Slider("Position X", Drawing.Width - 100, 0, Drawing.Width - 20));
            var posY = _main.Add("positiony", new Slider("Position Y", Drawing.Height / 2, 0, Drawing.Height - 20));
            posX.OnValueChange += (sender, arg) => _screenPos.X = arg.NewValue;
            posY.OnValueChange += (sender, arg) => _screenPos.Y = arg.NewValue;
            _screenPos.X = posX.Cast<Slider>().CurrentValue;
            _screenPos.Y = posY.Cast<Slider>().CurrentValue;
            Drawing.OnDraw += Drawing_OnDraw;
            Chat.OnInput += Game_OnInput;
            timerTick();
        }

        static void Drawing_OnDraw(EventArgs args)
        {
            if (!_main["drawing"].Cast<CheckBox>().CurrentValue) return;
            Drawing.DrawText(_screenPos.X, _screenPos.Y, Color.Yellow, "Messages Available: " + (_Count.ToString()) + "/5");
        }


        static void Game_OnInput(ChatInputEventArgs args)
        {
            if (_Count < 0)
                _Count = 0;
            if ((_Count == 0) && (!args.Input.Equals("")))
            {
                args.Process = false;
                Chat.Print("Limit exceeded");
            }

            if (_allowed.Any(str => args.Input.StartsWith(str)))
            {
                args.Process = true;
            }
            if (!_allowed.Any(str => args.Input.StartsWith(str)) && (_Count > 0) && (!args.Input.Equals("")))
            {
                _Count--;
                args.Process = true;
            }
        }

        static void timerTick()
        {
            var enableTimer = new System.Timers.Timer(240000); // 4 minutes = 240 000 ms
            enableTimer.Elapsed += enableTimer_Elapsed;
            enableTimer.Enabled = true;
            enableTimer.Start();
        }

        static void enableTimer_Elapsed(Object sender, ElapsedEventArgs e)
        {
            increment();
        }

        static void increment()
        {
            if (_Count < 5)
            {
                _Count++;
            }
        }
    }
}
