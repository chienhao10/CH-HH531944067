using ClipperLib;
using Color = System.Drawing.Color;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK;
using EloBuddy;
using Font = SharpDX.Direct3D9.Font;
using LeagueSharp.Common.Data;
using LeagueSharp.Common;
using SharpDX.Direct3D9;
using SharpDX;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using System.Security.AccessControl;
using System;
using System.Speech.Synthesis;
using Nechrito_Nidalee.Drawings;
using Nechrito_Nidalee.Handlers;

namespace Nechrito_Nidalee
{
    class Program : Core
    {
        public static void OnLoad()
        {
            if (Player.ChampionName != "Nidalee")
                return;
            
           // Modes.Flee();
            Drawing.OnEndScene += Drawing_OnEndScene;
            Drawing.OnDraw += DRAWING.Drawing_OnDraw;
            Game.OnUpdate += OnUpdate;
            Champion.Load();
            MenuConfig.Load();
            Extras.Item.SmiteCombo();
            Extras.Item.SmiteJungle();
        }
        private static void OnUpdate(EventArgs args)
        {
            HealManager.Heal();
            Killsteal.KillSteal();
            Modes.Flee();

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                Modes.Combo();
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                Modes.Harass();
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear) || Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear))
            {
                Modes.Lane();
                Modes.Jungle();
            }
        }
        private static readonly HpBarIndicator Indicator = new HpBarIndicator();
        private static void Drawing_OnEndScene(EventArgs args)
        {
            foreach (var enemy in ObjectManager.Get<AIHeroClient>().Where(ene => ene.IsValidTarget() && !ene.IsZombie))
            {
                if (MenuConfig.dind)
                {
                    var EasyKill = Champion.Javelin.IsReady() && Dmg.IsLethal(enemy)
                       ? new ColorBGRA(0, 255, 0, 120)
                       : new ColorBGRA(255, 255, 0, 120);
                    Indicator.unit = enemy;
                    Indicator.drawDmg(Dmg.ComboDmg(enemy), EasyKill);
                }
            }
        }
    }
}
