using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using GuTenTak.Sivir;

namespace GuTenTak.Sivir
{
    internal class DamageLib
    {
        private static readonly AIHeroClient _Player = ObjectManager.Player;
        public static float QCalc(Obj_AI_Base target)
        {
            return _Player.CalculateDamageOnUnit(target, DamageType.Physical,
                (float)(new[] { 0, 46.25, 83.25, 120.25, 159.1, 194.25 }[Program.Q.Level] + (new[] { 0, 1.295, 1.48, 1.665, 1.85, 2.35 }[Program.Q.Level] * _Player.FlatPhysicalDamageMod + 0.925f * _Player.FlatMagicDamageMod
                    )));
        }
        public static float DmgCalc(AIHeroClient target)
        {
            var damage = 0f;
            if (Program.Q.IsReady() && target.IsValidTarget(Program.Q.Range))
                damage += QCalc(target);

            damage += _Player.GetAutoAttackDamage(target, true) * 2;
            return damage;
        }
    }
}