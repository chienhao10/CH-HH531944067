namespace Valvrave_Sharp.Plugin
{
    #region

    using System;
    using System.Collections.Generic;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.SDK;
    using LeagueSharp.SDK.Core.Utils;
    using EloBuddy.SDK.Menu;
    using SharpDX;
    using EloBuddy.SDK.Menu.Values;
    using Valvrave_Sharp.Core;

    using Color = System.Drawing.Color;

    #endregion
    using EloBuddy;
    using LeagueSharp.SDK.Modes;
    using EloBuddy.SDK;
    using LeagueSharp.Data.Enumerations;
    using EloBuddy.SDK.Enumerations;
    internal class LeeSin : Program
    {
        #region Constants

        private const int RKickRange = 725;

        #endregion

        #region Static Fields

        private static readonly List<string> SpecialPet = new List<string>
                                                              { "jarvanivstandard", "teemomushroom", "illaoiminion" };

        private static int cPassive;

        private static bool isDashing;

        private static int lastW, lastW2, lastE2, lastR;

        private static Obj_AI_Base objQ;

        private static EloBuddy.SDK.Menu.Menu config = Program._MainMenu;

        #endregion

        public static Menu miscMenu, drawMenu, ksMenu, lhMenu, lcMenu, comboMenu, kuMenu, insecMenu;

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

        #region Constructors and Destructors

        public static LeagueSharp.Common.Spell QS;

        public LeeSin()
        {
            Q = new LeagueSharp.SDK.Spell(SpellSlot.Q, 1100).SetSkillshot(0.25f, 60, 1800, true, SkillshotType.SkillshotLine);
            QS = new LeagueSharp.Common.Spell(SpellSlot.Q, 1100);
            QS.SetSkillshot(0.275f, 60f, 1850f, true, LeagueSharp.Common.SkillshotType.SkillshotLine);
            Q2 = new LeagueSharp.SDK.Spell(Q.Slot, 1300);
            W = new LeagueSharp.SDK.Spell(SpellSlot.W, 700);
            E = new LeagueSharp.SDK.Spell(SpellSlot.E, 425).SetTargetted(0.25f, float.MaxValue);
            E2 = new LeagueSharp.SDK.Spell(E.Slot, 570);
            R = new LeagueSharp.SDK.Spell(SpellSlot.R, 375);
            R2 = new LeagueSharp.SDK.Spell(R.Slot).SetSkillshot(0.325f, 0, 950, false, SkillshotType.SkillshotLine);
            Q.DamageType = Q2.DamageType = W.DamageType = R.DamageType = DamageType.Physical;
            E.DamageType = DamageType.Magical;
            Q.MinHitChance = LeagueSharp.SDK.HitChance.VeryHigh;

            WardManager.Init();
            Insec.Init();

            kuMenu = config.AddSubMenu("KnockUp", "击飞");
            kuMenu.AddGroupLabel("自动击飞设置");
            kuMenu.Add("RAuto", new KeyBind("使用 R", false, KeyBind.BindTypes.PressToggle, 'L'));
            kuMenu.Add("RAutoKill", new CheckBox("如果可以杀后面英雄"));
            kuMenu.Add("RAutoCountA", new Slider("或者可击中英雄数量 >=", 1, 1, 4));

            comboMenu = config.AddSubMenu("Combo", "连招");
            comboMenu.Add("Ignite", new CheckBox("使用 点燃"));
            comboMenu.Add("Item", new CheckBox("使用 物品"));
            comboMenu.Add("W", new CheckBox("使用 W", false));
            comboMenu.Add("E", new CheckBox("使用 E"));
            comboMenu.AddGroupLabel("Q 设置");
            comboMenu.Add("Q", new CheckBox("使用 Q"));
            comboMenu.Add("Q2", new CheckBox("使用 Q2"));
            comboMenu.Add("Q2Obj", new CheckBox("Q2 就算物体远离", false));
            comboMenu.Add("QCol", new CheckBox("惩戒 体积碰撞单位"));
            comboMenu.AddGroupLabel("明星连招");
            comboMenu.Add("Star", new KeyBind("明星连招按键", false, KeyBind.BindTypes.HoldActive, 'X'));
            comboMenu.Add("StarKill", new CheckBox("自动明星连招如果可击杀", false));
            comboMenu.Add("StarKillWJ", new CheckBox("-> 明星连招使用跳眼", false));

            lcMenu = config.AddSubMenu("LaneClear", "清线");
            lcMenu.Add("W", new CheckBox("使用 W", false));
            lcMenu.Add("E", new CheckBox("使用 E"));
            lcMenu.AddGroupLabel("Q 设置");
            lcMenu.Add("Q", new CheckBox("使用 Q"));
            lcMenu.Add("QBig", new CheckBox("只对打野怪使用Q"));

            lhMenu = config.AddSubMenu("LastHit", "尾兵");
            lhMenu.Add("Q", new CheckBox("使用 Q1"));

            ksMenu = config.AddSubMenu("KillSteal", "其他");
            ksMenu.Add("Q", new CheckBox("使用 Q"));
            ksMenu.Add("E", new CheckBox("使用 E"));
            ksMenu.Add("R", new CheckBox("使用 R"));
            ksMenu.AddGroupLabel("额外 R 设置");
            foreach (var enemy in ObjectManager.Get<AIHeroClient>().Where(o => o.IsEnemy))
            {
                ksMenu.Add("RCast" + enemy.NetworkId, new CheckBox("使用在 " + enemy.ChampionName, false));
            }

            drawMenu = config.AddSubMenu("Draw", "线圈");
            drawMenu.Add("Q", new CheckBox("Q 范围", false));
            drawMenu.Add("W", new CheckBox("W 范围", false));
            drawMenu.Add("E", new CheckBox("E 范围", false));
            drawMenu.Add("R", new CheckBox("R 范围", false));
            drawMenu.Add("KnockUp", new CheckBox("显示自动击飞状态"));

            miscMenu = config.AddSubMenu("杂项", "Misc");
            miscMenu.Add("FleeW", new KeyBind("使用 W 逃跑", false, KeyBind.BindTypes.HoldActive, 'C'));
            miscMenu.Add("RFlash", new KeyBind("R-闪现至鼠标", false, KeyBind.BindTypes.HoldActive, 'Z'));

            Game.OnUpdate += OnUpdate;
            Drawing.OnDraw += OnDraw;

            Obj_AI_Base.OnBuffGain += (sender, args) =>
                {
                    if (sender.IsMe)
                    {
                        switch (args.Buff.DisplayName)
                        {
                            case "BlindMonkFlurry":
                                cPassive = 2;
                                break;
                            case "BlindMonkQTwoDash":
                                isDashing = true;
                                break;
                        }
                    }
                    else if (sender.IsEnemy)
                    {
                        if (args.Buff.DisplayName == "BlindMonkSonicWave")
                        {
                            objQ = sender;
                        }
                        else if (args.Buff.Name == "blindmonkrroot" && Flash.IsReady())
                        {
                            CastRFlash(sender);
                        }
                    }
                };
            Obj_AI_Base.OnBuffLose += (sender, args) =>
            {
                if (sender.IsMe)
                {
                    switch (args.Buff.DisplayName)
                    {
                        case "BlindMonkFlurry":
                            cPassive = 0;
                            break;
                        case "BlindMonkQTwoDash":
                            isDashing = false;
                            break;
                    }
                }
                else if (sender.IsEnemy && args.Buff.DisplayName == "BlindMonkSonicWave")
                {
                    objQ = null;
                }
            };
            Obj_AI_Base.OnBuffUpdate += (sender, args) =>
            {
                if (!sender.IsMe || args.Buff.DisplayName != "BlindMonkFlurry")
                {
                    return;
                }
                cPassive = args.Buff.Count;
            };
            Obj_AI_Base.OnProcessSpellCast += (sender, args) =>
                {
                    if (!sender.IsMe || args.Slot != SpellSlot.R)
                    {
                        return;
                    }
                    lastR = Variables.TickCount;
                };
        }

        #endregion

        #region Properties

        private static bool IsDashing => (lastW > 0 && Variables.TickCount - lastW <= 100) || Player.IsDashing();

        private static bool IsEOne => E.Instance.SData.Name.ToLower().Contains("one");

        private static bool IsQOne => Q.Instance.SData.Name.ToLower().Contains("one");

        private static bool IsRecentR => Variables.TickCount - lastR < 2500;

        private static bool IsWOne => W.Instance.SData.Name.ToLower().Contains("one");

        #endregion

        #region Methods

        private static void AutoKnockUp()
        {
            if (!R.IsReady() || !getKeyBindItem(kuMenu, "RAuto"))
            {
                return;
            }
            var multiR = GetMultiR(Player.ServerPosition);
            if (multiR.Item2 == null)
            {
                return;
            }
            if (multiR.Item1 == -1 || multiR.Item1 >= getSliderItem(kuMenu, "RAutoCountA") + 1)
            {
                R.CastOnUnit(multiR.Item2);
            }
        }

        private static bool CanE2(Obj_AI_Base target)
        {
            var buff = target.GetBuff("BlindMonkTempest");
            return buff != null && buff.EndTime - Game.Time < 0.25 * (buff.EndTime - buff.StartTime);
        }

        private static bool CanQ2(Obj_AI_Base target)
        {
            var buff = target.GetBuff("BlindMonkSonicWave");
            return buff != null && buff.EndTime - Game.Time < 0.25 * (buff.EndTime - buff.StartTime);
        }

        private static bool CanR(AIHeroClient target)
        {
            var buff = target.GetBuff("BlindMonkDragonsRage");
            return buff != null && buff.EndTime - Game.Time <= 0.75 * (buff.EndTime - buff.StartTime);
        }

        private static void CastE(List<Obj_AI_Minion> minions = null)
        {
            if (!E.IsReady() || isDashing || Variables.TickCount - lastW <= 150 || Variables.TickCount - lastW2 <= 100)
            {
                return;
            }
            if (minions == null)
            {
                CastECombo();
            }
            else
            {
                CastELaneClear(minions);
            }
        }

        private static void CastECombo()
        {
            if (IsEOne)
            {
                var target = EntityManager.Heroes.Enemies.Where(x => !x.IsDead && Player.IsInRange(x, E.Range + 20) && x.LSIsValidTarget()).Where(x => E.CanHitCircle(x)).ToList();
                if (target.Count == 0)
                {
                    return;
                }
                if ((cPassive == 0 && Player.Mana >= 70) || target.Count > 2
                    || (Orbwalker.LastTarget == null
                            ? target.Any(i => i.DistanceToPlayer() > Player.GetRealAutoAttackRange() + 100)
                            : cPassive < 2))
                {
                    E.Cast();
                }
            }
            else
            {
                var target = GameObjects.EnemyHeroes.Where(i => i.LSIsValidTarget(E2.Range) && HaveE(i)).ToList();
                if (target.Count == 0)
                {
                    return;
                }
                if ((cPassive == 0 || target.Count > 2
                     || target.Any(i => CanE2(i) || i.DistanceToPlayer() > i.GetRealAutoAttackRange() + 50))
                    && E2.Cast())
                {
                    lastE2 = Variables.TickCount;
                }
            }
        }

        private static void CastELaneClear(List<Obj_AI_Minion> minions)
        {
            if (IsEOne)
            {
                if (cPassive > 0)
                {
                    return;
                }
                var count = minions.Count(i => i.IsValidTarget(E.Range));
                if (count > 0 && (Player.Mana >= 70 || count > 2))
                {
                    E.Cast();
                }
            }
            else
            {
                var minion = minions.Where(i => i.IsValidTarget(E2.Range) && HaveE(i)).ToList();
                if (minion.Count > 0 && (cPassive == 0 || minion.Any(CanE2)) && E2.Cast())
                {
                    lastE2 = Variables.TickCount;
                }
            }
        }

        private static void CastQSmite(AIHeroClient target)
        {
            var pred = Q.GetPrediction(target, false, -1, LeagueSharp.SDK.CollisionableObjects.YasuoWall);
            var predS = QS.GetPrediction(target);
            if (pred.Hitchance < Q.MinHitChance)
            {
                return;
            }
            var col = Q.GetCollision(target, new List<Vector3> { pred.UnitPosition, target.Position });
            if (col.Count == 0 || (getCheckBoxItem(comboMenu, "QCol") && Common.CastSmiteKillCollision(col)))
            {
                Q.Cast(predS.CastPosition);
            }
        }

        private static void CastRFlash(Obj_AI_Base target)
        {
            var targetSelect = TargetSelector.SelectedTarget;
            if (!targetSelect.IsValidTarget() || !targetSelect.Compare(target)
                || target.Health + target.AttackShield <= R.GetDamage(target))
            {
                return;
            }
            var pos = new Vector3();
            if (getKeyBindItem(miscMenu, "RFlash"))
            {
                pos = Game.CursorPos;
            }
            else if (getKeyBindItem(insecMenu, "Insec")
                     && Variables.TickCount - Insec.LastRFlashTime < 5000)
            {
                pos = Insec.GetPositionKickTo((AIHeroClient)target);
            }
            if (!pos.IsValid())
            {
                return;
            }
            Player.Spellbook.CastSpell(Flash, target.ServerPosition.LSExtend(pos, -(150 + target.BoundingRadius / 2)));
        }

        private static void CastW(List<Obj_AI_Minion> minions = null)
        {
            if (!W.IsReady() || Variables.TickCount - lastW <= 300 || isDashing || Variables.TickCount - lastE2 <= 100)
            {
                return;
            }
            var hero = Orbwalker.LastTarget as AIHeroClient;
            Obj_AI_Minion minion = null;
            if (minions != null && minions.Count > 0)
            {
                minion = minions.FirstOrDefault(i => i.InAutoAttackRange());
            }
            if (hero == null && minion == null)
            {
                return;
            }
            if (hero != null && Player.HealthPercent < hero.HealthPercent && Player.HealthPercent < 30)
            {
                if (IsWOne)
                {
                    if (W.Cast())
                    {
                        lastW = Variables.TickCount;
                        return;
                    }
                }
                else if (W.Cast())
                {
                    lastW2 = Variables.TickCount;
                    return;
                }
            }
            if (Player.HealthPercent < (minions == null ? 8 : 5) || (!IsWOne && Variables.TickCount - lastW > 2600)
                || cPassive == 0
                || (minion != null && minion.Team == GameObjectTeam.Neutral
                    && minion.GetJungleType() != JungleType.Small && Player.HealthPercent < 40 && IsWOne))
            {
                if (IsWOne)
                {
                    if (W.Cast())
                    {
                        lastW = Variables.TickCount;
                    }
                }
                else if (W.Cast())
                {
                    lastW2 = Variables.TickCount;
                }
            }
        }

        private static void Combo()
        {
            if (getCheckBoxItem(comboMenu, "StarKill"))
            {
                if (R.IsReady() && Q.IsReady() && !IsQOne && getCheckBoxItem(comboMenu, "Q") && getCheckBoxItem(comboMenu, "Q2"))
                {
                    var target = EntityManager.Heroes.Enemies.Where(x => Q2.IsInRange(x) && HaveQ(x)).FirstOrDefault();
                    if (target != null && target.Health + target.AttackShield > Q.GetDamage(target, DamageStage.SecondCast) + Player.GetAutoAttackDamage(target) && target.Health + target.AttackShield <= GetQ2Dmg(target, R.GetDamage(target)) + Player.GetAutoAttackDamage(target))
                    {
                        if (R.CastOnUnit(target))
                        {
                            return;
                        }
                        if (getCheckBoxItem(comboMenu, "StarKillWJ") && !R.IsInRange(target) && target.DistanceToPlayer() < WardManager.WardRange + R.Range - 50 && Player.Mana >= 80 && !isDashing)
                        {
                            Flee(target.ServerPosition, true);
                        }
                    }
                }
            }

            if (getCheckBoxItem(comboMenu, "Q") && Q.IsReady())
            {
                if (IsQOne)
                {
                    var target = Q.GetTarget(Q.Width / 2);
                    if (target != null)
                    {
                        CastQSmite(target);
                    }
                }
                else if (getCheckBoxItem(comboMenu, "Q2") && !IsDashing && objQ.IsValidTarget(Q2.Range))
                {
                    var target = objQ as AIHeroClient;
                    if (target != null)
                    {
                        if (CanQ2(target) || (!R.IsReady() && IsRecentR && CanR(target)) || (target.Health + target.AttackShield <= Q.GetDamage(target, DamageStage.SecondCast) + Player.GetAutoAttackDamage(target)) || ((R.IsReady() || (!target.HasBuff("BlindMonkDragonsRage") && Variables.TickCount - lastR > 1000)) && target.DistanceToPlayer() > target.GetRealAutoAttackRange() + 100) || cPassive == 0)
                        {
                            Q2.Cast();
                            isDashing = true;
                            return;
                        }
                    }
                    else if (getCheckBoxItem(comboMenu, "Q2Obj"))
                    {
                        var targetQ2 = Q2.GetTarget(200);
                        if (targetQ2 != null && objQ.Distance(targetQ2) < targetQ2.DistanceToPlayer() && !targetQ2.InAutoAttackRange())
                        {
                            Q2.Cast();
                            isDashing = true;
                            return;
                        }
                    }
                }
            }
            if (getCheckBoxItem(comboMenu, "E"))
            {
                CastE();
            }
            if (getCheckBoxItem(comboMenu, "W"))
            {
                CastW();
            }
            var subTarget = W.GetTarget();
            if (getCheckBoxItem(comboMenu, "Item"))
            {
                UseItem(subTarget);
            }
            if (subTarget != null && getCheckBoxItem(comboMenu, "Ignite") && Ignite.IsReady() && subTarget.HealthPercent < 30 && subTarget.DistanceToPlayer() <= IgniteRange)
            {
                Player.Spellbook.CastSpell(Ignite, subTarget);
            }
        }

        private static void Flee(Vector3 pos, bool isStar = false)
        {
            if (!W.IsReady() || !IsWOne || Variables.TickCount - lastW <= 500)
            {
                return;
            }
            var posPlayer = Player.ServerPosition;
            var posJump = pos.Distance(posPlayer) < W.Range ? pos : posPlayer.LSExtend(pos, W.Range);
            var objJumps = new List<Obj_AI_Base>();
            objJumps.AddRange(GameObjects.AllyHeroes.Where(i => !i.IsMe));
            objJumps.AddRange(GameObjects.AllyWards.Where(i => i.IsWard()));
            objJumps.AddRange(
                GameObjects.AllyMinions.Where(
                    i => i.IsMinion() || i.IsPet() || SpecialPet.Contains(i.CharData.BaseSkinName.ToLower())));
            var objJump =
                objJumps.Where(
                    i => i.IsValidTarget(W.Range, false) && i.Distance(posJump) < (isStar ? R.Range - 50 : 200))
                    .MinOrDefault(i => i.Distance(posJump));
            if (objJump != null)
            {
                if (W.CastOnUnit(objJump))
                {
                    lastW = Variables.TickCount;
                }
            }
            else
            {
                WardManager.Place(posJump);
            }
        }

        private static Tuple<int, AIHeroClient> GetMultiR(Vector3 from)
        {
            var bestHit = 0;
            AIHeroClient bestTarget = null;
            foreach (var targetKick in
                GameObjects.EnemyHeroes.Where(
                    i =>
                    i.IsValidTarget(R.Range, true, from) && i.Health + i.AttackShield > R.GetDamage(i)
                    && !i.HasBuffOfType(BuffType.SpellShield) && !i.HasBuffOfType(BuffType.SpellImmunity))
                    .OrderByDescending(i => i.AllShield))
            {
                var posTarget = targetKick.ServerPosition;
                R2.Width = targetKick.BoundingRadius;
                R2.Range = RKickRange + R2.Width / 2;
                R2.UpdateSourcePosition(posTarget, posTarget);
                var targetHits =
                    GameObjects.EnemyHeroes.Where(
                        i => i.IsValidTarget(R2.Range, true, R2.From) && !i.Compare(targetKick)).ToList();
                if (targetHits.Count == 0)
                {
                    continue;
                }
                var cHit = 1;
                foreach (var targetHit in from target in targetHits
                                          let posPred = R2.GetPredPosition(target)
                                          let project = posPred.ProjectOn(R2.From, R2.From.LSExtend(@from, -R2.Range))
                                          where
                                              project.IsOnSegment
                                              && project.SegmentPoint.Distance(posPred)
                                              <= R2.Width + target.BoundingRadius / 2
                                          select target)
                {
                    if (getCheckBoxItem(kuMenu, "RAutoKill"))
                    {
                        var dmgR = GetRColDmg(targetKick, targetHit);
                        if (targetHit.Health + targetHit.AttackShield <= dmgR
                            && !Invulnerable.Check(targetHit, R.DamageType, true, dmgR))
                        {
                            return new Tuple<int, AIHeroClient>(-1, targetKick);
                        }
                    }
                    cHit++;
                }
                if (bestHit == 0 || bestHit < cHit)
                {
                    bestHit = cHit;
                    bestTarget = targetKick;
                }
            }
            return new Tuple<int, AIHeroClient>(bestHit, bestTarget);
        }

        private static double GetQ2Dmg(Obj_AI_Base target, double subHp)
        {
            var dmg = new[] { 50, 80, 110, 140, 170 }[Q.Level - 1] + 0.9 * Player.FlatPhysicalDamageMod
                      + 0.08 * (target.MaxHealth - (target.Health - subHp));
            return Player.CalculateDamage(
                target,
                DamageType.Physical,
                target is Obj_AI_Minion ? Math.Min(dmg, 400) : dmg) + subHp;
        }

        private static float GetRColDmg(AIHeroClient kickTarget, AIHeroClient hitTarget)
        {
            return R.GetDamage(hitTarget)
                   + (float)
                     Player.CalculateDamage(
                         hitTarget,
                         DamageType.Physical,
                         new[] { 0.12, 0.15, 0.18 }[R.Level - 1] * kickTarget.AllShield);
        }

        private static bool HaveE(Obj_AI_Base target)
        {
            return target.HasBuff("BlindMonkTempest");
        }

        private static bool HaveQ(Obj_AI_Base target)
        {
            return target.HasBuff("BlindMonkSonicWave");
        }

        private static void KillSteal()
        {
            if (getCheckBoxItem(ksMenu, "Q") && Q.IsReady())
            {
                if (IsQOne)
                {
                    var target = Q.GetTarget(Q.Width / 2);
                    var predS = QS.GetPrediction(target);
                    if (target != null
                        && (target.Health + target.AttackShield <= Q.GetDamage(target)
                            || (target.Health + target.AttackShield
                                <= GetQ2Dmg(target, Q.GetDamage(target)) + Player.GetAutoAttackDamage(target)
                                && Player.Mana - Q.Instance.SData.Mana >= 30))
                        && Q.Cast(predS.CastPosition))
                    {
                        return;
                    }
                }
                else if (!IsDashing)
                {
                    var target = objQ as AIHeroClient;
                    if (target != null
                        && target.Health + target.AttackShield
                        <= Q.GetDamage(target, DamageStage.SecondCast) + Player.GetAutoAttackDamage(target) && Q2.Cast())
                    {
                        isDashing = true;
                        return;
                    }
                }
            }
            if (getCheckBoxItem(ksMenu, "E") && E.IsReady() && IsEOne && EntityManager.Heroes.Enemies.Where(x => !x.IsDead && E.IsInRange(x) && E.CanHitCircle(x) && x.Health + x.MagicShield <= E.GetDamage(x) && x.LSIsValidTarget()).Any() && E.Cast())
            {
                return;
            }
            if (getCheckBoxItem(ksMenu, "R") && R.IsReady())
            {
                var targetList = EntityManager.Heroes.Enemies.Where(x => !x.IsDead && R.IsInRange(x) && getCheckBoxItem(ksMenu, "RCast" + x.NetworkId) && x.LSIsValidTarget()).ToList();
                if (targetList.Count > 0)
                {
                    var targetR = targetList.FirstOrDefault(i => i.Health + i.AttackShield <= R.GetDamage(i));
                    if (targetR != null)
                    {
                        R.CastOnUnit(targetR);
                    }
                    else if (getCheckBoxItem(ksMenu, "Q") && Q.IsReady() && !IsQOne)
                    {
                        var targetQ2R =
                            targetList.FirstOrDefault(
                                i =>
                                HaveQ(i)
                                && i.Health + i.AttackShield
                                <= GetQ2Dmg(i, R.GetDamage(i)) + Player.GetAutoAttackDamage(i));
                        if (targetQ2R != null)
                        {
                            R.CastOnUnit(targetQ2R);
                        }
                    }
                }
            }
        }

        private static void LaneClear()
        {
            var minions =
                Common.ListMinions().Where(i => i.IsValidTarget(Q2.Range)).OrderByDescending(i => i.MaxHealth).ToList();
            if (minions.Count == 0)
            {
                return;
            }
            if (getCheckBoxItem(lcMenu, "E"))
            {
                CastE(minions);
            }
            if (getCheckBoxItem(lcMenu, "W"))
            {
                CastW(minions);
            }
            if (getCheckBoxItem(lcMenu, "Q") && Q.IsReady())
            {
                if (IsQOne)
                {
                    if (cPassive < 2)
                    {
                        var minionQ = minions.Where(i => i.DistanceToPlayer() < Q.Range - 10).ToList();
                        if (minionQ.Count > 0)
                        {
                            var minionJungle =
                                minionQ.Where(i => i.Team == GameObjectTeam.Neutral)
                                    .OrderByDescending(i => i.MaxHealth)
                                    .ThenBy(i => i.DistanceToPlayer())
                                    .ToList();
                            if (getCheckBoxItem(lcMenu, "QBig") && minionJungle.Count > 0 && Player.Health > 100)
                            {
                                minionJungle =
                                    minionJungle.Where(
                                        i =>
                                        i.GetJungleType() == JungleType.Legendary
                                        || i.GetJungleType() == JungleType.Large || i.Name.Contains("Crab")).ToList();
                            }
                            if (minionJungle.Count > 0)
                            {
                                minionJungle.ForEach(i => Q.Casting(i));
                            }
                            else
                            {
                                var minionLane =
                                    minionQ.Where(i => i.Team != GameObjectTeam.Neutral)
                                        .OrderByDescending(i => i.GetMinionType().HasFlag(MinionTypes.Siege))
                                        .ThenBy(i => i.GetMinionType().HasFlag(MinionTypes.Super))
                                        .ThenBy(i => i.Health)
                                        .ThenByDescending(i => i.MaxHealth)
                                        .ToList();
                                if (minionLane.Count == 0)
                                {
                                    return;
                                }
                                foreach (var minion in minionLane)
                                {
                                    if (minion.InAutoAttackRange())
                                    {
                                        if (Q.GetHealthPrediction(minion) > Q.GetDamage(minion)
                                            && Q.Casting(minion).IsCasted())
                                        {
                                            return;
                                        }
                                    }
                                    else if ((Orbwalker.LastTarget != null
                                                  ? Q.CanLastHit(minion, Q.GetDamage(minion))
                                                  : Q.GetHealthPrediction(minion) > Q.GetDamage(minion))
                                             && Q.Casting(minion).IsCasted())
                                    {
                                        return;
                                    }
                                }
                            }
                        }
                    }
                }
                else if (!IsDashing)
                {
                    var q2Minion = objQ;
                    if (q2Minion.IsValidTarget(Q2.Range)
                        && (CanQ2(q2Minion) || q2Minion.Health <= Q.GetDamage(q2Minion, DamageStage.SecondCast)
                            || q2Minion.DistanceToPlayer() > q2Minion.GetRealAutoAttackRange() + 100 || cPassive == 0)
                        && Q2.Cast())
                    {
                        isDashing = true;
                    }
                }
            }
        }

        private static void LastHit()
        {
            if (!getCheckBoxItem(lhMenu, "Q") || !Q.IsReady() || !IsQOne || Player.Spellbook.IsAutoAttacking)
            {
                return;
            }
            var minions =
                GameObjects.EnemyMinions.Where(
                    i => (i.IsMinion() || i.IsPet(false)) && i.IsValidTarget(Q.Range) && Q.CanLastHit(i, Q.GetDamage(i)))
                    .OrderByDescending(i => i.MaxHealth)
                    .ToList();
            if (minions.Count == 0)
            {
                return;
            }
            minions.ForEach(
                i =>
                Q.Casting(
                    i,
                    false,
                    LeagueSharp.SDK.CollisionableObjects.Heroes | LeagueSharp.SDK.CollisionableObjects.Minions | LeagueSharp.SDK.CollisionableObjects.YasuoWall));
        }

        private static void OnDraw(EventArgs args)
        {
            if (Player.IsDead)
            {
                return;
            }
            if (Q == null || W == null || E == null || R == null)
            {
                return;
            }
            if (getCheckBoxItem(drawMenu, "Q") && Q != null)
            {
                if (Q.Level > 0)
                    Render.Circle.DrawCircle(Player.Position, (IsQOne ? Q : Q2).Range, Q.IsReady() ? Color.LimeGreen : Color.IndianRed);
            }
            if (getCheckBoxItem(drawMenu, "W") && W.Level > 0 && IsWOne)
            {
                Render.Circle.DrawCircle(Player.Position, W.Range, W.IsReady() ? Color.LimeGreen : Color.IndianRed);
            }
            if (getCheckBoxItem(drawMenu, "E") && E.Level > 0)
            {
                Render.Circle.DrawCircle(
                    Player.Position,
                    (IsEOne ? E : E2).Range, E.IsReady() ? Color.LimeGreen : Color.IndianRed);
            }
            if (R.Level > 0)
            {
                if (getCheckBoxItem(drawMenu, "R"))
                {
                    Render.Circle.DrawCircle(Player.Position, R.Range, R.IsReady() ? Color.LimeGreen : Color.IndianRed);
                }
                if (getCheckBoxItem(drawMenu, "KnockUp"))
                {
                    var menu = getKeyBindItem(kuMenu, "RAuto");
                    var text = $"Auto KnockUp: {(menu ? "On" : "Off")} [{menu}]";
                    var pos = Drawing.WorldToScreen(Player.Position);
                    Drawing.DrawText(pos.X - (float)90 / 2, pos.Y + 20, menu ? Color.White : Color.Gray, text);
                }
            }
        }

        private static void OnUpdate(EventArgs args)
        {
            if (Player.IsDead || MenuGUI.IsChatOpen || Player.LSIsRecalling() || Shop.IsOpen)
            {
                return;
            }
            KillSteal();

            Orbwalker.DisableAttacking = getKeyBindItem(insecMenu, "Insec");

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                Combo();
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear))
            {
                LaneClear();
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LastHit))
            {
                LastHit();
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.None))
            {
                if (getKeyBindItem(miscMenu, "FleeW"))
                {
                    Orbwalker.OrbwalkTo(Game.CursorPos);
                    Flee(Game.CursorPos);
                }

                else if (getKeyBindItem(miscMenu, "RFlash"))
                {
                    Orbwalker.OrbwalkTo(Game.CursorPos);
                    if (R.IsReady() && Flash.IsReady())
                    {
                        var target = EntityManager.Heroes.Enemies.Where(i => i.Health + i.AttackShield > R.GetDamage(i) && R.IsInRange(i)).FirstOrDefault();
                        if (target != null && R.CastOnUnit(target))
                        {
                            Orbwalker.ForcedTarget = target;
                        }
                    }
                }
                else if (getKeyBindItem(comboMenu, "Star"))
                {
                    StarCombo();
                }
                else if (getKeyBindItem(insecMenu, "Insec"))
                {
                    Insec.Start(Insec.GetTarget);
                }
            }

            if (!getKeyBindItem(insecMenu, "Insec"))
            {
                AutoKnockUp();
            }
        }

        private static void StarCombo()
        {
            var target = Q.GetTarget(Q.Width / 2);
            if (!IsQOne)
            {
                target = objQ as AIHeroClient;
            }
            if (!Q.IsReady())
            {
                target = W.GetTarget();
            }
            Orbwalker.OrbwalkTo(Game.CursorPos);
            if (target == null)
            {
                return;
            }
            if (Q.IsReady())
            {
                if (IsQOne)
                {
                    CastQSmite(target);
                }
                else if (!IsDashing && HaveQ(target)
                         && (target.Health + target.AttackShield
                             <= Q.GetDamage(target, DamageStage.SecondCast) + Player.GetAutoAttackDamage(target)
                             || (!R.IsReady() && IsRecentR && CanR(target))) && Q2.Cast())
                {
                    isDashing = true;
                    return;
                }
            }
            if (E.IsReady() && IsEOne && E.CanHitCircle(target) && (!HaveQ(target) || Player.Mana >= 70) && E.Cast())
            {
                return;
            }
            if (!R.IsReady() || !Q.IsReady() || IsQOne || !HaveQ(target))
            {
                return;
            }
            if (R.IsInRange(target))
            {
                R.CastOnUnit(target);
            }
            else if (target.DistanceToPlayer() < WardManager.WardRange + R.Range - 50 && Player.Mana >= 70 && !isDashing)
            {
                Flee(target.ServerPosition, true);
            }
        }

        private static void UseItem(AIHeroClient target)
        {
            if (target != null && (target.HealthPercent < 40 || Player.HealthPercent < 50))
            {
                if (Bilgewater.IsReady())
                {
                    Bilgewater.Cast(target);
                }
                if (BotRuinedKing.IsReady())
                {
                    BotRuinedKing.Cast(target);
                }
            }
            if (Youmuu.IsReady() && Player.CountEnemyHeroesInRange(W.Range + E.Range) > 0)
            {
                Youmuu.Cast();
            }
            if (Tiamat.IsReady() && Player.CountEnemyHeroesInRange(Tiamat.Range) > 0)
            {
                Tiamat.Cast();
            }
            if (Hydra.IsReady() && Player.CountEnemyHeroesInRange(Hydra.Range) > 0)
            {
                Hydra.Cast();
            }
            if (Titanic.IsReady() && !Player.Spellbook.IsAutoAttacking && Orbwalker.LastTarget != null)
            {
                Titanic.Cast();
            }
        }

        #endregion

        private static class Insec
        {
            #region Constants

            private const int DistWard = 200, DistFlash = 130;

            #endregion

            #region Static Fields

            internal static bool IsWardFlash;

            internal static int LastRFlashTime;

            private static Vector3 lastEndPos, lastFlashPos;

            private static int lastInsecTime, lastMoveTime, lastFlashRTime;

            private static Obj_AI_Base lastObjQ;

            #endregion

            #region Properties

            internal static AIHeroClient GetTarget
            {
                get
                {
                    AIHeroClient target = null;
                    if (getCheckBoxItem(insecMenu, "TargetSelect"))
                    {
                        var sub = TargetSelector.SelectedTarget;
                        if (sub.LSIsValidTarget())
                        {
                            target = sub;
                        }
                    }
                    else
                    {
                        target = Q.GetTarget(-100);
                        if ((getCheckBoxItem(insecMenu, "Q") && Q.IsReady()) || objQ.IsValidTarget(Q2.Range))
                        {
                            target = Q2.GetTarget(FlashRange);
                        }
                    }
                    return target;
                }
            }

            private static bool CanInsec
                =>
                    (WardManager.CanWardJump || (getCheckBoxItem(insecMenu, "Flash") && Flash.IsReady()) || IsRecent)
                    && R.IsReady();

            private static bool CanWardFlash
                =>
                    getCheckBoxItem(insecMenu, "Flash") && getCheckBoxItem(insecMenu, "FlashJump") && WardManager.CanWardJump
                    && Flash.IsReady();

            private static bool IsRecent
                => IsRecentWardJump || (getCheckBoxItem(insecMenu, "Flash") && Variables.TickCount - lastFlashRTime < 5000);

            private static bool IsRecentWardJump
                =>
                    Variables.TickCount - WardManager.LastInsecWardTime < 5000
                    || Variables.TickCount - WardManager.LastInsecJumpTme < 5000;

            private static float RangeNormal
                =>
                    WardManager.CanWardJump || IsRecentWardJump
                        ? WardManager.WardRange - DistWard
                        : FlashRange - DistFlash;

            private static float RangeWardFlash => WardManager.WardRange + R.Range - 100;

            #endregion

            #region Methods

            internal static Vector3 GetPositionKickTo(AIHeroClient target)
            {
                if (lastEndPos.IsValid() && target.Distance(lastEndPos) <= RKickRange + 700)
                {
                    return lastEndPos;
                }
                var pos = Player.ServerPosition;
                switch (getBoxItem(insecMenu, "Mode"))
                {
                    case 0:
                        var turret =
                            GameObjects.AllyTurrets.Where(
                                i =>
                                !i.IsDead && target.Distance(i) <= RKickRange + 500
                                && i.Distance(target) - RKickRange <= 950 && i.Distance(target) > 400)
                                .MinOrDefault(i => i.DistanceToPlayer());
                        if (turret != null)
                        {
                            pos = turret.ServerPosition;
                        }
                        else
                        {
                            var hero =
                                GameObjects.AllyHeroes.Where(
                                    i =>
                                    i.IsValidTarget(RKickRange + 700, false, target.ServerPosition) && !i.IsMe
                                    && i.HealthPercent > 10 && i.Distance(target) > 350)
                                    .MaxOrDefault(i => new Priority().GetDefaultPriority(i));
                            if (hero != null)
                            {
                                pos = hero.ServerPosition;
                            }
                        }
                        break;
                    case 1:
                        pos = Game.CursorPos;
                        break;
                }
                return pos;
            }

            internal static void Init()
            {
                insecMenu = config.AddSubMenu("Insec", "回旋踢");
                insecMenu.Add("TargetSelect", new CheckBox("只使用在选择的目标上", false));
                insecMenu.Add("Mode", new ComboBox("模式", 0, "塔/英雄/当前", "鼠标位置", "当前位置"));

                insecMenu.AddGroupLabel("线圈");
                insecMenu.Add("DLine", new CheckBox("线"));
                insecMenu.Add("DWardFlash", new CheckBox("跳眼 闪现范围"));

                insecMenu.AddGroupLabel("闪现设置");
                insecMenu.Add("Flash", new CheckBox("使用 闪现"));
                insecMenu.Add("FlashMode", new ComboBox("闪现 模式", 0, "R-闪现", "闪现-R", "两者"));
                insecMenu.Add("FlashJump", new CheckBox("使用跳眼接近后闪现"));

                insecMenu.AddGroupLabel("Q 设置");
                insecMenu.Add("Q", new CheckBox("声音 Q"));
                insecMenu.Add("QCol", new CheckBox("惩戒体积碰撞目标"));
                insecMenu.Add("QObj", new CheckBox("使用Q至附近的物体"));

                insecMenu.AddGroupLabel("键位");
                insecMenu.Add("Insec", new KeyBind("回旋踢按键", false, KeyBind.BindTypes.HoldActive, 'T'));

                Game.OnUpdate += args =>
                    {
                        if (lastInsecTime > 0 && Variables.TickCount - lastInsecTime > 5000)
                        {
                            CleanData();
                        }
                        if (lastMoveTime > 0 && Variables.TickCount - lastMoveTime > 1000 && !R.IsReady())
                        {
                            lastMoveTime = 0;
                        }
                    };
                Drawing.OnDraw += args =>
                    {
                        if (Player.IsDead || R.Level == 0 || !CanInsec)
                        {
                            return;
                        }
                        if (getCheckBoxItem(insecMenu, "DLine"))
                        {
                            var target = GetTarget;
                            if (target != null)
                            {
                                Render.Circle.DrawCircle(
                                    target.Position,
                                    target.BoundingRadius * 1.35f,
                                    Color.BlueViolet);
                                Render.Circle.DrawCircle(
                                    GetPositionBehind(target),
                                    target.BoundingRadius * 1.35f,
                                    Color.BlueViolet);
                                Drawing.DrawLine(
                                    Drawing.WorldToScreen(target.Position),
                                    Drawing.WorldToScreen(GetPositionKickTo(target)),
                                    1,
                                    Color.BlueViolet);
                            }
                        }
                        if (getCheckBoxItem(insecMenu, "DWardFlash") && CanWardFlash)
                        {
                            Render.Circle.DrawCircle(Player.Position, RangeWardFlash, Color.Orange);
                        }
                    };
                Obj_AI_Base.OnBuffGain += (sender, args) =>
                    {
                        if (!sender.IsEnemy || args.Buff.DisplayName != "BlindMonkSonicWave")
                        {
                            return;
                        }
                        lastObjQ = sender;
                    };
                Obj_AI_Base.OnProcessSpellCast += (sender, args) =>
                    {
                        if (!sender.IsMe || !getKeyBindItem(insecMenu, "Insec")
                            || !lastFlashPos.IsValid() || args.SData.Name != "SummonerFlash"
                            || !getCheckBoxItem(insecMenu, "Flash") || Variables.TickCount - lastFlashRTime > 1250
                            || args.End.Distance(lastFlashPos) > 150)
                        {
                            return;
                        }
                        lastFlashRTime = Variables.TickCount;
                        var target = TargetSelector.SelectedTarget;
                        if (target.IsValidTarget())
                        {
                            DelayAction.Add(5, () => R.CastOnUnit(target));
                        }
                    };
                Obj_AI_Base.OnSpellCast += (sender, args) =>
                    {
                        if (!sender.IsMe || args.Slot != SpellSlot.R)
                        {
                            return;
                        }
                        CleanData();
                    };
            }

            internal static void Start(AIHeroClient target)
            {
                if (Orbwalker.CanMove && Variables.TickCount - lastMoveTime > 250)
                {
                    if (target != null && lastMoveTime > 0 && CanInsec)
                    {
                        var posEnd = GetPositionKickTo(target);
                        Orbwalker.MoveTo(posEnd.DistanceToPlayer() > target.Distance(posEnd) ? GetPositionBehind(target) : Game.CursorPos);
                    }
                    else
                    {
                        Orbwalker.MoveTo(Game.CursorPos);
                    }
                }
                if (target == null || !CanInsec)
                {
                    return;
                }
                if (R.IsInRange(target))
                {
                    var posEnd = GetPositionKickTo(target);
                    var posTarget = target.Position;
                    var posPlayer = Player.Position;
                    if (posPlayer.Distance(posEnd) > posTarget.Distance(posEnd))
                    {
                        var project = posTarget.LSExtend(posPlayer, -RKickRange)
                            .ProjectOn(posTarget, posEnd.LSExtend(posTarget, -(RKickRange * 0.5f)));
                        if (project.IsOnSegment && project.SegmentPoint.Distance(posEnd) <= RKickRange * 0.5f
                            && R.CastOnUnit(target))
                        {
                            return;
                        }
                    }
                }
                if (!IsRecent)
                {
                    if (!IsWardFlash)
                    {
                        var checkJump = GapCheck(target);
                        if (checkJump.Item2)
                        {
                            GapByWardJump(target, checkJump.Item1);
                        }
                        else
                        {
                            var checkFlash = GapCheck(target, true);
                            if (checkFlash.Item2)
                            {
                                GapByFlash(target, checkFlash.Item1);
                            }
                            else if (CanWardFlash)
                            {
                                var posTarget = target.ServerPosition;
                                if (posTarget.DistanceToPlayer() < RangeWardFlash
                                    && (!isDashing
                                        || (!lastObjQ.Compare(target) && lastObjQ.Distance(posTarget) > RangeNormal)))
                                {
                                    IsWardFlash = true;
                                    return;
                                }
                            }
                        }
                    }
                    else if (WardManager.Place(target.ServerPosition))
                    {
                        Orbwalker.ForcedTarget = target;
                        return;
                    }
                }
                if (!IsDashing && (!CanWardFlash || !IsWardFlash))
                {
                    GapByQ(target);
                }
            }

            private static void CleanData()
            {
                lastEndPos = lastFlashPos = new Vector3();
                lastInsecTime = 0;
                IsWardFlash = false;
                Orbwalker.ForcedTarget = null;
            }

            private static void GapByFlash(AIHeroClient target, Vector3 posBehind)
            {
                switch (getBoxItem(insecMenu, "FlashMode"))
                {
                    case 0:
                        GapByRFlash(target);
                        break;
                    case 1:
                        GapByFlashR(target, posBehind);
                        break;
                    case 2:
                        if (!posBehind.IsValid())
                        {
                            GapByRFlash(target);
                        }
                        else
                        {
                            GapByFlashR(target, posBehind);
                        }
                        break;
                }
            }

            private static void GapByFlashR(AIHeroClient target, Vector3 posBehind)
            {
                if (!Player.Spellbook.CastSpell(Flash, posBehind))
                {
                    return;
                }
                if (Player.CanMove)
                {
                    lastMoveTime = Variables.TickCount;
                    Orbwalker.MoveTo(posBehind.LSExtend(GetPositionKickTo(target), -(DistFlash + Player.BoundingRadius / 2))); // - might bug
                }
                lastFlashPos = posBehind;
                lastEndPos = GetPositionAfterKick(target);
                lastInsecTime = lastFlashRTime = Variables.TickCount;
                Orbwalker.ForcedTarget = target;
            }

            private static void GapByQ(AIHeroClient target)
            {
                if (!getCheckBoxItem(insecMenu, "Q") || !Q.IsReady())
                {
                    return;
                }
                if (CanWardFlash && IsQOne && Player.Mana < 50 + 80)
                {
                    return;
                }
                var minDist = CanWardFlash ? RangeWardFlash : RangeNormal;
                if (IsQOne)
                {
                    var pred = Q.GetPrediction(target, false, -1, LeagueSharp.SDK.CollisionableObjects.YasuoWall);
                    var predS = QS.GetPrediction(target);
                    if (pred.Hitchance >= Q.MinHitChance)
                    {
                        var col = Q.GetCollision(target, new List<Vector3> { pred.UnitPosition, target.Position });
                        if ((col.Count == 0 || (getCheckBoxItem(insecMenu, "QCol") && Common.CastSmiteKillCollision(col)))
                            && Q.Cast(predS.CastPosition))
                        {
                            return;
                        }
                    }
                    if (!getCheckBoxItem(insecMenu, "QObj"))
                    {
                        return;
                    }
                    var nearObj =
                        Common.ListEnemies(true)
                            .Where(
                                i =>
                                !i.Compare(target) && i.IsValidTarget(Q.Range)
                                && Q.GetHealthPrediction(i) > Q.GetDamage(i) && i.Distance(target) < minDist - 50)
                            .OrderBy(i => i.Distance(target))
                            .ThenByDescending(i => i.Health)
                            .ToList();
                    if (nearObj.Count == 0)
                    {
                        return;
                    }
                    nearObj.ForEach(i => Q.Casting(i));
                }
                else if (target.DistanceToPlayer() > minDist
                         && (HaveQ(target) || (objQ.IsValidTarget(Q2.Range) && target.Distance(objQ) < minDist - 50))
                         && ((WardManager.CanWardJump && Player.Mana >= 80)
                             || (getCheckBoxItem(insecMenu, "Flash") && Flash.IsReady())) && Q2.Cast())
                {
                    isDashing = true;
                    Orbwalker.ForcedTarget = target;
                }
            }

            private static void GapByRFlash(AIHeroClient target)
            {
                if (!R.CastOnUnit(target))
                {
                    return;
                }
                lastEndPos = GetPositionAfterKick(target);
                lastInsecTime = LastRFlashTime = Variables.TickCount;
                Orbwalker.ForcedTarget = target;
            }

            private static void GapByWardJump(AIHeroClient target, Vector3 posBehind)
            {
                if (!WardManager.Place(posBehind, 1))
                {
                    return;
                }
                if (Orbwalker.CanMove)
                {
                    lastMoveTime = Variables.TickCount;
                    Orbwalker.MoveTo(posBehind.LSExtend(GetPositionKickTo(target), -(DistWard + Player.BoundingRadius / 2)));
                }
                lastEndPos = GetPositionAfterKick(target);
                lastInsecTime = WardManager.LastInsecWardTime = WardManager.LastInsecJumpTme = Variables.TickCount;
                Orbwalker.ForcedTarget = target;
            }

            private static Tuple<Vector3, bool> GapCheck(AIHeroClient target, bool useFlash = false)
            {
                if (!useFlash ? !WardManager.CanWardJump : !getCheckBoxItem(insecMenu, "Flash") || !Flash.IsReady())
                {
                    return new Tuple<Vector3, bool>(new Vector3(), false);
                }
                var posEnd = GetPositionKickTo(target);
                var posTarget = target.ServerPosition;
                var posPlayer = Player.ServerPosition;
                if (!useFlash)
                {
                    var posBehind = posTarget.LSExtend(posEnd, -DistWard);
                    if (posPlayer.Distance(posBehind) < WardManager.WardRange
                        && posTarget.Distance(posBehind) < posEnd.Distance(posBehind))
                    {
                        return new Tuple<Vector3, bool>(posBehind, true);
                    }
                }
                else
                {
                    var flashMode = getBoxItem(insecMenu, "FlashMode");
                    if (flashMode != 1 && posPlayer.Distance(posTarget) < R.Range)
                    {
                        return new Tuple<Vector3, bool>(new Vector3(), true);
                    }
                    if (flashMode > 0)
                    {
                        var posBehind = posTarget.LSExtend(posEnd, -DistFlash);
                        if (posPlayer.Distance(posBehind) < FlashRange
                            && posTarget.Distance(posBehind) < posEnd.Distance(posBehind))
                        {
                            return new Tuple<Vector3, bool>(posBehind, true);
                        }
                    }
                }
                return new Tuple<Vector3, bool>(new Vector3(), false);
            }

            private static Vector3 GetPositionAfterKick(AIHeroClient target)
            {
                return target.ServerPosition.LSExtend(GetPositionKickTo(target), RKickRange);
            }

            private static Vector3 GetPositionBehind(AIHeroClient target)
            {
                return target.ServerPosition.LSExtend(
                    GetPositionKickTo(target),
                    -(WardManager.CanWardJump ? DistWard : DistFlash));
            }

            #endregion
        }

        private static class WardManager
        {
            #region Constants

            internal const int WardRange = 600;

            #endregion

            #region Static Fields

            internal static int LastInsecWardTime, LastInsecJumpTme;

            private static Vector3 lastPlacePos;

            private static int lastPlaceTime;

            #endregion

            #region Properties

            internal static bool CanWardJump => CanCastWard && W.IsReady() && IsWOne;

            private static bool CanCastWard => Variables.TickCount - lastPlaceTime > 1250 && Items.GetWardSlot() != null
                ;

            private static bool IsTryingToJump => lastPlacePos.IsValid() && Variables.TickCount - lastPlaceTime < 1250;

            #endregion

            #region Methods

            internal static void Init()
            {
                Game.OnUpdate += args =>
                    {
                        if (lastPlacePos.IsValid() && Variables.TickCount - lastPlaceTime > 1500)
                        {
                            lastPlacePos = new Vector3();
                        }
                        if (Player.IsDead)
                        {
                            return;
                        }
                        if (IsTryingToJump)
                        {
                            Jump(lastPlacePos);
                        }
                    };
                Obj_AI_Base.OnProcessSpellCast += (sender, args) =>
                    {
                        if (!lastPlacePos.IsValid() || !sender.IsMe || args.Slot != SpellSlot.W
                            || !args.SData.Name.ToLower().Contains("one"))
                        {
                            return;
                        }
                        var ward = args.Target as Obj_AI_Minion;
                        if (ward == null || !ward.IsValid() || ward.Distance(lastPlacePos) > 150)
                        {
                            return;
                        }
                        var tick = Variables.TickCount;
                        if (tick - LastInsecJumpTme < 1250)
                        {
                            LastInsecJumpTme = tick;
                        }
                        Insec.IsWardFlash = false;
                        lastPlacePos = new Vector3();
                    };
                GameObjectNotifier<Obj_AI_Minion>.OnCreate += (sender, minion) =>
                {
                    if (!lastPlacePos.IsValid() || minion.Distance(lastPlacePos) > 150 || !minion.IsAlly
                        || !minion.IsWard() || !W.IsInRange(minion))
                    {
                        return;
                    }
                    var tick = Variables.TickCount;
                    if (tick - LastInsecWardTime < 1250)
                    {
                        LastInsecWardTime = tick;
                    }
                    if (tick - lastPlaceTime < 1250 && W.IsReady() && IsWOne && W.CastOnUnit(minion))
                    {
                        lastW = tick;
                    }
                };
            }

            internal static bool Place(Vector3 pos, int type = 0)
            {
                if (!CanWardJump)
                {
                    return false;
                }
                var ward = Items.GetWardSlot();
                if (ward == null)
                {
                    return false;
                }
                var posPlayer = Player.ServerPosition;
                var posPlace = pos.Distance(posPlayer) < WardRange ? pos : posPlayer.LSExtend(pos, WardRange);
                if (!Player.Spellbook.CastSpell(ward.SpellSlot, posPlace))
                {
                    return false;
                }
                if (type == 0)
                {
                    lastPlaceTime = Variables.TickCount + 1100;
                }
                else if (type == 1)
                {
                    lastPlaceTime = LastInsecWardTime = LastInsecJumpTme = Variables.TickCount;
                }
                lastPlacePos = posPlace;
                return true;
            }

            private static void Jump(Vector3 pos)
            {
                if (!W.IsReady() || !IsWOne || Variables.TickCount - lastW <= 500)
                {
                    return;
                }
                var wardObj =
                    GameObjects.AllyWards.Where(
                        i => i.IsValidTarget(W.Range, false) && i.IsWard() && i.Distance(pos) < 200)
                        .MinOrDefault(i => i.Distance(pos));
                if (wardObj != null && W.CastOnUnit(wardObj))
                {
                    lastW = Variables.TickCount;
                }
            }

            #endregion
        }
    }
}