using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using GuTenTak.Tristana;

namespace GuTenTak.Tristana
{
    internal class DamageLib
    {
        private static readonly AIHeroClient _Player = ObjectManager.Player;
        public static float WCalc(Obj_AI_Base target)
        {
            return _Player.CalculateDamageOnUnit(target, DamageType.Physical,
                (float)(new[] { 0, 60, 110, 160, 210, 260 }[Program.W.Level] + 0.5f * _Player.FlatMagicDamageMod
                    ));

        }
        public static float ECalc(Obj_AI_Base target)
        {
            return _Player.CalculateDamageOnUnit(target, DamageType.Physical,
                (float)(new[] { 0, 60, 70, 80, 90, 100 }[Program.E.Level] + (new[] { 0, 0.5, 0.65, 0.8, 0.95, 1.1 }[Program.E.Level] * _Player.FlatPhysicalDamageMod + 0.5f * _Player.FlatMagicDamageMod
                    )));

        }
        public static float RCalc(Obj_AI_Base target)
        {
            return _Player.CalculateDamageOnUnit(target, DamageType.Physical,
                (float)(new[] { 0, 300, 400, 500 }[Program.R.Level] + 1.0f * _Player.FlatMagicDamageMod
                    ));

        }

        public static int EStacks(Obj_AI_Base target)
        {
            var buff = target.GetBuff("TristanaECharge");
            if (buff == null)
            {
                return 0;
            }
            else
            {
                return buff.Count;
            }
        }
        
        public static float DmgCalc(AIHeroClient target)
        {
            var damage = 0f;
            if (Program.E.IsReady() && target.IsValidTarget(2000))
                if (!target.HasBuff("TristanaECharge"))
                {
                    damage += ECalc(target);
                }
            if (target.HasBuff("TristanaECharge") && target.IsValidTarget(2000))
            {
               damage += ECalc(target) + ECalc(target) * (target.GetBuffCount("TristanaECharge") * 30f);
            }
            if (Program.R.IsReady() && target.IsValidTarget(Program.R.Range))
                damage += RCalc(target);
            return damage;
        }
        
        //E
        public static float ECharge(AIHeroClient target)
        {
            var damage = 0f;
            if (Program.E.IsReady() && target.IsValidTarget(Program.E.Range))
                if (!target.HasBuff("TristanaECharge"))
                {
                    damage += ECalc(target);
                }
                else
                {
                    damage += ECalc(target) + ECalc(target) * (target.GetBuffCount("TristanaECharge") * 30f);
                }
            //if (Program.E.IsReady() && target.IsValidTarget(Program.E.Range) && target.HasBuff("TristanaECharge"))
                //damage += ECalc(target) + ECalc(target) * (target.GetBuffCount("TristanaECharge") * 30f);
            damage += _Player.GetAutoAttackDamage(target, true) * 2;
            return damage;
        }
    }
}