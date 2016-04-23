using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using GuTenTak.Ezreal;

namespace GuTenTak.Ezreal
{
    //Thanks KEz
    internal class DamageLib
    {
        private static readonly AIHeroClient _Player = Player.Instance;
        public static float QCalc(Obj_AI_Base target)
        {
            return _Player.CalculateDamageOnUnit(target, DamageType.Physical,
                (float)(new[] { 0, 35, 55, 75, 95, 115 }[Program.Q.Level] + 1.1f * _Player.FlatPhysicalDamageMod + 0.4f * _Player.FlatMagicDamageMod
                    ));
        }
        public static float WCalc(Obj_AI_Base target)
        {
            return _Player.CalculateDamageOnUnit(target, DamageType.Physical,
                (float)(new[] { 0, 70, 115, 160, 205, 250 }[Program.W.Level] + 0.8f * _Player.FlatMagicDamageMod
                    ));
        }
        public static float ECalc(Obj_AI_Base target)
        {
            return _Player.CalculateDamageOnUnit(target, DamageType.Physical,
                (float)(new[] { 0, 75, 125, 175, 225, 275 }[Program.E.Level] + 0.5f * _Player.FlatPhysicalDamageMod + 0.75f * _Player.FlatMagicDamageMod
                    ));
        }
        public static float RCalc(Obj_AI_Base target)
        {
            return _Player.CalculateDamageOnUnit(target, DamageType.Physical,
                (float)(new[] { 0, 350, 500, 650 }[Program.R.Level] + 1.0f * _Player.FlatPhysicalDamageMod + 0.9f * _Player.FlatMagicDamageMod
                    ));
        }
        public static float DmgCalc(AIHeroClient target)
        {
            var damage = 0f;
            if (Program.Q.IsReady() && target.IsValidTarget(Program.Q.Range))
                damage += QCalc(target);
            if (Program.W.IsReady() && target.IsValidTarget(Program.W.Range))
                damage += WCalc(target);
            if (Program.E.IsReady() && target.IsValidTarget(Program.E.Range))
                damage += ECalc(target);
            if (Program.R.IsReady() && target.IsValidTarget(Program.R.Range))
                damage += RCalc(target);
            damage += _Player.GetAutoAttackDamage(target, true) * 2;
            return damage;
        }
    }
}