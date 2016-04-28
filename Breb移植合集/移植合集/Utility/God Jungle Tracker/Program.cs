/*
██████╗ ██╗   ██╗     █████╗ ██╗     ██████╗ ██╗  ██╗ █████╗  ██████╗  ██████╗ ██████╗ 
██╔══██╗╚██╗ ██╔╝    ██╔══██╗██║     ██╔══██╗██║  ██║██╔══██╗██╔════╝ ██╔═══██╗██╔══██╗
██████╔╝ ╚████╔╝     ███████║██║     ██████╔╝███████║███████║██║  ███╗██║   ██║██║  ██║
██╔══██╗  ╚██╔╝      ██╔══██║██║     ██╔═══╝ ██╔══██║██╔══██║██║   ██║██║   ██║██║  ██║
██████╔╝   ██║       ██║  ██║███████╗██║     ██║  ██║██║  ██║╚██████╔╝╚██████╔╝██████╔╝
╚═════╝    ╚═╝       ╚═╝  ╚═╝╚══════╝╚═╝     ╚═╝  ╚═╝╚═╝  ╚═╝ ╚═════╝  ╚═════╝ ╚═════╝ 
*/

using System;
using System.Collections.Generic;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using SharpDX;
using Color = System.Drawing.Color;
using LeagueSharp.Common;
using Vector2 = SharpDX.Vector2;
using GodJungleTracker.Classes;

namespace GodJungleTracker
{
    class Program
    {
        public static Menu _menu;

        #region Definitions

        /*
        camp.State == 0 Not Tracking
        camp.State == 1 Attacking
        camp.State == 2 Disengaged
        camp.State == 3 Tracking/Iddle
        camp.State == 4 Presumed Dead
        camp.State == 5 Guessed on fow with networkId
        camp.State == 6 Guessed maybe dead
        camp.State == 7 dead on timer to respawn
        */

        public static LeagueSharp.Common.Utility.Map.MapType MapType { get; set; }

        public static Jungle.Camp DragonCamp;
        public static Jungle.Camp BaronCamp;

        public static List<int> OnAttackList;
        public static List<int> MissileHitList;
        public static List<int[]> OnCreateGrompList;
        public static List<int[]> OnCreateCampIconList;
        public static List<int[]> PossibleBaronList;
        public static List<int> PossibleDragonList;
        public static List<int> ObjectsList;

        public static int UpdateTick;
        public static int PossibleDragonTimer;
        public static int GuessNetworkId1 = 1;
        public static int GuessNetworkId2 = 1;
        public static int GuessDragonId = 1;
        public static int Seed1 = 3;
        public static int Seed2 = 2;
        public static float ClockTimeAdjust;
        public static int BiggestNetworkId;

        public static bool Timeronmap;
        public static bool Timeronminimap;
        public static int Circleradius;
        public static Color Colorattacking = Color.FromArgb(255, 255, 0, 0);
        public static Color Colortracked = Color.FromArgb(255, 0, 255, 0);
        public static Color Colordisengaged = Color.FromArgb(255, 255, 210, 0);
        public static Color Colordead = Color.FromArgb(255, 200, 200, 200);
        public static Color Colorguessed = Color.FromArgb(255, 0, 255, 255);
        public static int Circlewidth;

        public static ColorBGRA White;

        public static string GameVersion = Game.Version.Substring(0, 4);
        public static int[] HeroNetworkId = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        public static string[] BlockHeroes = { "Caitlyn", "Nidalee" };
        public static int[] SeedOrder = { 0, 1, 0, 1, 0, 0, 1, 0, 0, 1, 0, 0, 1, 0, 0, 1, 0, 0, 0, 0, 1, 0, 1, 1, 0, 0, 0, 1, 0, 0, 1, 0 };
        public static int[] CreateOrder = { 14, 15, 10, 9, 8, 13, 12, 11, 4, 3, 2, 7, 6, 5, 23, 22, 21, 20, 29, 28, 27, 26, 19, 18, 17, 16, 35, 34, 33, 32, 31, 30 };
        public static int[] IdOrder = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 10, 0, 0, 0, 2, 5, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

        #endregion

        public static bool getCheckBoxItem(string item)
        {
            return _menu[item].Cast<CheckBox>().CurrentValue;
        }

        public static int getSliderItem(string item)
        {
            return _menu[item].Cast<Slider>().CurrentValue;
        }

        public static int getBoxItem(string item)
        {
            return _menu[item].Cast<ComboBox>().CurrentValue;
        }

        public static bool getKeyBindItem(string item)
        {
            return _menu[item].Cast<KeyBind>().CurrentValue;
        }

        public static void OnGameLoad()
        {
            GameVersion = Game.Version.Substring(0, 4);

            LoadMenu();

            #region Set Defin

            OnAttackList = new List<int>();
            MissileHitList = new List<int>();
            OnCreateGrompList = new List<int[]>();
            OnCreateCampIconList = new List<int[]>();
            PossibleBaronList = new List<int[]>();
            PossibleDragonList = new List<int>();
            ObjectsList = new List<int>();

            White = new ColorBGRA(255, 255, 255, 255);

            #endregion

            #region Set Headers

            Packets.Attack.Header = getSliderItem("headerOnAttack2" + GameVersion);
            Packets.MissileHit.Header = getSliderItem("headerOnMissileHit2" + GameVersion);
            Packets.Disengaged.Header = getSliderItem("headerOnDisengaged" + GameVersion);
            Packets.MonsterSkill.Header = getSliderItem("headerOnMonsterSkill" + GameVersion);
            Packets.CreateGromp.Header = getSliderItem("headerOnCreateGromp" + GameVersion);
            Packets.CreateCampIcon.Header = getSliderItem("headerOnCreateCampIcon" + GameVersion);

            #endregion

            #region Set Dragon/Baron Camp
            foreach (var camp in Jungle.Camps.Where(camp => camp.MapType.ToString() == "SummonersRift"))
            {
                if (camp.Name == "Dragon")
                {
                    DragonCamp = camp;
                }
                else if (camp.Name == "Baron")
                {
                    BaronCamp = camp;
                }
            }
            #endregion

            #region Load Minions

            foreach (Obj_AI_Minion minion in ObjectManager.Get<Obj_AI_Minion>().Where(x => x.Name.Contains("SRU_") || x.Name.Contains("Sru_")))
            {
                foreach (var camp in Jungle.Camps.Where(camp => camp.MapType.ToString() == Game.MapId.ToString()))
                {
                    foreach (var mob in camp.Mobs)
                    {
                        //Do Stuff for each mob in a camp

                        if (mob.Name.Contains(minion.Name) && !minion.IsDead && mob.NetworkId != minion.NetworkId)
                        {
                            mob.NetworkId = minion.NetworkId;
                            
                            mob.LastChangeOnState = Environment.TickCount;
                            mob.Unit = minion;

                            if (!camp.IsRanged && camp.Mobs.Count > 1)
                            {
                                mob.State = 6;
                            }
                            else
                            {
                                mob.State = 5;
                            }

                            if (camp.Mobs.Count == 1)
                            {
                                camp.State = mob.State;
                                camp.LastChangeOnState = mob.LastChangeOnState;
                            }
                        }
                    }
                }
            }

            #endregion

            #region Load Static Menu

            Timeronmap = getCheckBoxItem("timeronmap");
            Timeronminimap = getCheckBoxItem("timeronminimap");
            Circleradius = getSliderItem("circleradius");

            Colorattacking = Color.FromArgb(255, 255, 0, 0);
            Colortracked = Color.FromArgb(255, 0, 255, 0);
            Colordisengaged = Color.FromArgb(255, 255, 210, 0);
            Colordead = Color.FromArgb(255, 200, 200, 200);
            Colorguessed = Color.FromArgb(255, 0, 255, 255);

            Circlewidth = getSliderItem("circlewidth");

            #endregion

            #region Load Others

            if (Game.Time > 450f)
            {
                GuessNetworkId1 = 0;

                GuessNetworkId2 = 0;
            }

            int c = 0;
            foreach (AIHeroClient hero in ObjectManager.Get<AIHeroClient>())
            {
                HeroNetworkId[c] = hero.NetworkId;
                c++;
                if (hero.NetworkId > BiggestNetworkId)
                {
                    BiggestNetworkId = hero.NetworkId;
                }

                if (!hero.IsAlly)
                {
                    for (int i = 0; i <= 1; i++)
                    {
                        if (hero.ChampionName.Contains(BlockHeroes[i]))
                        {
                            //Console.WriteLine("God Jungle Tracker: " + hero.ChampionName + " in enemy team so GuessDragonId is disabled ");
                            GuessDragonId = 0;
                        }
                    }
                }
            }

            #endregion

            Console.WriteLine("Loading Events");
            Game.OnProcessPacket += OnProcessPacket;
            Drawing.OnEndScene += Drawing_OnEndScene;
            GameObject.OnCreate += GameObjectOnCreate;
            GameObject.OnDelete += GameObjectOnDelete;
            Game.OnUpdate += OnGameUpdate;
            Obj_AI_Base.OnBuffGain += Obj_AI_Base_OnBuffAdd;
            Console.WriteLine("Finished Events");
        }

        private static void Obj_AI_Base_OnBuffAdd(Obj_AI_Base sender, Obj_AI_BaseBuffGainEventArgs args)
        {
            if (!(sender is AIHeroClient))
            {
                return;
            }

            if (args.Buff.Name.Contains("exaltedwithbaronnashor") && Utils.GameTimeTickCount - args.Buff.StartTime * 1000 <= BaronCamp.RespawnTimer * 1000)
            {
                BaronCamp.Mobs[0].State = 7;
                BaronCamp.Mobs[0].JustDied = false;
                BaronCamp.State = 7;
                BaronCamp.RespawnTime = Environment.TickCount + BaronCamp.RespawnTimer * 1000 - (Utils.GameTimeTickCount - (int)(args.Buff.StartTime * 1000) - 1000);
            }

            if (args.Buff.Name.Contains("tooltip_dragonslayerbuff") && Utils.GameTimeTickCount - (int)(args.Buff.StartTime * 1000) <= DragonCamp.RespawnTimer * 1000)
            {
                DragonCamp.Mobs[0].State = 7;
                DragonCamp.Mobs[0].JustDied = false;
                DragonCamp.State = 7;
                DragonCamp.RespawnTime = Environment.TickCount + DragonCamp.RespawnTimer * 1000 - (Utils.GameTimeTickCount - (int)(args.Buff.StartTime * 1000) - 1000);
            }
        }

        public static void GameObjectOnCreate(GameObject sender, EventArgs args)
        {
            if (!(sender is Obj_AI_Minion) || sender.Team != GameObjectTeam.Neutral)
            {
                return;
            }

            var minion = (Obj_AI_Minion)sender;

            foreach (var camp in Jungle.Camps.Where(camp => camp.MapType.ToString() == Game.MapId.ToString()))
            {
                //Do Stuff for each camp

                foreach (var mob in camp.Mobs.Where(mob => mob.Name == minion.Name))
                {
                    //Do Stuff for each mob in a camp

                    mob.NetworkId = minion.NetworkId;
                    mob.LastChangeOnState = Environment.TickCount;
                    mob.Unit = minion;
                    if (!minion.IsDead)
                    {
                        mob.State = 3;
                        mob.JustDied = false;
                    }
                    else
                    {
                        mob.State = 7;
                        mob.JustDied = true;
                    }

                    if (camp.Mobs.Count == 1)
                    {
                        camp.State = mob.State;
                        camp.LastChangeOnState = mob.LastChangeOnState;
                    }

                    if (mob.Name.Contains("Baron") && PossibleBaronList.Count >= 1)
                    {
                        try
                        {
                            PossibleBaronList.Clear();
                        }
                        catch (Exception)
                        {
                            //ignored
                        }
                    }

                    if (camp.Name == "Gromp" && getSliderItem("headerOnCreateGromp" + GameVersion) == 0)
                    {
                        foreach (var item in OnCreateGrompList.Where(item => item[0] == mob.NetworkId))
                        {
                            _menu["headerOnCreateGromp" + GameVersion].Cast<Slider>().CurrentValue = item[1];
                            Packets.CreateGromp.Header = item[1];
                            break;
                        }
                    }
                }
            }
        }

        public static void GameObjectOnDelete(GameObject sender, EventArgs args)
        {
            if (!(sender is Obj_AI_Minion) || sender.Team != GameObjectTeam.Neutral)
            {
                return;
            }

            if (sender.NetworkId == 0)
            {
                return;
            }

            //var minion = (Obj_AI_Minion)sender;

            foreach (var camp in Jungle.Camps.Where(camp => camp.MapType.ToString() == Game.MapId.ToString()))
            {
                //Do Stuff for each camp

                foreach (var mob in camp.Mobs.Where(mob => mob.NetworkId == sender.NetworkId))
                {
                    //Do Stuff for each mob in a camp

                    mob.LastChangeOnState = Environment.TickCount - 3000;
                    mob.Unit = null;
                    if (mob.State != 7)
                    {
                        mob.State = 7;
                        mob.JustDied = true;
                    }

                    if (camp.Mobs.Count == 1)
                    {
                        camp.State = mob.State;
                        camp.LastChangeOnState = mob.LastChangeOnState;
                    }
                }
            }
        }

        public static void OnGameUpdate(EventArgs args)
        {
            if (Environment.TickCount > UpdateTick + getSliderItem("updatetick"))
            {
                var enemy = HeroManager.Enemies.FirstOrDefault(x => x.IsValidTarget());
        
                foreach (var camp in Jungle.Camps.Where(camp => camp.MapType.ToString() == Game.MapId.ToString()))
                {
                    #region Update States

                    int mobCount = 0;
                    bool firstMob = true;
                    int visibleMobsCount = 0;
                    int rangedMobsCount = 0;
                    int deadRangedMobsCount = 0;

                    foreach (var mob in camp.Mobs)
                    {
                        //Do Stuff for each mob in a camp

                        try
                        {
                            if (mob.Unit != null && mob.Unit.IsVisible)
                            {
                                visibleMobsCount++;
                            }
                        }
                        catch (Exception)
                        {
                            //ignored
                        }
                    

                        if (mob.IsRanged)
                        {
                            rangedMobsCount++;

                            if (mob.JustDied)
                            {
                                deadRangedMobsCount++;
                            }
                        }

                        bool visible = false;

                        mobCount += 1;

                        int guessedTimetoDead = 3000;

                        if (camp.Name == "Baron")
                        {
                            guessedTimetoDead = 5000;
                        }
                        

                        switch (mob.State)
                        {
                            case 1:
                                if ((Environment.TickCount - mob.LastChangeOnState) >= guessedTimetoDead && camp.Name != "Crab")
                                {
                                    if (camp.Name == "Dragon")
                                    {
                                        //do nothing
                                    }
                                    else if (camp.Name == "Baron")
                                    {
                                            mob.State = 5;
                                            mob.LastChangeOnState = Environment.TickCount - 2000;
                                    }
                                    else
                                    {
                                        mob.State = 4;
                                        mob.LastChangeOnState = Environment.TickCount - 2000;
                                    }
                                }

                                if ((Environment.TickCount - mob.LastChangeOnState >= 10000 && camp.Name == "Crab"))
	                            {
		                            mob.State = 3;
                                    mob.LastChangeOnState = Environment.TickCount;
	                            }
                                break;
                            case 2:
                                if (Environment.TickCount - mob.LastChangeOnState >= 4000)
	                            {
                                    if (!camp.IsRanged && camp.Mobs.Count > 1)
                                    {
                                        mob.State = 6;
                                    }
                                    else
                                    {
                                        mob.State = 5;
                                    }
                                    mob.LastChangeOnState = Environment.TickCount;
	                            }
                                break;
                            case 4:
                                if (Environment.TickCount - mob.LastChangeOnState >= 5000)
	                            {
		                            mob.State = 7;
                                    mob.JustDied = true;
	                            }
                                break;
                            case 5:
                                if (Environment.TickCount - mob.LastChangeOnState >= 45000)
	                            {
		                            mob.State = 6;
	                            }
                                if (mob.Unit != null && mob.Unit.IsVisible && !mob.Unit.IsDead)
                                {
                                    mob.State = 3;
                                }
                                break;
                            case 6:
                                if (mob.Unit != null && mob.Unit.IsVisible && !mob.Unit.IsDead)
                                {
                                    mob.State = 3;
                                }
                                break;
                            default:
                                break;
                        }

                        if (mob.Unit != null && mob.Unit.IsVisible && !mob.Unit.IsDead)
                        {
                            visible = true;
                        }

                        if ((mob.State == 7 || mob.State == 4) && visible) //check again
                        {
                            mob.State = 3;
                            mob.LastChangeOnState = Environment.TickCount;
                            mob.JustDied = false;
                        }

                        if (camp.Mobs.Count == 1)
                        {
                            camp.State = mob.State;
                            camp.LastChangeOnState = mob.LastChangeOnState;
                        }

                        if (camp.IsRanged && camp.Mobs.Count > 1 && mob.State > 0)
                        {
                            if (visible)
                            {
                                if (firstMob)
                                {
                                    camp.State = mob.State;
                                    camp.LastChangeOnState = mob.LastChangeOnState;
                                    firstMob = false;
                                }
                                else if (!firstMob)
                                {
                                    if (mob.State < camp.State)
                                    {
                                        camp.State = mob.State;
                                    }
                                    if (mob.LastChangeOnState > camp.LastChangeOnState)
                                    {
                                        camp.LastChangeOnState = mob.LastChangeOnState;
                                    }
                                }

                                if (!mob.IsRanged)
                                {
                                    camp.LastChangeOnState = Environment.TickCount;
                                    camp.RespawnTime = (camp.LastChangeOnState + camp.RespawnTimer * 1000);
                                }
                            }
                            else
                            {
                                if (firstMob)
                                {
                                    if (mob.IsRanged)
                                    {
                                        camp.State = mob.State;
                                        firstMob = false;
                                    }
                                    camp.LastChangeOnState = mob.LastChangeOnState;
                                }
                                else if (!firstMob)
                                {
                                    if (mob.State < camp.State && mob.IsRanged)
                                    {
                                        camp.State = mob.State;
                                    }
                                    if (mob.LastChangeOnState > camp.LastChangeOnState)
                                    {
                                        camp.LastChangeOnState = mob.LastChangeOnState;
                                    }
                                }
                            }
                        }
                        else if (!camp.IsRanged && camp.Mobs.Count > 1 && mob.State > 0)
                        {
                            if (firstMob)
                            {
                                camp.State = mob.State;
                                camp.LastChangeOnState = mob.LastChangeOnState;
                                firstMob = false;
                            }
                            else
                            {
                                if (mob.State < camp.State)
                                {
                                    camp.State = mob.State;
                                }
                                if (mob.LastChangeOnState > camp.LastChangeOnState)
                                {
                                    camp.LastChangeOnState = mob.LastChangeOnState;
                                }
                            }
                            if (visible)
                            {
                                camp.LastChangeOnState = Environment.TickCount;
                                camp.RespawnTime = (camp.LastChangeOnState + camp.RespawnTimer * 1000);
                            }
                        }

                        if (visible && camp.RespawnTime > Environment.TickCount)
                        {
                            camp.RespawnTime = (Environment.TickCount + camp.RespawnTimer * 1000);
                        }
                    }


                    //Do Stuff for each camp

                    if (camp.State == 7)
                    {
                        int mobsJustDiedCount = 0;

                        for (int i = 0; i < mobCount; i++)
                        {
                            try
                            {
                                if (camp.Mobs[i].JustDied)
                                {
                                    mobsJustDiedCount++;
                                }
                            }
                            catch (Exception)
                            {
                                //ignored
                            }
                        
                        }

                        if (mobsJustDiedCount == mobCount)
                        {
                            camp.RespawnTime = (camp.LastChangeOnState + camp.RespawnTimer * 1000);

                            for (int i = 0; i < mobCount; i++)
                            {
                                try
                                {
                                    camp.Mobs[i].JustDied = false;
                                }
                                catch (Exception)
                                {
                                    //ignored
                                }
                            }
                        }
                    }

                    if (camp.IsRanged && visibleMobsCount == 0 && rangedMobsCount == deadRangedMobsCount)
                    {
                        camp.RespawnTime = (camp.LastChangeOnState + camp.RespawnTimer * 1000);

                        for (int i = 0; i < mobCount; i++)
                        {
                            try
                            {
                                camp.Mobs[i].JustDied = false;
                            }
                            catch (Exception)
                            {
                                //ignored
                            }
                        
                        }
                    }

                    if (camp.Name == "Baron" && PossibleBaronList.Count >= 1 && camp.State >= 1 && camp.State <= 3)
                    {
                        try
                        {
                            PossibleBaronList.Clear();
                        }
                        catch (Exception)
                        {
                            //ignored
                        }
                    }

                    #endregion

                    #region Timers

                    if (camp.RespawnTime > Environment.TickCount && camp.State == 7)
                    {
                        var timespan = TimeSpan.FromSeconds((camp.RespawnTime - Environment.TickCount) / 1000f);

                        bool format;

                        if (camp.Position.IsOnScreen() && getCheckBoxItem("timeronmap"))
                        {
                            format = !getBoxItem("timeronmapformat").Equals(0);

                            camp.Timer.TextOnMap = string.Format(format ? "{1}" : "{0}:{1:00}", (int)timespan.TotalMinutes,
                                format ? (int)timespan.TotalSeconds : timespan.Seconds);
                        }

                        if (getCheckBoxItem("timeronminimap"))
                        {
                            format = !getBoxItem("timeronmapformat").Equals(0);

                            camp.Timer.TextOnMinimap = string.Format(format ? "{1}" : "{0}:{1:00}", (int)timespan.TotalMinutes, format ? (int)timespan.TotalSeconds : timespan.Seconds);

                            camp.Timer.MinimapPosition = new Vector2((int)(camp.MinimapPosition.X), (int)(camp.MinimapPosition.Y));
                        }
                    }

                    

                    #endregion

                    #region Guess Blue/Red NetworkID

                    if (GuessNetworkId1 == 1 && camp.Name == "Blue" && camp.Team.ToString().Contains("Order") && visibleMobsCount == camp.Mobs.Count &&
                        camp.Mobs[0].NetworkId != 0 && camp.Mobs[1].NetworkId != 0 && camp.Mobs[2].NetworkId != 0)
                    {
                        Seed1 = (camp.Mobs[1].NetworkId - camp.Mobs[0].NetworkId);
                        Seed2 = (camp.Mobs[2].NetworkId - camp.Mobs[1].NetworkId);

                        int id = 0;

                        for (int c = 0; c <= 31; c++)
                        {
                            int order = CreateOrder[c];

                            if (c == 2)
                            {
                                id += Seed1;
                                id += Seed2;
                            }
                            else
                            {
                                if (SeedOrder[c] == 1) id += Seed1;
                                else id += Seed2;
                            }

                            IdOrder[order] = id;
                        }

                        foreach (var camp2 in Jungle.Camps.Where(camp2 => camp2.MapType.ToString() == Game.MapId.ToString() && camp2.Name == "Blue" && !camp2.Team.ToString().Contains("Order")))
                        {
                            for (int j = 5; j <= 7; j++)
                            {
                                if (IdOrder[j] == 0) continue;
                                int i = 0;
                                switch (j)
                                {
                                    case 5:
                                        i = 2;
                                        break;
                                    case 6:
                                        i = 1;
                                        break;
                                    case 7:
                                        i = 0;
                                        break;
                                    default:
                                        break;
                                }

                                if (camp2.Mobs[i].NetworkId == 0)
                                {
                                    if (IdOrder[j] < IdOrder[4])
                                    {
                                        camp2.Mobs[i].NetworkId = camp.Mobs[0].NetworkId - ((IdOrder[4] - IdOrder[j]));
                                        camp2.Mobs[i].State = 5;
                                        camp2.Mobs[i].LastChangeOnState = Environment.TickCount;
                                    }
                                    else if (IdOrder[j] > IdOrder[4])
                                    {
                                        camp2.Mobs[i].NetworkId = camp.Mobs[0].NetworkId + ((IdOrder[j] - IdOrder[4]));
                                        camp2.Mobs[i].State = 5;
                                        camp2.Mobs[i].LastChangeOnState = Environment.TickCount;
                                    }
                                }
                            }
                        }
                        GuessNetworkId1 = 0;
                    
                    }
                    else if (GuessNetworkId1 == 1 && camp.Name == "Blue" && !camp.Team.ToString().Contains("Order") && visibleMobsCount == camp.Mobs.Count &&
                        camp.Mobs[0].NetworkId != 0 && camp.Mobs[1].NetworkId != 0 && camp.Mobs[2].NetworkId != 0)
                    {
                        Seed1 = (camp.Mobs[1].NetworkId - camp.Mobs[0].NetworkId);
                        Seed2 = (camp.Mobs[2].NetworkId - camp.Mobs[1].NetworkId);

                        //Console.WriteLine("Seed1:" + Seed1 + "  Seed2:" + Seed2);

                        int id = 0;

                        for (int c = 0; c <= 31; c++)
                        {
                            int order = CreateOrder[c];

                            if (c == 2)
                            {
                                id += Seed1;
                                id += Seed2;
                            }
                            else
                            {
                                if (SeedOrder[c] == 1) id += Seed1;
                                else id += Seed2;
                            }

                            IdOrder[order] = id;
                        }

                        foreach (var camp2 in Jungle.Camps.Where(camp2 => camp2.MapType.ToString() == Game.MapId.ToString() && camp2.Name == "Blue" && camp2.Team.ToString().Contains("Order")))
                        {
                            for (int j = 2; j <= 4; j++)
                            {
                                if (IdOrder[j] == 0) continue;
                                int i = 0;
                                switch (j)
                                {
                                    case 2:
                                        i = 2;
                                        break;
                                    case 3:
                                        i = 1;
                                        break;
                                    case 4:
                                        i = 0;
                                        break;
                                    default:
                                        break;
                                }

                                if (camp2.Mobs[i].NetworkId == 0)
                                {
                                    if (IdOrder[j] < IdOrder[7])
                                    {
                                        camp2.Mobs[i].NetworkId = camp.Mobs[0].NetworkId - ((IdOrder[7] - IdOrder[j]));
                                        camp2.Mobs[i].State = 5;
                                        camp2.Mobs[i].LastChangeOnState = Environment.TickCount;
                                    }
                                    else if (IdOrder[j] > IdOrder[7])
                                    {
                                        camp2.Mobs[i].NetworkId = camp.Mobs[0].NetworkId + ((IdOrder[j] - IdOrder[7]));
                                        camp2.Mobs[i].State = 5;
                                        camp2.Mobs[i].LastChangeOnState = Environment.TickCount;
                                    }
                                    //Console.WriteLine("NetworkID[" + j + "]:" + NetworkID[j] + " and Name: " + NameToCompare[j]);
                                }
                            }
                        }
                        GuessNetworkId1 = 0;
                    }

                    else if (GuessNetworkId1 == 1 && camp.Name == "Red" && camp.Team.ToString().Contains("Order") && visibleMobsCount == camp.Mobs.Count &&
                    camp.Mobs[0].NetworkId != 0 && camp.Mobs[1].NetworkId != 0 && camp.Mobs[2].NetworkId != 0)
                    {
                        Seed1 = (camp.Mobs[1].NetworkId - camp.Mobs[0].NetworkId);
                        Seed2 = (camp.Mobs[2].NetworkId - camp.Mobs[1].NetworkId);

                        //Console.WriteLine("Seed1:" + Seed1 + "  Seed2:" + Seed2);

                        int id = 0;

                        for (int c = 0; c <= 31; c++)
                        {
                            int order = CreateOrder[c];

                            if (c == 2)
                            {
                                id += Seed1;
                                id += Seed2;
                            }
                            else
                            {
                                if (SeedOrder[c] == 1) id += Seed1;
                                else id += Seed2;
                            }

                            IdOrder[order] = id;
                        }

                        foreach (var camp2 in Jungle.Camps.Where(camp2 => camp2.MapType.ToString() == Game.MapId.ToString() && camp2.Name == "Red" && !camp2.Team.ToString().Contains("Order")))
                        {
                            for (int j = 11; j <= 13; j++)
                            {
                                if (IdOrder[j] == 0) continue;
                                int i = 0;
                                switch (j)
                                {
                                    case 11:
                                        i = 2;
                                        break;
                                    case 12:
                                        i = 1;
                                        break;
                                    case 13:
                                        i = 0;
                                        break;
                                    default:
                                        break;
                                }

                                if (camp2.Mobs[i].NetworkId == 0)
                                {
                                    if (IdOrder[j] < IdOrder[10])
                                    {
                                        camp2.Mobs[i].NetworkId = camp.Mobs[0].NetworkId - ((IdOrder[10] - IdOrder[j]));
                                        camp2.Mobs[i].State = 5;
                                        camp2.Mobs[i].LastChangeOnState = Environment.TickCount;
                                    }
                                    else if (IdOrder[j] > IdOrder[10])
                                    {
                                        camp2.Mobs[i].NetworkId = camp.Mobs[0].NetworkId + ((IdOrder[j] - IdOrder[10]));
                                        camp2.Mobs[i].State = 5;
                                        camp2.Mobs[i].LastChangeOnState = Environment.TickCount;
                                    }
                                    //Console.WriteLine("NetworkID[" + j + "]:" + NetworkID[j] + " and Name: " + NameToCompare[j]);
                                }
                            }
                        }
                        GuessNetworkId1 = 0;

                    }
                    else if (GuessNetworkId1 == 1 && camp.Name == "Red" && !camp.Team.ToString().Contains("Order") && visibleMobsCount == camp.Mobs.Count &&
                        camp.Mobs[0].NetworkId != 0 && camp.Mobs[1].NetworkId != 0 && camp.Mobs[2].NetworkId != 0)
                    {
                        Seed1 = (camp.Mobs[1].NetworkId - camp.Mobs[0].NetworkId);
                        Seed2 = (camp.Mobs[2].NetworkId - camp.Mobs[1].NetworkId);

                        int id = 0;

                        for (int c = 0; c <= 31; c++)
                        {
                            int order = CreateOrder[c];

                            if (c == 2)
                            {
                                id += Seed1;
                                id += Seed2;
                            }
                            else
                            {
                                if (SeedOrder[c] == 1) id += Seed1;
                                else id += Seed2;
                            }

                            IdOrder[order] = id;
                        }

                        foreach (var camp2 in Jungle.Camps.Where(camp2 => camp2.MapType.ToString() == Game.MapId.ToString() && camp2.Name == "Red" && camp2.Team.ToString().Contains("Order")))
                        {
                            for (int j = 8; j <= 10; j++)
                            {
                                if (IdOrder[j] == 0) continue;
                                int i = 0;
                                switch (j)
                                {
                                    case 8:
                                        i = 2;
                                        break;
                                    case 9:
                                        i = 1;
                                        break;
                                    case 10:
                                        i = 0;
                                        break;
                                    default:
                                        break;
                                }

                                if (camp2.Mobs[i].NetworkId == 0)
                                {
                                    if (IdOrder[j] < IdOrder[13])
                                    {
                                        camp2.Mobs[i].NetworkId = camp.Mobs[0].NetworkId - ((IdOrder[13] - IdOrder[j]));
                                        camp2.Mobs[i].State = 5;
                                        camp2.Mobs[i].LastChangeOnState = Environment.TickCount;
                                    }
                                    else if (IdOrder[j] > IdOrder[13])
                                    {
                                        camp2.Mobs[i].NetworkId = camp.Mobs[0].NetworkId + ((IdOrder[j] - IdOrder[13]));
                                        camp2.Mobs[i].State = 5;
                                        camp2.Mobs[i].LastChangeOnState = Environment.TickCount;
                                    }
                                }
                            }
                        }
                        GuessNetworkId1 = 0;
                    }

                    #endregion

                    #region Ping

                    if (camp.State != 1 && !camp.ShouldPing)
                    {
                        camp.ShouldPing = true;
                    }

                    if (camp.State == 1 && camp.ShouldPing)
                    {
                        if (camp.Name == "Baron" && ((getCheckBoxItem("pingfow") && visibleMobsCount == 0) || !getCheckBoxItem("pingfow")) &&
                            ((getCheckBoxItem("pingfow") && visibleMobsCount == 0) || !getCheckBoxItem("pingfow")) &&
                            ((getCheckBoxItem("pingscreen") && !camp.Position.LSIsOnScreen()) || !getCheckBoxItem("pingscreen")) &&
                            getCheckBoxItem("pingbaron") && Environment.TickCount - camp.LastPing >= (getSliderItem("pingdelay") * 1000))
                        {
                            TacticalMap.ShowPing(PingCategory.Danger, DragonCamp.Position, true);
                            camp.LastPing = Environment.TickCount;
                        }
                        else if(camp.Name == "Dragon" && ((getCheckBoxItem("pingfow") && visibleMobsCount == 0) || !getCheckBoxItem("pingfow")) &&
                                ((getCheckBoxItem("pingfow") && visibleMobsCount == 0) || !getCheckBoxItem("pingfow")) &&
                                ((getCheckBoxItem("pingscreen") && !camp.Position.LSIsOnScreen()) || !getCheckBoxItem("pingscreen")) &&
                                getCheckBoxItem("pingdragon") && Environment.TickCount - camp.LastPing >= (getSliderItem("pingdelay") * 1000))
                        {
                            TacticalMap.ShowPing(PingCategory.Danger, DragonCamp.Position, true);
                            camp.LastPing = Environment.TickCount;
                        }
                        else
                        {
                            if (((getCheckBoxItem("pingfow") && visibleMobsCount == 0) || !getCheckBoxItem("pingfow")) &&
                                ((getCheckBoxItem("pingscreen") && !camp.Position.LSIsOnScreen()) || !getCheckBoxItem("pingscreen")) &&
                                 getCheckBoxItem("pingsmall") && Environment.TickCount - camp.LastPing >= (getSliderItem("pingdelay") * 1000))
                            {
                                TacticalMap.ShowPing(PingCategory.Normal, camp.Position, true);
                                camp.LastPing = Environment.TickCount;
                            }
                        }
                        camp.ShouldPing = false;
                    }
                    #endregion
                }
            
                #region Static Menu Update

                Timeronmap = getCheckBoxItem("timeronmap");
                Timeronminimap = getCheckBoxItem("timeronminimap");
                Circleradius = getSliderItem("circleradius");

                Colorattacking = Color.FromArgb(255, 255, 0, 0);
                Colortracked = Color.FromArgb(255, 0, 255, 0);
                Colordisengaged = Color.FromArgb(255, 255, 210, 0);
                Colordead = Color.FromArgb(255, 200, 200, 200);
                Colorguessed = Color.FromArgb(255, 0, 255, 255);

                Circlewidth = getSliderItem("circlewidth");

                #endregion

                foreach (var obj in PossibleBaronList.ToList().Where(item => Environment.TickCount >= item[3] + 20000))
                {
                    try
                    {
                        PossibleBaronList.Remove(obj);
                    }
                    catch (Exception)
                    {
                        //ignored
                    }
                }

                UpdateTick = Environment.TickCount;
            }

            foreach (var camp in Jungle.Camps.Where(camp => camp.MapType.ToString() == Game.MapId.ToString()))
            {
                if (camp.Position.IsOnScreen() && getCheckBoxItem("timeronmap"))
                {
                    camp.Timer.Position = Drawing.WorldToScreen(camp.Position);
                }
            }
        }

        public static void OnProcessPacket(GamePacketEventArgs args)
        {

            Console.WriteLine("GOT EM");

            short header = BitConverter.ToInt16(args.PacketData, 0);
            int length = BitConverter.ToString(args.PacketData, 0).Length;
            int networkID = BitConverter.ToInt32(args.PacketData, 2);

            if (header == 0)
            {
                return;
            }

            Console.WriteLine("GOT EM2");

            //debug
            //foreach (var item in PossibleDragonList.ToList().Where(id => id == networkID))
            //{
            //    Console.WriteLine("Header: " + header + " lenght: " + length + " id: " + networkID);
            //}

            #region AutoFind Headers

            if (getCheckBoxItem("forcefindheaders"))
            {

                Console.WriteLine("SWITCH 1");

                _menu["headerOnAttack2" + GameVersion].Cast<Slider>().CurrentValue = 0;
                _menu["headerOnMissileHit2" + GameVersion].Cast<Slider>().CurrentValue = 0;
                _menu["headerOnDisengaged" + GameVersion].Cast<Slider>().CurrentValue = 0;
                _menu["headerOnMonsterSkill" + GameVersion].Cast<Slider>().CurrentValue = 0;
                _menu["headerOnCreateGromp" + GameVersion].Cast<Slider>().CurrentValue = 0;
                _menu["headerOnCreateCampIcon" + GameVersion].Cast<Slider>().CurrentValue = 0;

                Console.WriteLine("SWITCH 2");

                Packets.Attack.Header = 0;
                Packets.MissileHit.Header = 0;
                Packets.Disengaged.Header = 0;
                Packets.MonsterSkill.Header = 0;
                Packets.CreateGromp.Header = 0;
                Packets.CreateCampIcon.Header = 0;
                //_menu.Item("forcefindheaders").SetValue<bool>(false);
                _menu["forcefindheaders"].Cast<CheckBox>().CurrentValue = false;

                Console.WriteLine("SWITCH 3");

            }



            if (getSliderItem("headerOnAttack2" + GameVersion) == 0 && length == Packets.Attack.Length && networkID > 0)
            {
                foreach (Obj_AI_Minion obj in ObjectManager.Get<Obj_AI_Minion>().Where(obj => obj.NetworkId == networkID))
                {
                    OnAttackList.Add(header);
                    if (OnAttackList.Count<int>(x => x == header) == 10)
                    {
                        //_menu.Item("headerOnAttack2" + GameVersion).SetValue<Slider>(new Slider(header, 0, 400));
                        _menu["headerOnAttack2" + GameVersion].Cast<Slider>().CurrentValue = header;
                        Packets.Attack.Header = header;
                        try
                        {
                            OnAttackList.Clear();
                        }
                        catch (Exception)
                        {
                            //ignored
                        }
                    }
                }
            }

            if (getSliderItem("headerOnMissileHit2" + GameVersion) == 0 && length == Packets.MissileHit.Length && networkID > 0)
            {
                foreach (Obj_AI_Minion obj in ObjectManager.Get<Obj_AI_Minion>().Where(obj => obj.IsRanged && obj.NetworkId == networkID))
                {
                    MissileHitList.Add(header);
                    if (MissileHitList.Count<int>(x => x == header) == 10)
                    {
                        //_menu.Item("headerOnMissileHit2" + GameVersion).SetValue<Slider>(new Slider(header, 0, 400));
                        _menu["headerOnMissileHit2" + GameVersion].Cast<Slider>().CurrentValue = header;
                        Packets.MissileHit.Header = header;
                        try
                        {
                            MissileHitList.Clear();
                        }
                        catch (Exception)
                        {
                            //ignored
                        }
                    }
                }
            }

            if (getSliderItem("headerOnDisengaged" + GameVersion) == 0 && length == Packets.Disengaged.Length && networkID > 0)
            {
                foreach (Obj_AI_Minion obj in ObjectManager.Get<Obj_AI_Minion>().Where(obj => obj.Team.ToString().Contains("Neutral") && obj.NetworkId == networkID))
                {
                    //_menu.Item("headerOnDisengaged" + GameVersion).SetValue<Slider>(new Slider(header, 0, 400));
                    _menu["headerOnDisengaged" + GameVersion].Cast<Slider>().CurrentValue = header;
                    Packets.Disengaged.Header = header;
                }
            }

            if (getSliderItem("headerOnMonsterSkill" + GameVersion) == 0 && length == Packets.MonsterSkill.Length && networkID > 0)
            {
                foreach (Obj_AI_Minion obj in ObjectManager.Get<Obj_AI_Minion>().Where(obj => obj.Name.Contains("Dragon") && obj.NetworkId == networkID))
                {
                    //_menu.Item("headerOnMonsterSkill" + GameVersion).SetValue<Slider>(new Slider(header, 0, 400));
                    _menu["headerOnMonsterSkill" + GameVersion].Cast<Slider>().CurrentValue = header;
                    Packets.MonsterSkill.Header = header;
                }
            }

            if (getSliderItem("headerOnCreateGromp" + GameVersion) == 0 && (length == Packets.CreateGromp.Length || length == Packets.CreateGromp.Length2) && networkID > 0)
            {
                OnCreateGrompList.Add(new int[] { networkID, (int)header, length });
            }

            if (getSliderItem("headerOnCreateCampIcon" + GameVersion) == 0 && networkID == 0 && (length == Packets.CreateCampIcon.Length || length == Packets.CreateCampIcon.Length2 || length == Packets.CreateCampIcon.Length3 || length == Packets.CreateCampIcon.Length4 || length == Packets.CreateCampIcon.Length5))
            {
                OnCreateCampIconList.Add(new int[] { (int)header, length });

                if ((OnCreateCampIconList.Count(item => item[0] == (int)header && item[1] == Packets.CreateCampIcon.Length) == 6) &&
                    (OnCreateCampIconList.Count(item => item[0] == (int)header && item[1] == Packets.CreateCampIcon.Length2) == 3) &&
                    (OnCreateCampIconList.Count(item => item[0] == (int)header && item[1] == Packets.CreateCampIcon.Length3) == 1) &&
                    (OnCreateCampIconList.Count(item => item[0] == (int)header && item[1] == Packets.CreateCampIcon.Length4) == 1) &&
                    (OnCreateCampIconList.Count(item => item[0] == (int)header && item[1] == Packets.CreateCampIcon.Length5) == 1))
                {
                    //_menu.Item("headerOnCreateCampIcon" + GameVersion).SetValue<Slider>(new Slider(header, 0, 400));
                    _menu["headerOnCreateCampIcon" + GameVersion].Cast<Slider>().CurrentValue = header;
                    Packets.CreateCampIcon.Header = header;
                    try
                    {
                        OnCreateCampIconList.Clear();
                    }
                    catch (Exception)
                    {
                        //ignored
                    }
                }
            }

            #endregion
            
            #region Update States

            bool isMob = false;

            foreach (var camp in Jungle.Camps.Where(camp => camp.MapType.ToString() == Game.MapId.ToString()))
            {
                //Do Stuff for each camp

                foreach (var mob in camp.Mobs.Where(mob => mob.NetworkId == networkID))
                {
                    //Do Stuff for each mob in a camp

                    isMob = true;

                    if (header == Packets.MonsterSkill.Header)
                    {
                        mob.State = mob.Name.Contains("Crab") ? 4 : 1;
                        mob.LastChangeOnState = Environment.TickCount;
                    }

                    else if (header == Packets.Attack.Header)
                    {
                        //Console.WriteLine(NameToCompare[i] + " is Attacking");

                        mob.State = 1;
                        mob.LastChangeOnState = Environment.TickCount;
                    }

                    else if (header == Packets.MissileHit.Header)
                    {
                        //Console.WriteLine(NameToCompare[i] + " is Attacking (ranged)");

                        mob.State = 1;
                        mob.LastChangeOnState = Environment.TickCount;
                    }

                    else if (header == Packets.Disengaged.Header)
                    {
                        //Console.WriteLine(NameToCompare[i] + " is Disengaged");
                        if (mob.Name.Contains("Crab"))
                        {
                            mob.State = mob.State == 0 ? 5 : 1;
                        }
                        if (!mob.Name.Contains("Crab") && !mob.Name.Contains("Spider"))
                        {
                            mob.State = mob.State == 0 ? 5 : 2;
                        }
                        mob.LastChangeOnState = Environment.TickCount;
                    }

                    if (mob.LastChangeOnState == Environment.TickCount && camp.Mobs.Count == 1)
                    {
                        camp.State = mob.State;
                        camp.LastChangeOnState = mob.LastChangeOnState;
                    }
                }
            }

            #endregion

            #region Guess Dragon/Baron NetworkID

            bool foundObj = false;

            foreach (var obj in ObjectManager.Get<GameObject>().ToList().Where(x => x.NetworkId == networkID))
            {
                foundObj = true;
            }

            //Find Baron NetworkID
            if (Game.MapId.ToString() == "SummonersRift" && 
                !isMob && !foundObj &&
                networkID != DragonCamp.Mobs[0].NetworkId &&
                networkID != BaronCamp.Mobs[0].NetworkId &&
                networkID > BiggestNetworkId
                )
            {
                if (Packets.MissileHit.Header == header && Packets.MissileHit.Length == length)
                {
                    PossibleBaronList.Add(new int[] { networkID, (int)header, length, Environment.TickCount });

                    if ((PossibleBaronList.Count(item => item[0] == networkID && item[1] == Packets.MonsterSkill.Header && item[2] == Packets.MonsterSkill.Length) >= 1) &&
                    (PossibleBaronList.Count(item => item[0] == networkID && item[1] == Packets.MonsterSkill.Header && item[2] == Packets.MonsterSkill.Length2) >= 1))
                    {
                        BaronCamp.Mobs[0].State = 1;
                        BaronCamp.Mobs[0].LastChangeOnState = Environment.TickCount;
                        BaronCamp.Mobs[0].NetworkId = networkID;
                    }

                }
                else if (Packets.MonsterSkill.Header == header && Packets.MonsterSkill.Length == length)
                {
                    PossibleBaronList.Add(new int[] { networkID, (int)header, length, Environment.TickCount });

                    if ((PossibleBaronList.Count(item => item[0] == networkID && item[1] == Packets.MissileHit.Header && item[2] == Packets.MissileHit.Length) >= 1) &&
                    (PossibleBaronList.Count(item => item[0] == networkID && item[1] == Packets.MonsterSkill.Header && item[2] == Packets.MonsterSkill.Length2) >= 1))
                    {
                        BaronCamp.Mobs[0].State = 1;
                        BaronCamp.Mobs[0].LastChangeOnState = Environment.TickCount;
                        BaronCamp.Mobs[0].NetworkId = networkID;
                    }
                }
                else if (Packets.MonsterSkill.Header == header && Packets.MonsterSkill.Length2 == length)
                {
                    PossibleBaronList.Add(new int[] { networkID, (int)header, length, Environment.TickCount });

                    if ((PossibleBaronList.Count(item => item[0] == networkID && item[1] == Packets.MissileHit.Header && item[2] == Packets.MissileHit.Length) >= 1) &&
                    (PossibleBaronList.Count(item => item[0] == networkID && item[1] == Packets.MonsterSkill.Header && item[2] == Packets.MonsterSkill.Length) >= 1))
                    {
                        BaronCamp.Mobs[0].State = 1;
                        BaronCamp.Mobs[0].LastChangeOnState = Environment.TickCount;
                        BaronCamp.Mobs[0].NetworkId = networkID;
                    }
                }
            }

            //Find Dragon NetworkID
            if (Environment.TickCount <= PossibleDragonTimer + 5000)
            {
                foreach (var id in PossibleDragonList.ToList().Where(id => id == networkID))
                {
                    try
                    {
                        PossibleDragonList.RemoveAll(x => x == networkID);
                    }
                    catch (Exception)
                    {
                        //ignored
                    }
                }
            }
            else
            {
                if (PossibleDragonList.Count() == 1)
                {
                    DragonCamp.Mobs[0].State = 1;
                    DragonCamp.Mobs[0].LastChangeOnState = Environment.TickCount;
                    DragonCamp.Mobs[0].NetworkId = PossibleDragonList[0];
                }
                try
                {
                    PossibleDragonList.Clear();
                }
                catch (Exception)
                {
                    //ignored
                }
            }



            if (header == Packets.MonsterSkill.Header &&
                Game.MapId.ToString() == "SummonersRift" &&
                !isMob && !foundObj &&
                networkID != DragonCamp.Mobs[0].NetworkId &&
                networkID != BaronCamp.Mobs[0].NetworkId &&
                networkID > BiggestNetworkId &&
                length == Packets.MonsterSkill.Length &&
                GuessDragonId == 1)
            {
                /*foreach (var obj in ObjectManager.Get<GameObject>().Where(x => x.NetworkId == networkID))
                {
                    Game.PrintChat("<font color=\"#FF0000\"> God Jungle Tracker (debug): Tell AlphaGod he forgot to consider: " + obj.Name + " - " + obj.SkinName + " - " + obj.CharData.BaseSkinName + " - Guess Dragon NetWorkID disabled</font>");
                }*/
                if (!ObjectsList.Contains(networkID))
                {
                    PossibleDragonList.Add(networkID);
                    PossibleDragonTimer = Environment.TickCount;
                }
            }

            #endregion  

            #region Gromp Created

            if (header == Packets.CreateGromp.Header && Game.MapId.ToString() == "SummonersRift")  //Gromp Created
            {
                if (length == Packets.CreateGromp.Length)
                {
                    foreach (var camp in Jungle.Camps.Where(camp => camp.Name == "Gromp"))
                    {
                        foreach (var mob in camp.Mobs.Where(mob => mob.Name.Contains("SRU_Gromp13.1.1")))
                        {
                            mob.NetworkId = BitConverter.ToInt32(args.PacketData, 2);
                            mob.State = 3;
                            mob.LastChangeOnState = Environment.TickCount;
                            camp.State = mob.State;
                            camp.LastChangeOnState = mob.LastChangeOnState;
                        }
                    }

                    if (Game.Time - 111f < 90 && ClockTimeAdjust == 0)
                    {
                        ClockTimeAdjust = Game.Time - 111f;
                        DragonCamp.Mobs[0].State = 0;
                        DragonCamp.RespawnTime = Environment.TickCount + 39000;
                        DragonCamp.State = 0;
                        BiggestNetworkId = BitConverter.ToInt32(args.PacketData, 2);
                    }
                }
                else if (length == Packets.CreateGromp.Length2)
                {
                    foreach (var camp in Jungle.Camps.Where(camp => camp.Name == "Gromp"))
                    {
                        foreach (var mob in camp.Mobs.Where(mob => mob.Name.Contains("SRU_Gromp14.1.1")))
                        {
                            mob.NetworkId = BitConverter.ToInt32(args.PacketData, 2);
                            mob.State = 3;
                            mob.LastChangeOnState = Environment.TickCount;
                            camp.State = mob.State;
                            camp.LastChangeOnState = mob.LastChangeOnState;
                        }
                    }
                }
            }
            #endregion


            #region ObjectsList

            if (!ObjectsList.Contains(networkID) && (header != Packets.MonsterSkill.Header || length != Packets.MonsterSkill.Length))
            {
                ObjectsList.Add(networkID);
            }
            

            #endregion
        }

        private static void Drawing_OnEndScene(EventArgs args)
        {
            foreach (var camp in Jungle.Camps.Where(camp => camp.MapType.ToString() == Game.MapId.ToString()))
            {
                //Do Stuff for each camp

                #region Minimap Circles

                if (camp.State == 1)
                {
                    LeagueSharp.Common.Utility.DrawCircle(camp.Position, Circleradius, Colorattacking, Circlewidth + 1, 30, true);
                }
                else if (camp.State == 2)
                {
                    LeagueSharp.Common.Utility.DrawCircle(camp.Position, Circleradius, Colordisengaged, Circlewidth + 1, 30, true);
                }
                else if (camp.State == 3 && (camp.IsRanged || (camp.Name == "Dragon" || camp.Name == "Crab" || camp.Name == "Spider")))
                {
                    LeagueSharp.Common.Utility.DrawCircle(camp.Position, Circleradius, Colortracked, Circlewidth, 30, true);
                }
                else if (camp.State == 4)
                {
                    LeagueSharp.Common.Utility.DrawCircle(camp.Position, Circleradius, Colordead, Circlewidth, 30, true);
                }
                else if (camp.State == 5)
                {
                    LeagueSharp.Common.Utility.DrawCircle(camp.Position, Circleradius, Colorguessed, Circlewidth, 30, true);
                }

                #endregion
            }
        }
         
        public static void LoadMenu()
        {
            try
            {
                //Start Menu
                _menu = MainMenu.AddMenu("God Jungle Tracker", "God Jungle Tracker");

                //Ping
                _menu.AddGroupLabel("Ping (Local)");
                _menu.Add("pingdragon", new CheckBox("On Dragon Attack"));
                _menu.Add("pingbaron", new CheckBox("On Baron Attack"));
                _menu.Add("pingsmall", new CheckBox("On Small Camp Attack"));
                _menu.Add("pingfow", new CheckBox("Only On Fog of War"));
                _menu.Add("pingscreen", new CheckBox("Only If Camp Not On Screen"));
                _menu.Add("pingdelay", new Slider("Ping Delay (s)", 10, 1, 20));
                _menu.AddSeparator();

                //Timers
                _menu.AddGroupLabel("Timers");
                _menu.AddGroupLabel("On Map");
                _menu.Add("timeronmapformat", new ComboBox("Format : ", 0, "mm:ss", "ss"));
                _menu.Add("timerfontmap", new Slider("Font Size", 20, 3, 30));
                _menu.Add("timeronmap", new CheckBox("Enabled"));
                _menu.AddSeparator();
                _menu.AddGroupLabel("On Minimap");
                _menu.Add("timeronminimapformat", new ComboBox("Format ", 0, "mm:ss", "ss"));
                _menu.Add("timerfontminimap", new Slider("Font Height", 13, 3, 30));
                _menu.Add("timeronminimap", new CheckBox("Enabled"));
                _menu.AddSeparator();
            
                //Drawing
                _menu.AddGroupLabel("Drawing");
                _menu.Add("circleradius", new Slider("Circle Radius", 300, 1, 500));
                _menu.Add("circlewidth", new Slider("Circle Width", 1, 1, 4));
                _menu.AddSeparator();

                //Advanced
                _menu.AddGroupLabel("Advanced");
                _menu.Add("forcefindheaders", new CheckBox("Force Auto-Find Headers", false));
                _menu.Add("headerOnAttack2" + GameVersion, new Slider("Header OnAttack", 0, 0, 400));
                _menu.Add("headerOnMissileHit2" + GameVersion, new Slider("Header OnMissileHit", 0, 0, 400));
                _menu.Add("headerOnDisengaged" + GameVersion, new Slider("Header OnDisengaged", 0, 0, 400));
                _menu.Add("headerOnMonsterSkill" + GameVersion, new Slider("Header OnMonsterSkill", 0, 0, 400));
                _menu.Add("headerOnCreateGromp" + GameVersion, new Slider("Header OnCreateGromp", 0, 0, 400));
                _menu.Add("headerOnCreateCampIcon" + GameVersion, new Slider("Header OnCreateCampIcon", 0, 0, 400));
                _menu.Add("updatetick", new Slider("Update Tick", 150, 0, 1000));
                _menu.AddSeparator();
                _menu.AddGroupLabel("Headers From Patch: " + GameVersion);
                _menu.AddSeparator();
            }
            catch (Exception)
            {
                Console.WriteLine("[GJT] - Menu Init Failed");
            }
        }
    }
}
