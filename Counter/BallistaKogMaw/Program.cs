using System;
using System.Drawing;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Events;

namespace BallistaKogMaw
{
    // Created by Counter
    internal class Program
    {
        // Grab Player Attributes
        public static AIHeroClient Champion { get { return Player.Instance; } }
        public static int ChampionSkin;

        // Global Passive Target
        public static Obj_AI_Base Ptarget;

        public static void Main()
        {
            Loading.OnLoadingComplete += Loading_OnLoadingComplete;
        }

        public static void Loading_OnLoadingComplete(EventArgs args)
        {
            // Validate Player.Instace is Addon Champion
            if (Champion.ChampionName != "KogMaw") return;
            ChampionSkin = Champion.SkinId;

            // Initialize classes
            SpellManager.Initialize();
            MenuManager.Initialize();

            // Listen to Events
            Drawing.OnDraw += Drawing_OnDraw;
            Game.OnUpdate += Game_OnUpdate;
            Game.OnTick += SpellManager.ConfigSpells;
            Game.OnTick += Game_OnTick;
            Gapcloser.OnGapcloser += ModeManager.GapCloserMode;
        }

        public static void Game_OnUpdate(EventArgs args)
        {
            // Initialize Skin Designer
            Champion.SetSkinId(MenuManager.DesignerMode
                ? MenuManager.DesignerSkin
                : ChampionSkin);
        }

        public static void Drawing_OnDraw(EventArgs args)
        {
            // Wait for Game Load
            if (Game.Time < 10) return;

            // No Responce While Dead
            if (Champion.IsDead) return;

            Color color;
            Color color2;

            // Setup Designer Coloration
            switch (Champion.SkinId)
            {
                default:
                    color = Color.Transparent;
                    color2 = Color.Transparent;
                    break;
                case 0:
                    color = Color.SeaGreen;
                    color2 = Color.Goldenrod;
                    break;
                case 1:
                    color = Color.Lime;
                    color2 = Color.GreenYellow;
                    break;
                case 2:
                    color = Color.DarkOrange;
                    color2 = Color.PaleGreen;
                    break;
                case 3:
                    color = Color.FloralWhite;
                    color2 = Color.Wheat;
                    break;
                case 4:
                    color = Color.Tan;
                    color2 = Color.RosyBrown;
                    break;
                case 5:
                    color = Color.MediumVioletRed;
                    color2 = Color.LightYellow;
                    break;
                case 6:
                    color = Color.SeaGreen;
                    color2 = Color.Goldenrod;
                    break;
                case 7:
                    color = Color.SaddleBrown;
                    color2 = Color.DarkRed;
                    break;
                case 8:
                    color = Color.Firebrick;
                    color2 = Color.Tomato;
                    break;
            }

            // Apply Designer Color into Circle
            if (!MenuManager.DrawMode) return;
            if (!Champion.HasBuff("kogmawicathiansurprise"))
            {
                if (MenuManager.DrawQ && SpellManager.Q.IsLearned)
                    Drawing.DrawCircle(Champion.Position, SpellManager.Q.Range, color);
                if (MenuManager.DrawW && SpellManager.W.IsLearned
                    && Champion.HasBuff("KogMawBioArcaneBarrage"))
                    Drawing.DrawCircle(Champion.Position, SpellManager.W.Range, color2);
                if (MenuManager.DrawE && SpellManager.E.IsLearned)
                    Drawing.DrawCircle(Champion.Position, SpellManager.E.Range, color);
                if (MenuManager.DrawR && SpellManager.R.IsLearned)
                    Drawing.DrawCircle(Champion.Position, SpellManager.R.Range, color);
            }
            else
            {
                Drawing.DrawCircle(Champion.Position, SpellManager.P.Range, color);
            }
        }

        public static void Game_OnTick(EventArgs args)
        {
            // Initialize Leveler
            if (MenuManager.LevelerMode && Champion.SpellTrainingPoints >= 1)
                LevelerManager.Initialize();

            // No Responce While Dead
            if (Champion.IsDead) return;

            // Mode Activation
            if (MenuManager.FollowerMode)
            {
                if (!Champion.HasBuff("kogmawicathiansurprise"))
                    Ptarget = null;
                else
                    ModeManager.DeathFollowMode();
            }
            switch (Orbwalker.ActiveModesFlags)
            {
                case Orbwalker.ActiveModes.Combo:
                    ModeManager.ComboMode();
                    break;
                case Orbwalker.ActiveModes.Harass:
                    ModeManager.HarassMode();
                    break;
                case Orbwalker.ActiveModes.JungleClear:
                    ModeManager.JungleMode();
                    break;
                case Orbwalker.ActiveModes.LaneClear:
                    ModeManager.LaneClearMode();
                    break;
                case Orbwalker.ActiveModes.LastHit:
                    ModeManager.LastHitMode();
                    break;
            }
            if (MenuManager.KsMode)
                ModeManager.KsMode();
            if (MenuManager.StackerMode)
                ModeManager.StackMode();
        }
    }
}