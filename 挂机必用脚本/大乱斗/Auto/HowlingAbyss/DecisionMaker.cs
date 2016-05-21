using System;
using System.Linq;
using AutoSharp.Auto.SummonersRift;
using AutoSharp.Utils;
using EloBuddy;
using EloBuddy.SDK;
using SharpDX;


namespace AutoSharp.Auto.HowlingAbyss
{
    internal static class DecisionMaker
    {
        private static int _lastUpdate = 0;
        public static Vector3 IntroducedPos;
        
        public static Vector3 GetOrbPos
        {
            get { return (IntroducedPos == Vector3.Zero) ? Game.CursorPos : IntroducedPos; }
        }

        public static void Goto(Vector3 pos)
        {
            Goto(pos,"auto");
        }
        public static void Goto(Vector3 pos, string from)
        {
            //Chat.Print(from);
            IntroducedPos = pos;
            Orbwalker.OrbwalkTo(pos);
        }
        
        public static void OnUpdate(EventArgs args)
        {
            if (Environment.TickCount - _lastUpdate < 150) return;
            _lastUpdate = Environment.TickCount;

            var player = Heroes.Player;

            if (Decisions.ImSoLonely())
            {
                return;
            }

            //if (Program.Config.Item("autosharp.options.healup").GetValue<bool>() && Decisions.HealUp())
            if(Decisions.HealUp())
            {
                return;
            }

            if (player.UnderTurret(true) && Wizard.GetClosestEnemyTurret().CountNearbyAllyMinions(700) <= 3 && Wizard.GetClosestEnemyTurret().Position.CountAlliesInRange(700) == 0)
            {
                //Program.Orbwalker.ActiveMode = MyOrbwalker.OrbwalkingMode.Mixed;
                Orbwalker.ActiveModesFlags = Orbwalker.ActiveModes.Harass;
                Player.IssueOrder(GameObjectOrder.MoveTo, player.Position.Extend(HeadQuarters.AllyHQ.Position.RandomizePosition(), 800).To3D());
                return;
            }

            if (Heroes.Player.InFountain())
            {
                Shopping.Shop();
                Wizard.AntiAfk();
            }

            if (Decisions.Farm())
            {
                return;
            }
            Decisions.Fight();
            
            
            if (GetOrbPos == Game.CursorPos)
            {
                Decisions.ImSoLonely();
            }
        }
    }
}
