using System;
using System.Linq;
using System.Net;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Rendering;
using SharpDX;
using SettingsMisc = Tristana.Config.MiscMenu;
using SettingsMana = Tristana.Config.ManaManagerMenu;
using SettingsModes = Tristana.Config.ModesMenu;
using SettingsDrawing = Tristana.Config.DrawingMenu;

namespace Tristana
{
    public static class Events
    {

        static Item Youmuu;
        private static float PlayerMana
        {
            get { return Player.Instance.ManaPercent; }
        }

        static Events()
        {
            Youmuu = new Item(ItemId.Youmuus_Ghostblade);
            Interrupter.OnInterruptableSpell += InterrupterOnInterruptableSpell;
            Gapcloser.OnGapcloser += GapcloserOnGapcloser;
            Orbwalker.OnPreAttack += OrbwalkerOnPreAttack;
            Orbwalker.OnAttack += OrbwalkerOnAttack;
            Drawing.OnDraw += OnDraw;
        }

        private static void OrbwalkerOnAttack(AttackableUnit target, EventArgs args)
        {
            if (SettingsModes.Combo.UseItems && Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo) && CanUseItem(ItemId.Youmuus_Ghostblade))
            {
                Youmuu.Cast();
            }
            // No sense in checking if Q is off cooldown
            if (!SpellManager.Q.IsReady())
            {
                return;
            }
            // Check if we should use Q to attack heroes
            if ((SettingsModes.Combo.UseQ && Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo)) ||
                (SettingsModes.Harass.UseQ && Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass)) ||
                (Orbwalker.LaneClearAttackChamps && SettingsModes.LaneClear.UseQ &&
                 Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear)))
            {
                if (target is AIHeroClient && PlayerMana >= SettingsMana.MinQMana)
                {
                    SpellManager.Q.Cast();
                    Debug.WriteChat("Casting Q, because attacking enemy hero in Combo or Harras or LaneClear.");
                    return;
                }
            }
            // Check if we should use Q to attack minions/monsters/turrets
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear) ||
                Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear))
            {
                if (target is Obj_AI_Minion && PlayerMana >= SettingsMana.MinQMana)
                {
                    if (SettingsModes.JungleClear.UseQ && target.Team == GameObjectTeam.Neutral)
                    {
                        SpellManager.Q.Cast();
                        Debug.WriteChat("Casting Q, because attacking monster in JungleClear");
                    }
                    else if (SettingsModes.LaneClear.UseQ && target.IsEnemy)
                    {
                        SpellManager.Q.Cast();
                        Debug.WriteChat("Casting Q, because attacking minion in LaneClear");
                    }
                }

            }
        }

        private static void OrbwalkerOnPreAttack(AttackableUnit target, Orbwalker.PreAttackArgs args)
        {
            // No sense in checking if E is off cooldown
            if (!SpellManager.E.IsReady())
            {
                return;
            }
            // Check if we should use E to attack heroes
            if ((SettingsModes.Combo.UseE && Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo)) ||
                (SettingsModes.Harass.UseE && Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass)) ||
                (Orbwalker.LaneClearAttackChamps && SettingsModes.LaneClear.UseE &&
                 Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear)))
            {
                if (target is AIHeroClient && PlayerMana >= SettingsMana.MinQMana)
                {
                    SpellManager.E.Cast((Obj_AI_Base)target);
                    Debug.WriteChat("Casting E, because attacking enemy hero in Combo or Harras or LaneClear.");
                    return;
                }
            }
            // Check if we should use E to attack minions/monsters/turrets
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear) ||
                Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear))
            {
                if (target is Obj_AI_Minion && PlayerMana >= SettingsMana.MinQMana)
                {
                    if (SettingsModes.JungleClear.UseE && target.Team == GameObjectTeam.Neutral)
                    {
                        var ETargets = EntityManager.MinionsAndMonsters.GetJungleMonsters(target.Position, 250.0f).Count();
                        if (ETargets >= SettingsModes.JungleClear.MinETargets)
                        {
                            SpellManager.E.Cast((Obj_AI_Base)target);
                            Debug.WriteChat("Casting E, because attacking monsters in LaneClear");
                        }
                    }
                    else if (SettingsModes.LaneClear.UseE && target.IsEnemy)
                    {
                        var ETargets = EntityManager.MinionsAndMonsters.GetLaneMinions(EntityManager.UnitTeam.Enemy, target.Position, 250.0f).Count();
                        if (ETargets >= SettingsModes.LaneClear.MinETargets)
                        {
                            SpellManager.E.Cast((Obj_AI_Base)target);
                            Debug.WriteChat("Casting E, because attacking minions in LaneClear");
                        }
                    }
                }

            }
        }

        private static void GapcloserOnGapcloser(AIHeroClient sender, Gapcloser.GapcloserEventArgs gapcloserEventArgs)
        {
            if (SettingsMisc.GapcloserR && sender.IsEnemy && sender.IsValidTarget() &&
                SpellManager.R.IsReady() && gapcloserEventArgs.End.Distance(Player.Instance.Position) < 200)
            {
                SpellManager.R.Cast(sender);
                Chat.Print("Casting R in Antigapcloser on {0}", ((AIHeroClient)sender).ChampionName);
            }
        }

        private static void InterrupterOnInterruptableSpell(Obj_AI_Base sender, Interrupter.InterruptableSpellEventArgs interruptableSpellEventArgs)
        {
            if (SettingsMisc.InterruptR && sender.IsEnemy && sender.IsValidTarget(SpellManager.ERRange()) &&
                SpellManager.R.IsReady())
            {
                SpellManager.R.Cast(sender);
                Chat.Print("Interrupting spell from {0}", ((AIHeroClient)sender).ChampionName);
            }
        }

        public static void Initialize()
        {

        }

        private static void OnDraw(EventArgs args)
        {
            var ERRange = SpellManager.ERRange();
            if (SettingsDrawing.DrawW)
            {
                if (!(SettingsDrawing.DrawOnlyReady && !SpellManager.W.IsReady()))
                {
                    Circle.Draw(Color.LightBlue, SpellManager.W.Range, Player.Instance.Position);
                }
            }
            if (SettingsDrawing.DrawE)
            {
                if (!(SettingsDrawing.DrawOnlyReady && !SpellManager.E.IsReady()))
                {
                    Circle.Draw(Color.Orange, ERRange, Player.Instance.Position);
                }
            }
            if (SettingsDrawing.DrawR)
            {
                if (!(SettingsDrawing.DrawOnlyReady && !SpellManager.R.IsReady()))
                {
                    Circle.Draw(Color.OrangeRed, ERRange, Player.Instance.Position);
                }
            }
            if (SettingsDrawing.DrawIgnite && SpellManager.HasIgnite())
            {
                if (!(SettingsDrawing.DrawOnlyReady && !SpellManager.Ignite.IsReady()))
                {
                    Circle.Draw(Color.Red, SpellManager.Ignite.Range, Player.Instance.Position);
                }
            }
        }

        private static bool CanUseItem(ItemId id)
        {
            return Item.HasItem(id) && Item.CanUseItem(id);
        }
    }
}
