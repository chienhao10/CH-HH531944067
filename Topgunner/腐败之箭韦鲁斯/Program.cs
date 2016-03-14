using System;
using System.Collections.Generic;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK.Rendering;
using SharpDX;
using Settings = CorruptedVarus.Config.Misc;
using CorruptedVarus.Modes;
namespace CorruptedVarus
{
    public static class Program
    {
        // Change this line to the champion you want to make the addon for,
        // watch out for the case being correct!
        public const string ChampName = "Varus";
        public static AIHeroClient lastTarget;
        public static float lastSeen = Game.Time;
        public static Vector3 predictedPos;
        public static bool disableMove = false;

        public static void Main(string[] args)
        {
            // Wait till the loading screen has passed
            Loading.OnLoadingComplete += OnLoadingComplete;
        }

        private static void OnLoadingComplete(EventArgs args)
        {
            // Verify the champion we made this addon for
            if (Player.Instance.ChampionName != ChampName)
            {
                // Champion is not the one we made this addon for,
                // therefore we return
                return;
            }

            // Initialize the classes that we need
            Config.Initialize();
            SpellManager.Initialize();
            ModeManager.Initialize();
            SaveMePls.Initialize();
            if (Settings.autolevelskills)
            {
                Player.Instance.Spellbook.LevelSpell(SpellSlot.Q);
            }
            // Listen to events we need
            Drawing.OnDraw += OnDraw;
            Player.OnLevelUp += CorruptedVarus.Modes.PermaActive.autoLevelSkills;
            Player.OnBasicAttack += Player_OnBasicAttack;
            Game.OnTick += Game_OnTick;
            Orbwalker.OnPostAttack += Modes.Combo.Spellblade;            
        }

        private static void Game_OnTick(EventArgs args)
        {
            if (lastTarget != null)
            {
                if (lastTarget.IsVisible)
                {
                    predictedPos = Prediction.Position.PredictUnitPosition(lastTarget, 300).To3D();
                    lastSeen = Game.Time;
                }
                if (lastTarget.Distance(Player.Instance) > 700)
                {
                    lastTarget = null;
                }
            }
        }

        private static void Player_OnBasicAttack(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender != Player.Instance)
                return;
            if (args.Target is AIHeroClient)
                lastTarget = (AIHeroClient)args.Target;
            else
                lastTarget = null;
        }

        private static void OnDraw(EventArgs args)
        {
            // Draw range circles of our spells
            if(Settings._drawQ.CurrentValue && (SpellManager.Q.IsReady() || Settings.drawCDSpells))
                Circle.Draw(Color.Red, SpellManager.Q.MaximumRange, Player.Instance.Position);
            if (Settings._drawE.CurrentValue && (SpellManager.E.IsReady() || Settings.drawCDSpells))
                Circle.Draw(Color.DarkGreen, SpellManager.E.Range, Player.Instance.Position);
            if (Settings._drawR.CurrentValue && (SpellManager.R.IsReady() || Settings.drawCDSpells))
                Circle.Draw(Color.DarkOrange, SpellManager.R.Range, Player.Instance.Position);

        }
    }
}
