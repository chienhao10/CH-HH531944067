using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using LeagueSharp;
using LeagueSharp.SDK;
using LeagueSharp.SDK.Core;
using LeagueSharp.SDK.Core.Utils;
using SharpDX;
using EloBuddy;
using Color = System.Drawing.Color;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK;

namespace HTrackerSDK
{
    class WardData
    {
        public float Duration = int.MaxValue;
        public string ObjectName = "visionward";
        public float Range = 1100;
        public string SpellName = "visionward";
    }

    class WardDatabase
    {
        // ReSharper disable once InconsistentNaming
        public static List<WardData> wardDatabase = new List<WardData>();
        public static Dictionary<string, WardData> WardspellNames = new Dictionary<string, WardData>();
        public static Dictionary<string, WardData> WardObjNames = new Dictionary<string, WardData>();
        public static WardData MissileWardData;

        static WardDatabase()
        {
            LoadWardDatabase();
            LoadWardDictionary();
        }

        private static void LoadWardDictionary()
        {
            foreach (var ward in wardDatabase)
            {
                var spellName = ward.SpellName.ToLower();
                if (!WardspellNames.ContainsKey(spellName))
                {
                    WardspellNames.Add(spellName, ward);
                }

                var objName = ward.ObjectName.ToLower();
                if (!WardObjNames.ContainsKey(objName))
                {
                    WardObjNames.Add(objName, ward);
                }
            }
        }

        private static void LoadWardDatabase()
        {
            //Trinkets:
            wardDatabase.Add(
            new WardData
            {
                Duration = 1 * 60 * 1000,
                ObjectName = "YellowTrinket",
                Range = 1100,
                SpellName = "TrinketTotemLvl1",
            });

            wardDatabase.Add(
            new WardData
            {
                Duration = 2 * 60 * 1000,
                ObjectName = "YellowTrinketUpgrade",
                Range = 1100,
                SpellName = "TrinketTotemLvl2",
            });

            wardDatabase.Add(
            new WardData
            {
                Duration = int.MaxValue,
                ObjectName = "VisionWard",
                Range = 1100,
                SpellName = "VisionWard",
            });

            wardDatabase.Add(
            new WardData
            {
                Duration = int.MaxValue,
                ObjectName = "BlueTrinket",
                Range = 1100,
                SpellName = "TrinketOrbLvl3",
            });


            wardDatabase.Add(
            new WardData
            {
                Duration = 3 * 60 * 1000,
                ObjectName = "SightWard",
                Range = 1100,
                SpellName = "TrinketTotemLvl3",
            });
            //Ward items and normal wards:
            wardDatabase.Add(
            new WardData
            {
                Duration = 3 * 60 * 1000,
                ObjectName = "SightWard",
                Range = 1100,
                SpellName = "SightWard",
            });

            wardDatabase.Add(
            new WardData
            {
                Duration = 3 * 60 * 1000,
                ObjectName = "SightWard",
                Range = 1100,
                SpellName = "ItemGhostWard",
            });

            MissileWardData =
            new WardData
            {
                Duration = 3 * 60 * 1000,
                ObjectName = "MissileWard",
                Range = 1100,
                SpellName = "MissileWard",
            };
        }
    }
    class WardTrackerInfo
    {
        public WardData WardData;
        public Vector3 Position;
        public Obj_AI_Base WardObject;
        public float Timestamp;
        public float EndTime;
        public bool UnknownDuration;
        public Vector2 StartPos;
        public Vector2 EndPos;

        public WardTrackerInfo(WardData wardData, Vector3 position, Obj_AI_Base wardObject, bool fromMissile = false, float timestamp = 0)
        {
            WardData = wardData;
            Position = position;
            WardObject = wardObject;
            UnknownDuration = fromMissile;
            Timestamp = timestamp == 0 ? Variables.TickCount : timestamp;
            EndTime = Timestamp + wardData.Duration;
        }

    }

    class WardTracker
    {
        public static List<WardTrackerInfo> Wards = new List<WardTrackerInfo>();
        public static float LastCheckExpiredWards;

        public static Menu wardMenu;

        public static bool getCheckBoxItem(string item)
        {
            return wardMenu[item].Cast<CheckBox>().CurrentValue;
        }

        public static int getSliderItem(string item)
        {
            return wardMenu[item].Cast<Slider>().CurrentValue;
        }

        public static bool getKeyBindItem(string item)
        {
            return wardMenu[item].Cast<KeyBind>().CurrentValue;
        }

        public static int getBoxItem(string item)
        {
            return wardMenu[item].Cast<ComboBox>().CurrentValue;
        }

        public static void OnLoad(Menu m)
        {
            wardMenu = m.AddSubMenu("Ward Tracker", "Ward Tracker");
            wardMenu.Add("enemy.ward", new CheckBox("Track Enemy Wards", true));

            Obj_AI_Base.OnProcessSpellCast += Game_OnProcessSpell;
            GameObject.OnCreate += Game_OnCreateObj;
            GameObject.OnDelete += Game_OnDeleteObj;
            Game.OnUpdate += Game_OnUpdate;
            Drawing.OnDraw += Drawing_OnDraw;
            Drawing.OnEndScene += DrawingOnOnEndScene;
            InitWards();
        }


        static WardTracker()
        {
            LastCheckExpiredWards = 0;
        }

        public static void DrawText(float x, float y, Color c, string text)
        {
            if (text != null)
            {
                Drawing.DrawText(x, y, c, text);
            }
        }
        public static string FormatTime(float time)
        {
            if (time > 0)
            {
                return Convert.ToString(time, CultureInfo.InvariantCulture);
            }
            else
            {
                return "00.00";
            }
        }
        private static void InitWards()
        {
            foreach (var obj in ObjectManager.Get<Obj_AI_Minion>())
            {
                if (obj != null && obj.IsValid && !obj.IsDead)
                {
                    Game_OnCreateObj(obj, null);
                }
            }
        }

        private static void Game_OnDeleteObj(GameObject sender, EventArgs args)
        {
            if (!getCheckBoxItem("enemy.ward"))
            {
                return;
            }

            if (sender.Type == GameObjectType.obj_AI_Minion)
            {
                foreach (var ward in Wards)
                {
                    if (ward.WardObject != null && ward.WardObject.NetworkId == sender.NetworkId)
                    {
                        var ward1 = ward;
                        DelayAction.Add(0, () => Wards.Remove(ward1));
                    }
                }
            }
        }

        private static void Game_OnUpdate(EventArgs args)
        {
            if (!getCheckBoxItem("enemy.ward"))
            {
                return;
            }

            if (Variables.TickCount > LastCheckExpiredWards)
            {
                CheckExpiredWards();
                LastCheckExpiredWards = Variables.TickCount + 500;
            }

        }

        private static void CheckExpiredWards()
        {
            foreach (var ward in Wards)
            {
                if (Variables.TickCount > ward.EndTime)
                {
                    var ward1 = ward;
                    DelayAction.Add(0, () => Wards.Remove(ward1));
                }

                if (ward.WardObject != null && (ward.WardObject.IsDead || !ward.WardObject.IsValid))
                {
                    var ward1 = ward;
                    DelayAction.Add(0, () => Wards.Remove(ward1));
                }
            }
        }

        private static void Game_OnProcessSpell(Obj_AI_Base hero, GameObjectProcessSpellCastEventArgs args)
        {
            var heroA = hero as AIHeroClient;
            if (!getCheckBoxItem("enemy.ward"))
            {
                return;
            }

            if (hero.IsEnemy && hero is AIHeroClient && hero.LSIsValid())
            {
                WardData wardData;
                if (WardDatabase.WardspellNames.TryGetValue(args.SData.Name.ToLower(), out wardData))
                {
                    var pos = args.End.ToVector2();

                    if (args.SData.Name.ToLower().Contains("TrinketTotem") || args.SData.Name.ToLower().Contains("trinkettotem"))
                    {
                        wardData.Duration = 1000 * (60 + (int)Math.Round(3.5 * (heroA.Level - 1))); // Not sure.
                    }

                    DelayAction.Add((float)50, () =>
                    {
                        if (Wards.Any(ward => ward.Position.ToVector2().Distance(pos) < 50 && Variables.TickCount - ward.Timestamp < 100))
                        {
                            return;
                        }

                        var newWard = new WardTrackerInfo(wardData, pos.To3D(), null)
                        {
                            StartPos = args.Start.ToVector2(),
                            EndPos = args.End.ToVector2()
                        };

                        Wards.Add(newWard);
                    });
                }
            }
        }

        private static void Game_OnCreateObj(GameObject sender, EventArgs args)
        {

            if (!getCheckBoxItem("enemy.ward"))
            {
                return;
            }

            //Visible ward placement
            var obj = sender as Obj_AI_Minion;
            WardData wardData;

            if (obj != null && obj.IsEnemy
                && WardDatabase.WardObjNames.TryGetValue(obj.CharData.BaseSkinName.ToLower(), out wardData))
            {
                var timestamp = Variables.TickCount - (obj.MaxMana - obj.Mana) * 1000;

                WardTrackerInfo newWard = new WardTrackerInfo(
                            wardData,
                            obj.Position,
                            obj,
                            !obj.IsVisible && args == null,
                            timestamp
                            );

                Wards.Add(newWard);

                DelayAction.Add((float)500, () =>
                {
                    if (newWard.WardObject != null && newWard.WardObject.IsValid && !newWard.WardObject.IsDead)
                    {
                        timestamp = Variables.TickCount - (obj.MaxMana - obj.Mana) * 1000;

                        newWard.Timestamp = timestamp;

                        foreach (var ward in Wards)
                        {
                            if (ward.WardObject == null)
                            {
                                //Check for Process Spell wards
                                if (ward.Position.Distance(sender.Position) < 550
                                        && Math.Abs(ward.Timestamp - timestamp) < 2000)
                                {
                                    var ward1 = ward;
                                    DelayAction.Add(0, () => Wards.Remove(ward1));
                                    break;
                                }
                            }
                        }
                    }
                });

            }

            //FOW placement
            var missile = sender as MissileClient;

            if (missile != null && missile.SpellCaster.IsEnemy)
            {
                if (missile.SData.Name.ToLower() == "itemplacementmissile")// && !missile.SpellCaster.IsVisible)
                {
                    var dir = (missile.EndPosition.ToVector2() - missile.StartPosition.ToVector2()).LSNormalized();
                    var pos = missile.StartPosition.ToVector2() + dir * 500;

                    if (Wards.Any(ward => ward.Position.ToVector2().Distance(pos) < 750
                                          && Variables.TickCount - ward.Timestamp < 50))
                    {
                        return;
                    }

                    DelayAction.Add((float)100, () =>
                    {
                        if (Wards.Any(ward => ward.Position.ToVector2().Distance(pos) < 750
                                              && Variables.TickCount - ward.Timestamp < 125))
                        {
                            return;
                        }

                        var newWard = new WardTrackerInfo(
                            WardDatabase.MissileWardData,
                            pos.ToVector3(),
                            null,
                            true
                            )
                        {
                            StartPos = missile.StartPosition.ToVector2(),
                            EndPos = missile.EndPosition.ToVector2()
                        };


                        Wards.Add(newWard);
                    });
                }
            }
        }

        private static void DrawingOnOnEndScene(EventArgs args)
        {
            if (!getCheckBoxItem("enemy.ward"))
            {
                return;
            }

            foreach (var ward in Wards)
            {
                var wardPos = ward.WardObject != null ? ward.WardObject.Position : ward.Position;

                if (ward.EndTime > Variables.TickCount)
                {
                    var wardScreenPos = Drawing.WorldToScreen(wardPos);
                    var timeStr = FormatTime((ward.EndTime - Variables.TickCount) / 1000f);

                    if (timeStr != null)
                    {
                        if (ward.WardData.ObjectName == "VisionWard")
                        {
                            LeagueSharp.Common.Utility.DrawCircle(wardPos, 300, Color.DeepPink, 1, 3, true);
                        }
                        else if (ward.WardData.ObjectName == "BlueTrinket")
                        {
                            LeagueSharp.Common.Utility.DrawCircle(wardPos, 300, Color.DodgerBlue, 1, 3, true);
                        }
                        else
                        {
                            LeagueSharp.Common.Utility.DrawCircle(wardPos, 300, Color.LawnGreen, 1, 3, true);
                        }
                    }
                }
            }
        }

        private static void Drawing_OnDraw(EventArgs args)
        {
            if (!getCheckBoxItem("enemy.ward"))
            {
                return;
            }

            foreach (var ward in Wards)
            {
                var wardPos = ward.WardObject != null ? ward.WardObject.Position : ward.Position;

                if (wardPos.IsOnScreen() && ward.EndTime > Variables.TickCount)
                {
                    var wardScreenPos = Drawing.WorldToScreen(wardPos);
                    var timeStr = FormatTime((ward.EndTime - Variables.TickCount) / 1000f);

                    if (timeStr != null)
                    {
                        if (ward.WardData.ObjectName == "VisionWard")
                        {
                            Drawing.DrawCircle(wardPos, 100, Color.DeepPink);
                            Drawing.DrawText(wardScreenPos.X - 50 / 2, wardScreenPos.Y, Color.DeepPink, "Pink");
                        }
                        else if (ward.WardData.ObjectName == "BlueTrinket")
                        {
                            Drawing.DrawCircle(wardPos, 100, Color.DodgerBlue);
                            Drawing.DrawText(wardScreenPos.X - 50 / 2, wardScreenPos.Y, Color.DodgerBlue, "Blue");
                        }
                        else
                        {
                            Drawing.DrawCircle(wardPos, 100, Color.LawnGreen);
                            Drawing.DrawText(wardScreenPos.X - 50 / 2, wardScreenPos.Y, Color.White, "Ward");
                            Drawing.DrawText(wardScreenPos.X - 60 / 2, wardScreenPos.Y + 30, Color.LawnGreen, "" + timeStr);
                        }
                    }
                }
            }
        }
    }
}