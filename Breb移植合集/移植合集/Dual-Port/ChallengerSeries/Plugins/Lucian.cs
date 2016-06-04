using System;
using System.Collections.Generic;
using System.Linq;
using Challenger_Series.Utils;
using LeagueSharp.SDK;
using SharpDX;
using LeagueSharp.Data.Enumerations;

namespace Challenger_Series.Plugins
{
    using EloBuddy;
    using EloBuddy.SDK;
    using EloBuddy.SDK.Menu;
    using EloBuddy.SDK.Menu.Values;
    public class Lucian : CSPlugin
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

        public Lucian()
        {
            Q = new LeagueSharp.SDK.Spell(SpellSlot.Q, 675);
            Q2 = new LeagueSharp.SDK.Spell(SpellSlot.Q, 900f);
            W = new LeagueSharp.SDK.Spell(SpellSlot.W, 1000);
            E = new LeagueSharp.SDK.Spell(SpellSlot.E, 475f);
            R = new LeagueSharp.SDK.Spell(SpellSlot.R, 1400);

            Q.SetTargetted(0.25f, 1400f);
            Q2.SetSkillshot(0.5f, 50, float.MaxValue, false, SkillshotType.SkillshotLine);
            W.SetSkillshot(0.30f, 70f, 1600f, true, SkillshotType.SkillshotLine);
            R.SetSkillshot(0.2f, 110f, 2500, true, SkillshotType.SkillshotLine);
            InitMenu();
            DelayedOnUpdate += OnUpdate;
            Events.OnGapCloser += EventsOnOnGapCloser;
            Events.OnInterruptableTarget += OnInterruptableTarget;
            Orbwalker.OnPostAttack += Orbwalker_OnPostAttack;
            Orbwalker.OnPreAttack += Orbwalker_OnPreAttack;
            Spellbook.OnCastSpell += OnCastSpell;
        }

        private void Orbwalker_OnPreAttack(AttackableUnit target, Orbwalker.PreAttackArgs args)
        {
            //Anti Melee
            var possibleNearbyMeleeChampion =
                ValidTargets.FirstOrDefault(
                    e => e.IsMelee && e.ServerPosition.Distance(ObjectManager.Player.ServerPosition) < 350 && e.IsEnemy);

            if (possibleNearbyMeleeChampion.LSIsValidTarget())
            {
                if (E.IsReady() && UseEAntiMelee)
                {
                    var pos = ObjectManager.Player.ServerPosition.LSExtend(possibleNearbyMeleeChampion.ServerPosition,
                        -Misc.GiveRandomInt(250, 475));
                    if (!IsDangerousPosition(pos))
                    {
                        if (pos.IsUnderEnemyTurret() && !ObjectManager.Player.IsUnderEnemyTurret())
                        {
                            return;
                        }
                        E.Cast(pos);
                    }
                }
            }
        }

        private void Orbwalker_OnPostAttack(AttackableUnit target, EventArgs args)
        {
            //JungleClear
            if (target is Obj_AI_Base)
            {
                JungleClear(target);
            }
        }

        int GetGapclosingAngle()
        {
            var randI = Misc.GiveRandomInt(0, 100);
            if (randI > 50)
            {
                return Misc.GiveRandomInt(15, 30);
            }
            return Misc.GiveRandomInt(330, 345);
        }

        int GetHugAngle()
        {
            var randI = Misc.GiveRandomInt(0, 100);
            if (randI > 50)
            {
                return Misc.GiveRandomInt(60, 75);
            }
            return Misc.GiveRandomInt(285, 300);

        }

        private bool pressedR = false;

        private int ECastTime = 0;

        bool QLogic(AttackableUnit target)
        {
            var hero = (AIHeroClient)target;
            if (hero != null && Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo) && UseQCombo)
            {
                Q.Cast(hero);
                return true;
            }
            return false;
        }

        bool ELogic(Obj_AI_Base target)
        {
            switch (UseEMode)
            {
                case 0:
                    {
                        var pos = Deviate(ObjectManager.Player.Position.ToVector2(), target.Position.ToVector2(), this.GetHugAngle())
                                .ToVector3();
                        if (pos.IsUnderEnemyTurret() && !ObjectManager.Player.IsUnderEnemyTurret())
                        {
                            return false;
                        }
                        E.Cast(pos);
                        return true;
                    }
                case 1:
                    {
                        if (!IsDangerousPosition(Game.CursorPos))
                        {
                            var pos = ObjectManager.Player.Position.LSExtend(Game.CursorPos, Misc.GiveRandomInt(50, 100));
                            if (pos.IsUnderEnemyTurret() && !ObjectManager.Player.IsUnderEnemyTurret())
                            {
                                return false;
                            }
                            E.Cast(pos);
                        }
                        return true;
                    }
                case 2:
                    {
                        var pos = ObjectManager.Player.Position.LSExtend(target.Position, Misc.GiveRandomInt(50, 100));
                        if (pos.IsUnderEnemyTurret() && !ObjectManager.Player.IsUnderEnemyTurret())
                        {
                            return false;
                        }
                        E.Cast(pos);
                        return true;
                    }
            }
            return false;
        }

        bool WLogic(Obj_AI_Base target)
        {
            if (this.UseWCombo)
            {
                if (IgnoreWCollision && target.Distance(ObjectManager.Player) < 600)
                {
                    W.Cast(target.ServerPosition);
                    return true;
                }
                var pred = W.GetPrediction(target);
                if (target.Health < ObjectManager.Player.GetAutoAttackDamage(target) * 3)
                {
                    W.Cast(pred.UnitPosition);
                    return true;
                }
                if (pred.Hitchance >= HitChance.High)
                {
                    W.Cast(pred.UnitPosition);
                    return true;
                }
            }
            return false;
        }

        void JungleClear(AttackableUnit target)
        {
            var tg = target as Obj_AI_Base;
            if (tg != null && !HasPassive)
            {
                if (tg.IsHPBarRendered && tg.CharData.BaseSkinName.Contains("SRU") && !tg.CharData.BaseSkinName.Contains("Mini") && (tg.IsMonster || tg.IsMinion))
                {
                    if (EJg && E.IsReady())
                    {
                        var pos = Deviate(ObjectManager.Player.Position.ToVector2(), tg.Position.ToVector2(), this.GetHugAngle())
                                .ToVector3();
                        if (pos.IsUnderEnemyTurret() && !ObjectManager.Player.IsUnderEnemyTurret())
                        {
                            return;
                        }
                        E.Cast(pos);
                        return;
                    }
                    if (QJg && Q.IsReady())
                    {
                        Q.Cast(tg);
                        return;
                    }
                    if (WJg && W.IsReady())
                    {
                        var pred = W.GetPrediction(tg);
                        W.Cast(pred.UnitPosition);
                        return;
                    }
                }
            }
        }

        void QExHarass()
        {
            // no drawing turret aggro for no reason
            if (ObjectManager.Player.UnderTurret(true)) return;
            var q2tg = TargetSelector.GetTarget(Q2.Range, DamageType.Physical);
            if (q2tg != null && q2tg.IsHPBarRendered)
            {
                if (q2tg.Distance(ObjectManager.Player) > 600)
                {
                    if (!Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.None) && (!Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo) || q2tg.Health < Q.GetDamage(q2tg)))
                    {
                        var menuItem = HarassMenu["qexbl" + q2tg.CharData.BaseSkinName];
                        if (UseQExtended && ObjectManager.Player.ManaPercent > QExManaPercent && menuItem != null && !menuItem.Cast<CheckBox>().CurrentValue)
                        {
                            var QPred = Q2.GetPrediction(q2tg);
                            if (QPred.Hitchance >= HitChance.Medium)
                            {
                                var minions =
                                    GameObjects.EnemyMinions.Where(
                                        m => m.IsHPBarRendered && m.Distance(ObjectManager.Player) < Q.Range);
                                var objAiMinions = minions as IList<Obj_AI_Minion> ?? minions.ToList();
                                if (objAiMinions.Any())
                                {
                                    foreach (var minion in objAiMinions)
                                    {
                                        var QHit = new Utils.Geometry.Rectangle(
                                            ObjectManager.Player.Position,
                                            ObjectManager.Player.Position.LSExtend(minion.Position, Q2.Range),
                                            Q2.Width);
                                        if (!QPred.UnitPosition.IsOutside(QHit))
                                        {
                                            Q.Cast(minion);
                                            return;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                else if (!HasPassive)
                {
                    Q.Cast(q2tg);
                }
            }
        }

        private void OnCastSpell(Spellbook sender, SpellbookCastSpellEventArgs args)
        {
            if (sender.Owner.IsMe)
            {
                if (args.Slot == SpellSlot.R && this.BlockManualR)
                {
                    if (!pressedR && !ObjectManager.Player.IsCastingInterruptableSpell())
                    {
                        args.Process = false;
                    }
                    else
                    {
                        args.Process = true;
                        this.pressedR = false;
                    }
                }
                if (args.Slot == SpellSlot.E)
                {
                    this.ECastTime = Variables.TickCount;
                }
            }
        }

        private void OnInterruptableTarget(object sender, Events.InterruptableTargetEventArgs args)
        {
            if (E.IsReady() && this.UseEGapclose && args.DangerLevel == LeagueSharp.SDK.DangerLevel.High && args.Sender.Distance(ObjectManager.Player) < 400)
            {
                var pos = ObjectManager.Player.Position.LSExtend(args.Sender.Position, -Misc.GiveRandomInt(300, 600));
                if (pos.IsUnderEnemyTurret() && !ObjectManager.Player.IsUnderEnemyTurret())
                {
                    return;
                }
                E.Cast(ObjectManager.Player.Position.LSExtend(args.Sender.Position, -Misc.GiveRandomInt(300, 600)));
            }
        }

        private void EventsOnOnGapCloser(object sender, Events.GapCloserEventArgs args)
        {
            if (E.IsReady() && UseEGapclose && args.Sender.IsMelee && args.End.Distance(ObjectManager.Player.ServerPosition) > args.Sender.AttackRange)
            {
                var pos = ObjectManager.Player.Position.LSExtend(args.Sender.Position, -Misc.GiveRandomInt(250, 600));
                if (pos.IsUnderEnemyTurret() && !ObjectManager.Player.IsUnderEnemyTurret())
                {
                    return;
                }
                E.Cast(ObjectManager.Player.Position.LSExtend(args.Sender.Position, -Misc.GiveRandomInt(250, 600)));
            }
        }

        public override void OnUpdate(EventArgs args)
        {
            if (ObjectManager.Player.IsCastingInterruptableSpell()) return;

            if (this.SemiAutoRKey)
            {
                if (ObjectManager.Player.CountEnemyHeroesInRange(1300) > 0)
                {
                    var ultTarget = TargetSelector.GetTarget(R.Range, DamageType.Physical);
                    if (ultTarget != null && ultTarget.IsHPBarRendered)
                    {
                        this.pressedR = true;
                        var rPred = R.GetPrediction(ultTarget);
                        if (rPred.Hitchance >= HitChance.High)
                        {
                            R.Cast(rPred.UnitPosition);
                        }
                        return;
                    }
                }
                else
                {
                    R.Cast(Game.CursorPos);
                }
            }
            if (Variables.TickCount - this.ECastTime > 300)
            {
                if (!HasPassive && Orbwalker.CanMove)
                {
                    if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
                    {
                        if (E.IsReady() && this.UseEMode != 3)
                        {
                            var target = TargetSelector.GetTarget(875, DamageType.Physical);
                            if (target != null && target.IsHPBarRendered)
                            {
                                var dist = target.Distance(ObjectManager.Player);
                                if (dist > 500 && Game.CursorPos.Distance(target.Position) < ObjectManager.Player.Position.Distance(target.Position))
                                {
                                    var pos = ObjectManager.Player.ServerPosition.LSExtend(
                                        target.ServerPosition, Math.Abs(dist - 500));
                                    if (!IsDangerousPosition(pos))
                                    {
                                        if (pos.IsUnderEnemyTurret() && !ObjectManager.Player.IsUnderEnemyTurret())
                                        {
                                            return;
                                        }
                                        return;
                                    }
                                }
                                else
                                {
                                    if (ELogic(target))
                                    {
                                        return;
                                    }
                                }
                            }
                        }
                        var qtar = TargetSelector.GetTarget(Q.Range, DamageType.Physical);
                        if (qtar != null && qtar.IsHPBarRendered)
                        {
                            if (Q.IsReady())
                            {
                                if (QLogic(qtar)) return;
                            }
                            if (W.IsReady())
                            {
                                if (WLogic(qtar)) return;
                            }
                        }
                        else
                        {
                            this.QExHarass();
                        }
                    }
                    if (!Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.None) && !Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
                    {
                        this.QExHarass();
                    }
                }
            }
            if (UsePassiveOnEnemy && HasPassive)
            {
                var tg = TargetSelector.GetTarget(ObjectManager.Player.AttackRange, DamageType.Physical);
                if (tg != null && tg.IsHPBarRendered)
                {
                    Orbwalker.ForcedTarget = tg;
                    return;
                }
            }
            else
            {
                Orbwalker.ForcedTarget = null;
            }
        }

        private Menu ComboMenu;
        private bool UseQCombo;
        private bool UseWCombo;
        private bool IgnoreWCollision;
        private int UseEMode;
        private bool UseEGapclose;
        private bool UseEAntiMelee;
        private bool SemiAutoRKey;
        private bool BlockManualR;
        private Menu HarassMenu;
        private bool UseQExtended;
        private int QExManaPercent;
        private bool UseQHarass;
        private bool UsePassiveOnEnemy;
        private Menu JungleMenu;
        private bool QJg;
        private bool WJg;
        private bool EJg;
        private bool QKS;

        public void InitMenu()
        {
            ComboMenu = MainMenu.AddSubMenu("连招设置:", "Combo Settings: ");
            ComboMenu.Add("Lucianqcombo", new CheckBox("使用 Q", true));
            ComboMenu.Add("Lucianwcombo", new CheckBox("使用 W", true));
            ComboMenu.Add("Lucianignorewcollision", new CheckBox("无视 W 体积碰撞 (触发被动)", false));
            ComboMenu.Add("Lucianecombo", new ComboBox("E 模式", 0, "边上", "鼠标", "敌人", "从不"));
            ComboMenu.Add("Lucianecockblocker", new CheckBox("使用 E 防近距离英雄", true));
            ComboMenu.Add("Lucianegoham", new CheckBox("使用 E 爆发", false));
            ComboMenu.Add("Luciansemiauto", new KeyBind("半自动 R 按键", false, KeyBind.BindTypes.HoldActive, 'T'));
            ComboMenu.Add("Lucianblockmanualr", new CheckBox("屏蔽手动 R", true));
            ComboMenu.Add("Lucianqks", new CheckBox("使用 Q 抢头", true));

            HarassMenu = MainMenu.AddSubMenu("骚扰设置:", "Harass Settings: ");
            HarassMenu.Add("Lucianqextended", new CheckBox("使用延长 Q", true));
            HarassMenu.Add("Lucianqexmanapercent", new Slider("使用延长Q 如果蓝量 > %", 75, 0, 100));
            HarassMenu.AddGroupLabel("延长 Q 黑名单: ");
            foreach (var ally in ObjectManager.Get<AIHeroClient>().Where(h => h.IsEnemy))
            {
                var championName = ally.CharData.BaseSkinName;
                HarassMenu.Add("qexbl" + championName, new CheckBox(championName, false));
            }
            HarassMenu.Add("Lucianqharass", new CheckBox("使用 Q 骚扰", true));
            HarassMenu.Add("Lucianpassivefocus", new CheckBox("对英雄使用被动", true));

            JungleMenu = MainMenu.AddSubMenu("清野设置:", "Jungle Settings: ");
            JungleMenu.Add("Lucianqjungle", new CheckBox("使用 Q", true));
            JungleMenu.Add("Lucianwjungle", new CheckBox("使用 W", true));
            JungleMenu.Add("Lucianejungle", new CheckBox("使用 E", true));

            UseQCombo = getCheckBoxItem(ComboMenu, "Lucianqcombo");
            UseWCombo = getCheckBoxItem(ComboMenu, "Lucianwcombo");
            IgnoreWCollision = getCheckBoxItem(ComboMenu, "Lucianignorewcollision");
            UseEMode = getBoxItem(ComboMenu, "Lucianecombo");
            UseEAntiMelee = getCheckBoxItem(ComboMenu, "Lucianecockblocker");
            UseEGapclose = getCheckBoxItem(ComboMenu, "Lucianegoham");
            SemiAutoRKey = getKeyBindItem(ComboMenu, "Luciansemiauto");
            BlockManualR = getCheckBoxItem(ComboMenu, "Lucianblockmanualr");
            QKS = getCheckBoxItem(ComboMenu, "Lucianqks");
            UseQExtended = getCheckBoxItem(HarassMenu, "Lucianqextended");
            QExManaPercent = getSliderItem(HarassMenu, "Lucianqexmanapercent");
            UseQHarass = getCheckBoxItem(HarassMenu, "Lucianqharass");
            UsePassiveOnEnemy = getCheckBoxItem(HarassMenu, "Lucianpassivefocus");
            QJg = getCheckBoxItem(JungleMenu, "Lucianqjungle");
            WJg = getCheckBoxItem(JungleMenu, "Lucianwjungle");
            EJg = getCheckBoxItem(JungleMenu, "Lucianejungle");
        }

        public static Vector2 Deviate(Vector2 point1, Vector2 point2, double angle)
        {
            angle *= Math.PI / 180.0;
            Vector2 temp = Vector2.Subtract(point2, point1);
            Vector2 result = new Vector2(0);
            result.X = (float)(temp.X * Math.Cos(angle) - temp.Y * Math.Sin(angle)) / 4;
            result.Y = (float)(temp.X * Math.Sin(angle) + temp.Y * Math.Cos(angle)) / 4;
            result = Vector2.Add(result, point1);
            return result;
        }

        private bool IsDangerousPosition(Vector3 pos)
        {
            return GameObjects.EnemyHeroes.Any(
                e => e.IsHPBarRendered && e.IsMelee &&
                     (e.Distance(pos) < 375)) ||
                   (pos.UnderTurret(true) && !ObjectManager.Player.UnderTurret(true));
        }

        public bool HasPassive => ObjectManager.Player.HasBuff("LucianPassiveBuff");
    }
}
