using System;
using LeagueSharp.Common;
using ItemData = LeagueSharp.Common.Data.ItemData;
using EloBuddy.SDK;

namespace NechritoRiven
{
    class Burst
    {
        private static void Game_OnUpdate(EventArgs args)
        {
            BurstLogic();
        }

        private static void FlashW()
        {
            var target = TargetSelector.SelectedTarget;
            if (target != null && target.IsValidTarget() && !target.IsZombie)
            {
                Spells._w.Cast();
                Utility.DelayAction.Add(10, () => Program.Player.Spellbook.CastSpell(Spells.Flash, target.Position));
            }
        }

        private static void CastYoumoo() { if (ItemData.Youmuus_Ghostblade.GetItem().IsReady()) ItemData.Youmuus_Ghostblade.GetItem().Cast(); }
        private static bool HasItem() => ItemData.Tiamat_Melee_Only.GetItem().IsReady() || ItemData.Ravenous_Hydra_Melee_Only.GetItem().IsReady();
        private const string IsFirstR = "RivenFengShuiEngine";

        public static void BurstLogic()
        {
            var target = TargetSelector.SelectedTarget;
            if (target != null && target.IsValidTarget() && !target.IsZombie)
            {
                if (Spells._r.IsReady() && Spells._r.Instance.Name == IsFirstR && Spells._w.IsReady() && Spells._e.IsReady() && Program.Player.Distance(target.Position) <= 250 + 70 + Program.Player.AttackRange)
                {
                    Spells._e.Cast(target.Position);
                    CastYoumoo();
                    Logic.ForceR();
                    Utility.DelayAction.Add(100, Logic.ForceW);
                }
                else if (Spells._r.IsReady() && Spells._r.Instance.Name == IsFirstR && Spells._e.IsReady() && Spells._w.IsReady() && Spells._q.IsReady() && Program.Player.Distance(target.Position) <= 400 + 70 + Program.Player.AttackRange)
                {
                    Spells._e.Cast(target.Position);
                    CastYoumoo();
                    Logic.ForceR();
                    Utility.DelayAction.Add(150, () => Logic.ForceCastQ(target));
                    Utility.DelayAction.Add(160, Logic.ForceW);
                }
                else if (Program.Player.Spellbook.GetSpell(Spells.Flash).IsReady()
                    && Spells._r.IsReady() && Spells._r.Instance.Name == IsFirstR && (Program.Player.Distance(target.Position) <= 800) && (!MenuConfig.FirstHydra || (MenuConfig.FirstHydra && !HasItem())))
                {
                    Spells._e.Cast(target.Position);
                    CastYoumoo();
                    Logic.ForceR();
                    Utility.DelayAction.Add(180, FlashW);
                }
                else if (Program.Player.Spellbook.GetSpell(Spells.Flash).IsReady() && Spells._r.IsReady() && Spells._e.IsReady() && Spells._w.IsReady() && Spells._r.Instance.Name == IsFirstR && (Program.Player.Distance(target.Position) <= 800) && MenuConfig.FirstHydra && HasItem())
                {
                    Spells._e.Cast(target.Position);
                    Logic.ForceR();
                    Utility.DelayAction.Add(100, Logic.ForceItem);
                    Utility.DelayAction.Add(210, FlashW);
                }
            }
        }
    }
}