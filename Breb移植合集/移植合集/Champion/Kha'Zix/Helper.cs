using LeagueSharp;
using LeagueSharp.Common;
using System;
using System.Collections.Generic;
using SharpDX;
using System.Linq;
using EloBuddy;

namespace SephKhazix
{
    class Helper
    {
        internal static AIHeroClient Khazix = ObjectManager.Player;

        internal static KhazixMenu Config;

        internal Items.Item Hydra, Tiamat, Blade, Bilgewater, Youmu, Titanic;

        internal Spell Q, W, WE, E, R;

        internal const float Wangle = 22 * (float) Math.PI / 180;

        internal static bool EvolvedQ, EvolvedW, EvolvedE;

        internal static List<AIHeroClient> HeroList;
        internal static List<Vector3> EnemyTurretPositions = new List<Vector3>();
        internal static Vector3 NexusPosition;
        internal static Vector3 Jumppoint1, Jumppoint2;
        internal static bool Jumping;
        
        internal void InitSkills()
        {
            Q = new Spell(SpellSlot.Q, 325f);
            W = new Spell(SpellSlot.W, 1000f);
            WE = new Spell(SpellSlot.W, 1000f);
            E = new Spell(SpellSlot.E, 700f);
            R = new Spell(SpellSlot.R, 0);
            W.SetSkillshot(0.225f, 80f, 828.5f, true, SkillshotType.SkillshotLine);
            WE.SetSkillshot(0.225f, 100f, 828.5f, true, SkillshotType.SkillshotLine);
            E.SetSkillshot(0.25f, 100f, 1000f, false, SkillshotType.SkillshotCircle);

            Hydra = new Items.Item(3074, 225f);
            Tiamat = new Items.Item(3077, 225f);
            Blade = new Items.Item(3153, 450f);
            Bilgewater = new Items.Item(3144, 450f);
            Youmu = new Items.Item(3142, 185f);
            Titanic = new Items.Item(3748, 225f);

        }

        internal void EvolutionCheck()
        {
            if (!EvolvedQ && Khazix.HasBuff("khazixqevo"))
            {
                Q.Range = 375;
                EvolvedQ = true;
            }
            if (!EvolvedW && Khazix.HasBuff("khazixwevo"))
            {
                EvolvedW = true;
                W.SetSkillshot(0.225f, 100f, 828.5f, true, SkillshotType.SkillshotLine);
            }

            if (!EvolvedE && Khazix.HasBuff("khazixeevo"))
            {
                E.Range = 1000;
                EvolvedE = true;
            }
        }

        internal void UseItems(Obj_AI_Base target)
        {
            var KhazixServerPosition = Khazix.ServerPosition.LSTo2D();
            var targetServerPosition = target.ServerPosition.LSTo2D();

            if (Hydra.IsReady() && Khazix.LSDistance(target) <= Hydra.Range)
            {
                Hydra.Cast();
            }
            if (Tiamat.IsReady() && Khazix.LSDistance(target) <= Tiamat.Range)
            {
                Tiamat.Cast();
            }
            if (Titanic.IsReady() && Khazix.LSDistance(target) <= Tiamat.Range)
            {
                Tiamat.Cast();
            }
            if (Blade.IsReady() && Khazix.LSDistance(target) <= Blade.Range)
            {
                Blade.Cast(target);
            }
            if (Youmu.IsReady() && Khazix.LSDistance(target) <= Youmu.Range)
            {
                Youmu.Cast(target);
            }
            if (Bilgewater.IsReady() && Khazix.LSDistance(target) <= Bilgewater.Range)
            {
                Bilgewater.Cast(target);
            }
        }

        internal double GetQDamage(Obj_AI_Base target)
        {
            if (Q.Range < 326)
            {
                return 0.984 * Khazix.LSGetSpellDamage(target, SpellSlot.Q, target.IsIsolated() ? 1 : 0);
            }
            if (Q.Range > 325)
            {
                var isolated = target.IsIsolated();
                if (isolated)
                {
                    return 0.984 * Khazix.LSGetSpellDamage(target, SpellSlot.Q, 3);
                }
                return Khazix.LSGetSpellDamage(target, SpellSlot.Q, 0);
            }
            return 0;
        }

        internal List<AIHeroClient> GetIsolatedTargets()
        {
            var validtargets = HeroList.Where(h => h.LSIsValidTarget(E.Range) && h.IsIsolated()).ToList();
            return validtargets;
        }

        internal static HitChance HarassHitChance(KhazixMenu menu)
        {
            int hitchance = SephKhazix.Khazix.getSliderItem(SephKhazix.KhazixMenu.harass, "Harass.WHitchance");
            switch (hitchance)
            {
                case 0:
                    return HitChance.Low;
                case 1:
                    return HitChance.Medium;
                case 2:
                    return HitChance.High;
            }
            return HitChance.Medium;
        }

        internal KhazixMenu GenerateMenu()
        {
            Config = new KhazixMenu();
            return Config;
        }

        internal bool IsHealthy
        {
            get
            {
                return Khazix.HealthPercent >= SephKhazix.Khazix.getSliderItem(SephKhazix.KhazixMenu.safety, "Safety.MinHealth");
            }
        }

        internal bool Override
        {
            get
            {
                return SephKhazix.Khazix.getKeyBindItem(SephKhazix.KhazixMenu.safety, "Safety.Override");
            }
        }

        internal bool IsInvisible
        {
            get
            {
                return Khazix.HasBuff("khazixrstealth");
            }
        }

    }
}

