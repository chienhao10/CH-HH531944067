namespace Genesis.Library.Spells
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using EloBuddy;
    using EloBuddy.SDK;
    using EloBuddy.SDK.Enumerations;

    public class Annie : SpellBase
    {
        public override sealed Spell.SpellBase Q { get; set; }

        public override sealed Spell.SpellBase W { get; set; }

        public override sealed Spell.SpellBase E { get; set; }

        public override sealed Spell.SpellBase R { get; set; }

        public Annie()
        {
            this.Q = new Spell.Targeted(SpellSlot.Q, 625);
            this.W = new Spell.Skillshot(SpellSlot.W, 500, SkillShotType.Cone, 250, 100, 80);
            this.E = new Spell.Active(SpellSlot.E, 0);
            this.R = new Spell.Skillshot(SpellSlot.R, 600, SkillShotType.Circular, 250, 0, 290);
            this.LogicDictionary = new Dictionary<string, Func<AIHeroClient, Obj_AI_Base, bool>>();
            this.LogicDictionary.Add("RLogic", this.RLogic);
        }

        public bool RLogic(AIHeroClient player, Obj_AI_Base target)
        {
            if (player == null)
            {
                return false;
            }

            return EntityManager.Heroes.Enemies.Count(e => e.Distance(target) < 300) >= 1;
        }
    }
}