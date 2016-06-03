using System;
using System.Collections.Generic;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using EloBuddy;

namespace VCursor
{
    internal static class MouseManager
    {
        private static MousePath _currentPath;

        static MouseManager()
        {
            Game.OnUpdate += Game_OnUpdate;
            Player.OnIssueOrder += Obj_AI_Base_OnIssueOrder;
            Spellbook.OnCastSpell += Spellbook_OnCastSpell;
        }

        public static int LastPath { get; private set; }

        public static bool FollowingPath => _currentPath != null && !_currentPath.Finished;


        public static void StartPath(Vector2 start, Vector2 end)
        {
            if (FollowingPath)
            {
                CancelPath();
            }

            LastPath = Utils.TickCount;
            _currentPath = new MousePath(start, end);
        }

        public static void StartPath(Vector2 end)
        {
            _currentPath = new MousePath(VirtualCursor.Position, end);
        }

        public static void StartPathWorld(Vector3 end)
        {
            var screenEnd = end.ToScreenPoint();

            if (!screenEnd.IsValid())
            {
                return;
            }

            _currentPath = new MousePath(VirtualCursor.Position, screenEnd);
        }

        public static void CancelPath()
        {
            _currentPath = null;
        }

        private static void Game_OnUpdate(EventArgs args)
        {
            if (!FollowingPath || !_currentPath.CanMove)
            {
                return;
            }

            var point = _currentPath.NextPoint;

            if (point == Vector2.Zero)
            {
                return;
            }

            if (_currentPath.Finished)
            {
                VirtualCursor.UpdateIcon(point);
            }

            VirtualCursor.SetPosition(point);
        }

        private static void Spellbook_OnCastSpell(Spellbook sender, SpellbookCastSpellEventArgs args)
        {
            if (sender == null || !sender.HasSpellCaster || !sender.Owner.IsMe)
            {
                return;
            }

            var target = args.Target as Obj_AI_Base;
            var castTime = sender.GetSpell(args.Slot).SData.CastFrame / 30f * 1000f + Game.Ping / 2f;

            // target offscreen
            if (target != null)
            {
                if (!target.IsHPBarRendered) {}

                //StartPathWorld(target.ServerPosition);
            }


            //StartPathWorld(args.StartPosition);
        }

        private static void Obj_AI_Base_OnIssueOrder(Obj_AI_Base sender, PlayerIssueOrderEventArgs args)
        {
            if (sender == null || !sender.IsValid || !sender.IsMe)
            {
                return;
            }

            if (args.Order == GameObjectOrder.Stop || args.Order == GameObjectOrder.HoldPosition)
            {
                return;
            }

            var target = args.Target as Obj_AI_Base;

            // target offscreen
            if (target != null && !target.IsHPBarRendered) {}

            //StartPathWorld(args.TargetPosition);
        }

        internal class MousePath
        {
            private readonly Queue<Vector2> _path;

            public MousePath(Vector2 start, Vector2 end)
            {
                _path = GeneratePath(start, end);
            }


            public Vector2 NextPoint => _path.Dequeue();

            public bool Finished => _path.Count == 0;

            public bool CanMove => !Finished;

            private static Queue<Vector2> GeneratePath(Vector2 start, Vector2 end)
            {
                //return PathGenerator.GeneratePath(start.ToWorldPoint().To2D(), end.ToWorldPoint().To2D());
                var d = start.LSDistance(end);

                var path = new Queue<Vector2>();

                if (d < 75)
                {
                    path.Enqueue(end);
                    return path;
                }

                var increment = (int) d / 30; //(2 * d / FPS)
                var count = 0;
                for (var i = 0; i < d; i += increment)
                {
                    if (i > d)
                    {
                        break;
                    }

                    var point = start.LSExtend(end, i);
                    if (count++ % 10 == 0)
                    {
                        point.Randomize(10, 50);

                        if (count % 6 == 0)
                        {
                            point.Randomize(50, 100);
                        }
                    }

                    path.Enqueue(point);
                }

                path.Enqueue(end);
                return path;
            }
        }
    }
}