using LeagueSharp;
using LeagueSharp.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using SharpDX;
using Color = System.Drawing.Color;
using EloBuddy;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK;

namespace xSLx_Orbwalker
{
    // ReSharper disable once InconsistentNaming
    public class xSLxOrbwalker
    {

        /// <summary>
        ///     Spells that reset the attack timer.
        /// </summary>
        private static readonly string[] AttackResets =
        {
            "dariusnoxiantacticsonh", "fioraflurry", "garenq",
            "gravesmove", "hecarimrapidslash", "jaxempowertwo", "jaycehypercharge", "leonashieldofdaybreak", "luciane",
            "monkeykingdoubleattack", "mordekaisermaceofspades", "nasusq", "nautiluspiercinggaze", "netherblade",
            "gangplankqwrapper", "powerfist", "renektonpreexecute", "rengarq",
            "shyvanadoubleattack", "sivirw", "takedown", "talonnoxiandiplomacy", "trundletrollsmash", "vaynetumble",
            "vie", "volibearq", "xenzhaocombotarget", "yorickspectral", "reksaiq", "itemtitanichydracleave", "masochism",
            "illaoiw", "elisespiderw", "fiorae", "meditate", "sejuaninorthernwinds", "asheq"
        };


        /// <summary>
        ///     Spells that are not attacks even if they have the "attack" word in their name.
        /// </summary>
        private static readonly string[] NoAttacks =
        {
            "volleyattack", "volleyattackwithsound",
            "jarvanivcataclysmattack", "monkeykingdoubleattack", "shyvanadoubleattack", "shyvanadoubleattackdragon",
            "zyragraspingplantattack", "zyragraspingplantattack2", "zyragraspingplantattackfire",
            "zyragraspingplantattack2fire", "viktorpowertransfer", "sivirwattackbounce", "asheqattacknoonhit",
            "elisespiderlingbasicattack", "heimertyellowbasicattack", "heimertyellowbasicattack2",
            "heimertbluebasicattack", "annietibbersbasicattack", "annietibbersbasicattack2",
            "yorickdecayedghoulbasicattack", "yorickravenousghoulbasicattack", "yorickspectralghoulbasicattack",
            "malzaharvoidlingbasicattack", "malzaharvoidlingbasicattack2", "malzaharvoidlingbasicattack3",
            "kindredwolfbasicattack"
        };


        /// <summary>
        ///     Spells that are attacks even if they dont have the "attack" word in their name.
        /// </summary>
        private static readonly string[] Attacks =
        {
            "caitlynheadshotmissile", "frostarrow", "garenslash2",
            "kennenmegaproc", "masteryidoublestrike", "quinnwenhanced", "renektonexecute", "renektonsuperexecute",
            "rengarnewpassivebuffdash", "trundleq", "xenzhaothrust", "xenzhaothrust2", "xenzhaothrust3", "viktorqbuff", "lucianpassiveshot"
        };


        public static Menu Menu;
        public static AIHeroClient MyHero = ObjectManager.Player;
        public static Obj_AI_Base ForcedTarget = null;
        public static IEnumerable<AIHeroClient> AllEnemys = ObjectManager.Get<AIHeroClient>().Where(hero => hero.IsEnemy);
        public static IEnumerable<AIHeroClient> AllAllys = ObjectManager.Get<AIHeroClient>().Where(hero => hero.IsAlly);
        public static bool CustomOrbwalkMode;

        public delegate void BeforeAttackEvenH(BeforeAttackEventArgs args);
        public delegate void OnTargetChangeH(Obj_AI_Base oldTarget, Obj_AI_Base newTarget);
        public delegate void AfterAttackEvenH(Obj_AI_Base unit, Obj_AI_Base target);
        public delegate void OnAttackEvenH(Obj_AI_Base unit, Obj_AI_Base target);

        public static event BeforeAttackEvenH BeforeAttack;
        public static event OnTargetChangeH OnTargetChange;
        public static event AfterAttackEvenH AfterAttack;
        public static event OnAttackEvenH OnAttack;

        public enum Mode
        {
            Combo,
            Harass,
            LaneClear,
            LaneFreeze,
            Lasthit,
            Flee,
            None,
        }

        private static bool _drawing = true;
        private static bool _attack = true;
        private static bool _movement = true;
        private static bool _disableNextAttack;
        private const float LaneClearWaitTimeMod = 2f;
        private static int _lastAATick;
        private static Obj_AI_Base _lastTarget;
        private static LeagueSharp.Common.Spell _movementPrediction;
        private static int _lastMovement;
        private static int _windup;
        private static int lastRealAttack;

        public static Menu menuDrawing, menuMisc, menuMelee, modeCombo, modeHarass, modeLaneClear, modeLaneFreeze, modeLasthit, modeFlee;

        public static void AddToMenu()
        {
            _movementPrediction = new LeagueSharp.Common.Spell(SpellSlot.Unknown, GetAutoAttackRange());
            _movementPrediction.SetTargetted(MyHero.BasicAttack.SpellCastTime, MyHero.BasicAttack.MissileSpeed);

            Menu = MainMenu.AddMenu("走砍", "LSORABSD");

            menuDrawing = Menu.AddSubMenu("线圈", "orb_Draw");
            menuDrawing.Add("orb_Draw_AARange", new CheckBox("AA 范围"));//.SetValue(new Circle(true, Color.FloralWhite)));
            menuDrawing.Add("orb_Draw_AARange_Enemy", new CheckBox("敌人AA范围"));//.SetValue(new Circle(true, Color.Pink)));
            menuDrawing.Add("orb_Draw_Holdzone", new CheckBox("停止区域"));//.SetValue(new Circle(true, Color.FloralWhite)));
            menuDrawing.Add("orb_Draw_MinionHPBar", new CheckBox("小兵血条"));//.SetValue(new Circle(true, Color.Black)));
            menuDrawing.Add("orb_Draw_MinionHPBar_thickness", new Slider("^ 血条宽度", 1, 1, 3));
            menuDrawing.Add("orb_Draw_hitbox", new CheckBox("显示边框"));//.SetValue(new Circle(true, Color.FloralWhite)));
            menuDrawing.Add("orb_Draw_Lasthit", new CheckBox("小兵尾兵助手"));//.SetValue(new Circle(true, Color.Lime)));
            menuDrawing.Add("orb_Draw_nearKill", new CheckBox("接近死亡的小兵提示"));//.SetValue(new Circle(true, Color.Gold)));

            menuMisc = Menu.AddSubMenu("杂项", "orb_Misc");
            menuMisc.Add("orb_Misc_Holdzone", new Slider("停止区域范围", 50, 0, 100));
            menuMisc.Add("orb_Misc_Farmdelay", new Slider("农兵延迟", 0, 0, 200));
            menuMisc.Add("orb_Misc_ExtraWindUp", new Slider("额外前摇", 80, 0, 200));
            menuMisc.Add("orb_Misc_AutoWindUp", new CheckBox("自动设置前摇", false));
            menuMisc.Add("orb_Misc_Priority_Unit", new ComboBox("优先攻击", 0, "小兵", "英雄"));
            menuMisc.Add("orb_Misc_Humanizer", new Slider("Humanizer Delay", 80, 15, 200));
            menuMisc.Add("orb_Misc_AllMovementDisabled", new CheckBox("屏蔽所有动作", false));
            menuMisc.Add("orb_Misc_AllAttackDisabled", new CheckBox("屏蔽所有攻击", false));

            menuMelee = Menu.AddSubMenu("近程", "orb_Melee");
            menuMelee.Add("orb_Melee_Prediction", new CheckBox("移动预判", false));

            modeCombo = Menu.AddSubMenu("连招", "orb_Modes_Combo");
            modeCombo.Add("Combo_Key", new KeyBind("按键", false, KeyBind.BindTypes.HoldActive, 32));
            modeCombo.Add("Combo_move", new CheckBox("移动"));
            modeCombo.Add("Combo_attack", new CheckBox("攻击"));

            modeHarass = Menu.AddSubMenu("骚扰", "orb_Modes_Harass");
            modeHarass.Add("Harass_Key", new KeyBind("按键", false, KeyBind.BindTypes.HoldActive, 'C'));
            modeHarass.Add("Harass_move", new CheckBox("移动"));
            modeHarass.Add("Harass_attack", new CheckBox("攻击"));
            modeHarass.Add("Harass_Lasthit", new CheckBox("尾兵"));

            modeLaneClear = Menu.AddSubMenu("清线", "orb_Modes_LaneClear");
            modeLaneClear.Add("LaneClear_Key", new KeyBind("按键", false, KeyBind.BindTypes.HoldActive, 'V'));
            modeLaneClear.Add("LaneClear_move", new CheckBox("移动"));
            modeLaneClear.Add("LaneClear_attack", new CheckBox("攻击"));

            modeLaneFreeze = Menu.AddSubMenu("控线", "orb_Modes_LaneFreeze");
            modeLaneFreeze.Add("LaneFreeze_Key", new KeyBind("按键", false, KeyBind.BindTypes.HoldActive, 'Z'));
            modeLaneFreeze.Add("LaneFreeze_move", new CheckBox("移动"));
            modeLaneFreeze.Add("LaneFreeze_attack", new CheckBox("攻击"));

            modeLasthit = Menu.AddSubMenu("尾兵", "orb_Modes_LastHit");
            modeLasthit.Add("LastHit_Key", new KeyBind("按键", false, KeyBind.BindTypes.HoldActive, 'X'));
            modeLasthit.Add("LastHit_move", new CheckBox("移动"));
            modeLasthit.Add("LastHit_attack", new CheckBox("攻击"));

            modeFlee = Menu.AddSubMenu("逃跑", "orb_Modes_Flee");
            modeFlee.Add("Flee_Key", new KeyBind("按键", false, KeyBind.BindTypes.HoldActive, 'A'));

            m_baseAttackSpeed = 1f / (ObjectManager.Player.AttackDelay * ObjectManager.Player.GetAttackSpeed());
            m_baseWindUp = 1f / (ObjectManager.Player.AttackCastDelay * ObjectManager.Player.GetAttackSpeed());

            Drawing.OnDraw += OnDraw;
            Game.OnUpdate += OnUpdate;
            Obj_AI_Base.OnProcessSpellCast += OnProcessSpell;
            GameObject.OnCreate += Obj_SpellMissile_OnCreate;
        }

        private static float m_baseWindUp;

        private static void Obj_SpellMissile_OnCreate(GameObject sender, EventArgs args)
        {
            if (sender.IsMe)
            {
                var obj = (AIHeroClient)sender;
                if (obj.IsMelee())
                    return;
            }
            if (!(sender is MissileClient) || !sender.IsValid)
                return;
            var missile = (MissileClient)sender;
            if (missile.SpellCaster is AIHeroClient && missile.SpellCaster.IsValid && IsAutoAttack(missile.SData.Name))
            {
                FireAfterAttack(missile.SpellCaster, _lastTarget);
                if (sender.IsMe)
                    lastRealAttack = Environment.TickCount;
            }
        }

        private static void OnUpdate(EventArgs args)
        {
            Orbwalker.DisableAttacking = true;
            Orbwalker.DisableMovement = true;
            CheckAutoWindUp();
            if (CurrentMode == Mode.None || MenuGUI.IsChatOpen || CustomOrbwalkMode)
                return;
            var target = GetPossibleTarget();
            Orbwalk(Game.CursorPos, target);
        }

        private static void OnDraw(EventArgs args)
        {
            if (!_drawing)
                return;

            if (getCheckBoxItem(menuDrawing, "orb_Draw_AARange"))
            {
                LeagueSharp.Common.Utility.DrawCircle(MyHero.Position, GetAutoAttackRange(), Color.FloralWhite);
            }

            if (getCheckBoxItem(menuDrawing, "orb_Draw_AARange_Enemy") ||
                getCheckBoxItem(menuDrawing, "orb_Draw_hitbox"))
            {
                foreach (var enemy in AllEnemys.Where(enemy => enemy.LSIsValidTarget(1500)))
                {
                    if (getCheckBoxItem(menuDrawing, "orb_Draw_AARange_Enemy"))
                        LeagueSharp.Common.Utility.DrawCircle(enemy.Position, GetAutoAttackRange(enemy, MyHero), Color.Pink);
                    if (getCheckBoxItem(menuDrawing, "orb_Draw_hitbox"))
                        LeagueSharp.Common.Utility.DrawCircle(enemy.Position, enemy.BoundingRadius, Color.FloralWhite);
                }
            }

            if (getCheckBoxItem(menuDrawing, "orb_Draw_AARange_Enemy"))
            {
                foreach (var enemy in AllEnemys.Where(enemy => enemy.LSIsValidTarget(1500)))
                {
                    LeagueSharp.Common.Utility.DrawCircle(enemy.Position, GetAutoAttackRange(enemy, MyHero), Color.Pink);

                }
            }

            if (getCheckBoxItem(menuDrawing, "orb_Draw_Holdzone"))
            {
                LeagueSharp.Common.Utility.DrawCircle(MyHero.Position, getSliderItem(menuMisc, "orb_Misc_Holdzone"), Color.FloralWhite);
            }

            if (getCheckBoxItem(menuDrawing, "orb_Draw_MinionHPBar") ||
                getCheckBoxItem(menuDrawing, "orb_Draw_Lasthit") ||
                getCheckBoxItem(menuDrawing, "orb_Draw_nearKill"))
            {
                var minionList = MinionManager.GetMinions(MyHero.Position, GetAutoAttackRange() + 500, MinionTypes.All, MinionTeam.Enemy, MinionOrderTypes.MaxHealth);
                foreach (var minion in minionList.Where(minion => minion.LSIsValidTarget(GetAutoAttackRange() + 500)))
                {
                    var attackToKill = Math.Ceiling(minion.MaxHealth / MyHero.LSGetAutoAttackDamage(minion, true));
                    var hpBarPosition = minion.HPBarPosition;
                    var barWidth = minion.IsMelee() ? 75 : 80;
                    if (minion.HasBuff("turretshield"))
                        barWidth = 70;
                    var barDistance = (float)(barWidth / attackToKill);
                    if (getCheckBoxItem(menuDrawing, "orb_Draw_MinionHPBar"))
                    {
                        for (var i = 1; i < attackToKill; i++)
                        {
                            var startposition = hpBarPosition.X + 45 + barDistance * i;
                            Drawing.DrawLine(
                                new Vector2(startposition, hpBarPosition.Y + 18),
                                new Vector2(startposition, hpBarPosition.Y + 23),
                                getSliderItem(menuDrawing, "orb_Draw_MinionHPBar_thickness"),
                                Color.Black);
                        }
                    }
                    if (getCheckBoxItem(menuDrawing, "orb_Draw_Lasthit") &&
                        minion.Health <= MyHero.LSGetAutoAttackDamage(minion, true))
                        LeagueSharp.Common.Utility.DrawCircle(minion.Position, minion.BoundingRadius, Color.Lime);
                    else if (getCheckBoxItem(menuDrawing, "orb_Draw_nearKill") &&
                             minion.Health <= MyHero.LSGetAutoAttackDamage(minion, true) * 2)
                        LeagueSharp.Common.Utility.DrawCircle(minion.Position, minion.BoundingRadius, Color.Gold);
                }
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

        public static void Orbwalk(Vector3 goalPosition, Obj_AI_Base target)
        {
            if (target != null && (CanAttack() || HaveCancled()) && IsAllowedToAttack())
            {
                _disableNextAttack = false;
                FireBeforeAttack(target);
                if (!_disableNextAttack)
                {
                    if (CurrentMode != Mode.Harass || !target.IsMinion || getCheckBoxItem(modeHarass, "Harass_Lasthit"))
                    {
                        Player.IssueOrder(GameObjectOrder.AttackUnit, target);
                        _lastAATick = Environment.TickCount + Game.Ping / 2;
                    }
                }
            }
            if (!CanMove() || !IsAllowedToMove())
                return;
            if (MyHero.IsMelee() && target != null && target.LSDistance(MyHero) < GetAutoAttackRange(MyHero, target) &&
                getCheckBoxItem(menuMelee, "orb_Melee_Prediction") && target is AIHeroClient && Game.CursorPos.LSDistance(target.Position) < 300)
            {
                _movementPrediction.Delay = MyHero.BasicAttack.SpellCastTime;
                _movementPrediction.Speed = MyHero.BasicAttack.MissileSpeed;
                MoveTo(_movementPrediction.GetPrediction(target).UnitPosition);
            }
            else
                MoveTo(goalPosition);
        }


        private static void MoveTo(Vector3 position)
        {
            var delay = getSliderItem(menuMisc, "orb_Misc_Humanizer");
            if (Environment.TickCount - _lastMovement < delay)
                return;
            _lastMovement = Environment.TickCount;

            var holdAreaRadius = getSliderItem(menuMisc, "orb_Misc_Holdzone");
            if (MyHero.ServerPosition.LSDistance(position) < holdAreaRadius)
            {
                if (MyHero.Path.Count() > 1)
                    Player.IssueOrder(GameObjectOrder.Stop, MyHero.Position);
                return;
            }
            var point = MyHero.ServerPosition +
            300 * (position.LSTo2D() - MyHero.ServerPosition.LSTo2D()).LSNormalized().To3D();
            Player.IssueOrder(GameObjectOrder.MoveTo, point);
        }

        private static bool IsAllowedToMove()
        {
            if (!_movement)
                return false;
            if (getCheckBoxItem(menuMisc, "orb_Misc_AllMovementDisabled"))
                return false;
            if (CurrentMode == Mode.Combo && !getCheckBoxItem(modeCombo, "Combo_move"))
                return false;
            if (CurrentMode == Mode.Harass && !getCheckBoxItem(modeHarass, "Harass_move"))
                return false;
            if (CurrentMode == Mode.LaneClear && !getCheckBoxItem(modeLaneClear, "LaneClear_move"))
                return false;
            if (CurrentMode == Mode.LaneFreeze && !getCheckBoxItem(modeLaneFreeze, "LaneFreeze_move"))
                return false;
            return CurrentMode != Mode.Lasthit || getCheckBoxItem(modeLasthit, "LastHit_move");
        }

        private static bool IsAllowedToAttack()
        {
            if (!_attack)
                return false;
            if (getCheckBoxItem(menuMisc, "orb_Misc_AllAttackDisabled"))
                return false;
            if (CurrentMode == Mode.Combo && !getCheckBoxItem(modeCombo, "Combo_attack"))
                return false;
            if (CurrentMode == Mode.Harass && !getCheckBoxItem(modeHarass, "Harass_attack"))
                return false;
            if (CurrentMode == Mode.LaneClear && !getCheckBoxItem(modeLaneClear, "LaneClear_attack"))
                return false;
            if (CurrentMode == Mode.LaneFreeze && !getCheckBoxItem(modeLaneFreeze, "LaneFreeze_attack"))
                return false;
            return CurrentMode != Mode.Lasthit || getCheckBoxItem(modeLasthit, "LastHit_attack");

        }

        private static void OnProcessSpell(Obj_AI_Base unit, GameObjectProcessSpellCastEventArgs spell)
        {
            var spellName = spell.SData.Name;

            if (unit.IsMe && IsAutoAttackReset(spellName) && spell.SData.SpellCastTime == 0)
            {
                ResetAutoAttackTimer();
            }

            if (!IsAutoAttack(spellName))
            {
                return;
            }

            if (IsAutoAttackReset(spell.SData.Name) && unit.IsMe)
                LeagueSharp.Common.Utility.DelayAction.Add(100, ResetAutoAttackTimer);

            if (unit.IsMe)
            {
                _lastAATick = Environment.TickCount - Game.Ping / 2; // need test todo
                                                                     // ReSharper disable once CanBeReplacedWithTryCastAndCheckForNull
                if (spell.Target is Obj_AI_Base)
                {
                    FireOnTargetSwitch((Obj_AI_Base)spell.Target);
                    _lastTarget = (Obj_AI_Base)spell.Target;
                }

                if (unit.IsMelee())
                    LeagueSharp.Common.Utility.DelayAction.Add((int)(unit.AttackCastDelay * 1000 + Game.Ping * 0.5) + 50, () => FireAfterAttack(unit, _lastTarget));

                FireOnAttack(unit, _lastTarget);
            }
            else
            {
                FireOnAttack(unit, (Obj_AI_Base)spell.Target);
            }
        }

        public static double GetAzirAASandwarriorDamage(Obj_AI_Base unit)
        {
            var damagelist = new List<int> { 50, 55, 60, 65, 70, 75, 80, 85, 90, 95, 100, 110, 120, 130, 140, 150, 160, 170 };
            var dmg = damagelist[MyHero.Level - 1] + (MyHero.BaseAbilityDamage * 0.7);
            if (
                ObjectManager.Get<Obj_AI_Minion>()
                    .Count(
                        obj =>
                            obj.Name == "AzirSoldier" && obj.IsAlly && obj.BoundingRadius < 66 && obj.AttackSpeedMod > 1 &&
                            obj.LSDistance(unit) < 350) == 2)
                return MyHero.CalcDamage(unit, DamageType.Magical, dmg) +
                       (MyHero.CalcDamage(unit, DamageType.Magical, dmg) * 0.25);
            return MyHero.CalcDamage(unit, DamageType.Magical, dmg);
        }

        public static bool InSoldierAttackRange(Obj_AI_Base target)
        {
            return target != null && ObjectManager.Get<Obj_AI_Minion>().Any(obj => obj.Name == "AzirSoldier" && obj.IsAlly && obj.BoundingRadius < 66 && obj.AttackSpeedMod > 1 && obj.LSDistance(target) < 380);
        }

        public static Obj_AI_Base GetPossibleTarget()
        {
            if (ForcedTarget != null)
            {
                if (InAutoAttackRange(ForcedTarget))
                    return ForcedTarget;
                ForcedTarget = null;
            }


            Obj_AI_Base tempTarget = null;

            if (getBoxItem(menuMisc, "orb_Misc_Priority_Unit") == 1 &&
                (CurrentMode == Mode.Harass || CurrentMode == Mode.LaneClear))
            {
                tempTarget = GetBestHeroTarget();
                if (tempTarget != null)
                    return tempTarget;
            }

            if (CurrentMode == Mode.Harass || CurrentMode == Mode.Lasthit || CurrentMode == Mode.LaneClear || CurrentMode == Mode.LaneFreeze)
            {
                if (MyHero.ChampionName == "Azir")
                {
                    foreach (
                    var minion in
                        from minion in
                            ObjectManager.Get<Obj_AI_Minion>().Where(minion => minion.LSIsValidTarget() && minion.Name != "Beacon" && InSoldierAttackRange(minion))
                        let t = (int)(MyHero.AttackCastDelay * 1000) - 100 + Game.Ping / 2 +
                                1000 * (int)MyHero.LSDistance(minion) / (int)MyProjectileSpeed()
                        let predHealth = HealthPrediction.GetHealthPrediction(minion, t, FarmDelay(-125))
                        where minion.Team != GameObjectTeam.Neutral && predHealth > 0 &&
                              predHealth <= GetAzirAASandwarriorDamage(minion)
                        select minion)
                        return minion;
                }

                foreach (
                    var minion in
                        from minion in
                            ObjectManager.Get<Obj_AI_Minion>().Where(minion => minion.LSIsValidTarget() && minion.Name != "Beacon" && InAutoAttackRange(minion))
                        let t = (int)(MyHero.AttackCastDelay * 1000) - 100 + Game.Ping / 2 +
                                1000 * (int)MyHero.LSDistance(minion) / (int)MyProjectileSpeed()
                        let predHealth = HealthPrediction.GetHealthPrediction(minion, t, FarmDelay())
                        where minion.Team != GameObjectTeam.Neutral && predHealth > 0 &&
                              predHealth <= MyHero.LSGetAutoAttackDamage(minion, true)
                        select minion)
                    return minion;
            }

            if (CurrentMode != Mode.Lasthit)
            {
                tempTarget = GetBestHeroTarget();
                if (tempTarget != null)
                    return tempTarget;
            }

            if (CurrentMode == Mode.Harass || CurrentMode == Mode.LaneClear || CurrentMode == Mode.LaneFreeze)
            {

                foreach (
                    var turret in
                        ObjectManager.Get<Obj_AI_Turret>().Where(turret => turret.LSIsValidTarget(GetAutoAttackRange(MyHero, turret))))
                    return turret;
            }

            float[] maxhealth;
            if (CurrentMode == Mode.LaneClear || CurrentMode == Mode.Harass || CurrentMode == Mode.LaneFreeze)
            {
                if (MyHero.ChampionName == "Azir")
                {
                    maxhealth = new float[] { 0 };
                    var maxhealth1 = maxhealth;
                    foreach (
                        var minion in
                            ObjectManager.Get<Obj_AI_Minion>()
                                .Where(minion => InSoldierAttackRange(minion) && minion.Name != "Beacon" && minion.LSIsValidTarget() && minion.Team == GameObjectTeam.Neutral)
                                .Where(minion => minion.MaxHealth >= maxhealth1[0] || Math.Abs(maxhealth1[0] - float.MaxValue) < float.Epsilon))
                    {
                        tempTarget = minion;
                        maxhealth[0] = minion.MaxHealth;
                    }
                    if (tempTarget != null)
                        return tempTarget;
                }

                maxhealth = new float[] { 0 };
                var maxhealth2 = maxhealth;
                foreach (var minion in ObjectManager.Get<Obj_AI_Minion>().Where(minion => minion.LSIsValidTarget(GetAutoAttackRange(MyHero, minion)) && minion.Name != "Beacon" && minion.Team == GameObjectTeam.Neutral).Where(minion => minion.MaxHealth >= maxhealth2[0] || Math.Abs(maxhealth2[0] - float.MaxValue) < float.Epsilon))
                {
                    tempTarget = minion;
                    maxhealth[0] = minion.MaxHealth;
                }
                if (tempTarget != null)
                    return tempTarget;
            }

            if (CurrentMode != Mode.LaneClear || EloBuddy.SDK.Orbwalker.ShouldWait)
            {
                //ResetAutoAttackTimer();
                return null;
            }

            if (MyHero.ChampionName == "Azir")
            {
                maxhealth = new float[] { 0 };
                float[] maxhealth1 = maxhealth;
                foreach (var minion in from minion in ObjectManager.Get<Obj_AI_Minion>()
                    .Where(minion => minion.LSIsValidTarget() && minion.Name != "Beacon" && InSoldierAttackRange(minion))
                                       let predHealth = HealthPrediction.LaneClearHealthPrediction(minion, (int)((MyHero.AttackDelay * 1000) * LaneClearWaitTimeMod), FarmDelay(-125))
                                       where predHealth >=
                                             GetAzirAASandwarriorDamage(minion) + MyHero.LSGetAutoAttackDamage(minion, true) ||
                                             Math.Abs(predHealth - minion.Health) < float.Epsilon
                                       where minion.Health >= maxhealth1[0] || Math.Abs(maxhealth1[0] - float.MaxValue) < float.Epsilon
                                       select minion)
                {
                    tempTarget = minion;
                    maxhealth[0] = minion.MaxHealth;
                }
                if (tempTarget != null)
                    return tempTarget;
            }

            maxhealth = new float[] { 0 };
            foreach (var minion in from minion in ObjectManager.Get<Obj_AI_Minion>()
                .Where(minion => minion.LSIsValidTarget(GetAutoAttackRange(MyHero, minion)) && minion.Name != "Beacon")
                                   let predHealth = HealthPrediction.LaneClearHealthPrediction(minion, (int)((MyHero.AttackDelay * 1000) * LaneClearWaitTimeMod), FarmDelay())
                                   where predHealth >=
                                         2 * MyHero.LSGetAutoAttackDamage(minion, true) ||
                                         Math.Abs(predHealth - minion.Health) < float.Epsilon
                                   where minion.Health >= maxhealth[0] || Math.Abs(maxhealth[0] - float.MaxValue) < float.Epsilon
                                   select minion)
            {
                tempTarget = minion;
                maxhealth[0] = minion.MaxHealth;
            }
            if (tempTarget != null)
                return tempTarget;

            return null;
        }

        public static bool IsAutoAttack(string name)
        {
            return (name.ToLower().Contains("attack") && !NoAttacks.Contains(name.ToLower())) ||
            Attacks.Contains(name.ToLower());
        }

        public static void ResetAutoAttackTimer()
        {
            _lastAATick = 0;
        }

        private static float m_baseAttackSpeed;

        public static bool IsAutoAttackReset(string name)
        {
            return AttackResets.Contains(name.ToLower());
        }

        public static float GetNextAATime()
        {
            return (_lastAATick + MyHero.AttackDelay * 1000) - (Environment.TickCount + Game.Ping / 2 + 25);
        }

        public static bool CanAttack()
        {
            if (MyHero.ChampionName == "Graves")
            {
                var attackDelay = 1.0740296828d * 1000 * MyHero.AttackDelay - 716.2381256175d;
                if (Utils.TickCount + Game.Ping / 2 + 25 >= _lastAATick + attackDelay && Player.HasBuff("GravesBasicAttackAmmo1"))
                {
                    return true;
                }

                return false;
            }

            if (MyHero.ChampionName == "Jhin")
            {
                if (Player.HasBuff("JhinPassiveReload"))
                {
                    return false;
                }
            }

            if (MyHero.IsCastingInterruptableSpell())
            {
                return false;
            }

            return Utils.TickCount + Game.Ping - _lastAATick - GetCurrentWindupTime() >= 1000 / (ObjectManager.Player.GetAttackSpeed() * m_baseAttackSpeed);
        }

        private static readonly string[] NoCancelChamps = { "Kalista" };

        private static bool HaveCancled()
        {
            if (_lastAATick - Environment.TickCount > MyHero.AttackCastDelay * 1000 + 25)
                return lastRealAttack < _lastAATick;
            return false;
        }

        public static bool CanMove()
        {
            var localExtraWindup = 0;
            if (MyHero.ChampionName == "Rengar" && (Player.HasBuff("rengarqbase") || Player.HasBuff("rengarqemp")))
            {
                localExtraWindup = 200;
            }

            return NoCancelChamps.Contains(MyHero.ChampionName) || (Environment.TickCount + Game.Ping / 2 >= _lastAATick + MyHero.AttackCastDelay * 1000 + 90 + localExtraWindup);
        }

        private static float MyProjectileSpeed()
        {
            return IsMelee(MyHero) || MyHero.ChampionName == "Azir" || MyHero.ChampionName == "Velkoz" ||
                   MyHero.ChampionName == "Viktor" && Player.HasBuff("ViktorPowerTransferReturn")
                ? float.MaxValue
                : MyHero.BasicAttack.MissileSpeed;
        }

        private static bool IsMelee(AIHeroClient myHero)
        {
            return myHero.CombatType == GameObjectCombatType.Melee;
        }

        private static int FarmDelay(int offset = 0)
        {
            var ret = offset;
            if (MyHero.ChampionName == "Azir")
                ret += 125;
            return getSliderItem(menuMisc, "orb_Misc_Farmdelay") + ret;
        }

        private static Obj_AI_Base GetBestHeroTarget()
        {
            AIHeroClient killableEnemy = null;
            var hitsToKill = double.MaxValue;
            if (MyHero.ChampionName == "Azir")
            {
                foreach (var enemy in AllEnemys.Where(hero => hero.LSIsValidTarget() && InSoldierAttackRange(hero)))
                {
                    var killHits = CountKillhitsAzirSoldier(enemy);
                    if (killableEnemy != null && (!(killHits < hitsToKill) || enemy.HasBuffOfType(BuffType.Invulnerability)))
                        continue;
                    killableEnemy = enemy;
                    hitsToKill = killHits;
                }
                if (hitsToKill <= 4)
                    return killableEnemy;
                AIHeroClient[] mostdmgenemy = { null };
                foreach (var enemy in AllEnemys.Where(hero => hero.LSIsValidTarget() && InSoldierAttackRange(hero)).Where(enemy => mostdmgenemy[0] == null || GetAzirAASandwarriorDamage(enemy) > GetAzirAASandwarriorDamage(mostdmgenemy[0])))
                {
                    mostdmgenemy[0] = enemy;
                }
                if (mostdmgenemy[0] != null)
                    return mostdmgenemy[0];
            }
            foreach (var enemy in AllEnemys.Where(hero => hero.LSIsValidTarget() && InAutoAttackRange(hero)))
            {
                var killHits = CountKillhits(enemy);
                if (killableEnemy != null && (!(killHits < hitsToKill) || enemy.HasBuffOfType(BuffType.Invulnerability)))
                    continue;
                hitsToKill = killHits;
                killableEnemy = enemy;
            }
            return hitsToKill <= 3 ? killableEnemy : TargetSelector.GetTarget(GetAutoAttackRange(), DamageType.Physical);
        }

        public static double CountKillhits(Obj_AI_Base enemy)
        {
            return enemy.Health / MyHero.LSGetAutoAttackDamage(enemy);
        }

        public static double CountKillhitsAzirSoldier(Obj_AI_Base enemy)
        {
            return enemy.Health / GetAzirAASandwarriorDamage(enemy);
        }

        private static void CheckAutoWindUp()
        {
            if (!getCheckBoxItem(menuMisc, "orb_Misc_AutoWindUp"))
            {
                _windup = GetCurrentWindupTime();
                return;
            }
            var additional = 0;
            if (Game.Ping >= 100)
                additional = Game.Ping / 100 * 5;
            else if (Game.Ping > 40 && Game.Ping < 100)
                additional = Game.Ping / 100 * 10;
            else if (Game.Ping <= 40)
                additional = +20;
            var windUp = Game.Ping + additional;
            if (windUp < 40)
                windUp = 40;
            menuMisc["orb_Misc_ExtraWindUp"].Cast<Slider>().CurrentValue = windUp < 200 ? windUp : 200;
            _windup = windUp;
        }

        public static int GetCurrentWindupTime()
        {
            return getSliderItem(menuMisc, "orb_Misc_ExtraWindUp");
        }

        public static void EnableDrawing()
        {
            _drawing = true;
        }

        public static void DisableDrawing()
        {
            _drawing = false;
        }

        public static float GetAutoAttackRange(Obj_AI_Base source = null, Obj_AI_Base target = null)
        {
            if (source == null)
                source = MyHero;
            var ret = source.AttackRange + MyHero.BoundingRadius;
            if (target != null)
                ret += target.BoundingRadius;
            return ret;
        }

        public static bool InAutoAttackRange(Obj_AI_Base target)
        {
            if (target == null)
                return false;
            var myRange = GetAutoAttackRange(MyHero, target);
            return target.LSIsValidTarget(myRange);
        }

        public static Mode CurrentMode
        {
            get
            {
                if (getKeyBindItem(modeCombo, "Combo_Key"))
                    return Mode.Combo;
                if (getKeyBindItem(modeHarass, "Harass_Key"))
                    return Mode.Harass;
                if (getKeyBindItem(modeLaneClear, "LaneClear_Key"))
                    return Mode.LaneClear;
                if (getKeyBindItem(modeLaneFreeze, "LaneFreeze_Key"))
                    return Mode.LaneFreeze;
                if (getKeyBindItem(modeLasthit, "LastHit_Key"))
                    return Mode.Lasthit;
                return getKeyBindItem(modeFlee, "Flee_Key") ? Mode.Flee : Mode.None;
            }
        }

        public static void SetAttack(bool value)
        {
            _attack = value;
        }

        public static void SetMovement(bool value)
        {
            _movement = value;
        }

        public static bool GetAttack()
        {
            return _attack;
        }

        public static bool GetMovement()
        {
            return _movement;
        }

        public class BeforeAttackEventArgs
        {
            public Obj_AI_Base Target;
            public Obj_AI_Base Unit = ObjectManager.Player;
            private bool _process = true;
            public bool Process
            {
                get
                {
                    return _process;
                }
                set
                {
                    _disableNextAttack = !value;
                    _process = value;
                }
            }
        }
        private static void FireBeforeAttack(Obj_AI_Base target)
        {
            if (BeforeAttack != null)
            {
                BeforeAttack(new BeforeAttackEventArgs
                {
                    Target = target
                });
            }
            else
            {
                _disableNextAttack = false;
            }
        }

        private static void FireOnTargetSwitch(Obj_AI_Base newTarget)
        {
            if (OnTargetChange != null && (_lastTarget == null || _lastTarget.NetworkId != newTarget.NetworkId))
            {
                OnTargetChange(_lastTarget, newTarget);
            }
        }

        private static void FireAfterAttack(Obj_AI_Base unit, Obj_AI_Base target)
        {
            if (AfterAttack != null)
            {
                AfterAttack(unit, target);
            }
        }

        private static void FireOnAttack(Obj_AI_Base unit, Obj_AI_Base target)
        {
            if (OnAttack != null)
            {
                OnAttack(unit, target);
            }
        }
    }
}