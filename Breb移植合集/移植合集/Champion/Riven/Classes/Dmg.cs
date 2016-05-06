using EloBuddy;
using EloBuddy.SDK;
using LeagueSharp;
using LeagueSharp.Common;

namespace NechritoRiven
{
    class Dmg
    {
        public static float IgniteDamage(AIHeroClient target)
        {
            if (Spells.Ignite == SpellSlot.Unknown || Program.Player.Spellbook.CanUseSpell(Spells.Ignite) != SpellState.Ready)
            {
                return 0f;
            }
            return (float)Program.Player.GetSummonerSpellDamage(target, LeagueSharp.Common.Damage.SummonerSpell.Ignite);
        }
        public static double Basicdmg(Obj_AI_Base target)
        {
            if (target != null)
            {
                double dmg = 0;
                double passivenhan;
                if (Program.Player.Level >= 18)
                    passivenhan = 0.5;
                else if (Program.Player.Level >= 15)
                    passivenhan = 0.45;
                else if (Program.Player.Level >= 12)
                    passivenhan = 0.4;
                else if (Program.Player.Level >= 9)
                    passivenhan = 0.35;
                else if (Program.Player.Level >= 6)
                    passivenhan = 0.3;
                else if (Program.Player.Level >= 3)
                    passivenhan = 0.25;
                else
                    passivenhan = 0.2;
                if (Spells._w.IsReady()) dmg = dmg + Spells._w.GetDamage(target);
                if (Spells._q.IsReady())
                {
                    var qnhan = 4 - Program._qstack;
                    dmg = dmg + Spells._q.GetDamage(target) * qnhan + Program.Player.GetAutoAttackDamage(target) * qnhan * (1 + passivenhan);
                }
                dmg = dmg + Program.Player.GetAutoAttackDamage(target) * (1 + passivenhan);
                return dmg;
            }
            return 0;
        }


        public static float GetComboDamage(Obj_AI_Base enemy)
        {
            if (enemy != null)
            {
                float damage = 0;
                float passivenhan;
                if (Program.Player.Level >= 18)
                    passivenhan = 0.5f;
                else if (Program.Player.Level >= 15)
                    passivenhan = 0.45f;
                else if (Program.Player.Level >= 12)
                    passivenhan = 0.4f;
                else if (Program.Player.Level >= 9)
                    passivenhan = 0.35f;
                else if (Program.Player.Level >= 6)
                    passivenhan = 0.3f;
                else if (Program.Player.Level >= 3)
                    passivenhan = 0.25f;
                else
                    passivenhan = 0.2f;
                
                if (Spells._w.IsReady()) damage = damage + Spells._w.GetDamage(enemy);
                if (Spells._q.IsReady())
                {
                    var qnhan = 4 - Program._qstack;
                    damage = damage + Spells._q.GetDamage(enemy) * qnhan +
                             (float)Program.Player.GetAutoAttackDamage(enemy) * qnhan * (1 + passivenhan);
                }
                damage = damage + (float)Program.Player.GetAutoAttackDamage(enemy) * (1 + passivenhan);
                if (Spells._r.IsReady())
                {
                    return damage * 1.2f + Spells._r.GetDamage(enemy);
                }
                return damage;
            }
            return 0;
        }

        public static bool IsKillableR(AIHeroClient target)
        {
            return !target.IsInvulnerable && Totaldame(target) >= target.Health &&
                   Basicdmg(target) <= target.Health;
        }

        public static double Totaldame(Obj_AI_Base target)
        {
            if (target == null) return 0;
            double dmg = 0;
            double passivenhan;
            if (Program.Player.Level >= 18)
                passivenhan = 0.5;
            else if (Program.Player.Level >= 15)
                passivenhan = 0.45;
            else if (Program.Player.Level >= 12)
                passivenhan = 0.4;
            else if (Program.Player.Level >= 9)
                passivenhan = 0.35;
            else if (Program.Player.Level >= 6)
                passivenhan = 0.3;
            else if (Program.Player.Level >= 3)
                passivenhan = 0.25;
            else
                passivenhan = 0.2;
            
            if (Spells._w.IsReady()) dmg = dmg + Spells._w.GetDamage(target);
            if (Spells._q.IsReady())
            {
                var qnhan = 4 - Program._qstack;
                dmg = dmg + Spells._q.GetDamage(target) * qnhan + Program.Player.GetAutoAttackDamage(target) * qnhan * (1 + passivenhan);
            }
            dmg = dmg + Program.Player.GetAutoAttackDamage(target) * (1 + passivenhan);
            if (!Spells._r.IsReady()) return dmg;
            var rdmg = Rdame(target, target.Health - dmg * 1.2);
            return dmg * 1.2 + rdmg;
        }

        public static double Rdame(Obj_AI_Base target, double health)
        {
            if (target != null)
            {
                var missinghealth = (target.MaxHealth - health) / target.MaxHealth > 0.75 ? 0.75 : (target.MaxHealth - health) / target.MaxHealth;
                var pluspercent = missinghealth * 2;
                var rawdmg = new double[] { 80, 120, 160 }[Spells._r.Level - 1] + 0.6 * Program.Player.FlatPhysicalDamageMod;
                return Program.Player.CalcDamage(target, DamageType.Physical, rawdmg * (1 + pluspercent));
            }
            return 0;
        }
    }
}
