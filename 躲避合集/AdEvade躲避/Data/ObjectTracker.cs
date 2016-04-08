using System;
using System.Collections.Generic;
using System.Linq;
using AdEvade.Utils;
using EloBuddy;
using EloBuddy.SDK;
using SharpDX;

namespace AdEvade.Data
{
    public class ObjectTrackerInfo
    {
        public GameObject Obj;
        public Vector3 Position;
        public Vector3 Direction;
        public string Name;
        public int OwnerNetworkId;
        public bool UsePosition = false;
        public float Timestamp = 0;

        public ObjectTrackerInfo(GameObject obj)
        {
            Obj = obj;
            Name = obj.Name;
            Timestamp = EvadeUtils.TickCount;
        }

        public ObjectTrackerInfo(GameObject obj, string name)
        {
            Obj = obj;
            Name = name;
            Timestamp = EvadeUtils.TickCount;
        }

        public ObjectTrackerInfo(string name, Vector3 position)
        {
            Name = name;
            UsePosition = true;
            Position = position;

            Timestamp = EvadeUtils.TickCount;
        }
    }

    public static class ObjectTracker
    {
        public static Dictionary<int, ObjectTrackerInfo> ObjTracker = new Dictionary<int, ObjectTrackerInfo>();
        public static int ObjTrackerId = 0;

        static ObjectTracker()
        {
            GameObject.OnCreate += HiuCreate_ObjectTracker;
            //Obj_AI_Minion.OnCreate += HiuDelete_ObjectTracker;
        }

        public static void AddObjTrackerPosition(string name, Vector3 position, float timeExpires)
        {
            ObjTracker.Add(ObjTrackerId, new ObjectTrackerInfo(name, position));

            int trackerId = ObjTrackerId; //store the id for deletion
            DelayAction.Add((int) timeExpires, () => ObjTracker.Remove(ObjTrackerId));

            ObjTrackerId += 1;
        }

        private static void HiuCreate_ObjectTracker(GameObject obj, EventArgs args)
        {
            if (obj.IsEnemy && obj.Type == GameObjectType.obj_AI_Minion
                && !ObjTracker.ContainsKey(obj.NetworkId))
            {
                var minion = obj as Obj_AI_Minion;

                if (minion.BaseSkinName.Contains("testcube"))
                {
                    ObjTracker.Add(obj.NetworkId, new ObjectTrackerInfo(obj, "hiu"));
                    DelayAction.Add(250, () => ObjTracker.Remove(obj.NetworkId));
                }
            }
        }

        private static void HiuDelete_ObjectTracker(GameObject obj, EventArgs args)
        {
            if (ObjTracker.ContainsKey(obj.NetworkId))
            {
                ObjTracker.Remove(obj.NetworkId);
            }
        }

        public static Vector2 GetLastHiuOrientation()
        {
            var objList = ObjTracker.Values.Where(o => o.Name == "hiu");
            var sortedObjList = objList.OrderByDescending(o => o.Timestamp);

            if (sortedObjList.Count() >= 2)
            {
                var pos1 = sortedObjList.First().Obj.Position;
                var pos2 = sortedObjList.ElementAt(1).Obj.Position;

                return (pos2.To2D() - pos1.To2D()).Normalized();
            }

            return Vector2.Zero;
        }
    }
}