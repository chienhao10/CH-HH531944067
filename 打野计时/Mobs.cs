using EloBuddy;
using SharpDX;
using System.Collections.Generic;

namespace JungleTimers
{
    public class Mobs
    {
        public bool Dead { get; set; }
        public GameMapId MapID { get; set; }
        public Vector2 MinimapPosition { get { return Drawing.WorldToMinimap(this.Position); } }
        public string[] MobNames { get; set; }
        public int NextRespawnTime { get; set; }
        public List<string> ObjectsAlive { get; set; }
        public List<string> ObjectsDead { get; set; }
        public Vector3 Position { get; set; }
        public int RespawnTime { get; set; }
        public GameObjectTeam Team { get; set; }
        public Mobs(
                int respawnTime,
                Vector3 position,
                string[] mobNames,
                GameMapId MapID,
                GameObjectTeam team)
        {
            this.RespawnTime = respawnTime;
            this.Position = position;
            this.MobNames = mobNames;
            this.MapID = MapID;
            this.Team = team;

            this.ObjectsDead = new List<string>();
            this.ObjectsAlive = new List<string>();
        }
    }
}