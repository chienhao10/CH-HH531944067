#region Credits

//=====================================================================
//+ Massive thanks to the entire community of EB for making this
//+ Spell library possible. Special thanks to: Coman3, MarioGK, 
//+ KarmaPanda, Bloodimir, Hellsing, iRaxe, plebsot, Chaos, 
//+ zpitty and many others!
//+
//+ This spell database was last updated 2/29/2016
//======================================================================

#endregion

namespace Genesis.Library.Spells
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using EloBuddy;
    using EloBuddy.SDK;
    using EloBuddy.SDK.Enumerations;

    public class Aatrox : SpellBase // Quality Tested, Genesis Approved
    {
        public override sealed Spell.SpellBase Q { get; set; }

        public override sealed Spell.SpellBase W { get; set; }

        public override sealed Spell.SpellBase E { get; set; }

        public override sealed Spell.SpellBase R { get; set; }

        public Aatrox()
        {
            this.Q = new Spell.Skillshot(SpellSlot.Q, 650, SkillShotType.Circular, 250, 450, 285)
                         { AllowedCollisionCount = int.MaxValue };
            this.W = new Spell.Active(SpellSlot.W);
            this.E = new Spell.Skillshot(SpellSlot.E, 1000, SkillShotType.Linear, 250, 1200, 100)
                         { AllowedCollisionCount = int.MaxValue };
            this.R = new Spell.Active(SpellSlot.R);
            this.QisCc = true;
            this.QisDash = true;
            this.WisToggle = true;
            this.EisCc = true;
            this.LogicDictionary = new Dictionary<string, Func<AIHeroClient, Obj_AI_Base, bool>>();
            this.LogicDictionary.Add("RLogic", RLogic);
        }

        public static bool RLogic(AIHeroClient player, object _)
        {
            if (player == null)
            {
                return false;
            }
            return EntityManager.Heroes.Enemies.Count(e => e.Distance(player) < 500) >= 1;
        }
    }

    /*public class Elise : SpellBase
    {
        public sealed override Spell.SpellBase Q { get; set; }
        public sealed override Spell.SpellBase W { get; set; }
        public sealed override Spell.SpellBase E { get; set; }
        public sealed override Spell.SpellBase R { get; set; }
        public Elise()
        {
            Q = new Spell.Targeted(SpellSlot.Q, 625);
            W = new Spell.Skillshot(SpellSlot.W, 950, SkillShotType.Circular);
            E = new Spell.Skillshot(SpellSlot.E, 1075, SkillShotType.Linear, 250, 1600, 80) { AllowedCollisionCount = 0 };
            R = new Spell.Active(SpellSlot.R); // TODO: Support Elise
        }
    }*/

    /*public class Gnar : SpellBase 
    {
        public sealed override Spell.SpellBase Q { get; set; }
        public sealed override Spell.SpellBase W { get; set; }
        public sealed override Spell.SpellBase E { get; set; }
        public sealed override Spell.SpellBase R { get; set; }
        public Gnar()
        {
            Q = new Spell.Skillshot(SpellSlot.Q, 1100, SkillShotType.Linear, 250, 1200, 55);
            W = new Spell.Active(SpellSlot.W);
            E = new Spell.Skillshot(SpellSlot.E, 475, SkillShotType.Circular, 500, int.MaxValue, 150);
            R = new Spell.Active(SpellSlot.R);
        }
    }*/ // TODO: Same boat as Elise

    /*public class Jayce : SpellBase //todo
    {
        public sealed override Spell.SpellBase Q { get; set; }
        public sealed override Spell.SpellBase W { get; set; }
        public sealed override Spell.SpellBase E { get; set; }
        public sealed override Spell.SpellBase R { get; set; }
        public Jayce()
        {
            Q = new Spell.Targeted(SpellSlot.Q, 600);
            W = new Spell.Skillshot(SpellSlot.W, 700, SkillShotType.Circular);
            E = new Spell.Active(SpellSlot.E, 325);
            R = new Spell.Targeted(SpellSlot.R, 700);
        }
    } // TODO: FUCK THERE ARE 3 OF YOU?!
    public class Jhin : SpellBase // todo
    {
        public sealed override Spell.SpellBase Q { get; set; }
        public sealed override Spell.SpellBase W { get; set; }
        public sealed override Spell.SpellBase E { get; set; }
        public sealed override Spell.SpellBase R { get; set; }
        public Jhin()
        {
            Q = new Spell.Targeted(SpellSlot.Q, 600);
            W = new Spell.Skillshot(SpellSlot.W, 700, SkillShotType.Circular);
            E = new Spell.Active(SpellSlot.E, 325);
            R = new Spell.Targeted(SpellSlot.R, 700);
        }
    }*/ // Just fuck you jhin.

    /*public class Nidalee : SpellBase // todo
    {
        public sealed override Spell.SpellBase Q { get; set; }
        public sealed override Spell.SpellBase W { get; set; }
        public sealed override Spell.SpellBase E { get; set; }
        public sealed override Spell.SpellBase R { get; set; }
        public Nidalee()
        {
            Q = new Spell.Targeted(SpellSlot.Q, 600);
            W = new Spell.Skillshot(SpellSlot.W, 700, SkillShotType.Circular);
            E = new Spell.Active(SpellSlot.E, 325);
            R = new Spell.Targeted(SpellSlot.R, 700);
        }
    }*/

    /*  public class Orianna : SpellBase
    {
        public sealed override Spell.SpellBase Q { get; set; }
        public sealed override Spell.SpellBase W { get; set; }
        public sealed override Spell.SpellBase E { get; set; }
        public sealed override Spell.SpellBase R { get; set; }
        public Orianna()
        {
            {
                Q = new Spell.Skillshot(SpellSlot.Q, 1000, SkillShotType.Linear, 250, 1550, 75);
                //Q2 = new Spell.Skillshot(SpellSlot.Q, 900, SkillShotType.Linear, 250, 1550, 75)     
                W = new Spell.Active(SpellSlot.W);
                E = new Spell.Targeted(SpellSlot.E, 325);
                R = new Spell.Active(SpellSlot.R);
            }
    } */ //Todo
}