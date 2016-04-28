#region License
/* Copyright (c) LeagueSharp 2016
 * No reproduction is allowed in any way unless given written consent
 * from the LeagueSharp staff.
 * 
 * Author: imsosharp
 * Date: 2/24/2016
 * File: KogMaw.cs
 */
#endregion License

using System;
using System.Collections.Generic;
using System.Linq;
using Challenger_Series.Utils;
using LeagueSharp;
using LeagueSharp.SDK;
using SharpDX;
using Color = System.Drawing.Color;
using LeagueSharp.Data.Enumerations;
using EloBuddy;
using EloBuddy.SDK;
using LeagueSharp.SDK.Core.Utils;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;

namespace Challenger_Series.Plugins
{
    public class Humanizer
    {
        public Humanizer(int lifespan)
        {
            ExpireTime = Variables.TickCount + lifespan;
        }
        private int ExpireTime;

        public bool ShouldDestroy
        {
            get
            {
                return !ObjectManager.Player.HasBuff("KogMawBioArcaneBarrage") || Variables.TickCount > ExpireTime;
            }
        }
    }

    public class KogMaw : CSPlugin
    {
        #region Spells
        public LeagueSharp.SDK.Spell Q { get; set; }
        public LeagueSharp.SDK.Spell Q2 { get; set; }
        public LeagueSharp.SDK.Spell W { get; set; }
        public LeagueSharp.SDK.Spell W2 { get; set; }
        public LeagueSharp.SDK.Spell E { get; set; }
        public LeagueSharp.SDK.Spell E2 { get; set; }
        public LeagueSharp.SDK.Spell R { get; set; }
        public LeagueSharp.SDK.Spell R2 { get; set; }
        #endregion Spells

        public KogMaw()
        {
            Q = new LeagueSharp.SDK.Spell(SpellSlot.Q, 1175);
            Q.SetSkillshot(0.25f, 70f, 1650f, true, SkillshotType.SkillshotLine);
            W = new LeagueSharp.SDK.Spell(SpellSlot.W, 630);
            E = new LeagueSharp.SDK.Spell(SpellSlot.E, 1250);
            E.SetSkillshot(0.25f, 120f, 1400f, false, SkillshotType.SkillshotLine);
            R = new LeagueSharp.SDK.Spell(SpellSlot.R, 1200);
            R.SetSkillshot(1.2f, 75f, 12000f, false, SkillshotType.SkillshotCircle);
            InitializeMenu();
            DelayedOnUpdate += OnUpdate;
            Drawing.OnDraw += OnDraw;
            Orbwalker.OnPostAttack += OnAction;
            AIHeroClient.OnSpellCast += OnDoCast;
            _rand = new Random();
        }

        private void OnDoCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (args.SData.Name.Contains("BarrageAttack"))
            {
                _attacksSoFar++;
            }
        }

        private Humanizer _humanizer;
        private int _attacksSoFar;
        private Random _rand;

        private void OnAction(AttackableUnit asd, EventArgs args)
        {
            if (asd is AIHeroClient)
            {
                var target = asd as AIHeroClient;
                var distFromTargetToMe = target.Distance(ObjectManager.Player.ServerPosition);
                if (Q.IsReady())
                {
                    QLogic(target);
                }
                if (distFromTargetToMe < 350 && target.IsMelee)
                {
                    ELogic(target);
                }
            }
            if (asd is Obj_AI_Minion)
            {
                if (GetJungleCampsOnCurrentMap() != null && (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear) || Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear)))
                {
                    var targetName = (asd as Obj_AI_Minion).CharData.BaseSkinName;

                    if (!targetName.Contains("Mini") && GetJungleCampsOnCurrentMap().Contains(targetName) && JungleclearMenu[targetName].Cast<CheckBox>().CurrentValue)
                    {
                        W.Cast();
                    }
                }
            }
        }

        #region Events

        public override void OnUpdate(EventArgs args)
        {
            base.OnUpdate(args);
            if (getCheckBoxItem(ComboMenu, "koggieusew"))
            {
                WLogic();
            }
            if (getCheckBoxItem(ComboMenu, "koggieuser"))
            {
                RLogic();
            }
            if (Q.IsReady() && getCheckBoxItem(ComboMenu, "koggieuseq") && Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo) && ObjectManager.Player.Mana > GetQMana() + GetWMana())
            {
                foreach (var enemy in ValidTargets.Where(t => t.Distance(ObjectManager.Player) < 800).OrderBy(e => e.Distance(ObjectManager.Player)))
                {
                    var prediction = Q.GetPrediction(enemy);
                    if ((int)prediction.Hitchance >= (int)HitChance.VeryHigh)
                    {
                        Q.Cast(prediction.UnitPosition);
                    }
                }
            }
            var attackrange = GetAttackRangeAfterWIsApplied();
            var target = TargetSelector.GetTarget(attackrange, DamageType.Physical);
            if (IsWActive() && target != null && target.Distance(ObjectManager.Player) > attackrange - 150)
            {
                E.CastIfHitchanceMinimum(target, HitChance.Medium);
            }

            #region Humanizer
            if (_humanizer != null)
            {
                _attacksSoFar = 0;
            }
            else if (_attacksSoFar >= getSliderItem(HumanizerMenu, "koggieminattacks"))
            {
                _humanizer = new Humanizer(getSliderItem(HumanizerMenu, "koggiehumanizermovetime"));
            }
            if (!IsWActive())
            {
                _humanizer = null;
                _attacksSoFar = 0;
            }
            if (_humanizer != null && _humanizer.ShouldDestroy)
            {
                _humanizer = null;
            }

            Orbwalker.DisableAttacking = !CanMove();
            Orbwalker.DisableMovement = !CanAttack();
            #endregion Humanizer
        }

        public override void OnDraw(EventArgs args)
        {
            base.OnDraw(args);
            W.Range = GetAttackRangeAfterWIsApplied();
            R.Range = GetRRange();
            if (getCheckBoxItem(DrawMenu, "koggiedraww"))
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, GetAttackRangeAfterWIsApplied(), W.IsReady() || IsWActive() ? Color.LimeGreen : Color.Red);
            }
            if (getCheckBoxItem(DrawMenu, "koggiedrawr"))
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, GetRRange() + 25, R.IsReady() ? Color.LimeGreen : Color.Red);
            }
        }

        private bool CanAttack()
        {
            if (!getCheckBoxItem(HumanizerMenu, "koggiehumanizerenabled")) return true;
            if (IsWActive())
            {
                return _humanizer == null;
            }
            return true;
        }
        private bool CanMove()
        {
            if (!getCheckBoxItem(HumanizerMenu, "koggiehumanizerenabled")) return true;
            if (IsWActive() && ObjectManager.Player.AttackSpeedMod / 2 > _rand.Next(167, 230) / 100)
            {
                if ((Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo) && ObjectManager.Player.CountEnemyHeroesInRange(GetAttackRangeAfterWIsApplied() - 25) < 1) || (!Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.None) && !Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo) && (!GameObjects.EnemyMinions.Any(m => m.IsHPBarRendered && m.Distance(ObjectManager.Player) < GetAttackRangeAfterWIsApplied() - 25) && !GameObjects.Jungle.Any(m => m.IsHPBarRendered && m.Distance(ObjectManager.Player) < GetAttackRangeAfterWIsApplied() - 25))))
                {
                    return true;
                }
                return _humanizer != null;
            }
            return true;
        }

        #endregion Events

        private Menu ComboMenu;
        private Menu HarassMenu;
        private Menu JungleclearMenu;
        private Menu DrawMenu;
        private Menu HumanizerMenu;

        public override void InitializeMenu()
        {
            base.InitializeMenu();

            ComboMenu = MainMenu.AddSubMenu("Combo Settings: ", "koggiecombomenu");
            ComboMenu.Add("koggieuseq", new CheckBox("Use Q", true));
            ComboMenu.Add("koggieusew", new CheckBox("Use W", true));
            ComboMenu.Add("koggieusee", new CheckBox("Use E", true));
            ComboMenu.Add("koggieuser", new CheckBox("Use R", true));
            ComboMenu.Add("koggiewintime", new CheckBox("Dont Activate W if In Danger!", false));

            HarassMenu = MainMenu.AddSubMenu("Harass Settings", "koggieharassmenu");
            HarassMenu.Add("koggieuserharass", new CheckBox("Use R", true));

            JungleclearMenu = MainMenu.AddSubMenu("Jungleclear Settings: ", "koggiejgclearmenu");
            JungleclearMenu.AddGroupLabel("W if TARGET is: ");
            if (GetJungleCampsOnCurrentMap() != null)
            {
                foreach (var mob in GetJungleCampsOnCurrentMap())
                {
                    JungleclearMenu.Add(mob, new CheckBox(mob, true));
                }
            }

            DrawMenu = MainMenu.AddSubMenu("Drawing Settings", "koggiedrawmenu");
            DrawMenu.Add("koggiedraww", new CheckBox("Draw W Range", true));
            DrawMenu.Add("koggiedrawr", new CheckBox("Draw R Range", true));

            HumanizerMenu = MainMenu.AddSubMenu("Humanizer Settings: ", "koggiehumanizermenu");
            HumanizerMenu.Add("koggieminattacks", new Slider("Min attacks before moving", 2, 1, 10));
            HumanizerMenu.Add("koggiehumanizermovetime", new Slider("Time for moving (milliseconds)", 200, 0, 1000));
            HumanizerMenu.Add("koggiehumanizerenabled", new CheckBox("Enable Humanizer? ", true));

            MainMenu.Add("koggiermaxstacks", new Slider("R Max Stacks: ", 2, 0, 11));
            MainMenu.Add("koggiesavewmana", new CheckBox("Always Save Mana For W!", true));
        }

        #region ChampionLogic

        private void QLogic(AIHeroClient target)
        {
            if (!getCheckBoxItem(ComboMenu, "koggieuseq") || !Q.IsReady() || !Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo)) return;
            if (getCheckBoxItem(MainMenu, "koggiesavewmana") && ObjectManager.Player.Mana < GetQMana() + GetWMana()) return;
            var prediction = Q.GetPrediction(target);
            if (target.LSIsValidTarget() && (int)prediction.Hitchance > (int)HitChance.Medium)
            {
                Q.Cast(prediction.UnitPosition);
            }
        }
        private void WLogic()
        {
            if (W.IsReady() && !IsWActive() && ValidTargets.Any(h => h.Health > 1 && h.Distance(ObjectManager.Player.ServerPosition) < GetAttackRangeAfterWIsApplied() && h.LSIsValidTarget()) && Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                W.Cast();
            }
        }

        private void ELogic(AIHeroClient target)
        {
            if (!getCheckBoxItem(ComboMenu, "koggieusee") || !E.IsReady() || !Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo)) return;
            if (getCheckBoxItem(MainMenu, "koggiesavewmana") && ObjectManager.Player.Mana < GetEMana() + GetQMana()) return;
            var prediction = E.GetPrediction(target);
            if (target.LSIsValidTarget() && (int)prediction.Hitchance >= (int)HitChance.Medium)
            {
                E.Cast(prediction.UnitPosition);
            }
        }

        private void RLogic()
        {
            if (!getCheckBoxItem(ComboMenu, "koggieuser") || !R.IsReady() || ObjectManager.Player.IsRecalling() || Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.None)) return;
            if (getCheckBoxItem(MainMenu, "koggiesavewmana") && ObjectManager.Player.Mana < GetRMana() + GetWMana()) return;
            var myPos = ObjectManager.Player.ServerPosition;
            foreach (
                var enemy in
                    ValidTargets.Where(h => h.Distance(myPos) < R.Range && (!IsWActive() || h.Distance(myPos) > W.Range + 85) && h.HealthPercent < 25 && h.LSIsValidTarget()))
            {
                var prediction = R.GetPrediction(enemy, true);
                if ((int)prediction.Hitchance > (int)HitChance.Medium)
                {
                    R.Cast(prediction.UnitPosition);
                }
            }
            if (GetRStacks() >= getSliderItem(MainMenu, "koggiermaxstacks")) return;
            if (IsWActive() || (!Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo) && !getCheckBoxItem(HarassMenu, "koggieuserharass"))) return;

            foreach (var enemy in ValidTargets.Where(h => h.Distance(myPos) < R.Range && h.LSIsValidTarget() && h.HealthPercent < 35))
            {
                var dist = enemy.Distance(ObjectManager.Player.ServerPosition);
                if (Orbwalker.CanAutoAttack && dist < 550) break;
                var prediction = R.GetPrediction(enemy, true);
                if ((int)prediction.Hitchance > (int)HitChance.Medium)
                {
                    R.Cast(prediction.UnitPosition);
                }
            }
        }

        private float GetAttackRangeAfterWIsApplied()
        {
            return W.Level > 0 ? new[] { 630, 660, 690, 720, 750 }[W.Level - 1] : 540;
        }

        private float GetRRange()
        {
            return R.Level > 0 ? new[] { 1200, 1500, 1800 }[R.Level - 1] : 1200;
        }

        private float GetQMana()
        {
            return 60;
        }

        private float GetWMana()
        {
            return 40;
        }

        private float GetEMana()
        {
            return E.Level > 0 ? new[] { 80, 90, 100, 110, 120 }[E.Level - 1] : 80;
        }

        private float GetRMana()
        {
            return new[] { 50, 100, 150, 200, 250, 300, 350, 400, 450, 500, 500 }[GetRStacks()];
        }

        private int GetRStacks()
        {
            return ObjectManager.Player.HasBuff("kogmawlivingartillerycost") ? ObjectManager.Player.GetBuff("kogmawlivingartillerycost").Count : 0;
        }

        private bool IsWActive()
        {
            return ObjectManager.Player.HasBuff("KogMawBioArcaneBarrage");
        }
        private List<string> GetJungleCampsOnCurrentMap()
        {
            switch ((int)Game.MapId)
            {
                //Summoner's Rift
                case 11:
                    {
                        return SRMobs;
                    }
                //Twisted Treeline
                case 10:
                    {
                        return TTMobs;
                    }
            }
            return null;
        }

        /// <summary>
        /// Summoner's Rift Jungle "Big" Mobs
        /// </summary>
        private List<string> SRMobs = new List<string>
        {
            "SRU_Baron",
            "SRU_Blue",
            "Sru_Crab",
            "SRU_Dragon",
            "SRU_Gromp",
            "SRU_Krug",
            "SRU_Murkwolf",
            "SRU_Razorbeak",
            "SRU_Red",
        };

        /// <summary>
        /// Twisted Treeline Jungle "Big" Mobs
        /// </summary>
        private List<string> TTMobs = new List<string>
        {
            "TT_NWraith",
            "TT_NGolem",
            "TT_NWolf",
            "TT_Spiderboss"
        };
        #endregion ChampionLogic
    }
}
