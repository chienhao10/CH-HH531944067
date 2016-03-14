using System;
using EloBuddy;
using EloBuddy.SDK;
using SharpDX;
using Color = System.Drawing.Color;

namespace EvadePlus.SkillshotTypes.SpecialTypes
{
    public class SummonerMark : EvadeSkillshot
    {
        public SummonerMark()
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

        private Vector3 _castStartPos;
        private Vector3 _castEndPos;
        private Vector3 _endPos;

        public Vector3 StartPosition
        {
            get
            {
                if (SpawnObject == null)
                {
                    return _castStartPos;
                }
                else
                {
                    return SpawnObject.Position;
                }
            }
        }

        public Vector3 EndPosition
        {
            get
            {
                if (SpawnObject == null)
                {
                    return _castEndPos;
                }
                else
                {
                    return _endPos;
                }
            }
        }

        public override Vector3 GetPosition()
        {
            return StartPosition;
        }

        public override EvadeSkillshot NewInstance()
        {
            var newInstance = new SummonerMark() {SpellData = SpellData};
            return newInstance;
        }

        public override void OnCreate(GameObject obj)
        {
            if (obj == null)
            {
                _castStartPos = Caster.Position;
                _castEndPos = _castStartPos.ExtendVector3(CastArgs.End, SpellData.Range);
            }
            else
            {
            }
        }

        public override void OnCreateObject(GameObject obj)
        {
            var minion = obj as Obj_AI_Minion;

            if (SpawnObject == null && minion != null)
            {
                if (minion.BaseSkinName == SpellData.MissileSpellName)
                {
                    // Force skillshot to be removed
                    IsValid = false;
                }
            }

            if (SpawnObject != null)
            {
                if (Utils.GetGameObjectName(obj) == SpellData.ToggleParticleName &&
                    obj.Distance(SpawnObject, true) <= 300.Pow())
                {
                    IsValid = false;
                }
            }
        }

        public override void OnTick()
        {
            if (SpawnObject == null)
            {
                if (Environment.TickCount > TimeDetected + SpellData.Delay + 50)
                    IsValid = false;
            }
            else
            {
                //_endPos = (SpawnObject as Obj_AI_Minion).Path.LastOrDefault();

                if (Environment.TickCount > TimeDetected + 9000)
                    IsValid = false;
            }
        }

        public override void OnDraw()
        {
            if (!IsValid)
            {
                return;
            }

            Utils.Draw3DRect(StartPosition, EndPosition, SpellData.Radius*2, Color.White);
        }

        public override Geometry.Polygon ToPolygon(float extrawidth = 0)
        {
            if (SpellData.AddHitbox)
                extrawidth += Player.Instance.BoundingRadius;

            return new Geometry.Polygon.Rectangle(StartPosition, EndPosition.ExtendVector3(StartPosition, -extrawidth),
                SpellData.Radius*2 + extrawidth);
        }

        public override int GetAvailableTime(Vector2 pos)
        {
            var dist1 =
                Math.Abs((EndPosition.Y - StartPosition.Y) * pos.X - (EndPosition.X - StartPosition.X) * pos.Y +
                         EndPosition.X * StartPosition.Y - EndPosition.Y * StartPosition.X) / (StartPosition.Distance(EndPosition));

            var actualDist = Math.Sqrt(StartPosition.Distance(pos).Pow() - dist1.Pow());

            var time = SpellData.MissileSpeed > 0 ? (int)((actualDist / SpellData.MissileSpeed) * 1000) : 0;

            if (SpawnObject == null)
            {
                time += Math.Max(0, SpellData.Delay - (Environment.TickCount - TimeDetected));
            }

            return time;
        }

        public override bool IsFromFow()
        {
            return SpawnObject != null && !SpawnObject.IsVisible;
        }
    }
}