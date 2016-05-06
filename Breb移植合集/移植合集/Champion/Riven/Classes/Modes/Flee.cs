using System;
using LeagueSharp.Common;
using LeagueSharp;
using System.Linq;
using SharpDX;
using EloBuddy;

namespace NechritoRiven
{
    class Flee : Program
    {
        private static void Game_OnUpdate(EventArgs args)
        {
            FleeLogic();
        }
        public static void FleeLogic()
        {
            if (!MenuConfig.WallFlee)
            {
                return;
            }

            var end = Player.ServerPosition.LSExtend(Game.CursorPos, Spells._q.Range);
            var IsWallDash = FleeLOGIC.IsWallDash(end, Spells._q.Range);

            var Eend = Player.ServerPosition.LSExtend(Game.CursorPos, Spells._e.Range);
            var WallE = FleeLOGIC.GetFirstWallPoint(Player.ServerPosition, Eend);
            var WallPoint = FleeLOGIC.GetFirstWallPoint(Player.ServerPosition, end);

            if (Spells._q.IsReady() && _qstack < 3)
            { Spells._q.Cast(Game.CursorPos); }


            if (IsWallDash && _qstack == 3 && WallPoint.LSDistance(Player.ServerPosition) <= 800)
            {
                EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, WallPoint);
                if (WallPoint.LSDistance(Player.ServerPosition) <= 600)
                {
                    EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, WallPoint);
                    if (WallPoint.LSDistance(Player.ServerPosition) < 55)
                    {
                        if (Spells._e.IsReady())
                        {
                            Spells._e.Cast(WallE);
                        }
                        if (_qstack == 3)
                        {
                            EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, WallPoint);
                            Spells._q.Cast(WallPoint);
                        }

                    }
                }
            }

            if (!IsWallDash && !MenuConfig.WallFlee)
            {
                var enemy =
             HeroManager.Enemies.Where(
                 hero =>
                     hero.LSIsValidTarget(Program.Player.HasBuff("RivenFengShuiEngine")
                         ? 70 + 195 + Program.Player.BoundingRadius
                         : 70 + 120 + Program.Player.BoundingRadius) && Spells._w.IsReady());
                var x = Program.Player.Position.LSExtend(Game.CursorPos, 300);
                var objAiHeroes = enemy as AIHeroClient[] ?? enemy.ToArray();
                if (Spells._q.IsReady() && !Program.Player.LSIsDashing()) Spells._q.Cast(Game.CursorPos);
                if (Spells._w.IsReady() && objAiHeroes.Any()) foreach (var target in objAiHeroes) if (Logic.InWRange(target)) Spells._w.Cast();
                if (Spells._e.IsReady() && !Player.LSIsDashing()) Spells._e.Cast(x);
            }
        }
    }
}