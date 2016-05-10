using System;
using System.Linq;

namespace Jinx
{
    using EloBuddy;
    using EloBuddy.SDK;

    using SharpDX;

    using Color = System.Drawing.Color;

    /// <summary>
    /// A Class Containing Methods that are needed for the Main Program.
    /// </summary>
    internal class Essentials
    {
        /// <summary>
        /// Jungle Mob List 
        /// </summary>
        public static readonly string[] JungleMobsList =
        {
            "SRU_Red", "SRU_Blue", "SRU_Dragon", "SRU_Baron", "SRU_Gromp",
            "SRU_Murkwolf", "SRU_Razorbeak", "SRU_Krug", "Sru_Crab"
        };

        /// <summary>
        /// Jungle Mob List for Twisted Treeline
        /// </summary>
        public static readonly string[] JungleMobsListTwistedTreeline =
        {
            "TT_NWraith1.1", "TT_NWraith4.1",
            "TT_NGolem2.1", "TT_NGolem5.1", "TT_NWolf3.1", "TT_NWolf6.1", "TT_Spiderboss8.1"
        };

        /// <summary>
        /// Thank you ScienceARK for this method
        /// </summary>
        /// <returns>If Jinx is using FishBones.</returns>
        public static bool FishBones()
        {
            return Player.Instance.HasBuff("JinxQ");
        }

        /// <summary>
        /// Check if Player has Undying Buff
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public static bool HasUndyingBuff(Obj_AI_Base target)
        {
            var tChampion = target as AIHeroClient;

            if (tChampion != null)
            {
                // Poppy R
                if (tChampion.ChampionName == "Poppy")
                {
                    if (
                        EntityManager.Heroes.Allies.Any(
                            o =>
                                !o.IsMe &&
                                o.Buffs.Any(
                                    b =>
                                        b.Caster.NetworkId == target.NetworkId && b.IsValid() &&
                                        b.DisplayName == "PoppyDITarget")))
                    {
                        return true;
                    }
                }
                return tChampion.IsInvulnerable;
            }

            // Various buffs
            if (target.Buffs.Any(
                b => b.IsValid() &&
                     (b.DisplayName == "Chrono Shift" /* Zilean R */||
                      b.DisplayName == "JudicatorIntervention" /* Kayle R */||
                      b.DisplayName == "Undying Rage" /* Tryndamere R */)))
            {
                return true;
            }
            return target.IsInvulnerable;
        }

        /// <summary>
        /// Taken from OKTW. Spells that useE can be used on.
        /// </summary>
        /// <param name="spellName">The name of the Spell</param>
        /// <returns>If useE should be used or not.</returns>
        public static bool ShouldUseE(string spellName)
        {
            switch (spellName)
            {
                case "ThreshQ":
                    return true;
                case "KatarinaR":
                    return true;
                case "AlZaharNetherGrasp":
                    return true;
                case "GalioIdolOfDurand":
                    return true;
                case "LuxMaliceCannon":
                    return true;
                case "MissFortuneBulletTime":
                    return true;
                case "RocketGrabMissile":
                    return true;
                case "CaitlynPiltoverPeacemaker":
                    return true;
                case "EzrealTrueshotBarrage":
                    return true;
                case "InfiniteDuress":
                    return true;
                case "VelkozR":
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Checks if Player Has Rapid Fire Cannon Buff
        /// </summary>
        public static bool RapidFireCannon
        {
            get { return Player.HasBuff("itemstatikshankcharge"); }
        }

        /// <summary>
        /// Contains the range of Minigun.
        /// </summary>
        public static float MinigunRange
        {
            get { return RapidFireCannon ? 525f + 150f : 525f; }
        }

        /// <summary>
        /// Contains the Last Blitzcrank hook time.
        /// </summary>
        public static double GrabTime = 0;

        /// <summary>
        /// Gets the Range of FishBones
        /// </summary>
        /// <returns>Returns the range of FishBones</returns>
        public static float FishBonesRange()
        {
            return 670f + Player.Instance.BoundingRadius + 25*Program.Q.Level;
        }

        /// <summary>
        /// Taken from AdEvade which was taken from OKTW
        /// </summary>
        /// <param name="start">Start Position of Line</param>
        /// <param name="end">End Position of Line</param>
        /// <param name="radius">Radius of Line</param>
        /// <param name="width">Width of Line</param>
        /// <param name="color">Color of Line</param>
        public static void DrawLineRectangle(Vector2 start, Vector2 end, int radius, int width, Color color)
        {
            var dir = (end - start).Normalized();
            var pDir = dir.Perpendicular();

            var rightStartPos = start + pDir*radius;
            var leftStartPos = start - pDir*radius;
            var rightEndPos = end + pDir*radius;
            var leftEndPos = end - pDir*radius;

            var rStartPos =
                Drawing.WorldToScreen(new Vector3(rightStartPos.X, rightStartPos.Y, Player.Instance.Position.Z));
            var lStartPos =
                Drawing.WorldToScreen(new Vector3(leftStartPos.X, leftStartPos.Y, Player.Instance.Position.Z));
            var rEndPos = Drawing.WorldToScreen(new Vector3(rightEndPos.X, rightEndPos.Y, Player.Instance.Position.Z));
            var lEndPos = Drawing.WorldToScreen(new Vector3(leftEndPos.X, leftEndPos.Y, Player.Instance.Position.Z));

            Drawing.DrawLine(rStartPos, rEndPos, width, color);
            Drawing.DrawLine(lStartPos, lEndPos, width, color);
            Drawing.DrawLine(rStartPos, lStartPos, width, color);
            Drawing.DrawLine(lEndPos, rEndPos, width, color);
        }

        /// <summary>
        /// DamageLibrary Class for Jinx Spells.
        /// </summary>
        public static class DamageLibrary
        {
            /// <summary>
            /// Calculates and returns damage totally done to the Target
            /// </summary>
            /// <param name="target">The Target</param>
            /// <param name="useQ">Include Q in Calculations?</param>
            /// <param name="useW">Include W in Calculations?</param>
            /// <param name="useE">Include E in Calculations?</param>
            /// <param name="useR">Include R in Calculations?</param>
            /// <returns>The total damage done to target.</returns>
            public static float CalculateDamage(Obj_AI_Base target, bool useQ, bool useW, bool useE, bool useR)
            {
                if (target == null)
                {
                    return 0;
                }

                var totaldamage = 0f;

                if (useQ && Program.Q.IsReady())
                {
                    totaldamage = totaldamage + QDamage(target);
                }

                if (useW && Program.W.IsReady())
                {
                    totaldamage = totaldamage + WDamage(target);
                }

                if (useE && Program.E.IsReady())
                {
                    totaldamage = totaldamage + EDamage(target);
                }

                if (useR && Program.R.IsReady())
                {
                    totaldamage = totaldamage + RDamage(target);
                }

                return totaldamage;
            }

            /// <summary>
            /// Calculates the Damage done with Q
            /// </summary>
            /// <param name="target">The Target</param>
            /// <returns>Returns the Damage done with useQ</returns>
            private static float QDamage(Obj_AI_Base target)
            {
                return Player.Instance.GetAutoAttackDamage(target);
            }

            /// <summary>
            /// Calculates the Damage done with W
            /// </summary>
            /// <param name="target">The Target</param>
            /// <returns>Returns the Damage done with useW</returns>
            private static float WDamage(Obj_AI_Base target)
            {
                return Player.Instance.CalculateDamageOnUnit(
                    target,
                    DamageType.Physical,
                    new[] {0, 10, 60, 110, 160, 210}[Program.W.Level]
                    + (Player.Instance.TotalAttackDamage*1.4f));
            }

            /// <summary>
            /// Calculates the Damage done with E
            /// </summary>
            /// <param name="target">The Target</param>
            /// <returns>Returns the Damage done with useE</returns>
            private static float EDamage(Obj_AI_Base target)
            {
                return Player.Instance.CalculateDamageOnUnit(
                    target,
                    DamageType.Magical,
                    new[] {0, 80, 135, 190, 245, 300}[Program.E.Level] + (Player.Instance.TotalMagicalDamage));
            }

            /// <summary>
            /// Calculates the Damage done with R
            /// </summary>
            /// <param name="target">The Target</param>
            /// <returns>Returns the Damage done with useR</returns>
            private static float RDamage(Obj_AI_Base target)
            {
                var distance = Player.Instance.Distance(target);
                var increment = distance/100f;
                var extraPercent = ((10f + (increment*6f))/100f);
                var damage = new[] {0f, 25f, 35f, 45f}[Program.R.Level]*(1f + extraPercent) +
                             (new[] {0, 0.25f, 0.3f, 0.35f}[Program.R.Level]*(target.MaxHealth - target.Health)/100f) +
                             (0.1f + Player.Instance.FlatPhysicalDamageMod);

                return damage;
            }
        }
    }
}