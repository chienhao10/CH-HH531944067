using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using AutoSharp.Auto;
using AutoSharp.Utils;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using EloBuddy.SDK.Menu;
using EloBuddy;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK;

// ReSharper disable ObjectCreationAsStatement

namespace AutoSharp
{
    class Program
    {
        public static LeagueSharp.Common.Utility.Map.MapType Map;
        public static Menu Config, options, randomizer;
        private static bool _loaded = false;

        public static void Init()
        {
            Orbwalker.DisableMovement = true;

            Map = LeagueSharp.Common.Utility.Map.GetMap().Type;
            Config = MainMenu.AddMenu("AutoSharp: " + ObjectManager.Player.ChampionName, "autosharp." + ObjectManager.Player.ChampionName);

            Config.Add("autosharp.mode", new ComboBox("Mode", 0, "AUTO", "SBTW")).OnValueChange +=
                (sender, args) =>
                {
                    if (Config["autosharp.mode"].Cast<ComboBox>().CurrentValue == 0)
                    {
                        Autoplay.Load();
                    }
                    else
                    {
                        Autoplay.Unload();
                        //Orbwalker.MoveTo(Game.CursorPos);
                    }
                };

            Config.Add("autosharp.humanizer", new Slider("Humanize Movement by ", new Random().Next(125, 350), 125, 350));
            Config.Add("autosharp.quit", new CheckBox("Quit after Game End"));
            Config.Add("autosharp.shop", new CheckBox("AutoShop?"));
            Config.Add("autosharp.disablesr", new CheckBox("Disable for Summoners Rift?"));

            options = Config.AddSubMenu("Options: ", "autosharp.options");
            options.Add("autosharp.options.healup", new CheckBox("Take Heals?"));
            options.Add("onlyfarm", new CheckBox("Only Farm", false));
            if (Map == LeagueSharp.Common.Utility.Map.MapType.SummonersRift)
            {
                options.Add("recallhp", new Slider("Recall if Health% <", 30, 0, 100));
            }

            randomizer = Config.AddSubMenu("Randomizer", "autosharp.randomizer");
            //var orbwalker = Config.AddSubMenu(new Menu("Orbwalker", "autosharp.orbwalker"));
            randomizer.Add("autosharp.randomizer.minrand", new Slider("Min Rand By", 0, 0, 90));
            randomizer.Add("autosharp.randomizer.maxrand", new Slider("Max Rand By", 100, 100, 300));
            randomizer.Add("autosharp.randomizer.playdefensive", new CheckBox("Play Defensive?"));
            randomizer.Add("autosharp.randomizer.auto", new CheckBox("Auto-Adjust? (ALPHA)"));

            new PluginLoader();

            Cache.Load();
            Game.OnUpdate += Positioning.OnUpdate;
            Autoplay.Load();
            Game.OnEnd += OnEnd;
            Player.OnIssueOrder += AntiShrooms;
            Game.OnUpdate += AntiShrooms2;
            Spellbook.OnCastSpell += OnCastSpell;
            Obj_AI_Base.OnDamage += OnDamage;


            LeagueSharp.Common.Utility.DelayAction.Add(
                    new Random().Next(1000, 10000), () =>
                    {
                        var _autoLevel = new Utils.AutoLevel(Utils.AutoLevel.GetSequenceFromDb());
                        _autoLevel.Enable();
                        LeagueSharp.Common.AutoLevel.Enable();
                        Console.WriteLine("AutoLevel Init Success!");
                    });
        }

        public static int getMinRand
        {
            get { return getSliderItem(randomizer, "autosharp.randomizer.minrand"); }
        }

        public static int getMaxRand
        {
            get { return getSliderItem(randomizer, "autosharp.randomizer.maxrand"); }
        }

        public static bool getDefen
        {
            get { return getCheckBoxItem(randomizer, "autosharp.randomizer.playdefensive"); }
        }

        //public static object Orbwalker { get; internal set; }

        public static void OnDamage(AttackableUnit sender, AttackableUnitDamageEventArgs args)
        {
            if (sender == null) return;
            if (args.Target.NetworkId == ObjectManager.Player.NetworkId && (sender is Obj_AI_Turret || sender is Obj_AI_Minion))
            {
                Orbwalker.MoveTo(Heroes.Player.Position.LSExtend(Wizard.GetFarthestMinion().Position, 500).RandomizePosition());
            }
        }

        private static void AntiShrooms2(EventArgs args)
        {
            if (Map == LeagueSharp.Common.Utility.Map.MapType.SummonersRift && !Heroes.Player.InFountain() &&
                Heroes.Player.HealthPercent < getSliderItem(options, "recallhp"))
            {
                if (Heroes.Player.HealthPercent > 0 && Heroes.Player.CountEnemiesInRange(1800) == 0 &&
                    !Turrets.EnemyTurrets.Any(t => t.LSDistance(Heroes.Player) < 950) &&
                    !Minions.EnemyMinions.Any(m => m.LSDistance(Heroes.Player) < 950))
                {
                    Orbwalker.ActiveModesFlags = Orbwalker.ActiveModes.None;
                    if (!Heroes.Player.HasBuff("Recall"))
                    {
                        Heroes.Player.Spellbook.CastSpell(SpellSlot.Recall);
                    }
                }
            }

            var turretNearTargetPosition =
                    Turrets.EnemyTurrets.FirstOrDefault(t => t.LSDistance(Heroes.Player.ServerPosition) < 950);
            if (turretNearTargetPosition != null && turretNearTargetPosition.CountNearbyAllyMinions(950) < 3)
            {
                Orbwalker.MoveTo(Heroes.Player.Position.LSExtend(HeadQuarters.AllyHQ.Position, 950));
            }
        }

        private static void OnCastSpell(Spellbook sender, SpellbookCastSpellEventArgs args)
        {
            if (sender.Owner.IsMe)
            {
                if (sender.Owner.IsDead)
                {
                    args.Process = false;
                    return;
                }
                if (Map == LeagueSharp.Common.Utility.Map.MapType.SummonersRift)
                {
                    if (getCheckBoxItem(options, "onlyfarm") && args.Target.IsValid<AIHeroClient>() &&
                        args.Target.IsEnemy)
                    {
                        args.Process = false;
                        return;
                    }
                    if (Heroes.Player.InFountain() && args.Slot == SpellSlot.Recall)
                    {
                        args.Process = false;
                        return;
                    }
                    if (Heroes.Player.HasBuff("Recall"))
                    {
                        args.Process = false;
                        return;
                    }
                }
                if (Heroes.Player.UnderTurret(true) && args.Target.IsValid<AIHeroClient>())
                {
                    args.Process = false;
                    return;
                }
            }
        }

        private static void OnEnd(GameEndEventArgs args)
        {
            if (getCheckBoxItem(Config, "autosharp.quit"))
            {
                Thread.Sleep(30000);
                Game.QuitGame();
            }
        }

        public static void AntiShrooms(Obj_AI_Base sender, PlayerIssueOrderEventArgs args)
        {
            if (sender != null && sender.IsMe)
            {
                if (sender.IsDead)
                {
                    args.Process = false;
                    return;
                }
                var turret = Turrets.ClosestEnemyTurret;
                if (Map == LeagueSharp.Common.Utility.Map.MapType.SummonersRift && Heroes.Player.HasBuff("Recall") && Heroes.Player.LSCountEnemiesInRange(1800) == 0 &&
                    turret.LSDistance(Heroes.Player) > 950 && !Minions.EnemyMinions.Any(m => m.LSDistance(Heroes.Player) < 950))
                {
                    args.Process = false;
                    return;
                }

                if (args.Order == GameObjectOrder.MoveTo)
                {
                    if (args.TargetPosition.IsZero)
                    {
                        args.Process = false;
                        return;
                    }
                    if (!args.TargetPosition.IsValid())
                    {
                        args.Process = false;
                        return;
                    }
                    if (Map == LeagueSharp.Common.Utility.Map.MapType.SummonersRift && Heroes.Player.InFountain() &&
                        Heroes.Player.HealthPercent < 100)
                    {
                        args.Process = false;
                        return;
                    }
                    if (turret != null && turret.LSDistance(args.TargetPosition) < 950 &&
                        turret.CountNearbyAllyMinions(950) < 3)
                    {
                        args.Process = false;
                        return;
                    }
                }

                #region BlockAttack

                if (args.Target != null && args.Order == GameObjectOrder.AttackUnit || args.Order == GameObjectOrder.AttackTo)
                {
                    if (getCheckBoxItem(options, "onlyfarm") && args.Target.IsValid<AIHeroClient>())
                    {
                        args.Process = false;
                        return;
                    }
                    if (args.Target.IsValid<AIHeroClient>())
                    {
                        if (Minions.AllyMinions.Count(m => m.LSDistance(Heroes.Player) < 900) <
                            Minions.EnemyMinions.Count(m => m.LSDistance(Heroes.Player) < 900))
                        {
                            args.Process = false;
                            return;
                        }
                        if (((AIHeroClient)args.Target).UnderTurret(true))
                        {
                            args.Process = false;
                            return;
                        }
                    }
                    if (Heroes.Player.UnderTurret(true) && args.Target.IsValid<AIHeroClient>())
                    {
                        args.Process = false;
                        return;
                    }
                    if (turret != null && turret.LSDistance(ObjectManager.Player) < 950 && turret.CountNearbyAllyMinions(950) < 3)
                    {
                        args.Process = false;
                        return;
                    }
                    if (Heroes.Player.HealthPercent < getSliderItem(options, "recallhp"))
                    {
                        args.Process = false;
                        return;
                    }
                }

                #endregion
            }
            if (sender != null && args.Target != null && args.Target.IsMe)
            {
                if (sender is Obj_AI_Turret || sender is Obj_AI_Minion)
                {
                    var minion = Wizard.GetClosestAllyMinion();
                    if (minion != null)
                    {
                        Orbwalker.MoveTo(Heroes.Player.Position.LSExtend(Wizard.GetClosestAllyMinion().Position, Heroes.Player.LSDistance(minion) + 100));
                    }
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

        public static void Main()
        {
            Game.OnUpdate += AdvancedLoading;
        }

        private static void AdvancedLoading(EventArgs args)
        {
            if (!_loaded)
            {
                if (ObjectManager.Player.Gold > 0)
                {
                    _loaded = true;
                    LeagueSharp.Common.Utility.DelayAction.Add(new Random().Next(1000, 2000), Init);
                }
            }
        }
    }
}
