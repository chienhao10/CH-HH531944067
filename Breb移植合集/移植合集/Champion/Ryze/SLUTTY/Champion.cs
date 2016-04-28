using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using LeagueSharp.Common;
using Damage = LeagueSharp.Common.Damage;
using Spell = LeagueSharp.Common.Spell;

namespace Slutty_ryze
{
    internal class Champion
    {
        #region Public Properties

        public static string ChampName
        {
            get { return _champName; }
        }

        #endregion

        internal static void RyzeInterruptableSpell(AIHeroClient sender, Interrupter2.InterruptableTargetEventArgs args)
        {
            var wSpell = getCheckBoxItem(eventMenu, "useW2I");
            if (!wSpell || !sender.IsValidTarget(W.Range)) return;
            W.Cast(sender);
        }

        internal static void OnGapClose(ActiveGapcloser gapcloser)
        {
            if (gapcloser.End.Distance(Player.ServerPosition) < W.Range && getCheckBoxItem(eventMenu, "useQW2D"))
            {
                W.Cast(gapcloser.Sender);
            }
        }

        #region Variable Declaration

        private static SpellSlot _ignite;
        public static readonly AIHeroClient Player = ObjectManager.Player;
        private const string _champName = "Ryze";

        public static Menu
            _config = MenuManager._config,
            humanizerMenu = MenuManager.humanizerMenu,
            combo1Menu = MenuManager.combo1Menu,
            mixedMenu = MenuManager.mixedMenu,
            laneMenu = MenuManager.laneMenu,
            jungleMenu = MenuManager.jungleMenu,
            lastMenu = MenuManager.lastMenu,
            passiveMenu = MenuManager.passiveMenu,
            itemMenu = MenuManager.itemMenu,
            eventMenu = MenuManager.eventMenu,
            ksMenu = MenuManager.ksMenu,
            chase = MenuManager.chase;

        public static bool getCheckBoxItem(Menu m, string item)
        {
            return m[item].Cast<CheckBox>().CurrentValue;
        }

        public static int getSliderItem(Menu m, string item)
        {
            return m[item].Cast<Slider>().CurrentValue;
        }

        public static bool getKeyBindItem(Menu m, string item)
        {
            return m[item].Cast<KeyBind>().CurrentValue;
        }

        public static int getBoxItem(Menu m, string item)
        {
            return m[item].Cast<ComboBox>().CurrentValue;
        }

        //private static Spell _q, _w, _e, _r, _qn;
        // Does not work as a property o-o
        public static Spell Q, W, E, R, Qn;

        #endregion

        #region Public Functions

        public static float IgniteDamage(AIHeroClient target)
        {
            if (_ignite == SpellSlot.Unknown || Player.Spellbook.CanUseSpell(_ignite) != SpellState.Ready)
                return 0f;
            return (float) Player.GetSummonerSpellDamage(target, Damage.SummonerSpell.Ignite);
        }

        public static SpellSlot GetIgniteSlot()
        {
            return _ignite;
        }

        public static void SetIgniteSlot(SpellSlot nSpellSlot)
        {
            _ignite = nSpellSlot;
        }

        public static float GetComboDamage(Obj_AI_Base enemy)
        {
            if (Q.IsReady() || Player.Mana <= Q.Instance.SData.Mana)
                return Q.GetDamage(enemy);

            if (E.IsReady() || Player.Mana <= E.Instance.SData.Mana)
                return E.GetDamage(enemy);

            if (W.IsReady() || Player.Mana <= W.Instance.SData.Mana)
                return W.GetDamage(enemy);

            return 0;
        }

        public static void KillSteal()
        {
            var target = TargetSelector.GetTarget(Q.Range, DamageType.Magical);
            if (target == null || !target.IsValidTarget(Q.Range) || target.IsInvulnerable)
                return;

            var qSpell = getCheckBoxItem(ksMenu, "useQ2KS");
            var wSpell = getCheckBoxItem(ksMenu, "useW2KS");
            var eSpell = getCheckBoxItem(ksMenu, "useE2KS");
            if (qSpell
                && Q.GetDamage(target) > target.Health
                && target.IsValidTarget(Q.Range))
                Q.Cast(target);

            if (wSpell
                && W.GetDamage(target) > target.Health
                && target.IsValidTarget(W.Range))
                W.Cast(target);

            if (eSpell
                && E.GetDamage(target) > target.Health
                && target.IsValidTarget(E.Range))
                E.Cast(target);
        }

        public static void Orbwalking_BeforeAttack(AttackableUnit target, Orbwalker.PreAttackArgs args)
        {
            if (W.IsReady() && W.Level > 0 && Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                args.Process = false;
            }
        }

        #endregion
    }
}