using System;
using System.Collections.Generic;
using System.Linq;
using AdEvade.Config;
using AdEvade.Config.Controls;
using AdEvade.Draw;
using AdEvade.Utils;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using SharpDX;
using Color = System.Drawing.Color;

namespace AdEvade.Helpers
{
    class AutoSetPing
    {
        public static AIHeroClient MyHero { get { return ObjectManager.Player; } }

        private static float _sumExtraDelayTime = 0;
        private static float _avgExtraDelayTime = 0;
        private static float _numExtraDelayTime = 0;

        private static float _maxExtraDelayTime = 0;

        private static PlayerIssueOrderEventArgs _lastIssueOrderArgs;
        private static Vector2 _lastMoveToServerPos;
        private static Vector2 _lastPathEndPos;

        private static SpellbookCastSpellEventArgs _lastSpellCastArgs;
        private static Vector2 _lastSpellCastServerPos;
        private static Vector2 _lastSpellCastEndPos;

        private static float _testSkillshotDelayStart = 0;
        private static bool _testSkillshotDelayOn = false;

        private static bool _checkPing = true;

        private static List<float> _pingList = new List<float>();

        public static Menu Menu;

        public AutoSetPing(Menu mainMenu)
        {
            Obj_AI_Base.OnNewPath += Hero_OnNewPath;
            Player.OnIssueOrder += Hero_OnIssueOrder;

            Spellbook.OnCastSpell += Game_OnCastSpell;
            GameObject.OnCreate += Game_OnCreateObj;
            Obj_AI_Base.OnProcessSpellCast += Game_ProcessSpell;

            //Game.OnUpdate += Game_OnUpdate;

            //Drawing.OnDraw += Game_OnDraw;

            Menu autoSetPingMenu = mainMenu.AddSubMenu("AutoSetPing", "AutoSetPingMenu");
            autoSetPingMenu.Add(ConfigValue.AutoSetPing.Name(), new DynamicCheckBox(ConfigDataType.Data, ConfigValue.AutoSetPing, "Auto Set Ping", true).CheckBox);
            autoSetPingMenu.Add(ConfigValue.AutoSetPingPercentile.Name(), new DynamicSlider(ConfigDataType.Data, ConfigValue.AutoSetPingPercentile, "Auto Set Percentile", 75, 0, 100).Slider);


            //autoSetPingMenu.AddItem(new MenuItem("TestSkillshotDelay", "TestSkillshotDelay").SetValue<bool>(false));

            Menu = mainMenu;
        }

        private void Game_ProcessSpell(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!sender.IsMe)
            {
                return;
            }

            //lastSpellCastServerPos = myHero.Position.To2D();
        }

        private void Game_OnDraw(EventArgs args)
        {
            Render.Circle.DrawCircle(MyHero.Position, 10, Color.Red, 5);
            Render.Circle.DrawCircle(MyHero.ServerPosition, 10, Color.Red, 5);
        }

        private void Game_OnCastSpell(Spellbook sender, SpellbookCastSpellEventArgs args)
        {
            var hero = sender.Owner;

            _checkPing = false;

            if (!hero.IsMe)
            {
                return;
            }

            _lastSpellCastArgs = args;


            if (MyHero.IsMoving && MyHero.Path.Count() > 0)
            {
                _lastSpellCastServerPos = EvadeUtils.GetGamePosition(MyHero, Game.Ping);
                _lastSpellCastEndPos = MyHero.Path.Last().To2D();
                _checkPing = true;

                RenderObjects.Add(new RenderCircle(_lastSpellCastServerPos, 1000, Color.Green, 10));
            }

        }

        private void Game_OnCreateObj(GameObject sender, EventArgs args)
        {
            var missile = sender as MissileClient;
            if (missile != null && missile.SpellCaster.IsMe)
            {
                if (_lastSpellCastArgs.Process == true
                    )
                {
                    //Draw.RenderObjects.Add(new Draw.RenderPosition(lastSpellCastServerPos, 1000, System.Drawing.Color.Red, 10));
                    RenderObjects.Add(new RenderCircle(missile.StartPosition.To2D(), 1000, Color.Red, 10));

                    var distance = _lastSpellCastServerPos.Distance(missile.StartPosition.To2D());
                    float moveTime = 1000 * distance / MyHero.MoveSpeed;
                    ConsoleDebug.WriteLine("Extra Delay: " + moveTime);
                }
            }
        }

        private void Hero_OnIssueOrder(Obj_AI_Base hero, PlayerIssueOrderEventArgs args)
        {
            _checkPing = false;

            var distance = MyHero.Position.To2D().Distance(MyHero.ServerPosition.To2D());
            float moveTime = 1000 * distance / MyHero.MoveSpeed;
            //ConsoleDebug.WriteLine("Extra Delay: " + moveTime);

            if (!Config.Properties.GetBool(ConfigValue.AutoSetPing))
            {
                return;
            }

            if (!hero.IsMe)
            {
                return;
            }

            _lastIssueOrderArgs = args;

            if (args.Order == GameObjectOrder.MoveTo)
            {
                if (MyHero.IsMoving && MyHero.Path.Count() > 0)
                {
                    _lastMoveToServerPos = MyHero.ServerPosition.To2D();
                    _lastPathEndPos = MyHero.Path.Last().To2D();
                    _checkPing = true;
                }
            }
        }

        private void Hero_OnNewPath(Obj_AI_Base hero, GameObjectNewPathEventArgs args)
        {
            if (!Config.Properties.GetBool(ConfigValue.AutoSetPing))
            {
                return;
            }

            if (!hero.IsMe)
            {
                return;
            }

            var path = args.Path;

            if (path.Length > 1 && !args.IsDash)
            {
                var movePos = path.Last().To2D();

                if (_checkPing
                    && _lastIssueOrderArgs.Process == true
                    && _lastIssueOrderArgs.Order == GameObjectOrder.MoveTo
                    && _lastIssueOrderArgs.TargetPosition.To2D().Distance(movePos) < 3
                    && MyHero.Path.Count() == 1
                    && args.Path.Count() == 2
                    && MyHero.IsMoving)
                {
                    //Draw.RenderObjects.Add(new Draw.RenderPosition(myHero.Path.Last().To2D(), 1000));

                    RenderObjects.Add(new RenderLine(args.Path.First().To2D(), args.Path.Last().To2D(), 1000));
                    RenderObjects.Add(new RenderLine(MyHero.Position.To2D(), MyHero.Path.Last().To2D(), 1000));

                    //Draw.RenderObjects.Add(new Draw.RenderCircle(lastMoveToServerPos, 1000, System.Drawing.Color.Red, 10));

                    var distanceTillEnd = MyHero.Path.Last().To2D().Distance(MyHero.Position.To2D());
                    float moveTimeTillEnd = 1000 * distanceTillEnd / MyHero.MoveSpeed;

                    if (moveTimeTillEnd < 500)
                    {
                        return;
                    }

                    var dir1 = (MyHero.Path.Last().To2D() - MyHero.Position.To2D()).Normalized();
                    var ray1 = new Ray(MyHero.Position.SetZ(0), new Vector3(dir1.X, dir1.Y, 0));

                    var dir2 = (args.Path.First().To2D() - args.Path.Last().To2D()).Normalized();
                    var pos2 = new Vector3(args.Path.First().X, args.Path.First().Y, 0);
                    var ray2 = new Ray(args.Path.First().SetZ(0), new Vector3(dir2.X, dir2.Y, 0));

                    Vector3 intersection3;
                    if (ray2.Intersects(ref ray1, out intersection3))
                    {
                        var intersection = intersection3.To2D();

                        var projection = intersection.ProjectOn(MyHero.Path.Last().To2D(), MyHero.Position.To2D());

                        if (projection.IsOnSegment && dir1.AngleBetween(dir2) > 20 && dir1.AngleBetween(dir2) < 160)
                        {
                            RenderObjects.Add(new RenderCircle(intersection, 1000, Color.Red, 10));

                            var distance = //args.Path.First().To2D().Distance(intersection);
                                _lastMoveToServerPos.Distance(intersection);
                            float moveTime = 1000 * distance / MyHero.MoveSpeed;

                            //ConsoleDebug.WriteLine("waa: " + distance);

                            if (moveTime < 1000)
                            {
                                if (_numExtraDelayTime > 0)
                                {
                                    _sumExtraDelayTime += moveTime;
                                    _avgExtraDelayTime = _sumExtraDelayTime / _numExtraDelayTime;

                                    _pingList.Add(moveTime);
                                }
                                _numExtraDelayTime += 1;

                                if (_maxExtraDelayTime == 0)
                                {
                                    _maxExtraDelayTime = Config.Properties.GetInt(ConfigValue.ExtraPingBuffer);
                                }

                                if (_numExtraDelayTime % 100 == 0)
                                {
                                    _pingList.Sort();

                                    var percentile = ConfigValue.AutoSetPingPercentile.GetInt();
                                    int percentIndex = (int)Math.Floor(_pingList.Count() * (percentile / 100f)) - 1;
                                    _maxExtraDelayTime = Math.Max(_pingList.ElementAt(percentIndex) - Game.Ping,0);
                                    _maxExtraDelayTime.SetTo(ConfigValue.ExtraPingBuffer);

                                    _pingList.Clear();

                                    ConsoleDebug.WriteLine("Max Extra Delay: " + _maxExtraDelayTime);
                                }

                                ConsoleDebug.WriteLine("Extra Delay: " + Math.Max(moveTime - Game.Ping,0));
                            }
                        }
                    }
                }

                _checkPing = false;
            }
        }
    }
}
