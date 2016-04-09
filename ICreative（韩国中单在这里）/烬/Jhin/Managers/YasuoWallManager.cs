using System;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using SharpDX;

namespace Jhin.Managers
{
    public static class YasuoWallManager
    {
        private static Vector3 _startPosition;
        private static GameObject _wallObject;
        private static bool _containsYasuo;

        public static void Initialize()
        {
            if (EntityManager.Heroes.Enemies.Any(h => h.Hero == Champion.Yasuo))
            {
                _containsYasuo = true;
                GameObject.OnCreate += delegate(GameObject sender, EventArgs args)
                {
                    if (sender.Name.Contains("Yasuo_Base_W_windwall") && !sender.Name.Contains("_activate.troy"))
                    {
                        _wallObject = sender;
                    }
                };
                GameObject.OnDelete += delegate(GameObject sender, EventArgs args)
                {
                    if (sender.Name.Contains("Yasuo_Base_W_windwall") && !sender.Name.Contains("_activate.troy"))
                    {
                        _wallObject = null;
                    }
                };
                Obj_AI_Base.OnProcessSpellCast +=
                    delegate(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
                    {
                        var hero = sender as AIHeroClient;
                        if (hero != null && hero.IsEnemy && hero.Hero == Champion.Yasuo && args.Slot == SpellSlot.W)
                        {
                            _startPosition = hero.ServerPosition;
                        }
                    };
            }
        }

        public static bool WillHitYasuoWall(Vector3 startPosition, Vector3 endPosition)
        {
            if (_containsYasuo)
            {
                if (_wallObject != null && _wallObject.IsValid && !_wallObject.IsDead)
                {
                    var level = Convert.ToInt32(_wallObject.Name.Substring(_wallObject.Name.Length - 6, 1));
                    var width = 250f + level*50f;
                    var pos1 = _wallObject.Position.To2D() +
                               (_wallObject.Position.To2D() - _startPosition.To2D()).Normalized().Perpendicular()*width/
                               2f;
                    var pos2 = _wallObject.Position.To2D() +
                               (_wallObject.Position.To2D() - _startPosition.To2D()).Normalized().Perpendicular2()*width/
                               2f;
                    var intersection = pos1.Intersection(pos2, startPosition.To2D(), endPosition.To2D());
                    return intersection.Point.IsValid();
                }
            }
            return false;
        }
    }
}