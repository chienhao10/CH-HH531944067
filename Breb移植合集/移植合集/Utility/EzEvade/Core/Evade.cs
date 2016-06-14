using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using EloBuddy;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK;

namespace ezEvade
{
    internal class Evade
    {
        public static AIHeroClient myHero { get { return ObjectManager.Player; } }

        public static SpellDetector spellDetector;
        private static SpellDrawer spellDrawer;
        private static PingTester pingTester;
        private static EvadeSpell evadeSpell;

        public static SpellSlot lastSpellCast;
        public static float lastSpellCastTime = 0;

        public static float lastWindupTime = 0;

        public static float lastTickCount = 0;
        public static float lastStopEvadeTime = 0;

        public static Vector3 lastMovementBlockPos = Vector3.Zero;
        public static float lastMovementBlockTime = 0;

        public static float lastEvadeOrderTime = 0;
        public static float lastIssueOrderGameTime = 0;
        public static float lastIssueOrderTime = 0;
        public static PlayerIssueOrderEventArgs lastIssueOrderArgs = null;

        public static Vector2 lastMoveToPosition = Vector2.Zero;
        public static Vector2 lastMoveToServerPos = Vector2.Zero;
        public static Vector2 lastStopPosition = Vector2.Zero;

        public static DateTime assemblyLoadTime = DateTime.Now;

        public static bool isDodging = false;
        public static bool dodgeOnlyDangerous = false;

        public static bool hasGameEnded = false;
        public static bool isChanneling = false;
        public static Vector2 channelPosition = Vector2.Zero;

        public static PositionInfo lastPosInfo;

        public static EvadeCommand lastEvadeCommand = new EvadeCommand { isProcessed = true, timestamp = EvadeUtils.TickCount };

        public static EvadeCommand lastBlockedUserMoveTo = new EvadeCommand { isProcessed = true, timestamp = EvadeUtils.TickCount };
        public static float lastDodgingEndTime = 0;

        public static Menu menu;

        public static float sumCalculationTime = 0;
        public static float numCalculationTime = 0;
        public static float avgCalculationTime = 0;

        public Evade()
        {
            LoadAssembly();
        }

        private void LoadAssembly()
        {
            DelayAction.Add(0, () =>
            {
                if (Game.Mode == GameMode.Running)
                {
                    Game_OnGameLoad(new EventArgs());
                }
                else
                {
                    Game.OnLoad += Game_OnGameLoad;
                }
            });
        }

        private void Game_OnGameLoad(EventArgs args)
        {
            try
            {
                Player.OnIssueOrder += Game_OnIssueOrder;
                Spellbook.OnCastSpell += Game_OnCastSpell;
                Game.OnUpdate += Game_OnGameUpdate;

                AIHeroClient.OnProcessSpellCast += Game_OnProcessSpell;

                Game.OnEnd += Game_OnGameEnd;
                SpellDetector.OnProcessDetectedSpells += SpellDetector_OnProcessDetectedSpells;
                Orbwalker.OnPreAttack += Orbwalking_BeforeAttack;

                /*Console.WriteLine("<font color=\"#66CCFF\" >Yomie's </font><font color=\"#CCFFFF\" >ezEvade</font> - " +
                   "<font color=\"#FFFFFF\" >Version " + Assembly.GetExecutingAssembly().GetName().Version + "</font>");
                */

                menu = MainMenu.AddMenu("CH汉化-EZ躲避", "ezEvade");
                ObjectCache.menuCache.AddMenuToCache(menu);

                mainMenu = menu.AddSubMenu("核心", "Main");
                ObjectCache.menuCache.AddMenuToCache(mainMenu);
                mainMenu.Add("DodgeSkillShots", new KeyBind("开启躲避", true, KeyBind.BindTypes.PressToggle, 'K'));
                mainMenu.Add("ActivateEvadeSpells", new KeyBind("使用技能躲避", true, KeyBind.BindTypes.PressToggle, 'K'));
                mainMenu.Add("DodgeDangerous", new CheckBox("只躲避危险技能", false));
                mainMenu.Add("DodgeFOWSpells", new CheckBox("躲避战争迷雾技能"));
                mainMenu.Add("DodgeCircularSpells", new CheckBox("躲避圈形指向性技能"));

                spellDetector = new SpellDetector(menu);
                evadeSpell = new EvadeSpell(menu);

                keyMenu = menu.AddSubMenu("按键设置", "KeySettings");
                ObjectCache.menuCache.AddMenuToCache(keyMenu);
                keyMenu.Add("DodgeDangerousKeyEnabled", new CheckBox("开启只躲避危险技能按键", false));
                keyMenu.Add("DodgeDangerousKey", new KeyBind("只躲避危险技能按键", false, KeyBind.BindTypes.HoldActive, 32));
                keyMenu.Add("DodgeDangerousKey2", new KeyBind("只躲避危险技能按键 2", false, KeyBind.BindTypes.HoldActive, 'V'));
                keyMenu.AddSeparator();
                keyMenu.Add("DodgeOnlyOnComboKeyEnabled", new CheckBox("开启只在连招中躲避", false));
                keyMenu.Add("DodgeComboKey", new KeyBind("只在连招中躲避按键", false, KeyBind.BindTypes.HoldActive, 32));
                keyMenu.AddSeparator();
                keyMenu.Add("DontDodgeKeyEnabled", new CheckBox("开启不躲避按键", false));
                keyMenu.Add("DontDodgeKey", new KeyBind("不躲避按键", false, KeyBind.BindTypes.HoldActive, 'Z'));

                miscMenu = menu.AddSubMenu("杂项设置", "MiscSettings");
                ObjectCache.menuCache.AddMenuToCache(miscMenu);
                miscMenu.AddGroupLabel("Misc : ");
                miscMenu.Add("HigherPrecision", new CheckBox("增强躲避精密度", false));
                miscMenu.Add("RecalculatePosition", new CheckBox("重新计算路径"));
                miscMenu.Add("ContinueMovement", new CheckBox("继续躲避前的移动", false));
                miscMenu.Add("CalculateWindupDelay", new CheckBox("计算延迟", true));
                miscMenu.Add("CheckSpellCollision", new CheckBox("检查技能弹道阻挡", false));
                miscMenu.Add("DodgeCheckHP", new CheckBox("检查血量 %时无视", false));
                miscMenu.Add("PreventDodgingUnderTower", new CheckBox("防止塔下躲避", false));
                miscMenu.Add("PreventDodgingNearEnemy", new CheckBox("防止在敌人附近躲避"));
                miscMenu.Add("AdvancedSpellDetection", new CheckBox("进阶技能探测", false));
                miscMenu.AddSeparator();
                miscMenu.AddGroupLabel("模式 : ");
                miscMenu.Add("EvadeMode", new ComboBox("躲避模式", 0, "Smooth", "Fastest", "Very Smooth"));
                miscMenu["EvadeMode"].Cast<ComboBox>().OnValueChange += OnEvadeModeChange;
                miscMenu.AddSeparator();
                miscMenu.AddGroupLabel("人性化");
                miscMenu.Add("ClickOnlyOnce", new CheckBox("只点击一次", true));
                miscMenu.Add("EnableEvadeDistance", new CheckBox("延长躲避", false));
                miscMenu.Add("TickLimiter", new Slider("点击限制", 100, 0, 500));
                miscMenu.Add("SpellDetectionTime", new Slider("技能探测时间", 0, 0, 1000));
                miscMenu.Add("ReactionTime", new Slider("反应时间", 0, 0, 500));
                miscMenu.Add("DodgeInterval", new Slider("躲避间隔时间", 0, 0, 2000));
                miscMenu.AddSeparator();
                miscMenu.AddGroupLabel("快速躲避");
                miscMenu.Add("FastMovementBlock", new CheckBox("阻挡快速移动", false));
                miscMenu.Add("FastEvadeActivationTime", new Slider("快速移动激活时间", 65, 0, 500));
                miscMenu.Add("SpellActivationTime", new Slider("技能激活时间", 200, 0, 1000));
                miscMenu.Add("RejectMinDistance", new Slider("碰撞缓冲距离", 10, 0, 100));
                miscMenu.AddSeparator();
                miscMenu.AddGroupLabel("高级人性化");
                miscMenu.Add("ExtraPingBuffer", new Slider("额外网络延迟缓冲", 65, 0, 200));
                miscMenu.Add("ExtraCPADistance", new Slider("额外体积碰撞距离", 10, 0, 150));
                miscMenu.Add("ExtraSpellRadius", new Slider("额外技能半径", 0, 0, 100));
                miscMenu.Add("ExtraEvadeDistance", new Slider("额外躲避距离", 100, 0, 300));
                miscMenu.Add("ExtraAvoidDistance", new Slider("额外防止距离", 50, 0, 300));
                miscMenu.Add("MinComfortZone", new Slider("最低英雄安全范围", 550, 0, 1000));
                miscMenu.AddSeparator();
                miscMenu.AddGroupLabel("测试");
                miscMenu.Add("LoadPingTester", new CheckBox("载入网络延迟测试", false));
                miscMenu["LoadPingTester"].Cast<CheckBox>().OnValueChange += OnLoadPingTesterChange;

                spellDrawer = new SpellDrawer(menu);

                var initCache = ObjectCache.myHeroCache;

                Console.WriteLine("ezEvade Loaded");

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public static Menu miscMenu, keyMenu, mainMenu;

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

        private void OnEvadeModeChange(ValueBase<int> sender, ValueBase<int>.ValueChangeArgs args)
        {
            var mode = args.NewValue;

            if (mode == 2)
            {
                miscMenu["FastEvadeActivationTime"].Cast<Slider>().CurrentValue = 0;
                miscMenu["RejectMinDistance"].Cast<Slider>().CurrentValue = 0;
                miscMenu["ExtraCPADistance"].Cast<Slider>().CurrentValue = 0;
                miscMenu["ExtraPingBuffer"].Cast<Slider>().CurrentValue = 40;
            }
            else if (mode == 0)
            {
                miscMenu["FastEvadeActivationTime"].Cast<Slider>().CurrentValue = 65;
                miscMenu["RejectMinDistance"].Cast<Slider>().CurrentValue = 10;
                miscMenu["ExtraCPADistance"].Cast<Slider>().CurrentValue = 10;
                miscMenu["ExtraPingBuffer"].Cast<Slider>().CurrentValue = 65;
            }
        }

        private void OnLoadPingTesterChange(ValueBase<bool> sender, ValueBase<bool>.ValueChangeArgs args)
        {
            miscMenu["LoadPingTester"].Cast<CheckBox>().CurrentValue = false;
            if (pingTester == null)
            {
                pingTester = new PingTester();
            }
        }

        private void Game_OnGameEnd(GameEndEventArgs args)
        {
            hasGameEnded = true;
        }

        private void Game_OnCastSpell(Spellbook spellbook, SpellbookCastSpellEventArgs args)
        {
            if (!spellbook.Owner.IsMe)
                return;

            var sData = spellbook.GetSpell(args.Slot);
            string name;

            if (SpellDetector.channeledSpells.TryGetValue(sData.Name, out name))
            {
                //Evade.isChanneling = true;
                //Evade.channelPosition = ObjectCache.myHeroCache.serverPos2D;
                lastStopEvadeTime = EvadeUtils.TickCount + ObjectCache.gamePing + 100;
            }

            //block spell commmands if evade spell just used
            if (EvadeSpell.lastSpellEvadeCommand != null && EvadeSpell.lastSpellEvadeCommand.timestamp + ObjectCache.gamePing + 150 > EvadeUtils.TickCount)
            {
                args.Process = false;
            }

            lastSpellCast = args.Slot;
            lastSpellCastTime = EvadeUtils.TickCount;

            //moved from processPacket

            /*if (args.Slot == SpellSlot.Recall)
            {
                lastStopPosition = myHero.ServerPosition.LSTo2D();
            }*/

            if (Situation.ShouldDodge())
            {
                if (isDodging && SpellDetector.spells.Count() > 0)
                {
                    foreach (KeyValuePair<String, SpellData> entry in SpellDetector.windupSpells)
                    {
                        SpellData spellData = entry.Value;

                        if (spellData.spellKey == args.Slot) //check if it's a spell that we should block
                        {
                            args.Process = false;
                            return;
                        }
                    }
                }
            }

            foreach (var evadeSpell in EvadeSpell.evadeSpells)
            {
                if (evadeSpell.isItem == false && evadeSpell.spellKey == args.Slot
                    && evadeSpell.untargetable == false)
                {
                    if (//evadeSpell.evadeType == EvadeType.Blink || 
                        evadeSpell.evadeType == EvadeType.Dash)
                    {
                        //Block spell cast if flashing/blinking into spells
                        /*if (args.EndPosition.LSTo2D().CheckDangerousPos(6, true)) //for blink + dash
                        {
                            args.Process = false;
                            return;
                        }*/

                        if (evadeSpell.evadeType == EvadeType.Dash)
                        {
                            //var extraDelayBuffer = ObjectCache.menuCache.cache["ExtraPingBuffer"].Cast<Slider>().CurrentValue;
                            //var extraDist = ObjectCache.menuCache.cache["ExtraCPADistance"].Cast<Slider>().CurrentValue;

                            var dashPos = args.StartPosition.LSTo2D(); //real pos?

                            if (args.Target != null)
                            {
                                dashPos = args.Target.Position.LSTo2D();
                            }

                            if (evadeSpell.fixedRange
                                || dashPos.LSDistance(myHero.ServerPosition.LSTo2D()) > evadeSpell.range)
                            {
                                var dir = (dashPos - myHero.ServerPosition.LSTo2D()).LSNormalized();
                                dashPos = myHero.ServerPosition.LSTo2D() + dir * evadeSpell.range;
                            }

                            //Draw.RenderObjects.Add(new Draw.RenderCircle(dashPos, 1000));

                            var posInfo = EvadeHelper.CanHeroWalkToPos(dashPos, evadeSpell.speed,
                                ObjectCache.gamePing, 0);

                            if (posInfo.posDangerLevel > 0)
                            {
                                args.Process = false;
                                return;
                            }
                        }

                        lastPosInfo = PositionInfo.SetAllUndodgeable(); //really?

                        if (isDodging || EvadeUtils.TickCount < lastDodgingEndTime + 500)
                        {
                            EvadeCommand.MoveTo(Game.CursorPos.LSTo2D()); //block moveto
                            lastStopEvadeTime = EvadeUtils.TickCount + ObjectCache.gamePing + 100;
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

            if (args.Order == GameObjectOrder.MoveTo)
            {
                if (isDodging && SpellDetector.spells.Any())
                {
                    CheckHeroInDanger();

                    lastBlockedUserMoveTo = new EvadeCommand
                    {
                        order = EvadeOrderCommand.MoveTo,
                        targetPosition = args.TargetPosition.LSTo2D(),
                        timestamp = EvadeUtils.TickCount,
                        isProcessed = false,
                    };

                    args.Process = false;
                }
                else
                {
                    var movePos = args.TargetPosition.LSTo2D();
                    var extraDelay = ObjectCache.menuCache.cache["ExtraPingBuffer"].Cast<Slider>().CurrentValue;
                    if (EvadeHelper.CheckMovePath(movePos, ObjectCache.gamePing + extraDelay))
                    {
                        /*if (ObjectCache.menuCache.cache["AllowCrossing"].Cast<CheckBox>().CurrentValue)
                        {
                            var extraDelayBuffer = ObjectCache.menuCache.cache["ExtraPingBuffer"]
                                .GetValue<Slider>().Value + 30;
                            var extraDist = ObjectCache.menuCache.cache["ExtraCPADistance"]
                                .GetValue<Slider>().Value + 10;

                            var tPosInfo = EvadeHelper.CanHeroWalkToPos(movePos, ObjectCache.myHeroCache.moveSpeed, extraDelayBuffer + ObjectCache.gamePing, extraDist);

                            if (tPosInfo.posDangerLevel == 0)
                            {
                                lastPosInfo = tPosInfo;
                                return;
                            }
                        }*/

                        lastBlockedUserMoveTo = new EvadeCommand
                        {
                            order = EvadeOrderCommand.MoveTo,
                            targetPosition = args.TargetPosition.LSTo2D(),
                            timestamp = EvadeUtils.TickCount,
                            isProcessed = false,
                        };

                        args.Process = false; //Block the command

                        if (EvadeUtils.TickCount - lastMovementBlockTime < 500 && lastMovementBlockPos.LSDistance(args.TargetPosition) < 100)
                        {
                            return;
                        }

                        lastMovementBlockPos = args.TargetPosition;
                        lastMovementBlockTime = EvadeUtils.TickCount;

                        var posInfo = EvadeHelper.GetBestPositionMovementBlock(movePos);
                        if (posInfo != null)
                        {
                            EvadeCommand.MoveTo(posInfo.position);
                        }
                        return;
                    }
                    else
                    {
                        lastBlockedUserMoveTo.isProcessed = true;
                    }
                }
            }
            else //need more logic
            {
                if (isDodging)
                {
                    args.Process = false; //Block the command
                }
                else
                {
                    if (args.Order == GameObjectOrder.AttackUnit)
                    {
                        var target = args.Target;
                        if (target != null && target.IsValid<Obj_AI_Base>())
                        {
                            var baseTarget = target as Obj_AI_Base;
                            if (ObjectCache.myHeroCache.serverPos2D.LSDistance(baseTarget.ServerPosition.LSTo2D()) >
                                myHero.AttackRange + ObjectCache.myHeroCache.boundingRadius + baseTarget.BoundingRadius)
                            {
                                var movePos = args.TargetPosition.LSTo2D();
                                var extraDelay = ObjectCache.menuCache.cache["ExtraPingBuffer"].Cast<Slider>().CurrentValue;
                                if (EvadeHelper.CheckMovePath(movePos, ObjectCache.gamePing + extraDelay))
                                {
                                    args.Process = false; //Block the command
                                    return;
                                }
                            }
                        }
                    }
                }
            }

            if (args.Process == true)
            {
                lastIssueOrderGameTime = Game.Time * 1000;
                lastIssueOrderTime = EvadeUtils.TickCount;
                lastIssueOrderArgs = args;

                if (args.Order == GameObjectOrder.MoveTo)
                {
                    lastMoveToPosition = args.TargetPosition.LSTo2D();
                    lastMoveToServerPos = myHero.ServerPosition.LSTo2D();
                }

                if (args.Order == GameObjectOrder.Stop)
                {
                    lastStopPosition = myHero.ServerPosition.LSTo2D();
                }
            }
        }

        private void Orbwalking_BeforeAttack(AttackableUnit target, Orbwalker.PreAttackArgs args)
        {
            if (isDodging)
            {
                args.Process = false; //Block orbwalking
            }
        }

        private void Game_OnProcessSpell(Obj_AI_Base hero, GameObjectProcessSpellCastEventArgs args)
        {
            if (!hero.IsMe)
            {
                return;
            }

            if (hero.NetworkId != myHero.NetworkId)
            {
                Console.WriteLine("IS NOT ME");
            }

            /*if (args.SData.Name.Contains("Recall"))
            {
                var distance = lastStopPosition.LSDistance(args.Start.LSTo2D());
                float moveTime = 1000 * distance / myHero.MoveSpeed;

                Console.WriteLine("Extra dist: " + distance + " Extra Delay: " + moveTime);
            }*/

            string name;
            if (SpellDetector.channeledSpells.TryGetValue(args.SData.Name, out name))
            {
                Evade.isChanneling = true;
                Evade.channelPosition = myHero.ServerPosition.LSTo2D();
            }

            if (ObjectCache.menuCache.cache["CalculateWindupDelay"].Cast<CheckBox>().CurrentValue)
            {
                var castTime = (hero.Spellbook.CastTime - Game.Time) * 1000;

                if (castTime > 0 && !Orbwalking.IsAutoAttack(args.SData.Name)
                    && Math.Abs(castTime - myHero.AttackCastDelay * 1000) > 1)
                {
                    Evade.lastWindupTime = EvadeUtils.TickCount + castTime - Game.Ping / 2;

                    if (Evade.isDodging)
                    {
                        SpellDetector_OnProcessDetectedSpells(); //reprocess
                    }
                }
            }


        }

        private void Game_OnGameUpdate(EventArgs args)
        {
            try
            {
                ObjectCache.myHeroCache.UpdateInfo();
                CheckHeroInDanger();

                if (isChanneling && channelPosition.LSDistance(ObjectCache.myHeroCache.serverPos2D) > 50
                    && !myHero.IsChannelingImportantSpell())
                {
                    isChanneling = false;
                }

                var limitDelay = ObjectCache.menuCache.cache["TickLimiter"].Cast<Slider>().CurrentValue; //Tick limiter                
                if (EvadeHelper.fastEvadeMode || EvadeUtils.TickCount - lastTickCount > limitDelay && EvadeUtils.TickCount > lastStopEvadeTime)
                {
                    DodgeSkillShots(); //walking           
                    ContinueLastBlockedCommand();
                    lastTickCount = EvadeUtils.TickCount;
                }

                EvadeSpell.UseEvadeSpell(); //using spells
                CheckDodgeOnlyDangerous();
                RecalculatePath();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void RecalculatePath()
        {
            if (ObjectCache.menuCache.cache["RecalculatePosition"].Cast<CheckBox>().CurrentValue && isDodging)//recheck path
            {
                if (lastPosInfo != null && !lastPosInfo.recalculatedPath)
                {
                    var path = myHero.Path;
                    if (path.Length > 0)
                    {
                        var movePos = path.Last().LSTo2D();

                        if (movePos.LSDistance(lastPosInfo.position) < 5) //more strict checking
                        {
                            var posInfo = EvadeHelper.CanHeroWalkToPos(movePos, ObjectCache.myHeroCache.moveSpeed, 0, 0, false);
                            if (posInfo.posDangerCount > lastPosInfo.posDangerCount)
                            {
                                lastPosInfo.recalculatedPath = true;

                                if (EvadeSpell.PreferEvadeSpell())
                                {
                                    lastPosInfo = PositionInfo.SetAllUndodgeable();
                                }
                                else
                                {
                                    var newPosInfo = EvadeHelper.GetBestPosition();
                                    if (newPosInfo.posDangerCount < posInfo.posDangerCount)
                                    {
                                        lastPosInfo = newPosInfo;
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
            if (ObjectCache.menuCache.cache["ContinueMovement"].Cast<CheckBox>().CurrentValue
                && Situation.ShouldDodge())
            {
                var movePos = lastBlockedUserMoveTo.targetPosition;
                var extraDelay = ObjectCache.menuCache.cache["ExtraPingBuffer"].Cast<Slider>().CurrentValue;

                if (isDodging == false && lastBlockedUserMoveTo.isProcessed == false
                    && EvadeUtils.TickCount - lastEvadeCommand.timestamp > ObjectCache.gamePing + extraDelay
                    && EvadeUtils.TickCount - lastBlockedUserMoveTo.timestamp < 1500)
                {
                    movePos = movePos + (movePos - ObjectCache.myHeroCache.serverPos2D).LSNormalized()
                        * EvadeUtils.random.NextFloat(1, 65);

                    if (!EvadeHelper.CheckMovePath(movePos, ObjectCache.gamePing + extraDelay))
                    {
                        //Console.WriteLine("Continue Movement");
                        //myHero.IssueOrder(GameObjectOrder.MoveTo, movePos.To3D());
                        EvadeCommand.MoveTo(movePos);
                        lastBlockedUserMoveTo.isProcessed = true;
                    }
                }
            }
        }

        private void CheckHeroInDanger()
        {
            bool playerInDanger = false;
            foreach (KeyValuePair<int, Spell> entry in SpellDetector.spells)
            {
                Spell spell = entry.Value;

                if (lastPosInfo != null && lastPosInfo.dodgeableSpells.Contains(spell.spellID))
                {
                    if (myHero.ServerPosition.LSTo2D().InSkillShot(spell, ObjectCache.myHeroCache.boundingRadius))
                    {
                        playerInDanger = true;
                        break;
                    }

                    if (ObjectCache.menuCache.cache["EnableEvadeDistance"].Cast<CheckBox>().CurrentValue &&
                        EvadeUtils.TickCount < lastPosInfo.endTime)
                    {
                        playerInDanger = true;
                        break;
                    }
                }
            }

            if (isDodging && !playerInDanger)
            {
                lastDodgingEndTime = EvadeUtils.TickCount;
            }

            if (isDodging == false && !Situation.ShouldDodge())
                return;

            isDodging = playerInDanger;
        }

        private void DodgeSkillShots()
        {
            if (!Situation.ShouldDodge())
            {
                isDodging = false;
                return;
            }

            /*
            if (isDodging && playerInDanger == false) //serverpos test
            {
                myHero.IssueOrder(GameObjectOrder.HoldPosition, myHero, false);
            }*/

            if (isDodging)
            {
                if (lastPosInfo != null)
                {
                    /*foreach (KeyValuePair<int, Spell> entry in SpellDetector.spells)
                    {
                        Spell spell = entry.Value;

                        Console.WriteLine("" + (int)(TickCount-spell.startTime));
                    }*/


                    Vector2 lastBestPosition = lastPosInfo.position;

                    if (ObjectCache.menuCache.cache["ClickOnlyOnce"].Cast<CheckBox>().CurrentValue == false
                        || !(myHero.Path.Count() > 0 && lastPosInfo.position.LSDistance(myHero.Path.Last().LSTo2D()) < 5))
                    //|| lastPosInfo.timestamp > lastEvadeOrderTime)
                    {
                        EvadeCommand.MoveTo(lastBestPosition);
                        lastEvadeOrderTime = EvadeUtils.TickCount;
                    }
                }
            }
            else //if not dodging
            {
                //Check if hero will walk into a skillshot
                var path = myHero.Path;
                if (path.Length > 0)
                {
                    var movePos = path[path.Length - 1].LSTo2D();

                    if (EvadeHelper.CheckMovePath(movePos))
                    {

                        /*if (ObjectCache.menuCache.cache["AllowCrossing"].Cast<CheckBox>().CurrentValue)
                        {
                            var extraDelayBuffer = ObjectCache.menuCache.cache["ExtraPingBuffer"]
                                .GetValue<Slider>().Value + 30;
                            var extraDist = ObjectCache.menuCache.cache["ExtraCPADistance"]
                                .GetValue<Slider>().Value + 10;

                            var tPosInfo = EvadeHelper.CanHeroWalkToPos(movePos, ObjectCache.myHeroCache.moveSpeed, extraDelayBuffer + ObjectCache.gamePing, extraDist);

                            if (tPosInfo.posDangerLevel == 0)
                            {
                                lastPosInfo = tPosInfo;
                                return;
                            }
                        }*/

                        var posInfo = EvadeHelper.GetBestPositionMovementBlock(movePos);
                        if (posInfo != null)
                        {
                            EvadeCommand.MoveTo(posInfo.position);
                        }
                        return;
                    }
                }
            }
        }

        public void CheckLastMoveTo()
        {
            if (EvadeHelper.fastEvadeMode || ObjectCache.menuCache.cache["FastMovementBlock"].Cast<CheckBox>().CurrentValue)
            {
                if (isDodging == false && lastIssueOrderArgs != null
                && lastIssueOrderArgs.Order == GameObjectOrder.MoveTo
                && Game.Time * 1000 - lastIssueOrderGameTime < 500)
                {
                    Game_OnIssueOrder(myHero, lastIssueOrderArgs);
                    lastIssueOrderArgs = null;
                }
            }
        }

        public static bool isDodgeDangerousEnabled()
        {
            if (ObjectCache.menuCache.cache["DodgeDangerous"].Cast<CheckBox>().CurrentValue == true)
            {
                return true;
            }

            if (ObjectCache.menuCache.cache["DodgeDangerousKeyEnabled"].Cast<CheckBox>().CurrentValue == true)
            {
                if (ObjectCache.menuCache.cache["DodgeDangerousKey"].Cast<CheckBox>().CurrentValue == true || ObjectCache.menuCache.cache["DodgeDangerousKey2"].Cast<CheckBox>().CurrentValue == true)
                    return true;
            }

            return false;
        }

        public static void CheckDodgeOnlyDangerous() //Dodge only dangerous event
        {
            bool bDodgeOnlyDangerous = isDodgeDangerousEnabled();

            if (dodgeOnlyDangerous == false && bDodgeOnlyDangerous)
            {
                spellDetector.RemoveNonDangerousSpells();
                dodgeOnlyDangerous = true;
            }
            else
            {
                dodgeOnlyDangerous = bDodgeOnlyDangerous;
            }
        }

        public static void SetAllUndodgeable()
        {
            lastPosInfo = PositionInfo.SetAllUndodgeable();
        }

        private void SpellDetector_OnProcessDetectedSpells()
        {
            ObjectCache.myHeroCache.UpdateInfo();

            if (ObjectCache.menuCache.cache["DodgeSkillShots"].Cast<KeyBind>().CurrentValue == false)
            {
                lastPosInfo = PositionInfo.SetAllUndodgeable();
                EvadeSpell.UseEvadeSpell();
                return;
            }

            if (ObjectCache.myHeroCache.serverPos2D.CheckDangerousPos(0)
                || ObjectCache.myHeroCache.serverPos2DExtra.CheckDangerousPos(0))
            {
                if (EvadeSpell.PreferEvadeSpell())
                {
                    lastPosInfo = PositionInfo.SetAllUndodgeable();
                }
                else
                {

                    var posInfo = EvadeHelper.GetBestPosition();

                    var calculationTimer = EvadeUtils.TickCount;
                    var caculationTime = EvadeUtils.TickCount - calculationTimer;

                    //computing time
                    /*if (numCalculationTime > 0)
                    {
                        sumCalculationTime += caculationTime;
                        avgCalculationTime = sumCalculationTime / numCalculationTime;
                    }
                    numCalculationTime += 1;*/

                    //Console.WriteLine("CalculationTime: " + caculationTime);

                    /*if (EvadeHelper.GetHighestDetectedSpellID() > EvadeHelper.GetHighestSpellID(posInfo))
                    {
                        return;
                    }*/
                    if (posInfo != null)
                    {
                        lastPosInfo = posInfo.CompareLastMovePos();

                        var travelTime = ObjectCache.myHeroCache.serverPos2DPing.LSDistance(lastPosInfo.position) / myHero.MoveSpeed;

                        lastPosInfo.endTime = EvadeUtils.TickCount + travelTime * 1000 - 100;
                    }

                    CheckHeroInDanger();
                    DodgeSkillShots(); //walking
                    CheckLastMoveTo();
                    EvadeSpell.UseEvadeSpell(); //using spells
                }
            }
            else
            {
                lastPosInfo = PositionInfo.SetAllDodgeable();
                CheckLastMoveTo();
            }


            //Console.WriteLine("SkillsDodged: " + lastPosInfo.dodgeableSpells.Count + " DangerLevel: " + lastPosInfo.undodgeableSpells.Count);            
        }
    }
}
