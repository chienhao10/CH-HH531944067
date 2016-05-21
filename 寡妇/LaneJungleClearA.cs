using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu.Values;

namespace Evelynn
{
    internal static class LaneJungleClearA
    {

        public static AIHeroClient Evelynn
        {
            get { return ObjectManager.Player; }
        }

        public static void LaneClearB()
        {
            var echeck = Program.LaneJungleClear["LCE"].Cast<CheckBox>().CurrentValue;
            var eready = Program.E.IsReady();
            var qcheck = Program.LaneJungleClear["LCQ"].Cast<CheckBox>().CurrentValue;
            var qready = Program.Q.IsReady();
            foreach (
               var minioon in EntityManager.MinionsAndMonsters.GetLaneMinions(EntityManager.UnitTeam.Enemy))
            { 

            if (echeck && eready) 
            {

                if (minioon != null)
                    Program.E.Cast(minioon);
            }

            if (qcheck && qready)
            {

                if (minioon != null)
                    Program.Q.Cast();
            }
        }}

        public static void JungleClearB()
        {
            foreach (
                var minion in EntityManager.MinionsAndMonsters.Get(EntityManager.MinionsAndMonsters.EntityType.Monster))
            {
                var echeck = Program.LaneJungleClear["LCE"].Cast<CheckBox>().CurrentValue;
                var eready = Program.E.IsReady();
                var qcheck = Program.LaneJungleClear["LCQ"].Cast<CheckBox>().CurrentValue;
                var qready = Program.Q.IsReady();
                { 
                if (qcheck && qready)
                if (minion != null)
                    Program.Q.Cast();
            }

                if (echeck && eready) 
                {
                    if (minion != null)
                        Program.E.Cast(minion);
                }
        }
    }
    }
}