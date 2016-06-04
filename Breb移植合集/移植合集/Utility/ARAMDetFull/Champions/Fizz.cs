using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common; using EloBuddy;

namespace ARAMDetFull.Champions
{
    class Fizz : Champion
    {
        public Fizz()
        {
            ARAMSimulator.champBuild = new Build
            {
                coreItems = new List<ConditionalItem>
                    {
                        new ConditionalItem(ItemId.Lich_Bane),
                        new ConditionalItem(ItemId.Sorcerers_Shoes),
                        new ConditionalItem(ItemId.Rabadons_Deathcap),
                        new ConditionalItem(ItemId.Rylais_Crystal_Scepter),
                        new ConditionalItem(ItemId.Void_Staff,ItemId.Liandrys_Torment, ItemCondition.ENEMY_MR),
                        new ConditionalItem(ItemId.Zhonyas_Hourglass),
                    },
                startingItems = new List<ItemId>
                    {
                        ItemId.Sheen
                    }
            };
            Console.WriteLine("Fizz i");
        }
        public override void useQ(Obj_AI_Base target)
        {
            if (!Q.IsReady() || target == null || !safeGap(player.Position.LSExtend(target.Position,475).LSTo2D()))
                return;
            Q.CastOnUnit(target);
        }

        public override void useW(Obj_AI_Base target)
        {
            if (!W.IsReady())
                return;
            W.Cast();
        }

        public override void useE(Obj_AI_Base target)
        {
            if (!E.IsReady() || target == null || !safeGap(target))
                return;
            if (player.Spellbook.GetSpell(SpellSlot.E).Name == "fizzjumptwo" && Environment.TickCount - E.LastCastAttemptT > 150)
                E.Cast(target.Position);
            if (player.Spellbook.GetSpell(SpellSlot.E).Name == "FizzJump")
                E.Cast(target.Position);

        }


        public override void useR(Obj_AI_Base target)
        {
            if (!R.IsReady() || target == null)
                return;
            if (!safeGap(target) || target.HealthPercent < 28)
            {
                if(R.Cast(target) == Spell.CastStates.SuccessfullyCasted)
                    Aggresivity.addAgresiveMove(new AgresiveMove(25,4000,true));
            }
        }

        public override void useSpells()
        {
            var tar = ARAMTargetSelector.getBestTarget(Q.Range);
            if (tar != null) useQ(tar);
            tar = ARAMTargetSelector.getBestTarget(W.Range);
            if (tar != null) useW(tar);
            tar = ARAMTargetSelector.getBestTarget(E.Range);
            if (tar != null) useE(tar);
            tar = ARAMTargetSelector.getBestTarget(R.Range);
            if (tar != null) useR(tar);
        }

        public override void setUpSpells()
        {
            Q = new Spell(SpellSlot.Q, 550);
            W = new Spell(SpellSlot.W, 240);
            E = new Spell(SpellSlot.E, 725);
            R = new Spell(SpellSlot.R, 1300);

            E.SetSkillshot(0.5f, 270f, 1300, false, SkillshotType.SkillshotCircle);
            R.SetSkillshot(0.25f, 120f, 1350f, false, SkillshotType.SkillshotLine);
        }


        public override void farm()
        {
           // if (player.ManaPercent < 55 || !E.IsReady())
                return;
        }
    }
}
