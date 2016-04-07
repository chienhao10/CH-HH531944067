using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using GuTenTak.Corki;

namespace GuTenTak.Corki
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
        public static float RCalc(Obj_AI_Base target)
        {
            return _Player.CalculateDamageOnUnit(target, DamageType.Physical,
                (float)(new[] { 0, 100, 130, 160 }[Program.R.Level] + (float)(new[] { 0, 20, 50, 80 }[Program.R.Level] * _Player.FlatPhysicalDamageMod + 0.3f * _Player.FlatMagicDamageMod
                    )));
        }
        public static float DmgCalc(AIHeroClient target)
        {
            var damage = 0f;
            if (Program.Q.IsReady() && target.IsValidTarget(Program.Q.Range))
                damage += QCalc(target);
            if (Program.R.IsReady() && target.IsValidTarget(Program.R.Range))
                damage += RCalc(target);
            damage += _Player.GetAutoAttackDamage(target, true) * 2;
            return damage;
        }
    }
}