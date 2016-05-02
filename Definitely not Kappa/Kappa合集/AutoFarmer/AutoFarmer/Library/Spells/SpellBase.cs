namespace Genesis.Library.Spells
{
    using System;
    using System.Collections.Generic;

    using EloBuddy;
    using EloBuddy.SDK;

    public abstract class SpellBase
    {
        public abstract Spell.SpellBase Q { get; set; }

        public abstract Spell.SpellBase W { get; set; }

        public abstract Spell.SpellBase E { get; set; }

        public abstract Spell.SpellBase R { get; set; }

        public Dictionary<string, object> Options;

        public Dictionary<string, Func<AIHeroClient, Obj_AI_Base, bool>> LogicDictionary;

        public bool QisCc = false;

        public bool QisDash = false;

        public bool QisToggle = false;

        public bool WisCc = false;

        public bool WisDash = false;

        public bool WisToggle = false;

        public bool EisCc = false;

        public bool EisDash = false;

        public bool EisToggle = false;

        public bool RisCc = false;

        public bool RisDash = false;

        public bool RisToggle = false;
    }
}