using System.Collections.Generic;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using KoreanAIO.Managers;
using KoreanAIO.Model;

namespace KoreanAIO.Champions
{
    public class Cassiopeia : ChampionBase
    {
        public Dictionary<int, bool> CachedPoisoned = new Dictionary<int, bool>();

        public Cassiopeia()
        {
            Q = new SpellBase(SpellSlot.Q, SpellType.Circular, 750)
            {
                Width = 130,
                CastDelay = 400
            };
            W = new SpellBase(SpellSlot.W, SpellType.Circular, 800)
            {
                Width = 180,
                CastDelay = 250,
                Speed = 3000
            };
            E = new SpellBase(SpellSlot.E, SpellType.Targeted, 700)
            {
                Speed = 1900,
                CastDelay = 125
            };
            R = new SpellBase(SpellSlot.R, SpellType.Cone, 825)
            {
                Width = 80,
                CastDelay = 500
            };

            Spellbook.OnCastSpell += delegate (Spellbook sender, SpellbookCastSpellEventArgs args)
            {
                if (sender.Owner.IsMe)
                {
                    switch (args.Slot)
                    {
                        case SpellSlot.Q:
                            Q.LastSentTime = Core.GameTickCount;
                            Q.LastEndPosition = args.EndPosition;
                            break;
                        case SpellSlot.W:
                            W.LastSentTime = Core.GameTickCount;
                            W.LastEndPosition = args.EndPosition;
                            break;
                    }
                }
            };

            Obj_AI_Base.OnProcessSpellCast += delegate (Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
            {
                if (sender.IsMe)
                {
                    switch (args.Slot)
                    {
                        case SpellSlot.Q:
                            Q.LastCastTime = Core.GameTickCount;
                            Q.LastEndPosition = args.End;
                            break;
                        case SpellSlot.W:
                            W.LastCastTime = Core.GameTickCount;
                            W.LastEndPosition = args.End;
                            break;
                    }
                }
            };

            MenuManager.AddSubMenu("Keys");
            {
                KeysMenu.AddValue("AssistedUltimate",
                    new KeyBind("Assisted Ultimate", false, KeyBind.BindTypes.HoldActive, 'T'));
                ToggleManager.RegisterToggle(
                    KeysMenu.AddValue("HarassToggle",
                        new KeyBind("Harass Toggle", false, KeyBind.BindTypes.PressToggle, 'K')),
                    delegate
                    {
                        if (!ModeManager.Combo)
                        {
                            Harass(HarassMenu);
                        }
                    });
                ToggleManager.RegisterToggle(
                    KeysMenu.AddValue("LastHitToggle",
                        new KeyBind("LastHit Toggle", false, KeyBind.BindTypes.PressToggle, 'L')),
                    delegate
                    {
                        if (!ModeManager.Combo)
                        {
                            LastHit(ClearMenu);
                        }
                    });
            }
            Q.AddConfigurableHitChancePercent();
            W.AddConfigurableHitChancePercent();
            R.AddConfigurableHitChancePercent();

            MenuManager.AddSubMenu("Combo");
            {
                ComboMenu.AddValue("Q", new CheckBox("Use Q"));
                ComboMenu.AddValue("W", new CheckBox("Use W"));
                ComboMenu.AddStringList("E", "Use E", new[] { "Never", "If Poisoned", "Always" }, 1);
                ComboMenu.AddValue("R", new Slider("Use R if hit >= {0}", 3, 0, 5));
            }

            MenuManager.AddSubMenu("Harass");
            {
                HarassMenu.AddValue("Q", new CheckBox("Use Q"));
                HarassMenu.AddValue("W", new CheckBox("Use W"));
                HarassMenu.AddStringList("E", "Use E", new[] { "Never", "If Poisoned", "Always" }, 1);
                HarassMenu.AddValue("ManaPercent", new Slider("Minimum Mana Percent", 25));
            }

            MenuManager.AddSubMenu("Clear");
            {
                ClearMenu.AddValue("LaneClear", new GroupLabel("LaneClear"));
                {
                    ClearMenu.AddValue("LaneClear.Q", new Slider("Use Q if hit >= {0}", 2, 0, 10));
                    ClearMenu.AddValue("LaneClear.W", new Slider("Use W if hit >= {0}", 3, 0, 10));
                    ClearMenu.AddStringList("LaneClear.E", "Use E", new[] { "Never", "If Poisoned", "Always" }, 1);
                    ClearMenu.AddValue("LaneClear.ManaPercent", new Slider("Minimum Mana Percent", 50));
                }
                ClearMenu.AddValue("LastHit", new GroupLabel("LastHit"));
                {
                    ClearMenu.AddStringList("LastHit.E", "Use E", new[] { "Never", "If Poisoned", "Always" }, 1);
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
                KillStealMenu.AddValue("R", new CheckBox("Use R", false));
            }
            MenuManager.AddSubMenu("Automatic");
            {
                AutomaticMenu.AddValue("R", new CheckBox("Use R on channeling spells"));
            }
            MenuManager.AddDrawingsMenu();
            {
                Q.AddDrawings();
                W.AddDrawings(false);
                E.AddDrawings();
                R.AddDrawings();
                DrawingsMenu.AddValue("Toggles", new CheckBox("Draw toggles status"));
            }
        }


        protected override void PermaActive()
        {
            CachedPoisoned.Clear();
            Range = Q.Range + Q.Width;
            Target = TargetSelector.GetTarget(UnitManager.ValidEnemyHeroesInRange, DamageType.Magical);
            if (ModeManager.Combo && MyHero.InAutoAttackRange(Target) && E.IsLearned && MyHero.Mana >= E.Mana &&
                (E.IsReady || E.Cooldown < 1) &&
                IsPoisoned(Target))
            {
                Orbwalker.DisableAttacking = true;
            }
            else
            {
                Orbwalker.DisableAttacking = false;
            }
            if (KeysMenu.KeyBind("AssistedUltimate"))
            {
                var enemyNear =
                    UnitManager.ValidEnemyHeroesInRange.OrderBy(client => client.Distance(MousePos, true))
                        .FirstOrDefault();
                if (enemyNear != null)
                {
                    R.Cast(enemyNear);
                }
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
                    Q.Cast(enemy);
                }
                if (menu.CheckBox("W") && (result.W || W.IsKillable(enemy)))
                {
                    W.Cast(enemy);
                }
                if (menu.CheckBox("E") && (result.E || E.IsKillable(enemy)))
                {
                    E.Cast(enemy);
                }
                if (menu.CheckBox("R") && (result.R || R.IsKillable(enemy)))
                {
                    CastR(enemy);
                }
            }
            base.KillSteal(menu);
        }

        protected override void Combo(Menu menu)
        {
            if (Target != null)
            {
                if (menu.Slider("E") > 0)
                {
                    switch (menu.Slider("E"))
                    {
                        case 1:
                            CastE(Target);
                            break;
                        case 2:
                            E.Cast(Target);
                            break;
                    }
                }
                if (menu.CheckBox("Q"))
                {
                    CastQ(Target);
                }
                if (menu.CheckBox("W"))
                {
                    CastW(Target);
                }
                if (menu.Slider("R") > 0)
                {
                    if (MyHero.CountEnemiesInRange(R.Range) >= menu.Slider("R"))
                    {
                        R.Cast(Target);
                    }
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
                    if (menu.CheckBox("Q"))
                    {
                        CastQ(Target);
                    }
                    if (menu.CheckBox("W"))
                    {
                        CastW(Target);
                    }
                    if (menu.Slider("E") > 0)
                    {
                        switch (menu.Slider("E"))
                        {
                            case 1:
                                CastE(Target);
                                break;
                            case 2:
                                E.Cast(Target);
                                break;
                        }
                    }
                }
            }
            base.Harass(menu);
        }

        protected override void LaneClear(Menu menu)
        {
            if (MyHero.ManaPercent >= menu.Slider("LaneClear.ManaPercent"))
            {
                Q.LaneClear(true, menu.Slider("LaneClear.Q"));
                W.LaneClear(true, menu.Slider("LaneClear.W"));
                var minion = E.LaneClear(false);
                if (minion != null)
                {
                    switch (menu.Slider("LaneClear.E"))
                    {
                        case 1:
                            CastE(minion);
                            break;
                        case 2:
                            E.Cast(minion);
                            break;
                    }
                }
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
                    var minion = E.JungleClear(false);
                    if (minion != null)
                    {
                        CastE(minion);
                    }
                }
            }
            base.JungleClear(menu);
        }

        protected override void LastHit(Menu menu)
        {
            if (menu.Slider("LastHit.E") > 0)
            {
                var minion = E.LastHit(LastHitType.Always, false).FirstOrDefault();
                if (minion != null)
                {
                    switch (menu.Slider("LastHit.E"))
                    {
                        case 1:
                            CastE(minion);
                            break;
                        case 2:
                            E.Cast(minion);
                            break;
                    }
                }
            }
            base.LastHit(menu);
        }

        public void CastQ(Obj_AI_Base target)
        {
            if (Q.IsReady && target != null)
            {
                if (IsPoisoned(target) && E.IsReady)
                {
                    return;
                }
                if (W.LastSentTime > 0)
                {
                    var arrivalTime = W.GetArrivalTime(W.LastEndPosition);
                    if (Core.GameTickCount - W.LastSentTime <= arrivalTime)
                    {
                        return;
                    }
                    if (W.LastCastTime > 0 && Core.GameTickCount - W.LastCastTime <= arrivalTime)
                    {
                        return;
                    }
                }
                Q.Cast(target);
            }
        }

        public void CastW(Obj_AI_Base target)
        {
            if (W.IsReady && target != null)
            {
                if (IsPoisoned(target) && E.IsReady)
                {
                    return;
                }
                if (Q.LastSentTime > 0)
                {
                    var arrivalTime = Q.GetArrivalTime(Q.LastEndPosition);
                    if (Core.GameTickCount - Q.LastSentTime <= arrivalTime)
                    {
                        return;
                    }
                    if (Q.LastCastTime > 0 && Core.GameTickCount - Q.LastCastTime <= arrivalTime)
                    {
                        return;
                    }
                }
                W.Cast(target);
            }
        }

        public void CastE(Obj_AI_Base target)
        {
            if (E.IsReady && target != null)
            {
                var canCast = true;
                if (!IsPoisoned(target))
                {
                    canCast = false;
                    var timeToArriveE = E.GetArrivalTime(target);
                    var timeToArriveW = W.GetArrivalTime(target) - (Core.GameTickCount - W.LastCastTime);
                    var timeToArriveQ = Q.GetArrivalTime(target) - (Core.GameTickCount - Q.LastCastTime);
                    if (timeToArriveW >= 0)
                    {
                        if (timeToArriveE >= timeToArriveW)
                        {
                            var pred = W.GetPrediction(target);
                            if (pred.HitChancePercent >= W.HitChancePercent &&
                                W.LastEndPosition.Distance(pred.CastPosition, true) <=
                                (target.BoundingRadius / 2f + W.Radius).Pow())
                            {
                                canCast = true;
                            }
                        }
                    }
                    if (timeToArriveQ >= 0)
                    {
                        if (timeToArriveE >= timeToArriveQ)
                        {
                            var pred = Q.GetPrediction(target);
                            if (pred.HitChancePercent >= Q.HitChancePercent &&
                                Q.LastEndPosition.Distance(pred.CastPosition, true) <=
                                (target.BoundingRadius / 2f + Q.Radius).Pow())
                            {
                                canCast = true;
                            }
                        }
                    }
                }
                if (canCast)
                {
                    E.Cast(target);
                }
            }
        }

        public void CastR(AIHeroClient target)
        {
            if (R.IsReady && target != null && IsFacing(target))
            {
                R.Cast(target);
            }
        }

        public bool IsFacing(AIHeroClient target)
        {
            return R.GetPrediction(target).CastPosition.Distance(MyHero, true) <= target.GetDistanceSqr(MyHero);
        }

        public bool IsPoisoned(Obj_AI_Base target)
        {
            if (target == null)
            {
                return false;
            }
            if (!CachedPoisoned.ContainsKey(target.NetworkId))
            {
                CachedPoisoned[target.NetworkId] = false;
                if (target.HasBuffOfType(BuffType.Poison))
                {
                    var bestEndTime =
                        target.Buffs.Where(b => b.IsActive && b.Type == BuffType.Poison && b.EndTime > 0)
                            .OrderByDescending(b => b.EndTime)
                            .FirstOrDefault();
                    if (bestEndTime != null && bestEndTime.EndTime - Game.Time >= E.GetArrivalTime(target) / 1000f)
                    {
                        CachedPoisoned[target.NetworkId] = true;
                    }
                }
            }
            return CachedPoisoned[target.NetworkId];
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
                            45f * level + 30f + 0.45f * MyHero.FlatMagicDamageMod);
                    case SpellSlot.W:
                        return MyHero.CalculateDamageOnUnit(target, DamageType.Magical,
                            15f * level + 5f + 0.15f * MyHero.FlatMagicDamageMod) * 2.5f;
                    case SpellSlot.E:
                        return MyHero.CalculateDamageOnUnit(target, DamageType.Magical,
                            4f * level + 48f + 0.1f * MyHero.FlatMagicDamageMod + (IsPoisoned(target) ? (-20 + 30 * level + 0.35f * MyHero.FlatMagicDamageMod) : 0f));
                    case SpellSlot.R:
                        return MyHero.CalculateDamageOnUnit(target, DamageType.Magical,
                            100f * level + 50f + 0.5f * MyHero.FlatMagicDamageMod);
                }
            }
            return base.GetSpellDamage(slot, target);
        }
    }
}