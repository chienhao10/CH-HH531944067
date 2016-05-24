using System;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using LeagueSharp.Common;
using OneKeyToWin_AIO_Sebby.Core;
using SebbyLib;
using SharpDX;
using Color = System.Drawing.Color;
using Orbwalking = SebbyLib.Orbwalking;
using Spell = LeagueSharp.Common.Spell;
using Utility = LeagueSharp.Common.Utility;

namespace OneKeyToWin_AIO_Sebby
{
    internal class Sivir
    {
        private static readonly Menu Config = Program.Config;
        public static Spell E, Q, Qc, W, R;
        public static float QMANA, WMANA, EMANA, RMANA;


        public static MissileReturn missileManager;

        public static Menu drawMenu, farmMenu, eMenu, wMenu, rMenu;


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

        public static int getBoxItem(Menu m, string item)
        {
            return m[item].Cast<ComboBox>().CurrentValue;
        }

        public static void LoadOKTW()
        {
            Q = new Spell(SpellSlot.Q, 1200f);
            Qc = new Spell(SpellSlot.Q, 1200f);
            W = new Spell(SpellSlot.W);
            E = new Spell(SpellSlot.E);
            R = new Spell(SpellSlot.R, 25000f);

            Q.SetSkillshot(0.25f, 90f, 1350f, false, SkillshotType.SkillshotLine);
            Qc.SetSkillshot(0.25f, 90f, 1350f, true, SkillshotType.SkillshotLine);

            missileManager = new MissileReturn("SivirQMissile", "SivirQMissileReturn", Q);

            drawMenu = Config.AddSubMenu("Draw");
            drawMenu.Add("notif", new CheckBox("Notification (timers)"));
            drawMenu.Add("noti", new CheckBox("Show KS notification"));
            drawMenu.Add("qRange", new CheckBox("Q range", false));
            drawMenu.Add("onlyRdy", new CheckBox("Draw only ready spells"));

            farmMenu = Config.AddSubMenu("Farm");
            farmMenu.Add("farmQ", new CheckBox("Lane clear Q"));
            farmMenu.Add("farmW", new CheckBox("Lane clear W"));
            farmMenu.Add("Mana", new Slider("LaneClear Mana", 80));
            farmMenu.Add("LCminions", new Slider("LaneClear minimum minions", 5, 0, 10));
            farmMenu.Add("jungleQ", new CheckBox("Jungle clear Q"));
            farmMenu.Add("jungleW", new CheckBox("Jungle clear W"));

            wMenu = Config.AddSubMenu("Harass");
            wMenu.Add("harasW", new CheckBox("Harras W"));
            foreach (var enemy in ObjectManager.Get<AIHeroClient>().Where(enemy => enemy.Team != Player.Team))
                wMenu.Add("haras" + enemy.NetworkId, new CheckBox(enemy.ChampionName));

            eMenu = Config.AddSubMenu("E Shield Config");
            eMenu.AddGroupLabel("E On : ");
            foreach (var enemy in ObjectManager.Get<AIHeroClient>().Where(enemy => enemy.IsEnemy))
            {
                for (var i = 0; i < 4; i++)
                {
                    var spell = enemy.Spellbook.Spells[i];
                    if (spell.SData.TargettingType != SpellDataTargetType.Self &&
                        spell.SData.TargettingType != SpellDataTargetType.SelfAndUnit)
                    {
                        eMenu.Add("spell" + spell.SData.Name,
                            spell.SData.TargettingType == SpellDataTargetType.Unit
                                ? new CheckBox(spell.Name)
                                : new CheckBox(spell.Name, false));
                    }
                }
            }
            eMenu.AddSeparator();
            eMenu.Add("autoE", new CheckBox("Auto E"));
            eMenu.Add("AGC", new CheckBox("AntiGapcloserE"));
            eMenu.Add("Edmg", new Slider("Block under % hp", 90));

            rMenu = Config.AddSubMenu("R Config");
            rMenu.Add("autoR", new CheckBox("Auto R"));

            Game.OnUpdate += Game_OnGameUpdate;
            Drawing.OnDraw += Drawing_OnDraw;
            Orbwalker.OnPostAttack += Orbwalker_OnPostAttack;
            Obj_AI_Base.OnProcessSpellCast += Obj_AI_Base_OnProcessSpellCast;
            AntiGapcloser.OnEnemyGapcloser += AntiGapcloser_OnEnemyGapcloser;
        }

        private static void Orbwalker_OnPostAttack(AttackableUnit target, EventArgs args)
        {
            if (W.IsReady())
            {
                if (target is AIHeroClient)
                {
                    var t = target as AIHeroClient;
                    if (Program.Combo && Player.Mana > RMANA + WMANA)
                        W.Cast();
                    else if (getCheckBoxItem(wMenu, "harasW") && !Player.UnderTurret(true) && Player.Mana > RMANA + WMANA + QMANA && getCheckBoxItem(wMenu, "haras" + t.NetworkId))
                    {
                        W.Cast();
                    }
                }
                else
                {
                    var t = TargetSelector.GetTarget(900, DamageType.Physical);
                    if (t.IsValidTarget() && getCheckBoxItem(wMenu, "harasW") && getCheckBoxItem(wMenu, "haras" + t.NetworkId) && !Player.UnderTurret(true) && Player.Mana > RMANA + WMANA + QMANA && t.LSDistance(target.Position) < 500)
                    {
                        W.Cast();
                    }
                    if (target is Obj_AI_Minion && Program.LaneClear && getCheckBoxItem(farmMenu, "farmW") && Player.ManaPercent > getSliderItem(farmMenu, "Mana") && !Player.UnderTurret(true))
                    {
                        var minions = Cache.GetMinions(target.Position, 500);
                        if (minions.Count >= getSliderItem(farmMenu, "LCminions"))
                        {
                            W.Cast();
                        }
                    }
                }
            }
        }

        private static void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!E.IsReady() || args.SData.IsAutoAttack() || Player.HealthPercent > getSliderItem(eMenu, "Edmg") || !getCheckBoxItem(eMenu, "autoE") || !sender.IsEnemy || sender.IsMinion || !sender.IsValid<AIHeroClient>() || args.SData.Name.ToLower() == "tormentedsoil")
                return;

            if (eMenu["spell" + args.SData.Name] == null || !getCheckBoxItem(eMenu, "spell" + args.SData.Name))
                return;

            if (args.Target != null)
            {
                if (args.Target.IsMe)
                    E.Cast();
            }
            else if (OktwCommon.CanHitSkillShot(Player, args))
            {
                E.Cast();
            }
        }

        private static void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            var Target = gapcloser.Sender;
            if (getCheckBoxItem(eMenu, "AGC") && E.IsReady() && Target.IsValidTarget(5000) && Target.IsEnemy)
                E.Cast();
        }

        private static void Game_OnGameUpdate(EventArgs args)
        {
            if (Program.LagFree(0))
            {
                SetMana();
            }

            if (Program.LagFree(1) && Q.IsReady() && !Orbwalker.IsAutoAttacking)
            {
                LogicQ();
            }

            if (Program.LagFree(2) && R.IsReady() && Program.Combo && getCheckBoxItem(rMenu, "autoR"))
            {
                LogicR();
            }

            if (Program.LagFree(3) && Program.LaneClear)
            {
                Jungle();
            }
        }

        private static void LogicQ()
        {
            var t = TargetSelector.GetTarget(Q.Range, DamageType.Physical);
            if (t.IsValidTarget())
            {
                missileManager.Target = t;
                var qDmg = OktwCommon.GetKsDamage(t, Q)*1.9;
                if (Orbwalking.InAutoAttackRange(t))
                    qDmg = qDmg + Player.GetAutoAttackDamage(t)*3;
                if (qDmg > t.Health)
                    Q.Cast(t, true);
                else if (Program.Combo && Player.Mana > RMANA + QMANA)
                    Program.CastSpell(Q, t);
                else if (Program.Farm && getCheckBoxItem(wMenu, "haras" + t.NetworkId) && !Player.UnderTurret(true))
                {
                    if (Player.Mana > Player.MaxMana*0.9)
                        Program.CastSpell(Q, t);
                    else if (ObjectManager.Player.Mana > RMANA + WMANA + QMANA + QMANA)
                        Program.CastSpell(Qc, t);
                    else if (Player.Mana > RMANA + WMANA + QMANA + QMANA)
                    {
                        Q.CastIfWillHit(t, 2, true);
                        if (Program.LaneClear)
                            Program.CastSpell(Q, t);
                    }
                }
                if (Player.Mana > RMANA + WMANA)
                {
                    foreach (
                        var enemy in
                            Program.Enemies.Where(enemy => enemy.IsValidTarget(Q.Range) && !OktwCommon.CanMove(enemy)))
                        Q.Cast(enemy);
                }
            }
            else if (Program.LaneClear && Player.ManaPercent > getSliderItem(farmMenu, "Mana") && getCheckBoxItem(farmMenu, "farmQ") && Player.Mana > RMANA + QMANA)
            {
                var minionList = Cache.GetMinions(Player.ServerPosition, Q.Range);
                var farmPosition = Q.GetLineFarmLocation(minionList, Q.Width);
                if (farmPosition.MinionsHit >= getSliderItem(farmMenu, "LCminions"))
                    Q.Cast(farmPosition.Position);
            }
        }

        private static void LogicR()
        {
            var t = TargetSelector.GetTarget(800, DamageType.Physical);
            if (Player.CountEnemiesInRange(800f) > 2)
                R.Cast();
            else if (t.IsValidTarget() && Orbwalker.LastTarget == null && Program.Combo &&
                     Player.GetAutoAttackDamage(t)*2 > t.Health && !Q.IsReady() && t.CountEnemiesInRange(800) < 3)
                R.Cast();
        }

        private static void Jungle()
        {
            if (Player.Mana > RMANA + WMANA + RMANA)
            {
                var mobs = Cache.GetMinions(ObjectManager.Player.ServerPosition, 600, MinionTeam.Neutral);
                if (mobs.Count > 0)
                {
                    var mob = mobs[0];
                    if (W.IsReady() && getCheckBoxItem(farmMenu, "jungleW"))
                    {
                        W.Cast();
                        return;
                    }
                    if (Q.IsReady() && getCheckBoxItem(farmMenu, "jungleQ"))
                    {
                        Q.Cast(mob);
                    }
                }
            }
        }

        private static void SetMana()
        {
            if ((Program.getCheckBoxItem("manaDisable") && Program.Combo) || Player.HealthPercent < 20)
            {
                QMANA = 0;
                WMANA = 0;
                EMANA = 0;
                RMANA = 0;
                return;
            }

            QMANA = Q.Instance.SData.Mana;
            WMANA = W.Instance.SData.Mana;
            EMANA = E.Instance.SData.Mana;

            if (!R.IsReady())
                RMANA = QMANA - Player.PARRegenRate*Q.Instance.Cooldown;
            else
                RMANA = R.Instance.SData.Mana;
        }

        public static void drawText2(string msg, Vector3 Hero, int high, Color color)
        {
            var wts = Drawing.WorldToScreen(Hero);
            Drawing.DrawText(wts[0] - msg.Length*5, wts[1] - high, color, msg);
        }

        private static void Drawing_OnDraw(EventArgs args)
        {
            if (getCheckBoxItem(drawMenu, "notif"))
            {
                if (Player.HasBuff("sivirwmarker"))
                {
                    var color = Color.Yellow;
                    var buffTime = OktwCommon.GetPassiveTime(Player, "sivirwmarker");
                    if (buffTime < 1)
                        color = Color.Red;
                    drawText2("W:  " + string.Format("{0:0.0}", buffTime), Player.Position, 175, color);
                }
                if (Player.HasBuff("SivirE"))
                {
                    var color = Color.Aqua;
                    var buffTime = OktwCommon.GetPassiveTime(Player, "SivirE");
                    if (buffTime < 1)
                        color = Color.Red;
                    drawText2("E:  " + string.Format("{0:0.0}", buffTime), Player.Position, 200, color);
                }
                if (Player.HasBuff("SivirR"))
                {
                    var color = Color.GreenYellow;
                    var buffTime = OktwCommon.GetPassiveTime(Player, "SivirR");
                    if (buffTime < 1)
                        color = Color.Red;
                    drawText2("R:  " + string.Format("{0:0.0}", buffTime), Player.Position, 225, color);
                }
            }

            if (getCheckBoxItem(drawMenu, "qRange"))
            {
                if (getCheckBoxItem(drawMenu, "onlyRdy"))
                {
                    if (Q.IsReady())
                        Utility.DrawCircle(ObjectManager.Player.Position, Q.Range, Color.Cyan, 1, 1);
                }
                else
                    Utility.DrawCircle(ObjectManager.Player.Position, Q.Range, Color.Cyan, 1, 1);
            }

            if (getCheckBoxItem(drawMenu, "noti"))
            {
                var target = TargetSelector.GetTarget(1500, DamageType.Physical);
                if (target.IsValidTarget())
                {
                    if (Q.GetDamage(target)*2 > target.Health)
                    {
                        Render.Circle.DrawCircle(target.ServerPosition, 200, Color.Red);
                        Drawing.DrawText(Drawing.Width*0.1f, Drawing.Height*0.4f, Color.Red,
                            "Q kill: " + target.ChampionName + " have: " + target.Health + "hp");
                    }
                }
            }
        }
    }
}