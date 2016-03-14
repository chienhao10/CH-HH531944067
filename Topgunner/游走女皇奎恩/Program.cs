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
using Settings = RoamQueenQuinn.Config.Misc;
using RoamQueenQuinn.Modes;
namespace RoamQueenQuinn
{
    public static class Program
    {
        // Change this line to the champion you want to make the addon for,
        // watch out for the case being correct!
        public const string ChampName = "Quinn";
        public static int counter = 0;

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
            Player.OnLevelUp += RoamQueenQuinn.Modes.PermaActive.autoLevelSkills;
            Game.OnTick += HarrierForceTarget;
            Interrupter.OnInterruptableSpell += Interrupter_OnInterruptableSpell;
        }

        private static void Interrupter_OnInterruptableSpell(Obj_AI_Base sender, Interrupter.InterruptableSpellEventArgs e)
        {
            if (Settings.interruptE && !sender.IsDead && !sender.IsInvulnerable && !sender.IsZombie && sender.IsEnemy && e.DangerLevel >= DangerLevel.Medium && sender.IsInRange(Player.Instance, SpellManager.E.Range) && SpellManager.E.IsReady())
            {
                SpellManager.E.Cast(sender);
            }
        }

        private static void HarrierForceTarget(EventArgs args)
        {
            bool newtarget = false;
            if (Settings.useHarrier && (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo) || Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass)))
            {
                foreach (var e in EntityManager.Heroes.Enemies.Where(t => !t.IsDead && t.IsTargetable && !t.IsZombie && !t.IsInvulnerable && Player.Instance.IsInRange(t, 525)).OrderBy(t => t.MaxHealth))
                {
                    if (e.HasBuff("quinnw"))
                    {
                        Orbwalker.ForcedTarget = e;
                        newtarget = true;

                        break;
                    }
                }
            }
            else if (Settings.useHarrier)
            {
                foreach (var e in EntityManager.MinionsAndMonsters.CombinedAttackable)
                {
                    if (e.HasBuff("quinnw"))
                    {
                        Orbwalker.ForcedTarget = e;
                        newtarget = true;
                        break;
                    }
                }
            }
            if (!newtarget)
            {
                Orbwalker.ForcedTarget = null;
                //Orbwalker.ResetAutoAttack();
            }
        }

        private static void OnDraw(EventArgs args)
        {
            // Draw range circles of our spells
            if(Settings._drawQ.CurrentValue)
                Circle.Draw(Color.Red, SpellManager.Q.Range, Player.Instance.Position);
            if (Settings._drawW.CurrentValue)
                Circle.Draw(Color.Aqua, SpellManager.W.Range, Player.Instance.Position);
            if (Settings._drawE.CurrentValue)
                Circle.Draw(Color.DarkGreen, SpellManager.E.Range, Player.Instance.Position);

        }
    }
}
