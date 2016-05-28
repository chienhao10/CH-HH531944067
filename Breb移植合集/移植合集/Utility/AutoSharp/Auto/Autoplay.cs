using AutoSharp.Auto.HowlingAbyss;
using AutoSharp.Auto.SummonersRift;
using EloBuddy;
using LeagueSharp;

namespace AutoSharp.Auto
{
    public static class Autoplay
    {
        public static void Load()
        {
            switch (Game.MapId)
            {
                case GameMapId.SummonersRift:
                {
                    Game.OnUpdate += args => { MyTeam.Update(); };
                    SRManager.Load();
                    break;
                }
                case GameMapId.CrystalScar:
                {
                    break;
                }
                case GameMapId.TwistedTreeline:
                {
                    break;
                }
                case GameMapId.HowlingAbyss:
                {
                    HAManager.Load();
                    break;
                }
                default:
                {
                    HAManager.Load();
                    break;
                }
            }
        }

        public static void Unload()
        {
            switch (Game.MapId)
            {
                case GameMapId.SummonersRift:
                {
                    SRManager.Unload();
                    break;
                }
                case GameMapId.HowlingAbyss:
                {
                    HAManager.Unload();
                    break;
                }
                default:
                {
                    HAManager.Unload();
                    break;
                }
            }
        }
    }
}
