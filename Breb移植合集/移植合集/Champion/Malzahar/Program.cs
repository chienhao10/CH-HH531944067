using System;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using LeagueSharp.Common;
using SebbyLib;
using SharpDX;
using Color = System.Drawing.Color;
using Spell = LeagueSharp.Common.Spell;
using Utility = LeagueSharp.Common.Utility;

namespace OneKeyToWin_AIO_Sebby.Champions
{
    internal class Malzahar
    {
        private static readonly Menu Config = Program.Config;
        private static Spell Q, Qr, W, E, R;
        private static float QMANA, WMANA, EMANA, RMANA;
        private static float Rtime;

        public static GameObject WMissile;

        public static Menu drawMenu, qMenu, wMenu, eMenu, rMenu, farmMenu;

        public static AIHeroClient Player
        {
            get { return ObjectManager.Player; }
        }

        public static void LoadOKTW()
        {
            Q = new Spell(SpellSlot.Q, 900);
            Qr = new Spell(SpellSlot.Q, 900);
            W = new Spell(SpellSlot.W, 800);
            E = new Spell(SpellSlot.E, 650);
            R = new Spell(SpellSlot.R, 700);

            Qr.SetSkillshot(0.25f, 100, float.MaxValue, false, SkillshotType.SkillshotCircle);
            Q.SetSkillshot(1.05f, 100, float.MaxValue, false, SkillshotType.SkillshotCircle);
            W.SetSkillshot(1.2f, 230, float.MaxValue, false, SkillshotType.SkillshotCircle);

            drawMenu = Config.AddSubMenu("Draw");
            drawMenu.Add("noti", new CheckBox("Show notification & line"));
            drawMenu.Add("onlyRdy", new CheckBox("Draw only ready spells"));
            drawMenu.Add("qRange", new CheckBox("Q range", false));
            drawMenu.Add("wRange", new CheckBox("W range", false));
            drawMenu.Add("eRange", new CheckBox("E range", false));
            drawMenu.Add("rRange", new CheckBox("R range", false));

            qMenu = Config.AddSubMenu("Q Config");
            qMenu.Add("autoQ", new CheckBox("Auto Q"));
            qMenu.Add("harrasQ", new CheckBox("Harass Q"));
            qMenu.Add("intQ", new CheckBox("Interrupt spells Q"));
            qMenu.Add("gapQ", new CheckBox("Gapcloser Q"));

            wMenu = Config.AddSubMenu("W Config");
            wMenu.Add("autoW", new CheckBox("Auto W"));
            wMenu.Add("harrasW", new CheckBox("Harass W"));

            eMenu = Config.AddSubMenu("E Config");
            eMenu.Add("autoE", new CheckBox("Auto E"));
            eMenu.Add("harrasE", new CheckBox("Harras E"));
            eMenu.Add("harrasEminion", new CheckBox("Try harras E on minion"));

            rMenu = Config.AddSubMenu("R Config");
            rMenu.Add("autoR", new CheckBox("Auto R"));
            rMenu.Add("useR", new KeyBind("Fast combo key", false, KeyBind.BindTypes.HoldActive, 'T')); //32 == space
            foreach (var enemy in ObjectManager.Get<AIHeroClient>().Where(enemy => enemy.IsEnemy))
                rMenu.Add("gapcloser" + enemy.ChampionName, new CheckBox("Gapclose : " + enemy.ChampionName, false));
            foreach (var enemy in ObjectManager.Get<AIHeroClient>().Where(enemy => enemy.IsEnemy))
                rMenu.Add("Ron" + enemy.ChampionName, new CheckBox("Fast Combo : " + enemy.ChampionName));
            rMenu.Add("Rturrent", new CheckBox("Don't R under turret"));

            farmMenu = Config.AddSubMenu("Farm");
            farmMenu.Add("farmQ", new CheckBox("Lane clear Q"));
            farmMenu.Add("farmW", new CheckBox("Lane clear W"));
            farmMenu.Add("farmE", new CheckBox("Lane clear E"));
            farmMenu.Add("Mana", new Slider("LaneClear Mana", 80));
            farmMenu.Add("LCminions", new Slider("LaneClear minimum minions", 2, 0, 10));
            farmMenu.Add("jungleE", new CheckBox("Jungle clear E"));
            farmMenu.Add("jungleQ", new CheckBox("Jungle clear Q"));
            farmMenu.Add("jungleW", new CheckBox("Jungle clear W"));

            Game.OnUpdate += Game_OnGameUpdate;
            Drawing.OnDraw += Drawing_OnDraw;
            Interrupter2.OnInterruptableTarget += Interrupter2_OnInterruptableTarget;
            AntiGapcloser.OnEnemyGapcloser += AntiGapcloser_OnEnemyGapcloser;
            Spellbook.OnCastSpell += Spellbook_OnCastSpell;
            GameObject.OnCreate += Obj_AI_Base_OnCreate;
            GameObject.OnDelete += GameObject_OnDelete;
        }

        private static void GameObject_OnDelete(GameObject sender, EventArgs args)
        {
            if (sender.IsValid && sender.IsAlly)
            {
                if (sender.Name == "Malzahar_Base_W_flash.troy")
                {
                    WMissile = null;
                }
            }
        }

        private static void Obj_AI_Base_OnCreate(GameObject sender, EventArgs args)
        {
            if (sender.IsValid && sender.IsAlly)
            {
                if (sender.Name == "Malzahar_Base_W_flash.troy")
                    WMissile = sender;
            }
        }

        private static void Spellbook_OnCastSpell(Spellbook sender, SpellbookCastSpellEventArgs args)
        {
            if (args.Slot == SpellSlot.R)
            {
                var t = TargetSelector.GetTarget(R.Range - 20, DamageType.Magical);

                if (E.IsReady() && t.IsValidTarget(E.Range) && Player.Mana > RMANA + EMANA)
                {
                    E.CastOnUnit(t);
                    args.Process = false;
                    return;
                }

                if (W.IsReady() && t.IsValidTarget(W.Range) && Player.Mana > RMANA + WMANA)
                {
                    W.Cast(t);
                    args.Process = false;
                    return;
                }

                if (Q.IsReady() && t.IsValidTarget(Q.Range) && Player.Mana > RMANA + QMANA)
                {
                    Qr.Cast(t);
                    args.Process = false;
                    return;
                }

                if (R.IsReady() && t.IsValidTarget())
                    Rtime = Game.Time;
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

        private static void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            var t = gapcloser.Sender;

            if (Q.IsReady() && getCheckBoxItem(qMenu, "gapQ") && t.IsValidTarget(Q.Range))
            {
                Q.Cast(gapcloser.End);
            }
            else if (R.IsReady() && getCheckBoxItem(rMenu, "gapcloser" + gapcloser.Sender.ChampionName) &&
                     t.IsValidTarget(R.Range))
            {
                R.CastOnUnit(t);
            }
        }

        private static void Interrupter2_OnInterruptableTarget(AIHeroClient t,
            Interrupter2.InterruptableTargetEventArgs args)
        {
            if (!getCheckBoxItem(qMenu, "intQ") || !Q.IsReady())
                return;

            if (t.IsValidTarget(Q.Range))
            {
                Q.Cast(t);
            }
        }

        private static void Game_OnGameUpdate(EventArgs args)
        {
            if (Player.IsChannelingImportantSpell() || Game.Time - Rtime < 0.5)
            {
                Orbwalker.DisableAttacking = true;
                Orbwalker.DisableMovement = true;
                Program.debug("cast R");
                return;
            }
            Orbwalker.DisableAttacking = false;
            Orbwalker.DisableMovement = false;

            if (R.IsReady() && getKeyBindItem(rMenu, "useR"))
            {
                var t = TargetSelector.GetTarget(R.Range, DamageType.Magical);
                if (t.IsValidTarget(R.Range) && getCheckBoxItem(rMenu, "Ron" + t.ChampionName))
                {
                    R.CastOnUnit(t);
                    return;
                }
            }

            if (Program.LagFree(0))
            {
                SetMana();
                Jungle();
            }

            if (Program.LagFree(1) && E.IsReady() && getCheckBoxItem(eMenu, "autoE"))
                LogicE();
            if (Program.LagFree(2) && Q.IsReady() && getCheckBoxItem(qMenu, "autoQ"))
                LogicQ();
            if (Program.LagFree(3) && W.IsReady() && getCheckBoxItem(wMenu, "autoW"))
                LogicW();
            if (Program.LagFree(4) && R.IsReady() && getCheckBoxItem(rMenu, "autoR"))
                LogicR();
        }

        private static void LogicQ()
        {
            var t = TargetSelector.GetTarget(Q.Range, DamageType.Magical);
            if (t.IsValidTarget())
            {
                var qDmg = OktwCommon.GetKsDamage(t, Q) + BonusDmg(t);

                if (qDmg > t.Health)
                    Program.CastSpell(Q, t);

                if (R.IsReady() && t.IsValidTarget(R.Range))
                {
                    return;
                }
                if (Program.Combo && Player.Mana > RMANA + QMANA)
                    Program.CastSpell(Q, t);
                else if (Program.Farm && getCheckBoxItem(qMenu, "harrasQ") && Player.Mana > RMANA + EMANA + WMANA + EMANA)
                    Program.CastSpell(Q, t);

                if (Player.Mana > RMANA + QMANA)
                {
                    foreach (
                        var enemy in
                            Program.Enemies.Where(enemy => enemy.IsValidTarget(Q.Range) && !OktwCommon.CanMove(enemy)))
                        Q.Cast(enemy);
                }
            }
            else if (Program.LaneClear && Player.ManaPercent > getSliderItem(farmMenu, "Mana") &&
                     getCheckBoxItem(farmMenu, "farmQ"))
            {
                var allMinions = Cache.GetMinions(Player.ServerPosition, Q.Range);
                var farmPos = Q.GetCircularFarmLocation(allMinions, 150);
                if (farmPos.MinionsHit > getSliderItem(farmMenu, "LCminions"))
                    Q.Cast(farmPos.Position);
            }
        }

        private static void LogicW()
        {
            var t = TargetSelector.GetTarget(W.Range, DamageType.Magical);
            if (t.IsValidTarget())
            {
                var qDmg = Q.GetDamage(t);
                var wDmg = OktwCommon.GetKsDamage(t, W) + BonusDmg(t);
                if (wDmg > t.Health)
                {
                    Program.CastSpell(W, t);
                }
                else if (wDmg + qDmg > t.Health && Player.Mana > QMANA + EMANA)
                    Program.CastSpell(W, t);
                else if (Program.Combo && Player.Mana > RMANA + WMANA)
                    Program.CastSpell(W, t);
                else if (Program.Farm && getCheckBoxItem(wMenu, "harrasW") && !Player.UnderTurret(true) &&
                         (Player.Mana > Player.MaxMana*0.8 || W.Level > Q.Level) &&
                         Player.Mana > RMANA + WMANA + EMANA + QMANA + WMANA && OktwCommon.CanHarras())
                    Program.CastSpell(W, t);

                if (Player.Mana > RMANA + WMANA)
                {
                    foreach (
                        var enemy in
                            Program.Enemies.Where(enemy => enemy.IsValidTarget(W.Range) && !OktwCommon.CanMove(enemy)))
                        W.Cast(enemy, true);
                }
            }
            else if (Program.LaneClear && Player.ManaPercent > getSliderItem(farmMenu, "Mana") &&
                     getCheckBoxItem(farmMenu, "farmW"))
            {
                var allMinions = Cache.GetMinions(Player.ServerPosition, W.Range);
                var farmPos = W.GetCircularFarmLocation(allMinions, W.Width);
                if (farmPos.MinionsHit >= getSliderItem(farmMenu, "LCminions"))
                    W.Cast(farmPos.Position);
            }
        }

        private static void LogicE()
        {
            var t = TargetSelector.GetTarget(E.Range, DamageType.Magical);
            if (t.IsValidTarget())
            {
                var eDmg = OktwCommon.GetKsDamage(t, E) + BonusDmg(t);
                var wDmg = W.GetDamage(t);

                if (eDmg > t.Health)
                    E.CastOnUnit(t);
                else if (W.IsReady() && wDmg + eDmg > t.Health && Player.Mana > WMANA + EMANA)
                    E.CastOnUnit(t);
                else if (R.IsReady() && W.IsReady() && wDmg + eDmg + R.GetDamage(t) > t.Health &&
                         Player.Mana > WMANA + EMANA + RMANA)
                    E.CastOnUnit(t);
                if (Program.Combo && Player.Mana > RMANA + EMANA)
                    E.CastOnUnit(t);
                else if (Program.Farm && getCheckBoxItem(eMenu, "harrasE") && Player.Mana > RMANA + EMANA + WMANA + EMANA)
                    E.CastOnUnit(t);
            }
            else if (Program.LaneClear && Player.ManaPercent > getSliderItem(farmMenu, "Mana") &&
                     getCheckBoxItem(farmMenu, "farmE"))
            {
                var allMinions = Cache.GetMinions(Player.ServerPosition, E.Range);
                if (allMinions.Count >= getSliderItem(farmMenu, "LCminions"))
                {
                    foreach (
                        var minion in
                            allMinions.Where(
                                minion =>
                                    minion.IsValidTarget(E.Range) && minion.Health < E.GetDamage(minion) &&
                                    !minion.HasBuff("AlZaharMaleficVisions")))
                    {
                        E.CastOnUnit(minion);
                    }
                }
            }
            else if (Program.Farm && Player.Mana > RMANA + EMANA + WMANA + EMANA &&
                     getCheckBoxItem(eMenu, "harrasEminion"))
            {
                var te = TargetSelector.GetTarget(E.Range + 400, DamageType.Magical);
                if (te.IsValidTarget())
                {
                    var allMinions = Cache.GetMinions(Player.ServerPosition, E.Range);
                    foreach (
                        var minion in
                            allMinions.Where(
                                minion =>
                                    minion.IsValidTarget(E.Range) && minion.Health < E.GetDamage(minion) &&
                                    te.Distance(minion.Position) < 500 &&
                                    !minion.HasBuff("AlZaharMaleficVisions")))
                    {
                        E.CastOnUnit(minion);
                    }
                }
            }
        }

        private static void LogicR()
        {
            if (Player.UnderTurret(true) && getCheckBoxItem(rMenu, "Rturrent"))
                return;
            var t = TargetSelector.GetTarget(R.Range, DamageType.Magical);
            if (Player.CountEnemiesInRange(900) < 3 && t.IsValidTarget())
            {
                var totalComboDamage = OktwCommon.GetKsDamage(t, R);
                // E calculation

                totalComboDamage += E.GetDamage(t);

                if (W.IsReady() ||
                    WMissile != null && WMissile.Position.Distance(t.Position) < 250 && Player.Mana > RMANA + WMANA)
                {
                    totalComboDamage += W.GetDamage(t)*5;
                }

                if (Player.Mana > RMANA + QMANA)
                    totalComboDamage += Q.GetDamage(t);

                if (totalComboDamage > t.Health - OktwCommon.GetIncomingDamage(t) && OktwCommon.ValidUlt(t))
                {
                    R.CastOnUnit(t);
                }
            }
        }

        private static void Jungle()
        {
            if (Program.LaneClear && Player.Mana > RMANA + EMANA)
            {
                var mobs = Cache.GetMinions(Player.ServerPosition, 600, MinionTeam.Neutral);
                if (mobs.Count > 0)
                {
                    var mob = mobs[0];
                    if (W.IsReady() && getCheckBoxItem(farmMenu, "jungleW"))
                    {
                        W.Cast(mob.ServerPosition);
                        return;
                    }

                    if (Q.IsReady() && getCheckBoxItem(farmMenu, "jungleQ"))
                    {
                        Q.Cast(mob.ServerPosition);
                        return;
                    }

                    if (E.IsReady() && getCheckBoxItem(farmMenu, "jungleE") && mob.HasBuff("brandablaze"))
                    {
                        E.Cast(mob);
                    }
                }
            }
        }

        private static float BonusDmg(AIHeroClient target)
        {
            return (float) Player.CalcDamage(target, DamageType.Magical, target.MaxHealth*0.08 - target.HPRegenRate*5);
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
                RMANA = WMANA - Player.PARRegenRate*W.Instance.Cooldown;
            else
                RMANA = R.Instance.SData.Mana;
        }

        public static void drawLine(Vector3 pos1, Vector3 pos2, int bold, Color color)
        {
            var wts1 = Drawing.WorldToScreen(pos1);
            var wts2 = Drawing.WorldToScreen(pos2);

            Drawing.DrawLine(wts1[0], wts1[1], wts2[0], wts2[1], bold, color);
        }

        private static void Drawing_OnDraw(EventArgs args)
        {
            if (getCheckBoxItem(drawMenu, "qRange"))
            {
                if (getCheckBoxItem(drawMenu, "onlyRdy"))
                {
                    if (Q.IsReady())
                        Utility.DrawCircle(Player.Position, Q.Range, Color.Cyan, 1, 1);
                }
                else
                    Utility.DrawCircle(Player.Position, Q.Range, Color.Cyan, 1, 1);
            }

            if (getCheckBoxItem(drawMenu, "wRange"))
            {
                if (getCheckBoxItem(drawMenu, "onlyRdy"))
                {
                    if (W.IsReady())
                        Utility.DrawCircle(Player.Position, W.Range, Color.Orange, 1, 1);
                }
                else
                    Utility.DrawCircle(Player.Position, W.Range, Color.Orange, 1, 1);
            }

            if (getCheckBoxItem(drawMenu, "eRange"))
            {
                if (getCheckBoxItem(drawMenu, "onlyRdy"))
                {
                    if (E.IsReady())
                        Utility.DrawCircle(Player.Position, E.Range, Color.Yellow, 1, 1);
                }
                else
                    Utility.DrawCircle(Player.Position, E.Range, Color.Yellow, 1, 1);
            }

            if (getCheckBoxItem(drawMenu, "rRange"))
            {
                if (getCheckBoxItem(drawMenu, "onlyRdy"))
                {
                    if (R.IsReady())
                        Utility.DrawCircle(Player.Position, R.Range, Color.Gray, 1, 1);
                }
                else
                    Utility.DrawCircle(Player.Position, R.Range, Color.Gray, 1, 1);
            }

            if (getCheckBoxItem(drawMenu, "noti") && R.IsReady())
            {
            }
        }
    }
}