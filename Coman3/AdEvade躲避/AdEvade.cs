using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;
using AdEvade.Config;
using AdEvade.Config.Controls;
using AdEvade.Data;
using AdEvade.Data.EvadeSpells;
using AdEvade.Data.Spells;
using AdEvade.Draw;
using AdEvade.Helpers;
using AdEvade.Utils;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu.Values;
using SharpDX;
using CheckBox = EloBuddy.SDK.Menu.Values.CheckBox;
using PositionInfo = AdEvade.Data.PositionInfo;
using SpellData = AdEvade.Data.Spells.SpellData;
using Debug = AdEvade.Draw.Debug;
using MainMenu = EloBuddy.SDK.Menu.MainMenu;
using Menu = EloBuddy.SDK.Menu.Menu;
using Spell = AdEvade.Data.Spells.Spell;

// ReSharper disable AccessToStaticMemberViaDerivedType

namespace AdEvade
{
    public class AdEvade
    {
        public const string LastUpdate = "3:30PM 24th January 2016";
        public static SpellDetector SpellDetector;
        private static SpellDrawer _spellDrawer;
        //private static EvadeTester _evadeTester;
        private static EvadeSpell _evadeSpell;
        //private static SpellTester _spellTester;
        private static AutoSetPing _autoSetPing;

        public static SpellSlot LastSpellCast;
        public static float LastSpellCastTime = 0;

        public static float LastWindupTime = 0;

        public static float LastTickCount = 0;
        public static float LastStopEvadeTime = 0;

        public static Vector3 LastMovementBlockPos = Vector3.Zero;
        public static float LastMovementBlockTime = 0;

        public static float LastEvadeOrderTime = 0;
        public static float LastIssueOrderGameTime = 0;
        public static float LastIssueOrderTime = 0;
        public static PlayerIssueOrderEventArgs LastIssueOrderArgs = null;

        public static Vector2 LastMoveToPosition = Vector2.Zero;
        public static Vector2 LastMoveToServerPos = Vector2.Zero;
        public static Vector2 LastStopPosition = Vector2.Zero;

        public static DateTime AssemblyLoadTime = DateTime.Now;

        public static bool IsDodging = false;
        public static bool DodgeOnlyDangerous = false;

        public static bool HasGameEnded = false;
        public static bool IsChanneling = false;
        public static Vector2 ChannelPosition = Vector2.Zero;

        public static Data.PositionInfo LastPosInfo;

        public static EvadeCommand LastEvadeCommand = new EvadeCommand
        {
            IsProcessed = true,
            Timestamp = EvadeUtils.TickCount
        };

        public static EvadeCommand LastBlockedUserMoveTo = new EvadeCommand
        {
            IsProcessed = true,
            Timestamp = EvadeUtils.TickCount
        };

        public static float LastDodgingEndTime = 0;

        public static Menu Menu;

        public static float SumCalculationTime = 0;
        public static float NumCalculationTime = 0;
        public static float AvgCalculationTime = 0;
        public AdEvade()
        {
            Load();
        }

        private void Load()
        {
            Loading.OnLoadingComplete += Game_OnGameLoad;
        }

        private void Game_OnGameLoad_Disabled(EventArgs args)
        {
            ConsoleDebug.WriteLineColor("Failed loading AdEvade...", ConsoleColor.Red);
            ConsoleDebug.WriteLine("   Disabled due to needed core update (as of 5.24), please be patient!");
            Chat.Print("<font color='#ff0000'>Failed loading AdEvade...</font>");
            Chat.Print("   Disabled due to needed core update (as of 5.24), please be patient!");
            Menu = MainMenu.AddMenu("AdEvade (已关闭)", "AdEvade", "AdEvade (已关闭)");
            Menu.AddGroupLabel("暂停使用!需要更新核心！");
            Menu.AddLabel("As the latest update has caused issues with getting buffs and sending movement commands\n" +
                          " AdEvade can not be fixed.\n");
            Menu.AddSeparator();
            Menu.AddLabel("请等待更新，等待时可使用其他躲避");
        }

        private void Game_OnGameLoad(EventArgs args)
        {
            ConsoleDebug.WriteLineColor("Loading...", ConsoleColor.Blue, true);

            try
            {
                Menu = MainMenu.AddMenu("Ad躲避", "AdEvade");
                ConsoleDebug.WriteLineColor("   Creating Menu...", ConsoleColor.Yellow, true);
                Menu.AddGroupLabel("AdEvade (EzEvade Port-CH汉化)");
                Menu.AddLabel("我游戏是英文版，技能如果是英文的话就看后面的快捷键就知道是啥技能了");
                Menu.AddLabel("Please report any bugs or anything you think is a ");
                Menu.AddLabel("problem / issue, on the GitHub Issues Section, or with a reply to the AdEvade forum thread.");
                Menu.Add("OpenGithub", new CheckBox("Open Github's Issues Section in browser", false)).OnValueChange +=
                    delegate(ValueBase<bool> sender, ValueBase<bool>.ValueChangeArgs changeArgs)
                    {
                        if (changeArgs.OldValue == false && changeArgs.NewValue)
                        {
                            sender.CurrentValue = false;
                            Process.Start(@"https://github.com/coman3/EloBuddy.Addons/issues");
                        }
                    };
                Menu.AddLabel("All Credit for the actual evading (Movement and detection) in this assembly ");
                Menu.AddLabel("goes to the creator of EzEvade.");
                Menu.AddLabel("AD躲避由CH汉化.");
                Menu.AddSeparator(100);

                Menu.AddLabel("Created By: Coman3");
                Menu.AddLabel("     Github: https://github.com/coman3/");
                Menu.Add("OpenGithubComan3", new CheckBox("Open Coman3's Github in Browser", false)).OnValueChange +=
                   delegate (ValueBase<bool> sender, ValueBase<bool>.ValueChangeArgs changeArgs)
                   {
                       if (changeArgs.OldValue == false && changeArgs.NewValue)
                       {
                           sender.CurrentValue = false;
                           Process.Start(@"https://github.com/coman3/");
                       }
                Menu.AddLabel("Last Update: " + LastUpdate);
                   };

                Menu.AddLabel("CH汉化地址");
                Menu.AddLabel("     Github: http://github.com/chienhao10/Translate-Project");
                Menu.Add("打开地址", new CheckBox("在浏览器进入汉化地址", false)).OnValueChange +=
                   delegate (ValueBase<bool> sender, ValueBase<bool>.ValueChangeArgs changeArgs)
                   {
                       if (changeArgs.OldValue == false && changeArgs.NewValue)
                       {
                           sender.CurrentValue = false;
                           Process.Start(@"http://github.com/chienhao10/Translate-Project");
                       }
                   };


                Menu mainMenu = Menu.AddSubMenu("核心", "核心");
                mainMenu.Add(new DynamicKeyBind(ConfigValue.DodgeSkillShots, "躲避指向性技能", true, KeyBind.BindTypes.PressToggle, 'K'));
                mainMenu.Add(new DynamicKeyBind(ConfigValue.ActivateEvadeSpells, "使用技能躲避", true, KeyBind.BindTypes.PressToggle, 'K'));
                mainMenu.Add(new DynamicCheckBox(ConfigDataType.Data, ConfigValue.OnlyDodgeDangerous, "只躲避危险的", false));
                mainMenu.Add(new DynamicCheckBox(ConfigDataType.Data, ConfigValue.DodgeFowSpells, "探测战争迷雾的指向性技能", true));
                mainMenu.Add(new DynamicCheckBox(ConfigDataType.Data, ConfigValue.DodgeCircularSpells, "躲避圈形指向性技能", true));
                mainMenu.AddSeparator();
                mainMenu.Add(new DynamicCheckBox(ConfigDataType.Data, ConfigValue.DodgeDangerousKeysEnabled, "开启只躲避危险的热键", false));
                mainMenu.Add(new DynamicKeyBind(ConfigValue.DodgeDangerousKey1, "只躲避危险的", false, KeyBind.BindTypes.HoldActive, 32));
                mainMenu.Add(new DynamicKeyBind(ConfigValue.DodgeDangerousKey2, "只躲避危险的 2", false, KeyBind.BindTypes.HoldActive, 'V'));
                mainMenu.AddSeparator();
                mainMenu.Add(new DynamicComboBox(ConfigDataType.Data, ConfigValue.EvadeMode, "躲避模式（快速/平滑（默认）/非常平滑/人性化）", 1, Enum.GetNames(typeof(EvadeMode))));

                ConsoleDebug.WriteLineColor("       Detecting Spells...", ConsoleColor.Yellow, true);
                SpellDetector = new SpellDetector(Menu);
                _evadeSpell = new EvadeSpell(Menu);

                ConsoleDebug.WriteLineColor("       Adding Humanizer and Miscellaneous Menus...", ConsoleColor.Yellow, true);
                Menu miscMenu = Menu.AddSubMenu("杂项设置", "杂项设置");
                miscMenu.Add(new DynamicCheckBox(ConfigDataType.Data, ConfigValue.HighPrecision, "增强躲避精密度", false));
                miscMenu.Add(new DynamicCheckBox(ConfigDataType.Data, ConfigValue.RecalculatePath, "重新计算路径", true));
                miscMenu.Add(new DynamicCheckBox(ConfigDataType.Data, ConfigValue.ContinueMovement, "继续最后一次的移动", true));
                miscMenu.Add(new DynamicCheckBox(ConfigDataType.Data, ConfigValue.CalculateWindupDelay, "计算延迟", true));
                miscMenu.Add(new DynamicCheckBox(ConfigDataType.Data, ConfigValue.CheckSpellCollision, "检查技能弹道阻挡", false));
                miscMenu.Add(new DynamicCheckBox(ConfigDataType.Data, ConfigValue.PreventDodgingUnderTower, "防止塔下躲避", false));
                miscMenu.Add(new DynamicCheckBox(ConfigDataType.Data, ConfigValue.PreventDodgingNearEnemy, "防止在敌人附近躲避", false));
                miscMenu.Add(new DynamicCheckBox(ConfigDataType.Data, ConfigValue.AdvancedSpellDetection, "进阶技能探测", false));
                miscMenu.Add(new DynamicSlider(ConfigDataType.Data, ConfigValue.ExtraDetectionRange, "额外检测距离", 1000, 500, 5000));
                //TODO: Add Reset
                //miscMenu.AddSeparator(100);
                //miscMenu.AddGroupLabel("Reset");
                //miscMenu.Add("ResetConfig", new DynamicCheckBox(ConfigDataType.Data, "ResetConfig", "Reset Properties", false).CheckBox);

                Menu fastEvadeMenu = Menu.AddSubMenu("快速躲避", "快速躲避");
                fastEvadeMenu.Add(new DynamicCheckBox(ConfigDataType.Data, ConfigValue.FastMovementBlock, "阻挡快速移动", false));
                fastEvadeMenu.Add(new DynamicSlider(ConfigDataType.Data, ConfigValue.FastEvadeActivationTime, "快速移动激活时间", 65, 0, 500));
                fastEvadeMenu.Add(new DynamicSlider(ConfigDataType.Data, ConfigValue.SpellActivationTime, "技能激活时间", 200, 0, 1000));
                fastEvadeMenu.Add(new DynamicSlider(ConfigDataType.Data, ConfigValue.RejectMinDistance, "体积碰撞距离缓冲", 10, 0, 100));

                Menu limiterMenu = Menu.AddSubMenu("人性化", "限制器");
                limiterMenu.Add(new DynamicCheckBox(ConfigDataType.Data, ConfigValue.ClickOnlyOnce, "只点击一次", true));
                limiterMenu.Add(new DynamicCheckBox(ConfigDataType.Data, ConfigValue.EnableEvadeDistance, "延长躲避", false));
                limiterMenu.Add(new DynamicSlider(ConfigDataType.Data, ConfigValue.TickLimiter, "点击限制", 100, 0, 500));
                limiterMenu.Add(new DynamicSlider(ConfigDataType.Data, ConfigValue.SpellDetectionTime, "技能探测时间", 0, 0, 1000));
                limiterMenu.Add(new DynamicSlider(ConfigDataType.Data, ConfigValue.ReactionTime, "反应时间", 0, 0, 500));
                limiterMenu.Add(new DynamicSlider(ConfigDataType.Data, ConfigValue.DodgeInterval, "躲避曲线", 0, 0, 2000));

                Menu randomizerMenu = Menu.AddSubMenu("随机器", "随机器");
                randomizerMenu.Add(new DynamicCheckBox(ConfigDataType.Data, ConfigValue.EnableRandomizer, "开启", false));
                randomizerMenu.Add(new DynamicCheckBox(ConfigDataType.Data, ConfigValue.DrawBlockedRandomizerSpells, "显示阻挡的技能", true));
                randomizerMenu.Add(new DynamicSlider(ConfigDataType.Data, ConfigValue.RandomizerPercentage, "精确度", 80, 0, 100));
                randomizerMenu.Add(new DynamicComboBox(ConfigDataType.Data, ConfigValue.RandomizerMaxDangerLevel, "最大危险等级（低/正常/高（默认）/极端）", (int)SpellDangerLevel.High, Enum.GetNames(typeof(SpellDangerLevel))));

                Menu bufferMenu = Menu.AddSubMenu("高级人性化", "额外加成");
                bufferMenu.Add(new DynamicSlider(ConfigDataType.Data, ConfigValue.ExtraPingBuffer, "额外警告", 65, 0, 200));
                bufferMenu.Add(new DynamicSlider(ConfigDataType.Data, ConfigValue.ExtraCpaDistance, "额外体积碰撞距离", 10, 0, 150));
                bufferMenu.Add(new DynamicSlider(ConfigDataType.Data, ConfigValue.ExtraSpellRadius, "额外技能半径", 0, 0, 100));
                bufferMenu.Add(new DynamicSlider(ConfigDataType.Data, ConfigValue.ExtraEvadeDistance, "额外躲避距离", 100, 0, 300));
                //bufferMenu.Add(ConfigValue.ExtraSpellRadius.Name(), new DynamicSlider(ConfigDataType.Data, ConfigValue.ExtraSpellRadius, "Extra Avoid Distance", 50, 0, 300).Slider);
                bufferMenu.Add(new DynamicSlider(ConfigDataType.Data, ConfigValue.MinimumComfortZone, "英雄最短路程", 550, 0, 1000));

                Menu debugMenu = Menu.AddSubMenu("调试", "调试菜单");

                debugMenu.AddGroupLabel("调试");
                debugMenu.Add(new DynamicCheckBox(ConfigDataType.Data, ConfigValue.ShowDebugInfo, "显示调试资讯 (Console)", false)).OnValueChange +=
                    (sender, changeArgs) =>
                    {
                        ConsoleDebug.Enabled = sender.CurrentValue;
                    };

                debugMenu.AddSeparator();

                _spellDrawer = new SpellDrawer(Menu);

                ConsoleDebug.WriteLineColor("   Hooking Events...", ConsoleColor.Yellow, true);
                Player.OnIssueOrder += Game_OnIssueOrder;
                Spellbook.OnCastSpell += Game_OnCastSpell;
                Game.OnUpdate += Game_OnGameUpdate;

                ConsoleDebug.WriteLineColor("   Loading Spells...", ConsoleColor.Yellow, true);
                SpellDetector.LoadSpellDictionary();
                SpellDetector.InitChannelSpells();

                AIHeroClient.OnProcessSpellCast += Game_OnProcessSpell;

                Game.OnEnd += Game_OnGameEnd;
                SpellDetector.OnProcessDetectedSpells += SpellDetector_OnProcessDetectedSpells;
                Orbwalker.OnPreAttack += Orbwalking_BeforeAttack;

                ConsoleDebug.WriteLineColor("   Setting Loaded Presets Values...", ConsoleColor.Yellow, true);

            }
            catch (Exception e)
            {
                ConsoleDebug.WriteLineColor(e, ConsoleColor.Red, true);
            }
            ConsoleDebug.WriteLineColor("Successfully Loaded!", ConsoleColor.Green, true);
        }

        private void OnEvadeModeChange(ValueBase<int> sender, ValueBase<int>.ValueChangeArgs changeArgs)
        {
            //TODO: Change
            var mode = sender.DisplayName;

            if (mode == "Very Smooth")
            {
                ConfigValue.FastEvadeActivationTime.SetInt(0);
                ConfigValue.RejectMinDistance.SetInt(0);
                ConfigValue.ExtraCpaDistance.SetInt(0);
                ConfigValue.ExtraPingBuffer.SetInt(40);
            }
            else if (mode == "Smooth")
            {
                ConfigValue.FastEvadeActivationTime.SetInt(65);
                ConfigValue.RejectMinDistance.SetInt(10);
                ConfigValue.ExtraCpaDistance.SetInt(10);
                ConfigValue.ExtraPingBuffer.SetInt(65);
            }
        }

        private void Game_OnGameEnd(GameEndEventArgs args)
        {
            HasGameEnded = true;
        }

        private void Game_OnCastSpell(Spellbook spellbook, SpellbookCastSpellEventArgs args)
        {
            if (!spellbook.Owner.IsMe)
                return;

            var sData = spellbook.GetSpell(args.Slot);
            string name;

            if (SpellDetector.ChanneledSpells.TryGetValue(sData.Name, out name))
            {
                //Evade.isChanneling = true;
                //Evade.channelPosition = GameData.HeroInfo.serverPos2D;
                LastStopEvadeTime = EvadeUtils.TickCount + Game.Ping + 100;
            }

            if (EvadeSpell.LastSpellEvadeCommand != null &&
                EvadeSpell.LastSpellEvadeCommand.Timestamp + Game.Ping + 150 > EvadeUtils.TickCount)
            {
                args.Process = false;
            }

            LastSpellCast = args.Slot;
            LastSpellCastTime = EvadeUtils.TickCount;

            //moved from processPacket

            /*if (args.Slot == SpellSlot.Recall)
            {
                lastStopPosition = GameData.MyHero.ServerPosition.To2D();
            }*/

            if (Situation.ShouldDodge())
            {
                if (IsDodging && SpellDetector.Spells.Count() > 0)
                {
                    foreach (KeyValuePair<String, SpellData> entry in SpellDetector.WindupSpells)
                    {
                        SpellData spellData = entry.Value;

                        if (spellData.SpellKey == args.Slot) //check if it's a spell that we should block
                        {
                            args.Process = false;
                            return;
                        }
                    }
                }
            }

            foreach (var evadeSpell in EvadeSpell.EvadeSpells)
            {
                if (evadeSpell.IsItem == false && evadeSpell.SpellKey == args.Slot)
                {
                    if (evadeSpell.EvadeType == EvadeType.Blink
                        || evadeSpell.EvadeType == EvadeType.Dash)
                    {
                        //Block spell cast if flashing/blinking into spells
                        if (args.EndPosition.To2D().CheckDangerousPos(6, true)) //for blink + dash
                        {
                            args.Process = false;
                            return;
                        }

                        if (evadeSpell.EvadeType == EvadeType.Dash)
                        {
                            var extraDelayBuffer = Config.Properties.GetInt(ConfigValue.ExtraPingBuffer);
                            var extraDist = Config.Properties.GetInt(ConfigValue.ExtraCpaDistance);

                            var dashPos = Game.CursorPos.To2D(); //real pos?

                            if (evadeSpell.FixedRange)
                            {
                                var dir = (dashPos - GameData.MyHero.ServerPosition.To2D()).Normalized();
                                dashPos = GameData.MyHero.ServerPosition.To2D() + dir * evadeSpell.Range;
                            }

                            //Draw.RenderObjects.Add(new Draw.RenderPosition(dashPos, 1000));

                            var posInfo = EvadeHelper.CanHeroWalkToPos(dashPos, evadeSpell.Speed,
                                extraDelayBuffer + Game.Ping, extraDist);

                            if (posInfo.PosDangerLevel > 0)
                            {
                                args.Process = false;
                                return;
                            }
                        }

                        LastPosInfo = PositionInfo.SetAllUndodgeable(); //really?

                        if (IsDodging || EvadeUtils.TickCount < LastDodgingEndTime + 500)
                        {
                            EvadeCommand.MoveTo(Game.CursorPos.To2D()); //block moveto
                            LastStopEvadeTime = EvadeUtils.TickCount + Game.Ping + 100;
                        }
                    }
                    return;
                }
            }
        }

        private void Game_OnIssueOrder(Obj_AI_Base hero, PlayerIssueOrderEventArgs args)
        {

            if (!hero.IsMe)
                return;

            if (!Situation.ShouldDodge())
                return;
            //DebugIssueOrders(args);

            if (args.Order == GameObjectOrder.MoveTo)
            {
                //movement block code goes in here
                if (IsDodging && SpellDetector.Spells.Count > 0)
                {
                    ConsoleDebug.WriteLineColor("Issue Order detected while spells exist", ConsoleColor.Yellow);
                    CheckHeroInDanger();

                    LastBlockedUserMoveTo = new EvadeCommand
                    {
                        Order = EvadeOrderCommand.MoveTo,
                        TargetPosition = args.TargetPosition.To2D(),
                        Timestamp = EvadeUtils.TickCount,
                        IsProcessed = false,
                    };

                    args.Process = false; //Block the command
                    ConsoleDebug.WriteLineColor("   Blocked Movement Command", ConsoleColor.Red);
                }
                else
                {
                    var movePos = args.TargetPosition.To2D();
                    var extraDelay = Config.Properties.GetInt(ConfigValue.ExtraPingBuffer);
                    if (EvadeHelper.CheckMovePath(movePos, Game.Ping + extraDelay))
                    {
                        ConsoleDebug.WriteLineColor("Move Path is colliding with spell", ConsoleColor.Yellow);
                        /*if (() Properties.Properties.Data["AllowCrossing"].Cast<CheckBox>().CurrentValue)
                        {
                            var extraDelayBuffer = () Properties.Properties.Data["ExtraPingBuffer"]
                                 + 30;
                            var extraDist = () Properties.Properties.Data["ExtraCPADistance"]
                                 + 10;

                            var tPosInfo = EvadeHelper.CanHeroWalkToPos(movePos, GameData.HeroInfo.moveSpeed, extraDelayBuffer + Game.Ping, extraDist);

                            if (tPosInfo.posDangerLevel == 0)
                            {
                                lastPosInfo = tPosInfo;
                                return;
                            }
                        }*/

                        LastBlockedUserMoveTo = new EvadeCommand
                        {
                            Order = EvadeOrderCommand.MoveTo,
                            TargetPosition = args.TargetPosition.To2D(),
                            Timestamp = EvadeUtils.TickCount,
                            IsProcessed = false,
                        };

                        args.Process = false; //Block the command
                        ConsoleDebug.WriteLineColor("   Blocked Movement Command", ConsoleColor.Red);
                        if (EvadeUtils.TickCount - LastMovementBlockTime < 500 && LastMovementBlockPos.Distance(args.TargetPosition) < 100)
                        {
                            return;
                        }

                        LastMovementBlockPos = args.TargetPosition;
                        LastMovementBlockTime = EvadeUtils.TickCount;

                        var posInfo = EvadeHelper.GetBestPositionMovementBlock(movePos);
                        if (posInfo != null)
                        {
                            EvadeCommand.MoveTo(posInfo.Position);
                        }
                        return;
                    }
                    else
                    {
                        LastBlockedUserMoveTo.IsProcessed = true;
                    }
                }
            }
            else //need more logic
            {
                if (IsDodging)
                {
                    args.Process = false; //Block the command
                    ConsoleDebug.WriteLineColor("   Blocked IssueOrder(" + args.Order + ") Command", ConsoleColor.Red);
                }
                else
                {
                    if (args.Order == GameObjectOrder.AttackUnit)
                    {
                        var target = args.Target;
                        if (target != null && target.GetType() == typeof(Obj_AI_Base))
                        {
                            var baseTarget = (Obj_AI_Base)target;
                            if (baseTarget.IsValid())
                                if (GameData.HeroInfo.ServerPos2D.Distance(baseTarget.ServerPosition.To2D()) > GameData.MyHero.AttackRange + GameData.HeroInfo.BoundingRadius + baseTarget.BoundingRadius)
                                {
                                    var movePos = args.TargetPosition.To2D();
                                    var extraDelay = Config.Properties.GetInt(ConfigValue.ExtraPingBuffer);
                                    if (EvadeHelper.CheckMovePath(movePos, Game.Ping + extraDelay))
                                    {
                                        args.Process = false; //Block the command
                                        ConsoleDebug.WriteLineColor("   Blocked Attack Unit Command", ConsoleColor.Red);
                                        return;
                                    }
                                }
                        }
                    }
                }
            }

            if (args.Process == true)
            {
                LastIssueOrderGameTime = Game.Time * 1000;
                LastIssueOrderTime = EvadeUtils.TickCount;
                LastIssueOrderArgs = args;

                if (args.Order == GameObjectOrder.MoveTo)
                {
                    LastMoveToPosition = args.TargetPosition.To2D();
                    LastMoveToServerPos = GameData.MyHero.ServerPosition.To2D();
                }

                if (args.Order == GameObjectOrder.Stop)
                {
                    LastStopPosition = GameData.MyHero.ServerPosition.To2D();
                }
            }
        }

        private static void DebugIssueOrders(PlayerIssueOrderEventArgs args)
        {
            switch (args.Order)
            {
                case GameObjectOrder.HoldPosition:
                    ConsoleDebug.WriteLineColor("HoldPosition: " + args.TargetPosition, ConsoleColor.Blue);
                    break;
                case GameObjectOrder.MoveTo:
                    ConsoleDebug.WriteLineColor("MoveTo: " + args.TargetPosition, ConsoleColor.Blue);
                    break;
                case GameObjectOrder.AttackUnit:
                    ConsoleDebug.WriteLineColor("AttackUnit: " + args.Target.Name, ConsoleColor.Blue);
                    break;
                case GameObjectOrder.AutoAttackPet:
                    ConsoleDebug.WriteLineColor("AutoAttackPet: " + args.Target.Name, ConsoleColor.Blue);
                    break;
                case GameObjectOrder.AutoAttack:
                    ConsoleDebug.WriteLineColor("AutoAttack: " + args.Target.Name, ConsoleColor.Blue);
                    break;
                case GameObjectOrder.MovePet:
                    ConsoleDebug.WriteLineColor("MovePet: " + args.TargetPosition, ConsoleColor.Blue);
                    break;
                case GameObjectOrder.AttackTo:
                    ConsoleDebug.WriteLineColor("AttackTo: " + args.TargetPosition, ConsoleColor.Blue);
                    break;
                case GameObjectOrder.Stop:
                    ConsoleDebug.WriteLineColor("Stop: " + args.TargetPosition, ConsoleColor.Blue);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            ConsoleDebug.WriteLineColor(NavMesh.GetCollisionFlags(args.TargetPosition), ConsoleColor.DarkMagenta);
        }

        private void Orbwalking_BeforeAttack(AttackableUnit target, Orbwalker.PreAttackArgs args)
        {
            if (IsDodging)
            {
                args.Process = false; //Block orbwalking
                ConsoleDebug.WriteLineColor("Blocked Orbwalk Before Attack", ConsoleColor.Red);
            }
        }

        private void Game_OnProcessSpell(Obj_AI_Base hero, GameObjectProcessSpellCastEventArgs args)
        {
            if (!hero.IsMe)
            {
                return;
            }

            /*if (args.SData.Name.Contains("Recall"))
            {
                var distance = lastStopPosition.Distance(args.Start.To2D());
                float moveTime = 1000 * distance / GameData.MyHero.MoveSpeed;

                ConsoleDebug.WriteLine("Extra dist: " + distance + " Extra Delay: " + moveTime);
            }*/

            string name;
            if (SpellDetector.ChanneledSpells.TryGetValue(args.SData.Name, out name))
            {
                IsChanneling = true;
                ChannelPosition = GameData.MyHero.ServerPosition.To2D();
                ConsoleDebug.WriteLineColor("Channeling...", ConsoleColor.Green);
            }
            if (ConfigValue.CalculateWindupDelay.GetBool())
            {
                var castTime = (hero.Spellbook.CastTime - Game.Time)*1000;

                if (castTime > 0 && !EloBuddy.SDK.Constants.AutoAttacks.IsAutoAttack(args.SData.Name) && Math.Abs(castTime - GameData.MyHero.AttackCastDelay*1000) > 1)
                {
                    LastWindupTime = EvadeUtils.TickCount + castTime - Game.Ping/2;
                    if (IsDodging)
                    {
                        SpellDetector_OnProcessDetectedSpells(); //reprocess
                        ConsoleDebug.WriteLineColor("Reprocessing Detected Spells...", ConsoleColor.Yellow);
                    }
                }
            }
        }

        private void Game_OnGameUpdate(EventArgs args)
        {
            try
            {
                CheckHeroInDanger();
                CheckDodgeOnlyDangerous();
                if (IsChanneling && ChannelPosition.Distance(GameData.HeroInfo.ServerPos2D) > 50) //TODO: !GameData.MyHero.IsChannelingImportantSpell()
                {
                    IsChanneling = false;
                    ConsoleDebug.WriteLineColor("Stopped Channeling.", ConsoleColor.Yellow);
                }

                //if (() Properties.Properties.Data["ResetConfig"].Cast<CheckBox>().CurrentValue)
                //{
                //    ResetConfig();
                //    Menu["ResetConfig"].Cast<CheckBox>().CurrentValue = false;
                //}

                //if (() Properties.Properties.Data["ResetConfig200"].Cast<CheckBox>().CurrentValue)
                //{
                //    SetPatchConfig();
                //    Menu["ResetConfig200"].Cast<CheckBox>().CurrentValue = false;
                //}

                var limitDelay = ConfigValue.TickLimiter.GetInt();
                //Tick limiter                
                if (EvadeUtils.TickCount - LastTickCount > limitDelay && EvadeUtils.TickCount > LastStopEvadeTime)
                {
                    DodgeSkillShots(); //walking         


                    ContinueLastBlockedCommand();
                    LastTickCount = EvadeUtils.TickCount;
                }

                EvadeSpell.UseEvadeSpell(); //using spells
                RecalculatePath();
            }
            catch (Exception e)
            {
                ConsoleDebug.WriteLine(e);
            }
        }

        private void RecalculatePath()
        {
            if (ConfigValue.RecalculatePath.GetBool() && IsDodging) //recheck path
            {
                if (LastPosInfo != null && !LastPosInfo.RecalculatedPath)
                {
                    var path = GameData.MyHero.Path;
                    if (path.Length > 0)
                    {
                        var movePos = path.Last().To2D();

                        if (movePos.Distance(LastPosInfo.Position) < 5) //more strict checking
                        {
                            var posInfo = EvadeHelper.CanHeroWalkToPos(movePos, GameData.HeroInfo.MoveSpeed, 0, 0, false);
                            if (posInfo.PosDangerCount > LastPosInfo.PosDangerCount)
                            {
                                LastPosInfo.RecalculatedPath = true;

                                if (EvadeSpell.PreferEvadeSpell())
                                {
                                    LastPosInfo = PositionInfo.SetAllUndodgeable();
                                }
                                else
                                {
                                    var newPosInfo = EvadeHelper.GetBestPosition();
                                    if (newPosInfo.PosDangerCount < posInfo.PosDangerCount)
                                    {
                                        LastPosInfo = newPosInfo;
                                        CheckHeroInDanger();
                                        DodgeSkillShots();
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private void ContinueLastBlockedCommand()
        {
            if (ConfigValue.ContinueMovement.GetBool() && Situation.ShouldDodge())
            {
                var movePos = LastBlockedUserMoveTo.TargetPosition;
                var extraDelay = Config.Properties.GetInt(ConfigValue.ExtraPingBuffer);

                if (IsDodging == false && LastBlockedUserMoveTo.IsProcessed == false && EvadeUtils.TickCount - LastEvadeCommand.Timestamp > Game.Ping + extraDelay && EvadeUtils.TickCount - LastBlockedUserMoveTo.Timestamp < 1500)
                {
                    movePos = movePos + (movePos - GameData.HeroInfo.ServerPos2D).Normalized()*EvadeUtils.Random.NextFloat(1, 65);

                    if (!EvadeHelper.CheckMovePath(movePos, Game.Ping + extraDelay))
                    {
                        //ConsoleDebug.WriteLine("Continue Movement");
                        //GameData.MyHero.IssueOrder(GameObjectOrder.MoveTo, movePos.To3D());
                        ConsoleDebug.WriteLineColor("Continuing Last Blocked Command", ConsoleColor.Yellow);
                        EvadeCommand.MoveTo(movePos);
                        LastBlockedUserMoveTo.IsProcessed = true;
                    }
                }
            }
        }

        private void CheckHeroInDanger()
        {
            bool playerInDanger = false;
            foreach (KeyValuePair<int, Spell> entry in SpellDetector.Spells)
            {
                Spell spell = entry.Value;

                if (LastPosInfo != null) //&& lastPosInfo.dodgeableSpells.Contains(spell.spellID))
                {
                    if (GameData.MyHero.ServerPosition.To2D().InSkillShot(spell, GameData.HeroInfo.BoundingRadius))
                    {
                        playerInDanger = true;
                        break;
                    }

                    if (ConfigValue.EnableEvadeDistance.GetBool() && EvadeUtils.TickCount < LastPosInfo.EndTime)
                    {
                        playerInDanger = true;
                        break;
                    }
                }
            }

            if (IsDodging && !playerInDanger)
            {
                LastDodgingEndTime = EvadeUtils.TickCount;
            }

            if (IsDodging == false && !Situation.ShouldDodge())
                return;

            IsDodging = playerInDanger;
        }

        private void DodgeSkillShots()
        {
            if (!Situation.ShouldDodge())
            {
                IsDodging = false;
                return;
            }

            /*
            if (isDodging && playerInDanger == false) //serverpos test
            {
                GameData.MyHero.IssueOrder(GameObjectOrder.HoldPosition, myHero, false);
            }*/

            if (IsDodging)
            {
                ConsoleDebug.WriteLineColor("Dodging Skill Shots by walking", ConsoleColor.Green);
                if (LastPosInfo != null)
                {
                    /*foreach (KeyValuePair<int, Spell> entry in SpellDetector.spells)
                    {
                        Spell spell = entry.Value;

                        ConsoleDebug.WriteLine("" + (int)(TickCount-spell.startTime));
                    }*/


                    Vector2 lastBestPosition = LastPosInfo.Position;

                    if (!ConfigValue.ClickOnlyOnce.GetBool() || !(GameData.MyHero.Path.Length > 0 && LastPosInfo.Position.Distance(GameData.MyHero.Path.Last().To2D()) < 5))
                        //|| lastPosInfo.timestamp > lastEvadeOrderTime)
                    {
                        EvadeCommand.MoveTo(lastBestPosition);
                        LastEvadeOrderTime = EvadeUtils.TickCount;
                    }
                }
            }
            else //if not dodging
            {
                //Check if hero will walk into a skillshot
                var path = GameData.MyHero.Path;
                if (path.Length > 0)
                {
                    var movePos = path[path.Length - 1].To2D();

                    if (EvadeHelper.CheckMovePath(movePos))
                    {
                        /*if (() Properties.Properties.Data["AllowCrossing"].Cast<CheckBox>().CurrentValue)
                        {
                            var extraDelayBuffer = () Properties.Properties.Data["ExtraPingBuffer"]
                                 + 30;
                            var extraDist = () Properties.Properties.Data["ExtraCPADistance"]
                                 + 10;

                            var tPosInfo = EvadeHelper.CanHeroWalkToPos(movePos, GameData.HeroInfo.moveSpeed, extraDelayBuffer + Game.Ping, extraDist);

                            if (tPosInfo.posDangerLevel == 0)
                            {
                                lastPosInfo = tPosInfo;
                                return;
                            }
                        }*/

                        var posInfo = EvadeHelper.GetBestPositionMovementBlock(movePos);
                        if (posInfo != null)
                        {
                            EvadeCommand.MoveTo(posInfo.Position);
                        }
                        return;
                    }
                }
            }
        }

        public void CheckLastMoveTo()
        {
            if (ConfigValue.FastMovementBlock.GetBool())
            {
                if (IsDodging == false && LastIssueOrderArgs != null && LastIssueOrderArgs.Order == GameObjectOrder.MoveTo && Game.Time*1000 - LastIssueOrderGameTime < 500)
                {
                    Game_OnIssueOrder(GameData.MyHero, LastIssueOrderArgs);
                    LastIssueOrderArgs = null;
                }
            }
        }

        public static bool IsDodgeDangerousEnabled()
        {
            if (ConfigValue.OnlyDodgeDangerous.GetBool())
            {
                return true;
            }

            if (ConfigValue.DodgeDangerousKeysEnabled.GetBool())
            {
                if (ConfigValue.DodgeDangerousKey1.GetBool() || ConfigValue.DodgeDangerousKey2.GetBool())
                    return true;
            }

            return false;
        }

        public static void CheckDodgeOnlyDangerous() //Dodge only dangerous event
        {
            bool bDodgeOnlyDangerous = IsDodgeDangerousEnabled();

            if (DodgeOnlyDangerous && bDodgeOnlyDangerous)
            {
                SpellDetector.RemoveNonDangerousSpells();
                DodgeOnlyDangerous = true;
            }
            else
            {
                DodgeOnlyDangerous = bDodgeOnlyDangerous;
            }
        }

        public static void SetAllUndodgeable()
        {
            LastPosInfo = PositionInfo.SetAllUndodgeable();
        }

        private void SpellDetector_OnProcessDetectedSpells()
        {
            GameData.HeroInfo.UpdateInfo();
            if (!ConfigValue.DodgeSkillShots.GetBool())
            {
                LastPosInfo = PositionInfo.SetAllUndodgeable();
                EvadeSpell.UseEvadeSpell();
                return;
            }
            if (GameData.HeroInfo.ServerPos2D.CheckDangerousPos(0) || GameData.HeroInfo.ServerPos2DExtra.CheckDangerousPos(0))
            {
                if (EvadeSpell.PreferEvadeSpell())
                {
                    LastPosInfo = PositionInfo.SetAllUndodgeable();
                }
                else
                {
                    var calculationTimer = EvadeUtils.TickCount;

                    var posInfo = EvadeHelper.GetBestPosition();

                    var caculationTime = EvadeUtils.TickCount - calculationTimer;

                    if (NumCalculationTime > 0)
                    {
                        SumCalculationTime += caculationTime;
                        AvgCalculationTime = SumCalculationTime/NumCalculationTime;
                    }
                    NumCalculationTime += 1;

                    //ConsoleDebug.WriteLine("CalculationTime: " + caculationTime);

                    /*if (EvadeHelper.GetHighestDetectedSpellID() > EvadeHelper.GetHighestSpellID(posInfo))
                    {
                        return;
                    }*/
                    if (posInfo != null)
                    {
                        LastPosInfo = posInfo.CompareLastMovePos();

                        var travelTime = GameData.HeroInfo.ServerPos2DPing.Distance(LastPosInfo.Position)/GameData.MyHero.MoveSpeed;

                        LastPosInfo.EndTime = EvadeUtils.TickCount + travelTime*1000 - 100;
                    }

                    CheckHeroInDanger();
                    DodgeSkillShots(); //walking
                    CheckLastMoveTo();
                    EvadeSpell.UseEvadeSpell(); //using spells
                }
            }
            else
            {
                LastPosInfo = PositionInfo.SetAllDodgeable();
                CheckLastMoveTo();
            }


            //ConsoleDebug.WriteLine("SkillsDodged: " + lastPosInfo.dodgeableSpells.Count + " DangerLevel: " + lastPosInfo.undodgeableSpells.Count);            
        }
    }
}