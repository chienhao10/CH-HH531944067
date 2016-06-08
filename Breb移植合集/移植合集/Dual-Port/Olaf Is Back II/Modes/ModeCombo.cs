using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using OlafxQx.Common;
using Color = SharpDX.Color;
using EloBuddy.SDK.Menu;
using EloBuddy;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK;

namespace OlafxQx.Modes
{
    internal static class ModeCombo
    {
        public static Menu MenuLocal { get; private set; }
        private static LeagueSharp.Common.Spell Q => Champion.PlayerSpells.Q;
        private static LeagueSharp.Common.Spell W => Champion.PlayerSpells.W;
        private static LeagueSharp.Common.Spell E => Champion.PlayerSpells.E;
        private static LeagueSharp.Common.Spell R => Champion.PlayerSpells.R;

        public static SpellSlot IgniteSlot = ObjectManager.Player.GetSpellSlot("SummonerDot");
        public static void Init()
        {
            MenuLocal = ModeConfig.MenuConfig.AddSubMenu("Combo", "Combo");
            MenuLocal.Add("Combo.Q", new ComboBox("Q:", 1, "Off", "On"));
            MenuLocal.Add("Combo.W", new ComboBox("W:", 1, "Off", "On"));
            MenuLocal.Add("Combo.E", new ComboBox("E:", 1, "Off", "On"));

            Game.OnUpdate += OnUpdate;
            Orbwalker.OnPreAttack += OrbwalkingOnBeforeAttack;
        }

        private static void OrbwalkingOnBeforeAttack(AttackableUnit target, Orbwalker.PreAttackArgs args)
        {
            if (!Common.CommonHelper.ShouldCastSpell(TargetSelector.GetTarget(Orbwalking.GetRealAutoAttackRange(null) + 65, DamageType.Physical)))
            {
                return;
            }

            if (!W.IsReady() || !Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo) || MenuLocal["Combo.W"].Cast<ComboBox>().CurrentValue == 0)
            {
                return;
            }

            if (Common.CommonHelper.ShouldCastSpell((AIHeroClient) args.Target) && args.Target is AIHeroClient)
            {
                W.Cast();
            }
        }

        private static void OnUpdate(EventArgs args)
        {
            if (!Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                return;
            }

            ExecuteCombo();


        }

        private static void ExecuteCombo()
        {
            if (!Common.CommonHelper.ShouldCastSpell(TargetSelector.GetTarget(Orbwalking.GetRealAutoAttackRange(null) + 65, DamageType.Physical)))
            {
                return;
            }

            var t = TargetSelector.GetTarget(Q.Range, DamageType.Physical);
            if (!t.IsValidTarget())
            {
                return;
            }

            if (IgniteSlot != SpellSlot.Unknown &&
                ObjectManager.Player.Spellbook.CanUseSpell(IgniteSlot) == SpellState.Ready)
            {
                if (t.IsValidTarget(650) && !t.HaveImmortalBuff() && ObjectManager.Player.GetSummonerSpellDamage(t, LeagueSharp.Common.Damage.SummonerSpell.Ignite) + 150 >= t.Health)
                {
                    ObjectManager.Player.Spellbook.CastSpell(IgniteSlot, t);
                }
            }

            if (MenuLocal["Combo.Q"].Cast<ComboBox>().CurrentValue != 0)
            {
                Champion.PlayerSpells.CastQ(t, Q.Range);
            }

            if (Q.CanCast(t) && MenuLocal["Combo.Q"].Cast<ComboBox>().CurrentValue == 1 && t.Health < Q.GetDamage(t))
            {
                Champion.PlayerSpells.CastQ(t, Q.Range);
            }

            if (E.CanCast(t) && MenuLocal["Combo.E"].Cast<ComboBox>().CurrentValue == 1)
            {
                Champion.PlayerSpells.CastE(t);
            }
        }
    }
}
