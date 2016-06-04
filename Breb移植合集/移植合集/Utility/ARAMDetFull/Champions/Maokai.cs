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
    class Maokai : Champion
    {
        public Maokai()
        {
            Interrupter.OnPossibleToInterrupt += OnPossibleToInterrupt;
            AntiGapcloser.OnEnemyGapcloser += OnEnemyGapcloser;
            ARAMSimulator.champBuild = new Build
            {
                coreItems = new List<ConditionalItem>
                {
                    new ConditionalItem(ItemId.Rod_of_Ages),
                    new ConditionalItem(ItemId.Mercurys_Treads, ItemId.Ninja_Tabi, ItemCondition.ENEMY_AP),
                    new ConditionalItem(ItemId.Spirit_Visage, ItemId.Frozen_Heart, ItemCondition.ENEMY_AP),
                    new ConditionalItem(ItemId.Abyssal_Scepter),
                    new ConditionalItem(ItemId.Locket_of_the_Iron_Solari),
                    new ConditionalItem(ItemId.Banshees_Veil),
                },
                startingItems = new List<ItemId>
                {
                    ItemId.Catalyst_the_Protector,ItemId.Boots_of_Speed
                }
            };
        }

        public override void useQ(Obj_AI_Base target)
        {
            if (!Q.IsReady())
                return;
            Q.Cast(target);
        }

        public override void useW(Obj_AI_Base target)
        {
            if (!W.IsReady())
                return;
            if (safeGap(target))
                W.CastOnUnit(target);
        }

        public override void useE(Obj_AI_Base target)
        {
            if (!E.IsReady() )
                return;
            E.Cast(target);
        }

        public override void useR(Obj_AI_Base target)
        {
            if (!R.IsReady())
                return;
            if ((CanKill(target, R, GetRDmg(target)) || player.Position.LSCountEnemiesInRange((int)R.Range-100)>2))
                R.Cast();
            if (player.ManaPercent >= 30)
            {
                if (!player.HasBuff("MaokaiDrain")) R.Cast();
            }
            else if (player.HasBuff("MaokaiDrain")) R.Cast();


        }
        private double GetRDmg(Obj_AI_Base Target)
        {
            return player.CalcDamage(Target, DamageType.Magical, new double[] { 100, 150, 200 }[R.Level - 1] + 0.5 * player.FlatMagicDamageMod + R.Instance.Ammo);
        }

        public override void setUpSpells()
        {
            Q = new Spell(SpellSlot.Q, 630);
            W = new Spell(SpellSlot.W, 525);
            E = new Spell(SpellSlot.E, 1115);
            R = new Spell(SpellSlot.R, 478);
            Q.SetSkillshot(0.3333f, 110, 1100, false, SkillshotType.SkillshotLine);
            W.SetTargetted(0.5f, 1000);
            E.SetSkillshot(0.25f, 225, 1750, false, SkillshotType.SkillshotCircle);
            R.SetSkillshot(0.25f, 478, float.MaxValue, false, SkillshotType.SkillshotCircle);
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

        private void OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (player.IsDead || !Q.CanCast(gapcloser.Sender)) return;
            if (player.Distance3D(gapcloser.Sender) <= 100) Q.Cast(gapcloser.Sender.Position);
        }

        private void OnPossibleToInterrupt(Obj_AI_Base unit, InterruptableSpell spell)
        {
            if ( player.IsDead || !Q.IsReady()) return;
            if (player.Distance3D(unit) > 100 && W.CanCast(unit) && player.Mana >= Q.Instance.SData.Mana + W.Instance.SData.Mana)
            {
                W.CastOnUnit(unit);
                return;
            }
            if (Q.IsInRange(unit) && player.Distance3D(unit) <= 100) Q.Cast(unit.Position);
        }
        public static bool CanKill(Obj_AI_Base Target, Spell Skill, double Health, double SubDmg)
        {
            return Skill.GetHealthPrediction(Target) - Health + 5 <= SubDmg;
        }

        public static bool CanKill(Obj_AI_Base Target, Spell Skill, double SubDmg)
        {
            return CanKill(Target, Skill, 0, SubDmg);
        }

        public static bool CanKill(Obj_AI_Base Target, Spell Skill, int Stage = 0, double SubDmg = 0)
        {
            return Skill.GetHealthPrediction(Target) + 5 <= (SubDmg > 0 ? SubDmg : Skill.GetDamage(Target, Stage));
        }
    }
}
