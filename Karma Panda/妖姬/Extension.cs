using System.Linq;
using EloBuddy;
using EloBuddy.SDK;

namespace LelBlanc
{
    internal class Extension
    {
        /// <summary>
        /// Checks if Player should use W to return.
        /// </summary>
        public static bool LogicReturn(bool w2 = false)
        {
            var enemiesBeingE =
                EntityManager.Heroes.Enemies.Where(t => t.IsValidTarget(Program.E.Range) && IsBeingE(t))
                    .ToArray();

            if (enemiesBeingE.Any())
            {
                return false;
            }

            if (!enemiesBeingE.Any() && Program.E.IsReady() && Player.Instance.CountEnemiesInRange(Program.E.Range) > 0)
            {
                return false;
            }

            var enemiesNearLastPosition = Program.LastWPosition.CountEnemiesInRange(Player.Instance.AttackRange);
            var enemiesNearCurrentPosition = Player.Instance.CountEnemiesInRange(Player.Instance.AttackRange);
            var alliesNearLastPosition = Program.LastWPosition.CountAlliesInRange(Player.Instance.AttackRange);
            var alliesNearCurrentPosition = Player.Instance.CountAlliesInRange(Player.Instance.AttackRange);

            if (enemiesNearCurrentPosition < enemiesNearLastPosition ||
                alliesNearCurrentPosition > alliesNearLastPosition ||
                !Player.Instance.IsUnderTurret() && Program.LastWPosition.IsUnderTurret())
            {
                return false;
            }

            if (w2)
            {
                if (Program.RReturn.IsReady() &&
                    Player.Instance.Spellbook.GetSpell(SpellSlot.R).Name.ToLower() != "leblancslidereturnm")
                {
                    Program.RReturn.Cast();
                    return true;
                }
                return false;
            }

            if (Program.WReturn.IsReady() &&
                Player.Instance.Spellbook.GetSpell(SpellSlot.W).Name.ToLower() == "leblancslidereturn")
            {
                Program.WReturn.Cast();
                return true;
            }
            return false;
        }

        /// <summary>
        /// Checks if the Player has the spell.
        /// </summary>
        /// <param name="s">The Spell Name</param>
        /// <returns></returns>
        public static bool HasSpell(string s)
        {
            return Player.Spells.FirstOrDefault(o => o.SData.Name.ToLower() == s) != null;
        }

        /// <summary>
        /// Checks to see if the target is being silenced
        /// </summary>
        /// <param name="target">The Target</param>
        /// <returns></returns>
        public static bool IsMarked(Obj_AI_Base target)
        {
            return target.HasBuff("LeblancMarkOfSilence") || target.HasBuff("LeblancMarkOfSilenceM");
        }

        /// <summary>
        /// Checks to see if the target is being E'ed
        /// </summary>
        /// <param name="target">The Target</param>
        /// <returns></returns>
        public static bool IsBeingE(Obj_AI_Base target)
        {
            return target.HasBuff("LeblancShackleBeam") || target.HasBuff("LeblancShackleBeamM");
        }

        internal class DamageLibrary
        {
            /// <summary>
            /// Calculates Damage for LeBlanc
            /// </summary>
            /// <param name="target">The Target</param>
            /// <param name="q">The Q</param>
            /// <param name="w">The W</param>
            /// <param name="e">The E</param>
            /// <param name="r">The R</param>
            /// <returns></returns>
            public static float CalculateDamage(Obj_AI_Base target, bool q, bool w, bool e, bool r, bool ignite)
            {
                var totaldamage = 0f;

                if (target == null) return totaldamage;

                if (q && Program.Q.IsReady())
                {
                    totaldamage += QDamage(target);
                }

                if (w && Program.W.IsReady() &&
                    Player.Instance.Spellbook.GetSpell(SpellSlot.W).Name.ToLower() == "leblancslide")
                {
                    totaldamage = WDamage(target);

                    if (q && Program.Q.IsReady() || IsMarked(target))
                    {
                        totaldamage += QDamage(target);
                    }
                }

                if (e && Program.E.IsReady())
                {
                    totaldamage += EDamage(target);

                    if (q && Program.Q.IsReady() || IsMarked(target))
                    {
                        totaldamage += QDamage(target);
                    }
                }

                if (r && Program.QUltimate.IsReady() &&
                    Player.Instance.Spellbook.GetSpell(SpellSlot.R).Name.ToLower() == "leblancchaosorbm")
                {
                    totaldamage += QDamage(target);

                    if (q && Program.Q.IsReady() || IsMarked(target))
                    {
                        totaldamage += QDamage(target);
                    }
                }

                if (r && Program.WUltimate.IsReady() &&
                    Player.Instance.Spellbook.GetSpell(SpellSlot.R).Name.ToLower() == "leblancslidem")
                {
                    totaldamage += WrDamage(target);

                    if (q && Program.Q.IsReady() || IsMarked(target))
                    {
                        totaldamage += QDamage(target);
                    }
                }

                if (r && Program.EUltimate.IsReady() &&
                    Player.Instance.Spellbook.GetSpell(SpellSlot.R).Name.ToLower() == "leblancsoulshacklem")
                {
                    totaldamage += ErDamage(target);

                    if (q && Program.Q.IsReady() || IsMarked(target))
                    {
                        totaldamage += QDamage(target);
                    }
                }

                if (ignite && Program.Ignite != null && Program.Ignite.IsReady() && Program.Ignite.IsInRange(target))
                {
                    totaldamage += Player.Instance.GetSummonerSpellDamage(target,
                        EloBuddy.SDK.DamageLibrary.SummonerSpells.Ignite);
                }

                return totaldamage;
            }

            /// <summary>
            /// Calculates the Damage done with Q
            /// </summary>
            /// <param name="target">The Target</param>
            /// <returns>Returns the Damage done with Q</returns>
            private static float QDamage(Obj_AI_Base target)
            {
                return Player.Instance.CalculateDamageOnUnit(target, DamageType.Magical,
                    new[] {0, 55, 80, 105, 130, 155}[Program.Q.Level] + (Player.Instance.TotalMagicalDamage*0.4f));
            }

            /// <summary>
            /// Calculates the Damage done with W
            /// </summary>
            /// <param name="target">The Target</param>
            /// <returns>Returns the Damage done with W</returns>
            private static float WDamage(Obj_AI_Base target)
            {
                return Player.Instance.CalculateDamageOnUnit(target, DamageType.Magical,
                    new[] {0, 85, 125, 165, 205, 245}[Program.W.Level] + (Player.Instance.TotalMagicalDamage*0.6f));
            }

            /// <summary>
            /// Calculates the Damage done with E
            /// </summary>
            /// <param name="target">The Target</param>
            /// <returns>Returns the Damage done with E</returns>
            private static float EDamage(Obj_AI_Base target)
            {
                return Player.Instance.CalculateDamageOnUnit(target, DamageType.Magical,
                    new[] {0, 40, 65, 90, 115, 140}[Program.E.Level] + (Player.Instance.TotalMagicalDamage*0.5f));
            }

            /// <summary>
            /// Calculates the Damage done with R
            /// </summary>
            /// <returns>Returns the Damage done with R</returns>
            private static float WrDamage(Obj_AI_Base target)
            {
                return Player.Instance.CalculateDamageOnUnit(target, DamageType.Magical,
                    new[] {0, 150, 300, 450}[Program.WUltimate.Level] + (Player.Instance.TotalMagicalDamage*0.9f));
            }

            /// <summary>
            /// Calculates the Damage done with R
            /// </summary>
            /// <returns>Returns the Damage done with R</returns>
            private static float ErDamage(Obj_AI_Base target)
            {
                return Player.Instance.CalculateDamageOnUnit(target, DamageType.Magical,
                    new[] {0, 100, 200, 300}[Program.EUltimate.Level] + (Player.Instance.TotalMagicalDamage*0.6f));
            }
        }
    }
}