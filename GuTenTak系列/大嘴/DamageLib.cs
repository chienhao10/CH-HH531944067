using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using GuTenTak.KogMaw;

namespace GuTenTak.KogMaw
{
    internal class DamageLib
    {
        private static readonly AIHeroClient _Player = ObjectManager.Player;
        public static float QCalc(Obj_AI_Base target)
        {
            return _Player.CalculateDamageOnUnit(target, DamageType.Physical,
                (float)(new[] { 0, 70, 115, 160, 205, 250 }[Program.Q.Level] + 0.5f * _Player.FlatPhysicalDamageMod + 0.5f * _Player.FlatMagicDamageMod
                    ));
        }
        public static float R1Calc(Obj_AI_Base target)
        {
            return _Player.CalculateDamageOnUnit(target, DamageType.Physical,
                (float)(new[] { 0, 70, 110, 150 }[Program.R.Level] + 0.65f * _Player.FlatPhysicalDamageMod + 0.25f * _Player.FlatMagicDamageMod
                    ));
        }
        public static float R2Calc(Obj_AI_Base target)
        {
            return _Player.CalculateDamageOnUnit(target, DamageType.Physical,
                (float)(new[] { 0, 140, 220, 300 }[Program.R.Level] + 1.3f * _Player.FlatPhysicalDamageMod + 0.5f * _Player.FlatMagicDamageMod
                    ));
        }
        public static float R3Calc(Obj_AI_Base target)
        {
            return _Player.CalculateDamageOnUnit(target, DamageType.Physical,
                (float)(new[] { 0, 210, 330, 450 }[Program.R.Level] + 1.95f * _Player.FlatPhysicalDamageMod + 0.75f * _Player.FlatMagicDamageMod
                    ));
        }
        public static float DmgCalc(AIHeroClient target)
        {
            var damage = 0f;
            if (Program.Q.IsReady() && target.IsValidTarget(Program.Q.Range))
                damage += QCalc(target);
            if (Program.R.IsReady() && target.IsValidTarget(Program.R.Range))
                damage += R1Calc(target);
            if (Program.R.IsReady() && target.IsValidTarget(Program.R.Range))
                damage += R2Calc(target);
            if (Program.R.IsReady() && target.IsValidTarget(Program.R.Range))
                damage += R3Calc(target);
            damage += _Player.GetAutoAttackDamage(target, true) * 2;
            return damage;
        }
    }
}