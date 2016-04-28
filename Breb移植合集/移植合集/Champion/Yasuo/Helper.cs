using System;
using System.Collections.Generic;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Events;
using LeagueSharp.Common;
using SharpDX;
using Spell = LeagueSharp.Common.Spell;

namespace YasuoPro
{
    internal class Helper
    {
        internal const float LaneClearWaitTimeMod = 2f;

        internal static AIHeroClient Yasuo;

        public static Obj_AI_Base ETarget;

        internal static Obj_Shop shop = ObjectManager.Get<Obj_Shop>().FirstOrDefault(x => x.IsAlly);

        internal static bool DontDash = false;

        internal static int Q = 1, Q2 = 2, W = 3, E = 4, R = 5, Ignite = 6;

        internal static ItemManager.Item Hydra, Tiamat, Blade, Bilgewater, Youmu;

        /* Credits to Brian for Q Skillshot values */
        internal static Dictionary<int, Spell> Spells;

        internal static Vector2 DashPosition;

        internal string[] DangerousSpell =
        {
            "syndrar", "veigarprimordialburst", "dazzle", "leblancchaosorb",
            "judicatorreckoning", "iceblast", "disintegrate"
        };

        private static float GetQDelay
        {
            get { return 1 - Math.Min((Yasuo.AttackSpeedMod - 1)*0.0058552631578947f, 0.6675f); }
        }

        private static float GetQ1Delay
        {
            get { return 0.4f*GetQDelay; }
        }

        private static float GetQ2Delay
        {
            get { return 0.5f*GetQDelay; }
        }


        internal float Qrange
        {
            get { return TornadoReady ? Spells[Q2].Range : Spells[Q].Range; }
        }

        internal float Qdelay
        {
            get { return 0.250f - Math.Min(BonusAttackSpeed, 0.66f)*0.250f; }
        }


        internal float BonusAttackSpeed
        {
            get { return 1/Yasuo.AttackDelay - 0.658f; }
        }

        internal float Erange
        {
            get { return Spells[E].Range; }
        }

        internal float Rrange
        {
            get { return Spells[R].Range; }
        }

        internal bool TornadoReady
        {
            get { return Yasuo.HasBuff("yasuoq3w"); }
        }

        internal static int DashCount
        {
            get
            {
                var bc = Yasuo.GetBuffCount("yasuodashscalar");
                return bc;
            }
        }

        internal IEnumerable<AIHeroClient> KnockedUp
        {
            get
            {
                return (from hero in HeroManager.Enemies where hero.IsValidEnemy(Spells[R].Range) let knockup = hero.Buffs.Find(x => (x.Type == BuffType.Knockup && x.EndTime - Game.Time <= YasuoMenu.getSliderItem(YasuoMenu.ComboA, "Combo.knockupremainingpct")/100*(x.EndTime - x.StartTime)) || x.Type == BuffType.Knockback) where knockup != null select hero).ToList();
            }
        }

        internal static bool isHealthy
        {
            get
            {
                return Yasuo.IsInvulnerable || Yasuo.HasBuffOfType(BuffType.Invulnerability) ||
                       Yasuo.HasBuffOfType(BuffType.SpellShield) || Yasuo.HasBuffOfType(BuffType.SpellImmunity) ||
                       Yasuo.HealthPercent > YasuoMenu.getSliderItem(YasuoMenu.MiscA, "Misc.Healthy") ||
                       Yasuo.HasBuff("yasuopassivemovementshield") && Yasuo.HealthPercent > 30;
            }
        }

        internal static bool Debug
        {
            get { return YasuoMenu.getCheckBoxItem(YasuoMenu.MiscA, "Misc.Debug"); }
        }


        internal FleeType FleeMode
        {
            get
            {
                var GetFM = YasuoMenu.getBoxItem(YasuoMenu.Flee, "Flee.Mode");
                if (GetFM == 0)
                {
                    return FleeType.ToNexus;
                }
                if (GetFM == 1)
                {
                    return FleeType.ToAllies;
                }
                return FleeType.ToCursor;
            }
        }


        internal void InitSpells()
        {
            Spells = new Dictionary<int, Spell>
            {
                {1, new Spell(SpellSlot.Q, 500f)},
                {2, new Spell(SpellSlot.Q, 1150f)},
                {3, new Spell(SpellSlot.W, 450f)},
                {4, new Spell(SpellSlot.E, 475f)},
                {5, new Spell(SpellSlot.R, 1250f)},
                {6, new Spell(ObjectManager.Player.GetSpellSlot("summonerdot"), 600)}
            };

            Spells[Q].SetSkillshot(GetQ1Delay, 20f, float.MaxValue, false, SkillshotType.SkillshotLine);
            Spells[Q2].SetSkillshot(GetQ2Delay, 90, 1500, false, SkillshotType.SkillshotLine);
            Spells[E].SetTargetted(0.075f, 1025);
        }

        internal bool UseQ(AIHeroClient target, HitChance minhc = HitChance.Medium, bool UseQ1 = true, bool UseQ2 = true)
        {
            if (target == null)
            {
                return false;
            }

            var tready = TornadoReady;

            if ((tready && !UseQ2) || !tready && !UseQ1)
            {
                return false;
            }

            if (tready && Yasuo.IsDashing())
            {
                if (YasuoMenu.getCheckBoxItem(YasuoMenu.ComboA, "Combo.NoQ2Dash") || ETarget == null ||
                    !(ETarget is AIHeroClient) && ETarget.CountEnemiesInRange(120) < 1)
                {
                    return false;
                }
            }

            var sp = tready ? Spells[Q2] : Spells[Q];
            var pred = sp.GetPrediction(target);

            if (pred.Hitchance >= minhc)
            {
                return sp.Cast(pred.CastPosition);
            }

            return false;
        }

        internal static Vector2 GetDashPos(Obj_AI_Base @base)
        {
            var predictedposition =
                Yasuo.ServerPosition.LSExtend(@base.Position, Yasuo.Distance(@base) + 475 - Yasuo.Distance(@base))
                    .To2D();
            DashPosition = predictedposition;
            return predictedposition;
        }

        internal static double GetProperEDamage(Obj_AI_Base target)
        {
            double dmg = Yasuo.GetSpellDamage(target, SpellSlot.E);
            float amplifier = 0;
            if (DashCount == 0)
            {
                amplifier = 0;
            }
            else if (DashCount == 1)
            {
                amplifier = 0.25f;
            }
            else if (DashCount == 2)
            {
                amplifier = 0.50f;
            }
            dmg += dmg*amplifier;
            return dmg;
        }

        internal static HitChance GetHitChance(string search)
        {
            var hitchance = YasuoMenu.getBoxItem(YasuoMenu.MiscA, "Hitchance.Q");
            switch (hitchance)
            {
                case 0:
                    return HitChance.Low;
                case 1:
                    return HitChance.Medium;
                case 2:
                    return HitChance.High;
                case 3:
                    return HitChance.VeryHigh;
            }
            return HitChance.Medium;
        }

        internal UltMode GetUltMode()
        {
            switch (YasuoMenu.getBoxItem(YasuoMenu.ComboA, "Combo.UltMode"))
            {
                case 0:
                    return UltMode.Health;
                case 1:
                    return UltMode.Priority;
                case 2:
                    return UltMode.EnemiesHit;
            }
            return UltMode.Priority;
        }


        internal void InitItems()
        {
            Hydra = new ItemManager.Item(3074, 225f, ItemManager.ItemCastType.RangeCast, 1);
            Tiamat = new ItemManager.Item(3077, 225f, ItemManager.ItemCastType.RangeCast, 1);
            Blade = new ItemManager.Item(3153, 450f, ItemManager.ItemCastType.TargettedCast);
            Bilgewater = new ItemManager.Item(3144, 450f, ItemManager.ItemCastType.TargettedCast);
            Youmu = new ItemManager.Item(3142, 185f, ItemManager.ItemCastType.SelfCast, 1);
        }

        internal enum FleeType
        {
            ToNexus,
            ToAllies,
            ToCursor
        }

        internal enum UltMode
        {
            Health,
            Priority,
            EnemiesHit
        }
    }
}