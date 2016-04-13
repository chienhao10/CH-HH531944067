using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using GuTenTak.TwistedFate;

namespace GuTenTak.TwistedFate
{
    internal class DamageLib
    {
        private static readonly AIHeroClient _Player = ObjectManager.Player;
        public static float QCalc(Obj_AI_Base target)
        {
            return _Player.CalculateDamageOnUnit(target, DamageType.Physical,
                (float)(new[] { 0, 60, 105, 150, 195, 240 }[Program.Q.Level] + 0.6f * _Player.FlatMagicDamageMod
                    ));
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