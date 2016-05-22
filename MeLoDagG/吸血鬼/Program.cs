using System;
using System.Drawing;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK.Rendering;
using VladimirTheTroll.Utility;
using Activator = VladimirTheTroll.Utility.Activator;

namespace VladimirTheTroll
{
    public static class Program
    {
        public static string Version = "Version 1.3 22/5/2016";
        public static AIHeroClient Target = null;
        public static int QOff = 0, WOff = 0, EOff = 0, ROff = 0;
        public static Spell.Targeted Q;
        public static Spell.Chargeable E;
        public static Spell.Active W;
        public static Spell.Skillshot R;
        public static bool Out = false;
        public static int CurrentSkin;

        public static readonly AIHeroClient Player = ObjectManager.Player;


        internal static void Main(string[] args)
        {
            Loading.OnLoadingComplete += OnLoadingComplete;
            Bootstrap.Init(null);
        }


        private static void OnLoadingComplete(EventArgs args)
        {
            if (Player.ChampionName != "Vladimir") return;
            Chat.Print(
                "<font color=\"#d80303\" >MeLoDag Presents </font><font color=\"#ffffff\" > Vladimir </font><font color=\"#d80303\" >Kappa Kippo</font>");
            VladimirTheTrollMeNu.LoadMenu();
            Game.OnTick += GameOnTick;
            Activator.LoadSpells();
            Game.OnUpdate += OnGameUpdate;

            #region Skill

            Q = new Spell.Targeted(SpellSlot.Q, 600);
            W = new Spell.Active(SpellSlot.W);
            E = new Spell.Chargeable(SpellSlot.E, 600, 600, 1250, 0, 1500, 70);
            R = new Spell.Skillshot(SpellSlot.R, 700, SkillShotType.Circular, 250, 1200, 150);

            #endregion

            Drawing.OnDraw += GameOnDraw;
            DamageIndicator.Initialize(SpellDamage.GetTotalDamage);
        }

        private static void GameOnDraw(EventArgs args)
        {
            if (VladimirTheTrollMeNu.Nodraw()) return;

            {
                if (VladimirTheTrollMeNu.DrawingsQ())
                {
                    new Circle {Color = Color.Red, Radius = Q.Range, BorderWidth = 2f}.Draw(Player.Position);
                }
                if (VladimirTheTrollMeNu.DrawingsW())
                {
                    new Circle {Color = Color.Red, Radius = W.Range, BorderWidth = 2f}.Draw(Player.Position);
                }
                if (VladimirTheTrollMeNu.DrawingsE())
                {
                    new Circle {Color = Color.Red, Radius = E.Range, BorderWidth = 2f}.Draw(Player.Position);
                }
                if (VladimirTheTrollMeNu.DrawingsR())
                {
                    new Circle {Color = Color.Red, Radius = R.Range, BorderWidth = 2f}.Draw(Player.Position);
                }
            }
        }

        private static
            void OnGameUpdate(EventArgs args)
        {
            if (Activator.Heal != null)
                Heal();
            if (Activator.Ignite != null)
                Ignite();
            if (VladimirTheTrollMeNu.CheckSkin())
            {
                if (VladimirTheTrollMeNu.SkinId() != CurrentSkin)
                {
                    Player.SetSkinId(VladimirTheTrollMeNu.SkinId());
                    CurrentSkin = VladimirTheTrollMeNu.SkinId();
                }
            }
        }
        

        private static void Ignite()
        {
            var autoIgnite = TargetSelector.GetTarget(Activator.Ignite.Range, DamageType.True);
            if (autoIgnite != null && autoIgnite.Health <= Player.GetSpellDamage(autoIgnite, Activator.Ignite.Slot) ||
                autoIgnite != null && autoIgnite.HealthPercent <= VladimirTheTrollMeNu.SpellsIgniteFocus())
                Activator.Ignite.Cast(autoIgnite);
        }

        private static void Heal()
        {
            if (Activator.Heal != null && Activator.Heal.IsReady() &&
                Player.HealthPercent <= VladimirTheTrollMeNu.SpellsHealHp()
                && Player.CountEnemiesInRange(600) > 0 && Activator.Heal.IsReady())
            {
                Activator.Heal.Cast();
            }
        }

        private static void GameOnTick(EventArgs args)
        {
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                OnCombo();
                UseE();
            }
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass))
            {
                OnHarrass();
            }
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear))
            {
                FarmQ();
                FarmQAlways();
            }
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear))
            {
                OnJungle();
            }
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.None))
            KillSteal();
            AutoPotions();
            AutoHarass();
            AutoHourglass();
        }


        private static
            void AutoHourglass()
        {
            var Zhonyas = VladimirTheTrollMeNu.Activator["Zhonyas"].Cast<CheckBox>().CurrentValue;
            var ZhonyasHp = VladimirTheTrollMeNu.Activator["ZhonyasHp"].Cast<Slider>().CurrentValue;

            if (Zhonyas && Player.HealthPercent <= ZhonyasHp && Activator.ZhonyaHourglass.IsReady())
            {
                Activator.ZhonyaHourglass.Cast();
                Chat.Print("<font color=\"#fffffff\" > Use Zhonyas <font>");
            }
        }

        private static
            void AutoPotions()
        {
            if (VladimirTheTrollMeNu.SpellsPotionsCheck() && !Player.IsInShopRange() &&
                Player.HealthPercent <= VladimirTheTrollMeNu.SpellsPotionsHp() &&
                !(Player.HasBuff("RegenerationPotion") || Player.HasBuff("ItemCrystalFlaskJungle") ||
                  Player.HasBuff("ItemMiniRegenPotion") || Player.HasBuff("ItemCrystalFlask") ||
                  Player.HasBuff("ItemDarkCrystalFlask")))
            {
                if (Activator.HuntersPot.IsReady() && Activator.HuntersPot.IsOwned())
                {
                    Activator.HuntersPot.Cast();
                }
                if (Activator.CorruptPot.IsReady() && Activator.CorruptPot.IsOwned())
                {
                    Activator.CorruptPot.Cast();
                }
                if (Activator.Biscuit.IsReady() && Activator.Biscuit.IsOwned())
                {
                    Activator.Biscuit.Cast();
                }
                if (Activator.HpPot.IsReady() && Activator.HpPot.IsOwned())
                {
                    Activator.HpPot.Cast();
                }
                if (Activator.RefillPot.IsReady() && Activator.RefillPot.IsOwned())
                {
                    Activator.RefillPot.Cast();
                }
            }
            if (VladimirTheTrollMeNu.SpellsPotionsCheck() && !Player.IsInShopRange() &&
                Player.ManaPercent <= VladimirTheTrollMeNu.SpellsPotionsM() &&
                !(Player.HasBuff("RegenerationPotion") || Player.HasBuff("ItemCrystalFlaskJungle") ||
                  Player.HasBuff("ItemMiniRegenPotion") || Player.HasBuff("ItemCrystalFlask") ||
                  Player.HasBuff("ItemDarkCrystalFlask")))
            {
                if (Activator.CorruptPot.IsReady() && Activator.CorruptPot.IsOwned())
                {
                    Activator.CorruptPot.Cast();
                }
            }
        }
        
        private static void KillSteal()
        {
            var ksQ = VladimirTheTrollMeNu.HarassMeNu["ksQ"].Cast<CheckBox>().CurrentValue;

            foreach (
                var enemy in
                    EntityManager.Heroes.Enemies.Where(
                        e => e.Distance(Player) <= Q.Range && e.IsValidTarget() && !e.IsInvulnerable))

            {
                if (ksQ && Q.IsReady() &&
                    SpellDamage.QDamage(enemy) >= enemy.Health &&
                    enemy.Distance(Player) <= Q.Range)
                {
                    Q.Cast(enemy);
                    Chat.Print("<font color=\"#fffffff\" > Use Q Free Kill<font>");
                }
            }
        }

      private static
            void FarmQ()
        {
            var useQ = VladimirTheTrollMeNu.FarmMeNu["qFarm"].Cast<CheckBox>().CurrentValue;
            var qminion =
                EntityManager.MinionsAndMonsters.GetLaneMinions(EntityManager.UnitTeam.Enemy, Player.Position, Q.Range)
                    .FirstOrDefault(
                        m =>
                            m.Distance(Player) <= Q.Range &&
                            m.Health <= SpellDamage.QDamage(m) - 20 &&
                            m.IsValidTarget());

            if (Q.IsReady() && useQ && qminion != null && !Orbwalker.IsAutoAttacking)
            {
                Q.Cast(qminion);
            }
        }

        private static void FarmQAlways()
        {
            var qFarmAlways = VladimirTheTrollMeNu.FarmMeNu["qFarmAlways"].Cast<CheckBox>().CurrentValue;
            var qminion =
                EntityManager.MinionsAndMonsters.GetLaneMinions(EntityManager.UnitTeam.Enemy, Player.Position, Q.Range)
                    .FirstOrDefault(
                        m =>
                            m.Distance(Player) <= Q.Range &&
                            m.IsValidTarget());

            if (Q.IsReady() && qFarmAlways && qminion != null && !Orbwalker.IsAutoAttacking)
            {
                Q.Cast(qminion);
            }
        }


        private static
            void OnJungle()
        {
            var useQJungle = VladimirTheTrollMeNu.FarmMeNu["useQJungle"].Cast<CheckBox>().CurrentValue;

            if (useQJungle)
            {
                var minion =
                    EntityManager.MinionsAndMonsters.GetJungleMonsters(Player.ServerPosition, 950f, true)
                        .FirstOrDefault();
                if (Q.IsReady() && useQJungle && minion != null)
                {
                    Q.Cast(minion);
                }
            }
        }

        private static
            void AutoHarass()
        {
            var target = TargetSelector.GetTarget(Q.Range, DamageType.Magical);

            if (target == null || !target.IsValidTarget()) return;

            Orbwalker.ForcedTarget = target;

            var AutoQharass = VladimirTheTrollMeNu.HarassMeNu["useQAuto"].Cast<CheckBox>().CurrentValue;

            {
                if (Q.IsReady() && AutoQharass)
                {
                    Q.Cast(target);
                }
            }
        }

        private static void OnHarrass()
        {
            var target = TargetSelector.GetTarget(Q.Range, DamageType.Physical);
            if (!target.IsValidTarget())
            {
                return;
            }

            var useQh = VladimirTheTrollMeNu.HarassMeNu["useQHarass"].Cast<CheckBox>().CurrentValue;

            {
                if (Q.IsReady() && useQh)
                {
                    Q.Cast(target);
                }
            }
        }

        private static
            void UseE()
        {
            var target = TargetSelector.GetTarget(E.Range, DamageType.Magical);

            if (target == null || !target.IsValidTarget()) return;

            var useE = VladimirTheTrollMeNu.ComboMenu["useECombo"].Cast<CheckBox>().CurrentValue;
            {
                if (useE && E.IsReady() && target.Distance(Player) <= 420)
                {
                    if (E.IsCharging)
                    {
                        E.Cast(Game.CursorPos);
                    }
                    E.StartCharging();
                }
            }
        }


        private static
            void OnCombo()
        {
            var enemies = EntityManager.Heroes.Enemies.OrderByDescending
                (a => a.HealthPercent).Where(a => !a.IsMe && a.IsValidTarget() && a.Distance(Player) <= Q.Range);
            var target = TargetSelector.GetTarget(1400, DamageType.Physical);
            if (!target.IsValidTarget(Q.Range) || target == null)
            {
                return;
            }
            var useWcostumHp = VladimirTheTrollMeNu.ComboMenu["useWcostumHP"].Cast<Slider>().CurrentValue;
            var useQ = VladimirTheTrollMeNu.ComboMenu["useQCombo"].Cast<CheckBox>().CurrentValue;
            var useW = VladimirTheTrollMeNu.ComboMenu["useWCombo"].Cast<CheckBox>().CurrentValue;
            var useR = VladimirTheTrollMeNu.ComboMenu["useRCombo"].Cast<CheckBox>().CurrentValue;
            var rCount = VladimirTheTrollMeNu.ComboMenu["Rcount"].Cast<Slider>().CurrentValue;
            {
                if (Q.IsReady() && useQ)
                {
                    Q.Cast(target);
                }

                if (W.IsReady() && useW && Player.HealthPercent <= useWcostumHp)
                {
                    W.Cast();
                }
                if (R.IsReady() && Player.CountEnemiesInRange(R.Range) >= rCount && useR)
                {
                    R.Cast(target);
                }
            }
            
        }
    }
}