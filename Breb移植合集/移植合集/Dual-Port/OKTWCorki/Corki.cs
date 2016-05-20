using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using SebbyLib;
using EloBuddy;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK;

namespace OneKeyToWin_AIO_Sebby.Champions
{
    class Corki
    {
        private static Menu Config = Program.Config;
        private static LeagueSharp.Common.Spell E, Q, R, W;
        private static float QMANA = 0, WMANA = 0, EMANA = 0, RMANA = 0;
        public static AIHeroClient Player { get { return ObjectManager.Player; } }

        public static Menu drawMenu, qMenu, wMenu, eMenu, rMenu, harassMenu, farmMenu;

        public static void LoadOKTW()
        {
            Q = new LeagueSharp.Common.Spell(SpellSlot.Q, 825);
            W = new LeagueSharp.Common.Spell(SpellSlot.W, 600);
            E = new LeagueSharp.Common.Spell(SpellSlot.E, 800);
            R = new LeagueSharp.Common.Spell(SpellSlot.R, 1230);

            Q.SetSkillshot(0.3f, 200f, 1000f, false, SkillshotType.SkillshotCircle);
            R.SetSkillshot(0.2f, 40f, 2000f, true, SkillshotType.SkillshotLine);

            drawMenu = Config.AddSubMenu("Drawings");
            drawMenu.Add("onlyRdy", new CheckBox("Draw only ready spells", true));
            drawMenu.Add("qRange", new CheckBox("Q range", false));
            drawMenu.Add("wRange", new CheckBox("W range", false));
            drawMenu.Add("eRange", new CheckBox("E range", false));
            drawMenu.Add("rRange", new CheckBox("R range", false));

            qMenu = Config.AddSubMenu("Q Config");
            qMenu.Add("autoQ", new CheckBox("Auto Q", true));
            qMenu.Add("harassQ", new CheckBox("Q harass", true));

            wMenu = Config.AddSubMenu("W Config");
            wMenu.Add("nktdE", new CheckBox("NoKeyToDash", true));

            eMenu = Config.AddSubMenu("E Config");
            eMenu.Add("autoE", new CheckBox("Auto E", true));
            eMenu.Add("harassE", new CheckBox("E harass", true));

            rMenu = Config.AddSubMenu("R Config");
            rMenu.Add("autoR", new CheckBox("Auto R", true));
            rMenu.Add("Rammo", new Slider("Minimum R ammo harass", 3, 0, 6));
            rMenu.Add("minionR", new CheckBox("Try R on minion", true));
            rMenu.Add("useR", new KeyBind("Semi-manual cast R key", false, KeyBind.BindTypes.HoldActive, 'T')); //32 == space

            harassMenu = Config.AddSubMenu("Harass");
            foreach (var enemy in ObjectManager.Get<AIHeroClient>().Where(enemy => enemy.IsEnemy))
                harassMenu.Add("harras" + enemy.ChampionName, new CheckBox(enemy.ChampionName));

            farmMenu = Config.AddSubMenu("Farm");
            farmMenu.Add("LCminions", new Slider("LaneClear minimum minions", 2, 0, 10));
            farmMenu.Add("RammoLC", new Slider("Minimum R ammo Lane clear", 3, 0, 6));
            farmMenu.Add("farmQ", new CheckBox("LaneClear + jungle Q", true));
            farmMenu.Add("farmR", new CheckBox("LaneClear + jungle  R", true));
            farmMenu.Add("Mana", new Slider("LaneClear  Mana", 80, 30, 100));

            Game.OnUpdate += Game_OnGameUpdate;
            Drawing.OnDraw += Drawing_OnDraw;
            Orbwalker.OnPreAttack += BeforeAttack;
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

        private static void BeforeAttack(AttackableUnit target, Orbwalker.PreAttackArgs args)
        {
            if (E.IsReady() && Sheen() && args.Target.IsValid<AIHeroClient>())
            {
                if (Program.Combo && getCheckBoxItem(eMenu, "autoE") && Player.Mana > EMANA + RMANA)
                    E.Cast(args.Target.Position);
                if (Program.Farm && getCheckBoxItem(eMenu, "harassE") && Player.Mana > EMANA + RMANA + QMANA && OktwCommon.CanHarras())
                    E.Cast(args.Target.Position);
                if (!Q.IsReady() && !R.IsReady() && args.Target.Health < Player.FlatPhysicalDamageMod * 2)
                    E.Cast();
            }
        }

        private static void Game_OnGameUpdate(EventArgs args)
        {
            if (Program.LagFree(0))
            {
                SetMana();
                farm();
            }
            if (Program.LagFree(1) && Q.IsReady() && !Player.Spellbook.IsAutoAttacking && Sheen())
                LogicQ();
            if (Program.LagFree(2) && Program.Combo && W.IsReady())
                LogicW();
            if (Program.LagFree(4) && R.IsReady() && !Player.Spellbook.IsAutoAttacking && Sheen() && !Orbwalker.IsAutoAttacking)
                LogicR();
        }

        private static void LogicR()
        {
            float rSplash = 150;
            if (bonusR)
            {
                rSplash = 300;
            }

            var t = TargetSelector.GetTarget(R.Range, DamageType.Physical);

            if (t.IsValidTarget())
            {
                var rDmg = OktwCommon.GetKsDamage(t, R);
                var qDmg = Q.GetDamage(t);
                if (rDmg * 2 > t.Health)
                    CastR(R, t);
                else if (t.IsValidTarget(Q.Range) && qDmg + rDmg > t.Health)
                    CastR(R, t);
                if (Player.Spellbook.GetSpell(SpellSlot.R).Ammo > 1)
                {
                    foreach (var enemy in Program.Enemies.Where(enemy => enemy.IsValidTarget(R.Range) && enemy.CountEnemiesInRange(rSplash) > 1))
                        t = enemy;

                    if (Program.Combo && Player.Mana > RMANA * 3)
                    {
                        CastR(R, t);
                    }
                    else if (Program.Farm && Player.Mana > RMANA + EMANA + QMANA + WMANA && Player.Spellbook.GetSpell(SpellSlot.R).Ammo >= getSliderItem(rMenu, "Rammo") && OktwCommon.CanHarras())
                    {
                        foreach (var enemy in Program.Enemies.Where(enemy => enemy.IsValidTarget(R.Range) && getCheckBoxItem(harassMenu, "harras" + enemy.ChampionName)))
                            CastR(R, enemy);
                    }

                    if (!Program.None && Player.Mana > RMANA + QMANA + EMANA)
                    {
                        foreach (var enemy in Program.Enemies.Where(enemy => enemy.IsValidTarget(R.Range) && !OktwCommon.CanMove(enemy)))
                            CastR(R, t);
                    }
                }
            }
        }

        private static void CastR(LeagueSharp.Common.Spell R, AIHeroClient t)
        {
            Program.CastSpell(R, t);
            if (getCheckBoxItem(rMenu, "minionR"))
            {
                // collision + predictio R
                var poutput = R.GetPrediction(t);
                var col = poutput.CollisionObjects.Count(ColObj => ColObj.IsEnemy && ColObj.IsMinion && !ColObj.IsDead);

                //hitchance
                var prepos = LeagueSharp.Common.Prediction.GetPrediction(t, 0.4f);

                if (col == 0 && (int)prepos.Hitchance < 5)
                    return;

                float rSplash = 140;
                if (bonusR)
                    rSplash = 290f;

                var minions = Cache.GetMinions(Player.ServerPosition, R.Range - rSplash);
                foreach (var minion in minions.Where(minion => minion.Distance(poutput.CastPosition) < rSplash))
                {
                    R.Cast(minion);
                    return;
                }
            }
        }

        private static void LogicW()
        {
            var dashPosition = Player.Position.Extend(Game.CursorPos, W.Range);

            if (Game.CursorPos.Distance(Player.Position) > Player.AttackRange + Player.BoundingRadius * 2 && Program.Combo && getCheckBoxItem(wMenu, "nktdE") && Player.Mana > RMANA + WMANA - 10)
            {
                W.Cast(dashPosition);
            }
        }

        private static void LogicQ()
        {
            var t = TargetSelector.GetTarget(Q.Range, DamageType.Physical);
            if (t.IsValidTarget())
            {
                if (Program.Combo && getCheckBoxItem(qMenu, "autoQ") && Player.Mana > RMANA + QMANA)
                    Program.CastSpell(Q, t);
                else if (Program.Farm && getCheckBoxItem(qMenu, "harassQ") && getCheckBoxItem(harassMenu, "harras" + t.ChampionName) && Player.Mana > RMANA + EMANA + WMANA + RMANA && OktwCommon.CanHarras())
                    Program.CastSpell(Q, t);
                else
                {
                    var qDmg = OktwCommon.GetKsDamage(t, Q);
                    var rDmg = R.GetDamage(t);
                    if (qDmg > t.Health)
                        Q.Cast(t);
                    else if (rDmg + qDmg > t.Health && Player.Mana > RMANA + QMANA)
                        Program.CastSpell(Q, t);
                    else if (rDmg + 2 * qDmg > t.Health && Player.Mana > QMANA + RMANA * 2)
                        Program.CastSpell(Q, t);
                }

                if (!Program.None && Player.Mana > RMANA + WMANA + EMANA)
                {
                    foreach (var enemy in Program.Enemies.Where(enemy => enemy.IsValidTarget(Q.Range) && !OktwCommon.CanMove(enemy)))
                        Q.Cast(enemy, true, true);
                }
            }
        }
        public static void farm()
        {
            if (Program.LaneClear && !Orbwalker.IsAutoAttacking && Sheen())
            {
                var mobs = Cache.GetMinions(Player.ServerPosition, Q.Range, MinionTeam.Neutral);
                if (mobs.Count > 0 && Player.Mana > RMANA + WMANA + EMANA + QMANA)
                {
                    var mob = mobs[0];
                    if (Q.IsReady() && getCheckBoxItem(farmMenu, "farmQ"))
                    {
                        Q.Cast(mob);
                        return;
                    }

                    if (R.IsReady() && getCheckBoxItem(farmMenu, "farmR"))
                    {
                        R.Cast(mob);
                        return;
                    }
                }

                if (Player.ManaPercent > getSliderItem(farmMenu, "Mana"))
                {
                    var minions = Cache.GetMinions(Player.ServerPosition, Q.Range);

                    if (R.IsReady() && getCheckBoxItem(farmMenu, "farmR") && Player.Spellbook.GetSpell(SpellSlot.R).Ammo >= getSliderItem(rMenu, "RammoLC"))
                    {
                        var rfarm = R.GetCircularFarmLocation(minions, 100);
                        if (rfarm.MinionsHit >= getSliderItem(drawMenu, "LCminions"))
                        {
                            R.Cast(rfarm.Position);
                            return;
                        }
                    }
                    if (Q.IsReady() && getCheckBoxItem(farmMenu, "farmQ"))
                    {
                        var qfarm = Q.GetCircularFarmLocation(minions, Q.Width);
                        if (qfarm.MinionsHit >= getSliderItem(farmMenu, "LCminions"))
                        {
                            Q.Cast(qfarm.Position);
                            return;
                        }
                    }
                }
            }
        }

        private static bool Sheen()
        {
            var target = Orbwalker.LastTarget;

            if (target.IsValidTarget() && Player.HasBuff("sheen"))
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        private static bool bonusR { get { return Player.HasBuff("corkimissilebarragecounterbig"); } }

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
                RMANA = QMANA - Player.PARRegenRate * Q.Instance.Cooldown;
            else
                RMANA = R.Instance.SData.Mana;
        }

        public static void drawText(string msg, Vector3 Hero, System.Drawing.Color color)
        {
            var wts = Drawing.WorldToScreen(Hero);
            Drawing.DrawText(wts[0] - (msg.Length) * 5, wts[1] - 200, color, msg);
        }

        private static void Drawing_OnDraw(EventArgs args)
        {
            if (getCheckBoxItem(wMenu, "nktdE"))
            {
                if (Game.CursorPos.Distance(Player.Position) > Player.AttackRange + Player.BoundingRadius * 2)
                    drawText("dash: ON ", Player.Position, System.Drawing.Color.Red);
                else
                    drawText("dash: OFF ", Player.Position, System.Drawing.Color.GreenYellow);
            }
            if (getCheckBoxItem(drawMenu, "qRange"))
            {
                if (getCheckBoxItem(drawMenu, "onlyRdy"))
                {
                    if (Q.IsReady())
                        LeagueSharp.Common.Utility.DrawCircle(ObjectManager.Player.Position, Q.Range, System.Drawing.Color.Cyan, 1, 1);
                }
                else
                    LeagueSharp.Common.Utility.DrawCircle(ObjectManager.Player.Position, Q.Range, System.Drawing.Color.Cyan, 1, 1);
            }
            if (getCheckBoxItem(drawMenu, "wRange"))
            {
                if (getCheckBoxItem(drawMenu, "onlyRdy"))
                {
                    if (W.IsReady())
                        LeagueSharp.Common.Utility.DrawCircle(ObjectManager.Player.Position, W.Range, System.Drawing.Color.Orange, 1, 1);
                }
                else
                    LeagueSharp.Common.Utility.DrawCircle(ObjectManager.Player.Position, W.Range, System.Drawing.Color.Orange, 1, 1);
            }
            if (getCheckBoxItem(drawMenu, "eRange"))
            {
                if (getCheckBoxItem(drawMenu, "onlyRdy"))
                {
                    if (E.IsReady())
                        LeagueSharp.Common.Utility.DrawCircle(ObjectManager.Player.Position, E.Range, System.Drawing.Color.Yellow, 1, 1);
                }
                else
                    LeagueSharp.Common.Utility.DrawCircle(ObjectManager.Player.Position, E.Range, System.Drawing.Color.Yellow, 1, 1);
            }
            if (getCheckBoxItem(drawMenu, "rRange"))
            {
                if (getCheckBoxItem(drawMenu, "onlyRdy"))
                {
                    if (R.IsReady())
                        LeagueSharp.Common.Utility.DrawCircle(ObjectManager.Player.Position, R.Range, System.Drawing.Color.Gray, 1, 1);
                }
                else
                    LeagueSharp.Common.Utility.DrawCircle(ObjectManager.Player.Position, R.Range, System.Drawing.Color.Gray, 1, 1);
            }
        }
    }
}