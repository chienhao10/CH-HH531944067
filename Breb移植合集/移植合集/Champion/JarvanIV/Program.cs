using System;
using System.Collections.Generic;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using LeagueSharp.Common;
using PortAIO.Utility.BrianSharp;
using SharpDX;
using Color = System.Drawing.Color;
using Prediction = LeagueSharp.Common.Prediction;
using Spell = LeagueSharp.Common.Spell;
using Utility = LeagueSharp.Common.Utility;
//using BrianSharp.Common;

namespace BrianSharp.Plugin
{
    internal class JarvanIV : Helper
    {
        private const int RWidth = 325;
        private static bool _rCasted;
        public static Spell Q, Q2, W, E, R;

        public static Menu config, comboMenu, harassMenu, clearMenu, lastHitMenu, fleeMenu, miscMenu, drawMenu;

        public static AIHeroClient Player
        {
            get { return ObjectManager.Player; }
        }

        private static IEnumerable<Obj_AI_Minion> Flag
        {
            get
            {
                return
                    ObjectManager.Get<Obj_AI_Minion>()
                        .Where(i => i.IsValidTarget(Q2.Range) && i.IsAlly && i.Name == "Beacon");
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

        public static void OnLoad()
        {
            Q = new Spell(SpellSlot.Q, 770);
            Q2 = new Spell(SpellSlot.Q, 880);
            W = new Spell(SpellSlot.W, 520);
            E = new Spell(SpellSlot.E, 860, DamageType.Magical);
            R = new Spell(SpellSlot.R, 650);
            Q.SetSkillshot(0.6f, 70, float.MaxValue, false, SkillshotType.SkillshotLine);
            Q2.SetSkillshot(0.25f, 140, 1450, false, SkillshotType.SkillshotLine);
            E.SetSkillshot(0.5f, 175, float.MaxValue, false, SkillshotType.SkillshotCircle);

            config = MainMenu.AddMenu("皇子", Player.ChampionName + "_Plugin");

            comboMenu = config.AddSubMenu("连招", "Combo");
            comboMenu.Add("Q", new CheckBox("使用 Q"));
            comboMenu.Add("QFlagRange", new Slider("-> 有旗子的话 Q <", 400, 100, 880));
            comboMenu.Add("W", new CheckBox("使用 W"));
            comboMenu.Add("WHpU", new Slider("-> 如果玩家HP <", 40));
            comboMenu.Add("WCountA", new Slider("-> 或者敌人数量 >=", 2, 1, 5));
            comboMenu.Add("E", new CheckBox("使用 E"));
            comboMenu.Add("EQ", new CheckBox("-> 保留 E (Q 必须开启)"));
            comboMenu.Add("R", new CheckBox("使用 R"));
            comboMenu.Add("RHpU", new Slider("-> 如果敌人Hp <", 40));
            comboMenu.Add("RCountA", new Slider("-> 或则敌人数量 >=", 2, 1, 5));

            harassMenu = config.AddSubMenu("骚扰", "Harass");
            harassMenu.Add("AutoQ", new KeyBind("自动 Q", false, KeyBind.BindTypes.PressToggle, 'H'));
            harassMenu.Add("AutoQMpA", new Slider("-> 如果蓝量 >=", 50));
            harassMenu.Add("Q", new CheckBox("使用 Q"));

            clearMenu = config.AddSubMenu("清线", "Clear");
            clearMenu.Add("Q", new CheckBox("使用 Q"));
            clearMenu.Add("W", new CheckBox("使用 W"));
            clearMenu.Add("WHpU", new Slider("-> 如果 Hp <", 40));
            clearMenu.Add("E", new CheckBox("使用 E"));

            lastHitMenu = config.AddSubMenu("尾兵", "LastHit");
            lastHitMenu.Add("Q", new CheckBox("使用 Q"));

            fleeMenu = config.AddSubMenu("逃跑", "Flee");
            fleeMenu.Add("EQ", new CheckBox("使用 EQ"));
            fleeMenu.Add("W", new CheckBox("使用 W 减速敌人"));

            miscMenu = config.AddSubMenu("杂项", "Misc");
            miscMenu.Add("Packet", new CheckBox("封包?"));
            miscMenu.AddSeparator();
            miscMenu.AddGroupLabel("抢头设置");
            miscMenu.Add("Q", new CheckBox("使用 Q"));
            miscMenu.Add("E", new CheckBox("使用 E"));
            miscMenu.Add("R", new CheckBox("使用 R"));
            miscMenu.AddSeparator();

            miscMenu.AddGroupLabel("技能打断");
            miscMenu.Add("EQ", new CheckBox("使用 EQ"));
            foreach (
                var spell in
                    Interrupter.Spells.Where(i => HeroManager.Enemies.Any(a => i.ChampionName == a.ChampionName)))
            {
                miscMenu.Add(spell.ChampionName + "_" + spell.Slot,
                    new CheckBox("-> Skill " + spell.Slot + " Of " + spell.ChampionName));
            }

            drawMenu = config.AddSubMenu("线圈", "Draw");
            drawMenu.Add("Q", new CheckBox("Q 范围"));
            drawMenu.Add("W", new CheckBox("W 范围"));
            drawMenu.Add("E", new CheckBox("E 范围"));
            drawMenu.Add("R", new CheckBox("R 范围"));

            Game.OnUpdate += OnUpdate;
            Drawing.OnDraw += OnDraw;
            Interrupter.OnPossibleToInterrupt += OnPossibleToInterrupt;
            Obj_AI_Base.OnProcessSpellCast += OnProcessSpellCast;
        }

        private static void OnUpdate(EventArgs args)
        {
            if (Player.IsDead || MenuGUI.IsChatOpen || Player.IsRecalling())
            {
                return;
            }
            if (R.IsReady() && _rCasted && Player.CountEnemiesInRange(RWidth) == 0 &&
                R.Cast(getCheckBoxItem(miscMenu, "Packet")))
            {
                return;
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                Fight(comboMenu);
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass))
            {
                Fight(harassMenu);
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear) ||
                Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear))
            {
                Clear();
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LastHit))
            {
                LastHit();
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Flee))
            {
                Flee();
            }

            AutoQ();
            KillSteal();
        }

        private static void OnDraw(EventArgs args)
        {
            if (Player.IsDead)
            {
                return;
            }
            if (getCheckBoxItem(drawMenu, "Q") && Q.Level > 0)
            {
                Render.Circle.DrawCircle(Player.Position, Q.Range, Q.IsReady() ? Color.Green : Color.Red);
            }
            if (getCheckBoxItem(drawMenu, "W") && W.Level > 0)
            {
                Render.Circle.DrawCircle(Player.Position, W.Range, W.IsReady() ? Color.Green : Color.Red);
            }
            if (getCheckBoxItem(drawMenu, "E") && E.Level > 0)
            {
                Render.Circle.DrawCircle(Player.Position, E.Range, E.IsReady() ? Color.Green : Color.Red);
            }
            if (getCheckBoxItem(drawMenu, "R") && R.Level > 0)
            {
                Render.Circle.DrawCircle(Player.Position, R.Range, R.IsReady() ? Color.Green : Color.Red);
            }
        }

        private static void OnPossibleToInterrupt(AIHeroClient unit, InterruptableSpell spell)
        {
            if (Player.IsDead || !getCheckBoxItem(miscMenu, "EQ") ||
                !getCheckBoxItem(miscMenu, unit.ChampionName + "_" + spell.Slot) || !Q.IsReady())
            {
                return;
            }
            if (E.CanCast(unit) && Player.Mana >= Q.Instance.SData.Mana + E.Instance.SData.Mana)
            {
                var predE = E.GetPrediction(unit);
                if (predE.Hitchance >= E.MinHitChance &&
                    E.Cast(
                        predE.UnitPosition.Extend(Player.ServerPosition, -E.Width/(unit.IsFacing(Player) ? 2 : 1)),
                        getCheckBoxItem(miscMenu, "Packet")) &&
                    Q.Cast(predE.UnitPosition, getCheckBoxItem(miscMenu, "Packet")))
                {
                    return;
                }
            }
            foreach (var flag in
                Flag.Where(i => Q2.WillHit(unit, i.ServerPosition)))
            {
                Q.Cast(flag.ServerPosition, getCheckBoxItem(miscMenu, "Packet"));
            }
        }

        private static void OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!sender.IsMe)
            {
                return;
            }
            if (args.SData.Name == "JarvanIVCataclysm")
            {
                _rCasted = true;
                Utility.DelayAction.Add(3500, () => _rCasted = false);
            }
        }

        private static void Fight(Menu mode)
        {
            if (mode == comboMenu && getCheckBoxItem(mode, "E") && E.IsReady())
            {
                if (getCheckBoxItem(mode, "EQ") && getCheckBoxItem(mode, "Q") &&
                    (Player.Mana < E.Instance.SData.Mana + Q.Instance.SData.Mana || (!Q.IsReady() && Q.IsReady(4000))))
                {
                    return;
                }
                var target = E.GetTarget(getCheckBoxItem(mode, "Q") && Q.IsReady() ? 0 : E.Width/2);
                if (target != null)
                {
                    var predE = E.GetPrediction(target);
                    if (predE.Hitchance >= E.MinHitChance)
                    {
                        E.Cast(
                            predE.UnitPosition.Extend(Player.ServerPosition, -E.Width/(target.IsFacing(Player) ? 2 : 1)),
                            getCheckBoxItem(miscMenu, "Packet"));
                        if (getCheckBoxItem(mode, "Q") && Q.IsReady())
                        {
                            Q.Cast(predE.UnitPosition, getCheckBoxItem(miscMenu, "Packet"));
                        }
                        return;
                    }
                }
            }

            if (getCheckBoxItem(mode, "Q") && Q.IsReady())
            {
                if (mode == comboMenu)
                {
                    if (getCheckBoxItem(mode, "E") && getCheckBoxItem(mode, "EQ") &&
                        Player.Mana >= E.Instance.SData.Mana + Q.Instance.SData.Mana && E.IsReady(2000))
                    {
                        return;
                    }
                    var target = Q2.GetTarget();
                    if (getCheckBoxItem(mode, "E") && target != null &&
                        Flag.Where(
                            i =>
                                Player.LSDistance(i) < getSliderItem(mode, "QFlagRange") &&
                                Q2.WillHit(target, i.ServerPosition))
                            .Any(i => Q.Cast(i.ServerPosition, getCheckBoxItem(miscMenu, "Packet"))))
                    {
                        return;
                    }
                }
                Q.CastOnBestTarget(0, getCheckBoxItem(miscMenu, "Packet"));
            }
            if (mode != comboMenu)
            {
                return;
            }
            if (getCheckBoxItem(mode, "R") && R.IsReady() && !_rCasted)
            {
                var obj = (from i in HeroManager.Enemies.Where(i => i.IsValidTarget(R.Range))
                    let enemy = GetRTarget(i.ServerPosition)
                    where
                        (enemy.Count > 1 && R.IsKillable(i)) ||
                        enemy.Any(a => a.HealthPercent < getSliderItem(mode, "RHpU")) ||
                        enemy.Count >= getSliderItem(mode, "RCountA")
                    select i).MaxOrDefault(i => GetRTarget(i.ServerPosition).Count);
                if (obj != null && R.CastOnUnit(obj, getCheckBoxItem(miscMenu, "Packet")))
                {
                    return;
                }
            }
            if (getCheckBoxItem(mode, "W") && W.IsReady() &&
                (Player.HealthPercent < getSliderItem(mode, "WHpU") ||
                 Player.CountEnemiesInRange(W.Range) >= getSliderItem(mode, "WCountA")))
            {
                W.Cast(getCheckBoxItem(miscMenu, "Packet"));
            }
        }

        private static void Clear()
        {
            if (getCheckBoxItem(clearMenu, "E") && E.IsReady())
            {
                var minionObj = GetMinions(E.Range, MinionTypes.All, MinionTeam.NotAlly, MinionOrderTypes.MaxHealth);
                if (minionObj.Any())
                {
                    var pos = E.GetCircularFarmLocation(minionObj.Cast<Obj_AI_Base>().ToList());
                    if (pos.MinionsHit > 1)
                    {
                        if (E.Cast(pos.Position, getCheckBoxItem(miscMenu, "Packet")))
                        {
                            if (getCheckBoxItem(clearMenu, "Q") && Q.IsReady())
                            {
                                Q.Cast(pos.Position, getCheckBoxItem(miscMenu, "Packet"));
                            }
                            return;
                        }
                    }
                    else
                    {
                        var obj = minionObj.FirstOrDefault(i => i.MaxHealth >= 1200);
                        if (obj != null && E.Cast(obj, getCheckBoxItem(miscMenu, "Packet")).IsCasted())
                        {
                            return;
                        }
                    }
                }
            }
            if (getCheckBoxItem(clearMenu, "Q") && Q.IsReady())
            {
                var minionObj =
                    GetMinions(Q2.Range, MinionTypes.All, MinionTeam.NotAlly, MinionOrderTypes.MaxHealth)
                        .Cast<Obj_AI_Base>()
                        .ToList();
                if (minionObj.Any() &&
                    (!getCheckBoxItem(clearMenu, "E") || !E.IsReady() ||
                     (E.IsReady() && E.GetCircularFarmLocation(minionObj).MinionsHit == 1)))
                {
                    if (getCheckBoxItem(clearMenu, "E") &&
                        Flag.Where(i => minionObj.Count(a => Q2.WillHit(a, i.ServerPosition)) > 1)
                            .Any(i => Q.Cast(i.ServerPosition, getCheckBoxItem(miscMenu, "Packet"))))
                    {
                        return;
                    }
                    var pos = Q.GetLineFarmLocation(minionObj.Where(i => Q.IsInRange(i)).ToList());
                    if (pos.MinionsHit > 0 && Q.Cast(pos.Position, getCheckBoxItem(miscMenu, "Packet")))
                    {
                        return;
                    }
                }
            }
            if (getCheckBoxItem(clearMenu, "W") && W.IsReady() &&
                Player.HealthPercent < getSliderItem(clearMenu, "WHpU") &&
                GetMinions(W.Range, MinionTypes.All, MinionTeam.NotAlly).Any() &&
                W.Cast(getCheckBoxItem(miscMenu, "Packet")))
            {
            }
        }

        private static void LastHit()
        {
            if (!getCheckBoxItem(lastHitMenu, "Q") || !Q.IsReady())
            {
                return;
            }
            var obj =
                GetMinions(Q.Range, MinionTypes.All, MinionTeam.NotAlly, MinionOrderTypes.MaxHealth)
                    .FirstOrDefault(i => Q.IsKillable(i));
            if (obj == null)
            {
                return;
            }
            Q.Cast(obj, getCheckBoxItem(miscMenu, "Packet"));
        }

        private static void Flee()
        {
            if (getCheckBoxItem(fleeMenu, "EQ") && Q.IsReady() && E.IsReady() &&
                Player.Mana >= Q.Instance.SData.Mana + E.Instance.SData.Mana &&
                E.Cast(Game.CursorPos, getCheckBoxItem(miscMenu, "Packet")) &&
                Q.Cast(Game.CursorPos, getCheckBoxItem(miscMenu, "Packet")))
            {
                return;
            }
            if (getCheckBoxItem(fleeMenu, "W") && W.IsReady() && !Q.IsReady() && W.GetTarget() != null)
            {
                W.Cast(getCheckBoxItem(miscMenu, "Packet"));
            }
        }

        private static void AutoQ()
        {
            if (!getKeyBindItem(harassMenu, "AutoQ") || Player.ManaPercent < getSliderItem(harassMenu, "AutoQMpA"))
            {
                return;
            }

            Q.CastOnBestTarget(0, getCheckBoxItem(miscMenu, "Packet"), true);
        }

        private static void KillSteal()
        {
            if (getCheckBoxItem(miscMenu, "Q") && Q.IsReady())
            {
                var target = Q.GetTarget();
                if (target != null && Q.IsKillable(target) &&
                    Q.Cast(target, getCheckBoxItem(miscMenu, "Packet")).IsCasted())
                {
                    return;
                }
            }
            if (getCheckBoxItem(miscMenu, "E") && E.IsReady())
            {
                var target = E.GetTarget(E.Width/2);
                if (target != null && E.IsKillable(target) &&
                    E.Cast(target, getCheckBoxItem(miscMenu, "Packet")).IsCasted())
                {
                    return;
                }
            }
            if (getCheckBoxItem(miscMenu, "R") && R.IsReady())
            {
                var target = R.GetTarget();
                if (target != null && R.IsKillable(target))
                {
                    R.CastOnUnit(target, getCheckBoxItem(miscMenu, "Packet"));
                }
            }
        }

        private static List<AIHeroClient> GetRTarget(Vector3 pos)
        {
            return
                HeroManager.Enemies.Where(
                    i => i.IsValidTarget() && pos.LSDistance(Prediction.GetPrediction(i, 0.25f).UnitPosition) < RWidth)
                    .ToList();
        }
    }
}
