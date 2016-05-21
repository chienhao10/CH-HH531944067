using System;
using System.Collections.Generic;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;

// ReSharper disable InconsistentNaming

namespace AutoSharp.Utils
{
    public static class Traps
    {
        public static List<GameObject> EnemyTraps
        {
            get
            {
                return ObjectManager.Get<GameObject>().Where(o =>
                {
                    var name = o.Name.ToLower();
                    return name.Contains("teemo") || name.Contains("shroom") || name.Contains("trap") ||
                           name.Contains("mine");
                }).ToList();
            }
        }
    }

    public static class HealingBuffs
    {
        private static List<GameObject> _healingBuffs;
        private static int LastUpdate = 0;

        public static List<GameObject> AllyBuffs
        {
            get { return _healingBuffs.FindAll(hb => hb.IsValid && hb.Distance(HeadQuarters.AllyHQ.Position) < 5400).OrderBy(buff => buff.Position.Distance(Heroes.Player.Position)).ToList(); }
        }

        public static List<GameObject> EnemyBuffs
        {
            get { return _healingBuffs.FindAll(hb => hb.IsValid && hb.Distance(HeadQuarters.AllyHQ.Position) > 5400); }
        }

        public static void Load()
        {
            _healingBuffs = ObjectManager.Get<GameObject>().Where(h=>h.Name.Contains("healingBuff")).ToList();
            GameObject.OnCreate += OnCreate;
            GameObject.OnDelete += OnDelete;
            Game.OnUpdate += UpdateBuffs;
        }

        private static void UpdateBuffs(EventArgs args)
        {
            if (Environment.TickCount > LastUpdate + 1000)
            {
                foreach (var buff in _healingBuffs)
                {
                    if (Heroes.Player.ServerPosition.Distance(buff.Position) < 80) _healingBuffs.Remove(buff);
                }
                LastUpdate = Environment.TickCount;
            }
        }

        private static void OnCreate(GameObject sender, EventArgs args)
        {
            if (sender.Name.Contains("healingBuff"))
            {
                _healingBuffs.Add(sender);
            }
        }

        private static void OnDelete(GameObject sender, EventArgs args)
        {
            var iList = _healingBuffs.Where(buff => buff.NetworkId == sender.NetworkId);
            foreach (var i in iList)
            {
                _healingBuffs.Remove(i);
            }
        }
    }

    public static class Turrets
    {
        private static List<Obj_AI_Turret> _turrets;

        public static List<Obj_AI_Turret> AllyTurrets
        {
            get { return _turrets.FindAll(t => t.IsValid() && !t.IsDead && t.IsAlly && !t.Name.ToLower().Contains("shrine")); }
        }

        public static List<Obj_AI_Turret> EnemyTurrets
        {
            get { return _turrets.FindAll(t => t.IsValid() && !t.IsDead && t.IsEnemy && !t.Name.ToLower().Contains("shrine")); }
        }

        public static Obj_AI_Turret ClosestEnemyTurret
        {
            get { return EnemyTurrets.OrderBy(t => t.Distance(Heroes.Player)).FirstOrDefault(); }
        }

        public static void Load()
        {
            _turrets = ObjectManager.Get<Obj_AI_Turret>().ToList();
            Obj_AI_Turret.OnCreate += OnCreate;
            Obj_AI_Turret.OnDelete += OnDelete;
        }

        private static void OnCreate(GameObject sender, EventArgs args)
        {
            if (sender.IsValid) _turrets.Add((Obj_AI_Turret)sender);
        }

        private static void OnDelete(GameObject sender, EventArgs args)
        {
            var iList = _turrets.Where(turret => turret.NetworkId == sender.NetworkId);
            foreach (var i in iList)
            {
                _turrets.Remove(i);
            }
        }
    }

    public static class HeadQuarters
    {
        public static Obj_HQ AllyHQ
        {
            get { return ObjectManager.Get<Obj_HQ>().FirstOrDefault(hq => hq.IsAlly); }
        }
        public static Obj_HQ EnemyHQ
        {
            get { return ObjectManager.Get<Obj_HQ>().FirstOrDefault(hq => hq.IsEnemy); }
        }
    }

    public static class Heroes
    {
        private static List<AIHeroClient> _heroes;

        public static AIHeroClient Player = ObjectManager.Player;

        /// <summary>
        /// Ally Heroes, excluding the player
        /// </summary>
        public static List<AIHeroClient> AllyHeroes
        {
            get { return _heroes.FindAll(h => h.IsAlly); }
        }

        public static List<AIHeroClient> EnemyHeroes
        {
            get { return _heroes.FindAll(h => h.IsEnemy); }
        }

        public static void Load()
        {
            Player = ObjectManager.Player;
            _heroes = ObjectManager.Get<AIHeroClient>().Where(h=>!h.IsMe).ToList();
        }
    }

    public static class Minions
    {
        
        private static List<Obj_AI_Minion> _minions;

        public static List<Obj_AI_Minion> AllyMinions
        {
            get { return _minions.FindAll(t => t.IsValid() && !t.IsDead && t.IsAlly); }
        }
        public static List<Obj_AI_Minion> EnemyMinions
        {
            get { return _minions.FindAll(t => t.IsValid() && !t.IsDead && t.IsValidTarget()); }
        }

        public static void Load()
        {
            _minions = new List<Obj_AI_Minion>();
            Obj_AI_Minion.OnCreate += OnCreate;
            Obj_AI_Minion.OnDelete += OnDelete;
        }

        private static void OnDelete(GameObject sender, EventArgs args)
        {
            var iList = new List<Obj_AI_Minion>();
            foreach (var minion in _minions)
            {
                if (minion.NetworkId == sender.NetworkId) iList.Add(minion);
            }
            foreach (var i in iList)
            {
                _minions.Remove(i);
            }
        }

        private static void OnCreate(GameObject sender, EventArgs args)
        {
            var name = sender.Name.ToLower();
            if (sender.IsValid && !name.Contains("sru_") && !name.Contains("ward") && !name.Contains("ttn") && !name.Contains("tt_") && !name.Contains("trinket") && !name.Contains("teemo") && sender.Team != GameObjectTeam.Neutral) _minions.Add((Obj_AI_Minion)sender);
        }

    }
    public static class JungleCamps
    {

        private static List<Obj_AI_Minion> _minions;

        public static List<Obj_AI_Minion> Mobs
        {
            get { return _minions.FindAll(t => t.IsValid() && !t.IsDead && t.IsAlly); }
        }

        public static void Load()
        {
            _minions = new List<Obj_AI_Minion>();
            // ReSharper disable once AccessToStaticMemberViaDerivedType
            Obj_AI_Minion.OnCreate += OnCreate;
            // ReSharper disable once AccessToStaticMemberViaDerivedType
            Obj_AI_Minion.OnDelete += OnDelete;
        }

        private static void OnDelete(GameObject sender, EventArgs args)
        {
           
            var iList = new List<Obj_AI_Minion>();
            foreach (var minion in _minions)
            {
                if (minion.NetworkId == sender.NetworkId) iList.Add(minion);
            }
            foreach (var i in iList)
            {
                _minions.Remove(i);
            }
        }

        private static void OnCreate(GameObject sender, EventArgs args)
        {
            var name = sender.Name.ToLower();
            if (sender.IsValid && (name.Contains("sru_") || name.Contains("worm")) && sender.Team != GameObjectTeam.Neutral) _minions.Add((Obj_AI_Minion)sender);
        }

    }

    public static class Cache
    {
        public static void Load()
        {
            Turrets.Load();
            Heroes.Load();
            Minions.Load();
            HealingBuffs.Load();
            JungleCamps.Load();
        }
    }
}
