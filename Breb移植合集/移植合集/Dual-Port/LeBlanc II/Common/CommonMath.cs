using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using EloBuddy;

namespace Leblanc.Common
{
    internal class CommonMath
    {

        public static float GetComboDamage(Obj_AI_Base t)
        {
            var fComboDamage = 0d;

            if (Champion.PlayerSpells.Q.IsReady())
            {
                fComboDamage += ObjectManager.Player.LSGetSpellDamage(t, SpellSlot.Q);
            }


            //if (ObjectManager.Player.Health >= 20 && ObjectManager.Player.Health <= 50)
            //{
            //    fComboDamage += ObjectManager.Player.TotalAttackDamage*3;
            //}

            //if (ObjectManager.Player.Health > 50)
            //{
            //    fComboDamage += ObjectManager.Player.TotalAttackDamage * 7;
            //}

            if (Champion.PlayerSpells.E.IsReady())
            {
                fComboDamage += ObjectManager.Player.LSGetSpellDamage(t, SpellSlot.E);
            }

            if (Champion.PlayerSpells.R.IsReady())
            {
                fComboDamage += ObjectManager.Player.LSGetSpellDamage(t, SpellSlot.R) * 4;
            }

            if (t.LSIsValidTarget(Champion.PlayerSpells.Q.Range + Champion.PlayerSpells.E.Range) && Champion.PlayerSpells.Q.IsReady() && Champion.PlayerSpells.R.IsReady())
            {
                fComboDamage += ObjectManager.Player.TotalAttackDamage * 2;
            }

            fComboDamage += ObjectManager.Player.TotalAttackDamage * 2;

            if (CommonItems.Youmuu.IsReady())
            {
                fComboDamage += ObjectManager.Player.TotalAttackDamage * 3;
            }

            if (Common.CommonSummoner.IgniteSlot != SpellSlot.Unknown
                && ObjectManager.Player.Spellbook.CanUseSpell(Common.CommonSummoner.IgniteSlot) == SpellState.Ready)
            {
                fComboDamage += ObjectManager.Player.GetSummonerSpellDamage(t, Damage.SummonerSpell.Ignite);
            }

            if (LeagueSharp.Common.Items.CanUseItem(3128))
            {
                fComboDamage += ObjectManager.Player.GetItemDamage(t, Damage.DamageItems.Botrk);
            }

            return (float)fComboDamage;
        }

    }
}
