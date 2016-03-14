using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Events;

namespace MagicianRyze
{
    internal class ModeManager
    {
        // Clone Character Object
        public static AIHeroClient Champion = Program.Champion;

        public static void CounterCombo()
        {
            if (Champion.HasBuff("RyzeR")
                && MenuManager.UltimateMode)
            {
                UltimateMode(GameObjectType.AIHeroClient);
                return;
            }
            if (MenuManager.ComboUseW)
            {
                var target = TargetManager.GetChampionTarget(SpellManager.W.Range, DamageType.Magical);
                if (target != null)
                    SpellManager.CastW(target);
            }
            if (MenuManager.ComboUseQ)
            {
                var target = TargetManager.GetChampionTarget(SpellManager.Q.Range, DamageType.Magical, false, true);
                if (target != null)
                    SpellManager.CastQ(target);
            }
            if (MenuManager.ComboUseE)
            {
                var target = TargetManager.GetChampionTarget(SpellManager.E.Range, DamageType.Magical);
                if (target != null)
                    SpellManager.CastE(target);
            }
            if (MenuManager.ComboUseR)
            {
                if (Champion.HasBuff("ryzepassivecharged")
                    || Champion.GetBuffCount("ryzepassivestack") >= MenuManager.ComboStacks)
                {
                    var target = TargetManager.GetChampionTarget(SpellManager.Q.Range, DamageType.Magical);
                    if (target != null)
                        SpellManager.CastR(target);
                }
            }
        }

        public static void SluttyCombo()
        {
            if (Champion.HasBuff("RyzeR")
                && MenuManager.UltimateMode)
            {
                UltimateMode(GameObjectType.AIHeroClient);
                return;
            }

            var qtarget = TargetManager.GetChampionTarget(SpellManager.Q.Range, DamageType.Magical, false, true);
            var target = TargetManager.GetChampionTarget(SpellManager.W.Range, DamageType.Magical);
            if (target != null && qtarget != null)
            {
                var bcount = Champion.GetBuffCount("ryzepassivestack");
                if (!Champion.HasBuff("ryzepassivecharged") && bcount > 0)
                {
                    switch (bcount)
                    {
                        case 1:
                            if (MenuManager.ComboUseQ)
                                SpellManager.CastQ(qtarget);
                            if (MenuManager.ComboUseE)
                                SpellManager.CastE(target);
                            if (MenuManager.ComboUseW)
                                SpellManager.CastW(target);
                            if (MenuManager.ComboUseR)
                                SpellManager.CastR(target);
                            break;
                        case 2:
                            if (MenuManager.ComboUseQ)
                                SpellManager.CastQ(qtarget);
                            if (MenuManager.ComboUseW)
                                SpellManager.CastW(target);
                            if (MenuManager.ComboUseE)
                                SpellManager.CastE(target);
                            if (MenuManager.ComboUseR)
                                SpellManager.CastR(target);
                            break;
                        case 3:
                            if (MenuManager.ComboUseQ)
                                SpellManager.CastQ(qtarget);
                            if (MenuManager.ComboUseE)
                                SpellManager.CastE(target);
                            if (MenuManager.ComboUseW)
                                SpellManager.CastW(target);
                            if (MenuManager.ComboUseR)
                                SpellManager.CastR(target);
                            break;
                        case 4:
                            if (MenuManager.ComboUseW)
                                SpellManager.CastW(target);
                            if (MenuManager.ComboUseQ)
                                SpellManager.CastQ(qtarget);
                            if (MenuManager.ComboUseE)
                                SpellManager.CastE(target);
                            if (MenuManager.ComboUseR)
                                SpellManager.CastR(target);
                            break;
                    }
                }
                else
                {
                    if (MenuManager.ComboUseW)
                        SpellManager.CastW(target);
                    if (MenuManager.ComboUseQ)
                        SpellManager.CastQ(qtarget);
                    if (MenuManager.ComboUseE)
                        SpellManager.CastE(target);
                    if (MenuManager.ComboUseR)
                        SpellManager.CastR(target);
                }

                if (MenuManager.ComboUseR
                    && (Champion.GetBuffCount("ryzepassivestack") == 4 || Champion.HasBuff("ryzepassivecharged"))
                    && !SpellManager.Q.IsReady() && !SpellManager.W.IsReady() && !SpellManager.E.IsReady())
                    SpellManager.CastR(target);
            }
        }

        public static void HarassMode()
        {
            if (Champion.HasBuff("RyzeR")
                && MenuManager.UltimateMode)
            {
                UltimateMode(GameObjectType.AIHeroClient);
                return;
            }
            if (Champion.ManaPercent < MenuManager.HarassMana) return;
            if (MenuManager.HarassUseQ)
            {
                var target = TargetManager.GetChampionTarget(SpellManager.Q.Range, DamageType.Magical, false, true);
                if (target != null)
                    SpellManager.CastQ(target);
            }
        }

        public static void JungleMode()
        {
            if (Champion.HasBuff("RyzeR")
                && MenuManager.UltimateMode)
            {
                UltimateMode(GameObjectType.obj_AI_Minion, true);
                return;
            }
            if (Champion.ManaPercent < MenuManager.JungleMana) return;
            if (MenuManager.JungleUseQ)
            {
                var target = TargetManager.GetMinionTarget(SpellManager.Q.Range, DamageType.Magical, false, true, true);
                if (target != null)
                    SpellManager.CastQ(target);
            }
            if (MenuManager.JungleUseW)
            {
                var target = TargetManager.GetMinionTarget(SpellManager.W.Range, DamageType.Magical, false, true);
                if (target != null)
                    SpellManager.CastW(target);
            }
            if (MenuManager.JungleUseE)
            {
                var target = TargetManager.GetMinionTarget(SpellManager.E.Range, DamageType.Magical, false, true);
                if (target != null)
                    SpellManager.CastE(target);
            }
        }

        public static void LaneClearMode()
        {
            if (Champion.HasBuff("RyzeR")
                && MenuManager.UltimateMode)
            {
                UltimateMode(GameObjectType.obj_AI_Minion);
                return;
            }
            if (Champion.ManaPercent < MenuManager.LaneClearMana) return;
            if (MenuManager.LaneClearUseQ)
            {
                var target = TargetManager.GetMinionTarget(SpellManager.Q.Range, DamageType.Magical, false, false, true);
                if (target != null)
                    SpellManager.CastQ(target);
            }
            if (MenuManager.LaneClearUseW)
            {
                var target = TargetManager.GetMinionTarget(SpellManager.W.Range, DamageType.Magical);
                if (target != null)
                    SpellManager.CastW(target);
            }
            if (MenuManager.LaneClearUseE)
            {
                var target = TargetManager.GetMinionTarget(SpellManager.E.Range, DamageType.Magical);
                if (target != null)
                    SpellManager.CastE(target);
            }
        }

        public static void LastHitMode()
        {
            if (Champion.HasBuff("RyzeR")
                && MenuManager.UltimateMode)
            {
                UltimateMode(GameObjectType.obj_AI_Minion);
                return;
            }
            if (Champion.ManaPercent < MenuManager.LastHitMana) return;
            if (Orbwalker.CanAutoAttack) return;
            if (MenuManager.LastHitUseQ)
            {
                var target = TargetManager.GetMinionTarget(SpellManager.Q.Range, DamageType.Magical, false, false, true, SpellManager.QDamage());
                if (target != null)
                    SpellManager.CastQ(target);
            }
            if (MenuManager.LastHitUseW)
            {
                var target = TargetManager.GetMinionTarget(SpellManager.W.Range, DamageType.Magical, false, false, false, SpellManager.WDamage());
                if (target != null)
                    SpellManager.CastW(target);
            }
            if (MenuManager.LastHitUseE)
            {
                var target = TargetManager.GetMinionTarget(SpellManager.E.Range, DamageType.Magical, false, false, false, SpellManager.EDamage());
                if (target != null)
                    SpellManager.CastE(target);
            }
        }

        public static void UltimateMode(GameObjectType type, bool isMonster = false)
        {
            switch (type)
            {
                case GameObjectType.AIHeroClient:
                {
                    if (SpellManager.Q.IsReady())
                    {
                        var target = TargetManager.GetChampionTarget(SpellManager.Q.Range, DamageType.Magical, false, true);
                        if (target != null)
                            SpellManager.CastQ(target);
                    }
                    if (SpellManager.W.IsReady())
                    {
                        var target = TargetManager.GetChampionTarget(SpellManager.W.Range, DamageType.Magical);
                        if (target != null)
                            SpellManager.CastW(target);
                    }
                    if (SpellManager.E.IsReady())
                    {
                        var target = TargetManager.GetChampionTarget(SpellManager.E.Range, DamageType.Magical);
                        if (target != null)
                            SpellManager.CastE(target);
                    }
                    break;
                }
                case GameObjectType.obj_AI_Minion:
                {
                    if (isMonster)
                    {
                        if (SpellManager.Q.IsReady())
                        {
                            var target = TargetManager.GetMinionTarget(SpellManager.Q.Range, DamageType.Magical, false, true, true);
                            if (target != null)
                                SpellManager.CastQ(target);
                        }
                        if (SpellManager.W.IsReady())
                        {
                            var target = TargetManager.GetMinionTarget(SpellManager.W.Range, DamageType.Magical, false, true);
                            if (target != null)
                                SpellManager.CastW(target);
                        }
                        if (SpellManager.E.IsReady())
                        {
                            var target = TargetManager.GetMinionTarget(SpellManager.E.Range, DamageType.Magical, false, true);
                            if (target != null)
                                SpellManager.CastE(target);
                        }
                    }
                    else
                    {
                        if (SpellManager.Q.IsReady())
                        {
                            var target = TargetManager.GetMinionTarget(SpellManager.Q.Range, DamageType.Magical, false, false, true);
                            if (target != null)
                                SpellManager.CastQ(target);
                        }
                        if (SpellManager.W.IsReady())
                        {
                            var target = TargetManager.GetMinionTarget(SpellManager.W.Range, DamageType.Magical);
                            if (target != null)
                                SpellManager.CastW(target);
                        }
                        if (SpellManager.E.IsReady())
                        {
                            var target = TargetManager.GetMinionTarget(SpellManager.E.Range, DamageType.Magical);
                            if (target != null)
                                SpellManager.CastE(target);
                        }
                    }
                    break;
                }
            }
        }

        public static void KsMode()
        {
            if (MenuManager.KsUseQ)
            {
                var target = TargetManager.GetChampionTarget(SpellManager.Q.Range, DamageType.Magical, false, true, SpellManager.QDamage());
                if (target != null)
                    SpellManager.CastQ(target);
            }
            if (MenuManager.KsUseW)
            {
                var target = TargetManager.GetChampionTarget(SpellManager.W.Range, DamageType.Magical, false, false, SpellManager.WDamage());
                if (target != null)
                    SpellManager.CastW(target);
            }
            if (MenuManager.KsUseE)
            {
                var target = TargetManager.GetChampionTarget(SpellManager.E.Range, DamageType.Magical, false, false, SpellManager.EDamage());
                if (target != null)
                    SpellManager.CastE(target);
            }
        }

        public static void StackMode()
        {
            foreach (var item in Champion.InventoryItems)
            {
                if ((item.Id == ItemId.Tear_of_the_Goddess || item.Id == ItemId.Tear_of_the_Goddess_Crystal_Scar ||
                     item.Id == ItemId.Archangels_Staff || item.Id == ItemId.Archangels_Staff_Crystal_Scar ||
                     item.Id == ItemId.Manamune || item.Id == ItemId.Manamune_Crystal_Scar)
                    && Champion.IsInShopRange())
                {
                    if ((int)(Game.Time - SpellManager.StackerStamp) >= 2)
                    {
                        SpellManager.CastQ(Champion);
                        SpellManager.StackerStamp = Game.Time;
                    }
                }
            }
        }

        public static void InterruptMode(Obj_AI_Base sender, Interrupter.InterruptableSpellEventArgs args)
        {
            if (!MenuManager.InterrupterMode) return;
            if (sender != null && MenuManager.InterrupterUseW)
            {
                var target = TargetManager.GetChampionTarget(SpellManager.W.Range, DamageType.Magical);
                if (target != null)
                    SpellManager.CastW(target);
            }
        }

        public static void GapCloserMode(Obj_AI_Base sender, Gapcloser.GapcloserEventArgs args)
        {
            if (!MenuManager.GapCloserMode) return;
            if (sender != null && MenuManager.GapCloserUseW)
            {
                var target = TargetManager.GetChampionTarget(SpellManager.W.Range, DamageType.Magical);
                if (target != null)
                    SpellManager.CastW(target);
            }
        }
    }
}
