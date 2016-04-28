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

            drawMenu = Config.AddSubMenu("Draw");
            drawMenu.Add("notif", new CheckBox("Notification (timers)"));
            drawMenu.Add("noti", new CheckBox("Show KS notification"));
            drawMenu.Add("qRange", new CheckBox("Q range"));
            drawMenu.Add("eRange", new CheckBox("E range"));
            drawMenu.Add("rRange", new CheckBox("R range"));
            drawMenu.Add("onlyRdy", new CheckBox("Draw only ready spells"));

            qMenu = Config.AddSubMenu("Q Config");
            qMenu.Add("autoQ", new CheckBox("Auto Q"));
            qMenu.Add("AGCq", new CheckBox("Anti Gapcloser Q"));

            ewMenu = Config.AddSubMenu("E W Shield Config");
            ewMenu.AddGroupLabel("Spell Manager");
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
            ewMenu.Add("autoE", new CheckBox("Auto E"));
            ewMenu.Add("Edmg", new Slider("Shield incoming damage %", 20));
            ewMenu.AddGroupLabel("Use On : ");
            foreach (var enemy in ObjectManager.Get<AIHeroClient>().Where(enemy => enemy.Team == Player.Team))
            {
                ewMenu.Add("Eon" + enemy.ChampionName, new CheckBox(enemy.ChampionName));
            }
            ewMenu.AddGroupLabel("GapCloser");
            ewMenu.Add("AGC", new CheckBox("Anti Gapcloser E + W"));
            foreach (var enemy in ObjectManager.Get<AIHeroClient>().Where(enemy => enemy.Team != Player.Team))
            {
                ewMenu.Add("gapcloser" + enemy.ChampionName, new CheckBox("Antigapclose : " + enemy.ChampionName));
            }

            rMenu = Config.AddSubMenu("R Config");
            rMenu.Add("autoR", new CheckBox("Auto R"));
            rMenu.Add("useR", new KeyBind("Semi-manual cast R", false, KeyBind.BindTypes.HoldActive, 'T'));
            rMenu.Add("rCombo", new CheckBox("Always in combo"));
            rMenu.Add("rCount", new Slider("Auto R if hit x enemies", 3, 0, 5));
            rMenu.Add("rCc", new CheckBox("Auto R immobile enemy korean style"));
            rMenu.Add("OnInterruptableSpell", new CheckBox("OnInterruptableSpell"));
            foreach (var enemy in ObjectManager.Get<AIHeroClient>().Where(enemy => enemy.IsEnemy))
            {
                rMenu.Add("Rmode" + enemy.ChampionName,
                    new Slider(enemy.ChampionName + " 0 : Normal | 1 : Always | 2 : Never | 3 : Normal + Gapcloser R"));
            }

            harassMenu = Config.AddSubMenu("Harass");
            foreach (var enemy in ObjectManager.Get<AIHeroClient>().Where(enemy => enemy.Team != Player.Team))
            {
                harassMenu.Add("haras" + enemy.ChampionName, new CheckBox(enemy.ChampionName));
            }

            Game.OnUpdate += Game_OnGameUpdate;
            Drawing.OnDraw += Drawing_OnDraw;
            Obj_AI_Base.OnProcessSpellCast += Obj_AI_Base_OnProcessSpellCast;
            AntiGapcloser.OnEnemyGapcloser += AntiGapcloser_OnEnemyGapcloser;
        }

        private static void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            var t = gapcloser.Sender;
            if (!getCheckBoxItem(ewMenu, "gapcloser" + t.ChampionName))
                return;

            if (getCheckBoxItem(ewMenu, "AGC"))
            {
                if (W.IsReady() && gapcloser.End.Distance(Player.Position) < gapcloser.Start.Distance(Player.Position))
                {
                    var allyHero = SebbyLib.Program.Allies.Where(ally => ally.Distance(Player) <= W.Range && !ally.IsMe)
                        .OrderBy(ally => ally.Distance(gapcloser.End)).FirstOrDefault();

                    if (allyHero != null && getCheckBoxItem(ewMenu, "Eon" + allyHero.ChampionName))
                        W.Cast(allyHero);
                }
                if (E.IsReady())
                    LeagueSharp.Common.Utility.DelayAction.Add(200, () => E.Cast(t.ServerPosition));
            }

            if (Q.IsReady() && getCheckBoxItem(qMenu, "AGCq"))
                Q.Cast(t);

            if (R.IsReady() && getSliderItem(rMenu, "Rmode" + t.ChampionName) == 3)
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
                                    getCheckBoxItem(harassMenu, "haras" + enemy.ChampionName)))
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
                var Rmode = getSliderItem(rMenu, "Rmode" + t.ChampionName);

                if (Rmode == 2)
                    continue;
                if (Rmode == 1)
                    SebbyLib.Program.CastSpell(R, t);

                if (rCount > 0)
                    R.CastIfWillHit(t, rCount);

                if (getCheckBoxItem(rMenu, "rCc") && !OktwCommon.CanMove(t) && t.HealthPercent > 20*t.CountAlliesInRange(500))
                {
                    var t1 = t;
                    LeagueSharp.Common.Utility.DelayAction.Add(800 - (int) (Player.Distance(t.Position)/2), () => CastRtime(t1));
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

            if (ewMenu["spell" + args.SData.Name] != null &&
                !getCheckBoxItem(ewMenu, "spell" + args.SData.Name))
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
                                ally.IsValid && Player.Distance(ally.ServerPosition) < W.Range &&
                                getCheckBoxItem(ewMenu, "Eon" + ally.ChampionName)))
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