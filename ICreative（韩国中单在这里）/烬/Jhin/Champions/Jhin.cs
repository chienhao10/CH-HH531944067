using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK.Rendering;
using Jhin.Managers;
using Jhin.Model;
using Jhin.Utilities;
using SharpDX;
using Color = System.Drawing.Color;

namespace Jhin.Champions
{
    public class Jhin : ChampionBase
    {
        public bool IsCastingR;
        public bool IsCharging;
        public bool TapKeyPressed;
        public int Stacks;
        public int LastBlockTick;
        public int WShouldWaitTick;
        public Geometry.Polygon.Sector LastRCone;
        public Dictionary<int, Text> TextsInScreen = new Dictionary<int, Text>();
        public Dictionary<int, Text> TextsInHeroPosition = new Dictionary<int, Text>();
        public Dictionary<int, Text> LastPredictedPositionText = new Dictionary<int, Text>();
        public Dictionary<int, Tuple<Vector3, bool>> LastPredictedPosition = new Dictionary<int, Tuple<Vector3, bool>>();
        public Geometry.Polygon.Sector RPolygon;

        public bool IsR1
        {
            get { return R.Instance.SData.Name == "JhinR"; }
        }

        public Jhin()
        {
            foreach (var enemy in EntityManager.Heroes.Enemies)
            {
                TextsInScreen.Add(enemy.NetworkId, new Text(enemy.ChampionName + " 可击杀", new Font("Arial", 30F, FontStyle.Bold)) { Color = Color.Red });
                TextsInHeroPosition.Add(enemy.NetworkId, new Text("R 可击杀", new Font("Arial", 23F, FontStyle.Bold)) { Color = Color.Red });
                LastPredictedPositionText.Add(enemy.NetworkId, new Text(enemy.ChampionName + " 最后预知位置", new Font("Arial", 23F, FontStyle.Bold)) { Color = Color.Red });
            }
            Q = new SpellBase(SpellSlot.Q, SpellType.Targeted, 600)
            {
                Width = 450,
                Speed = 1800,
                CastDelay = 250,
            };
            W = new SpellBase(SpellSlot.W, SpellType.Linear, 2500)
            {
                Width = 40,
                CastDelay = 750,
                AllowedCollisionCount = -1,
            };
            E = new SpellBase(SpellSlot.E, SpellType.Circular, 750)
            {
                Width = 135,
                CastDelay = 250,
                Speed = 1600,
            };
            R = new SpellBase(SpellSlot.R, SpellType.Linear, 3500)
            {
                Width = 80,
                CastDelay = 200,
                Speed = 5000,
                AllowedCollisionCount = -1,
            };
            Evader.OnEvader += delegate
            {
                if (EvaderMenu.CheckBox("BlockW"))
                {
                    LastBlockTick = Core.GameTickCount;
                }
            };

            Spellbook.OnCastSpell += delegate (Spellbook sender, SpellbookCastSpellEventArgs args)
            {
                if (sender.Owner.IsMe)
                {
                    if (args.Slot == SpellSlot.W)
                    {
                        args.Process = Core.GameTickCount - LastBlockTick > 750;
                    }
                }
            };
            Obj_AI_Base.OnProcessSpellCast += delegate (Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
            {
                if (sender.IsMe)
                {
                    switch (args.Slot)
                    {
                        case SpellSlot.W:
                            W.LastCastTime = Core.GameTickCount;
                            W.LastEndPosition = args.End;
                            break;
                        case SpellSlot.R:
                            if (args.SData.Name == "JhinR")
                            {
                                IsCastingR = true;
                                LastRCone = new Geometry.Polygon.Sector(sender.Position, args.End, (float)(60f * Math.PI / 180f), R.Range);
                                Stacks = 4;
                            }
                            else if (args.SData.Name == "JhinRShot")
                            {
                                R.LastCastTime = Core.GameTickCount;
                                TapKeyPressed = false;
                                Stacks--;
                            }
                            break;
                    }
                }
            };
            Gapcloser.OnGapcloser += delegate (AIHeroClient sender, Gapcloser.GapcloserEventArgs args)
            {
                if (sender.IsValidTarget() && sender.IsEnemy)
                {
                    if (MyHero.Distance(args.Start, true) > MyHero.Distance(args.End))
                    {
                        if (AutomaticMenu.CheckBox("E.Gapcloser") && MyHero.IsInRange(args.End, E.Range))
                        {
                            E.Cast(args.End);
                        }
                        if (MyHero.Distance(args.End, true) < (sender.GetAutoAttackRange(MyHero) * 1.5f).Pow())
                        {
                            WShouldWaitTick = Core.GameTickCount;
                        }
                    }
                }
            };

            MenuManager.AddSubMenu("Keys");
            {
                KeysMenu.AddValue("TapKey", new KeyBind("R 半自动发射", false, KeyBind.BindTypes.HoldActive, 32)).OnValueChange +=
                    delegate (ValueBase<bool> sender, ValueBase<bool>.ValueChangeArgs args)
                    {
                        if (args.NewValue && R.IsLearned && IsCastingR)
                        {
                            TapKeyPressed = true;
                        }
                    };
                ToggleManager.RegisterToggle(
                    KeysMenu.AddValue("AutoW",
                        new KeyBind("自动W开关", true, KeyBind.BindTypes.PressToggle, 'K')),
                    delegate
                    {
                        foreach (var enemy in UnitManager.ValidEnemyHeroes.Where(TargetHaveEBuff))
                        {
                            if (MyHero.ManaPercent >= MiscMenu.Slider("W.ManaPercent"))
                            {
                                if (MiscMenu.CheckBox("自动W." + enemy.ChampionName))
                                {
                                    CastW(enemy);
                                }
                            }
                        }
                    });
            }

            W.AddConfigurableHitChancePercent();
            R.AddConfigurableHitChancePercent();

            MenuManager.AddSubMenu("Combo");
            {
                ComboMenu.AddValue("Q", new CheckBox("Use Q"));
                ComboMenu.AddValue("E", new CheckBox("Use E"));
                ComboMenu.AddValue("Items", new CheckBox("Use offensive items"));
                ComboMenu.AddStringList("W", "Use W", new[] { "Never", "Only buffed enemies", "Always" }, 2);
            }
            MenuManager.AddSubMenu("Ultimate");
            {
                UltimateMenu.AddStringList("Mode", "R 瞄准模式", new[] { "不使用", "使用扳机键", "自动" }, 2);
                UltimateMenu.AddValue("OnlyKillable", new CheckBox("只攻击可击杀目标"));
                UltimateMenu.AddValue("Delay", new Slider("R之间的延迟(毫秒)", 0, 0, 1500));
                UltimateMenu.AddValue("NearMouse", new GroupLabel("鼠标附近设置"));
                UltimateMenu.AddValue("NearMouse.Enabled", new CheckBox("只选择鼠标附近目标", false));
                UltimateMenu.AddValue("NearMouse.Radius", new Slider("靠近鼠标半径", 500, 100, 1500));
                UltimateMenu.AddValue("NearMouse.Draw", new CheckBox("显示鼠标半径"));
            }
            MenuManager.AddSubMenu("Harass");
            {
                HarassMenu.AddValue("Q", new CheckBox("Use Q"));
                HarassMenu.AddValue("W", new CheckBox("Use W", false));
                HarassMenu.AddValue("E", new CheckBox("Use E", false));
                HarassMenu.AddValue("ManaPercent", new Slider("Minimum Mana Percent", 20));
            }

            MenuManager.AddSubMenu("Clear");
            {
                ClearMenu.AddValue("LaneClear", new GroupLabel("LaneClear"));
                {
                    ClearMenu.AddValue("LaneClear.Q", new Slider("Use Q if hit is greater than {0}", 3, 0, 10));
                    ClearMenu.AddValue("LaneClear.W", new Slider("Use W if hit is greater than {0}", 5, 0, 10));
                    ClearMenu.AddValue("LaneClear.E", new Slider("Use E if hit is greater than {0}", 4, 0, 10));
                    ClearMenu.AddValue("LaneClear.ManaPercent", new Slider("Minimum Mana Percent", 50));
                }
                ClearMenu.AddValue("LastHit", new GroupLabel("LastHit"));
                {
                    ClearMenu.AddStringList("LastHit.Q", "Use Q", new[] { "Never", "Smartly", "Always" }, 1);
                    ClearMenu.AddValue("LastHit.ManaPercent", new Slider("Minimum Mana Percent", 50));
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
                KillStealMenu.AddValue("R", new CheckBox("Use R"));
            }

            MenuManager.AddSubMenu("Automatic");
            {
                AutomaticMenu.AddValue("E.Gapcloser", new CheckBox("Use E on hero gapclosing / dashing"));
                AutomaticMenu.AddValue("Immobile", new CheckBox("对无法移动目标使用E"));
            }
            MenuManager.AddSubMenu("Evader");
            {
                EvaderMenu.AddValue("BlockW", new CheckBox("保留W进行躲避"));
            }
            Evader.Initialize();
            Evader.AddCrowdControlSpells();
            Evader.AddDangerousSpells();
            MenuManager.AddSubMenu("Misc");
            {
                MiscMenu.AddValue("W.ManaPercent", new Slider("最低蓝量百分比使用自动W", 10));
                MiscMenu.AddValue("Champions", new GroupLabel("自动使用W英雄"));
                foreach (var enemy in EntityManager.Heroes.Enemies)
                {
                    MiscMenu.AddValue("自动W." + enemy.ChampionName, new CheckBox(enemy.ChampionName));
                }
            }
            MenuManager.AddDrawingsMenu();
            {
                Q.AddDrawings(false);
                W.AddDrawings();
                E.AddDrawings(false);
                R.AddDrawings();
                DrawingsMenu.AddValue("Toggles", new CheckBox("Draw toggles status"));
                DrawingsMenu.AddValue("R.Killable", new CheckBox("显示可被R击杀的目标"));
                DrawingsMenu.AddValue("R.LastPredictedPosition", new CheckBox("显示预判敌人最后出现位置"));
            }
        }

        public override void OnEndScene()
        {
            if (R.IsReady || IsCastingR)
            {
                var count = 0;
                foreach (var enemy in R.EnemyHeroes.Where(h => R.IsKillable(h) && TextsInScreen.ContainsKey(h.NetworkId)))
                {
                    TextsInScreen[enemy.NetworkId].Position = new Vector2(100, 50 * count);
                    TextsInScreen[enemy.NetworkId].Draw();
                    if (enemy.VisibleOnScreen)
                    {
                        TextsInHeroPosition[enemy.NetworkId].Position = enemy.Position.WorldToScreen();
                        TextsInHeroPosition[enemy.NetworkId].Draw();
                    }
                    count++;
                }
            }
            base.OnEndScene();
        }

        public override void OnDraw()
        {
            if (UltimateMenu.CheckBox("NearMouse.Enabled") && UltimateMenu.CheckBox("NearMouse.Draw") && IsCastingR)
            {
                EloBuddy.SDK.Rendering.Circle.Draw(SharpDX.Color.Blue, UltimateMenu.Slider("NearMouse.Radius"), 1, MousePos);
            }
            if (DrawingsMenu.CheckBox("R.LastPredictedPosition") && (R.IsReady || IsCastingR))
            {
                foreach (var enemy in EntityManager.Heroes.Enemies.Where(h => !h.IsValidTarget() && !h.IsDead && h.Health > 0 && LastPredictedPosition.ContainsKey(h.NetworkId)))
                {
                    var tuple = LastPredictedPosition[enemy.NetworkId];
                    if (tuple.Item1.IsOnScreen() && tuple.Item2)
                    {
                        LastPredictedPositionText[enemy.NetworkId].Position = tuple.Item1.WorldToScreen() + new Vector2(-LastPredictedPositionText[enemy.NetworkId].Bounding.Width / 2f, 50f);
                        LastPredictedPositionText[enemy.NetworkId].Draw();
                        EloBuddy.SDK.Rendering.Circle.Draw(SharpDX.Color.Red, 120, 1, tuple.Item1);
                    }
                }
            }
            base.OnDraw();
        }

        protected override void PermaActive()
        {
            if (IsCastingR)
            {
                IsCastingR = R.Instance.Name == "JhinRShot"; //MyHero.Spellbook.IsChanneling;
            }
            IsCharging = MyHero.HasBuff("JhinPassiveReload");
            Orbwalker.DisableAttacking = IsCastingR;
            Orbwalker.DisableMovement = IsCastingR;
            if (R.IsReady && !IsCastingR)
            {
                Stacks = 4;
            }
            foreach (var enemy in UnitManager.ValidEnemyHeroes)
            {
                LastPredictedPosition[enemy.NetworkId] = new Tuple<Vector3, bool>(R.GetPrediction(enemy).CastPosition, R.IsKillable(enemy));
            }
            Range = 1300;
            Target = TargetSelector.GetTarget(UnitManager.ValidEnemyHeroesInRange, DamageType.Physical);
            Q.Type = SpellType.Targeted;
            if (IsCastingR)
            {
                if (TapKeyPressed || UltimateMenu.Slider("Mode") == 2)
                {
                    CastR();
                }
                return;
            }
            if (AutomaticMenu.CheckBox("Immobile"))
            {
                foreach (var enemy in E.EnemyHeroes)
                {
                    var time = enemy.GetMovementBlockedDebuffDuration();
                    if (time > 0 && time * 1000 >= E.CastDelay)
                    {
                        E.Cast(enemy.Position);
                    }
                }
            }
            base.PermaActive();
        }

        protected override void KillSteal()
        {
            foreach (var enemy in UnitManager.ValidEnemyHeroes.Where(h => h.HealthPercent <= 40f))
            {
                var result = GetBestCombo(enemy);
                if (KillStealMenu.CheckBox("Q") && (result.Q || Q.IsKillable(enemy)))
                {
                    CastQ(enemy);
                }
                if (KillStealMenu.CheckBox("W") && (result.W || W.IsKillable(enemy)))
                {
                    CastW(enemy);
                }
                if (KillStealMenu.CheckBox("E") && (result.E || E.IsKillable(enemy)))
                {
                    CastE(enemy);
                }
                if (KillStealMenu.CheckBox("R") && IsCastingR && enemy.TotalShieldHealth() <= GetCurrentShotDamage(enemy))
                {
                    CastR();
                }
            }
            base.KillSteal();
        }

        protected override void Combo()
        {
            if (Target != null)
            {
                if (ComboMenu.CheckBox("E") && MyHero.Mana >= E.Mana + R.Mana)
                {
                    CastE(Target);
                }
                if (ComboMenu.Slider("W") > 0 && MyHero.Mana >= W.Mana + E.Mana + R.Mana)
                {
                    if (ComboMenu.Slider("W") == 2)
                    {
                        if (TargetHaveEBuff(Target))
                        {
                            CastW(Target);
                        }
                    }
                    else if (ComboMenu.Slider("W") == 3)
                    {
                        CastW(Target);
                    }
                }
                if (ComboMenu.CheckBox("Q") && MyHero.Mana >= Q.Mana + R.Mana)
                {
                    CastQ(Target);
                }
            }
            base.Combo();
        }

        protected override void Harass()
        {
            if (MyHero.ManaPercent >= HarassMenu.Slider("ManaPercent"))
            {
                if (Target != null)
                {
                    if (HarassMenu.CheckBox("Q") && MyHero.Mana >= E.Mana + R.Mana + W.Mana)
                    {
                        CastQ(Target);
                    }
                    if (HarassMenu.CheckBox("W"))
                    {
                        CastW(Target);
                    }
                    if (HarassMenu.CheckBox("E"))
                    {
                        CastE(Target);
                    }
                }
            }
            base.Harass();
        }

        protected override void LaneClear()
        {
            if (MyHero.ManaPercent >= ClearMenu.Slider("LaneClear.ManaPercent"))
            {
                Q.Type = SpellType.Circular;
                var minion = Q.LaneClear(false, ClearMenu.Slider("LaneClear.Q"));
                Q.Type = SpellType.Targeted;
                CastQ(minion);
                W.LaneClear(true, ClearMenu.Slider("LaneClear.W"));
                E.LaneClear(true, ClearMenu.Slider("LaneClear.E"));
            }
            base.LaneClear();
        }

        protected override void LastHit()
        {
            if (MyHero.ManaPercent >= ClearMenu.Slider("LastHit.ManaPercent"))
            {
                Q.LastHit((LastHitType)ClearMenu.Slider("LastHit.Q"));
            }
            base.LastHit();
        }
        protected override void JungleClear()
        {
            if (MyHero.ManaPercent >= ClearMenu.Slider("JungleClear.ManaPercent"))
            {
                if (ClearMenu.CheckBox("JungleClear.Q"))
                {
                    Q.JungleClear();
                }
                if (ClearMenu.CheckBox("JungleClear.W"))
                {
                    W.JungleClear();
                }
                if (ClearMenu.CheckBox("JungleClear.E"))
                {
                    E.JungleClear();
                }
            }
            base.JungleClear();
        }

        public void CastQ(Obj_AI_Base target)
        {
            if (IsCastingR)
            {
                return;
            }
            if (Q.IsReady && target != null && !Orbwalker.CanAutoAttack)
            {
                if (target is AIHeroClient)
                {
                    Q.Type = SpellType.Circular;
                    Q.EnemyMinionsCanBeCalculated = true;
                    Q.LaneClearMinionsCanBeCalculated = true;
                    Q.EnemyHeroesCanBeCalculated = true;
                    var best = Q.GetBestCircularObject(Q.EnemyMinions.Concat(Q.EnemyHeroes).ToList());
                    if (best.Hits > 0 && best.Target.IsInRange(target, Q.Width))
                    {
                        /*
                        if (best.Target.IdEquals(target))
                        {
                            if (IsCharging)
                            {
                                Q.Type = SpellType.Targeted;
                                Q.Cast(target);
                            }
                        }
                        else
                        {
                            Q.Type = SpellType.Targeted;
                            Q.Cast(target);
                        }*/
                        Q.Type = SpellType.Targeted;
                        Q.Cast(target);
                    }
                }
                else
                {
                    Q.Type = SpellType.Targeted;
                    if (Q.InRange(target))
                    {
                        Q.Cast(target);
                    }
                }
                /*
                var bestMinion = Q.LaneClear(false);
                if (bestMinion != null && bestMinion.IsInRange(target, Q.Width) &&
                    Q.EnemyMinions.Count(o => o.IsInRange(bestMinion, Q.Width)) <= 3)
                {
                    Q.Type = SpellType.Targeted;
                    Q.Cast(target);
                }
                else
                {
                    Q.Type = SpellType.Targeted;
                    if (Q.InRange(target))
                    {
                        Q.Cast(target);
                    }
                }*/
                Q.Type = SpellType.Targeted;
            }
        }
        public void CastW(Obj_AI_Base target)
        {
            if (IsCastingR)
            {
                return;
            }
            if (W.IsReady && target != null)
            {
                if (UnitManager.ValidEnemyHeroesInRange.Any(h => MyHero.InRange(h, 400) && !TargetHaveEBuff(h)))
                {
                    return;
                }
                var hero = target as AIHeroClient;
                if (hero != null)
                {
                    if (!TargetHaveEBuff(hero))
                    {
                        if (Core.GameTickCount - LastBlockTick < 750)
                        {
                            return;
                        }
                        if (Core.GameTickCount - WShouldWaitTick < 750)
                        {
                            return;
                        }
                        if ((Orbwalker.CanAutoAttack && MyHero.IsInAutoAttackRange(target)) || (E.IsReady && E.InRange(hero)))
                        {
                            return;
                        }
                    }
                }
                W.Cast(target);
            }
        }
        public void CastE(Obj_AI_Base target)
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
        public void CastR()
        {
            if (!IsR1 && Core.GameTickCount - R.LastCastTime >= UltimateMenu.Slider("Delay"))
            {
                var rTargets = UnitManager.ValidEnemyHeroes.Where(h => R.InRange(h) && (!UltimateMenu.CheckBox("OnlyKillable") || R.IsKillable(h)) && LastRCone.IsInside(h)).ToList();
                var targets = UltimateMenu.CheckBox("NearMouse.Enabled")
                    ? rTargets.Where(
                        h => h.IsInRange(MousePos, UltimateMenu.Slider("NearMouse.Radius"))).ToList()
                    : rTargets;
                var target = TargetSelector.GetTarget(targets, DamageType.Physical);
                if (target != null)
                {
                    var pred = R.GetPrediction(target);
                    if (pred.HitChancePercent >= R.HitChancePercent && !R.WillHitYasuoWall(pred.CastPosition))
                    {
                        MyHero.Spellbook.CastSpell(SpellSlot.R, pred.CastPosition);
                    }
                }
            }
        }

        public bool TargetHaveEBuff(AIHeroClient target)
        {
            return target.TargetHaveBuff("jhinespotteddebuff");
        }

        public override float GetSpellDamage(SpellSlot slot, Obj_AI_Base target)
        {
            if (target != null)
            {
                var level = slot.GetSpellDataInst().Level;
                switch (slot)
                {
                    case SpellSlot.Q:
                        return MyHero.CalculateDamageOnUnit(target, DamageType.Physical,
                            25f * level + 35f + (0.25f + 0.05f * level) * MyHero.TotalAttackDamage + 0.6f * MyHero.FlatMagicDamageMod);
                    case SpellSlot.W:
                        return MyHero.CalculateDamageOnUnit(target, DamageType.Physical,
                            (target is AIHeroClient ? 1f : 0.65f) * (35f * level + 15f + 0.7f * MyHero.TotalAttackDamage));
                    case SpellSlot.E:
                        return MyHero.CalculateDamageOnUnit(target, DamageType.Magical,
                            (target is AIHeroClient ? 1f : 0.65f) * (60f * level - 40f + 1.2f * MyHero.TotalAttackDamage + 1f * MyHero.FlatMagicDamageMod));
                    case SpellSlot.R:
                        var shotDamage =
                            MyHero.CalculateDamageOnUnit(target, DamageType.Physical, 75f * level - 25f + 0.25f * MyHero.TotalAttackDamage) * (1f + 0.02f * (target.MaxHealth - target.Health) / target.MaxHealth * 100f);
                        var normalShotsDamage = (Stacks - 1) * shotDamage;
                        var lastShotDamage = (2f + (MyHero.HasItem(ItemId.Infinity_Edge) ? 0.5f : 0f)) * shotDamage;
                        return normalShotsDamage + lastShotDamage;
                }
            }
            return base.GetSpellDamage(slot, target);
        }

        public float GetCurrentShotDamage(Obj_AI_Base target)
        {
            var level = R.Slot.GetSpellDataInst().Level;
            return (Stacks == 1 ? (2f + (MyHero.HasItem(ItemId.Infinity_Edge) ? 0.5f : 0f)) : 1f) *
                   MyHero.CalculateDamageOnUnit(target, DamageType.Physical,
                       75f * level - 25f + 0.25f * MyHero.TotalAttackDamage) *
                   (1f + 0.02f * (target.MaxHealth - target.Health) / target.MaxHealth * 100f);
        }
    }
}
