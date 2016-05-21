using System;
using System.Linq;
using AutoSharp.Auto.HowlingAbyss;
using AutoSharp.Utils;
using EloBuddy;
using EloBuddy.SDK;
using SharpDX;

namespace AutoSharp.Auto.SummonersRift
{
    public static class RoleSwitcher
    {
        private static int _lastSwitchedRoleTick = 0;
        private static bool _paused = false;

        public static void Load()
        {
            if (Minions.AllyMinions.Any())
            {
                Game.OnUpdate += ChooseBest;
            }
            else
            {
                Core.DelayAction(() => Game.OnUpdate += ChooseBest, new Random().Next(17500, 25000));
            }
        }

        public static void Unload()
        {
            Game.OnUpdate -= ChooseBest;
        }

        public static void ChooseBest(EventArgs args)
        {
            if (_paused) return;

            if (Heroes.Player.CountNearbyAllyMinions(800) >
                Minions.EnemyMinions.Count(m => m.Distance(Heroes.Player) < 800))
            {
                Orbwalker.ActiveModesFlags = Orbwalker.ActiveModes.Combo;
                
            }
            else
            {
                Orbwalker.ActiveModesFlags = Orbwalker.ActiveModes.LaneClear;
            }

            if (Environment.TickCount - _lastSwitchedRoleTick > 220000)
            {
                MyTeam.MyRole = MyTeam.Roles.Unknown;
                _lastSwitchedRoleTick = Environment.TickCount;
            }

            if (MyTeam.MyRole == MyTeam.Roles.Unknown)
            {
                if (MyTeam.Midlaner == null) MyTeam.MyRole = MyTeam.Roles.Midlaner;
                if (MyTeam.Support == null) MyTeam.MyRole = MyTeam.Roles.Support;
                if (MyTeam.ADC == null) MyTeam.MyRole = MyTeam.Roles.ADC;
                if (MyTeam.Toplaner == null) MyTeam.MyRole = MyTeam.Roles.Toplaner;
                if (MyTeam.Jungler == null) MyTeam.MyRole = MyTeam.Roles.Toplaner;
            }

            if (MyTeam.MyRole == MyTeam.Roles.Support || MyTeam.MyRole == MyTeam.Roles.ADC)
            {
                BotLaneLogic();
            }

            else if (MyTeam.MyRole == MyTeam.Roles.Midlaner)
            {
                MidLaneLogic();
            }

            else if (MyTeam.MyRole == MyTeam.Roles.Toplaner)
            {
                TopLaneLogic();
            }
            else
            {
                MidLaneLogic();
            }
        }

        public static void BotLaneLogic()
        {
            Vector3 followObjectPos = Vector3.Zero;
            var redZone = Map.BottomLane.Red_Zone;
            var blueZone = Map.BottomLane.Blue_Zone;
            var myOuterTurret = Heroes.Player.Team == GameObjectTeam.Order
                ? Turrets.EnemyTurrets.FirstOrDefault(t => t.Distance(Map.BottomLane.Blue_Outer_Turret) < 950)
                : Turrets.EnemyTurrets.FirstOrDefault(t => t.Distance(Map.BottomLane.Red_Outer_Turret) < 950);

            var minionsOnMyLane =
                Minions.AllyMinions.FindAll(
                    m =>
                        redZone.Points.Any(p => p.Distance(m.Position) < 300) ||
                        blueZone.Points.Any(p => p.Distance(m.Position) < 300)).OrderByDescending(minion => minion.Distance(HeadQuarters.AllyHQ));

            var farthestMinionInLane = minionsOnMyLane.FirstOrDefault();
            var farthestMinionInGame =
                Minions.AllyMinions.OrderByDescending(m => m.Distance(HeadQuarters.AllyHQ)).FirstOrDefault();
            var farthestAlly =
                Heroes.AllyHeroes.OrderByDescending(h => h.Distance(HeadQuarters.AllyHQ)).FirstOrDefault();

            var closestAllyTurret = Wizard.GetClosestAllyTurret();

            if (farthestMinionInLane != null)
            {
                followObjectPos = farthestMinionInLane.Position;
            }
            else if (farthestMinionInGame != null)
            {
                followObjectPos = farthestMinionInGame.Position;
            }
            else if (farthestAlly != null)
            {
                followObjectPos = farthestAlly.Position;
            }
            else if (myOuterTurret != null)
            {
                followObjectPos = myOuterTurret.Position;
            }
            else
            {
                followObjectPos = Game.CursorPos;
            }
            if (followObjectPos != Vector3.Zero)
            {
                //DecisionMaker.Goto(followObjectPos.Extend(closestAllyTurret.Position, Heroes.Player.AttackRange/2).To3D());
            }
        }

        public static void MidLaneLogic()
        {
            Vector3 followObjectPos = Vector3.Zero;
            var redZone = Map.MidLane.Red_Zone;
            var blueZone = Map.MidLane.Blue_Zone;
            var myOuterTurret = Heroes.Player.Team == GameObjectTeam.Order
                ? Turrets.EnemyTurrets.FirstOrDefault(t => t.Distance(Map.MidLane.Blue_Outer_Turret) < 950)
                : Turrets.EnemyTurrets.FirstOrDefault(t => t.Distance(Map.MidLane.Red_Outer_Turret) < 950);

            var minionsOnMyLane =
                Minions.AllyMinions.FindAll(
                    m =>
                        redZone.Points.Any(p => p.Distance(m.Position) < 300) ||
                        blueZone.Points.Any(p => p.Distance(m.Position) < 300)).OrderByDescending(minion => minion.Distance(HeadQuarters.AllyHQ));

            var farthestMinionInLane = minionsOnMyLane.FirstOrDefault();
            var farthestMinionInGame =
                Minions.AllyMinions.OrderByDescending(m => m.Distance(HeadQuarters.AllyHQ)).FirstOrDefault();
            var farthestAlly =
                Heroes.AllyHeroes.OrderByDescending(h => h.Distance(HeadQuarters.AllyHQ)).FirstOrDefault();

            var closestAllyTurret = Wizard.GetClosestAllyTurret();

            if (farthestMinionInLane != null)
            {
                followObjectPos = farthestMinionInLane.Position;
            }
            else if (farthestMinionInGame != null)
            {
                followObjectPos = farthestMinionInGame.Position;
            }
            else if (farthestAlly != null)
            {
                followObjectPos = farthestAlly.Position;
            }
            else if (myOuterTurret != null)
            {
                followObjectPos = myOuterTurret.Position;
            }
            else
            {
                followObjectPos = Game.CursorPos;
            }
            if (followObjectPos != Vector3.Zero)
            {
                //DecisionMaker.Goto(followObjectPos.Extend(closestAllyTurret.Position, Heroes.Player.AttackRange / 2).To3D());
            }
        }

        public static void TopLaneLogic()
        {
            Vector3 followObjectPos = Vector3.Zero;
            var redZone = Map.TopLane.Red_Zone;
            var blueZone = Map.TopLane.Blue_Zone;
            var myOuterTurret = Heroes.Player.Team == GameObjectTeam.Order
                ? Turrets.EnemyTurrets.FirstOrDefault(t => t.Distance(Map.TopLane.Blue_Outer_Turret) < 950)
                : Turrets.EnemyTurrets.FirstOrDefault(t => t.Distance(Map.TopLane.Red_Outer_Turret) < 950);

            var minionsOnMyLane =
                Minions.AllyMinions.FindAll(
                    m =>
                        redZone.Points.Any(p => p.Distance(m.Position) < 300) ||
                        blueZone.Points.Any(p => p.Distance(m.Position) < 300)).OrderByDescending(minion => minion.Distance(HeadQuarters.AllyHQ));

            var farthestMinionInLane = minionsOnMyLane.FirstOrDefault();
            var farthestMinionInGame =
                Minions.AllyMinions.OrderByDescending(m => m.Distance(HeadQuarters.AllyHQ)).FirstOrDefault();
            var farthestAlly =
                Heroes.AllyHeroes.OrderByDescending(h => h.Distance(HeadQuarters.AllyHQ)).FirstOrDefault();

            var closestAllyTurret = Wizard.GetClosestAllyTurret();

            if (farthestMinionInLane != null)
            {
                followObjectPos = farthestMinionInLane.Position;
            }
            else if (farthestMinionInGame != null)
            {
                followObjectPos = farthestMinionInGame.Position;
            }
            else if (farthestAlly != null)
            {
                followObjectPos = farthestAlly.Position;
            }
            else if (myOuterTurret != null)
            {
                followObjectPos = myOuterTurret.Position;
            }
            else
            {
                followObjectPos = Game.CursorPos;
            }
            if (followObjectPos != Vector3.Zero)
            {
                //DecisionMaker.Goto(followObjectPos.Extend(closestAllyTurret.Position, Heroes.Player.AttackRange / 2).To3D());
            }
        }

        public static void Pause()
        {
            _paused = true;
        }

        public static void Unpause()
        {
            _paused = false;
        }
    }
}
