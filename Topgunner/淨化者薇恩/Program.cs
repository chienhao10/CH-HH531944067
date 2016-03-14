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
using Settings = PurifierVayne.Config.Misc;
using PurifierVayne.Modes;
namespace PurifierVayne
{
    public static class Program
    {
        // Change this line to the champion you want to make the addon for,
        // watch out for the case being correct!
        public const string ChampName = "Vayne";
        public static AIHeroClient lastTarget;
        public static float lastSeen = Game.Time;
        public static Vector3 predictedPos;

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
            Player.OnLevelUp += PurifierVayne.Modes.PermaActive.autoLevelSkills;
            Gapcloser.OnGapcloser += Gapcloser_OnGapcloser;
            Player.OnBasicAttack += Player_OnBasicAttack;
            Game.OnTick += Game_OnTick;
            Orbwalker.OnPostAttack += Modes.Combo.Spellblade;
            Orbwalker.OnPostAttack += Modes.LaneClear.Spellblade;
            Orbwalker.OnPostAttack += Modes.JungleClear.Spellblade;
            Orbwalker.OnPostAttack += Modes.Harass.Spellblade;
            Orbwalker.OnPostAttack += CondemnAfterNextAA;
            Orbwalker.OnUnkillableMinion += Modes.LastHit.Unkillable;
            Interrupter.OnInterruptableSpell += Interrupter_OnInterruptableSpell;
            Obj_AI_Base.OnProcessSpellCast += SpellManager.OnProcessSpellCast;
            Evade.Evade.Initialize();
            
        }

        private static void CondemnAfterNextAA(AttackableUnit target, EventArgs args)
        {
            if (Config.ESettings.condemnAfterNextAA)
            {
                if (target is AIHeroClient)
                {
                    SpellManager.E.Cast((AIHeroClient)target);
                }
            }
        }

        private static void Interrupter_OnInterruptableSpell(Obj_AI_Base sender, Interrupter.InterruptableSpellEventArgs e)
        {
            if (sender.IsEnemy && SpellManager.E.IsReady() && e.DangerLevel >= DangerLevel.High && sender.HealthPercent > 0 && Config.ESettings.interruptE && sender.IsInRange(Player.Instance, SpellManager.E.Range))
            {
                SpellManager.E.Cast(sender);
            }
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

        private static void Gapcloser_OnGapcloser(AIHeroClient sender, Gapcloser.GapcloserEventArgs e)
        {
            if (Settings.useQOnGapcloser && sender.IsEnemy && e.End.Distance(Player.Instance) < 300)
            {
                Vector3 castTo = e.End + 5 * (Player.Instance.Position- e.End);
                Player.CastSpell(SpellSlot.Q, castTo);
            }
            else if (Config.ESettings.useEOnGapcloser && sender.IsEnemy && e.End.Distance(Player.Instance) < 300)
            {
                SpellManager.E.Cast(sender);
            }
        }

        private static void OnDraw(EventArgs args)
        {
            // Draw range circles of our spells
            if(Settings._drawQ.CurrentValue && (SpellManager.Q.IsReady() || !Settings.drawReady))
                Circle.Draw(Color.Red, SpellManager.Q.Range, Player.Instance.Position);
            if (Settings._drawE.CurrentValue && (SpellManager.E.IsReady() || !Settings.drawReady))
                Circle.Draw(Color.DarkGreen, SpellManager.E.Range, Player.Instance.Position);

        }
    }
}
