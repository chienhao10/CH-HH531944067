using System.Collections.Generic;
using System.Linq;
using EvadeA;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using System;
using EloBuddy;

namespace YasuoPro
{
    static class YasuoEvade
    {
        private static Random rand = new Random();

        internal static void Evade()
        {
            if (!Helper.GetBool("Evade.Enabled", YasuoMenu.EvadeM))
            {
                return;
            }
           
            foreach (var skillshot in Program.DetectedSkillshots.ToList())
            {
                if (skillshot.Dodged)
                {
                    if (Helper.Debug)
                    {
                        //Game.PrintChat(skillshot.SpellData.SpellName + " Dodged already");
                    }
                    continue;
                }

                //Avoid trying to evade while dashing
                if (Helper.Yasuo.LSIsDashing())
                {
                    return;
                }

                if (Helper.GetBool("Evade.OnlyDangerous", YasuoMenu.EvadeM) && !skillshot.SpellData.IsDangerous)
                {
                    continue;
                }

                if ((skillshot.SpellData.Type == SkillShotType.SkillshotCircle || (skillshot.SpellData.CollisionObjects.Contains(CollisionObjectTypes.YasuoWall)) && !SpellSlot.E.IsReady()))
                {
                    continue;
                }


                if (((Program.NoSolutionFound ||
                      !Program.IsSafePath(Helper.Yasuo.GetWaypoints(), 250).IsSafe &&
                      !Program.IsSafe(Helper.Yasuo.Position.LSTo2D()).IsSafe)))
                {
                    Helper.DontDash = true;
                    if (skillshot.IsAboutToHit(700, Helper.Yasuo) && skillshot.SpellData.Type != SkillShotType.SkillshotCircle && Helper.GetBool("Evade.UseW", YasuoMenu.EvadeM))
                    {
                        if (skillshot.SpellData.CollisionObjects.Contains(CollisionObjectTypes.YasuoWall) && skillshot.Evade(SpellSlot.W)
                             && skillshot.SpellData.DangerValue >= Helper.GetSliderInt("Evade.MinDangerLevelWW", YasuoMenu.EvadeM))
                        {
                            var castpos = Helper.Yasuo.ServerPosition.LSExtend(skillshot.MissilePosition.To3D(), 0.25f * Helper.Yasuo.LSDistance(skillshot.MissilePosition));
                            var delay = Helper.GetSliderInt("Evade.Delay", YasuoMenu.EvadeM);
                            if (TickCount - skillshot.StartTick >= skillshot.SpellData.setdelay + rand.Next(delay - 77 > 0 ? delay - 77 : 0, delay + 65)) 
                            {
                                bool WCasted = Helper.Spells[Helper.W].Cast(castpos);
                                Program.DetectedSkillshots.Remove(skillshot);
                                skillshot.Dodged = WCasted;
                                if (WCasted)
                                {
                                    if (Helper.Debug)
                                    {
                                        Chat.Print("Blocked " + skillshot.SpellData.SpellName + " with Windwall ");
                                    }
                                    continue;
                                }
                            }
                        }
                    }
                    if (skillshot.IsAboutToHit(500, Helper.Yasuo) && skillshot.Evade(SpellSlot.E) && !skillshot.Dodged && Helper.GetBool("Evade.UseE", YasuoMenu.EvadeM) && skillshot.SpellData.DangerValue >= Helper.GetSliderInt("Evade.MinDangerLevelE", YasuoMenu.EvadeM))
                    {
                        var evadetarget =
                            ObjectManager
                                .Get<Obj_AI_Base>().Where(x => x.IsDashable() && !Helper.GetDashPos(x).PointUnderEnemyTurret() && Program.IsSafe(x.ServerPosition.LSTo2D()).IsSafe && Program.IsSafePath(x.GeneratePathTo(), 0, 1200, 250).IsSafe).MinOrDefault(x => x.LSDistance(Helper.shop));
                     
                        if (evadetarget != null)
                        {
                            Helper.Spells[Helper.E].CastOnUnit(evadetarget);
                            Program.DetectedSkillshots.Remove(skillshot);
                            skillshot.Dodged = true;
                            if (Helper.Debug)
                            {
                                Chat.Print("Evading " + skillshot.SpellData.SpellName + " " + "using E to " + evadetarget.BaseSkinName);
                            }
                        }
                    }
                }
            }

            Helper.DontDash = false;
        }

        static List<Vector2> GeneratePathTo(this Obj_AI_Base unit)
        {
            List<Vector2> path = new List<Vector2>();
            path.Add(Helper.Yasuo.ServerPosition.LSTo2D());
            path.Add(Helper.GetDashPos(unit));
            return path;
        }

        public static int TickCount
        {
            get { return (int)(Game.Time * 1000f); }
        }
    }
}
