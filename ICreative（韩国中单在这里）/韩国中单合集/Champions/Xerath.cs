using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK.Rendering;
using KoreanAIO.Managers;
using KoreanAIO.Model;
using SharpDX;
using Color = System.Drawing.Color;

namespace KoreanAIO.Champions
{
    public class Xerath : ChampionBase
    {
        private static class Spells
        {
            public static class Q
            {
                public const int CastRangeGrowthMin = 750;
                public const int CastRangeGrowthMax = 1400;
                public const float CastRangeGrowthStartTime = 0f;
                public const float CastRangeGrowthDuration = 1.5f;
                public const float CastRangeGrowthEndTime = 3f;
            }

            public static class W
            {
                public const int CastRadiusSecondary = 250;
                public const int CastRadius = 100;
            }

            public static class E
            {
                public static MissileClient Missile;
                public const string MissileName = "xerathmagespearmissile";
            }
        }

        public bool HasPassive
        {
            get { return MyHero.TargetHaveBuff("XerathAscended2OnHit"); }
        }
        public bool IsChargingQ
        {
            get { return MyHero.TargetHaveBuff("XerathArcanopulseChargeUp"); }
        }
        public bool IsCastingR
        {
            get { return MyHero.TargetHaveBuff("XerathR"); }
        }
        public int Stacks
        {
            get { return IsCastingR ? MyHero.GetBuffCount("XerathRShots") : (R.Level + 2); }
        }
        public bool StartWithCc
        {
            get { return KeysMenu.KeyBind("StartWithCC"); }
        }
        public readonly Dictionary<int, Text> IsKillableOnEnemyPosition = new Dictionary<int, Text>();
        public readonly Dictionary<int, Text> IsKillableOnScreen = new Dictionary<int, Text>();
        private static readonly Vector2 BaseScreenPoint = new Vector2(100, 50);
        private const float TextScreenSize = 28F;
        private const float TextEnemyPositionSize = 22F;
        public bool TapKeyPressed;
        public Xerath()
        {
            Q = new SpellBase(SpellSlot.Q, SpellType.Linear, 1500)
            {
                CastDelay = 500,
                Width = 95,
            };
            W = new SpellBase(SpellSlot.W, SpellType.Circular, 1000)
            {
                CastDelay = 750,
                Width = 100,//Width = 250,
            };
            E = new SpellBase(SpellSlot.E, SpellType.Linear, 1125)
            {
                CastDelay = 250,
                Speed = 1400,
                Width = 60,
                CollidesWithYasuoWall = true,
                AllowedCollisionCount = 0,
            };
            R = new SpellBase(SpellSlot.R, SpellType.Circular, 5600)
            {
                Width = 200,
                CastDelay = 600,
            };
            foreach (var enemy in EntityManager.Heroes.Enemies)
            {
                IsKillableOnEnemyPosition.Add(enemy.NetworkId, new Text("R KILLABLE", new Font("Arial", TextEnemyPositionSize, FontStyle.Bold))
                {
                    Color = Color.Red,
                });
                IsKillableOnScreen.Add(enemy.NetworkId, new Text(enemy.BaseSkinName + " is killable", new Font("Arial", TextScreenSize, FontStyle.Bold))
                {
                    Color = Color.Red,
                });
            }
            Obj_AI_Base.OnProcessSpellCast += delegate (Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
            {
                if (sender.IsMe)
                {
                    switch (args.Slot)
                    {
                        case SpellSlot.Q:
                            Q.LastCastTime = Core.GameTickCount;
                            break;
                        case SpellSlot.W:
                            W.LastCastTime = Core.GameTickCount;
                            W.LastEndPosition = args.End;
                            break;
                        case SpellSlot.E:
                            E.LastCastTime = Core.GameTickCount;
                            E.LastEndPosition = sender.Position + (args.End - sender.Position).Normalized() * E.Range;
                            break;
                        case SpellSlot.R:
                            R.LastCastTime = Core.GameTickCount;
                            TapKeyPressed = false;
                            break;
                    }
                }
            };
            GameObject.OnCreate += delegate (GameObject sender, EventArgs args)
            {
                var missile = sender as MissileClient;
                if (missile != null && missile.SpellCaster.IsMe && missile.SData.Name.ToLower() == Spells.E.MissileName)
                {
                    Spells.E.Missile = missile;
                }
            };
            GameObject.OnDelete += delegate (GameObject sender, EventArgs args)
            {
                var missile = sender as MissileClient;
                if (missile != null && missile.SpellCaster.IsMe && missile.SData.Name.ToLower() == Spells.E.MissileName)
                {
                    Spells.E.Missile = null;
                }
            };
            Gapcloser.OnGapcloser += delegate (AIHeroClient sender, Gapcloser.GapcloserEventArgs args)
            {
                if (sender.IsEnemy && AutomaticMenu.CheckBox("Gapcloser") && args.End.Distance(MyHero, true) <= sender.Distance(MyHero, true))
                {
                    CastE(sender);
                }
            };
            Dash.OnDash += delegate (Obj_AI_Base sender, Dash.DashEventArgs args)
            {
                if (sender.IsEnemy && AutomaticMenu.CheckBox("Gapcloser") && args.EndPos.Distance(MyHero, true) <= sender.Distance(MyHero, true))
                {
                    CastE(sender);
                }
            };
            Interrupter.OnInterruptableSpell +=
                delegate (Obj_AI_Base sender, Interrupter.InterruptableSpellEventArgs args)
                {
                    if (sender.IsEnemy && AutomaticMenu.CheckBox("Interrupter"))
                    {
                        CastE(sender);
                    }
                };

            MenuManager.AddSubMenu("Keys");
            {
                KeysMenu.AddValue("TapKey",
                    new KeyBind("R Tap Key", false, KeyBind.BindTypes.HoldActive, 'T')).OnValueChange += delegate (ValueBase<bool> sender, ValueBase<bool>.ValueChangeArgs args)
                    {
                        if (args.NewValue && R.IsLearned)
                        {
                            TapKeyPressed = true;
                            if (!IsCastingR)
                            {
                                R.Cast();
                            }
                        }
                    };
                ToggleManager.RegisterToggle(
                    KeysMenu.AddValue("StartWithCC",
                        new KeyBind("Start with CC", false, KeyBind.BindTypes.PressToggle, 'L')),
                    delegate
                    {

                    });
            }
            Q.AddConfigurableHitChancePercent();
            W.AddConfigurableHitChancePercent();
            E.AddConfigurableHitChancePercent();
            R.AddConfigurableHitChancePercent();

            MenuManager.AddSubMenu("Combo");
            {
                ComboMenu.AddValue("Q", new CheckBox("Use Q"));
                ComboMenu.AddValue("W", new CheckBox("Use W"));
                ComboMenu.AddValue("E", new CheckBox("Use E"));
            }

            MenuManager.AddSubMenu("Ultimate");
            {
                UltimateMenu.AddStringList("Mode", "R AIM Mode", new[] { "Disabled", "Using TapKey", "Automatic" }, 2);
                UltimateMenu.AddValue("Delay", new Slider("Delay between R's (in ms)", 0, 0, 1500));
                UltimateMenu.AddValue("NearMouse", new GroupLabel("Near Mouse Settings"));
                UltimateMenu.AddValue("NearMouse.Enabled", new CheckBox("Only select target near mouse", false));
                UltimateMenu.AddValue("NearMouse.Radius", new Slider("Near mouse radius", 500, 100, 1500));
                UltimateMenu.AddValue("NearMouse.Draw", new CheckBox("Draw near mouse radius"));
            }

            MenuManager.AddSubMenu("Harass");
            {
                HarassMenu.AddValue("Q", new CheckBox("Use Q", false));
                HarassMenu.AddValue("W", new CheckBox("Use W", false));
                HarassMenu.AddValue("E", new CheckBox("Use E", false));
                HarassMenu.AddValue("ManaPercent", new Slider("Minimum Mana Percent", 25));
            }

            MenuManager.AddSubMenu("Clear");
            {
                ClearMenu.AddValue("LaneClear", new GroupLabel("LaneClear"));
                {
                    ClearMenu.AddValue("LaneClear.Q", new Slider("Use Q if hit >= {0}", 0, 0, 10));
                    ClearMenu.AddValue("LaneClear.W", new Slider("Use W if hit >= {0}", 3, 0, 10));
                    ClearMenu.AddValue("LaneClear.ManaPercent", new Slider("Minimum Mana Percent", 50));
                }
                ClearMenu.AddValue("JungleClear", new GroupLabel("JungleClear"));
                {
                    ClearMenu.AddValue("JungleClear.Q", new CheckBox("Use Q"));
                    ClearMenu.AddValue("JungleClear.W", new CheckBox("Use W"));
                    ClearMenu.AddValue("JungleClear.E", new CheckBox("Use E"));
                    ClearMenu.AddValue("JungleClear.ManaPercent", new Slider("Minimum Mana Percent", 20));
                }
            }
            MenuManager.AddKillStealMenu();
            {
                KillStealMenu.AddValue("Q", new CheckBox("Use Q"));
                KillStealMenu.AddValue("W", new CheckBox("Use W"));
                KillStealMenu.AddValue("E", new CheckBox("Use E"));
                KillStealMenu.AddValue("R", new CheckBox("Use R", false));
            }
            MenuManager.AddSubMenu("Automatic");
            {
                AutomaticMenu.AddValue("Interrupter", new CheckBox("Use E on channeling spells"));
                AutomaticMenu.AddValue("Gapcloser", new CheckBox("Use E on hero gapclosing / dashing"));
            }
            MenuManager.AddDrawingsMenu();
            {
                Q.AddDrawings();
                W.AddDrawings();
                E.AddDrawings();
                R.AddDrawings();
                DrawingsMenu.AddValue("R.Killable", new CheckBox("Draw text if target is r killable"));
                DrawingsMenu.AddValue("Toggles", new CheckBox("Draw toggles status"));
            }
        }

        public override void OnDraw(Menu menu)
        {
            if (UltimateMenu.CheckBox("NearMouse.Enabled") && UltimateMenu.CheckBox("NearMouse.Draw") && IsCastingR)
            {
                EloBuddy.SDK.Rendering.Circle.Draw(SharpDX.Color.Blue, UltimateMenu.Slider("NearMouse.Radius"), 1, MousePos);
            }
            base.OnDraw(menu);
        }
        public override void OnEndScene(Menu menu)
        {
            if (menu.CheckBox("R.Killable") && R.IsReady && MyHero.Mana >= R.Mana)
            {
                var count = 0;
                foreach (var enemy in R.EnemyHeroes.Where(h => R.GetDamage(h) * Stacks >= h.TotalShieldHealth()))
                {
                    if (enemy.VisibleOnScreen && IsKillableOnEnemyPosition.ContainsKey(enemy.NetworkId))
                    {
                        IsKillableOnEnemyPosition[enemy.NetworkId].Position = enemy.Position.WorldToScreen();
                        IsKillableOnEnemyPosition[enemy.NetworkId].Draw();
                    }
                    if (IsKillableOnScreen.ContainsKey(enemy.NetworkId))
                    {
                        IsKillableOnScreen[enemy.NetworkId].Position = BaseScreenPoint + new Vector2(0, 50 * count);
                        IsKillableOnScreen[enemy.NetworkId].Draw();
                        count++;
                    }
                }
            }
            base.OnEndScene(menu);
        }

        protected override void PermaActive()
        {
            Orbwalker.DisableMovement = IsCastingR;
            Orbwalker.DisableAttacking = IsCastingR || IsChargingQ;
            if (IsChargingQ)
            {
                var percentGrowth = Math.Max(0, Math.Min(1, ((Core.GameTickCount - Q.LastCastTime) / 1000f - Spells.Q.CastRangeGrowthStartTime) / Spells.Q.CastRangeGrowthDuration)) ;
                Q.Range = (int)((Spells.Q.CastRangeGrowthMax - Spells.Q.CastRangeGrowthMin) * percentGrowth + Spells.Q.CastRangeGrowthMin);
            }
            else
            {
                Q.Range = Spells.Q.CastRangeGrowthMax;
            }
            W.Width = Spells.W.CastRadiusSecondary;
            R.Range = 2000 + R.Instance.Level * 1200;
            Range = new[] { Q.IsReady ? Spells.Q.CastRangeGrowthMax : 0f, W.IsReady ? W.Range + W.Radius : 0, E.IsReady ? E.Range : 0, MyHero.GetAutoAttackRange() }.Max();
            if (Spells.E.Missile != null && !Spells.E.Missile.IsValidMissile())
            {
                Spells.E.Missile = null;
            }
            Target = TargetSelector.GetTarget(UnitManager.ValidEnemyHeroesInRange, DamageType.Magical);
            if (IsCastingR)
            {
                if (Core.GameTickCount - R.LastCastTime > UltimateMenu.Slider("Delay"))
                {
                    if (UltimateMenu.Slider("Mode") == 2 || (UltimateMenu.Slider("Mode") == 1 && TapKeyPressed))
                    {
                        if (UltimateMenu.CheckBox("NearMouse.Enabled"))
                        {
                            var target = FindBestKillableTarget(MousePos, UltimateMenu.Slider("NearMouse.Radius"));
                            if (target != null)
                            {
                                R.Cast(target);
                            }
                        }
                        else
                        {
                            var target = FindBestKillableTarget(MyHero.Position, R.Range);
                            if (target != null)
                            {
                                R.Cast(target);
                            }
                        }
                    }
                }
                return;
            }
            TapKeyPressed = false;
            if (KeysMenu.KeyBind("TapKey") && R.IsReady && !IsCastingR)
            {
                R.Cast();
                return;
            }
            base.PermaActive();
        }
        protected override void KillSteal(Menu menu)
        {
            foreach (var enemy in UnitManager.ValidEnemyHeroesInRange.Where(h => h.HealthPercent <= 40f))
            {
                var result = GetBestCombo(enemy);
                if (menu.CheckBox("Q") && (result.Q || Q.IsKillable(enemy)))
                {
                    CastQ(enemy);
                }
                if (menu.CheckBox("W") && (result.W || W.IsKillable(enemy)))
                {
                    CastW(enemy);
                }
                if (menu.CheckBox("E") && (result.E || E.IsKillable(enemy)))
                {
                    CastE(enemy);
                }
                if (menu.CheckBox("R") && IsCastingR && R.GetDamage(enemy) * Stacks >= enemy.TotalShieldHealth())
                {
                    R.Cast(enemy);
                }
            }
            base.KillSteal(menu);
        }

        protected override void Combo(Menu menu)
        {
            if (Target != null)
            {
                if (Core.GameTickCount - W.LastSentTime < 100)
                {
                    return;
                }
                if (Core.GameTickCount - E.LastSentTime < 100)
                {
                    return;
                }
                if (Core.GameTickCount - W.LastCastTime < W.CastDelay + 50)
                {
                    return;
                }
                if (Core.GameTickCount - E.LastCastTime < E.CastDelay + 50)
                {
                    return;
                }
                if (Spells.E.Missile != null && Spells.E.Missile.IsValidMissile() && MyHero.Distance(Spells.E.Missile, true) <= MyHero.Distance(Target, true))
                {
                    return;
                }
                if (Orbwalker.CanAutoAttack && HasPassive && MyHero.IsInAutoAttackRange(Target))
                {
                    return;
                }
                if (menu.CheckBox("E"))
                {
                    CastE(Target);
                }
                if (menu.CheckBox("W"))
                {
                    CastW(Target);
                }
                if (StartWithCc && W.IsReady && E.IsReady)
                {
                    return;
                }
                if (menu.CheckBox("Q"))
                {
                    CastQ(Target);
                }
            }
            base.Combo(menu);
        }

        protected override void Harass(Menu menu)
        {
            if (MyHero.ManaPercent >= menu.Slider("ManaPercent"))
            {
                if (Target != null)
                {
                    if (Core.GameTickCount - W.LastSentTime < 100)
                    {
                        return;
                    }
                    if (Core.GameTickCount - E.LastSentTime < 100)
                    {
                        return;
                    }
                    if (Core.GameTickCount - W.LastCastTime < W.CastDelay + 50)
                    {
                        return;
                    }
                    if (Core.GameTickCount - E.LastCastTime < E.CastDelay + 50)
                    {
                        return;
                    }
                    if (Spells.E.Missile != null && Spells.E.Missile.IsValidMissile() && MyHero.Distance(Spells.E.Missile, true) <= MyHero.Distance(Target, true))
                    {
                        return;
                    }
                    if (Orbwalker.CanAutoAttack && HasPassive && MyHero.IsInAutoAttackRange(Target))
                    {
                        return;
                    }
                    if (menu.CheckBox("E"))
                    {
                        CastE(Target);
                    }
                    if (menu.CheckBox("W"))
                    {
                        CastW(Target);
                    }
                    if (StartWithCc && W.IsReady && E.IsReady)
                    {
                        return;
                    }
                    if (menu.CheckBox("Q"))
                    {
                        CastQ(Target);
                    }
                }
            }
            base.Harass(menu);
        }
        protected override void LaneClear(Menu menu)
        {
            if (MyHero.ManaPercent >= menu.Slider("LaneClear.ManaPercent"))
            {
                CastQ(Q.LaneClear(false, menu.Slider("LaneClear.Q")));
                CastW(W.LaneClear(false, menu.Slider("LaneClear.W")));
            }
            base.LaneClear(menu);
        }

        protected override void JungleClear(Menu menu)
        {
            if (MyHero.ManaPercent >= menu.Slider("JungleClear.ManaPercent"))
            {
                if (menu.CheckBox("JungleClear.Q"))
                {
                    CastQ(Q.JungleClear(false));
                }
                if (menu.CheckBox("JungleClear.W"))
                {
                    CastW(W.JungleClear(false));
                }
                if (menu.CheckBox("JungleClear.E"))
                {
                    CastE(E.JungleClear(false));
                }
            }
            base.JungleClear(menu);
        }


        private void CastQ(Obj_AI_Base target)
        {
            if (IsCastingR)
            {
                return;
            }
            if (Q.IsReady && target != null)
            {
                var pred = Q.GetPrediction(target);
                if (IsChargingQ)
                {
                    if (pred.HitChancePercent >= Q.HitChancePercent)
                    {
                        MyHero.Spellbook.UpdateChargeableSpell(Q.Slot, pred.CastPosition, true);
                    }
                    else if (Core.GameTickCount - Q.LastCastTime >= Spells.Q.CastRangeGrowthEndTime * 1000 * 0.85f)
                    {
                        MyHero.Spellbook.UpdateChargeableSpell(Q.Slot, pred.CastPosition, true);
                    }
                }
                else
                {
                    if (pred.HitChancePercent >= Q.HitChancePercent / 2f)
                    {
                        MyHero.Spellbook.CastSpell(Q.Slot, Game.CursorPos, true);
                    }
                }
            }
        }

        private void CastW(Obj_AI_Base target)
        {
            if (IsCastingR)
            {
                return;
            }
            if (W.IsReady && target != null)
            {
                W.Width = target.Type == GameObjectType.AIHeroClient ? Spells.W.CastRadius : Spells.W.CastRadiusSecondary;
                W.Cast(target);
            }
        }
        private void CastE(Obj_AI_Base target)
        {
            if (IsCastingR)
            {
                return;
            }
            if (E.IsReady && target != null)
            {
                E.Cast(target);
            }
        }
        protected AIHeroClient FindBestKillableTarget(Vector3 source, float range)
        {
            return UnitManager.ValidEnemyHeroes.Where(h => source.IsInRange(h, range) && R.GetDamage(h) * (Stacks + 2) >= h.TotalShieldHealth()).OrderByDescending(h => (h.Health - R.GetDamage(h)) * Stacks / h.MaxHealth).FirstOrDefault();
        }
        public override float GetSpellDamage(SpellSlot slot, Obj_AI_Base target)
        {
            if (target != null)
            {
                var level = slot.GetSpellDataInst().Level;
                switch (slot)
                {
                    case SpellSlot.Q:
                        return MyHero.CalculateDamageOnUnit(target, DamageType.Magical,
                            40f * level + 40f + 0.75f * MyHero.FlatMagicDamageMod);
                    case SpellSlot.W:
                        return MyHero.CalculateDamageOnUnit(target, DamageType.Magical,
                            30f * level + 30f + 0.6f * MyHero.FlatMagicDamageMod);
                    case SpellSlot.E:
                        return MyHero.CalculateDamageOnUnit(target, DamageType.Magical,
                            50f * level + 30f + 0.45f * MyHero.FlatMagicDamageMod);
                    case SpellSlot.R:
                        return MyHero.CalculateDamageOnUnit(target, DamageType.Magical,
                            30f * level + 160f + 0.43f * MyHero.FlatMagicDamageMod);
                }
            }
            return base.GetSpellDamage(slot, target);
        }
    }
}
