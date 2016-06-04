using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common; using EloBuddy;

namespace ARAMDetFull.Champions
{
    internal class Olaf : Champion
    {
        public Olaf()
        {
            GameObject.OnCreate += onCreate;
            GameObject.OnDelete += onDelete;
            LXOrbwalker.AfterAttack += afterAttack;

            ARAMSimulator.champBuild = new Build
            {
                coreItems = new List<ConditionalItem>
                        {
                            new ConditionalItem(ItemId.The_Black_Cleaver),
                            new ConditionalItem(ItemId.Mercurys_Treads,ItemId.Ninja_Tabi,ItemCondition.ENEMY_AP),
                            new ConditionalItem((ItemId)3053),
                            new ConditionalItem(ItemId.Randuins_Omen),
                            new ConditionalItem(ItemId.Maw_of_Malmortius),
                            new ConditionalItem(ItemId.Banshees_Veil),
                        },
                startingItems = new List<ItemId>
                        {
                            ItemId.Phage
                        }
            };
        }

        private void afterAttack(Obj_AI_Base unit, Obj_AI_Base target)
        {
            if (unit is AIHeroClient && W.IsReady())
                W.Cast();
        }

        public GameObject olafAxe = null;

        private void onDelete(GameObject sender, EventArgs args)
        {
            if (sender.Name == "olaf_axe_totem_team_id_green.troy" && sender.IsAlly)
            {
                olafAxe = null;
            }
        }

        private void onCreate(GameObject sender, EventArgs args)
        {
            if (sender.Name == "olaf_axe_totem_team_id_green.troy" && sender.IsAlly)
            {
                olafAxe = sender;
            }
        }

        public override void useQ(Obj_AI_Base target)
        {
            if (!Q.IsReady() || target == null)
                return;
            Q.Cast(target);
        }

        public override void useW(Obj_AI_Base target)
        {
            if (!W.IsReady())
                return;
        }

        public override void useE(Obj_AI_Base target)
        {
            if (!E.IsReady() || target == null)
                return;
            E.CastOnUnit(target);
        }


        public override void useR(Obj_AI_Base target)
        {
            if (!R.IsReady() || target == null)
                return;
            if (player.HealthPercent < 65)
                R.Cast();
        }

        public override void useSpells()
        {
            gatherAze();

            var tar = ARAMTargetSelector.getBestTarget(Q.Range);
            if (tar != null) useQ(tar);
            tar = ARAMTargetSelector.getBestTarget(W.Range);
            if (tar != null) useW(tar);
            tar = ARAMTargetSelector.getBestTarget(E.Range);
            if (tar != null) useE(tar);
            tar = ARAMTargetSelector.getBestTarget(R.Range);
            if (tar != null) useR(tar);
        }

        public void gatherAze()
        {
            LXOrbwalker.CustomOrbwalkMode = false;
            if (olafAxe == null)
                return;
            if (olafAxe.Position.LSDistance(player.Position, true) > 500*500)
                return;
            if (olafAxe.Position.UnderTurret(true))
                return;
            LXOrbwalker.CustomOrbwalkMode = true;
            LXOrbwalker.Orbwalk(olafAxe.Position, LXOrbwalker.GetPossibleTarget());

        }

        public override void setUpSpells()
        {
            Q = new Spell(SpellSlot.Q, 1000);
            W = new Spell(SpellSlot.W);
            E = new Spell(SpellSlot.E, 325);
            R = new Spell(SpellSlot.R,350);

            Q.SetSkillshot(0.25f, 75f, 1500f, false, SkillshotType.SkillshotLine);
        }
    }
}
