using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common; using EloBuddy;
using SharpDX;

namespace ARAMDetFull.Champions
{
    class Twitch : Champion
    {

        public Twitch()
        {
            ARAMSimulator.champBuild = new Build
            {
                coreItems = new List<ConditionalItem>
                    {
                        new ConditionalItem(ItemId.Infinity_Edge),
                        new ConditionalItem(ItemId.Berserkers_Greaves),
                        new ConditionalItem(ItemId.The_Bloodthirster),
                        new ConditionalItem(ItemId.Phantom_Dancer),
                        new ConditionalItem(ItemId.Runaans_Hurricane_Ranged_Only),
                        new ConditionalItem(ItemId.Last_Whisper),
                    },
                startingItems = new List<ItemId>
                    {
                        ItemId.Vampiric_Scepter,ItemId.Boots_of_Speed
                    }
            };
        }

        public override void useQ(Obj_AI_Base target)
        {
            if (!Q.IsReady() || MapControl.fightIsOn() == null)
                return;
            Q.Cast();
        }

        public override void useW(Obj_AI_Base target)
        {
            if (!W.IsReady())
                return;
            W.Cast(target);
        }

        public override void useE(Obj_AI_Base target)
        {
            if (!E.IsReady())
                return;
            var eTarget = ARAMTargetSelector.getBestTarget(E.Range);
            if (eTarget.LSIsValidTarget(E.Range))
            {
                if (eTarget.Buffs.Where(buff => buff.DisplayName.ToLower() == "twitchdeadlyvenom").Any(buff => buff.Count > 4))
                {
                    E.Cast();
                    return;
                }
            }

            foreach (
                var hero in
                    ObjectManager.Get<AIHeroClient>()
                        .Where(
                            hero =>
                                hero.LSIsValidTarget(E.Range) &&
                                (player.LSGetSpellDamage(hero, SpellSlot.E) - 10 > hero.Health)))
            {
                E.Cast();
            }

        }

        public override void useR(Obj_AI_Base target)
        {
            if (!R.IsReady())
                return;
            if (player.LSCountEnemiesInRange(700)>1)
                if(R.Cast())
                    Aggresivity.addAgresiveMove(new AgresiveMove(35, 5000, true));
        }

        public override void setUpSpells()
        {
            Q = new Spell(SpellSlot.Q, 800);
            R = new Spell(SpellSlot.R, 800);

            W = new Spell(SpellSlot.W, 950);
            W.SetSkillshot(0.25f, 120f, 1400f, false, SkillshotType.SkillshotCircle);
            E = new Spell(SpellSlot.E, 1200);
        }

        public override void useSpells()
        {
            var tar = ARAMTargetSelector.getBestTarget(Q.Range);
            if (tar != null) useQ(tar);
            tar = ARAMTargetSelector.getBestTarget(E.Range);
            if (tar != null) useE(tar);
            tar = ARAMTargetSelector.getBestTarget(W.Range);
            if (tar != null) useW(tar);
            tar = ARAMTargetSelector.getBestTarget(R.Range);
            if (tar != null) useR(tar);
        }



    }
}
