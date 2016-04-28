using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using DZLib.Modules;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using iKalistaReborn.Modules;
using iKalistaReborn.Utils;
using LeagueSharp.Common;

namespace iKalistaReborn
{
    internal class Kalista
    {
        public static Menu Menu;

        /// <summary>
        ///     The Modules
        /// </summary>
        public static readonly List<IModule> Modules = new List<IModule>
        {
            new AutoRendModule(),
            new JungleStealModule(),
            new AutoEModule(),
            new AutoELeavingModule()
        };

        public static Dictionary<string, string> JungleMinions = new Dictionary<string, string>
        {
            {"SRU_Baron", "Baron"},
            {"SRU_Dragon", "Dragon"},
            {"SRU_Blue", "Blue Buff"},
            {"SRU_Red", "Red Buff"},
            {"SRU_Crab", "Crab"},
            {"SRU_Gromp", "Gromp"},
            {"SRU_Razorbeak", "Wraiths"},
            {"SRU_Murkwolf", "Wolves"},
            {"SRU_Krug", "Krug"}
        };

        public static Menu comboMenu, mixedMenu, laneclearMenu, jungleStealMenu, miscMenu, drawingMenu;

        public Kalista()
        {
            CreateMenu();
            LoadModules();
            Game.OnUpdate += OnUpdate;
            Drawing.OnDraw += OnDraw;
            Obj_AI_Base.OnProcessSpellCast += OnProcessSpell;
            Spellbook.OnCastSpell += (sender, args) =>
            {
                if (sender.Owner.IsMe && args.Slot == SpellSlot.Q && ObjectManager.Player.IsDashing())
                {
                    args.Process = false;
                }
            };
            Orbwalker.OnUnkillableMinion += (Obj_AI_Base target, Orbwalker.UnkillableMinionArgs args) =>
            {
                var killableMinion = target as Obj_AI_Base;
                if (killableMinion == null || !SpellManager.Spell[SpellSlot.E].IsReady() || ObjectManager.Player.HasBuff("summonerexhaust") || !killableMinion.HasRendBuff())
                {
                    return;
                }

                if (getCheckBoxItem(laneclearMenu, "com.ikalista.laneclear.useEUnkillable") &&
                    killableMinion.IsMobKillable())
                {
                    SpellManager.Spell[SpellSlot.E].Cast();
                }
            };
            Orbwalker.OnPreAttack += (AttackableUnit target, Orbwalker.PreAttackArgs args) =>
            {
                if (!getCheckBoxItem(miscMenu, "com.ikalista.misc.forceW")) return;

                target = HeroManager.Enemies.FirstOrDefault(x => ObjectManager.Player.Distance(x) <= 600 && x.HasBuff("kalistacoopstrikemarkally"));
                if (target != null)
                {
                    Orbwalker.ForcedTarget = target;
                }
            };
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

        /// <summary>
        ///     This is where jeff creates his first Menu in a long time
        /// </summary>
        private void CreateMenu()
        {
            Menu = MainMenu.AddMenu("iKalista: Reborn", "com.ikalista");

            comboMenu = Menu.AddSubMenu("iKalista: Reborn - Combo", "com.ikalista.combo");
            comboMenu.Add("com.ikalista.combo.useQ", new CheckBox("Use Q"));
            comboMenu.Add("com.ikalista.combo.useE", new CheckBox("Use E"));
            comboMenu.Add("com.ikalista.combo.stacks", new Slider("Rend at X stacks", 10, 1, 20));
            comboMenu.Add("com.ikalista.combo.eLeaving", new CheckBox("Use E Leaving", true));
            comboMenu.Add("com.ikalista.combo.ePercent", new Slider("Min Percent for E Leaving", 50, 10, 100));
            comboMenu.Add("com.ikalista.combo.saveMana", new CheckBox("Save Mana for E"));
            comboMenu.Add("com.ikalista.combo.saveAlly", new CheckBox("Save Ally With R"));
            comboMenu.Add("com.ikalista.combo.balista", new CheckBox("Use Balista", true));
            comboMenu.Add("com.ikalista.combo.autoE", new CheckBox("Auto E Minion > Champion"));
            comboMenu.Add("com.ikalista.combo.orbwalkMinions", new CheckBox("Orbwalk Minions in combo"));
            comboMenu.Add("com.ikalista.combo.allyPercent", new Slider("Min Health % for Ally", 20, 10));

            mixedMenu = Menu.AddSubMenu("iKalista: Reborn - Mixed", "com.ikalista.mixed");
            mixedMenu.Add("com.ikalista.mixed.useQ", new CheckBox("Use Q"));
            mixedMenu.Add("com.ikalista.mixed.useE", new CheckBox("Use E"));
            mixedMenu.Add("com.ikalista.mixed.stacks", new Slider("Rend at X stacks", 10, 1, 20));

            laneclearMenu = Menu.AddSubMenu("iKalista: Reborn - Laneclear", "com.ikalista.laneclear");
            laneclearMenu.Add("com.ikalista.laneclear.useQ", new CheckBox("Use Q"));
            laneclearMenu.Add("com.ikalista.laneclear.qMinions", new Slider("Min Minions for Q", 3, 1, 10));
            laneclearMenu.Add("com.ikalista.laneclear.useE", new CheckBox("Use E"));
            laneclearMenu.Add("com.ikalista.laneclear.eMinions", new Slider("Min Minions for E", 5, 1, 10));
            laneclearMenu.Add("com.ikalista.laneclear.useEUnkillable", new CheckBox("E Unkillable Minions"));
            laneclearMenu.Add("com.ikalista.laneclear.eSiege", new CheckBox("Auto E Siege Minions", true));

            jungleStealMenu = Menu.AddSubMenu("iKalista: Reborn - Jungle Steal", "com.ikalista.jungleSteal");
            jungleStealMenu.Add("com.ikalista.jungleSteal.enabled", new CheckBox("Use Rend To Steal Jungle Minions", true));
            foreach (var minion in JungleMinions)
            {
                jungleStealMenu.Add(minion.Key, new CheckBox(minion.Value, true));
            }
            
            miscMenu = Menu.AddSubMenu("iKalista: Reborn - Misc", "com.ikalista.Misc");
            miscMenu.Add("com.ikalista.misc.forceW", new CheckBox("Focus Enemy With W"));

            drawingMenu = Menu.AddSubMenu("iKalista: Reborn - Drawing", "com.ikalista.drawing");
            drawingMenu.Add("com.ikalista.drawing.spellRanges", new CheckBox("Draw Spell Ranges"));
            drawingMenu.Add("com.ikalista.drawing.eDamage", new CheckBox("Draw E Damage"));//.SetValue(new Circle(true, Color.DarkOliveGreen)));
            drawingMenu.Add("com.ikalista.drawing.damagePercent", new CheckBox("Draw Percent Damage"));//.SetValue(new Circle(true, Color.DarkOliveGreen)));
        }

        private void LoadModules()
        {
            foreach (var module in Modules.Where(x => x.ShouldGetExecuted()))
            {
                try
                {
                    module.OnLoad();
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error loading module: " + module.GetName() + " Exception: " + e);
                }
            }
        }

        /// <summary>
        ///     My names definatly jeffery.
        /// </summary>
        /// <param name="args">even more gay</param>
        private void OnDraw(EventArgs args)
        {
            if (getCheckBoxItem(drawingMenu, "com.ikalista.drawing.spellRanges"))
            {
                foreach (var spell in SpellManager.Spell.Values)
                {
                    Render.Circle.DrawCircle(ObjectManager.Player.Position, spell.Range, Color.DarkOliveGreen);
                }
            }

            if (getCheckBoxItem(drawingMenu, "com.ikalista.drawing.damagePercent"))
            {
                foreach (var source in HeroManager.Enemies.Where(x => ObjectManager.Player.Distance(x) <= 2000f && !x.IsDead))
                {
                    var currentPercentage = Math.Round(Helper.GetRendDamage(source) * 100 / source.GetHealthWithShield(), 2);

                    Drawing.DrawText(
                        Drawing.WorldToScreen(source.Position)[0],
                        Drawing.WorldToScreen(source.Position)[1],
                        currentPercentage >= 100
                            ? Color.DarkOliveGreen
                            : Color.White,
                        currentPercentage >= 100
                            ? "Killable With E"
                            : "Current Damage: " + currentPercentage + "%");
                }
            }
        }

        /// <summary>
        ///     The on process spell function
        /// </summary>
        /// <param name="sender">
        ///     The Spell Sender
        /// </param>
        /// <param name="args">
        ///     The Arguments
        /// </param>
        private void OnProcessSpell(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.IsMe && args.SData.Name == "KalistaExpungeWrapper")
            {
                Orbwalker.ResetAutoAttack();
            }

            if (sender.Type == GameObjectType.obj_AI_Base && sender.IsEnemy && args.Target != null &&
                getCheckBoxItem(comboMenu, "com.ikalista.combo.saveAlly"))
            {
                var soulboundhero =
                    HeroManager.Allies.FirstOrDefault(
                        hero =>
                            hero.HasBuff("kalistacoopstrikeally") && args.Target.NetworkId == hero.NetworkId);

                if (soulboundhero != null &&
                    soulboundhero.HealthPercent < getSliderItem(comboMenu, "com.ikalista.combo.allyPercent"))
                {
                    SpellManager.Spell[SpellSlot.R].Cast();
                }
            }
        }

        /// <summary>
        ///     My Names Jeff
        /// </summary>
        /// <param name="args">gay</param>
        private void OnUpdate(EventArgs args)
        {
            Orbwalker.ForcedTarget = null;

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                OnCombo();
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass))
            {
                OnMixed();
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear) ||
                Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear))
            {
                OnLaneclear();
            }

            //BALISTA
            if (getCheckBoxItem(comboMenu, "com.ikalista.combo.balista") && SpellManager.Spell[SpellSlot.R].IsReady())
            {
                var soulboundhero = HeroManager.Allies.FirstOrDefault(x => x.HasBuff("kalistacoopstrikeally"));
                if (soulboundhero?.ChampionName == "Blitzcrank")
                {
                    foreach (
                        var unit in
                            HeroManager.Enemies
                                .Where(
                                    h => h.IsHPBarRendered &&
                                         h.Distance(ObjectManager.Player.ServerPosition) > 700 &&
                                         h.Distance(ObjectManager.Player.ServerPosition) < 1400)
                        )
                    {
                        if (unit.HasBuff("rocketgrab2"))
                        {
                            SpellManager.Spell[SpellSlot.R].Cast();
                        }
                    }
                }
            }

            foreach (var module in Modules.Where(x => x.ShouldGetExecuted()))
            {
                module.OnExecute();
            }
        }

        private void OnCombo()
        {
            if (getCheckBoxItem(comboMenu, "com.ikalista.combo.orbwalkMinions"))
            {
                var targets =
                    HeroManager.Enemies.Where(
                        x =>
                            ObjectManager.Player.Distance(x) <= SpellManager.Spell[SpellSlot.E].Range*2 &&
                            x.IsValidTarget(SpellManager.Spell[SpellSlot.E].Range*2));

                if (targets.Count(x => ObjectManager.Player.Distance(x) < Orbwalking.GetRealAutoAttackRange(x)) == 0)
                {
                    var minion =
                        ObjectManager.Get<Obj_AI_Minion>()
                            .Where(
                                x =>
                                    ObjectManager.Player.Distance(x) <= Orbwalking.GetRealAutoAttackRange(x) &&
                                    x.IsEnemy)
                            .OrderBy(x => x.Health)
                            .FirstOrDefault();
                    if (minion != null)
                    {
                        Orbwalker.ForcedTarget = minion;
                    }
                }
            }

            if (!SpellManager.Spell[SpellSlot.Q].IsReady() || !getCheckBoxItem(comboMenu, "com.ikalista.combo.useQ"))
                return;

            if (getCheckBoxItem(comboMenu, "com.ikalista.combo.saveMana") && ObjectManager.Player.Mana < SpellManager.Spell[SpellSlot.E].ManaCost * 2)
            {
                return;
            }

            var target = TargetSelector.GetTarget(SpellManager.Spell[SpellSlot.Q].Range, DamageType.Physical);

            var prediction = SpellManager.Spell[SpellSlot.Q].GetPrediction(target);

            if (prediction.Hitchance >= HitChance.High && target.IsValidTarget(SpellManager.Spell[SpellSlot.Q].Range) &&
                !ObjectManager.Player.IsDashing() && !Orbwalker.IsAutoAttacking)
            {
                SpellManager.Spell[SpellSlot.Q].Cast(prediction.CastPosition);
            }
        }

        private void OnMixed()
        {
            if (SpellManager.Spell[SpellSlot.Q].IsReady() && getCheckBoxItem(mixedMenu, "com.ikalista.mixed.useQ"))
            {
                var target = TargetSelector.GetTarget(SpellManager.Spell[SpellSlot.Q].Range, DamageType.Physical);
                var prediction = SpellManager.Spell[SpellSlot.Q].GetPrediction(target);
                if (prediction.Hitchance >= HitChance.High &&
                    target.IsValidTarget(SpellManager.Spell[SpellSlot.Q].Range))
                {
                    SpellManager.Spell[SpellSlot.Q].Cast(prediction.CastPosition);
                }
            }

            if (SpellManager.Spell[SpellSlot.E].IsReady() && getCheckBoxItem(mixedMenu, "com.ikalista.mixed.useE"))
            {
                foreach (
                    var source in
                        HeroManager.Enemies.Where(
                            x => x.IsValid && x.HasRendBuff() && SpellManager.Spell[SpellSlot.E].IsInRange(x)))
                {
                    if (source.IsRendKillable() ||
                        source.GetRendBuffCount() >= getSliderItem(mixedMenu, "com.ikalista.mixed.stacks"))
                    {
                        SpellManager.Spell[SpellSlot.E].Cast();
                    }
                }
            }
        }

        private void OnLaneclear()
        {
            if (getCheckBoxItem(laneclearMenu, "com.ikalista.laneclear.useQ"))
            {
                var minions = MinionManager.GetMinions(SpellManager.Spell[SpellSlot.Q].Range).ToList();
                if (minions.Count < 0)
                    return;

                foreach (var minion in minions.Where(x => x.Health <= SpellManager.Spell[SpellSlot.Q].GetDamage(x)))
                {
                    var killableMinions = Helper.GetCollisionMinions(ObjectManager.Player,
                        ObjectManager.Player.ServerPosition.LSExtend(
                            minion.ServerPosition,
                            SpellManager.Spell[SpellSlot.Q].Range))
                        .Count(
                            collisionMinion =>
                                collisionMinion.Health
                                <= ObjectManager.Player.GetSpellDamage(collisionMinion, SpellSlot.Q));

                    if (killableMinions >= getSliderItem(laneclearMenu, "com.ikalista.laneclear.qMinions"))
                    {
                        SpellManager.Spell[SpellSlot.Q].Cast(minion.ServerPosition);
                    }
                }
            }
            if (getCheckBoxItem(laneclearMenu, "com.ikalista.laneclear.useE"))
            {
                var minions = MinionManager.GetMinions(SpellManager.Spell[SpellSlot.E].Range).ToList();
                if (minions.Count < 0)
                    return;

                var siegeMinion = minions.FirstOrDefault(x => x.Name.Contains("siege") && x.IsRendKillable());

                if (getCheckBoxItem(laneclearMenu, "com.ikalista.laneclear.eSiege") && siegeMinion != null)
                {
                    SpellManager.Spell[SpellSlot.E].Cast();
                }

                var count =
                    minions.Count(
                        x => SpellManager.Spell[SpellSlot.E].CanCast(x) && x.IsMobKillable());

                if (count >= getSliderItem(laneclearMenu, "com.ikalista.laneclear.eMinions") &&
                    !ObjectManager.Player.HasBuff("summonerexhaust"))
                {
                    SpellManager.Spell[SpellSlot.E].Cast();
                }
            }
        }
    }
}