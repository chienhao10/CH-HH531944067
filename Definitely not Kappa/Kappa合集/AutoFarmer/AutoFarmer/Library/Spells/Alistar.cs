namespace Genesis.Library.Spells
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using EloBuddy;
    using EloBuddy.SDK;

    public class Alistar : SpellBase
    {
        public override sealed Spell.SpellBase Q { get; set; }

        public override sealed Spell.SpellBase W { get; set; }

        public override sealed Spell.SpellBase E { get; set; }

        public override sealed Spell.SpellBase R { get; set; }

        public Alistar()
        {
            this.Q = new Spell.Active(SpellSlot.Q, 315);
            this.W = new Spell.Targeted(SpellSlot.W, 625);
            this.E = new Spell.Active(SpellSlot.E);
            this.R = new Spell.Active(SpellSlot.R);
            this.QisCc = true;
            this.WisDash = true;
            this.WisCc = true;
            this.LogicDictionary = new Dictionary<string, Func<AIHeroClient, Obj_AI_Base, bool>>();
            this.LogicDictionary.Add("RLogic", RLogic);
        }

        public static bool RLogic(AIHeroClient player, object _)
        {
            if (player == null)
            {
                return false;
            }
            int x = EntityManager.Heroes.Enemies.Count(e => e.Distance(player) < 1000);
            if ((player.HasBuffOfType(BuffType.Fear) || player.HasBuffOfType(BuffType.Silence)
                 || player.HasBuffOfType(BuffType.Snare) || player.HasBuffOfType(BuffType.Stun)
                 || player.HasBuffOfType(BuffType.Charm) || player.HasBuffOfType(BuffType.Blind)
                 || player.HasBuffOfType(BuffType.Taunt)) || (x > 3))
            {
                return true;
            }
            return false;
        }
    }
}