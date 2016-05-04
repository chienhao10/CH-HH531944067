using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using Color = System.Drawing.Color;
using LeagueSharp.Common.Data;
using ItemData = LeagueSharp.Common.Data.ItemData;
using FioraProject.Evade;
using EloBuddy;

namespace FioraProject
{
    public static class FioraPassive
    {
        #region FioraPassive
        public static List<Vector2> GetRadiusPoints(Vector2 targetpredictedpos, Vector2 passivepredictedposition)
        {
            List<Vector2> RadiusPoints = new List<Vector2>();
            for (int i = 50; i <= 300; i = i + 25)
            {
                var x = targetpredictedpos.LSExtend(passivepredictedposition, i);
                for (int j = -45; j <= 45; j = j + 5)
                {
                    RadiusPoints.Add(x.RotateAround(targetpredictedpos, j * (float)(Math.PI / 180)));
                }
            }
            return RadiusPoints;
        }
        public static PassiveStatus GetPassiveStatus(this AIHeroClient target, float delay = 0.25f)
        {
            var allobjects = GetPassiveObjects()
                .Where(x => x.Object != null && x.Object.IsValid
                           && x.Object.Position.LSTo2D().LSDistance(target.Position.LSTo2D()) <= 50);
            var targetpredictedpos = Prediction.GetPrediction(target, delay).UnitPosition.LSTo2D();
            if (!allobjects.Any())
            {
                return new PassiveStatus(false, PassiveType.None, new Vector2(), new List<PassiveDirection>(), new List<Vector2>());
            }
            else
            {
                var x = allobjects.First();
                var listdirections = new List<PassiveDirection>();
                foreach (var a in allobjects)
                {
                    listdirections.Add(a.PassiveDirection);
                }
                var listpositions = new List<Vector2>();
                foreach (var a in listdirections)
                {
                    if (a == PassiveDirection.NE)
                    {
                        var pos = targetpredictedpos;
                        pos.Y = pos.Y + 200;
                        listpositions.Add(pos);
                    }
                    else if (a == PassiveDirection.NW)
                    {
                        var pos = targetpredictedpos;
                        pos.X = pos.X + 200;
                        listpositions.Add(pos);
                    }
                    else if (a == PassiveDirection.SE)
                    {
                        var pos = targetpredictedpos;
                        pos.X = pos.X - 200;
                        listpositions.Add(pos);
                    }
                    else if (a == PassiveDirection.SW)
                    {
                        var pos = targetpredictedpos;
                        pos.Y = pos.Y - 200;
                        listpositions.Add(pos);
                    }
                }
                if (x.PassiveType == PassiveType.PrePassive)
                {
                    return new PassiveStatus(true, PassiveType.PrePassive, targetpredictedpos, listdirections, listpositions);
                }
                if (x.PassiveType == PassiveType.NormalPassive)
                {
                    return new PassiveStatus(true, PassiveType.NormalPassive, targetpredictedpos, listdirections, listpositions);
                }
                if (x.PassiveType == PassiveType.UltiPassive)
                {
                    return new PassiveStatus(true, PassiveType.UltiPassive, targetpredictedpos, listdirections, listpositions);
                }
                return new PassiveStatus(false, PassiveType.None, new Vector2(), new List<PassiveDirection>(), new List<Vector2>());
            }
        }
        public static List<PassiveObject> GetPassiveObjects()
        {
            List<PassiveObject> PassiveObjects = new List<PassiveObject>();
            foreach (var x in FioraPrePassiveObjects.Where(i => i != null && i.IsValid))
            {
                PassiveObjects.Add(new PassiveObject(x.Name, x, PassiveType.PrePassive, GetPassiveDirection(x)));
            }
            foreach (var x in FioraPassiveObjects.Where(i => i != null && i.IsValid))
            {
                PassiveObjects.Add(new PassiveObject(x.Name, x, PassiveType.NormalPassive, GetPassiveDirection(x)));
            }
            foreach (var x in FioraUltiPassiveObjects.Where(i => i != null && i.IsValid))
            {
                PassiveObjects.Add(new PassiveObject(x.Name, x, PassiveType.UltiPassive, GetPassiveDirection(x)));
            }
            return PassiveObjects;
        }
        public static PassiveDirection GetPassiveDirection(Obj_GeneralParticleEmitter x)
        {
            if (x.Name.Contains("NE"))
            {
                return PassiveDirection.NE;
            }
            else if (x.Name.Contains("SE"))
            {
                return PassiveDirection.SE;
            }
            else if (x.Name.Contains("NW"))
            {
                return PassiveDirection.NW;
            }
            else
            {
                return PassiveDirection.SW;
            }
        }
        public class PassiveStatus
        {
            public bool HasPassive;
            public PassiveType PassiveType;
            public Vector2 TargetPredictedPosition;
            public List<PassiveDirection> PassiveDirections = new List<PassiveDirection>();
            public List<Vector2> PassivePredictedPositions = new List<Vector2>();
            public PassiveStatus(bool hasPassive, PassiveType passiveType, Vector2 targetPredictedPosition
                , List<PassiveDirection> passiveDirections, List<Vector2> passivePredictedPositions)
            {
                HasPassive = hasPassive;
                PassiveType = passiveType;
                TargetPredictedPosition = targetPredictedPosition;
                PassiveDirections = passiveDirections;
                PassivePredictedPositions = passivePredictedPositions;
            }
        }
        public enum PassiveType
        {
            None, PrePassive, NormalPassive, UltiPassive
        }
        public enum PassiveDirection
        {
            NE, SE, NW, SW
        }
        public class PassiveObject
        {
            public string PassiveName;
            public Obj_GeneralParticleEmitter Object;
            public PassiveType PassiveType;
            public PassiveDirection PassiveDirection;
            public PassiveObject(string passiveName, Obj_GeneralParticleEmitter obj, PassiveType passiveType, PassiveDirection passiveDirection)
            {
                PassiveName = passiveName;
                Object = obj;
                PassiveType = passiveType;
                PassiveDirection = passiveDirection;
            }
        }
        public static List<Obj_GeneralParticleEmitter> FioraUltiPassiveObjects = new List<Obj_GeneralParticleEmitter>();
        //{
        //    get
        //    {
        //        var x = ObjectManager.Get<Obj_GeneralParticleEmitter>()
        //        .Where(a => a.Name.Contains("Fiora_Base_R_Mark") || (a.Name.Contains("Fiora_Base_R") && a.Name.Contains("Timeout_FioraOnly.troy")))
        //        .ToList();
        //        return x;
        //    }
        //}
        public static List<Obj_GeneralParticleEmitter> FioraPassiveObjects = new List<Obj_GeneralParticleEmitter>();
        //{
        //    get
        //    {
        //        var x = ObjectManager.Get<Obj_GeneralParticleEmitter>().Where(a => FioraPassiveName.Contains(a.Name)).ToList();
        //        return x;
        //    }
        //}
        public static List<Obj_GeneralParticleEmitter> FioraPrePassiveObjects = new List<Obj_GeneralParticleEmitter>();
        //{
        //    get
        //    {
        //        var x = ObjectManager.Get<Obj_GeneralParticleEmitter>().Where(a => FioraPrePassiveName.Contains(a.Name)).ToList();
        //        return x;
        //    }
        //}
        public static List<string> FioraPassiveName = new List<string>()
        {
            "Fiora_Base_Passive_NE.troy",
            "Fiora_Base_Passive_SE.troy",
            "Fiora_Base_Passive_NW.troy",
            "Fiora_Base_Passive_SW.troy",
            "Fiora_Base_Passive_NE_Timeout.troy",
            "Fiora_Base_Passive_SE_Timeout.troy",
            "Fiora_Base_Passive_NW_Timeout.troy",
            "Fiora_Base_Passive_SW_Timeout.troy"
        };
        public static List<string> FioraPrePassiveName = new List<string>()
        {
            "Fiora_Base_Passive_NE_Warning.troy",
            "Fiora_Base_Passive_SE_Warning.troy",
            "Fiora_Base_Passive_NW_Warning.troy",
            "Fiora_Base_Passive_SW_Warning.troy"
        };
        public static void FioraPassiveUpdate()
        {
            FioraPrePassiveObjects = new List<Obj_GeneralParticleEmitter>();
            FioraPassiveObjects = new List<Obj_GeneralParticleEmitter>();
            FioraUltiPassiveObjects = new List<Obj_GeneralParticleEmitter>();
            var ObjectEmitter = ObjectManager.Get<Obj_GeneralParticleEmitter>()
                                             .Where(a => FioraPassiveName.Contains(a.Name) || FioraPassiveName.Contains(a.Name)
                                             || a.Name.Contains("Fiora_Base_R_Mark")
                                             || (a.Name.Contains("Fiora_Base_R") && a.Name.Contains("Timeout_FioraOnly.troy")))
                                             .ToList();
            FioraPrePassiveObjects.AddRange(ObjectEmitter.Where(a => FioraPrePassiveName.Contains(a.Name)));
            FioraPassiveObjects.AddRange(ObjectEmitter.Where(a => FioraPassiveName.Contains(a.Name)));
            FioraUltiPassiveObjects.AddRange(ObjectEmitter
                .Where(a =>
                       a.Name.Contains("Fiora_Base_R_Mark")
                       || (a.Name.Contains("Fiora_Base_R") && a.Name.Contains("Timeout_FioraOnly.troy"))));
        }
        #endregion FioraPassive
    }
}
