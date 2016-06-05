using ClipperLib;
using Color = System.Drawing.Color;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK;
using EloBuddy;
using Font = SharpDX.Direct3D9.Font;
using LeagueSharp.Common.Data;
using LeagueSharp.Common;
using SharpDX.Direct3D9;
using SharpDX;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using System.Security.AccessControl;
using System;
using System.Speech.Synthesis;
using static FreshBooster.FreshCommon;

namespace FreshBooster.Champion
{
    class Veigar
    {
        // Default Setting
        public static int ErrorTime;
        public const string ChampName = "Veigar";   // Edit
        public static AIHeroClient Player;
        public static LeagueSharp.Common.Spell _Q, _W, _E, _R;
        public static Menu menu, Combo, Harass, LaneClear, JungleClear, KillSteal, Misc, Draw;
        // Default Setting

        private void SkillSet()
        {
            try
            {
                _Q = new LeagueSharp.Common.Spell(SpellSlot.Q, 900);
                _Q.SetSkillshot(0.25f, 70f, 2000f, false, SkillshotType.SkillshotLine);
                _W = new LeagueSharp.Common.Spell(SpellSlot.W, 900);
                _W.SetSkillshot(0.5f, 200f, float.MaxValue, false, SkillshotType.SkillshotCircle);
                _E = new LeagueSharp.Common.Spell(SpellSlot.E, 1150); // true range 750, circle 400
                _E.SetSkillshot(1.2f, 80f, float.MaxValue, false, SkillshotType.SkillshotCircle);
                _R = new LeagueSharp.Common.Spell(SpellSlot.R, 615);
            }
            catch (Exception)
            {
                if (NowTime() > ErrorTime)
                {
                    Chat.Print(ChampName + " in FreshBooster isn't Load. Error Code 01");
                    ErrorTime = TickCount(10000);
                }
            }
        }

        public static bool getCheckBoxItem(Menu m, string item)
        {
            return m[item].Cast<CheckBox>().CurrentValue;
        }

        public static int getSliderItem(Menu m, string item)
        {
            return m[item].Cast<Slider>().CurrentValue;
        }

        public static bool getKeyBindItem(Menu m, string item)
        {
            return m[item].Cast<KeyBind>().CurrentValue;
        }

        public static int getBoxItem(Menu m, string item)
        {
            return m[item].Cast<ComboBox>().CurrentValue;
        }

        public static int getQHitChance()
        {
            return Combo["Veigar_CUseQ_Hit"].Cast<Slider>().CurrentValue;
        }

        private void Menu()
        {
            try
            {
                menu = MainMenu.AddMenu("FreshBooster", "VEIGAERSASD");
                Combo = menu.AddSubMenu("Combo", "Combo");
                Combo.Add("Veigar_CUseQ", new CheckBox("Use Q"));
                Combo.Add("Veigar_CUseW", new CheckBox("Use W"));
                Combo.Add("Veigar_CUseE", new CheckBox("Use E"));
                Combo.Add("Veigar_CUseR", new CheckBox("Use R"));
                Combo.Add("Veigar_CUseR_Select", new CheckBox("When can be Kill, Only use R"));
                //Combo.Add("buffer", new Slider("Buffer", 5, 0, 25));
                Combo.AddLabel("1 : Out of Range");
                Combo.AddLabel("2 : Impossible");
                Combo.AddLabel("3 : Low");
                Combo.AddLabel("4 : Medium");
                Combo.AddLabel("5 : High");
                Combo.AddLabel("6 : Very High");
                Combo.Add("Veigar_CUseQ_Hit", new Slider("Q HitChance", 3, 1, 6));

                Harass = menu.AddSubMenu("Harass", "Harass");
                Harass.Add("Veigar_HUseQ", new CheckBox("Use Q"));
                Harass.Add("Veigar_HUseW", new CheckBox("Use W - When target can't only move"));
                Harass.Add("Veigar_HUseE", new CheckBox("Use E - When target can't only move"));
                Harass.Add("Veigar_HManarate", new Slider("Mana %", 20));
                Harass.Add("Veigar_AutoHUseQ", new KeyBind("Auto Harass", false, KeyBind.BindTypes.PressToggle, 'T'));

                LaneClear = menu.AddSubMenu("LaneClear", "LaneClear");
                LaneClear.Add("Veigar_LUseQ", new CheckBox("Use Q"));
                LaneClear.Add("Veigar_LUseQSet", new CheckBox("Use Q Only use lasthit to minion"));
                LaneClear.Add("Veigar_LManarate", new Slider("Mana %", 20));

                JungleClear = menu.AddSubMenu("JungleClear", "JungleClear");
                JungleClear.Add("Veigar_JUseQ", new CheckBox("Use Q"));
                JungleClear.Add("Veigar_JUseQSet", new CheckBox("Use Q Only use lasthit to minion"));
                JungleClear.Add("Veigar_JManarate", new Slider("Mana %", 20));

                KillSteal = menu.AddSubMenu("KillSteal", "KillSteal");
                KillSteal.Add("Veigar_KseQ", new CheckBox("Use Q"));
                KillSteal.Add("Veigar_KseW", new CheckBox("Use W"));
                KillSteal.Add("Veigar_KseR", new CheckBox("Use R"));

                Misc = menu.AddSubMenu("Misc", "Misc");
                Misc.Add("Veigar_Anti-GapCloser", new CheckBox("Anti GapCloser"));
                Misc.Add("Veigar_Interrupt", new CheckBox("E with Interrupt"));

                Draw = menu.AddSubMenu("Draw", "Draw");
                Draw.Add("Veigar_Draw_Q", new CheckBox("Draw Q", false));
                Draw.Add("Veigar_Draw_W", new CheckBox("Draw W", false));
                Draw.Add("Veigar_Draw_E", new CheckBox("Draw E", false));
                Draw.Add("Veigar_Draw_R", new CheckBox("Draw R", false));
            }
            catch (Exception)
            {
                if (NowTime() > ErrorTime)
                {
                    Chat.Print(ChampName + " in FreshBooster isn't Load. Error Code 02");
                    ErrorTime = TickCount(10000);
                }
            }
        }
        public static void Drawing_OnDraw(EventArgs args)
        {
            try
            {
                if (Player.IsDead)
                    return;
                if (getCheckBoxItem(Draw, "Veigar_Draw_Q"))
                    Render.Circle.DrawCircle(Player.Position, _Q.Range, Color.White, 1);
                if (getCheckBoxItem(Draw, "Veigar_Draw_W"))
                    Render.Circle.DrawCircle(Player.Position, _W.Range, Color.White, 1);
                if (getCheckBoxItem(Draw, "Veigar_Draw_E"))
                    Render.Circle.DrawCircle(Player.Position, _E.Range, Color.White, 1);
                if (getCheckBoxItem(Draw, "Veigar_Draw_R"))
                    Render.Circle.DrawCircle(Player.Position, _R.Range, Color.White, 1);
                var QTarget = TargetSelector.GetTarget(_E.Range, DamageType.Magical);
                if (QTarget != null)
                    Drawing.DrawCircle(QTarget.Position, 150, Color.Green);
            }
            catch (Exception)
            {
                if (NowTime() > ErrorTime)
                {
                    Chat.Print(ChampName + " in FreshBooster isn't Load. Error Code 03");
                    ErrorTime = TickCount(10000);
                }
            }
        }

        // OnLoad
        public Veigar()
        {
            Player = ObjectManager.Player;
            SkillSet();
            Menu();
            Game.OnUpdate += OnGameUpdate;
            Drawing.OnDraw += Drawing_OnDraw;
            AntiGapcloser.OnEnemyGapcloser += AntiGapcloser_OnEnemyGapcloser;
            Interrupter2.OnInterruptableTarget += Interrupter2_OnInterruptableTarget;
        }

        private static void OnGameUpdate(EventArgs args)
        {
            try
            {
                if (Player.IsDead) return;
                var QTarget = TargetSelector.GetTarget(_Q.Range, DamageType.Magical);
                var WTarget = TargetSelector.GetTarget(_W.Range, DamageType.Magical);
                var ETarget = TargetSelector.GetTarget(_E.Range, DamageType.Magical);
                var RTarget = TargetSelector.GetTarget(_E.Range, DamageType.Magical);

                var KTarget = ObjectManager.Get<AIHeroClient>().OrderBy(x => x.Health).FirstOrDefault(x => x.IsEnemy && Player.LSDistance(x) < 900);
                //KillSteal
                if (KTarget != null && !KTarget.IsDead && KTarget.IsEnemy && KTarget.Health > 0)
                {
                    if (getCheckBoxItem(KillSteal, "Veigar_KseQ") && _Q.IsReady() && KTarget.Health < _Q.GetDamage(KTarget) && KTarget.LSDistance(Player) <= _Q.Range)
                    {
                        _Q.CastIfHitchanceEquals(KTarget, Hitchance("Veigar_CUseQ_Hit"), true);
                        return;
                    }
                    if (getCheckBoxItem(KillSteal, "Veigar_KseR") && _R.IsReady() && KTarget.Health < getRDam(KTarget) && KTarget.LSDistance(Player) <= _R.Range)
                    {
                        _R.Cast(KTarget, true);
                        return;
                    }

                    if (getCheckBoxItem(KillSteal, "Veigar_KseW") && _W.IsReady() && !KTarget.CanMove && KTarget.Health < _W.GetDamage(KTarget) && KTarget.LSDistance(Player) <= _W.Range)
                    {
                        _W.CastIfHitchanceEquals(KTarget, LeagueSharp.Common.HitChance.VeryHigh, true);
                        return;
                    }
                }

                //Combo
                if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
                {
                    if (getCheckBoxItem(Combo, "Veigar_CUseE") && ETarget != null && _E.IsReady())
                    {
                        SpellUseE(ETarget);
                    }
                    if (getCheckBoxItem(Combo, "Veigar_CUseW") && WTarget != null && _W.IsReady())
                    {
                        _W.CastIfHitchanceEquals(WTarget, LeagueSharp.Common.HitChance.VeryHigh, true);
                        return;
                    }
                    if (getCheckBoxItem(Combo, "Veigar_CUseQ") && QTarget != null && _Q.IsReady())
                    {
                        _Q.CastIfHitchanceEquals(QTarget, Hitchance("Veigar_CUseQ_Hit"), true);
                        return;
                    }

                    if (getCheckBoxItem(Combo, "Veigar_CUseR_Select") && getCheckBoxItem(Combo, "Veigar_CUseR") && _R.IsReady() && KTarget.Health < getRDam(KTarget) && KTarget.LSDistance(Player) <= _R.Range)
                    {
                        _R.Cast(KTarget, true);
                        return;
                    }

                    if (getCheckBoxItem(Combo, "Veigar_CUseR_Select"))
                    {
                        return;
                    }

                    if (getCheckBoxItem(Combo, "Veigar_CUseR") && RTarget != null && _R.IsReady())
                    {
                        _R.Cast(RTarget, true);
                        return;
                    }
                }

                //Harass
                if ((Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass) || getKeyBindItem(Harass, "Veigar_AutoHUseQ")) && getSliderItem(Harass, "Veigar_HManarate") < Player.ManaPercent)
                {
                    if (getCheckBoxItem(Harass, "Veigar_HUseE") && ETarget != null && _E.IsReady() && !ETarget.CanMove)
                    {
                        SpellUseE(ETarget);
                    }
                    if (getCheckBoxItem(Harass, "Veigar_HUseW") && WTarget != null && _W.IsReady() && !WTarget.CanMove)
                    {
                        _W.CastIfHitchanceEquals(WTarget, LeagueSharp.Common.HitChance.VeryHigh, true);
                        return;
                    }
                    if (getCheckBoxItem(Harass, "Veigar_HUseQ") && QTarget != null && _Q.IsReady())
                    {
                        _Q.CastIfHitchanceEquals(QTarget, Hitchance("Veigar_CUseQ_Hit"), true);
                        return;
                    }
                }

                //LaneClear
                if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear) || Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear) && getSliderItem(LaneClear, "Veigar_LManarate") < Player.ManaPercent)
                {
                    var MinionsTarget = MinionManager.GetMinions(900, MinionTypes.All, MinionTeam.Enemy, MinionOrderTypes.Health);
                    foreach (var item in MinionsTarget)
                    {
                        if (getCheckBoxItem(LaneClear, "Veigar_LUseQ"))
                        {
                            if (getCheckBoxItem(LaneClear, "Veigar_LUseQSet") && item.Health < _Q.GetDamage(item))
                            {
                                _Q.CastIfHitchanceEquals(item, LeagueSharp.Common.HitChance.High, true);
                                return;
                            }
                            if (!getCheckBoxItem(LaneClear, "Veigar_LUseQSet"))
                            {
                                _Q.CastIfHitchanceEquals(item, LeagueSharp.Common.HitChance.High, true);
                                return;
                            }
                        }
                    }

                    var MonsterTarget = MinionManager.GetMinions(900, MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.Health);
                    foreach (var item in MonsterTarget)
                    {
                        if (getCheckBoxItem(JungleClear, "Veigar_JUseQ"))
                        {
                            if (getCheckBoxItem(JungleClear, "Veigar_JUseQSet") && item.Health < _Q.GetDamage(item))
                            {
                                _Q.CastIfHitchanceEquals(item, LeagueSharp.Common.HitChance.Low, true);
                                return;
                            }
                            if (!getCheckBoxItem(JungleClear, "Veigar_JUseQSet"))
                            {
                                _Q.CastIfHitchanceEquals(item, LeagueSharp.Common.HitChance.Low, true);
                                return;
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                if (NowTime() > ErrorTime)
                {
                    //Chat.Print(ChampName + " in FreshBooster isn't Load. Error Code 06");
                    ErrorTime = TickCount(10000);
                }
            }
        }

        public static double getRDam(AIHeroClient target)
        {
            // thanks GOS
            if (target == null)
                return 0f;

            var rDam = 0f;

            if (target.HealthPercent > 33.3)
            {
                rDam = new[] { 175, 250, 325 }[_R.Level] + (.75f * Player.TotalMagicalDamage);
            }
            else
            {
                rDam = new[] { 350, 500, 650 }[_R.Level] + (1.5f * Player.TotalMagicalDamage);
            }

            return rDam + ((0.015 * rDam) * (100 - ((target.Health / target.MaxHealth) * 100)));
        }

        public static void SpellUseE(AIHeroClient target)
        {
            if (!target.CanMove && !target.IsMoving)
            {
                var EPosition = target.ServerPosition.LSExtend(Player.Position, 400);
                _E.Cast(EPosition, true);
                return;
            }

            if (target.LSDistance(Player) > 750)
            {
                var EPosition = target.ServerPosition.LSExtend(Player.Position, 200);
                _E.Cast(EPosition, true);
                return;
            }
            else
            {
                var EPosition = target.ServerPosition;
                _E.Cast(EPosition, true);
                return;
            }
        }
        private static void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            try
            {
                if (getCheckBoxItem(Misc, "Veigar_Anti-GapCloser") && _E.IsReady())
                {
                    var EPosition = gapcloser.End.LSExtend(Player.ServerPosition, 400);
                    _E.Cast(EPosition, true);
                    return;
                }
            }
            catch (Exception)
            {
                if (NowTime() > ErrorTime)
                {
                    Chat.Print(ChampName + " in FreshBooster isn't Load. Error Code 07");
                    ErrorTime = TickCount(10000);
                }
            }
        }

        private static void Interrupter2_OnInterruptableTarget(AIHeroClient sender, Interrupter2.InterruptableTargetEventArgs args)
        {
            try
            {
                if (getCheckBoxItem(Misc, "Veigar_Interrupt") && sender.IsEnemy && _E.IsReady() && sender.LSDistance(Player) < 1150)
                {
                    var EPosition = sender.ServerPosition.LSExtend(Player.Position, 400);
                    _E.Cast(EPosition, true);
                    return;
                }
            }
            catch (Exception)
            {
                if (NowTime() > ErrorTime)
                {
                    Chat.Print(ChampName + " in FreshBooster isn't Load. Error Code 09");
                    ErrorTime = TickCount(10000);
                }
            }

        }
    }
}
