using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using SharpDX;
using EloBuddy.SDK.Rendering;
using Color1 = System.Drawing.Color;

namespace SmiteGH
{
    public class GetDamages
    {
        // Nunu'S Q Spell damage.
        public static double Nunu(int SpellLevel)
        {
            return new double[] { 400, 550, 700, 850, 1000 }[SpellLevel - 1];
        }

        // Cho'Goth's R Spell damage.
        public static double ChoGath()
        {
            return 1000;// Stupid ha? XD
        }

        // Shaco's E Spell damage.
        public static double Shaco(int SpellLevel)
        {
            return new double[] { 50, 90, 130, 170, 210 }[SpellLevel - 1]
                + 1 * ObjectManager.Player.FlatPhysicalDamageMod + 1 * ObjectManager.Player.FlatMagicDamageMod;
        }

        // Vi's E Spell damage.
        public static double Vi(int SpellLevel)
        {
            return new double[] { 5, 20, 35, 50, 65 }[SpellLevel - 1]
                + 1.15 * (ObjectManager.Player.BaseAttackDamage + ObjectManager.Player.FlatPhysicalDamageMod)
                + 0.7 * ObjectManager.Player.FlatMagicDamageMod;
        }

        //Master's Q Spell damage.
        public static double Master(int SpellLevel)
        {
            return new double[] { 25, 60, 95, 130, 165 }[SpellLevel - 1]
                 + 1 * (ObjectManager.Player.BaseAttackDamage + ObjectManager.Player.FlatPhysicalDamageMod)
                 + 0.6 * (ObjectManager.Player.BaseAttackDamage + ObjectManager.Player.FlatPhysicalDamageMod);
        }

        //Rengar's Q Spell damage.
        public static double Rengar(int SpellLevel)
        {
            return new double[] { 30, 60, 90, 120, 150 }[SpellLevel - 1]
                 + new double[] { 0, 5, 10, 15, 20 }[SpellLevel] / 100
                 * (ObjectManager.Player.BaseAttackDamage + ObjectManager.Player.FlatPhysicalDamageMod);
        }

        //Nasus's Q Spell damage.
        public static double Nasus(int SpellLevel)
        {
            return (from buff in ObjectManager.Player.Buffs
                    where buff.Name == "nasusqstacks"
                    select buff.Count).FirstOrDefault()
                 + new double[] { 30, 50, 70, 90, 110 }[SpellLevel - 1];
        }

        //Khazix's Q Spell damage.
        public static double Khazix(int SpellLevel)
        {
            return new double[] { 70, 95, 120, 145, 170 }[SpellLevel - 1]
                 + 1.2 * ObjectManager.Player.FlatPhysicalDamageMod;
        }

        //Fizz's Q Spell damage.
        public static double Fizz(int SpellLevel)
        {
            return new double[] { 10, 25, 40, 55, 70 }[SpellLevel - 1]
                 + 0.35 * ObjectManager.Player.FlatMagicDamageMod;
        }

        //Elise's Q Spell damage.
        public static double Elise(int SpellLevel, Obj_AI_Base Monster)
        {
            return new double[] { 40, 75, 110, 145, 180 }[SpellLevel - 1]
                 + (0.08 + 0.03 / 100 * ObjectManager.Player.FlatMagicDamageMod) * Monster.Health;
        }

        //Lee Sin's Checks and Damage Q!
        #region Lee Sin Checks..
        public static bool HasQBuff(Obj_AI_Base unit)
        {
            return (HasBuffs(unit, "BlindMonkQOne") || HasBuffs(unit, "blindmonkqonechaos"));
        }

        public static bool HasBuffs(Obj_AI_Base unit, string s)
        {
            return
                unit.Buffs.Any(
                    a => a.Name.ToLower().Contains(s.ToLower()) || a.DisplayName.ToLower().Contains(s.ToLower()));
        }

        public static Obj_AI_Base BuffedEnemy
        {
            get { return ObjectManager.Get<Obj_AI_Base>().FirstOrDefault(unit => unit.IsEnemy && HasQBuff(unit)); }
        }

        public static double Q2Damage(Obj_AI_Base target, int SpellLevel, float subHp = 0, bool monster = true)
        {
            var damage = (50 + (SpellLevel * 30)) + (0.09 * ObjectManager.Player.FlatPhysicalDamageMod) +
                         ((target.MaxHealth - (target.Health - subHp)) * 0.08);
            if (monster && damage > 400)
            {
                return ObjectManager.Player.CalculateDamageOnUnit(target, DamageType.Physical, 400);
            }
            return ObjectManager.Player.CalculateDamageOnUnit(target, DamageType.Physical, (float)damage);
        }

        public static double QDamage(Obj_AI_Base target, int SpellLevel)
        {
            return ObjectManager.Player.CalculateDamageOnUnit(target, DamageType.Physical,
                (float)(new double[] { 50, 80, 110, 140, 170 }[SpellLevel] + 0.9 * ObjectManager.Player.FlatPhysicalDamageMod));
        }
        #endregion

        //Warwick's Q Spell damage.
        public static double Warwick(int SpellLevel, Obj_AI_Base Monster)
        {
            return Math.Max(new double[] { 75, 125, 175, 225, 275 }[SpellLevel - 1],
                            new double[] { 8, 10, 12, 14, 16 }[SpellLevel - 1] / 100 * Monster.MaxHealth)
                            + 1 * ObjectManager.Player.FlatMagicDamageMod;
        }

        //Volibear's W Spell damage.
        public static double Volibear(int SpellLevel, Obj_AI_Base Monster)
        {
            return (new double[] { 80, 125, 170, 215, 260 }[SpellLevel - 1])
                                    * ((Monster.MaxHealth - Monster.Health) / Monster.MaxHealth + 1);
        }

        //Olaf's E Spell damage.
        public static double Olaf(int SpellLevel)
        {
            return new double[] { 70, 115, 160, 205, 250 }[SpellLevel - 1]
                            + 0.4 * (ObjectManager.Player.BaseAttackDamage + ObjectManager.Player.FlatPhysicalDamageMod);
        }

        //Pantheon's Q Spell damage.
        public static double Pantheon(int SpellLevel, Obj_AI_Base Monster)
        {
            return (new double[] { 65, 105, 145, 185, 225 }[SpellLevel - 1]
                                     + 1.4 * ObjectManager.Player.FlatPhysicalDamageMod)
                             * ((Monster.Health / Monster.MaxHealth < 0.15) ? 2 : 1);
        }

        //MonkeyKing's Q Spell damage.
        public static double MonkeyKing(int SpellLevel)
        {
            return new double[] { 30, 60, 90, 120, 150 }[SpellLevel - 1]
                             + 0.1 * (ObjectManager.Player.BaseAttackDamage + ObjectManager.Player.FlatPhysicalDamageMod);
        }

        //Hecarim's Q Spell damage.
        public static double Hecarim(int SpellLevel)
        {
            return new double[] { 60, 95, 130, 165, 200 }[SpellLevel - 1]
                                    + 0.6 * ObjectManager.Player.FlatPhysicalDamageMod;
        }

        //Amumu's Q Spell damage.
        public static double Amumu(int SpellLevel)
        {
            return new double[] { 80, 130, 180, 230, 280 }[SpellLevel - 1]
                              + 0.7 * ObjectManager.Player.FlatMagicDamageMod;
        }

        //Irelia's Q Spell damage.
        public static double Irelia(int SpellLevel)
        {
            return new double[] { 20, 50, 80, 110, 140 }[SpellLevel - 1]
                             + 1 * (ObjectManager.Player.BaseAttackDamage + ObjectManager.Player.FlatPhysicalDamageMod);
        }

        //Evelynn's E Spell damage.
        public static double Evelynn(int SpellLevel)
        {
            return new double[] { 70, 110, 150, 190, 230 }[SpellLevel - 1]
                              + 1 * ObjectManager.Player.FlatMagicDamageMod + 1 * ObjectManager.Player.FlatPhysicalDamageMod;
        }

        //Diana's Q Spell damage.
        public static double DianaQ(int SpellLevel)
        {
            return new double[] { 60, 95, 130, 165, 200 }[SpellLevel - 1]
                             + 0.7 * ObjectManager.Player.FlatMagicDamageMod;
        }

        public static double DianaR(int SpellLevel)
        {
            return new double[] { 100, 160, 220 }[SpellLevel - 1]
                             + 0.6 * ObjectManager.Player.FlatMagicDamageMod;
        }
    }
}
