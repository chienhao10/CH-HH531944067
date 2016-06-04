using System;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu;
using LeagueSharp.Common;
using SCommon.Damage;
using SCommon.Database;
using SharpDX;
using Color = System.Drawing.Color;
using Utility = SCommon.Orbwalking.Utility;

namespace SCommon.TS
{
    public static class TargetSelector
    {
        public enum Mode
        {
            Auto,
            LowHP,
            MostAD,
            MostAP,
            Closest,
            NearMouse,
            LessAttack,
            LessCast,
            MostStack
        }

        private static string[] StackNames =
        {
            "kalistaexpungemarker",
            "vaynesilvereddebuff",
            "twitchdeadlyvenom",
            "ekkostacks",
            "dariushemo",
            "gnarwproc",
            "tahmkenchpdebuffcounter",
            "varuswdebuff"
        };

        private static Func<AIHeroClient, float> s_fnCustomMultipler;

        static TargetSelector()
        {
            SelectedTarget = null;
            Game.OnWndProc += Game_OnWndProc;
            Drawing.OnDraw += Drawing_OnDraw;
        }

        public static AIHeroClient SelectedTarget { get; private set; }

        public static bool IsInvulnerable(Obj_AI_Base target, DamageType damageType, bool ignoreShields = true)
        {
            //Kindred's Lamb's Respite(R)

            if (target.HasBuff("kindredrnodeathbuff") && target.HealthPercent <= 10)
            {
                return true;
            }

            // Tryndamere's Undying Rage (R)
            if (target.HasBuff("Undying Rage") && target.Health <= target.MaxHealth*0.10f)
            {
                return true;
            }

            // Kayle's Intervention (R)
            if (target.HasBuff("JudicatorIntervention"))
            {
                return true;
            }

            if (ignoreShields)
            {
                return false;
            }

            // Morgana's Black Shield (E)
            if (damageType.Equals(DamageType.Magical) && target.HasBuff("BlackShield"))
            {
                return true;
            }

            // Banshee's Veil (PASSIVE)
            if (damageType.Equals(DamageType.Magical) && target.HasBuff("BansheesVeil"))
            {
                // TODO: Get exact Banshee's Veil buff name.
                return true;
            }

            // Sivir's Spell Shield (E)
            if (damageType.Equals(DamageType.Magical) && target.HasBuff("SivirShield"))
            {
                // TODO: Get exact Sivir's Spell Shield buff name
                return true;
            }

            // Nocturne's Shroud of Darkness (W)
            if (damageType.Equals(DamageType.Magical) && target.HasBuff("ShroudofDarkness"))
            {
                // TODO: Get exact Nocturne's Shourd of Darkness buff name
                return true;
            }

            return false;
        }

        public static void Initialize()
        {
            ConfigMenu.Create();
        }

        public static AIHeroClient GetTarget(float range, DamageType dmgType = DamageType.Physical,
            Vector3? _from = null)
        {
            var from = _from ?? ObjectManager.Player.ServerPosition;
            //if (s_LastTarget == null || !s_LastTarget.IsValidTarget(range) || Utils.TickCount - s_LastTargetSent > 250)
            //{
            //    var t = GetNewTarget(range, dmgType, from);
            //    s_LastTarget = t;
            //    s_LastTargetSent = Utils.TickCount;
            //}
            return GetNewTarget(range, dmgType, from);
        }

        public static void RegisterCustomMultipler(Func<AIHeroClient, float> fn)
        {
            s_fnCustomMultipler = fn;
        }

        public static void UnregisterCustomMultipler()
        {
            s_fnCustomMultipler = null;
        }

        private static AIHeroClient GetNewTarget(float range, DamageType dmgType = DamageType.Physical,
            Vector3? _from = null)
        {
            if (range == -1)
                range = Utility.GetAARange();

            if (ConfigMenu.OnlyAttackSelected)
            {
                if (SelectedTarget != null)
                {
                    if (SelectedTarget.IsValidTarget(range))
                        return SelectedTarget;
                    if (SelectedTarget.IsValidTarget())
                        return null;
                }
            }

            if (ConfigMenu.FocusSelected)
            {
                if (SelectedTarget != null)
                {
                    if (SelectedTarget.IsValidTarget(range))
                        return SelectedTarget;
                    if (ConfigMenu.FocusExtraRange > 0 &&
                        SelectedTarget.IsValidTarget(range + ConfigMenu.FocusExtraRange))
                        return null;
                }
            }
            var from = _from ?? ObjectManager.Player.ServerPosition;

            var enemies =
                HeroManager.Enemies.Where(
                    p => p.IsValidTarget(range + p.BoundingRadius, true, from) && !IsInvulnerable(p, dmgType));
            if (enemies.Count() == 0)
                return null;

            switch (ConfigMenu.TargettingMode)
            {
                case 1:
                    return enemies.MinOrDefault(hero => hero.Health);

                case 2:
                    return enemies.MaxOrDefault(hero => hero.BaseAttackDamage + hero.FlatPhysicalDamageMod);

                case 3:
                    return enemies.MaxOrDefault(hero => hero.BaseAbilityDamage + hero.FlatMagicDamageMod);

                case 4:
                    return
                        enemies.MinOrDefault(
                            hero =>
                                (_from ?? ObjectManager.Player.ServerPosition).Distance(
                                    hero.ServerPosition, true));

                case 5:
                    return enemies.Find(hero => hero.Distance(Game.CursorPos, true) < 22500); // 150 * 150

                case 6:
                    return
                        enemies.MaxOrDefault(
                            hero =>
                                ObjectManager.Player.CalcDamage(hero, DamageType.Physical, 100)/(1 + hero.Health)*
                                GetPriority(hero));

                case 7:
                    return
                        enemies.MaxOrDefault(
                            hero =>
                                ObjectManager.Player.CalcDamage(hero, DamageType.Magical, 100)/(1 + hero.Health)*
                                GetPriority(hero));

                case 0:
                {
                    var killableWithAA = enemies.Where(p => p.Health <= AutoAttack.GetDamage(p, true)).FirstOrDefault();
                    if (killableWithAA != null)
                        return killableWithAA;

                    var possibleTargets = enemies.OrderByDescending(GetPriority);
                    if (possibleTargets.Count() == 1)
                        return possibleTargets.First();
                    if (possibleTargets.Count() > 1)
                    {
                        var killableTarget =
                            possibleTargets.OrderByDescending(GetTotalADAPMultipler)
                                .FirstOrDefault(q => GetHealthMultipler(q) >= 10);
                        if (killableTarget != null)
                            return killableTarget;

                        var targets = possibleTargets.OrderBy(p => ObjectManager.Player.Distance(p.ServerPosition));
                        AIHeroClient mostImportant = null;
                        double mostImportantsDamage = 0;
                        foreach (var target in targets)
                        {
                            var dmg = target.CalcDamage(ObjectManager.Player, DamageType.Physical, 100) +
                                      target.CalcDamage(ObjectManager.Player, DamageType.Magical, 100);
                            if (mostImportant == null)
                            {
                                mostImportant = target;
                                mostImportantsDamage = dmg;
                            }
                            else
                            {
                                if (Utility.InAARange(ObjectManager.Player, target) &&
                                    !Utility.InAARange(ObjectManager.Player, mostImportant))
                                {
                                    mostImportant = target;
                                    mostImportantsDamage = dmg;
                                }
                                else if ((Utility.InAARange(ObjectManager.Player, target) &&
                                          Utility.InAARange(ObjectManager.Player, mostImportant)) ||
                                         (!Utility.InAARange(ObjectManager.Player, target) &&
                                          !Utility.InAARange(ObjectManager.Player, mostImportant)))
                                {
                                    if (mostImportantsDamage < dmg/2f)
                                    {
                                        mostImportant = target;
                                        mostImportantsDamage = dmg;
                                        continue;
                                    }

                                    if ((mostImportant.IsMelee && !target.IsMelee) ||
                                        (!mostImportant.IsMelee && target.IsMelee))
                                    {
                                        var targetMultp = GetHealthMultipler(target);
                                        var mostImportantsMultp = GetHealthMultipler(mostImportant);
                                        if (mostImportantsMultp < targetMultp)
                                        {
                                            mostImportant = target;
                                            mostImportantsDamage = dmg;
                                        }
                                    }
                                }
                            }
                        }
                        return mostImportant;
                    }
                    return null;
                }
            }

            return null;
        }

        private static float GetPriority(AIHeroClient target)
        {
            return GetTotalMultipler(target)*
                   (GetRoleMultipler(target) + GetCustomMultipler(target) +
                    (s_fnCustomMultipler != null ? s_fnCustomMultipler(target) : 0));
        }

        private static float GetTotalMultipler(AIHeroClient target)
        {
            return GetHealthMultipler(target) + GetTotalADAPMultipler(target) +
                   (SelectedTarget == target && ConfigMenu.FocusSelected ? 10 : 0);
        }

        private static float GetTotalADAPMultipler(AIHeroClient target)
        {
            return
                HeroManager.Enemies.OrderByDescending(p => p.TotalMagicalDamage + p.TotalAttackDamage)
                    .ToList()
                    .FindIndex(q => q.NetworkId == target.NetworkId)*2;
        }

        private static float GetCustomMultipler(AIHeroClient target)
        {
            return ConfigMenu.GetChampionPriority(target)*2;
        }

        private static float GetRoleMultipler(AIHeroClient target)
        {
            return 5 - target.GetPriority();
        }

        private static float GetHealthMultipler(AIHeroClient target)
        {
            if (target.Health <= ObjectManager.Player.GetAutoAttackDamage(target)*2f)
                return 20;

            if (target.HealthPercent <= 50 && target.GetRole() != ChampionRole.Tank)
                return 10/(target.HealthPercent + 1);

            return 0;
        }

        private static void Game_OnWndProc(WndEventArgs args)
        {
            if (args.Msg != (uint) WindowsMessages.WM_LBUTTONDOWN)
            {
                return;
            }
            SelectedTarget =
                HeroManager.Enemies
                    .FindAll(hero => hero.IsValidTarget() && hero.Distance(Game.CursorPos, true) < 40000) // 200 * 200
                    .OrderBy(h => h.Distance(Game.CursorPos, true)).FirstOrDefault();
        }

        private static void Drawing_OnDraw(EventArgs args)
        {
            if (ConfigMenu.FocusSelected && ConfigMenu.SelectedTargetColor)
            {
                if (SelectedTarget != null && SelectedTarget.IsValidTarget())
                    Render.Circle.DrawCircle(SelectedTarget.Position, 150, Color.Red, 7, true);
            }
        }
    }
}