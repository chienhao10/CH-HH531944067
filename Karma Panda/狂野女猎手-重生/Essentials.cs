using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using SharpDX;
using Color = System.Drawing.Color;

namespace NidaleeBuddyEvolution
{
    using System.Collections.Generic;
    using System.Linq;

    using EloBuddy;
    using EloBuddy.SDK;

    internal class Essentials
    {
        /// <summary>
        /// Attack range for Human Form
        /// </summary>
        public const float HumanRange = 525;

        /// <summary>
        /// Attack range for Cougar Form
        /// </summary>
        public const float CatRange = 125;

        /// <summary>
        /// Contains the Last Hunted Target
        /// </summary>
        public static AIHeroClient LastHuntedTarget = null;

        /// <summary>
        /// Returns if the spell should be casted
        /// </summary>
        /// <param name="target">The Target</param>
        /// <param name="spellSlot">The Spell Slot</param>
        /// <param name="menu">The Menu</param>
        /// <returns></returns>
        public static bool ShouldUseSpell(AIHeroClient target, SpellSlot spellSlot, Menu menu)
        {
            if (CatForm())
            {
                switch (spellSlot)
                {
                    case SpellSlot.Q:
                        if (menu["useQC"].Cast<CheckBox>().CurrentValue && target.IsValidTarget(Program.QCat.Range) &&
                            Program.QCat.IsReady() && ManaManager(SpellSlot.Q))
                        {
                            return true;
                        }
                        break;
                    case SpellSlot.W:
                        if (menu["useWC"].Cast<CheckBox>().CurrentValue &&
                            Program.WCat.IsReady() && ManaManager(SpellSlot.W))
                        {
                            if (IsHunted(target) && target.IsValidTarget(Program.WExtended.Range))
                            {
                                return true;
                            }

                            if (!IsHunted(target) && target.IsValidTarget(Program.WCat.Range))
                            {
                                return true;
                            }
                        }
                        break;
                    case SpellSlot.E:
                        if (menu["useEC"].Cast<CheckBox>().CurrentValue && target.IsValidTarget(Program.ECat.Range) &&
                            Program.ECat.IsReady() && ManaManager(SpellSlot.E))
                        {
                            return true;
                        }
                        break;
                    case SpellSlot.R:
                        if (menu["useR"].Cast<CheckBox>().CurrentValue &&
                            Program.R.IsReady())
                        {
                            return true;
                        }
                        break;
                }
                return false;
            }

            if (!CatForm())
            {
                switch (spellSlot)
                {
                    case SpellSlot.Q:
                        if (menu["useQH"].Cast<CheckBox>().CurrentValue && target.IsValidTarget(Program.QHuman.Range) &&
                            Program.QHuman.IsReady() && ManaManager(SpellSlot.Q))
                        {
                            return true;
                        }
                        break;
                    case SpellSlot.W:
                        if (menu["useWH"].Cast<CheckBox>().CurrentValue && target.IsValidTarget(Program.WHuman.Range) &&
                            Program.WHuman.IsReady() && ManaManager(SpellSlot.W))
                        {
                            return true;
                        }
                        break;
                    case SpellSlot.E:
                        if (Program.EHuman.IsReady() && Program.EHuman.IsInRange(target) &&
                            NidaleeMenu.MiscMenu["autoHeal"].Cast<CheckBox>().CurrentValue &&
                            target.Health <= NidaleeMenu.MiscMenu["autoHealPercent"].Cast<Slider>().CurrentValue &&
                            NidaleeMenu.MiscMenu["autoHeal_" + target.BaseSkinName].Cast<CheckBox>().CurrentValue)
                        {
                            return true;
                        }
                        break;
                    case SpellSlot.R:
                        if (menu["useR"].Cast<CheckBox>().CurrentValue &&
                            Program.R.IsReady())
                        {
                            return true;
                        }
                        break;
                }
                return false;
            }

            return false;
        }

        /// <summary>
        /// ManaManager
        /// </summary>
        /// <param name="spellSlot">The spell</param>
        /// <returns>If the spell should be casted</returns>
        public static bool ManaManager(SpellSlot spellSlot)
        {
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo) &&
                NidaleeMenu.MiscMenu["disableMM"].Cast<CheckBox>().CurrentValue)
            {
                return true;
            }

            if (CatForm())
            {
                return true;
            }

            int result;

            switch (spellSlot)
            {
                case SpellSlot.Q:
                    result = NidaleeMenu.MiscMenu["manaQ"].Cast<Slider>().CurrentValue;
                    break;
                case SpellSlot.W:
                    result = NidaleeMenu.MiscMenu["manaW"].Cast<Slider>().CurrentValue;
                    break;
                case SpellSlot.E:
                    result = NidaleeMenu.MiscMenu["manaE"].Cast<Slider>().CurrentValue;
                    break;
                default:
                    result = 0;
                    break;
            }

            return Player.Instance.ManaPercent >= result;
        }

        /// <summary>
        /// Checks if Nidalee is in Cougar Form
        /// </summary>
        /// <returns></returns>
        public static bool CatForm()
        {
            return Player.GetSpell(SpellSlot.Q).Name != "JavelinToss";
        }

        /// <summary>
        /// Checks if target has Nidalee Mark
        /// </summary>
        /// <param name="target">The Target</param>
        /// <returns>If target has Nidalee Mark</returns>
        public static bool IsHunted(Obj_AI_Base target)
        {
            return target.HasBuff("nidaleepassivehunted");
        }

        /// <summary>
        /// Returns true if the spell is ready via game time.
        /// </summary>
        /// <param name="time"></param>
        /// <param name="extra"></param>
        /// <returns></returns>
        public static bool IsReady(float time, float extra = 0f)
        {
            return time < 1 + extra;
        }

        /// <summary>
        /// Returns if there is a summoner spell with Name Provided. (Taken from ActivatorBuddy)
        /// </summary>
        /// <param name="s">String</param>
        /// <returns>Returns if there is a summoner spell with Name Provided.</returns>
        public static bool HasSpell(string s)
        {
            return Player.Spells.FirstOrDefault(o => o.SData.Name.ToLower().Contains(s)) != null;
        }

        /// <summary>
        /// Gets the amount of damage smite does.
        /// </summary>
        /// <returns></returns>
        public static float GetSmiteDamage(Obj_AI_Base target)
        {
            return Player.Instance.GetSummonerSpellDamage(target, EloBuddy.SDK.DamageLibrary.SummonerSpells.Smite);
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
        /// Item ID's for Skirmersher
        /// </summary>
        public static ItemId[] SkirmisherItemIds =
        {
            ItemId.Skirmishers_Sabre,
            ItemId.Skirmishers_Sabre_Enchantment_Cinderhulk, ItemId.Skirmishers_Sabre_Enchantment_Devourer,
            ItemId.Skirmishers_Sabre_Enchantment_Runic_Echoes, ItemId.Skirmishers_Sabre_Enchantment_Sated_Devourer,
            ItemId.Skirmishers_Sabre_Enchantment_Warrior,
        };

        /// <summary>
        /// Item ID's for Stalker Blade
        /// </summary>
        public static ItemId[] StalkerBladeItemIds =
        {
            ItemId.Stalkers_Blade,
            ItemId.Stalkers_Blade_Enchantment_Cinderhulk, ItemId.Stalkers_Blade_Enchantment_Devourer,
            ItemId.Stalkers_Blade_Enchantment_Runic_Echoes, ItemId.Stalkers_Blade_Enchantment_Sated_Devourer,
            ItemId.Stalkers_Blade_Enchantment_Warrior,
        };

        /// <summary>
        /// Item ID's for Tracker Knife
        /// </summary>
        public static ItemId[] TrackerKnifeItemIds =
        {
            ItemId.Trackers_Knife,
            ItemId.Trackers_Knife_Enchantment_Cinderhulk, ItemId.Trackers_Knife_Enchantment_Devourer,
            ItemId.Trackers_Knife_Enchantment_Runic_Echoes, ItemId.Trackers_Knife_Enchantment_Sated_Devourer,
            ItemId.Trackers_Knife_Enchantment_Warrior,
        };

        /// <summary>
        /// Fixed for Patch 6.x
        /// </summary>
        public static void SetSmiteSlot()
        {
            SpellSlot smiteSlot;

            if (StalkerBladeItemIds.Any(item => Player.Instance.HasItem(item)))
            {
                smiteSlot = ObjectManager.Player.GetSpellSlotFromName("s5_summonersmiteplayerganker");
            }
            else if (SkirmisherItemIds.Any(item => Player.Instance.HasItem(item)))
            {

                smiteSlot = ObjectManager.Player.GetSpellSlotFromName("s5_summonersmiteduel");
            }
            else if (TrackerKnifeItemIds.Any(item => Player.Instance.HasItem(item)))
            {
                smiteSlot = ObjectManager.Player.GetSpellSlotFromName("summonersmite");
            }
            else
            {
                smiteSlot = ObjectManager.Player.GetSpellSlotFromName("summonersmite");
            }

            Program.Smite = new Spell.Targeted(smiteSlot, 500);
        }

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
        /// Stores the current tickcount of the spell.
        /// </summary>
        public static Dictionary<string, float> SpellTimer = new Dictionary<string, float>
        {
            {"Takedown", 0f},
            {"Pounce", 0f},
            {"ExPounce", 0f},
            {"Swipe", 0f},
            {"Javelin", 0f},
            {"Bushwhack", 0f},
            {"Primalsurge", 0f},
            {"Aspect", 0f}
        };

        /// <summary>
        /// Stores when the last spell was used.
        /// </summary>
        public static Dictionary<string, float> TimeStamp = new Dictionary<string, float>
        {
            {"Takedown", 0f},
            {"Pounce", 0f},
            {"Swipe", 0f},
            {"Javelin", 0f},
            {"Bushwhack", 0f},
            {"Primalsurge", 0f},
        };

        /// <summary>
        /// DamageLibrary Class for Jinx Spells.
        /// </summary>
        public static class DamageLibrary
        {
            /// <summary>
            /// Calculates and returns damage totally done to the target
            /// </summary>
            /// <param name="target">The Target</param>
            /// <param name="useQ">Include useQ in Calculations?</param>
            /// <param name="useW">Include useW in Calculations?</param>
            /// <param name="useE">Include useE in Calculations?</param>
            /// <param name="useR">Include useR in Calculations?</param>
            /// <returns>The total damage done to target.</returns>
            public static float CalculateDamage(Obj_AI_Base target, bool useQ, bool useW, bool useE, bool useR)
            {
                if (target == null)
                {
                    return 0;
                }

                var totaldamage = 0f;

                if (CatForm())
                {
                    if (useQ && Program.QHuman.IsReady())
                    {
                        totaldamage = totaldamage + QDamage(target);
                    }

                    if (useW && Program.WHuman.IsReady())
                    {
                        totaldamage = totaldamage + WDamage(target);
                    }

                    if (useE && Program.EHuman.IsReady())
                    {
                        totaldamage = totaldamage + EDamage(target);
                    }

                    if (useR && Program.R.IsReady())
                    {
                        totaldamage = totaldamage + RDamage();
                    }
                }
                else
                {
                    if (useQ && Program.QCat.IsReady())
                    {
                        totaldamage = totaldamage + QDamage(target);
                    }

                    if (useW && (Program.WCat.IsReady() || Program.WExtended.IsReady()))
                    {
                        totaldamage = totaldamage + WDamage(target);
                    }

                    if (useE && Program.ECat.IsReady())
                    {
                        totaldamage = totaldamage + EDamage(target);
                    }

                    if (useR && Program.R.IsReady())
                    {
                        totaldamage = totaldamage + RDamage();
                    }
                }

                return totaldamage;
            }

            /// <summary>
            /// Calculates the Damage done with useQ
            /// </summary>
            /// <param name="target">The Target</param>
            /// <returns>Returns the Damage done with useQ</returns>
            private static float QDamage(Obj_AI_Base target)
            {
                if (!CatForm())
                {
                    var dist = target.Distance(Player.Instance);
                    var extraDmg2 = 0;

                    for (double i = 0; i < (dist); i++)
                    {
                        i = i + 3.875;
                        extraDmg2 += 1;
                    }

                    var finalExtra2 = extraDmg2 <= 200f ? extraDmg2/100f : 2f;

                    return Player.Instance.CalculateDamageOnUnit(target, DamageType.Magical,
                        (new[] {0f, 60f, 77.5f, 95f, 112.5f, 130f}[Program.QHuman.Level] +
                         (Player.Instance.TotalMagicalDamage*(0.4f)))) * finalExtra2;
                }

                if (CatForm())
                {
                    var missingHealth = (target.MaxHealth - target.Health)/100f;
                    var extraDmg = 0f;
                    for (var i = 0; i < missingHealth; i++)
                    {
                        extraDmg += 1.5f;
                    }
                    var finalExtra = extraDmg <= 150f ? (extraDmg/100f) : 1.5f;

                    if (IsHunted(target))
                    {
                        return Player.Instance.CalculateDamageOnUnit(target, DamageType.Mixed,
                            (new[] {0f, 5.3f, 26.7f, 66.7f, 120f}[Program.R.Level] +
                             (Player.Instance.TotalAttackDamage*finalExtra) +
                             (Player.Instance.TotalMagicalDamage*(0.48f*finalExtra))));
                    }
                    return Player.Instance.CalculateDamageOnUnit(target, DamageType.Mixed,
                        (new[] {0f, 4f, 20f, 50f, 90f}[Program.R.Level] +
                         (Player.Instance.TotalAttackDamage*(0.75f*finalExtra)) +
                         (Player.Instance.TotalMagicalDamage*(0.36f*finalExtra))));
                }

                return 0f;
            }

            /// <summary>
            /// Calculates the Damage done with useW
            /// </summary>
            /// <param name="target">The Target</param>
            /// <returns>Returns the Damage done with useW</returns>
            private static float WDamage(Obj_AI_Base target)
            {
                if (!CatForm())
                {
                    return Player.Instance.CalculateDamageOnUnit(target, DamageType.Magical,
                        new[] {0, 10, 20, 30, 40, 50}[Program.WHuman.Level] + (Player.Instance.TotalMagicalDamage*0.05f));
                }
                return Player.Instance.CalculateDamageOnUnit(target, DamageType.Magical,
                    new[] {0, 50, 100, 150, 200}[Program.R.Level] + (Player.Instance.TotalMagicalDamage*0.3f));
            }

            /// <summary>
            /// Calculates the Damage done with useE
            /// </summary>
            /// <param name="target">The Target</param>
            /// <returns>Returns the Damage done with useE</returns>
            private static float EDamage(Obj_AI_Base target)
            {
                if (!CatForm())
                {
                    return 0f;
                }
                return Player.Instance.CalculateDamageOnUnit(target, DamageType.Magical,
                    new[] {0, 70, 130, 190, 250}[Program.R.Level] + (Player.Instance.TotalMagicalDamage*0.45f));
            }

            /// <summary>
            /// Calculates the Damage done with useR
            /// </summary>
            /// <returns>Returns the Damage done with useR</returns>
            private static float RDamage()
            {
                return 0f;
            }
        }
    }
}