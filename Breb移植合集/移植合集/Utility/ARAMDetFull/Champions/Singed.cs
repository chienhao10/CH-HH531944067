using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common; using EloBuddy;
using SharpDX;

namespace ARAMDetFull.Champions
{
    class Singed : Champion
    {

        public Singed()
        {

            ARAMSimulator.champBuild = new Build
            {
                coreItems = new List<ConditionalItem>
                        {
                            new ConditionalItem(ItemId.Rod_of_Ages),
                            new ConditionalItem(ItemId.Mercurys_Treads),
                            new ConditionalItem(ItemId.Randuins_Omen),
                            new ConditionalItem(ItemId.Rylais_Crystal_Scepter),
                            new ConditionalItem(ItemId.Liandrys_Torment),
                            new ConditionalItem(ItemId.Banshees_Veil),
                        },
                startingItems = new List<ItemId>
                        {
                            ItemId.Catalyst_the_Protector
                        }
            };
        }

        public override void useQ(Obj_AI_Base target)
        {
            if (!Q.IsReady())
                return;
            if (!player.HasBuff("Poison Trail"))
            {
                Q.Cast();
            }
        }

        public override void useW(Obj_AI_Base target)
        {
            if (!W.IsReady() || player.UnderTurret(true) || MapControl.fightIsOn() == null)
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
            if (!R.IsReady() || player.HealthPercent>70 )
                return;
            R.Cast();
        }

        public override void setUpSpells()
        {
            Q = new Spell(SpellSlot.Q, 550);
            W = new Spell(SpellSlot.W, 1175);
            E = new Spell(SpellSlot.E, 125);
            R = new Spell(SpellSlot.R, 250);
            
            W.SetSkillshot(0.5f, 350, 700, false, SkillshotType.SkillshotCircle);
        }

        public override void useSpells()
        {
            var tar = ARAMTargetSelector.getBestTarget(550);
            if (tar != null) useQ(tar); else if (player.HasBuff("Poison Trail")) {if(Q.IsReady())Q.Cast();}
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
