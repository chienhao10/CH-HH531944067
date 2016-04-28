using System;
using LeagueSharp.Common;
using LeagueSharp;
using System.Linq;
using SharpDX;
using EloBuddy;
using EloBuddy.SDK;

namespace NechritoRiven
{
    class Flee
    {
        private static void Game_OnUpdate(EventArgs args)
        {
            FleeLogic();
        }
        public static void FleeLogic()
        {
            /*
            var jump = Program.JumpPos.Where(x => x.Value.Distance(Program.Player.Position) < 300f && x.Value.Distance(Game.CursorPos) < 300f).FirstOrDefault();
            if(Spells._q.IsReady() && Program._qstack < 3)
            { Spells._q.Cast(); }
            if (jump.Value.IsValid())
            {
                ObjectManager.Player.IssueOrder(GameObjectOrder.MoveTo, jump.Value);
                foreach (var pos in Program.JumpPos)
                {
                    if (Game.CursorPos.Distance(pos.Value) <= 350 && ObjectManager.Player.Position.Distance(pos.Value) <= 900 && Spells._q.IsReady() && Spells._e.IsReady() && Program._qstack == 3)
                    {
                        Spells._e.Cast(pos.Value);
                        Spells._q.Cast(pos.Value);
                    }
                    else if (Game.CursorPos.Distance(pos.Value) <= 350 && ObjectManager.Player.Position.Distance(pos.Value) <= 900 && Spells._q.IsReady() && !Spells._e.IsReady() && Program._qstack == 3)
                    {
                        Spells._q.Cast(pos.Value);
                    }
                }
            }
            */
            //if (!MenuConfig.WallFlee)
            // {
            var enemy =
         HeroManager.Enemies.Where(
             hero =>
                 hero.IsValidTarget(Program.Player.HasBuff("RivenFengShuiEngine")
                     ? 70 + 195 + Program.Player.BoundingRadius
                     : 70 + 120 + Program.Player.BoundingRadius) && Spells._w.IsReady());
            var x = Program.Player.Position.Extend(Game.CursorPos, 300);
            var objAiHeroes = enemy as AIHeroClient[] ?? enemy.ToArray();
            if (Spells._q.IsReady() && !Program.Player.LSIsDashing()) Spells._q.Cast(Game.CursorPos);
            if (Spells._w.IsReady() && objAiHeroes.Any()) foreach (var target in objAiHeroes) if (Logic.InWRange(target)) Spells._w.Cast();
            if (Spells._e.IsReady() && !Program.Player.LSIsDashing()) Spells._e.Cast(x);
            //  }
        }
    }
}