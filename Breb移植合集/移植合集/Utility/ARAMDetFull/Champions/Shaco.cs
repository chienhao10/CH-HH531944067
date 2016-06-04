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
    class Shaco : Champion
    {

        public Shaco()
        {
            ARAMSimulator.champBuild = new Build
            {
                coreItems = new List<ConditionalItem>
                        {
                            new ConditionalItem(ItemId.Ravenous_Hydra_Melee_Only),
                            new ConditionalItem(ItemId.Berserkers_Greaves),
                            new ConditionalItem(ItemId.Statikk_Shiv),
                            new ConditionalItem(ItemId.Last_Whisper),
                            new ConditionalItem(ItemId.Mercurial_Scimitar),
                            new ConditionalItem(ItemId.Banshees_Veil),
                        },
                startingItems = new List<ItemId>
                        {
                            ItemId.Vampiric_Scepter,ItemId.Boots_of_Speed
                        }
            };
        }

        public override void useQ(Obj_AI_Base target)
        {
            if (!Q.IsReady())
                return;
            if (EnemyInRange(2, 500) && player.HealthPercent < 65)
                Q.Cast(player.Position.LSTo2D().LSExtend(ARAMSimulator.fromNex.Position.LSTo2D(), 400));
        }

        public override void useW(Obj_AI_Base target)
        {
            if (!W.IsReady())
                return;
            W.Cast(target.Position);
        }

        public override void useE(Obj_AI_Base target)
        {
            if (!E.IsReady())
                return;
            E.CastOnUnit(target);

        }

        public override void useR(Obj_AI_Base target)
        {
            if (!R.IsReady())
                return;
            R.Cast();
        }

        public override void setUpSpells()
        {
            Q = new Spell(SpellSlot.Q, 400);
            W = new Spell(SpellSlot.W, 525);
            E = new Spell(SpellSlot.E, 625);
            R = new Spell(SpellSlot.R, 0);
        }

        public override void useSpells()
        {
            var tar = ARAMTargetSelector.getBestTarget(Q.Range);
            if (tar != null) useQ(tar);
            tar = ARAMTargetSelector.getBestTarget(E.Range);
            if (tar != null) useE(tar);
            tar = ARAMTargetSelector.getBestTarget(W.Range);
            if (tar != null) useW(tar);
            tar = ARAMTargetSelector.getBestTarget(300);
            if (tar != null) useR(tar);
        }

        public static bool EnemyInRange(int numOfEnemy, float range)
        {
            return LeagueSharp.Common.Utility.LSCountEnemiesInRange(ObjectManager.Player, (int)range) >= numOfEnemy;
        }

    }
}
