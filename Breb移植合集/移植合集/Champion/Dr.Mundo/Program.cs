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
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo) ||
                Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass))
            {
                if (getCheckBoxItem(comboMenu, "useE") && e.IsReady() && target is AIHeroClient &&
                    target.IsValidTarget(e.Range))
                {
                    e.Cast();
                }
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear))
            {
                if (getCheckBoxItem(clearMenu, "useEj") && e.IsReady() && target is Obj_AI_Minion &&
                    target.IsValidTarget(e.Range))
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

                comboMenu = config.AddSubMenu("Combo Settings", "Combo");
                comboMenu.AddGroupLabel("Q Settings");
                comboMenu.Add("useQ", new CheckBox("Use Q"));
                comboMenu.Add("QHealthCombo", new Slider("Minimum HP% to use Q", 20, 1));
                comboMenu.AddSeparator();
                comboMenu.AddGroupLabel("W Settings");
                comboMenu.Add("useW", new CheckBox("Use W"));
                comboMenu.Add("WHealthCombo", new Slider("Minimum HP% to use W", 20, 1));
                comboMenu.AddSeparator();
                comboMenu.AddGroupLabel("E Settings");
                comboMenu.Add("useE", new CheckBox("Use E"));
                comboMenu.AddSeparator();

                harassMenu = config.AddSubMenu("Harass Settings");
                harassMenu.Add("useQHarass", new CheckBox("Use Q"));
                harassMenu.Add("useQHarassHP", new Slider("Minimum HP% to use Q", 60, 1));

                ksMenu = config.AddSubMenu("KillSteal Settings", "KillSteal");
                ksMenu.Add("killsteal", new CheckBox("Activate KillSteal"));
                ksMenu.Add("useQks", new CheckBox("Use Q to KillSteal"));
                ksMenu.Add("useIks", new CheckBox("Use Ignite to KillSteal"));

                miscMenu = config.AddSubMenu("Misc Settings", "Misc");

                miscMenu.AddGroupLabel("Q");
                miscMenu.Add("autoQ", new KeyBind("Auto Q on enemies", false, KeyBind.BindTypes.PressToggle, 'J'));
                miscMenu.Add("autoQhp", new Slider("Minimum HP% to auto Q", 50, 1));
                miscMenu.Add("hitchanceQ", new Slider("Global Q Hitchance", 3, 0, 3));

                miscMenu.AddGroupLabel("W");
                miscMenu.Add("handleW", new CheckBox("Automatically handle W"));

                miscMenu.AddGroupLabel("R");
                miscMenu.Add("useR", new CheckBox("Use R"));
                miscMenu.Add("RHealth", new Slider("Minimum HP% to use R", 20, 1));
                miscMenu.Add("RHealthEnemies", new CheckBox("If enemies nearby"));

                miscMenu.AddGroupLabel("Item");
                miscMenu.Add("titanicC", new CheckBox("Use titanic Hydra in combo"));
                miscMenu.Add("tiamatC", new CheckBox("Use Tiamat in combo"));
                miscMenu.Add("ravenousC", new CheckBox("Use Ravenous Hydra in combo"));
                miscMenu.Add("titanicF", new CheckBox("Use titanic Hydra in Farm"));
                miscMenu.Add("tiamatF", new CheckBox("Use Tiamat in Farm"));
                miscMenu.Add("ravenousF", new CheckBox("Use Ravenous Hydra in Farm"));

                lastHitMenu = config.AddSubMenu("Last Hit Settings", "LastHit");
                lastHitMenu.Add("useQlh", new CheckBox("Use Q to last hit minions"));
                lastHitMenu.Add("useQlhHP", new Slider("Minimum HP% to use Q to lasthit", 50, 1));
                lastHitMenu.Add("qRange", new CheckBox("Only use Q if far from minions"));

                clearMenu = config.AddSubMenu("Clear Settings", "Clear");
                clearMenu.Add("useQlc", new CheckBox("Use Q to last hit in laneclear"));
                clearMenu.Add("useQlcHP", new Slider("Minimum HP% to use Q to laneclear", 40, 1));
                clearMenu.Add("useWlc", new CheckBox("Use W in laneclear"));
                clearMenu.Add("useWlcHP", new Slider("Minimum HP% to use W to laneclear", 40, 1));
                clearMenu.Add("useWlcMinions", new Slider("Minimum minions to W in laneclear", 3, 1, 10));
                clearMenu.AddSeparator();
                clearMenu.Add("useQj", new CheckBox("Use Q to jungle"));
                clearMenu.Add("useQjHP", new Slider("Minimum HP% to use Q in jungle", 20, 1));
                clearMenu.Add("useWj", new CheckBox("Use W to jungle"));
                clearMenu.Add("useWjHP", new Slider("Minimum HP% to use W to jungle", 20, 1));
                clearMenu.Add("useEj", new CheckBox("Use E to jungle"));

                drawMenu = config.AddSubMenu("Drawings", "Drawings");
                drawMenu.Add("disableDraw", new CheckBox("Disable all drawings"));
                drawMenu.Add("drawQ", new CheckBox("Q range"));
                drawMenu.Add("drawW", new CheckBox("W range"));
                drawMenu.Add("width", new Slider("Drawings width", 2, 1, 5));
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

            if (castW && CommonUtilities.Player.HealthPercent >= wHealth && !IsBurning() && target.IsValidTarget(400))
            {
                w.Cast();
            }
            else if (castW && IsBurning() && !FoundEnemies(450))
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