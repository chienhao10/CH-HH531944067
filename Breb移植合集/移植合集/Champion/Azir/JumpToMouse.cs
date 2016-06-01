using EloBuddy.SDK;
using EloBuddy;
using LeagueSharp.Common;
using SharpDX;
using System;
using SAutoCarry.Champions.Helpers;

namespace HeavenStrikeAzir
{
    public static class JumpToMouse
    {
        public static AIHeroClient Player { get { return ObjectManager.Player; } }
        public static int LastJump;
        private static Vector2 CastQLocation, CastELocation, JumpTo;
        private static int CastET, CastQT;

        public static void Initialize()
        {
            Game.OnUpdate += Game_OnUpdate;
            Obj_AI_Base.OnProcessSpellCast += Obj_AI_Base_OnProcessSpellCast;
        }

        public static void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.IsMe)
            {
                if (args.SData.Name == "azirq")
                    Orbwalker.ResetAutoAttack();

                if (Program.eqmouse)
                {
                    if (args.SData.Name == "azire" && Utils.TickCount - CastQT < 500 + Game.Ping)
                    {
                        Program._q.Cast(CastQLocation, true);
                        CastQT = 0;
                    }

                    if (args.SData.Name == "azirq" && Utils.TickCount - CastET < 500 + Game.Ping)
                    {
                        Program._e.Cast(CastELocation, true);
                        CastET = 0;
                    }
                }
            }
        }

        public static void Jump(Vector3 pos, bool juke = false, bool castq = true)
        {
            if (Math.Abs(Program._e.Cooldown) < 0.00001)
            {
                var extended = ObjectManager.Player.ServerPosition.LSTo2D().LSExtend(pos.LSTo2D(), 800f);
                if (!JumpTo.IsValid())
                    JumpTo = pos.LSTo2D();

                if (Program._w.IsReady() && SoldierMgr.ActiveSoldiers.Count == 0)
                {
                    if (juke)
                    {
                        var outRadius = 250 / (float)Math.Cos(2 * Math.PI / 12);

                        for (var i = 1; i <= 12; i++)
                        {
                            var angle = i * 2 * Math.PI / 12;
                            var x = ObjectManager.Player.Position.X + outRadius * (float)Math.Cos(angle);
                            var y = ObjectManager.Player.Position.Y + outRadius * (float)Math.Sin(angle);
                            if (NavMesh.GetCollisionFlags(x, y).HasFlag(CollisionFlags.Wall) && !ObjectManager.Player.ServerPosition.LSTo2D().LSExtend(new Vector2(x, y), 500f).LSIsWall())
                            {
                                Program._w.Cast(ObjectManager.Player.ServerPosition.LSTo2D().LSExtend(new Vector2(x, y), 800f));
                                return;
                            }
                        }
                    }
                    Program._w.Cast(extended);
                }

                if (SoldierMgr.ActiveSoldiers.Count > 0 && Program._q.IsReady())
                {
                    var closestSoldier = SoldierMgr.ActiveSoldiers.MinOrDefault(s => s.Position.LSTo2D().LSDistance(extended, true));
                    CastELocation = closestSoldier.Position.LSTo2D();
                    CastQLocation = closestSoldier.Position.LSTo2D().LSExtend(JumpTo, 800f);

                    if (CastELocation.LSDistance(JumpTo) > ObjectManager.Player.ServerPosition.LSTo2D().LSDistance(JumpTo) && !juke && castq)
                    {
                        CastQLocation = extended;
                        CastET = Utils.TickCount + 250;
                        Program._q.Cast(CastQLocation);
                    }
                    else
                    {
                        Program._e.Cast(CastELocation, true);
                        if (ObjectManager.Player.ServerPosition.LSTo2D().LSDistance(CastELocation) < 700 && castq)
                            LeagueSharp.Common.Utility.DelayAction.Add(250, () => Program._q.Cast(CastQLocation, true));
                    }
                }
            }
            else
            {
                if (Program._q.IsReady() && CastELocation.LSDistance(ObjectManager.Player.ServerPosition) <= 200 && castq)
                    Program._q.Cast(CastQLocation, true);

                JumpTo = Vector2.Zero;
            }
        }

        private static void Game_OnUpdate(EventArgs args)
        {
            if (!Program.eqmouse)
                return;
            if (Orbwalker.CanMove)
            {
                Orbwalker.OrbwalkTo(Game.CursorPos);
            }
            Jump(Game.CursorPos, true);
        }
    }
}
