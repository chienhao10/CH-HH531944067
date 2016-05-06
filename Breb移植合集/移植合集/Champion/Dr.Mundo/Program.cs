using System;
using System.Drawing;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using LeagueSharp.Common;
using Damage = LeagueSharp.Common.Damage;

namespace Mundo
{
    internal class Mundo : Spells
    {
        public static Menu config, comboMenu, harassMenu, ksMenu, miscMenu, lastHitMenu, clearMenu, drawMenu;

        public static void OnLoad()
        {
            if (ObjectManager.Player.ChampionName != "DrMundo")
                return;

            InitializeSpells();
            InitializeMenu();

            Game.OnUpdate += OnUpdate;
            Orbwalker.OnPostAttack += AfterAttack;
            Drawing.OnDraw += OnDraw;
        }

        public static void OnDraw(EventArgs args)
        {
            if (CommonUtilities.Player.IsDead || getCheckBoxItem(drawMenu, "disableDraw"))
                return;

            var width = getSliderItem(drawMenu, "width");

            if (getCheckBoxItem(drawMenu, "drawQ") && q.Level > 0)
            {
                Render.Circle.DrawCircle(CommonUtilities.Player.Position, q.Range, Color.DarkOrange, width);
            }

            if (getCheckBoxItem(drawMenu, "drawW") && w.Level > 0)
            {
                Render.Circle.DrawCircle(CommonUtilities.Player.Position, w.Range, Color.DarkOrange, width);
            }
        }

        private static void AfterAttack(AttackableUnit target, EventArgs args)
        {
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo) || Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass))
            {
                if (getCheckBoxItem(comboMenu, "useE") && e.IsReady() && target is AIHeroClient && target.IsValidTarget(e.Range))
                {
                    e.Cast();
                }
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear))
            {
                if (getCheckBoxItem(clearMenu, "useEj") && e.IsReady() && target is Obj_AI_Minion && target.IsValidTarget(e.Range))
                {
                    e.Cast();
                }
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo) ||
                Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass))
            {
                if ((getCheckBoxItem(miscMenu, "titanicC") || getCheckBoxItem(miscMenu, "ravenousC") ||
                     getCheckBoxItem(miscMenu, "tiamatC")) && !e.IsReady() && target is AIHeroClient &&
                    target.IsValidTarget(e.Range) && CommonUtilities.CheckItem())
                {
                    CommonUtilities.UseItem();
                }
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear))
            {
                if ((getCheckBoxItem(miscMenu, "titanicF") || getCheckBoxItem(miscMenu, "ravenousF") ||
                     getCheckBoxItem(miscMenu, "tiamatF")) && !e.IsReady() && target is Obj_AI_Minion &&
                    target.IsValidTarget(e.Range) && CommonUtilities.CheckItem())
                {
                    CommonUtilities.UseItem();
                }
            }
        }

        public static void InitializeMenu()
        {
            try
            {
                config = MainMenu.AddMenu(CommonUtilities.Player.ChampionName, CommonUtilities.Player.ChampionName);

                comboMenu = config.AddSubMenu("连招设置", "Combo");
                comboMenu.AddGroupLabel("Q 设置");
                comboMenu.Add("useQ", new CheckBox("使用 Q"));
                comboMenu.Add("QHealthCombo", new Slider("最低血量% 使用Q", 20, 1));
                comboMenu.AddSeparator();
                comboMenu.AddGroupLabel("W 设置");
                comboMenu.Add("useW", new CheckBox("使用 W"));
                comboMenu.Add("WHealthCombo", new Slider("最低血量% 使用W", 20, 1));
                comboMenu.AddSeparator();
                comboMenu.AddGroupLabel("E 设置");
                comboMenu.Add("useE", new CheckBox("使用 E"));
                comboMenu.AddSeparator();

                harassMenu = config.AddSubMenu("骚扰设置");
                harassMenu.Add("useQHarass", new CheckBox("使用 Q"));
                harassMenu.Add("useQHarassHP", new Slider("最低血量% 使用Q", 60, 1));

                ksMenu = config.AddSubMenu("抢头设置", "KillSteal");
                ksMenu.Add("killsteal", new CheckBox("开启抢头"));
                ksMenu.Add("useQks", new CheckBox("使用Q抢头"));
                ksMenu.Add("useIks", new CheckBox("使用点燃抢头"));

                miscMenu = config.AddSubMenu("杂项设置", "Misc");

                miscMenu.AddGroupLabel("Q");
                miscMenu.Add("autoQ", new KeyBind("自动 Q 敌人", false, KeyBind.BindTypes.PressToggle, 'J'));
                miscMenu.Add("autoQhp", new Slider("自动Q 最低血量%", 50, 1));
                miscMenu.Add("hitchanceQ", new Slider("全局 Q 命中率", 3, 0, 3));

                miscMenu.AddGroupLabel("W");
                miscMenu.Add("handleW", new CheckBox("自动开关 W"));

                miscMenu.AddGroupLabel("R");
                miscMenu.Add("useR", new CheckBox("使用 R"));
                miscMenu.Add("RHealth", new Slider("最低血量使用R", 20, 1));
                miscMenu.Add("RHealthEnemies", new CheckBox("如果敌方在附近"));

                miscMenu.AddGroupLabel("物品");
                miscMenu.Add("titanicC", new CheckBox("连招使用九头蛇"));
                miscMenu.Add("tiamatC", new CheckBox("连招使用提亚马特"));
                miscMenu.Add("ravenousC", new CheckBox("连招使用泰坦"));
                miscMenu.Add("titanicF", new CheckBox("农兵使用泰坦"));
                miscMenu.Add("tiamatF", new CheckBox("农兵使用提亚马特"));
                miscMenu.Add("ravenousF", new CheckBox("农兵使用提九头蛇"));

                lastHitMenu = config.AddSubMenu("尾兵设置", "LastHit");
                lastHitMenu.Add("useQlh", new CheckBox("使用Q"));
                lastHitMenu.Add("useQlhHP", new Slider("最低血量% 使用Q尾兵", 50, 1));
                lastHitMenu.Add("qRange", new CheckBox("只对有距离的兵使用"));

                clearMenu = config.AddSubMenu("推线设置", "Clear");
                clearMenu.Add("useQlc", new CheckBox("清线 Q 尾兵"));
                clearMenu.Add("useQlcHP", new Slider("最低血量% 使用Q", 40, 1));
                clearMenu.Add("useWlc", new CheckBox("清线 W"));
                clearMenu.Add("useWlcHP", new Slider("最低血量% 使用W", 40, 1));
                clearMenu.Add("useWlcMinions", new Slider("最低命中小兵数量", 3, 1, 10));
                clearMenu.AddSeparator();
                clearMenu.Add("useQj", new CheckBox("清野 Q"));
                clearMenu.Add("useQjHP", new Slider("最低血量% 使用Q", 20, 1));
                clearMenu.Add("useWj", new CheckBox("清野 W"));
                clearMenu.Add("useWjHP", new Slider("最低血量% 使用W", 20, 1));
                clearMenu.Add("useEj", new CheckBox("清野 E"));

                drawMenu = config.AddSubMenu("线圈", "Drawings");
                drawMenu.Add("disableDraw", new CheckBox("屏蔽线圈"));
                drawMenu.Add("drawQ", new CheckBox("Q 范围"));
                drawMenu.Add("drawW", new CheckBox("W 范围"));
                drawMenu.Add("width", new Slider("线圈宽度", 2, 1, 5));
            }
            catch (Exception exception)
            {
                Console.WriteLine(@"Could not load the menu - {0}", exception);
            }
        }

        private static void OnUpdate(EventArgs args)
        {
            if (CommonUtilities.Player.IsDead)
                return;

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                ExecuteCombo();
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass))
            {
                LastHit();
                ExecuteHarass();
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LastHit))
            {
                LastHit();
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear) ||
                Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear))
            {
                LaneClear();
                JungleClear();
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.None))
            {
                BurningManager();
            }

            AutoR();
            AutoQ();
            KillSteal();
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

        private static void ExecuteCombo()
        {
            var target = TargetSelector.GetTarget(q.Range, DamageType.Magical);

            if (target == null || !target.IsValid)
                return;

            var castQ = getCheckBoxItem(comboMenu, "useQ") && q.IsReady();
            var castW = getCheckBoxItem(comboMenu, "useW") && w.IsReady();

            var qHealth = getSliderItem(comboMenu, "QHealthCombo");
            var wHealth = getSliderItem(comboMenu, "WHealthCombo");

            if (castQ && CommonUtilities.Player.HealthPercent >= qHealth && target.IsValidTarget(q.Range))
            {
                if (q.GetPrediction(target).Hitchance >= CommonUtilities.GetHitChance("hitchanceQ"))
                {
                    q.Cast(target);
                }
            }

            if (castW && CommonUtilities.Player.HealthPercent >= wHealth && !IsBurning() && target.IsValidTarget(500))
            {
                w.Cast();
            }
            else if (castW && IsBurning() && !FoundEnemies(600))
            {
                w.Cast();
            }
        }

        private static void ExecuteHarass()
        {
            var target = TargetSelector.GetTarget(q.Range, DamageType.Magical);

            if (target == null || !target.IsValid)
                return;

            var castQ = getCheckBoxItem(harassMenu, "useQHarass") && q.IsReady();
            var qHealth = getSliderItem(harassMenu, "useQHarassHP");

            if (castQ && CommonUtilities.Player.HealthPercent >= qHealth && target.IsValidTarget(q.Range))
            {
                if (q.GetPrediction(target).Hitchance >= CommonUtilities.GetHitChance("hitchanceQ"))
                {
                    q.Cast(target);
                }
            }
        }

        private static void LastHit()
        {
            var castQ = getCheckBoxItem(lastHitMenu, "useQlh") && q.IsReady();

            var qHealth = getSliderItem(lastHitMenu, "useQlhHP");

            var minions = MinionManager.GetMinions(CommonUtilities.Player.ServerPosition, q.Range, MinionTypes.All,
                MinionTeam.NotAlly);

            if (minions.Count > 0 && castQ && CommonUtilities.Player.HealthPercent >= qHealth)
            {
                foreach (var minion in minions)
                {
                    if (getCheckBoxItem(miscMenu, "qRange"))
                    {
                        if (
                            HealthPrediction.GetHealthPrediction(minion,
                                (int) (q.Delay + minion.Distance(CommonUtilities.Player.Position)/q.Speed)) <
                            CommonUtilities.Player.GetSpellDamage(minion, SpellSlot.Q) &&
                            CommonUtilities.Player.Distance(minion) > CommonUtilities.Player.AttackRange*2)
                        {
                            q.Cast(minion);
                        }
                    }
                    else
                    {
                        if (
                            HealthPrediction.GetHealthPrediction(minion,
                                (int) (q.Delay + minion.Distance(CommonUtilities.Player.Position)/q.Speed)) <
                            CommonUtilities.Player.GetSpellDamage(minion, SpellSlot.Q))
                        {
                            q.Cast(minion);
                        }
                    }
                }
            }
        }

        private static void LaneClear()
        {
            var castQ = getCheckBoxItem(clearMenu, "useQlc") && q.IsReady();
            var castW = getCheckBoxItem(clearMenu, "useWlc") && w.IsReady();

            var qHealth = getSliderItem(clearMenu, "useQlcHP");
            var wHealth = getSliderItem(clearMenu, "useWlcHP");
            var wMinions = getSliderItem(clearMenu, "useWlcMinions");

            var minions = MinionManager.GetMinions(CommonUtilities.Player.ServerPosition, q.Range);
            var minionsW = MinionManager.GetMinions(CommonUtilities.Player.ServerPosition, 400);

            if (minions.Count > 0)
            {
                if (castQ && CommonUtilities.Player.HealthPercent >= qHealth)
                {
                    foreach (var minion in minions)
                    {
                        if (
                            HealthPrediction.GetHealthPrediction(minion,
                                (int) (q.Delay + minion.Distance(CommonUtilities.Player.Position)/q.Speed)) <
                            CommonUtilities.Player.GetSpellDamage(minion, SpellSlot.Q))
                        {
                            q.Cast(minion);
                        }
                    }
                }
            }

            if (minionsW.Count >= wMinions)
            {
                if (castW && CommonUtilities.Player.HealthPercent >= wHealth && !IsBurning())
                {
                    w.Cast();
                }
                else if (castW && IsBurning() && minions.Count < wMinions)
                {
                    w.Cast();
                }
            }
        }

        private static void JungleClear()
        {
            var castQ = getCheckBoxItem(clearMenu, "useQj") && q.IsReady();
            var castW = getCheckBoxItem(clearMenu, "useWj") && w.IsReady();

            var qHealth = getSliderItem(clearMenu, "useQjHP");
            var wHealth = getSliderItem(clearMenu, "useWjHP");

            var minions = MinionManager.GetMinions(CommonUtilities.Player.ServerPosition, q.Range, MinionTypes.All,
                MinionTeam.Neutral, MinionOrderTypes.MaxHealth);
            var minionsW = MinionManager.GetMinions(CommonUtilities.Player.ServerPosition, 400, MinionTypes.All,
                MinionTeam.Neutral, MinionOrderTypes.MaxHealth);

            if (minions.Count > 0)
            {
                var minion = minions[0];

                if (castQ && CommonUtilities.Player.HealthPercent >= qHealth)
                {
                    q.Cast(minion);
                }
            }

            if (minionsW.Count > 0)
            {
                if (castW && CommonUtilities.Player.HealthPercent >= wHealth && !IsBurning())
                {
                    w.Cast();
                }
                else if (castW && IsBurning() && minionsW.Count < 1)
                {
                    w.Cast();
                }
            }
        }

        private static void KillSteal()
        {
            if (!getCheckBoxItem(ksMenu, "killsteal"))
                return;

            if (getCheckBoxItem(ksMenu, "useQks") && q.IsReady())
            {
                foreach (
                    var target in
                        HeroManager.Enemies.Where(
                            enemy => enemy.IsValidTarget(q.Range) && !enemy.HasBuffOfType(BuffType.Invulnerability))
                            .Where(target => target.Health < CommonUtilities.Player.GetSpellDamage(target, SpellSlot.Q))
                    )
                {
                    if (q.GetPrediction(target).Hitchance >= CommonUtilities.GetHitChance("hitchanceQ"))
                    {
                        q.Cast(target);
                    }
                }
            }

            if (getCheckBoxItem(ksMenu, "useIks") && ignite.Slot.IsReady() && ignite != null &&
                ignite.Slot != SpellSlot.Unknown)
            {
                foreach (
                    var target in
                        HeroManager.Enemies.Where(
                            enemy =>
                                enemy.IsValidTarget(ignite.SData.CastRange) &&
                                !enemy.HasBuffOfType(BuffType.Invulnerability))
                            .Where(
                                target =>
                                    target.Health <
                                    CommonUtilities.Player.GetSummonerSpellDamage(target, Damage.SummonerSpell.Ignite)))
                {
                    CommonUtilities.Player.Spellbook.CastSpell(ignite.Slot, target);
                }
            }
        }

        private static void AutoQ()
        {
            if (CommonUtilities.Player.IsDead)
                return;

            var autoQ = getKeyBindItem(miscMenu, "autoQ") && q.IsReady();

            var qHealth = getSliderItem(miscMenu, "autoQhp");

            var target = TargetSelector.GetTarget(q.Range, DamageType.Magical);

            if (autoQ && CommonUtilities.Player.HealthPercent >= qHealth && target.IsValidTarget(q.Range))
            {
                if (q.GetPrediction(target).Hitchance >= CommonUtilities.GetHitChance("hitchanceQ"))
                {
                    q.Cast(target);
                }
            }
        }

        private static void AutoR()
        {
            if (CommonUtilities.Player.IsDead)
                return;

            var castR = getCheckBoxItem(miscMenu, "useR") && r.IsReady();

            var rHealth = getSliderItem(miscMenu, "RHealth");
            var rEnemies = getCheckBoxItem(miscMenu, "RHealthEnemies");

            if (rEnemies && castR && CommonUtilities.Player.HealthPercent <= rHealth &&
                !CommonUtilities.Player.InFountain())
            {
                if (FoundEnemies(q.Range))
                {
                    r.Cast();
                }
            }
            else if (!rEnemies && castR && CommonUtilities.Player.HealthPercent <= rHealth &&
                     !CommonUtilities.Player.InFountain())
            {
                r.Cast();
            }
        }

        private static bool IsBurning()
        {
            return CommonUtilities.Player.HasBuff("BurningAgony");
        }

        private static bool FoundEnemies(float range)
        {
            return HeroManager.Enemies.Any(enemy => enemy.IsValidTarget(range));
        }

        private static void BurningManager()
        {
            if (!getCheckBoxItem(miscMenu, "handleW"))
                return;

            if (IsBurning() && w.IsReady())
            {
                w.Cast();
            }
        }
    }
}