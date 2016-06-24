using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu.Values;

namespace Bloodimir_Tryndamere
{
    internal static class Combo
    {
        public enum AttackSpell
        {
            E,
            W
        };

        private static AIHeroClient Tryndamere
        {
            get { return ObjectManager.Player; }
        }

        public static void TrynCombo()
        {
            var wcheck = Program.ComboMenu["usecombow"].Cast<CheckBox>().CurrentValue;
            var echeck = Program.ComboMenu["usecomboe"].Cast<CheckBox>().CurrentValue;
            var wready = Program.W.IsReady();
            var eready = Program.E.IsReady();

            if (!echeck || !eready) return;
            {
                var enemy = TargetSelector.GetTarget(Program.E.Range, DamageType.Physical);
                if (enemy != null)
                    if (Tryndamere.Distance(enemy) <= Program.E.Range - Player.Instance.GetAutoAttackRange())
                    {
                        Program.E.Cast(enemy.ServerPosition);
                    }
            }

            if (!wcheck || !wready) return;
            var wenemy = TargetSelector.GetTarget(Program.W.Range, DamageType.Magical);
            {
                if (!wenemy.IsFacing(Program.Tryndamere))
                {
                    Program.W.Cast();
                }
            }
        }

        public static
            void Items()
        {
            var ienemy = TargetSelector.GetTarget(Player.Instance.GetAutoAttackRange() + 425,
                DamageType.Physical);
            if (ienemy == null) return;
            if (!ienemy.IsValid || ienemy.IsZombie) return;
            if (Program.MiscMenu["usebotrk"].Cast<CheckBox>().CurrentValue)
            {
                if (Program.Botrk.IsOwned() && Program.Botrk.IsReady() &&
                    Program.Botrk.IsInRange(ienemy))
                    Program.Botrk.Cast(ienemy);
            }
            if (Program.MiscMenu["usebilge"].Cast<CheckBox>().CurrentValue)
            {
                if (Program.Bilgewater.IsOwned() && Program.Bilgewater.IsReady())
                    Program.Bilgewater.Cast(ienemy);
            }
            if (Program.MiscMenu["usehydra"].Cast<CheckBox>().CurrentValue)
            {
                if (Program.Hydra.IsOwned() && Program.Hydra.IsReady() &&
                    Program.Hydra.IsInRange(ienemy))
                    Program.Hydra.Cast();
            }
            if (Program.MiscMenu["usetiamat"].Cast<CheckBox>().CurrentValue)
            {
                if (Program.Tiamat.IsOwned() && Program.Tiamat.IsReady() &&
                    Program.Tiamat.IsInRange(ienemy))
                    Program.Tiamat.Cast();
            }
            if (!Program.MiscMenu["useyoumuu"].Cast<CheckBox>().CurrentValue) return;
            if (Program.Youmuu.IsOwned() && Program.Youmuu.IsReady())
                Program.Youmuu.Cast();
        }
    }
}