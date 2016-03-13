using EloBuddy;
using EloBuddy.SDK;
using System.Linq;


namespace KKayle
{
    internal class DamageLib
    {


        private static readonly AIHeroClient _Player = ObjectManager.Player;
        public static float QCalc(Obj_AI_Base target)
        {
            return _Player.CalculateDamageOnUnit(target, DamageType.Magical,
                (float)(new[] { 0, 60, 110, 160, 210, 260 }[Program.Q.Level] + 0.6f * _Player.FlatMagicDamageMod + 0.99f * _Player.FlatPhysicalDamageMod
                    ));
        }


        public static float ECalc(Obj_AI_Base target)
        {
            return _Player.CalculateDamageOnUnit(target, DamageType.Magical,
                (float)(new[] { 0, 20, 30, 40, 50, 60 }[Program.E.Level] + 0.3f * _Player.FlatMagicDamageMod + 0.3f * _Player.FlatPhysicalDamageMod
                    ));
        }

        public static float DmgCalc(AIHeroClient target)
        {
            var damage = 0f;
            if (Program.Q.IsReady() && target.IsValidTarget(Program.Q.Range))
                damage += QCalc(target);
           if (Program.E.IsReady() && target.IsValidTarget(Program.E.Range))
                damage += ECalc(target) + _Player.GetAutoAttackDamage(target);
            damage += _Player.GetAutoAttackDamage(target, true) * 2;
            return damage;
        }







     }
}
