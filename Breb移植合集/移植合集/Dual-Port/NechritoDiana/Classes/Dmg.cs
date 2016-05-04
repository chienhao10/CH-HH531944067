using ClipperLib;
using Color = System.Drawing.Color;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK;
using EloBuddy;
using Font = SharpDX.Direct3D9.Font;
using LeagueSharp.Common.Data;
using LeagueSharp.Common;

namespace Nechrito_Diana
{
    class Dmg
    {
        public static float SmiteDamage(AIHeroClient target)
        {
            if (Logic.Smite == SpellSlot.Unknown || Program.Player.Spellbook.CanUseSpell(Logic.Smite) != SpellState.Ready)
            {
                return 0f;
            }
            return (float)Program.Player.GetSummonerSpellDamage(target, LeagueSharp.Common.Damage.SummonerSpell.Smite);
        }
        public static float IgniteDamage(AIHeroClient target)
        {
            if (Spells.Ignite == SpellSlot.Unknown || Program.Player.Spellbook.CanUseSpell(Spells.Ignite) != SpellState.Ready)
            {
                return 0f;
            }
            return (float)Program.Player.GetSummonerSpellDamage(target, LeagueSharp.Common.Damage.SummonerSpell.Ignite);
        }
        public static float ComboDmg(Obj_AI_Base enemy)
        {
            if (enemy != null)
            {
                float damage = 0;
                
                if (Program.Player.Masteries.Equals("thunderlordsdecree")) damage += (float)Program.Player.GetAutoAttackDamage(enemy) * (1.05f);
                // dianapassivebuff or dianamoonlight, cba to actually check yet. Also showing too much dmg on dmg indicator, like 30% too much

                if (Program.Player.HasBuff("dianapassivebuff"))
                {
                    if (Spells._r.IsReady() && Spells._q.IsReady())
                        damage += Spells._q.GetDamage(enemy) + Spells._r.GetDamage(enemy) +
                            Spells._r.GetDamage(enemy) + (float)Program.Player.GetAutoAttackDamage(enemy);
                }
                damage = damage + (float)Program.Player.GetAutoAttackDamage(enemy);

                if (Spells._q.IsReady()) damage += Spells._q.GetDamage(enemy);
                if (Spells._w.IsReady()) damage += Spells._w.GetDamage(enemy);
                if (Spells._r.IsReady()) damage += Spells._r.GetDamage(enemy);
                return damage;
            }
            return 0;
        }
        public static bool IsLethal(Obj_AI_Base unit)
        {
            return ComboDmg(unit) / 1.65 >= unit.Health;
        }
    }
}
