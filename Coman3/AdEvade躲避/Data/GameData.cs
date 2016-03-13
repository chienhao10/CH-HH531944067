using System;
using System.Collections.Generic;
using AdEvade.Config;
using AdEvade.Utils;
using EloBuddy;
using EloBuddy.SDK;
using SharpDX;

namespace AdEvade.Data
{
    public class HeroInfo
    {
        public AIHeroClient Hero;
        public Vector2 ServerPos2D;
        public Vector2 ServerPos2DExtra;
        public Vector2 ServerPos2DPing;
        public Vector2 CurrentPosition;
        public bool IsMoving;
        public float BoundingRadius;
        public float MoveSpeed;

        public HeroInfo(AIHeroClient hero)
        {
            Hero = hero;
            Game.OnUpdate += Game_OnGameUpdate;
        }

        private void Game_OnGameUpdate(EventArgs args)
        {
            UpdateInfo();
        }

        public void UpdateInfo()
        {
            var extraDelayBuffer = ConfigValue.ExtraPingBuffer.GetInt();

            ServerPos2D = Hero.ServerPosition.To2D();
            ServerPos2DExtra = EvadeUtils.GetGamePosition(Hero, Game.Ping + extraDelayBuffer);
            ServerPos2DPing = EvadeUtils.GetGamePosition(Hero, Game.Ping); 
            CurrentPosition = Drawing.WorldToScreen(Hero.Position);
            BoundingRadius = Hero.BoundingRadius;
            MoveSpeed = Hero.MoveSpeed;
            IsMoving = Hero.IsMoving;
        }
    }
    public static class GameData
    {
        public static Dictionary<int, Obj_AI_Turret> Turrets = new Dictionary<int, Obj_AI_Turret>();
        public static AIHeroClient MyHero { get { return ObjectManager.Player; } }
        public static HeroInfo HeroInfo = new HeroInfo(MyHero);

        static GameData()
        {
            InitializeCache();
        }

        private static void InitializeCache()
        {
            foreach (var obj in ObjectManager.Get<Obj_AI_Turret>())
            {
                if (!Turrets.ContainsKey(obj.NetworkId))
                {
                    Turrets.Add(obj.NetworkId, obj);
                }
            }
        }

    }
}