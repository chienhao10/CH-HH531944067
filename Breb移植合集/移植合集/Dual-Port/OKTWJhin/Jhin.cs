
using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using SebbyLib;
using EloBuddy;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu.Values;

namespace OneKeyToWin_AIO_Sebby.Champions
{
    class Jhin
    {
        private static Menu Config = Program.Config;
        private static LeagueSharp.Common.Spell E, Q, R, W;
        private static float QMANA = 0, WMANA = 0, EMANA = 0, RMANA = 0;
        public static AIHeroClient Player { get { return ObjectManager.Player; } }
        private static Vector3 rPosLast;
        private static AIHeroClient rTargetLast;
        private static Vector3 rPosCast;

        private static Items.Item
                    FarsightOrb = new Items.Item(3342, 4000f),
                    ScryingOrb = new Items.Item(3363, 3500f);

        private static string[] Spells =
        {
            "katarinar","drain","consume","absolutezero", "staticfield","reapthewhirlwind","jinxw","jinxr","shenstandunited","threshe","threshrpenta","threshq","meditate","caitlynpiltoverpeacemaker", "volibearqattack",
            "cassiopeiapetrifyinggaze","ezrealtrueshotbarrage","galioidolofdurand","luxmalicecannon", "missfortunebullettime","infiniteduress","alzaharnethergrasp","lucianq","velkozr","rocketgrabmissile"
        };

        public static Menu drawMenu, qMenu, wMenu, eMenu, rMenu, farmMenu, harassMenu;

        public static bool getBushE()
        {
            return getCheckBoxItem(eMenu, "bushE");
        }

        public static void LoadOKTW()
        {
            Q = new LeagueSharp.Common.Spell(SpellSlot.Q, 600);
            W = new LeagueSharp.Common.Spell(SpellSlot.W, 2500);
            E = new LeagueSharp.Common.Spell(SpellSlot.E, 760);
            R = new LeagueSharp.Common.Spell(SpellSlot.R, 3500);

            W.SetSkillshot(0.75f, 40, float.MaxValue, false, SkillshotType.SkillshotLine);
            E.SetSkillshot(1.3f, 200, 1600, false, SkillshotType.SkillshotCircle);
            R.SetSkillshot(0.2f, 80, 5000, false, SkillshotType.SkillshotLine);

            drawMenu = Config.AddSubMenu("Drawings");
            drawMenu.Add("qRange", new CheckBox("Q range", false));
            drawMenu.Add("wRange", new CheckBox("W range", false));
            drawMenu.Add("eRange", new CheckBox("E range", false));
            drawMenu.Add("rRange", new CheckBox("R range", false));
            drawMenu.Add("onlyRdy", new CheckBox("Draw only ready spells", true));

            qMenu = Config.AddSubMenu("Q Config");
            qMenu.Add("autoQ", new CheckBox("Auto Q", true));
            qMenu.Add("harrasQ", new CheckBox("Harass Q", true));
            qMenu.Add("Qminion", new CheckBox("Q on minion", true));

            wMenu = Config.AddSubMenu("W Config");
            wMenu.Add("autoW", new CheckBox("Auto W", true));
            wMenu.Add("autoWcombo", new CheckBox("Auto W only in combo", false));
            wMenu.Add("harrasW", new CheckBox("Harass W", true));
            wMenu.Add("Wstun", new CheckBox("W stun, marked only", true));
            wMenu.Add("Waoe", new CheckBox("W aoe (above 2 enemy)", true));
            wMenu.Add("autoWcc", new CheckBox("Auto W CC enemy or marked", true));
            wMenu.Add("MaxRangeW", new Slider("Max W range", 2500, 0, 2500));

            eMenu = Config.AddSubMenu("E Config");
            eMenu.Add("autoE", new CheckBox("Auto E on hard CC", true));
            eMenu.Add("bushE", new CheckBox("Auto E bush", true));
            eMenu.Add("Espell", new CheckBox("E on special spell detection", true));
            eMenu.Add("EmodeCombo", new ComboBox("E combo mode", 1, "always", "run - cheese", "disable"));
            eMenu.Add("Eaoe", new Slider("Auto E x enemies", 2, 0, 5));
            eMenu.AddGroupLabel("E GapClose :");
            eMenu.Add("EmodeGC", new ComboBox("Gap Closer position mode", 0, "Dash end position", "My hero position"));
            foreach (var enemy in ObjectManager.Get<AIHeroClient>().Where(enemy => enemy.IsEnemy))
                eMenu.Add("EGCchampion" + enemy.ChampionName, new CheckBox("GapClose : " + enemy.ChampionName, true));

            rMenu = Config.AddSubMenu("R Config");
            rMenu.Add("autoR", new CheckBox("Enable R", true));
            rMenu.Add("Rvisable", new CheckBox("Don't shot if enemy is not visable", false));
            rMenu.Add("Rks", new CheckBox("Auto R if can kill in 3 hits", true));
            rMenu.Add("useR", new KeyBind("Semi-manual cast R key", false, KeyBind.BindTypes.HoldActive, 'T'));
            rMenu.Add("MaxRangeR", new Slider("Max R range", 3000, 0, 3500));
            rMenu.Add("MinRangeR", new Slider("Min R range", 1000, 0, 3500));
            rMenu.Add("Rsafe", new Slider("R safe area", 1000, 0, 2000));
            rMenu.Add("trinkiet", new CheckBox("Auto blue trinkiet", true));

            harassMenu = Config.AddSubMenu("Harass");
            foreach (var enemy in ObjectManager.Get<AIHeroClient>().Where(enemy => enemy.IsEnemy))
                harassMenu.Add("harras" + enemy.ChampionName, new CheckBox(enemy.ChampionName));

            farmMenu = Config.AddSubMenu("Farm");
            farmMenu.Add("farmQ", new CheckBox("Lane clear Q", true));
            farmMenu.Add("farmW", new CheckBox("Lane clear W", false));
            farmMenu.Add("farmE", new CheckBox("Lane clear E", true));
            farmMenu.Add("Mana", new Slider("LaneClear Mana", 50, 0, 100));
            farmMenu.Add("LCminions", new Slider("LaneClear minimum minions", 5, 0, 10));
            farmMenu.Add("jungleE", new CheckBox("Jungle clear E", true));
            farmMenu.Add("jungleQ", new CheckBox("Jungle clear Q", true));
            farmMenu.Add("jungleW", new CheckBox("Jungle clear W", true));

            Drawing.OnDraw += Drawing_OnDraw;
            Game.OnUpdate += Game_OnGameUpdate;
            AntiGapcloser.OnEnemyGapcloser += AntiGapcloser_OnEnemyGapcloser;
            Obj_AI_Base.OnProcessSpellCast += Obj_AI_Base_OnProcessSpellCast;
            Spellbook.OnCastSpell += Spellbook_OnCastSpell;
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

        private static void Spellbook_OnCastSpell(Spellbook sender, SpellbookCastSpellEventArgs args)
        {
            if (args.Slot == SpellSlot.R)
            {
                if (getCheckBoxItem(rMenu, "trinkiet") && !IsCastingR)
                {
                    if (Player.Level < 9)
                        ScryingOrb.Range = 2500;
                    else
                        ScryingOrb.Range = 3500;

                    if (ScryingOrb.IsReady())
                        ScryingOrb.Cast(rPosLast);
                    if (FarsightOrb.IsReady())
                        FarsightOrb.Cast(rPosLast);
                }
            }
        }

        private static void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.IsMe && args.SData.Name.ToLower() == "jhinr")
            {
                rPosCast = args.End;
            }
            if (!E.IsReady() || sender.IsMinion || !sender.IsEnemy || !getCheckBoxItem(eMenu, "Espell") || !sender.IsValid<AIHeroClient>() || !sender.LSIsValidTarget(E.Range))
                return;

            var foundSpell = Spells.Find(x => args.SData.Name.ToLower() == x);
            if (foundSpell != null)
            {
                E.Cast(sender.Position);
            }
        }

        private static void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (E.IsReady() && Player.Mana > RMANA + WMANA)
            {
                var t = gapcloser.Sender;
                if (t.LSIsValidTarget(W.Range) && getCheckBoxItem(eMenu, "EGCchampion" + t.ChampionName))
                {
                    if (getBoxItem(eMenu, "EmodeGC") == 0)
                        E.Cast(gapcloser.End);
                    else
                        E.Cast(Player.ServerPosition);
                }
            }
        }

        private static void Game_OnGameUpdate(EventArgs args)
        {

            if (Program.LagFree(0))
            {
                SetMana();
                Jungle();
            }

            if (Program.LagFree(1) && R.IsReady() && getCheckBoxItem(rMenu, "autoR"))
                LogicR();

            if (IsCastingR)
            {
                Orbwalker.DisableAttacking = true;
                Orbwalker.DisableMovement = true;
                return;
            }
            else
            {
                Orbwalker.DisableAttacking = false;
                Orbwalker.DisableMovement = false;
            }


            if (Program.LagFree(4) && E.IsReady())
                LogicE();

            if (Program.LagFree(2) && Q.IsReady() && getCheckBoxItem(qMenu, "autoQ"))
                LogicQ();

            if (Program.LagFree(3) && W.IsReady() && !Player.Spellbook.IsAutoAttacking && getCheckBoxItem(wMenu, "autoW"))
                LogicW();
        }

        private static void LogicR()
        {
            if (!IsCastingR)
                R.Range = getSliderItem(rMenu, "MaxRangeR");
            else
                R.Range = 3500;

            var t = TargetSelector.GetTarget(R.Range, DamageType.Physical);
            if (t.IsValidTarget())
            {
                rPosLast = R.GetPrediction(t).CastPosition;
                if (getKeyBindItem(rMenu, "useR") && !IsCastingR)
                {
                    R.Cast(rPosLast);
                    rTargetLast = t;
                }

                if (!IsCastingR && getCheckBoxItem(rMenu, "Rks")
                    && GetRdmg(t) * 4 > t.Health && t.CountAlliesInRange(700) == 0 && Player.LSCountEnemiesInRange(getSliderItem(rMenu, "Rsafe")) == 0
                    && Player.LSDistance(t) > getSliderItem(rMenu, "MinRangeR")
                    && !Player.UnderTurret(true) && OktwCommon.ValidUlt(t) && !OktwCommon.IsSpellHeroCollision(t, R))
                {
                    R.Cast(rPosLast);
                    rTargetLast = t;
                }
                if (IsCastingR)
                {
                    if (InCone(t.ServerPosition))
                        R.Cast(t);
                    else
                    {
                        foreach (var enemy in Program.Enemies.Where(enemy => enemy.LSIsValidTarget(R.Range) && InCone(t.ServerPosition)).OrderBy(enemy => enemy.Health))
                        {
                            R.Cast(t);
                            rPosLast = R.GetPrediction(enemy).CastPosition;
                            rTargetLast = enemy;
                        }
                    }
                }
            }
            else if (IsCastingR && rTargetLast != null && !rTargetLast.IsDead)
            {
                if (!getCheckBoxItem(rMenu, "Rvisable") && InCone(rTargetLast.Position) && InCone(rPosLast))
                    R.Cast(rPosLast);
            }
        }

        private static void LogicW()
        {
            var t = TargetSelector.GetTarget(W.Range, DamageType.Physical);
            if (t.LSIsValidTarget())
            {
                var wDmg = GetWdmg(t);
                if (wDmg > t.Health - OktwCommon.GetIncomingDamage(t))
                    Program.CastSpell(W, t);

                if (getCheckBoxItem(wMenu, "autoWcombo") && !Program.Combo)
                    return;

                if (Player.LSCountEnemiesInRange(400) > 1 || Player.LSCountEnemiesInRange(250) > 0)
                    return;

                if (t.HasBuff("jhinespotteddebuff") || !getCheckBoxItem(wMenu, "Wstun"))
                {
                    if (Player.LSDistance(t) < getSliderItem(wMenu, "MaxRangeW"))
                    {
                        if (Program.Combo && Player.Mana > RMANA + WMANA)
                            Program.CastSpell(W, t);
                        else if (Program.Farm && getCheckBoxItem(wMenu, "harrasW") && getCheckBoxItem(harassMenu, "harras" + t.ChampionName)
                            && Player.Mana > RMANA + WMANA + QMANA + WMANA && OktwCommon.CanHarras())
                            Program.CastSpell(W, t);
                    }
                }


                if (!Program.None && Player.Mana > RMANA + WMANA)
                {
                    if (getCheckBoxItem(wMenu, "Waoe"))
                        W.CastIfWillHit(t, 2);
                    if (getCheckBoxItem(wMenu, "autoWcc") && !Program.Combo)
                    {
                        foreach (var enemy in Program.Enemies.Where(enemy => enemy.LSIsValidTarget(W.Range) && (!OktwCommon.CanMove(enemy) || enemy.HasBuff("jhinespotteddebuff"))))
                        {
                            if (!OktwCommon.CanMove(enemy) && !enemy.CanMove)
                            {
                                W.Cast(enemy);
                            }
                            if (enemy.HasBuff("jhinespotteddebuff"))
                            {
                                Program.CastSpell(W, enemy);
                            }
                        }
                    }
                }
            }
            if (Program.LaneClear && Player.ManaPercent > getSliderItem(farmMenu, "Mana") && getCheckBoxItem(farmMenu, "farmW") && Player.Mana > RMANA + WMANA)
            {
                var minionList = Cache.GetMinions(Player.ServerPosition, W.Range);
                var farmPosition = W.GetLineFarmLocation(minionList, W.Width);

                if (farmPosition.MinionsHit >= getSliderItem(farmMenu, "LCminions"))
                    W.Cast(farmPosition.Position);
            }
        }

        private static void LogicE()
        {
            if (getCheckBoxItem(eMenu, "autoE"))
            {
                var trapPos = OktwCommon.GetTrapPos(E.Range);
                if (!trapPos.IsZero)
                    E.Cast(trapPos);

                foreach (var enemy in Program.Enemies.Where(enemy => enemy.LSIsValidTarget(E.Range) && !OktwCommon.CanMove(enemy)))
                    E.Cast(enemy.ServerPosition);
            }

            var t = TargetSelector.GetTarget(E.Range, DamageType.Physical);
            if (t.IsValidTarget() && getBoxItem(eMenu, "EmodeCombo") != 2)
            {
                if (Program.Combo && !Player.Spellbook.IsAutoAttacking)
                {
                    if (getBoxItem(eMenu, "EmodeCombo") == 1)
                    {
                        if (E.GetPrediction(t).CastPosition.LSDistance(t.Position) > 100)
                        {
                            if (Player.Position.LSDistance(t.ServerPosition) > Player.Position.LSDistance(t.Position))
                            {
                                if (t.Position.LSDistance(Player.ServerPosition) < t.Position.LSDistance(Player.Position))
                                    Program.CastSpell(E, t);
                            }
                            else
                            {
                                if (t.Position.LSDistance(Player.ServerPosition) > t.Position.LSDistance(Player.Position))
                                    Program.CastSpell(E, t);
                            }
                        }
                    }
                    else
                    {
                        Program.CastSpell(E, t);
                    }
                }

                E.CastIfWillHit(t, getSliderItem(eMenu, "Eaoe"));
            }
            else if (Program.LaneClear && Player.ManaPercent > getSliderItem(farmMenu, "Mana") && getCheckBoxItem(farmMenu, "farmE"))
            {
                var minionList = Cache.GetMinions(Player.ServerPosition, E.Range);
                var farmPosition = E.GetCircularFarmLocation(minionList, E.Width);

                if (farmPosition.MinionsHit >= getSliderItem(farmMenu, "LCminions"))
                    E.Cast(farmPosition.Position);
            }
        }

        private static void LogicQ()
        {
            var torb = Orbwalker.LastTarget;

            if (torb == null || torb.Type != GameObjectType.AIHeroClient)
            {
                if (getCheckBoxItem(qMenu, "Qminion"))
                {
                    var t = TargetSelector.GetTarget(Q.Range + 300, DamageType.Physical);
                    if (t.LSIsValidTarget())
                    {

                        var minion = Cache.GetMinions(LeagueSharp.Common.Prediction.GetPrediction(t, 0.1f).CastPosition, 300).Where(minion2 => minion2.LSIsValidTarget(Q.Range)).OrderBy(x => x.LSDistance(t)).FirstOrDefault();
                        if (minion.LSIsValidTarget())
                        {
                            if (t.Health < GetQdmg(t))
                                Q.CastOnUnit(minion);
                            if (Program.Combo && Player.Mana > RMANA + EMANA)
                                Q.CastOnUnit(minion);
                            else if (Program.Farm && getCheckBoxItem(qMenu, "harrasQ") && Player.Mana > RMANA + EMANA + WMANA + EMANA && getCheckBoxItem(harassMenu, "harras" + t.ChampionName))
                                Q.CastOnUnit(minion);
                        }
                    }
                }

            }
            else if (!SebbyLib.Orbwalking.CanAttack() && !Player.Spellbook.IsAutoAttacking)
            {
                var t = torb as AIHeroClient;
                if (t.Health < GetQdmg(t) + GetWdmg(t))
                    Q.CastOnUnit(t);
                if (Program.Combo && Player.Mana > RMANA + QMANA)
                    Q.CastOnUnit(t);
                else if (Program.Farm && getCheckBoxItem(qMenu, "harrasQ") && Player.Mana > RMANA + QMANA + WMANA + EMANA && getCheckBoxItem(harassMenu, "harras" + t.ChampionName))
                    Q.CastOnUnit(t);
            }
            if (Program.LaneClear && Player.ManaPercent > getSliderItem(farmMenu, "Mana") && getCheckBoxItem(farmMenu, "farmQ"))
            {
                var minionList = Cache.GetMinions(Player.ServerPosition, Q.Range);

                if (minionList.Count >= getSliderItem(farmMenu, "LCminions"))
                {
                    var minionAttack = minionList.FirstOrDefault(x => Q.GetDamage(x) > SebbyLib.HealthPrediction.GetHealthPrediction(x, 300));
                    if (minionAttack.LSIsValidTarget())
                        Q.CastOnUnit(minionAttack);
                }

            }
        }


        private static bool InCone(Vector3 Position)
        {
            var range = R.Range;
            var angle = 70f * (float)Math.PI / 180;
            var end2 = rPosCast.LSTo2D() - Player.Position.LSTo2D();
            var edge1 = end2.LSRotated(-angle / 2);
            var edge2 = edge1.LSRotated(angle);

            var point = Position.LSTo2D() - Player.Position.LSTo2D();
            if (point.LSDistance(new Vector2(), true) < range * range && edge1.LSCrossProduct(point) > 0 && point.LSCrossProduct(edge2) > 0)
                return true;

            return false;
        }

        private static void Jungle()
        {
            if (Program.LaneClear)
            {
                var mobs = Cache.GetMinions(Player.ServerPosition, Q.Range, MinionTeam.Neutral);
                if (mobs.Count > 0)
                {
                    var mob = mobs[0];

                    if (W.IsReady() && getCheckBoxItem(farmMenu, "jungleW"))
                    {
                        W.Cast(mob.ServerPosition);
                        return;
                    }
                    if (E.IsReady() && getCheckBoxItem(farmMenu, "jungleE"))
                    {
                        E.Cast(mob.ServerPosition);
                        return;
                    }
                    if (Q.IsReady() && getCheckBoxItem(farmMenu, "jungleQ"))
                    {
                        Q.CastOnUnit(mob);
                        return;
                    }
                }
            }
        }

        private static bool IsCastingR { get { return R.Instance.Name == "JhinRShot"; } }

        private static double GetRdmg(Obj_AI_Base target)
        {
            var damage = (-25 + 75 * R.Level + 0.2 * Player.FlatPhysicalDamageMod) * (1 + (100 - target.HealthPercent) * 0.02);

            return Player.CalcDamage(target, DamageType.Physical, damage);
        }

        private static double GetWdmg(Obj_AI_Base target)
        {
            var damage = 55 + W.Level * 35 + 0.7 * Player.FlatPhysicalDamageMod;

            return Player.CalcDamage(target, DamageType.Physical, damage);
        }

        private static double GetQdmg(Obj_AI_Base target)
        {
            var damage = 35 + Q.Level * 25 + 0.4 * Player.FlatPhysicalDamageMod;

            return Player.CalcDamage(target, DamageType.Physical, damage);
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
                RMANA = WMANA - Player.PARRegenRate * W.Instance.Cooldown;
            else
                RMANA = R.Instance.SData.Mana;
        }

        public static void drawLine(Vector3 pos1, Vector3 pos2, int bold, System.Drawing.Color color)
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
