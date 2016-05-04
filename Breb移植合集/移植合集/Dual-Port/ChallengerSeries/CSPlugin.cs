#region License
/* Copyright (c) LeagueSharp 2016
 * No reproduction is allowed in any way unless given written consent
 * from the LeagueSharp staff.
 * 
 * Author: imsosharp
 * Date: 2/20/2016
 * File: CSPlugin.cs
 */
#endregion License

using System;
using System.Collections.Generic;
using System.Linq;
using Challenger_Series.Utils;
using LeagueSharp;
using LeagueSharp.SDK;
using SharpDX;
using Color = System.Drawing.Color;
using LeagueSharp.Data.Enumerations;

namespace Challenger_Series
{
    using EloBuddy;
    using EloBuddy.SDK;
    using EloBuddy.SDK.Menu;
    using EloBuddy.SDK.Menu.Values;
    using LeagueSharp.SDK.Core.Utils;

    public abstract class CSPlugin
    {
        public Menu CrossAssemblySettings;
        public bool IsPerformanceChallengerEnabled;
        public int TriggerOnUpdate;
        private int _lastOnUpdateTriggerT = 0;

        public static bool getCheckBoxItem(Menu m, string item)
        {
            return m[item].Cast<CheckBox>().CurrentValue;
        }

        public static int getSliderItem(Menu m, string item)
        {
            return m[item].Cast<Slider>().CurrentValue;
        }

        public static bool getKeyBindItem(Menu m, string item)
        {
            return m[item].Cast<KeyBind>().CurrentValue;
        }

        public static int getBoxItem(Menu m, string item)
        {
            return m[item].Cast<ComboBox>().CurrentValue;
        }


        public CSPlugin()
        {
            MainMenu = EloBuddy.SDK.Menu.MainMenu.AddMenu("challengerseries", ObjectManager.Player.ChampionName + " To The Challenger");
            CrossAssemblySettings = MainMenu.AddSubMenu("Challenger Utils: ");
            CrossAssemblySettings.Add("performancechallengerx", new CheckBox("Use Performance Challenger", false));
            CrossAssemblySettings.Add("triggeronupdate", new Slider("Trigger OnUpdate X times a second", 26, 20, 33));

            IsPerformanceChallengerEnabled = getCheckBoxItem(CrossAssemblySettings, "performancechallengerx");
            TriggerOnUpdate = getSliderItem(CrossAssemblySettings, "triggeronupdate");

            Game.OnUpdate += this.DelayOnUpdate;
        }

        public IEnumerable<AIHeroClient> ValidTargets { get {return EntityManager.Heroes.Enemies.Where(enemy=>enemy.IsHPBarRendered);}}        
        public Menu MainMenu { get; set; }
        public virtual void OnUpdate(EventArgs args) { }
        public virtual void OnProcessSpellCast(GameObject sender, GameObjectProcessSpellCastEventArgs args) { }
        public virtual void OnDraw(EventArgs args) { }
        public virtual void InitializeMenu() { }

        public delegate void DelayedOnUpdateEH(EventArgs args);

        public event DelayedOnUpdateEH DelayedOnUpdate;

        public void DelayOnUpdate(EventArgs args)
        {
            IsPerformanceChallengerEnabled = getCheckBoxItem(CrossAssemblySettings, "performancechallengerx");
            TriggerOnUpdate = getSliderItem(CrossAssemblySettings, "triggeronupdate");
            if (this.DelayedOnUpdate != null)
            {
                if (this.IsPerformanceChallengerEnabled && Variables.TickCount - this._lastOnUpdateTriggerT > 1000 / this.TriggerOnUpdate)
                {
                    this._lastOnUpdateTriggerT = Variables.TickCount;
                    this.DelayedOnUpdate(args);
                    return;
                }
                this.DelayedOnUpdate(args);
            }
        }
    }
}
