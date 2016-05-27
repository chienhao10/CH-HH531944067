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
using HTrackerSDK;

namespace NabbTracker
{
    /// <summary>
    /// The main class.
    /// </summary>
    class Tracker
    {
        /// <summary>
        /// Called when the game loads itself.
        /// </summary>
        public static void OnLoad()
        {
            Menus.Initialize();          
            Drawings.Initialize();

            if (Variables.Menu["ward"].Cast<CheckBox>().CurrentValue)
            {
                HTrackerSDK.WardTracker.OnLoad(Variables.Menu);
            }

            ExpTracker.Initialize();
        }
    }
}
