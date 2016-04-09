using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EloBuddy;
using EloBuddy.SDK;
using SharpDX;

namespace Auto_Carry_Vayne.Logic
{
    class Mechanics
    {
        #region Mechanics:Insec
        public static void Insec()
        {
            if (!Manager.MenuManager.InsecE) return;

            var mode = (Manager.MenuManager.InsecPositions);
            var target = TargetSelector.GetTarget((int)Variables._Player.GetAutoAttackRange(),
    DamageType.Physical);
            if (target != null)
            {
                //var targetfuturepos = Prediction.GetPrediction(target, 0.1f).UnitPosition;
                bool caninsec = Variables._Player.Distance(target) <= 400;
                switch (mode)
                {
                    case 0:
                        var hero =
                            EntityManager.Heroes.Allies.Where(x => !x.IsMe && !x.IsDead)
                                .OrderByDescending(x => x.Distance(Variables._Player.Position))
                                .LastOrDefault();
                        if (hero != null && caninsec &&
                            Variables._Player.ServerPosition.Distance(hero.Position) + 100 >=
                            target.Distance(hero.Position))
                        {
                            var ePred = Manager.SpellManager.E2.GetPrediction(target);
                            int pushDist = 550;
                            for (int i = 0; i < pushDist; i += (int)target.BoundingRadius)
                            {
                                Vector3 loc3 =
                                    ePred.UnitPosition.To2D().Extend(GetFlashPos(target, true).To2D(), -i).To3D();
                                if (loc3.Distance(hero) < hero.Position.Distance(target))
                                {
                                    Variables._Player.Spellbook.CastSpell(Variables.FlashSlot, GetFlashPos(target, true));
                                    Manager.SpellManager.E.Cast(target);
                                }
                            }
                        }
                        break;
                    case 1:
                        var turret =
                            ObjectManager.Get<Obj_AI_Turret>()
                                .Where(x => x.IsAlly && !x.IsDead)
                                .OrderByDescending(x => x.Distance(Variables._Player.Position))
                                .LastOrDefault();
                        if (turret != null && caninsec &&
                            Variables._Player.ServerPosition.Distance(turret.Position) + 100 >=
                            target.Distance(turret.Position))
                        {
                            var ePred = Manager.SpellManager.E2.GetPrediction(target);
                            int pushDist = 550;
                            for (int i = 0; i < pushDist; i += (int)target.BoundingRadius)
                            {
                                Vector3 loc3 =
                                    ePred.UnitPosition.To2D().Extend(GetFlashPos(target, true).To2D(), -i).To3D();
                                if (loc3.Distance(turret) < turret.Position.Distance(target))
                                {
                                    Variables._Player.Spellbook.CastSpell(Variables.FlashSlot, GetFlashPos(target, true));
                                    Manager.SpellManager.E.Cast(target);
                                }
                            }
                        }
                        break;
                    case 2:
                        if (caninsec &&
                            Variables._Player.ServerPosition.Distance(Game.CursorPos) + 100 >=
                            target.Distance(Game.CursorPos))
                        {
                            var ePred = Manager.SpellManager.E2.GetPrediction(target);
                            int pushDist = 550;
                            for (int i = 0; i < pushDist; i += (int)target.BoundingRadius)
                            {
                                Vector3 loc3 =
                                    ePred.UnitPosition.To2D().Extend(GetFlashPos(target, true).To2D(), -i).To3D();
                                if (loc3.Distance(Game.CursorPos) < Game.CursorPos.Distance(target))
                                {
                                    Variables._Player.Spellbook.CastSpell(Variables.FlashSlot, GetFlashPos(target, true));
                                    Manager.SpellManager.E.Cast(target);
                                }
                            }
                        }
                        break;

                }
            }
        }
        #endregion
        #region Mechanics:FlashE
        public static void FlashE()
        {
            if (!Manager.MenuManager.FlashE) return;

            var positions = GetRotatedFlashPositions();

            foreach (var p in positions)
            {
                var condemnUnit = CondemnCheck(p);
                if (condemnUnit != null)
                {
                    Manager.SpellManager.E.Cast(condemnUnit);

                    Variables._Player.Spellbook.CastSpell(Variables.FlashSlot, p);

                }
            }
        }

        public static int CountHerosInRange(AIHeroClient target, bool checkteam, float range = 1200f)
        {
            var objListTeam =
                ObjectManager.Get<AIHeroClient>()
                    .Where(
                        x => x.IsValidTarget(range));

            return objListTeam.Count(hero => checkteam ? hero.Team != target.Team : hero.Team == target.Team);
        }

        public static Vector2 GetFirstNonWallPos(Vector2 startPos, Vector2 endPos)
        {
            int distance = 0;
            for (int i = 0; i < Manager.MenuManager.CondemnPushDistance; i += 20)
            {
                var cell = startPos.Extend(endPos, endPos.Distance(startPos) + i);
                if (NavMesh.GetCollisionFlags(cell).HasFlag(CollisionFlags.Wall) ||
                    NavMesh.GetCollisionFlags(cell).HasFlag(CollisionFlags.Building))
                {
                    distance = i - 20;
                }
            }
            return startPos.Extend(endPos, distance + endPos.Distance(startPos));
        }

        public static List<Vector3> GetRotatedFlashPositions()
        {
            const int currentStep = 30;
            var direction = Variables._Player.Direction.To2D().Perpendicular();

            var list = new List<Vector3>();
            for (var i = -90; i <= 90; i += currentStep)
            {
                var angleRad = Geometry.DegreeToRadian(i);
                var rotatedPosition = Variables._Player.Position.To2D() + (425f * direction.Rotated(angleRad));
                list.Add(rotatedPosition.To3D());
            }
            return list;
        }

        public static void LoadFlash()
        {
            var testSlot = Variables._Player.GetSpellSlotFromName("summonerflash");
            if (testSlot != SpellSlot.Unknown)
            {
                Console.WriteLine("Flash Slot: {0}", testSlot);
                Variables.FlashSlot = testSlot;
            }
            else
            {
                Console.WriteLine("Error loading Flash! Not found!");
            }
        }

        public static Vector3 GetFlashPos(AIHeroClient target, bool serverPos, int distance = 150)
        {
            var enemyPos = serverPos ? target.ServerPosition : target.Position;
            var myPos = serverPos ? Variables._Player.ServerPosition : Variables._Player.Position;

            return enemyPos + Vector3.Normalize(enemyPos - myPos) * distance;
        }

        public static AIHeroClient CondemnCheck(Vector3 fromPosition)
        {
            var HeroList = EntityManager.Heroes.Enemies.Where(
                h =>
                    h.IsValidTarget(Manager.SpellManager.E.Range) &&
                    !h.HasBuffOfType(BuffType.SpellShield) &&
                    !h.HasBuffOfType(BuffType.SpellImmunity));
            foreach (var Hero in HeroList)
            {
                var ePred = Manager.SpellManager.E2.GetPrediction(Hero);
                int pushDist = Manager.MenuManager.CondemnPushDistance;
                for (int i = 0; i < pushDist; i += (int)Hero.BoundingRadius)
                {
                    Vector3 loc3 = ePred.UnitPosition.To2D().Extend(fromPosition.To2D(), -i).To3D();
                    var collFlags = NavMesh.GetCollisionFlags(loc3);
                    if (collFlags.HasFlag(CollisionFlags.Wall) || collFlags.HasFlag(CollisionFlags.Building))
                    {
                        return Hero;
                    }
                }
            }
            return null;
        }
        #endregion
        #region Mechanics:RotE
        public static void RotE()
        {
            var target = TargetSelector.GetTarget((int)Manager.SpellManager.zzrot.Range, DamageType.Physical);
            if (Manager.MenuManager.RotE && Manager.SpellManager.E.IsReady() && Manager.SpellManager.zzrot.IsReady() && target != null)
            {
                if (Manager.SpellManager.E.Cast(target))
                {
                    Core.DelayAction(() => Manager.SpellManager.zzrot.Cast(target.ServerPosition.Extend(Variables._Player.ServerPosition, -100)), 100);
                }
            }
        }
        #endregion
    }
}

