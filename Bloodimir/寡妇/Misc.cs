using EloBuddy;
using EloBuddy.SDK;

namespace Evelynn
{
    internal class Misc
    {
        private static AIHeroClient Evelynn
        {
            get { return ObjectManager.Player; }
        }

        public static float Qcalc(Obj_AI_Base target)
        {
            return Evelynn.CalculateDamageOnUnit(target, DamageType.Magical,
                new float[] {0, 40, 50, 60, 70, 80}[Program.Q.Level] +
                new float[] {0, 35, 40, 45, 50, 55}[Program.Q.Level]/100*Player.Instance.FlatMagicDamageMod +
                new float[] {0, 50, 55, 60, 65, 70}[Program.Q.Level]/100*Player.Instance.FlatPhysicalDamageMod);
        }

        public static float Ecalc(Obj_AI_Base target)
        {
            return Evelynn.CalculateDamageOnUnit(target, DamageType.Magical,
                (new float[] {0, 70, 110, 150, 190, 230}[Program.E.Level] +
                 (1.0f*Evelynn.FlatMagicDamageMod + 1.0f*Evelynn.FlatPhysicalDamageMod)));
        }
    }
}