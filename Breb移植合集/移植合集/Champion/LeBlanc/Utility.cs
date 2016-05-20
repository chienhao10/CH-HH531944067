using System;
using System.Collections.Generic;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using LeagueSharp.Common;
using SharpDX;
using Color = System.Drawing.Color;

namespace PopBlanc
{
    public static class UtilityA
    {
        public static bool HasEBuff(this Obj_AI_Base unit)
        {
            return unit.HasBuff("leblancsoulshackle");
        }

        public static bool HasQBuff(this Obj_AI_Base unit)
        {
            return unit.HasBuff("leblancchaosorb");
        }

        public static bool HasQRBuff(this Obj_AI_Base unit)
        {
            return unit.HasBuff("leblancchaosorbm");
        }

        public static bool IsValidWPoint(this Vector3 position)
        {
            if (!position.IsValid())
            {
                return false;
            }

            for (var i = 10; i < SpellManager.W.Range; i += 10)
            {
                if (i > SpellManager.W.Range)
                {
                    return false;
                }

                if (ObjectManager.Player.ServerPosition.LSExtend(position, i).IsWall())
                {
                    return true;
                }
            }

            return false;
        }
    }

    public class WBackPosition
    {
        public static List<WBackPosition> Positions = new List<WBackPosition>();
        private readonly int spawnTime;
        public int EndTime;
        public GameObject Obj;

        public WBackPosition(GameObject obj)
        {
            Obj = obj;
            spawnTime = Utils.TickCount;
            EndTime = spawnTime + 4200;
            Positions.Add(this);
        }

        public bool IsR
        {
            get { return Obj.Name.Contains("RW"); }
        }

        public static void Initialize()
        {
            GameObject.OnCreate += GameObject_OnCreate;
            GameObject.OnDelete += GameObject_OnDelete;
            Drawing.OnDraw += Drawing_OnDraw;
        }

        private static void Drawing_OnDraw(EventArgs args)
        {
            foreach (var w in Positions.Where(p => p.Obj.IsValid))
            {
                var pos = Drawing.WorldToScreen(w.Obj.Position);
                var time = (w.EndTime - Utils.TickCount)/1000f;
                Drawing.DrawText(pos.X - 10, pos.Y - 100, Color.Red, string.Format("{0:0.0}", time));
                Render.Circle.DrawCircle(w.Obj.Position, 150, Color.Red);
            }
        }

        private static void GameObject_OnCreate(GameObject sender, EventArgs args)
        {
            if (sender == null || !sender.IsValid || sender.DistanceToPlayer() > SpellManager.W.Range ||
                !sender.Name.Contains("return_indicator"))
            {
                return;
            }

            Console.WriteLine("ADD " + sender.Name);
            new WBackPosition(sender);
        }

        private static void GameObject_OnDelete(GameObject sender, EventArgs args)
        {
            Positions.RemoveAll(p => p.Obj.NetworkId == sender.NetworkId);
        }
    }
}