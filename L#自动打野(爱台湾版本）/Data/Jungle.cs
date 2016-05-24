using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;

namespace AutoJungle.Data
{
    public class Jungle
    {
        public static Obj_AI_Hero player = ObjectManager.Player;

        public static readonly string[] bosses = { "TT_Spiderboss", "SRU_Dragon", "SRU_Baron", "SRU_RiftHerald" };
        public static SpellSlot smiteSlot = SpellSlot.Unknown;
        public static Spell smite;
        public static int smiteRange = 700;

        public static double smiteDamage(Obj_AI_Base target)
        {
            return player.GetSummonerSpellDamage(target, Damage.SummonerSpell.Smite);
        }

        public static void CastSmite(Obj_AI_Base target)
        {
            if (!target.IsValidTarget())
            {
                return;
            }
            if (Program._GameInfo.CurrentMonster == 13 && !target.Name.Contains("Dragon"))
            {
                return;
            }
            if (smiteSlot != SpellSlot.Unknown)
            {
                bool smiteReady = ObjectManager.Player.Spellbook.CanUseSpell(smiteSlot) == SpellState.Ready;
                if (target != null)
                {
                    if (smite.CanCast(target) && smiteReady && player.Distance(target.Position) <= smite.Range &&
                        target.Health < target.MaxHealth)
                    {
                        if (smiteDamage(target) > target.Health ||
                            (((target.Name.Contains("Krug") || target.Name.Contains("Gromp")) &&
                              player.CountEnemiesInRange(1000) == 0)) ||
                            (target.Name.Contains("SRU_Red") && player.HealthPercent < 5))
                        {
                            smite.Cast(target);
                        }
                    }
                }
            }
        }

        public static void CastSmiteHero(Obj_AI_Hero target)
        {
            if (!target.IsValidTarget())
            {
                return;
            }
            if (Program._GameInfo.CurrentMonster == 13 && !target.Name.Contains("Dragon"))
            {
                return;
            }
            if (smiteSlot != SpellSlot.Unknown)
            {
                bool smiteReady = ObjectManager.Player.Spellbook.CanUseSpell(smiteSlot) == SpellState.Ready;
                if (target != null)
                {
                    if (smite.CanCast(target) && smiteReady && player.Distance(target.Position) <= smite.Range &&
                        target.Health > Helpers.GetComboDMG(player, target) * 0.7f &&
                        player.Distance(target) < Orbwalking.GetRealAutoAttackRange(target) &&
                        Program._GameInfo.SmiteableMob == null)
                    {
                        smite.Cast(target);
                    }
                }
            }
        }

        public static bool SmiteReady()
        {
            if (smiteSlot != SpellSlot.Unknown)
            {
                return ObjectManager.Player.Spellbook.CanUseSpell(smiteSlot) == SpellState.Ready;
            }
            return false;
        }

        //Kurisu
        private static readonly int[] SmitePurple = { 3713, 3726, 3725, 3724, 3723, 3933 };
        private static readonly int[] SmiteGrey = { 3711, 3722, 3721, 3720, 3719, 3932 };
        private static readonly int[] SmiteRed = { 3715, 3718, 3717, 3716, 3714, 3931, 1415 };
        private static readonly int[] SmiteBlue = { 3706, 3710, 3709, 3708, 3707, 3930 };

        public static string smitetype()
        {
            if (SmiteBlue.Any(id => Items.HasItem(id)))
            {
                return "s5_summonersmiteplayerganker";
            }
            if (SmiteRed.Any(id => Items.HasItem(id)))
            {
                return "s5_summonersmiteduel";
            }
            if (SmiteGrey.Any(id => Items.HasItem(id)))
            {
                return "s5_summonersmitequick";
            }
            if (SmitePurple.Any(id => Items.HasItem(id)))
            {
                return "itemsmiteaoe";
            }
            return "summonersmite";
        }

        public static void setSmiteSlot()
        {
            foreach (var spell in
                ObjectManager.Player.Spellbook.Spells.Where(
                    spell => String.Equals(spell.Name, smitetype(), StringComparison.CurrentCultureIgnoreCase)))
            {
                smiteSlot = spell.Slot;
                smite = new Spell(smiteSlot, smiteRange);
                return;
            }
        }
    }
}