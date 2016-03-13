using System;
using System.Collections.Generic;
using AdEvade.Utils;
using EloBuddy;

namespace AdEvade.Draw
{
    abstract class RenderObject
    {
        public float EndTime = 0;
        public float StartTime = 0;

        abstract public void Draw();
    }

    class RenderObjects
    {
        private static List<RenderObject> _objects = new List<RenderObject>();
        private static List<RenderObject> _removeList;
        static RenderObjects()
        {
            Drawing.OnDraw += Drawing_OnDraw;
        }

        private static void Drawing_OnDraw(EventArgs args)
        {
            Render();
        }

        private static void Render()
        {
            _removeList = new List<RenderObject>(_objects.Count);
            foreach (RenderObject obj in _objects)
            {                
                if (obj.EndTime - EvadeUtils.TickCount > 0)
                {
                    obj.Draw(); //weird after draw
                }
                else
                {
                    _removeList.Add(obj);
                }
            }
            foreach (var i in _removeList)
            {
                _objects.Remove(i);
            }
            _removeList = null;
        }

        public static void Add(RenderObject obj)
        {
            _objects.Add(obj);
        }
    }
}
