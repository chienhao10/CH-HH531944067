using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Events;

namespace BallistaKogMaw
{
    internal class ModeManager
    {
        // Clone Character Object
        public static AIHeroClient Champion = Program.Champion;
        public static Obj_AI_Base Ptarget = Program.Ptarget;

        public static void ComboMode()
        {
            if (MenuManager.ComboUseQ)
            {
                var target = TargetManager.GetChampionTarget(SpellManager.Q.Range, DamageType.Magical, false, true);
                if (target != null)
                    SpellManager.CastQ(target);
            }
            if (MenuManager.ComboUseW)
            {
                var target = TargetManager.GetChampionTarget(SpellManager.W.Range, DamageType.Mixed);
                if (target != null)
                    SpellManager.CastW(target);
            }
            if (MenuManager.ComboUseE)
            {
                var target = TargetManager.GetChampionTarget(SpellManager.E.Range, DamageType.Magical);
                if (target != null)
                    SpellManager.CastE(target);
            }
            if (MenuManager.ComboUseR
                && Champion.GetBuffCount("kogmawlivingartillerycost") <= MenuManager.ComboStacks)
            {
                var target = TargetManager.GetChampionTarget(SpellManager.R.Range, DamageType.Mixed);
                if (target != null)
                    SpellManager.CastR(target);
            }
        }

        public static void HarassMode()
        {
            if (Champion.ManaPercent < MenuManager.HarassMana) return;
            if (MenuManager.HarassUseQ)
            {
                var target = TargetManager.GetChampionTarget(SpellManager.Q.Range, DamageType.Magical, false, true);
                if (target != null)
                    SpellManager.CastQ(target);
            }
            if (MenuManager.HarassUseW)
            {
                var target = TargetManager.GetChampionTarget(SpellManager.W.Range, DamageType.Mixed);
                if (target != null)
                    SpellManager.CastW(target);
            }
            if (MenuManager.HarassUseE)
            {
                var target = TargetManager.GetChampionTarget(SpellManager.E.Range, DamageType.Magical);
                if (target != null)
                    SpellManager.CastE(target);
            }
            if (MenuManager.HarassUseR
                && Champion.GetBuffCount("kogmawlivingartillerycost") <= MenuManager.HarassStacks)
            {
                var target = TargetManager.GetChampionTarget(SpellManager.R.Range, DamageType.Mixed);
                if (target != null)
                    SpellManager.CastR(target);
            }
        }

        public static void JungleMode()
        {
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
            if (MenuManager.JungleUseR
                && Champion.GetBuffCount("kogmawlivingartillerycost") <= MenuManager.JungleStacks)
            {
                var target = TargetManager.GetMinionTarget(SpellManager.R.Range, DamageType.Mixed, false, true);
                if (target != null)
                    SpellManager.CastR(target);
            }
        }

        public static void LaneClearMode()
        {
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
            if (MenuManager.LaneClearUseR
                && Champion.GetBuffCount("kogmawlivingartillerycost") <= MenuManager.LaneClearStacks)
            {
                var target = TargetManager.GetMinionTarget(SpellManager.R.Range, DamageType.Mixed);
                if (target != null)
                    SpellManager.CastR(target);
            }
        }

        public static void LastHitMode()
        {
            if (Champion.ManaPercent < MenuManager.LastHitMana) return;
            if (Orbwalker.CanAutoAttack && Orbwalker.IsAutoAttacking) return;
            if (MenuManager.LastHitUseQ)
            {
                var target = TargetManager.GetMinionTarget(SpellManager.Q.Range, DamageType.Magical, false, false, true, SpellManager.QDamage());
                if (target != null)
                    SpellManager.CastQ(target);
            }
            if (MenuManager.LastHitUseR
                && Champion.GetBuffCount("kogmawlivingartillerycost") <= MenuManager.LastHitStacks)
            {
                var target = TargetManager.GetMinionTarget(SpellManager.R.Range, DamageType.Mixed, false, false, false, SpellManager.RDamage());
                if (target != null)
                    SpellManager.CastR(target);
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
            if (MenuManager.KsUseR)
            {
                var target = TargetManager.GetChampionTarget(SpellManager.R.Range, DamageType.Mixed, false, false, SpellManager.RDamage());
                if (target != null)
                    SpellManager.CastR(target);
            }
        }

        public static void DeathFollowMode()
        {
            if (Ptarget != null)
            {
                Player.IssueOrder(GameObjectOrder.MoveTo, Ptarget.ServerPosition - 100);
            }
            else
            {
                var kstarget = TargetManager.GetChampionTarget(SpellManager.Q.Range, DamageType.True, false, false, SpellManager.PDamage());

                if (kstarget != null)
                    Ptarget = kstarget;
                else
                {
                    var target = TargetManager.GetChampionTarget(SpellManager.Q.Range, DamageType.True);
                    if (target != null)
                        Ptarget = target;
                    else
                    {
                        var ksminion = TargetManager.GetMinionTarget(SpellManager.Q.Range, DamageType.True, false, false, false, SpellManager.PDamage());
                        if (ksminion != null)
                            Ptarget = ksminion;
                        else
                        {
                            var minion = TargetManager.GetMinionTarget(SpellManager.Q.Range, DamageType.True);
                            if (minion != null)
                                Ptarget = minion;
                        }
                    }
                }
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

        public static void GapCloserMode(Obj_AI_Base sender, Gapcloser.GapcloserEventArgs args)
        {
            if (!MenuManager.GapCloserMode) return;
            if (sender != null && MenuManager.GapCloserUseE)
            {
                var target = TargetManager.GetChampionTarget(SpellManager.E.Range, DamageType.Magical);
                if (target != null)
                    SpellManager.CastE(target);
            }
        }
    }
}