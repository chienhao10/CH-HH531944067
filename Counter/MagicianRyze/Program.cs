using System;
using System.Drawing;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu.Values;

namespace MagicianRyze
{
    // Created by Counter
    internal class Program
    {
        // Grab Player Attributes
        public static AIHeroClient Champion { get { return Player.Instance; } }
        public static int ChampionSkin;

        public static void Main()
        {
            Loading.OnLoadingComplete += Loading_OnLoadingComplete;
        }

        public static void Loading_OnLoadingComplete(EventArgs args)
        {
            // Validate Player.Instace is Addon Champion
            if (Champion.ChampionName != "Ryze") return;
            ChampionSkin = Champion.SkinId;

            // Initialize classes
            SpellManager.Initialize();
            MenuManager.Initialize();
            
            // Listen to Events
            Drawing.OnDraw += Drawing_OnDraw;
            Game.OnUpdate += Game_OnUpdate;
            Game.OnTick += Game_OnTick;
            Interrupter.OnInterruptableSpell += ModeManager.InterruptMode;
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

            // Setup Designer Coloration
            switch (Champion.SkinId)
            {
                default:
                    color = Color.Transparent;
                    break;
                case 0:
                    color = Color.SkyBlue;
                    break;
                case 1:
                    color = Color.Green;
                    break;
                case 2:
                    color = Color.DeepSkyBlue;
                    break;
                case 3:
                    color = Color.LightSkyBlue;
                    break;
                case 4:
                    color = Color.RoyalBlue;
                    break;
                case 5:
                    color = Color.MediumPurple;
                    break;
                case 6:
                    color = Color.ForestGreen;
                    break;
                case 7:
                    color = Color.DarkMagenta;
                    break;
                case 8:
                    color = Color.Firebrick;
                    break;
                case 9:
                    color = Color.SlateBlue;
                    break;
            }

            // Apply Designer Color into Circle
            if (!MenuManager.DrawMode) return;
            if (MenuManager.DrawQ && SpellManager.Q.IsLearned)
                Drawing.DrawCircle(Champion.Position, SpellManager.Q.Range, color);
            if (MenuManager.DrawWe && (SpellManager.W.IsLearned || SpellManager.E.IsLearned))
                Drawing.DrawCircle(Champion.Position, SpellManager.W.Range, color);
        }

        public static void Game_OnTick(EventArgs args)
        {
            // Initialize Leveler
            if (MenuManager.LevelerMode && Champion.SpellTrainingPoints >= 1)
                LevelerManager.Initialize();

            // Design Menu Options
            MenuManager.ComboMenu["Ucombo"].DisplayName = MenuManager.ComboMode == 1
                ? "作者连招 - 作者喜欢的连招"
                : "明星连招 - 最快的连招技能";

            // No Responce While Dead
            if (Champion.IsDead) return;

            // Mode Activation
            switch (Orbwalker.ActiveModesFlags)
            {
                case Orbwalker.ActiveModes.Combo:
                    if (MenuManager.ComboMenu["Ucombo"].Cast<Slider>().DisplayName == "作者连招 - 作者喜欢的连招")
                        ModeManager.CounterCombo();
                    if (MenuManager.ComboMenu["Ucombo"].Cast<Slider>().DisplayName == "明星连招 - 最快的连招技能")
                        ModeManager.SluttyCombo();
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
            if (MenuManager.StackMode)
                ModeManager.StackMode();
        }
    }
}