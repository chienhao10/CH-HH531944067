using System;
using System.Collections.Generic;
using System.Linq;
using EloBuddy;
using SharpDX;

// ReSharper disable InconsistentNaming

namespace Auto_Carry_Vayne.Manager
{
    public static class Traps
    {
        private static List<GameObject> _traps;

        private static readonly List<string> _trapNames = new List<string> { "teemo", "shroom", "trap", "mine", "ziggse_red", "jhin" };

        public static List<GameObject> EnemyTraps
        {
            get { return _traps.FindAll(t => t.IsValid && t.IsEnemy); }
        }

        public static void OnCreate(GameObject sender, EventArgs args)
        {
            foreach (var trapName in _trapNames)
            {
                if (sender.Name.ToLower().Contains(trapName)) _traps.Add(sender);
            }
        }

        public static void OnDelete(GameObject sender, EventArgs args)
        {
            _traps.RemoveAll(trap => trap.NetworkId == sender.NetworkId);
        }

        public static void Load()
        {
            _traps = new List<GameObject>();
            GameObject.OnCreate += OnCreate;
            GameObject.OnDelete += OnDelete;
        }
    }
}
