using System;
using System.Collections.Generic;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using SebbyLib;
using EloBuddy.SDK;
using EloBuddy;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;

namespace OneKeyToWin_AIO_Sebby.Champions
{
    class Lux
    {
        private static Menu Config = Program.Config;
        private static LeagueSharp.Common.Spell E, Q, R, W, Qcol;
        private static float QMANA = 0, WMANA = 0, EMANA = 0, RMANA = 0;
        public static AIHeroClient Player { get { return ObjectManager.Player; } }
        private static Vector3 Epos = Vector3.Zero;
        private static float DragonDmg = 0;
        private static double DragonTime = 0;

        public static Menu drawMenu, qMenu, eMenu, wMenu, rMenu, harassMenu, farmMenu;

        public static void LoadOKTW()
        {
            Q = new LeagueSharp.Common.Spell(SpellSlot.Q, 1175);
            Qcol = new LeagueSharp.Common.Spell(SpellSlot.Q, 1175);
            W = new LeagueSharp.Common.Spell(SpellSlot.W, 1075);
            E = new LeagueSharp.Common.Spell(SpellSlot.E, 1075);
            R = new LeagueSharp.Common.Spell(SpellSlot.R, 3000);

            Qcol.SetSkillshot(0.25f, 70f, 1200f, true, SkillshotType.SkillshotLine);
            Q.SetSkillshot(0.25f, 70f, 1200f, false, SkillshotType.SkillshotLine);
            W.SetSkillshot(0.25f, 110f, 1200f, false, SkillshotType.SkillshotLine);
            E.SetSkillshot(0.3f, 250f, 1050f, false, SkillshotType.SkillshotCircle);
            R.SetSkillshot(1f, 110f, float.MaxValue, false, SkillshotType.SkillshotLine);

            drawMenu = Config.AddSubMenu("Drawings");
            drawMenu.Add("noti", new CheckBox("Show notification", true));
            drawMenu.Add("qRange", new CheckBox("Q range", false));
            drawMenu.Add("wRange", new CheckBox("W range", false));
            drawMenu.Add("eRange", new CheckBox("E range", false));
            drawMenu.Add("rRange", new CheckBox("R range", false));
            drawMenu.Add("rRangeMini", new CheckBox("R range minimap", true));
            drawMenu.Add("onlyRdy", new CheckBox("Draw when skill rdy", true));

            qMenu = Config.AddSubMenu("Q Config");
            qMenu.Add("autoQ", new CheckBox("Auto Q", true));
            qMenu.Add("harrasQ", new CheckBox("Harass Q", true));
            foreach (var enemy in ObjectManager.Get<AIHeroClient>().Where(enemy => enemy.IsEnemy))
                qMenu.Add("Qon" + enemy.ChampionName, new CheckBox("Use Q On : " + enemy.ChampionName));
            qMenu.AddSeparator();
            qMenu.AddGroupLabel("Gap Closer Settings : ");
            qMenu.Add("gapQ", new CheckBox("Auto Q Gap Closer", true));
            foreach (var enemy in ObjectManager.Get<AIHeroClient>().Where(enemy => enemy.IsEnemy))
                qMenu.Add("Qgap" + enemy.ChampionName, new CheckBox("GapClose : " + enemy.ChampionName));

            eMenu = Config.AddSubMenu("E Config");
            eMenu.Add("autoE", new CheckBox("Auto E", true));
            eMenu.Add("harrasE", new CheckBox("Harras E", false));
            eMenu.Add("autoEcc", new CheckBox("Auto E only CC enemy", false));
            eMenu.Add("autoEslow", new CheckBox("Auto E slow logic detonate", true));
            eMenu.Add("autoEdet", new CheckBox("Only detonate if target in E ", false));

            wMenu = Config.AddSubMenu("W Config");
            wMenu.Add("Wdmg", new Slider("W dmg % hp", 10, 0, 100));
            wMenu.AddSeparator();
            foreach (var ally in ObjectManager.Get<AIHeroClient>().Where(enemy => enemy.Team == Player.Team))
            {
                wMenu.AddGroupLabel(ally.ChampionName);
                wMenu.Add("damage" + ally.ChampionName, new CheckBox("Damage incoming", true));
                wMenu.Add("HardCC" + ally.ChampionName, new CheckBox("Hard CC", true));
                wMenu.Add("Poison" + ally.ChampionName, new CheckBox("Poison", true));
                wMenu.AddSeparator();
            }

            rMenu = Config.AddSubMenu("R Config");
            rMenu.Add("autoR", new CheckBox("Auto R", true));
            rMenu.Add("Rcc", new CheckBox("R fast KS combo", true));
            rMenu.Add("RaoeCount", new Slider("R x enemies in combo [0 == off]", 3, 0, 5));
            rMenu.Add("hitchanceR", new Slider("Hit Chance R", 2, 0, 3));
            rMenu.Add("useR", new KeyBind("Semi-manual cast R key", false, KeyBind.BindTypes.HoldActive, 'T'));
            rMenu.Add("Rjungle", new CheckBox("R Jungle stealer", true));
            rMenu.Add("Rdragon", new CheckBox("Dragon", true));
            rMenu.Add("Rbaron", new CheckBox("Baron", true));
            rMenu.Add("Rred", new CheckBox("Red", true));
            rMenu.Add("Rblue", new CheckBox("Blue", true));
            rMenu.Add("Rally", new CheckBox("Ally stealer", false));

            harassMenu = Config.AddSubMenu("Harass");
            foreach (var enemy in ObjectManager.Get<AIHeroClient>().Where(enemy => enemy.IsEnemy))
                harassMenu.Add("harras" + enemy.ChampionName, new CheckBox(enemy.ChampionName));

            farmMenu = Config.AddSubMenu("Farm");
            farmMenu.Add("farmE", new CheckBox("Lane clear E", true));
            farmMenu.Add("Mana", new Slider("LaneClear Mana", 80, 0, 100));
            farmMenu.Add("LCminions", new Slider("LaneClear minimum minions", 2, 0, 10));
            farmMenu.Add("jungleQ", new CheckBox("Jungle clear Q", true));
            farmMenu.Add("jungleE", new CheckBox("Jungle clear E", true));

            Game.OnUpdate += Game_OnGameUpdate;
            Drawing.OnDraw += Drawing_OnDraw;
            Drawing.OnEndScene += Drawing_OnEndScene;
            Obj_AI_Base.OnProcessSpellCast += Obj_AI_Base_OnProcessSpellCast;
            AntiGapcloser.OnEnemyGapcloser += AntiGapcloser_OnEnemyGapcloser;
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
            if (Q.IsReady() && gapcloser.Sender.IsValidTarget(Q.Range) && getCheckBoxItem(qMenu, "gapQ") && getCheckBoxItem(qMenu, "Qgap" + gapcloser.Sender.ChampionName))
                Q.Cast(gapcloser.Sender);
        }


        private static void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.IsMe && args.SData.Name == "LuxLightStrikeKugel")
            {
                Epos = args.End;
            }


        }

        private static void Game_OnGameUpdate(EventArgs args)
        {
            if (R.IsReady())
            {
                if (getCheckBoxItem(rMenu, "Rjungle"))
                {
                    KsJungle();
                }

                if (getKeyBindItem(rMenu, "useR"))
                {
                    var t = TargetSelector.GetTarget(R.Range, DamageType.Magical);
                    if (t.IsValidTarget())
                        R.Cast(t, true, true);
                }
            }
            else
                DragonTime = 0;


            if (Program.LagFree(0))
            {
                SetMana();
                Jungle();
            }

            if ((Program.LagFree(4) || Program.LagFree(1) || Program.LagFree(3)) && W.IsReady() && !Player.IsRecalling())
                LogicW();
            if (Program.LagFree(1) && Q.IsReady() && getCheckBoxItem(qMenu, "autoQ"))
                LogicQ();
            if (Program.LagFree(2) && E.IsReady() && getCheckBoxItem(eMenu, "autoE"))
                LogicE();
            if (Program.LagFree(3) && R.IsReady())
                LogicR();
        }

        private static void LogicW()
        {
            foreach (var ally in Program.Allies.Where(ally => ally.IsValid && !ally.IsDead && getCheckBoxItem(wMenu, "damage" + ally.ChampionName) && Player.ServerPosition.Distance(ally.ServerPosition) < W.Range))
            {
                double dmg = OktwCommon.GetIncomingDamage(ally);


                int nearEnemys = ally.CountEnemiesInRange(800);

                if (dmg == 0 && nearEnemys == 0)
                    continue;

                int sensitivity = 20;

                double HpPercentage = (dmg * 100) / ally.Health;
                double shieldValue = 65 + W.Level * 25 + 0.35 * Player.FlatMagicDamageMod;

                if (getCheckBoxItem(wMenu, "HardCC" + ally.ChampionName) && nearEnemys > 0 && HardCC(ally))
                {
                    W.CastOnUnit(ally);
                }
                else if (getCheckBoxItem(wMenu, "Poison" + ally.ChampionName) && ally.HasBuffOfType(BuffType.Poison))
                {
                    W.Cast(W.GetPrediction(ally).CastPosition);
                }

                nearEnemys = (nearEnemys == 0) ? 1 : nearEnemys;

                if (dmg > shieldValue)
                    W.Cast(W.GetPrediction(ally).CastPosition);
                else if (dmg > 100 + Player.Level * sensitivity)
                    W.Cast(W.GetPrediction(ally).CastPosition);
                else if (ally.Health - dmg < nearEnemys * ally.Level * sensitivity)
                    W.Cast(W.GetPrediction(ally).CastPosition);
                else if (HpPercentage >= getSliderItem(wMenu, "Wdmg"))
                    W.Cast(W.GetPrediction(ally).CastPosition);
            }
        }

        private static void LogicQ()
        {
            foreach (var enemy in Program.Enemies.Where(enemy => enemy.IsValidTarget(Q.Range) && E.GetDamage(enemy) + Q.GetDamage(enemy) + BonusDmg(enemy) > enemy.Health))
            {
                CastQ(enemy);
                return;
            }

            var t = Orbwalker.LastTarget as AIHeroClient;
            if (!t.IsValidTarget())
                t = TargetSelector.GetTarget(Q.Range, DamageType.Magical);
            if (t.IsValidTarget() && getCheckBoxItem(qMenu, "Qon" + t.ChampionName))
            {
                if (Program.Combo && Player.Mana > RMANA + QMANA)
                    CastQ(t);
                else if (Program.Farm && getCheckBoxItem(qMenu, "harrasQ") && getCheckBoxItem(harassMenu, "harras" + t.ChampionName) && Player.Mana > RMANA + EMANA + WMANA + EMANA)
                    CastQ(t);
                else if (OktwCommon.GetKsDamage(t, Q) > t.Health)
                    CastQ(t);

                foreach (var enemy in Program.Enemies.Where(enemy => enemy.IsValidTarget(Q.Range) && !OktwCommon.CanMove(enemy)))
                    CastQ(enemy);
            }
        }

        private static void CastQ(Obj_AI_Base t)
        {
            var poutput = Qcol.GetPrediction(t);
            var col = poutput.CollisionObjects.Count(ColObj => ColObj.IsEnemy && ColObj.IsMinion && !ColObj.IsDead);

            if (col < 4)
                Program.CastSpell(Q, t);
        }

        private static void LogicE()
        {
            if (Player.HasBuff("LuxLightStrikeKugel") && !Program.None)
            {
                int eBig = Epos.CountEnemiesInRange(350);
                if (getCheckBoxItem(eMenu, "autoEslow"))
                {
                    int detonate = eBig - Epos.CountEnemiesInRange(160);

                    if (detonate > 0 || eBig > 1)
                        E.Cast();
                }
                else if (getCheckBoxItem(eMenu, "autoEdet"))
                {
                    if (eBig > 0)
                        E.Cast();
                }
                else
                {
                    E.Cast();
                }
            }
            else
            {
                var t = TargetSelector.GetTarget(E.Range, DamageType.Magical);
                if (t.IsValidTarget())
                {
                    if (!getCheckBoxItem(eMenu, "autoEcc"))
                    {
                        if (Program.Combo && Player.Mana > RMANA + EMANA)
                            Program.CastSpell(E, t);
                        else if (Program.Farm && OktwCommon.CanHarras() && getCheckBoxItem(eMenu, "harrasE") && Player.Mana > RMANA + EMANA + EMANA + RMANA)
                            Program.CastSpell(E, t);
                        else if (OktwCommon.GetKsDamage(t, E) > t.Health)
                            Program.CastSpell(E, t);
                    }

                    foreach (var enemy in Program.Enemies.Where(enemy => enemy.IsValidTarget(E.Range) && !OktwCommon.CanMove(enemy)))
                        E.Cast(enemy, true);
                }
                else if (Program.LaneClear && Player.ManaPercent > getSliderItem(farmMenu, "Mana") && getCheckBoxItem(farmMenu, "farmE") && Player.Mana > RMANA + WMANA)
                {
                    var minionList = Cache.GetMinions(Player.ServerPosition, E.Range);
                    var farmPosition = E.GetCircularFarmLocation(minionList, E.Width);

                    if (farmPosition.MinionsHit > getSliderItem(farmMenu, "LCminions"))
                        E.Cast(farmPosition.Position);
                }
            }
        }

        private static void LogicR()
        {
            if (getCheckBoxItem(rMenu, "autoR"))
            {
                foreach (var target in Program.Enemies.Where(target => target.IsValidTarget(R.Range) && target.CountAlliesInRange(600) < 2 && OktwCommon.ValidUlt(target)))
                {
                    float predictedHealth = target.Health + target.HPRegenRate * 2;
                    float Rdmg = OktwCommon.GetKsDamage(target, R);

                    if (Items.HasItem(3155, target))
                    {
                        Rdmg = Rdmg - 250;
                    }

                    if (Items.HasItem(3156, target))
                    {
                        Rdmg = Rdmg - 400;
                    }

                    if (target.HasBuff("luxilluminatingfraulein"))
                    {
                        Rdmg += (float)Player.CalcDamage(target, DamageType.Magical, 10 + (8 * Player.Level) + 0.2 * Player.FlatMagicDamageMod);
                    }

                    if (Player.HasBuff("itemmagicshankcharge"))
                    {
                        if (Player.GetBuff("itemmagicshankcharge").Count == 100)
                        {
                            Rdmg += (float)Player.CalcDamage(target, DamageType.Magical, 100 + 0.1 * Player.FlatMagicDamageMod);
                        }
                    }

                    if (Rdmg > predictedHealth)
                    {
                        castR(target);
                        Program.debug("R normal");
                    }
                    else if (!OktwCommon.CanMove(target) && getCheckBoxItem(rMenu, "Rcc") && target.IsValidTarget(E.Range))
                    {
                        float dmgCombo = Rdmg;

                        if (E.IsReady())
                        {
                            var eDmg = E.GetDamage(target);

                            if (eDmg > predictedHealth)
                                return;
                            else
                                dmgCombo += eDmg;
                        }

                        if (target.IsValidTarget(800))
                            dmgCombo += BonusDmg(target);

                        if (dmgCombo > predictedHealth)
                        {
                            R.CastIfWillHit(target, 2);
                            R.Cast(target);
                        }

                    }
                    else if (Program.Combo && getSliderItem(rMenu, "RaoeCount") > 0)
                    {
                        R.CastIfWillHit(target, getSliderItem(rMenu, "RaoeCount"));
                    }
                }
            }
        }

        private static float BonusDmg(AIHeroClient target)
        {
            float damage = 10 + (Player.Level) * 8 + 0.2f * Player.FlatMagicDamageMod;
            if (Player.HasBuff("lichbane"))
            {
                damage += (Player.BaseAttackDamage * 0.75f) + ((Player.BaseAbilityDamage + Player.FlatMagicDamageMod) * 0.5f);
            }

            return (float)(Player.GetAutoAttackDamage(target) + Player.CalcDamage(target, DamageType.Magical, damage));
        }

        private static void castR(AIHeroClient target)
        {
            var inx = getSliderItem(rMenu, "hitchanceR");
            if (inx == 0)
            {
                R.Cast(R.GetPrediction(target).CastPosition);
            }
            else if (inx == 1)
            {
                R.Cast(target);
            }
            else if (inx == 2)
            {
                Program.CastSpell(R, target);
            }
            else if (inx == 3)
            {
                List<Vector2> waypoints = target.GetWaypoints();
                if ((Player.Distance(waypoints.Last<Vector2>().To3D()) - Player.Distance(target.Position)) > 400)
                {
                    Program.CastSpell(R, target);
                }
            }
        }

        private static void Jungle()
        {
            if (Program.LaneClear && Player.Mana > RMANA + WMANA + RMANA + WMANA)
            {
                var mobs = Cache.GetMinions(Player.ServerPosition, 600, MinionTeam.Neutral);
                if (mobs.Count > 0)
                {
                    var mob = mobs[0];
                    if (Q.IsReady() && getCheckBoxItem(farmMenu, "jungleQ"))
                    {
                        Q.Cast(mob.ServerPosition);
                        return;
                    }
                    if (E.IsReady() && getCheckBoxItem(farmMenu, "jungleE"))
                    {
                        E.Cast(mob.ServerPosition);
                        return;
                    }
                }
            }
        }

        private static void KsJungle()
        {
            var mobs = Cache.GetMinions(Player.ServerPosition, R.Range, MinionTeam.Neutral);
            foreach (var mob in mobs)
            {
                //debug(mob.SkinName);
                if (((mob.BaseSkinName == "SRU_Dragon" && getCheckBoxItem(rMenu, "Rdragon"))
                    || (mob.BaseSkinName == "SRU_Baron" && getCheckBoxItem(rMenu, "Rbaron"))
                    || (mob.BaseSkinName == "SRU_Red" && getCheckBoxItem(rMenu, "Rred"))
                    || (mob.BaseSkinName == "SRU_Blue" && getCheckBoxItem(rMenu, "Rblue")))
                    && (mob.CountAlliesInRange(1000) == 0 || getCheckBoxItem(rMenu, "Rally"))
                    && mob.Health < mob.MaxHealth
                    && mob.Distance(Player.Position) > 1000
                    )
                {
                    if (DragonDmg == 0)
                        DragonDmg = mob.Health;

                    if (Game.Time - DragonTime > 3)
                    {
                        if (DragonDmg - mob.Health > 0)
                        {
                            DragonDmg = mob.Health;
                        }
                        DragonTime = Game.Time;
                    }
                    else
                    {
                        var DmgSec = (DragonDmg - mob.Health) * (Math.Abs(DragonTime - Game.Time) / 3);
                        //Program.debug("DS  " + DmgSec);
                        if (DragonDmg - mob.Health > 0)
                        {
                            var timeTravel = R.Delay;
                            var timeR = (mob.Health - R.GetDamage(mob)) / (DmgSec / 3);
                            //Program.debug("timeTravel " + timeTravel + "timeR " + timeR + "d " + R.GetDamage(mob));
                            if (timeTravel > timeR)
                                R.Cast(mob.Position);
                        }
                        else
                            DragonDmg = mob.Health;

                        //Program.debug("" + GetUltTravelTime(ObjectManager.Player, R.Speed, R.Delay, mob.Position));
                    }
                }
            }
        }


        private static bool HardCC(AIHeroClient target)
        {
            if (target.HasBuffOfType(BuffType.Stun) || target.HasBuffOfType(BuffType.Snare) || target.HasBuffOfType(BuffType.Knockup) ||
                target.HasBuffOfType(BuffType.Charm) || target.HasBuffOfType(BuffType.Fear) || target.HasBuffOfType(BuffType.Knockback) ||
                target.HasBuffOfType(BuffType.Taunt) || target.HasBuffOfType(BuffType.Suppression) ||
                target.IsStunned)
            {
                return true;

            }
            else
                return false;
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
                RMANA = QMANA - Player.PARRegenRate * Q.Instance.Cooldown;
            else
                RMANA = R.Instance.SData.Mana;
        }

        public static void drawLine(Vector3 pos1, Vector3 pos2, int bold, System.Drawing.Color color)
        {
            var wts1 = Drawing.WorldToScreen(pos1);
            var wts2 = Drawing.WorldToScreen(pos2);

            Drawing.DrawLine(wts1[0], wts1[1], wts2[0], wts2[1], bold, color);
        }

        private static void Drawing_OnEndScene(EventArgs args)
        {

            if (getCheckBoxItem(drawMenu, "rRangeMini"))
            {
                if (R.IsReady())
                    LeagueSharp.Common.Utility.DrawCircle(Player.Position, R.Range, System.Drawing.Color.Aqua, 1, 20, true);
            }
            else
                LeagueSharp.Common.Utility.DrawCircle(Player.Position, R.Range, System.Drawing.Color.Aqua, 1, 20, true);


        }

        private static void Drawing_OnDraw(EventArgs args)
        {

            if (getCheckBoxItem(drawMenu, "qRange"))
            {
                if (getCheckBoxItem(drawMenu, "onlyRdy"))
                {
                    if (Q.IsReady())
                        LeagueSharp.Common.Utility.DrawCircle(Player.Position, Q.Range, System.Drawing.Color.Cyan, 1, 1);
                }
                else
                    LeagueSharp.Common.Utility.DrawCircle(Player.Position, Q.Range, System.Drawing.Color.Cyan, 1, 1);
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
                        LeagueSharp.Common.Utility.DrawCircle(Player.Position, E.Range, System.Drawing.Color.Yellow, 1, 1);
                }
                else
                    LeagueSharp.Common.Utility.DrawCircle(Player.Position, E.Range, System.Drawing.Color.Yellow, 1, 1);
            }
            if (getCheckBoxItem(drawMenu, "rRange"))
            {
                if (getCheckBoxItem(drawMenu, "onlyRdy"))
                {
                    if (R.IsReady())
                        LeagueSharp.Common.Utility.DrawCircle(Player.Position, R.Range, System.Drawing.Color.Gray, 1, 1);
                }
                else
                    LeagueSharp.Common.Utility.DrawCircle(Player.Position, R.Range, System.Drawing.Color.Gray, 1, 1);
            }
            if (R.IsReady() && getCheckBoxItem(drawMenu, "noti"))
            {
                var t = TargetSelector.GetTarget(R.Range, DamageType.Physical);

                if (t.IsValidTarget() && R.GetDamage(t) > t.Health)
                {
                    Drawing.DrawText(Drawing.Width * 0.1f, Drawing.Height * 0.5f, System.Drawing.Color.Red, "Ult can kill: " + t.ChampionName + " have: " + t.Health + "hp");
                    drawLine(t.Position, Player.Position, 5, System.Drawing.Color.Red);
                }
            }
        }
    }
}