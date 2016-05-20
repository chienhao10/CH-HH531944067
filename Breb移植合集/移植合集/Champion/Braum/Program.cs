using System;
using System.Drawing;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using LeagueSharp.Common;
using SebbyLib;
using Spell = LeagueSharp.Common.Spell;

namespace PortAIO.Champion.Braum
{
    public class Program
    {
        private static readonly Menu Config = SebbyLib.Program.Config;
        public static Menu drawMenu, qMenu, ewMenu, rMenu, harassMenu;
        public static Spell Q, W, E, R;
        public static float QMANA = 0, WMANA = 0, EMANA = 0, RMANA = 0;

        public static AIHeroClient Player
        {
            get { return ObjectManager.Player; }
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

        public static void LoadOKTW()
        {
            Q = new Spell(SpellSlot.Q, 1000);
            W = new Spell(SpellSlot.W, 650);
            E = new Spell(SpellSlot.E, 0);
            R = new Spell(SpellSlot.R, 1250);

            Q.SetSkillshot(0.25f, 60f, 1700f, true, SkillshotType.SkillshotLine);
            R.SetSkillshot(0.5f, 115f, 1400f, false, SkillshotType.SkillshotLine);

            drawMenu = Config.AddSubMenu("线圈");
            drawMenu.Add("notif", new CheckBox("提示 (计时)"));
            drawMenu.Add("noti", new CheckBox("显示抢头提示"));
            drawMenu.Add("qRange", new CheckBox("Q 范围"));
            drawMenu.Add("eRange", new CheckBox("E 范围"));
            drawMenu.Add("rRange", new CheckBox("R 范围"));
            drawMenu.Add("onlyRdy", new CheckBox("只显示无冷却技能"));

            qMenu = Config.AddSubMenu("Q 设置");
            qMenu.Add("autoQ", new CheckBox("自动 Q"));
            qMenu.Add("AGCq", new CheckBox("自动防突进 Q"));

            ewMenu = Config.AddSubMenu("E W 盾牌设置");
            ewMenu.AddGroupLabel("技能设置");
            foreach (var enemy in ObjectManager.Get<AIHeroClient>().Where(enemy => enemy.IsEnemy))
            {
                for (var i = 0; i < 4; i++)
                {
                    var spell = enemy.Spellbook.Spells[i];
                    if (spell.SData.TargettingType != SpellDataTargetType.Self &&
                        spell.SData.TargettingType != SpellDataTargetType.SelfAndUnit)
                    {
                        ewMenu.Add("spell" + spell.SData.Name, new CheckBox(enemy.ChampionName + " : " + spell.Name));
                    }
                }
            }
            ewMenu.Add("autoE", new CheckBox("自动 E"));
            ewMenu.Add("Edmg", new Slider("用盾吸收来自 X %的伤害", 20));
            ewMenu.AddGroupLabel("使用在 : ");
            foreach (var enemy in ObjectManager.Get<AIHeroClient>().Where(enemy => enemy.Team == Player.Team))
            {
                ewMenu.Add("Eon" + enemy.NetworkId, new CheckBox(enemy.ChampionName));
            }
            ewMenu.AddGroupLabel("防突进");
            ewMenu.Add("AGC", new CheckBox("防突进 E + W"));
            foreach (var enemy in ObjectManager.Get<AIHeroClient>().Where(enemy => enemy.Team != Player.Team))
            {
                ewMenu.Add("gapcloser" + enemy.NetworkId, new CheckBox("Antigapclose : " + enemy.ChampionName));
            }

            rMenu = Config.AddSubMenu("R 设置");
            rMenu.Add("autoR", new CheckBox("自动 R"));
            rMenu.Add("useR", new KeyBind("半自动 R", false, KeyBind.BindTypes.HoldActive, 'T'));
            rMenu.Add("rCombo", new CheckBox("连招中使用"));
            rMenu.Add("rCount", new Slider("自动R如果能命中敌人数 X", 3, 0, 5));
            rMenu.Add("rCc", new CheckBox("自动R无法移动目标 （韩国模式）"));
            rMenu.Add("OnInterruptableSpell", new CheckBox("可打断技能"));
            foreach (var enemy in ObjectManager.Get<AIHeroClient>().Where(enemy => enemy.IsEnemy))
            {
            rMenu.Add("Rmode" + enemy.NetworkId, new Slider(enemy.ChampionName + " 0 : 正常 | 1 : 一直 | 2 : 从不 | 3 : 正常 + 防突进 R"));
            }

            harassMenu = Config.AddSubMenu("骚扰");
            foreach (var enemy in ObjectManager.Get<AIHeroClient>().Where(enemy => enemy.Team != Player.Team))
            {
                harassMenu.Add("haras" + enemy.NetworkId, new CheckBox(enemy.ChampionName));
            }

            Game.OnUpdate += Game_OnGameUpdate;
            Drawing.OnDraw += Drawing_OnDraw;
            Obj_AI_Base.OnProcessSpellCast += Obj_AI_Base_OnProcessSpellCast;
            AntiGapcloser.OnEnemyGapcloser += AntiGapcloser_OnEnemyGapcloser;
        }

        private static void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            var t = gapcloser.Sender;
            if (!getCheckBoxItem(ewMenu, "gapcloser" + t.NetworkId))
                return;

            if (getCheckBoxItem(ewMenu, "AGC"))
            {
                if (W.IsReady() && gapcloser.End.LSDistance(Player.Position) < gapcloser.Start.LSDistance(Player.Position))
                {
                    var allyHero = SebbyLib.Program.Allies.Where(ally => ally.LSDistance(Player) <= W.Range && !ally.IsMe)
                        .OrderBy(ally => ally.LSDistance(gapcloser.End)).FirstOrDefault();

                    if (allyHero != null && getCheckBoxItem(ewMenu, "Eon" + allyHero.NetworkId))
                        W.Cast(allyHero);
                }
                if (E.IsReady())
                    LeagueSharp.Common.Utility.DelayAction.Add(200, () => E.Cast(t.ServerPosition));
            }

            if (Q.IsReady() && getCheckBoxItem(qMenu, "AGCq"))
                Q.Cast(t);

            if (R.IsReady() && getSliderItem(rMenu, "Rmode" + t.NetworkId) == 3)
                R.Cast(t);
        }

        private static void Game_OnGameUpdate(EventArgs args)
        {
            if (R.IsReady())
            {
                if (getKeyBindItem(rMenu, "useR"))
                {
                    var t = TargetSelector.GetTarget(R.Range, DamageType.Physical);
                    if (t.IsValidTarget())
                        R.Cast(t, true, true);
                }
            }

            if (SebbyLib.Program.LagFree(2) && Q.IsReady() && getCheckBoxItem(qMenu, "autoQ"))
                LogicQ();

            if (SebbyLib.Program.LagFree(4) && R.IsReady() && getCheckBoxItem(rMenu, "autoR"))
                LogicR();
        }

        private static void LogicQ()
        {
            var t = TargetSelector.GetTarget(500, DamageType.Physical);

            if (!t.IsValidTarget())
                t = TargetSelector.GetTarget(Q.Range, DamageType.Physical);

            if (t.IsValidTarget())
            {
                if (SebbyLib.Program.Combo && Player.Mana > RMANA + QMANA)
                    SebbyLib.Program.CastSpell(Q, t);
                else if (SebbyLib.Program.Farm)
                {
                    foreach (
                        var enemy in
                            SebbyLib.Program.Enemies.Where(
                                enemy =>
                                    enemy.IsValidTarget(Q.Range) &&
                                    getCheckBoxItem(harassMenu, "haras" + enemy.NetworkId)))
                    {
                        SebbyLib.Program.CastSpell(Q, enemy);
                    }
                }
                if (!SebbyLib.Program.None && Player.Mana > RMANA + QMANA + EMANA)
                {
                    foreach (
                        var enemy in
                            SebbyLib.Program.Enemies.Where(
                                enemy => enemy.IsValidTarget(Q.Range) && !OktwCommon.CanMove(enemy)))
                        Q.Cast(enemy, true);
                }
            }
        }

        private static void LogicR()
        {
            var rCount = getSliderItem(rMenu, "rCount");
            foreach (
                var t in
                    SebbyLib.Program.Enemies.Where(t => t.IsValidTarget(R.Range) && OktwCommon.ValidUlt(t))
                        .OrderBy(t => t.Health))
            {
                var Rmode = getSliderItem(rMenu, "Rmode" + t.NetworkId);

                if (Rmode == 2)
                    continue;
                if (Rmode == 1)
                    SebbyLib.Program.CastSpell(R, t);

                if (rCount > 0)
                    R.CastIfWillHit(t, rCount);

                if (getCheckBoxItem(rMenu, "rCc") && !OktwCommon.CanMove(t) && t.HealthPercent > 20*t.CountAlliesInRange(500))
                {
                    var t1 = t;
                    LeagueSharp.Common.Utility.DelayAction.Add(800 - (int) (Player.LSDistance(t.Position)/2), () => CastRtime(t1));
                }

                if (getCheckBoxItem(rMenu, "rCombo") && SebbyLib.Program.Combo)
                {
                    SebbyLib.Program.CastSpell(R, t);
                    return;
                }
            }
        }

        private static void CastRtime(AIHeroClient t)
        {
            if (OktwCommon.ValidUlt(t))
                R.Cast(t, true, true);
        }

        private static void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!sender.IsEnemy)
                return;

            if (ewMenu["spell" + args.SData.Name] == null || !getCheckBoxItem(ewMenu, "spell" + args.SData.Name))
                return;

            if (E.IsReady() && getCheckBoxItem(ewMenu, "autoE") && OktwCommon.CanHitSkillShot(Player, args))
            {
                E.Cast(sender.Position);
            }

            if (W.IsReady() && args.SData.MissileSpeed > 0)
            {
                foreach (
                    var ally in
                        SebbyLib.Program.Allies.Where(
                            ally =>
                                ally.IsValid && Player.LSDistance(ally.ServerPosition) < W.Range &&
                                getCheckBoxItem(ewMenu, "Eon" + ally.NetworkId)))
                {
                    if (OktwCommon.CanHitSkillShot(ally, args) ||
                        OktwCommon.GetIncomingDamage(ally, 1) > ally.Health*getSliderItem(ewMenu, "Edmg")*0.01)
                    {
                        if (E.IsReady())
                            LeagueSharp.Common.Utility.DelayAction.Add(200, () => E.Cast(sender.Position));

                        if (Player.HealthPercent < 20 && !ally.IsMe)
                            continue;
                        if (Player.HealthPercent < 50 && !ally.IsMe && ally.UnderTurret(true))
                            continue;

                        W.Cast(ally);
                    }
                }
            }
        }

        private static void Drawing_OnDraw(EventArgs args)
        {
            if (getCheckBoxItem(drawMenu, "qRange"))
            {
                if (getCheckBoxItem(drawMenu, "onlyRdy"))
                {
                    if (Q.IsReady())
                        LeagueSharp.Common.Utility.DrawCircle(ObjectManager.Player.Position, Q.Range, Color.Cyan, 1, 1);
                }
                else
                    LeagueSharp.Common.Utility.DrawCircle(ObjectManager.Player.Position, Q.Range, Color.Cyan, 1, 1);
            }

            if (getCheckBoxItem(drawMenu, "eRange"))
            {
                if (getCheckBoxItem(drawMenu, "onlyRdy"))
                {
                    if (E.IsReady())
                        LeagueSharp.Common.Utility.DrawCircle(ObjectManager.Player.Position, E.Range, Color.Yellow, 1, 1);
                }
                else
                    LeagueSharp.Common.Utility.DrawCircle(ObjectManager.Player.Position, E.Range, Color.Yellow, 1, 1);
            }

            if (getCheckBoxItem(drawMenu, "rRange"))
            {
                if (getCheckBoxItem(drawMenu, "onlyRdy"))
                {
                    if (R.IsReady())
                        LeagueSharp.Common.Utility.DrawCircle(ObjectManager.Player.Position, R.Range, Color.Gray, 1, 1);
                }
                else
                    LeagueSharp.Common.Utility.DrawCircle(ObjectManager.Player.Position, R.Range, Color.Gray, 1, 1);
            }
        }
    }
}
