using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using Color = System.Drawing.Color;
using EloBuddy;
using EloBuddy.SDK.Menu.Values;

namespace BadaoKingdom.BadaoChampion.BadaoPoppy
{
    public static class BadaoPoppyHelper
    {
        // can use skills
        public static bool UseQCombo()
        {
            return BadaoMainVariables.Q.IsReady() && BadaoPoppyVariables.ComboQ;
        }
        public static bool UseWCombo()
        {
            return BadaoMainVariables.W.IsReady() && BadaoPoppyVariables.ComboW;
        }
        public static bool UseEComboGap()
        {
            return BadaoMainVariables.E.IsReady() && BadaoPoppyVariables.ComboE;
        }
        public static bool UseECombo(AIHeroClient target)
        {
            return BadaoMainVariables.E.IsReady() && BadaoPoppyConfig.Combo["ComboE" + target.NetworkId].Cast<CheckBox>().CurrentValue;
        }
        public static bool UseRComboKillable()
        {
            return BadaoMainVariables.R.IsReady() && BadaoPoppyVariables.ComboRKillable;
        }
        public static bool UseQHarass()
        {
            return BadaoMainVariables.Q.IsReady() && BadaoPoppyVariables.HarassQ;
        }
        public static bool UseQJungle()
        {
            return BadaoMainVariables.Q.IsReady() && BadaoPoppyVariables.JungleQ;
        }
        public static bool UseEJungle()
        {
            return BadaoMainVariables.E.IsReady() && BadaoPoppyVariables.JungleE;
        }
        public static bool ManaJungle()
        {
            return BadaoPoppyVariables.JungleMana <= ObjectManager.Player.Mana * 100 / ObjectManager.Player.MaxMana;
        }
        public static bool AssasinateActive()
        {
            return BadaoPoppyVariables.AssassinateKey;
        }
        public static bool UseEAutoInterrupt()
        {
            return BadaoMainVariables.E.IsReady() && BadaoPoppyVariables.AutoEInterrupt;
        }
        public static bool UseRAutoKS()
        {
            return BadaoMainVariables.R.IsReady() && BadaoPoppyVariables.AutoRKS;
        }
        public static bool UseRAutoInterrupt()
        {
            return BadaoMainVariables.R.IsReady() && BadaoPoppyVariables.AutoRInterrupt;
        }
        public static bool UseRAuto3Target()
        {
            return BadaoMainVariables.R.IsReady() && BadaoPoppyVariables.AutoR3Target;
        }
        public static bool UseWAutoAntiDash(AIHeroClient target)
        {
            return BadaoMainVariables.W.IsReady() && BadaoPoppyConfig.Auto["AutoAntiDash" + target.NetworkId].Cast<CheckBox>().CurrentValue;
        }
    }
}
