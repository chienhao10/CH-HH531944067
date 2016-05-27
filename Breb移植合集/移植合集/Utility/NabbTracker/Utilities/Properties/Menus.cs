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
    using System.Drawing;
    using Color = SharpDX.Color;

    /// <summary>
    /// The menu class.
    /// </summary>
    class Menus
    {
        /// <summary>
        /// Builds the general Menu.
        /// </summary>
        public static void Initialize()
        {
            /// <summary>
            /// The general Menu.
            /// </summary>
            Variables.Menu = MainMenu.AddMenu(Variables.MainMenuCodeName, Variables.MainMenuName);
            Variables.Menu.AddGroupLabel("技能计时");
            Variables.Menu.Add("allies", new CheckBox("友军 开启"));
            Variables.Menu.Add("enemies", new CheckBox("敌方 开启"));
            Variables.Menu.AddGroupLabel("眼计时");
            Variables.Menu.Add("ward", new CheckBox("开启眼位计时"));
            Variables.Menu.AddGroupLabel("经验计算");
            Variables.Menu.Add("me", new CheckBox("计算自身"));
            Variables.Menu.Add("alliesEXP", new CheckBox("计算 友军"));
            Variables.Menu.Add("enemiesEXP", new CheckBox("计算 敌人"));
        }
    }
}
