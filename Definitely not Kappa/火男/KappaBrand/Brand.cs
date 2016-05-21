namespace KappaBrand
{
    using System;
    using System.Linq;

    using EloBuddy;
    using EloBuddy.SDK;
    using EloBuddy.SDK.Enumerations;
    using EloBuddy.SDK.Events;
    using EloBuddy.SDK.Menu;
    using EloBuddy.SDK.Menu.Values;
    using EloBuddy.SDK.Rendering;

    using SharpDX;

    internal static class Brand
    {
        private static void Main(string[] args)
        {
            Loading.OnLoadingComplete += Loading_OnLoadingComplete1;
        }

        public static Menu MenuIni, TS, Auto, Combo, Harass, LaneClear, JungleClear, KillSteal, DrawMenu;

        public static ComboBox tsmode, tselect;

        public static Spell.Skillshot Q = new Spell.Skillshot(SpellSlot.Q, 1000, SkillShotType.Linear, 250, 1600, 120);

        public static Spell.Skillshot W = new Spell.Skillshot(SpellSlot.W, 900, SkillShotType.Circular, 650, -1, 200);

        public static Spell.Targeted E = new Spell.Targeted(SpellSlot.E, 630);

        public static Spell.Targeted R = new Spell.Targeted(SpellSlot.R, 750);

        private static void Loading_OnLoadingComplete1(EventArgs args)
        {
            if(Player.Instance.Hero != Champion.Brand) return;

            MenuIni = MainMenu.AddMenu("CH汉化-火男", "Brand");
            TS = MenuIni.AddSubMenu("目标选择器");
            Auto = MenuIni.AddSubMenu("自动化");
            Combo = MenuIni.AddSubMenu("连招");
            Harass = MenuIni.AddSubMenu("骚扰");
            LaneClear = MenuIni.AddSubMenu("清线");
            JungleClear = MenuIni.AddSubMenu("清野");
            KillSteal = MenuIni.AddSubMenu("抢头");
            DrawMenu = MenuIni.AddSubMenu("线圈");

            TS.AddGroupLabel("目标选择器");
            tsmode = TS.Add("tsmode", new ComboBox("目标选择模式", 0, "自定义选择", "库自定义选择"));
            tselect = TS.Add("select", new ComboBox("集火模式", 0, "最多被动叠加目标", "最少技能可击杀目标", "鼠标附近目标"));
            if (tsmode.CurrentValue == 1)
            {
                tselect.IsVisible = false;
            }
            tsmode.OnValueChange += delegate { tselect.hide(tsmode); };

            Auto.AddGroupLabel("自动化设置");
            Auto.Add("AutoR", new Slider("自动 R 如果能命中 [{0}] 个目标/以上", 2, 1, 6));
            Auto.Add("Gap", new CheckBox("自动防突进"));
            Auto.Add("Int", new CheckBox("自动技能打断"));
            Auto.Add("Danger", new ComboBox("技能危险等级", 1, "高", "中", "低"));
            Auto.AddSeparator(0);
            Auto.AddGroupLabel("自动被动");
            Auto.Add("AutoQ", new CheckBox("自动 Q 被动"));
            Auto.Add("AutoW", new CheckBox("自动 W 被动", false));
            Auto.Add("AutoE", new CheckBox("自动 E 被动"));
            Auto.AddSeparator(0);
            Auto.AddGroupLabel("防突进 - 技能");
            foreach (var enemy in EntityManager.Heroes.Enemies)
            {
                foreach (var gapspell in Gapcloser.GapCloserList.Where(e => e.ChampName == enemy.ChampionName))
                {
                    Auto.AddLabel(gapspell.ChampName);
                    Auto.Add(gapspell.SpellName, new CheckBox(gapspell.SpellName + " - " + gapspell.SpellSlot));
                }
            }

            Combo.AddGroupLabel("连招设置");
            Combo.Add("Q", new CheckBox("使用 Q"));
            Combo.AddLabel("额外 Q 设置");
            Combo.Add("Qp", new CheckBox("只用Q晕眩"));
            Combo.Add(Q.Slot + "Mana", new Slider("蓝量高于 [{0}%] 使用 Q", 10));
            Combo.AddSeparator(1);

            Combo.Add("W", new CheckBox("使用 W"));
            Combo.AddLabel("额外 W 设置");
            Combo.Add("Wp", new CheckBox("只用W 当目标有被动", false));
            Combo.Add(W.Slot + "Mana", new Slider("蓝量高于 [{0}%] 使用 W", 5));
            Combo.AddSeparator(1);

            Combo.Add("E", new CheckBox("使用 E"));
            Combo.AddLabel("额外 E 设置");
            Combo.Add("Ep", new CheckBox("只用E 当目标有被动", false));
            Combo.Add(E.Slot + "Mana", new Slider("蓝量高于 [{0}%] 使用 E", 15));
            Combo.AddSeparator(1);

            Combo.Add("RFinisher", new CheckBox("使用 R 结束连招/尾头"));
            Combo.Add("RAoe", new CheckBox("使用 R 范围伤害"));
            Combo.Add("Rhit", new Slider("R 范围命中 [{0}] 个目标", 2, 1, 6));
            Combo.Add(R.Slot + "Mana", new Slider("蓝量高于 [{0}%] 使用 R"));

            Harass.AddGroupLabel("骚扰");
            Harass.Add("Q", new CheckBox("使用 Q"));
            Harass.Add(Q.Slot + "Mana", new Slider("蓝量高于 [{0}%] 使用 Q", 65));
            Harass.AddSeparator(1);

            Harass.Add("W", new CheckBox("使用 W"));
            Harass.Add(W.Slot + "Mana", new Slider("蓝量高于 [{0}%] 使用 W", 65));
            Harass.AddSeparator(1);

            Harass.Add("E", new CheckBox("使用 E"));
            Harass.Add(E.Slot + "Mana", new Slider("蓝量高于 [{0}%] 使用 E", 65));

            LaneClear.AddGroupLabel("清线");
            LaneClear.Add("Q", new CheckBox("使用 Q"));
            LaneClear.Add(Q.Slot + "Mana", new Slider("蓝量高于 [{0}%] 使用 Q", 65));
            LaneClear.AddSeparator(1);
            LaneClear.Add("W", new CheckBox("使用 W"));
            LaneClear.Add(W.Slot + "Mana", new Slider("蓝量高于 [{0}%] 使用 W", 65));
            LaneClear.AddSeparator(1);
            LaneClear.Add("E", new CheckBox("使用 E"));
            LaneClear.Add(E.Slot + "Mana", new Slider("蓝量高于 [{0}%] 使用 E", 65));

            JungleClear.AddGroupLabel("清野");
            JungleClear.Add("Q", new CheckBox("使用 Q"));
            JungleClear.Add(Q.Slot + "Mana", new Slider("蓝量高于 [{0}%] 使用 Q", 65));
            JungleClear.AddSeparator(1);
            JungleClear.Add("W", new CheckBox("使用 W"));
            JungleClear.Add(W.Slot + "Mana", new Slider("蓝量高于 [{0}%] 使用 W", 65));
            JungleClear.AddSeparator(1);
            JungleClear.Add("E", new CheckBox("使用 E"));
            JungleClear.Add(E.Slot + "Mana", new Slider("蓝量高于 [{0}%] 使用 E", 65));

            KillSteal.AddGroupLabel("抢头");
            KillSteal.Add("Q", new CheckBox("使用 Q"));
            KillSteal.Add("W", new CheckBox("使用 W"));
            KillSteal.Add("E", new CheckBox("使用 E"));
            KillSteal.Add("R", new CheckBox("使用 R", false));

            DrawMenu.AddGroupLabel("线圈");
            DrawMenu.Add("damage", new CheckBox("显示连招伤害"));
            DrawMenu.AddLabel("显示 = 连招伤害 / 敌人当前血量");
            DrawMenu.AddSeparator(1);
            DrawMenu.Add("Q", new CheckBox("显示 Q 范围"));
            DrawMenu.Add(Q.Name, new ComboBox("Q 颜色", 0, "Chartreuse", "BlueViolet", "Aqua", "Purple", "White", "Orange", "Green"));
            DrawMenu.AddSeparator(1);
            DrawMenu.Add("W", new CheckBox("显示 W 范围"));
            DrawMenu.Add(W.Name, new ComboBox("W 颜色", 0, "Chartreuse", "BlueViolet", "Aqua", "Purple", "White", "Orange", "Green"));
            DrawMenu.AddSeparator(1);
            DrawMenu.Add("E", new CheckBox("显示 E 范围"));
            DrawMenu.Add(E.Name, new ComboBox("E 颜色", 0, "Chartreuse", "BlueViolet", "Aqua", "Purple", "White", "Orange", "Green"));
            DrawMenu.AddSeparator(1);
            DrawMenu.Add("R", new CheckBox("显示 R 范围"));
            DrawMenu.Add(R.Name, new ComboBox("R 颜色", 0, "Chartreuse", "BlueViolet", "ChartAquareuse", "Purple", "White", "Orange", "Green"));
            DrawMenu.AddSeparator(1);

            Game.OnTick += Game_OnTick;
            Drawing.OnDraw += Drawing_OnDraw;
            Gapcloser.OnGapcloser += Gapcloser_OnGapcloser;
            Interrupter.OnInterruptableSpell += Interrupter_OnInterruptableSpell;
            Orbwalker.OnUnkillableMinion += Orbwalker_OnUnkillableMinion;
        }

        private static void Orbwalker_OnUnkillableMinion(Obj_AI_Base target, Orbwalker.UnkillableMinionArgs args)
        {
            if (target == null || !Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear))
            {
                return;
            }

            var Eready = LaneClear["E"].Cast<CheckBox>().CurrentValue && E.IsReady() && target.IsValidTarget(E.Range) && Common.Emana
                         && E.Mana(LaneClear);

            if (Eready && E.GetDamage(target) >= Prediction.Health.GetPrediction(target, E.CastDelay))
            {
                E.Cast(target);
            }
        }

        private static void Game_OnTick(EventArgs args)
        {
            if (Common.orbmode(Orbwalker.ActiveModes.Combo))
            {
                ComboLogic();
            }

            if (Common.orbmode(Orbwalker.ActiveModes.Harass))
            {
                HarassLogic();
            }

            if (Common.orbmode(Orbwalker.ActiveModes.LaneClear))
            {
                LaneClearLogic();
            }

            if (Common.orbmode(Orbwalker.ActiveModes.JungleClear))
            {
                JungleClearLogic();
            }

            Automated();
            KillStealLogic();
        }

        public static void ComboLogic()
        {
            var target = KappaSelector.SelectTarget(Q.Range + 100);
            if (target == null)
            {
                return;
            }

            var Qready = Combo["Q"].Cast<CheckBox>().CurrentValue && Q.IsReady() && target.IsValidTarget(Q.Range) && Q.Mana(Combo)
                         && (Q.GetPrediction(target).HitChance >= HitChance.High || target.IsCC()) && Common.Qmana;
            var Wready = Combo["W"].Cast<CheckBox>().CurrentValue && W.IsReady() && target.IsValidTarget(W.Range)
                         && (W.GetPrediction(target).HitChance >= HitChance.High || target.IsCC()) && Common.Wmana && W.Mana(Combo);
            var Eready = Combo["E"].Cast<CheckBox>().CurrentValue && E.IsReady() && target.IsValidTarget(E.Range) && Common.Emana && E.Mana(Combo);
            var RFinisher = Combo["RFinisher"].Cast<CheckBox>().CurrentValue && R.IsReady() && target.IsValidTarget(R.Range) && Common.Rmana
                            && R.Mana(Combo);
            var RAoe = Combo["RAoe"].Cast<CheckBox>().CurrentValue && R.IsReady() && target.IsValidTarget(R.Range) && Common.Rmana && R.Mana(Combo);

            if (Qready)
            {
                Qlogic(target);
            }
            if (Wready)
            {
                Wlogic(target);
            }
            if (Eready)
            {
                Elogic(target);
            }
            if (RFinisher)
            {
                Rlogic(target);
            }
            if (RAoe)
            {
                Rlogic(target);
            }
        }

        public static void HarassLogic()
        {
            var target = KappaSelector.SelectTarget(Q.Range + 100);
            if (target == null)
            {
                return;
            }

            var Qready = Harass["Q"].Cast<CheckBox>().CurrentValue && Q.IsReady() && target.IsValidTarget(Q.Range) && Q.Mana(Harass)
                         && (Q.GetPrediction(target).HitChance >= HitChance.High || target.IsCC()) && Common.Qmana;
            var Wready = Harass["W"].Cast<CheckBox>().CurrentValue && W.IsReady() && target.IsValidTarget(W.Range)
                         && (W.GetPrediction(target).HitChance >= HitChance.High || target.IsCC()) && Common.Wmana && W.Mana(Harass);
            var Eready = Harass["E"].Cast<CheckBox>().CurrentValue && E.IsReady() && target.IsValidTarget(E.Range) && Common.Emana && E.Mana(Harass);

            if (Qready)
            {
                Qlogic(target);
            }
            if (Wready)
            {
                Wlogic(target);
            }
            if (Eready)
            {
                Elogic(target);
            }
        }

        public static void LaneClearLogic()
        {
            var target = EntityManager.MinionsAndMonsters.EnemyMinions.FirstOrDefault(m => m.IsValidTarget(Q.Range + 50));

            if (target == null)
            {
                return;
            }

            var Qready = LaneClear["Q"].Cast<CheckBox>().CurrentValue && Q.IsReady() && target.IsValidTarget(Q.Range) && Q.Mana(LaneClear)
                         && (Q.GetPrediction(target).HitChance >= HitChance.High || target.IsCC()) && Common.Qmana;
            var Wready = LaneClear["W"].Cast<CheckBox>().CurrentValue && W.IsReady() && target.IsValidTarget(W.Range)
                         && (W.GetPrediction(target).HitChance >= HitChance.High || target.IsCC()) && Common.Wmana && W.Mana(LaneClear);
            var Eready = LaneClear["E"].Cast<CheckBox>().CurrentValue && E.IsReady() && target.IsValidTarget(E.Range) && Common.Emana
                         && E.Mana(LaneClear);

            if (Qready)
            {
                Qlogic(target);
            }
            if (Wready)
            {
                Wlogic(target);
            }
            if (Eready)
            {
                Elogic(target);
            }
        }

        public static void JungleClearLogic()
        {
            var target = EntityManager.MinionsAndMonsters.GetJungleMonsters().FirstOrDefault(m => m.IsValidTarget(Q.Range + 50));

            if (target == null)
            {
                return;
            }

            var Qready = JungleClear["Q"].Cast<CheckBox>().CurrentValue && Q.IsReady() && target.IsValidTarget(Q.Range) && Q.Mana(JungleClear)
                         && (Q.GetPrediction(target).HitChance >= HitChance.High || target.IsCC()) && Common.Qmana;
            var Wready = JungleClear["W"].Cast<CheckBox>().CurrentValue && W.IsReady() && target.IsValidTarget(W.Range)
                         && (W.GetPrediction(target).HitChance >= HitChance.High || target.IsCC()) && Common.Wmana && W.Mana(JungleClear);
            var Eready = JungleClear["E"].Cast<CheckBox>().CurrentValue && E.IsReady() && target.IsValidTarget(E.Range) && Common.Emana
                         && E.Mana(JungleClear);

            if (Qready)
            {
                Qlogic(target);
            }
            if (Wready)
            {
                Wlogic(target);
            }
            if (Eready)
            {
                Elogic(target);
            }
        }

        public static void KillStealLogic()
        {
            var Qready = KillSteal["Q"].Cast<CheckBox>().CurrentValue && Q.IsReady();
            var Wready = KillSteal["W"].Cast<CheckBox>().CurrentValue && W.IsReady();
            var Eready = KillSteal["E"].Cast<CheckBox>().CurrentValue && E.IsReady();
            var Rready = KillSteal["R"].Cast<CheckBox>().CurrentValue && R.IsReady();

            var Qksenemy = EntityManager.Heroes.Enemies.Where(e => e.IsKillable() && e.IsValidTarget(Q.Range));
            var Wksenemy = EntityManager.Heroes.Enemies.Where(e => e.IsKillable() && e.IsValidTarget(W.Range));
            var Eksenemy = EntityManager.Heroes.Enemies.Where(e => e.IsKillable() && e.IsValidTarget(E.Range));

            if (Qready)
            {
                if (Qksenemy != null)
                {
                    foreach (var enemy in Qksenemy.Where(enemy => Q.GetDamage(enemy) >= Prediction.Health.GetPrediction(enemy, Q.CastDelay)))
                    {
                        if (Player.Instance.GetAutoAttackDamage(enemy, true) >= Prediction.Health.GetPrediction(enemy, (int)Orbwalker.AttackDelay))
                        {
                            return;
                        }
                        Q.Cast(enemy);
                    }
                }
            }

            if (Wready)
            {
                if (Wksenemy != null)
                {
                    foreach (var enemy in Wksenemy.Where(enemy => W.GetDamage(enemy) >= Prediction.Health.GetPrediction(enemy, W.CastDelay)))
                    {
                        if ((Q.GetDamage(enemy) >= Prediction.Health.GetPrediction(enemy, Q.CastDelay) && Q.IsReady())
                            || (Orbwalker.CanAutoAttack
                                && Player.Instance.GetAutoAttackDamage(enemy, true)
                                >= Prediction.Health.GetPrediction(enemy, (int)Orbwalker.AttackDelay)))
                        {
                            return;
                        }
                        W.Cast(enemy);
                    }
                }
            }

            if (Eready)
            {
                if (Eksenemy != null)
                {
                    foreach (var enemy in Eksenemy.Where(enemy => E.GetDamage(enemy) >= Prediction.Health.GetPrediction(enemy, E.CastDelay)))
                    {
                        if ((Q.GetDamage(enemy) >= Prediction.Health.GetPrediction(enemy, Q.CastDelay) && Q.IsReady())
                            || (Orbwalker.CanAutoAttack
                                && Player.Instance.GetAutoAttackDamage(enemy, true)
                                >= Prediction.Health.GetPrediction(enemy, (int)Orbwalker.AttackDelay)))
                        {
                            return;
                        }
                        if (W.GetDamage(enemy) >= Prediction.Health.GetPrediction(enemy, W.CastDelay) && W.IsReady())
                        {
                            return;
                        }
                        E.Cast(enemy);
                    }
                }
            }

            if (Rready)
            {
                var ksenemy = EntityManager.Heroes.Enemies.Where(e => e.IsKillable() && e.IsValidTarget(R.Range));
                if (ksenemy != null)
                {
                    foreach (var enemy in ksenemy.Where(enemy => R.GetDamage(enemy) >= Prediction.Health.GetPrediction(enemy, R.CastDelay)))
                    {
                        if ((Q.GetDamage(enemy) >= Prediction.Health.GetPrediction(enemy, Q.CastDelay) && Q.IsReady())
                            || (W.GetDamage(enemy) >= Prediction.Health.GetPrediction(enemy, W.CastDelay) && W.IsReady())
                            || (E.GetDamage(enemy) >= Prediction.Health.GetPrediction(enemy, E.CastDelay) && E.IsReady())
                            || (Orbwalker.CanAutoAttack
                                && Player.Instance.GetAutoAttackDamage(enemy, true)
                                >= Prediction.Health.GetPrediction(enemy, (int)Orbwalker.AttackDelay)))
                        {
                            return;
                        }

                        R.Cast(enemy);
                    }
                }
            }
        }

        public static void Automated()
        {
            var targets = EntityManager.Heroes.Enemies.Where(e => e.countpassive() >= 2 && e.IsValidTarget() && e.IsKillable());

            if (targets != null)
            {
                if (Auto.checkbox("AutoQ"))
                {
                    var target = targets.FirstOrDefault(e => e.IsValidTarget(Q.Range));
                    if (target != null)
                    {
                        Q.Cast(target);
                    }
                }
                if (Auto.checkbox("AutoW"))
                {
                    var target = targets.FirstOrDefault(e => e.IsValidTarget(W.Range));
                    if (target != null)
                    {
                        W.Cast(target);
                    }
                }
                if (Auto.checkbox("AutoE"))
                {
                    var target = targets.FirstOrDefault(e => e.IsValidTarget(E.Range));
                    if (target != null)
                    {
                        E.Cast(target);
                    }
                }
            }

            var hits = Auto.slider("AutoR");
            var enemies = EntityManager.Heroes.Enemies.Where(e => e.IsValidTarget(R.Range) && e.IsKillable());
            var aoetarget = enemies.OrderByDescending(e => Common.CountEnemeis(400, e)).FirstOrDefault(e => Common.CountEnemeis(400, e) >= hits);

            if (aoetarget != null)
            {
                R.Cast(aoetarget);
            }
        }

        public static void Qlogic(Obj_AI_Base target)
        {
            if (target == null)
            {
                return;
            }
            var Combomode = Common.orbmode(Orbwalker.ActiveModes.Combo);
            var Harassmode = Common.orbmode(Orbwalker.ActiveModes.Harass);
            var LaneClearmode = Common.orbmode(Orbwalker.ActiveModes.LaneClear);
            var JungleClearmode = Common.orbmode(Orbwalker.ActiveModes.JungleClear);

            if (Combomode)
            {
                if (Combo.checkbox("Qp"))
                {
                    if (target.brandpassive())
                    {
                        Q.Cast(target);
                    }
                }
                else
                {
                    Q.Cast(target);
                }
            }

            if (Harassmode)
            {
                Q.Cast(target);
            }

            if (LaneClearmode)
            {
                var minion =
                    EntityManager.MinionsAndMonsters.EnemyMinions.FirstOrDefault(
                        m => Q.GetDamage(m) >= Prediction.Health.GetPrediction(m, Q.CastDelay) && Q.GetPrediction(m).HitChance >= HitChance.High);
                if (minion != null)
                {
                    if (Player.Instance.GetAutoAttackDamage(minion, true) >= Prediction.Health.GetPrediction(minion, (int)Orbwalker.AttackDelay))
                    {
                        return;
                    }
                    Q.Cast(minion);
                }
            }

            if (JungleClearmode)
            {
                var minion =
                    EntityManager.MinionsAndMonsters.GetJungleMonsters()
                        .OrderByDescending(m => m.MaxHealth)
                        .FirstOrDefault(m => Q.GetPrediction(m).HitChance >= HitChance.High);

                if (minion != null)
                {
                    Q.Cast(minion);
                }
            }
        }

        public static void Wlogic(Obj_AI_Base target)
        {
            if (target == null)
            {
                return;
            }
            var Combomode = Common.orbmode(Orbwalker.ActiveModes.Combo);
            var Harassmode = Common.orbmode(Orbwalker.ActiveModes.Harass);
            var LaneClearmode = Common.orbmode(Orbwalker.ActiveModes.LaneClear);
            var JungleClearmode = Common.orbmode(Orbwalker.ActiveModes.JungleClear);

            var enemies = EntityManager.Heroes.Enemies.Where(e => e.IsValidTarget(W.Range) && e.IsKillable());
            var pred = Prediction.Position.PredictCircularMissileAoe(
                enemies.Cast<Obj_AI_Base>().ToArray(),
                W.Range,
                W.Width + 50,
                W.CastDelay,
                W.Speed);
            var castpos =
                pred.OrderByDescending(p => p.GetCollisionObjects<AIHeroClient>().Length).FirstOrDefault(p => p.CollisionObjects.Contains(target));
            if (Combomode)
            {
                if (Combo.checkbox("Wp"))
                {
                    if (castpos != null && castpos.CollisionObjects.Length > 1)
                    {
                        W.Cast(castpos.CastPosition);
                    }
                    if (target.brandpassive())
                    {
                        W.Cast(target);
                    }
                    if (target.IsCC())
                    {
                        W.Cast(target);
                    }
                }
                else
                {
                    W.Cast(target);
                }
            }

            if (Harassmode)
            {
                if (castpos != null && castpos.CollisionObjects.Length > 1)
                {
                    W.Cast(castpos.CastPosition);
                }
                W.Cast(target);
            }

            if (LaneClearmode)
            {
                var minions = EntityManager.MinionsAndMonsters.EnemyMinions.Where(e => e.IsValidTarget(W.Range) && e.IsKillable());
                var loc = EntityManager.MinionsAndMonsters.GetCircularFarmLocation(
                    minions.ToArray(),
                    W.Width + 75,
                    (int)W.Range + 50,
                    W.CastDelay,
                    W.Speed);

                var farmpos = loc.CastPosition;

                if (farmpos != null && loc.HitNumber >= 2)
                {
                    W.Cast(farmpos);
                }
            }

            if (JungleClearmode)
            {
                var minions =
                    EntityManager.MinionsAndMonsters.GetJungleMonsters()
                        .OrderByDescending(m => m.MaxHealth)
                        .Where(e => e.IsValidTarget(W.Range) && e.IsKillable());
                var loc = EntityManager.MinionsAndMonsters.GetCircularFarmLocation(
                    minions.ToArray(),
                    W.Width + 75,
                    (int)W.Range + 50,
                    W.CastDelay,
                    W.Speed);
                var farmpos = loc.CastPosition;

                if (farmpos != null)
                {
                    W.Cast(farmpos);
                }
            }
        }

        public static void Elogic(Obj_AI_Base target)
        {
            if (target == null)
            {
                return;
            }

            var Combomode = Common.orbmode(Orbwalker.ActiveModes.Combo);
            var Harassmode = Common.orbmode(Orbwalker.ActiveModes.Harass);
            var LaneClearmode = Common.orbmode(Orbwalker.ActiveModes.LaneClear);
            var JungleClearmode = Common.orbmode(Orbwalker.ActiveModes.JungleClear);

            if (Combomode)
            {
                if (Combo.checkbox("Ep") && target.brandpassive())
                {
                    E.Cast(target);
                }
                else
                {
                    E.Cast(target);
                }
            }

            if (Harassmode)
            {
                E.Cast(target);
            }

            if (LaneClearmode)
            {
                var minionpassive = EntityManager.MinionsAndMonsters.EnemyMinions.Where(m => m.brandpassive() && m.IsValidTarget(E.Range));
                if (minionpassive != null)
                {
                    foreach (var minion in minionpassive)
                    {
                        var minions = EntityManager.MinionsAndMonsters.EnemyMinions.Where(e => e.IsValidTarget(E.Range) && e.IsKillable());
                        var count = minions.Count(m => m.IsInRange(minion, 300));
                        if (count >= 2)
                        {
                            E.Cast(minion);
                        }
                    }
                }
            }

            if (JungleClearmode)
            {
                var minionpassive = EntityManager.MinionsAndMonsters.GetJungleMonsters().Where(m => m.brandpassive() && m.IsValidTarget(E.Range));
                if (minionpassive != null)
                {
                    foreach (var minion in minionpassive)
                    {
                        var minions = EntityManager.MinionsAndMonsters.GetJungleMonsters().Where(e => e.IsValidTarget(E.Range) && e.IsKillable());
                        var count = minions.Count(m => m.IsInRange(minion, 300));
                        if (count >= 2)
                        {
                            E.Cast(minion);
                        }
                    }
                }
            }
        }

        public static void Rlogic(Obj_AI_Base target)
        {
            if (target == null)
            {
                return;
            }

            var Combomode = Common.orbmode(Orbwalker.ActiveModes.Combo);
            var hits = Combo.slider("Rhit");

            if (Combomode)
            {
                if (Combo.checkbox("RAoe"))
                {
                    var AoeHit = Common.CountEnemeis(400, target) >= hits;
                    var bestaoe =
                        EntityManager.Heroes.Enemies.OrderByDescending(e => Common.CountEnemeis(400, e))
                            .FirstOrDefault(e => e.IsValidTarget(R.Range) && e.IsKillable() && Common.CountEnemeis(400, e) >= hits);
                    if (AoeHit)
                    {
                        R.Cast(target);
                    }
                    else
                    {
                        if (bestaoe != null)
                        {
                            R.Cast(bestaoe);
                        }
                    }
                }

                if (Combo.checkbox("RFinisher"))
                {
                    var pred = R.GetDamage(target) >= Prediction.Health.GetPrediction(target, Q.CastDelay);
                    var health = R.GetDamage(target) >= target.TotalShieldHealth();

                    if (Q.GetDamage(target) >= Prediction.Health.GetPrediction(target, Q.CastDelay))
                    {
                        return;
                    }
                    if (W.GetDamage(target) >= Prediction.Health.GetPrediction(target, W.CastDelay))
                    {
                        return;
                    }
                    if (E.GetDamage(target) >= Prediction.Health.GetPrediction(target, E.CastDelay))
                    {
                        return;
                    }

                    if (pred || health)
                    {
                        R.Cast(target);
                    }
                }
            }
        }

        private static void Interrupter_OnInterruptableSpell(Obj_AI_Base sender, Interrupter.InterruptableSpellEventArgs e)
        {
            if (!sender.IsEnemy || !Auto.checkbox("Int") || sender == null || e == null)
            {
                return;
            }

            if (e.DangerLevel >= Common.danger() && sender.IsValidTarget(Q.Range))
            {
                if (sender.brandpassive())
                {
                    if (Q.IsReady())
                    {
                        Q.Cast(sender);
                    }
                }
                else
                {
                    if (E.IsReady() && Q.IsReady() && Common.Qmana && Common.Emana)
                    {
                        if (E.Cast(sender))
                        {
                            if (sender.brandpassive())
                            {
                                Q.Cast(sender);
                            }
                        }
                    }
                }
            }
        }

        private static void Gapcloser_OnGapcloser(AIHeroClient sender, Gapcloser.GapcloserEventArgs e)
        {
            if (!sender.IsEnemy || !Auto.checkbox("Gap") || sender == null || e == null || e.End == Vector3.Zero
                || !e.End.IsInRange(Player.Instance, Q.Range))
            {
                return;
            }

            if (Auto.checkbox(e.SpellName) && sender.IsValidTarget(Q.Range))
            {
                if (sender.brandpassive())
                {
                    if (Q.IsReady())
                    {
                        Q.Cast(sender);
                    }
                }
                else
                {
                    if (E.IsReady() && Q.IsReady() && Common.Qmana && Common.Emana)
                    {
                        if (E.Cast(sender))
                        {
                            if (sender.brandpassive())
                            {
                                Q.Cast(sender);
                            }
                        }
                    }
                }
            }
        }

        private static void Drawing_OnDraw(EventArgs args)
        {
            var pos = Player.Instance.ServerPosition;

            if (DrawMenu["Q"].Cast<CheckBox>().CurrentValue)
            {
                Circle.Draw(Q.IsReady() ? Colors.Select(Q) : Color.Red, Q.Range, pos);
            }

            if (DrawMenu["W"].Cast<CheckBox>().CurrentValue)
            {
                Circle.Draw(W.IsReady() ? Colors.Select(W) : Color.Red, W.Range, pos);
            }

            if (DrawMenu["E"].Cast<CheckBox>().CurrentValue)
            {
                Circle.Draw(E.IsReady() ? Colors.Select(E) : Color.Red, E.Range, pos);
            }

            if (DrawMenu["R"].Cast<CheckBox>().CurrentValue)
            {
                Circle.Draw(R.IsReady() ? Colors.Select(R) : Color.Red, R.Range, pos);
            }

            if (DrawMenu["damage"].Cast<CheckBox>().CurrentValue)
            {
                foreach (var enemy in EntityManager.Heroes.Enemies.Where(e => e.IsHPBarRendered))
                {
                    if (enemy != null)
                    {
                        var hpx = enemy.HPBarPosition.X;
                        var hpy = enemy.HPBarPosition.Y;
                        var damage = (int)Damagelib.GetDamage(enemy) + " / " + (int)enemy.TotalShieldHealth();
                        var c = System.Drawing.Color.GreenYellow;

                        if (Damagelib.GetDamage(enemy) >= enemy.TotalShieldHealth() / 2)
                        {
                            damage = "骚扰可击杀: " + (int)Damagelib.GetDamage(enemy) + " / " + (int)enemy.TotalShieldHealth();
                            c = System.Drawing.Color.Orange;
                        }

                        if (Damagelib.GetDamage(enemy) >= Prediction.Health.GetPrediction(enemy, 1000))
                        {
                            damage = "可击杀: " + (int)Damagelib.GetDamage(enemy) + " / " + (int)enemy.TotalShieldHealth();
                            c = System.Drawing.Color.Red;
                        }

                        Drawing.DrawText(hpx + 145, hpy, c, damage, 3);
                    }
                }
            }
        }
    }
}