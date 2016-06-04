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
    class Graves : Champion
    {
        public static Vector2 QCastPos = new Vector2();


        public Graves()
        {
            ARAMSimulator.champBuild = new Build
            {
                coreItems = new List<ConditionalItem>
                    {
                        new ConditionalItem(ItemId.The_Bloodthirster),
                        new ConditionalItem(ItemId.Berserkers_Greaves),
                        new ConditionalItem(ItemId.Infinity_Edge),
                        new ConditionalItem(ItemId.Blade_of_the_Ruined_King),
                        new ConditionalItem(ItemId.Last_Whisper),
                        new ConditionalItem(ItemId.Phantom_Dancer),
                    },
                startingItems = new List<ItemId>
                    {
                        ItemId.Boots_of_Speed,ItemId.Vampiric_Scepter,
                    }
            };
            AntiGapcloser.OnEnemyGapcloser += OnEnemyGapcloser;
            Interrupter2.OnInterruptableTarget += OnPossibleToInterrupt;
        }

        private void OnPossibleToInterrupt(AIHeroClient sender, Interrupter2.InterruptableTargetEventArgs args)
        {
        }

        private void OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            
        }

        public override void useQ(Obj_AI_Base target)
        {
            if (!Q.IsReady() || target == null)
                return;
            Q.Cast(target);
        }

        public override void useW(Obj_AI_Base target)
        {
            if (!W.IsReady() || target == null)
                return;
            //if (!Q.IsReady(4500) && player.Mana > 200)
            W.Cast(target);
        }

        public override void useE(Obj_AI_Base target)
        {
            if (!E.IsReady() || target == null || !safeGap(player.Position.LSExtend(target.Position,450).LSTo2D()))
                return;
            E.Cast(target.Position);
        }

        public override void useR(Obj_AI_Base target)
        {
            if (!R.IsReady() || target == null)
                return;
            R.CastIfWillHit(target,2);
        }

        public override void useSpells()
        {

            if (player.IsChannelingImportantSpell())
                return;

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
            Q = new Spell(SpellSlot.Q, 850);
            W = new Spell(SpellSlot.W, 940f);
            E = new Spell(SpellSlot.E, 850f);
            R = new Spell(SpellSlot.R, 1000f);

            Q.SetSkillshot(0.25f, 50f, 2000f, false, SkillshotType.SkillshotLine);
            W.SetSkillshot(0.35f, 150f, 1650f, false, SkillshotType.SkillshotCircle);
            R.SetSkillshot(0.25f, 120f, 2100f, false, SkillshotType.SkillshotLine);

        }

        public override void killSteal()
        {
            return;
        }

        public override void farm()
        {
            if (player.ManaPercent < 55)
                return;

            var AllMinions = MinionManager.GetMinions(player.ServerPosition, Q.Range, MinionTypes.All, MinionTeam.Enemy, MinionOrderTypes.Health);
            foreach (var minion in AllMinions)
            {
                if (Q.IsReady() && Q.GetDamage(minion) > minion.Health)
                {
                    Q.Cast(minion);
                }
            }
        }
    }
}
