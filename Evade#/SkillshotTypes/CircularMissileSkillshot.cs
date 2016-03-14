using System;
using System.Text.RegularExpressions;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Rendering;
using SharpDX;
using Color = System.Drawing.Color;

namespace EvadePlus.SkillshotTypes
{
    public class CircularMissileSkillshot : EvadeSkillshot
    {
        public CircularMissileSkillshot()
        {
            Caster = null;
            CastArgs = null;
            SpawnObject = null;
            SData = null;
            SpellData = null;
            Team = GameObjectTeam.Unknown;
            IsValid = true;
            TimeDetected = Environment.TickCount;
        }

        public Vector3 Position { get; private set; }

        public MissileClient Missile
        {
            get { return SpawnObject as MissileClient; }
        }

        private bool MissileDeleted;
        private Vector3 StartMissilePos;
        private Circle PrimaryCircle;
        private Circle SecondaryCircle;

        public override Vector3 GetPosition()
        {
            return Position;
        }

        public override EvadeSkillshot NewInstance()
        {
            var newInstance = new CircularMissileSkillshot() {SpellData = SpellData};
            return newInstance;
        }

        public override void OnCreate(GameObject obj)
        {
            if (Missile == null)
            {
                Position = CastArgs.End;
            }
            else
            {
                Position = Missile.EndPosition;
                StartMissilePos = Missile.Position;
            }

            PrimaryCircle = new Circle
            {
                Radius = SpellData.Radius,
                BorderWidth = 3,
                Color = Color.White
            };

            SecondaryCircle = new Circle
            {
                Radius = 10,
                Color = Color.LawnGreen
            };
        }

        public override void OnCreateObject(GameObject obj)
        {
            var missile = obj as MissileClient;

            if (SpawnObject == null && missile != null)
            {
                if (missile.SData.Name == SpellData.MissileSpellName && missile.SpellCaster.Index == Caster.Index)
                {
                    // Force skillshot to be removed
                    IsValid = false;
                }
            }
        }

        public override bool OnDelete(GameObject obj)
        {
            if (Missile != null && obj.Index == Missile.Index && !string.IsNullOrEmpty(SpellData.ToggleParticleName))
            {
                MissileDeleted = true;
                return false;
            }

            return true;
        }

        public override void OnDeleteObject(GameObject obj)
        {
            if (Missile != null && MissileDeleted && !string.IsNullOrEmpty(SpellData.ToggleParticleName))
            {
                var r = new Regex(SpellData.ToggleParticleName);
                if (r.Match(obj.Name).Success && obj.Distance(Position, true) <= 100*100)
                {
                    IsValid = false;
                }
            }
        }

        public override void OnTick()
        {
            if (Missile == null)
            {
                if (Environment.TickCount > TimeDetected + SpellData.Delay + 250)
                    IsValid = false;
            }
            else
            {
                if (Environment.TickCount > TimeDetected + 6000)
                    IsValid = false;
            }
        }

        public override void OnDraw()
        {
            if (!IsValid)
            {
                return;
            }

            if (Missile != null && !MissileDeleted)
            {
                SecondaryCircle.Radius = StartMissilePos.To2D().Distance(Missile.Position.To2D())/
                                         (StartMissilePos.To2D().Distance(Position.To2D()))*SpellData.Radius;
                SecondaryCircle.Draw(Position);
            }

            PrimaryCircle.Draw(Position);
        }

        public override Geometry.Polygon ToPolygon(float extrawidth = 0)
        {
            if (SpellData.AddHitbox)
                extrawidth += Player.Instance.HitBoxRadius();

            return new Geometry.Polygon.Circle(Position, SpellData.Radius + extrawidth);
        }

        public override int GetAvailableTime(Vector2 pos)
        {
            if (Missile == null)
            {
                return
                    (int)
                        (SpellData.Delay - (Environment.TickCount - TimeDetected) +
                         (Caster.Position.To2D().Distance(Position.To2D())) / SpellData.MissileSpeed * 1000);
            }

            if (!MissileDeleted)
            {
                return (int) ((Missile.Position.To2D().Distance(Position.To2D()))/SpellData.MissileSpeed*1000);
            }

            return -1;
        }

        public override bool IsFromFow()
        {
            return Missile != null && !Missile.SpellCaster.IsVisible;
        }
    }
}