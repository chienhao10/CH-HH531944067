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
    internal static class SmiteManager
    {
        private static Obj_AI_Base Minion;

        private static readonly string[] SmiteableMinions =
        {
            "SRU_Red", "SRU_Blue", "SRU_Dragon", "SRU_Baron",
            "SRU_Gromp", "SRU_Murkwolf", "SRU_Razorbeak", "SRU_Krug", "Sru_Crab", "TT_Spiderboss", "TTNGolem", "TTNWolf",
            "TTNWraith"
        };

        private static Menu Menu;

        private static LeagueSharp.Common.Spell Smite
        {
            get { return SpellManager.Smite; }
        }

        private static AIHeroClient Player
        {
            get { return ObjectManager.Player; }
        }

        private static IEnumerable<Obj_AI_Base> NearbyMinions
        {
            get
            {
                return MinionManager.GetMinions(
                    ObjectManager.Player.ServerPosition, 500, MinionTypes.All, MinionTeam.Neutral);
            }
        }

        public static void Initialize()
        {
            Menu = SpellManager.Menu.AddSubMenu("Smite", "Smite");
            Menu.Add("Enabled", new KeyBind("Smite Enabled", true, KeyBind.BindTypes.PressToggle, 'K'));
            Menu.Add("DrawSmite", new CheckBox("Draw Smite Range"));
            Menu.Add("DrawDamage", new CheckBox("Draw Smite Damage"));
            Drawing.OnDraw += Drawing_OnDraw;
            Game.OnUpdate += Game_OnUpdate;
        }

        private static void Game_OnUpdate(EventArgs args)
        {
            if (Minion != null && !Minion.IsValidTarget(float.MaxValue, false))
            {
                Minion = null;
            }

            if (!Menu["Enabled"].Cast<CheckBox>().CurrentValue || Player.IsDead || !Smite.IsReady())
            {
                return;
            }

            var minion =
                NearbyMinions.FirstOrDefault(
                    buff => buff.IsValidTarget() && SmiteableMinions.Contains(buff.CharData.BaseSkinName));

            if (minion == null || !minion.IsValid)
            {
                return;
            }

            Minion = minion;

            if (Player.GetSummonerSpellDamage(minion, LeagueSharp.Common.Damage.SummonerSpell.Smite) > Minion.Health)
            {
                Smite.CastOnUnit(Minion);
            }
        }

        private static void Drawing_OnDraw(EventArgs args)
        {
            if (ObjectManager.Player.IsDead || !Menu["Enabled"].Cast<CheckBox>().CurrentValue)
            {
                return;
            }

            if (Menu["DrawSmite"].Cast<CheckBox>().CurrentValue)
            {
                if (!Smite.IsReady())
                {
                    Render.Circle.DrawCircle(ObjectManager.Player.Position, 500, Color.Red);

                    return;
                }
                Render.Circle.DrawCircle(ObjectManager.Player.Position, 500, Color.Green);
            }

            if (Menu["DrawDamage"].Cast<CheckBox>().CurrentValue && Minion != null && Minion.IsValid && !Minion.IsDead &&
                Minion.IsVisible && Minion.IsHPBarRendered)
            {
                DrawMinion(Minion);
            }
        }

        private static void DrawMinion(Obj_AI_Base minion)
        {
            var hpBarPosition = minion.HPBarPosition;
            var maxHealth = minion.MaxHealth;
            var sDamage = Player.GetSummonerSpellDamage(minion, LeagueSharp.Common.Damage.SummonerSpell.Smite);
            var x = sDamage / maxHealth;
            var barWidth = 0;

            switch (minion.CharData.BaseSkinName)
            {
                case "SRU_Red":
                case "SRU_Blue":
                case "SRU_Dragon":
                    barWidth = 145;
                    Drawing.DrawLine(
                        new Vector2(hpBarPosition.X + 3 + (float) (barWidth * x), hpBarPosition.Y + 18),
                        new Vector2(hpBarPosition.X + 3 + (float) (barWidth * x), hpBarPosition.Y + 28), 2f,
                        Color.Chartreuse);
                    Drawing.DrawText(
                        hpBarPosition.X - 22 + (float) (barWidth * x), hpBarPosition.Y, Color.Chartreuse,
                        sDamage.ToString());
                    break;
                case "SRU_Baron":
                    barWidth = 194;
                    Drawing.DrawLine(
                        new Vector2(hpBarPosition.X - 22 + (float) (barWidth * x), hpBarPosition.Y + 13),
                        new Vector2(hpBarPosition.X - 22 + (float) (barWidth * x), hpBarPosition.Y + 29), 2f,
                        Color.Chartreuse);
                    Drawing.DrawText(
                        hpBarPosition.X - 22 + (float) (barWidth * x), hpBarPosition.Y - 3, Color.Chartreuse,
                        sDamage.ToString());
                    break;
                case "Sru_Crab":
                    barWidth = 61;
                    Drawing.DrawLine(
                        new Vector2(hpBarPosition.X + 45 + (float) (barWidth * x), hpBarPosition.Y + 34),
                        new Vector2(hpBarPosition.X + 45 + (float) (barWidth * x), hpBarPosition.Y + 37), 2f,
                        Color.Chartreuse);
                    Drawing.DrawText(
                        hpBarPosition.X + 40 + (float) (barWidth * x), hpBarPosition.Y + 16, Color.Chartreuse,
                        sDamage.ToString());
                    break;
                case "SRU_Murkwolf":
                    barWidth = 75;
                    Drawing.DrawLine(
                        new Vector2(hpBarPosition.X + 54 + (float) (barWidth * x), hpBarPosition.Y + 19),
                        new Vector2(hpBarPosition.X + 54 + (float) (barWidth * x), hpBarPosition.Y + 23), 2f,
                        Color.Chartreuse);
                    Drawing.DrawText(
                        hpBarPosition.X + 50 + (float) (barWidth * x), hpBarPosition.Y, Color.Chartreuse,
                        sDamage.ToString());
                    break;
                case "SRU_Razorbeak":
                    barWidth = 75;
                    Drawing.DrawLine(
                        new Vector2(hpBarPosition.X + 54 + (float) (barWidth * x), hpBarPosition.Y + 18),
                        new Vector2(hpBarPosition.X + 54 + (float) (barWidth * x), hpBarPosition.Y + 22), 2f,
                        Color.Chartreuse);
                    Drawing.DrawText(
                        hpBarPosition.X + 54 + (float) (barWidth * x), hpBarPosition.Y, Color.Chartreuse,
                        sDamage.ToString());
                    break;
                case "SRU_Krug":
                    barWidth = 81;
                    Drawing.DrawLine(
                        new Vector2(hpBarPosition.X + 58 + (float) (barWidth * x), hpBarPosition.Y + 18),
                        new Vector2(hpBarPosition.X + 58 + (float) (barWidth * x), hpBarPosition.Y + 22), 2f,
                        Color.Chartreuse);
                    Drawing.DrawText(
                        hpBarPosition.X + 54 + (float) (barWidth * x), hpBarPosition.Y, Color.Chartreuse,
                        sDamage.ToString());
                    break;
                case "SRU_Gromp":
                    barWidth = 87;
                    Drawing.DrawLine(
                        new Vector2(hpBarPosition.X + 62 + (float) (barWidth * x), hpBarPosition.Y + 18),
                        new Vector2(hpBarPosition.X + 62 + (float) (barWidth * x), hpBarPosition.Y + 22), 2f,
                        Color.Chartreuse);
                    Drawing.DrawText(
                        hpBarPosition.X + 58 + (float) (barWidth * x), hpBarPosition.Y, Color.Chartreuse,
                        sDamage.ToString());
                    break;
            }
        }
    }
}