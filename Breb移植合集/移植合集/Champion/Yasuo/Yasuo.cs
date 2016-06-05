namespace Valvrave_Sharp.Plugin
{
    #region

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using EloBuddy.SDK.Menu.Values;
    using EloBuddy.SDK.Menu;
    using EloBuddy.SDK;
    using LeagueSharp;
    using LeagueSharp.SDK;
    using LeagueSharp.SDK.Core.Utils;
    using LeagueSharp.SDK.Modes;
    using EloBuddy;
    using SharpDX;

    using Valvrave_Sharp.Core;
    using Valvrave_Sharp.Evade;

    using Color = System.Drawing.Color;
    using Skillshot = Valvrave_Sharp.Evade.Skillshot;
    using LeagueSharp.Data.Enumerations;
    #endregion

    internal class Yasuo : Program
    {
        #region Constants

        private const float QDelay = 0.38f, Q2Delay = 0.35f, QDelays = 0.19f, Q2Delays = 0.285f;

        private const int RWidth = 400;

        #endregion

        #region Static Fields

        private static int cDash;

        public static Menu config = _MainMenu;

        private static bool haveQ3;

        private static bool isDash;

        private static int lastE;

        private static Vector2 posDash;

        public static Menu comboMenu, hybridMenu, lcMenu, lhMenu, ksMenu, fleeMenu, drawMenu, miscMenu;

        #endregion

        #region Constructors and Destructors

        public Yasuo()
        {

            Q = new LeagueSharp.SDK.Spell(SpellSlot.Q, 505).SetSkillshot(QDelay, 20, float.MaxValue, false, SkillshotType.SkillshotLine);
            Q2 = new LeagueSharp.SDK.Spell(Q.Slot, 1100).SetSkillshot(Q2Delay, 90, 1200, true, Q.Type);
            Q3 = new LeagueSharp.SDK.Spell(Q.Slot, 250).SetTargetted(0.005f, float.MaxValue);
            W = new LeagueSharp.SDK.Spell(SpellSlot.W, 400);
            E = new LeagueSharp.SDK.Spell(SpellSlot.E, 475).SetTargetted(0, 1050);
            E2 = new LeagueSharp.SDK.Spell(E.Slot).SetTargetted(E.Delay + Q3.Delay, E.Speed);
            R = new LeagueSharp.SDK.Spell(SpellSlot.R, 1200);
            Q.DamageType = Q2.DamageType = R.DamageType = DamageType.Physical;
            E.DamageType = DamageType.Magical;
            Q.MinHitChance = Q2.MinHitChance = HitChance.VeryHigh;
            E.CastCondition += () => !posDash.IsValid();

            if (YasuoPro.YasuoMenu.ComboM != null)
            {
                if (EntityManager.Heroes.Enemies.Any())
                {
                    Evade.Init();
                }
                Evade.Evading += Evading;
                Evade.TryEvading += TryEvading;
                return;
            }

            comboMenu = config.AddSubMenu("Combo", "Combo");
            comboMenu.AddGroupLabel("Q: Always On");
            comboMenu.AddGroupLabel("E Gap Settings");
            comboMenu.Add("EGap", new CheckBox("Use E"));
            comboMenu.Add("EMode", new ComboBox("Follow Mode", 0, "Enemy", "Mouse"));
            comboMenu.Add("ETower", new CheckBox("Under Tower", false));
            comboMenu.Add("EStackQ", new CheckBox("Stack Q While Gap", false));
            comboMenu.AddGroupLabel("R Settings");
            comboMenu.Add("R", new KeyBind("Use R", false, KeyBind.BindTypes.PressToggle, 'X'));
            comboMenu.Add("RDelay", new CheckBox("Delay Cast"));
            comboMenu.Add("RHpU", new Slider("If Enemies Hp < (%)", 60));
            comboMenu.Add("RCountA", new Slider("Or Count >=", 2, 1, 5));

            hybridMenu = config.AddSubMenu("Hybrid", "Hybrid");
            hybridMenu.AddGroupLabel("Q: Always On");
            hybridMenu.Add("Q3", new CheckBox("Also Q3"));
            hybridMenu.Add("QLastHit", new CheckBox("Last Hit (Q1/2)"));
            hybridMenu.AddGroupLabel("Auto Q Settings");
            hybridMenu.Add("AutoQ", new KeyBind("KeyBind", false, KeyBind.BindTypes.PressToggle, 'T'));
            hybridMenu.Add("AutoQ3", new CheckBox("Also Q3", false));

            lcMenu = config.AddSubMenu("LaneClear", "Lane Clear");
            lcMenu.AddGroupLabel("Q Settings");
            lcMenu.Add("Q", new CheckBox("Use Q"));
            lcMenu.Add("Q3", new CheckBox("Also Q3", false));
            lcMenu.AddGroupLabel("E Settings");
            lcMenu.Add("E", new CheckBox("Use E"));
            lcMenu.Add("ELastHit", new CheckBox("Last Hit Only", false));
            lcMenu.Add("ETower", new CheckBox("Under Tower", false));

            lhMenu = config.AddSubMenu("LastHit", "Last Hit");
            lhMenu.AddGroupLabel("Q Settings");
            lhMenu.Add("Q", new CheckBox("Use Q"));
            lhMenu.Add("Q3", new CheckBox("Also Q3", false));
            lhMenu.AddGroupLabel("E Settings");
            lhMenu.Add("E", new CheckBox("Use E"));
            lhMenu.Add("ETower", new CheckBox("Under Tower", false));

            ksMenu = config.AddSubMenu("KillSteal", "Kill Steal");
            ksMenu.Add("Q", new CheckBox("Use Q"));
            ksMenu.Add("E", new CheckBox("Use E"));
            ksMenu.Add("R", new CheckBox("Use R"));
            foreach (var enemy in ObjectManager.Get<AIHeroClient>().Where(o => o.IsEnemy))
            {
                ksMenu.Add("RCast" + enemy.NetworkId, new CheckBox("Cast On " + enemy.ChampionName, false));
            }

            fleeMenu = config.AddSubMenu("Flee", "Flee");
            fleeMenu.Add("E", new KeyBind("Use E", false, KeyBind.BindTypes.HoldActive, 'C'));
            fleeMenu.Add("Q", new CheckBox("Stack Q While Dash"));

            if (EntityManager.Heroes.Enemies.Any())
            {
                Evade.Init();
            }

            drawMenu = config.AddSubMenu("Draw", "Draw");
            drawMenu.Add("Q", new CheckBox("Q Range", false));
            drawMenu.Add("E", new CheckBox("E Range", false));
            drawMenu.Add("R", new CheckBox("R Range", false));
            drawMenu.Add("UseR", new CheckBox("R In Combo Status"));
            drawMenu.Add("StackQ", new CheckBox("Auto Stack Q Status"));

            miscMenu = config.AddSubMenu("Misc", "Misc");
            miscMenu.Add("StackQ", new KeyBind("Auto Stack Q", false, KeyBind.BindTypes.PressToggle, 'Z'));

            Evade.Evading += Evading;
            Evade.TryEvading += TryEvading;
            Game.OnUpdate += OnUpdate;
            Drawing.OnDraw += OnDraw;
            Game.OnUpdate += args =>
                {
                    if (Player.IsDead)
                    {
                        if (isDash)
                        {
                           isDash = false;
                           posDash = new Vector2();
                        }
                        return;
                    }
                    if (isDash && !Player.IsDashing())
                    {
                        isDash = false;
                        DelayAction.Add(50, () => posDash = new Vector2());
                    }
                    Q.Delay = GetQDelay(false);
                    Q2.Delay = GetQDelay(true);
                    E.Speed = E2.Speed = 1045 + (Player.MoveSpeed - 345);
                };
            Orbwalker.OnPostAttack += (sender, args) =>
                {
                    if (!Q.IsReady() || haveQ3 || (!Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear) || !Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear)) || Orbwalker.LastTarget is AIHeroClient || Orbwalker.LastTarget is Obj_AI_Minion)
                    {
                        return;
                    }
                    if (Q.GetTarget(50) != null || Common.ListMinions().Count(i => i.IsValidTarget(Q.Range + 50)) > 0)
                    {
                        return;
                    }
                    if ((Items.HasItem((int)ItemId.Sheen) && Items.CanUseItem((int)ItemId.Sheen)) || (Items.HasItem((int)ItemId.Trinity_Force) && Items.CanUseItem((int)ItemId.Trinity_Force)))
                    {
                        Q.Cast(Game.CursorPos);
                    }
                };
            Events.OnDash += (sender, args) =>
                {
                    if (!args.Unit.IsMe)
                    {
                        return;
                    }
                    isDash = true;
                    posDash = args.EndPos;
                };
            Obj_AI_Base.OnBuffGain += (sender, args) =>
                {
                    if (!sender.IsMe)
                    {
                        return;
                    }
                    switch (args.Buff.DisplayName)
                    {
                        case "YasuoQ3W":
                            haveQ3 = true;
                            break;
                        case "YasuoDashScalar":
                            cDash = 1;
                            break;
                    }
                };
            Obj_AI_Base.OnBuffUpdate += (sender, args) =>
            {
                if (!sender.IsMe || !args.Buff.Caster.IsMe || args.Buff.DisplayName != "YasuoDashScalar")
                {
                    return;
                }
                cDash = 2;
            };
            Obj_AI_Base.OnBuffLose += (sender, args) =>
                {
                    if (!sender.IsMe)
                    {
                        return;
                    }
                    switch (args.Buff.DisplayName)
                    {
                        case "YasuoQ3W":
                            haveQ3 = false;
                            break;
                        case "YasuoDashScalar":
                            cDash = 0;
                            break;
                    }
                };
            Obj_AI_Base.OnSpellCast += (sender, args) =>
            {
                if (!sender.IsMe || !args.SData.Name.StartsWith("YasuoQ") || args.SData.Name.EndsWith("Mis"))
                {
                    return;
                }
                if (args.SData.Name.EndsWith("W"))
                {
                    DelayAction.Add(50, Orbwalker.ResetAutoAttack);
                }
                EloBuddy.Player.IssueOrder(GameObjectOrder.AttackTo, Player.ServerPosition.LSExtend(args.End, -(Player.BoundingRadius * 2)));
            };
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

        #endregion

        #region Properties

        private static bool CanCastQCir => posDash.IsValid() && posDash.DistanceToPlayer() < 150;

        private static List<Obj_AI_Base> GetQCirObj => Common.ListEnemies(true).Where(i => i.LSIsValidTarget() && Q3.GetPredPosition(i).Distance(posDash) < Q3.Range).ToList();

        private static List<Obj_AI_Base> GetQCirTarget => EntityManager.Heroes.Enemies.Where(i => Q3.GetPredPosition(i).Distance(posDash) < Q3.Range && Q3.IsInRange(i) && i.LSIsValidTarget()).Cast<Obj_AI_Base>().ToList();

        private static List<AIHeroClient> GetRTarget => EntityManager.Heroes.Enemies.Where(i => i.IsValidTarget(R.Range) && HaveR(i)).ToList();

        private static bool IsDashing => (lastE > 0 && Variables.TickCount - lastE <= 100) || Player.IsDashing() || posDash.IsValid();

        #endregion

        #region Methods

        private static void AutoQ()
        {
            if (!getKeyBindItem(hybridMenu, "AutoQ") || !Q.IsReady() || IsDashing || (haveQ3 && !getCheckBoxItem(hybridMenu, "AutoQ3")))
            {
                return;
            }
            if (!haveQ3)
            {
                Q.CastingBestTarget(true);
            }
            else
            {
                CastQ3();
            }
        }

        private static bool CanCastDelayR(AIHeroClient target)
        {
            if (target.HasBuffOfType(BuffType.Knockback))
            {
                return true;
            }
            var buff = target.Buffs.FirstOrDefault(i => i.IsValid && i.Type == BuffType.Knockup);
            if (buff == null)
            {
                return false;
            }
            var dur = buff.EndTime - buff.StartTime;
            return buff.EndTime - Game.Time <= (dur <= 0.75 ? 0.3 : 0.235) * dur;
        }

        private static bool CanDash(
            Obj_AI_Base target,
            bool inQCir = false,
            bool underTower = true,
            Vector3 pos = new Vector3())
        {
            if (HaveE(target))
            {
                return false;
            }
            if (!pos.IsValid())
            {
                pos = target.ServerPosition;
            }
            var posAfterE = GetPosAfterDash(target);
            return (underTower || !posAfterE.IsUnderEnemyTurret())
                   && posAfterE.Distance(pos) < (inQCir ? Q3.Range : pos.DistanceToPlayer())
                   && Evade.IsSafePoint(posAfterE.ToVector2()).IsSafe;
        }

        private static bool CastQ3()
        {
            var targets = EntityManager.Heroes.Enemies.Where(i => Q2.IsInRange(i) && i.LSIsValidTarget()).ToList();
            if (targets.Count == 0)
            {
                return false;
            }
            var posCast = new Vector3();
            foreach (var pred in
                targets.Select(i => Q2.GetPrediction(i, true, -1, LeagueSharp.SDK.CollisionableObjects.YasuoWall))
                    .Where(
                        i =>
                        i.Hitchance >= Q2.MinHitChance || (i.Hitchance >= HitChance.High && i.AoeTargetsHitCount > 1))
                    .OrderByDescending(i => i.AoeTargetsHitCount))
            {
                posCast = pred.CastPosition;
                break;
            }
            return posCast.IsValid() && Q2.Cast(posCast);
        }

        private static bool CastQCir(List<Obj_AI_Base> obj)
        {
            if (obj.Count == 0)
            {
                return false;
            }
            var target = obj.FirstOrDefault();
            return target != null && Q3.Cast(!haveQ3 ? Q.GetPredPosition(target) : Q2.GetPredPosition(target));
        }

        private static void Combo()
        {
            if (R.IsReady() && getKeyBindItem(comboMenu, "R"))
            {
                var targetR = GetRTarget;
                if (targetR.Count > 0)
                {
                    var targets = (from enemy in targetR let nearEnemy = EntityManager.Heroes.Enemies.Where(i => i.IsValidTarget(RWidth, true, enemy.ServerPosition) && HaveR(i)).ToList() where (nearEnemy.Count > 1 && enemy.Health + enemy.AttackShield <= R.GetDamage(enemy)) || nearEnemy.Sum(i => i.HealthPercent) / nearEnemy.Count <= getSliderItem(comboMenu, "RHpU") || nearEnemy.Count >= getSliderItem(comboMenu, "RCountA") orderby nearEnemy.Count descending select enemy).ToList();
                    if (getCheckBoxItem(comboMenu, "RDelay"))
                    {
                        targets = targets.Where(CanCastDelayR).ToList();
                    }
                    if (targets.Count > 0)
                    {
                        var target = targets.MaxOrDefault(i => new Priority().GetPriority(i));
                        if (target != null && R.CastOnUnit(target))
                        {
                            return;
                        }
                    }
                }
            }

            if (getCheckBoxItem(comboMenu, "EGap") && E.IsReady())
            {
                var underTower = getCheckBoxItem(comboMenu, "ETower");

                if (getBoxItem(comboMenu, "EMode") == 0)
                {
                    var listDashObj = GetDashObj(underTower);

                    var target = E.GetTarget(Q3.Range);
                    if (target != null && haveQ3 && Q.IsReady(50))
                    {
                        var nearObj = GetBestObj(listDashObj, target, true);
                        if (nearObj != null && (GetPosAfterDash(nearObj).CountEnemyHeroesInRange(Q3.Range) > 1 || Player.CountEnemyHeroesInRange(Q.Range + E.Range / 2) == 1) && E.CastOnUnit(nearObj))
                        {
                            lastE = Variables.TickCount;
                            return;
                        }
                    }

                    target = E.GetTarget();
                    if (target != null && ((cDash > 0 && CanDash(target, false, underTower)) || (haveQ3 && Q.IsReady(50) && CanDash(target, true, underTower))) && E.CastOnUnit(target))
                    {
                        lastE = Variables.TickCount;
                        return;
                    }

                    target = Q.GetTarget(100) ?? Q2.GetTarget();

                    if (target != null && (!Player.Spellbook.IsAutoAttacking || Player.HealthPercent < 40))
                    {
                        var nearObj = GetBestObj(listDashObj, target);
                        var canDash = cDash == 0 && nearObj != null && !HaveE(target);
                        if (Q.IsReady(50))
                        {
                            var nearObjQ3 = GetBestObj(listDashObj, target, true);
                            if (nearObjQ3 != null)
                            {
                                nearObj = nearObjQ3;
                                canDash = true;
                            }
                        }
                        if (!canDash && target.DistanceToPlayer() > target.GetRealAutoAttackRange() * 0.7)
                        {
                            canDash = true;
                        }
                        if (canDash)
                        {
                            if (nearObj == null && E.IsInRange(target) && CanDash(target, false, underTower))
                            {
                                nearObj = target;
                            }
                            if (nearObj != null && E.CastOnUnit(nearObj))
                            {
                                lastE = Variables.TickCount;
                                return;
                            }
                        }
                    }
                }
                else
                {
                    var target = Orbwalker.LastTarget;
                    if (target == null || Player.Distance(target) > target.GetRealAutoAttackRange() * 0.4)
                    {
                        var obj = GetBestObjToMouse(underTower);
                        if (obj != null && E.CastOnUnit(obj))
                        {
                            lastE = Variables.TickCount;
                            return;
                        }
                    }
                }
            }

            if (Q.IsReady())
            {
                if (IsDashing)
                {
                    if (CanCastQCir)
                    {
                        if (CastQCir(GetQCirTarget))
                        {
                            return;
                        }
                        if (!haveQ3 && getCheckBoxItem(comboMenu, "EGap") && getCheckBoxItem(comboMenu, "EStackQ") && Player.CountEnemyHeroesInRange(700) == 0 && CastQCir(GetQCirObj))
                        {
                            return;
                        }
                    }
                }
                else if (!haveQ3 ? Q.CastingBestTarget(true).IsCasted() : CastQ3())
                {
                    return;
                }
            }
        }

        private static void Evading(Obj_AI_Base sender)
        {
            var yasuoW = EvadeSpellDatabase.Spells.FirstOrDefault(i => i.Enable && i.IsReady && i.Slot == SpellSlot.W);
            if (yasuoW == null)
            {
                return;
            }
            var skillshot = Evade.SkillshotAboutToHit(sender, yasuoW.Delay - Evade.getSliderItem("Yasuo WDelay"), true).OrderByDescending(i => i.DangerLevel).FirstOrDefault(i => i.DangerLevel >= yasuoW.DangerLevel);
            if (skillshot != null)
            {
                W.Cast(sender.ServerPosition.LSExtend((Vector3)skillshot.Start, 100));

            }
        }

        private static void Flee()
        {
            if (getCheckBoxItem(fleeMenu, "Q") && Q.IsReady() && !haveQ3 && IsDashing && CanCastQCir && CastQCir(GetQCirObj))
            {
                return;
            }
            if (!E.IsReady())
            {
                return;
            }
            var obj = GetBestObjToMouse();
            if (obj != null && E.CastOnUnit(obj))
            {
                lastE = Variables.TickCount;
            }
        }

        private static Obj_AI_Base GetBestObj(List<Obj_AI_Base> obj, AIHeroClient target, bool inQCir = false)
        {
            obj.RemoveAll(i => i.Compare(target));
            if (obj.Count == 0)
            {
                return null;
            }
            var pos = E.GetPredPosition(target, true);
            return obj.Where(i => CanDash(i, inQCir, true, pos)).MinOrDefault(i => GetPosAfterDash(i).Distance(pos));
        }

        private static Obj_AI_Base GetBestObjToMouse(bool underTower = true)
        {
            var pos = Game.CursorPos;
            return
                GetDashObj(underTower)
                    .Where(i => CanDash(i, false, true, pos))
                    .MinOrDefault(i => GetPosAfterDash(i).Distance(pos));
        }

        private static List<Obj_AI_Base> GetDashObj(bool underTower = false)
        {
            return
                Common.ListEnemies()
                    .Where(i => i.IsValidTarget(E.Range) && (underTower || !GetPosAfterDash(i).IsUnderEnemyTurret()))
                    .ToList();
        }

        private static double GetEDmg(Obj_AI_Base target)
        {
            return E.GetDamage(target) + E.GetDamage(target, DamageStage.Buff) - 5;
        }

        private static Vector3 GetPosAfterDash(Obj_AI_Base target)
        {
            return Player.ServerPosition.LSExtend(target.ServerPosition, E.Range);
        }

        private static float GetQDelay(bool isQ3)
        {
            var delayOri = !isQ3 ? QDelay : Q2Delay;
            var delayMax = !isQ3 ? QDelays : Q2Delays;
            var perReduce = 1 - delayMax / delayOri;
            var delayReal =
                Math.Max(
                    delayOri * (1 - Math.Min((Player.AttackSpeedMod - 1) * (perReduce / 1.1f), perReduce)),
                    delayMax);
            return (float)Math.Round((decimal)delayReal, 3, MidpointRounding.AwayFromZero);
        }

        private static double GetQDmg(Obj_AI_Base target)
        {
            var dmgItem = 0d;
            if (Items.HasItem((int)ItemId.Sheen) && (Items.CanUseItem((int)ItemId.Sheen) || Player.HasBuff("Sheen")))
            {
                dmgItem = Player.BaseAttackDamage;
            }
            if (Items.HasItem((int)ItemId.Trinity_Force)
                && (Items.CanUseItem((int)ItemId.Trinity_Force) || Player.HasBuff("Sheen")))
            {
                dmgItem = Player.BaseAttackDamage * 2;
            }
            if (dmgItem > 0)
            {
                dmgItem = Player.CalculateDamage(target, DamageType.Physical, dmgItem);
            }
            double dmgQ = Q.GetDamage(target);
            if (Math.Abs(Player.Crit - 1) < float.Epsilon)
            {
                dmgQ += Player.CalculateDamage(
                    target,
                    Q.DamageType,
                    (Items.HasItem((int)ItemId.Infinity_Edge) ? 0.875 : 0.5) * Player.TotalAttackDamage);
            }
            return dmgQ + dmgItem;
        }

        private static bool HaveE(Obj_AI_Base target)
        {
            return target.HasBuff("YasuoDashWrapper");
        }

        private static bool HaveR(AIHeroClient target)
        {
            return target.HasBuffOfType(BuffType.Knockback) || target.HasBuffOfType(BuffType.Knockup);
        }

        private static void Hybrid()
        {
            if (!Q.IsReady() || IsDashing)
            {
                return;
            }
            if (!haveQ3)
            {
                var state = Q.CastingBestTarget(true);
                if (state.IsCasted())
                {
                    return;
                }
                if (state == CastStates.InvalidTarget && getCheckBoxItem(hybridMenu, "QLastHit") && Q.GetTarget(50) == null && !Player.Spellbook.IsAutoAttacking)
                {
                    var minion =
                        EntityManager.MinionsAndMonsters.EnemyMinions.Where(i => (i.IsMinion() || i.IsPet(false)) && IsInRangeQ(i) && Q.CanLastHit(i, GetQDmg(i))).MaxOrDefault(i => i.MaxHealth);
                    if (minion != null)
                    {
                        Q.Casting(minion);
                    }
                }
            }
            else if (getCheckBoxItem(hybridMenu, "Q3"))
            {
                CastQ3();
            }
        }

        private static bool IsInRangeQ(Obj_AI_Minion minion)
        {
            return minion.IsValidTarget(Math.Max(475 + minion.BoundingRadius / 3 - 5, 475));
        }

        private static void KillSteal()
        {
            if (getCheckBoxItem(ksMenu, "Q") && Q.IsReady())
            {
                if (IsDashing)
                {
                    if (CanCastQCir)
                    {
                        var targets = GetQCirTarget.Where(i => i.Health + i.AttackShield <= GetQDmg(i)).ToList();
                        if (CastQCir(targets))
                        {
                            return;
                        }
                    }
                }
                else
                {
                    var target = !haveQ3 ? Q.GetTarget(Q.Width / 2) : Q2.GetTarget(Q2.Width / 2);
                    if (target != null && target.Health + target.AttackShield <= GetQDmg(target))
                    {
                        if (!haveQ3)
                        {
                            if (Q.Casting(target).IsCasted())
                            {
                                return;
                            }
                        }
                        else if (Q2.Casting(target, false, LeagueSharp.SDK.CollisionableObjects.YasuoWall).IsCasted())
                        {
                            return;
                        }
                    }
                }
            }
            if (getCheckBoxItem(ksMenu, "E") && E.IsReady())
            {
                var targets = EntityManager.Heroes.Enemies.Where(i => !HaveE(i) && E.IsInRange(i)).ToList();
                if (targets.Count > 0)
                {
                    var target = targets.FirstOrDefault(i => i.Health + i.MagicShield <= GetEDmg(i));
                    if (target != null)
                    {
                        if (E.CastOnUnit(target))
                        {
                            lastE = Variables.TickCount;
                            return;
                        }
                    }
                    else if (getCheckBoxItem(ksMenu, "Q") && Q.IsReady(50))
                    {
                        target = targets.Where(i => i.Distance(GetPosAfterDash(i)) < Q3.Range).FirstOrDefault(
                            i =>
                                {
                                    var dmgE = GetEDmg(i) - i.MagicShield;
                                    return i.Health - (dmgE > 0 ? dmgE : 0) + i.AttackShield <= GetQDmg(i);
                                });
                        if (target != null && E.CastOnUnit(target))
                        {
                            lastE = Variables.TickCount;
                            return;
                        }
                    }
                }
            }
            if (getCheckBoxItem(ksMenu, "R") && R.IsReady())
            {
                var targets = GetRTarget;
                if (targets.Count > 0)
                {
                    var target =
                        targets.Where(
                            i =>
                            getCheckBoxItem(ksMenu, "RCast" + i.NetworkId)
                            && (i.Health + i.AttackShield <= R.GetDamage(i)
                                || (Q.IsReady(1000) && i.Health + i.AttackShield <= R.GetDamage(i) + GetQDmg(i)))
                            && !Invulnerable.Check(i, R.DamageType))
                            .MaxOrDefault(i => new Priority().GetDefaultPriority(i));
                    if (target != null)
                    {
                        R.CastOnUnit(target);
                    }
                }
            }
        }

        private static void LaneClear()
        {
            var useQ = getCheckBoxItem(lcMenu, "Q");
            var useQ3 = getCheckBoxItem(lcMenu, "Q3");
            if (haveQ3 && useQ3)
            {
                var minions = EntityManager.MinionsAndMonsters.GetJungleMonsters().Where(i => Q2.IsInRange(i)).Cast<Obj_AI_Base>().ToList();
                if (minions != null)
                {
                    Q2.Cast(minions.FirstOrDefault());
                }
            }
            if (getCheckBoxItem(lcMenu, "E") && E.IsReady())
            {
                var minions = Common.ListMinions().Where(i => i.LSIsValidTarget(E.Range) && !HaveE(i) && (!GetPosAfterDash(i).IsUnderEnemyTurret() || getCheckBoxItem(lcMenu, "ETower")) && Evade.IsSafePoint(GetPosAfterDash(i).ToVector2()).IsSafe).OrderByDescending(i => i.MaxHealth).ToList();
                if (minions.Count > 0)
                {
                    var minion = minions.FirstOrDefault(i => E.CanLastHit(i, GetEDmg(i)));
                    if (useQ && minion == null && Q.IsReady(50) && (!haveQ3 || useQ3))
                    {
                        var sub = new List<Obj_AI_Minion>();
                        foreach (var mob in minions)
                        {
                            if ((E2.CanLastHit(mob, GetQDmg(mob), GetEDmg(mob)) || mob.Team == GameObjectTeam.Neutral)
                                && mob.Distance(GetPosAfterDash(mob)) < Q3.Range)
                            {
                                sub.Add(mob);
                            }
                            if (getCheckBoxItem(lcMenu, "ELastHit"))
                            {
                                continue;
                            }
                            var nearMinion =
                                Common.ListMinions()
                                    .Where(i => i.IsValidTarget(Q3.Range, true, GetPosAfterDash(mob)))
                                    .ToList();
                            if (nearMinion.Count > 2 || nearMinion.Count(i => mob.Health <= GetQDmg(mob)) > 1)
                            {
                                sub.Add(mob);
                            }
                        }
                        minion = sub.FirstOrDefault();
                    }
                    if (minion != null && E.CastOnUnit(minion))
                    {
                        lastE = Variables.TickCount;
                        return;
                    }
                }
            }
            if (useQ && Q.IsReady() && (!haveQ3 || useQ3))
            {
                if (IsDashing)
                {
                    if (CanCastQCir)
                    {
                        var minions = GetQCirObj.Where(i => i is Obj_AI_Minion).ToList();
                        if (minions.Any(i => i.Health <= GetQDmg(i) || i.Team == GameObjectTeam.Neutral) || minions.Count > 1)
                        {
                            CastQCir(minions);
                        }
                    }
                }
                else
                {
                    var minions = Common.ListMinions().Where(i => !haveQ3 ? IsInRangeQ(i) : i.IsValidTarget(Q2.Range - i.BoundingRadius / 2)).OrderByDescending(i => i.MaxHealth).ToList();
                    if (minions.Count == 0)
                    {
                        return;
                    }
                    if (!haveQ3)
                    {
                        var minion = minions.FirstOrDefault(i => Q.CanLastHit(i, GetQDmg(i)));
                        if (minion != null)
                        {
                            Q.Casting(minion);
                        }
                        else
                        {
                            var pos = Q.GetLineFarmLocation(minions);
                            if (pos.MinionsHit > 0)
                            {
                                Q.Cast(pos.Position);
                            }
                        }
                    }
                    else
                    {
                        var pos = Q2.GetLineFarmLocation(minions);
                        if (pos.MinionsHit > 0)
                        {
                            Console.WriteLine("ASKDJLAKSJDLKASJDKSAJD");
                            Q2.Cast(pos.Position);
                        }
                    }
                }
            }
        }

        private static void LastHit()
        {
            if (getCheckBoxItem(lhMenu, "Q") && Q.IsReady() && !IsDashing && (!haveQ3 || getCheckBoxItem(lhMenu, "Q3")))
            {
                if (!haveQ3)
                {
                    var minion =
                        EntityManager.MinionsAndMonsters.EnemyMinions.Where(
                            i => (i.IsMinion() || i.IsPet(false)) && IsInRangeQ(i) && Q.CanLastHit(i, GetQDmg(i)))
                            .MaxOrDefault(i => i.MaxHealth);
                    if (minion != null && Q.Casting(minion).IsCasted())
                    {
                        return;
                    }
                }
                else
                {
                    var minion =
                        EntityManager.MinionsAndMonsters.EnemyMinions.Where(
                            i =>
                            (i.IsMinion() || i.IsPet(false)) && i.IsValidTarget(Q2.Range - i.BoundingRadius / 2)
                            && Q2.CanLastHit(i, GetQDmg(i))).MaxOrDefault(i => i.MaxHealth);
                    if (minion != null && Q2.Casting(minion, false, LeagueSharp.SDK.CollisionableObjects.YasuoWall).IsCasted())
                    {
                        return;
                    }
                }
            }

            if (getCheckBoxItem(lhMenu, "E") && E.IsReady() && !Orbwalker.IsAutoAttacking)
            {
                var minion = EntityManager.MinionsAndMonsters.EnemyMinions.Where(i =>
                (i.IsMinion() || i.IsPet(false)) &&
                i.IsValidTarget(E.Range) &&
                !HaveE(i) &&
                E.CanLastHit(i, GetEDmg(i)) &&
                Evade.IsSafePoint(GetPosAfterDash(i).ToVector2()).IsSafe &&
                (!GetPosAfterDash(i).IsUnderEnemyTurret() || getCheckBoxItem(lhMenu, "ETower"))).MaxOrDefault(i => i.MaxHealth);

                if (minion != null && E.CastOnUnit(minion))
                {
                    lastE = Variables.TickCount;
                }
            }
        }

        private static void OnDraw(EventArgs args)
        {
            if (Player.IsDead)
            {
                return;
            }
            if (getCheckBoxItem(drawMenu, "Q") && Q.Level > 0)
            {
                Render.Circle.DrawCircle(
                    Player.Position,
                    (IsDashing ? Q3 : (!haveQ3 ? Q : Q2)).Range,
                    Q.IsReady() ? Color.LimeGreen : Color.IndianRed);
            }
            if (getCheckBoxItem(drawMenu, "E") && E.Level > 0)
            {
                Render.Circle.DrawCircle(Player.Position, E.Range, E.IsReady() ? Color.LimeGreen : Color.IndianRed);
            }
            if (R.Level > 0)
            {
                if (getCheckBoxItem(drawMenu, "R") && R.IsReady())
                {
                    Render.Circle.DrawCircle(
                        Player.Position,
                        R.Range,
                        GetRTarget.Count > 0 ? Color.LimeGreen : Color.IndianRed);
                }
                if (getCheckBoxItem(drawMenu, "UseR"))
                {
                    var menuR = getKeyBindItem(comboMenu, "R");
                    var pos = Drawing.WorldToScreen(Player.Position);
                    var text = $"Use R In Combo: {(menuR ? "On" : "Off")}";
                    Drawing.DrawText(pos.X - (float)50 / 2, pos.Y + 40, menuR ? Color.White : Color.Gray, text);
                }
            }
            if (getCheckBoxItem(drawMenu, "StackQ") && Q.Level > 0)
            {
                var menu = getKeyBindItem(miscMenu, "StackQ");
                var text =
                    $"Auto Stack Q: {(menu ? (haveQ3 ? "Full" : (Q.IsReady() ? "Ready" : "Not Ready")) : "Off")}";
                var pos = Drawing.WorldToScreen(Player.Position);
                Drawing.DrawText(
                    pos.X - (float)50 / 2,
                    pos.Y + 20,
                    menu ? Color.White : Color.Gray,
                    text);
            }
        }

        private static void OnUpdate(EventArgs args)
        {
            if (Player.IsDead || MenuGUI.IsChatOpen || Player.IsRecalling())
            {
                return;
            }

            KillSteal();

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                Combo();
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass))
            {
                Hybrid();
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear) || Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear))
            {
                LaneClear();
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LastHit))
            {
                LastHit();
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Flee) || getKeyBindItem(fleeMenu, "E"))
            {
                Orbwalker.OrbwalkTo(Game.CursorPos);
                Flee();
            }

            if (!Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo) && !Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass))
            {
                AutoQ();
            }
            if (!getKeyBindItem(fleeMenu, "E"))
            {
                StackQ();
            }
        }

        private static void StackQ()
        {
            if (!getKeyBindItem(miscMenu, "StackQ") || !Q.IsReady() || haveQ3 || IsDashing)
            {
                return;
            }
            var state = Q.CastingBestTarget(true);
            if (state.IsCasted() || state != CastStates.InvalidTarget)
            {
                return;
            }
            var minions =
                Common.ListMinions().Where(IsInRangeQ).OrderByDescending(i => i.MaxHealth).ToList();
            if (minions.Count == 0)
            {
                return;
            }
            var minion = minions.FirstOrDefault(i => Q.CanLastHit(i, GetQDmg(i))) ?? minions.FirstOrDefault();
            if (minion == null)
            {
                return;
            }
            Q.Casting(minion);
        }

        private static void TryEvading(List<Skillshot> hitBy, Vector2 to)
        {
            var dangerLevel = hitBy.Select(i => i.DangerLevel).Concat(new[] { 0 }).Max();
            var yasuoE =
                EvadeSpellDatabase.Spells.FirstOrDefault(
                    i => i.Enable && dangerLevel >= i.DangerLevel && i.IsReady && i.Slot == SpellSlot.E);
            if (yasuoE == null)
            {
                return;
            }
            yasuoE.Speed = (int)E.Speed;
            var target =
                yasuoE.GetEvadeTargets(false, true)
                    .OrderBy(i => GetPosAfterDash(i).CountEnemyHeroesInRange(400))
                    .ThenBy(i => GetPosAfterDash(i).Distance(to))
                    .FirstOrDefault();
            if (target != null && E.CastOnUnit(target))
            {
                lastE = Variables.TickCount;
            }
        }
        #endregion
    }
}