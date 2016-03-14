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
using Settings = MissFortune.Config.Misc;
using MissFortune.Modes;
namespace MissFortune
{
    public static class Program
    {
        // Change this line to the champion you want to make the addon for,
        // watch out for the case being correct!
        public const string ChampName = "MissFortune";

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
            Player.OnLevelUp += MissFortune.Modes.PermaActive.autoLevelSkills;
            Obj_AI_Base.OnBuffLose += Obj_AI_Base_OnBuffLose;
            Obj_AI_Base.OnBuffGain += Obj_AI_Base_OnBuffGain;
            Orbwalker.OnPostAttack += Orbwalker_OnPostAttack;
            Orbwalker.OnUnkillableMinion += UnkillableMinion;
            Game.OnUpdate += Combo.Update;
            
        }

        private static void UnkillableMinion(Obj_AI_Base target, Orbwalker.UnkillableMinionArgs args)
        {
            if (Config.Modes.Harass.Mana >= Player.Instance.ManaPercent)
                return;
            if (Settings.useQFarm && Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LastHit) || Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass))
            {
                if (SpellManager.Q.IsReady() && target.IsInRange(Player.Instance, SpellManager.Q.Range))
                {
                    SpellManager.Q.Cast(target);
                }
            }
        }

        private static void Orbwalker_OnPostAttack(AttackableUnit target, EventArgs args)
        {
            if (!Settings.useLoveTaps || !(Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo) || Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass)))
                return;
            bool newtarget = false;
            foreach (var e in EntityManager.Heroes.Enemies.Where(t => !t.IsDead && t.IsTargetable && !t.IsZombie && !t.IsInvulnerable && Player.Instance.IsInRange(t, 500)).OrderBy(t => t.MaxHealth))
            {
                if (e.NetworkId == target.NetworkId)
                    continue;
                Orbwalker.ForcedTarget = e;
                newtarget = true;
                break;
            }
            if (!newtarget)
            {
                Orbwalker.ForcedTarget = null;
            }
        }

        private static void Obj_AI_Base_OnBuffGain(Obj_AI_Base sender, Obj_AI_BaseBuffGainEventArgs args)
        {
            if (sender.IsMe && args.Buff.DisplayName == "MissFortuneBulletSound")
            {
                Combo.Rchanneling = true;
                Orbwalker.DisableAttacking = true;
                Orbwalker.DisableMovement = true;
                Combo.RcameOut = true;
            }
        }

        private static void Obj_AI_Base_OnBuffLose(Obj_AI_Base sender, Obj_AI_BaseBuffLoseEventArgs args)
        {
            if (sender.IsMe && args.Buff.DisplayName == "MissFortuneBulletSound")
            {
                Combo.Rchanneling = false;
                Orbwalker.DisableAttacking = false;
                Orbwalker.DisableMovement = false;
                Combo.RcameOut = false;
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
            if (Settings._drawR.CurrentValue)
                Circle.Draw(Color.DarkOrange, SpellManager.R.Range, Player.Instance.Position);

        }
    }
}
