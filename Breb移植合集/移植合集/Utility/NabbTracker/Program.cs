using System;
using System.Collections.Generic;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using SharpDX;
using Color = System.Drawing.Color;
using LeagueSharp.Common;
using LeagueSharp.Common.Data;

namespace NabbTracker
{
    using System;

    class Program
    {
        public static void Game_OnGameLoad()
        {
            Tracker.OnLoad();
            Chat.Print("Nabb<font color=\"#228B22\">Tracker</font>: Ultima - Loaded!");
        }
    }
}