using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using EloBuddy;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;

namespace ezEvade
{
    class PingTester
    {
        public static Menu testMenu;

        private static AIHeroClient myHero { get { return ObjectManager.Player; } }

        private static bool lastRandomMoveCoeff = false;

        private static float sumPingTime = 0;
        private static float averagePingTime = ObjectCache.gamePing;
        private static int testCount = 0;
        private static int autoTestCount = 0;
        private static float maxPingTime = ObjectCache.gamePing;

        private static bool autoTestPing = false;

        private static EvadeCommand lastTestMoveToCommand;

        public PingTester()
        {
            Game.OnUpdate += Game_OnGameUpdate;
            
            testMenu = Evade.menu.AddSubMenu("Ping Tester", "PingTest");
            testMenu.Add("AutoSetPing", new CheckBox("Auto Set Ping", false));
            testMenu.Add("TestMoveTime", new CheckBox("Test Ping", false));
            testMenu.Add("SetMaxPing", new CheckBox("Set Max Ping", false));
            testMenu.Add("SetAvgPing", new CheckBox("Set Avg Ping", false));
            testMenu.Add("Test20MoveTime", new CheckBox("Test Ping x20", false));
            testMenu.Add("PrintResults", new CheckBox("Print Results", false));
        }

        private void IssueTestMove(int recursionCount)
        {

            var movePos = ObjectCache.myHeroCache.serverPos2D;

            Random rand = new Random();

            lastRandomMoveCoeff = !lastRandomMoveCoeff;
            if (lastRandomMoveCoeff)
            {
                movePos.X += 65 + rand.Next(0, 20);
            }
            else
            {
                movePos.X -= 65 + rand.Next(0, 20);
            }

            lastTestMoveToCommand = new EvadeCommand
            {
                order = EvadeOrderCommand.MoveTo,
                targetPosition = movePos,
                timestamp = EvadeUtils.TickCount,
                isProcessed = false
            };

            Player.IssueOrder(GameObjectOrder.MoveTo, movePos.To3D(), true);

            if (recursionCount > 1)
            {
                DelayAction.Add(500, () => IssueTestMove(recursionCount - 1));
            }

        }

        private void SetPing(int ping)
        {
            testMenu["ExtraPingBuffer"].Cast<Slider>().CurrentValue = ping;
        }

        private void Game_OnGameUpdate(EventArgs args)
        {
            if (testMenu["AutoSetPing"].Cast<CheckBox>().CurrentValue)
            {
                Console.WriteLine("Testing Ping...Please wait 10 seconds");

                int testAmount = 20;

                testMenu["AutoSetPing"].Cast<CheckBox>().CurrentValue = false;
                IssueTestMove(testAmount);
                autoTestCount = testCount + testAmount;
                autoTestPing = true;
                
            }

            if (testMenu["PrintResults"].Cast<CheckBox>().CurrentValue)
            {
                testMenu["PrintResults"].Cast<CheckBox>().CurrentValue = false;

                Console.WriteLine("Average Extra Delay: " + averagePingTime);
                Console.WriteLine("Max Extra Delay: " + maxPingTime);
            }

            if (autoTestPing == true && testCount >= autoTestCount)
            {
                Console.WriteLine("Auto Set Ping Complete");

                Console.WriteLine("Average Extra Delay: " + averagePingTime);
                Console.WriteLine("Max Extra Delay: " + maxPingTime);

                SetPing((int)(averagePingTime+10));
                Console.WriteLine("Set Average extra ping + 10: " + (averagePingTime+10));

                autoTestPing = false;
            }

            if (testMenu["TestMoveTime"].Cast<CheckBox>().CurrentValue)
            {
                testMenu["TestMoveTime"].Cast<CheckBox>().CurrentValue = false;
                IssueTestMove(1);
            }


            if (testMenu["Test20MoveTime"].Cast<CheckBox>().CurrentValue)
            {
                testMenu["Test20MoveTime"].Cast<CheckBox>().CurrentValue = false;
                IssueTestMove(20);
            }

            if (testMenu["SetMaxPing"].Cast<CheckBox>().CurrentValue)
            {
                testMenu["SetMaxPing"].Cast<CheckBox>().CurrentValue = false;

                if (testCount < 10)
                {
                    Console.WriteLine("Please test 10 times before setting ping");
                }
                else
                {
                    Console.WriteLine("Set Max extra ping: " + maxPingTime);
                    SetPing((int)maxPingTime);
                }                
            }

            if (testMenu["SetAvgPing"].Cast<CheckBox>().CurrentValue)
            {
                testMenu["SetAvgPing"].Cast<CheckBox>().CurrentValue = false;

                if (testCount < 10)
                {
                    Console.WriteLine("Please test 10 times before setting ping");
                }
                else
                {
                    Console.WriteLine("Set Average extra ping: " + averagePingTime);
                    SetPing((int)averagePingTime);
                }                         
            }

            if (myHero.IsMoving)
            {
                if (lastTestMoveToCommand != null && lastTestMoveToCommand.isProcessed == false && lastTestMoveToCommand.order == EvadeOrderCommand.MoveTo)
                {
                    var path = myHero.Path;

                    if (path.Length > 0)
                    {
                        var movePos = path[path.Length - 1].LSTo2D();

                        if (movePos.LSDistance(lastTestMoveToCommand.targetPosition) < 10)
                        {
                            float moveTime = EvadeUtils.TickCount - lastTestMoveToCommand.timestamp - ObjectCache.gamePing;
                            Console.WriteLine("Extra Delay: " + moveTime);
                            lastTestMoveToCommand.isProcessed = true;

                            sumPingTime += moveTime;
                            testCount += 1;
                            averagePingTime = sumPingTime / testCount;
                            maxPingTime = Math.Max(maxPingTime, moveTime);
                        }
                    }

                }
            }
        }
    }
}
