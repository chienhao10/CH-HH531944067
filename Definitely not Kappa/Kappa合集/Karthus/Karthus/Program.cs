using System;
using System.Collections.Generic;
using System.Linq;

using EloBuddy;
using EloBuddy.SDK;

using SharpDX;

using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Rendering;

namespace Karthus
{
    using Color = Color;
    using RectangleF = RectangleF;

    internal class Program
    {
        private static Vector2 PingLocation;

        private static int LastPingT = 0;

        public static Vector2[] GMinMaxCorners;

        public static RectangleF GMinMaxBox;

        public static Vector2[] GNonCulledPoints;

        private static bool cz = false;

        private static float czx = 0, czy = 0, czx2 = 0, czy2 = 0;

        private static readonly AIHeroClient player = ObjectManager.Player;

        public static Spell.Skillshot Q { get; private set; }

        public static Spell.Skillshot Q2 { get; private set; }

        public static Spell.Skillshot W { get; private set; }

        public static Spell.Active E { get; private set; }

        public static Spell.Skillshot R { get; private set; }

        public static Menu UltMenu { get; private set; }

        public static Menu ComboMenu { get; private set; }

        public static Menu HarassMenu { get; private set; }

        public static Menu LaneMenu { get; private set; }

        public static Menu LhMenu { get; private set; }

        public static Menu KillStealMenu { get; private set; }

        public static Menu MiscMenu { get; private set; }

        public static Menu DrawMenu { get; private set; }

        private static Menu menuIni;

        private static AIHeroClient qTarget;

        private static AIHeroClient wTarget;

        private static AIHeroClient eTarget;

        private static bool nowE = false;

        public static void Execute()
        {
            if (player.ChampionName != "Karthus")
            {
                return;
            }

            Q = new Spell.Skillshot(SpellSlot.Q, 875, SkillShotType.Circular, 1000, int.MaxValue, 160);
            Q2 = new Spell.Skillshot(SpellSlot.Q, 875, SkillShotType.Circular, 650, int.MaxValue, 100);
            W = new Spell.Skillshot(SpellSlot.W, 1000, SkillShotType.Circular, 500, int.MaxValue, 70);
            E = new Spell.Active(SpellSlot.E, 510);
            R = new Spell.Skillshot(SpellSlot.R, 25000, SkillShotType.Circular, 3000, int.MaxValue, int.MaxValue);

            menuIni = MainMenu.AddMenu("CH汉化-死哥", "Karthus");
            menuIni.AddGroupLabel("欢迎使用最“渣”死哥!");
            menuIni.AddGroupLabel("全局设定");
            menuIni.Add("Ultimate", new CheckBox("使用大招?"));
            menuIni.Add("Combo", new CheckBox("使用连招?"));
            menuIni.Add("Harass", new CheckBox("使用骚扰?"));
            menuIni.Add("LastHit", new CheckBox("使用尾兵?"));
            menuIni.Add("LaneClear", new CheckBox("使用清线?"));
            menuIni.Add("JungleClear", new CheckBox("使用清野?"));
            menuIni.Add("KillSteal", new CheckBox("使用抢头?"));
            menuIni.Add("Misc", new CheckBox("使用杂项?"));
            menuIni.Add("Drawings", new CheckBox("使用线圈?"));

            UltMenu = menuIni.AddSubMenu("大招");
            UltMenu.AddGroupLabel("大招设置");
            UltMenu.Add("UltKS", new CheckBox("R抢人头", false));
            UltMenu.Add("UltMode", new ComboBox("大招逻辑", 0, "Kappa逻辑"));
            UltMenu.AddGroupLabel("Kappa逻辑设置");
            UltMenu.Add("RnearE", new CheckBox("禁用R当敌人在我身边时?"));
            UltMenu.Add("RnearEn", new Slider("附近敌人数量禁用R", 1, 1, 5));
            UltMenu.Add("Rranged", new Slider("附近敌人搜索范围", 1600, 100, 3000));
            UltMenu.AddLabel("推荐距离 (1600 >)");

            ComboMenu = menuIni.AddSubMenu("连招");
            ComboMenu.AddGroupLabel("连招设置");
            ComboMenu.Add("CUse_Q", new CheckBox("使用 Q"));
            ComboMenu.Add("CUse_W", new CheckBox("使用 W"));
            ComboMenu.Add("CUse_E", new CheckBox("使用 E"));
            ComboMenu.Add("CUse_AA", new CheckBox("禁用普攻", false));
            ComboMenu.Add("CEPercent", new Slider("使用 E 蓝量百分比 %", 30, 0, 100));
            ComboMenu.AddSeparator();
            ComboMenu.Add("CE_Auto_False", new CheckBox("自动 E"));
            ComboMenu.AddLabel("自动关闭E当无敌人时");

            HarassMenu = menuIni.AddSubMenu("骚扰");
            HarassMenu.AddGroupLabel("骚扰设置");
            HarassMenu.Add("HUse_Q", new CheckBox("使用 Q"));
            HarassMenu.Add("HUse_E", new CheckBox("使用 E"));
            HarassMenu.Add("HEPercent", new Slider("使用 E 蓝量百分比 %", 30, 0, 100));
            HarassMenu.Add("HUse_AA", new CheckBox("禁用普攻", false));
            HarassMenu.Add("E_LastHit", new CheckBox("使用 E 尾兵"));
            HarassMenu.AddSeparator();
            HarassMenu.Add("HE_Auto_False", new CheckBox("自动 E"));
            HarassMenu.AddLabel("自动关闭E当无敌人时");

            LaneMenu = menuIni.AddSubMenu("农兵");
            LaneMenu.AddGroupLabel("清线设置");
            LaneMenu.Add("FUse_Q", new CheckBox("使用 Q"));
            LaneMenu.Add("FQPercent", new Slider("使用 Q 蓝量百分比 %", 30, 0, 100));
            LaneMenu.AddSeparator();
            LaneMenu.AddGroupLabel("清野设置");
            LaneMenu.Add("JUse_Q", new CheckBox("使用 Q"));
            LaneMenu.Add("JQPercent", new Slider("使用 Q 蓝量百分比 %", 30, 0, 100));
            LaneMenu.AddSeparator();
            LaneMenu.AddGroupLabel("尾兵设置");
            LaneMenu.Add("LUse_Q", new CheckBox("使用 Q"));
            LaneMenu.Add("LAA", new CheckBox("可使用Q则屏蔽普攻", false));
            LaneMenu.Add("LHQPercent", new Slider("使用 Q 蓝量百分比 %", 30, 0, 100));
            /*
            JungleMenu = menuIni.AddSubMenu("JungleClear");
            JungleMenu.Add("JUse_Q", new CheckBox("Use Q"));
            JungleMenu.Add("JQPercent", new Slider("Use Q Mana %", 30, 0, 100));

            LhMenu = menuIni.AddSubMenu("Last Hit");
            LhMenu.AddGroupLabel("LastHit Settings");
            LhMenu.Add("LUse_Q", new CheckBox("Use Q"));
            */

            KillStealMenu = menuIni.AddSubMenu("抢头");
            KillStealMenu.AddGroupLabel("抢头设置");
            KillStealMenu.Add("KS", new CheckBox("Q抢头"));

            MiscMenu = menuIni.AddSubMenu("杂项");
            MiscMenu.AddGroupLabel("杂项设置");
            MiscMenu.Add("NotifyUlt", new CheckBox("大招提示"));
            MiscMenu.Add("ping", new CheckBox("本地提示: 可击杀敌人"));
            MiscMenu.Add("DeadCast", new CheckBox("可击杀"));
            MiscMenu.Add("SaveR", new CheckBox("为R保存蓝"));
            MiscMenu.Add("gapcloser", new CheckBox("防突击"));
            MiscMenu.Add("gapclosermana", new Slider("防突击蓝量设置", 25, 0, 100));

            DrawMenu = menuIni.AddSubMenu("线圈");
            DrawMenu.AddGroupLabel("线圈设置");
            DrawMenu.Add("Draw_Q", new CheckBox("显示 Q"));
            DrawMenu.Add("Draw_W", new CheckBox("显示 W"));
            DrawMenu.Add("Draw_E", new CheckBox("显示 E"));
            DrawMenu.Add("Rranged", new CheckBox("显示最少附近敌人数量使用R"));
            DrawMenu.Add("Rtarget", new CheckBox("显示R目标"));
            DrawMenu.Add("Track", new CheckBox("监控敌人血量"));

            Game.OnUpdate += Zigzag;
            Game.OnUpdate += OnUpdate;
            Drawing.OnDraw += OnDraw;
            Gapcloser.OnGapcloser += Gapcloser_OnGap;
        }

        private static void Gapcloser_OnGap(AIHeroClient Sender, Gapcloser.GapcloserEventArgs args)
        {
            if (!menuIni.Get<CheckBox>("Misc").CurrentValue || !MiscMenu.Get<CheckBox>("gapcloser").CurrentValue
                || ObjectManager.Player.ManaPercent < MiscMenu.Get<Slider>("gapclosermana").CurrentValue || Sender == null)
            {
                return;
            }
            var predw = W.GetPrediction(Sender);
            if (Sender.IsValidTarget(W.Range) && W.IsReady() && !Sender.IsAlly && !Sender.IsMe)
            {
                if (MiscMenu.Get<CheckBox>("SaveR").CurrentValue && player.Level >= 6 && R.IsLearned
                    && player.Mana - (SaveR() / 3) > R.Handle.SData.Mana)
                {
                    W.Cast(predw.CastPosition);
                }
            }
        }

        private static void Ping(Vector2 position)
        {
            if (Environment.TickCount - LastPingT < 30 * 1000)
            {
                return;
            }

            LastPingT = Environment.TickCount;
            PingLocation = position;
            SimplePing();
            Core.DelayAction(SimplePing, 150);
            Core.DelayAction(SimplePing, 300);
            Core.DelayAction(SimplePing, 400);
            Core.DelayAction(SimplePing, 800);
        }

        private static void SimplePing()
        {
            TacticalMap.ShowPing(PingCategory.Danger, PingLocation, true);
        }

        private static void Zigzag(EventArgs args)
        {
            if (qTarget == null)
            {
                czx = 0;
                czx2 = 0;
                czy = 0;
                czy2 = 0;
                return;
            }

            if (czx < czx2)
            {
                cz = czx2 >= qTarget.ServerPosition.X;
            }
            else if (czx == czx2)
            {
                cz = false;
                czx = czx2;
                czx2 = qTarget.ServerPosition.X;
                return;
            }
            else
            {
                cz = czx2 <= qTarget.ServerPosition.X;
            }
            czx = czx2;
            czx2 = qTarget.ServerPosition.X;

            if (czy < czy2)
            {
                cz = czy2 >= qTarget.ServerPosition.Y;
            }
            else if (czy == czy2)
            {
                cz = false;
            }
            else
            {
                cz = czy2 <= qTarget.ServerPosition.Y;
            }
            czy = czy2;
            czy2 = qTarget.ServerPosition.Y;
        }

        private static void OnUpdate(EventArgs args)
        {
            if (player.IsDead)
            {
                return;
            }

            qTarget = TargetSelector.GetTarget(Q.Range, DamageType.Magical);
            wTarget = TargetSelector.GetTarget(W.Range, DamageType.Magical);
            eTarget = TargetSelector.GetTarget(E.Range, DamageType.Magical);

            var flags = Orbwalker.ActiveModesFlags;
            if (flags.HasFlag(Orbwalker.ActiveModes.Combo) && menuIni.Get<CheckBox>("Combo").CurrentValue)
            {
                Orbwalker.DisableAttacking = ComboMenu.Get<CheckBox>("CUse_AA").CurrentValue && player.Mana > Q.Handle.SData.Mana * 3;
                if (MiscMenu.Get<CheckBox>("SaveR").CurrentValue && player.Mana - (SaveR() / 3) - 30 > R.Handle.SData.Mana && player.Level >= 6
                    && R.IsLearned)
                {
                    Combo();
                }

                if (!MiscMenu.Get<CheckBox>("SaveR").CurrentValue || player.Level < 6 && !R.IsLearned || player.IsZombie)
                {
                    Combo();
                }
            }

            if (flags.HasFlag(Orbwalker.ActiveModes.LaneClear) && menuIni.Get<CheckBox>("LaneClear").CurrentValue)
            {
                Orbwalker.DisableAttacking = false;
                if (MiscMenu.Get<CheckBox>("SaveR").CurrentValue && player.Mana - (SaveR() / 3) > R.Handle.SData.Mana && player.Level >= 6
                    && R.IsLearned)
                {
                    LaneClear();
                }

                if (!MiscMenu.Get<CheckBox>("SaveR").CurrentValue || player.Level < 6 && !R.IsLearned)
                {
                    LaneClear();
                }
            }

            if (flags.HasFlag(Orbwalker.ActiveModes.JungleClear) && menuIni.Get<CheckBox>("JungleClear").CurrentValue)
            {
                Orbwalker.DisableAttacking = false;
                if (MiscMenu.Get<CheckBox>("SaveR").CurrentValue && player.Level >= 6 && R.IsLearned
                    && player.Mana - (SaveR() / 3) > R.Handle.SData.Mana)
                {
                    JungleClear();
                }

                if (!MiscMenu.Get<CheckBox>("SaveR").CurrentValue || player.Level < 6 && !R.IsLearned)
                {
                    JungleClear();
                }
            }

            if (flags.HasFlag(Orbwalker.ActiveModes.Harass) && menuIni.Get<CheckBox>("Harass").CurrentValue)
            {
                Orbwalker.DisableAttacking = HarassMenu.Get<CheckBox>("HUse_AA").CurrentValue && Player.Instance.Mana < Q.Handle.SData.Mana * 3;

                if (MiscMenu.Get<CheckBox>("SaveR").CurrentValue && player.Level >= 6 && R.IsLearned
                    && player.Mana - (SaveR() / 2) > R.Handle.SData.Mana)
                {
                    Harass();
                }

                if (!MiscMenu.Get<CheckBox>("SaveR").CurrentValue || player.Level < 6 && !R.IsLearned)
                {
                    Harass();
                }
            }

            if (flags.HasFlag(Orbwalker.ActiveModes.LastHit) && menuIni.Get<CheckBox>("LastHit").CurrentValue)
            {
                if (LaneMenu.Get<CheckBox>("LAA").CurrentValue
                    && (Q.IsReady() || ObjectManager.Player.ManaPercent >= LaneMenu.Get<Slider>("LHQPercent").CurrentValue))
                {
                    Orbwalker.DisableAttacking = true;
                }
                else
                {
                    Orbwalker.DisableAttacking = false;
                }

                if (MiscMenu.Get<CheckBox>("SaveR").CurrentValue && player.Level >= 6 && R.IsLearned
                    && player.Mana - (SaveR() / 3) > R.Handle.SData.Mana)
                {
                    LastHit();
                }

                if (!MiscMenu.Get<CheckBox>("SaveR").CurrentValue || player.Level < 6 && !R.IsLearned)
                {
                    LastHit();
                }
            }

            if (MiscMenu.Get<CheckBox>("DeadCast").CurrentValue)
            {
                if (ObjectManager.Player.IsZombie && !Combo())
                {
                    LaneClear();
                }
            }

            if (menuIni.Get<CheckBox>("KillSteal").CurrentValue)
            {
                Ks();
            }

            if (menuIni.Get<CheckBox>("Ultimate").CurrentValue && UltMenu.Get<ComboBox>("UltMode").CurrentValue == 0
                && UltMenu.Get<CheckBox>("UltKS").CurrentValue && (R.IsLearned && R.IsReady()))
            {
                Ult();
            }
        }

        private static void OnDraw(EventArgs args)
        {
            if (!player.IsDead && menuIni.Get<CheckBox>("Drawings").CurrentValue)
            {
                if (DrawMenu.Get<CheckBox>("Draw_Q").CurrentValue)
                {
                    Circle.Draw(Color.DarkRed, Q.Range, Player.Instance.Position);
                }
                if (DrawMenu.Get<CheckBox>("Draw_W").CurrentValue)
                {
                    Circle.Draw(Color.DarkRed, W.Range, Player.Instance.Position);
                }
                if (DrawMenu.Get<CheckBox>("Draw_E").CurrentValue)
                {
                    Circle.Draw(Color.DarkRed, E.Range, Player.Instance.Position);
                }
                if (DrawMenu.Get<CheckBox>("Rranged").CurrentValue)
                {
                    Circle.Draw(Color.DarkRed, UltMenu.Get<Slider>("Rranged").CurrentValue, Player.Instance.Position);
                }
                if (DrawMenu.Get<CheckBox>("Track").CurrentValue)
                {
                    DrawEnemyHealth();
                }
            }
            DrawKillable();
        }

        private static void DrawKillable()
        {
            var time = Environment.TickCount;
            var enemiesrange = ObjectManager.Player.Position.CountEnemiesInRange(UltMenu.Get<Slider>("Rranged").CurrentValue);
            var enemieinsrange = UltMenu.Get<Slider>("RnearEn").CurrentValue;
            if (UltMenu.Get<CheckBox>("RnearE").CurrentValue && enemiesrange >= enemieinsrange)
            {
                Drawing.DrawText(
                    Drawing.Width * 0.44f,
                    Drawing.Height * 0.8f,
                    System.Drawing.Color.Red,
                    "屏蔽R附近敌人数: " + enemieinsrange);
            }

            if (R.IsLearned)
            {
                var killable = string.Empty;

                foreach (var target in
                    EntityManager.Heroes.Enemies.Where(
                        x =>
                        !x.IsDead && x.IsEnemy
                        && player.GetSpellDamage(x, SpellSlot.R) > Prediction.Health.GetPrediction(x, (int)(R.CastDelay * 1000f)))
                        .Where(target => target.IsVisible))
                {
                    killable += target.ChampionName + ", ";
                    if (MiscMenu.Get<CheckBox>("ping").CurrentValue)
                    {
                        Ping(target.Position.To2D());
                    }
                    if (DrawMenu.Get<CheckBox>("Rtarget").CurrentValue)
                    {
                        Circle.Draw(Color.DarkRed, 650, target.Position);
                        Drawing.DrawText(
                            Drawing.WorldToScreen(target.Position) - new Vector2(0.44f, 0.8f),
                            System.Drawing.Color.Red,
                            "R可击杀",
                            2);
                    }

                    if (killable != string.Empty)
                    {
                        if (MiscMenu.Get<CheckBox>("NotifyUlt").CurrentValue)
                        {
                            Drawing.DrawText(Drawing.Width * 0.44f, Drawing.Height * 0.7f, System.Drawing.Color.Red, "可被大招击杀: " + killable);
                        }
                    }
                }
            }
        }

        private static void DrawEnemyHealth()
        {
            {
                float i = 0;
                foreach (var hero in
                    EntityManager.Heroes.Enemies.Where(hero => hero != null && hero.IsEnemy && !hero.IsMe && !hero.IsDead))
                {
                    var champion = hero.ChampionName;
                    if (champion.Length > 12)
                    {
                        champion = champion.Remove(7) + "..";
                    }

                    var percent = (int)hero.HealthPercent;
                    var color = System.Drawing.Color.Red;
                    if (percent > 25)
                    {
                        color = System.Drawing.Color.Orange;
                    }

                    if (percent > 50)
                    {
                        color = System.Drawing.Color.Yellow;
                    }

                    if (percent > 75)
                    {
                        color = System.Drawing.Color.LimeGreen;
                    }

                    Drawing.DrawText(Drawing.Width * 0.01f, Drawing.Height * 0.1f + i, color, champion);
                    Drawing.DrawText(
                        Drawing.Width * 0.06f,
                        Drawing.Height * 0.1f + i,
                        color,
                        (" ( " + (int)hero.TotalShieldHealth()) + " / " + (int)hero.MaxHealth + " | " + percent + "% ) ");

                    if (hero.IsVisible)
                    {
                        if (player.GetSpellDamage(hero, SpellSlot.R) > hero.TotalShieldHealth())
                        {
                            Drawing.DrawText(Drawing.Width * 0.13f, Drawing.Height * 0.1f + i, color, " ULT = DEAD ");
                        }
                    }

                    i += 20f;
                }
            }
        }

        private static void calcE()
        {
            calcE(false);
        }

        private static void calcE(bool tc = false)
        {
            if (!E.IsReady() || player.IsZombie || player.Spellbook.GetSpell(SpellSlot.E).ToggleState != 2)
            {
                return;
            }

            var minions = EntityManager.MinionsAndMonsters.GetLaneMinions(EntityManager.UnitTeam.Enemy, Player.Instance.Position, E.Range).ToArray();

            if (!tc && (eTarget != null || (!nowE && minions.Count() != 0)))
            {
                return;
            }

            E.Cast();
            nowE = false;
        }

        private static void Harass()
        {
            {
                if (qTarget != null)
                {
                    var predQ = Q2.GetPrediction(qTarget);
                    if (HarassMenu.Get<CheckBox>("HUse_Q").CurrentValue && (Q.IsReady() && qTarget.IsValidTarget(Q.Range)))
                    {
                        if (!cz && predQ.HitChance >= HitChance.High)
                        {
                            Q.Cast(predQ.CastPosition);
                        }
                        else
                        {
                            Q.Cast(qTarget.ServerPosition);
                        }
                    }
                }

                if (HarassMenu.Get<CheckBox>("HUse_E").CurrentValue && HarassMenu.Get<CheckBox>("E_LastHit").CurrentValue && E.IsReady()
                    && !player.IsZombie)
                {
                    if (!E.IsReady() || player.IsZombie)
                    {
                        return;
                    }

                    nowE = false;
                    var minions =
                        new List<Obj_AI_Base>(
                            EntityManager.MinionsAndMonsters.GetLaneMinions(EntityManager.UnitTeam.Enemy, Player.Instance.Position, E.Range).ToArray());
                    minions.RemoveAll(x => x.Health <= 5);
                    minions.RemoveAll(x => player.Distance(x.ServerPosition) > E.Range || x.Health > player.GetSpellDamage(eTarget, SpellSlot.E));
                    var jgm = minions.Any(x => x.Team == GameObjectTeam.Neutral);

                    if ((player.Spellbook.GetSpell(SpellSlot.E).ToggleState == 1 && (minions.Count >= 1 || jgm))
                        && (player.ManaPercent >= HarassMenu.Get<Slider>("HEPercent").CurrentValue))
                    {
                        E.Cast();
                    }
                    else if ((player.Spellbook.GetSpell(SpellSlot.E).ToggleState == 2 && (minions.Count == 0 && !jgm))
                             || !(player.ManaPercent >= HarassMenu.Get<Slider>("HEPercent").CurrentValue))
                    {
                        calcE(true);
                    }
                }

                if (HarassMenu.Get<CheckBox>("HUse_E").CurrentValue && E.IsReady() && !player.IsZombie)
                {
                    if (HarassMenu.Get<CheckBox>("HE_Auto_False").CurrentValue)
                    {
                        if (eTarget != null)
                        {
                            if (player.Spellbook.GetSpell(SpellSlot.E).ToggleState == 1)
                            {
                                if (player.Distance(eTarget.ServerPosition) <= E.Range
                                    && (player.ManaPercent >= HarassMenu.Get<Slider>("HEPercent").CurrentValue))
                                {
                                    nowE = true;
                                    E.Cast();
                                }
                            }
                            else if (player.Distance(eTarget.ServerPosition) >= E.Range
                                     || (player.ManaPercent <= HarassMenu.Get<Slider>("HEPercent").CurrentValue))
                            {
                                calcE(true);
                            }
                        }
                        else
                        {
                            calcE();
                        }
                    }
                    else
                    {
                        if (eTarget != null)
                        {
                            if (player.Spellbook.GetSpell(SpellSlot.E).ToggleState == 1)
                            {
                                if (player.Distance(eTarget.ServerPosition) <= E.Range
                                    && (player.ManaPercent >= HarassMenu.Get<Slider>("HEPercent").CurrentValue))
                                {
                                    nowE = true;
                                    E.Cast();
                                }
                            }
                            else if (player.ManaPercent <= HarassMenu.Get<Slider>("HEPercent").CurrentValue)
                            {
                                calcE(true);
                            }
                        }
                    }
                }
            }
        }

        private static float SaveR()
        {
            if (Q.IsReady())
            {
                return Q.Handle.SData.Mana;
            }
            if (W.IsReady())
            {
                return W.Handle.SData.Mana;
            }
            if (E.IsReady())
            {
                return E.Handle.SData.Mana;
            }
            return 0;
        }

        private static bool Combo()
        {
            var flags = Orbwalker.ActiveModesFlags;
            if (flags.HasFlag(Orbwalker.ActiveModes.Combo) && menuIni.Get<CheckBox>("Combo").CurrentValue)
            {
                var qm = ComboMenu.Get<CheckBox>("CUse_Q").CurrentValue;
                var wm = ComboMenu.Get<CheckBox>("CUse_W").CurrentValue;
                var em = ComboMenu.Get<CheckBox>("CUse_E").CurrentValue;

                {
                    if (eTarget == null && E.Handle.ToggleState == 2)
                    {
                        E.Cast();
                    }

                    if (eTarget != null)
                    {
                        if (em && E.IsReady() && !player.IsZombie)
                        {
                            if (eTarget.IsValidTarget(E.Range) && E.Handle.ToggleState != 2)
                            {
                                E.Cast();
                            }
                        }
                    }

                    double countmana = W.Handle.SData.Mana;
                    if (wm && W.IsReady() && wTarget.IsValid && wTarget != null)
                    {
                        double ds = 0;

                        if (R.IsReady())
                        {
                            ds += player.GetSpellDamage(qTarget, SpellSlot.R);
                            countmana += R.Handle.SData.Mana;
                        }

                        while (qTarget != null && ds < qTarget.MaxHealth)
                        {
                            var qd = player.GetSpellDamage(qTarget, SpellSlot.Q);

                            ds += qd;
                            if (Q.Handle != null)
                            {
                                countmana += Q.Handle.SData.Mana;
                            }
                        }

                        var predW = W.GetPrediction(wTarget);
                        if (player.MaxMana > countmana || qTarget.CountAlliesInRange(W.Range) > 1 || player.IsZombie)
                        {
                            W.Cast(predW.CastPosition);
                        }
                    }

                    if (qTarget == null || (!qm || !Q.IsReady() || !qTarget.IsValid))
                    {
                        return false;
                    }

                    var predQ = Q2.GetPrediction(qTarget);
                    if (!cz && predQ.HitChance >= HitChance.High)
                    {
                        Q.Cast(predQ.CastPosition);
                    }
                    else
                    {
                        Q.Cast(qTarget.ServerPosition);
                    }
                }
            }
            return true;
        }

        private static void JungleClear()
        {
            var canQ = LaneMenu.Get<CheckBox>("JUse_Q").CurrentValue && Q.IsReady();
            if (canQ && Q.IsReady() && player.ManaPercent >= LaneMenu.Get<Slider>("JQPercent").CurrentValue)
            {
                var minions1 = EntityManager.MinionsAndMonsters.GetJungleMonsters();
                if (minions1 == null || !minions1.Any())
                {
                    return;
                }
                var location =
                    GetBestCircularFarmLocation(
                        EntityManager.MinionsAndMonsters.GetJungleMonsters()
                            .Where(x => x.Distance(Player.Instance) <= Q.Range)
                            .Select(xm => xm.ServerPosition.To2D())
                            .ToList(),
                        Q.Width,
                        Q.Range);

                if (location.MinionsHit >= 1)
                {
                    Q.Cast(location.Position.To3D());
                }
            }
        }

        private static void LaneClear()
        {
            LastHit();
            var canQ = LaneMenu.Get<CheckBox>("FUse_Q").CurrentValue && Q.IsReady();
            if (canQ && Q.IsReady() && player.ManaPercent >= LaneMenu.Get<Slider>("FQPercent").CurrentValue)
            {
                var minions1 = EntityManager.MinionsAndMonsters.EnemyMinions;
                if (minions1 == null || !minions1.Any())
                {
                    return;
                }

                var location =
                    GetBestCircularFarmLocation(
                        EntityManager.MinionsAndMonsters.EnemyMinions.Where(x => x.Distance(Player.Instance) <= Q.Range)
                            .Select(xm => xm.ServerPosition.To2D())
                            .ToList(),
                        Q.Width,
                        Q.Range);
                if (location.MinionsHit >= 1)
                {
                    Q.Cast(location.Position.To3D());
                }
            }
        }

        private static void LastHit()
        {
            var canQ = LaneMenu.Get<CheckBox>("LUse_Q").CurrentValue && Q.IsReady();
            if (canQ && player.ManaPercent >= LaneMenu.Get<Slider>("LHQPercent").CurrentValue)
            {
                var minions1 = EntityManager.MinionsAndMonsters.EnemyMinions;
                if (minions1 == null || !minions1.Any())
                {
                    return;
                }

                var location =
                    GetBestCircularFarmLocation(
                        EntityManager.MinionsAndMonsters.EnemyMinions.Where(
                            x =>
                            x.Distance(Player.Instance) <= Q.Range && x.Health > 5 && (x.CountEnemiesInRange(155) == 0) && !x.IsDead && x.IsValid
                            && Prediction.Health.GetPrediction(x, (int)(Q.CastDelay * 1000)) < (2 * player.GetSpellDamage(x, SpellSlot.Q)))
                            .Select(xm => xm.ServerPosition.To2D())
                            .ToList(),
                        Q.Width + 5,
                        Q.Range);

                if (Q.IsReady() && location.MinionsHit == 1)
                {
                    Q.Cast(location.Position.To3D());
                }
            }

            if (canQ && player.ManaPercent >= LaneMenu.Get<Slider>("FQPercent").CurrentValue)
            {
                var minions1 = EntityManager.MinionsAndMonsters.EnemyMinions;
                if (minions1 == null || !minions1.Any())
                {
                    return;
                }

                var location =
                    GetBestCircularFarmLocation(
                        EntityManager.MinionsAndMonsters.EnemyMinions.Where(
                            x =>
                            x.Distance(Player.Instance) <= Q.Range && x.Health > 5 && !x.IsDead && x.IsValid
                            && (Prediction.Health.GetPrediction(x, (int)(Q.CastDelay * 1000)) < player.GetSpellDamage(x, SpellSlot.Q)))
                            .Select(xm => xm.ServerPosition.To2D())
                            .ToList(),
                        Q.Width,
                        Q.Range);

                if (Q.IsReady() && location.MinionsHit > 0)
                {
                    Q.Cast(location.Position.To3D());
                }
            }
        }

        private static void Ult()
        {
            // Kappa ult logic.
            var time = Environment.TickCount;
            var enemiesrange = ObjectManager.Player.Position.CountEnemiesInRange(UltMenu.Get<Slider>("Rranged").CurrentValue);
            var enemieinsrange = UltMenu.Get<Slider>("RnearEn").CurrentValue;
            foreach (var rtarget in
                EntityManager.Heroes.Enemies.Where(
                    x =>
                    x != null && x.IsValid && !x.IsDead && x.IsEnemy
                    && (!x.HasBuff("kindrednodeathbuff") || !x.HasBuff("Undying Rage") || !x.HasBuff("JudicatorIntervention")) && !x.IsZombie
                    && player.GetSpellDamage(x, SpellSlot.R) > Prediction.Health.GetPrediction(x, (int)(R.CastDelay * 1000f))
                    && x.CountAlliesInRange(750) < 1))
            {
                if (UltMenu.Get<CheckBox>("RnearE").CurrentValue && enemieinsrange <= enemiesrange)
                {
                    R.Cast(rtarget.Position);
                }

                if (!UltMenu.Get<CheckBox>("RnearE").CurrentValue)
                {
                    R.Cast(rtarget.Position);
                }

                if (player.IsZombie)
                {
                    R.Cast(rtarget.Position);
                }
            }
        }

        private static void Ks()
        {
            if (qTarget != null && KillStealMenu.Get<CheckBox>("KS").CurrentValue)
            {
                if (!cz && qTarget.TotalShieldHealth() < player.GetSpellDamage(qTarget, SpellSlot.Q))
                {
                    Q.Cast(Q.GetPrediction(qTarget).CastPosition);
                }
            }
        }

        // For debugging.

        // Find the points nearest the upper left, upper right,
        // lower left, and lower right corners.
        private static void GetMinMaxCorners(List<Vector2> points, ref Vector2 ul, ref Vector2 ur, ref Vector2 ll, ref Vector2 lr)
        {
            // Start with the first point as the solution.
            ul = points[0];
            ur = ul;
            ll = ul;
            lr = ul;

            // Search the other points.
            foreach (var pt in points)
            {
                if (-pt.X - pt.Y > -ul.X - ul.Y)
                {
                    ul = pt;
                }
                if (pt.X - pt.Y > ur.X - ur.Y)
                {
                    ur = pt;
                }
                if (-pt.X + pt.Y > -ll.X + ll.Y)
                {
                    ll = pt;
                }
                if (pt.X + pt.Y > lr.X + lr.Y)
                {
                    lr = pt;
                }
            }

            GMinMaxCorners = new[] { ul, ur, lr, ll }; // For debugging.
        }

        // Find a box that fits inside the MinMax quadrilateral.
        private static RectangleF GetMinMaxBox(List<Vector2> points)
        {
            // Find the MinMax quadrilateral.
            Vector2 ul = new Vector2(0, 0), ur = ul, ll = ul, lr = ul;
            GetMinMaxCorners(points, ref ul, ref ur, ref ll, ref lr);

            // Get the coordinates of a box that lies inside this quadrilateral.
            var xmin = ul.X;
            var ymin = ul.Y;

            var xmax = ur.X;
            if (ymin < ur.Y)
            {
                ymin = ur.Y;
            }

            if (xmax > lr.X)
            {
                xmax = lr.X;
            }
            var ymax = lr.Y;

            if (xmin < ll.X)
            {
                xmin = ll.X;
            }
            if (ymax > ll.Y)
            {
                ymax = ll.Y;
            }

            var result = new RectangleF(xmin, ymin, xmax - xmin, ymax - ymin);
            GMinMaxBox = result; // For debugging.
            return result;
        }

        // Cull points out of the convex hull that lie inside the
        // trapezoid defined by the vertices with smallest and
        // largest X and Y coordinates.
        // Return the points that are not culled.
        private static List<Vector2> HullCull(List<Vector2> points)
        {
            // Find a culling box.
            var cullingBox = GetMinMaxBox(points);

            // Cull the points.
            var results =
                points.Where(pt => pt.X <= cullingBox.Left || pt.X >= cullingBox.Right || pt.Y <= cullingBox.Top || pt.Y >= cullingBox.Bottom)
                    .ToList();

            GNonCulledPoints = new Vector2[results.Count]; // For debugging.
            results.CopyTo(GNonCulledPoints); // For debugging.
            return results;
        }

        // Return the points that make up a polygon's convex hull.
        // This method leaves the points list unchanged.
        public static List<Vector2> MakeConvexHull(List<Vector2> points)
        {
            // Cull.
            points = HullCull(points);

            // Find the remaining point with the smallest Y value.
            // if (there's a tie, take the one with the smaller X value.
            Vector2[] bestPt = { points[0] };
            foreach (var pt in points.Where(pt => (pt.Y < bestPt[0].Y) || ((pt.Y == bestPt[0].Y) && (pt.X < bestPt[0].X))))
            {
                bestPt[0] = pt;
            }

            // Move this point to the convex hull.
            var hull = new List<Vector2> { bestPt[0] };
            points.Remove(bestPt[0]);

            // Start wrapping up the other points.
            float sweepAngle = 0;
            for (;;)
            {
                // If all of the points are on the hull, we're done.
                if (points.Count == 0)
                {
                    break;
                }

                // Find the point with smallest AngleValue
                // from the last point.
                var x = hull[hull.Count - 1].X;
                var y = hull[hull.Count - 1].Y;
                bestPt[0] = points[0];
                float bestAngle = 3600;

                // Search the rest of the points.
                foreach (var pt in points)
                {
                    var testAngle = AngleValue(x, y, pt.X, pt.Y);
                    if ((testAngle >= sweepAngle) && (bestAngle > testAngle))
                    {
                        bestAngle = testAngle;
                        bestPt[0] = pt;
                    }
                }

                // See if the first point is better.
                // If so, we are done.
                var firstAngle = AngleValue(x, y, hull[0].X, hull[0].Y);
                if ((firstAngle >= sweepAngle) && (bestAngle >= firstAngle))
                {
                    // The first point is better. We're done.
                    break;
                }

                // Add the best point to the convex hull.
                hull.Add(bestPt[0]);
                points.Remove(bestPt[0]);

                sweepAngle = bestAngle;
            }

            return hull;
        }

        // Return a number that gives the ordering of angles
        // WRST horizontal from the point (x1, y1) to (x2, y2).
        // In other words, AngleValue(x1, y1, x2, y2) is not
        // the angle, but if:
        //   Angle(x1, y1, x2, y2) > Angle(x1, y1, x2, y2)
        // then
        //   AngleValue(x1, y1, x2, y2) > AngleValue(x1, y1, x2, y2)
        // this angle is greater than the angle for another set
        // of points,) this number for
        //
        // This function is dy / (dy + dx).
        private static float AngleValue(float x1, float y1, float x2, float y2)
        {
            float t;

            var dx = x2 - x1;
            var ax = Math.Abs(dx);
            var dy = y2 - y1;
            var ay = Math.Abs(dy);
            if (ax + ay == 0)
            {
                // if (the two points are the same, return 360.
                t = 360f / 9f;
            }
            else
            {
                t = dy / (ax + ay);
            }
            if (dx < 0)
            {
                t = 2 - t;
            }
            else if (dy < 0)
            {
                t = 4 + t;
            }
            return t * 90;
        }

        // Find a minimal bounding circle.
        public static void FindMinimalBoundingCircle(List<Vector2> points, out Vector2 center, out float radius)
        {
            // Find the convex hull.
            var hull = MakeConvexHull(points);

            // The best solution so far.
            var bestCenter = points[0];
            var bestRadius2 = float.MaxValue;

            // Look at pairs of hull points.
            for (var i = 0; i < hull.Count - 1; i++)
            {
                for (var j = i + 1; j < hull.Count; j++)
                {
                    // Find the circle through these two points.
                    var testCenter = new Vector2((hull[i].X + hull[j].X) / 2f, (hull[i].Y + hull[j].Y) / 2f);
                    var dx = testCenter.X - hull[i].X;
                    var dy = testCenter.Y - hull[i].Y;
                    var testRadius2 = dx * dx + dy * dy;

                    // See if this circle would be an improvement.
                    if (testRadius2 < bestRadius2)
                    {
                        // See if this circle encloses all of the points.
                        if (CircleEnclosesPoints(testCenter, testRadius2, points, i, j, -1))
                        {
                            // Save this solution.
                            bestCenter = testCenter;
                            bestRadius2 = testRadius2;
                        }
                    }
                } // for i
            } // for j

            // Look at triples of hull points.
            for (var i = 0; i < hull.Count - 2; i++)
            {
                for (var j = i + 1; j < hull.Count - 1; j++)
                {
                    for (var k = j + 1; k < hull.Count; k++)
                    {
                        // Find the circle through these three points.
                        Vector2 testCenter;
                        float testRadius2;
                        FindCircle(hull[i], hull[j], hull[k], out testCenter, out testRadius2);

                        // See if this circle would be an improvement.
                        if (testRadius2 < bestRadius2)
                        {
                            // See if this circle encloses all of the points.
                            if (CircleEnclosesPoints(testCenter, testRadius2, points, i, j, k))
                            {
                                // Save this solution.
                                bestCenter = testCenter;
                                bestRadius2 = testRadius2;
                            }
                        }
                    } // for k
                } // for i
            } // for j

            center = bestCenter;
            if (bestRadius2 == float.MaxValue)
            {
                radius = 0;
            }
            else
            {
                radius = (float)Math.Sqrt(bestRadius2);
            }
        }

        // Return true if the indicated circle encloses all of the points.
        private static bool CircleEnclosesPoints(Vector2 center, float radius2, List<Vector2> points, int skip1, int skip2, int skip3)
        {
            return (from point in points.Where((t, i) => (i != skip1) && (i != skip2) && (i != skip3))
                    let dx = center.X - point.X
                    let dy = center.Y - point.Y
                    select dx * dx + dy * dy).All(testRadius2 => !(testRadius2 > radius2));
        }

        // Find a circle through the three points.
        private static void FindCircle(Vector2 a, Vector2 b, Vector2 c, out Vector2 center, out float radius2)
        {
            // Get the perpendicular bisector of (x1, y1) and (x2, y2).
            var x1 = (b.X + a.X) / 2;
            var y1 = (b.Y + a.Y) / 2;
            var dy1 = b.X - a.X;
            var dx1 = -(b.Y - a.Y);

            // Get the perpendicular bisector of (x2, y2) and (x3, y3).
            var x2 = (c.X + b.X) / 2;
            var y2 = (c.Y + b.Y) / 2;
            var dy2 = c.X - b.X;
            var dx2 = -(c.Y - b.Y);

            // See where the lines intersect.
            var cx = (y1 * dx1 * dx2 + x2 * dx1 * dy2 - x1 * dy1 * dx2 - y2 * dx1 * dx2) / (dx1 * dy2 - dy1 * dx2);
            var cy = (cx - x1) * dy1 / dx1 + y1;
            center = new Vector2(cx, cy);

            var dx = cx - a.X;
            var dy = cy - a.Y;
            radius2 = dx * dx + dy * dy;
        }

        public struct MecCircle
        {
            public Vector2 Center;

            public float Radius;

            public MecCircle(Vector2 center, float radius)
            {
                this.Center = center;
                this.Radius = radius;
            }
        }

        public static MecCircle GetMec(List<Vector2> points)
        {
            var center = new Vector2();
            float radius;

            var convexHull = MakeConvexHull(points);
            FindMinimalBoundingCircle(convexHull, out center, out radius);
            return new MecCircle(center, radius);
        }

        public static FarmLocation GetBestCircularFarmLocation(List<Vector2> minionPositions, float width, float range, int useMecMax = 9)
        {
            var result = new Vector2();
            var minionCount = 0;
            var startPos = ObjectManager.Player.ServerPosition.To2D();

            range = range * range;

            if (minionPositions.Count == 0)
            {
                return new FarmLocation(result, minionCount);
            }

            /* Use MEC to get the best positions only when there are less than 9 positions because it causes lag with more. */
            if (minionPositions.Count <= useMecMax)
            {
                var subGroups = GetCombinations(minionPositions);
                foreach (var subGroup in subGroups)
                {
                    if (subGroup.Count > 0)
                    {
                        var circle = GetMec(subGroup);

                        if (circle.Radius <= width && circle.Center.Distance(startPos, true) <= range)
                        {
                            minionCount = subGroup.Count;
                            return new FarmLocation(circle.Center, minionCount);
                        }
                    }
                }
            }
            else
            {
                foreach (var pos in minionPositions)
                {
                    if (pos.Distance(startPos, true) <= range)
                    {
                        var count = minionPositions.Count(pos2 => pos.Distance(pos2, true) <= width * width);

                        if (count >= minionCount)
                        {
                            result = pos;
                            minionCount = count;
                        }
                    }
                }
            }

            return new FarmLocation(result, minionCount);
        }

        private static List<List<Vector2>> GetCombinations(List<Vector2> allValues)
        {
            var collection = new List<List<Vector2>>();
            for (var counter = 0; counter < (1 << allValues.Count); ++counter)
            {
                var combination = allValues.Where((t, i) => (counter & (1 << i)) == 0).ToList();

                collection.Add(combination);
            }
            return collection;
        }

        public struct FarmLocation
        {
            public int MinionsHit;

            public Vector2 Position;

            public FarmLocation(Vector2 position, int minionsHit)
            {
                this.Position = position;
                this.MinionsHit = minionsHit;
            }
        }
    }
}