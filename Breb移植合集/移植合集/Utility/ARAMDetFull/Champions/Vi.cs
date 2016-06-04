using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common; using EloBuddy;

namespace ARAMDetFull.Champions
{
    class Vi : Champion
    {
        public Spell E2;

        public Vi()
        {
            ARAMSimulator.champBuild = new Build
            {
                coreItems = new List<ConditionalItem>
                        {
                            new ConditionalItem(ItemId.Trinity_Force),
                            new ConditionalItem(ItemId.Mercurys_Treads),
                            new ConditionalItem(ItemId.Last_Whisper),
                            new ConditionalItem(ItemId.Sunfire_Cape),
                            new ConditionalItem(ItemId.Randuins_Omen),
                            new ConditionalItem(ItemId.Banshees_Veil),
                        },
                startingItems = new List<ItemId>
                        {
                            ItemId.Phage
                        }
            };
        }

        public override void useQ(Obj_AI_Base target)
        {
            if (!Q.IsReady() || target == null)
                return;
            if (safeGap(target))
            {
                if (target.LSIsValidTarget(Q.Range))
                {
                    if (Q.IsCharging)
                    {
                        Q.Cast(target);
                    }
                    else
                    {
                        Q.StartCharging();
                    }
                }
            }
        }

        public override void useW(Obj_AI_Base target)
        {
        }

        public override void useE(Obj_AI_Base target)
        {
            if (!E.IsReady() || target == null)
                return;
            if (target.LSIsValidTarget(Orbwalking.GetRealAutoAttackRange(target)))
            {
                E.Cast();
            }
        }


        public override void useR(Obj_AI_Base t)
        {
            if (!R.IsReady() || t == null)
                return;
            var qDamage = player.LSGetSpellDamage(t, SpellSlot.Q);
            var eDamage = player.LSGetSpellDamage(t, SpellSlot.E) * E.Instance.Ammo;
            var rDamage = player.LSGetSpellDamage(t, SpellSlot.R);

            if (Q.IsReady() && t.Health < qDamage)
                return;

            if (E.IsReady() && Orbwalking.InAutoAttackRange(t) && t.Health < eDamage)
                return;

            if (Q.IsReady() && E.IsReady() && t.Health < qDamage + eDamage)
                return;

            if (t.Health > rDamage)
            {
                if (Q.IsReady() && E.IsReady() && t.Health < rDamage + qDamage + eDamage)
                    R.CastOnUnit(t);
                else if (E.IsReady() && t.Health < rDamage + eDamage)
                    R.CastOnUnit(t);
                else if (Q.IsReady() && t.Health < rDamage + qDamage)
                    R.CastOnUnit(t);
            }
            else
            {
                if (!Orbwalking.InAutoAttackRange(t))
                    R.CastOnUnit(t);
            }
        }

        public override void useSpells()
        {
            var tar = ARAMTargetSelector.getBestTarget(Q.Range);
            if (tar != null) useQ(tar);
            tar = ARAMTargetSelector.getBestTarget(E.Range);
            if (tar != null) useE(tar);
            tar = ARAMTargetSelector.getBestTarget(R.Range);
            if (tar != null) useR(tar);
        }

        public override void setUpSpells()
        {
            Q = new Spell(SpellSlot.Q, 860f);
            E = new Spell(SpellSlot.E);
            E2 = new Spell(SpellSlot.E, 600f);
            R = new Spell(SpellSlot.R, 800f);

            Q.SetSkillshot(0.5f, 75f, float.MaxValue, false, SkillshotType.SkillshotLine);
            Q.SetCharged("ViQ", "ViQ", 100, 860, 1f);

            E.SetSkillshot(0.15f, 150f, float.MaxValue, false, SkillshotType.SkillshotLine);
            R.SetTargetted(0.15f, 1500f);
        }
    }
}
