using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using Microsoft.Win32;
using SharpDX;
using Color = System.Drawing.Color;
using EloBuddy.SDK.Menu;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu.Values;

namespace e.Motion_Katarina
{
    class Program
    {

        #region Declaration

        private static bool ShallJumpNow;
        private static Vector3 JumpPosition;
        private static LeagueSharp.Common.Spell Q, W, E, R;
        private static Menu _menu;
        private static AIHeroClient Player { get { return ObjectManager.Player; } }
        private static AIHeroClient qTarget;
        private static Obj_AI_Base qMinion;
        private static readonly AIHeroClient[] AllEnemy = HeroManager.Enemies.ToArray();
        private static bool WardJumpReady;
        private static SpellSlot IgniteSpellSlot = SpellSlot.Unknown;
        private static readonly List<int> AllEnemyTurret = new List<int>();
        private static readonly List<int> AllAllyTurret = new List<int>();
        private static Dictionary<int, bool> TurretHasAggro = new Dictionary<int, bool>();
        private static int lastLeeQTick;
        private static int tickValue;

        #endregion

        static bool IsTurretPosition(Vector3 pos)
        {
            float mindistance = 2000;
            foreach (int NetID in AllEnemyTurret)
            {
                Obj_AI_Turret turret = ObjectManager.GetUnitByNetworkId<Obj_AI_Turret>((uint)NetID);
                if (turret != null && !turret.IsDead && !TurretHasAggro[NetID])
                {
                    float distance = pos.LSDistance(turret.Position);
                    if (mindistance >= distance)
                    {
                        mindistance = distance;

                    }

                }
            }
            return mindistance <= 950;
        }

        public static Menu comboMenu, harassMenu, laneclear, jungleclear, lasthit, ksMenu, drawingsMenu, miscMenu, performanceMenu, devMenu;

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

        public static void Game_OnGameLoad()
        {
            //Wird aufgerufen, wenn LeagueSharp Injected
            if (Player.ChampionName != "Katarina")
            {
                return;
            }
            #region Spells
            Q = new LeagueSharp.Common.Spell(SpellSlot.Q, 675, DamageType.Magical);
            W = new LeagueSharp.Common.Spell(SpellSlot.W, 375, DamageType.Magical);
            E = new LeagueSharp.Common.Spell(SpellSlot.E, 700, DamageType.Magical);
            R = new LeagueSharp.Common.Spell(SpellSlot.R, 550, DamageType.Magical);
            //Get Ignite
            if (Player.Spellbook.GetSpell(SpellSlot.Summoner1).Name.Contains("summonerdot"))
            {
                IgniteSpellSlot = SpellSlot.Summoner1;
            }
            if (Player.Spellbook.GetSpell(SpellSlot.Summoner2).Name.Contains("summonerdot"))
            {
                IgniteSpellSlot = SpellSlot.Summoner2;
            }
            #endregion

            foreach (Obj_AI_Turret turret in ObjectManager.Get<Obj_AI_Turret>())
            {
                if (turret.IsEnemy)
                {
                    AllEnemyTurret.Add(turret.NetworkId);
                    TurretHasAggro[turret.NetworkId] = false;
                }
                if (turret.IsAlly)
                {
                    AllAllyTurret.Add(turret.NetworkId);
                    TurretHasAggro[turret.NetworkId] = false;
                }
            }

            LeagueSharp.Common.Utility.HpBarDamageIndicator.Enabled = true;
            LeagueSharp.Common.Utility.HpBarDamageIndicator.DamageToUnit = CalculateDamage;


            #region Menu
            _menu = MainMenu.AddMenu("e.Motion卡特", "motion.katarina");

            //Combo-Menü
            comboMenu = _menu.AddSubMenu("连招", "motion.katarina.combo");
            comboMenu.Add("motion.katarina.combo.useq", new CheckBox("使用 Q"));
            comboMenu.Add("motion.katarina.combo.usew", new CheckBox("使用 W"));
            comboMenu.Add("motion.katarina.combo.usee", new CheckBox("使用 E"));
            comboMenu.Add("motion.katarina.combo.user", new CheckBox("使用 R"));
            comboMenu.Add("motion.katarina.combo.mode", new ComboBox("连招模式", 0, "智能 [#推荐]", "快速 [#不推荐]"));
            comboMenu.Add("motion.katarina.combo.order", new ComboBox("循环顺序", 0, "Q -> E -> W -> R", "E -> Q -> W -> R", "动态"));

            //Harrass-Menü
            harassMenu = _menu.AddSubMenu("骚扰", "motion.katarina.harrass");
            harassMenu.Add("motion.katarina.harass.useq", new CheckBox("使用 Q"));
            harassMenu.Add("motion.katarina.harass.usew", new CheckBox("使用 W"));
            harassMenu.AddGroupLabel("Auto-Harass");
            harassMenu.Add("motion.katarina.harass.autoharass.toggle", new CheckBox("自动骚扰"));
            harassMenu.Add("motion.katarina.harass.autoharass.key", new KeyBind("骚扰开关按键", false, KeyBind.BindTypes.PressToggle, 'N'));
            harassMenu.Add("motion.katarina.harass.autoharass.useq", new CheckBox("使用 Q", false));
            harassMenu.Add("motion.katarina.harass.autoharass.usew", new CheckBox("使用 W"));

            //Laneclear-Menü
            laneclear = _menu.AddSubMenu("清线", "motion.katarina.laneclear");
            laneclear.Add("motion.katarina.laneclear.useq", new CheckBox("使用 Q"));
            laneclear.Add("motion.katarina.laneclear.usew", new CheckBox("使用 W"));
            laneclear.Add("motion.katarina.laneclear.minw", new Slider("最低小兵数量使用 W", 3, 1, 6));
            laneclear.Add("motion.katarina.laneclear.minwlasthit", new Slider("最低小兵数量尾兵使用 W", 2, 0, 6));

            //Jungleclear-Menü
            jungleclear = _menu.AddSubMenu("清野", "motion.katarina.jungleclear");
            jungleclear.Add("motion.katarina.jungleclear.useq", new CheckBox("使用 Q"));
            jungleclear.Add("motion.katarina.jungleclear.usew", new CheckBox("使用 W"));
            jungleclear.Add("motion.katarina.jungleclear.usee", new CheckBox("使用E"));

            //Lasthit-Menü
            lasthit = _menu.AddSubMenu("尾兵", "motion.katarina.lasthit");
            lasthit.Add("motion.katarina.lasthit.useq", new CheckBox("使用Q"));
            lasthit.Add("motion.katarina.lasthit.usew", new CheckBox("使用W"));
            lasthit.Add("motion.katarina.lasthit.usee", new CheckBox("使用E", false));
            lasthit.Add("motion.katarina.lasthit.noenemiese", new CheckBox("只使用 E 当附近无敌人时"));

            //KS-Menü
            ksMenu = _menu.AddSubMenu("抢头", "motion.katarina.killsteal");
            ksMenu.Add("motion.katarina.killsteal.useq", new CheckBox("使用Q"));
            ksMenu.Add("motion.katarina.killsteal.usew", new CheckBox("使用W"));
            ksMenu.Add("motion.katarina.killsteal.usee", new CheckBox("使用E"));
            ksMenu.Add("motion.katarina.killsteal.usef", new CheckBox("使用 点燃"));
            ksMenu.Add("motion.katarina.killsteal.wardjump", new CheckBox("跳眼抢头"));

            //Drawings-Menü
            drawingsMenu = _menu.AddSubMenu("线圈", "motion.katarina.drawings");
            drawingsMenu.Add("motion.katarina.drawings.drawq", new CheckBox("显示 Q", false));
            drawingsMenu.Add("motion.katarina.drawings.draww", new CheckBox("显示 W", false));
            drawingsMenu.Add("motion.katarina.drawings.drawe", new CheckBox("显示 E", false));
            drawingsMenu.Add("motion.katarina.drawings.drawr", new CheckBox("显示 R", false));
            drawingsMenu.Add("motion.katarina.drawings.dmg", new CheckBox("显示对目标的伤害"));
            drawingsMenu.Add("motion.katarina.drawings.drawalways", new CheckBox("一直显示", false));

            //Misc-Menü
            miscMenu = _menu.AddSubMenu("杂项", "motion.katarina.misc");
            miscMenu.Add("motion.katarina.misc.wardjump", new CheckBox("使用 跳眼"));
            miscMenu.Add("motion.katarina.misc.wardjumpkey", new KeyBind("跳眼按键", false, KeyBind.BindTypes.HoldActive, 'Z'));
            miscMenu.Add("motion.katarina.misc.noRCancel", new CheckBox("防大招中断"));
            miscMenu.Add("motion.katarina.misc.cancelR", new CheckBox("当附近无敌人停止 R", false));
            miscMenu.Add("motion.katarina.misc.kswhileult", new CheckBox("大招时进行抢头"));
            miscMenu.Add("motion.katarina.misc.allyTurret", new CheckBox("跳至友军塔下防突进"));

            performanceMenu = _menu.AddSubMenu("性能", "motion.katarina.performance");
            performanceMenu.Add("motion.katarina.performance.track", new Slider("探测可被尾兵的小兵数量", 3, 1, 10));
            performanceMenu.Add("motion.katarina.performance.tickmanager", new CheckBox("开启 探测次数管理", false));
            performanceMenu.Add("motion.katarina.performance.ticks", new Slider("增加探测次数", 8, 2, 50));

            //Dev-Menü
            devMenu = _menu.AddSubMenu("开发者", "motion.katarina.dev");
            devMenu.Add("motion.katarina.dev.enable", new CheckBox("开启开发者模式", false));
            devMenu.Add("motion.katarina.dev.targetdistance", new KeyBind("目标距离", false, KeyBind.BindTypes.HoldActive, 'L'));

            //alles zum Hauptmenü hinzufügen

            #endregion
            Chat.Print("<font color='#bb0000'>e</font>.<font color='#0000cc'>Motion</font> Katarina loaded");
            #region Subscriptions
            Game.OnUpdate += Game_OnUpdate;
            Drawing.OnDraw += OnDraw;
            Obj_AI_Base.OnProcessSpellCast += OnProcessSpellCast;
            EloBuddy.Player.OnIssueOrder += Obj_AI_Base_OnIssueOrder;
            Obj_AI_Turret.OnBasicAttack += Turret_OnTarget;
            Obj_AI_Base.OnBuffLose += BuffRemove;


            #endregion
        }



        private static void OnDraw(EventArgs args)
        {
            if (getCheckBoxItem(drawingsMenu, "motion.katarina.drawings.drawq") && (Q.IsReady() || getCheckBoxItem(drawingsMenu, "motion.katarina.drawings.drawalways")))
                Render.Circle.DrawCircle(Player.Position, Q.Range, Color.IndianRed);
            if (getCheckBoxItem(drawingsMenu, "motion.katarina.drawings.draww") && (W.IsReady() || getCheckBoxItem(drawingsMenu, "motion.katarina.drawings.drawalways")))
                Render.Circle.DrawCircle(Player.Position, W.Range, Color.IndianRed);
            if (getCheckBoxItem(drawingsMenu, "motion.katarina.drawings.drawe") && (E.IsReady() || getCheckBoxItem(drawingsMenu, "motion.katarina.drawings.drawalways")))
                Render.Circle.DrawCircle(Player.Position, E.Range, Color.IndianRed);
            if (getCheckBoxItem(drawingsMenu, "motion.katarina.drawings.drawr") && (R.IsReady() || getCheckBoxItem(drawingsMenu, "motion.katarina.drawings.drawalways")))
                Render.Circle.DrawCircle(Player.Position, R.Range, Color.IndianRed);
        }


        private static void BuffRemove(Obj_AI_Base sender, Obj_AI_BaseBuffLoseEventArgs args)
        {
            if (sender.IsMe && args.Buff.Name == "BlindMonkQOne")
            {
                lastLeeQTick = Utils.TickCount;
            }
        }


        static void Game_OnUpdate(EventArgs args)
        {
            Demark();
            LeagueSharp.Common.Utility.HpBarDamageIndicator.Enabled = getCheckBoxItem(drawingsMenu, "motion.katarina.drawings.dmg");
            if (Player.IsDead || Player.IsRecalling())
            {
                return;
            }
            if (HasRBuff())
            {
                Orbwalker.DisableAttacking = true;
                Orbwalker.DisableMovement = true;
                if (getCheckBoxItem(miscMenu, "motion.katarina.misc.cancelR") && Player.GetEnemiesInRange(R.Range + 50).Count == 0)
                    EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
                if (getCheckBoxItem(miscMenu, "motion.katarina.misc.kswhileult"))
                    Killsteal();
                return;
            }
            if (ShallJumpNow)
            {
                WardJump(JumpPosition, false, false);
                if (!E.IsReady())
                {
                    ShallJumpNow = false;
                }
            }
            Orbwalker.DisableAttacking = false;
            Orbwalker.DisableMovement = false;

            //Dev();
            Killsteal();

            //Combo
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
                Combo(Q.IsReady() && getCheckBoxItem(comboMenu, "motion.katarina.combo.useq"), W.IsReady() && getCheckBoxItem(comboMenu, "motion.katarina.combo.usew"), E.IsReady() && getCheckBoxItem(comboMenu, "motion.katarina.combo.usee"), R.IsReady() && getCheckBoxItem(comboMenu, "motion.katarina.combo.user"));

            //Harass
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass))
                Combo(Q.IsReady() && getCheckBoxItem(harassMenu, "motion.katarina.harass.useq"), W.IsReady() && getCheckBoxItem(harassMenu, "motion.katarina.harass.usew"), false, false, true);

            //Autoharass
            if (getCheckBoxItem(harassMenu, "motion.katarina.harass.autoharass.toggle") && getKeyBindItem(harassMenu, "motion.katarina.harass.autoharass.key"))
                Combo(Q.IsReady() && getCheckBoxItem(harassMenu, "motion.katarina.harass.autoharass.useq"), W.IsReady() && getCheckBoxItem(harassMenu, "motion.katarina.harass.autoharass.usew"), false, false, true);

            Lasthit();
            LaneClear();
            JungleClear();

            if (getKeyBindItem(miscMenu, "motion.katarina.misc.wardjumpkey") && getCheckBoxItem(miscMenu, "motion.katarina.misc.wardjump"))
            {
                WardJump(Game.CursorPos);
            }
        }

        private static void Dev()
        {
            if (getCheckBoxItem(devMenu, "motion.katarina.dev.enable") && getKeyBindItem(devMenu, "motion.katarina.dev.targetdistance"))
            {
                AIHeroClient target = TargetSelector.GetTarget(1000, DamageType.Magical);
                if (target != null)
                {
                    Chat.Print("Distance to Target:" + Player.LSDistance(target));
                }
            }
        }


        static bool HasRBuff()
        {
            return Player.HasBuff("KatarinaR") || Player.IsChannelingImportantSpell() || Player.HasBuff("katarinarsound");

        }

        static void Combo(bool useq, bool usew, bool usee, bool user, bool anyTarget = false)
        {
            bool startWithQ = getBoxItem(comboMenu, "motion.katarina.combo.order") == 0 && useq;
            bool dynamic = getBoxItem(comboMenu, "motion.katarina.combo.order") == 2;
            bool smartcombo = getBoxItem(comboMenu, "motion.katarina.combo.mode") == 0;
            AIHeroClient target = TargetSelector.GetTarget(!startWithQ || dynamic ? E.Range : Q.Range, DamageType.Magical);
            if (target != null && !target.IsZombie)
            {
                if (useq && (startWithQ || !usee || dynamic) && target.LSDistance(Player) < Q.Range)
                {
                    Q.Cast(target);
                    qTarget = target;
                    return;
                }
                if (usee && (usew || user || qTarget != target || !smartcombo))
                {
                    E.Cast(target);
                    return;
                }
                if (anyTarget)
                {
                    List<AIHeroClient> enemies = Player.Position.GetEnemiesInRange(390);
                    if (enemies.Count >= 2)
                    {
                        W.Cast();
                        return;
                    }
                    if (enemies.Count == 1)
                    {
                        target = enemies.ElementAt(0);
                    }
                }
                if (target.LSDistance(Player) < 390 && usew && (user || qTarget != target || !smartcombo))
                {
                    W.Cast();
                    return;
                }
                if (target.LSDistance(Player) < R.Range - 200 && user)
                {
                    R.Cast();
                }
            }
        }

        private static void Turret_OnTarget(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.GetType() == typeof(Obj_AI_Turret))
            {
                TurretHasAggro[sender.NetworkId] = !(args.Target == null || args.Target is Obj_AI_Minion);
            }
        }

        public static void OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (args.SData.Name == "KatarinaQ" && args.Target.GetType() == typeof(AIHeroClient))
            {
                qTarget = (AIHeroClient)args.Target;
            }
            if (args.SData.Name == "katarinaE")
            {
                WardJumpReady = false;
            }
            if (sender.IsMe && WardJumpReady)
            {
                E.Cast((Obj_AI_Base)args.Target);
                WardJumpReady = false;
            }
            //Todo Check for Lee Q
            if (args.SData.Name == "blindmonkqtwo")
            {

                if (lastLeeQTick - Utils.TickCount <= 10)
                {
                    //Game.PrintChat("Trying to Jump undeder Ally Turret - OnProcessSpellCast");
                    JumpUnderTurret(-100, sender.Position);
                }
                lastLeeQTick = Utils.TickCount;
            }
            // Todo Test
            if (args.Target != null && args.Target.IsMe && getCheckBoxItem(miscMenu, "motion.katarina.misc.allyTurret"))
            {
                switch (args.SData.Name)
                {
                    case "ZedR":
                        JumpUnderTurret(-100, sender.Position);
                        break;
                    case "ViR":
                        JumpUnderTurret(100, sender.Position);
                        break;
                    case "NocturneParanoia":
                        JumpUnderTurret(100, sender.Position);
                        break;
                    case "MaokaiUnstableGrowth":
                        JumpUnderTurret(0, sender.Position);
                        break;
                }

            }

        }



        private static void JumpUnderTurret(float extrarange, Vector3 objectPosition)
        {
            float mindistance = 100000;
            //Getting next Turret
            Obj_AI_Turret turretToJump = null;

            foreach (int NetID in AllAllyTurret)
            {
                Obj_AI_Turret turret = ObjectManager.GetUnitByNetworkId<Obj_AI_Turret>((uint)NetID);
                if (turret != null && !turret.IsDead)
                {
                    float distance = Player.Position.LSDistance(turret.Position);
                    if (mindistance >= distance)
                    {
                        mindistance = distance;
                        turretToJump = turret;
                    }

                }
            }
            if (turretToJump != null && !TurretHasAggro[turretToJump.NetworkId] && Player.Position.LSDistance(turretToJump.Position) < 1500)
            {
                int i = 0;

                do
                {
                    Vector3 extPos = Player.Position.LSExtend(turretToJump.Position, 685 - i);
                    float dist = objectPosition.LSDistance(extPos + extrarange);
                    Vector3 predictedPosition = objectPosition.LSExtend(extPos, dist);
                    if (predictedPosition.LSDistance(turretToJump.Position) <= 890 && !predictedPosition.IsWall())
                    {
                        WardJump(Player.Position.LSExtend(turretToJump.Position, 650 - i), false);
                        JumpPosition = Player.Position.LSExtend(turretToJump.Position, 650 - i);
                        ShallJumpNow = true;
                        break;
                    }

                    i += 50;
                } while (i <= 300 || !Player.Position.Extend(turretToJump.Position, 650 - i).IsWall());
            }

        }


        static void Demark()
        {
            if ((qTarget != null && qTarget.HasBuff("katarinaqmark")) || Q.Cooldown < 3)
            {
                qTarget = null;
            }
        }


        #region WardJumping
        private static void WardJump(Vector3 where, bool move = true, bool placeward = true)
        {
            if (move)
                Orbwalker.MoveTo(Game.CursorPos);
            if (!E.IsReady())
            {
                return;
            }
            Vector3 wardJumpPosition = Player.Position.LSDistance(where) < 600 ? where : Player.Position.LSExtend(where, 600);
            var lstGameObjects = ObjectManager.Get<Obj_AI_Base>().ToArray();
            Obj_AI_Base entityToWardJump = lstGameObjects.FirstOrDefault(obj =>
                obj.Position.LSDistance(wardJumpPosition) < 150
                && (obj is Obj_AI_Minion || obj is AIHeroClient)
                && !obj.IsMe && !obj.IsDead
                && obj.Position.LSDistance(Player.Position) < E.Range);

            if (entityToWardJump != null)
            {
                E.Cast(entityToWardJump);
            }
            else if (placeward)
            {
                int wardId = GetWardItem();
                if (wardId != -1 && !wardJumpPosition.IsWall())
                {
                    WardJumpReady = true;
                    PutWard(wardJumpPosition.To2D(), (ItemId)wardId);
                }
            }

        }

        public static int GetWardItem()
        {
            int[] wardItems = { 3340, 3350, 3205, 3207, 2049, 2045, 2044, 3361, 3154, 3362, 3160, 2043 };
            foreach (var id in wardItems.Where(id => Items.HasItem(id) && Items.CanUseItem(id)))
                return id;
            return -1;
        }

        public static void PutWard(Vector2 pos, ItemId warditem)
        {

            foreach (var slot in Player.InventoryItems.Where(slot => slot.Id == warditem))
            {
                ObjectManager.Player.Spellbook.CastSpell(slot.SpellSlot, pos.To3D());
                return;
            }
        }
        #endregion
        //Calculating Damage
        static float CalculateDamage(AIHeroClient target)
        {
            double damage = 0d;
            if (Q.IsReady())
            {
                damage += Player.GetSpellDamage(target, SpellSlot.Q) + Player.LSGetSpellDamage(target, SpellSlot.Q, 1);
            }
            if (target.HasBuff("katarinaqmark") || target == qTarget)
            {
                damage += Player.LSGetSpellDamage(target, SpellSlot.Q, 1);
            }
            if (W.IsReady())
            {
                damage += Player.GetSpellDamage(target, SpellSlot.W);
            }
            if (E.IsReady())
            {
                damage += Player.GetSpellDamage(target, SpellSlot.E);
            }
            if (R.IsReady() || (Player.GetSpell(R.Slot).State == SpellState.Surpressed && R.Level > 0))
            {
                damage += Player.GetSpellDamage(target, SpellSlot.R);
            }
            if (Player.GetSummonerSpellDamage(target, LeagueSharp.Common.Damage.SummonerSpell.Ignite) > 0 && IgniteSpellSlot != SpellSlot.Unknown && IgniteSpellSlot.IsReady())
            {
                damage += Player.GetSummonerSpellDamage(target, LeagueSharp.Common.Damage.SummonerSpell.Ignite);
                damage -= target.HPRegenRate * 2.5;
            }
            return (float)damage;
        }

        #region Killsteal
        static int CanKill(AIHeroClient target, bool useq, bool usew, bool usee, bool usef)
        {
            double damage = 0;
            if (!useq && !usew && !usee && !usef)
                return 0;
            if (Q.IsReady() && useq)
            {
                damage += ObjectManager.Player.GetSpellDamage(target, SpellSlot.Q);
                if ((W.IsReady() && usew) || (E.IsReady() && usee))
                {
                    damage += ObjectManager.Player.LSGetSpellDamage(target, SpellSlot.Q, 1);
                }
            }
            if (target.HasBuff("katarinaqmark") || target == qTarget)
            {
                damage += ObjectManager.Player.LSGetSpellDamage(target, SpellSlot.Q, 1);
            }
            if (W.IsReady() && usew)
            {
                damage += ObjectManager.Player.GetSpellDamage(target, SpellSlot.W);
            }
            if (E.IsReady() && usee)
            {
                damage += ObjectManager.Player.GetSpellDamage(target, SpellSlot.E);
            }
            if (damage >= target.Health)
            {
                return 1;
            }
            if (Player.GetSummonerSpellDamage(target, LeagueSharp.Common.Damage.SummonerSpell.Ignite) > 0 && !target.HasBuff("summonerdot") && !HasRBuff() && IgniteSpellSlot != SpellSlot.Unknown && IgniteSpellSlot.IsReady())
            {
                damage += Player.GetSummonerSpellDamage(target, LeagueSharp.Common.Damage.SummonerSpell.Ignite);
                damage -= target.HPRegenRate * 2.5;
            }
            return damage >= target.Health ? 2 : 0;

        }

        private static void Killsteal()
        {
            foreach (AIHeroClient enemy in AllEnemy)
            {
                if (enemy == null || enemy.HasBuffOfType(BuffType.Invulnerability))
                    return;

                if (CanKill(enemy, false, getCheckBoxItem(ksMenu, "motion.katarina.killsteal.usew"), false, false) == 1 && enemy.IsValidTarget(390))
                {
                    W.Cast(enemy);
                    return;
                }
                if (CanKill(enemy, false, false, getCheckBoxItem(ksMenu, "motion.katarina.killsteal.usee"), false) == 1 && enemy.IsValidTarget(700))
                {
                    E.Cast(enemy);
                    return;
                }
                if (CanKill(enemy, getCheckBoxItem(ksMenu, "motion.katarina.killsteal.useq"), false, false, false) == 1 && enemy.IsValidTarget(675))
                {
                    Q.Cast(enemy);
                    qTarget = enemy;
                    return;
                }
                int cankill = CanKill(enemy, getCheckBoxItem(ksMenu, "motion.katarina.killsteal.useq"), getCheckBoxItem(ksMenu, "motion.katarina.killsteal.usew"), getCheckBoxItem(ksMenu, "motion.katarina.killsteal.usee"), getCheckBoxItem(ksMenu, "motion.katarina.killsteal.usef"));
                if ((cankill == 1 || cankill == 2) && enemy.IsValidTarget(Q.Range))
                {
                    if (cankill == 2 && enemy.IsValidTarget(600))
                        Player.Spellbook.CastSpell(IgniteSpellSlot, enemy);
                    if (Q.IsReady())
                        Q.Cast(enemy);
                    if (E.IsReady() && (W.IsReady() || qTarget != enemy))
                        E.Cast(enemy);
                    if (W.IsReady() && enemy.IsValidTarget(390) && qTarget != enemy)
                        W.Cast();
                    return;
                }
                //KS with Wardjump
                cankill = CanKill(enemy, true, false, false, getCheckBoxItem(ksMenu, "motion.katarina.killsteal.usef"));
                if (getCheckBoxItem(ksMenu, "motion.katarina.killsteal.wardjump") && (cankill == 1 || cankill == 2) && enemy.IsValidTarget(1300) && Q.IsReady() && E.IsReady())
                {
                    WardJump(enemy.Position, false);
                    if (cankill == 2 && enemy.IsValidTarget(600))
                        Player.Spellbook.CastSpell(IgniteSpellSlot, enemy);
                    if (enemy.IsValidTarget(675))
                        Q.Cast(enemy);
                    return;
                }
            }
        }
        #endregion



        #region Lasthit

        private static void Lasthit()
        {

            if (!Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LastHit) || (getCheckBoxItem(performanceMenu, "motion.katarina.performance.tickmanager") && Utils.TickCount < tickValue))
                return;
            Obj_AI_Base[] sourroundingMinions;
            int tickCount = getSliderItem(performanceMenu, "motion.katarina.performance.ticks");
            tickValue = Utils.TickCount + tickCount;
            if (getCheckBoxItem(lasthit, "motion.katarina.lasthit.usew") && W.IsReady())
            {
                sourroundingMinions = MinionManager.GetMinions(Player.Position, 390).Take(3).ToArray();
                {
                    //Only Cast W when minion is not killable with Autoattacks
                    if (
                        sourroundingMinions.Any(
                            minion =>
                                !minion.IsDead && Orbwalker.LastTarget != minion && (qMinion == null || minion != qMinion) &&
                                W.GetDamage(minion) > minion.Health &&
                                HealthPrediction.GetHealthPrediction(minion,
                                    (Player.CanAttack
                                        ? Game.Ping / 2
                                        : Orbwalker.LastAutoAttack - Utils.GameTimeTickCount +
                                          (int)Player.AttackDelay * 1000) + 200 + (getCheckBoxItem(performanceMenu, "motion.katarina.performance.tickmanager") ? tickCount - 1 : 0) + (int)Player.AttackCastDelay * 1000) <= 0))
                    {
                        W.Cast();
                    }
                }

            }
            if (getCheckBoxItem(lasthit, "motion.katarina.lasthit.useq") && Q.IsReady())
            {
                sourroundingMinions = MinionManager.GetMinions(Player.Position, Q.Range).ToArray();
                foreach (var minion in sourroundingMinions.Where(minion => !minion.IsDead && Q.GetDamage(minion) > minion.Health))
                {
                    Q.Cast(minion);
                    qMinion = minion;
                    break;
                }
            }
            if (getCheckBoxItem(lasthit, "motion.katarina.lasthit.usee") && E.IsReady() && (!getCheckBoxItem(lasthit, "motion.katarina.lasthit.noenemiese") || Player.GetEnemiesInRange(1000).Count == 0))
            {
                //Same Logic with W + not killable with W
                sourroundingMinions = MinionManager.GetMinions(Player.Position, E.Range).Take(getSliderItem(performanceMenu, "motion.katarina.performance.track")).ToArray();
                {
                    foreach (var minions in sourroundingMinions.Where(
                        minion =>
                            !minion.IsDead && Orbwalker.LastTarget != minion && (qMinion == null || minion != qMinion) &&
                            E.GetDamage(minion) >= minion.Health &&
                            (!W.IsReady() || !getCheckBoxItem(lasthit, "motion.katarina.lasthit.usew") || Player.Position.LSDistance(minion.Position) > 390)
                            &&
                            HealthPrediction.GetHealthPrediction(minion,
                                (Player.CanAttack
                                    ? Game.Ping / 2
                                    : Orbwalker.LastAutoAttack - Utils.GameTimeTickCount + (int)Player.AttackDelay * 1000) +
                                200 + (getCheckBoxItem(performanceMenu, "motion.katarina.performance.tickmanager") ? tickCount - 1 : 0) + (int)Player.AttackCastDelay * 1000) <= 0
                            &&
                            !IsTurretPosition(Player.Position.LSExtend(minion.Position,
                                Player.Position.LSDistance(minion.Position) + 35))))
                    {
                        E.Cast(minions);
                        break;
                    }
                }
            }
        }
        #endregion

        #region LaneClear
        private static void LaneClear()
        {
            if (!Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear) || !Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear))
                return;
            Obj_AI_Base[] sourroundingMinions;
            if (getCheckBoxItem(laneclear, "motion.katarina.laneclear.usew") && W.IsReady())
            {
                sourroundingMinions = MinionManager.GetMinions(Player.Position, W.Range - 5).ToArray();
                if (sourroundingMinions.GetLength(0) >= getSliderItem(laneclear, "motion.katarina.laneclear.minw"))
                {
                    int lasthittable = sourroundingMinions.Count(minion => W.GetDamage(minion) + (minion.HasBuff("katarinaqmark") ? Q.GetDamage(minion, 1) : 0) > minion.Health);
                    if (lasthittable >= getSliderItem(laneclear, "motion.katarina.laneclear.minwlasthit"))
                    {
                        W.Cast();
                    }
                }
            }
            if (getCheckBoxItem(laneclear, "motion.katarina.laneclear.useq") && Q.IsReady())
            {
                sourroundingMinions = MinionManager.GetMinions(Player.Position, Q.Range - 5).ToArray();
                foreach (var minion in sourroundingMinions.Where(minion => !minion.IsDead))
                {
                    Q.Cast(minion);
                    break;
                }
            }
        }
        #endregion

        #region Jungleclear

        private static void JungleClear()
        {
            Obj_AI_Base[] sourroundingMinions;
            if (!Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear) || !Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear))
                return;
            if (getCheckBoxItem(jungleclear, "motion.katarina.jungleclear.useq") && Q.IsReady())
            {
                sourroundingMinions = MinionManager.GetMinions(Player.Position, Q.Range, MinionTypes.All, MinionTeam.Neutral).ToArray();
                float maxhealth = 0;
                int chosenminion = 0;
                if (sourroundingMinions.GetLength(0) >= 1)
                {
                    for (int i = 0; i < sourroundingMinions.Length; i++)
                    {
                        if (maxhealth < sourroundingMinions[i].MaxHealth)
                        {
                            maxhealth = sourroundingMinions[i].MaxHealth;
                            chosenminion = i;
                        }
                    }
                    Q.Cast(sourroundingMinions[chosenminion]);
                }
            }
            if (getCheckBoxItem(jungleclear, "motion.katarina.jungleclear.usew") && W.IsReady())
            {
                sourroundingMinions = MinionManager.GetMinions(Player.Position, W.Range - 5, MinionTypes.All, MinionTeam.Neutral).ToArray();
                if (sourroundingMinions.GetLength(0) >= 1)
                {
                    W.Cast();
                }
            }
            if (getCheckBoxItem(jungleclear, "motion.katarina.jungleclear.usee") && E.IsReady())
            {
                sourroundingMinions = MinionManager.GetMinions(Player.Position, E.Range, MinionTypes.All, MinionTeam.Neutral).ToArray();
                float maxhealth = 0;
                int chosenminion = 0;
                if (sourroundingMinions.GetLength(0) >= 1)
                {
                    for (int i = 0; i < sourroundingMinions.Length; i++)
                    {
                        if (maxhealth < sourroundingMinions[i].MaxHealth)
                        {
                            maxhealth = sourroundingMinions[i].MaxHealth;
                            chosenminion = i;
                        }
                    }
                    E.Cast(sourroundingMinions[chosenminion]);
                }
            }
        }
        #endregion
        private static void Obj_AI_Base_OnIssueOrder(Obj_AI_Base sender, PlayerIssueOrderEventArgs args)
        {
            if (sender.IsMe && HasRBuff() && getCheckBoxItem(miscMenu, "motion.katarina.misc.noRCancel"))
                args.Process = false;
        }
    }
}
