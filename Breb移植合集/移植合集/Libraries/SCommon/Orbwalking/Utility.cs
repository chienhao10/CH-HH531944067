using System;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using SCommon.Database;
using SharpDX;

namespace SCommon.Orbwalking
{
    public static class Utility
    {
        private static readonly string[] AttackResets =
        {
            "dariusnoxiantacticsonh", "fioraflurry", "garenq",
            "hecarimrapidslash", "jaxempowertwo", "jaycehypercharge", "leonashieldofdaybreak", "luciane", "lucianq",
            "lucianw", "gravesmove",
            "monkeykingdoubleattack", "mordekaisermaceofspades", "nasusq", "nautiluspiercinggaze", "netherblade",
            "gangplankqwrapper", "poppydevastatingblow", "powerfist", "renektonpreexecute", "rengarq",
            "shyvanadoubleattack",
            "sivirw", "takedown", "talonnoxiandiplomacy", "trundletrollsmash", "vaynetumble", "vie", "volibearq",
            "masochism",
            "xenzhaocombotarget", "yorickspectral", "reksaiq", "itemtitanichydracleave", "ricochet",
            "siphoningstrikenew", "garenslash3", "hecarimramp", "shyvanadoubleattackdragon",
            "riventricleave", "itemtiamatcleave"
        };

        private static readonly string[] NoAttacks =
        {
            "volleyattack", "volleyattackwithsound", "jarvanivcataclysmattack",
            "monkeykingdoubleattack", "shyvanadoubleattack",
            "shyvanadoubleattackdragon", "zyragraspingplantattack",
            "zyragraspingplantattack2", "zyragraspingplantattackfire",
            "zyragraspingplantattack2fire", "viktorpowertransfer",
            "sivirwattackbounce", "asheqattacknoonhit",
            "elisespiderlingbasicattack", "heimertyellowbasicattack",
            "heimertyellowbasicattack2", "heimertbluebasicattack",
            "annietibbersbasicattack", "annietibbersbasicattack2",
            "yorickdecayedghoulbasicattack", "yorickravenousghoulbasicattack",
            "yorickspectralghoulbasicattack", "malzaharvoidlingbasicattack",
            "malzaharvoidlingbasicattack2", "malzaharvoidlingbasicattack3",
            "kindredwolfbasicattack", "kindredbasicattackoverridelightbombfinal"
        };

        private static readonly string[] Attacks =
        {
            "caitlynheadshotmissile", "frostarrow", "garenslash2",
            "kennenmegaproc", "lucianpassiveattack", "masteryidoublestrike", "quinnwenhanced", "renektonexecute",
            "renektonsuperexecute", "rengarnewpassivebuffdash", "trundleq", "xenzhaothrust", "xenzhaothrust2",
            "xenzhaothrust3", "viktorqbuff"
        };

        private static readonly string[] NoCancelChamps = {"Kalista"};

        /// <summary>
        ///     Gets auto attack range of given unit
        /// </summary>
        /// <param name="_unit">Unit to get aa range</param>
        /// <returns>Auto attack range of unit</returns>
        public static float GetAARange(Obj_AI_Base _unit = null)
        {
            var unit = CorrectUnit(_unit);
            if (unit.CharData.BaseSkinName == "Azir")
                return 1000f;
            return unit.AttackRange + unit.BoundingRadius;
        }

        /// <summary>
        ///     Gets real auto attack range of given unit by target
        /// </summary>
        /// <param name="target">Target</param>
        /// <param name="_unit">Unit to get real aa range</param>
        /// <returns>Real auto attack range of unit by target</returns>
        public static float GetRealAARange(AttackableUnit target, Obj_AI_Base _unit = null)
        {
            return GetAARange(_unit) + target.BoundingRadius;
        }

        /// <summary>
        ///     Checks if target is in unit's auto attack range
        /// </summary>
        /// <param name="target">Target</param>
        /// <param name="_unit">Unit to check range</param>
        /// <returns>true if in auto attack range</returns>
        public static bool InAARange(AttackableUnit target, Obj_AI_Base _unit = null)
        {
            if (!target.IsValidTarget())
                return false;

            var unit = CorrectUnit(_unit);
            var range = GetRealAARange(target, _unit);
            return Vector2.DistanceSquared(target.Position.To2D(), unit.ServerPosition.To2D()) <= range*range;
        }

        /// <summary>
        ///     Gets projectile speed of given unit
        /// </summary>
        /// <param name="_unit">Unit to get projectile speed</param>
        /// <returns>Projectile speed of unit</returns>
        public static float GetProjectileSpeed(Obj_AI_Base _unit = null)
        {
            var unit = CorrectUnit(_unit);
            return IsMelee(unit) || unit.CharData.BaseSkinName == "Azir" || unit.CharData.BaseSkinName == "Velkoz" ||
                   (unit.CharData.BaseSkinName == "Viktor" && unit.HasBuff("ViktorPowerTransferReturn"))
                ? float.MaxValue
                : unit.BasicAttack.MissileSpeed;
        }

        /// <summary>
        ///     Checks if the unit has aa projectile
        /// </summary>
        /// <param name="_unit"></param>
        /// <returns></returns>
        public static bool HasProjectile(Obj_AI_Base _unit = null)
        {
            var unit = CorrectUnit(_unit);
            return
                !(IsMelee(unit) || unit.CharData.BaseSkinName == "Azir" || unit.CharData.BaseSkinName == "Velkoz" ||
                  (unit.CharData.BaseSkinName == "Viktor" && unit.HasBuff("ViktorPowerTransferReturn")));
        }

        /// <summary>
        ///     Checks if given unit is melee
        /// </summary>
        /// <param name="_unit">Unit to check</param>
        /// <returns>true if unit is melee</returns>
        public static bool IsMelee(Obj_AI_Base _unit)
        {
            return _unit.CombatType == GameObjectCombatType.Melee;
        }

        /// <summary>
        ///     Checks if given spell name is an aa reset
        /// </summary>
        /// <param name="name">Spell name</param>
        /// <returns>true if spell is an aa reset</returns>
        public static bool IsAutoAttackReset(string name)
        {
            return AttackResets.Contains(name.ToLower());
        }

        /// <summary>
        ///     Checks if given spell name is an auto attack
        /// </summary>
        /// <param name="name">Spell name</param>
        /// <returns>true if spell is an auto attack</returns>
        public static bool IsAutoAttack(string name)
        {
            return (name.ToLower().Contains("attack") && !NoAttacks.Contains(name.ToLower())) ||
                   Attacks.Contains(name.ToLower());
        }

        /// <summary>
        ///     Checks if given minion is cannon minion
        /// </summary>
        /// <param name="minion">Minion</param>
        /// <returns></returns>
        public static bool IsCannonMinion(this Obj_AI_Base minion)
        {
            return minion.CharData.BaseSkinName.Contains("MinionSiege");
        }

        /// <summary>
        ///     Gets unit's attack speed
        /// </summary>
        /// <param name="unit">Unit</param>
        /// <returns></returns>
        public static float GetAttackSpeed(this Obj_AI_Base unit)
        {
            return 1/unit.AttackDelay;
        }

        /// <summary>
        ///     Gets the unit's scaling range
        /// </summary>
        /// <param name="unit">The unit.</param>
        /// <returns></returns>
        public static float GetScalingRange(this AIHeroClient unit)
        {
            return Math.Max(0, unit.BBox.Minimum.Distance(unit.BBox.Maximum) - unit.GetOrginalHitBox())/2f;
        }

        /// <summary>
        ///     Checks if the hero is non cancel champ
        /// </summary>
        /// <param name="name">The hero's name.</param>
        /// <returns></returns>
        public static bool IsNonCancelChamp(string name)
        {
            return NoCancelChamps.Contains(name);
        }

        private static Obj_AI_Base CorrectUnit(Obj_AI_Base _unit)
        {
            return _unit == null ? ObjectManager.Player : _unit;
        }
    }
}