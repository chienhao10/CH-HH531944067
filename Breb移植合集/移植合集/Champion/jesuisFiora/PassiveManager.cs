using System;
using System.Collections.Generic;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using TreeLib.Extensions;
using Color = System.Drawing.Color;
using EloBuddy;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;

namespace jesuisFiora
{
    internal static class PassiveManager
    {
        private static readonly List<FioraPassive> PassiveList = new List<FioraPassive>();
        private static readonly List<string> DirectionList = new List<string> { "NE", "NW", "SE", "SW" };

        private static int _fioraCount;

        private static IEnumerable<Obj_GeneralParticleEmitter> VitalList
        {
            get { return ObjectManager.Get<Obj_GeneralParticleEmitter>().Where(IsFioraPassive); }
        }

        public static Menu Menu = Program.passiveM;

        public static void Initialize()
        {
            _fioraCount = HeroManager.AllHeroes.Count(h => h.ChampionName == "Fiora");

            Game.OnUpdate += Game_OnUpdate;
            Drawing.OnDraw += Drawing_OnDraw;
        }

        private static void Game_OnUpdate(EventArgs args)
        {
            if (ObjectManager.Player.IsDead)
            {
                return;
            }

            UpdatePassiveList();
        }

        public static bool getCheckBoxItem(Menu m, string item)
        {
            return m[item].Cast<CheckBox>().CurrentValue;
        }

        public static int getSliderItem(Menu m, string item)
        {
            return m[item].Cast<Slider>().CurrentValue;
        }

        public static bool getKeyBindItem(Menu m, string item)
        {
            return m[item].Cast<KeyBind>().CurrentValue;
        }

        public static int getBoxItem(Menu m, string item)
        {
            return m[item].Cast<ComboBox>().CurrentValue;
        }

        private static void Drawing_OnDraw(EventArgs args)
        {
            if (!getCheckBoxItem(Menu, "DrawPolygon") && !getCheckBoxItem(Menu, "DrawCenter"))
            {
                return;
            }

            try
            {
                foreach (var passive in PassiveList.Where(p => p.IsValid && p.IsVisible && p.Target.IsValid && p.Target.IsVisible && p.Target.IsHPBarRendered))
                {
                    if (getCheckBoxItem(Menu, "DrawPolygon") && passive.SimplePolygon != null)
                    {
                        passive.SimplePolygon.Draw(passive.Color);
                    }

                    if (getCheckBoxItem(Menu, "DrawCenter"))
                    {
                        Render.Circle.DrawCircle(passive.OrbwalkPosition, 50, passive.Color);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public static int CountPassive(this AIHeroClient target)
        {
            return PassiveList.Count(p => p.Target.NetworkId == target.NetworkId);
        }

        public static FioraPassive GetNearestPassive(this AIHeroClient target)
        {
            var list = PassiveList.Where(p => p.Target.NetworkId == target.NetworkId);
            var fioraPassives = list as FioraPassive[] ?? list.ToArray();
            return !fioraPassives.Any()
                ? null
                : fioraPassives.Where(p => p.IsValid && p.IsVisible)
                    .MinOrDefault(obj => obj.OrbwalkPosition.DistanceToPlayer());
        }

        public static FioraPassive GetFurthestPassive(this AIHeroClient target)
        {
            var list = PassiveList.Where(p => p.Target.NetworkId == target.NetworkId);
            var fioraPassives = list as FioraPassive[] ?? list.ToArray();
            return !fioraPassives.Any()
                ? null
                : fioraPassives.Where(p => p.IsValid && p.IsVisible)
                    .MaxOrDefault(obj => obj.OrbwalkPosition.DistanceToPlayer());
        }

        public static bool HasUltPassive(this AIHeroClient target)
        {
            return target.GetUltPassiveCount() > 0;
        }

        public static int GetUltPassiveCount(this AIHeroClient target)
        {
            var passive = PassiveList.Where(p => p.Target.NetworkId == target.NetworkId);
            var fioraPassives = passive as FioraPassive[] ?? passive.ToArray();

            if (!fioraPassives.Any())
            {
                return 0;
            }

            return fioraPassives.Count(
                p => p.IsValid && p.IsVisible && p.Passive == FioraPassive.PassiveType.UltPassive);
        }

        public static double GetPassiveDamage(this AIHeroClient target, int? passiveCount = null)
        {
            var modifier = (.03f +
                            Math.Min(
                                Math.Max(
                                    .028f,
                                    .027 +
                                    .001f * ObjectManager.Player.Level * ObjectManager.Player.FlatPhysicalDamageMod /
                                    100f), .45f)) * target.MaxHealth;
            return passiveCount * modifier ?? target.CountPassive() * modifier;
        }

        public static void UpdatePassiveList()
        {
            PassiveList.Clear();
            foreach (var vital in VitalList)
            {
                var vital1 = vital;
                var hero = HeroManager.Enemies.Where(h => h.LSIsValidTarget()).MinOrDefault(h => h.LSDistance(vital1.Position));
                if (hero != null)
                {
                    PassiveList.Add(new FioraPassive(vital, hero));
                }
            }
        }

        public static bool IsFioraPassive(this Obj_GeneralParticleEmitter emitter)
        {
            return emitter != null && emitter.IsValid &&
                   (emitter.Name.Contains("Fiora_Base_R_Mark") ||
                    (emitter.Name.Contains("Fiora_Base_R") && emitter.Name.Contains("Timeout")) ||
                    (emitter.Name.Contains("Fiora_Base_Passive") && DirectionList.Any(emitter.Name.Contains)));
        }
    }

    public class FioraPassive : Obj_GeneralParticleEmitter
    {
        public enum PassiveType
        {
            Prepassive,
            Passive,
            PassiveTimeout,
            UltPassive,
            None
        }

        public static Menu Menu = Program.passiveM;
        private static float LastPolygonRadius;
        private static float LastPolygonAngle;
        public readonly Color Color;
        public readonly PassiveType Passive;
        private readonly int PassiveDistance;
        public readonly AIHeroClient Target;
        private Geometry.Polygon _polygon;
        private Vector3 _polygonCenter;
        private Geometry.Polygon.Sector _simplePolygon;
        private Vector3 LastPolygonPosition;
        private Vector3 LastSimplePolygonPosition;
        
        public FioraPassive() {}

        public FioraPassive(Obj_GeneralParticleEmitter emitter, AIHeroClient enemy) : base(emitter.Index, (uint)emitter.NetworkId, emitter as GameObject)
        {
            Target = enemy;

            if (emitter.Name.Contains("Base_R"))
            {
                Passive = PassiveType.UltPassive;
                Color = Color.White;
            }
            else if (emitter.Name.Contains("Warning"))
            {
                Passive = PassiveType.Prepassive;
                Color = Color.Blue;
            }
            else if (emitter.Name.Contains("Timeout"))
            {
                Passive = PassiveType.PassiveTimeout;
                Color = Color.Red;
            }
            else
            {
                Passive = PassiveType.Passive;
                Color = Color.Green;
            }
            PassiveDistance = Passive == PassiveType.UltPassive ? 400 : 200;
        }

        public static bool getCheckBoxItem(Menu m, string item)
        {
            return m[item].Cast<CheckBox>().CurrentValue;
        }

        public static int getSliderItem(Menu m, string item)
        {
            return m[item].Cast<Slider>().CurrentValue;
        }

        public static bool getKeyBindItem(Menu m, string item)
        {
            return m[item].Cast<KeyBind>().CurrentValue;
        }

        public static int getBoxItem(Menu m, string item)
        {
            return m[item].Cast<ComboBox>().CurrentValue;
        }

        private static float PolygonAngle
        {
            get { return getSliderItem(Menu, "SectorAngle"); }
        }

        private static float PolygonRadius
        {
            get { return getSliderItem(Menu, "SectorMaxRadius"); }
        }

        public Geometry.Polygon Polygon
        {
            get
            {
                if (LastPolygonRadius == 0)
                {
                    LastPolygonRadius = PolygonRadius;
                }

                if (LastPolygonAngle == 0)
                {
                    LastPolygonAngle = PolygonAngle;
                }

                if (LastPolygonPosition != Vector3.Zero && Target.ServerPosition == LastPolygonPosition &&
                    PolygonRadius == LastPolygonRadius && PolygonAngle == LastPolygonAngle && _polygon != null)
                {
                    return _polygon;
                }

                _polygon = GetFilledPolygon();
                LastPolygonPosition = Target.ServerPosition;
                LastPolygonAngle = PolygonAngle;
                LastPolygonRadius = PolygonRadius;
                _polygonCenter = _polygon.CenterOfPolygone().To3D();
                return _polygon;
            }
        }

        public Geometry.Polygon.Sector SimplePolygon
        {
            get
            {
                if (LastPolygonRadius == 0)
                {
                    LastPolygonRadius = PolygonRadius;
                }

                if (LastPolygonAngle == 0)
                {
                    LastPolygonAngle = PolygonAngle;
                }

                if (LastSimplePolygonPosition != Vector3.Zero && Target.ServerPosition == LastSimplePolygonPosition &&
                    PolygonRadius == LastPolygonRadius && PolygonAngle == LastPolygonAngle && _simplePolygon != null)
                {
                    return _simplePolygon;
                }

                _simplePolygon = GetSimplePolygon();
                LastSimplePolygonPosition = Target.ServerPosition;
                LastPolygonAngle = PolygonAngle;
                LastPolygonRadius = PolygonRadius;

                return _simplePolygon;
            }
        }

        public Vector3 OrbwalkPosition
        {
            get
            {
                return _polygonCenter == Vector3.Zero ? Vector3.Zero : Target.ServerPosition.LSExtend(_polygonCenter, 150);
            }
        }

        public Vector3 CastPosition
        {
            get
            {
                return
                    Polygon.Points.Where(
                        p => SpellManager.Q.IsInRange(p) && p.DistanceToPlayer() > 100 && p.LSDistance(Target) > 50)
                        .OrderBy(p => p.LSDistance(Target))
                        .ThenByDescending(p => p.DistanceToPlayer())
                        .FirstOrDefault()
                        .To3D();
            }
        }

        private Geometry.Polygon.Sector GetSimplePolygon(bool predictPosition = false)
        {
            var basePos = predictPosition ? SpellManager.Q.GetPrediction(Target).UnitPosition : Target.ServerPosition;
            var pos = basePos + GetPassiveOffset();
            var r = Passive == PassiveType.UltPassive ? 400 : PolygonRadius;
            var sector = new Geometry.Polygon.Sector(basePos, pos, Geometry.DegreeToRadian(PolygonAngle), r);
            sector.UpdatePolygon();
            return sector;
        }

        private Geometry.Polygon GetFilledPolygon(bool predictPosition = false)
        {
            var basePos = predictPosition ? SpellManager.Q.GetPrediction(Target).UnitPosition : Target.ServerPosition;
            var pos = basePos + GetPassiveOffset();
            //var polygons = new List<Geometry.Polygon>();
            var list = new List<Vector2>();
            var r = Passive == PassiveType.UltPassive ? 400 : PolygonRadius;
            var angle = Geometry.DegreeToRadian(PolygonAngle);

            for (var i = 100; i < r; i += 10)
            {
                if (i > r)
                {
                    break;
                }

                var sector = new Geometry.Polygon.Sector(basePos, pos, angle, i);
                sector.UpdatePolygon();
                list.AddRange(sector.Points);
                //polygons.Add(sector);
            }

            return new Geometry.Polygon { Points = list.Distinct().ToList() };
            //return polygons.JoinPolygons().FirstOrDefault();
        }

        public Vector3 GetPassiveOffset(bool orbwalk = false)
        {
            var d = PassiveDistance;
            var offset = Vector3.Zero;

            if (orbwalk)
            {
                d -= 50;
                //d -= Passive.Equals(PassiveType.UltPassive) ? 200 : 50;
            }

            if (Name.Contains("NE"))
            {
                offset = new Vector3(0, d, 0);
            }

            if (Name.Contains("SE"))
            {
                offset = new Vector3(-d, 0, 0);
            }

            if (Name.Contains("NW"))
            {
                offset = new Vector3(d, 0, 0);
            }

            if (Name.Contains("SW"))
            {
                offset = new Vector3(0, -d, 0);
            }

            return offset;
        }
    }

    public class QPosition
    {
        public FioraPassive.PassiveType PassiveType;
        public Geometry.Polygon Polygon;
        public Vector3 Position;
        public Geometry.Polygon SimplePolygon;

        public QPosition(Vector3 position,
            FioraPassive.PassiveType passiveType = FioraPassive.PassiveType.None,
            Geometry.Polygon polygon = null,
            Geometry.Polygon simplePolygon = null)
        {
            Position = position;
            PassiveType = passiveType;
            Polygon = polygon;
            SimplePolygon = simplePolygon;
        }
    }
}