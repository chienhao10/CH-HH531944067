using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using GuTenTak.Lucian;

namespace GuTenTak.Lucian
{
    internal class DamageLib
    {
        private static readonly AIHeroClient _Player = ObjectManager.Player;
        public static float QCalc(Obj_AI_Base target)
        {
            return _Player.CalculateDamageOnUnit(target, DamageType.Physical,
                (float)(new[] { 0, 80, 110, 140, 170, 200 }[Program.Q.Level] + (new[] { 0, 0.6, 0.75, 0.9, 1.05, 1.2 }[Program.Q.Level] * _Player.FlatPhysicalDamageMod
                    )));
        }
        public static float WCalc(Obj_AI_Base target)
        {
            return _Player.CalculateDamageOnUnit(target, DamageType.Physical,
                (float)(new[] { 0, 60, 100, 140, 180, 220 }[Program.W.Level] + 0.9f * _Player.FlatMagicDamageMod
                    ));
        }
        public static float RCalc(Obj_AI_Base target)
        {
            return _Player.CalculateDamageOnUnit(target, DamageType.Physical,
                (float)(new[] { 0, 20, 35, 50 }[Program.R.Level] + 0.2f * _Player.FlatPhysicalDamageMod + 0.1f * _Player.FlatMagicDamageMod
                    ));
        }
 
        public static float DmgCalc(AIHeroClient target)
        {
            var damage = 0f;
            if (Program.Q.IsReady() && target.IsValidTarget(Program.Q.Range))
                damage += QCalc(target);
            if (Program.R.IsReady() && target.IsValidTarget(Program.R.Range))
                damage += RCalc(target);
            if (Program.R.IsReady() && target.IsValidTarget(Program.R.Range))

            damage += _Player.GetAutoAttackDamage(target, true) * 2;
            return damage;
        }
    }
}