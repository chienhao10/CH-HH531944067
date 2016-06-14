using System;
using System.Linq;
using ExorSDK.Utilities;
using LeagueSharp;
using LeagueSharp.Data.Enumerations;
using LeagueSharp.SDK.Core.Utils;
using EloBuddy;
using LeagueSharp.Common;
using EloBuddy.SDK;
using System.Collections.Generic;

namespace ExorSDK.Champions.Darius
{
    /// <summary>
    ///     The logics class.
    /// </summary>
    internal partial class Logics
    {
        internal static Dictionary<string, LeagueSharp.Common.Spell> Spellbook = new Dictionary<string, LeagueSharp.Common.Spell>
        {
            { "Q", new LeagueSharp.Common.Spell(SpellSlot.Q, 425f) },
            { "W", new LeagueSharp.Common.Spell(SpellSlot.W, 200f) },
            { "E", new LeagueSharp.Common.Spell(SpellSlot.E, 490f) },
            { "R", new LeagueSharp.Common.Spell(SpellSlot.R, 460f) }
        };
        /// <summary>
        ///     Called when the game updates itself.
        /// </summary>
        /// <param name="args">The <see cref="EventArgs" /> instance containing the event data.</param>
        public static void Killsteal(EventArgs args)
        {
            /// <summary>
            ///     The KillSteal R Logic.
            /// </summary>
            /// 
            var target = TargetSelector.GetTarget(Vars.R.Range, DamageType.True);
            if (target == null || !target.IsValid)
            {
                return;
            }
            if (Vars.getCheckBoxItem(Vars.RMenu, "killsteal") && Vars.R.IsReady() && target.IsValidTarget(Vars.R.Range))
            {
                foreach (var hero in
                    ObjectManager.Get<AIHeroClient>().Where(hero => hero.LSIsValidTarget(Vars.R.Range) && !Invulnerable.Check(hero) && !hero.HasBuffOfType(BuffType.SpellShield) && !hero.HasBuff("kindredrnodeathbuff")))
                {
                    var R = new LeagueSharp.Common.Spell(SpellSlot.R, 460);
                    var dmgR = SebbyLib.OktwCommon.GetKsDamage(target, R);
                    if (target.HasBuff("dariushemo"))
                        dmgR += R.GetDamage(target) * target.GetBuff("dariushemo").Count * 0.2f;
                    if (dmgR > hero.Health + target.HPRegenRate)
                    {
                        Vars.R.CastOnUnit(target);
                    }

                    if (dmgR < hero.Health + target.HPRegenRate && hero.CountEnemiesInRange(1200) <= 1)
                    {
                        foreach (var buff in hero.Buffs.Where(buff => buff.Name == "dariushemo"))
                        {
                            if (ObjectManager.Player.LSGetSpellDamage(target, SpellSlot.R, 1) * (1 + buff.Count / 5) - 1
                                > target.Health + target.HPRegenRate)
                            {
                                Vars.R.CastOnUnit(target);
                            }
                        }
                    }
                    if (hero.CountEnemiesInRange(1200) <= 1)
                    {
                        if (RDmg(hero, PassiveCount(hero)) +
                            Hemorrhage(hero, PassiveCount(hero) - 1) >= hero.Health + target.HPRegenRate && 1 <= target.GetBuff("dariushemo").Count)
                        {
                                if (!hero.HasBuff("kindredrnodeathbuff"))
                                    Spellbook["R"].CastOnUnit(hero);
                        }
                    }
                    if (RDmg(hero, PassiveCount(hero)) >= hero.Health +
                        Hemorrhage(hero, 1))
                    {
                            if (!hero.HasBuff("kindredrnodeathbuff"))
                                Spellbook["R"].CastOnUnit(hero);
                    }
                }
            }
        }
        internal static float RDmg(Obj_AI_Base unit, int stackcount)
        {
            var bonus = (new[] { 20, 20, 40, 60 }[Spellbook["R"].Level] +
                            (0.25 * ObjectManager.Player.FlatPhysicalDamageMod) * stackcount);
            return
                (float)(bonus + (ObjectManager.Player.CalcDamage(unit, DamageType.True,
                        new[] { 100, 100, 200, 300 }[Spellbook["R"].Level] + (0.75 * ObjectManager.Player.FlatPhysicalDamageMod))));
        }
        internal static float Hemorrhage(Obj_AI_Base unit, int stackcount)
        {
            if (stackcount < 1)
                stackcount = 1;

            return
                (float)
                    ObjectManager.Player.CalcDamage(unit, DamageType.Physical,
                        (9 + ObjectManager.Player.Level) + (0.3 * ObjectManager.Player.FlatPhysicalDamageMod)) * stackcount;
        }
        internal static int PassiveCount(Obj_AI_Base unit)
        {
            return unit.GetBuffCount("dariushemo") > 0 ? unit.GetBuffCount("dariushemo") : 0;
        }
    }
}
