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
using TreeLib.Extensions;

namespace TreeLib.Managers
{
    internal static class IgniteManager
    {
        public static Menu Menu;

        public static void Initialize()
        {
            Menu = SpellManager.Menu.AddSubMenu("Ignite", "Ignite");
            Menu.Add("IgniteEnabled", new CheckBox("Ignite Enabled"));

            Game.OnUpdate += Game_OnUpdate;
        }

        private static void Game_OnUpdate(EventArgs args)
        {
            if (!Menu["IgniteEnabled"].Cast<CheckBox>().CurrentValue || SpellManager.Ignite == null || !SpellManager.Ignite.IsReady() ||
                ObjectManager.Player.IsDead)
            {
                return;
            }

            var target =
                HeroManager.Enemies.FirstOrDefault(
                    h =>
                        h.IsValidTarget(SpellManager.Ignite.Range) &&
                        h.Health < ObjectManager.Player.GetSummonerSpellDamage(h, LeagueSharp.Common.Damage.SummonerSpell.Ignite));
            if (target != null)
            {
                SpellManager.Ignite.Cast(target);
            }
        }
    }
}