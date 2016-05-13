using System;
using EloBuddy;
using EloBuddy.SDK;
using SharpDX;
using Color = System.Drawing.Color;

namespace EvadePlus.SkillshotTypes
{
    public class LinearMissileSkillshot : EvadeSkillshot
    {
        public LinearMissileSkillshot()
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

        internal Vector3 _startPos;
        internal Vector3 _endPos;

        public MissileClient Missile
        {
            get { return SpawnObject as MissileClient; }
        }

        public Vector3 StartPosition
        {
            get
            {
                if (Missile == null)
                {
                    return _startPos;
                }
                return Missile.Position;
            }
        }

        public Vector3 EndPosition
        {
            get
            {
                if (Missile == null)
                {
                    return _endPos;
                }
                return Missile.StartPosition.ExtendVector3(Missile.EndPosition, SpellData.Range);
            }
        }

        public override Vector3 GetPosition()
        {
            return StartPosition;
        }

        public override EvadeSkillshot NewInstance()
        {
            var newInstance = new LinearMissileSkillshot { SpellData = SpellData };
            return newInstance;
        }

        public override void OnCreateObject(GameObject obj)
        {
            var missile = obj as MissileClient;

            if (SpawnObject == null && missile != null)
            {
                if (missile.SData.Name == SpellData.MissileSpellName && missile.SpellCaster.Index == Caster.Index)
                {
                    IsValid = false;
                }
            }
        }

        public override void OnSpellDetection(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            _startPos = Caster.ServerPosition;
            _endPos = _startPos.ExtendVector3(CastArgs.End, SpellData.Range);
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

            Utils.Draw3DRect(StartPosition, EndPosition, SpellData.Radius * 2, Color.White);
        }

        public override Geometry.Polygon ToPolygon(float extrawidth = 0)
        {
            if (SpellData.AddHitbox)
            {
                extrawidth += Player.Instance.HitBoxRadius();
            }

            return new Geometry.Polygon.Rectangle(StartPosition, EndPosition.ExtendVector3(StartPosition, -extrawidth), SpellData.Radius * 2 + extrawidth);
        }

        public override int GetAvailableTime(Vector2 pos)
        {
            var dist1 =
                Math.Abs((EndPosition.Y - StartPosition.Y) * pos.X - (EndPosition.X - StartPosition.X) * pos.Y +
                         EndPosition.X * StartPosition.Y - EndPosition.Y * StartPosition.X) / (StartPosition.Distance(EndPosition));

            var actualDist = Math.Sqrt(StartPosition.Distance(pos).Pow() - dist1.Pow());

            var time = SpellData.MissileSpeed > 0 ? (int) ((actualDist / SpellData.MissileSpeed) * 1000) : 0;

            if (Missile == null)
            {
                time += Math.Max(0, SpellData.Delay - (Environment.TickCount - TimeDetected));
            }

            return time;
        }

        public override bool IsFromFow()
        {
            return Missile != null && !Missile.SpellCaster.IsVisible;
        }
    }
}