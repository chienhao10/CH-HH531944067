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
    class Kassadin : Champion
    {

        private static LeagueSharp.Common.Items.Item DFG;

        public Kassadin()
        {
            ARAMSimulator.champBuild = new Build
            {
                coreItems = new List<ConditionalItem>
                        {
                            new ConditionalItem(ItemId.Rod_of_Ages),
                            new ConditionalItem(ItemId.Sorcerers_Shoes),
                            new ConditionalItem(ItemId.Rabadons_Deathcap),
                            new ConditionalItem(ItemId.Lich_Bane),
                            new ConditionalItem(ItemId.Ludens_Echo),
                            new ConditionalItem(ItemId.Void_Staff),
                        },
                startingItems = new List<ItemId>
                        {
                            ItemId.Catalyst_the_Protector
                        }
            };
        }

        public override void useQ(Obj_AI_Base target)
        {
            if (!Q.IsReady() || target == null)
                return;
            if (target.LSIsValidTarget(Q.Range + 100))
                Q.CastOnUnit(target);
        }

        public override void useW(Obj_AI_Base target)
        {
            if (!W.IsReady() || target == null)
                return;
            if (target.LSIsValidTarget(W.Range + 100))
                W.Cast(target);
        }

        public override void useE(Obj_AI_Base target)
        {
            if (!E.IsReady() || target == null)
                return;
                E.Cast(target);
        }

        public override void useR(Obj_AI_Base target)
        {
            if (target == null)
                return;
            if (R.IsReady())
            {
                if (getEnemiesInRange(player.Position, 500f) >= 3 && getAlliesInRange(player.Position, 410f) < 3)
                {
                    R.Cast(ARAMSimulator.fromNex.Position);
                    return;
                }


                if (getEnemiesInRange(target.Position, 500f) >= 3 && getAlliesInRange(target.Position, 410f) < 3) return;
                if (!Sector.inTowerRange(target.Position.LSTo2D()) && GetComboDamage(target) > target.Health)
                    R.Cast(target);
            }
        }

        int getEnemiesInRange(Vector3 point, float range)
        {
            return
                ObjectManager.Get<AIHeroClient>()
                    .Where(h => h.IsEnemy && !h.IsDead && h.IsValid && h.LSDistance(point) <= range).ToList().Count;
        }

        int getAlliesInRange(Vector3 point, float range)
        {
            return
                ObjectManager.Get<AIHeroClient>()
                    .Where(h => h.IsAlly && !h.IsDead && h.IsValid && h.LSDistance(point) <= range).ToList().Count;
        }

        public override void useSpells()
        {
            var tar = ARAMTargetSelector.getBestTarget(750);
            if (tar == null || tar.HasBuffOfType(BuffType.Invulnerability)) return;
            useQ(tar);
            useW(tar);
            useE(tar);
            useR(tar);


        }

        public override void setUpSpells()
        {
            DFG = LeagueSharp.Common.Utility.Map.GetMap().Type == LeagueSharp.Common.Utility.Map.MapType.TwistedTreeline ||
                  LeagueSharp.Common.Utility.Map.GetMap().Type == LeagueSharp.Common.Utility.Map.MapType.CrystalScar
                ? new LeagueSharp.Common.Items.Item(3188, 750) : new LeagueSharp.Common.Items.Item(3128, 750);

            Q = new Spell(SpellSlot.Q, 650);
            W = new Spell(SpellSlot.W, 150);
            E = new Spell(SpellSlot.E, 500);
            R = new Spell(SpellSlot.R, 500);

            Q.SetTargetted(0.5f, 1400f);
            E.SetSkillshot(0.5f, 10f, float.MaxValue, false, SkillshotType.SkillshotCone);
            R.SetSkillshot(0.5f, 150f, float.MaxValue, false, SkillshotType.SkillshotCircle);
        }

        private float GetComboDamage(Obj_AI_Base enemy)
        {
            var damage = 0d;
            if (R.IsReady())
                damage += player.LSGetSpellDamage(enemy, SpellSlot.R);

            if (DFG.IsReady())
                damage += player.GetItemDamage(enemy, Damage.DamageItems.Dfg) / 1.2;

            if (W.IsReady())
                damage += player.LSGetSpellDamage(enemy, SpellSlot.W);

            if (Q.IsReady())
                damage += player.LSGetSpellDamage(enemy, SpellSlot.Q);

            if (E.IsReady())
                damage += player.LSGetSpellDamage(enemy, SpellSlot.E);


            return (float)damage * (DFG.IsReady() ? 1.2f : 1);
        }
    }
}
