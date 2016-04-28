using System;
using System.Reflection;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using EzEvade;
using SharpDX;

// Credits to Rexy for porting this, I didn't use this much but some of the methods helped a lot.

namespace ezEvade
{
    internal class Evade
    {
        public static AIHeroClient myHero
        {
            get { return EloBuddy.ObjectManager.Player; }
        }

        public static SpellDetector spellDetector;
        private static SpellDrawer spellDrawer;


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

        //   private static bool GetDangerPoint = false;

        public static bool hasGameEnded = false;
        public static bool isChanneling = false;
        public static Vector2 channelPosition = Vector2.Zero;

        public static PositionInfo lastPosInfo;

        public static EvadeCommand lastEvadeCommand = new EvadeCommand
        {
            isProcessed = true,
            timestamp = EvadeUtils.TickCount
        };

        public static EvadeCommand lastBlockedUserMoveTo = new EvadeCommand
        {
            isProcessed = true,
            timestamp = EvadeUtils.TickCount
        };

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
            //Game_OnGameLoad(null);
            EloBuddy.SDK.Events.Loading.OnLoadingComplete += Game_OnGameLoad;
        }

        private void Game_OnGameLoad(EventArgs args)
        {
            Console.Write("ezEvade loading....");

            Player.OnIssueOrder += Game_OnIssueOrder;
            Spellbook.OnCastSpell += Game_OnCastSpell;
            Game.OnUpdate += Game_OnGameUpdate;

            Obj_AI_Base.OnProcessSpellCast += Game_OnProcessSpell;

            Game.OnEnd += Game_OnGameEnd;
            SpellDetector.OnProcessDetectedSpells += SpellDetector_OnProcessDetectedSpells;
            Orbwalker.OnPreAttack += Orbwalking_BeforeAttack;

            menu = MainMenu.AddMenu("ezEvade", "ezEvade");
            ObjectCache.menuCache.AddMenuToCache(menu);

            Menu mainMenu = menu.AddSubMenuEx("Main", "Main");
            mainMenu.Add("DodgeSkillShots", new KeyBind("Dodge SkillShots", true, KeyBind.BindTypes.PressToggle, 'K'));
            mainMenu.Add("ActivateEvadeSpells", new KeyBind("Use Evade Spells", true, KeyBind.BindTypes.PressToggle, 'K'));
            mainMenu.AddSeparator();
            mainMenu.Add("DodgeDangerous", new CheckBox("Dodge Only Dangerous", false));
            mainMenu.Add("ChaseModeMinHP", new CheckBox("Check Ignored HP %(ChaseMode)"));
            mainMenu.Add("DodgeFOWSpells", new CheckBox("Dodge FOW SkillShots"));
            mainMenu.Add("DodgeCircularSpells", new CheckBox("Dodge Circular SkillShots"));
            mainMenu.AddSeparator();
            mainMenu.Add("DodgeDangerousKeyEnabled", new CheckBox("Enable Dodge Only Dangerous Keys", false));
            mainMenu.Add("DodgeDangerousKey", new KeyBind("Dodge Only Dangerous Key", false, KeyBind.BindTypes.HoldActive, 32));
            mainMenu.Add("DodgeDangerousKey2", new KeyBind("Dodge Only Dangerous Key 2", false, KeyBind.BindTypes.HoldActive, 'V'));
            mainMenu.AddSeparator();
            mainMenu.AddGroupLabel("Evade Mode");
            var sliderEvadeMode = mainMenu.Add("EvadeMode", new Slider("Smooth", 0, 0, 2));
            var modeArray = new[] { "Smooth", "Fastest", "Very Smooth" };
            sliderEvadeMode.DisplayName = modeArray[sliderEvadeMode.CurrentValue];
            sliderEvadeMode.OnValueChange +=
                delegate(ValueBase<int> sender, ValueBase<int>.ValueChangeArgs changeArgs)
                {
                    sender.DisplayName = modeArray[changeArgs.NewValue];
                    OnEvadeModeChange(sender, changeArgs);
                };

            spellDetector = new SpellDetector(menu);
            evadeSpell = new EvadeSpell(menu);

            Menu miscMenu = menu.AddSubMenuEx("Misc Settings", "MiscSettings");
            miscMenu.Add("HigherPrecision", new CheckBox("Enhanced Dodge Precision", false));
            miscMenu.Add("RecalculatePosition", new CheckBox("Recalculate Path"));
            miscMenu.Add("ContinueMovement", new CheckBox("Continue Last Movement", false));
            miscMenu.Add("CalculateWindupDelay", new CheckBox("Calculate Windup Delay"));
            miscMenu.Add("CheckSpellCollision", new CheckBox("Check Spell Collision", false));
            miscMenu.Add("PreventDodgingUnderTower", new CheckBox("Prevent Dodging Under Tower", false));
            miscMenu.Add("PreventDodgingNearEnemy", new CheckBox("Prevent Dodging Near Enemies", false));
            miscMenu.Add("AdvancedSpellDetection", new CheckBox("Advanced Spell Detection", false));

            Menu limiterMenu = menu.AddSubMenuEx("Humanizer", "Limiter");
            limiterMenu.Add("ClickOnlyOnce", new CheckBox("Click Only Once", false));
            limiterMenu.Add("EnableEvadeDistance", new CheckBox("Extended Evade", false));
            limiterMenu.Add("TickLimiter", new Slider("Tick Limiter", 0, 0, 500));
            limiterMenu.Add("SpellDetectionTime", new Slider("Spell Detection Time", 0, 0, 1000));
            limiterMenu.Add("ReactionTime", new Slider("Reaction Time", 0, 0, 500));
            limiterMenu.Add("DodgeInterval", new Slider("Dodge Interval", 0, 0, 2000));

            Menu fastEvadeMenu = menu.AddSubMenuEx("Fast Evade", "FastEvade");
            fastEvadeMenu.Add("FastMovementBlock", new CheckBox("Fast Movement Block", false));
            fastEvadeMenu.Add("FastEvadeActivationTime", new Slider("FastEvade Activation Time", 65, 0, 500));
            fastEvadeMenu.Add("SpellActivationTime", new Slider("Spell Activation Time", 200, 0, 1000));
            fastEvadeMenu.Add("RejectMinDistance", new Slider("Collision Distance Buffer", 10));

            Menu bufferMenu = menu.AddSubMenuEx("Extra Buffers", "ExtraBuffers");
            bufferMenu.Add("ExtraPingBuffer", new Slider("Extra Ping Buffer", 65, 0, 200));
            bufferMenu.Add("ExtraCPADistance", new Slider("Extra Collision Distance", 10, 0, 150));
            bufferMenu.Add("ExtraSpellRadius", new Slider("Extra Spell Radius"));
            bufferMenu.Add("ExtraEvadeDistance", new Slider("Extra Evade Distance", 0, 0, 300));
            bufferMenu.Add("ExtraAvoidDistance", new Slider("Extra Avoid Distance", 50, 0, 300));
            bufferMenu.Add("MinComfortZone", new Slider("Min Distance to Champion", 1000, 0, 1000));

            spellDrawer = new SpellDrawer(menu);

            var initCache = ObjectCache.myHeroCache;

            Console.WriteLine("ezEvade Loaded");
        }

        private void OnEvadeModeChange(ValueBase<int> sender, ValueBase<int>.ValueChangeArgs changeArgs)
        {
            var mode = sender.DisplayName;

            if (mode == "Very Smooth")
            {
                menu["FastEvadeActivationTime"].Cast<Slider>().CurrentValue = 0;
                menu["RejectMinDistance"].Cast<Slider>().CurrentValue = 0;
                menu["ExtraCPADistance"].Cast<Slider>().CurrentValue = 0;
                menu["ExtraPingBuffer"].Cast<Slider>().CurrentValue = 40;
            }
            else if (mode == "Smooth")
            {
                menu["FastEvadeActivationTime"].Cast<Slider>().CurrentValue = 65;
                menu["RejectMinDistance"].Cast<Slider>().CurrentValue = 10;
                menu["ExtraCPADistance"].Cast<Slider>().CurrentValue = 10;
                menu["ExtraPingBuffer"].Cast<Slider>().CurrentValue = 65;
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

            if (EvadeSpell.lastSpellEvadeCommand != null &&
                EvadeSpell.lastSpellEvadeCommand.timestamp + ObjectCache.gamePing + 150 > EvadeUtils.TickCount)
            {
                args.Process = false;
            }

            lastSpellCast = args.Slot;
            lastSpellCastTime = EvadeUtils.TickCount;

            //moved from processPacket

            /*if (args.Slot == SpellSlot.Recall)
            {
                lastStopPosition = myHero.ServerPosition.To2D();
            }*/

            if (Situation.ShouldDodge())
            {
                if (isDodging && SpellDetector.spells.Count() > 0)
                {
                    if (SpellDetector.windupSpells.Select(entry => entry.Value).Any(spellData => spellData.spellKey == args.Slot))
                    {
                        args.Process = false;
                        return;
                    }
                }
            }

            foreach (var evadeSpell in EvadeSpell.evadeSpells)
            {
                if (evadeSpell.isItem == false && evadeSpell.spellKey == args.Slot)
                {
                    if (evadeSpell.evadeType == EvadeType.Blink
                        || evadeSpell.evadeType == EvadeType.Dash)
                    {
                        //Block spell cast if flashing/blinking into spells
                        if (args.EndPosition.To2D().CheckDangerousPos(6, true)) //for blink + dash
                        {
                            args.Process = false;
                            return;
                        }

                        if (evadeSpell.evadeType == EvadeType.Dash)
                        {
                            var extraDelayBuffer =
                                ObjectCache.menuCache.cache["ExtraPingBuffer"].Cast<Slider>().CurrentValue;
                            var extraDist = ObjectCache.menuCache.cache["ExtraCPADistance"].Cast<Slider>().CurrentValue;

                            var dashPos = Game.CursorPos.To2D(); //real pos?

                            if (evadeSpell.fixedRange)
                            {
                                var dir = (dashPos - myHero.ServerPosition.To2D()).Normalized();
                                dashPos = myHero.ServerPosition.To2D() + dir * evadeSpell.range;
                            }

                            //Draw.RenderObjects.Add(new Draw.RenderPosition(dashPos, 1000));

                            var posInfo = EvadeHelper.CanHeroWalkToPos(dashPos, evadeSpell.speed,
                                extraDelayBuffer + ObjectCache.gamePing, extraDist);

                            if (posInfo.posDangerLevel > 0)
                            {
                                args.Process = false;
                                return;
                            }
                        }

                        lastPosInfo = PositionInfo.SetAllUndodgeable(); //really?

                        if (isDodging || EvadeUtils.TickCount < lastDodgingEndTime + 500)
                        {
                            EvadeCommand.MoveTo(Game.CursorPos.To2D()); //block moveto
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
                //movement block code goes in here
                if (isDodging && SpellDetector.spells.Count() > 0)
                {
                    CheckHeroInDanger();

                    lastBlockedUserMoveTo = new EvadeCommand
                    {
                        order = EvadeOrderCommand.MoveTo,
                        targetPosition = args.TargetPosition.To2D(),
                        timestamp = EvadeUtils.TickCount,
                        isProcessed = false,
                    };

                    args.Process = false; //Block the command
                }
                else
                {
                    var movePos = args.TargetPosition.To2D();
                    var extraDelay = ObjectCache.menuCache.cache["ExtraPingBuffer"].Cast<Slider>().CurrentValue;
                    if (EvadeHelper.CheckMovePath(movePos, ObjectCache.gamePing + extraDelay))
                    {
                        /*if (ObjectCache.menuCache.cache["AllowCrossing"].Cast<CheckBox>().CurrentValue)
                        {
                            var extraDelayBuffer = ObjectCache.menuCache.cache["ExtraPingBuffer"]
                                .Cast<Slider>().CurrentValue + 30;
                            var extraDist = ObjectCache.menuCache.cache["ExtraCPADistance"]
                                .Cast<Slider>().CurrentValue + 10;

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
                            targetPosition = args.TargetPosition.To2D(),
                            timestamp = EvadeUtils.TickCount,
                            isProcessed = false,
                        };

                        args.Process = false; //Block the command

                        if (EvadeUtils.TickCount - lastMovementBlockTime < 500 &&
                            lastMovementBlockPos.Distance(args.TargetPosition) < 100)
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
                        if (target != null && target.GetType() == typeof(Obj_AI_Base) && ((Obj_AI_Base)target).IsValid())
                        {
                            var baseTarget = target as Obj_AI_Base;
                            if (ObjectCache.myHeroCache.serverPos2D.Distance(baseTarget.ServerPosition.To2D()) >
                                myHero.AttackRange + ObjectCache.myHeroCache.boundingRadius + baseTarget.BoundingRadius)
                            {
                                var movePos = args.TargetPosition.To2D();
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
                    lastMoveToPosition = args.TargetPosition.To2D();
                    lastMoveToServerPos = myHero.ServerPosition.To2D();
                }

                if (args.Order == GameObjectOrder.Stop)
                {
                    lastStopPosition = myHero.ServerPosition.To2D();
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

            /*if (args.SData.Name.Contains("Recall"))
            {
                var distance = lastStopPosition.Distance(args.Start.To2D());
                float moveTime = 1000 * distance / myHero.MoveSpeed;

                Console.WriteLine("Extra dist: " + distance + " Extra Delay: " + moveTime);
            }*/

            string name;
            if (SpellDetector.channeledSpells.TryGetValue(args.SData.Name, out name))
            {
                Evade.isChanneling = true;
                Evade.channelPosition = myHero.ServerPosition.To2D();
            }

            if (ObjectCache.menuCache.cache["CalculateWindupDelay"].Cast<CheckBox>().CurrentValue)
            {
                var castTime = (hero.Spellbook.CastTime - Game.Time) * 1000;

                if (castTime > 0 && !EloBuddy.SDK.Constants.AutoAttacks.IsAutoAttack(args.SData.Name)
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
            ObjectCache.myHeroCache.UpdateInfo();
            CheckHeroInDanger();

            if (isChanneling && channelPosition.Distance(ObjectCache.myHeroCache.serverPos2D) > 50
                ) //TODO: !myHero.IsChannelingImportantSpell()
            {
                isChanneling = false;
            }
            var limitDelay = ObjectCache.menuCache.cache["TickLimiter"].Cast<Slider>().CurrentValue;
            //Tick limiter                
            if (EvadeUtils.TickCount - lastTickCount > limitDelay
                && EvadeUtils.TickCount > lastStopEvadeTime)
            {
                DodgeSkillShots(); //walking           

                ContinueLastBlockedCommand();
                lastTickCount = EvadeUtils.TickCount;
            }

            EvadeSpell.UseEvadeSpell(); //using spells
            CheckDodgeOnlyDangerous();
            RecalculatePath();
        }

        private void RecalculatePath()
        {
            if (ObjectCache.menuCache.cache["RecalculatePosition"].Cast<CheckBox>().CurrentValue && isDodging) //recheck path
            {
                if (lastPosInfo != null && !lastPosInfo.recalculatedPath)
                {
                    var path = myHero.Path;
                    if (path.Length > 0)
                    {
                        var movePos = path.Last().To2D();

                        if (movePos.Distance(lastPosInfo.position) < 5) //more strict checking
                        {
                            var posInfo = EvadeHelper.CanHeroWalkToPos(movePos, ObjectCache.myHeroCache.moveSpeed, 0, 0,
                                false);
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
                    movePos = movePos + (movePos - ObjectCache.myHeroCache.serverPos2D).Normalized()
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

                if (lastPosInfo != null) //&& lastPosInfo.dodgeableSpells.Contains(spell.spellID))
                {
                    if (myHero.ServerPosition.To2D().InSkillShot(spell, ObjectCache.myHeroCache.boundingRadius))
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
                        || !(myHero.Path.Count() > 0 && lastPosInfo.position.Distance(myHero.Path.Last().To2D()) < 5))
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
                    var movePos = path[path.Length - 1].To2D();

                    if (EvadeHelper.CheckMovePath(movePos))
                    {
                        /*if (ObjectCache.menuCache.cache["AllowCrossing"].Cast<CheckBox>().CurrentValue)
                        {
                            var extraDelayBuffer = ObjectCache.menuCache.cache["ExtraPingBuffer"]
                                .Cast<Slider>().CurrentValue + 30;
                            var extraDist = ObjectCache.menuCache.cache["ExtraCPADistance"]
                                .Cast<Slider>().CurrentValue + 10;

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
            if (ObjectCache.menuCache.cache["FastMovementBlock"].Cast<CheckBox>().CurrentValue)
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
                if (ObjectCache.menuCache.cache["DodgeDangerousKey"].Cast<KeyBind>().CurrentValue == true
                    || ObjectCache.menuCache.cache["DodgeDangerousKey2"].Cast<KeyBind>().CurrentValue == true)
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
                    var calculationTimer = EvadeUtils.TickCount;

                    var posInfo = EvadeHelper.GetBestPosition();

                    var caculationTime = EvadeUtils.TickCount - calculationTimer;

                    if (numCalculationTime > 0)
                    {
                        sumCalculationTime += caculationTime;
                        avgCalculationTime = sumCalculationTime / numCalculationTime;
                    }
                    numCalculationTime += 1;

                    //Console.WriteLine("CalculationTime: " + caculationTime);

                    /*if (EvadeHelper.GetHighestDetectedSpellID() > EvadeHelper.GetHighestSpellID(posInfo))
                    {
                        return;
                    }*/
                    if (posInfo != null)
                    {
                        lastPosInfo = posInfo.CompareLastMovePos();

                        var travelTime = ObjectCache.myHeroCache.serverPos2DPing.Distance(lastPosInfo.position) /
                                         myHero.MoveSpeed;

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