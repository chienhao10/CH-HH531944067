/*
██████╗ ██╗   ██╗     █████╗ ██╗     ██████╗ ██╗  ██╗ █████╗  ██████╗  ██████╗ ██████╗ 
██╔══██╗╚██╗ ██╔╝    ██╔══██╗██║     ██╔══██╗██║  ██║██╔══██╗██╔════╝ ██╔═══██╗██╔══██╗
██████╔╝ ╚████╔╝     ███████║██║     ██████╔╝███████║███████║██║  ███╗██║   ██║██║  ██║
██╔══██╗  ╚██╔╝      ██╔══██║██║     ██╔═══╝ ██╔══██║██╔══██║██║   ██║██║   ██║██║  ██║
██████╔╝   ██║       ██║  ██║███████╗██║     ██║  ██║██║  ██║╚██████╔╝╚██████╔╝██████╔╝
╚═════╝    ╚═╝       ╚═╝  ╚═╝╚══════╝╚═╝     ╚═╝  ╚═╝╚═╝  ╚═╝ ╚═════╝  ╚═════╝ ╚═════╝ 
+ Credits:
 Based on Jungle.cs made by Lizzaran in SFXUtility (Copyright 2014 - 2015 Nikita Bernthaler)
*/

#region

using System;
using System.Collections.Generic;
using EloBuddy;
using SharpDX;
using Color = System.Drawing.Color;
using LeagueSharp.Common;

#endregion

namespace GodJungleTracker.Classes
{
    public class Jungle
    {
        public static List<Camp> Camps;

        static Jungle()
        {
            try
            {
                Camps = new List<Camp>
                {
                    // Order: Blue
                    new Camp("Blue",
                        115, 300, new Vector3(3872f, 7900f, 51f),
                        new List<Mob>(
                            new[]
                            {
                                new Mob("SRU_Blue1.1.1"), 
                                new Mob("SRU_BlueMini1.1.2", true),
                                new Mob("SRU_BlueMini21.1.3", true)
                            }), 
                        LeagueSharp.Common.Utility.Map.MapType.SummonersRift,
                        GameObjectTeam.Order,
                        Color.Cyan, new Timers(new Vector2(0,0),new Vector2(0,0)), true),
                    //Order: Wolves
                    new Camp("Wolves",
                        115, 100, new Vector3(3825f, 6491f, 52f),
                        new List<Mob>(
                            new[]
                            {
                                new Mob("SRU_Murkwolf2.1.1"), 
                                new Mob("SRU_MurkwolfMini2.1.2"),
                                new Mob("SRU_MurkwolfMini2.1.3")
                            }), 
                        LeagueSharp.Common.Utility.Map.MapType.SummonersRift,
                        GameObjectTeam.Order,
                        Color.White, new Timers(new Vector2(0,0),new Vector2(0,0))),
                    //Order: Raptor
                    new Camp("Raptor",
                        115, 100, new Vector3(6954f, 5458f, 53f),
                        new List<Mob>(
                            new[]
                            {
                                new Mob("SRU_Razorbeak3.1.1", true), 
                                new Mob("SRU_RazorbeakMini3.1.2"),
                                new Mob("SRU_RazorbeakMini3.1.3"), 
                                new Mob("SRU_RazorbeakMini3.1.4")
                            }), 
                        LeagueSharp.Common.Utility.Map.MapType.SummonersRift, 
                        GameObjectTeam.Order,
                        Color.Salmon, new Timers(new Vector2(0,0),new Vector2(0,0)), true),
                    //Order: Red
                    new Camp("Red",
                        115, 300, new Vector3(7862f, 4111f, 54f),
                        new List<Mob>(
                            new[]
                            { 
                                new Mob("SRU_Red4.1.1"), 
                                new Mob("SRU_RedMini4.1.2", true), 
                                new Mob("SRU_RedMini4.1.3", true) 
                            }), 
                        LeagueSharp.Common.Utility.Map.MapType.SummonersRift, 
                        GameObjectTeam.Order,
                        Color.Red, new Timers(new Vector2(0,0),new Vector2(0,0)), true),
                        
                    //Order: Krug
                    new Camp("Krug",
                        115, 100, new Vector3(8381f, 2711f, 51f),
                        new List<Mob>(
                            new[] 
                            { 
                                new Mob("SRU_Krug5.1.2"), 
                                new Mob("SRU_KrugMini5.1.1") 
                            }), 
                        LeagueSharp.Common.Utility.Map.MapType.SummonersRift, 
                        GameObjectTeam.Order,
                        Color.White, new Timers(new Vector2(0,0),new Vector2(0,0))),
                    //Order: Gromp
                    new Camp("Gromp",
                        115, 100, new Vector3(2091f, 8428f, 52f),
                        new List<Mob>(
                            new[] 
                            { 
                                new Mob("SRU_Gromp13.1.1", true) 
                            }), 
                        LeagueSharp.Common.Utility.Map.MapType.SummonersRift, 
                        GameObjectTeam.Order,
                        Color.Green, new Timers(new Vector2(0,0),new Vector2(0,0)), true),
                    //Chaos: Blue
                    new Camp("Blue",
                        115, 300, new Vector3(10930f, 6992f, 52f),
                        new List<Mob>(
                            new[]
                            {
                                new Mob("SRU_Blue7.1.1"), 
                                new Mob("SRU_BlueMini7.1.2", true),
                                new Mob("SRU_BlueMini27.1.3", true)
                            }), 
                        LeagueSharp.Common.Utility.Map.MapType.SummonersRift,
                        GameObjectTeam.Chaos,
                        Color.Cyan, new Timers(new Vector2(0,0),new Vector2(0,0)), true),
                    //Chaos: Wolves
                    new Camp("Wolves",
                        115, 100, new Vector3(10957f, 8350f, 62f),
                        new List<Mob>(
                            new[]
                            {
                                new Mob("SRU_Murkwolf8.1.1"), 
                                new Mob("SRU_MurkwolfMini8.1.2"),
                                new Mob("SRU_MurkwolfMini8.1.3")
                            }), 
                        LeagueSharp.Common.Utility.Map.MapType.SummonersRift,
                        GameObjectTeam.Chaos,
                        Color.White, new Timers(new Vector2(0,0),new Vector2(0,0))),
                    //Chaos: Raptor
                    new Camp("Raptor",
                        115, 100, new Vector3(7857f, 9471f, 52f),
                        new List<Mob>(
                            new[]
                            {
                                new Mob("SRU_Razorbeak9.1.1", true), 
                                new Mob("SRU_RazorbeakMini9.1.2"),
                                new Mob("SRU_RazorbeakMini9.1.3"), 
                                new Mob("SRU_RazorbeakMini9.1.4")
                            }), 
                        LeagueSharp.Common.Utility.Map.MapType.SummonersRift, 
                        GameObjectTeam.Chaos,
                        Color.Salmon, new Timers(new Vector2(0,0),new Vector2(0,0)), true),
                    //Chaos: Red
                    new Camp("Red",
                        115, 300, new Vector3(7017f, 10775f, 56f),
                        new List<Mob>(
                            new[]
                            {
                                new Mob("SRU_Red10.1.1"), 
                                new Mob("SRU_RedMini10.1.2", true), 
                                new Mob("SRU_RedMini10.1.3", true)
                            }), 
                        LeagueSharp.Common.Utility.Map.MapType.SummonersRift, 
                        GameObjectTeam.Chaos,
                        Color.Red, new Timers(new Vector2(0,0),new Vector2(0,0)), true),
                    //Chaos: Krug
                    new Camp("Krug",
                        115, 100, new Vector3(6449f, 12117f, 56f),
                        new List<Mob>(
                            new[] 
                            { 
                                new Mob("SRU_Krug11.1.2"), 
                                new Mob("SRU_KrugMini11.1.1") 
                            }), 
                        LeagueSharp.Common.Utility.Map.MapType.SummonersRift, 
                        GameObjectTeam.Chaos,
                        Color.White, new Timers(new Vector2(0,0),new Vector2(0,0))),
                    //Chaos: Gromp
                    new Camp("Gromp",
                        115, 100, new Vector3(12703f, 6444f, 52f),
                        new List<Mob>(
                            new[] 
                            { 
                                new Mob("SRU_Gromp14.1.1", true) 
                            }),
                        LeagueSharp.Common.Utility.Map.MapType.SummonersRift, 
                        GameObjectTeam.Chaos,
                        Color.Green, new Timers(new Vector2(0,0),new Vector2(0,0)), true),
                    //Neutral: Dragon
                    new Camp("Dragon",
                        150, 360, new Vector3(9866f, 4414f, -71f),
                        new List<Mob>(
                            new[] 
                            { 
                                new Mob("SRU_Dragon6.1.1") 
                            }), 
                        LeagueSharp.Common.Utility.Map.MapType.SummonersRift, 
                        GameObjectTeam.Neutral,
                        Color.Orange, new Timers(new Vector2(0,0),new Vector2(0,0))),
                    //Neutral: Baron
                    new Camp("Baron",
                        120, 420, new Vector3(5007f, 10471f, -71f),
                        new List<Mob>(
                            new[] 
                            { 
                                new Mob("SRU_Baron12.1.1", true) 
                            }), 
                        LeagueSharp.Common.Utility.Map.MapType.SummonersRift, 
                        GameObjectTeam.Neutral,
                        Color.DarkOrchid, new Timers(new Vector2(0,0),new Vector2(0,0)), true,  8),
                    //Dragon: Crab
                    new Camp("Crab",
                        150, 180, new Vector3(10508f, 5271f, -62f),
                        new List<Mob>(
                            new[] 
                            { 
                                new Mob("Sru_Crab15.1.1") 
                            }), 
                        LeagueSharp.Common.Utility.Map.MapType.SummonersRift, 
                        GameObjectTeam.Neutral,
                        Color.PaleGreen, new Timers(new Vector2(0,0),new Vector2(0,0))),
                    //Baron: Crab
                    new Camp("Crab",
                        150, 180, new Vector3(4418f, 9664f, -69f),
                        new List<Mob>(
                            new[] 
                            { 
                                new Mob("Sru_Crab16.1.1") 
                            }), 
                        LeagueSharp.Common.Utility.Map.MapType.SummonersRift, 
                        GameObjectTeam.Neutral,
                        Color.PaleGreen, new Timers(new Vector2(0,0),new Vector2(0,0))),
                    //Order: Wraiths
                    new Camp("Wraiths",
                        95, 75, new Vector3(4373f, 5843f, -107f),
                        new List<Mob>(
                            new[]
                            {
                                new Mob("TT_NWraith1.1.1", true), 
                                new Mob("TT_NWraith21.1.2", true), 
                                new Mob("TT_NWraith21.1.3", true)
                            }),  
                        LeagueSharp.Common.Utility.Map.MapType.TwistedTreeline, 
                        GameObjectTeam.Order,
                        Color.White, new Timers(new Vector2(0,0),new Vector2(0,0)), true),
                    //Order: Golems
                    new Camp("Golems",
                        95, 75, new Vector3(5107f, 7986f, -108f),
                        new List<Mob>(
                            new[] 
                            { 
                                new Mob("TT_NGolem2.1.1"), 
                                new Mob("TT_NGolem22.1.2") 
                            }), 
                        LeagueSharp.Common.Utility.Map.MapType.TwistedTreeline, 
                        GameObjectTeam.Order,
                        Color.White, new Timers(new Vector2(0,0),new Vector2(0,0))),
                    //Order: Wolves
                    new Camp("Wolves",
                        95, 75, new Vector3(6078f, 6094f, -99f),
                        new List<Mob>(
                            new[]
                            { 
                                new Mob("TT_NWolf3.1.1"), 
                                new Mob("TT_NWolf23.1.2"), 
                                new Mob("TT_NWolf23.1.3") 
                            }),
                         LeagueSharp.Common.Utility.Map.MapType.TwistedTreeline, 
                         GameObjectTeam.Order,
                        Color.White, new Timers(new Vector2(0,0),new Vector2(0,0))),
                    //Chaos: Wraiths
                    new Camp("Wraiths",
                        95, 75, new Vector3(11026f, 5806f, -107f),
                        new List<Mob>(
                            new[]
                            {
                                new Mob("TT_NWraith4.1.1", true), 
                                new Mob("TT_NWraith24.1.2", true),
                                new Mob("TT_NWraith24.1.3", true)
                            }),  
                        LeagueSharp.Common.Utility.Map.MapType.TwistedTreeline, 
                        GameObjectTeam.Chaos,
                        Color.White, new Timers(new Vector2(0,0),new Vector2(0,0)), true),
                    //Chaos: Golems
                    new Camp("Golems",
                        95, 75, new Vector3(10277f, 8038f, -109f),
                        new List<Mob>(
                            new[] 
                            { 
                                new Mob("TT_NGolem5.1.1"), 
                                new Mob("TT_NGolem25.1.2") 
                            }),
                        LeagueSharp.Common.Utility.Map.MapType.TwistedTreeline, 
                        GameObjectTeam.Chaos,
                        Color.White, new Timers(new Vector2(0,0),new Vector2(0,0))),
                    //Chaos: Wolves
                    new Camp("Wolves",
                        95, 75, new Vector3(9294f, 6085f, -97f),
                        new List<Mob>(
                            new[]
                            { 
                                new Mob("TT_NWolf6.1.1"), 
                                new Mob("TT_NWolf26.1.2"), 
                                new Mob("TT_NWolf26.1.3") }),
                         LeagueSharp.Common.Utility.Map.MapType.TwistedTreeline, 
                         GameObjectTeam.Chaos,
                        Color.White, new Timers(new Vector2(0,0),new Vector2(0,0))),
                    //Neutral: Spider
                    new Camp("Spider",
                        600, 360, new Vector3(7738f, 10080f, -62f),
                        new List<Mob>(
                            new[] 
                            { 
                                new Mob("TT_Spiderboss8.1.1") 
                            }),
                        LeagueSharp.Common.Utility.Map.MapType.TwistedTreeline, 
                        GameObjectTeam.Neutral,
                        Color.DarkOrchid, new Timers(new Vector2(0,0),new Vector2(0,0)), true)
                };
            }
            catch (Exception)
            {
                Camps = new List<Camp>();
            }
        }

        public class Camp
        {
            public Camp(string name,
                float spawnTime,
                int respawnTimer,
                Vector3 position,
                List<Mob> mobs,
                LeagueSharp.Common.Utility.Map.MapType mapType,
                GameObjectTeam team,
                Color colour,
                Timers timer,
                bool isRanged = false,
                int state = 0,
                int respawnTime = 0,
                int lastChangeOnState = 0,
                bool shouldping = true,
                int lastPing = 0)
            {
                Name = name;
                SpawnTime = spawnTime;
                RespawnTimer = respawnTimer;
                Position = position;
                MapPosition = Drawing.WorldToScreen(Position);
                MinimapPosition = Drawing.WorldToMinimap(Position);
                Mobs = mobs;
                MapType = mapType;
                Team = team;
                Colour = colour;
                IsRanged = isRanged;
                State = state;
                RespawnTime = respawnTime;
                LastChangeOnState = lastChangeOnState;
                Timer = timer;
                ShouldPing = shouldping;
                LastPing = lastPing;

                #region Load Text

                TextMinimap = new Render.Text(0, 0, "", Program.getSliderItem("timerfontminimap"), Program.White)
                {
                    VisibleCondition =
                        sender =>
                            Program.Timeronminimap && RespawnTime > Environment.TickCount && State == 7,
                    PositionUpdate = delegate
                    {
                        Vector2 v2 = Timer.MinimapPosition;
                        return v2;
                    },
                    TextUpdate = () => Timer.TextOnMinimap,
                    OutLined = false,
                    Centered = true
                };
                TextMinimap.Add();

                TextMap = new Render.Text(0, 0, "", Program.getSliderItem("timerfontmap"), Program.White)
                {
                    VisibleCondition =
                        sender =>
                            Program.Timeronmap && RespawnTime > Environment.TickCount && State == 7 && Position.LSIsOnScreen(),
                    PositionUpdate = delegate
                    {
                        Vector2 v2 = Timer.Position;
                        return v2;
                    },
                    TextUpdate = () => Timer.TextOnMap,
                    OutLined = false,
                    Centered = true
                };
                TextMap.Add();

                #endregion
            }

            public string Name { get; set; }
            public float SpawnTime { get; set; }
            public int RespawnTimer { get; set; }
            public Vector3 Position { get; set; }
            public Vector2 MinimapPosition { get; set; }
            public Vector2 MapPosition { get; set; }
            public List<Mob> Mobs { get; set; }
            public LeagueSharp.Common.Utility.Map.MapType MapType { get; set; }
            public GameObjectTeam Team { get; set; }
            public Color Colour { get; set; }
            public bool IsRanged { get; set; }
            public int State { get; set; }
            public int RespawnTime { get; set; }
            public int LastChangeOnState { get; set; }
            public Timers Timer { get; set; }
            public bool ShouldPing { get; set; }
            public int LastPing { get; set; }
            private Render.Text TextMinimap { get; set; }
            private Render.Text TextMap { get; set; }


            //private void Drawing_OnEndScene(EventArgs args)
            //{
            //    //if (TextMinimap.Visible)
            //    //{
            //    //    TextMinimap.OnEndScene();
            //    //}
            //}
        }

        public class Mob
        {
            public Mob(string name, bool isRanged = false, Obj_AI_Minion unit = null, int state = 0, int networkId = 0, int lastChangeOnState = 0, bool justDied = false)
            {
                Name = name;
                IsRanged = isRanged;
                Unit = unit;
                State = state;
                NetworkId = networkId;
                LastChangeOnState = lastChangeOnState;
                JustDied = justDied;
            }

            public Obj_AI_Minion Unit { get; set; }
            public string Name { get; set; }
            public bool IsRanged { get; set; }
            public int State { get; set; }
            public int NetworkId { get; set; }
            public int LastChangeOnState { get; set; }
            public bool JustDied { get; set; }
        }

        public class Timers
        {
            public Timers(Vector2 position, Vector2 minimapPosition, string textOnMap = "", string textOnMinimap = "")
            {
                TextOnMap = textOnMap;
                TextOnMinimap = textOnMinimap;
                Position = position;
                MinimapPosition = minimapPosition;
            }

            public string TextOnMap { get; set; }
            public string TextOnMinimap { get; set; }
            public Vector2 Position { get; set; }
            public Vector2 MinimapPosition { get; set; }
        }
    }
}

