using System.Linq;
using AutoSharp.Utils;
using EloBuddy;
using EloBuddy.SDK;

namespace AutoSharp.Auto.SummonersRift
{
    public static class MyTeam
    {
        public enum Roles
        {
            Toplaner,
            Midlaner,
            Jungler,
            Support,
            ADC,
            Unknown
        }

        public static AIHeroClient Toplaner;
        public static AIHeroClient Midlaner;
        public static AIHeroClient Jungler;
        public static AIHeroClient Support;
        public static AIHeroClient Player = Heroes.Player;
        public static AIHeroClient ADC;
        public static Roles MyRole = Roles.Unknown;

        public static void Update()
        {
            if (Jungler == null)
            {
                Jungler = Heroes.AllyHeroes.FirstOrDefault(h => h.GetSpellSlotFromName("summonersmite") != SpellSlot.Unknown || h.HasItem(ItemId.Hunters_Machete));
            }
            if (Midlaner == null)
            {
                var playerPos = ObjectManager.Player.ServerPosition.To2D();
                Midlaner = Heroes.AllyHeroes.FirstOrDefault(h => h != Jungler && (Map.MidLane.Red_Zone.Points.Any(p => p.Distance(playerPos) < 200) || Map.MidLane.Blue_Zone.Points.Any(p => p.Distance(playerPos) < 200)));
            }
            if (Support == null)
            {
                var playerPos = ObjectManager.Player.ServerPosition.To2D();
                Support = Heroes.AllyHeroes.FirstOrDefault(h => h != Jungler && (Map.BottomLane.Red_Zone.Points.Any(p => p.Distance(playerPos) < 200) || Map.BottomLane.Blue_Zone.Points.Any(p => p.Distance(playerPos) < 200)));
            }
            if (Toplaner == null)
            {
                var playerPos = ObjectManager.Player.ServerPosition.To2D();
                Toplaner = Heroes.AllyHeroes.FirstOrDefault(h => h != Jungler && (Map.BottomLane.Red_Zone.Points.Any(p => p.Distance(playerPos) < 200) || Map.BottomLane.Blue_Zone.Points.Any(p => p.Distance(playerPos) < 200)));
            }
            if (Support != null && ADC == null)
            {
                var playerPos = ObjectManager.Player.ServerPosition.To2D();
                ADC = Heroes.AllyHeroes.FirstOrDefault(h => h != Jungler && h != Support && (Map.BottomLane.Red_Zone.Points.Any(p => p.Distance(playerPos) < 200) || Map.BottomLane.Blue_Zone.Points.Any(p => p.Distance(playerPos) < 200)));
            }
        }
    }
}
