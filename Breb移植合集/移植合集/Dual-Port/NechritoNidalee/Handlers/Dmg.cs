
using EloBuddy;
using LeagueSharp;
using LeagueSharp.Common;
using Nechrito_Nidalee.Extras;

namespace Nechrito_Nidalee.Handlers
{
    class Dmg : Core
    {
        public static float IgniteDamage(AIHeroClient target)
        {
            if (Champion.Ignite == SpellSlot.Unknown || Player.Spellbook.CanUseSpell(Champion.Ignite) != SpellState.Ready)
            {
                return 0f;
            }
            return (float)Player.GetSummonerSpellDamage(target, Damage.SummonerSpell.Ignite);
        }
        public static float SmiteDamage(AIHeroClient target)
        {
            if (Item.Smite == SpellSlot.Unknown || Player.Spellbook.CanUseSpell(Item.Smite) != SpellState.Ready)
            {
                return 0f;
            }
            return (float)Player.GetSummonerSpellDamage(target, Damage.SummonerSpell.Smite);
        }

        public static float ComboDmg(Obj_AI_Base enemy)
        {
            if(enemy != null)
            {
                float dmg = 0;
                    dmg = dmg + (float)Player.LSGetAutoAttackDamage(enemy);

                    if (Champion.Pounce.IsReady()) dmg += Champion.Pounce.GetDamage(enemy);
                    if (Champion.Swipe.IsReady()) dmg += Champion.Swipe.GetDamage(enemy);
                    if (Champion.Takedown.IsReady()) dmg += Champion.Takedown.GetDamage(enemy);

                    if (Champion.Bushwack.IsReady()) dmg += Champion.Bushwack.GetDamage(enemy);
                    if (Champion.Javelin.IsReady()) dmg += Champion.Javelin.GetDamage(enemy);
              
                    return dmg;
            }
            return 0;
        }
        public static bool IsLethal(Obj_AI_Base unit)
        {
            return ComboDmg(unit) / 1.65 >= unit.Health;
        }
    }
}
