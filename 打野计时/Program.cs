using System;
using System.Collections.Generic;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using SharpDX;
using EloBuddy.SDK.Rendering;
using Color1 = System.Drawing.Color;

namespace JungleTimers
{
    class Program
    {
        public static Menu JungleTimer;
        public static Text txt;
        public static int Size;
        public static List<Mobs> JungleMobs { get; set; }
        public static IEnumerable<Mobs> DeadMobs
        {
            get
            {
                return JungleMobs.Where(x => x.Dead);
            }
        }
        static void Main(string[] args)
        {
            Loading.OnLoadingComplete += Loading_OnLoadingComplete;
        }

        private static void Loading_OnLoadingComplete(EventArgs args)
        {
            Bootstrap.Init(null);
            Init();
            
            JungleTimer = MainMenu.AddMenu("打野计时", "jungletimers");
            JungleTimer.AddGroupLabel("野区计时");
            JungleTimer.AddSeparator();
            JungleTimer.Add("active", new CheckBox("开启", true));
            JungleTimer.AddSeparator();
            JungleTimer.Add("size", new Slider("文字大小", 7, 1, 30));
            JungleTimer.AddSeparator();
            JungleTimer.AddLabel("打野可以配合自动惩戒使用.");
            Size = JungleTimer["size"].Cast<Slider>().CurrentValue;
            txt = new Text("", new System.Drawing.Font(System.Drawing.FontFamily.GenericSansSerif, Size, System.Drawing.FontStyle.Bold));

            GameObject.OnCreate += GameObject_OnCreate;
            GameObject.OnDelete += GameObject_OnDelete;
            Drawing.OnEndScene += Drawing_OnEndScene;
        }

        private static void Drawing_OnEndScene(EventArgs args)
        {
            if (!JungleTimer["active"].Cast<CheckBox>().CurrentValue)
                return;

            foreach (var camp in DeadMobs.Where(x => x.NextRespawnTime - Environment.TickCount > 0))
            {
                var timeSpan = TimeSpan.FromMilliseconds(camp.NextRespawnTime - Environment.TickCount);
                var text = timeSpan.ToString(@"m\:ss");

                txt.Position = new Vector2((int)camp.MinimapPosition.X - Size / 2, (int)camp.MinimapPosition.Y - Size / 2);
                txt.Color = Color1.White;
                txt.TextValue = text;
                txt.Draw();

            }
        }

        private static void GameObject_OnDelete(GameObject sender, EventArgs args)
        {

            if (sender.Type != GameObjectType.obj_AI_Minion)
            {
                return;
            }

            var mob =
                JungleMobs.FirstOrDefault(
                    x => x.MobNames.Select(y => y.ToLower()).Any(z => z.Equals(sender.Name.ToLower())));

            if (mob == null)
            {
                return;
            }

            mob.ObjectsDead.Add(sender.Name);
            mob.ObjectsAlive.Remove(sender.Name);

            if (mob.ObjectsDead.Count != mob.MobNames.Length)
            {
                return;
            }

            mob.Dead = true;
            mob.NextRespawnTime = Environment.TickCount + mob.RespawnTime - 3000;
        }

        private static void GameObject_OnCreate(GameObject sender, EventArgs args)
        {
            if (sender.Type != GameObjectType.obj_AI_Minion)
            {
                return;
            }
            var mob =
                JungleMobs.FirstOrDefault(
                    x => x.MobNames.Select(y => y.ToLower()).Any(z => z.Equals(sender.Name.ToLower())));

            if (mob == null)
            {
                return;
            }

            mob.ObjectsAlive.Add(sender.Name);
            mob.ObjectsDead.Remove(sender.Name);

            if (mob.ObjectsAlive.Count != mob.MobNames.Length)
            {
                return;
            }

            mob.Dead = false;
            mob.NextRespawnTime = 0;
        }

        private static void Init()
        {
            #region Init Mobs List
            JungleMobs = new List<Mobs>
                              {
                                  new Mobs(
                                      75000,
                                      new Vector3(6078.15f, 6094.45f, -98.63f),
                                      new[] { "TT_NWolf3.1.1", "TT_NWolf23.1.2", "TT_NWolf23.1.3" },
                                      GameMapId.TwistedTreeline,
                                      GameObjectTeam.Order),
                                  new Mobs(
                                      100000,
                                      new Vector3(6943.41f, 5422.61f, 52.62f),
                                      new[] { "SRU_Razorbeak3.1.1", "SRU_RazorbeakMini3.1.2", "SRU_RazorbeakMini3.1.3" },
                                      GameMapId.SummonersRift,
                                      GameObjectTeam.Order),
                                  new Mobs(
                                      100000,
                                      new Vector3(2164.34f, 8383.02f, 51.78f),
                                      new[] { "SRU_Gromp13.1.1" },
                                      GameMapId.SummonersRift,
                                      GameObjectTeam.Order),
                                  new Mobs(
                                      100000,
                                      new Vector3(8370.58f, 2718.15f, 51.09f),
                                      new[] { "SRU_Krug5.1.2", "SRU_KrugMini5.1.1" },
                                      GameMapId.SummonersRift,
                                      GameObjectTeam.Order),
                                  new Mobs(
                                      180000,
                                      new Vector3(4285.04f, 9597.52f, -67.6f),
                                      new[] { "SRU_Crab16.1.1" },
                                      GameMapId.SummonersRift,
                                      GameObjectTeam.Neutral),
                                  new Mobs(
                                      100000,
                                      new Vector3(6476.17f, 12142.51f, 56.48f),
                                      new[] { "SRU_Krug11.1.2", "SRU_KrugMini11.1.1" },
                                      GameMapId.SummonersRift,
                                      GameObjectTeam.Chaos),
                                  new Mobs(
                                      75000,
                                      new Vector3(11025.95f, 5805.61f, -107.19f),
                                      new[] { "TT_NWraith4.1.1", "TT_NWraith24.1.2", "TT_NWraith24.1.3" },
                                      GameMapId.TwistedTreeline,
                                      GameObjectTeam.Chaos),
                                  new Mobs(
                                      100000,
                                      new Vector3(10983.83f, 8328.73f, 62.22f),
                                      new[] { "SRU_Murkwolf8.1.1", "SRU_MurkwolfMini8.1.2", "SRU_MurkwolfMini8.1.3" },
                                      GameMapId.SummonersRift,
                                      GameObjectTeam.Chaos),
                                  new Mobs(
                                      100000,
                                      new Vector3(12671.83f, 6306.6f, 51.71f),
                                      new[] { "SRU_Gromp14.1.1" },
                                      GameMapId.SummonersRift,
                                      GameObjectTeam.Chaos),
                                  new Mobs(
                                      360000,
                                      new Vector3(7738.3f, 10079.78f, -61.6f),
                                      new[] { "TT_Spiderboss8.1.1" },
                                      GameMapId.SummonersRift,
                                      GameObjectTeam.Neutral),
                                  new Mobs(
                                      300000,
                                      new Vector3(3800.99f, 7883.53f, 52.18f),
                                      new[] { "SRU_Blue1.1.1", "SRU_BlueMini1.1.2", "SRU_BlueMini21.1.3" },
                                      GameMapId.SummonersRift,
                                      GameObjectTeam.Order),
                                  new Mobs(
                                      75000,
                                      new Vector3(4373.14f, 5842.84f, -107.14f),
                                      new[] { "TT_NWraith1.1.1", "TT_NWraith21.1.2", "TT_NWraith21.1.3" },
                                      GameMapId.TwistedTreeline,
                                      GameObjectTeam.Order),
                                  new Mobs(
                                      300000,
                                      new Vector3(4993.14f, 10491.92f, -71.24f),
                                      new[] { "SRU_RiftHerald" },
                                      GameMapId.SummonersRift,
                                      GameObjectTeam.Neutral),
                                  new Mobs(
                                      75000,
                                      new Vector3(5106.94f, 7985.9f, -108.38f),
                                      new[] { "TT_NGolem2.1.1", "TT_NGolem22.1.2" },
                                      GameMapId.TwistedTreeline,
                                      GameObjectTeam.Order),
                                  new Mobs(
                                      100000,
                                      new Vector3(7852.38f, 9562.62f, 52.3f),
                                      new[] { "SRU_Razorbeak9.1.1", "SRU_RazorbeakMini9.1.2", "SRU_RazorbeakMini9.1.3" },
                                      GameMapId.SummonersRift,
                                      GameObjectTeam.Chaos),
                                  new Mobs(
                                      300000,
                                      new Vector3(10984.11f, 6960.31f, 51.72f),
                                      new[] { "SRU_Blue7.1.1", "SRU_BlueMini7.1.2", "SRU_BlueMini27.1.3" },
                                      GameMapId.SummonersRift,
                                      GameObjectTeam.Chaos),
                                  new Mobs(
                                      180000,
                                      new Vector3(10647.7f, 5144.68f, -62.81f),
                                      new[] { "SRU_Crab15.1.1" },
                                      GameMapId.SummonersRift,
                                      GameObjectTeam.Neutral),
                                  new Mobs(
                                      75000,
                                      new Vector3(9294.02f, 6085.41f, -96.7f),
                                      new[] { "TT_NWolf6.1.1", "TT_NWolf26.1.2", "TT_NWolf26.1.3" },
                                      GameMapId.TwistedTreeline,
                                      GameObjectTeam.Chaos),
                                  new Mobs(
                                      420000,
                                      new Vector3(4993.14f, 10491.92f, -71.24f),
                                      new[] { "SRU_Baron12.1.1" },
                                      GameMapId.SummonersRift,
                                      GameObjectTeam.Neutral),
                                  new Mobs(
                                      100000,
                                      new Vector3(3849.95f, 6504.36f, 52.46f),
                                      new[] { "SRU_Murkwolf2.1.1", "SRU_MurkwolfMini2.1.2", "SRU_MurkwolfMini2.1.3" },
                                      GameMapId.SummonersRift,
                                      GameObjectTeam.Order),
                                  new Mobs(
                                      300000,
                                      new Vector3(7813.07f, 4051.33f, 53.81f),
                                      new[] { "SRU_Red4.1.1", "SRU_RedMini4.1.2", "SRU_RedMini4.1.3" },
                                      GameMapId.SummonersRift,
                                      GameObjectTeam.Order),
                                  new Mobs(
                                      360000,
                                      new Vector3(9813.83f, 4360.19f, -71.24f),
                                      new[] { "SRU_Dragon6.1.1" },
                                      GameMapId.SummonersRift,
                                      GameObjectTeam.Neutral),
                                  new Mobs(
                                      300000,
                                      new Vector3(7139.29f, 10779.34f, 56.38f),
                                      new[] { "SRU_Red10.1.1", "SRU_RedMini10.1.2", "SRU_RedMini10.1.3" },
                                      GameMapId.SummonersRift,
                                      GameObjectTeam.Chaos),
                                  new Mobs(
                                      75000,
                                      new Vector3(10276.81f, 8037.54f, -108.92f),
                                      new[] { "TT_NGolem5.1.1", "TT_NGolem25.1.2" },
                                      GameMapId.TwistedTreeline,
                                      GameObjectTeam.Chaos)
                              };
            #endregion
            JungleMobs = JungleMobs.Where(x => x.MapID == Game.MapId).ToList();

        }
        
    }
}
