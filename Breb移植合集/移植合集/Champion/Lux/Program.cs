using System;
using System.Collections.Generic;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using LeagueSharp.Common;
using SharpDX;
using Color = System.Drawing.Color;
using Spell = LeagueSharp.Common.Spell;

namespace MoonLux
{
    internal class Program
    {
        #region Properties

        /// <summary>
        ///     Gets or sets the e.
        /// </summary>
        /// <value>
        ///     The e.
        /// </value>
        private static Spell E { get; set; }

        /// <summary>
        ///     Gets a value indicating whether the E spell was casted.
        /// </summary>
        /// <value>
        ///     <c>true</c> if the E spell was casted; otherwise, <c>false</c>.
        /// </value>
        private static bool ECasted
        {
            get { return Player.HasBuff("LuxLightStrikeKugel") || EObject != null; }
        }

        private static GameObject EObject { get; set; }

        /// <summary>
        ///     Gets or sets the menu.
        /// </summary>
        /// <value>
        ///     The menu.
        /// </value>
        private static Menu Menu { get; set; }

        /// <summary>
        ///     Gets the player.
        /// </summary>
        /// <value>
        ///     The player.
        /// </value>
        private static AIHeroClient Player
        {
            get { return ObjectManager.Player; }
        }

        /// <summary>
        ///     Gets or sets the q.
        /// </summary>
        /// <value>
        ///     The q.
        /// </value>
        private static Spell Q { get; set; }

        /// <summary>
        ///     Gets or sets the r.
        /// </summary>
        /// <value>
        ///     The r.
        /// </value>
        private static Spell R { get; set; }

        /// <summary>
        ///     Gets or sets the w.
        /// </summary>
        /// <value>
        ///     The w.
        /// </value>
        private static Spell W { get; set; }

        #endregion

        #region Methods

        /// <summary>
        ///     Fired on an incoming gapcloser.
        /// </summary>
        /// <param name="gapcloser">The gapcloser.</param>
        private static void AntiGapcloserOnOnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (!gapcloser.Sender.LSIsValidTarget(Q.Range) || !getCheckBoxItem(miscMenu, "QGapcloser"))
            {
                return;
            }

            Q.Cast(gapcloser.Sender);
        }

        /// <summary>
        ///     Called when a <see cref="AttackableUnit" /> takes/gives damage.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="AttackableUnitDamageEventArgs" /> instance containing the event data.</param>
        private static void AttackableUnit_OnDamage(AttackableUnit sender, AttackableUnitDamageEventArgs args)
        {
            var source = ObjectManager.GetUnitByNetworkId<GameObject>((uint)args.Source.NetworkId);
            var obj = ObjectManager.GetUnitByNetworkId<GameObject>((uint)args.Target.NetworkId);

            if (source == null || obj == null)
            {
                return;
            }

            if (obj.Type != GameObjectType.AIHeroClient || source.Type != GameObjectType.AIHeroClient)
            {
                return;
            }

            var hero = (AIHeroClient)obj;

            if (hero.IsEnemy || (!hero.IsMe && !W.IsInRange(obj))
                || !getCheckBoxItem(shieldMenu, hero.NetworkId + ""))
            {
                return;
            }

            if (((int)(args.Damage / hero.Health) > getSliderItem(shieldMenu, "ASDamagePercent"))
                || (hero.HealthPercent < getSliderItem(shieldMenu, "ASHealthPercent")))
            {
                W.Cast(hero);
            }
        }

        /// <summary>
        ///     Casts the e.
        /// </summary>
        /// <param name="target">The target.</param>
        private static void CastE(AIHeroClient target)
        {
            if (Environment.TickCount - E.LastCastAttemptT < E.Delay * 1000)
            {
                return;
            }

            if (ECasted)
            {
                if (EObject.Position.CountEnemiesInRange(350) >= 1 && ObjectManager.Get<AIHeroClient>().Count(x => x.LSIsValidTarget(350, true, EObject.Position) && !x.HasPassive()) >= 1)
                {
                    E.Cast();
                }
            }
            else if (!target.HasPassive())
            {
                E.Cast(target);
            }
        }

        /// <summary>
        ///     Casts the q.
        /// </summary>
        /// <param name="target">The target.</param>
        private static void CastQ(AIHeroClient target)
        {
            Q.Cast(target);
        }

        /// <summary>
        ///     Creates the menu.
        /// </summary>
        public static Menu comboMenu, harassMenu, waveClearMenu, ksMenu, shieldMenu, jungleKsMenu, miscMenu, drawMenu;

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

        private static void CreateMenu()
        {
            Menu = MainMenu.AddMenu("Moon光辉", "ChewyLUXFF");

            comboMenu = Menu.AddSubMenu("连招", "ComboSettings");
            comboMenu.Add("UseQCombo", new CheckBox("使用 Q"));
            comboMenu.Add("UseQSlowedCombo", new CheckBox("只被E减速后使用Q", false));
            comboMenu.Add("UseWCombo", new CheckBox("使用 W", false));
            comboMenu.Add("UseECombo", new CheckBox("使用 E"));
            comboMenu.Add("UseRCombo", new CheckBox("使用 R"));
            comboMenu.Add("UseRComboMode", new ComboBox("R 模式", 1, "一直", "可击杀", "超出距离"));

            harassMenu = Menu.AddSubMenu("骚扰", "HarassSettings");
            harassMenu.Add("UseQHarass", new CheckBox("使用 Q"));
            harassMenu.Add("UseWHarass", new CheckBox("使用 W", false));
            harassMenu.Add("UseEHarass", new CheckBox("使用 E"));
            harassMenu.Add("HarassMinMana", new Slider("骚扰最低蓝量", 50));
            harassMenu.Add("HarassKeybind", new KeyBind("骚扰! (开关)", false, KeyBind.BindTypes.PressToggle, 84));

            waveClearMenu = Menu.AddSubMenu("清线", "WaveClearSettings");
            waveClearMenu.Add("UseQWaveClear", new CheckBox("使用 Q", false));
            waveClearMenu.Add("UseEWaveClear", new CheckBox("使用 E", false));
            waveClearMenu.Add("UseRWaveClear", new CheckBox("使用 R", false));
            waveClearMenu.Add("WaveClearMinMana", new Slider("清线最低蓝量", 75));

            ksMenu = Menu.AddSubMenu("抢头", "KSSettings");
            ksMenu.Add("UseQKS", new CheckBox("使用 Q"));
            ksMenu.Add("UseEKS", new CheckBox("使用 E", false));
            ksMenu.Add("UseRKS", new CheckBox("使用 R"));

            shieldMenu = Menu.AddSubMenu("自动W 设置", "ASSettings");
            shieldMenu.Add("ASHealthPercent", new Slider("生命 %", 25));
            shieldMenu.Add("ASDamagePercent", new Slider("伤害 %", 20));
            HeroManager.Allies.ForEach(x => shieldMenu.Add(x.NetworkId + "", new CheckBox("护盾 " + x.ChampionName)));

            jungleKsMenu = Menu.AddSubMenu("偷野", "JungleKS");
            jungleKsMenu.Add("StealBaron", new CheckBox("男爵"));
            jungleKsMenu.Add("StealDragon", new CheckBox("龙"));
            jungleKsMenu.Add("StealBlueBuff", new CheckBox("蓝"));
            jungleKsMenu.Add("StealRedBuff", new CheckBox("红"));
            jungleKsMenu.Add("StealBuffMode", new ComboBox("偷野模式", 0, "只敌方的", "双方", "只友军的"));

            miscMenu = Menu.AddSubMenu("杂项", "MiscSettings");
            miscMenu.Add("SpellWeaveCombo", new CheckBox("使用技能清线"));
            miscMenu.Add("QGapcloser", new CheckBox("防突进 Q"));
            
            drawMenu = Menu.AddSubMenu("线圈", "DrawSettings");
            drawMenu.Add("DrawQ", new CheckBox("显示 Q"));
            drawMenu.Add("DrawW", new CheckBox("显示 W", false));
            drawMenu.Add("DrawE", new CheckBox("显示 E"));
            drawMenu.Add("DrawERad", new CheckBox("显示 E 半径"));
            drawMenu.Add("DrawR", new CheckBox("显示 R"));
        }

        /// <summary>
        ///     Does the combo.
        /// </summary>
        private static void DoCombo()
        {
            var useQCombo = getCheckBoxItem(comboMenu, "UseQCombo");
            var useQSlowedCombo = getCheckBoxItem(comboMenu, "UseQSlowedCombo");
            var useWCombo = getCheckBoxItem(comboMenu, "UseWCombo");
            var useECombo = getCheckBoxItem(comboMenu, "UseECombo");
            var useRCombo = getCheckBoxItem(comboMenu, "UseRCombo");
            var useRComboMode = getBoxItem(comboMenu, "UseRComboMode");
            var spellWeaveCombo = getCheckBoxItem(miscMenu, "SpellWeaveCombo");

            var target = TargetSelector.GetTarget(Q.Range, DamageType.Magical);

            if (!target.LSIsValidTarget())
            {
                if (HeroManager.Enemies.Any(x => R.IsInRange(x)) && useRComboMode == 2 && R.IsReady())
                {
                    R.Cast(target);
                }

                return;
            }

            if (useQCombo && Q.IsReady())
            {
                if (spellWeaveCombo)
                {
                    if (!target.HasPassive())
                    {
                        if (useQSlowedCombo && target.HasBuffOfType(BuffType.Slow))
                        {
                            CastQ(target);
                        }
                        else if (!useQSlowedCombo)
                        {
                            CastQ(target);
                        }
                    }
                }
                else
                {
                    CastQ(target);
                }
            }

            if (useWCombo && W.IsReady())
            {
                W.Cast(Game.CursorPos);
            }

            if (useECombo && E.IsReady())
            {
                CastE(target);
            }

            if (!useRCombo || !R.IsReady())
            {
                return;
            }

            // ReSharper disable once SwitchStatementMissingSomeCases
            switch (useRComboMode)
            {
                case 0:
                    R.Cast(target);
                    break;
                case 1:
                    if (R.IsKillable(target))
                    {
                        R.Cast(target);
                    }
                    break;
            }
        }

        /// <summary>
        ///     Does the harass.
        /// </summary>
        private static void DoHarass()
        {
            var useQHarass = getCheckBoxItem(harassMenu, "UseQHarass");
            var useWHarass = getCheckBoxItem(harassMenu, "UseWHarass");
            var useEHarass = getCheckBoxItem(harassMenu, "UseEHarass");
            var spellWeaveCombo = getCheckBoxItem(miscMenu, "SpellWeaveCombo");

            var target = TargetSelector.GetTarget(Q.Range, DamageType.Magical);

            if (!target.LSIsValidTarget() || Player.ManaPercent < getSliderItem(harassMenu, "HarassMinMana"))
            {
                return;
            }

            if (useQHarass && Q.IsReady())
            {
                if (spellWeaveCombo)
                {
                    if (!target.HasPassive())
                    {
                        CastQ(target);
                    }
                }
                else
                {
                    CastQ(target);
                }
            }

            if (useWHarass && W.IsReady())
            {
                W.Cast(Game.CursorPos);
            }

            if (useEHarass && E.IsReady())
            {
                CastE(target);
            }
        }

        /// <summary>
        ///     Does the lane clear.
        /// </summary>
        private static void DoLaneClear()
        {
            var useQWaveClear = getCheckBoxItem(waveClearMenu, "UseQWaveClear");
            var useEWaveClear = getCheckBoxItem(waveClearMenu, "UseEWaveClear");
            var useRWaveClear = getCheckBoxItem(waveClearMenu, "UseRWaveClear");
            var waveClearMana = getSliderItem(waveClearMenu, "WaveClearMinMana");

            if (Player.ManaPercent < waveClearMana)
            {
                return;
            }

            if (useQWaveClear && Q.IsReady())
            {
                var farmLoc = Q.GetLineFarmLocation(MinionManager.GetMinions(Q.Range));

                if (farmLoc.MinionsHit >= 2)
                {
                    Q.Cast(farmLoc.Position);
                }
            }

            if (useEWaveClear && E.IsReady())
            {
                var farmLoc = E.GetCircularFarmLocation(MinionManager.GetMinions(E.Range));

                if (farmLoc.MinionsHit >= 3)
                {
                    E.Cast(farmLoc.Position);
                }
            }

            if (!useRWaveClear || !R.IsReady())
            {
                return;
            }
            {
                var farmLoc = R.GetLineFarmLocation(MinionManager.GetMinions(R.Range));

                if (farmLoc.MinionsHit >= 10)
                {
                    R.Cast(farmLoc.Position);
                }
            }
        }

        /// <summary>
        ///     Fired when the game is Drawn.
        /// </summary>
        /// <param name="args">The <see cref="EventArgs" /> instance containing the event data.</param>
        private static void Drawing_OnDraw(EventArgs args)
        {
            var drawQ = getCheckBoxItem(drawMenu, "DrawQ");
            var drawW = getCheckBoxItem(drawMenu, "DrawW");
            var drawE = getCheckBoxItem(drawMenu, "DrawE");
            var drawErad = getCheckBoxItem(drawMenu, "DrawERad");

            if (drawQ)
            {
                Render.Circle.DrawCircle(Player.Position, Q.Range, Q.IsReady() ? Color.Aqua : Color.Red);
            }

            if (drawW)
            {
                Render.Circle.DrawCircle(Player.Position, W.Range, W.IsReady() ? Color.Aqua : Color.Red);
            }

            if (drawE)
            {
                Render.Circle.DrawCircle(Player.Position, E.Range, E.IsReady() ? Color.Aqua : Color.Red);
            }

            if (drawErad && EObject != null)
            {
                Render.Circle.DrawCircle(EObject.Position, 350, Color.CornflowerBlue);
            }
        }

        /// <summary>
        ///     Fired when the scene has been fully drawn.
        /// </summary>
        /// <param name="args">The <see cref="EventArgs" /> instance containing the event data.</param>
        private static void Drawing_OnEndScene(EventArgs args)
        {
            if (!getCheckBoxItem(drawMenu, "DrawR") || !R.IsReady())
            {
                return;
            }

            var pointList = new List<Vector3>();

            for (var i = 0; i < 30; i++)
            {
                var angle = i * Math.PI * 2 / 30;
                pointList.Add(
                    new Vector3(
                        Player.Position.X + R.Range * (float)Math.Cos(angle),
                        Player.Position.Y + R.Range * (float)Math.Sin(angle),
                        Player.Position.Z));
            }

            for (var i = 0; i < pointList.Count; i++)
            {
                var a = pointList[i];
                var b = pointList[i == pointList.Count - 1 ? 0 : i + 1];

                var aonScreen = Drawing.WorldToMinimap(a);
                var bonScreen = Drawing.WorldToMinimap(b);

                Drawing.DrawLine(
                    aonScreen.X,
                    aonScreen.Y,
                    bonScreen.X,
                    bonScreen.Y,
                    1,
                    Color.Aqua);
            }
        }

        /// <summary>
        ///     Fired when the game is loaded.
        /// </summary>
        /// <param name="args">The <see cref="EventArgs" /> instance containing the event data.</param>
        private static void Game_OnUpdate(EventArgs args)
        {
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass))
            {
                DoHarass();
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear) ||
                Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear))
            {
                DoLaneClear();
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                DoCombo();
            }

            if (getKeyBindItem(harassMenu, "HarassKeybind") &&
                !Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass))
            {
                DoHarass();
            }

            if (EObject != null && Player.CountEnemiesInRange(E.Range + 200) < 1)
            {
                if (!Player.LSIsRecalling() && ECasted && EObject.Position.CountEnemiesInRange(350) >= 1)
                {
                    E.Cast();
                }
            }

            KillSteal();
            JungleKillSteal();
        }

        /// <summary>
        ///     Fired when the game is loaded.
        /// </summary>
        /// <param name="args">The <see cref="EventArgs" /> instance containing the event data.</param>
        public static void GameOnOnGameLoad()
        {
            if (Player.ChampionName != "Lux")
            {
                return;
            }

            Q = new Spell(SpellSlot.Q, 1175);
            W = new Spell(SpellSlot.W, 1075);
            E = new Spell(SpellSlot.E, 1075);
            R = new Spell(SpellSlot.R, 3000);

            Q.SetSkillshot(0.25f, 80f, 1200f, true, SkillshotType.SkillshotLine);
            W.SetSkillshot(0.25f, 110f, 1200f, false, SkillshotType.SkillshotLine);
            E.SetSkillshot(0.3f, 250f, 1050f, false, SkillshotType.SkillshotCircle);
            R.SetSkillshot(1f, 110f, float.MaxValue, false, SkillshotType.SkillshotLine);

            CreateMenu();

            GameObject.OnCreate += delegate (GameObject sender, EventArgs args2)
            {
                if (sender.Name.Contains("Lux_Base_E_tar"))
                {
                    EObject = sender;
                }
            };

            GameObject.OnDelete += delegate (GameObject sender, EventArgs args2)
            {
                if (sender.Name.Contains("Lux_Base_E_tar"))
                {
                    EObject = null;
                }
            };

            Game.OnUpdate += Game_OnUpdate;
            AttackableUnit.OnDamage += AttackableUnit_OnDamage;
            AntiGapcloser.OnEnemyGapcloser += AntiGapcloserOnOnEnemyGapcloser;
            Drawing.OnDraw += Drawing_OnDraw;
            Drawing.OnEndScene += Drawing_OnEndScene;
        }

        /// <summary>
        ///     Last hits jungle mobs with a spell.
        /// </summary>
        private static void JungleKillSteal()
        {
            if (!R.IsReady())
            {
                return;
            }

            var stealBlue = getCheckBoxItem(jungleKsMenu, "StealBlueBuff");
            var stealRed = getCheckBoxItem(jungleKsMenu, "StealRedBuff");
            var stealDragon = getCheckBoxItem(jungleKsMenu, "StealDragon");
            var stealBaron = getCheckBoxItem(jungleKsMenu, "StealBaron");
            var stealBuffMode = getBoxItem(jungleKsMenu, "StealBuffMode");

            if (stealBaron)
            {
                var baron =
                    ObjectManager.Get<Obj_AI_Minion>().FirstOrDefault(x => x.CharData.BaseSkinName.Equals("SRU_Baron"));

                if (baron != null)
                {
                    var healthPred = HealthPrediction.GetHealthPrediction(baron, (int)(R.Delay * 1000) + Game.Ping / 2);

                    if (R.GetDamage(baron) >= healthPred)
                    {
                        R.Cast(baron);
                    }
                }
            }

            if (stealDragon)
            {
                var dragon =
                    ObjectManager.Get<Obj_AI_Minion>().FirstOrDefault(x => x.CharData.BaseSkinName.Equals("SRU_Dragon"));

                if (dragon != null)
                {
                    var healthPred = HealthPrediction.GetHealthPrediction(dragon, (int)(R.Delay * 1000) + Game.Ping / 2);

                    if (R.GetDamage(dragon) >= healthPred)
                    {
                        R.Cast(dragon);
                    }
                }
            }

            if (stealBlue)
            {
                var blueBuffs =
                    ObjectManager.Get<Obj_AI_Minion>().Where(x => x.CharData.BaseSkinName.Equals("SRU_Blue")).ToList();

                if (blueBuffs.Any())
                {
                    var blueBuff =
                        blueBuffs.Where(
                            x =>
                                R.GetDamage(x) >
                                HealthPrediction.GetHealthPrediction(x, (int)(R.Delay * 1000) + Game.Ping / 2))
                            .FirstOrDefault(
                                x =>
                                    (x.CountAlliesInRange(1000) == 0 && stealBuffMode == 0)
                                    || (x.CountAlliesInRange(1000) > 0 && stealBuffMode == 2) || stealBuffMode == 3);

                    if (blueBuff != null)
                    {
                        R.Cast(blueBuff);
                    }
                }
            }

            if (!stealRed)
            {
                return;
            }

            var redBuffs =
                ObjectManager.Get<Obj_AI_Minion>().Where(x => x.CharData.BaseSkinName.Equals("SRU_Red")).ToList();

            if (!redBuffs.Any())
            {
                return;
            }

            var redBuff =
                redBuffs.Where(
                    x => R.GetDamage(x) > HealthPrediction.GetHealthPrediction(x, (int)(R.Delay * 1000) + Game.Ping / 2))
                    .FirstOrDefault(
                        x =>
                            (x.CountAlliesInRange(1000) == 0 && stealBuffMode == 0)
                            || (x.CountAlliesInRange(1000) > 0 && stealBuffMode == 2) || stealBuffMode == 3);

            if (redBuff != null)
            {
                R.Cast(redBuff);
            }
        }

        /// <summary>
        ///     Last hits champions with spells.
        /// </summary>
        private static void KillSteal()
        {
            var spellsToUse =
                new List<Spell>(
                    new[] { Q, E, R }.Where(
                        x =>
                            x.IsReady() &&
                            getCheckBoxItem(ksMenu, "Use" + Enum.GetName(typeof(SpellSlot), x.Slot) + "KS")));

            foreach (var enemy in HeroManager.Enemies)
            {
                var spell =
                    spellsToUse.Where(x => x.GetDamage(enemy) > enemy.Health && enemy.LSIsValidTarget(x.Range))
                        .MinOrDefault(x => x.GetDamage(enemy));

                if (spell == null)
                {
                    continue;
                }

                spell.Cast(enemy);

                return;
            }
        }

        #endregion
    }

    public static class LuxExtensions
    {
        #region Public Methods and Operators

        /// <summary>
        ///     Gets the passive damage.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <returns></returns>
        public static float GetPassiveDamage(this AIHeroClient target)
        {
            return
                (float)
                    ObjectManager.Player.CalcDamage(
                        target,
                        DamageType.Magical,
                        10 + 8 * ObjectManager.Player.Level + 0.2 * ObjectManager.Player.TotalMagicalDamage);
        }

        /// <summary>
        ///     Determines whether this instance has passive.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <returns></returns>
        public static bool HasPassive(this AIHeroClient target)
        {
            return target.HasBuff("luxilluminatingfraulein");
        }

        #endregion
    }
}