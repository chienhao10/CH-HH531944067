using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using Microsoft.Win32;
using SharpDX;
using System.Threading;
using System.Net;
using EloBuddy.SDK.Rendering;
using System.Diagnostics;
using Color = System.Drawing.Color;
using System.Drawing;

namespace EloBuddy
{
    class Program
    {
        static void Main(string[] args)
        {
            Loading.OnLoadingComplete += Loading_OnLoadingComplete;
        }

        private static void Loading_OnLoadingComplete(EventArgs args)
        {
            switch (Player.Instance.ChampionName)
            {
                case "Gangplank":
                    new UnrealSkill.Gangplank();
                    break;
            }
        }
    }
}
