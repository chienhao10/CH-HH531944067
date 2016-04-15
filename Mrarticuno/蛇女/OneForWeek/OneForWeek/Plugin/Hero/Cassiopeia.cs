using System;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK.Rendering;
using OneForWeek.Draw;
using OneForWeek.Util;
using OneForWeek.Util.MenuSettings;
using OneForWeek.Util.Misc;
using SharpDX;

namespace OneForWeek.Plugin.Hero
{
    class Cassiopeia : PluginModel, IChampion
    {
        public static Spell.Skillshot Q;
        public static Spell.Skillshot W;
        public static Spell.Targeted E;
        public static Spell.Skillshot R;

        private static int skinId = 1;

        private float _lastECast = 0f;

        private float lastQCast = 0f;

        public void Init()
        {
            InitVariables();
            DamageIndicator.Initialize(Spells.GetComboDamage);
        }

        public void InitVariables()
        {
            Q = new Spell.Skillshot(SpellSlot.Q, 850, SkillShotType.Circular, castDelay: 400, spellWidth: 75);
            W = new Spell.Skillshot(SpellSlot.W, 850, SkillShotType.Circular, spellWidth: 125);
            E = new Spell.Targeted(SpellSlot.E, 700);
            R = new Spell.Skillshot(SpellSlot.R, 825, SkillShotType.Cone, spellWidth: 80);
            InitMenu();

            Orbwalker.OnPostAttack += OnAfterAttack;
            Gapcloser.OnGapcloser += OnGapCloser;
            Interrupter.OnInterruptableSpell += OnPossibleToInterrupt;
            Obj_AI_Base.OnProcessSpellCast += OnProcessSpell;

            Game.OnUpdate += OnGameUpdate;
            Drawing.OnDraw += OnDraw;
        }

        public void InitMenu()
        {
            Menu = MainMenu.AddMenu(GCharname, GCharname);

            Menu.AddLabel("Version: " + GVersion);
            Menu.AddSeparator();
            Menu.AddLabel("By MrArticuno 由CH汉化");

            DrawMenu = Menu.AddSubMenu("线圈 - " + GCharname, GCharname + "线圈");
            DrawMenu.AddGroupLabel("线圈");
            DrawMenu.Add("drawReady", new CheckBox("显示无冷却技能线圈.", false));
            DrawMenu.Add("drawDisable", new CheckBox("关闭线圈"));
            DrawMenu.AddSeparator();
            //Q
            DrawMenu.Add("drawQ", new CheckBox("显示 Q"));
            DrawMenu.AddColorItem("colorQ");
            DrawMenu.AddWidthItem("widthQ");
            //W
            DrawMenu.Add("drawW", new CheckBox("显示 W"));
            DrawMenu.AddColorItem("colorW");
            DrawMenu.AddWidthItem("widthW");
            //E
            DrawMenu.Add("drawE", new CheckBox("显示 E"));
            DrawMenu.AddColorItem("colorE");
            DrawMenu.AddWidthItem("widthE");
            //R
            DrawMenu.Add("drawR", new CheckBox("显示 R"));
            DrawMenu.AddColorItem("colorR");
            DrawMenu.AddWidthItem("widthR");

            ComboMenu = Menu.AddSubMenu("连招 - " + GCharname, GCharname + "连招");
            ComboMenu.AddGroupLabel("连招");
            ComboMenu.Add("comboQ", new CheckBox("使用 Q", true));
            ComboMenu.Add("comboW", new CheckBox("使用 W", true));
            ComboMenu.Add("comboE", new CheckBox("使用 E", true));
            ComboMenu.Add("comboR", new CheckBox("使用 R", true));
            ComboMenu.AddGroupLabel("连招杂项");
            ComboMenu.Add("castWifQnotLand", new CheckBox("Q没中则使用W", true));
            ComboMenu.Add("disableAA", new CheckBox("连招时屏蔽平A", false));
            ComboMenu.AddLabel("此选项会无视最少敌人数量使用才R的设置");
            ComboMenu.Add("flashCombo", new CheckBox("闪现R连招如果敌人可被击杀", false));
            ComboMenu.Add("rsMinEnemiesForR", new Slider("至少敌人数量面对使用R: ", 2, 0, 5));

            HarassMenu = Menu.AddSubMenu("骚扰 - " + GCharname, GCharname + "骚扰");
            HarassMenu.AddGroupLabel("骚扰");
            HarassMenu.Add("hsQ", new CheckBox("使用 Q", true));
            HarassMenu.Add("hsW", new CheckBox("使用 W", true));
            HarassMenu.Add("hsE", new CheckBox("使用 E", true));
            HarassMenu.AddGroupLabel("骚扰杂项");
            HarassMenu.Add("disableAAHS", new CheckBox("骚扰时屏蔽平A", false));
            HarassMenu.Add("hsPE", new CheckBox("只对中毒的目标使用E", true));

            LaneClearMenu = Menu.AddSubMenu("清线 - " + GCharname, GCharname + "清线");
            LaneClearMenu.AddGroupLabel("清线");
            LaneClearMenu.Add("lcQ", new CheckBox("使用 Q", true));
            LaneClearMenu.Add("lcW", new CheckBox("使用 W", true));
            LaneClearMenu.Add("lcE", new CheckBox("使用 E", true));
            LaneClearMenu.Add("lcKE", new CheckBox("可击杀才使用E", false));
            LaneClearMenu.Add("lcPE", new CheckBox("中毒才使用E", true));

            LastHitMenu = Menu.AddSubMenu("尾兵 - " + GCharname, GCharname + "尾兵");
            LastHitMenu.AddGroupLabel("尾兵");
            LastHitMenu.Add("lhQ", new CheckBox("使用 Q", true));
            LastHitMenu.Add("lhW", new CheckBox("使用 W", true));
            LastHitMenu.Add("lhE", new CheckBox("尾兵E", true));

            JungleClearMenu = Menu.AddSubMenu("清野 - " + GCharname, GCharname + "清野");
            JungleClearMenu.AddGroupLabel("清野");
            JungleClearMenu.Add("jcQ", new CheckBox("使用 Q", true));
            JungleClearMenu.Add("jcW", new CheckBox("使用 W", true));
            JungleClearMenu.Add("jcE", new CheckBox("使用 E", true));
            JungleClearMenu.Add("jcKE", new CheckBox("可击杀才使用E", false));
            JungleClearMenu.Add("jcPE", new CheckBox("中毒才使用E", true));


            MiscMenu = Menu.AddSubMenu("杂项 - " + GCharname, GCharname + "杂项");
            MiscMenu.Add("skin", new Slider("换肤: ", 1, 1, 5));
            MiscMenu.Add("poisonForE", new CheckBox("只对中毒的目标使用E", true));
            MiscMenu.Add("miscDelayE", new Slider("E 延迟: ", 150, 0, 500));
            MiscMenu.Add("ksOn", new CheckBox("尝试抢人头", true));
            MiscMenu.Add("miscAntiGapW", new CheckBox("使用W造成间距", true));
            MiscMenu.Add("miscAntiGapR", new CheckBox("使用R造成间距", true));
            MiscMenu.Add("miscAntiMissR", new CheckBox("屏蔽R如果空大", true));
            MiscMenu.Add("miscMinHpAntiGap", new Slider("最低血量进行R造成间距: ", 40, 0, 100));
            MiscMenu.Add("miscInterruptDangerous", new CheckBox("打断威胁的技能", true));

        }

        public void OnCombo()
        {
            var target = TargetSelector.GetTarget(R.Range + 400, DamageType.Magical);

            if (target == null || !target.IsValidTarget(Q.Range)) return;

            var flash = Player.Spells.FirstOrDefault(a => a.SData.Name == "summonerflash");

            if (Misc.IsChecked(ComboMenu, "comboQ") && Q.IsReady() && target.IsValidTarget(Q.Range) && !IsPoisoned(target))
            {
                var predictionQ = Q.GetPrediction(target);

                if (predictionQ.HitChancePercent >= 80)
                {
                    Q.Cast(predictionQ.CastPosition);
                    lastQCast = Game.Time;
                }
            }

            if (Misc.IsChecked(ComboMenu, "comboW") && W.IsReady() && target.IsValidTarget(W.Range))
            {
                if (Misc.IsChecked(ComboMenu, "castWifQnotLand"))
                {
                    if ((!IsPoisoned(target) && !Q.IsReady()) &&
                        (lastQCast - Game.Time) < -0.43f)
                    {
                        var predictionW = W.GetPrediction(target);

                        if (predictionW.HitChancePercent >= 70)
                        {
                            W.Cast(predictionW.CastPosition);
                        }
                    }
                }
                else
                {
                    var predictionW = W.GetPrediction(target);

                    if (predictionW.HitChancePercent >= 70)
                    {
                        W.Cast(predictionW.CastPosition);
                    }
                }
            }

            if (Misc.IsChecked(ComboMenu, "comboE") && E.IsReady() && target.IsValidTarget(E.Range) && (IsPoisoned(target) || !Misc.IsChecked(MiscMenu, "poisonForE")) && canCastE())
            {
                E.Cast(target);
            }

            if (Misc.IsChecked(ComboMenu, "comboR") && R.IsReady())
            {
                if (Misc.IsChecked(ComboMenu, "flashCombo") && PossibleDamage(target) > target.Health && target.IsFacing(_Player) && target.Distance(_Player) > R.Range && (flash != null && flash.IsReady))
                {
                    Player.CastSpell(flash.Slot, target.Position);
                    Core.DelayAction(() => R.Cast(target), 250);
                }

                var countFacing = EntityManager.Heroes.Enemies.Count(t => t.IsValidTarget(R.Range) && t.IsFacing(_Player) && ProbablyFacing(t));

                if (Misc.GetSliderValue(ComboMenu, "rsMinEnemiesForR") <= countFacing && target.IsFacing(_Player) && target.IsValidTarget(R.Range - 50))
                {
                    R.Cast(target);
                }
            }
        }

        private static bool ProbablyFacing(Obj_AI_Base target)
        {
            var predictPos = Prediction.Position.PredictUnitPosition(target, 250);

            return predictPos.Distance(Player.Instance.ServerPosition)  < target.ServerPosition.Distance(Player.Instance.ServerPosition);
        }

        public void OnHarass()
        {
            var target = TargetSelector.GetTarget(Q.Range, DamageType.Magical);

            if (target == null || !target.IsValidTarget(Q.Range)) return;

            if (Misc.IsChecked(HarassMenu, "hsQ") && Q.IsReady() && target.IsValidTarget(Q.Range) && !IsPoisoned(target))
            {
                var predictionQ = Q.GetPrediction(target);

                if (predictionQ.HitChancePercent >= 80)
                {
                    Q.Cast(predictionQ.CastPosition);
                    lastQCast = Game.Time;
                }
            }

            if (Misc.IsChecked(HarassMenu, "hsW") && W.IsReady() && target.IsValidTarget(W.Range))
            {
                if((!IsPoisoned(target) && !Q.IsReady()) &&
                        (lastQCast - Game.Time) < -0.43f)
                    {
                    var predictionW = W.GetPrediction(target);

                    if (predictionW.HitChancePercent >= 70)
                    {
                        W.Cast(predictionW.CastPosition);
                    }
                }
            }

            if (Misc.IsChecked(HarassMenu, "hsE") && E.IsReady() && target.IsValidTarget(E.Range) && (IsPoisoned(target)) && canCastE())
            {
                E.Cast(target);
            }
        }

        public void OnLaneClear()
        {
            var minions = EntityManager.MinionsAndMonsters.EnemyMinions;

            if (minions == null || !minions.Any()) return;

            var bestFarmQ =
                Misc.GetBestCircularFarmLocation(
                    EntityManager.MinionsAndMonsters.EnemyMinions.Where(x => x.Distance(_Player) <= Q.Range)
                        .Select(xm => xm.ServerPosition.To2D())
                        .ToList(), Q.Width, Q.Range);
            var bestFarmW =
                Misc.GetBestCircularFarmLocation(
                    EntityManager.MinionsAndMonsters.EnemyMinions.Where(x => x.Distance(_Player) <= W.Range)
                        .Select(xm => xm.ServerPosition.To2D())
                        .ToList(), W.Width, W.Range);

            if (Misc.IsChecked(LaneClearMenu, "lcQ") && Q.IsReady() && bestFarmQ.MinionsHit > 0)
            {
                Q.Cast(bestFarmQ.Position.To3D());
            }

            if (Misc.IsChecked(LaneClearMenu, "lcW") && W.IsReady() && bestFarmW.MinionsHit > 0)
            {
                W.Cast(bestFarmW.Position.To3D());
            }

            if (Misc.IsChecked(LaneClearMenu, "lcE") && E.IsReady())
            {
                if (Misc.IsChecked(LaneClearMenu, "lcKE"))
                {
                    var minion =
                        EntityManager.MinionsAndMonsters.EnemyMinions.FirstOrDefault(
                            t =>
                                t.IsValidTarget(E.Range) && _Player.GetSpellDamage(t, SpellSlot.E) > t.Health &&
                                (!Misc.IsChecked(LaneClearMenu, "lcPE") || IsPoisoned(t)));

                    if (minion != null)
                        E.Cast(minion);
                }
                else
                {
                    var minion =
                        EntityManager.MinionsAndMonsters.EnemyMinions.FirstOrDefault(
                            t =>
                                t.IsValidTarget(E.Range) &&
                                (Misc.IsChecked(LaneClearMenu, "lcPE") || IsPoisoned(t)));

                    if (minion != null)
                        E.Cast(minion);
                }
            }

        }

        public void OnJungleClear()
        {
            var minions = EntityManager.MinionsAndMonsters.Monsters;

            if (minions == null || !minions.Any(m => m.IsValidTarget(900))) return;

            var bestFarmQ =
                Misc.GetBestCircularFarmLocation(
                    EntityManager.MinionsAndMonsters.EnemyMinions.Where(x => x.Distance(_Player) <= Q.Range)
                        .Select(xm => xm.ServerPosition.To2D())
                        .ToList(), Q.Width, Q.Range);
            var bestFarmW =
                Misc.GetBestCircularFarmLocation(
                    EntityManager.MinionsAndMonsters.EnemyMinions.Where(x => x.Distance(_Player) <= W.Range)
                        .Select(xm => xm.ServerPosition.To2D())
                        .ToList(), W.Width, W.Range);

            if (Misc.IsChecked(JungleClearMenu, "jcQ") && Q.IsReady() && bestFarmQ.MinionsHit > 0)
            {
                Q.Cast(bestFarmQ.Position.To3D());
            }

            if (Misc.IsChecked(JungleClearMenu, "jcW") && W.IsReady() && bestFarmW.MinionsHit > 0)
            {
                W.Cast(bestFarmW.Position.To3D());
            }

            if (Misc.IsChecked(JungleClearMenu, "jcE") && E.IsReady())
            {
                if (Misc.IsChecked(JungleClearMenu, "jcKE"))
                {
                    var minion =
                        EntityManager.MinionsAndMonsters.EnemyMinions.First(
                            t =>
                                t.IsValidTarget(E.Range) && _Player.GetSpellDamage(t, SpellSlot.E) > t.Health &&
                                (!Misc.IsChecked(JungleClearMenu, "jcPE") || IsPoisoned(t)));

                    if (minion != null)
                        E.Cast(minion);
                }
                else
                {
                    var minion =
                        EntityManager.MinionsAndMonsters.EnemyMinions.First(
                            t =>
                                t.IsValidTarget(E.Range) &&
                                (Misc.IsChecked(JungleClearMenu, "jcPE") || IsPoisoned(t)));

                    if (minion != null)
                        E.Cast(minion);
                }
            }

        }

        public void OnFlee()
        {

        }

        public void OnGameUpdate(EventArgs args)
        {
            if (skinId != Misc.GetSliderValue(MiscMenu, "skin"))
            {
                skinId = Misc.GetSliderValue(MiscMenu, "skin");
                Player.SetSkinId(skinId);
            }

            switch (Orbwalker.ActiveModesFlags)
            {
                case Orbwalker.ActiveModes.Combo:
                    if (Misc.IsChecked(ComboMenu, "disableAA"))
                        Orbwalker.DisableAttacking = true;

                    OnCombo();
                    break;
                case Orbwalker.ActiveModes.Flee:
                    OnFlee();
                    break;
                case Orbwalker.ActiveModes.Harass:
                    if (Misc.IsChecked(HarassMenu, "disableAAHS"))
                        Orbwalker.DisableAttacking = true;
                    OnHarass();
                    break;
            }

            if (Orbwalker.DisableAttacking && (Orbwalker.ActiveModesFlags != Orbwalker.ActiveModes.Combo && Orbwalker.ActiveModesFlags != Orbwalker.ActiveModes.Harass))
                Orbwalker.DisableAttacking = false;

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear))
                OnLaneClear();

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear))
                OnJungleClear();

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LastHit))
                OnLastHit();

            if (Misc.IsChecked(MiscMenu, "ksOn"))
                KS();
        }

        private void OnLastHit()
        {
            var minions = EntityManager.MinionsAndMonsters.EnemyMinions.Where(m => m.IsValidTarget(E.Range) && Player.Instance.GetSpellDamage(m, SpellSlot.E) > m.Health);

            if (minions == null || !minions.Any() || !minions.Any(m => m.IsValidTarget(E.Range))) return;

            var target = minions.FirstOrDefault();

            if (Misc.IsChecked(LastHitMenu, "lhQ") && Q.IsReady() && Q.IsInRange(target) && !IsPoisoned(target))
            {
                Q.Cast(target.ServerPosition);
                lastQCast = Game.Time;
            }

            if (Misc.IsChecked(LastHitMenu, "lhW") && W.IsReady() && W.IsInRange(target) && !IsPoisoned(target))
            {
                if (Misc.IsChecked(ComboMenu, "castWifQnotLand"))
                {
                    if ((!IsPoisoned(target) && !Q.IsReady()) &&
                        (lastQCast - Game.Time) < -0.43f)
                    {
                        var predictionW = W.GetPrediction(target);

                        if (predictionW.HitChancePercent >= 70)
                        {
                            W.Cast(predictionW.CastPosition);
                        }
                    }
                }
                else
                {
                    var predictionW = W.GetPrediction(target);

                    if (predictionW.HitChancePercent >= 70)
                    {
                        W.Cast(predictionW.CastPosition);
                    }
                }
            }

            if (Misc.IsChecked(LastHitMenu, "lhE") && E.IsReady() && E.IsInRange(target) && IsPoisoned(target))
            {
                E.Cast(target);
            }
        }

        public void OnDraw(EventArgs args)
        {
            if (Misc.IsChecked(DrawMenu, "drawDisable")) return;

            if (Misc.IsChecked(DrawMenu, "drawReady") ? Q.IsReady() : Misc.IsChecked(DrawMenu, "drawQ"))
            {
                new Circle { Color = DrawMenu.GetColor("colorQ"), BorderWidth = DrawMenu.GetWidth("widthQ"), Radius = Q.Range }.Draw(Player.Instance.Position);
            }

            if (Misc.IsChecked(DrawMenu, "drawReady") ? W.IsReady() : Misc.IsChecked(DrawMenu, "drawW"))
            {
                new Circle { Color = DrawMenu.GetColor("colorW"), BorderWidth = DrawMenu.GetWidth("widthW"), Radius = W.Range }.Draw(Player.Instance.Position);
            }

            if (Misc.IsChecked(DrawMenu, "drawReady") ? E.IsReady() : Misc.IsChecked(DrawMenu, "drawE"))
            {
                new Circle { Color = DrawMenu.GetColor("colorE"), BorderWidth = DrawMenu.GetWidth("widthE"), Radius = E.Range }.Draw(Player.Instance.Position);
            }

            if (Misc.IsChecked(DrawMenu, "drawReady") ? R.IsReady() : Misc.IsChecked(DrawMenu, "drawR"))
            {
                new Circle { Color = DrawMenu.GetColor("colorR"), BorderWidth = DrawMenu.GetWidth("widthR"), Radius = R.Range }.Draw(Player.Instance.Position);
            }
        }

        public void OnAfterAttack(AttackableUnit target, EventArgs args)
        {

        }

        public void OnPossibleToInterrupt(Obj_AI_Base sender, Interrupter.InterruptableSpellEventArgs interruptableSpellEventArgs)
        {
            if (!sender.IsEnemy) return;

            if (Misc.IsChecked(MiscMenu, "miscInterruptDangerous") && interruptableSpellEventArgs.DangerLevel >= DangerLevel.High && R.IsReady() && R.IsInRange(sender))
            {
                R.Cast(sender);
            }
        }

        public void OnGapCloser(AIHeroClient sender, Gapcloser.GapcloserEventArgs e)
        {
            if(!sender.IsEnemy) return;

            if ((e.End.Distance(_Player) < 50 || e.Sender.IsAttackingPlayer) && Misc.IsChecked(MiscMenu, "miscAntiGapR") &&
                _Player.HealthPercent < Misc.GetSliderValue(MiscMenu, "miscMinHpAntiGap") && R.IsReady() && R.IsInRange(sender))
            {
                R.Cast(sender);
            }else if ((e.End.Distance(_Player) < 50 || e.Sender.IsAttackingPlayer) && Misc.IsChecked(MiscMenu, "miscAntiGapW") && W.IsReady() && W.IsInRange(sender))
            {
                W.Cast(e.End);
            }
        }

        public void OnProcessSpell(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {

            if(!sender.IsMe) return;

            if (args.SData.Name == "CassiopeiaTwinFang")
            {
                _lastECast = Game.Time;
            }

            if (args.SData.Name == "CassiopeiaPetrofyingGaze" && Misc.IsChecked(MiscMenu, "miscAntiMissR"))
            {
                if (EntityManager.Heroes.Enemies.Count(t => t.IsValidTarget(R.Range) && t.IsFacing(_Player)) < 1)
                {
                    args.Process = false;
                }
            }

        }

        public void GameObjectOnCreate(GameObject sender, EventArgs args)
        {

        }

        public void GameObjectOnDelete(GameObject sender, EventArgs args)
        {

        }

        private static void KS()
        {

            if (E.IsReady() && EntityManager.Heroes.Enemies.Any(t => t.IsValidTarget(E.Range) && t.Health < _Player.GetSpellDamage(t, SpellSlot.E)))
            {
                E.Cast(EntityManager.Heroes.Enemies.FirstOrDefault(t => t.IsValidTarget(E.Range) && t.Health < _Player.GetSpellDamage(t, SpellSlot.E)));
            }

            if (Q.IsReady() && EntityManager.Heroes.Enemies.Any(t => t.IsValidTarget(Q.Range) && t.Health < _Player.GetSpellDamage(t, SpellSlot.Q)))
            {
                var predictionQ = Q.GetPrediction(EntityManager.Heroes.Enemies.FirstOrDefault(t => t.IsValidTarget(Q.Range) && t.Health < _Player.GetSpellDamage(t, SpellSlot.Q)));

                if (predictionQ.HitChancePercent >= 70)
                {
                    Q.Cast(predictionQ.CastPosition);
                }
            }
        }

        private static float PossibleDamage(Obj_AI_Base target)
        {
            var damage = 0f;
            if (R.IsReady())
                damage += _Player.GetSpellDamage(target, SpellSlot.R);
            if (E.IsReady())
                damage += _Player.GetSpellDamage(target, SpellSlot.E);
            if (W.IsReady())
                damage += _Player.GetSpellDamage(target, SpellSlot.W);
            if (Q.IsReady())
                damage += _Player.GetSpellDamage(target, SpellSlot.Q);

            return damage;
        }

        public bool canCastE()
        {
            return (_lastECast - Game.Time + Misc.GetSliderValue(MiscMenu, "miscDelayE") / 1000f) < 0;
        }

        public bool IsPoisoned(Obj_AI_Base target)
        {
            return target.HasBuffOfType(BuffType.Poison);
        }
    }
}
