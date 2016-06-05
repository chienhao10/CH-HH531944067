using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu.Values;
using LeagueSharp.Common;
using Prediction = LeagueSharp.Common.Prediction;
using Utility = LeagueSharp.Common.Utility;

namespace ElRengarRevamped
{
    public enum Spells
    {
        Q,

        W,

        E,

        R
    }

    internal class Rengar : Standards
    {
        #region Properties

        private static IEnumerable<AIHeroClient> Enemies
        {
            get { return HeroManager.Enemies; }
        }

        #endregion

        #region Static Fields

        public static Obj_AI_Base SelectedEnemy;

        #endregion

        #region Public Methods and Operators

        public static void OnClick(WndEventArgs args)
        {
            if (args.Msg != (uint)WindowsMessages.WM_LBUTTONDOWN)
            {
                return;
            }
            var unit2 =
                ObjectManager.Get<Obj_AI_Base>()
                    .FirstOrDefault(
                        a => (a.IsValid<AIHeroClient>()) && a.IsEnemy && a.Distance(Game.CursorPos) < a.BoundingRadius + 1000 && a.IsValidTarget());
            if (unit2 != null)
            {
                SelectedEnemy = unit2;
            }
        }

        public static void OnLoad()
        {
            if (Player.ChampionName != "Rengar")
            {
                return;
            }

            try
            {
                Youmuu = new Items.Item(3142);

                Ignite = Player.GetSpellSlot("summonerdot");
                spells[Spells.E].SetSkillshot(0.25f, 70f, 1500f, true, SkillshotType.SkillshotLine);

                MenuInit.Initialize();
                Game.OnUpdate += OnUpdate;
                Drawing.OnDraw += OnDraw;
                CustomEvents.Unit.OnDash += OnDash;
                Drawing.OnEndScene += OnDrawEndScene;
                Obj_AI_Base.OnProcessSpellCast += OnProcessSpellCast;
                Orbwalker.OnPostAttack += AfterAttack;
                Orbwalker.OnPreAttack += BeforeAttack;
                Game.OnWndProc += OnClick;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        #endregion

        #region Methods

        private static void AfterAttack(AttackableUnit target, EventArgs args)
        {
            try
            {
                var enemy = target as Obj_AI_Base;
                if (enemy == null || !(target is AIHeroClient))
                {
                    return;
                }

                if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo) ||
                    Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass))
                {
                    if (Player.LSCountEnemiesInRange(Player.AttackRange + Player.BoundingRadius + 100) != 0)
                    {
                        spells[Spells.Q].Cast();
                        ActiveModes.CastItems(enemy);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private static void BeforeAttack(AttackableUnit target, Orbwalker.PreAttackArgs args)
        {
            try
            {
                if (!MenuInit.getCheckBoxItem(MenuInit.comboMenu, "Combo.Use.QQ"))
                {
                    return;
                }
                if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo) && !HasPassive &&
                spells[Spells.Q].IsReady() &&
                !(MenuInit.getBoxItem(MenuInit.comboMenu, "Combo.Prio") == 0 ||
                  MenuInit.getBoxItem(MenuInit.comboMenu, "Combo.Prio") == 1 && Ferocity == 5))
                {
                    var x = Prediction.GetPrediction(args.Target as Obj_AI_Base, Player.AttackCastDelay * 1000);
                    if (Player.Position.LSTo2D().LSDistance(x.UnitPosition.LSTo2D())
                        >= Player.BoundingRadius + Player.AttackRange + args.Target.BoundingRadius)
                    {
                        args.Process = false;
                        spells[Spells.Q].Cast();
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private static void Heal()
        {
            try
            {
                if (RengarR || Player.LSIsRecalling() || Player.InFountain() || Ferocity != 5)
                {
                    return;
                }

                if (Player.LSCountEnemiesInRange(1000) > 1 && spells[Spells.W].IsReady())
                {
                    if (MenuInit.getCheckBoxItem(MenuInit.healMenu, "Heal.AutoHeal")
                        && (Player.Health / Player.MaxHealth) * 100
                        <= MenuInit.getSliderItem(MenuInit.healMenu, "Heal.HP"))
                    {
                        spells[Spells.W].Cast();
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private static void KillstealHandler()
        {
            try
            {
                if (!MenuInit.getCheckBoxItem(MenuInit.ksMenu, "Killsteal.On") || RengarR || Player.LSIsRecalling())
                {
                    return;
                }

                var target = Enemies.FirstOrDefault(x => x.IsValidTarget(spells[Spells.E].Range));

                if (target == null)
                {
                    return;
                }

                if (spells[Spells.W].GetDamage(target) > target.Health && target.LSIsValidTarget(spells[Spells.W].Range))
                {
                    spells[Spells.W].Cast();
                }

                if (spells[Spells.E].GetDamage(target) > target.Health && target.LSIsValidTarget(spells[Spells.E].Range))
                {
                    var prediction = spells[Spells.E].GetPrediction(target);
                    if (prediction.Hitchance >= HitChance.VeryHigh)
                    {
                        spells[Spells.E].Cast(prediction.CastPosition);
                    }
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private static void OnDash(Obj_AI_Base sender, Dash.DashItem args)
        {
            try
            {
                if (!sender.IsMe || !Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
                {
                    return;
                }

                var target = TargetSelector.GetTarget(1500f, DamageType.Physical);
                if (!target.LSIsValidTarget())
                {
                    return;
                }

                if (!RengarR)
                {
                    ActiveModes.CastItems(target);
                }

                if (Ferocity == 5)
                {
                    switch (MenuInit.getBoxItem(MenuInit.comboMenu, "Combo.Prio"))
                    {
                        case 0:
                            if (spells[Spells.E].IsReady() && target.LSIsValidTarget(spells[Spells.E].Range))
                            {
                                var pred = spells[Spells.E].GetPrediction(target);
                                spells[Spells.E].Cast(target);
                            }
                            break;
                        case 2:
                            if (spells[Spells.Q].IsReady()
                                && Player.LSCountEnemiesInRange(Player.AttackRange + Player.BoundingRadius + 100) != 0)
                            {
                                spells[Spells.Q].Cast();
                            }
                            break;
                    }
                }
                else
                {
                    if (MenuInit.getBoxItem(MenuInit.comboMenu, "Combo.Prio") != 0)
                    {
                        if (spells[Spells.E].IsReady())
                        {
                            var targetE = TargetSelector.GetTarget(
                                spells[Spells.E].Range,
                                DamageType.Physical);
                            if (targetE.IsValidTarget(spells[Spells.E].Range))
                            {
                                var pred = spells[Spells.E].GetPrediction(targetE);
                                if (pred.Hitchance >= HitChance.Medium)
                                {
                                    spells[Spells.E].Cast(pred.CastPosition);
                                }
                            }
                        }
                    }
                }

                switch (MenuInit.getBoxItem(MenuInit.comboMenu, "Combo.Prio"))
                {
                    case 0:
                        if (spells[Spells.E].IsReady() && target.LSIsValidTarget(spells[Spells.E].Range))
                        {
                            var pred = spells[Spells.E].GetPrediction(target);
                            spells[Spells.E].Cast(target);
                        }
                        break;

                    case 2:
                        if (MenuInit.getCheckBoxItem(MenuInit.betaMenu, "Beta.Cast.Q1") && RengarR)
                        {
                            spells[Spells.Q].Cast();
                        }
                        break;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private static void OnDraw(EventArgs args)
        {
            try
            {
                var drawW = MenuInit.getCheckBoxItem(MenuInit.miscMenu, "Misc.Drawings.W");
                var drawE = MenuInit.getCheckBoxItem(MenuInit.miscMenu, "Misc.Drawings.E");
                var drawExclamation = MenuInit.getCheckBoxItem(MenuInit.miscMenu, "Misc.Drawings.Exclamation");
                //Exclamation mark

                var drawSearchRange = MenuInit.getCheckBoxItem(MenuInit.betaMenu, "Beta.Search.Range");
                var searchrange = MenuInit.getSliderItem(MenuInit.betaMenu, "Beta.searchrange");

                var drawsearchrangeQ = MenuInit.getCheckBoxItem(MenuInit.betaMenu, "Beta.Search.QCastRange");
                var searchrangeQCastRange = MenuInit.getSliderItem(MenuInit.betaMenu, "Beta.searchrange.Q");

                if (SelectedEnemy.LSIsValidTarget() && SelectedEnemy.IsVisible && !SelectedEnemy.IsDead)
                {
                    Drawing.DrawText(Drawing.WorldToScreen(SelectedEnemy.Position).X - 40,
                        Drawing.WorldToScreen(SelectedEnemy.Position).Y + 10, Color.White, "Selected Target");
                }

                if (MenuInit.getCheckBoxItem(MenuInit.miscMenu, "Misc.Drawings.Off"))
                {
                    return;
                }

                if (MenuInit.getCheckBoxItem(MenuInit.betaMenu, "Beta.Cast.Q1"))
                {
                    if (drawSearchRange && spells[Spells.R].Level > 0)
                    {
                        Render.Circle.DrawCircle(ObjectManager.Player.Position, searchrange, Color.Orange);
                    }

                    if (drawsearchrangeQ && spells[Spells.R].Level > 0)
                    {
                        Render.Circle.DrawCircle(ObjectManager.Player.Position, searchrangeQCastRange, Color.Orange);
                    }
                }

                if (RengarR && drawExclamation)
                {
                    if (spells[Spells.R].Level > 0)
                    {
                        Render.Circle.DrawCircle(ObjectManager.Player.Position, 1450f, Color.DeepSkyBlue);
                    }
                }

                if (drawW)
                {
                    if (spells[Spells.W].Level > 0)
                    {
                        Render.Circle.DrawCircle(ObjectManager.Player.Position, spells[Spells.W].Range, Color.Purple);
                    }
                }

                if (drawE)
                {
                    if (spells[Spells.E].Level > 0)
                    {
                        Render.Circle.DrawCircle(ObjectManager.Player.Position, spells[Spells.E].Range, Color.White);
                    }
                }

                if (MenuInit.getCheckBoxItem(MenuInit.miscMenu, "Misc.Drawings.Prioritized"))
                {
                    switch (MenuInit.getBoxItem(MenuInit.comboMenu, "Combo.Prio"))
                    {
                        case 0:
                            Drawing.DrawText(
                                Drawing.Width * 0.70f,
                                Drawing.Height * 0.95f,
                                Color.Yellow,
                                "Prioritized spell: E");
                            break;
                        case 1:
                            Drawing.DrawText(
                                Drawing.Width * 0.70f,
                                Drawing.Height * 0.95f,
                                Color.White,
                                "Prioritized spell: W");
                            break;
                        case 2:
                            Drawing.DrawText(
                                Drawing.Width * 0.70f,
                                Drawing.Height * 0.95f,
                                Color.White,
                                "Prioritized spell: Q");
                            break;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private static void OnDrawEndScene(EventArgs args)
        {
            try
            {
                if (Player.IsDead)
                {
                    return;
                }

                if (MenuInit.getCheckBoxItem(MenuInit.miscMenu, "Misc.Drawings.Minimap") && spells[Spells.R].Level > 0)
                {
                    Utility.DrawCircle(ObjectManager.Player.Position, spells[Spells.R].Range, Color.White, 1, 23, true);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private static void OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            try
            {
                if (sender.IsMe)
                {
                    switch (args.SData.Name.ToLower())
                    {
                        case "RengarR":
                            if (Items.HasItem(3142) && Items.CanUseItem(3142))
                            {
                                Utility.DelayAction.Add(2000, () => Items.UseItem(3142));
                            }
                            break;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private static void OnUpdate(EventArgs args)
        {
            try
            {
                if (Player.IsDead)
                {
                    return;
                }

                if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
                {
                    ActiveModes.Combo();
                }

                if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear) ||
                    Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear))
                {
                    ActiveModes.Laneclear();
                    ActiveModes.Jungleclear();
                }

                if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass))
                {
                    ActiveModes.Harass();
                }

                SwitchCombo();
                Heal();
                KillstealHandler();

                // E on Immobile targets
                if (MenuInit.getCheckBoxItem(MenuInit.miscMenu, "Misc.Root") && spells[Spells.E].IsReady())
                {
                    if (RengarR)
                    {
                        return;
                    }

                    var target = HeroManager.Enemies.FirstOrDefault(h => h.IsValidTarget(spells[Spells.E].Range));

                    if (target != null)
                    {
                        if (Ferocity != 5)
                        {
                            return;
                        }

                        spells[Spells.E].CastIfHitchanceEquals(target, HitChance.Immobile);
                    }
                }

                if (MenuInit.getCheckBoxItem(MenuInit.betaMenu, "Beta.Cast.Q1") &&
                    MenuInit.getBoxItem(MenuInit.comboMenu, "Combo.Prio") == 2)
                {
                    if (Ferocity != 5)
                    {
                        return;
                    }

                    var searchrange = MenuInit.getSliderItem(MenuInit.betaMenu, "Beta.searchrange");
                    var target = HeroManager.Enemies.FirstOrDefault(h => h.LSIsValidTarget(searchrange, false));

                    if (!target.LSIsValidTarget())
                    {
                        return;
                    }

                    if (RengarR)
                    {
                        if (Player.LSDistance(target) <= MenuInit.getSliderItem(MenuInit.betaMenu, "Beta.searchrange.Q"))
                        {
                            Utility.DelayAction.Add(
                                MenuInit.getSliderItem(MenuInit.betaMenu, "Beta.Cast.Q1.Delay"),
                                () => spells[Spells.Q].Cast());
                        }
                    }
                }

                spells[Spells.R].Range = 1000 + spells[Spells.R].Level * 1000;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private static void SwitchCombo()
        {
            try
            {
                var switchTime = Utils.GameTimeTickCount - LastSwitch;
                if (MenuInit.getKeyBindItem(MenuInit.comboMenu, "Combo.Switch") && switchTime >= 350)
                {
                    switch (MenuInit.getBoxItem(MenuInit.comboMenu, "Combo.Prio"))
                    {
                        case 0:
                            //MenuInit.Menu.Item("Combo.Prio").SetValue(new StringList(new[] { "E", "W", "Q" }, 2));
                            MenuInit.comboMenu["Combo.Prio"].Cast<ComboBox>().CurrentValue = 2;
                            LastSwitch = Utils.GameTimeTickCount;
                            break;
                        case 1:
                            //MenuInit.Menu.Item("Combo.Prio").SetValue(new StringList(new[] { "E", "W", "Q" }, 0));
                            MenuInit.comboMenu["Combo.Prio"].Cast<ComboBox>().CurrentValue = 0;
                            LastSwitch = Utils.GameTimeTickCount;
                            break;

                        default:
                            //MenuInit.Menu.Item("Combo.Prio").SetValue(new StringList(new[] { "E", "W", "Q" }, 0));
                            MenuInit.comboMenu["Combo.Prio"].Cast<ComboBox>().CurrentValue = 0;
                            LastSwitch = Utils.GameTimeTickCount;
                            break;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        #endregion
    }
}