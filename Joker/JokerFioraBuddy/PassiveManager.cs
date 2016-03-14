using System;
using System.Collections.Generic;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK.Enumerations;
using SharpDX;
using EloBuddy.SDK;

namespace JokerFioraBuddy
{
    public static class PassiveManager
    {
        private static List<string> FioraPassiveName = new List<string>()
        {
            "Fiora_Base_Passive_NE.troy",
            "Fiora_Base_Passive_SE.troy",
            "Fiora_Base_Passive_NW.troy",
            "Fiora_Base_Passive_SW.troy",
            "Fiora_Base_Passive_NE_Timeout.troy",
            "Fiora_Base_Passive_SE_Timeout.troy",
            "Fiora_Base_Passive_NW_Timeout.troy",
            "Fiora_Base_Passive_SW_Timeout.troy",
        };

        public static Vector3 PassivePosition(Obj_AI_Base target)
        {
            var passive = FioraPassiveObjects.Where(x => x.Position.Distance(target.Position) <= 50).FirstOrDefault();
            var Position = Prediction.Position.PredictUnitPosition(target, 250);
            if (passive != null)
            {
                if (passive.Name.Contains("NE"))
                {
                    var pos = new Vector2();
                    pos.X = Position.X;
                    pos.Y = Position.Y + 150;
                    return pos.To3D();
                }
                if (passive.Name.Contains("SE"))
                {
                    var pos = new Vector2();
                    pos.X = Position.X - 150;
                    pos.Y = Position.Y;
                    return pos.To3D();
                }
                if (passive.Name.Contains("NW"))
                {
                    var pos = new Vector2();
                    pos.X = Position.X + 150;
                    pos.Y = Position.Y;
                    return pos.To3D();
                }
                if (passive.Name.Contains("SW"))
                {
                    var pos = new Vector2();
                    pos.X = Position.X;
                    pos.Y = Position.Y - 150;
                    return pos.To3D();
                }
                return new Vector3();
            }
            return new Vector3();

        }

        public static double AngleBetween(Vector2 a, Vector2 b, Vector2 c)
        {
            float a1 = c.Distance(b);
            float b1 = a.Distance(c);
            float c1 = b.Distance(a);
            if (a1 == 0 || c1 == 0) { return 0; }
            else
            {
                return Math.Acos((a1 * a1 + c1 * c1 - b1 * b1) / (2 * a1 * c1)) * (180 / Math.PI);
            }
        }

        public static bool InTheCone(this Vector3 pos, List<Vector3> poses, Vector2 targetpos)
        {
            bool x = true;
            foreach (var i in poses)
            {
                if (AngleBetween(pos.To2D(), targetpos, i.To2D()) > 90)
                    x = false;
            }
            return x;
        }

        private static List<Vector3> UltiPassivePos(Obj_AI_Base target)
        {
            List<Vector3> poses = new List<Vector3>();
            
            var passive = ObjectManager.Get<Obj_GeneralParticleEmitter>()
                .Where(x => x.Name.Contains("Fiora_Base_R_Mark") || (x.Name.Contains("Fiora_Base_R") && x.Name.Contains("Timeout_FioraOnly.troy")));

            var Position = Prediction.Position.PredictUnitPosition(target, 250);
           
            foreach (var x in passive)
            {
                if (x.Name.Contains("NE"))
                {
                    var pos = new Vector2();
                    pos.X = Position.X;
                    pos.Y = Position.Y + 150;
                    poses.Add(pos.To3D());
                }
                else if (x.Name.Contains("SE"))
                {
                    var pos = new Vector2();
                    pos.X = Position.X - 150;
                    pos.Y = Position.Y;
                    poses.Add(pos.To3D());
                }
                else if (x.Name.Contains("NW"))
                {
                    var pos = new Vector2();
                    pos.X = Position.X + 150;
                    pos.Y = Position.Y;
                    poses.Add(pos.To3D());
                }
                else if (x.Name.Contains("SW"))
                {
                    var pos = new Vector2();
                    pos.X = Position.X;
                    pos.Y = Position.Y - 150;
                    poses.Add(pos.To3D());
                }
            }

            return poses;
        }

        private static bool HasUltiPassive(Obj_AI_Base target)
        {
            return ObjectManager.Get<Obj_GeneralParticleEmitter>()
                .Where(x => x.Name.Contains("Fiora_Base_R_Mark") || (x.Name.Contains("Fiora_Base_R") && x.Name.Contains("Timeout_FioraOnly.troy"))).Any(x => x.Position.Distance(target.Position) <= 50);
        }

        public static bool HasPassive(Obj_AI_Base target)
        {
            return FioraPassiveObjects.Any(x => x.Position.Distance(target.Position) <= 50);
        }

        private static List<Obj_GeneralParticleEmitter> FioraPassiveObjects
        {
            get
            {
                var x = ObjectManager.Get<Obj_GeneralParticleEmitter>().Where(a => FioraPassiveName.Contains(a.Name)).ToList();
                return x;
            }
        }

        public static List<Vector3> PassiveRadiusPoint(Obj_AI_Base target)
        {
            var passive = FioraPassiveObjects.Where(x => x.Position.Distance(target.Position) <= 50).FirstOrDefault();
            var Position = Prediction.Position.PredictUnitPosition(target,250);
            if (passive != null)
            {
                if (passive.Name.Contains("NE"))
                {
                    var pos1 = new Vector2();
                    var pos2 = new Vector2();
                    pos1.X = Position.X + 150 / (float)Math.Sqrt(2);
                    pos2.X = Position.X - 150 / (float)Math.Sqrt(2);
                    pos1.Y = Position.Y + 150 / (float)Math.Sqrt(2);
                    pos2.Y = Position.Y + 150 / (float)Math.Sqrt(2);
                    return new List<Vector3>() { pos1.To3D(), pos2.To3D() };
                }
                if (passive.Name.Contains("SE"))
                {
                    var pos1 = new Vector2();
                    var pos2 = new Vector2();
                    pos1.X = Position.X - 150 / (float)Math.Sqrt(2);
                    pos2.X = Position.X - 150 / (float)Math.Sqrt(2);
                    pos1.Y = Position.Y - 150 / (float)Math.Sqrt(2);
                    pos2.Y = Position.Y + 150 / (float)Math.Sqrt(2);
                    return new List<Vector3>() { pos1.To3D(), pos2.To3D() };
                }
                if (passive.Name.Contains("NW"))
                {
                    var pos1 = new Vector2();
                    var pos2 = new Vector2();
                    pos1.X = Position.X + 150 / (float)Math.Sqrt(2);
                    pos2.X = Position.X + 150 / (float)Math.Sqrt(2);
                    pos1.Y = Position.Y - 150 / (float)Math.Sqrt(2);
                    pos2.Y = Position.Y + 150 / (float)Math.Sqrt(2);
                    return new List<Vector3>() { pos1.To3D(), pos2.To3D() };
                }
                if (passive.Name.Contains("SW"))
                {
                    var pos1 = new Vector2();
                    var pos2 = new Vector2();
                    pos1.X = Position.X + 150 / (float)Math.Sqrt(2);
                    pos2.X = Position.X - 150 / (float)Math.Sqrt(2);
                    pos1.Y = Position.Y - 150 / (float)Math.Sqrt(2);
                    pos2.Y = Position.Y - 150 / (float)Math.Sqrt(2);
                    return new List<Vector3>() { pos1.To3D(), pos2.To3D() };
                }
                return new List<Vector3>();
            }
            else
            {
                return new List<Vector3>();
            }

        }

        public static Vector3 castQPos(Obj_AI_Base target)
        {
            var targetpos = Prediction.Position.PredictUnitPosition(target, 250);
            if (HasPassive(target))
            {
                var poses = PassiveRadiusPoint(target);
                var pos = target.Position.To2D().Extend(PassivePosition(target).To2D(), 100);
                var possibleposes = new List<Vector2>();

                for (int i = 0; i <= 400; i = i + 100)
                {
                    var p = Player.Instance.Position.To2D().Extend(pos, i);
                    possibleposes.Add(p);
                }

                var castpos = possibleposes.Where(x => x.To3D().InTheCone(poses, targetpos) && x.Distance(targetpos) <= 300)
                                            .OrderByDescending(x => 1 - x.Distance(target.Position.To2D()))
                                            .FirstOrDefault();

                if (castpos != null && castpos.IsValid() && castpos.Distance(targetpos) <= 300)
                {
                    return (castpos.To3D());
                }
                else
                {
                    var pos1 = Player.Instance.Position.Extend(targetpos, 400);
                    if (Player.Instance.Distance(targetpos) < 400)
                        pos1 = targetpos;
                    if (pos1.Distance(targetpos) <= 300)
                    {
                        return (pos1.To3D());
                    }
                    return (Game.CursorPos);
                }
            }
            else if (HasUltiPassive(target))
            {
                var poses = UltiPassivePos(target);
                var castpos = poses.OrderByDescending(x => 1 - x.Distance(targetpos)).FirstOrDefault();
                if (castpos != null)
                {
                    return (castpos);
                }
                else
                {
                    var pos1 = Player.Instance.Position.Extend(targetpos, 400);
                    if (Player.Instance.Distance(targetpos) < 400)
                        pos1 = targetpos;
                    if (pos1.Distance(targetpos) <= 300)
                    {
                        return (pos1.To3D());
                    }
                    return (Game.CursorPos);
                }
            }
            else
            {
                var pos1 = Player.Instance.Position.Extend(targetpos, 400);
                if (Player.Instance.Distance(targetpos) < 400)
                    pos1 = targetpos;
                if (pos1.Distance(targetpos) <= 300)
                {
                    return (pos1.To3D());
                }
                return (Game.CursorPos);
            }
        }

        public static void castQhelper(Obj_AI_Base target)
        {
            var targetpos = Prediction.Position.PredictUnitPosition(target, 250);
            if (HasPassive(target))
            {
                var poses = PassiveRadiusPoint(target);
                var pos = PassivePosition(target).To2D();
                var possibleposes = new List<Vector2>();
                for (int i = 0; i <= 400; i = i + 100)
                {
                    var p = Player.Instance.Position.To2D().Extend(pos, i);
                    possibleposes.Add(p);
                }
                var castpos = possibleposes.Where(x => x.To3D().InTheCone(poses, targetpos) && x.Distance(targetpos) <= 300)
                                            .OrderByDescending(x => 1 - x.Distance(target.Position.To2D()))
                                            .FirstOrDefault();
                if (castpos != null && castpos.IsValid() && castpos.Distance(targetpos) <= 300)
                {
                    SpellManager.Q.Cast(castpos.To3D());
                }
                else
                {
                    var pos1 = Player.Instance.Position.Extend(targetpos, 400);
                    if (Player.Instance.Distance(targetpos) < 400)
                        pos1 = targetpos;
                    if (pos1.Distance(targetpos) <= 300)
                    {
                        SpellManager.Q.Cast(pos1.To3D());
                    }
                }
            }
            else if (HasUltiPassive(target))
            {
                var poses = UltiPassivePos(target);
                var castpos = poses.OrderByDescending(x => 1 - x.Distance(targetpos)).FirstOrDefault();
                if (castpos != null)
                {
                    SpellManager.Q.Cast(castpos);
                }
                else
                {
                    var pos1 = Player.Instance.Position.Extend(targetpos, 400);
                    if (Player.Instance.Distance(targetpos) < 400)
                        pos1 = targetpos;
                    if (pos1.Distance(targetpos) <= 300)
                    {
                        SpellManager.Q.Cast(pos1.To3D());
                    }
                }
            }
            else
            {
                var pos1 = Player.Instance.Position.Extend(targetpos, 400);
                if (Player.Instance.Distance(targetpos) < 400)
                    pos1 = targetpos;
                if (pos1.Distance(targetpos) <= 300)
                {
                    SpellManager.Q.Cast(pos1.To3D());
                }
            }
        }

        public static void castAutoAttack(Obj_AI_Base target)
        {
            var targetpos = Prediction.Position.PredictUnitPosition(target, 250);
            var autoAttackRange = Player.Instance.GetAutoAttackRange();

            if (HasPassive(target))
            {
                var poses = PassiveRadiusPoint(target);
                var pos = PassivePosition(target).To2D();
                var possibleposes = new List<Vector2>();
                for (int i = 0; i <= 400; i = i + 100)
                {
                    var p = Player.Instance.Position.To2D().Extend(pos, i);
                    possibleposes.Add(p);
                }
                var castpos = possibleposes.Where(x => x.To3D().InTheCone(poses, targetpos) && x.Distance(targetpos) <= autoAttackRange)
                                            .OrderByDescending(x => 1 - x.Distance(target.Position.To2D()))
                                            .FirstOrDefault();
                if (castpos != null && castpos.IsValid() && castpos.Distance(targetpos) <= autoAttackRange)
                {
                    Orbwalker.OrbwalkTo(castpos.To3D());
                    Player.IssueOrder(GameObjectOrder.AttackUnit, castpos.To3D());
                }
            }
            else if (HasUltiPassive(target))
            {
                var poses = UltiPassivePos(target);
                var castpos = poses.OrderByDescending(x => 1 - x.Distance(targetpos)).FirstOrDefault();
                if (castpos != null)
                {
                    Orbwalker.OrbwalkTo(castpos);
                    Player.IssueOrder(GameObjectOrder.AttackUnit, castpos);
                }
            }
        }

        public static int GetPassiveCount(Obj_AI_Base target)
        {
            var count = FioraPassiveObjects.Count(x => x.Position.Distance(target.Position) <= 50);

            return count;
        }

        public static double GetPassiveDamage(Obj_AI_Base target, int passiveCount)
        {
            return passiveCount * (.03f + Math.Min(Math.Max(.028f, (.027 + .001f * ObjectManager.Player.Level * ObjectManager.Player.FlatPhysicalDamageMod / 100f)), .45f)) * target.MaxHealth;
        }

        public static void Initialize()
        {

        }
        
        static PassiveManager()
        {

        }
    }
}
