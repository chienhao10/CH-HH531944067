using Color = System.Drawing.Color;
using EloBuddy.SDK;
using EloBuddy;
using LeagueSharp.Common;
using SharpDX;
using System.Collections.Generic;
using System.Linq;
using System;
using TreeLib.Extensions;

namespace LuluLicious
{
    internal static class Pix
    {
        private const int EDuration = 6000;
        private static Obj_AI_Minion _instance;
        private static Render.Circle _circle;
        private static bool _drawPix;
        private static int _lastECast;

        public static void Initialize(bool drawPix)
        {
            _drawPix = drawPix;
            FindPix();
            _circle = new Render.Circle(Vector3.Zero, 50, Color.Purple);
            _circle.VisibleCondition += sender => _drawPix;
            _circle.Add();

            Obj_AI_Base.OnBuffLose += (sender, args) =>
            {
                if (args.Buff.SourceName == ObjectManager.Player.ChampionName &&
                    (args.Buff.Name == "luluevision" || args.Buff.Name == "lulufaerieshield"))
                {
                    _lastECast = 0;
                }
            };
            Game.OnUpdate += Game_OnUpdate;
            Obj_AI_Base.OnProcessSpellCast += Obj_AI_Base_OnProcessSpellCast;
        }

        private static void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.IsMe && args.Slot == SpellSlot.E)
            {
                _lastECast = Utils.TickCount;
                SpellManager.PixQ.UpdateSourcePosition(args.Target.Position, args.Target.Position);
            }
        }

        public static List<Obj_AI_Base> GetMinions()
        {
            return MinionManager.GetMinions(
                _instance.ServerPosition, SpellManager.PixQ.Range, MinionTypes.All, MinionTeam.NotAlly);
        }

        public static MinionManager.FarmLocation GetFarmLocation()
        {
            var minions = GetMinions();
            return SpellManager.PixQ.GetLineFarmLocation(minions);
        }

        public static AIHeroClient GetTarget(float range = 0)
        {
            var r = range == 0 ? SpellManager.Q.Range : range;
            return TargetSelector.GetTarget(r, DamageType.Magical);
        }

        public static Obj_AI_Base GetETarget(AIHeroClient target)
        {
            return
                ObjectManager.Get<Obj_AI_Base>()
                    .Where(
                        o =>
                            o.LSIsValidTarget(SpellManager.E.Range, false, _instance.ServerPosition) &&
                            o.LSDistance(target) < 600 && (o.IsAlly || !SpellManager.Q.IsKillable(o)))
                    .OrderBy(o => o.LSDistance(target))
                    .ThenBy(o => o.Team != ObjectManager.Player.Team)
                    .FirstOrDefault();
        }

        public static bool IsValid()
        {
            return _instance != null && _instance.IsValid && _instance.DistanceToPlayer() < 1900;
        }

        private static void Game_OnUpdate(EventArgs args)
        {
            if (ObjectManager.Player.IsDead)
            {
                return;
            }

            if (_instance == null)
            {
                FindPix();
                return;
            }

            _circle.Position = _instance.Position;
            SpellManager.PixQ.UpdateSourcePosition(_instance.ServerPosition, _instance.ServerPosition);
        }

        private static void FindPix()
        {
            _instance =
                ObjectManager.Get<Obj_AI_Minion>()
                    .Where(m => m.IsValid && m.IsAlly && m.Name == "RobotBuddy")
                    .MinOrDefault(m => m.DistanceToPlayer());
        }
    }
}